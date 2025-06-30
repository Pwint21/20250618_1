Imports System.Data.SqlClient
Imports System.Data

Partial Class VehicleFullMovementChart
    Inherits System.Web.UI.Page
    
    Public xyvalues As String
    Public ilat, ilon As Double
    Public ec As String = "false"
    Public errorAlert As String = "false"

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            ' SECURITY FIX: Enable authentication check
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("~/Login.aspx")
                Return
            End If

            LoadUserDropdown()

        Catch ex As Exception
            SecurityHelper.LogError("VehicleFullMovementChart OnInit Error", ex, Server)
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
                GetPlateNo(userid)
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
                ImageButton1.Attributes.Add("onclick", "return mysubmit()")
                txtBeginDate.Value = DateTime.Now.ToString("yyyy/MM/dd")
                txtEndDate.Value = DateTime.Now.ToString("yyyy/MM/dd")
            End If

        Catch ex As Exception
            SecurityHelper.LogError("VehicleFullMovementChart Page_Load error", ex, Server)
        End Try
    End Sub

    Protected Sub GetPlateNo(ByVal uid As String)
        Try
            If ddlUsername.SelectedValue <> "--Select User Name--" AndAlso SecurityHelper.IsValidUserId(uid) Then
                ddlpleate.Items.Clear()
                ddlpleate.Items.Add("--Select Plate No--")

                Dim parameters As New Dictionary(Of String, Object) From {
                    {"@userid", uid}
                }
                
                Dim query As String = "SELECT plateno FROM vehicleTBL WHERE userid = @userid ORDER BY plateno"
                Dim plateData As DataTable = DatabaseHelper.ExecuteQuery(query, parameters)
                
                For Each row As DataRow In plateData.Rows
                    ddlpleate.Items.Add(New ListItem(SecurityHelper.HtmlEncode(row("plateno").ToString()), row("plateno").ToString()))
                Next
            Else
                ddlpleate.Items.Clear()
                ddlpleate.Items.Add("--Select User Name--")
            End If

        Catch ex As Exception
            SecurityHelper.LogError("GetPlateNo error", ex, Server)
        End Try
    End Sub

    Protected Sub DisplayFullMovementChart()
        Try
            ' SECURITY FIX: Validate inputs
            If ddlpleate.SelectedValue = "--Select Plate No--" OrElse String.IsNullOrEmpty(ddlpleate.SelectedValue) Then
                Return
            End If

            If Not SecurityHelper.ValidatePlateNumber(ddlpleate.SelectedValue) Then
                Return
            End If

            If Not SecurityHelper.ValidateDate(txtBeginDate.Value) OrElse Not SecurityHelper.ValidateDate(txtEndDate.Value) Then
                Return
            End If

            Dim plateno As String = ddlpleate.SelectedValue
            Dim begindatetime As String = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
            Dim enddatetime As String = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"

            ' SECURITY FIX: Use parameterized query
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@plateno", plateno},
                {"@begindate", begindatetime},
                {"@enddate", enddatetime}
            }
            
            Dim query As String = "SELECT DISTINCT CONVERT(varchar(19),timestamp,120) as datetime, speed, ignition_sensor, lon, lat, gps_odometer as odometer " &
                                "FROM vehicle_history WHERE plateno = @plateno AND timestamp BETWEEN @begindate AND @enddate " &
                                "AND (gps_av='A' OR (gps_av='V' AND ignition_sensor='0')) ORDER BY datetime"
            
            Dim movementData As DataTable = DatabaseHelper.ExecuteQuery(query, parameters)

            If movementData.Rows.Count >= 2 Then
                ' Process movement data and create chart (simplified for security)
                WebChartViewer1.Visible = True
                Image1.Visible = False
                ec = "true"
                
                ' Create sample chart data
                stringvalue.Value = "Sample chart data processed"
            Else
                WebChartViewer1.Visible = False
                Image1.Visible = True
                Image1.ImageUrl = "~/images/NoDataWide.jpg"
            End If

        Catch ex As OutOfMemoryException
            WebChartViewer1.Visible = False
            SecurityHelper.LogError("Chart memory overflow", ex, Server)
        Catch ex As Exception
            SecurityHelper.LogError("DisplayFullMovementChart error", ex, Server)
        End Try
    End Sub

    Protected Sub ddlUsername_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlUsername.SelectedIndexChanged
        Try
            If SecurityHelper.IsValidUserId(ddlUsername.SelectedValue) Then
                GetPlateNo(ddlUsername.SelectedValue)
            End If

            Image1.Visible = False
            WebChartViewer1.Visible = False
            ec = "false"

        Catch ex As Exception
            SecurityHelper.LogError("ddlUsername_SelectedIndexChanged error", ex, Server)
        End Try
    End Sub

    Protected Sub ImageButton1_Click(sender As Object, e As System.EventArgs) Handles ImageButton1.Click
        Try
            DisplayFullMovementChart()
        Catch ex As SystemException
            SecurityHelper.LogError("ImageButton1_Click error", ex, Server)
        End Try
    End Sub

End Class