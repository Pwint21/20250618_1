Imports System
Imports System.Data
Imports System.Data.SqlClient

Partial Class UpdateJobStatusNew
    Inherits System.Web.UI.Page
    
    Private Sub UpdateJobStatus_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Write("Unauthorized")
                Response.ContentType = "text/plain"
                Response.End()
                Return
            End If

            ' SECURITY FIX: Check user permissions
            Dim userRole As String = SecurityHelper.ValidateAndGetUserRole(Request)
            If userRole <> "Admin" AndAlso userRole <> "SuperUser" Then
                SecurityHelper.LogSecurityEvent("UNAUTHORIZED_JOB_STATUS_UPDATE", "User attempted to update job status without permission")
                Response.Write("Insufficient permissions")
                Response.ContentType = "text/plain"
                Response.End()
                Return
            End If

            Dim Result As String = "No"
            Dim Patch_No As String = Request.QueryString("p")
            Dim status_code As String = Request.QueryString("i")
            Dim remarks As String = Request.QueryString("rem")
            Dim reason As String = Request.QueryString("rdb")
            Dim DivStatus As String = Request.QueryString("d")

            ' SECURITY FIX: Validate input parameters
            If String.IsNullOrEmpty(Patch_No) OrElse String.IsNullOrEmpty(status_code) OrElse String.IsNullOrEmpty(DivStatus) Then
                Response.Write("Invalid parameters")
                Response.ContentType = "text/plain"
                Response.End()
                Return
            End If

            ' SECURITY FIX: Validate patch number format
            If Not Regex.IsMatch(Patch_No, "^[A-Za-z0-9\-_]{1,50}$") Then
                Response.Write("Invalid patch number format")
                Response.ContentType = "text/plain"
                SecurityHelper.LogSecurityEvent("INVALID_PATCH_NUMBER", $"Invalid patch number format: {Patch_No}")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Validate status codes
            If Not SecurityHelper.ValidateNumeric(status_code, 0, 10) OrElse Not SecurityHelper.ValidateNumeric(DivStatus, 0, 10) Then
                Response.Write("Invalid status codes")
                Response.ContentType = "text/plain"
                Response.End()
                Return
            End If

            ' SECURITY FIX: Validate and sanitize remarks and reason
            If Not String.IsNullOrEmpty(remarks) Then
                If remarks.Length > 500 Then
                    remarks = SecurityHelper.SafeTruncate(remarks, 500)
                End If
                If SecurityHelper.ContainsDangerousPatterns(remarks) Then
                    Response.Write("Invalid remarks format")
                    Response.ContentType = "text/plain"
                    SecurityHelper.LogSecurityEvent("DANGEROUS_INPUT_JOB_REMARKS", $"Dangerous patterns in job remarks for {Patch_No}")
                    Response.End()
                    Return
                End If
            End If

            If Not String.IsNullOrEmpty(reason) Then
                If reason.Length > 200 Then
                    reason = SecurityHelper.SafeTruncate(reason, 200)
                End If
                If SecurityHelper.ContainsDangerousPatterns(reason) Then
                    Response.Write("Invalid reason format")
                    Response.ContentType = "text/plain"
                    SecurityHelper.LogSecurityEvent("DANGEROUS_INPUT_JOB_REASON", $"Dangerous patterns in job reason for {Patch_No}")
                    Response.End()
                    Return
                End If
            End If

            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("OSSConnection").ConnectionString)
                Try
                    Dim query As String
                    Dim cmd As SqlCommand

                    If status_code = "3" Then
                        query = "UPDATE OSS_EXTENSION_TABLE SET DivStatus = @DivStatus, UploadStatus = @UploadStatus, remarks = @remarks, reason = @reason WHERE patch_no = @patch_no"
                        cmd = New SqlCommand(query, conn)
                        cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@remarks", If(String.IsNullOrEmpty(remarks), DBNull.Value, remarks), SqlDbType.VarChar))
                        cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@reason", If(String.IsNullOrEmpty(reason), DBNull.Value, reason), SqlDbType.VarChar))
                    Else
                        query = "UPDATE OSS_EXTENSION_TABLE SET DivStatus = @DivStatus, UploadStatus = @UploadStatus WHERE patch_no = @patch_no"
                        cmd = New SqlCommand(query, conn)
                    End If

                    cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@DivStatus", Convert.ToInt32(DivStatus), SqlDbType.Int))
                    cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@UploadStatus", status_code, SqlDbType.VarChar))
                    cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@patch_no", Patch_No, SqlDbType.VarChar))

                    conn.Open()
                    If cmd.ExecuteNonQuery() > 0 Then
                        Result = "Yes"
                        SecurityHelper.LogSecurityEvent("JOB_STATUS_UPDATED_NEW", $"Job status updated: {Patch_No} - Status: {status_code} - DivStatus: {DivStatus}")
                    End If
                    
                Catch ex As Exception
                    SecurityHelper.LogError("UpdateJobStatusNew Error", ex, Server)
                    Result = "Error"
                End Try
            End Using

            Response.Write(Result)
            Response.ContentType = "text/plain"
            
        Catch ex As Exception
            SecurityHelper.LogError("UpdateJobStatusNew Page_Load Error", ex, Server)
            Response.Write("Error")
            Response.ContentType = "text/plain"
        End Try
    End Sub
End Class