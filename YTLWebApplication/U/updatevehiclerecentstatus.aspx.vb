Imports System.Data.SqlClient

Partial Class updatevehiclerecentstatus
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Write("Unauthorized")
                Response.ContentType = "text/plain"
                Response.End()
                Return
            End If

            ' SECURITY FIX: Validate input parameters
            Dim pno As String = Request.QueryString("pno")
            Dim vtype As String = Request.QueryString("type")
            Dim re As String = Request.QueryString("re")
            Dim status As String = Request.QueryString("status")

            If Not SecurityHelper.ValidatePlateNumber(pno) Then
                Response.Write("Invalid plate number")
                Response.ContentType = "text/plain"
                Response.End()
                Return
            End If

            If String.IsNullOrEmpty(vtype) OrElse (vtype <> "1" AndAlso vtype <> "0") Then
                Response.Write("Invalid type")
                Response.ContentType = "text/plain"
                Response.End()
                Return
            End If

            ' SECURITY FIX: Validate remarks and status
            If Not String.IsNullOrEmpty(re) AndAlso re.Length > 500 Then
                re = SecurityHelper.SafeTruncate(re, 500)
            End If

            If Not String.IsNullOrEmpty(status) AndAlso status.Length > 100 Then
                status = SecurityHelper.SafeTruncate(status, 100)
            End If

            ' SECURITY FIX: Check for dangerous patterns
            If SecurityHelper.ContainsDangerousPatterns(re) OrElse SecurityHelper.ContainsDangerousPatterns(status) Then
                Response.Write("Invalid input detected")
                Response.ContentType = "text/plain"
                SecurityHelper.LogSecurityEvent("DANGEROUS_INPUT_VEHICLE_STATUS", $"Dangerous patterns in vehicle status update for {pno}")
                Response.End()
                Return
            End If

            Dim r As String = "0"
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
                Try
                    conn.Open()
                    
                    If vtype = "1" Then
                        ' Delete operation
                        Dim deleteQuery As String = "DELETE FROM vehicle_status_tracked2 WHERE plateno = @plateno"
                        Using cmd As New SqlCommand(deleteQuery, conn)
                            cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@plateno", pno, SqlDbType.VarChar))
                            cmd.ExecuteNonQuery()
                            r = "1"
                            SecurityHelper.LogSecurityEvent("VEHICLE_STATUS_DELETED", $"Vehicle status deleted for plate: {pno}")
                        End Using
                    Else
                        ' Insert operation using stored procedure
                        Using cmd As New SqlCommand("sp_InsertVehicleTrack", conn)
                            cmd.CommandType = CommandType.StoredProcedure
                            cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@plateno", pno, SqlDbType.VarChar))
                            cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@remarks", If(String.IsNullOrEmpty(re), DBNull.Value, re), SqlDbType.VarChar))
                            cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@status", If(String.IsNullOrEmpty(status), DBNull.Value, status), SqlDbType.VarChar))
                            
                            Using dr As SqlDataReader = cmd.ExecuteReader()
                                If dr.Read() Then
                                    If dr("result").ToString() = "1" Then
                                        r = "1"
                                        SecurityHelper.LogSecurityEvent("VEHICLE_STATUS_UPDATED", $"Vehicle status updated for plate: {pno}")
                                    Else
                                        r = "0"
                                    End If
                                End If
                            End Using
                        End Using
                    End If
                    
                Catch ex As Exception
                    SecurityHelper.LogError("updatevehiclerecentstatus Error", ex, Server)
                    r = "Error occurred"
                End Try
            End Using

            Response.Write(r)
            Response.ContentType = "text/plain"
            
        Catch ex As Exception
            SecurityHelper.LogError("updatevehiclerecentstatus Page_Load Error", ex, Server)
            Response.Write("Error occurred")
            Response.ContentType = "text/plain"
        End Try
    End Sub
End Class