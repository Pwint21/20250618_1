Imports System.Web

Public Class SecurityModule
    Implements IHttpModule

    Public Sub Init(context As HttpApplication) Implements IHttpModule.Init
        AddHandler context.BeginRequest, AddressOf OnBeginRequest
        AddHandler context.PreSendRequestHeaders, AddressOf OnPreSendRequestHeaders
        AddHandler context.Error, AddressOf OnError
    End Sub

    Private Sub OnBeginRequest(sender As Object, e As EventArgs)
        Dim context As HttpContext = HttpContext.Current
        Dim request As HttpRequest = context.Request
        Dim response As HttpResponse = context.Response

        Try
            ' Security checks
            PerformSecurityChecks(request, response)
            
        Catch ex As Exception
            SecurityHelper.LogError("Security module error", ex, context.Server)
        End Try
    End Sub

    Private Sub OnPreSendRequestHeaders(sender As Object, e As EventArgs)
        Dim response As HttpResponse = HttpContext.Current.Response
        
        ' Remove revealing headers
        response.Headers.Remove("Server")
        response.Headers.Remove("X-AspNet-Version")
        response.Headers.Remove("X-AspNetMvc-Version")
        response.Headers.Remove("X-Powered-By")
    End Sub

    Private Sub OnError(sender As Object, e As EventArgs)
        Dim context As HttpContext = HttpContext.Current
        Dim ex As Exception = context.Server.GetLastError()
        
        If ex IsNot Nothing Then
            SecurityHelper.LogError("HTTP module error", ex, context.Server)
        End If
    End Sub

    Private Sub PerformSecurityChecks(request As HttpRequest, response As HttpResponse)
        ' Check for suspicious requests
        If ContainsSuspiciousContent(request) Then
            SecurityHelper.LogSecurityEvent($"Suspicious request detected from {request.UserHostAddress}: {request.RawUrl}")
            response.StatusCode = 400
            response.End()
            Return
        End If

        ' Check request size
        If request.ContentLength > 4194304 Then ' 4MB limit
            SecurityHelper.LogSecurityEvent($"Large request blocked from {request.UserHostAddress}: {request.ContentLength} bytes")
            response.StatusCode = 413
            response.End()
            Return
        End If

        ' Check for directory traversal
        If request.RawUrl.Contains("../") OrElse request.RawUrl.Contains("..\\") Then
            SecurityHelper.LogSecurityEvent($"Directory traversal attempt from {request.UserHostAddress}: {request.RawUrl}")
            response.StatusCode = 400
            response.End()
            Return
        End If
    End Sub

    Private Function ContainsSuspiciousContent(request As HttpRequest) As Boolean
        ' Check URL for suspicious patterns
        Dim suspiciousPatterns() As String = {
            "eval\(",
            "javascript:",
            "vbscript:",
            "<script",
            "onload=",
            "onerror=",
            "union.*select",
            "drop.*table",
            "exec.*xp_",
            "sp_oacreate"
        }

        Dim urlToCheck As String = request.RawUrl.ToLower()
        
        For Each pattern In suspiciousPatterns
            If System.Text.RegularExpressions.Regex.IsMatch(urlToCheck, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase) Then
                Return True
            End If
        Next

        ' Check form data if present
        If request.Form IsNot Nothing Then
            For Each key As String In request.Form.AllKeys
                If key IsNot Nothing Then
                    Dim value As String = request.Form(key)
                    If Not String.IsNullOrEmpty(value) AndAlso SecurityHelper.ContainsDangerousPatterns(value) Then
                        Return True
                    End If
                End If
            Next
        End If

        Return False
    End Function

    Public Sub Dispose() Implements IHttpModule.Dispose
        ' Cleanup code if needed
    End Sub
End Class