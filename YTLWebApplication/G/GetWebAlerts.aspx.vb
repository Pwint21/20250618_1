Imports Newtonsoft.Json
Imports System.Data
Imports System.Data.SqlClient

Partial Class GetWebAlerts
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.StatusCode = 401
                Response.Write("{""error"":""Unauthorized""}")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Get validated user information
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)

            ' SECURITY FIX: Rate limiting
            If SecurityHelper.IsRateLimited(Request.UserHostAddress, 60, 1) Then
                Response.StatusCode = 429
                Response.Write("{""error"":""Rate limit exceeded""}")
                Response.End()
                Return
            End If

            Dim aa As New ArrayList
            Dim condition As String = ""
            Dim count As Integer = 0

            ' SECURITY FIX: Build secure condition based on role
            If role = "User" Then
                condition = " AND userid = @userid"
            ElseIf role = "SuperUser" Or role = "Operator" Then
                If Not String.IsNullOrEmpty(userslist) AndAlso SecurityHelper.IsValidUsersList(userslist) Then
                    ' Create parameterized query for multiple user IDs
                    Dim userIds() As String = userslist.Split(","c)
                    Dim parameters As New List(Of String)
                    For i As Integer = 0 To userIds.Length - 1
                        parameters.Add($"@userid{i}")
                    Next
                    condition = $" AND userid IN ({String.Join(",", parameters)})"
                Else
                    condition = " AND userid = @userid"
                End If
            End If

            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
                ' SECURITY FIX: Use parameterized query
                Dim query As String = "SELECT u.username, a.id, a.userid, a.plateno, a.timestamp, a.lat, a.lon, a.speed, a.bearing, a.odometer, a.ignition, a.alert_type, a.extra_info, a.resolved, a.resolved_datetime, a.event_reason, a.remarks, a.remarks_userid " &
                                    "FROM (SELECT * FROM alert_notification WHERE timestamp BETWEEN @startTime AND @endTime" & condition & ") a " &
                                    "LEFT OUTER JOIN userTBL u ON a.remarks_userid = u.userid " &
                                    "ORDER BY a.timestamp"

                Using cmd As New SqlCommand(query, conn)
                    ' Add time parameters
                    cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@startTime", DateTime.Now.AddHours(-24).ToString("yyyy/MM/dd HH:mm:ss"), SqlDbType.DateTime))
                    cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@endTime", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), SqlDbType.DateTime))

                    ' Add user parameters based on role
                    If role = "User" OrElse (role <> "User" AndAlso String.IsNullOrEmpty(userslist)) Then
                        cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@userid", userid, SqlDbType.Int))
                    ElseIf role = "SuperUser" Or role = "Operator" Then
                        If Not String.IsNullOrEmpty(userslist) AndAlso SecurityHelper.IsValidUsersList(userslist) Then
                            Dim userIds() As String = userslist.Split(","c)
                            For i As Integer = 0 To userIds.Length - 1
                                cmd.Parameters.Add(SecurityHelper.CreateSqlParameter($"@userid{i}", userIds(i).Trim(), SqlDbType.Int))
                            Next
                        End If
                    End If

                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            count += 1
                            Dim a As New ArrayList()
                            
                            ' SECURITY FIX: Validate and sanitize data
                            a.Add(SecurityHelper.HtmlEncode(dr("id").ToString()))
                            a.Add(count.ToString())
                            a.Add(SecurityHelper.HtmlEncode(dr("plateno").ToString()))
                            
                            ' SECURITY FIX: Validate timestamp
                            Dim timestamp As String = dr("timestamp").ToString()
                            If SecurityHelper.ValidateDate(timestamp) Then
                                a.Add(DateTime.Parse(timestamp).ToString("yyyy/MM/dd HH:mm:ss"))
                            Else
                                a.Add("Invalid Date")
                            End If

                            ' SECURITY FIX: Validate alert type
                            Dim alertType As String = dr("alert_type").ToString()
                            If SecurityHelper.ValidateNumeric(alertType, 0, 11) Then
                                Select Case alertType
                                    Case "0" : a.Add("PTO ON")
                                    Case "1" : a.Add("IMMOBILIZER")
                                    Case "2" : a.Add("OVERSPEED")
                                    Case "3" : a.Add("PANIC")
                                    Case "4" : a.Add("POWERCUT")
                                    Case "5" : a.Add("UNLOCK")
                                    Case "6" : a.Add("IDLING")
                                    Case "7" : a.Add("IGNITION OFF")
                                    Case "8" : a.Add("IGNITION ON")
                                    Case "9" : a.Add("OVERTIME")
                                    Case "10" : a.Add("Geofence In")
                                    Case "11" : a.Add("Geofence Out")
                                    Case Else : a.Add("Unknown")
                                End Select
                            Else
                                a.Add("Invalid Alert Type")
                            End If

                            ' SECURITY FIX: Handle resolved status
                            Dim resolved As String = ""
                            If Convert.ToBoolean(dr("resolved")) Then
                                Dim eventReason As String = dr("event_reason").ToString()
                                If SecurityHelper.ValidateNumeric(eventReason, 1, 7) Then
                                    Select Case eventReason
                                        Case "1" : a.Add("Accident")
                                        Case "2" : a.Add("Battery Taken Out")
                                        Case "3" : a.Add("In Workshop")
                                        Case "4" : a.Add("Not In Operation")
                                        Case "5" : a.Add("Signal Lost")
                                        Case "6" : a.Add("Other")
                                        Case "7" : a.Add("Geofence")
                                        Case Else : a.Add("Unknown Reason")
                                    End Select
                                Else
                                    a.Add("Invalid Reason")
                                End If
                                a.Add(SecurityHelper.HtmlEncode(dr("remarks").ToString()))
                                resolved = "Yes"
                            Else
                                a.Add("--")
                                a.Add("--")
                                resolved = "No"
                            End If

                            ' SECURITY FIX: Validate speed/extra info
                            If alertType = "2" Then
                                Dim speed As String = dr("speed").ToString()
                                If SecurityHelper.ValidateNumeric(speed, 0, 300) Then
                                    a.Add(speed)
                                Else
                                    a.Add("Invalid Speed")
                                End If
                            Else
                                a.Add(SecurityHelper.HtmlEncode(dr("extra_info").ToString()))
                            End If

                            ' SECURITY FIX: Handle username
                            If IsDBNull(dr("username")) Then
                                a.Add("--")
                            Else
                                a.Add(SecurityHelper.HtmlEncode(dr("username").ToString()))
                            End If

                            ' SECURITY FIX: Handle resolved datetime
                            If IsDBNull(dr("resolved_datetime")) Then
                                a.Add("--")
                            Else
                                Dim resolvedDateTime As String = dr("resolved_datetime").ToString()
                                If SecurityHelper.ValidateDate(resolvedDateTime) Then
                                    a.Add(DateTime.Parse(resolvedDateTime).ToString("yyyy/MM/dd HH:mm:ss"))
                                Else
                                    a.Add("Invalid Date")
                                End If
                            End If
                            
                            a.Add(resolved)
                            aa.Add(a)
                        End While
                    End Using
                End Using
            End Using

            ' SECURITY FIX: Set proper content type and return JSON
            Response.ContentType = "application/json"
            Response.Write("{""aaData"":" & JsonConvert.SerializeObject(aa, Formatting.None) & "}")

        Catch ex As Exception
            ' SECURITY FIX: Log error securely and return generic error
            SecurityHelper.LogError("GetWebAlerts error", ex, Server)
            Response.StatusCode = 500
            Response.ContentType = "application/json"
            Response.Write("{""error"":""Internal server error""}")
        End Try
    End Sub

End Class