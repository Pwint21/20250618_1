<%@ Application Language="VB" %>

<script runat="server">
    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Application startup code
        SecurityHelper.LogSecurityEvent("Application started")
    End Sub
    
    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Application shutdown code
        SecurityHelper.LogSecurityEvent("Application ended")
    End Sub
    
    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Global error handler
        Dim ex As Exception = Server.GetLastError()
        If ex IsNot Nothing Then
            SecurityHelper.LogError("Global application error", ex, Server)
            
            ' Clear the error
            Server.ClearError()
            
            ' Redirect to error page
            Response.Redirect("~/Error.aspx")
        End If
    End Sub
    
    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Session start code
        Session.Timeout = 30
    End Sub
    
    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Session end code - this only fires for InProc sessions
        Try
            If Session("userId") IsNot Nothing Then
                SecurityHelper.LogSecurityEvent($"Session ended for user: {Session("userId")}")
            End If
        Catch
            ' Fail silently
        End Try
    End Sub
    
    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Add security headers to all responses
        Response.Headers.Remove("Server")
        Response.Headers.Add("X-Frame-Options", "DENY")
        Response.Headers.Add("X-Content-Type-Options", "nosniff")
        Response.Headers.Add("X-XSS-Protection", "1; mode=block")
        
        ' Force HTTPS in production
        If Not Request.IsSecureConnection AndAlso Not Request.IsLocal Then
            Dim secureUrl As String = Request.Url.ToString().Replace("http://", "https://")
            Response.Redirect(secureUrl, True)
        End If
    End Sub
    
    Sub Application_PreSendRequestHeaders(ByVal sender As Object, ByVal e As EventArgs)
        ' Remove potentially revealing headers
        Response.Headers.Remove("Server")
        Response.Headers.Remove("X-AspNet-Version")
        Response.Headers.Remove("X-AspNetMvc-Version")
    End Sub
</script>