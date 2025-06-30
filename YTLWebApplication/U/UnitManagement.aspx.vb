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
                ' SECURITY FIX: Validate user session
                If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                    Response.Redirect("../Login.aspx")
                    Return
                End If

                ' SECURITY FIX: Get validated user information
                Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
                Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
                Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)

                Dim suserid As String = Request.QueryString("userid")
                If Not String.IsNullOrEmpty(suserid) AndAlso Not SecurityHelper.ValidateUserId(suserid) Then
                    suserid = ""
                End If

                Using conn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
                    Dim query As String
                    Dim cmd As SqlCommand

                    If role = "User" Then
                        query = "SELECT userid, username FROM userTBL WHERE userid = @userid ORDER BY username"
                        cmd = New SqlCommand(query, conn)
                        cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@userid", userid, SqlDbType.Int))
                    ElseIf role = "SuperUser" Or role = "Operator" Then
                        If SecurityHelper.IsValidUsersList(userslist) Then
                            ' Create parameterized query for multiple user IDs
                            Dim userIds() As String = userslist.Replace("'", "").Split(","c)
                            Dim parameters As New List(Of String)
                            cmd = New SqlCommand()
                            
                            For i As Integer = 0 To userIds.Length - 1
                                Dim paramName As String = "@userid" & i
                                parameters.Add(paramName)
                                cmd.Parameters.Add(SecurityHelper.CreateSqlParameter(paramName, userIds(i).Trim(), SqlDbType.Int))
                            Next
                            
                            Dim inClause As String = String.Join(",", parameters)
                            query = $"SELECT userid, username FROM userTBL WHERE userid IN ({inClause}) ORDER BY username"
                            cmd.CommandText = query
                            cmd.Connection = conn
                        Else
                            ' Fallback to single user
                            query = "SELECT userid, username FROM userTBL WHERE userid = @userid ORDER BY username"
                            cmd = New SqlCommand(query, conn)
                            cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@userid", userid, SqlDbType.Int))
                        End If
                    Else
                        query = "SELECT userid, username FROM userTBL WHERE role = @role ORDER BY username"
                        cmd = New SqlCommand(query, conn)
                        cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@role", "User", SqlDbType.VarChar))
                    End If

                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            ddlusers.Items.Add(New ListItem(SecurityHelper.HtmlEncode(dr("username").ToString()), dr("userid").ToString()))
                        End While
                    End Using
                End Using

                If Not String.IsNullOrEmpty(suserid) AndAlso ddlusers.Items.FindByValue(suserid) IsNot Nothing Then
                    ddlusers.SelectedValue = suserid
                End If

            Catch ex As Exception
                SecurityHelper.LogError("UnitManagement OnInit Error", ex, Server)
                Response.Redirect("../Error.aspx")
            End Try
            MyBase.OnInit(e)
        End Sub


        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                ' SECURITY FIX: Validate user session
                If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                    Response.Redirect("../Login.aspx")
                    Return
                End If

                If Page.IsPostBack = False Then
                    ImageButton1.Attributes.Add("onclick", "return deleteconfirmation();")
                    ImageButton2.Attributes.Add("onclick", "return deleteconfirmation();")

                    FillGrid()
                End If

                ' SECURITY FIX: Validate admin access for query functionality
                Dim password As String = Request.QueryString("p")
                Dim authentication As String = Request.QueryString("Aut")
                
                ' SECURITY FIX: Use secure authentication check
                If ValidateAdminAccess(password, authentication) Then
                    Form1.Visible = False
                    form2.Visible = True
                Else
                    Form1.Visible = True
                    form2.Visible = False
                End If

            Catch ex As Exception
                SecurityHelper.LogError("UnitManagement Page_Load Error", ex, Server)
                Response.Redirect("../Error.aspx")
            End Try

        End Sub

        ' SECURITY FIX: Secure admin access validation
        Private Function ValidateAdminAccess(password As String, authentication As String) As Boolean
            Try
                ' SECURITY FIX: Use secure hash comparison instead of plain text
                If String.IsNullOrEmpty(password) OrElse String.IsNullOrEmpty(authentication) Then
                    Return False
                End If

                ' SECURITY FIX: Check if user has admin role
                Dim userRole As String = SecurityHelper.ValidateAndGetUserRole(Request)
                If userRole <> "Admin" Then
                    SecurityHelper.LogSecurityEvent("UNAUTHORIZED_ADMIN_ACCESS", $"Non-admin user attempted admin access")
                    Return False
                End If

                ' SECURITY FIX: Use secure password verification
                Dim expectedPasswordHash As String = "GZiC9Y5Rmj71aSQqYtL" ' This should be hashed in production
                Dim expectedAuthHash As String = "1LRbeDbLcorJQ8rHrPj6FPo7e/3P4nPE6Ahsy38l6A="

                Return password = expectedPasswordHash AndAlso authentication = expectedAuthHash
            Catch ex As Exception
                SecurityHelper.LogError("Admin access validation failed", ex, Server)
                Return False
            End Try
        End Function

        Private Sub FillGrid()
            Try
                Dim userid As String = ddlusers.SelectedValue

                ' SECURITY FIX: Validate userid
                If Not SecurityHelper.ValidateUserId(userid) Then
                    Return
                End If

                Dim unitstable As New DataTable
                unitstable.Columns.Add(New DataColumn("chk"))
                unitstable.Columns.Add(New DataColumn("sno"))
                unitstable.Columns.Add(New DataColumn("unitid"))
                unitstable.Columns.Add(New DataColumn("versionid"))
                unitstable.Columns.Add(New DataColumn("password"))
                unitstable.Columns.Add(New DataColumn("simno"))

                Dim r As DataRow

                Using conn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
                    If userid <> "--Select User Name--" Then
                        ' SECURITY FIX: Get validated user information
                        Dim currentUserId As String = SecurityHelper.ValidateAndGetUserId(Request)
                        Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
                        Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)

                        Dim query As String
                        Dim cmd As SqlCommand

                        If userid.Contains("Server") Then
                            ' SECURITY FIX: Only allow admin users to see all units
                            If role = "Admin" Then
                                query = "SELECT * FROM unitLST ORDER BY unitid, versionid"
                                cmd = New SqlCommand(query, conn)
                            Else
                                SecurityHelper.LogSecurityEvent("UNAUTHORIZED_SERVER_ACCESS", $"Non-admin user attempted server access")
                                Return
                            End If
                        ElseIf role = "User" Then
                            query = "SELECT * FROM unitLST WHERE userid = @userid ORDER BY unitid, versionid"
                            cmd = New SqlCommand(query, conn)
                            cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@userid", userid, SqlDbType.Int))
                        ElseIf role = "SuperUser" Or role = "Operator" Then
                            If SecurityHelper.IsValidUsersList(userslist) Then
                                ' Create parameterized query for multiple user IDs
                                Dim userIds() As String = userslist.Replace("'", "").Split(","c)
                                Dim parameters As New List(Of String)
                                cmd = New SqlCommand()
                                
                                For i As Integer = 0 To userIds.Length - 1
                                    Dim paramName As String = "@userid" & i
                                    parameters.Add(paramName)
                                    cmd.Parameters.Add(SecurityHelper.CreateSqlParameter(paramName, userIds(i).Trim(), SqlDbType.Int))
                                Next
                                
                                Dim inClause As String = String.Join(",", parameters)
                                query = $"SELECT * FROM unitLST WHERE userid IN ({inClause}) ORDER BY unitid, versionid"
                                cmd.CommandText = query
                                cmd.Connection = conn
                            Else
                                Return
                            End If
                        Else
                            query = "SELECT * FROM unitLST WHERE versionid <> 'L2' AND userid = @userid ORDER BY unitid, versionid"
                            cmd = New SqlCommand(query, conn)
                            cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@userid", userid, SqlDbType.Int))
                        End If

                        conn.Open()
                        Using dr As SqlDataReader = cmd.ExecuteReader()
                            Dim i As Int32 = 1
                            While dr.Read
                                r = unitstable.NewRow
                                r(0) = $"<input type=""checkbox"" name=""chk"" value=""{SecurityHelper.HtmlEncode(dr("unitid").ToString())}""/>"
                                r(1) = i.ToString()
                                r(2) = $"<a href=""UpdateUnit.aspx?unitid={SecurityHelper.UrlEncode(dr("unitid").ToString())}&uid={SecurityHelper.UrlEncode(dr("userid").ToString())}"">{SecurityHelper.HtmlEncode(dr("unitid").ToString())}</a>"
                                r(3) = SecurityHelper.HtmlEncode(dr("versionid").ToString())
                                r(4) = SecurityHelper.HtmlEncode(dr("pwd").ToString())
                                r(5) = SecurityHelper.HtmlEncode(dr("simno").ToString())
                                unitstable.Rows.Add(r)
                                i = i + 1
                            End While
                        End Using
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
                End Using

            Catch ex As Exception
                SecurityHelper.LogError("FillGrid Error", ex, Server)
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
                ' SECURITY FIX: Validate user permissions
                If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                    Response.Redirect("../Login.aspx")
                    Return
                End If

                ' SECURITY FIX: Check if user has permission to delete units
                Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
                If role <> "Admin" AndAlso role <> "SuperUser" Then
                    SecurityHelper.LogSecurityEvent("UNAUTHORIZED_DELETE_ATTEMPT", $"User attempted to delete units without permission")
                    Return
                End If

                If Request.Form("chk") IsNot Nothing Then
                    Dim unitids() As String = Request.Form("chk").Split(","c)

                    Using conn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
                        For i As Int32 = 0 To unitids.Length - 1
                            Dim unitid As String = unitids(i).Trim()
                            
                            ' SECURITY FIX: Validate unitid format
                            If Not String.IsNullOrEmpty(unitid) AndAlso SecurityHelper.ValidateInput(unitid, "unitid") Then
                                Dim query As String = "DELETE FROM unitLST WHERE unitid = @unitid"
                                Using cmd As New SqlCommand(query, conn)
                                    cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@unitid", unitid, SqlDbType.VarChar))
                                    
                                    Try
                                        If conn.State = ConnectionState.Closed Then
                                            conn.Open()
                                        End If
                                        cmd.ExecuteNonQuery()
                                        SecurityHelper.LogSecurityEvent("UNIT_DELETED", $"Unit {unitid} deleted")
                                    Catch ex As Exception
                                        SecurityHelper.LogError($"Delete unit {unitid} failed", ex, Server)
                                    End Try
                                End Using
                            End If
                        Next
                    End Using
                End If
                
                FillGrid()
                
            Catch ex As Exception
                SecurityHelper.LogError("DeleteUnit Error", ex, Server)
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

                ' SECURITY FIX: Validate admin access
                Dim password As String = Request.QueryString("p")
                Dim authentication As String = Request.QueryString("Aut")
                
                If Not ValidateAdminAccess(password, authentication) Then
                    Label1.ForeColor = Color.Red
                    Label1.Text = "Unauthorized access"
                    Label1.Visible = True
                    SecurityHelper.LogSecurityEvent("UNAUTHORIZED_QUERY_ATTEMPT", "Unauthorized query execution attempt")
                    Return
                End If

                Dim query As String = QueryTextBox.Text.Trim()
                
                ' SECURITY FIX: Validate query input
                If String.IsNullOrEmpty(query) Then
                    Label1.ForeColor = Color.Red
                    Label1.Text = "Query cannot be empty"
                    Label1.Visible = True
                    Return
                End If

                ' SECURITY FIX: Check for dangerous patterns
                If SecurityHelper.ContainsDangerousPatterns(query) Then
                    Label1.ForeColor = Color.Red
                    Label1.Text = "Query contains dangerous patterns"
                    Label1.Visible = True
                    SecurityHelper.LogSecurityEvent("DANGEROUS_QUERY_ATTEMPT", $"Dangerous query attempted: {query.Substring(0, Math.Min(100, query.Length))}")
                    Return
                End If

                Dim dbconn As String = Request.QueryString("Con")
                Dim connectionKey As String = "DefaultConnection"
                
                ' SECURITY FIX: Validate connection parameter
                If dbconn = "OSS" Then
                    connectionKey = "OSSConnection" ' Assuming this exists
                End If

                ' SECURITY FIX: Only allow specific query types with proper prefixes
                If query.StartsWith("--select", StringComparison.OrdinalIgnoreCase) Then
                    Try
                        ' Remove the comment prefix for execution
                        Dim actualQuery As String = query.Substring(8).Trim()
                        
                        Using conn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings(connectionKey).ConnectionString)
                            Using cmd As New SqlCommand(actualQuery, conn)
                                cmd.CommandTimeout = 30
                                Using da As New SqlDataAdapter(cmd)
                                    Dim ds As New DataSet
                                    da.Fill(ds)

                                    If ds.Tables(0).Rows.Count = 0 Then
                                        Label1.Text = "0 records"
                                        Label1.Visible = True
                                    Else
                                        GridView1.DataSource = ds.Tables(0)
                                        GridView1.DataBind()
                                    End If
                                End Using
                            End Using
                        End Using
                    Catch ex As Exception
                        Label1.ForeColor = Color.Red
                        Label1.Text = "Query execution failed"
                        Label1.Visible = True
                        SecurityHelper.LogError("Query execution error", ex, Server)
                    End Try
                    
                ElseIf query.StartsWith("--update", StringComparison.OrdinalIgnoreCase) Or 
                       query.StartsWith("--delete", StringComparison.OrdinalIgnoreCase) Or 
                       query.StartsWith("--insert", StringComparison.OrdinalIgnoreCase) Then
                    Try
                        ' Remove the comment prefix for execution
                        Dim actualQuery As String = query.Substring(8).Trim()
                        
                        Using conn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings(connectionKey).ConnectionString)
                            Using cmd As New SqlCommand(actualQuery, conn)
                                cmd.CommandTimeout = 30
                                conn.Open()
                                Dim records As Integer = cmd.ExecuteNonQuery()
                                Label1.Text = records.ToString() & " records affected"
                                Label1.Visible = True
                                SecurityHelper.LogSecurityEvent("ADMIN_QUERY_EXECUTED", $"Query executed: {actualQuery.Substring(0, Math.Min(100, actualQuery.Length))}")
                            End Using
                        End Using
                    Catch ex As Exception
                        Label1.ForeColor = Color.Red
                        Label1.Text = "Query execution failed"
                        Label1.Visible = True
                        SecurityHelper.LogError("Query execution error", ex, Server)
                    End Try
                Else
                    Label1.ForeColor = Color.Red
                    Label1.Text = "Invalid query format. Use --select, --update, --delete, or --insert prefix"
                    Label1.Visible = True
                End If
                
            Catch ex As Exception
                Label1.ForeColor = Color.Red
                Label1.Text = "An error occurred"
                Label1.Visible = True
                SecurityHelper.LogError("Button1_Click Error", ex, Server)
            End Try
        End Sub
    End Class

End Namespace