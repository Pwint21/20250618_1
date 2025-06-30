Imports System.Data.SqlClient
Imports System.Web.Security

Partial Class UpdateWebData
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Enable authentication check
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("~/Login.aspx")
                Return
            End If

            ' SECURITY FIX: Validate user permissions
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)

            ' SECURITY FIX: Validate input parameters
            Dim id As String = Request.QueryString("id")
            Dim eventtype As String = Request.QueryString("reason")
            Dim remarks As String = Request.QueryString("remarks")

            If Not SecurityHelper.IsValidUserId(id) Then
                Response.StatusCode = 400
                Response.End()
                Return
            End If

            If String.IsNullOrEmpty(eventtype) OrElse SecurityHelper.ContainsDangerousPatterns(eventtype) Then
                Response.StatusCode = 400
                Response.End()
                Return
            End If

            If String.IsNullOrEmpty(remarks) OrElse SecurityHelper.ContainsDangerousPatterns(remarks) Then
                Response.StatusCode = 400
                Response.End()
                Return
            End If

            ' SECURITY FIX: Use parameterized query
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@eventtype", SecurityHelper.SanitizeString(eventtype, 100)},
                {"@remarks", SecurityHelper.SanitizeString(remarks, 500)},
                {"@userid", userid},
                {"@datetime", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")},
                {"@id", id}
            }

            Dim query As String = "UPDATE alert_notification SET event_reason = @eventtype, remarks = @remarks, remarks_userid = @userid, resolved_datetime = @datetime, resolved = '1' WHERE id = @id"

            Try
                Dim rowsAffected As Integer = DatabaseHelper.ExecuteNonQuery(query, parameters)
                If rowsAffected > 0 Then
                    Response.Write("Success")
                Else
                    Response.Write("No records updated")
                End If
            Catch ex As Exception
                SecurityHelper.LogError("UpdateWebData database error", ex, Server)
                Response.StatusCode = 500
                Response.Write("Database error")
            End Try

        Catch ex As Exception
            SecurityHelper.LogError("UpdateWebData error", ex, Server)
            Response.StatusCode = 500
            Response.Write("Server error")
        End Try
    End Sub

End Class