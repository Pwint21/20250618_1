Imports System.Data.SqlClient
Imports AspMap
Imports ADODB
Imports System.Data
Imports ChartDirector
Imports ASPNetMultiLanguage
Imports System.Collections.Generic

Partial Class vehicleUsageReportLitePost
    Inherits System.Web.UI.Page
    Public show As Boolean = False
    Public ec As String = "false"
    Public statisticstable As New DataTable
    Public addressFunction As Location
    Public lng As String
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try

            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            Dim cmd As SqlCommand
            Dim dr As SqlDataReader

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role = Request.Cookies("userinfo")("role")
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
            getPlateNo(ddlUsername.SelectedValue)
        Catch ex As Exception

            Response.Write(ex.Message)
        End Try
        MyBase.OnInit(e)

      
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            If Page.IsPostBack = False Then
                ImageButton1.ToolTip = "Submit"
                ImageButton1.Text = "Submit"
                ImageButton1.Attributes.Add("onclick", "return mysubmit()")
                txtBeginDate.Value = Now().AddDays(-1).ToString("yyyy/MM/dd")
                txtEndDate.Value = Now().AddDays(-1).ToString("yyyy/MM/dd")
            End If

        Catch ex As Exception

        End Try
    End Sub

    Protected Sub getPlateNo(ByVal uid As String)
        Try

            If ddlUsername.SelectedValue <> "--Select User Name--" Then
                ddlpleate.Items.Clear()
                ddlpleate.Items.Add(New ListItem("Select Plate No", "--Select Plate No--"))
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
            Else
                ddlpleate.Items.Clear()
                ddlpleate.Items.Add(New ListItem("Select Plate No", "--Select Plate No--"))
            End If
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub

    Protected Sub DisplayInformation()
        'Try


        On Error Resume Next
        Dim userid As String = Request.Cookies("userinfo")("userid")
        Dim startdatetime As String
        Dim enddate As String

        Dim VehicleMomentReslstfinal As New List(Of VehicleMovementResult)
        Dim begindatetime As String = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
        Dim enddatetime As String = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"
        Dim plateno As String = ddlpleate.SelectedValue
        Dim idays As Integer = DateDiff(DateInterval.Day, Convert.ToDateTime(begindatetime), Convert.ToDateTime(enddatetime))

        addressFunction = New Location(userid)

        Dim table As New DataTable

        table.Columns.Add(New DataColumn("No"))
        table.Columns.Add(New DataColumn("Begin"))
        table.Columns.Add(New DataColumn("End"))
        table.Columns.Add(New DataColumn("Duration"))
        table.Columns.Add(New DataColumn("Status"))
        table.Columns.Add(New DataColumn("Address"))
        table.Columns.Add(New DataColumn("Location Name"))
      
        Dim numberofstationaryes As Int16 = 0
        Dim numberofidelings As Int16 = 0
        Dim numberoftravellings As Int16 = -1

        Dim totalstationarytime As TimeSpan = New TimeSpan(0, 0, 0, 0, 0)
        Dim totalidelingtime As TimeSpan = New TimeSpan(0, 0, 0, 0, 0)
        Dim totaltravellingtime As TimeSpan = New TimeSpan(0, 0, 0, 0, 0)

        Dim prevstatus As String = "Traveling"
        Dim presentstatus As String = "Traveling"

        Dim starttime As DateTime
        Dim endtime As DateTime
        Dim presstarttime As DateTime
        Dim presendtime As DateTime
        Dim temptime As TimeSpan

        For iDayCount As Integer = 0 To idays
            startdatetime = DateAdd(DateInterval.Day, iDayCount, CDate(begindatetime))
            enddate = Format(CDate(startdatetime & " 23:59:59"), "yyyy-MM-dd HH:mm:ss")
            startdatetime = Format(CDate(startdatetime & " 00:00:00"), "yyyy-MM-dd HH:mm:ss")
            Dim VehicleMomentReslst As List(Of VehicleMovementResult) = Getmovementdetails(plateno, Convert.ToDateTime(startdatetime), Convert.ToDateTime(enddate))


            If VehicleMomentReslst.Count > 0 Then
                For Each vehmomres As VehicleMovementResult In VehicleMomentReslst
                    If vehmomres.Status = MovementStatus.Stopm Then
                        VehicleMomentReslst.Remove(vehmomres)
                    Else
                        Exit For
                    End If
                Next
            End If

            If VehicleMomentReslst.Count > 0 Then
                For i As Integer = VehicleMomentReslst.Count - 1 To 0 Step -1
                    If VehicleMomentReslst(i).Status = MovementStatus.Stopm Then
                        VehicleMomentReslst.Remove(VehicleMomentReslst(i))
                    Else
                        Exit For
                    End If
                Next
            End If
            If VehicleMomentReslst.Count > 0 Then
                For i As Integer = 0 To VehicleMomentReslst.Count
                    If i = 0 Then
                        presentstatus = VehicleMomentReslst(i).Status
                        prevstatus = VehicleMomentReslst(i).Status
                        starttime = VehicleMomentReslst(i).StartDateTime
                        endtime = VehicleMomentReslst(i).EndDateTime
                        presstarttime = VehicleMomentReslst(i).StartDateTime
                        presendtime = VehicleMomentReslst(i).EndDateTime
                        temptime = VehicleMomentReslst(i).Duration
                    Else
                        presentstatus = VehicleMomentReslst(i).Status
                        If prevstatus = presentstatus Then
                            endtime = VehicleMomentReslst(i).EndDateTime
                            temptime = temptime + VehicleMomentReslst(i).Duration
                        Else
                            Dim vmres As VehicleMovementResult = New VehicleMovementResult()
                            vmres.StartDateTime = starttime
                            vmres.EndDateTime = endtime
                            vmres.Status = prevstatus
                            vmres.Duration = temptime
                            vmres.ToLat = VehicleMomentReslst(i - 1).ToLat
                            vmres.ToLon = VehicleMomentReslst(i - 1).ToLon
                            vmres.FromLat = VehicleMomentReslst(i - 1).FromLat
                            vmres.FromLon = VehicleMomentReslst(i - 1).FromLon
                            vmres.Mileage = 0
                            VehicleMomentReslstfinal.Add(vmres)
                            starttime = VehicleMomentReslst(i).StartDateTime
                            endtime = VehicleMomentReslst(i).EndDateTime
                            temptime = VehicleMomentReslst(i).Duration
                            prevstatus = presentstatus

                        End If

                    End If
                    If i = VehicleMomentReslst.Count Then
                        Dim vmres As VehicleMovementResult = New VehicleMovementResult()
                        vmres.StartDateTime = starttime
                        vmres.EndDateTime = endtime
                        vmres.Status = prevstatus
                        vmres.Duration = temptime
                        vmres.ToLat = VehicleMomentReslst(i - 1).ToLat
                        vmres.ToLon = VehicleMomentReslst(i - 1).ToLon
                        vmres.FromLat = VehicleMomentReslst(i - 1).FromLat
                        vmres.FromLon = VehicleMomentReslst(i - 1).FromLon
                        vmres.Mileage = 0
                        VehicleMomentReslstfinal.Add(vmres)
                    End If
                Next
            End If



        Next
       



        Dim rowcount As Integer = 1
        Dim tsp As TimeSpan
        Dim displaytime As String
        For Each vehmomres As VehicleMovementResult In VehicleMomentReslstfinal
            Dim chartWidth As Integer = vehmomres.Duration.TotalMinutes / 1440 * 3600
            Dim chartWidthDisplay As String = ""
            If chartWidth > 300 Then
                chartWidth = 300
                chartWidthDisplay = "More Than 2 Hours"
            End If
            Dim r As DataRow = table.NewRow
            r(0) = rowcount
            r(1) = vehmomres.StartDateTime.ToString("yyyy/MM/dd HH:mm:ss")
            r(2) = vehmomres.EndDateTime.ToString("yyyy/MM/dd HH:mm:ss")

            If vehmomres.Duration.Days > 0 Then
                displaytime = vehmomres.Duration.Days & "," & vehmomres.Duration.Hours.ToString.PadLeft(2, "0"c) & ":" & vehmomres.Duration.Minutes.ToString.PadLeft(2, "0"c) & ":" & vehmomres.Duration.Seconds.ToString.PadLeft(2, "0"c)
            Else
                displaytime = vehmomres.Duration.Hours.ToString.PadLeft(2, "0"c) & ":" & vehmomres.Duration.Minutes.ToString.PadLeft(2, "0"c) & ":" & vehmomres.Duration.Seconds.ToString.PadLeft(2, "0"c)
            End If

            Select Case vehmomres.Status
                Case 0
                    r(3) = "<span style='color:#FF0000; font-weight:bold;'>" & displaytime & "</span>"
                    r(4) = "<span style='color:#FF0000; font-weight:bold;'>Stop</span>"
                    numberofstationaryes += 1
                    totalstationarytime += vehmomres.Duration
                    Dim FinalAddress As String = addressFunction.GetLocation(vehmomres.FromLat, vehmomres.FromLon)
                    r(5) = "<a rel='balloon1' href='http://maps.google.com/maps?f=q&hl=en&q=" & vehmomres.FromLat & " + " & vehmomres.FromLon & "&om=1&t=k' target='_blank' style='color:black; text-decoration:none;'><span onmouseover='mouseover(" & vehmomres.FromLon & "," & vehmomres.FromLat & ");'>" & FinalAddress & "</span></a>"
                    r(6) = addressFunction.GetNearestTown(vehmomres.FromLat, vehmomres.FromLon)
                    ' r(7) = "<div style='width:" & chartWidth & "px;height:15px;background-color:#FF6633;margin-left:4px;color:white;'>" & chartWidthDisplay & "</div>"
                Case 1
                    r(3) = "<span style='color:#0000FF; font-weight:bold;'>" & displaytime & "</span>"
                    r(4) = "<span style='color:#0000FF; font-weight:bold;'>Idling</span>"
                    numberofidelings += 1
                    totalidelingtime += vehmomres.Duration
                    Dim FinalAddress As String = addressFunction.GetLocation(vehmomres.FromLat, vehmomres.FromLon)
                    r(5) = "<a rel='balloon1' href='http://maps.google.com/maps?f=q&hl=en&q=" & vehmomres.FromLat & " + " & vehmomres.FromLon & "&om=1&t=k' target='_blank' style='color:black; text-decoration:none;'><span onmouseover='mouseover(" & vehmomres.FromLon & "," & vehmomres.FromLat & ");'>" & FinalAddress & "</span></a>"
                    r(6) = addressFunction.GetNearestTown(vehmomres.FromLat, vehmomres.FromLon)
                    'r(7) = "<div style='width:" & chartWidth & "px;height:15px;background-color:#00AAF8;margin-left:4px;color:white;'>" & chartWidthDisplay & "</div>"
                Case 2
                    r(3) = "<span style='color:#01AD01; font-weight:bold;'>" & displaytime & "</span>"
                    r(4) = "<span style='color:#01AD01; font-weight:bold;'>Travelling</span>"
                    numberoftravellings += 1
                    totaltravellingtime += vehmomres.Duration
                    r(5) = "-"
                    r(6) = "-"
                    ' r(7) = "<div style='width:" & chartWidth & "px;height:15px;background-color:#00B88A;margin-left:4px;color:white;'>" & chartWidthDisplay & "</div>"
            End Select
            table.Rows.Add(r)
            rowcount += 1
        Next



        Dim t As New DataTable

        t.Columns.Add(New DataColumn("No of Stationaryes"))
        t.Columns.Add(New DataColumn("Total Stationary Time"))
        t.Columns.Add(New DataColumn("No of Idlings"))
        t.Columns.Add(New DataColumn("Total Idling Time"))

        Dim row As DataRow = t.NewRow
        row(0) = numberofstationaryes

        'If txtEndDate.Value = Now().ToString("yyyy/MM/dd") Then
        '    totalstationarytime = DateTime.Parse(Now.ToString("yyyy-MM-dd HH:mm:ss")) - DateTime.Parse(begindatetime) - totalidelingtime - totaltravellingtime
        'Else
        '    totalstationarytime = DateTime.Parse(enddatetime) - DateTime.Parse(begindatetime) - totalidelingtime - totaltravellingtime
        '    totalstationarytime = totalstationarytime.Add(TimeSpan.Parse("00:00:59"))
        'End If


        If totalstationarytime.Days > 0 Then
            row(1) = totalstationarytime.Days & " Days " & totalstationarytime.Hours & " Hours " & totalstationarytime.Minutes & " Minutes"
        Else
            row(1) = totalstationarytime.Hours & " Hours " & totalstationarytime.Minutes & " Minutes"
        End If
        row(2) = numberofidelings
        If totalidelingtime.Days > 0 Then
            row(3) = totalidelingtime.Days & " Days " & totalidelingtime.Hours & " Hours " & totalidelingtime.Minutes & " Minutes"
        Else
            row(3) = totalidelingtime.Hours & " Hours " & totalidelingtime.Minutes & " Minutes"
        End If
        'row(3) = totalidelingtime.Days
        t.Rows.Add(row)
        'GridView3.Columns(0).HeaderText = Literal12.Text
        'GridView3.Columns(2).HeaderText = Literal13.Text
        'GridView3.Columns(4).HeaderText = Literal14.Text
        Dim tSummary As New DataTable

        tSummary.Columns.Add(New DataColumn("Stops"))
        tSummary.Columns.Add(New DataColumn("Stop Summary"))

        tSummary.Columns.Add(New DataColumn("Idle"))
        tSummary.Columns.Add(New DataColumn("Idle Summary"))

        tSummary.Columns.Add(New DataColumn("Travel"))
        tSummary.Columns.Add(New DataColumn("Travel Summary"))

        Dim rowSummary As DataRow = tSummary.NewRow

        rowSummary(0) = "Number of Stops"
        rowSummary(1) = numberofstationaryes
        rowSummary(2) = "Number of Idles"
        rowSummary(3) = numberofidelings
        rowSummary(4) = "Number of Travels"
        If numberoftravellings = -1 Then
            rowSummary(5) = 0
        Else
            rowSummary(5) = numberoftravellings
        End If

        tSummary.Rows.Add(rowSummary)

        rowSummary = tSummary.NewRow
        rowSummary(0) = "Total Stop Time"

        If totalstationarytime.Days > 0 Then
            rowSummary(1) = totalstationarytime.Days & " Days " & totalstationarytime.Hours & " Hours " & totalstationarytime.Minutes & " Minutes"
        Else
            rowSummary(1) = totalstationarytime.Hours & " Hours " & totalstationarytime.Minutes & " Minutes"
        End If

        rowSummary(2) = "Total Idle Time"
        If totalidelingtime.Days > 0 Then
            rowSummary(3) = totalidelingtime.Days & " Days " & totalidelingtime.Hours & " Hours " & totalidelingtime.Minutes & " Minutes"
        Else
            rowSummary(3) = totalidelingtime.Hours & " Hours " & totalidelingtime.Minutes & " Minutes"
        End If
        rowSummary(4) = "Total Travel Time"
        If totaltravellingtime.Days > 0 Then
            rowSummary(5) = totaltravellingtime.Days & " Days " & totaltravellingtime.Hours & " Hours " & totaltravellingtime.Minutes & " Minutes"
        Else
            rowSummary(5) = totaltravellingtime.Hours & " Hours " & totaltravellingtime.Minutes & " Minutes"
        End If

        Dim averageStopTime, aveargeIdlingTime, avearageTravellingTime As TimeSpan

        averageStopTime = New TimeSpan(0, 0, totalstationarytime.TotalSeconds / numberofstationaryes)
        aveargeIdlingTime = New TimeSpan(0, 0, totalidelingtime.TotalSeconds / numberofidelings)
        avearageTravellingTime = New TimeSpan(0, 0, totaltravellingtime.TotalSeconds / numberoftravellings)

        tSummary.Rows.Add(rowSummary)

        rowSummary = tSummary.NewRow
        rowSummary(0) = "Avg Stop Time"
        If averageStopTime.Days > 0 Then
            rowSummary(1) = averageStopTime.Days & " Days " & averageStopTime.Hours & " Hours " & averageStopTime.Minutes & " Minutes"
        ElseIf averageStopTime.Hours > 0 Then
            rowSummary(1) = averageStopTime.Hours & " Hours " & averageStopTime.Minutes & " Minutes"
        Else
            rowSummary(1) = averageStopTime.Minutes & " Minutes"
        End If
        rowSummary(2) = "Avg Idle Time"
        If aveargeIdlingTime.Days > 0 Then
            rowSummary(3) = aveargeIdlingTime.Days & " Days " & aveargeIdlingTime.Hours & " Hours " & aveargeIdlingTime.Minutes & " Minutes"
        ElseIf aveargeIdlingTime.Hours > 0 Then
            rowSummary(3) = aveargeIdlingTime.Hours & " Hours " & aveargeIdlingTime.Minutes & " Minutes"
        Else
            rowSummary(3) = aveargeIdlingTime.Minutes & " Minutes"
        End If
        rowSummary(4) = "Avg Travel Time"
        If avearageTravellingTime.Days > 0 Then
            rowSummary(5) = avearageTravellingTime.Days & " Days " & avearageTravellingTime.Hours & " Hours " & avearageTravellingTime.Minutes & " Minutes"
        ElseIf avearageTravellingTime.Hours > 0 Then
            rowSummary(5) = avearageTravellingTime.Hours & " Hours " & avearageTravellingTime.Minutes & " Minutes"
        Else
            rowSummary(5) = avearageTravellingTime.Minutes & " Minutes"
        End If
        tSummary.Rows.Add(rowSummary)

        rowSummary = tSummary.NewRow
        rowSummary(0) = "% Stop Time"
        rowSummary(1) = CDbl((totalstationarytime.TotalMinutes / (totalstationarytime.TotalMinutes + totalidelingtime.TotalMinutes + totaltravellingtime.TotalMinutes)) * 100).ToString("00") & "%"
        rowSummary(2) = "% Idle Time"
        rowSummary(3) = CDbl((totalidelingtime.TotalMinutes / (totalstationarytime.TotalMinutes + totalidelingtime.TotalMinutes + totaltravellingtime.TotalMinutes)) * 100).ToString("00") & "%"
        rowSummary(4) = "% Travel Time"
        rowSummary(5) = CDbl((totaltravellingtime.TotalMinutes / (totalstationarytime.TotalMinutes + totalidelingtime.TotalMinutes + totaltravellingtime.TotalMinutes)) * 100).ToString("00") & "%"
        tSummary.Rows.Add(rowSummary)

        If tSummary.Rows.Count = 0 Then
            row = tSummary.NewRow
            row(0) = "-"
            row(1) = "-"
            row(2) = "-"
            row(3) = "-"
            row(4) = "-"
            row(5) = "-"
            tSummary.Rows.Add(row)

        End If

        If t.Rows.Count = 0 Then
            row = t.NewRow
            row(0) = "-"
            row(1) = "-"
            row(2) = "-"
            row(3) = "-"
            t.Rows.Add(row)
        End If

        If table.Rows.Count = 0 Then
            row = table.NewRow
            row(0) = "-"
            row(1) = "-"
            row(2) = "-"
            row(3) = "-"
            row(4) = "-"
            table.Rows.Add(row)
        End If

        GridView1.PageSize = 5000
        ec = "true"
        GridView1.DataSource = table
        GridView1.DataBind()

        If GridView1.PageCount > 1 Then
            show = True
        End If

        GridView3.DataSource = tSummary
        GridView3.DataBind()

        'DisplayFullMovementChart(totalstationarytime, totalidelingtime, totaltravellingtime)

        table.Columns.Remove(table.Columns("Location Name"))
        table.Columns.Remove(table.Columns("Bar"))


        'table.Columns(0).ColumnName = Literal16.Text
        'table.Columns(1).ColumnName = Literal17.Text
        'table.Columns(2).ColumnName = Literal18.Text
        'table.Columns(3).ColumnName = Literal15.Text
        'table.Columns(4).ColumnName = Literal20.Text
        'table.Columns(5).ColumnName = Literal19.Text
        Session.Remove("exceltable")
        Session.Remove("exceltable2")
        Session.Remove("exceltable3")
        'tSummary.Columns(0).ColumnName = Literal12.Text
        'tSummary.Columns(1).ColumnName = Literal26.Text
        'tSummary.Columns(2).ColumnName = Literal13.Text
        'tSummary.Columns(3).ColumnName = Literal27.Text
        'tSummary.Columns(4).ColumnName = Literal14.Text
        'tSummary.Columns(5).ColumnName = Literal28.Text
        Session("exceltable") = tSummary
        Session("exceltable2") = table

        GridView1.Dispose()
        GridView2.Dispose()
        GridView3.Dispose()
        table.Dispose()
        ' vehiclehistorytable.Dispose()
        row.Table.Dispose()
        t.Clear()
        t.Dispose()
        tSummary.Dispose()
      
    End Sub



   
    Protected Sub DisplayDateWise()
        Try
            'On Error Resume Next
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim plateno As String = ddlpleate.SelectedValue


            Dim begindate As Date = Date.Parse(txtBeginDate.Value.ToString())
            Dim enddate As Date = Date.Parse(txtEndDate.Value.ToString())
            Dim tempdate As Date = begindate

            statisticstable.Columns.Add(New DataColumn("Date"))
            statisticstable.Columns.Add(New DataColumn("SOne"))
            statisticstable.Columns.Add(New DataColumn("STwo"))
            statisticstable.Columns.Add(New DataColumn("SThree"))
            statisticstable.Columns.Add(New DataColumn("SFour"))
            statisticstable.Columns.Add(New DataColumn("STTime"))
            statisticstable.Columns.Add(New DataColumn("IOne"))
            statisticstable.Columns.Add(New DataColumn("ITwo"))
            statisticstable.Columns.Add(New DataColumn("IThree"))
            statisticstable.Columns.Add(New DataColumn("IFour"))
            statisticstable.Columns.Add(New DataColumn("ITTime"))


            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            While tempdate <= enddate

                Dim da As SqlDataAdapter = New SqlDataAdapter("select distinct convert(varchar(19),timestamp,120) as datetime,lat,lon,speed,ignition_sensor from vehicle_history " &
                "where plateno ='" & plateno & "' and (gps_av='A' or (gps_av='V' and ignition_sensor='0')) and timestamp between '" & Convert.ToDateTime(tempdate).ToString("yyyy/MM/dd") & " 00:00:00' and '" & Convert.ToDateTime(tempdate).ToString("yyyy/MM/dd") & " 23:59:59' order by datetime", conn)

                Dim ds As New Data.DataSet
                da.Fill(ds)

                Dim vehiclehistorytable As DataTable = ds.Tables(0)

                'vehicle statistics variables

                Dim sone As Int16 = 0
                Dim stwo As Int16 = 0
                Dim sthree As Int16 = 0
                Dim sfour As Int16 = 0
                Dim sttime As New TimeSpan(0, 0, 0)

                Dim ione As Int16 = 0
                Dim itwo As Int16 = 0
                Dim ithree As Int16 = 0
                Dim ifour As Int16 = 0
                Dim ittime As New TimeSpan(0, 0, 0)

                Dim prevstatus As String = "Traveling"
                Dim presentstatus As String = "Traveling"

                Dim prevtime As String = ""
                Dim presenttime As String = ""

                Dim r As DataRow = statisticstable.NewRow


                If vehiclehistorytable.Rows.Count > 0 Then
                    prevtime = vehiclehistorytable.Rows(0)("datetime")

                    If (vehiclehistorytable.Rows(0)("ignition_sensor") = 1) And (vehiclehistorytable.Rows(0)("speed") = 0) Then
                        prevstatus = "Ideling"
                    ElseIf (vehiclehistorytable.Rows(0)("ignition_sensor") = 0) Then
                        prevstatus = "Stop"
                    Else
                        prevstatus = "Traveling"
                    End If

                End If

                For i As Int32 = 1 To vehiclehistorytable.Rows.Count - 1     ''Ô­±¾ÊÇvehiclehistorytable.Rows.Count - 2

                    presenttime = vehiclehistorytable.Rows(i)("datetime")

                    If (vehiclehistorytable.Rows(i)("ignition_sensor") = 1) And (vehiclehistorytable.Rows(i)("speed") = 0) Then
                        presentstatus = "Ideling"
                    ElseIf (vehiclehistorytable.Rows(i)("ignition_sensor") = 0) Then
                        presentstatus = "Stop"
                    Else
                        presentstatus = "Traveling"
                    End If


                    If prevstatus <> presentstatus Then
                        'Status Changed

                        Dim tfirstdatetime As DateTime = prevtime
                        Dim tseconddatetime As DateTime = presenttime

                        Dim temptime As TimeSpan = tseconddatetime - tfirstdatetime
                        Dim tminutes As Int32 = temptime.TotalMinutes()

                        If prevstatus = "Stop" Then
                            sttime += temptime
                            Select Case tminutes
                                Case 15 To 29
                                    sone += 1
                                Case 30 To 59
                                    stwo += 1
                                Case 60 To 89
                                    sthree += 1
                                Case 90 To 1440
                                    sfour += 1
                            End Select

                        ElseIf prevstatus = "Ideling" Then
                            ittime += temptime
                            Select Case tminutes
                                Case 15 To 29
                                    ione += 1
                                Case 30 To 59
                                    itwo += 1
                                Case 60 To 89
                                    ithree += 1
                                Case 90 To 1440
                                    ifour += 1
                            End Select
                        Else

                        End If

                        prevtime = presenttime
                        prevstatus = presentstatus
                    End If
                Next

                If prevtime <> presenttime Then
                    Dim tfirstdatetime As DateTime = prevtime
                    Dim tseconddatetime As DateTime = presenttime

                    Dim temptime As TimeSpan = tseconddatetime - tfirstdatetime
                    Dim tminutes As Int32 = temptime.TotalMinutes()

                    If prevstatus = "Stop" Then
                        sttime += temptime
                        Select Case tminutes
                            Case 15 To 29
                                sone += 1
                            Case 30 To 59
                                stwo += 1
                            Case 60 To 89
                                sthree += 1
                            Case 90 To 1440
                                sfour += 1
                        End Select

                    ElseIf prevstatus = "Ideling" Then
                        ittime += temptime
                        Select Case tminutes
                            Case 15 To 29
                                ione += 1
                            Case 30 To 59
                                itwo += 1
                            Case 60 To 89
                                ithree += 1
                            Case 90 To 1440
                                ifour += 1
                        End Select
                    End If
                End If


                r(0) = tempdate.ToShortDateString()
                r(1) = sone
                r(2) = stwo
                r(3) = sthree
                r(4) = sfour
                r(5) = sttime
                r(6) = ione
                r(7) = itwo
                r(8) = ithree
                r(9) = ifour
                r(10) = ittime

                statisticstable.Rows.Add(r)
                tempdate = tempdate.AddDays(1)
            End While

            If statisticstable.Rows.Count = 0 Then
                Dim r As DataRow = statisticstable.NewRow
                r(0) = "-"
                r(1) = "-"
                r(2) = "-"
                r(3) = "-"
                r(4) = "-"
                r(5) = "-"
                r(6) = "-"
                r(7) = "-"
                r(8) = "-"
                r(9) = "-"
                r(10) = "-"
                statisticstable.Rows.Add(r)
            End If

            Session("statisticstable") = statisticstable
            customers.DataSource = statisticstable
            customers.DataBind()


        Catch ex As SystemException
            Response.Write(ex.Message)
        End Try

    End Sub


    Public Function Getmovementdetails(ByVal plateno As String, ByVal fromdate As DateTime, ByVal todate As DateTime) As Generic.List(Of VehicleMovementResult)
        Dim lstmovements As List(Of VehicleMovementResult) = New List(Of VehicleMovementResult)
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Try
            Dim movement As VehicleMovementResult
            Dim cmd As SqlCommand = New SqlCommand("select * from vehicle_movement where plateno='" & plateno & "' and totime between '" & fromdate.ToString("yyyy/MM/dd HH:mm:dd") & "' and '" & todate.ToString("yyyy/MM/dd HH:mm:ss") & "'", conn)
            conn.Open()
            Dim dr As SqlDataReader
            Dim ispan As TimeSpan
            dr = cmd.ExecuteReader()
            While (dr.Read())
                movement = New VehicleMovementResult()
                movement.StartDateTime = Convert.ToDateTime(dr("fromtime")).ToString("yyyy/MM/dd HH:mm:ss")
                movement.EndDateTime = Convert.ToDateTime(dr("totime")).ToString("yyyy/MM/dd HH:mm:ss")
                movement.FromLat = Convert.ToSingle(dr("frmlat"))
                movement.ToLat = Convert.ToSingle(dr("tolat"))
                movement.FromLon = Convert.ToSingle(dr("frmlon"))
                movement.ToLon = Convert.ToSingle(dr("tolon"))
                ispan = TimeSpan.FromSeconds(dr("duration"))
                movement.Duration = ispan
                movement.Status = dr("type")
                lstmovements.Add(movement)
            End While
        Catch ex As Exception
            Response.Write(ex.Message)
        Finally
            conn.Close()
        End Try
        Return lstmovements
    End Function

    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        GridView1.PageSize = 5000
        GridView1.PageIndex = e.NewPageIndex

        GridView1.DataSource = Session("exceltable")
        GridView1.DataBind()

        ec = "true"
        show = True

        customers.DataSource = Session("statisticstable")
        customers.DataBind()
    End Sub

    Protected Sub ddlUsername_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlUsername.SelectedIndexChanged
        getPlateNo(ddlUsername.SelectedValue)
    End Sub

    Protected Sub ImageButton1_Click(sender As Object, e As System.EventArgs) Handles ImageButton1.Click
        DisplayInformation()
    End Sub
    Public Structure VehicleMovementResult
        Public StartDateTime As DateTime
        Public EndDateTime As DateTime
        Public Duration As TimeSpan
        Public Mileage As Single
        Public Status As MovementStatus
        Public FromLat As Single
        Public FromLon As Single
        Public ToLat As Single
        Public ToLon As Single
    End Structure
    Public Enum MovementStatus As Byte
        Stopm = 0
        Idling = 1
        Travelling = 2
    End Enum
End Class

