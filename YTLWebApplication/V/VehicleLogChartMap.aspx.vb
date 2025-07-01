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
            ' SECURITY FIX: Enable authentication check
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If

            ' SECURITY FIX: Get validated user information
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)

            ' SECURITY FIX: Validate query string parameter
            Dim suserid As String = SecurityHelper.SanitizeString(Request.QueryString("userid"), 50)

            Dim query As String
            Dim param As New Dictionary(Of String, Object)
            
            If role = "User" Then
                query = "select userid,username from userTBL where userid=@userid order by username"
                param.Add("@userid", userid)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                ' SECURITY FIX: Validate userslist and use safe query construction
                If SecurityHelper.IsValidUsersList(userslist) Then
                    Dim userIds() As String = userslist.Split(","c)
                    Dim parameters As New List(Of String)
                    
                    For i As Integer = 0 To userIds.Length - 1
                        Dim paramName As String = "@userid" & i
                        parameters.Add(paramName)
                        param.Add(paramName, userIds(i).Trim())
                    Next
                    
                    Dim inClause As String = String.Join(",", parameters)
                    query = $"select userid,username from userTBL WHERE userid IN ({inClause}) order by username"
                Else
                    query = "select userid,username from userTBL where userid=@userid order by username"
                    param.Add("@userid", userid)
                End If
            Else
                query = "select userid,username from userTBL where role='User' order by username"
            End If

            user_lists.Items.Add(New ListItem("-- SELECT USER --", 0))
            
            Dim dt As DataTable = SecurityHelper.ExecuteSecureQuery(query, param)
            If dt.Rows.Count > 0 Then
                For Each dr As DataRow In dt.Rows
                    user_lists.Items.Add(New ListItem(SecurityHelper.HtmlEncode(dr("username").ToString().ToUpper()), dr("userid").ToString()))
                Next
            End If

        Catch ex As Exception
            SecurityHelper.LogError("VehicleLogChartMap OnInit Error", ex, Server)
            Response.Redirect("Error.aspx")
        Finally
            MyBase.OnInit(e)
        End Try
    End Sub
End Class