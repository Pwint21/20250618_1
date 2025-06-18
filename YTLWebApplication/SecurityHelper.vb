Imports System.Data.SqlClient
Imports System.Text.RegularExpressions
Imports System.Web.Security

Public Class SecurityHelper
    
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
    
    ' SECURITY FIX: User ID validation
    Public Shared Function ValidateUserId(userId As String) As Boolean
        Dim userIdInt As Integer
        Return Integer.TryParse(userId, userIdInt) AndAlso userIdInt > 0
    End Function
    
    ' SECURITY FIX: Role validation
    Public Shared Function ValidateUserRole(role As String) As Boolean
        Dim allowedRoles As String() = {"Admin", "SuperUser", "Operator", "User"}
        Return allowedRoles.Contains(role)
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
    
End Class