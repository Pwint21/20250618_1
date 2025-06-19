Imports System.Web.SessionState

Public Class SessionManager
    Private Const SESSION_TIMEOUT_MINUTES As Integer = 30
    Private Const MAX_SESSION_DURATION_HOURS As Integer = 8

    ' Create secure session
    Public Shared Sub CreateSecureSession(userId As String, username As String, role As String, userType As String, companyName As String)
        Try
            ' Clear any existing session
            HttpContext.Current.Session.Clear()
            
            ' Generate session token
            Dim sessionToken As String = Guid.NewGuid().ToString()
            
            ' Set session values
            With HttpContext.Current.Session
                .Item("authenticated") = True
                .Item("userId") = userId
                .Item("username") = username
                .Item("role") = role
                .Item("userType") = userType
                .Item("companyName") = companyName
                .Item("sessionToken") = sessionToken
                .Item("loginTime") = DateTime.Now
                .Item("lastActivity") = DateTime.Now
                .Item("ipAddress") = HttpContext.Current.Request.UserHostAddress
                .Item("userAgent") = HttpContext.Current.Request.UserAgent
                .Timeout = SESSION_TIMEOUT_MINUTES
            End With
            
            ' Log session creation
            LogSessionActivity("Session created", userId, sessionToken)
            
        Catch ex As Exception
            SecurityHelper.LogError("Session creation failed", ex, HttpContext.Current.Server)
            Throw New SecurityException("Session creation failed")
        End Try
    End Sub

    ' Validate session security
    Public Shared Function ValidateSession() As Boolean
        Try
            Dim session As HttpSessionState = HttpContext.Current.Session
            Dim request As HttpRequest = HttpContext.Current.Request
            
            ' Check if session exists and is authenticated
            If session("authenticated") Is Nothing OrElse Not CBool(session("authenticated")) Then
                Return False
            End If
            
            ' Check session timeout
            If session("lastActivity") IsNot Nothing Then
                Dim lastActivity As DateTime = CDate(session("lastActivity"))
                If DateTime.Now.Subtract(lastActivity).TotalMinutes > SESSION_TIMEOUT_MINUTES Then
                    DestroySession("Session timeout")
                    Return False
                End If
            End If
            
            ' Check maximum session duration
            If session("loginTime") IsNot Nothing Then
                Dim loginTime As DateTime = CDate(session("loginTime"))
                If DateTime.Now.Subtract(loginTime).TotalHours > MAX_SESSION_DURATION_HOURS Then
                    DestroySession("Maximum session duration exceeded")
                    Return False
                End If
            End If
            
            ' Validate IP address (optional - can cause issues with load balancers)
            'If session("ipAddress") IsNot Nothing AndAlso session("ipAddress").ToString() <> request.UserHostAddress Then
            '    DestroySession("IP address mismatch")
            '    Return False
            'End If
            
            ' Update last activity
            session("lastActivity") = DateTime.Now
            
            Return True
            
        Catch ex As Exception
            SecurityHelper.LogError("Session validation failed", ex, HttpContext.Current.Server)
            Return False
        End Try
    End Function

    ' Destroy session securely
    Public Shared Sub DestroySession(reason As String)
        Try
            Dim session As HttpSessionState = HttpContext.Current.Session
            Dim userId As String = If(session("userId"), "Unknown").ToString()
            Dim sessionToken As String = If(session("sessionToken"), "Unknown").ToString()
            
            ' Log session destruction
            LogSessionActivity($"Session destroyed: {reason}", userId, sessionToken)
            
            ' Clear session
            session.Clear()
            session.Abandon()
            
            ' Clear authentication cookies
            ClearAuthenticationCookies()
            
        Catch ex As Exception
            SecurityHelper.LogError("Session destruction failed", ex, HttpContext.Current.Server)
        End Try
    End Sub

    ' Get current user ID safely
    Public Shared Function GetCurrentUserId() As String
        Try
            If ValidateSession() Then
                Return HttpContext.Current.Session("userId").ToString()
            End If
        Catch
        End Try
        Return Nothing
    End Function

    ' Get current username safely
    Public Shared Function GetCurrentUsername() As String
        Try
            If ValidateSession() Then
                Return HttpContext.Current.Session("username").ToString()
            End If
        Catch
        End Try
        Return Nothing
    End Function

    ' Get current user role safely
    Public Shared Function GetCurrentUserRole() As String
        Try
            If ValidateSession() Then
                Return HttpContext.Current.Session("role").ToString()
            End If
        Catch
        End Try
        Return Nothing
    End Function

    ' Clear authentication cookies
    Private Shared Sub ClearAuthenticationCookies()
        Try
            Dim response As HttpResponse = HttpContext.Current.Response
            
            ' Clear userinfo cookie
            Dim userinfoCookie As New HttpCookie("userinfo") With {
                .Expires = DateTime.Now.AddDays(-1),
                .Value = "",
                .HttpOnly = True,
                .Secure = HttpContext.Current.Request.IsSecureConnection
            }
            response.Cookies.Add(userinfoCookie)
            
            ' Clear accesslevel cookie
            Dim accessCookie As New HttpCookie("accesslevel") With {
                .Expires = DateTime.Now.AddDays(-1),
                .Value = "",
                .HttpOnly = True,
                .Secure = HttpContext.Current.Request.IsSecureConnection
            }
            response.Cookies.Add(accessCookie)
            
        Catch ex As Exception
            SecurityHelper.LogError("Cookie clearing failed", ex, HttpContext.Current.Server)
        End Try
    End Sub

    ' Log session activities
    Private Shared Sub LogSessionActivity(activity As String, userId As String, sessionToken As String)
        Try
            Dim logMessage As String = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {activity} - User: {userId} - Token: {sessionToken.Substring(0, 8)}... - IP: {HttpContext.Current.Request.UserHostAddress}"
            SecurityHelper.LogSecurityEvent(logMessage)
        Catch
            ' Fail silently
        End Try
    End Sub
End Class