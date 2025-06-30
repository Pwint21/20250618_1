Imports System.Data.SqlClient
Partial Class UpdateWebData
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim id As String = Request.QueryString("id")
        Dim eventtype As String = Request.QueryString("reason").ToString()
        Dim remarks As String = Request.QueryString("remarks")
        Dim userid As String = Request.Cookies("userinfo")("userid")
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim cmd As SqlCommand
        cmd = New SqlCommand("update alert_notification set event_reason='" & eventtype & "',remarks='" & remarks & "', remarks_userid='" & userid & "',resolved_datetime='" & Now.ToString("yyyy/MM/dd HH:mm:ss") & "',resolved='1'  where id='" & id & "'", conn)
        Try
            conn.Open()
            'Response.Write(cmd.CommandText.ToString())
            Dim i As Integer = cmd.ExecuteNonQuery()
        Catch ex As Exception
            Response.Write(ex.Message)
        Finally

            conn.Close()
        End Try


    End Sub
End Class
