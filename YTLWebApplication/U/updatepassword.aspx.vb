Imports System.Data.SqlClient
Imports Newtonsoft.Json

Partial Class updatepassword
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim opr As String = Request.QueryString("opr")
            Dim pwd As String = Request.QueryString("pwd")
            Dim userid As String = Request.Cookies("userinfo")("userid")

            If opr = "check" Then
                checkpassword(userid)
            ElseIf opr = "change" Then
                updatepassword(userid, pwd)
            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub updatepassword(ByVal userid As String, ByVal pwd As String)
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim cmd As SqlCommand
        Dim cmd1 As SqlCommand
        Dim res As Integer = 0
        Dim json As String
        Dim dr As SqlDataReader
        Try
            cmd1 = New SqlCommand("select pwd from userTBL  where userid=" & userid, conn)
            conn.Open()
            dr = cmd1.ExecuteReader()
            If dr.Read() Then
                If UCase(dr("pwd")) = UCase(pwd) Then
                    res = 2
                Else
                    cmd = New SqlCommand("update userTBL set pwd='" & pwd & "' , pwdstatus=1 where userid=" & userid, conn)
                    res = cmd.ExecuteNonQuery()
                End If
            Else
                res = 3
            End If

        Catch ex As Exception
        Finally
            conn.Close()
        End Try
        Dim jss As New Newtonsoft.Json.JsonSerializer()
        json = "{""d"":" & JsonConvert.SerializeObject(res, Formatting.None) & "}"
        Response.ContentType = "text/plain"
        Response.Write(json)
    End Sub

    Protected Sub checkpassword(ByVal userid As String)
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader
        Dim res As Integer = 0
        Dim json As String
        Try
            cmd = New SqlCommand("select pwdstatus from userTBL  where userid=" & userid, conn)
            conn.Open()
            dr = cmd.ExecuteReader()
            If dr.Read() Then
                If dr("pwdstatus") = True Then
                    res = 1
                End If
            Else

            End If
        Catch ex As Exception
        Finally
            conn.Close()
        End Try
        Dim jss As New Newtonsoft.Json.JsonSerializer()
        json = "{""d"":" & JsonConvert.SerializeObject(res, Formatting.None) & "}"
        Response.ContentType = "text/plain"
        Response.Write(json)
    End Sub

End Class
