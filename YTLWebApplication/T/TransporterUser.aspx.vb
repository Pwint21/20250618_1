Imports System.Data
Imports System.Data.SqlClient


Partial Class TransporterUser
    Inherits System.Web.UI.Page
    Public sb As New StringBuilder
    Public opt As String

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            'If Request.Cookies("userinfo") Is Nothing Then
            '    Response.Redirect("Login.aspx")
            'End If
        Catch ex As Exception
        End Try
        MyBase.OnInit(e)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            If Not Page.IsPostBack Then
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As SqlCommand = New SqlCommand("select CompanyId,CompanyName  from ec_company", conn)
                Dim dr As SqlDataReader
                Try
                    conn.Open()
                    ddlcompany.Items.Clear()
                    ddlcompany2.Items.Clear()
                    dr = cmd.ExecuteReader()
                    While dr.Read()
                        ddlcompany.Items.Add(New ListItem(dr("CompanyName").ToString().ToUpper(), dr("CompanyId")))
                        ddlcompany2.Items.Add(New ListItem(dr("CompanyName").ToString().ToUpper(), dr("CompanyId")))
                    End While
                Catch ex As Exception
                Finally
                    conn.Close()
                End Try

            End If

        Catch ex As Exception

        End Try

    End Sub

End Class
