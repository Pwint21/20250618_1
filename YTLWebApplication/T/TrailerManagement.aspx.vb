Imports System.Data.SqlClient
Imports System.Collections.Generic
Imports Newtonsoft.Json

Partial Class TrailerManagement
    Inherits System.Web.UI.Page
    Public ec As String = "false"
    Public sb1 As New StringBuilder()
    
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            ' SECURITY FIX: Enable authentication check
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If

            ' SECURITY FIX: Validate user input
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)
            
            uid.Value = userid
            rle.Value = role
            ulist.Value = userslist
           
        Catch ex As Exception
            SecurityHelper.LogError("TrailerManagement OnInit Error", ex, Server)
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

            If Page.IsPostBack = False Then
                LoadUserDropdowns()
            End If
        Catch ex As Exception
            SecurityHelper.LogError("TrailerManagement Page_Load Error", ex, Server)
            Response.Redirect("Error.aspx")
        End Try
    End Sub

    Private Sub LoadUserDropdowns()
        Try
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)
            
            Dim parameters As New Dictionary(Of String, Object)
            Dim query As String = ""
            
            If role = "User" Then
                query = "SELECT userid, username FROM userTBL WHERE userid = @userid"
                parameters.Add("@userid", userid)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                If SecurityHelper.IsValidUsersList(userslist) Then
                    ' Create parameterized query for multiple user IDs
                    Dim userIds() As String = userslist.Split(","c)
                    Dim paramNames As New List(Of String)
                    
                    For i As Integer = 0 To userIds.Length - 1
                        Dim paramName As String = "@userid" & i
                        paramNames.Add(paramName)
                        parameters.Add(paramName, userIds(i).Trim())
                    Next
                    
                    query = $"SELECT userid, username FROM userTBL WHERE userid IN ({String.Join(",", paramNames)})"
                End If
            Else
                query = "SELECT userid, username FROM userTBL ORDER BY username"
            End If

            If Not String.IsNullOrEmpty(query) Then
                Dim userData As DataTable = SecurityHelper.ExecuteSecureQuery(query, parameters)
                
                For Each row As DataRow In userData.Rows
                    Dim username As String = SecurityHelper.HtmlEncode(row("username").ToString().ToUpper())
                    Dim userIdValue As String = row("userid").ToString()
                    
                    ddluser.Items.Add(New ListItem(username, userIdValue))
                    ddluser1.Items.Add(New ListItem(username, userIdValue))
                Next
            End If

        Catch ex As Exception
            SecurityHelper.LogError("LoadUserDropdowns Error", ex, Server)
        End Try
    End Sub
End Class