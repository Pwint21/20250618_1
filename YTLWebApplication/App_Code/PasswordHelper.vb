Imports System.Security.Cryptography
Imports System.Text

Public Class PasswordHelper
    Private Const SALT_SIZE As Integer = 32
    Private Const HASH_SIZE As Integer = 32
    Private Const ITERATIONS As Integer = 10000

    ' Hash password using PBKDF2 with salt
    Public Shared Function HashPassword(password As String) As String
        Try
            ' Generate salt
            Dim salt(SALT_SIZE - 1) As Byte
            Using rng As New RNGCryptoServiceProvider()
                rng.GetBytes(salt)
            End Using

            ' Hash password with salt
            Using pbkdf2 As New Rfc2898DeriveBytes(password, salt, ITERATIONS)
                Dim hash() As Byte = pbkdf2.GetBytes(HASH_SIZE)
                
                ' Combine salt and hash
                Dim hashBytes(SALT_SIZE + HASH_SIZE - 1) As Byte
                Array.Copy(salt, 0, hashBytes, 0, SALT_SIZE)
                Array.Copy(hash, 0, hashBytes, SALT_SIZE, HASH_SIZE)
                
                Return Convert.ToBase64String(hashBytes)
            End Using
        Catch ex As Exception
            Throw New CryptographicException("Password hashing failed", ex)
        End Try
    End Function

    ' Verify password against hash
    Public Shared Function VerifyPassword(password As String, hashedPassword As String) As Boolean
        Try
            ' Extract salt and hash from stored password
            Dim hashBytes() As Byte = Convert.FromBase64String(hashedPassword)
            
            If hashBytes.Length <> SALT_SIZE + HASH_SIZE Then
                Return False
            End If
            
            ' Extract salt
            Dim salt(SALT_SIZE - 1) As Byte
            Array.Copy(hashBytes, 0, salt, 0, SALT_SIZE)
            
            ' Extract hash
            Dim storedHash(HASH_SIZE - 1) As Byte
            Array.Copy(hashBytes, SALT_SIZE, storedHash, 0, HASH_SIZE)
            
            ' Hash the provided password with the extracted salt
            Using pbkdf2 As New Rfc2898DeriveBytes(password, salt, ITERATIONS)
                Dim testHash() As Byte = pbkdf2.GetBytes(HASH_SIZE)
                
                ' Compare hashes
                Return SlowEquals(storedHash, testHash)
            End Using
        Catch
            Return False
        End Try
    End Function

    ' Constant-time comparison to prevent timing attacks
    Private Shared Function SlowEquals(a() As Byte, b() As Byte) As Boolean
        Dim diff As UInteger = CUInt(a.Length) Xor CUInt(b.Length)
        For i As Integer = 0 To a.Length - 1 And b.Length - 1
            diff = diff Or (CUInt(a(i)) Xor CUInt(b(i)))
        Next
        Return diff = 0
    End Function

    ' Generate secure random password
    Public Shared Function GenerateSecurePassword(length As Integer) As String
        Const chars As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*"
        Dim result As New StringBuilder()
        
        Using rng As New RNGCryptoServiceProvider()
            Dim bytes(3) As Byte
            For i As Integer = 0 To length - 1
                rng.GetBytes(bytes)
                Dim value As UInteger = BitConverter.ToUInt32(bytes, 0)
                result.Append(chars(CInt(value Mod CUInt(chars.Length))))
            Next
        End Using
        
        Return result.ToString()
    End Function

    ' Validate password strength
    Public Shared Function ValidatePasswordStrength(password As String) As Boolean
        If String.IsNullOrEmpty(password) OrElse password.Length < 8 Then
            Return False
        End If

        Dim hasUpper As Boolean = False
        Dim hasLower As Boolean = False
        Dim hasDigit As Boolean = False
        Dim hasSpecial As Boolean = False

        For Each c As Char In password
            If Char.IsUpper(c) Then hasUpper = True
            If Char.IsLower(c) Then hasLower = True
            If Char.IsDigit(c) Then hasDigit = True
            If "!@#$%^&*()_+-=[]{}|;:,.<>?".Contains(c) Then hasSpecial = True
        Next

        Return hasUpper AndAlso hasLower AndAlso hasDigit AndAlso hasSpecial
    End Function
End Class