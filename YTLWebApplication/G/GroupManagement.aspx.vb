Imports System.Data.SqlClient
Imports System.Data

Namespace AVLS
    Partial Class GroupManagement
        Inherits System.Web.UI.Page

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            Try
                ' SECURITY FIX: Validate user session
                If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                    Response.Redirect("~/Login.aspx")
                    Return
                End If

                ' SECURITY FIX: Get validated user information
                Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
                Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
                Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)

                ' SECURITY FIX: Validate query string parameter
                Dim suserid As String = SecurityHelper.HtmlEncode(Request.QueryString("userid"))
                If Not String.IsNullOrEmpty(suserid) AndAlso Not SecurityHelper.ValidateUserId(suserid) Then
                    suserid = ""
                End If

                Using conn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
                    Dim cmd As SqlCommand
                    
                    ' SECURITY FIX: Use parameterized queries based on role
                    If role = "User" Then
                        cmd = New SqlCommand("SELECT userid, username FROM userTBL WHERE userid = @userid ORDER BY username", conn)
                        cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@userid", userid, SqlDbType.Int))
                    ElseIf role = "SuperUser" Or role = "Operator" Then
                        ddlusers.Items.Add(New ListItem("--Select User Name--", "--Select User Name--"))
                        ddlusers.Items.Add(New ListItem("--All Users--", "--All Users--"))
                        
                        If Not String.IsNullOrEmpty(userslist) AndAlso SecurityHelper.IsValidUsersList(userslist) Then
                            ' Create parameterized query for multiple user IDs
                            Dim userIds() As String = userslist.Split(","c)
                            Dim parameters As New List(Of String)
                            cmd = New SqlCommand()
                            
                            For i As Integer = 0 To userIds.Length - 1
                                Dim paramName As String = "@userid" & i
                                parameters.Add(paramName)
                                cmd.Parameters.Add(SecurityHelper.CreateSqlParameter(paramName, userIds(i).Trim(), SqlDbType.Int))
                            Next
                            
                            Dim inClause As String = String.Join(",", parameters)
                            cmd.CommandText = $"SELECT userid, username FROM userTBL WHERE userid IN ({inClause}) ORDER BY username"
                            cmd.Connection = conn
                        Else
                            cmd = New SqlCommand("SELECT userid, username FROM userTBL WHERE userid = @userid ORDER BY username", conn)
                            cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@userid", userid, SqlDbType.Int))
                        End If
                    Else
                        cmd = New SqlCommand("SELECT userid, username FROM userTBL WHERE role = @role ORDER BY username", conn)
                        cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@role", "User", SqlDbType.VarChar))
                        ddlusers.Items.Add(New ListItem("--Select User Name--", "--Select User Name--"))
                        ddlusers.Items.Add(New ListItem("--All Users--", "--All Users--"))
                    End If

                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            Dim username As String = SecurityHelper.HtmlEncode(dr("username").ToString())
                            Dim userIdValue As String = dr("userid").ToString()
                            ddlusers.Items.Add(New ListItem(username, userIdValue))
                        End While
                    End Using
                End Using

                If Not String.IsNullOrEmpty(suserid) Then
                    Try
                        ddlusers.SelectedValue = suserid
                    Catch
                        ' Invalid selection, ignore
                    End Try
                End If

            Catch ex As Exception
                SecurityHelper.LogError("GroupManagement OnInit error", ex, Server)
                Response.Redirect("~/Error.aspx")
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

                If Page.IsPostBack = False Then
                    ImageButton1.Attributes.Add("onclick", "return deleteconfirmation();")
                    ImageButton2.Attributes.Add("onclick", "return deleteconfirmation();")
                    FillGrid()
                End If

            Catch ex As Exception
                SecurityHelper.LogError("GroupManagement Page_Load error", ex, Server)
                Response.Redirect("~/Error.aspx")
            End Try
        End Sub

        Private Sub FillGrid()
            Try
                Dim userid As String = ddlusers.SelectedValue

                ' SECURITY FIX: Validate selected user ID
                If userid <> "--Select User Name--" AndAlso userid <> "--All Users--" AndAlso Not SecurityHelper.ValidateUserId(userid) Then
                    Return
                End If

                Dim groupstable As New DataTable
                groupstable.Columns.Add(New DataColumn("chk"))
                groupstable.Columns.Add(New DataColumn("sno"))
                groupstable.Columns.Add(New DataColumn("groupname"))
                groupstable.Columns.Add(New DataColumn("username"))
                groupstable.Columns.Add(New DataColumn("description"))

                Dim r As DataRow

                Using conn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
                    If userid <> "--Select User Name--" Then
                        Dim cmd As SqlCommand
                        
                        If userid = "--All Users--" Then
                            ' SECURITY FIX: Get current user's role and permissions
                            Dim currentUserId As String = SecurityHelper.ValidateAndGetUserId(Request)
                            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
                            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)
                            
                            If role = "User" Then
                                cmd = New SqlCommand("SELECT usertable.userid, usertable.username, groupid, groupname, description FROM vehicle_group grouptable, userTBL usertable WHERE usertable.userid = @userid AND usertable.userid = grouptable.userid ORDER BY usertable.username, groupname", conn)
                                cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@userid", currentUserId, SqlDbType.Int))
                            ElseIf role = "SuperUser" Or role = "Operator" Then
                                If Not String.IsNullOrEmpty(userslist) AndAlso SecurityHelper.IsValidUsersList(userslist) Then
                                    ' Create parameterized query for multiple user IDs
                                    Dim userIds() As String = userslist.Split(","c)
                                    Dim parameters As New List(Of String)
                                    cmd = New SqlCommand()
                                    
                                    For i As Integer = 0 To userIds.Length - 1
                                        Dim paramName As String = "@userid" & i
                                        parameters.Add(paramName)
                                        cmd.Parameters.Add(SecurityHelper.CreateSqlParameter(paramName, userIds(i).Trim(), SqlDbType.Int))
                                    Next
                                    
                                    Dim inClause As String = String.Join(",", parameters)
                                    cmd.CommandText = $"SELECT usertable.userid, usertable.username, groupid, groupname, description FROM vehicle_group grouptable, userTBL usertable WHERE usertable.userid IN ({inClause}) AND usertable.userid = grouptable.userid ORDER BY usertable.username, groupname"
                                    cmd.Connection = conn
                                Else
                                    cmd = New SqlCommand("SELECT usertable.userid, usertable.username, groupid, groupname, description FROM vehicle_group grouptable, userTBL usertable WHERE usertable.userid = @userid AND usertable.userid = grouptable.userid ORDER BY usertable.username, groupname", conn)
                                    cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@userid", currentUserId, SqlDbType.Int))
                                End If
                            Else
                                cmd = New SqlCommand("SELECT usertable.userid, usertable.username, groupid, groupname, description FROM vehicle_group grouptable, userTBL usertable WHERE usertable.userid = grouptable.userid ORDER BY usertable.username, groupname", conn)
                            End If
                        Else
                            cmd = New SqlCommand("SELECT usertable.userid, usertable.username, groupid, groupname, description FROM vehicle_group grouptable, userTBL usertable WHERE usertable.userid = @userid AND usertable.userid = grouptable.userid ORDER BY usertable.username, groupname", conn)
                            cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@userid", userid, SqlDbType.Int))
                        End If

                        conn.Open()
                        Using dr As SqlDataReader = cmd.ExecuteReader()
                            Dim i As Int32 = 1
                            While dr.Read
                                r = groupstable.NewRow
                                
                                ' SECURITY FIX: Validate group ID
                                Dim groupId As String = dr("groupid").ToString()
                                If SecurityHelper.ValidateUserId(groupId) Then
                                    r(0) = $"<input type=""checkbox"" name=""chk"" value=""{SecurityHelper.HtmlEncode(groupId)}""/>"
                                Else
                                    r(0) = "<input type=""checkbox"" name=""chk"" disabled/>"
                                End If
                                
                                r(1) = i.ToString()
                                
                                ' SECURITY FIX: HTML encode and validate data
                                Dim groupName As String = SecurityHelper.HtmlEncode(dr("groupname").ToString())
                                Dim userIdValue As String = dr("userid").ToString()
                                
                                If SecurityHelper.ValidateUserId(groupId) AndAlso SecurityHelper.ValidateUserId(userIdValue) Then
                                    r(2) = $" <a href=""UpdateGroup.aspx?gid={SecurityHelper.HtmlEncode(groupId)}&uid={SecurityHelper.HtmlEncode(userIdValue)}"">{groupName}</a>"
                                Else
                                    r(2) = groupName
                                End If
                                
                                r(3) = SecurityHelper.HtmlEncode(dr("username").ToString())
                                r(4) = SecurityHelper.HtmlEncode(dr("description").ToString())
                                groupstable.Rows.Add(r)
                                i = i + 1
                            End While
                        End Using
                    End If

                    If groupstable.Rows.Count = 0 Then
                        r = groupstable.NewRow
                        r(0) = "<input type=""checkbox"" name=""chk"" disabled/>"
                        r(1) = "--"
                        r(2) = "--"
                        r(3) = "--"
                        r(4) = "--"
                        groupstable.Rows.Add(r)
                    End If

                    groupsgrid.DataSource = groupstable
                    groupsgrid.DataBind()
                End Using

            Catch ex As Exception
                SecurityHelper.LogError("FillGrid error", ex, Server)
            End Try
        End Sub

        Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
            DeleteGroup()
        End Sub

        Protected Sub ImageButton2_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton2.Click
            DeleteGroup()
        End Sub

        Protected Sub DeleteGroup()
            Try
                ' SECURITY FIX: Validate user session
                If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                    Response.Redirect("~/Login.aspx")
                    Return
                End If

                ' SECURITY FIX: Validate CSRF token (if implemented)
                ' If Not SecurityHelper.ValidateCSRFToken(Session("CSRFToken"), Request.Form("CSRFToken")) Then
                '     Return
                ' End If

                If String.IsNullOrEmpty(Request.Form("chk")) Then
                    Return
                End If

                Dim groupides() As String = Request.Form("chk").Split(","c)

                Using conn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
                    For i As Int32 = 0 To groupides.Length - 1
                        Dim groupId As String = groupides(i).Trim()
                        
                        ' SECURITY FIX: Validate group ID
                        If SecurityHelper.ValidateUserId(groupId) Then
                            Using cmd As New SqlCommand("DELETE FROM vehicle_group WHERE groupid = @groupid", conn)
                                cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@groupid", groupId, SqlDbType.Int))
                                
                                Try
                                    conn.Open()
                                    cmd.ExecuteNonQuery()
                                    SecurityHelper.LogSecurityEvent($"Group deleted: {groupId} by user: {SecurityHelper.ValidateAndGetUserId(Request)}")
                                Catch ex As Exception
                                    SecurityHelper.LogError($"Delete group {groupId} error", ex, Server)
                                Finally
                                    conn.Close()
                                End Try
                            End Using
                        End If
                    Next
                End Using
                
                FillGrid()

            Catch ex As Exception
                SecurityHelper.LogError("DeleteGroup error", ex, Server)
            End Try
        End Sub

        Protected Sub ddlusers_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlusers.SelectedIndexChanged
            FillGrid()
        End Sub

    End Class
End Namespace