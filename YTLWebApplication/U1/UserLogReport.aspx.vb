Imports System.Data.SqlClient

Partial Class UserLogReport
    Inherits System.Web.UI.Page

    Public show As Boolean = False
    Public ec As String = "false"
    Public ucheck As Boolean = False
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            If Request.Cookies("userinfo")("userid") = Nothing Then
                Response.Redirect("Login.aspx")
            End If
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand
            Dim dr As SqlDataReader

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            cmd = New SqlCommand("select username,userid from userTBL order by username", conn)
            If role = "User" Then
                cmd = New SqlCommand("select username,userid from userTBL where userid='" & userid & "' order by username", conn)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select username,userid from userTBL where userid in(" & userslist & ") order by username", conn)
                ddluser.Items.Add(New ListItem("--Select User Name--", "--Select User Name--"))
                ddluser.Items.Add(New ListItem("--All Users--", "--All Users--"))
            End If

            conn.Open()
            dr = cmd.ExecuteReader()

            While dr.Read()
                ddluser.Items.Add(New ListItem(dr("username"), dr("userid")))
            End While
            conn.Close()
        Catch ex As Exception

        End Try

        MyBase.OnInit(e)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Request.Cookies("userinfo")("role") = "Admin" Then
            ucheck = True
        End If
        Try
            If Page.IsPostBack = False Then
                ImageButton1.Attributes.Add("onclick", "return mysubmit()")
                txtBeginDate.Value = Now().ToString("yyyy/MM/dd")
                txtEndDate.Value = Now().ToString("yyyy/MM/dd")
                Dim role = Request.Cookies("userinfo")("role")
                If Not role = "User" Then
                    ddluser.SelectedValue = "--All Users--"
                End If
                DisplayGrid()
            End If
        Catch ex As Exception
            Response.Write(ex.Message & " - " & ex.StackTrace)
        End Try
    End Sub

    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
        Try
            DisplayGrid()
        Catch ex As Exception
            Response.Write(ex.Message & " - " & ex.StackTrace)
        End Try

    End Sub

    Protected Sub DisplayGrid()
        Try
            Dim begindatetime As String = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
            Dim enddatetime As String = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"


            Dim userlogtable As New DataTable
            Dim r As DataRow

            userlogtable.Columns.Add(New DataColumn("sno"))
            userlogtable.Columns.Add(New DataColumn("username"))
            userlogtable.Columns.Add(New DataColumn("application"))
            userlogtable.Columns.Add(New DataColumn("logintime"))
            userlogtable.Columns.Add(New DataColumn("logouttime"))
            userlogtable.Columns.Add(New DataColumn("hostaddress"))
            userlogtable.Columns.Add(New DataColumn("browser"))
            userlogtable.Columns.Add(New DataColumn("url"))

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As New SqlCommand
            Dim condition As String = ""

            Dim userid As String = Session("userid")
            Dim userslist As String = Session("userslist")



            cmd = New SqlCommand("select t1.userid,t2.role,t2.username,t1.logintime,t1.logouttime,t1.hostaddress,t1.browser,t1.applicationversion,t1.url,t1.status from (select * from user_log where logintime between '" & begindatetime & "' and '" & enddatetime & "' and userid in (select userid  from userTBL where companyname like '%YTL%')) as t1,userTBL as t2 where t1.userid=t2.userid  order by t1.logintime", conn)
            '  Response.Write(cmd.CommandText)

            Dim dr As SqlDataReader
            Dim i As Int32 = 1

            conn.Open()
            dr = cmd.ExecuteReader()
            While dr.Read()
                r = userlogtable.NewRow
                r(0) = i.ToString()
                Dim role As String = ""
                Select Case dr("role")
                    Case "User"
                        role = " (U)"
                    Case "SuperUser"
                        role = " (S)"
                    Case "Operator"
                        role = " (O)"
                    Case "Admin"
                        role = " (A)"
                End Select

                r(1) = dr("username") & role
                r(2) = dr("applicationversion")
                r(3) = CType(dr("logintime"), Date).ToString("yyyy-MM-dd HH:mm:ss")
                If dr("status") = True Then
                    r(4) = "--"
                Else
                    r(4) = CType(dr("logouttime"), Date).ToString("yyyy-MM-dd HH:mm:ss")
                End If

                r(5) = dr("hostaddress")
                r(6) = dr("browser")
                Dim url As String = dr("url")
                'r(7) = "<img src=""images/url.gif"" alt=""" & url & """ title=""" & url & """/> "
                r(7) = url
                userlogtable.Rows.Add(r)

                i += 1
            End While

            If userlogtable.Rows.Count = 0 Then
                r = userlogtable.NewRow
                r(0) = "--"
                r(1) = "--"
                r(2) = "--"
                r(3) = "--"
                r(4) = "--"
                r(5) = "--"
                r(6) = "--"
                r(7) = "--"
                userlogtable.Rows.Add(r)
            End If

            Session.Remove("exceltable")
            Session.Remove("exceltable2")


            usersloggrid.PageSize = noofrecords.SelectedValue
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
            Response.Write(ex.Message & " - " & ex.StackTrace)
        End Try

    End Sub

    Protected Sub usersloggrid_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles usersloggrid.PageIndexChanging

        If cbxlogin.Checked = True Then
            usersloggrid.Columns.Item(4).Visible = False
        Else
            usersloggrid.Columns.Item(4).Visible = True
        End If

        usersloggrid.PageSize = noofrecords.SelectedValue
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
    End Sub
End Class
