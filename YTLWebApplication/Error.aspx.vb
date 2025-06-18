Public Class ErrorPage
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' SECURITY FIX: Log error without exposing details to user
        Try
            If Session("LastError") IsNot Nothing Then
                LogError(Session("LastError").ToString())
                Session.Remove("LastError")
            End If
        Catch
            ' Fail silently
        End Try
    End Sub

    Private Sub LogError(errorMessage As String)
        Try
            Dim logPath As String = Server.MapPath("~/Logs/ErrorLog.txt")
            Dim logEntry As String = $"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} - Error: {errorMessage}{Environment.NewLine}"
            
            ' Ensure logs directory exists
            Dim logDir As String = Path.GetDirectoryName(logPath)
            If Not Directory.Exists(logDir) Then
                Directory.CreateDirectory(logDir)
            End If
            
            File.AppendAllText(logPath, logEntry)
        Catch
            ' Fail silently if logging fails
        End Try
    End Sub

End Class