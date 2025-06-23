Imports Newtonsoft.Json
Imports System.Data
Imports System.Data.SqlClient
Partial Class GetWebAlerts
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim aa As New ArrayList
        Dim a As ArrayList
        Dim userid As String = Request.Cookies("userinfo")("userid")
        Dim role As String = Request.Cookies("userinfo")("role")
        Dim userslist As String = Request.Cookies("userinfo")("userslist")
        Dim condition As String = ""
        Dim count As Integer = 0
        If role = "User" Then
            condition = " and userid='" & userid & "'"
        ElseIf role = "SuperUser" Or role = "Operator" Then
            condition = " and userid in(" & userslist & ")"
        End If
        Dim cmd As New SqlCommand("select u.username,a.id,a.userid,a.plateno,a.timestamp,a.lat,a.lon,a.speed,a.bearing,a.odometer,a.ignition,a.alert_type,a.extra_info,a.resolved,a.resolved_datetime,a.event_reason,a.remarks,a.remarks_userid from (select * from alert_notification where timestamp between'" & Now.AddHours(-24).ToString("yyyy/MM/dd HH:mm:ss") & "' and '" & Now.ToString("yyyy/MM/dd HH:mm:ss") & "' " & condition & ") a left outer Join userTBL u  on a.remarks_userid = u.userid   order by a.timestamp", conn)

        '  Response.Write(cmd.CommandText)
        Try
            conn.Open()

            Dim dr As SqlDataReader = cmd.ExecuteReader()
            While dr.Read()
                count += 1
                a = New ArrayList()
                a.Add(dr("id"))
                a.Add(count.ToString())
                a.Add(dr("plateno"))
                a.Add(DateTime.Parse(dr("timestamp")).ToString("yyyy/MM/dd HH:mm:ss"))

                Select Case dr("alert_type").ToString()
                    Case "0"
                        a.Add("PTO ON")
                    Case "1"
                        a.Add("IMMOBILIZER")
                    Case "2"
                        a.Add("OVERSPEED")
                    Case "3"
                        a.Add("PANIC")
                    Case "4"
                        a.Add("POWERCUT")
                    Case "5"
                        a.Add("UNLOCK")
                    Case "6"
                        a.Add("IDLING")
                    Case "7"
                        a.Add("IGNITION OFF")
                    Case "8"
                        a.Add("IGNITION ON")
                    Case "9"
                        a.Add("OVERTIME")
                    Case "10"
                        a.Add("Geofence In")
                    Case "11"
                        a.Add("Geofence out")
                    Case Else

                End Select

                Dim resolved As String = ""
                If dr("resolved") Then
                    Dim remark As String = ""
                    Select Case dr("event_reason")
                        Case "1"
                            remark = "Accident"
                        Case "2"
                            remark = "Battery Taken Out"
                        Case "3"
                            remark = "In Workshop"
                        Case "4"
                            remark = "Not In Operation"
                        Case "5"
                            remark = "Signal Lost"
                        Case "6"
                            remark = "Other"
                        Case "7"
                            remark = "Geofence"
                        Case Else

                    End Select
                    a.Add(remark)
                    a.Add(dr("remarks"))
                    resolved = "Yes"
                Else
                    a.Add("--")
                    a.Add("--")
                    resolved = "No"
                End If

                If dr("alert_type").ToString() = "2" Then
                    a.Add(dr("speed").ToString())
                Else
                    a.Add(dr("extra_info").ToString())
                End If

                If IsDBNull(dr("username")) Then
                    a.Add("--")
                Else
                    a.Add(dr("username"))
                End If

                If IsDBNull(dr("resolved_datetime")) Then
                    a.Add("--")
                Else
                    a.Add(DateTime.Parse(dr("resolved_datetime")).ToString("yyyy/MM/dd HH:mm:ss"))
                End If
                a.Add(resolved)
                aa.Add(a)
            End While

        Catch ex As Exception
            a = New ArrayList()
            a.Add(1)
            a.Add(ex.Message.ToString())
            a.Add("")
            a.Add("")
            a.Add("")
            a.Add("")
            a.Add("")
            a.Add("")
            aa.Add(a)
        Finally
            conn.Close()
        End Try

        Dim jss As New Newtonsoft.Json.JsonSerializer()
        Response.Write("{""aaData"":" & JsonConvert.SerializeObject(aa, Formatting.None) & "}")
        Response.ContentType = "text/plain"
    End Sub
End Class
