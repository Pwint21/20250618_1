Imports System
Imports System.Collections.Generic
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Configuration
Imports System.Data.SqlClient
Imports System.Data
Imports System.Diagnostics

Partial Class VehicleLogChartMap
    Inherits System.Web.UI.Page
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try

            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            Dim suserid As String = Request.QueryString("userid")


            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand = New SqlCommand("select userid,username from userTBL where role='User' order by username", conn)
            Dim dr As SqlDataReader

            If role = "User" Then
                cmd = New SqlCommand("select userid,username from userTBL where userid='" & userid & "' order by username", conn)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select userid,username from userTBL where userid in(" & userslist & ") order by username", conn)
            End If

            conn.Open()
            dr = cmd.ExecuteReader()
            user_lists.Items.Add(New ListItem("-- SELECT USER --", 0))
            While dr.Read()
                user_lists.Items.Add(New ListItem(dr("username").ToString().ToUpper(), dr("userid")))
            End While
            conn.Close()




        Catch ex As Exception

        Finally

            MyBase.OnInit(e)

        End Try
    End Sub
End Class
