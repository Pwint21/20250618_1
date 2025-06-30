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
            ' SECURITY FIX: Enable authentication check
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If

            un = Literal2.Text
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)

            suserid = SecurityHelper.ValidateInput(Request.QueryString("userid"), "numeric")

            LoadUserDropdown(userid, role, userslist)

        Catch ex As Exception
            SecurityHelper.LogError("TrailerManagementNew OnInit Error", ex, Server)
            Response.Redirect("Error.aspx")
        End Try
        MyBase.OnInit(e)
    End Sub

    Private Sub LoadUserDropdown(userid As String, role As String, userslist As String)
        Try
            Dim parameters As New Dictionary(Of String, Object)
            Dim query As String = ""

            If role = "User" Then
                query = "SELECT userid, username FROM userTBL WHERE userid = @userid ORDER BY username"
                parameters.Add("@userid", userid)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                If SecurityHelper.IsValidUsersList(userslist) Then
                    Dim userIds() As String = userslist.Split(","c)
                    Dim paramNames As New List(Of String)
                    
                    For i As Integer = 0 To userIds.Length - 1
                        Dim paramName As String = "@userid" & i
                        paramNames.Add(paramName)
                        parameters.Add(paramName, userIds(i).Trim())
                    Next
                    
                    query = $"SELECT userid, username FROM userTBL WHERE userid IN ({String.Join(",", paramNames)}) ORDER BY username"
                End If
            Else
                query = "SELECT userid, username FROM userTBL WHERE role='User' ORDER BY username"
            End If

            If Not String.IsNullOrEmpty(query) Then
                Dim userData As DataTable = SecurityHelper.ExecuteSecureQuery(query, parameters)
                
                If role <> "User" Then
                    ddluserid.Items.Add(New ListItem(Literal39.Text, Literal39.Text))
                End If

                For Each row As DataRow In userData.Rows
                    Dim username As String = SecurityHelper.HtmlEncode(row("username").ToString().ToUpper())
                    Dim userIdValue As String = row("userid").ToString()
                    ddluserid.Items.Add(New ListItem(username, userIdValue))
                Next
            End If

        Catch ex As Exception
            SecurityHelper.LogError("LoadUserDropdown Error", ex, Server)
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If

            suserid = ss.Value

            If Not Page.IsPostBack Then
                Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
                Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
                Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)
                
                uid.Value = userid
                rle.Value = role
                ulist.Value = userslist
                
                BuildUserDropdownHtml(userid, role, userslist)
            End If
            
            fillDrop()

        Catch ex As Exception
            SecurityHelper.LogError("TrailerManagementNew Page_Load Error", ex, Server)
            Response.Redirect("Error.aspx")
        End Try
    End Sub

    Private Sub BuildUserDropdownHtml(userid As String, role As String, userslist As String)
        Try
            Dim parameters As New Dictionary(Of String, Object)
            Dim query As String = ""

            If role = "User" Then
                query = "SELECT userid, username, dbip FROM userTBL WHERE userid = @userid ORDER BY username"
                parameters.Add("@userid", userid)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                If SecurityHelper.IsValidUsersList(userslist) Then
                    Dim userIds() As String = userslist.Split(","c)
                    Dim paramNames As New List(Of String)
                    
                    For i As Integer = 0 To userIds.Length - 1
                        Dim paramName As String = "@userid" & i
                        paramNames.Add(paramName)
                        parameters.Add(paramName, userIds(i).Trim())
                    Next
                    
                    query = $"SELECT userid, username, dbip FROM userTBL WHERE userid IN ({String.Join(",", paramNames)}) ORDER BY username"
                End If
            Else
                query = "SELECT userid, username, dbip FROM userTBL WHERE role='User' ORDER BY username"
            End If

            If Not String.IsNullOrEmpty(query) Then
                Dim userData As DataTable = SecurityHelper.ExecuteSecureQuery(query, parameters)
                
                sb.Length = 0
                sb.Append("<select id=""ddluser1"" onchange=""javascript: return refreshTable()"" data-placeholder=""Select User Group"" style=""width:250px;"" class=""chosen"" tabindex=""5"">")
                sb.Append("<option id=""epty"" value=""""></option>")

                If role = "SuperUser" Or role = "Admin" Then
                    sb.Append("<option selected=""selected"" value=""SELECT USERNAME"">").Append(SecurityHelper.HtmlEncode(Literal39.Text)).Append("</option>")
                    sb.Append("<option value=""ALL USERS"">").Append(SecurityHelper.HtmlEncode(Literal40.Text)).Append("</option>")
                End If

                For Each row As DataRow In userData.Rows
                    sb.Append("<option value=""").Append(SecurityHelper.HtmlEncode(row("userid").ToString())).Append(""">")
                    sb.Append(SecurityHelper.HtmlEncode(row("username").ToString().ToUpper()))
                    sb.Append("</option>")
                Next
                
                sb.Append("</select>")
                opt = sb.ToString()
            End If

        Catch ex As Exception
            SecurityHelper.LogError("BuildUserDropdownHtml Error", ex, Server)
        End Try
    End Sub

    Private Sub fillDrop()
        Try
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)
            
            BuildUserDropdownHtml(userid, role, userslist)

        Catch ex As Exception
            SecurityHelper.LogError("fillDrop Error", ex, Server)
        End Try
    End Sub
End Class