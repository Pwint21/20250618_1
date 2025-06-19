Imports System.Data.SqlClient
Imports System.Text.RegularExpressions
Imports System.Web.Security

Partial Class Main
    Inherits System.Web.UI.Page
    
    ' Private variables instead of public for security
    Private IsSpeacialUsers As Boolean = False
    Private username As String = ""
    Private PubUserid As String = ""
    Private mainpage As String = "SmartFleetApk.aspx"
    Private OssReport As Boolean = True
    Private JReport As Boolean = False
    Private role As String = ""
    Private nid As String = "0"
    Private showOss As Boolean = True
    Private viewer As Boolean = False
    Private ytluser As Boolean = False
    Private customRole As String = ""
    Private checkItenery As Boolean = False

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load, Me.Load
        Try
            ' SECURITY FIX: Check server-side session instead of cookies
            If Not IsUserAuthenticated() Then
                Response.Redirect("Login.aspx")
                Return
            End If

            ' SECURITY FIX: Validate query string parameter
            Dim uname As String = ValidateInput(Request.QueryString("n"))
            If String.IsNullOrEmpty(uname) Then
                Response.Redirect("Login.aspx")
                Return
            End If

            ' SECURITY FIX: Get user data from session, not cookies
            username = Session("username").ToString().ToUpper()
            Dim usertype As String = Session("usertype").ToString()
            Dim userid As String = Session("userid").ToString()
            
            ' SECURITY FIX: Validate userid is numeric
            If Not IsNumeric(userid) Then
                Response.Redirect("Login.aspx")
                Return
            End If

            PubUserid = userid
            hiduserid.Value = userid
            role = Session("role").ToString()
            Dim la As String = Session("LA").ToString()

            ' SECURITY FIX: Validate username matches session
            If uname.ToUpper() <> username Then
                Response.Redirect("Login.aspx")
                Return
            End If

            ' Set permissions based on role
            If role.StartsWith("Admin") Or la = "Y" Then
                OssReport = False
            End If

            ' SECURITY FIX: Get company name from session
            Dim companyName As String = Session("companyname").ToString()
            If companyName.StartsWith("YTL") Then
                ytluser = True
            Else
                ytluser = False
            End If

            If usertype = "5" Then
                showOss = False
                viewer = True
                OssReport = False
            End If

            ' Check for special users
            If username = "SPYON" Or username = "MARTINYTL" Or role = "Admin" Or username = "SWEEHAR" Or username = "PCWong_BS" Then
                IsSpeacialUsers = True
            End If

            If username = "BINTANG" Or userid = "1912" Or userid = "1934" Or userid = "1618" Or userid = "1933" Or userid = "1944" Then
                JReport = True
            End If

            customRole = Session("customrole").ToString()

            ' SECURITY FIX: Use parameterized queries for database operations
            LoadAlertNotifications(userid, role)
            LoadUserItinerary(userid)

        Catch ex As Exception
            ' SECURITY FIX: Log error securely without exposing details
            LogSecurityEvent("Page_Load error for user: " & username, ex)
            Response.Redirect("Login.aspx")
        End Try
    End Sub

    ' SECURITY FIX: Server-side authentication check
    Private Function IsUserAuthenticated() As Boolean
        Try
            ' Check if user has valid session
            If Session("authenticated") Is Nothing OrElse Session("authenticated") <> True Then
                Return False
            End If

            ' Verify session hasn't expired
            If Session("loginTime") Is Nothing Then
                Return False
            End If

            Dim loginTime As DateTime = CType(Session("loginTime"), DateTime)
            If DateTime.Now.Subtract(loginTime).TotalMinutes > 30 Then ' 30 minute timeout
                Session.Clear()
                Return False
            End If

            ' Verify required session data exists
            If Session("username") Is Nothing Or Session("userid") Is Nothing Or Session("role") Is Nothing Then
                Return False
            End If

            Return True
        Catch ex As Exception
            LogSecurityEvent("Authentication check failed", ex)
            Return False
        End Try
    End Function

    ' SECURITY FIX: Input validation and sanitization
    Private Function ValidateInput(input As String) As String
        Try
            If String.IsNullOrEmpty(input) Then
                Return ""
            End If

            ' Remove potentially dangerous characters
            Dim sanitized As String = Regex.Replace(input, "[<>""'%;()&+\-*/=]", "")

            ' Limit length
            If sanitized.Length > 50 Then
                sanitized = sanitized.Substring(0, 50)
            End If

            ' Only allow alphanumeric and underscore
            If Not Regex.IsMatch(sanitized, "^[a-zA-Z0-9_]+$") Then
                Return ""
            End If

            Return sanitized
        Catch ex As Exception
            LogSecurityEvent("Input validation failed for: " & input, ex)
            Return ""
        End Try
    End Function

    ' SECURITY FIX: Parameterized query for alert notifications
    Private Sub LoadAlertNotifications(userid As String, role As String)
        Try
            Dim userslist As String = Session("userslist").ToString()
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As New SqlCommand()
            
            ' SECURITY FIX: Use parameterized queries
            If role = "User" Then
                cmd.CommandText = "SELECT TOP 1 id FROM dbo.alert_notification WHERE userid = @userid ORDER BY id DESC"
                cmd.Parameters.AddWithValue("@userid", userid)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                ' SECURITY FIX: Validate userslist contains only numbers and commas
                If ValidateUsersList(userslist) Then
                    cmd.CommandText = "SELECT TOP 1 id FROM dbo.alert_notification WHERE userid IN (" & userslist & ") ORDER BY id DESC"
                Else
                    ' Fallback to single user if validation fails
                    cmd.CommandText = "SELECT TOP 1 id FROM dbo.alert_notification WHERE userid = @userid ORDER BY id DESC"
                    cmd.Parameters.AddWithValue("@userid", userid)
                End If
            Else
                ' Default to single user for unknown roles
                cmd.CommandText = "SELECT TOP 1 id FROM dbo.alert_notification WHERE userid = @userid ORDER BY id DESC"
                cmd.Parameters.AddWithValue("@userid", userid)
            End If

            cmd.Connection = conn
            
            Try
                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader()
                If dr.Read() Then
                    nid = dr("id").ToString()
                End If
                dr.Close()
            Finally
                conn.Close()
            End Try

        Catch ex As Exception
            LogSecurityEvent("LoadAlertNotifications failed for userid: " & userid, ex)
            nid = "0"
        End Try
    End Sub

    ' SECURITY FIX: Parameterized query for user itinerary
    Private Sub LoadUserItinerary(userid As String)
        Try
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As New SqlCommand("SELECT itenery FROM dbo.userTBL WHERE userid = @userid", conn)
            cmd.Parameters.AddWithValue("@userid", userid)

            Try
                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader()
                If dr.Read() Then
                    If dr("itenery").ToString() = "1" Then
                        checkItenery = True
                    End If
                End If
                dr.Close()
            Finally
                conn.Close()
            End Try

        Catch ex As Exception
            LogSecurityEvent("LoadUserItinerary failed for userid: " & userid, ex)
        End Try
    End Sub

    ' SECURITY FIX: Validate users list contains only numbers and commas
    Private Function ValidateUsersList(usersList As String) As Boolean
        Try
            If String.IsNullOrEmpty(usersList) Then
                Return False
            End If

            ' Check if it contains only numbers, commas, and spaces
            If Not Regex.IsMatch(usersList, "^[0-9,\s]+$") Then
                Return False
            End If

            ' Split and validate each user ID
            Dim userIds() As String = usersList.Split(","c)
            For Each userId As String In userIds
                Dim trimmedId As String = userId.Trim()
                If Not IsNumeric(trimmedId) Then
                    Return False
                End If
            Next

            Return True
        Catch ex As Exception
            LogSecurityEvent("ValidateUsersList failed for: " & usersList, ex)
            Return False
        End Try
    End Function

    ' SECURITY FIX: Secure logging function
    Private Sub LogSecurityEvent(message As String, ex As Exception)
        Try
            ' Log to application event log or secure log file
            ' Don't expose sensitive information in logs
            Dim logMessage As String = String.Format("{0}: {1} - User: {2}, IP: {3}", 
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 
                message, 
                If(Session("username"), "Unknown"), 
                Request.UserHostAddress)
            
            ' Write to secure log (implement your logging mechanism here)
            System.Diagnostics.EventLog.WriteEntry("YTL_Security", logMessage, System.Diagnostics.EventLogEntryType.Warning)
        Catch
            ' Fail silently to prevent information disclosure
        End Try
    End Sub

End Class