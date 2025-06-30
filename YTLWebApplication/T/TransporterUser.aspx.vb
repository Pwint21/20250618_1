Imports System.Data
Imports System.Data.SqlClient

Partial Class TransporterUser
    Inherits System.Web.UI.Page
    Public sb As New StringBuilder
    Public opt As String

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
            SecurityHelper.LogError("TransporterUser OnInit Error", ex, Server)
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
                LoadCompanyData()
            End If

        Catch ex As Exception
            SecurityHelper.LogError("TransporterUser Page_Load Error", ex, Server)
            Response.Redirect("Error.aspx")
        End Try
    End Sub

    Private Sub LoadCompanyData()
        Try
            Dim query As String = "SELECT CompanyId, CompanyName FROM ec_company ORDER BY CompanyName"
            Dim companyData As DataTable = SecurityHelper.ExecuteSecureQuery(query, New Dictionary(Of String, Object))
            
            ddlcompany.Items.Clear()
            ddlcompany2.Items.Clear()
            
            For Each row As DataRow In companyData.Rows
                Dim companyName As String = SecurityHelper.HtmlEncode(row("CompanyName").ToString().ToUpper())
                Dim companyId As String = row("CompanyId").ToString()
                
                ddlcompany.Items.Add(New ListItem(companyName, companyId))
                ddlcompany2.Items.Add(New ListItem(companyName, companyId))
            Next

        Catch ex As Exception
            SecurityHelper.LogError("LoadCompanyData Error", ex, Server)
        End Try
    End Sub
End Class