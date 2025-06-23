Imports System.Data.SqlClient

Partial Class GMap
    Inherits System.Web.UI.Page
    Public sb As StringBuilder
    Public plateno As String
    Public begindatetime As String
    Public enddatetime As String
    Public searchin As String
    Public puserid As String
    Public mapsettings As String = ""
    Public role As String
    Public jspage As String
    Public querystring As String = ""
    Public acode As String = "0"
    Public polygonid As String = ""
    Public scode As String = ""
    Public sf As String = ""
    Public markerlat As String = ""
    Public markerlon As String = ""
    Public reqfrom As String = ""
    Public mvals As String
    Public la As String = "N"
    Public ucheck As String = "False"

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("~/Login.aspx")
                Return
            End If

            ' SECURITY FIX: Get validated user information
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            
            ' SECURITY FIX: Check special user permissions securely
            Dim specialUserIds As String() = {"7030", "7031", "7032", "7033", "1933", "6779", "6835", "1618", "1911", "6826"}
            If specialUserIds.Contains(userid) Then
                ucheck = "True"
            End If

        Catch ex As Exception
            SecurityHelper.LogError("GMap OnInit error", ex, Server)
            Response.Redirect("~/Login.aspx")
        End Try

        MyBase.OnInit(e)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("~/Login.aspx")
                Return
            End If

            ' SECURITY FIX: Validate query string parameters
            reqfrom = SecurityHelper.HtmlEncode(Request.QueryString("from"))
            
            ' SECURITY FIX: Get validated user information
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            Dim userRole As String = SecurityHelper.ValidateAndGetUserRole(Request)
            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)

            sb = New StringBuilder()
            puserid = userid

            ' SECURITY FIX: Validate and sanitize query string parameters
            plateno = SecurityHelper.HtmlEncode(Request.QueryString("plateno"))
            If Not String.IsNullOrEmpty(plateno) AndAlso Not SecurityHelper.ValidatePlateNumber(plateno) Then
                plateno = ""
            End If

            begindatetime = SecurityHelper.HtmlEncode(Request.QueryString("bdt"))
            If Not String.IsNullOrEmpty(begindatetime) AndAlso Not SecurityHelper.ValidateDate(begindatetime) Then
                begindatetime = ""
            End If

            enddatetime = SecurityHelper.HtmlEncode(Request.QueryString("edt"))
            If Not String.IsNullOrEmpty(enddatetime) AndAlso Not SecurityHelper.ValidateDate(enddatetime) Then
                enddatetime = ""
            End If

            searchin = SecurityHelper.HtmlEncode(Request.QueryString("si"))
            scode = SecurityHelper.HtmlEncode(Request.QueryString("scode"))
            sf = SecurityHelper.HtmlEncode(Request.QueryString("sf"))
            
            ' SECURITY FIX: Validate coordinates
            Dim markerLatStr As String = Request.QueryString("markerlat")
            Dim markerLonStr As String = Request.QueryString("markerlon")
            If SecurityHelper.ValidateCoordinate(markerLatStr, markerLonStr) Then
                markerlat = markerLatStr
                markerlon = markerLonStr
            Else
                markerlat = ""
                markerlon = ""
            End If

            querystring = SecurityHelper.HtmlEncode(Request.QueryString("qs"))
            acode = SecurityHelper.HtmlEncode(Request.QueryString("acode"))
            polygonid = SecurityHelper.HtmlEncode(Request.QueryString("id"))

            Session("MapRefresh") = "Y"
            begindate1.Value = DateTime.Now.ToString("yyyy/MM/dd")
            enddate1.Value = DateTime.Now.ToString("yyyy/MM/dd")

            ' SECURITY FIX: Validate sf parameter
            If sf = "1" Then
                sf = "1"
                If Not String.IsNullOrEmpty(enddatetime) AndAlso SecurityHelper.ValidateDate(enddatetime) Then
                    begindatetime = Convert.ToDateTime(enddatetime).AddDays(-1).ToString("yyyy/MM/dd HH:mm:ss")
                End If
            Else
                sf = ""
            End If

            ' SECURITY FIX: Check special user permissions
            Dim specialUserIds As String() = {"7030", "7031", "7032", "7033", "1933", "6779", "6835", "1618", "1911", "6826"}
            If specialUserIds.Contains(userid) Then
                ucheck = "True"
            End If

            role = userRole
            jspage = userid
            la = Request.Cookies("userinfo")("LA")

            ' SECURITY FIX: Load map settings securely
            LoadMapSettings(userid)

            ' SECURITY FIX: Load vehicle data securely
            LoadVehicleData(userid, userRole, userslist)

        Catch ex As Exception
            SecurityHelper.LogError("GMap Page_Load error", ex, Server)
            Response.Redirect("~/Error.aspx")
        End Try
    End Sub

    ' SECURITY FIX: Secure map settings loading
    Private Sub LoadMapSettings(userid As String)
        Try
            If Session("mapsettings") Is Nothing OrElse Session("mapsettings") = 0 Then
                Using conn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
                    Dim query As String = "SELECT zoomlevel, lat, lon FROM map_settings WHERE userid = @userid"
                    Using cmd As New SqlCommand(query, conn)
                        cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@userid", userid, SqlDbType.Int))
                        
                        conn.Open()
                        Using dr As SqlDataReader = cmd.ExecuteReader()
                            If dr.Read() Then
                                mapsettings = dr("zoomlevel").ToString() & "," & dr("lat").ToString() & "," & dr("lon").ToString()
                            End If
                        End Using
                    End Using
                End Using
            Else
                ' SECURITY FIX: Validate session map settings
                Dim sessionSetting As String = Session("mapsettings").ToString()
                Select Case sessionSetting
                    Case "1" : mapsettings = "12,5.808334,102.146759"
                    Case "2" : mapsettings = "12,3.117371,101.6833"
                    Case "3" : mapsettings = "9,3.504639,102.639771"
                    Case "4" : mapsettings = "9,3.117371,101.6833"
                    Case "5" : mapsettings = "12,5.324249,103.141022"
                    Case Else : LoadMapSettings(userid) ' Fallback to database
                End Select
            End If
        Catch ex As Exception
            SecurityHelper.LogError("LoadMapSettings error", ex, Server)
            mapsettings = "12,3.117371,101.6833" ' Default fallback
        End Try
    End Sub

    ' SECURITY FIX: Secure vehicle data loading
    Private Sub LoadVehicleData(userid As String, role As String, userslist As String)
        Try
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
                Dim ds As New DataSet
                Dim da As SqlDataAdapter

                ' SECURITY FIX: Use parameterized queries based on role
                If role = "User" Then
                    da = New SqlDataAdapter("SELECT userid, username FROM userTBL WHERE userid = @userid ORDER BY username", conn)
                    da.SelectCommand.Parameters.Add(SecurityHelper.CreateSqlParameter("@userid", userid, SqlDbType.Int))
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    If Not String.IsNullOrEmpty(userslist) AndAlso SecurityHelper.IsValidUsersList(userslist) Then
                        ' Create parameterized query for multiple user IDs
                        Dim userIds() As String = userslist.Split(","c)
                        Dim parameters As New List(Of String)
                        Dim cmd As New SqlCommand()
                        
                        For i As Integer = 0 To userIds.Length - 1
                            Dim paramName As String = "@userid" & i
                            parameters.Add(paramName)
                            cmd.Parameters.Add(SecurityHelper.CreateSqlParameter(paramName, userIds(i).Trim(), SqlDbType.Int))
                        Next
                        
                        Dim inClause As String = String.Join(",", parameters)
                        cmd.CommandText = $"SELECT userid, username FROM userTBL WHERE userid IN ({inClause}) ORDER BY username"
                        cmd.Connection = conn
                        da = New SqlDataAdapter(cmd)
                    Else
                        ' Fallback to single user
                        da = New SqlDataAdapter("SELECT userid, username FROM userTBL WHERE userid = @userid ORDER BY username", conn)
                        da.SelectCommand.Parameters.Add(SecurityHelper.CreateSqlParameter("@userid", userid, SqlDbType.Int))
                    End If
                Else
                    da = New SqlDataAdapter("SELECT userid, username FROM userTBL WHERE role = @role ORDER BY username", conn)
                    da.SelectCommand.Parameters.Add(SecurityHelper.CreateSqlParameter("@role", "User", SqlDbType.VarChar))
                End If

                da.Fill(ds)
                sb.Append("<select style=""width:238px;"" id=""ddlplateno"" runat=""server"" data-placeholder=""Select Plate Number"" style=""width:350px;"" class=""chzn-select"" tabindex=""5"">")
                sb.Append("<option value=""""></option>")

                For i As Integer = 0 To ds.Tables(0).Rows.Count - 1
                    Dim userIdValue As String = ds.Tables(0).Rows(i)(0).ToString()
                    Dim username As String = SecurityHelper.HtmlEncode(ds.Tables(0).Rows(i)(1).ToString().ToUpper())
                    
                    ' SECURITY FIX: Load vehicles with parameterized query
                    Using vcmd As New SqlCommand("SELECT plateno FROM vehicleTBL WHERE userid = @userid ORDER BY plateno", conn)
                        vcmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@userid", userIdValue, SqlDbType.Int))
                        
                        conn.Open()
                        sb.Append($"<optgroup label=""{username}"">")
                        
                        Using dr As SqlDataReader = vcmd.ExecuteReader()
                            While dr.Read()
                                Dim plateNumber As String = SecurityHelper.HtmlEncode(dr("plateno").ToString())
                                sb.Append($"<option value=""{plateNumber}"">{plateNumber}</option>")
                            End While
                        End Using
                        
                        sb.Append("</optgroup>")
                        conn.Close()
                    End Using
                Next
                
                sb.Append("</select>")
            End Using

        Catch ex As Exception
            SecurityHelper.LogError("LoadVehicleData error", ex, Server)
            sb.Append("<select><option>Error loading data</option></select>")
        End Try
    End Sub

End Class