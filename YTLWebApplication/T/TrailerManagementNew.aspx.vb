Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Collections.Generic
Imports System.Web.Script.Services
Imports Newtonsoft.Json
Imports ASPNetMultiLanguage

Partial Class TrailerManagementNew
    Inherits System.Web.UI.Page

    Public sb1 As New StringBuilder()
    Public opt As String
    Public sb As New StringBuilder()
    Public suserid As String
    Public userid As String
    Public un As String
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If
            un = Literal2.Text
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            suserid = Request.QueryString("userid")

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

            If (role <> "User") Then
                ddluserid.Items.Add(New ListItem(Literal39.Text, Literal39.Text))

            End If

            While dr.Read()
                ddluserid.Items.Add(New ListItem(dr("username").ToString().ToUpper(), dr("userid")))

            End While
            conn.Close()

        Catch ex As Exception
        End Try
        MyBase.OnInit(e)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            suserid = ss.Value

            If Not Page.IsPostBack Then
                Dim userid As String = Request.Cookies("userinfo")("userid")
                Dim role As String = Request.Cookies("userinfo")("role")
                Dim userslist As String = Request.Cookies("userinfo")("userslist")
                uid.Value = userid
                rle.Value = role
                ulist.Value = userslist
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As SqlCommand = New SqlCommand("select userid, username, dbip from userTBL where  role='User' order by username", conn)
                If role = "User" Then
                    cmd = New SqlCommand("select userid, username, dbip from userTBL where userid='" & userid & "' order by username ", conn)
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    cmd = New SqlCommand("select userid, username, dbip from userTBL where userid in (" & userslist & ") order by username", conn)
                End If
                Dim dr As SqlDataReader
                sb = New StringBuilder()
                Try
                    conn.Open()
                    dr = cmd.ExecuteReader()
                    sb.Length = 0
                    sb.Append("<select  id=""ddluser1"" onchange=""javascript: return refreshTable()""   data-placeholder=""Select User Group"" style=""width:250px;"" class=""chosen""  tabindex=""5"">")
                    sb.Append("<option id=""epty"" value=""""></option>")

                    If role = "SuperUser" Or role = "Admin" Then
                        sb.Append("<option selected=""selected"" value=""SELECT USERNAME""> " & Literal39.Text & "</option>")
                        sb.Append("<option value=ALL USERS>" & Literal40.Text & "</option>")
                    End If

                    Dim firstrecord As Boolean = True
                    While dr.Read
                        sb.Append("<option  value=")
                        sb.Append(dr("userid"))
                        sb.Append(">")
                        sb.Append(dr("username").ToString().ToUpper())
                        sb.Append("</option>")
                    End While
                    sb.Append("</select>")
                    dr.Close()
                Catch ex As Exception
                    '  Response.Write(ex.Message)
                Finally
                    conn.Close()
                End Try

                opt = sb.ToString()
            End If
            fillDrop()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub fillDrop()
        Try
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand = New SqlCommand("select userid, username, dbip from userTBL where  role='User' order by username", conn)
            If role = "User" Then
                cmd = New SqlCommand("select userid, username, dbip from userTBL where userid='" & userid & "' order by username ", conn)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select userid, username, dbip from userTBL where userid in (" & userslist & ") order by username", conn)
            End If
            Dim dr As SqlDataReader
            sb = New StringBuilder()
            Try
                conn.Open()
                dr = cmd.ExecuteReader()
                suserid = ss.Value
                sb.Length = 0
                sb.Append("<select  id=""ddluser1"" onchange=""javascript: return refreshTable()""   data-placeholder=""Select User Group"" style=""width:250px;"" class=""chosen""  tabindex=""5"">")
                sb.Append("<option id=""epty"" value=""""></option>")
                If role <> "User" Then
                    sb.Append("<option selected=""selected"" value=""SELECT USERNAME"">" & Literal39.Text & "</option>")
                End If
                If suserid <> "ALL USERS" Then
                    If role = "SuperUser" Or role = "Admin" Then
                        sb.Append("<option value=ALL USERS>" & Literal40.Text & "</option>")
                    End If

                End If
                Dim i As Integer = 0
                While dr.Read
                    userid = dr("userid")
                    If role = "User" Then
                        Dim ct As Integer = 0
                        Dim firstrecord As Boolean = True
                        If (firstrecord = True) Then
                            firstrecord = False
                            suserid = dr("userid")
                        End If
                        If ct = 0 Then
                            sb.Append("<option selected=""selected""  value=")
                            sb.Append(dr("userid"))
                            sb.Append(">")
                            sb.Append(dr("username").ToString().ToUpper())
                            sb.Append("</option>")

                        End If
                        ct = +1
                    Else
                    End If

                    If suserid = "ALL USERS" Then
                        If i = 0 Then
                            sb.Append("<option selected=""selected"" value=ALL USERS>" & Literal40.Text & "</option>")
                            i = +1
                        End If

                    End If
                    If role <> "User" Then
                        If userid = suserid Then
                            sb.Append("<option selected=""selected""  value=")
                            sb.Append(">")
                            sb.Append(dr("username").ToString().ToUpper())
                            sb.Append("</option>")
                        Else
                            sb.Append("<option  value=")
                            sb.Append(dr("userid"))
                            sb.Append(">")
                            sb.Append(dr("username").ToString().ToUpper())
                            sb.Append("</option>")
                        End If

                    End If
                End While
                sb.Append("</select>")
                dr.Close()
            Catch ex As Exception
                '  Response.Write(ex.Message)
            Finally
                conn.Close()
            End Try

            opt = sb.ToString()
        Catch ex As Exception

        End Try
    End Sub
End Class
