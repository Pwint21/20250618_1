Imports System.Data.SqlClient
Imports Newtonsoft.Json

Partial Class updatepassword
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Dim errorResponse As String = "{""d"":""Unauthorized""}"
                Response.ContentType = "application/json"
                Response.Write(errorResponse)
                Return
            End If

            Dim opr As String = Request.QueryString("opr")
            Dim pwd As String = Request.QueryString("pwd")
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)

            ' SECURITY FIX: Validate operation parameter
            If String.IsNullOrEmpty(opr) OrElse (opr <> "check" AndAlso opr <> "change") Then
                Dim errorResponse As String = "{""d"":""Invalid operation""}"
                Response.ContentType = "application/json"
                Response.Write(errorResponse)
                Return
            End If

            If opr = "check" Then
                checkpassword(userid)
            ElseIf opr = "change" Then
                ' SECURITY FIX: Validate password
                If String.IsNullOrEmpty(pwd) OrElse pwd.Length > 100 Then
                    Dim errorResponse As String = "{""d"":""Invalid password""}"
                    Response.ContentType = "application/json"
                    Response.Write(errorResponse)
                    Return
                End If

                ' SECURITY FIX: Check for dangerous patterns in password
                If SecurityHelper.ContainsDangerousPatterns(pwd) Then
                    Dim errorResponse As String = "{""d"":""Invalid password format""}"
                    Response.ContentType = "application/json"
                    Response.Write(errorResponse)
                    SecurityHelper.LogSecurityEvent("DANGEROUS_PASSWORD_ATTEMPT", "Dangerous patterns detected in password change")
                    Return
                End If

                updatepassword(userid, pwd)
            End If
        Catch ex As Exception
            SecurityHelper.LogError("updatepassword Page_Load Error", ex, Server)
            Dim errorResponse As String = "{""d"":""Error occurred""}"
            Response.ContentType = "application/json"
            Response.Write(errorResponse)
        End Try
    End Sub

    Protected Sub updatepassword(ByVal userid As String, ByVal pwd As String)
        Dim res As Integer = 0
        Dim json As String
        
        Try
            ' SECURITY FIX: Validate password strength
            If Not PasswordHelper.ValidatePasswordStrength(pwd) Then
                res = 4 ' Weak password
                json = "{""d"":" & JsonConvert.SerializeObject(res, Formatting.None) & "}"
                Response.ContentType = "application/json"
                Response.Write(json)
                Return
            End If

            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
                ' SECURITY FIX: Check current password first
                Dim checkQuery As String = "SELECT pwd, password_hash FROM userTBL WHERE userid = @userid"
                Using checkCmd As New SqlCommand(checkQuery, conn)
                    checkCmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@userid", userid, SqlDbType.Int))
                    
                    conn.Open()
                    Using dr As SqlDataReader = checkCmd.ExecuteReader()
                        If dr.Read() Then
                            Dim currentPwd As String = If(IsDBNull(dr("pwd")), "", dr("pwd").ToString())
                            Dim currentHash As String = If(IsDBNull(dr("password_hash")), "", dr("password_hash").ToString())
                            
                            ' Check if new password is same as current
                            Dim isSamePassword As Boolean = False
                            If Not String.IsNullOrEmpty(currentHash) Then
                                isSamePassword = PasswordHelper.VerifyPassword(pwd, currentHash)
                            Else
                                isSamePassword = String.Equals(pwd, currentPwd, StringComparison.OrdinalIgnoreCase)
                            End If
                            
                            If isSamePassword Then
                                res = 2 ' Same password
                            Else
                                dr.Close()
                                
                                ' SECURITY FIX: Hash the new password
                                Dim hashedPassword As String = PasswordHelper.HashPassword(pwd)
                                
                                ' Update with hashed password
                                Dim updateQuery As String = "UPDATE userTBL SET pwd = @pwd, password_hash = @password_hash, pwdstatus = 1 WHERE userid = @userid"
                                Using updateCmd As New SqlCommand(updateQuery, conn)
                                    updateCmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@pwd", pwd, SqlDbType.VarChar))
                                    updateCmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@password_hash", hashedPassword, SqlDbType.VarChar))
                                    updateCmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@userid", userid, SqlDbType.Int))
                                    
                                    res = updateCmd.ExecuteNonQuery()
                                    If res > 0 Then
                                        SecurityHelper.LogSecurityEvent("PASSWORD_CHANGED", $"Password changed for user {userid}")
                                    End If
                                End Using
                            End If
                        Else
                            res = 3 ' User not found
                        End If
                    End Using
                End Using
            End Using

        Catch ex As Exception
            SecurityHelper.LogError("updatepassword Error", ex, Server)
            res = 0 ' Error
        End Try
        
        json = "{""d"":" & JsonConvert.SerializeObject(res, Formatting.None) & "}"
        Response.ContentType = "application/json"
        Response.Write(json)
    End Sub

    Protected Sub checkpassword(ByVal userid As String)
        Dim res As Integer = 0
        Dim json As String
        
        Try
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
                Dim query As String = "SELECT pwdstatus FROM userTBL WHERE userid = @userid"
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@userid", userid, SqlDbType.Int))
                    
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        If dr.Read() Then
                            If Not IsDBNull(dr("pwdstatus")) AndAlso CBool(dr("pwdstatus")) Then
                                res = 1
                            End If
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            SecurityHelper.LogError("checkpassword Error", ex, Server)
            res = 0
        End Try
        
        json = "{""d"":" & JsonConvert.SerializeObject(res, Formatting.None) & "}"
        Response.ContentType = "application/json"
        Response.Write(json)
    End Sub

End Class