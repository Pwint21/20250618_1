Imports System.Data.SqlClient
Imports AspMap
Imports ADODB
Imports System.Data

Partial Class VehicleIdlingReporttemp
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

        Catch ex As Exception
            SecurityHelper.LogError("VehicleIdlingReport OnInit Error", ex, Server)
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

            If Page.IsPostBack = False Then
                ' SECURITY FIX: Validate date inputs
                If SecurityHelper.ValidateDate(DateTime.Now.ToString("yyyy/MM/dd")) Then
                    txtBeginDate.Value = DateTime.Now.ToString("yyyy/MM/dd")
                    txtEndDate.Value = DateTime.Now.ToString("yyyy/MM/dd")
                End If
            End If
            ImageButton1.Attributes.Add("onclick", "return mysubmit()")

        Catch ex As Exception
            SecurityHelper.LogError("VehicleIdlingReport Page_Load Error", ex, Server)
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

    Protected Sub DisplayIdlingInformation()
        Try
            ' SECURITY FIX: Validate user permissions
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If

            ' SECURITY FIX: Validate inputs
            If Not SecurityHelper.ValidateDate(txtBeginDate.Value) OrElse Not SecurityHelper.ValidateDate(txtEndDate.Value) Then
                Return
            End If

            If Not SecurityHelper.ValidatePlateNumber(ddlpleate.SelectedValue) Then
                Return
            End If

            Dim plateno As String = ddlpleate.SelectedValue
            Dim begindatetime As String = Date.Parse(txtBeginDate.Value).ToString("yyyy-MM-dd") & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
            Dim enddatetime As String = Date.Parse(txtEndDate.Value).ToString("yyyy-MM-dd") & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"

            Dim t As New DataTable
            t.Columns.Add(New DataColumn("sno"))
            t.Columns.Add(New DataColumn("plateno"))
            t.Columns.Add(New DataColumn("begindatetime"))
            t.Columns.Add(New DataColumn("enddatetime"))
            t.Columns.Add(New DataColumn("duration"))
            t.Columns.Add(New DataColumn("Address"))
            t.Columns.Add(New DataColumn("Nearest Town"))
            t.Columns.Add(New DataColumn("Maps"))
            t.Columns.Add(New DataColumn("Address1"))
            t.Columns.Add(New DataColumn("Lat"))
            t.Columns.Add(New DataColumn("Lon"))
            t.Columns.Add(New DataColumn("Distance Travelled before next idling (Kms)"))
            t.Columns.Add(New DataColumn("Time spent in last travel (Mins)"))

            Dim t1 As New DataTable
            t1.Columns.Add(New DataColumn("S No"))
            t1.Columns.Add(New DataColumn("Plateno"))
            t1.Columns.Add(New DataColumn("Begin Date Time"))
            t1.Columns.Add(New DataColumn("End Date Time"))
            t1.Columns.Add(New DataColumn("Duration"))
            t1.Columns.Add(New DataColumn("Address"))
            t1.Columns.Add(New DataColumn("Nearest Town"))
            t1.Columns.Add(New DataColumn("Maps"))
            t1.Columns.Add(New DataColumn("Lat"))
            t1.Columns.Add(New DataColumn("Lon"))
            t1.Columns.Add(New DataColumn("Distance Travelled before next idling (Kms)"))
            t1.Columns.Add(New DataColumn("Time spent in last travel (Mins)"))

            Dim i As Int32 = 1
            Dim address As String = ""
            Dim lat As Double = 0
            Dim lon As Double = 0
            Dim r As DataRow

            GridView1.Columns.Item(1).Visible = False
            
            ' SECURITY FIX: Use parameterized query
            Dim query As String = "select distinct convert(varchar(19),timestamp,120) as datetime,plateno,speed,ignition_sensor,lat,lon,gps_odometer from vehicle_history where plateno =@plateno and (gps_av='A' or (gps_av='V' and ignition_sensor='0')) and timestamp between @begindatetime and @enddatetime order by datetime asc"
            Dim param As New Dictionary(Of String, Object) From {
                {"@plateno", plateno},
                {"@begindatetime", begindatetime},
                {"@enddatetime", enddatetime}
            }

            Dim prevstatus As String = "stop"
            Dim currentstatus As String = "stop"
            Dim tempprevtime As DateTime = begindatetime
            Dim prevtime As DateTime = begindatetime
            Dim currenttime As DateTime = begindatetime

            Dim lastlat As Double = 0
            Dim lastlon As Double = 0
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            Dim locObj As New Location(userid)

            Dim totalSpan As TimeSpan
            Dim minOption As Byte = SecurityHelper.ValidateNumeric(ddlminutes.SelectedValue, 1, 1440)
            Dim traveldistnace As Double = 0
            Dim previusodo As Double = 0
            Dim currentodo As Double = 0
            Dim traveltimets As TimeSpan

            Dim dt As DataTable = SecurityHelper.ExecuteSecureQuery(query, param)
            
            If dt.Rows.Count > 0 Then
                For Each dr As DataRow In dt.Rows
                    lastlat = dr("lat")
                    lastlon = dr("lon")
                    currentodo = dr("gps_odometer")
                    currenttime = dr("datetime")
                    
                    If dr("ignition_sensor") = 1 And dr("speed") <> 0 Then
                        currentstatus = "moving"
                    ElseIf dr("ignition_sensor") = 1 And dr("speed") = 0 Then
                        currentstatus = "idle"
                    Else
                        currentstatus = "stop"
                    End If
                    
                    If prevstatus <> currentstatus Then
                        Dim temptime As TimeSpan = tempprevtime - prevtime
                        Dim minutes As Int16 = temptime.TotalMinutes()
                        
                        Select Case prevstatus
                            Case "stop"
                                ' No action needed
                            Case "moving"
                                If previusodo <> 0 And currentodo <> 0 Then
                                    If currentodo > previusodo Then
                                        traveldistnace += (currentodo - previusodo)
                                    End If
                                End If
                                If currenttime > prevtime Then
                                    traveltimets += (currenttime - prevtime)
                                End If
                            Case "idle"
                                If temptime.TotalMinutes >= minOption Then
                                    r = t.NewRow
                                    r(0) = i
                                    r(1) = SecurityHelper.HtmlEncode(dr("plateno").ToString())
                                    r(2) = prevtime.ToString("yyyy-MM-dd HH:mm:ss")
                                    r(3) = tempprevtime.ToString("yyyy-MM-dd HH:mm:ss")
                                    r(4) = temptime
                                    totalSpan = totalSpan + temptime
                                    lat = dr("lat")
                                    lon = dr("lon")
                                    
                                    ' SECURITY FIX: Validate coordinates
                                    If SecurityHelper.ValidateCoordinate(lat.ToString(), lon.ToString()) Then
                                        r(5) = SecurityHelper.HtmlEncode(locObj.GetLocation(lat, lon))
                                        r(6) = SecurityHelper.HtmlEncode(locObj.GetNearestTown(lat, lon))
                                    Else
                                        r(5) = "--"
                                        r(6) = "--"
                                    End If
                                    
                                    r(7) = $"<a href='http://maps.google.com/maps?f=q&hl=en&q={lat}+{lon}&om=1&t=k' target='_blank'><img style='border:solid 0 red;' src='images/googlemaps1.gif' title='View map in Google Maps'/></a> <a href='GoogleEarthMaps.aspx?x={lon}&y={lat}'><img style='border:solid 0 red;' src='images/googleearth1.gif' title='View map in GoogleEarth'/></a>"
                                    r(9) = lat.ToString("0.000000")
                                    r(10) = lon.ToString("0.000000")
                                    r(11) = Format((traveldistnace / 100.0), "0")
                                    r(12) = traveltimets.TotalMinutes.ToString("0")
                                    t.Rows.Add(r)
                                    traveldistnace = 0
                                    traveltimets = TimeSpan.Zero
                                    i = i + 1
                                End If
                        End Select
                        
                        prevtime = currenttime
                        prevstatus = currentstatus
                        previusodo = currentodo
                    End If
                    tempprevtime = currenttime
                Next
            End If

            ' Handle final record
            If prevtime <> currenttime Then
                Dim temptime As TimeSpan = currenttime - prevtime
                Dim minutes As Int16 = temptime.TotalMinutes()

                Select Case prevstatus
                    Case "idle"
                        If temptime.Minutes >= minOption Then
                            r = t.NewRow
                            r(0) = i
                            r(1) = SecurityHelper.HtmlEncode(plateno)
                            r(2) = prevtime.ToString("yyyy-MM-dd HH:mm:ss")
                            r(3) = currenttime.ToString("yyyy-MM-dd HH:mm:ss")
                            r(4) = temptime
                            totalSpan = totalSpan + temptime
                            
                            If lastlat <> 0 And lastlon <> 0 Then
                                ' SECURITY FIX: Validate coordinates
                                If SecurityHelper.ValidateCoordinate(lastlat.ToString(), lastlon.ToString()) Then
                                    r(5) = SecurityHelper.HtmlEncode(locObj.GetLocation(lastlat, lastlon))
                                    r(6) = SecurityHelper.HtmlEncode(locObj.GetNearestTown(lastlat, lastlon))
                                Else
                                    r(5) = "--"
                                    r(6) = "--"
                                End If
                            End If
                            
                            r(7) = $"<a href='http://maps.google.com/maps?f=q&hl=en&q={lastlat}+{lastlon}&om=1&t=k' target='_blank'><img style='border:solid 0 red;' src='images/googlemaps1.gif' title='View map in Google Maps'/></a> <a href='GoogleEarthMaps.aspx?x={lastlon}&y={lastlat}'><img style='border:solid 0 red;' src='images/googleearth1.gif' title='View map in GoogleEarth'/></a>"
                            r(9) = lastlat.ToString("0.000000")
                            r(10) = lastlon.ToString("0.000000")
                            t.Rows.Add(r)
                        End If
                End Select
            End If

            ' Build final table
            For k As Int32 = 0 To t.DefaultView.Count - 1
                r = t1.NewRow
                r(0) = (k + 1).ToString()
                r(1) = SecurityHelper.HtmlEncode(t.DefaultView.Item(k).Item("plateno").ToString())
                r(2) = SecurityHelper.HtmlEncode(t.DefaultView.Item(k).Item("begindatetime").ToString())
                r(3) = SecurityHelper.HtmlEncode(t.DefaultView.Item(k).Item("enddatetime").ToString())
                r(4) = t.DefaultView.Item(k).Item("duration")
                r(5) = SecurityHelper.HtmlEncode(t.DefaultView.Item(k).Item("Address").ToString())
                r(6) = SecurityHelper.HtmlEncode(t.DefaultView.Item(k).Item("Nearest Town").ToString())
                r(7) = t.DefaultView.Item(k).Item("Maps")
                r(8) = t.DefaultView.Item(k).Item("Lat")
                r(9) = t.DefaultView.Item(k).Item("Lon")
                r(10) = t.DefaultView.Item(k).Item("Distance Travelled before next idling (Kms)")
                r(11) = t.DefaultView.Item(k).Item("Time spent in last travel (Mins)")
                t1.Rows.Add(r)
            Next
            
            r = t1.NewRow
            r(0) = ""
            r(1) = ""
            r(2) = ""
            r(3) = "TOTAL"
            r(4) = totalSpan
            For j As Integer = 5 To 11
                r(j) = ""
            Next
            t1.Rows.Add(r)

            If t1.DefaultView.Count = 0 Then
                r = t1.NewRow
                For j As Integer = 0 To 11
                    r(j) = "--"
                Next
                t1.Rows.Add(r)
            End If

            ViewState("exceltable") = t1
            GridView1.PageSize = SecurityHelper.ValidateNumeric(noofrecords.SelectedValue, 1, 10000)
            GridView1.DataSource = t1
            GridView1.DataBind()

            ec = "true"

            If GridView1.PageCount > 1 Then
                show = True
            End If

            Session.Remove("exceltable")
            Session.Remove("exceltable2")
            Session.Remove("exceltable3")
            Session.Remove("excelchart")
            Session("exceltable") = t1

        Catch ex As SystemException
            SecurityHelper.LogError("DisplayIdlingInformation Error", ex, Server)
        End Try
    End Sub

    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        Try
            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If

            GridView1.PageSize = SecurityHelper.ValidateNumeric(noofrecords.SelectedValue, 1, 10000)
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

    Protected Sub ImageButton1_Click(sender As Object, e As System.EventArgs) Handles ImageButton1.Click
        Try
            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If
            
            DisplayIdlingInformation()
        Catch ex As Exception
            SecurityHelper.LogError("ImageButton1_Click Error", ex, Server)
        End Try
    End Sub
End Class