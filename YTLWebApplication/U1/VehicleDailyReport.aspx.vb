Imports System.Data.SqlClient
Imports AspMap
Imports ADODB
Imports System.Data
Partial Class VehicleDailyReport2
    Inherits System.Web.UI.Page
    Public show As Boolean = False
    Public ec As String = "false"
    Public plateno As String
    Public statisticstable As New DataTable
    Dim addressFunction As New Address()

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
            cmd = New SqlCommand("select userid, username from userTBL where role='User' order by username", conn)
            If role = "User" Then
                cmd = New SqlCommand("select userid, username from userTBL where userid='" & userid & "'", conn)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select userid, username from userTBL where userid in (" & userslist & ") order by username", conn)
            End If
            conn.Open()
            dr = cmd.ExecuteReader()
            While dr.Read()
                ddlUsername.Items.Add(New ListItem(dr("username"), dr("userid")))
            End While

            If role = "User" Then
                ddlUsername.Items.Remove("--Select User Name--")
                ddlUsername.SelectedValue = userid
            End If
            dr.Close()
            cmd.Dispose()
            conn.Close()
            conn.Dispose()
            addressFunction.LoadMapLayers()
            addressFunction.LoadSmartPoints()
            addressFunction.LoadPublicGeofence()
        Catch ex As Exception


        End Try
        MyBase.OnInit(e)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            If Page.IsPostBack = False Then
                txtBeginDate.Value = Now().ToString("yyyy/MM/dd")
                txtEndDate.Value = Now().ToString("yyyy/MM/dd")
            End If
            ImageButton1.Attributes.Add("onclick", "return mysubmit()")

        Catch ex As Exception

        End Try
    End Sub



    Protected Sub DisplayIdlingInformation()
        Try

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            Dim cmd As SqlCommand
            Dim dr1 As SqlDataReader
            Dim r As DataRow

            Dim x As Int32 = 1
            Dim stoptemtime As TimeSpan = New TimeSpan(0, 0, 0)
            Dim idletemtime As TimeSpan = New TimeSpan(0, 0, 0)
            Dim movingtemtime As TimeSpan = New TimeSpan(0, 0, 0)
            Dim starttime As String = ""
            Dim endtime As String = ""

            Dim startaddress As String = ""
            Dim endaddress As String = ""
            Dim startlat As Double = 0
            Dim startlon As Double = 0
            Dim endlat As Double = 0
            Dim endlon As Double = 0

            Dim begindatetime As String = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":59"
            Dim enddatetime As String = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"

           

            Dim totalStop As TimeSpan = New TimeSpan(0, 0, 0, 0, 0)
            Dim totalIdling As TimeSpan = New TimeSpan(0, 0, 0, 0, 0)
            Dim totalTraveling As TimeSpan = New TimeSpan(0, 0, 0, 0, 0)
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
            t.Columns.Add(New DataColumn("start Location"))
            t.Columns.Add(New DataColumn("End Location"))

            cmd = New SqlCommand("select plateno from vehicleTBL where userid='" & ddlUsername.SelectedValue & "' order by plateno", conn)

        

            conn.Open()

            dr1 = cmd.ExecuteReader()

            Dim plateno As String = ""
            Dim companyname As String = Request.Cookies("userinfo")("companyname")
            Dim locObj As New Location(ddlUsername.SelectedValue)
           

            While dr1.Read()
                plateno = dr1("plateno")
                Dim da As SqlDataAdapter = New SqlDataAdapter("select distinct convert(varchar(19),timestamp,120) as datetime,speed,ignition_sensor,lat,lon from vehicle_history where plateno ='" & plateno & "' and (gps_av='A' or (gps_av='V' and ignition_sensor='0')) and timestamp between '" & begindatetime & "' and '" & enddatetime & "' order by datetime asc", conn)

                Dim dsIdling As DataSet = New DataSet()
                da.Fill(dsIdling)

                Dim prevstatus As String = "Traveling"
                Dim presentstatus As String = "Traveling"

                Dim prevTime As DateTime
                Dim presenttime As DateTime

                Dim tempprevtime As DateTime
                Dim totalidelingtime As TimeSpan = New TimeSpan(0, 0, 0, 0, 0)

                If dsIdling.Tables(0).Rows.Count > 0 Then
                    prevTime = dsIdling.Tables(0).Rows(0)("datetime")
                    presenttime = prevTime
                    tempprevtime = prevTime
                    starttime = prevTime.ToString("yyyy-MM-dd HH:mm:ss")
                    startlat = dsIdling.Tables(0).Rows(0)("lat")
                    startlon = dsIdling.Tables(0).Rows(0)("lon")
                    If (dsIdling.Tables(0).Rows(0)("ignition_sensor") = 1) And (dsIdling.Tables(0).Rows(0)("speed") = 0) Then ' And dsIdling.Tables(0).Rows(0)("gps_av") = "A" Then
                        prevstatus = "Ideling"
                    ElseIf (dsIdling.Tables(0).Rows(0)("ignition_sensor") = 0) Then
                        prevstatus = "Stop"
                    Else
                        prevstatus = "Traveling"
                    End If
                End If

                For i As Int32 = 1 To dsIdling.Tables(0).Rows.Count - 1
                    presenttime = dsIdling.Tables(0).Rows(i)("datetime")
                    endlat = dsIdling.Tables(0).Rows(i)("lat")
                    endlon = dsIdling.Tables(0).Rows(i)("lon")

                    If (dsIdling.Tables(0).Rows(i)("ignition_sensor") = 1) And (dsIdling.Tables(0).Rows(i)("speed") = 0) Then ' And dsIdling.Tables(0).Rows(i)("gps_av") = "A" Then
                        presentstatus = "Ideling"
                    ElseIf (dsIdling.Tables(0).Rows(i)("ignition_sensor") = 0) Then
                        presentstatus = "Stop"
                    Else
                        presentstatus = "Traveling"
                    End If

                    If prevstatus <> presentstatus Then
                        If prevTime <> tempprevtime Then
                            Dim tfirstdatetime As DateTime = prevTime
                            Dim tseconddatetime As DateTime = tempprevtime 'presenttime
                            Dim temptime As TimeSpan = tseconddatetime - tfirstdatetime

                            If temptime.TotalMinutes >= 1 Then
                                If prevstatus = "Ideling" Then
                                    totalidelingtime += temptime
                                    totalIdling = totalIdling + temptime

                                ElseIf prevstatus = "Stop" Then
                                    'stoptemtime += temptime
                                    'totalStop = totalStop + temptime
                                Else
                                    movingtemtime += temptime
                                    totalTraveling = totalTraveling + temptime
                                End If
                            End If
                        End If

                        prevTime = presenttime
                        prevstatus = presentstatus
                    End If

                    tempprevtime = presenttime
                    endtime = presenttime.ToString("yyyy-MM-dd HH:mm:ss")
                Next

                If prevTime <> presenttime Then
                    Dim tfirstdatetime As DateTime = prevTime
                    Dim tseconddatetime As DateTime = presenttime

                    Dim temptime As TimeSpan = tseconddatetime - tfirstdatetime
                    If prevstatus = "Stop" Then
                        'stoptemtime += temptime
                    ElseIf prevstatus = "Ideling" Then
                        'temptime = temptime.Add(TimeSpan.Parse("00:01:00"))  'purposely add in to match the vehicleidlingreport
                        totalidelingtime += temptime
                        totalIdling = totalIdling + temptime

                    ElseIf prevstatus = "Traveling" Then
                        'temptime = temptime.Add(TimeSpan.Parse("00:01:00"))  'purposely add in to match the vehicleidlingreport
                        movingtemtime += temptime
                        totalTraveling = totalTraveling + temptime
                    End If

                    If dsIdling.Tables(0).Rows.Count > 0 Then
                        endtime = presenttime.ToString("yyyy-MM-dd HH:mm:ss")
                    End If
                Else
                    If dsIdling.Tables(0).Rows.Count > 0 Then
                        endtime = presenttime.ToString("yyyy-MM-dd HH:mm:ss")
                    End If
                End If
                If txtEndDate.Value = Now().ToString("yyyy/MM/dd") Then
                    stoptemtime = DateTime.Parse(Now.ToString("yyyy-MM-dd HH:mm:ss")) - DateTime.Parse(begindatetime) - totalidelingtime - movingtemtime
                Else
                    stoptemtime = DateTime.Parse(enddatetime) - DateTime.Parse(begindatetime) - totalidelingtime - movingtemtime
                    stoptemtime = stoptemtime.Add(TimeSpan.Parse("00:00:59"))
                End If

                totalStop = totalStop + stoptemtime
                r = t.NewRow
                r(0) = x
                r(1) = plateno
                r(2) = starttime
                r(3) = endtime
                r(4) = stoptemtime
                r(5) = totalidelingtime
                r(6) = movingtemtime

                r(7) = locObj.GetLocation(startlat, startlon)
               
                r(8) = locObj.GetLocation(endlat, endlon)

                If r(4) = "00:00:00" And r(5) = "00:00:00" And r(6) = "00:00:00" Then
                    r(4) = "-"
                    r(5) = "-"
                    r(6) = "-"
                End If

                If r(7) Is DBNull.Value Then
                    r(7) = "-"
                    r(9) = "-"
                End If

                If r(8) Is DBNull.Value Then
                    r(8) = "-"
                    r(10) = "-"
                End If

                t.Rows.Add(r)
                x = x + 1

                presenttime = New DateTime
                prevTime = New DateTime
                startlat = 0
                startlon = 0
                endlat = 0
                endlon = 0
                plateno = ""
                starttime = "-"
                endtime = "-"
                startaddress = "-"
                endaddress = "-"
                stoptemtime = New TimeSpan(0, 0, 0)
                totalidelingtime = New TimeSpan(0, 0, 0)
                movingtemtime = New TimeSpan(0, 0, 0)
               
                dsIdling.Dispose()
                da.Dispose()
            End While
            r = t.NewRow
            r(0) = ""
            r(1) = ""
            r(2) = ""
            r(3) = "TOTAL"
            r(4) = totalStop
            r(5) = totalIdling
            r(6) = totalTraveling
            r(7) = ""
            t.Rows.Add(r)


            If t.Rows.Count = 0 Then
                r = t.NewRow
                r(0) = "--"
                r(1) = "--"
                r(2) = "--"
                r(3) = "--"
                r(4) = "--"
                r(5) = "--"
                r(6) = "--"
                r(7) = "--"
                r(8) = "--"

                t.Rows.Add(r)
            End If

            GridView1.PageSize = noofrecords.SelectedValue
            GridView1.DataSource = t
            GridView1.DataBind()
            ec = "true"

            t.Columns.Remove("Start Location")
            t.Columns.Remove("End Location")

            Session.Remove("exceltable")
            Session.Remove("exceltable2")

            Session("exceltable") = t
            GridView1.Dispose()
            conn.Close()
            conn.Dispose()
            cmd.dispose()
            dr1.Close()
            cmd.Dispose()
            r.Table.Dispose()
            t.dispose()
            If GridView1.PageCount > 1 Then
                show = True
            End If
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub


  

    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridView1.PageIndexChanging

        GridView1.PageSize = noofrecords.SelectedValue
        GridView1.DataSource = Session("exceltable")
        GridView1.PageIndex = e.NewPageIndex
        GridView1.DataBind()

        ec = "true"
        show = True

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
    Protected Sub ImageButton1_Click(sender As Object, e As System.EventArgs) Handles ImageButton1.Click
        DisplayIdlingInformation()
    End Sub
End Class

