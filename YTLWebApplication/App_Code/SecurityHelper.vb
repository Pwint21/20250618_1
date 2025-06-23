Imports System.Data.SqlClient
Imports System.Text.RegularExpressions
Imports System.Web.Security
Imports System.IO
Imports System.Security.Cryptography
Imports System.Web
Imports System

Public Class SecurityHelper

    ' Enhanced user session validation
    Public Shared Function ValidateUserSession(request As HttpRequest, session As HttpSessionState) As Boolean
        Return SessionManager.ValidateSession()
    End Function
    
    Public Shared Function CreateSecureCommand(query As String, connection As SqlConnection) As SqlCommand
        Dim cmd As New SqlCommand(query, connection)
        cmd.CommandTimeout = 30
        Return cmd
    End Function
    
    Public Shared Function SanitizeForHtml(input As String) As String
        If String.IsNullOrEmpty(input) Then
            Return String.Empty
        End If
        Return HttpUtility.HtmlEncode(input)
    End Function
    
    ' JavaScript encode for embedding in JavaScript
    Public Shared Function SanitizeForJavaScript(input As String) As String
        If String.IsNullOrEmpty(input) Then
            Return String.Empty
        End If

        Return input.Replace("\", "\\") _
                   .Replace("'", "\'") _
                   .Replace("""", "\""") _
                   .Replace(vbCrLf, "\n") _
                   .Replace(vbCr, "\r") _
                   .Replace(vbTab, "\t") _
                   .Replace("<", "\u003c") _
                   .Replace(">", "\u003e")
    End Function
    
    ' Comprehensive input validation
    Public Shared Function ValidateInput(input As String, inputType As String) As Boolean
        If String.IsNullOrEmpty(input) Then
            Return False
        End If

        ' Check for dangerous patterns
        If ContainsDangerousPatterns(input) Then
            Return False
        End If

        Select Case inputType.ToLower()
            Case "username"
                Return Regex.IsMatch(input, "^[A-Za-z0-9_@.-]{1,50}$")
            Case "plateno"
                Return Regex.IsMatch(input, "^[A-Za-z0-9\-\s]{1,20}$")
            Case "numeric"
                Return IsNumeric(input)
            Case "date"
                Dim dateValue As DateTime
                Return DateTime.TryParse(input, dateValue)
            Case "shiptocode"
                Return Regex.IsMatch(input, "^[A-Za-z0-9]{1,20}$")
            Case "mobile"
                Return Regex.IsMatch(input, "^[0-9+\-\s()]{1,20}$")
            Case "geofenceid"
                Return IsNumeric(input) AndAlso CInt(input) > 0
            Case Else
                Return input.Length <= 100
        End Select
    End Function

    ' Detect dangerous patterns including SQL injection, XSS, and command injection
    Public Shared Function ContainsDangerousPatterns(input As String) As Boolean
        Dim dangerousPatterns() As String = {
            "(\%27)|(\')|(\-\-)|(\%23)|(#)",
            "((\%3D)|(=))[^\n]*((\%27)|(\')|(\-\-)|(\%3B)|(;))",
            "\w*((\%27)|(\'))((\%6F)|o|(\%4F))((\%72)|r|(\%52))",
            "((\%27)|(\'))union",
            "exec(\s|\+)+(s|x)p\w+",
            "<script[^>]*>.*?</script>",
            "javascript:",
            "vbscript:",
            "onload\s*=",
            "onerror\s*=",
            "onclick\s*=",
            "onmouseover\s*=",
            "eval\s*\(",
            "expression\s*\(",
            "url\s*\(",
            "xp_cmdshell",
            "sp_oacreate",
            "openrowset",
            "bulk\s+insert",
            "shutdown",
            "drop\s+table",
            "truncate\s+table",
            "\.\./",
            "\.\.\\",
            "cmd\.exe",
            "powershell",
            "/bin/sh",
            "/bin/bash"
        }

        For Each pattern As String In dangerousPatterns
            If Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase) Then
                LogSecurityEvent("DANGEROUS_PATTERN", $"Dangerous pattern detected: {pattern} in input: {input.Substring(0, Math.Min(50, input.Length))}")
                Return True
            End If
        Next

        Return False
    End Function

    ' Create SQL parameter safely
    Public Shared Function CreateSqlParameter(parameterName As String, value As Object, sqlDbType As SqlDbType) As SqlParameter
        Dim parameter As New SqlParameter(parameterName, sqlDbType)
        parameter.Value = If(value, DBNull.Value)
        Return parameter
    End Function

    ' HTML encoding with additional security
    Public Shared Function HtmlEncode(input As String) As String
        If String.IsNullOrEmpty(input) Then
            Return String.Empty
        End If

        ' First pass - standard HTML encoding
        Dim encoded As String = HttpUtility.HtmlEncode(input)

        ' Second pass - additional encoding for potential bypasses
        encoded = encoded.Replace("&#x", "&#x")
        encoded = encoded.Replace("&#", "&#")

        Return encoded
    End Function

    ' URL encoding with validation
    Public Shared Function UrlEncode(input As String) As String
        If String.IsNullOrEmpty(input) Then
            Return String.Empty
        End If

        Return HttpUtility.UrlEncode(input)
    End Function

    ' User ID validation and retrieval
    Public Shared Function ValidateAndGetUserId(request As HttpRequest) As String
        Dim userId As String = SessionManager.GetCurrentUserId()
        If String.IsNullOrEmpty(userId) Then
            Throw New SecurityException("Invalid or missing user ID")
        End If
        Return userId
    End Function

    Public Shared Function ValidateUserId(userId As String) As Boolean
        If String.IsNullOrEmpty(userId) Then
            Return False
        End If

        Dim userIdInt As Integer
        Return Integer.TryParse(userId, userIdInt) AndAlso userIdInt > 0
    End Function

    ' Role validation and retrieval
    Public Shared Function ValidateAndGetUserRole(request As HttpRequest) As String
        Dim role As String = SessionManager.GetCurrentUserRole()
        If String.IsNullOrEmpty(role) OrElse Not ValidateUserRole(role) Then
            Throw New SecurityException("Invalid user role")
        End If
        Return role
    End Function

    Public Shared Function ValidateUserRole(role As String) As Boolean
        Dim allowedRoles As String() = {"Admin", "SuperUser", "Operator", "User"}
        Return allowedRoles.Contains(role)
    End Function

    ' Users list validation
    Public Shared Function ValidateAndGetUsersList(request As HttpRequest) As String
        Try
            Dim session As HttpSessionState = HttpContext.Current.Session
            If session("userslist") IsNot Nothing Then
                Dim userslist As String = session("userslist").ToString()
                If IsValidUsersList(userslist) Then
                    Return userslist
                End If
            End If
        Catch
        End Try
        Return String.Empty
    End Function

    Public Shared Function IsValidUsersList(usersList As String) As Boolean
        If String.IsNullOrEmpty(usersList) Then
            Return False
        End If

        ' Remove quotes and validate format
        usersList = usersList.Replace("'", "").Replace(" ", "")

        If Not Regex.IsMatch(usersList, "^[0-9,]+$") Then
            Return False
        End If

        ' Validate each user ID
        Dim users As String() = usersList.Split(","c)
        For Each user As String In users
            If Not String.IsNullOrEmpty(user) Then
                Dim userId As Integer
                If Not Integer.TryParse(user, userId) OrElse userId <= 0 Then
                    Return False
                End If
            End If
        Next
        Return True
    End Function

    ' Date validation with format checking
    Public Shared Function ValidateDate(dateString As String) As Boolean
        If String.IsNullOrEmpty(dateString) Then
            Return False
        End If

        Dim dateValue As DateTime
        If Not DateTime.TryParse(dateString, dateValue) Then
            Return False
        End If

        ' Check reasonable date range (not too far in past or future)
        Dim minDate As DateTime = New DateTime(2000, 1, 1)
        Dim maxDate As DateTime = DateTime.Now.AddYears(1)

        Return dateValue >= minDate AndAlso dateValue <= maxDate
    End Function

    ' Enhanced plate number validation
    Public Shared Function ValidatePlateNumber(plateNumber As String) As Boolean
        If String.IsNullOrEmpty(plateNumber) Then
            Return False
        End If

        ' Check length
        If plateNumber.Length > 15 OrElse plateNumber.Length < 1 Then
            Return False
        End If

        ' Allow alphanumeric characters, spaces, and common plate number symbols
        Dim pattern As String = "^[A-Za-z0-9\-\s]{1,15}$"
        Return Regex.IsMatch(plateNumber, pattern)
    End Function

    ' Enhanced coordinate validation
    Public Shared Function ValidateCoordinate(latitude As String, longitude As String) As Boolean
        Dim lat, lon As Double

        If Not Double.TryParse(latitude, lat) OrElse Not Double.TryParse(longitude, lon) Then
            Return False
        End If

        ' Validate coordinate ranges
        If lat < -90 OrElse lat > 90 Then
            Return False
        End If

        If lon < -180 OrElse lon > 180 Then
            Return False
        End If

        ' Check for suspicious coordinates (0,0 might be invalid in some contexts)
        If lat = 0 AndAlso lon = 0 Then
            Return False
        End If

        Return True
    End Function

    ' Enhanced error logging with security considerations
    Public Shared Sub LogError(message As String, ex As Exception, server As HttpServerUtility)
        Try
            Dim logPath As String
            If server IsNot Nothing Then
                logPath = server.MapPath("~/Logs/ErrorLog.txt")
            Else
                logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "ErrorLog.txt")
            End If

            ' Sanitize error message to prevent log injection
            Dim sanitizedMessage As String = SanitizeLogMessage(message)
            Dim sanitizedError As String = SanitizeLogMessage(ex.Message)

            Dim logEntry As String = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} - {sanitizedMessage}: {sanitizedError}{Environment.NewLine}"

            ' Ensure logs directory exists
            Dim logDir As String = Path.GetDirectoryName(logPath)
            If Not Directory.Exists(logDir) Then
                Directory.CreateDirectory(logDir)
            End If

            ' Write to log file with proper locking
            SyncLock GetType(SecurityHelper)
                File.AppendAllText(logPath, logEntry)
            End SyncLock

        Catch
            ' Fail silently to prevent information disclosure
        End Try
    End Sub

    ' Security event logging
    Public Shared Sub LogSecurityEvent(eventType As String, message As String, Optional userId As String = "")
        Try
            Dim sanitizedMessage As String = SanitizeLogMessage(message)
            Dim currentUserId As String = If(String.IsNullOrEmpty(userId), GetCurrentUserId(), userId)
            Dim logEntry As String = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {eventType} - User: {currentUserId} - IP: {GetClientIP()} - {sanitizedMessage}"

            ' Log to Windows Event Log
            Try
                System.Diagnostics.EventLog.WriteEntry("YTL_Security", logEntry, System.Diagnostics.EventLogEntryType.Warning)
            Catch
                ' Fail silently if event log is not available
            End Try

            ' Also log to file
            Dim logPath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "SecurityLog.txt")
            Dim logDir As String = Path.GetDirectoryName(logPath)
            If Not Directory.Exists(logDir) Then
                Directory.CreateDirectory(logDir)
            End If

            SyncLock GetType(SecurityHelper)
                File.AppendAllText(logPath, logEntry & Environment.NewLine)
            End SyncLock

        Catch
            ' Fail silently
        End Try
    End Sub

    ' Get current user ID safely
    Private Shared Function GetCurrentUserId() As String
        Try
            If HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.Session IsNot Nothing Then
                Return If(HttpContext.Current.Session("userid"), "Anonymous").ToString()
            End If
        Catch
        End Try
        Return "Anonymous"
    End Function

    ' Get client IP address
    Private Shared Function GetClientIP() As String
        Try
            If HttpContext.Current IsNot Nothing Then
                Dim ip As String = HttpContext.Current.Request.ServerVariables("HTTP_X_FORWARDED_FOR")
                If String.IsNullOrEmpty(ip) Then
                    ip = HttpContext.Current.Request.ServerVariables("REMOTE_ADDR")
                End If
                Return ip
            End If
        Catch
        End Try
        Return "Unknown"
    End Function

    ' Sanitize log messages to prevent log injection
    Private Shared Function SanitizeLogMessage(message As String) As String
        If String.IsNullOrEmpty(message) Then
            Return String.Empty
        End If

        ' Remove or replace dangerous characters
        Dim sanitized As String = message.Replace(vbCrLf, " ").Replace(vbCr, " ").Replace(vbLf, " ")
        sanitized = Regex.Replace(sanitized, "[\x00-\x1F\x7F]", " ") ' Remove control characters

        ' Limit length
        If sanitized.Length > 500 Then
            sanitized = sanitized.Substring(0, 500) & "..."
        End If

        Return sanitized
    End Function

    ' Safe string truncation
    Public Shared Function SafeTruncate(input As String, maxLength As Integer) As String
        If String.IsNullOrEmpty(input) Then
            Return String.Empty
        End If

        If input.Length <= maxLength Then
            Return input
        End If

        Return input.Substring(0, maxLength)
    End Function

    ' Enhanced numeric validation
    Public Shared Function ValidateNumeric(input As String, minValue As Double, maxValue As Double) As Boolean
        If String.IsNullOrEmpty(input) Then
            Return False
        End If

        Dim numericValue As Double
        If Not Double.TryParse(input, numericValue) Then
            Return False
        End If

        Return numericValue >= minValue AndAlso numericValue <= maxValue
    End Function

    ' Generate secure random token
    Public Shared Function GenerateSecureToken(length As Integer) As String
        Using rng As New RNGCryptoServiceProvider()
            Dim bytes(length - 1) As Byte
            rng.GetBytes(bytes)
            Return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "")
        End Using
    End Function

    ' Validate file upload security
    Public Shared Function ValidateFileUpload(fileName As String, fileContent As Byte(), maxSizeBytes As Integer) As Boolean
        If String.IsNullOrEmpty(fileName) OrElse fileContent Is Nothing Then
            Return False
        End If

        ' Check file size
        If fileContent.Length > maxSizeBytes Then
            Return False
        End If

        ' Check file extension
        Dim allowedExtensions() As String = {".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx", ".xls", ".xlsx"}
        Dim extension As String = Path.GetExtension(fileName).ToLower()
        If Not allowedExtensions.Contains(extension) Then
            Return False
        End If

        ' Check for dangerous file signatures
        If HasDangerousFileSignature(fileContent) Then
            Return False
        End If

        Return True
    End Function

    ' Check for dangerous file signatures
    Private Shared Function HasDangerousFileSignature(fileContent As Byte()) As Boolean
        If fileContent.Length < 4 Then
            Return False
        End If

        ' Check for executable file signatures
        Dim dangerousSignatures()() As Byte = {
            New Byte() {&H4D, &H5A}, ' MZ (PE executable)
            New Byte() {&H50, &H4B}, ' PK (ZIP/Office documents - could contain macros)
            New Byte() {&H7F, &H45, &H4C, &H46} ' ELF executable
        }
        
        For Each Signature In dangerousSignatures
            If fileContent.Take(Signature.Length).SequenceEqual(Signature) Then
                Return True
            End If
        Next

        Return False
    End Function
    
    Public Shared Function HasRequiredRole(requiredRole As String) As Boolean
        Try
            Dim userRole As String = GetCurrentUserRole()
            If String.IsNullOrEmpty(userRole) Then
                Return False
            End If

            Select Case requiredRole.ToUpper()
                Case "ADMIN"
                    Return userRole.ToUpper() = "ADMIN"
                Case "SUPERUSER"
                    Return userRole.ToUpper() = "ADMIN" OrElse userRole.ToUpper() = "SUPERUSER"
                Case "OPERATOR"
                    Return userRole.ToUpper() = "ADMIN" OrElse userRole.ToUpper() = "SUPERUSER" OrElse userRole.ToUpper() = "OPERATOR"
                Case "USER"
                    Return True ' All authenticated users
                Case Else
                    Return False
            End Select
        Catch ex As Exception
            LogSecurityEvent("ROLE_CHECK_ERROR", ex.Message)
            Return False
        End Try
    End Function
    
    Public Shared Function GenerateCSRFToken() As String
        Try
            Dim token As String = Guid.NewGuid().ToString()
            HttpContext.Current.Session("CSRFToken") = token
            Return token
        Catch ex As Exception
            LogSecurityEvent("CSRF_TOKEN_GENERATION_ERROR", ex.Message)
            Return String.Empty
        End Try
    End Function
    
    ' Validate CSRF token
    Public Shared Function ValidateCSRFToken(submittedToken As String) As Boolean
        Try
            If String.IsNullOrEmpty(submittedToken) Then
                Return False
            End If

            Dim sessionToken As String = TryCast(HttpContext.Current.Session("CSRFToken"), String)
            If String.IsNullOrEmpty(sessionToken) Then
                Return False
            End If

            Return sessionToken.Equals(submittedToken, StringComparison.Ordinal)
        Catch ex As Exception
            LogSecurityEvent("CSRF_TOKEN_VALIDATION_ERROR", ex.Message)
            Return False
        End Try
    End Function
    
    ' Rate limiting check
    Public Shared Function CheckRateLimit(key As String, maxAttempts As Integer, timeWindow As TimeSpan) As Boolean
        Try
            Dim cacheKey As String = "RateLimit_" & key
            Dim attempts As Integer = 0

            If HttpContext.Current.Cache(cacheKey) IsNot Nothing Then
                attempts = CInt(HttpContext.Current.Cache(cacheKey))
            End If

            If attempts >= maxAttempts Then
                Return False
            End If

            attempts += 1
            HttpContext.Current.Cache.Insert(cacheKey, attempts, Nothing, DateTime.Now.Add(timeWindow), TimeSpan.Zero)

            Return True
        Catch ex As Exception
            LogSecurityEvent("RATE_LIMIT_ERROR", ex.Message)
            Return True ' Allow on error to prevent blocking legitimate users
        End Try
    End Function
    
    ' Validate users list format
    Public Shared Function ValidateUsersList(usersList As String) As Boolean
        Try
            If String.IsNullOrEmpty(usersList) Then
                Return False
            End If

            ' Remove quotes and validate format
            usersList = usersList.Replace("'", "").Replace(" ", "")

            If Not Regex.IsMatch(usersList, "^[0-9,]+$") Then
                Return False
            End If

            ' Validate each user ID
            Dim users As String() = usersList.Split(","c)
            For Each user As String In users
                If Not String.IsNullOrEmpty(user) Then
                    Dim userId As Integer
                    If Not Integer.TryParse(user, userId) OrElse userId <= 0 Then
                        Return False
                    End If
                End If
            Next

            Return True
        Catch ex As Exception
            LogSecurityEvent("USERS_LIST_VALIDATION_ERROR", ex.Message)
            Return False
        End Try
    End Function

    ' Get current user role safely
    Private Shared Function GetCurrentUserRole() As String
        Try
            If HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.Session IsNot Nothing Then
                Return If(HttpContext.Current.Session("role"), "").ToString()
            End If
        Catch
        End Try
        Return ""
    End Function

    ' Validate session
    Public Shared Function ValidateSession() As Boolean
        Try
            Return SessionManager.ValidateSession()
        Catch
            Return False
        End Try
    End Function

    ' Create secure database parameters
    Public Shared Function CreateSecureParameters(paramDict As Dictionary(Of String, Object)) As SqlParameter()
        Dim parameters As New List(Of SqlParameter)
        
        For Each kvp In paramDict
            Dim param As New SqlParameter(kvp.Key, If(kvp.Value, DBNull.Value))
            parameters.Add(param)
        Next
        
        Return parameters.ToArray()
    End Function

    ' Execute secure query
    Public Shared Function ExecuteSecureQuery(query As String, parameters As Dictionary(Of String, Object)) As DataTable
        Try
            Return DatabaseHelper.ExecuteQuery(query, parameters)
        Catch ex As Exception
            LogSecurityEvent("SECURE_QUERY_ERROR", ex.Message)
            Throw New SecurityException("Database operation failed")
        End Try
    End Function

    ' Execute secure non-query
    Public Shared Function ExecuteSecureNonQuery(query As String, parameters As Dictionary(Of String, Object)) As Integer
        Try
            Return DatabaseHelper.ExecuteNonQuery(query, parameters)
        Catch ex As Exception
            LogSecurityEvent("SECURE_NONQUERY_ERROR", ex.Message)
            Throw New SecurityException("Database operation failed")
        End Try
    End Function
End Class