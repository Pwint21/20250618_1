Imports System.Data.SqlClient
Imports ChartDirector
Imports System.Data

Partial Class VehicleUsageChart
    Inherits System.Web.UI.Page
    Public xyvalues As String
    Public ilat, ilon As Double
    Public ec As String = "false"
    Public errorAlert As String = "false"

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

            'cmd = New SqlCommand("select driverid,drivername from driver order by drivername", conn)
            'If role = "User" Then
            '    cmd = New SqlCommand("select driverid,drivername from driver where userid='" & userid & "' order by drivername", conn)
            'ElseIf role = "SuperUser" Or role = "Operator" Then
            '    cmd = New SqlCommand("select driverid,drivername from driver where userid in(" & userslist & ") order by drivername", conn)
            'End If

            'dr = cmd.ExecuteReader()
            'While dr.Read()
            '    ddldriver.Items.Add(New ListItem(dr("drivername"), dr("driverid")))
            'End While
            'dr.Close()

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

    Protected Sub DisplayFullMovementChart()
        Try
            'On Error Resume Next
            Dim plateno As String = ddlpleate.SelectedValue
            Dim begindatetime As String = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
            Dim enddatetime As String = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"

            'Read data from database server
           Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            Dim da As SqlDataAdapter = New SqlDataAdapter("select distinct convert(varchar(19),timestamp,120) as datetime,lat,lon,speed,ignition_sensor from vehicle_history " &
            "where plateno ='" & plateno & "' and (gps_av='A' or (gps_av='V' and ignition_sensor='0')) and timestamp between '" & begindatetime & "' and '" & enddatetime & "'", conn)

            Dim ds As Data.DataSet = New Data.DataSet()
            da.Fill(ds)

            Dim vehiclehistorytable As DataTable = ds.Tables(0)

            'vehicle statistics variables

            Dim numberofstationaryes As Int16 = 0
            Dim numberofidelings As Int16 = 0
            Dim numberoftravellings As Int16 = -1

            Dim totalstationarytime As TimeSpan
            Dim totalidelingtime As TimeSpan
            Dim totaltravellingtime As TimeSpan
            Dim totaltime As TimeSpan = New TimeSpan(0, 0, 0)

            Dim prevstatus As String = "Traveling"
            Dim presentstatus As String = "Traveling"

            Dim prevtime As String = DateTime.Now.ToString("yyyy/MM/dd 00:00:00")
            Dim presenttime As String = prevtime
            Dim tempprevtime As String = ""

            Dim templat, templon As Double

            Dim lon() As Double = {}
            Dim lat() As Double = {}

            Dim datavalues() As Double = {}
            Dim labelsvalues() As String = {}
            Dim colorsvalues() As Integer = {}

            Dim imagestatus As String = "no"
            Dim enter As String = "no"

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

            Dim k As Integer = 0

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

                        If temptime.Minutes > 0 Then
                            imagestatus = "yes"
                            ReDim Preserve datavalues(k)
                            ReDim Preserve labelsvalues(k)
                            ReDim Preserve colorsvalues(k)

                            ReDim Preserve lon(k)
                            ReDim Preserve lat(k)

                            datavalues(k) = Math.Round(temptime.TotalMinutes, 2)
                            labelsvalues(k) = tfirstdatetime.ToString("yyyy/MM/dd HH:mm:ss") & " To " & tseconddatetime.ToString("yyyy/MM/dd HH:mm:ss")

                            If prevstatus = "Stop" Then
                                colorsvalues(k) = &HFF6633
                                numberofstationaryes += 1
                                totalstationarytime += temptime
                            ElseIf prevstatus = "Ideling" Then
                                colorsvalues(k) = &H3366FF
                                numberofidelings += 1
                                totalidelingtime += temptime
                            Else
                                colorsvalues(k) = &H998877
                                numberoftravellings += 1
                                totaltravellingtime += temptime
                            End If
                            lat(k) = vehiclehistorytable.Rows(i)("lat")
                            lon(k) = vehiclehistorytable.Rows(i)("lon")
                            k += 1
                        End If
                    End If

                    prevtime = presenttime
                    prevstatus = presentstatus

                End If

                tempprevtime = presenttime
            Next

            '#############################################################################################################
            If Convert.ToDateTime(prevtime) <> Convert.ToDateTime(presenttime) Then
                imagestatus = "yes"
                Dim prevtime1 As DateTime = prevtime
                Dim currenttime1 As DateTime = presenttime

                Dim temptime As TimeSpan = currenttime1 - prevtime1
                Dim minutes As Int16 = temptime.TotalMinutes()

                ReDim Preserve datavalues(k)
                ReDim Preserve labelsvalues(k)
                ReDim Preserve colorsvalues(k)

                ReDim Preserve lon(k)
                ReDim Preserve lat(k)

                datavalues(k) = Math.Round(temptime.TotalMinutes, 2)
                labelsvalues(k) = prevtime1.ToString("yyyy/MM/dd HH:mm:ss") & " To " & currenttime1.ToString("yyyy/MM/dd HH:mm:ss")

                Select Case prevstatus
                    Case "Stop"
                        colorsvalues(k) = &HFF6633
                        numberofstationaryes += 1
                        totalstationarytime += temptime
                    Case "Travelling"
                        colorsvalues(k) = &HB88A
                        numberoftravellings += 1
                        totaltravellingtime += temptime
                    Case "Ideling"
                        colorsvalues(k) = &H3366FF
                        numberofidelings += 1
                        totalidelingtime += temptime
                End Select
            End If
            '#############################################################################################################

            Array.Reverse(datavalues)
            Array.Reverse(labelsvalues)
            Array.Reverse(colorsvalues)

            'The data for the pie chart
            Dim data() As Double = {Math.Round(totalstationarytime.TotalMinutes, 2), Math.Round(totaltravellingtime.TotalMinutes, 2), Math.Round(totalidelingtime.TotalMinutes, 2)}

            'The labels for the pie chart
            Dim labels() As String = {"Stop", "Travelling", "Idling"}

            'Create a PieChart object of size 450 x 240 pixels
            Dim c As PieChart = New PieChart(650, 400, &HF2F2F2)

            'Set the center of the pie at (150, 100) and the radius to 80 pixels
            c.setPieSize(320, 200, 120)

            'Add a title at the bottom of the chart using Arial Bold Italic font
            c.addTitle2(Chart.Top, "<*br*>" & ddlpleate.SelectedValue & " - Vehicle Usage Report", "Arial Bold Italic", 15)

            'Draw the pie in 3D
            c.set3D(30)

            'add a legend box where the top left corner is at (330, 40)
            c.addLegend(540, 320).setBackground(&HF4FDEF)

            'modify the label format for the sectors to $nnnK (pp.pp%)
            c.setLabelFormat("{label} {value} Minutes<*br*>({percent}%)")
            c.setLabelStyle("verdana", 8)

            'Set the pie data and the pie labels
            c.setData(data, labels)
            c.setColor(Chart.DataColor + 0, &HFF6633)
            c.setColor(Chart.DataColor + 1, &HB88A)
            c.setColor(Chart.DataColor + 2, &H3366FF)
            'Explode the 1st sector (index = 0)

            c.setExplode(0, 8)
            c.setExplode(1, 8)
            c.setExplode(2, 8)

            'output the chart
            WebChartViewer1.Image = c.makeWebImage(Chart.PNG)

            'include tool tip for the chart
            WebChartViewer1.ImageMap = c.getHTMLImageMap("", "", "title='{label}: {value} Minutes ({percent}%)'")

            If imagestatus = "yes" Then
                Image1.Visible = False
                WebChartViewer1.Visible = True
                ec = "true"
                Session("chart") = c.makeChart2(0)

            Else
                WebChartViewer1.Visible = False
                Image1.Visible = True
                Image1.ImageUrl = "~/images/NoDataWide.jpg"
            End If
        Catch esp As OutOfMemoryException
            WebChartViewer1.Visible = False
            Response.Write("<script type=""text/javascript"" language=""javascript"">alert (""" & " Record overflow!\n Please choose fewer days for better viewing result." & """)</script>")
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub

    Function timespanToText(ByVal timestampText As TimeSpan) As String
        Dim returnvalue As String = ""
        If timestampText.Days = 0 And timestampText.Hours = 0 And timestampText.Minutes > 0 Then
            returnvalue = timestampText.Minutes & " Minutes "
        ElseIf timestampText.Days = 0 And timestampText.Hours > 0 Then
            returnvalue = timestampText.Hours & " Hours " & timestampText.Minutes & " Minutes "
        Else
            returnvalue = timestampText.Days & " Days " & timestampText.Hours & " Hours " & timestampText.Minutes & " Minutes "
        End If
        Return returnvalue
    End Function

   

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

    Protected Sub ImageButton1_Click(sender As Object, e As System.EventArgs) Handles ImageButton1.Click
        DisplayFullMovementChart()
    End Sub
End Class


