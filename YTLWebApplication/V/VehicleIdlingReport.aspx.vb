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
            path = "http://" & Request.Url.Host & Request.ApplicationPath

            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            Dim cmd As SqlCommand
            Dim dr As SqlDataReader

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            cmd = New SqlCommand("select userid, username,dbip from userTBL where role='User' order by username", conn)
            If role = "User" Then
                cmd = New SqlCommand("select userid, username, dbip from userTBL where userid='" & userid & "'", conn)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select userid, username, dbip from userTBL where userid in (" & userslist & ") order by username", conn)
            End If
            conn.Open()
            dr = cmd.ExecuteReader()
            While dr.Read()
                ddlUsername.Items.Add(New ListItem(dr("username"), dr("userid")))
            End While
            dr.Close()
            cmd.Dispose()



            If role = "User" Then
                ddlUsername.Items.Remove("--Select User Name--")
                ddlUsername.SelectedValue = userid
                getPlateNo(userid)
            Else
                ddlUsername.SelectedIndex = 0

            End If

            conn.Close()
            conn.Dispose()

        Catch ex As Exception


        End Try
        MyBase.OnInit(e)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            If Page.IsPostBack = False Then
                txtBeginDate.Value = Now().ToString("yyyy/MM/dd")
                txtEndDate.Value = Now().ToString("yyyy/MM/dd")
            End If
            ImageButton1.Attributes.Add("onclick", "return mysubmit()")

        Catch ex As Exception

        End Try
    End Sub

    Protected Sub getPlateNo(ByVal uid As String)
        Try
            If ddlUsername.SelectedValue <> "--Select User Name--" Then
                ddlpleate.Items.Clear()
                ddlpleate.Items.Add("--Select Plate No--")
                Dim cmd As SqlCommand
                Dim dr As SqlDataReader
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                cmd = New SqlCommand("select plateno from vehicleTBL where userid='" & uid & "' order by plateno", conn)
                conn.Open()
                dr = cmd.ExecuteReader()
                While dr.Read()
                    ddlpleate.Items.Add(New ListItem(dr("plateno"), dr("plateno")))
                End While
                dr.Close()
                conn.Close()
                conn.Dispose()
                cmd.Dispose()
            Else
                ddlpleate.Items.Clear()
                ddlpleate.Items.Add("--Select User Name--")
            End If
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub



    Protected Sub DisplayIdlingInformation()

        Try
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

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            Dim cmd As SqlCommand
            Dim i As Int32 = 1

            Dim address As String = ""
            Dim lat As Double = 0
            Dim lon As Double = 0

            Dim r As DataRow

            GridView1.Columns.Item(1).Visible = False
            cmd = New SqlCommand("select distinct convert(varchar(19),timestamp,120) as datetime,plateno,speed,ignition_sensor,lat,lon,gps_odometer from vehicle_history where plateno ='" & plateno & "' and (gps_av='A' or (gps_av='V' and ignition_sensor='0')) and timestamp between '" & begindatetime & "' and '" & enddatetime & "' order by datetime asc", conn)

            Dim prevstatus As String = "stop"
            Dim currentstatus As String = "stop"

            Dim tempprevtime As DateTime = begindatetime
            Dim prevtime As DateTime = begindatetime
            Dim currenttime As DateTime = begindatetime

            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()

            Dim lastlat As Double = 0
            Dim lastlon As Double = 0

            Dim userid As String = ddlUsername.SelectedValue
            Dim locObj As New Location(userid)


            Dim totalSpan As TimeSpan
            Dim minOption As Byte = ddlminutes.SelectedValue
            Dim traveldistnace As Double = 0
            Dim previusodo As Double = 0
            Dim currentodo As Double = 0
            Dim traveltimets As TimeSpan
            While dr.Read()
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
                    Dim temptime As TimeSpan = tempprevtime - prevtime 'currenttime - prevtime
                    Dim minutes As Int16 = temptime.TotalMinutes()
                    Select Case prevstatus
                        Case "stop"

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
                                r(1) = dr("plateno")
                                r(2) = prevtime.ToString("yyyy-MM-dd HH:mm:ss")
                                r(3) = tempprevtime.ToString("yyyy-MM-dd HH:mm:ss")
                                r(4) = temptime
                                totalSpan = totalSpan + temptime
                                lat = dr("lat")
                                lon = dr("lon")
                                If lat <> 0 And lon <> 0 Then
                                    r(5) = locObj.GetLocation(lat, lon)
                                    r(6) = locObj.GetNearestTown(lat, lon)
                                End If
                                r(7) = "<a href='http://maps.google.com/maps?f=q&hl=en&q=" & dr("lat") & " + " & dr("lon") & "&om=1&t=k' target='_blank'><img style='border:solid 0 red;' src='images/googlemaps1.gif' title='View map in Google Maps'/></a>   <a href='GoogleEarthMaps.aspx?x=" & dr("lon") & "&y=" & dr("lat") & "'><img style='border:solid 0 red;' src='images/googleearth1.gif' title='View map in GoogleEarth'/></a>"
                                r(9) = dr("lat")
                                r(10) = dr("lon")
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
            End While

            If prevtime <> currenttime Then
                Dim temptime As TimeSpan = currenttime - prevtime
                Dim minutes As Int16 = temptime.TotalMinutes()

                Select Case prevstatus
                    Case "stop"

                    Case "moving"

                    Case "idle"
                        If temptime.Minutes >= minOption Then
                            r = t.NewRow
                            r(0) = i
                            r(1) = plateno
                            r(2) = prevtime.ToString("yyyy-MM-dd HH:mm:ss")
                            r(3) = currenttime.ToString("yyyy-MM-dd HH:mm:ss")
                            r(4) = temptime
                            totalSpan = totalSpan + temptime
                            If lastlat <> 0 And lastlon <> 0 Then
                                r(5) = locObj.GetLocation(lat, lon)
                                r(6) = locObj.GetNearestTown(lastlat, lastlon)
                            End If
                            r(7) = "<a href='http://maps.google.com/maps?f=q&hl=en&q=" & lastlat & " + " & lastlon & "&om=1&t=k' target='_blank'><img style='border:solid 0 red;' src='images/googlemaps1.gif' title='View map in Google Maps'/></a>   <a href='GoogleEarthMaps.aspx?x=" & lastlon & "&y=" & lastlat & "'><img style='border:solid 0 red;' src='images/googleearth1.gif' title='View map in GoogleEarth'/></a>"
                            r(9) = lastlat
                            r(10) = lastlon
                            '  Response.Write(lastlat & "--" & lastlon & "<br/>")

                            t.Rows.Add(r)
                        End If


                End Select
            End If

            conn.Close()
            conn.Dispose()
            For k As Int32 = 0 To t.DefaultView.Count - 1
                r = t1.NewRow
                r(0) = (k + 1).ToString()
                r(1) = t.DefaultView.Item(k).Item("plateno")
                r(2) = t.DefaultView.Item(k).Item("begindatetime")
                r(3) = t.DefaultView.Item(k).Item("enddatetime")
                r(4) = t.DefaultView.Item(k).Item("duration")
                r(5) = t.DefaultView.Item(k).Item("Address")
                r(6) = t.DefaultView.Item(k).Item("Nearest Town")
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
            r(5) = ""
            r(6) = ""
            r(7) = ""
            r(8) = ""
            r(9) = ""
            r(10) = ""
            r(11) = ""
            t1.Rows.Add(r)

            If t1.DefaultView.Count = 0 Then
                r = t1.NewRow
                r(0) = "--"
                r(1) = "--"
                r(2) = "--"
                r(3) = "--"
                r(4) = "--"
                r(5) = "--"
                r(6) = "--"
                r(7) = "--"
                r(8) = "--"
                r(9) = "--"
                r(10) = "--"
                r(11) = "--"
                t1.Rows.Add(r)
            End If

            ViewState("exceltable") = t1
            GridView1.PageSize = noofrecords.SelectedValue
            GridView1.DataSource = t1

            GridView1.DataBind()


            ec = "true"

            If GridView1.PageCount > 1 Then
                show = True
            End If

            t1.Clear()

            For k As Int32 = 0 To t.DefaultView.Count - 1
                r = t1.NewRow
                r(0) = (k + 1).ToString()
                r(1) = t.DefaultView.Item(k).Item("plateno")
                r(2) = t.DefaultView.Item(k).Item("begindatetime")
                r(3) = t.DefaultView.Item(k).Item("enddatetime")
                r(4) = t.DefaultView.Item(k).Item("duration")
                r(5) = t.DefaultView.Item(k).Item("Address")
                r(6) = t.DefaultView.Item(k).Item("Nearest Town")
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
            r(5) = ""
            r(6) = ""
            r(7) = ""
            r(8) = ""
            r(9) = ""
            r(10) = ""
            r(11) = ""
            t1.Rows.Add(r)

            Session.Remove("exceltable")
            Session.Remove("exceltable2")
            Session.Remove("exceltable3")
            Session.Remove("excelchart")
            Session("exceltable") = t1
            Try
                t1.Dispose()
                t.Dispose()
                r.Table.Dispose()
                dr.Close()
                cmd.Dispose()
                GridView1.Dispose()
            Catch ex As Exception

            End Try


        Catch ex As SystemException
            Response.Write(ex.Message)
        End Try
    End Sub



    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridView1.PageIndexChanging

        GridView1.PageSize = noofrecords.SelectedValue
        GridView1.DataSource = ViewState("exceltable")
        GridView1.PageIndex = e.NewPageIndex
        GridView1.DataBind()

        ec = "true"
        show = True
        'DisplayDateWise()
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridView1.RowDataBound
        Try
            If e.Row.RowType = DataControlRowType.DataRow Then
                'Dim minutesArray As String() = CStr(e.Row.Cells(3).Text).Split(" ")
                If Double.TryParse(e.Row.Cells(0).Text, 0) = False Then
                    'e.Row.Cells.Remove(e.Row.Cells(0))
                    'e.Row.Cells(0).ColumnSpan = 3
                    e.Row.Style.Add("background-color", "darkseagreen")
                    e.Row.Style.Add("color", "BLACK")
                    e.Row.Style.Add("font-weight", "Bold")
                    e.Row.Style.Add("BORDER-TOP", "BLACK 3px solid")
                    e.Row.Style.Add("BORDER-BOTTOM", "BLACK 3px solid")

                    'e.Row.Cells(3).Style.Add("BORDER-RIGHT", "#aaccee 10px solid")
                    'e.Row.Cells(3).Style.Add("padding-left", "50px")
                    'If e.Row.Cells(0).Text <> "--" Then
                    '    If CDbl(minutesArray(0)) > 600 Then
                    '        e.Row.Style.Add("color", "BLACK")
                    '    End If
                    'End If
                    'e.Row.Cells(0).Text = "Daily Trip Summary on " & ddlpleate.SelectedValue
                    'ElseIf CDbl(minutesArray(0)) > 240 And e.Row.Cells(0).Text <> "" Then
                    '    e.Row.Style.Add("background-color", "FIREBRICK")
                    '    e.Row.Style.Add("color", "WHITE")
                End If
            End If
            If e.Row.RowType = DataControlRowType.Footer Then
                e.Row.Style.Add("BORDER-BOTTOM", "BLACK 5px double")
                'e.Row.Cells(3).Text = "Total"
                'e.Row.Cells(4).Text = "SUM"
                'e.Row.Cells(5).Text = CDbl(FooterOdometer).ToString("0.00") & " KM"
                'e.Row.Cells(6).Text = CDbl(FooterFuelConsumption).ToString("0.00") & " L"
                'e.Row.Cells(7).Text = CDbl(FooterFuelCost).ToString("0.00") & " MYR"
                'e.Row.Cells(8).Text = CDbl(FooterFuelConsumption / FooterOdometer).ToString("0.00")
                'e.Row.Cells(9).Text = CDbl(FooterOdometer / FooterFuelConsumption).ToString("0.00")
                'e.Row.Cells(10).Text = CDbl(FooterFuelCost / FooterOdometer).ToString("0.00")
            End If
        Catch ex As SystemException
            Response.Write(ex.Message)
        End Try
    End Sub






    Protected Sub ddlUsername_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlUsername.SelectedIndexChanged
        getPlateNo(ddlUsername.SelectedValue)
    End Sub

    Protected Sub ImageButton1_Click(sender As Object, e As System.EventArgs) Handles ImageButton1.Click
        DisplayIdlingInformation()
    End Sub
End Class


