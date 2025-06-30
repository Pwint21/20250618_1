Imports System.Data.SqlClient

Partial Class UserLogReport
    Inherits System.Web.UI.Page

    Public show As Boolean = False
    Public ec As String = "false"
    Public ucheck As Boolean = False

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
            SecurityHelper.LogError("UserLogReport OnInit Error", ex, Server)
            Response.Redirect("~/Error.aspx")
        End Try

        MyBase.OnInit(e)
    End Sub

    Private Sub LoadUserDropdown(userid As String, role As String, userslist As String)
        Try
            Dim parameters As New Dictionary(Of String, Object)
            Dim query As String

            If role = "User" Then
                query = "SELECT username, userid FROM userTBL WHERE userid = @userid ORDER BY username"
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
                    
                    query = $"SELECT username, userid FROM userTBL WHERE userid IN ({String.Join(",", paramNames)}) ORDER BY username"
                    
                    ddluser.Items.Add(New ListItem("--Select User Name--", "--Select User Name--"))
                    ddluser.Items.Add(New ListItem("--All Users--", "--All Users--"))
                Else
                    query = "SELECT username, userid FROM userTBL WHERE userid = @userid ORDER BY username"
                    parameters.Add("@userid", userid)
                End If
            Else
                query = "SELECT username, userid FROM userTBL ORDER BY username"
            End If

            Dim userData As DataTable = DatabaseHelper.ExecuteQuery(query, parameters)
            
            For Each row As DataRow In userData.Rows
                ddluser.Items.Add(New ListItem(SecurityHelper.HtmlEncode(row("username").ToString()), row("userid").ToString()))
            Next

        Catch ex As Exception
            SecurityHelper.LogError("LoadUserDropdown error", ex, Server)
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("~/Login.aspx")
                Return
            End If

            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
            If role = "Admin" Then
                ucheck = True
            End If

            If Page.IsPostBack = False Then
                ImageButton1.Attributes.Add("onclick", "return mysubmit()")
                txtBeginDate.Value = DateTime.Now.ToString("yyyy/MM/dd")
                txtEndDate.Value = DateTime.Now.ToString("yyyy/MM/dd")
                
                If role <> "User" Then
                    ddluser.SelectedValue = "--All Users--"
                End If
                
                DisplayGrid()
            End If

        Catch ex As Exception
            SecurityHelper.LogError("UserLogReport Page_Load error", ex, Server)
        End Try
    End Sub

    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
        Try
            DisplayGrid()
        Catch ex As Exception
            SecurityHelper.LogError("ImageButton1_Click error", ex, Server)
        End Try
    End Sub

    Protected Sub DisplayGrid()
        Try
            ' SECURITY FIX: Validate date inputs
            If Not SecurityHelper.ValidateDate(txtBeginDate.Value) OrElse Not SecurityHelper.ValidateDate(txtEndDate.Value) Then
                Return
            End If

            Dim begindatetime As String = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
            Dim enddatetime As String = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"

            Dim userlogtable As New DataTable
            userlogtable.Columns.Add(New DataColumn("sno"))
            userlogtable.Columns.Add(New DataColumn("username"))
            userlogtable.Columns.Add(New DataColumn("application"))
            userlogtable.Columns.Add(New DataColumn("logintime"))
            userlogtable.Columns.Add(New DataColumn("logouttime"))
            userlogtable.Columns.Add(New DataColumn("hostaddress"))
            userlogtable.Columns.Add(New DataColumn("browser"))
            userlogtable.Columns.Add(New DataColumn("url"))

            ' SECURITY FIX: Use parameterized query
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@begindate", begindatetime},
                {"@enddate", enddatetime}
            }

            Dim query As String = "SELECT t1.userid, t2.role, t2.username, t1.logintime, t1.logouttime, t1.hostaddress, t1.browser, t1.applicationversion, t1.url, t1.status " &
                                "FROM (SELECT * FROM user_log WHERE logintime BETWEEN @begindate AND @enddate AND userid IN (SELECT userid FROM userTBL WHERE companyname LIKE '%YTL%')) AS t1 " &
                                "INNER JOIN userTBL AS t2 ON t1.userid = t2.userid ORDER BY t1.logintime"

            Dim logData As DataTable = DatabaseHelper.ExecuteQuery(query, parameters)
            
            Dim i As Integer = 1
            For Each row As DataRow In logData.Rows
                Dim r As DataRow = userlogtable.NewRow()
                r(0) = i.ToString()
                
                Dim role As String = ""
                Select Case row("role").ToString()
                    Case "User"
                        role = " (U)"
                    Case "SuperUser"
                        role = " (S)"
                    Case "Operator"
                        role = " (O)"
                    Case "Admin"
                        role = " (A)"
                End Select

                r(1) = SecurityHelper.HtmlEncode(row("username").ToString()) & role
                r(2) = SecurityHelper.HtmlEncode(row("applicationversion").ToString())
                r(3) = CType(row("logintime"), Date).ToString("yyyy-MM-dd HH:mm:ss")
                
                If CBool(row("status")) Then
                    r(4) = "--"
                Else
                    r(4) = CType(row("logouttime"), Date).ToString("yyyy-MM-dd HH:mm:ss")
                End If

                r(5) = SecurityHelper.HtmlEncode(row("hostaddress").ToString())
                r(6) = SecurityHelper.HtmlEncode(row("browser").ToString())
                r(7) = SecurityHelper.HtmlEncode(row("url").ToString())
                
                userlogtable.Rows.Add(r)
                i += 1
            Next

            If userlogtable.Rows.Count = 0 Then
                Dim r As DataRow = userlogtable.NewRow()
                For j As Integer = 0 To 7
                    r(j) = "--"
                Next
                userlogtable.Rows.Add(r)
            End If

            Session.Remove("exceltable")
            Session.Remove("exceltable2")

            usersloggrid.PageSize = CInt(noofrecords.SelectedValue)
            ec = "true"
            usersloggrid.DataSource = userlogtable
            usersloggrid.DataBind()
            
            userlogtable.Columns.RemoveAt(2)
            userlogtable.Columns.RemoveAt(3)
            Session("exceltable") = userlogtable

            If usersloggrid.PageCount > 1 Then
                show = True
            End If

            usersloggrid.Columns(4).Visible = False
            usersloggrid.Columns(5).Visible = True
            usersloggrid.Columns(6).Visible = True
            usersloggrid.Columns(7).Visible = True

        Catch ex As Exception
            SecurityHelper.LogError("DisplayGrid error", ex, Server)
        End Try
    End Sub

    Protected Sub usersloggrid_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles usersloggrid.PageIndexChanging
        Try
            If cbxlogin.Checked = True Then
                usersloggrid.Columns.Item(4).Visible = False
            Else
                usersloggrid.Columns.Item(4).Visible = True
            End If

            usersloggrid.PageSize = CInt(noofrecords.SelectedValue)
            usersloggrid.DataSource = Session("exceltable")
            usersloggrid.PageIndex = e.NewPageIndex
            usersloggrid.DataBind()

            ec = "true"
            show = True
            
            If ucheck Then
                usersloggrid.Columns(4).Visible = True
                usersloggrid.Columns(5).Visible = True
                usersloggrid.Columns(6).Visible = True
                usersloggrid.Columns(7).Visible = False
            Else
                usersloggrid.Columns(4).Visible = False
                usersloggrid.Columns(5).Visible = False
                usersloggrid.Columns(6).Visible = False
                usersloggrid.Columns(7).Visible = False
            End If

        Catch ex As Exception
            SecurityHelper.LogError("usersloggrid_PageIndexChanging error", ex, Server)
        End Try
    End Sub

End Class