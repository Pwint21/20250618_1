Imports System
Imports System.Data
Imports System.Data.SqlClient

Partial Class UpdateJobStatusNew
    Inherits System.Web.UI.Page
    Private Sub UpdateJobStatus_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            Dim Result As String = "No"
            Dim Patch_No As String = Request.QueryString("p")
            Dim status_code As String = Request.QueryString("i")

            Dim remarks As String = Request.QueryString("rem")
            Dim reason As String = Request.QueryString("rdb")

            Dim DivStatus As String = Request.QueryString("d")

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            Dim cmd As New SqlCommand
            Try
                conn.Open()
                If status_code = "3" Then
                    cmd = New SqlCommand("update  OSS_EXTENSION_TABLE set DivStatus=@DivStatus,UploadStatus=@US,remarks=@remarks,reason=@reason where patch_no=@p", conn)
                    cmd.Parameters.AddWithValue("@remarks", remarks)
                    cmd.Parameters.AddWithValue("@reason", reason)
                Else
                    cmd = New SqlCommand("update  OSS_EXTENSION_TABLE set DivStatus=@DivStatus,UploadStatus=@US where patch_no=@p", conn)
                End If
                cmd.Parameters.AddWithValue("@DivStatus", Convert.ToInt32(DivStatus))
                cmd.Parameters.AddWithValue("@US", status_code)
                cmd.Parameters.AddWithValue("@p", Patch_No)
                If cmd.ExecuteNonQuery() > 0 Then
                    Result = "Yes"
                End If
            Catch ex As Exception

            Finally
                conn.Close()
            End Try
            Response.Write(Result)
            Response.ContentType = "text/plain"
        Catch ex As Exception

        End Try
    End Sub
End Class
