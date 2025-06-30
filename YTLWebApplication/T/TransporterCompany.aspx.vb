Imports System.Data
Imports System.Data.SqlClient
Imports System.IO

Partial Class TransporterCompany
    Inherits System.Web.UI.Page
    Public sb As New StringBuilder
    Public opt As String
    Public companyid As String = "0"
    
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            ' SECURITY FIX: Enable authentication check
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If

            ' SECURITY FIX: Check user permissions
            If Not SecurityHelper.HasRequiredRole("Admin") Then
                Response.Redirect("Login.aspx")
                Return
            End If

        Catch ex As Exception
            SecurityHelper.LogError("TransporterCompany OnInit Error", ex, Server)
            Response.Redirect("Error.aspx")
        End Try
        MyBase.OnInit(e)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If

            If Not Page.IsPostBack Then
                Fill()
            End If

        Catch ex As Exception
            SecurityHelper.LogError("TransporterCompany Page_Load Error", ex, Server)
            Response.Redirect("Error.aspx")
        End Try
    End Sub

    Protected Sub Fill()
        Try
            ' SECURITY FIX: Validate company ID
            If Not SecurityHelper.ValidateInput(companyid, "numeric") Then
                companyid = "0"
            End If

            Dim parameters As New Dictionary(Of String, Object) From {
                {"@companyid", companyid}
            }

            ' Load geofences that are not assigned to this company
            Dim geofenceQuery As String = "SELECT ISNULL(shiptocode,'') shiptocode, geofenceid, geofencename FROM geofence WHERE geofenceid NOT IN (SELECT ISNULL(item,0) FROM fn_getgeofencelist(@companyid)) AND shiptocode NOT LIKE '000%' ORDER BY geofencename"
            
            Dim geofenceData As DataTable = SecurityHelper.ExecuteSecureQuery(geofenceQuery, parameters)
            
            Allgeofences.Items.Clear()
            For Each row As DataRow In geofenceData.Rows
                Dim displayText As String = SecurityHelper.HtmlEncode(row("geofencename").ToString().ToUpper() & " - " & row("shiptocode").ToString().ToUpper())
                Allgeofences.Items.Add(New ListItem(displayText, row("geofenceid").ToString()))
                Allgeofences.Attributes.Add("class", "list-group-item")
            Next

        Catch ex As Exception
            SecurityHelper.LogError("Fill Error", ex, Server)
        End Try
    End Sub

    Sub WriteLog(ByVal message As String)
        Try
            If message.Length > 0 Then
                ' SECURITY FIX: Sanitize log message
                Dim sanitizedMessage As String = SecurityHelper.SanitizeLogMessage(message)
                Dim logPath As String = Server.MapPath("~/Logs/TransporterCompany.log")
                
                ' Ensure logs directory exists
                Dim logDir As String = Path.GetDirectoryName(logPath)
                If Not Directory.Exists(logDir) Then
                    Directory.CreateDirectory(logDir)
                End If
                
                Using sw As New StreamWriter(logPath, True)
                    sw.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss} - {sanitizedMessage}")
                End Using
            End If
        Catch ex As Exception
            ' Fail silently to prevent information disclosure
        End Try
    End Sub
End Class