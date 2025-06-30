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

                If Request.Cookies("userinfo") Is Nothing Then
                    Response.Redirect("Login.aspx")
                End If

                Dim userid As String = Request.Cookies("userinfo")("userid")
                Dim role As String = Request.Cookies("userinfo")("role")
                Dim userslist As String = Request.Cookies("userinfo")("userslist")

                Dim suserid As String = Request.QueryString("userid")

                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As SqlCommand = New SqlCommand("select userid,username from userTBL order by username", conn)
                Dim dr As SqlDataReader

                If Convert.ToString(Request.Cookies("userinfo")("username")).ToUpper = "SVWONG" Then
                    cmd = New SqlCommand("select userid,username from userTBL where companyname='lafarge' and role='superuser' order by username", conn)
                ElseIf Request.Cookies("userinfo")("userid") <> "0002" And Request.Cookies("userinfo")("role") = "Admin" Then
                    cmd = New SqlCommand("select userid,username from userTBL where userid='" & userid & "' or role<>'Admin' order by username", conn)
                ElseIf role = "User" Then
                    cmd = New SqlCommand("select userid,username from userTBL where userid='" & userid & "' order by username", conn)
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    cmd = New SqlCommand("select userid,username from userTBL where userid in(" & userslist & ") order by username", conn)
                End If

                conn.Open()
                dr = cmd.ExecuteReader()

                If dr.HasRows() And (Request.Cookies("userinfo")("userid") = "0002" Or Request.Cookies("userinfo")("userid") = "923") Then
                    ddlusers.Items.Add(New ListItem("-- All Users --", "-- All Users --"))
                    ddlusers.Items.Add(New ListItem("-- All Operators --", "-- All Operators --"))
                    ddlusers.Items.Add(New ListItem("-- All Admins --", "-- All Admins --"))
                    ddlusers.Items.Add(New ListItem("-- All SuperUsers --", "-- All SuperUsers --"))
                End If
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


        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try

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

                Dim r As DataRow

                If Not userid = "--Select User Name--" Then

                    Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                    Dim cmd As SqlCommand = New SqlCommand("select userid,username,pwd,companyname,phoneno,faxno,streetname + ', ' + postcode + ', ' + state as address,role, usertype, erp,itenery,drcaccess, dbip from userTBL where userid='" & userid & "'", conn)
                    Dim dr As SqlDataReader

                    'If userid = "-- All Usernames --" Then
                    '    cmd = New SqlCommand("select userid,username,pwd,companyname,phoneno,faxno,streetname + ', ' + postcode + ', ' + state as address,role, usertype from userTBL where role<>'Admin' or userid='" & userid & "' order by username", conn)
                    'End If
                    If userid = "-- All Customers --" And Request.Cookies("userinfo")("userid") = "0002" Then
                        cmd = New SqlCommand("select userid,username,pwd,companyname,phoneno,faxno,streetname + ', ' + postcode + ', ' + state as address,role, usertype, erp,itenery,drcaccess, dbip from userTBL order by username", conn)
                    ElseIf userid = "-- All Customers --" And Request.Cookies("userinfo")("userid") <> "0002" Then
                        cmd = New SqlCommand("select userid,username,pwd,companyname,phoneno,faxno,streetname + ', ' + postcode + ', ' + state as address,role, usertype, erp,itenery,drcaccess, dbip from userTBL where userid='" & Request.Cookies("userinfo")("userid") & "' or role<>'Admin' order by username", conn)
                    ElseIf userid = "-- All Users --" Then
                        cmd = New SqlCommand("select userid,username,pwd,companyname,phoneno,faxno,streetname + ', ' + postcode + ', ' + state as address,role, usertype, erp,itenery,drcaccess, dbip from userTBL where role='User' order by username", conn)
                    ElseIf userid = "-- All Operators --" Then
                        cmd = New SqlCommand("select userid,username,pwd,companyname,phoneno,faxno,streetname + ', ' + postcode + ', ' + state as address,role, usertype, erp,itenery,drcaccess, dbip from userTBL where role='Operator' order by username", conn)
                    ElseIf userid = "-- All Admins --" Then
                        cmd = New SqlCommand("select userid,username,pwd,companyname,phoneno,faxno,streetname + ', ' + postcode + ', ' + state as address,role, usertype, erp,itenery,drcaccess, dbip from userTBL where role='Admin' order by username", conn)
                    ElseIf userid = "-- All SuperUsers --" Then
                        cmd = New SqlCommand("select userid,username,pwd,companyname,phoneno,faxno,streetname + ', ' + postcode + ', ' + state as address,role, usertype, erp,itenery,drcaccess, dbip from userTBL where role='SuperUser' order by username", conn)
                    End If

                    conn.Open()
                    dr = cmd.ExecuteReader()
                    Dim i As Int32 = 1
                    While dr.Read()
                        r = userstable.NewRow
                        r(0) = "<input type=""checkbox"" name=""chk"" value=""" & dr("userid") & """/>"
                        r(1) = i.ToString()
                        r(2) = "<a href= UpdateUser.aspx?userid=" & dr("userid") & "> " & dr("username") & " </a>"
                        r(3) = dr("pwd")
                        r(4) = dr("companyname")
                        r(5) = dr("phoneno")
                        r(6) = dr("address")
                        r(7) = "<img src=""images/" & dr("role") & ".gif"" alt=""" & dr("role") & """ width=""20px"" height=""20px""/> " & dr("role")
                        r(8) = dr("usertype")
                        If dr("erp") = False Then
                            r(9) = "No"
                        Else : r(9) = "Yes"
                        End If
                        If dr("itenery") = False Then
                            r(10) = "No"
                        Else : r(10) = "Yes"
                        End If
                        If dr("drcaccess") = False Then
                            r(11) = "No"
                        Else : r(11) = "Yes"
                        End If
                        If dr("dbip") = "192.168.1.21" Then
                            r(12) = "Lafarge"
                        Else : r(12) = dr("dbip")
                        End If

                        userstable.Rows.Add(r)
                        i = i + 1
                    End While
                    conn.Close()

                    Label1.Visible = True
                    Label2.Visible = True
                    Label3.Visible = True
                    Label4.Visible = True

                End If

                If userstable.Rows.Count = 0 Then
                    r = userstable.NewRow
                    r(0) = "<input type=""checkbox"" name=""chk"" />"
                    r(1) = "--"
                    r(2) = "--"
                    r(3) = "--"
                    r(4) = "--"
                    r(5) = "--"
                    r(5) = "--"
                    r(6) = "--"
                    r(7) = "--"
                    r(8) = "--"
                    r(9) = "--"
                    r(10) = "--"
                    r(11) = "--"
                    r(12) = "--"
                    userstable.Rows.Add(r)
                End If

                usersgrid.DataSource = userstable
                usersgrid.DataBind()

                If Convert.ToString(Request.Cookies("userinfo")("username")).ToUpper() = "SVWONG" Then
                    usersgrid.Columns(8).Visible = False
                    usersgrid.Columns(9).Visible = False
                    usersgrid.Columns(11).Visible = False
                End If

            Catch ex As Exception
                Response.Write(ex.Message)
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
                Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As SqlCommand
                Dim userids() As String = Split(Request.Form("chk"), ",")

                For i As Int16 = 0 To userids.Length - 1
                    conn.Open()
                    cmd = New SqlCommand("delete from userTBL where userid='" & userids(i) & "'", conn)
                    cmd.ExecuteNonQuery()
                    conn.Close()
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
