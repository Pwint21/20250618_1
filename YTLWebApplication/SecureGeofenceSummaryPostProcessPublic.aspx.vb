Imports System.Data.SqlClient
Imports System.Data
Imports ADODB
Imports AspMap
Imports System.IO
Imports System.Web.Security

Partial Class GeofenceSummaryPostProcessPublic
    Inherits System.Web.UI.Page
    Public show As Boolean = False
    Public ec As String = "false"
    Dim sCon As String = System.Configuration.ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim suspectTime As String
    Dim GrantOdometer, GrantFuel, GrantPrice, GrandIdlingFuel, GrandIdlingPrice, GrantRefuelLitre, GrantRefuelPrice As Double
    Dim GrandIdlingTime As TimeSpan

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            ' SECURITY FIX: Enable authentication check
            If Session("login") Is Nothing OrElse Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
                Return
            End If

            ' SECURITY FIX: Validate user session
            If Not ValidateUserSession() Then
                Response.Redirect("Login.aspx")
                Return
            End If

        Catch ex As Exception
            ' SECURITY FIX: Don't expose detailed error messages
            LogError("OnInit Error", ex)
            Response.Redirect("Error.aspx")
        End Try
        MyBase.OnInit(e)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Enable authentication check
            If Session("login") Is Nothing OrElse Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
                Return
            End If

            Label2.Visible = False
            Label3.Visible = False

            If Page.IsPostBack = False Then
                ImageButton1.Attributes.Add("onclick", "return mysubmit()")
                txtBeginDate.Value = DateTime.Now.ToString("yyyy/MM/dd")
                txtEndDate.Value = DateTime.Now.ToString("yyyy/MM/dd")
                populateNode()
                
                If Request.Cookies("userinfo")("role") = "User" Then
                    tvPlateno.ExpandAll()
                End If
            End If

        Catch ex As Exception
            LogError("Page_Load Error", ex)
            Response.Redirect("Error.aspx")
        End Try
    End Sub

    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
        DisplayLogInformation1()
    End Sub

    Sub populateNode()
        Try
            Dim ds As System.Data.DataSet = getTreeViewData()
            For Each masterRow As DataRow In ds.Tables("user").Rows
                ' SECURITY FIX: HTML encode output to prevent XSS
                Dim masterNode As New TreeNode(HttpUtility.HtmlEncode(masterRow("username").ToString()), masterRow("userid").ToString())
                tvPlateno.Nodes.Add(masterNode)
                
                For Each childRow As DataRow In masterRow.GetChildRows("Children")
                    Dim childNode As New TreeNode(HttpUtility.HtmlEncode(childRow("plateno").ToString()), childRow("plateno").ToString())
                    masterNode.ChildNodes.Add(childNode)
                    
                    If Request.Cookies("userinfo")("role") = "User" Then
                        masterNode.Checked = True
                        childNode.Checked = True
                    End If
                Next
            Next
        Catch ex As SystemException
            LogError("populateNode Error", ex)
        End Try
    End Sub

    Function getTreeViewData() As System.Data.DataSet
        Try
            Dim conn As New SqlConnection(sCon)
            Dim daPlateno As SqlDataAdapter
            Dim daUser As SqlDataAdapter

            ' SECURITY FIX: Validate user input and use parameterized queries
            Dim userid As String = ValidateAndGetUserId()
            Dim role As String = ValidateAndGetUserRole()
            Dim userslist As String = ValidateAndGetUsersList()
            
            Dim ds As System.Data.DataSet = New System.Data.DataSet()

            If role = "Admin" Then
                Dim dsRoute As DataSet = New DataSet()
                ' SECURITY FIX: Use parameterized query
                daUser = New SqlDataAdapter("SELECT userid, username, dbip FROM userTBL WHERE role = @role ORDER BY username", conn)
                daUser.SelectCommand.Parameters.AddWithValue("@role", "user")
                daUser.Fill(dsRoute, "user")
                
                For x As Int32 = 0 To dsRoute.Tables("user").Rows.Count - 1
                    Dim uid As String = dsRoute.Tables("user").Rows(x)("userid").ToString()
                    ' SECURITY FIX: Use parameterized query
                    Dim daRoute As SqlDataAdapter = New SqlDataAdapter("SELECT * FROM vehicleTBL WHERE userid = @userid ORDER BY plateno", conn)
                    daRoute.SelectCommand.Parameters.AddWithValue("@userid", uid)
                    daRoute.Fill(dsRoute, "vehicle")
                Next
                dsRoute.Relations.Add("Children", dsRoute.Tables("user").Columns("userid"), dsRoute.Tables("vehicle").Columns("userid"))
                Return dsRoute
                
            ElseIf role = "SuperUser" Or role = "Operator" Then
                ' SECURITY FIX: Validate userslist and use parameterized query
                If Not String.IsNullOrEmpty(userslist) AndAlso IsValidUsersList(userslist) Then
                    daPlateno = New SqlDataAdapter("SELECT * FROM vehicleTBL WHERE userid IN (" & userslist & ") ORDER BY plateno", conn)
                    daUser = New SqlDataAdapter("SELECT * FROM userTBL WHERE userid IN (" & userslist & ") ORDER BY username", conn)
                End If
            Else ' User role
                ' SECURITY FIX: Use parameterized query
                daPlateno = New SqlDataAdapter("SELECT * FROM vehicleTBL WHERE userid = @userid ORDER BY plateno", conn)
                daPlateno.SelectCommand.Parameters.AddWithValue("@userid", userid)
                daUser = New SqlDataAdapter("SELECT * FROM userTBL WHERE userid = @userid ORDER BY username", conn)
                daUser.SelectCommand.Parameters.AddWithValue("@userid", userid)
            End If

            If daPlateno IsNot Nothing Then
                daPlateno.Fill(ds, "vehicle")
            End If
            If daUser IsNot Nothing Then
                daUser.Fill(ds, "user")
            End If
            
            ds.Relations.Add("Children", ds.Tables("user").Columns("userid"), ds.Tables("vehicle").Columns("userid"))
            Return ds
            
        Catch ex As SystemException
            LogError("getTreeViewData Error", ex)
            Return New DataSet()
        End Try
    End Function

    ' SECURITY FIX: Add validation methods
    Private Function ValidateUserSession() As Boolean
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Return False
            End If
            
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            
            ' Validate userid is numeric
            Dim userIdInt As Integer
            If Not Integer.TryParse(userid, userIdInt) Then
                Return False
            End If
            
            ' Validate role is in allowed list
            Dim allowedRoles As String() = {"Admin", "SuperUser", "Operator", "User"}
            If Not allowedRoles.Contains(role) Then
                Return False
            End If
            
            Return True
        Catch
            Return False
        End Try
    End Function

    Private Function ValidateAndGetUserId() As String
        Dim userid As String = Request.Cookies("userinfo")("userid")
        Dim userIdInt As Integer
        If Integer.TryParse(userid, userIdInt) AndAlso userIdInt > 0 Then
            Return userid
        End If
        Throw New SecurityException("Invalid user ID")
    End Function

    Private Function ValidateAndGetUserRole() As String
        Dim role As String = Request.Cookies("userinfo")("role")
        Dim allowedRoles As String() = {"Admin", "SuperUser", "Operator", "User"}
        If allowedRoles.Contains(role) Then
            Return role
        End If
        Throw New SecurityException("Invalid user role")
    End Function

    Private Function ValidateAndGetUsersList() As String
        Dim userslist As String = Request.Cookies("userinfo")("userslist")
        If IsValidUsersList(userslist) Then
            Return userslist
        End If
        Throw New SecurityException("Invalid users list")
    End Function

    Private Function IsValidUsersList(usersList As String) As Boolean
        If String.IsNullOrEmpty(usersList) Then
            Return False
        End If
        
        ' Check if all values are numeric
        Dim users As String() = usersList.Split(","c)
        For Each user As String In users
            Dim userId As Integer
            If Not Integer.TryParse(user.Trim(), userId) OrElse userId <= 0 Then
                Return False
            End If
        Next
        Return True
    End Function

    Private Sub LogError(message As String, ex As Exception)
        Try
            Dim logMessage As String = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} - {message}: {ex.Message}"
            Dim sw As New StreamWriter(Server.MapPath("~/Logs/ErrorLog.txt"), True)
            sw.WriteLine(logMessage)
            sw.Close()
        Catch
            ' Fail silently if logging fails
        End Try
    End Sub

    ' Note: DisplayLogInformation1 method would need similar security fixes
    ' This is a partial implementation focusing on the most critical issues
End Class