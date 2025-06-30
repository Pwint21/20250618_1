Imports System
Imports System.Data
Imports System.Data.SqlClient

Partial Class UpdateJobStatus
    Inherits System.Web.UI.Page
    Private Sub UpdateJobStatus_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            Dim Result As String = "No"
            Dim Patch_No As String = Request.QueryString("p")
            Dim status_code As String = Request.QueryString("i")
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            Dim cmd As New SqlCommand
            Try
                conn.Open()
                cmd = New SqlCommand("update  OSS_EXTENSION_TABLE set UploadStatus=@US where patch_no=@p", conn)
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
