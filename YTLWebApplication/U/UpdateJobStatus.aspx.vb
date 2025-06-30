Imports System
Imports System.Data
Imports System.Data.SqlClient

Partial Class UpdateJobStatus
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

            ' SECURITY FIX: Validate input parameters
            If String.IsNullOrEmpty(Patch_No) OrElse String.IsNullOrEmpty(status_code) Then
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

            ' SECURITY FIX: Validate status code
            If Not SecurityHelper.ValidateNumeric(status_code, 0, 10) Then
                Response.Write("Invalid status code")
                Response.ContentType = "text/plain"
                Response.End()
                Return
            End If

            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("OSSConnection").ConnectionString)
                Try
                    Dim query As String = "UPDATE OSS_EXTENSION_TABLE SET UploadStatus = @UploadStatus WHERE patch_no = @patch_no"
                    Using cmd As New SqlCommand(query, conn)
                        cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@UploadStatus", status_code, SqlDbType.VarChar))
                        cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@patch_no", Patch_No, SqlDbType.VarChar))
                        
                        conn.Open()
                        If cmd.ExecuteNonQuery() > 0 Then
                            Result = "Yes"
                            SecurityHelper.LogSecurityEvent("JOB_STATUS_UPDATED", $"Job status updated: {Patch_No} - Status: {status_code}")
                        End If
                    End Using
                Catch ex As Exception
                    SecurityHelper.LogError("UpdateJobStatus Error", ex, Server)
                    Result = "Error"
                End Try
            End Using

            Response.Write(Result)
            Response.ContentType = "text/plain"
            
        Catch ex As Exception
            SecurityHelper.LogError("UpdateJobStatus Page_Load Error", ex, Server)
            Response.Write("Error")
            Response.ContentType = "text/plain"
        End Try
    End Sub
End Class