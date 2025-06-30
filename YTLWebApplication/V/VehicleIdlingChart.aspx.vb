Imports System.Data.SqlClient
Imports ChartDirector

    Partial Class VehicleIdlingChart
    Inherits System.Web.UI.Page
    Public xyvalues As String
    Public ilat, ilon As Double
    Public ec As String = "false"
    Public suser As String
    Public sgroup As String
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try

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
            If role = "User" Then
                ddlUsername.Items.Remove("--Select User Name--")
                ddlUsername.SelectedValue = userid
                getPlateNo(userid)
            End If
            conn.Close()

        Catch ex As Exception


        End Try
        MyBase.OnInit(e)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Page.IsPostBack = False Then
                ImageButton1.Attributes.Add("onclick", "return mysubmit()")
                txtBeginDate.Value = Now().ToString("yyyy/MM/dd")
                txtEndDate.Value = Now().ToString("yyyy/MM/dd")
                Dim cmd As SqlCommand
                Dim dr As SqlDataReader
                Dim userid As String = Request.QueryString("u")
                Dim plateno As String = Request.QueryString("p")

                If userid.IndexOf(",") > 0 Then
                    Dim sgroupname As String() = userid.Split(",")
                    suser = sgroupname(0)
                    sgroup = sgroupname(1)
                End If
              

                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                If suser <> "" Then
                    cmd = New SqlCommand("select plateno from vehicleTBL where userid='" & suser & "' order by plateno", conn)
                Else
                    cmd = New SqlCommand("select plateno from vehicleTBL where userid='" & userid & "' order by plateno", conn)
                End If
                conn.Open()
                dr = cmd.ExecuteReader()
                While dr.Read()
                    ddlpleate.Items.Add(New ListItem(dr("plateno"), dr("plateno")))
                End While
                dr.Close()
                If suser <> "" Then
                    ddlUsername.SelectedValue = suser
                Else
                    ddlUsername.SelectedValue = userid
                End If
                ddlpleate.SelectedValue = Request.QueryString("p")

                conn.Close()
                If (plateno <> "") Then
                    Dim begindatetime As String = Request.QueryString("bdt")
                    Dim enddatetime As String = Request.QueryString("edt")
                    txtBeginDate.Value = DateTime.Parse(begindatetime).ToString("yyyy/MM/dd")
                    txtEndDate.Value = DateTime.Parse(enddatetime).ToString("yyyy/MM/dd")
                    ddlUsername.SelectedValue = userid
                    ddlpleate.SelectedValue = plateno

                    DisplayChart(plateno, begindatetime, enddatetime, ddlUsername.SelectedValue)
                End If

            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ImageButton1.Click
        Try
            Dim plateno As String = ddlpleate.SelectedValue
            Dim begindatetime As String = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
            Dim enddatetime As String = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":00"
            Dim userid As String = ddlUsername.SelectedValue

            DisplayChart(plateno, begindatetime, enddatetime, userid)

        Catch ex As Exception

        End Try
    End Sub

    Protected Sub DisplayChart(ByVal plateno As String, ByVal begindatetime As String, ByVal enddatetime As String, ByVal userid As String)
        Try

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand = New SqlCommand("select distinct convert(varchar(19),timestamp,120) as datetime,speed,ignition,lon,lat from vehicle_history2 where plateno ='" & plateno & "' and (gps_av='A' or (gps_av='V' and ignition='0')) and  timestamp between '" & begindatetime & "' and '" & enddatetime & "'", conn)

            Dim stoptime As TimeSpan = New TimeSpan(0, 0, 0)
            Dim ideltime As TimeSpan = New TimeSpan(0, 0, 0)
            Dim movingtime As TimeSpan = New TimeSpan(0, 0, 0)

            Dim datavalues() As Double = {}
            Dim labelsvalues() As String = {}
            Dim colorsvalues() As Integer = {}

            Dim lon() As Double = {}
            Dim lat() As Double = {}

            Dim plon As Double
            Dim plat As Double

            Dim stopsstring As String = "Stops" & Environment.NewLine
            Dim idelstring As String = "idel" & Environment.NewLine
            Dim movingstring As String = "moving" & Environment.NewLine

            Dim imagestatus As String = "no"
            Dim prevstatus As String = "stop"
            Dim prevtime As DateTime = DateTime.Parse(begindatetime)

            Dim currentstatus As String = "stop"
            Dim currenttime As DateTime = DateTime.Parse(begindatetime)


            Dim temptime As TimeSpan = New TimeSpan(0, 0, 0)
            Dim idling As TimeSpan = New TimeSpan(0, ddlidling.SelectedValue, 0)

            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()

            Dim i As Integer = 0
            Dim enter As String = "no"


            While dr.Read()


                imagestatus = "yes"

                If enter = "no" Then

                    enter = "Yes"
                End If
                currenttime = dr("datetime")
                If dr("ignition") = True And dr("speed") <> 0 Then
                    currentstatus = "moving"
                ElseIf dr("ignition") = True And dr("speed") = "0" Then
                    currentstatus = "idle"
                Else
                    currentstatus = "stop"
                End If

                If prevstatus <> currentstatus Then
                    temptime = currenttime - prevtime
                    Dim minutes As Int16 = temptime.TotalMinutes()

                    Select Case prevstatus
                        Case "stop"

                        Case "moving"

                        Case "idle"

                            If temptime > New TimeSpan(0, 0, 0) Then

                                If cbxidling.Checked = True Then
                                    If temptime > idling Then
                                        ReDim Preserve datavalues(i)
                                        ReDim Preserve labelsvalues(i)
                                        ReDim Preserve colorsvalues(i)

                                        ReDim Preserve lon(i)
                                        ReDim Preserve lat(i)

                                        datavalues(i) = Math.Round(temptime.TotalMinutes, 2)
                                        labelsvalues(i) = prevtime.ToString("yyyy/MM/dd HH:mm:ss") & " To " & currenttime.ToString("yyyy/MM/dd HH:mm:ss")
                                        colorsvalues(i) = &HBB0000
                                        lon(i) = Math.Round(dr("lon"), 6)
                                        lat(i) = Math.Round(dr("lat"), 6)

                                        i += 1
                                    End If
                                Else
                                    ReDim Preserve datavalues(i)
                                    ReDim Preserve labelsvalues(i)
                                    ReDim Preserve colorsvalues(i)

                                    ReDim Preserve lon(i)
                                    ReDim Preserve lat(i)
                                    datavalues(i) = Math.Round(temptime.TotalMinutes, 2)
                                    labelsvalues(i) = prevtime.ToString("yyyy/MM/dd HH:mm:ss") & " To " & currenttime.ToString("yyyy/MM/dd HH:mm:ss")
                                    If temptime > idling Then
                                        colorsvalues(i) = &H638BBC
                                    Else
                                        colorsvalues(i) = &HFF
                                    End If
                                    lon(i) = Math.Round(dr("lon"), 6)
                                    lat(i) = Math.Round(dr("lat"), 6)

                                    i += 1
                                End If
                            End If
                            plon = Math.Round(dr("lon"), 6)
                            plat = Math.Round(dr("lat"), 6)
                    End Select

                    prevtime = currenttime
                    prevstatus = currentstatus
                End If

            End While
            If prevtime <> currenttime Then



                temptime = currenttime - prevtime
                Dim minutes As Int16 = temptime.TotalMinutes()

                Select Case prevstatus
                    Case "stop"

                    Case "moving"

                    Case "idle"

                        If temptime > New TimeSpan(0, 0, 0) Then

                            If cbxidling.Checked = True Then
                                If temptime > idling Then
                                    ReDim Preserve datavalues(i)
                                    ReDim Preserve labelsvalues(i)
                                    ReDim Preserve colorsvalues(i)

                                    ReDim Preserve lon(i)
                                    ReDim Preserve lat(i)

                                    datavalues(i) = Math.Round(temptime.TotalMinutes, 2)
                                    labelsvalues(i) = prevtime.ToString("yyyy/MM/dd HH:mm:ss") & " To " & currenttime.ToString("yyyy/MM/dd HH:mm:ss")
                                    colorsvalues(i) = &HBB0000
                                    lon(i) = Math.Round(plon, 6)
                                    lat(i) = Math.Round(plat, 6)
                                End If
                            Else
                                ReDim Preserve datavalues(i)
                                ReDim Preserve labelsvalues(i)
                                ReDim Preserve colorsvalues(i)

                                ReDim Preserve lon(i)
                                ReDim Preserve lat(i)

                                datavalues(i) = Math.Round(temptime.TotalMinutes, 2)
                                labelsvalues(i) = prevtime.ToString("yyyy/MM/dd HH:mm:ss") & " To " & currenttime.ToString("yyyy/MM/dd HH:mm:ss")
                                If temptime > idling Then
                                    colorsvalues(i) = &H638BBC
                                Else
                                    colorsvalues(i) = &HFF
                                End If
                                lon(i) = Math.Round(plon, 6)
                                lat(i) = Math.Round(plat, 6)

                            End If
                        End If

                End Select


                prevtime = currenttime
                prevstatus = currentstatus
            End If

            If imagestatus = "no" Then
                WebChartViewer1.Visible = False
                Image1.Visible = True
                Image1.ImageUrl = "~/images/NoDataWide.jpg"

            End If

            conn.Close()

            Array.Reverse(datavalues)
            Array.Reverse(labelsvalues)
            Array.Reverse(colorsvalues)


            Dim chight As Int64 = datavalues.Length * 30

            'Create a XYChart object of size 600 x 250 pixels
            Dim c As XYChart = New XYChart(730, chight + 100, &HFFFFFF, 0, 0)

            'Add a title to the chart using Arial Bold Italic font
            c.addTitle(plateno & " Idling Chart", "Verdana", 10) '.setBackground(&H9999FF)


            'Set the plotarea at (180, 30) and of size 400 x 200 pixels. Set the plotarea
            'border, background and grid lines to Transparent
            'c.setPlotArea(120, 40, 410, chight, Chart.Transparent, Chart.Transparent, _
            '    Chart.Transparent, Chart.Transparent, Chart.Transparent)
            c.setPlotArea(290, 50, 410, chight)

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

            ''Set the x axis to Transparent, with labels in dark red (0x663300)
            'c.xAxis().setColors(Chart.Transparent, 0)

            ''Set the y axis and labels to Transparent
            'c.yAxis().setColors(Chart.Transparent, Chart.Transparent)
            ''Dim yMark As Mark = c.yAxis().addMark(maxspeed, &HCC0000, "Max Speed Limit " & maxspeed & " Km/h")
            ''yMark.setLineWidth(2)


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
            WebChartViewer1.ImageMap = c.getHTMLImageMap("", "", showText & toolTip)

            Dim popUp As String = ""
            Dim semi As String = ""
            For i = 0 To UBound(datavalues)
                If i <> 0 Then
                    semi = ";"
                End If
                popUp = popUp & semi & lon(i) & "," & lat(i)

            Next
            'popupInfo.Text = popUp
            stringvalue.Value = popUp
            '--
            If imagestatus = "yes" Then
                Image1.Visible = False
                WebChartViewer1.Visible = True
                ec = "true"
                Session("Chart") = c.makeChart2(0)
            End If

        Catch ex As Exception

        End Try
    End Sub

    Protected Sub ddlUsername_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlUsername.SelectedIndexChanged
        getPlateNo(ddlUsername.SelectedValue)
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

End Class
