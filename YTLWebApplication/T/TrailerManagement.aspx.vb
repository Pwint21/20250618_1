Imports System.Data.SqlClient

Imports System.Collections.Generic
Imports Newtonsoft.Json
Partial Class TrailerManagement
    Inherits System.Web.UI.Page
    Public ec As String = "false"
    Public sb1 As New StringBuilder()
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If
            Dim userid As String = Request.Cookies("userinfo")("userid")
            uid.Value = userid
            Dim role As String = Request.Cookies("userinfo")("role")
            rle.Value = role
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            ulist.Value = userslist
           
        Catch ex As Exception

        End Try
        MyBase.OnInit(e)
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Page.IsPostBack = False Then
                Dim userid As String = Request.Cookies("userinfo")("userid")
                Dim role As String = Request.Cookies("userinfo")("role")
                Dim userslist As String = Request.Cookies("userinfo")("userslist")
                Dim query As String = "select userid,username from userTBL"
                If role = "User" Then
                    query = "select userid,username from userTBL where userid='" & userid & "'"
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    query = "select userid,username from userTBL where userid in (" & userslist & ")"
                End If

                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As New SqlCommand(query, conn)
                Try
                    conn.Open()
                    Dim dr As SqlDataReader = cmd.ExecuteReader()
                    While dr.Read()
                        ddluser.Items.Add(New ListItem(dr("username").ToString.ToUpper(), dr("userid")))
                        ddluser1.Items.Add(New ListItem(dr("username").ToString.ToUpper(), dr("userid")))
                    End While

                Catch ex As Exception
                    Response.Write(ex.Message)
                Finally
                    conn.Close()
                End Try

            End If
        Catch ex As Exception
        End Try
    End Sub


     

   
End Class

