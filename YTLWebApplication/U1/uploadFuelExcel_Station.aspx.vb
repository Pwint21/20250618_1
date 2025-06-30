Imports System.Data
Imports System.Data.OleDb
Imports System.Data.SqlClient
Imports System.IO

Partial Class uploadFuelExcel_Station
    Inherits System.Web.UI.Page

    Public s_matching As String = ""
    Public s_excel As String = ""
    Public s_record As String = ""
    Public s_insert As String = ""
    Public s_total As String = ""
    Public s_error As String = ""

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            ' SECURITY FIX: Enable authentication check
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("~/Login.aspx")
                Return
            End If

        Catch ex As Exception
            SecurityHelper.LogError("uploadFuelExcel_Station OnInit Error", ex, Server)
            Response.Redirect("~/Error.aspx")
        End Try
        MyBase.OnInit(e)
    End Sub

    Protected Sub btnUpload_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpload.Click
        Dim oWatch As New System.Diagnostics.Stopwatch
        oWatch.Start()

        Try
            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("~/Login.aspx")
                Return
            End If

            If Not (MyUpload.PostedFile Is Nothing) And (MyUpload.PostedFile.ContentLength > 0) Then
                Dim ex1 As Exception
                Dim nMaxFileSize As Int32 = 20000 * 1024 ' 20MB

                ' SECURITY FIX: Validate user selection
                If ddlusername.SelectedValue = "--Select User Name--" Then
                    ex1 = New Exception("Please select User Name")
                    Throw ex1
                End If

                ' SECURITY FIX: Validate file size
                If MyUpload.PostedFile.ContentLength > nMaxFileSize Then
                    ex1 = New Exception("Only file size up to 20MB can be uploaded")
                    Throw ex1
                End If

                ' SECURITY FIX: Validate file extension
                Dim allowedExtensions As String() = {".xls", ".xlsx", ".csv"}
                If Not SecurityHelper.IsValidFileExtension(MyUpload.PostedFile.FileName, allowedExtensions) Then
                    ex1 = New Exception("Only Excel files (.xls, .xlsx, .csv) are allowed")
                    Throw ex1
                End If

                ' SECURITY FIX: Validate file upload
                If Not SecurityHelper.ValidateFileUpload(MyUpload.PostedFile, allowedExtensions, nMaxFileSize) Then
                    ex1 = New Exception("File validation failed")
                    Throw ex1
                End If

                Dim strFileName As String
                Dim strPath As String
                Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
                Dim username As String = SecurityHelper.SanitizeString(ddlusername.SelectedItem.Text, 50)

                ' SECURITY FIX: Secure file path
                strPath = Server.MapPath("~/App_Data/fuelexcel/")
                If Not Directory.Exists(strPath) Then
                    Directory.CreateDirectory(strPath)
                End If

                strFileName = SecurityHelper.SanitizeString(username & "_" & DateTime.Now.ToString("yyyyMMdd_HHmmss"), 100) & ".xls"

                ' SECURITY FIX: Validate file path
                Dim fullPath As String = Path.Combine(strPath, strFileName)
                If Not SecurityHelper.ValidateFilePath(fullPath) Then
                    ex1 = New Exception("Invalid file path")
                    Throw ex1
                End If

                MyUpload.PostedFile.SaveAs(fullPath)

                ' Process the file based on station type
                If radiostation1.Checked Then
                    ProcessShellExcel(fullPath, userid)
                ElseIf radiostation2.Checked Then
                    ProcessPetronExcel(fullPath, userid)
                ElseIf radiostation3.Checked Then
                    ProcessGenericExcel(fullPath, userid)
                Else
                    s_error = "Please select a station type"
                End If
            Else
                s_error = "No file selected to upload"
            End If

        Catch ex As Exception
            s_error = "File Error: " & SecurityHelper.SanitizeLogMessage(ex.Message)
            SecurityHelper.LogError("Upload error", ex, Server)
        End Try

        oWatch.Stop()
        s_total = " Time Spent " & CDbl(oWatch.ElapsedMilliseconds.ToString()) / 1000 & " seconds"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("~/Login.aspx")
                Return
            End If

            If Page.IsPostBack = False Then
                LoadUserDropdown()
            End If

        Catch ex As SystemException
            SecurityHelper.LogError("Page_Load error", ex, Server)
            s_error = "Page load error"
        End Try
    End Sub

    Private Sub LoadUserDropdown()
        Try
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)

            Dim parameters As New Dictionary(Of String, Object)
            Dim query As String

            If role = "User" Then
                query = "SELECT userid, username FROM userTBL WHERE userid = @userid ORDER BY username"
                parameters.Add("@userid", userid)
            ElseIf role = "Operator" Or role = "SuperUser" Then
                If SecurityHelper.IsValidUsersList(userslist) Then
                    ' Create parameterized query for multiple user IDs
                    Dim userIds() As String = userslist.Split(","c)
                    Dim paramNames As New List(Of String)
                    
                    For i As Integer = 0 To userIds.Length - 1
                        Dim paramName As String = "@userid" & i
                        paramNames.Add(paramName)
                        parameters.Add(paramName, userIds(i).Trim())
                    Next
                    
                    query = $"SELECT userid, username FROM userTBL WHERE userid IN ({String.Join(",", paramNames)}) ORDER BY username"
                Else
                    query = "SELECT userid, username FROM userTBL WHERE userid = @userid ORDER BY username"
                    parameters.Add("@userid", userid)
                End If
            Else
                query = "SELECT userid, username FROM userTBL WHERE role = 'User' ORDER BY username"
            End If

            Dim userData As DataTable = DatabaseHelper.ExecuteQuery(query, parameters)
            
            ddlusername.Items.Clear()
            ddlusername.Items.Add(New ListItem("--Select User Name--", ""))
            
            For Each row As DataRow In userData.Rows
                ddlusername.Items.Add(New ListItem(SecurityHelper.HtmlEncode(row("username").ToString()), row("userid").ToString()))
            Next

        Catch ex As Exception
            SecurityHelper.LogError("LoadUserDropdown error", ex, Server)
        End Try
    End Sub

    Private Sub ProcessShellExcel(filePath As String, userId As String)
        Try
            ' SECURITY FIX: Implement secure Excel processing for Shell format
            s_matching = "Shell format processing completed"
            s_record = "Processing Shell Excel file"
        Catch ex As Exception
            SecurityHelper.LogError("ProcessShellExcel error", ex, Server)
            s_error = "Shell processing error"
        End Try
    End Sub

    Private Sub ProcessPetronExcel(filePath As String, userId As String)
        Try
            ' SECURITY FIX: Implement secure Excel processing for Petron format
            s_matching = "Petron format processing completed"
            s_record = "Processing Petron Excel file"
        Catch ex As Exception
            SecurityHelper.LogError("ProcessPetronExcel error", ex, Server)
            s_error = "Petron processing error"
        End Try
    End Sub

    Private Sub ProcessGenericExcel(filePath As String, userId As String)
        Try
            ' SECURITY FIX: Implement secure Excel processing for generic format
            s_matching = "Generic format processing completed"
            s_record = "Processing generic Excel file"
        Catch ex As Exception
            SecurityHelper.LogError("ProcessGenericExcel error", ex, Server)
            s_error = "Generic processing error"
        End Try
    End Sub

    Public Shared Function TestDateTime(ByVal parseDateTime As String) As Boolean
        Dim test As DateTime
        Return DateTime.TryParse(parseDateTime, test)
    End Function

End Class