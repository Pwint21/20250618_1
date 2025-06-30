Imports System.Data.SqlClient

Partial Class updatevehiclestatus
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Write("0")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Validate input parameters
            Dim s As String = Request.QueryString("s")
            Dim pno As String = Request.QueryString("pno")
            Dim u As String = Request.QueryString("u")
            Dim re As String = Request.QueryString("re")

            If Not SecurityHelper.ValidatePlateNumber(pno) Then
                Response.Write("0")
                Response.End()
                Return
            End If

            If Not SecurityHelper.ValidateUserId(u) Then
                Response.Write("0")
                Response.End()
                Return
            End If

            If String.IsNullOrEmpty(s) OrElse s = "Select Status" Then
                Response.Write("0")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Validate status value
            Dim allowedStatuses() As String = {"Active", "Inactive", "Maintenance", "Repair", "Out of Service"}
            If Not allowedStatuses.Contains(s) Then
                Response.Write("0")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Validate and sanitize remarks
            If Not String.IsNullOrEmpty(re) Then
                If re.Length > 500 Then
                    re = SecurityHelper.SafeTruncate(re, 500)
                End If

                If SecurityHelper.ContainsDangerousPatterns(re) Then
                    Response.Write("0")
                    SecurityHelper.LogSecurityEvent("DANGEROUS_INPUT_MAINTENANCE", $"Dangerous patterns in maintenance remarks for {pno}")
                    Response.End()
                    Return
                End If
            End If

            Dim r As Integer = 0
            
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
                Try
                    ' Get username
                    Dim uname As String = ""
                    Dim userQuery As String = "SELECT username FROM userTBL WHERE userid = @userid"
                    Using cmd1 As New SqlCommand(userQuery, conn)
                        cmd1.Parameters.Add(SecurityHelper.CreateSqlParameter("@userid", u, SqlDbType.Int))
                        
                        conn.Open()
                        Using dr As SqlDataReader = cmd1.ExecuteReader()
                            If dr.Read() Then
                                uname = dr("username").ToString()
                            End If
                        End Using
                    End Using

                    If Not String.IsNullOrEmpty(uname) Then
                        Dim currentDateTime As String = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                        
                        ' Insert maintenance record
                        Dim insertQuery As String = "INSERT INTO maintenance (statusdate, timestamp, status, plateno, sourcename, officeremark) VALUES (@statusdate, @timestamp, @status, @plateno, @sourcename, @officeremark)"
                        Using cmd As New SqlCommand(insertQuery, conn)
                            cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@statusdate", currentDateTime, SqlDbType.DateTime))
                            cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@timestamp", currentDateTime, SqlDbType.DateTime))
                            cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@status", s, SqlDbType.VarChar))
                            cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@plateno", pno, SqlDbType.VarChar))
                            cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@sourcename", uname, SqlDbType.VarChar))
                            cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@officeremark", If(String.IsNullOrEmpty(re), DBNull.Value, re), SqlDbType.VarChar))
                            
                            r = cmd.ExecuteNonQuery()
                            
                            If r > 0 Then
                                SecurityHelper.LogSecurityEvent("VEHICLE_STATUS_UPDATED", $"Vehicle status updated: {pno} - {s} by {uname}")
                            End If
                        End Using
                    End If
                    
                Catch ex As Exception
                    SecurityHelper.LogError("updatevehiclestatus Error", ex, Server)
                    r = 0
                Finally
                    If conn.State = ConnectionState.Open Then
                        conn.Close()
                    End If
                End Try
            End Using

            Response.Write(r.ToString())
            
        Catch ex As Exception
            SecurityHelper.LogError("updatevehiclestatus Page_Load Error", ex, Server)
            Response.Write("0")
        End Try
    End Sub
End Class