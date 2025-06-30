Imports System.Data.SqlClient
Imports System.Data

Namespace AVLS

    Partial Class UserManagement
        Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()
        End Sub

#End Region

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            Try
                ' SECURITY FIX: Enable authentication check
                If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                    Response.Redirect("~/Login.aspx")
                    Return
                End If

                Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
                Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
                Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)

                LoadUserDropdown(userid, role, userslist)

            Catch ex As Exception
                SecurityHelper.LogError("UserManagement OnInit Error", ex, Server)
                Response.Redirect("~/Error.aspx")
            End Try
            MyBase.OnInit(e)
        End Sub

        Private Sub LoadUserDropdown(userid As String, role As String, userslist As String)
            Try
                Dim parameters As New Dictionary(Of String, Object)
                Dim query As String

                Dim username As String = Session("username").ToString().ToUpper()

                If username = "SVWONG" Then
                    query = "SELECT userid, username FROM userTBL WHERE companyname = 'lafarge' AND role = 'superuser' ORDER BY username"
                ElseIf userid <> "0002" And role = "Admin" Then
                    query = "SELECT userid, username FROM userTBL WHERE userid = @userid OR role <> 'Admin' ORDER BY username"
                    parameters.Add("@userid", userid)
                ElseIf role = "User" Then
                    query = "SELECT userid, username FROM userTBL WHERE userid = @userid ORDER BY username"
                    parameters.Add("@userid", userid)
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    If SecurityHelper.IsValidUsersList(userslist) Then
                        ' Create parameterized query for multiple user IDs
                        Dim userIds() As String = userslist.Split(","c)
                        Dim paramNames As New List(Of String)
                        
                        For i As Integer = 0 To userIds.Length - 1
                            Dim paramName As String = "@userid" & i
                            paramNames.Add(paramName)
                            parameters.Add(paramName, userIds(i).Trim())
                        Next
                        
                        query = $"SELECT userid, username FROM userTBL WHERE userid IN ({String.Join(",", paramNames)}) ORDER BY username"
                    Else
                        query = "SELECT userid, username FROM userTBL WHERE userid = @userid ORDER BY username"
                        parameters.Add("@userid", userid)
                    End If
                Else
                    query = "SELECT userid, username FROM userTBL ORDER BY username"
                End If

                Dim userData As DataTable = DatabaseHelper.ExecuteQuery(query, parameters)
                
                ddlusers.Items.Clear()
                
                If userData.Rows.Count > 0 And (userid = "0002" Or userid = "923") Then
                    ddlusers.Items.Add(New ListItem("-- All Users --", "-- All Users --"))
                    ddlusers.Items.Add(New ListItem("-- All Operators --", "-- All Operators --"))
                    ddlusers.Items.Add(New ListItem("-- All Admins --", "-- All Admins --"))
                    ddlusers.Items.Add(New ListItem("-- All SuperUsers --", "-- All SuperUsers --"))
                End If
                
                For Each row As DataRow In userData.Rows
                    ddlusers.Items.Add(New ListItem(SecurityHelper.HtmlEncode(row("username").ToString()), row("userid").ToString()))
                Next

                Dim suserid As String = Request.QueryString("userid")
                If Not String.IsNullOrEmpty(suserid) AndAlso SecurityHelper.IsValidUserId(suserid) Then
                    ddlusers.SelectedValue = suserid
                End If

            Catch ex As Exception
                SecurityHelper.LogError("LoadUserDropdown error", ex, Server)
            End Try
        End Sub

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                ' SECURITY FIX: Validate user session
                If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                    Response.Redirect("~/Login.aspx")
                    Return
                End If

                If Page.IsPostBack = False Then
                    ImageButton1.Attributes.Add("onclick", "return deleteconfirmation();")
                    ImageButton2.Attributes.Add("onclick", "return deleteconfirmation();")
                    Label1.Visible = False
                    Label2.Visible = False
                    Label3.Visible = False
                    Label4.Visible = False
                    FillGrid()
                End If

            Catch ex As Exception
                SecurityHelper.LogError("UserManagement Page_Load error", ex, Server)
            End Try
        End Sub

        Private Sub FillGrid()
            Try
                Dim userid As String = ddlusers.SelectedValue

                Dim userstable As New DataTable
                userstable.Columns.Add(New DataColumn("chk"))
                userstable.Columns.Add(New DataColumn("sno"))
                userstable.Columns.Add(New DataColumn("username"))
                userstable.Columns.Add(New DataColumn("password"))
                userstable.Columns.Add(New DataColumn("companyname"))
                userstable.Columns.Add(New DataColumn("phoneno"))
                userstable.Columns.Add(New DataColumn("address"))
                userstable.Columns.Add(New DataColumn("role"))
                userstable.Columns.Add(New DataColumn("usertype"))
                userstable.Columns.Add(New DataColumn("ERP"))
                userstable.Columns.Add(New DataColumn("Itinery"))
                userstable.Columns.Add(New DataColumn("drcaccess"))
                userstable.Columns.Add(New DataColumn("server"))

                If userid <> "--Select User Name--" Then
                    Dim parameters As New Dictionary(Of String, Object)
                    Dim query As String

                    If userid = "-- All Users --" Then
                        query = "SELECT userid, username, pwd, companyname, phoneno, faxno, streetname + ', ' + postcode + ', ' + state as address, role, usertype, erp, itenery, drcaccess, dbip FROM userTBL WHERE role = 'User' ORDER BY username"
                    ElseIf userid = "-- All Operators --" Then
                        query = "SELECT userid, username, pwd, companyname, phoneno, faxno, streetname + ', ' + postcode + ', ' + state as address, role, usertype, erp, itenery, drcaccess, dbip FROM userTBL WHERE role = 'Operator' ORDER BY username"
                    ElseIf userid = "-- All Admins --" Then
                        query = "SELECT userid, username, pwd, companyname, phoneno, faxno, streetname + ', ' + postcode + ', ' + state as address, role, usertype, erp, itenery, drcaccess, dbip FROM userTBL WHERE role = 'Admin' ORDER BY username"
                    ElseIf userid = "-- All SuperUsers --" Then
                        query = "SELECT userid, username, pwd, companyname, phoneno, faxno, streetname + ', ' + postcode + ', ' + state as address, role, usertype, erp, itenery, drcaccess, dbip FROM userTBL WHERE role = 'SuperUser' ORDER BY username"
                    Else
                        query = "SELECT userid, username, pwd, companyname, phoneno, faxno, streetname + ', ' + postcode + ', ' + state as address, role, usertype, erp, itenery, drcaccess, dbip FROM userTBL WHERE userid = @userid"
                        parameters.Add("@userid", userid)
                    End If

                    Dim userData As DataTable = DatabaseHelper.ExecuteQuery(query, parameters)
                    
                    Dim i As Integer = 1
                    For Each row As DataRow In userData.Rows
                        Dim r As DataRow = userstable.NewRow()
                        r(0) = $"<input type=""checkbox"" name=""chk"" value=""{SecurityHelper.HtmlEncode(row("userid").ToString())}""/>"
                        r(1) = i.ToString()
                        r(2) = $"<a href=""UpdateUser.aspx?userid={SecurityHelper.HtmlEncode(row("userid").ToString())}"">{SecurityHelper.HtmlEncode(row("username").ToString())}</a>"
                        r(3) = "****" ' SECURITY FIX: Don't display actual passwords
                        r(4) = SecurityHelper.HtmlEncode(row("companyname").ToString())
                        r(5) = SecurityHelper.HtmlEncode(row("phoneno").ToString())
                        r(6) = SecurityHelper.HtmlEncode(row("address").ToString())
                        r(7) = $"<img src=""images/{SecurityHelper.HtmlEncode(row("role").ToString())}.gif"" alt=""{SecurityHelper.HtmlEncode(row("role").ToString())}"" width=""20px"" height=""20px""/> {SecurityHelper.HtmlEncode(row("role").ToString())}"
                        r(8) = SecurityHelper.HtmlEncode(row("usertype").ToString())
                        r(9) = If(CBool(row("erp")), "Yes", "No")
                        r(10) = If(CBool(row("itenery")), "Yes", "No")
                        r(11) = If(CBool(row("drcaccess")), "Yes", "No")
                        r(12) = If(row("dbip").ToString() = "192.168.1.21", "Lafarge", SecurityHelper.HtmlEncode(row("dbip").ToString()))

                        userstable.Rows.Add(r)
                        i += 1
                    Next

                    Label1.Visible = True
                    Label2.Visible = True
                    Label3.Visible = True
                    Label4.Visible = True
                End If

                If userstable.Rows.Count = 0 Then
                    Dim r As DataRow = userstable.NewRow()
                    For j As Integer = 0 To 12
                        r(j) = "--"
                    Next
                    r(0) = "<input type=""checkbox"" name=""chk"" />"
                    userstable.Rows.Add(r)
                End If

                usersgrid.DataSource = userstable
                usersgrid.DataBind()

                Dim username As String = Session("username").ToString().ToUpper()
                If username = "SVWONG" Then
                    usersgrid.Columns(8).Visible = False
                    usersgrid.Columns(9).Visible = False
                    usersgrid.Columns(11).Visible = False
                End If

            Catch ex As Exception
                SecurityHelper.LogError("FillGrid error", ex, Server)
            End Try
        End Sub

        Private Sub ImageButton1_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
            DeleteUser()
        End Sub

        Protected Sub ImageButton2_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton2.Click
            DeleteUser()
        End Sub

        Protected Sub DeleteUser()
            Try
                ' SECURITY FIX: Validate user permissions
                Dim currentRole As String = SecurityHelper.ValidateAndGetUserRole(Request)
                If currentRole <> "Admin" Then
                    Return
                End If

                Dim useridsToDelete As String = Request.Form("chk")
                If String.IsNullOrEmpty(useridsToDelete) Then
                    Return
                End If

                Dim userids() As String = useridsToDelete.Split(","c)
                
                For Each userId As String In userids
                    If SecurityHelper.IsValidUserId(userId.Trim()) Then
                        Dim parameters As New Dictionary(Of String, Object) From {
                            {"@userid", userId.Trim()}
                        }
                        
                        Dim query As String = "DELETE FROM userTBL WHERE userid = @userid"
                        DatabaseHelper.ExecuteNonQuery(query, parameters)
                        
                        SecurityHelper.LogSecurityEvent("USER_DELETED", $"User {userId} deleted by {Session("username")}")
                    End If
                Next

                FillGrid()

            Catch ex As Exception
                SecurityHelper.LogError("DeleteUser error", ex, Server)
            End Try
        End Sub
        
        Protected Sub ddlusers_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlusers.SelectedIndexChanged
            FillGrid()
        End Sub

    End Class

End Namespace