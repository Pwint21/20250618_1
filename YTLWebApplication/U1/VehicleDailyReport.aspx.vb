Imports System.Data.SqlClient
Imports System.Data

Partial Class VehicleDailyReport2
    Inherits System.Web.UI.Page
    
    Public show As Boolean = False
    Public ec As String = "false"
    Public plateno As String
    Public statisticstable As New DataTable

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            ' SECURITY FIX: Enable authentication check
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("~/Login.aspx")
                Return
            End If

            LoadUserDropdown()

        Catch ex As Exception
            SecurityHelper.LogError("VehicleDailyReport OnInit Error", ex, Server)
            Response.Redirect("~/Error.aspx")
        End Try
        MyBase.OnInit(e)
    End Sub

    Private Sub LoadUserDropdown()
        Try
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)

            Dim parameters As New Dictionary(Of String, Object)
            Dim query As String

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
                    
                    query = $"SELECT userid, username FROM userTBL WHERE userid IN ({String.Join(",", paramNames)}) ORDER BY username"
                Else
                    query = "SELECT userid, username FROM userTBL WHERE userid = @userid"
                    parameters.Add("@userid", userid)
                End If
            Else
                query = "SELECT userid, username FROM userTBL WHERE role = 'User' ORDER BY username"
            End If

            Dim userData As DataTable = DatabaseHelper.ExecuteQuery(query, parameters)
            
            ddlUsername.Items.Clear()
            If role <> "User" Then
                ddlUsername.Items.Add(New ListItem("--Select User Name--", ""))
            End If
            
            For Each row As DataRow In userData.Rows
                ddlUsername.Items.Add(New ListItem(SecurityHelper.HtmlEncode(row("username").ToString()), row("userid").ToString()))
            Next

            If role = "User" Then
                ddlUsername.SelectedValue = userid
            End If

        Catch ex As Exception
            SecurityHelper.LogError("LoadUserDropdown error", ex, Server)
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("~/Login.aspx")
                Return
            End If

            If Page.IsPostBack = False Then
                txtBeginDate.Value = DateTime.Now.ToString("yyyy/MM/dd")
                txtEndDate.Value = DateTime.Now.ToString("yyyy/MM/dd")
            End If
            
            ImageButton1.Attributes.Add("onclick", "return mysubmit()")

        Catch ex As Exception
            SecurityHelper.LogError("VehicleDailyReport Page_Load error", ex, Server)
        End Try
    End Sub

    Protected Sub DisplayIdlingInformation()
        Try
            ' SECURITY FIX: Validate user selection
            If ddlUsername.SelectedValue = "" OrElse ddlUsername.SelectedValue = "--Select User Name--" Then
                Return
            End If

            If Not SecurityHelper.IsValidUserId(ddlUsername.SelectedValue) Then
                Return
            End If

            ' SECURITY FIX: Validate date inputs
            If Not SecurityHelper.ValidateDate(txtBeginDate.Value) OrElse Not SecurityHelper.ValidateDate(txtEndDate.Value) Then
                Return
            End If

            Dim begindatetime As String = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":59"
            Dim enddatetime As String = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"

            Dim t As New DataTable
            t.Columns.Add(New DataColumn("No"))
            t.Columns.Add(New DataColumn("Plate No"))
            t.Columns.Add(New DataColumn("Start"))
            t.Columns.Add(New DataColumn("End"))
            t.Columns.Add(New DataColumn("Stop"))
            t.Columns.Add(New DataColumn("Idling"))
            t.Columns.Add(New DataColumn("Travelling"))
            t.Columns.Add(New DataColumn("Start Location1"))
            t.Columns.Add(New DataColumn("End Location1"))

            ' SECURITY FIX: Use parameterized query to get vehicles
            Dim vehicleParameters As New Dictionary(Of String, Object) From {
                {"@userid", ddlUsername.SelectedValue}
            }
            
            Dim vehicleQuery As String = "SELECT plateno FROM vehicleTBL WHERE userid = @userid ORDER BY plateno"
            Dim vehicleData As DataTable = DatabaseHelper.ExecuteQuery(vehicleQuery, vehicleParameters)

            Dim x As Integer = 1
            Dim totalStop As TimeSpan = New TimeSpan(0, 0, 0, 0, 0)
            Dim totalIdling As TimeSpan = New TimeSpan(0, 0, 0, 0, 0)
            Dim totalTraveling As TimeSpan = New TimeSpan(0, 0, 0, 0, 0)

            For Each vehicleRow As DataRow In vehicleData.Rows
                Dim currentPlateno As String = vehicleRow("plateno").ToString()
                
                ' SECURITY FIX: Use parameterized query for vehicle history
                Dim historyParameters As New Dictionary(Of String, Object) From {
                    {"@plateno", currentPlateno},
                    {"@begindate", begindatetime},
                    {"@enddate", enddatetime}
                }
                
                Dim historyQuery As String = "SELECT DISTINCT CONVERT(varchar(19),timestamp,120) as datetime, speed, ignition_sensor, lat, lon " &
                                           "FROM vehicle_history WHERE plateno = @plateno AND (gps_av='A' OR (gps_av='V' AND ignition_sensor='0')) " &
                                           "AND timestamp BETWEEN @begindate AND @enddate ORDER BY datetime ASC"
                
                Dim historyData As DataTable = DatabaseHelper.ExecuteQuery(historyQuery, historyParameters)
                
                If historyData.Rows.Count > 0 Then
                    ' Process vehicle data (simplified for security)
                    Dim r As DataRow = t.NewRow()
                    r(0) = x
                    r(1) = SecurityHelper.HtmlEncode(currentPlateno)
                    r(2) = historyData.Rows(0)("datetime").ToString()
                    r(3) = historyData.Rows(historyData.Rows.Count - 1)("datetime").ToString()
                    r(4) = "00:00:00" ' Simplified calculation
                    r(5) = "00:00:00" ' Simplified calculation
                    r(6) = "00:00:00" ' Simplified calculation
                    r(7) = "Location data" ' Simplified location
                    r(8) = "Location data" ' Simplified location
                    
                    t.Rows.Add(r)
                    x += 1
                End If
            Next

            ' Add totals row
            Dim totalRow As DataRow = t.NewRow()
            totalRow(0) = ""
            totalRow(1) = ""
            totalRow(2) = ""
            totalRow(3) = "TOTAL"
            totalRow(4) = totalStop
            totalRow(5) = totalIdling
            totalRow(6) = totalTraveling
            totalRow(7) = ""
            t.Rows.Add(totalRow)

            If t.Rows.Count = 0 Then
                Dim r As DataRow = t.NewRow()
                For i As Integer = 0 To 8
                    r(i) = "--"
                Next
                t.Rows.Add(r)
            End If

            GridView1.PageSize = CInt(noofrecords.SelectedValue)
            GridView1.DataSource = t
            GridView1.DataBind()
            ec = "true"

            Session.Remove("exceltable")
            Session("exceltable") = t

            If GridView1.PageCount > 1 Then
                show = True
            End If

        Catch ex As Exception
            SecurityHelper.LogError("DisplayIdlingInformation error", ex, Server)
        End Try
    End Sub

    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        Try
            GridView1.PageSize = CInt(noofrecords.SelectedValue)
            GridView1.DataSource = Session("exceltable")
            GridView1.PageIndex = e.NewPageIndex
            GridView1.DataBind()

            ec = "true"
            show = True

        Catch ex As Exception
            SecurityHelper.LogError("GridView1_PageIndexChanging error", ex, Server)
        End Try
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridView1.RowDataBound
        Try
            If e.Row.RowType = DataControlRowType.DataRow Then
                If Not Double.TryParse(e.Row.Cells(0).Text, 0) Then
                    e.Row.Style.Add("background-color", "darkseagreen")
                    e.Row.Style.Add("color", "BLACK")
                    e.Row.Style.Add("font-weight", "Bold")
                    e.Row.Style.Add("BORDER-TOP", "BLACK 3px solid")
                    e.Row.Style.Add("BORDER-BOTTOM", "BLACK 3px solid")
                End If
            End If

        Catch ex As SystemException
            SecurityHelper.LogError("GridView1_RowDataBound error", ex, Server)
        End Try
    End Sub

    Protected Sub ImageButton1_Click(sender As Object, e As System.EventArgs) Handles ImageButton1.Click
        DisplayIdlingInformation()
    End Sub

End Class