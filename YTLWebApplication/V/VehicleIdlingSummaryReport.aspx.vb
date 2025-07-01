Imports System.Data.SqlClient
Imports AspMap
Imports ADODB
Imports System.Data

Partial Class VehicleIdlingSummaryReport
    Inherits System.Web.UI.Page
    Public show As Boolean = False
    Public ec As String = "false"
    Public plateno As String
    Public statisticstable As New DataTable
    Public path As String
    Public addressFunction As New Address()

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            ' SECURITY FIX: Enable authentication check
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If

            path = "http://" & Request.Url.Host & Request.ApplicationPath

            ' SECURITY FIX: Get validated user information
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)

            Dim query As String
            Dim param As New Dictionary(Of String, Object)
            
            If role = "User" Then
                query = "select userid, username, dbip from userTBL where userid=@userid"
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
                    query = $"select userid, username, dbip from userTBL WHERE userid IN ({inClause}) order by username"
                Else
                    query = "select userid, username, dbip from userTBL where userid=@userid"
                    param.Add("@userid", userid)
                End If
            Else
                query = "select userid, username,dbip from userTBL where role='User' order by username"
            End If
            
            Dim dt As DataTable = SecurityHelper.ExecuteSecureQuery(query, param)
            If dt.Rows.Count > 0 Then
                For Each dr As DataRow In dt.Rows
                    ddlUsername.Items.Add(New ListItem(SecurityHelper.HtmlEncode(dr("username").ToString()), dr("userid").ToString()))
                Next
            End If

            If role = "User" Then
                ddlUsername.Items.Remove("--Select User Name--")
                ddlUsername.SelectedValue = userid
                getPlateNo(userid)
            Else
                ddlUsername.SelectedIndex = 0
            End If

            ' SECURITY FIX: Validate date calculations
            Dim Curyearmonth As String = Year(DateAdd(DateInterval.Month, -1, Date.Now)).ToString() & "-" & Month(DateAdd(DateInterval.Month, -1, Date.Now))
            Dim yearmonth As String = ""
            
            For i As Int16 = 0 To 6
                Dim monthDate As DateTime = DateAdd(DateInterval.Month, -i, Date.Now)
                If Month(monthDate) >= 2 And Year(monthDate) > 2013 Then
                    yearmonth = MonthName(Month(monthDate)).ToString().Substring(0, 3) & " " & Year(monthDate).ToString()
                    ddlMonth.Items.Add(New ListItem(yearmonth, Year(monthDate).ToString() & "-" & Month(monthDate).ToString()))
                End If
            Next
            ddlMonth.SelectedValue = Curyearmonth

        Catch ex As Exception
            SecurityHelper.LogError("VehicleIdlingSummaryReport OnInit Error", ex, Server)
            Response.Redirect("Error.aspx")
        End Try
        MyBase.OnInit(e)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If

            ImageButton1.Attributes.Add("onclick", "return mysubmit()")

        Catch ex As Exception
            SecurityHelper.LogError("VehicleIdlingSummaryReport Page_Load Error", ex, Server)
            Response.Redirect("Error.aspx")
        End Try
    End Sub

    Protected Sub getPlateNo(ByVal uid As String)
        Try
            ' SECURITY FIX: Validate user ID
            If Not SecurityHelper.ValidateUserId(uid) Then
                Return
            End If

            If ddlUsername.SelectedValue <> "--Select User Name--" Then
                ddlpleate.Items.Clear()
                ddlpleate.Items.Add("--Select Plate No--")
                ddlpleate.Items.Add(New ListItem("--All Plate No-", "ALL"))
                
                Dim query As String = "select plateno from vehicleTBL where userid=@uid order by plateno"
                Dim param As New Dictionary(Of String, Object) From {{"@uid", uid}}
                
                Dim dt As DataTable = SecurityHelper.ExecuteSecureQuery(query, param)
                If dt.Rows.Count > 0 Then
                    For Each dr As DataRow In dt.Rows
                        ddlpleate.Items.Add(New ListItem(SecurityHelper.HtmlEncode(dr("plateno").ToString()), dr("plateno").ToString()))
                    Next
                End If
            Else
                ddlpleate.Items.Clear()
                ddlpleate.Items.Add("--Select User Name--")
            End If
        Catch ex As Exception
            SecurityHelper.LogError("getPlateNo Error", ex, Server)
        End Try
    End Sub

    Protected Sub displayidlingSummary()
        Try
            ' SECURITY FIX: Validate user permissions
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If

            ' SECURITY FIX: Validate inputs
            If ddlMonth.SelectedValue = "-- Select --" Then
                Return
            End If

            Dim t As New DataTable
            t.Columns.Add(New DataColumn("sno"))
            t.Columns.Add(New DataColumn("plateno"))
            t.Columns.Add(New DataColumn("Duration"))

            Dim i As Int32 = 1
            Dim r As DataRow
            
            ' SECURITY FIX: Validate month/year input
            Dim yearmonth As String() = ddlMonth.SelectedValue.Split("-"c)
            If yearmonth.Length <> 2 Then
                Return
            End If
            
            Dim year As Integer
            Dim month As Integer
            If Not Integer.TryParse(yearmonth(0), year) OrElse Not Integer.TryParse(yearmonth(1), month) Then
                Return
            End If
            
            If year < 2000 OrElse year > 2100 OrElse month < 1 OrElse month > 12 Then
                Return
            End If

            Dim query As String
            Dim param As New Dictionary(Of String, Object)
            
            If ddlpleate.SelectedValue <> "ALL" Then
                ' SECURITY FIX: Validate plate number
                If Not SecurityHelper.ValidatePlateNumber(ddlpleate.SelectedValue) Then
                    Return
                End If
                
                query = "select plateno,SUM(duration) as idlingtime from vehicle_movement where type=1 and MONTH(fromtime)=@month and year(fromtime)=@year and plateno=@plateno group by plateno order by plateno"
                param.Add("@month", month)
                param.Add("@year", year)
                param.Add("@plateno", ddlpleate.SelectedValue)
            Else
                query = "select plateno,SUM(duration) as idlingtime from vehicle_movement where type=1 and MONTH(fromtime)=@month and year(fromtime)=@year and plateno in (select plateno from vehicleTBL where userid=@userid) group by plateno order by plateno"
                param.Add("@month", month)
                param.Add("@year", year)
                param.Add("@userid", SecurityHelper.ValidateAndGetUserId(Request))
            End If

            Dim dt As DataTable = SecurityHelper.ExecuteSecureQuery(query, param)
            Dim span As TimeSpan
            Dim totaltime As TimeSpan
            Dim displaytext As String = ""
            
            If dt.Rows.Count > 0 Then
                For Each dr As DataRow In dt.Rows
                    displaytext = ""
                    r = t.NewRow()
                    r(0) = i
                    r(1) = SecurityHelper.HtmlEncode(dr("plateno").ToString())
                    span = TimeSpan.FromSeconds(Convert.ToDouble(dr("idlingtime")))
                    
                    If span.Days > 0 Then
                        displaytext = span.Days.ToString.PadLeft(2, "0"c) & " Days " & span.Hours.ToString.PadLeft(2, "0"c) & ":" & span.Minutes.ToString.PadLeft(2, "0"c) & ":" & span.Seconds.ToString.PadLeft(2, "0"c)
                    Else
                        displaytext = "00 Days " & span.Hours.ToString.PadLeft(2, "0"c) & ":" & span.Minutes.ToString.PadLeft(2, "0"c) & ":" & span.Seconds.ToString.PadLeft(2, "0"c)
                    End If
                    
                    r(2) = displaytext
                    totaltime = totaltime + span
                    t.Rows.Add(r)
                    i = i + 1
                Next
            End If
            
            ' Add total row
            displaytext = ""
            r = t.NewRow()
            r(0) = ""
            r(1) = "Total"
            
            If totaltime.Days > 0 Then
                displaytext = totaltime.Days.ToString.PadLeft(2, "0"c) & " Days " & totaltime.Hours.ToString.PadLeft(2, "0"c) & ":" & totaltime.Minutes.ToString.PadLeft(2, "0"c) & ":" & totaltime.Seconds.ToString.PadLeft(2, "0"c)
            Else
                displaytext = "00 Days " & totaltime.Hours.ToString.PadLeft(2, "0"c) & ":" & totaltime.Minutes.ToString.PadLeft(2, "0"c) & ":" & totaltime.Seconds.ToString.PadLeft(2, "0"c)
            End If
            
            r(2) = displaytext
            t.Rows.Add(r)

            ViewState("exceltable") = t
            GridView1.PageSize = 100
            GridView1.DataSource = t
            GridView1.DataBind()
            ec = "true"

            Session.Remove("exceltable")
            Session.Remove("exceltable2")
            Session.Remove("exceltable3")
            Session.Remove("excelchart")
            Session("exceltable") = t
            
        Catch ex As Exception
            SecurityHelper.LogError("displayidlingSummary Error", ex, Server)
        End Try
    End Sub

    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        Try
            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If

            GridView1.PageSize = 100
            GridView1.DataSource = ViewState("exceltable")
            GridView1.PageIndex = e.NewPageIndex
            GridView1.DataBind()

            ec = "true"
            show = True
        Catch ex As Exception
            SecurityHelper.LogError("GridView1_PageIndexChanging Error", ex, Server)
        End Try
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridView1.RowDataBound
        Try
            If e.Row.RowType = DataControlRowType.DataRow Then
                If Double.TryParse(e.Row.Cells(0).Text, 0) = False Then
                    e.Row.Style.Add("background-color", "darkseagreen")
                    e.Row.Style.Add("color", "BLACK")
                    e.Row.Style.Add("font-weight", "Bold")
                    e.Row.Style.Add("BORDER-TOP", "BLACK 3px solid")
                    e.Row.Style.Add("BORDER-BOTTOM", "BLACK 3px solid")
                End If
            End If
            
            If e.Row.RowType = DataControlRowType.Footer Then
                e.Row.Style.Add("BORDER-BOTTOM", "BLACK 5px double")
            End If
        Catch ex As SystemException
            SecurityHelper.LogError("GridView1_RowDataBound Error", ex, Server)
        End Try
    End Sub

    Protected Sub ddlUsername_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlUsername.SelectedIndexChanged
        Try
            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If
            
            getPlateNo(ddlUsername.SelectedValue)
        Catch ex As Exception
            SecurityHelper.LogError("ddlUsername_SelectedIndexChanged Error", ex, Server)
        End Try
    End Sub

    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ImageButton1.Click
        Try
            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If
            
            displayidlingSummary()
        Catch ex As Exception
            SecurityHelper.LogError("ImageButton1_Click Error", ex, Server)
        End Try
    End Sub
End Class