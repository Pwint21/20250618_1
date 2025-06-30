Imports System.Data.SqlClient
Imports ChartDirector
Imports System.Data

Partial Class VehicleFullMovementChartOhsasFinal
    Inherits System.Web.UI.Page
    Public xyvalues As String
    Public ilat, ilon As Double
    Public ec As String = "false"
    Public errorAlert As String = "false"

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

        Dim cmd As SqlCommand
        Dim dr As SqlDataReader
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

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



            If role = "User" Then
                ddlUsername.Items.Remove("--Select User Name--")
                ddlUsername.SelectedValue = userid
                getPlateNo(userid)
            End If

        Catch ex As Exception

        Finally

            conn.Close()

        End Try
        MyBase.OnInit(e)
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            GridView1.Visible = False
            lblid.Visible = False
            msg.Visible = False
            If Page.IsPostBack = False Then
                ImageButton1.Attributes.Add("onclick", "return mysubmit()")
                txtBeginDate.Value = Now().ToString("yyyy/MM/dd")
                txtEndDate.Value = Now().ToString("yyyy/MM/dd")
            End If

        Catch ex As Exception

        End Try
    End Sub
    Protected Sub getPlateNo(ByVal uid As String)
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

        Try
            If ddlUsername.SelectedValue <> "--Select User Name--" Then
                ddlpleate.Items.Clear()
                ddlpleate.Items.Add("--Select Plate No--")



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
        Finally
            cmd.Dispose()
            dr.Close()
            conn.Dispose()

        End Try
    End Sub

    Protected Sub DisplayFullMovementChart()
        GridView1.Visible = True
        lblid.Visible = True
        msg.Visible = True
        Dim dr As SqlDataReader
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim cmd As SqlCommand
        Dim fr As DataRow
        Dim t As New DataTable
        Try

            Dim plateno As String = ddlpleate.SelectedValue
            Dim begindatetime As String = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
            Dim enddatetime As String = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"

            cmd = New SqlCommand("select distinct convert(varchar(19),timestamp,120) as datetime,speed,ignition_sensor,lon,lat, gps_odometer from vehicle_history where plateno ='" & plateno & "' and timestamp between '" & begindatetime & "' and '" & enddatetime & "' and (gps_av='A' or (gps_av='V' and ignition_sensor=0))", conn)
            conn.Open()
            dr = cmd.ExecuteReader()

            Dim finalTable As New DataTable
            finalTable.Columns.Add(New DataColumn("Status"))
            finalTable.Columns.Add(New DataColumn("duration"))
            finalTable.Columns.Add(New DataColumn("Start"))
            finalTable.Columns.Add(New DataColumn("End"))


            t.Columns.Add(New DataColumn("datetime"))
            t.Columns.Add(New DataColumn("status"))
            t.Columns.Add(New DataColumn("gps_odometer"))
            t.Columns.Add(New DataColumn("lat"))
            t.Columns.Add(New DataColumn("lon"))
            t.Columns.Add(New DataColumn("speed"))

            Dim r As DataRow

            Dim status As Byte = 0

            While dr.Read()
                r = t.NewRow
                r(0) = dr("datetime")
                status = 0
                If dr("ignition_sensor") = "1" Then
                    If dr("speed") = 0 Then
                        status = 1
                    Else
                        status = 2
                    End If
                End If

                r(1) = status
                r(2) = Convert.ToDouble(dr("gps_odometer")) / 100
                r(3) = dr("lat")
                r(4) = dr("lon")
                r(5) = dr("speed")
                t.Rows.Add(r)
            End While
            dr.Close()
            conn.Close()

            Dim stoptime As TimeSpan = New TimeSpan(0, 0, 0)
            Dim idlingtime As TimeSpan = New TimeSpan(0, 0, 0)
            Dim travellingtime As TimeSpan = New TimeSpan(0, 0, 0)
            Dim totaltime As TimeSpan = New TimeSpan(0, 0, 0)

            Dim lon() As Double = {}
            Dim lat() As Double = {}

            Dim datavalues() As Double = {}
            Dim speedvals() As Double = {}
            Dim labelsvalues() As String = {}

            Dim startvalues() As String = {}
            Dim endvalues() As String = {}


            Dim colorsvalues() As Integer = {}
            Dim cols() As Integer = {}


            Dim imagestatus As String = "no"

            Dim prevstatus As Byte = 0
            Dim prevdatetime As DateTime
            Dim currentstatus As Byte = 0
            Dim currentdatetime As DateTime
            Dim currentodometer As Double
            Dim nextodometer As Double
            Dim totalodometer As Double = 0

            Dim temptime As TimeSpan = New TimeSpan(0, 0, 0)

            Dim enter As String = "no"
            Dim i As Integer = 0

            'For j As Int32 = 0 To t.Rows.Count - 2
            '    currentodometer = t.Rows(j)(2)
            '    If (t.Rows(j)(1) <> 0) Then
            '        nextodometer = t.Rows(j + 1)(2)
            '        If currentodometer < nextodometer And nextodometer > 0 Then
            '            t.Rows(j)(1) = 2
            '            totalodometer = totalodometer + (nextodometer - currentodometer)
            '        End If
            '    End If
            'Next
            'GridView1.DataSource = t
            'GridView1.DataBind()
            If t.Rows.Count >= 2 Then

                r = t.NewRow
                r(0) = t.Rows(t.Rows.Count - 1)(0)
                r(1) = 4
                r(2) = t.Rows(t.Rows.Count - 1)(2)
                r(3) = t.Rows(t.Rows.Count - 1)(3)
                r(4) = t.Rows(t.Rows.Count - 1)(4)
                r(5) = t.Rows(t.Rows.Count - 1)(5)
                t.Rows.Add(r)

                imagestatus = "yes"

                prevdatetime = t.Rows(0)(0)
                prevstatus = t.Rows(0)(1)

                For j As Int32 = 1 To t.Rows.Count - 1
                    Try

                        currentdatetime = t.Rows(j)(0)
                        currentstatus = t.Rows(j)(1)

                        If prevstatus <> currentstatus Then
                            ReDim Preserve datavalues(i)
                            ReDim Preserve speedvals(i)
                            ReDim Preserve labelsvalues(i)
                            ReDim Preserve colorsvalues(i)
                            ReDim Preserve cols(i)
                            ReDim Preserve startvalues(i)
                            ReDim Preserve endvalues(i)

                            ReDim Preserve lon(i)
                            ReDim Preserve lat(i)

                            temptime = currentdatetime - prevdatetime
                            speedvals(i) = t.Rows(j)(5)
                            datavalues(i) = Math.Round(temptime.TotalMinutes, 2)
                            startvalues(i) = prevdatetime.ToString("yyyy/MM/dd HH:mm:ss")
                            endvalues(i) = currentdatetime.ToString("yyyy/MM/dd HH:mm:ss")

                            labelsvalues(i) = prevdatetime.ToString("yyyy/MM/dd HH:mm:ss") & " To " & currentdatetime.ToString("yyyy/MM/dd HH:mm:ss")

                            Select Case prevstatus
                                Case 0
                                    If temptime.TotalMinutes > 30 Then
                                        colorsvalues(i) = &HFF0000
                                        cols(i) = "0"
                                        stoptime = stoptime + temptime
                                        totaltime = totaltime + temptime
                                    Else
                                        cols(i) = "1"
                                        colorsvalues(i) = &HFF0000
                                        travellingtime = travellingtime + temptime
                                        totaltime = totaltime + temptime
                                    End If
                                Case 1
                                    If temptime.TotalMinutes > 30 Then
                                        colorsvalues(i) = &HFF
                                        cols(i) = "0"
                                        stoptime = stoptime + temptime
                                        totaltime = totaltime + temptime
                                    Else
                                        cols(i) = "1"
                                        colorsvalues(i) = &HFF
                                        travellingtime = travellingtime + temptime
                                        totaltime = totaltime + temptime
                                    End If


                                Case 2
                                    cols(i) = "1"
                                    colorsvalues(i) = &HFF00
                                    travellingtime = travellingtime + temptime
                                    totaltime = totaltime + temptime
                            End Select

                            lon(i) = t.Rows(j)("lon")
                            lat(i) = t.Rows(j)("lat")
                            i += 1

                            prevdatetime = currentdatetime
                            prevstatus = currentstatus

                        End If

                    Catch ex As Exception
                        ' Response.Write("####" & ex.Message)
                    End Try
                Next

            End If

            Array.Reverse(datavalues)
            Array.Reverse(labelsvalues)
            Array.Reverse(colorsvalues)
            Array.Reverse(cols)
            Array.Reverse(speedvals)
            Array.Reverse(startvalues)
            Array.Reverse(endvalues)
            Dim previs As String = "Rest"
            Dim present As String = "Rest"
            Dim ic As Integer = datavalues.Length - 1
            If cols(ic) = "1" Then
                present = "Travelling"
                previs = "Travelling"
            ElseIf cols(ic) = "0" Then
                present = "Rest"
                previs = "Rest"
            End If

            Dim totaldur As Integer = 0
            Dim starttm As String = ""
            Dim endtm As String = ""

            Dim onceInsert As Boolean = False
            Try
                If datavalues.Length > 0 Then

                    While ic >= 0
                        If cols(ic) = "1" Then
                            present = "Travelling"
                        Else
                            present = "Rest"
                        End If

                        If present <> previs Then
                            fr = finalTable.NewRow()
                            fr(0) = previs
                            fr(1) = totaldur
                            fr(2) = starttm
                            fr(3) = endtm
                            finalTable.Rows.Add(fr)
                            totaldur = datavalues(ic)
                            starttm = endtm
                        Else
                            If Not onceInsert Then
                                starttm = startvalues(ic)
                            End If
                            onceInsert = True

                            totaldur = totaldur + datavalues(ic)
                        End If
                        endtm = endvalues(ic)
                        previs = present
                        ic -= 1
                    End While


                    If previs = present Then
                        fr = finalTable.NewRow()
                        fr(0) = present
                        fr(1) = totaldur
                        fr(2) = starttm
                        fr(3) = endvalues(0)
                        finalTable.Rows.Add(fr)
                    End If
                End If
            Catch ex As Exception
                '  Response.Write(ex.Message)
            End Try
            Dim totalct As Integer = 0

            For ii As Integer = 0 To finalTable.Rows.Count - 1
                Dim ts As Double = finalTable.Rows(ii)(1) / 60
                If ts > 4 And finalTable.Rows(ii)(0) = "Travelling" Then
                    finalTable.Rows(ii)(1) = "<span style='Color:Red;'>" & finalTable.Rows(ii)(1) & "</span>"
                    If ts > 8 Then
                        If ts > 12 Then
                            If ts > 16 Then
                                totalct += 4
                            Else
                                totalct += 3
                            End If
                        Else
                            totalct += 2
                        End If
                    Else
                        totalct += 1
                    End If
                End If
            Next

            lblid.Text = "Total Working Hours >4 (i.e Travelling > 240 mins) is :" & totalct
            GridView1.DataSource = finalTable
            GridView1.DataBind()

            Dim chight As Int64 = datavalues.Length * 30

            'Create a XYChart object of size 600 x 250 pixels
            Dim c As XYChart = New XYChart(730, chight + 110, &HFAFAFA, 0, 0)

            'Add a title to the chart using Arial Bold Italic font
            'c.addTitle(plateno & " Full Movement Chart", "Verdana", 10) '.setBackground(&H9999FF)
            c.addTitle(plateno & " Full Movement Chart (For Ohsas Violation)", "Arial Bold", 15).setBackground(&HFAFAFA)

            ''  c.addText(175, 40, "Travelling : " & travellingtime.ToString() & " (" & (travellingtime.TotalMinutes / totaltime.TotalMinutes * 100).ToString("0.00") & "%)", "Arial Bold", 10).setFontColor(&H9900)
            '' c.addText(375, 40, "Idling : " & idlingtime.ToString() & " (" & (idlingtime.TotalMinutes / totaltime.TotalMinutes * 100).ToString("0.00") & "%)", "Arial Bold", 10).setFontColor(&HFF)
            '' c.addText(545, 40, "Rest : " & stoptime.ToString() & " (" & (stoptime.TotalMinutes / totaltime.TotalMinutes * 100).ToString("0.00") & "%)", "Arial Bold", 10).setFontColor(&HFF0000)

            c.setPlotArea(290, 70, 410, chight, &HF4FDEF)

            'Add a bar chart layer using the given data. Use a gradient color for the bars,
            'where the gradient is from dark green (0x008000) to white (0xffffff)

            Dim layer As BarLayer = c.addBarLayer3(datavalues, colorsvalues) 'c.addBarLayer(datavalues, colorsvalues)

            layer.set3D(6)
            'Set bar shape to circular (cylinder)
            ' layer.setBarShape(Chart.CircleShape)

            'Swap the axis so that the bars are drawn horizontally
            c.swapXY(True)

            ''Set the bar gap to 10%
            layer.setBarGap(0.3)


            'Use the format "US$ xxx millions" as the bar label
            layer.setAggregateLabelFormat(" {value}")

            'Set the bar label font to 10 pts Times Bold Italic/dark red (0x663300)
            layer.setAggregateLabelStyle("Verdana", 8)

            'Set the labels on the x axis
            Dim textbox As ChartDirector.TextBox = c.xAxis().setLabels(labelsvalues)

            'Set the x axis label font to 10pt Arial Bold Italic
            textbox.setFontStyle("Verdana")
            textbox.setFontSize(8)


            'Add a title to the x axis
            c.xAxis().setTitle("Date Time")
            'Add a title to the y axis
            c.yAxis().setTitle("Time (Minutes)")


            'output the chart
            WebChartViewer1.Image = c.makeWebImage(Chart.PNG)


            'Client side Javascript to show detail information "onmouseover"
            Dim showText As String = "onmouseover='showIt({x});'" 'setDIV(""info{x}"", ""visible"",""block"");' "

            'Client side Javascript to hide detail information "onmouseout"
            Dim hideText As String = "" 'setDIV(""info{x}"", ""hidden"",""none"");' "

            Dim toolTip As String = "title='Date Time : {xLabel}  " & Environment.NewLine & "Minutes : {value} Minutes'"
            'include tool tip for the chart
            WebChartViewer1.ImageMap = c.getHTMLImageMap("", "", showText & hideText & toolTip)

            Dim popUp As String = ""
            Dim semi As String = ""
            For i = 0 To datavalues.Length - 1
                If i <> 0 Then
                    semi = ";"
                End If
                popUp = popUp & semi & lon(i) & "," & lat(i)
            Next

            stringvalue.Value = popUp

            If imagestatus = "yes" Then
                Image1.Visible = False
                WebChartViewer1.Visible = True
                ec = "true"
                Session("Chart") = c.makeChart2(0)
            Else
                WebChartViewer1.Visible = False
                Image1.Visible = True
                Image1.ImageUrl = "~/images/NoDataWide.jpg"
            End If


        Catch esp As OutOfMemoryException
            WebChartViewer1.Visible = False
            Response.Write("<script type=""text/javascript"" language=""javascript"">alert (""" & " Record overflow!\n Please choose fewer days for better viewing result." & """)</script>")
        Catch ex As Exception
            '  Response.Write(ex.Message)
        Finally
            dr.Close()
            conn.Close()
            conn.Dispose()
            cmd.Dispose()
            WebChartViewer1.Dispose()
            Image1.Dispose()
            t.Dispose()
        End Try
    End Sub


    Protected Sub ddlUsername_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlUsername.SelectedIndexChanged
        Try
            getPlateNo(ddlUsername.SelectedValue)

            Image1.Visible = False
            WebChartViewer1.Visible = False
            ec = "false"
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub

    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ImageButton1.Click
        Try
            DisplayFullMovementChart()
        Catch ex As SystemException
        End Try
    End Sub
End Class


