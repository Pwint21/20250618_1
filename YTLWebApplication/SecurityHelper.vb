Imports System.Data.SqlClient
Imports System.Text.RegularExpressions
Imports System.Web.Security
Imports System.IO

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
    
End Class