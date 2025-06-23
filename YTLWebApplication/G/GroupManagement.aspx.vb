Imports System.Data.SqlClient
Imports System.Data

Namespace AVLS
    Partial Class GroupManagement
        Inherits System.Web.UI.Page

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            Try

                If Request.Cookies("userinfo") Is Nothing Then
                    Response.Redirect("Login.aspx")
                End If

                Dim userid As String = Request.Cookies("userinfo")("userid")
                Dim role As String = Request.Cookies("userinfo")("role")
                Dim userslist As String = Request.Cookies("userinfo")("userslist")

                Dim suserid As String = Request.QueryString("userid")

                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As SqlCommand
                Dim dr As SqlDataReader

                If role = "User" Then
                    cmd = New SqlCommand("select userid,username from userTBL where userid='" & userid & "' order by username", conn)
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    cmd = New SqlCommand("select userid,username from userTBL where userid in(" & userslist & ") order by username", conn)
                    ddlusers.Items.Add(New ListItem("--Select User Name--", "--Select User Name--"))
                    ddlusers.Items.Add(New ListItem("--All Users--", "--All Users--"))
                Else
                    cmd = New SqlCommand("select userid,username from userTBL where role='User' order by username", conn)
                    ddlusers.Items.Add(New ListItem("--Select User Name--", "--Select User Name--"))
                    ddlusers.Items.Add(New ListItem("--All Users--", "--All Users--"))
                End If

                conn.Open()
                dr = cmd.ExecuteReader()
                While dr.Read()
                    ddlusers.Items.Add(New ListItem(dr("username"), dr("userid")))
                End While
                conn.Close()

                If Not suserid = "" Then
                    ddlusers.SelectedValue = suserid
                End If


            Catch ex As Exception


            End Try
            MyBase.OnInit(e)
        End Sub


        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Try

                If Page.IsPostBack = False Then
                    ImageButton1.Attributes.Add("onclick", "return deleteconfirmation();")
                    ImageButton2.Attributes.Add("onclick", "return deleteconfirmation();")
                    FillGrid()
                End If

            Catch ex As Exception

            End Try
        End Sub

        Private Sub FillGrid()
            Try
                Dim userid As String = ddlusers.SelectedValue

                Dim groupstable As New DataTable
                groupstable.Columns.Add(New DataColumn("chk"))
                groupstable.Columns.Add(New DataColumn("sno"))
                groupstable.Columns.Add(New DataColumn("groupname"))
                groupstable.Columns.Add(New DataColumn("username"))
                groupstable.Columns.Add(New DataColumn("description"))

                Dim r As DataRow

                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

                If Not userid = "--Select User Name--" Then

                    Dim cmd As SqlCommand = New SqlCommand("select usertable.userid,usertable.username,groupid,groupname,description from vehicle_group grouptable,userTBL usertable where usertable.userid='" & userid & "' and usertable.userid=grouptable.userid order by usertable.username,groupname", conn)
                    Dim dr As SqlDataReader

                    If userid = "--All Users--" Then
                        Dim role = Request.Cookies("userinfo")("role")
                        Dim userslist As String = Request.Cookies("userinfo")("userslist")
                        cmd = New SqlCommand("select usertable.userid,usertable.username,groupid,groupname,description from vehicle_group grouptable,userTBL usertable where usertable.userid=grouptable.userid order by usertable.username,groupname", conn)

                        If role = "User" Then
                            cmd = New SqlCommand("select usertable.userid,usertable.username,groupid,groupname,description from vehicle_group grouptable,userTBL usertable where usertable.userid='" & userid & "' and usertable.userid=grouptable.userid order by usertable.username,groupname", conn)
                        ElseIf role = "SuperUser" Or role = "Operator" Then
                            cmd = New SqlCommand("select usertable.userid,usertable.username,groupid,groupname,description from vehicle_group grouptable,userTBL usertable where usertable.userid in (" & userslist & ") and usertable.userid=grouptable.userid order by usertable.username,groupname", conn)
                        End If

                    End If


                    conn.Open()
                    dr = cmd.ExecuteReader()
                    Dim i As Int32 = 1
                    While dr.Read
                        r = groupstable.NewRow
                        r(0) = "<input type=""checkbox"" name=""chk"" value=""" & dr("groupid") & """/>"
                        r(1) = i.ToString()
                        r(2) = " <a href= UpdateGroup.aspx?gid=" & dr("groupid") & "&uid=" & dr("userid") & "> " & dr("groupname") & " </a>"
                        r(3) = dr("username")
                        r(4) = dr("description")
                        groupstable.Rows.Add(r)
                        i = i + 1
                    End While

                    conn.Close()
                End If

                If groupstable.Rows.Count = 0 Then
                    r = groupstable.NewRow
                    r(0) = "<input type=""checkbox"" name=""chk"" />"
                    r(1) = "--"
                    r(2) = "--"
                    r(3) = "--"
                    r(4) = "--"
                    groupstable.Rows.Add(r)
                End If

                groupsgrid.DataSource = groupstable
                groupsgrid.DataBind()

            Catch ex As Exception

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
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As SqlCommand
                Dim unitid As String = ""

                Dim groupides() As String = Request.Form("chk").Split(",")

                For i As Int32 = 0 To groupides.Length - 1

                    cmd = New SqlCommand("delete from vehicle_group where groupid='" & groupides(i) & "'", conn)
                    Try
                        conn.Open()
                        cmd.ExecuteNonQuery()
                    Catch ex As Exception

                    Finally
                        conn.Close()
                    End Try

                Next
                FillGrid()

            Catch ex As Exception

            End Try
        End Sub

        Protected Sub ddlusers_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlusers.SelectedIndexChanged
            FillGrid()
        End Sub
    End Class

End Namespace