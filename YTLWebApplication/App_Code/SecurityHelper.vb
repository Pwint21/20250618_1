Imports System.Data.SqlClient
Imports System.Text.RegularExpressions
Imports System.Web.Security
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text

Public Class SecurityHelper
    
    ' SECURITY FIX: User session validation
    Public Shared Function ValidateUserSession(request As HttpRequest, session As HttpSessionState) As Boolean
        Try
            If request.Cookies("userinfo") Is Nothing Then
                Return False
            End If
            
            Dim userid As String = request.Cookies("userinfo")("userid")
            Dim role As String = request.Cookies("userinfo")("role")
            
            ' Validate userid is numeric
            Dim userIdInt As Integer
            If Not Integer.TryParse(userid, userIdInt) OrElse userIdInt <= 0 Then
                Return False
            End If
            
            ' Validate role is in allowed list
            If Not ValidateUserRole(role) Then
                Return False
            End If
            
            Return True
        Catch
            Return False
        End Try
    End Function
    
    ' SECURITY FIX: Input validation methods
    Public Shared Function ValidateInput(input As String, maxLength As Integer, allowedPattern As String) As Boolean
        If String.IsNullOrEmpty(input) Then
            Return False
        End If
        
        If input.Length > maxLength Then
            Return False
        End If
        
        If Not String.IsNullOrEmpty(allowedPattern) Then
            Dim regex As New Regex(allowedPattern)
            If Not regex.IsMatch(input) Then
                Return False
            End If
        End If
        
        Return True
    End Function
    
    ' SECURITY FIX: SQL parameter helper
    Public Shared Function CreateSqlParameter(parameterName As String, value As Object, sqlDbType As SqlDbType) As SqlParameter
        Dim parameter As New SqlParameter(parameterName, sqlDbType)
        parameter.Value = If(value, DBNull.Value)
        Return parameter
    End Function
    
    ' SECURITY FIX: HTML encoding helper
    Public Shared Function HtmlEncode(input As String) As String
        If String.IsNullOrEmpty(input) Then
            Return String.Empty
        End If
        Return HttpUtility.HtmlEncode(input)
    End Function
    
    ' SECURITY FIX: User ID validation and retrieval
    Public Shared Function ValidateAndGetUserId(request As HttpRequest) As String
        Dim userid As String = request.Cookies("userinfo")("userid")
        If ValidateUserId(userid) Then
            Return userid
        End If
        Throw New SecurityException("Invalid user ID")
    End Function
    
    Public Shared Function ValidateUserId(userId As String) As Boolean
        Dim userIdInt As Integer
        Return Integer.TryParse(userId, userIdInt) AndAlso userIdInt > 0
    End Function
    
    ' SECURITY FIX: Role validation and retrieval
    Public Shared Function ValidateAndGetUserRole(request As HttpRequest) As String
        Dim role As String = request.Cookies("userinfo")("role")
        If ValidateUserRole(role) Then
            Return role
        End If
        Throw New SecurityException("Invalid user role")
    End Function
    
    Public Shared Function ValidateUserRole(role As String) As Boolean
        Dim allowedRoles As String() = {"Admin", "SuperUser", "Operator", "User"}
        Return allowedRoles.Contains(role)
    End Function
    
    ' SECURITY FIX: Users list validation and retrieval
    Public Shared Function ValidateAndGetUsersList(request As HttpRequest) As String
        Dim userslist As String = request.Cookies("userinfo")("userslist")
        If IsValidUsersList(userslist) Then
            Return userslist
        End If
        Return String.Empty ' Return empty string instead of throwing exception for optional field
    End Function
    
    Public Shared Function IsValidUsersList(usersList As String) As Boolean
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
    
    ' SECURITY FIX: Date validation
    Public Shared Function ValidateDate(dateString As String) As Boolean
        Dim dateValue As DateTime
        Return DateTime.TryParse(dateString, dateValue)
    End Function
    
    ' SECURITY FIX: Plate number validation
    Public Shared Function ValidatePlateNumber(plateNumber As String) As Boolean
        If String.IsNullOrEmpty(plateNumber) Then
            Return False
        End If
        
        ' Allow alphanumeric characters and common plate number formats
        Dim pattern As String = "^[A-Za-z0-9\-\s]{1,15}$"
        Dim regex As New Regex(pattern)
        Return regex.IsMatch(plateNumber)
    End Function
    
    ' SECURITY FIX: Coordinate validation
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
        
        Return True
    End Function
    
    ' SECURITY FIX: Dangerous pattern detection
    Public Shared Function ContainsDangerousPatterns(input As String) As Boolean
        If String.IsNullOrEmpty(input) Then
            Return False
        End If
        
        Dim dangerousPatterns() As String = {
            "<script", "</script>", "javascript:", "vbscript:", "onload=", "onerror=",
            "eval\(", "expression\(", "url\(", "import\(", "document\.", "window\.",
            "alert\(", "confirm\(", "prompt\(", "setTimeout\(", "setInterval\(",
            "union.*select", "insert.*into", "update.*set", "delete.*from",
            "drop.*table", "create.*table", "alter.*table", "exec.*xp_",
            "sp_oacreate", "sp_oamethod", "openrowset", "opendatasource",
            "xp_cmdshell", "bulk.*insert", "truncate.*table", "--", "/*", "*/"
        }
        
        Dim inputLower As String = input.ToLower()
        For Each pattern As String In dangerousPatterns
            If Regex.IsMatch(inputLower, pattern, RegexOptions.IgnoreCase) Then
                Return True
            End If
        Next
        
        Return False
    End Function
    
    ' SECURITY FIX: Error logging
    Public Shared Sub LogError(message As String, ex As Exception, server As HttpServerUtility)
        Try
            Dim logPath As String = server.MapPath("~/Logs/ErrorLog.txt")
            Dim logEntry As String = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} - {message}: {ex.Message}{Environment.NewLine}"
            
            ' Ensure logs directory exists
            Dim logDir As String = Path.GetDirectoryName(logPath)
            If Not Directory.Exists(logDir) Then
                Directory.CreateDirectory(logDir)
            End If
            
            File.AppendAllText(logPath, logEntry)
        Catch
            ' Fail silently if logging fails
        End Try
    End Sub
    
    ' SECURITY FIX: Security event logging
    Public Shared Sub LogSecurityEvent(message As String)
        Try
            Dim logPath As String = HttpContext.Current.Server.MapPath("~/Logs/SecurityLog.txt")
            Dim logEntry As String = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} - SECURITY: {message} - IP: {HttpContext.Current.Request.UserHostAddress}{Environment.NewLine}"
            
            ' Ensure logs directory exists
            Dim logDir As String = Path.GetDirectoryName(logPath)
            If Not Directory.Exists(logDir) Then
                Directory.CreateDirectory(logDir)
            End If
            
            File.AppendAllText(logPath, logEntry)
        Catch
            ' Fail silently if logging fails
        End Try
    End Sub
    
    ' SECURITY FIX: Safe string truncation
    Public Shared Function SafeTruncate(input As String, maxLength As Integer) As String
        If String.IsNullOrEmpty(input) Then
            Return String.Empty
        End If
        
        If input.Length <= maxLength Then
            Return input
        End If
        
        Return input.Substring(0, maxLength)
    End Function
    
    ' SECURITY FIX: Numeric validation
    Public Shared Function ValidateNumeric(input As String, minValue As Double, maxValue As Double) As Boolean
        Dim numericValue As Double
        If Not Double.TryParse(input, numericValue) Then
            Return False
        End If
        
        Return numericValue >= minValue AndAlso numericValue <= maxValue
    End Function
    
    ' SECURITY FIX: CSRF token generation and validation
    Public Shared Function GenerateCSRFToken() As String
        Using rng As New RNGCryptoServiceProvider()
            Dim tokenBytes(31) As Byte
            rng.GetBytes(tokenBytes)
            Return Convert.ToBase64String(tokenBytes)
        End Using
    End Function
    
    Public Shared Function ValidateCSRFToken(sessionToken As String, requestToken As String) As Boolean
        Return Not String.IsNullOrEmpty(sessionToken) AndAlso 
               Not String.IsNullOrEmpty(requestToken) AndAlso 
               sessionToken.Equals(requestToken, StringComparison.Ordinal)
    End Function
    
    ' SECURITY FIX: Rate limiting helpers
    Public Shared Function IsRateLimited(identifier As String, maxRequests As Integer, timeWindowMinutes As Integer) As Boolean
        Try
            Dim cacheKey As String = $"RateLimit_{identifier}"
            Dim requestCount As Integer = 0
            
            If HttpContext.Current.Cache(cacheKey) IsNot Nothing Then
                requestCount = CInt(HttpContext.Current.Cache(cacheKey))
            End If
            
            If requestCount = 0 Then
                HttpContext.Current.Cache.Insert(cacheKey, 1, Nothing, DateTime.Now.AddMinutes(timeWindowMinutes), TimeSpan.Zero)
                Return False
            Else
                requestCount += 1
                HttpContext.Current.Cache.Insert(cacheKey, requestCount, Nothing, DateTime.Now.AddMinutes(timeWindowMinutes), TimeSpan.Zero)
                Return requestCount > maxRequests
            End If
        Catch
            Return False
        End Try
    End Function
    
    ' SECURITY FIX: File path validation
    Public Shared Function ValidateFilePath(filePath As String) As Boolean
        If String.IsNullOrEmpty(filePath) Then
            Return False
        End If
        
        ' Check for directory traversal attempts
        If filePath.Contains("..") OrElse filePath.Contains("~") Then
            Return False
        End If
        
        ' Check for invalid characters
        Dim invalidChars() As Char = Path.GetInvalidPathChars()
        For Each invalidChar As Char In invalidChars
            If filePath.Contains(invalidChar) Then
                Return False
            End If
        Next
        
        Return True
    End Function
    
    ' SECURITY FIX: URL validation
    Public Shared Function ValidateURL(url As String) As Boolean
        If String.IsNullOrEmpty(url) Then
            Return False
        End If
        
        Try
            Dim uri As New Uri(url)
            Return uri.Scheme = "http" OrElse uri.Scheme = "https"
        Catch
            Return False
        End Try
    End Function
    
End Class