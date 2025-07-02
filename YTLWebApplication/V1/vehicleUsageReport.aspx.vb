Imports System.Data.SqlClient
Imports AspMap
Imports ADODB
Imports System.Data
Partial Class vehicleUsageReport
    Inherits System.Web.UI.Page
    Public show As Boolean = False
    Public ec As String = "false"
    Public statisticstable As New DataTable


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
                ImageButton1.Attributes.Add("onclick", "return mysubmit()")
                txtBeginDate.Value = Now().ToString("yyyy/MM/dd")
                txtEndDate.Value = Now().ToString("yyyy/MM/dd")
            End If

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
            Else
                ddlpleate.Items.Clear()
                ddlpleate.Items.Add("--Select User Name--")
            End If
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub



    Protected Sub DisplayInformation()
        'Try
        On Error Resume Next
       
        Dim begindatetime As String = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
        Dim enddatetime As String = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"

        Dim plateno As String = ddlpleate.SelectedValue

        Dim table As New DataTable

        table.Columns.Add(New DataColumn("No"))
        table.Columns.Add(New DataColumn("Begin"))
        table.Columns.Add(New DataColumn("End"))
        table.Columns.Add(New DataColumn("Duration"))
        table.Columns.Add(New DataColumn("Status"))
        table.Columns.Add(New DataColumn("Location Name"))
        table.Columns.Add(New DataColumn("Address"))

        'Read data from database server

        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

        Dim da As SqlDataAdapter = New SqlDataAdapter("select distinct convert(varchar(19),timestamp,120) as datetime,lat,lon,speed,ignition_sensor from vehicle_history " &
        "where plateno ='" & plateno & "' and (gps_av='A' or (gps_av='V' and ignition_sensor='0')) and timestamp between '" & begindatetime & "' and '" & enddatetime & "' order by datetime", conn)

        Dim ds As DataSet = New DataSet()
        da.Fill(ds)

        Dim vehiclehistorytable As DataTable = ds.Tables(0)
        conn.Close()
        conn.Dispose()
        da.Dispose()
        ds.Dispose()

        'vehicle statistics variables

        Dim numberofstationaryes As Int16 = 0
        Dim numberofidelings As Int16 = 0
        Dim numberoftravellings As Int16 = -1

        Dim totalstationarytime As TimeSpan = New TimeSpan(0, 0, 0, 0, 0)
        Dim totalidelingtime As TimeSpan = New TimeSpan(0, 0, 0, 0, 0)
        Dim totaltravellingtime As TimeSpan = New TimeSpan(0, 0, 0, 0, 0)

        Dim prevstatus As String = "Traveling"
        Dim presentstatus As String = "Traveling"

        Dim prevtime As String = ""
        Dim presenttime As String = ""
        Dim tempprevtime As String = ""

        Dim templat, templon As Double
        Dim userid As String = ddlUsername.SelectedValue
        Dim locObj As New Location(userid)
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

        Dim rowcount As Integer = 1

        For i As Int32 = 1 To vehiclehistorytable.Rows.Count - 1

            presenttime = vehiclehistorytable.Rows(i)("datetime")
            templat = vehiclehistorytable.Rows(i)("lat")
            templon = vehiclehistorytable.Rows(i)("lon")

            If (vehiclehistorytable.Rows(i)("ignition_sensor") = 1) And (vehiclehistorytable.Rows(i)("speed") = 0) Then
                presentstatus = "Ideling"
            ElseIf (vehiclehistorytable.Rows(i)("ignition_sensor") = 0) Then
                presentstatus = "Stop"
            Else
                presentstatus = "Traveling"
            End If


            If prevstatus <> presentstatus Then


                If prevtime <> tempprevtime Then

                    Dim tfirstdatetime As DateTime = prevtime
                    Dim tseconddatetime As DateTime = tempprevtime 'presenttime
                    Dim temptime As TimeSpan = tseconddatetime - tfirstdatetime

                    If temptime.TotalMinutes >= 1 Then
                        Dim r As DataRow = table.NewRow
                        r(0) = rowcount
                        r(1) = prevtime
                        r(2) = tempprevtime 'presenttime
                        r(3) = temptime

                        If prevstatus = "Stop" Then
                            r(4) = "Stop"
                            numberofstationaryes += 1
                            'totalstationarytime += temptime
                        ElseIf prevstatus = "Ideling" Then
                            r(4) = "Idling"
                            numberofidelings += 1
                            totalidelingtime += temptime
                        Else
                            r(4) = "Travelling"
                            numberoftravellings += 1
                            totaltravellingtime += temptime
                        End If
                        r(5) = locObj.GetLocation(vehiclehistorytable.Rows(i)("lat"), vehiclehistorytable.Rows(i)("lon"))
                        table.Rows.Add(r)
                        rowcount += 1
                    End If
                End If

                prevtime = presenttime
                prevstatus = presentstatus

            End If

            tempprevtime = presenttime
        Next

        '#############################################################################################################
        If Convert.ToDateTime(prevtime) <> Convert.ToDateTime(presenttime) Then
            'Response.Write("prevtime:" & prevtime & "</br>")
            'Response.Write("presenttime:" & presenttime & "</br>")
            Dim prevtime1 As DateTime = prevtime
            Dim currenttime1 As DateTime = presenttime

            Dim temptime As TimeSpan = currenttime1 - prevtime1
            Dim minutes As Int16 = temptime.TotalMinutes()

            Select Case prevstatus
                Case "Stop"
                    Dim r As DataRow = table.NewRow
                    r(0) = rowcount
                    r(1) = prevtime1.ToString("yyyy-MM-dd HH:mm:ss")
                    r(2) = currenttime1.ToString("yyyy-MM-dd HH:mm:ss")
                    r(3) = temptime
                    r(4) = "Stop"
                    r(5) = locObj.GetLocation(templat, templon)



                    table.Rows.Add(r)

                Case "Travelling"
                    Dim r As DataRow = table.NewRow
                    r(0) = rowcount
                    r(1) = prevtime1.ToString("yyyy-MM-dd HH:mm:ss")
                    r(2) = currenttime1.ToString("yyyy-MM-dd HH:mm:ss")
                    r(3) = temptime
                    r(4) = "Travelling"
                    r(5) = locObj.GetLocation(templat, templon)



                    table.Rows.Add(r)

                Case "Ideling"
                    Dim r As DataRow = table.NewRow
                    r(0) = rowcount
                    r(1) = prevtime1.ToString("yyyy-MM-dd HH:mm:ss")
                    r(2) = currenttime1.ToString("yyyy-MM-dd HH:mm:ss")
                    r(3) = temptime
                    r(4) = "Idling"
                    r(5) = locObj.GetLocation(templat, templon)


                    table.Rows.Add(r)

            End Select
        End If
        '#############################################################################################################

        If prevtime <> presenttime Then
            Dim tfirstdatetime As DateTime = prevtime
            Dim tseconddatetime As DateTime = presenttime

            Dim temptime As TimeSpan = tseconddatetime - tfirstdatetime
            If prevstatus = "Stop" Then
                numberofstationaryes += 1
                'totalstationarytime += temptime
            ElseIf prevstatus = "Ideling" Then
                numberofidelings += 1
                totalidelingtime += temptime
            Else
                numberoftravellings += 1
                totaltravellingtime += temptime
            End If
        End If

        Dim t As New DataTable

        t.Columns.Add(New DataColumn("No of Stationaryes"))
        t.Columns.Add(New DataColumn("Total Stationary Time"))
        t.Columns.Add(New DataColumn("No of Idlings"))
        t.Columns.Add(New DataColumn("Total Idling Time"))

        Dim row As DataRow = t.NewRow
        row(0) = numberofstationaryes
        'row(1) = totalstationarytime

        If txtEndDate.Value = Now().ToString("yyyy/MM/dd") Then
            totalstationarytime = DateTime.Parse(Now.ToString("yyyy-MM-dd HH:mm:ss")) - DateTime.Parse(begindatetime) - totalidelingtime - totaltravellingtime
        Else
            totalstationarytime = DateTime.Parse(enddatetime) - DateTime.Parse(begindatetime) - totalidelingtime - totaltravellingtime
            totalstationarytime = totalstationarytime.Add(TimeSpan.Parse("00:00:59"))
        End If


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

        table.Columns.Remove(table.Columns("Address"))

        Session.Remove("exceltable")
        Session.Remove("exceltable2")
        Session.Remove("exceltable3")

        Session("exceltable") = tSummary
        Session("exceltable2") = table

        GridView1.Dispose()
        GridView2.Dispose()
        GridView3.Dispose()
        table.Dispose()
        vehiclehistorytable.Dispose()
        row.Table.Dispose()
        t.Clear()
        t.Dispose()
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
                "where plateno ='" & plateno & "' and (gps_av='A' or (gps_av='V' and ignition_sensor='0')) and timestamp between '" & Convert.ToDateTime(tempdate).ToString("yyyy/MM/dd") & " 00:00:00' and '" & Convert.ToDateTime(tempdate).ToString("yyyy/MM/dd") & " 23:59:59'", conn)

                Dim ds As New DataSet
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
End Class

