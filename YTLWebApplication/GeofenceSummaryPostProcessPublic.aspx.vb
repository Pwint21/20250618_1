Imports System.Data.SqlClient
Imports System.Data
Imports ADODB
Imports AspMap
Imports System.IO

Partial Class GeofenceSummaryPostProcessPublic
    Inherits System.Web.UI.Page
    Public show As Boolean = False
    Public ec As String = "false"
    Dim sCon As String = System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection")
    Dim suspectTime As String
    Dim GrantOdometer, GrantFuel, GrantPrice, GrandIdlingFuel, GrandIdlingPrice, GrantRefuelLitre, GrantRefuelPrice As Double
    Dim GrandIdlingTime As TimeSpan

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try

            'If Session("login") = Nothing Then
            '    Response.Redirect("Login.aspx")
            'End If

        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
        MyBase.OnInit(e)
    End Sub



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            'If Session("login") = Nothing Then
            '    Response.Redirect("Login.aspx")
            'End If

            Label2.Visible = False
            Label3.Visible = False


            If Page.IsPostBack = False Then
                ImageButton1.Attributes.Add("onclick", "return mysubmit()")
                txtBeginDate.Value = Now().ToString("yyyy/MM/dd")

                txtEndDate.Value = Now().ToString("yyyy/MM/dd")
                populateNode()
                If Request.Cookies("userinfo")("role") = "User" Then
                    tvPlateno.ExpandAll()
                End If
            End If

        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub

    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
        'DisplayFuelInformation()
        DisplayLogInformation1()
    End Sub

    Sub populateNode()
        'On Error Resume Next
        Try
            Dim ds As System.Data.DataSet = getTreeViewData()
            For Each masterRow As DataRow In ds.Tables("user").Rows
                Dim masterNode As New TreeNode(masterRow("username").ToString(), masterRow("userid").ToString())

                tvPlateno.Nodes.Add(masterNode)
                For Each childRow As DataRow In masterRow.GetChildRows("Children")
                    Dim childNode As New TreeNode(childRow("plateno").ToString(), childRow("plateno").ToString())
                    masterNode.ChildNodes.Add(childNode)
                    If Request.Cookies("userinfo")("role") = "User" Then
                        masterNode.Checked = True
                        childNode.Checked = True
                    End If
                Next
            Next
        Catch ex As SystemException
            Response.Write(ex.Message)
        End Try
    End Sub

    Function getTreeViewData() As System.Data.DataSet
        'On Error Resume Next
        Try
            Dim conn As New SqlConnection(sCon)
            Dim daPlateno As SqlDataAdapter
            Dim daUser As SqlDataAdapter

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            Dim ds As System.Data.DataSet = New System.Data.DataSet()

            If role = "Admin" Then
                Dim dsRoute As DataSet = New DataSet()
                daUser = New SqlDataAdapter("select userid,username,dbip from userTBL where role='user' order by username", conn)
                daUser.Fill(dsRoute, "user")
                For x As Int32 = 0 To dsRoute.Tables("user").Rows.Count - 1
                    Dim uid As String = dsRoute.Tables("user").Rows(x)("userid").ToString()

                    Dim daRoute As SqlDataAdapter = New SqlDataAdapter("select * from vehicleTBL where userid='" & uid & "' order by plateno", conn)
                    daRoute.Fill(dsRoute, "vehicle")
                Next
                dsRoute.Relations.Add("Children", dsRoute.Tables("user").Columns("userid"), dsRoute.Tables("vehicle").Columns("userid"))
                Return dsRoute
            ElseIf role = "SuperUser" Or role = "Operator" Then

                daPlateno = New SqlDataAdapter("select * from vehicleTBL where userid in(" & userslist & ") order by plateno", conn)
                daUser = New SqlDataAdapter("select * from userTBL where userid in (" & userslist & ") order by username", conn)
                'If role <> "Admin" Then
            Else 'If role = "User" Then

                daPlateno = New SqlDataAdapter("select * from vehicleTBL where userid='" & userid & "' order by plateno", conn)
                daUser = New SqlDataAdapter("select * from userTBL where userid='" & userid & "' order by username", conn)
            End If

            'Dim ds As System.Data.DataSet = New System.Data.DataSet()
            daPlateno.Fill(ds, "vehicle")
            daUser.Fill(ds, "user")
            ds.Relations.Add("Children", ds.Tables("user").Columns("userid"), ds.Tables("vehicle").Columns("userid"))
            Return ds
        Catch ex As SystemException
            Response.Write("user" & ex.Message)
        End Try
    End Function
    Protected Sub DisplayLogInformation1()
        Dim userid As String = Request.Cookies("userinfo")("userid")
        Dim role As String = Session("role")
        Dim userslist As String = Session("userslist")
        Dim dailyPlateNo, firstdate, lastDate As String
        Dim dailyOdometer, dailyIdlingTime, dailyTripTime As Double
        Dim enterFirst As Boolean = False
        Dim GeofenceFound As Boolean = False
        Dim count As Integer = 0
        ' Dim userid As String = ddlUsername.SelectedValue
        'Dim geofenceinfo As String = ddlGeofenceName.SelectedValue
        'Dim geofencedetails() As String = geofenceinfo.Split(";")
        'Dim lat_array() As String = geofencedetails(2).Split(",")
        'Dim lon_array() As String = geofencedetails(3).Split(",")


        '  Dim begindatetime = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
        '  Dim enddatetime = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"

        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader
        Dim tdr As SqlDataReader

        ' cmd = New SqlCommand("select plateno from vehicleTBL  order by plateno", conn)

        'If role = "User" Then
        '    cmd = New SqlCommand("select plateno,userid,groupname from vehicleTBL where userid='" & userid & "'   order by plateno", conn)
        'ElseIf role = "SuperUser" Or role = "Operator" Then
        '    cmd = New SqlCommand("select plateno,userid,groupname from vehicleTBL where userid in(" & userslist & ")' order by plateno", conn)
        'End If
        Try


            Dim t As New DataTable
            t.Columns.Add(New DataColumn("No"))
            t.Columns.Add(New DataColumn("Plate No"))
            t.Columns.Add(New DataColumn("Begin Date Time"))
            t.Columns.Add(New DataColumn("End Date Time"))
            t.Columns.Add(New DataColumn("Trip Time"))
            t.Columns.Add(New DataColumn("Idling Time"))
            t.Columns.Add(New DataColumn("Mileage"))
            t.Columns.Add(New DataColumn("Geofence"))
            t.Columns.Add(New DataColumn("Maps"))
            t.Columns.Add(New DataColumn("View"))
            t.Columns.Add(New DataColumn("Trail"))
            t.Columns.Add(New DataColumn("User Name"))
            Dim GeofenceTripTable As New DataTable
            GeofenceTripTable.Columns.Add(New DataColumn("startdatetime"))
            GeofenceTripTable.Columns.Add(New DataColumn("enddatetime"))
            GeofenceTripTable.Columns.Add(New DataColumn("ignition_sensor"))
            GeofenceTripTable.Columns.Add(New DataColumn("onlat"))
            GeofenceTripTable.Columns.Add(New DataColumn("onlon"))
            GeofenceTripTable.Columns.Add(New DataColumn("offlat"))
            GeofenceTripTable.Columns.Add(New DataColumn("offlon"))
            GeofenceTripTable.Columns.Add(New DataColumn("geofence"))

            Dim checkedNodes As TreeNodeCollection = tvPlateno.CheckedNodes

            Dim refuelCount As Int32 = 0
            Dim consumptionCount As Int32 = 0
            Dim idlingCount As Int32 = 0
            Dim eventCount As Int32 = 0
            Dim r As DataRow
            Dim dieselPrice As Double
            ' Dim reference As New references()

            For y As Int16 = 0 To checkedNodes.Count - 1
                If checkedNodes.Item(y).Checked = True Then
                    Dim plateno As String = checkedNodes.Item(y).Value
                    Dim begindatetime As String = txtBeginDate.Value & " 00:00:00"
                    Dim enddatetime As String = txtEndDate.Value & " 23:59:59"
                    'Dim begindatetime As String = "2013/08/02 00:00:00"
                    'Dim enddatetime As String = "2013/08/02 23:59:59"
                    Dim da As SqlDataAdapter
                    da = New SqlDataAdapter("select distinct convert(varchar(19),h.timestamp,120) as datetime,h.gps_av,h.ignition_sensor,h.lat,h.lon,h.speed,h.gps_odometer  , v.userid from vehicleTBL v, vehicle_history h where h.plateno =v.plateno and h.plateno ='" & plateno & "' and h.timestamp between '" & begindatetime & "' and '" & enddatetime & "' and h.gps_odometer<>99   order by datetime", conn)
                    'da = New SqlDataAdapter("select  distinct convert(varchar(19),h.timestamp,120) as datetime,h.gps_av,h.ignition_sensor,h.lat,h.lon,h.speed,h.gps_odometer  , v.userid from vehicleTBL v, vehicle_history2 h where h.plateno =v.plateno and v.userid='3445' and v.plateno ='NAX6490' and h.timestamp between '2014/04/15 07:16:00' and '2014-04-15 07:59:08' and h.gps_odometer<>99   order by datetime", conn)

                    Dim dsGeofenceTrip As New DataSet
                    da.Fill(dsGeofenceTrip)
                    Dim GeofenceTripRow As DataRow
                    Dim uid As String = checkedNodes.Item(0).Value
                    Dim OnIgnition As String = ""
                    Dim geofencename As String = ""
                    Dim geofenceComplete As Boolean = False
                    Dim OnDateTime As String = ""
                    Dim currentDateTime As String = ""
                    Dim onlat, onlon, offlat, offlon As Double
                    Dim speed As String
                    Dim odomeeter As String
                    '###########################################################################################################################
                    Dim map As New AspMap.Map()

                    LoadGeofence(map, plateno)

                    Dim vehiclepoint As New Point

                    ' Dim r As DataRow

                    Dim address As String = ""
                    Dim x As Int64 = 1
                    Dim geofencelayer As AspMap.Layer

                    Dim prevgeofenceid As Int32 = 0
                    Dim prevgeofencestatus As Byte = 0
                    Dim prevgeofencename As String = ""
                    Dim currentgeofenceid As Int32 = 0
                    Dim currentgeofencestatus As Byte = 0
                    Dim xlat, ylon As Double
                    geofencelayer = map.Layer("Geofence Layer")
                    Dim gid As Integer

                    If geofencelayer.Recordset.RecordCount > 0 Then

                        For i As Int32 = 0 To dsGeofenceTrip.Tables(0).Rows.Count - 1

                            address = ""

                            vehiclepoint.Y = dsGeofenceTrip.Tables(0).Rows(i)("lat")
                            vehiclepoint.X = dsGeofenceTrip.Tables(0).Rows(i)("lon")

                            Dim rs As AspMap.Recordset

                            'Geofence Checing...
                            rs = Nothing

                            Try
                                rs = geofencelayer.SearchShape(vehiclepoint, SearchMethod.mcPointInPolygon)
                                If Not rs.EOF Then
                                    currentgeofenceid = rs(1)

                                    If (prevgeofenceid <> currentgeofenceid And prevgeofenceid <> 0) Then
                                        currentgeofencestatus = 4
                                        geofenceComplete = True
                                    ElseIf prevgeofencestatus = 1 Or prevgeofencestatus = 2 Then
                                        currentgeofencestatus = 2
                                    Else
                                        currentgeofencestatus = 1
                                    End If
                                    prevgeofencename = rs(map("Geofence Layer").LabelField)

                                Else
                                    'If prevgeofenceid <> currentgeofenceid Then
                                    '    Response.Write(currentgeofenceid & "," & dsGeofenceTrip.Tables(0).Rows(i)("datetime"))

                                    'End If
                                    currentgeofenceid = 0
                                    If prevgeofencestatus = 1 Or prevgeofencestatus = 2 Then
                                        currentgeofencestatus = 3

                                    Else
                                        currentgeofencestatus = 0
                                    End If

                                End If
                            Catch ex As Exception
                                Response.Write(ex.Message)
                            End Try

                            If prevgeofencestatus <> currentgeofencestatus Then
                                Select Case currentgeofencestatus
                                    Case 0
                                        'r(4) = "--"
                                        'r(5) = 0
                                    Case 1
                                        'r(4) = rs(map("Geofence Layer").LabelField) & " - In"
                                        'r(5) = 1
                                        If geofenceComplete = False Then
                                            OnIgnition = dsGeofenceTrip.Tables(0).Rows(i)("ignition_sensor")
                                            OnDateTime = dsGeofenceTrip.Tables(0).Rows(i)("datetime")
                                            gid = currentgeofenceid
                                            onlat = dsGeofenceTrip.Tables(0).Rows(i)("lat")
                                            onlon = dsGeofenceTrip.Tables(0).Rows(i)("lon")
                                            speed = dsGeofenceTrip.Tables(0).Rows(i)("speed")
                                            odomeeter = dsGeofenceTrip.Tables(0).Rows(i)("gps_odometer")
                                            geofenceComplete = True
                                            geofencename = prevgeofencename
                                        End If
                                    Case 2
                                        'r(4) = rs(map("Geofence Layer").LabelField)
                                        'r(5) = 2

                                    Case 3
                                        'r(4) = prevgeofencename & " - Out"
                                        'r(5) = 3



                                        If geofenceComplete = True Then
                                            'If DateDiff(DateInterval.Minute, Convert.ToDateTime(OnDateTime), Convert.ToDateTime(dsGeofenceTrip.Tables(0).Rows(i)("datetime"))) > 5 Then

                                            '  Dim plateno As String = dr("plateno")
                                            Dim starttime As String = OnDateTime
                                            Dim endtime As String = dsGeofenceTrip.Tables(0).Rows(i)("datetime")

                                            '  Dim GeofenceTripRow As DataRow

                                            Dim geofencetripsummary As New RefuelBeta(plateno, starttime, endtime)
                                            Dim rowOdometer As Double = CDbl(geofencetripsummary.fuelOdometerTotal).ToString("0.00")
                                            Dim rowFuelConsumption As Double = CDbl(geofencetripsummary.fuelConsumptionTotal).ToString("0.00")
                                            Dim rowTripTime As Double = DateDiff(DateInterval.Minute, Convert.ToDateTime(starttime), Convert.ToDateTime(endtime))
                                            Dim rowIdlingTime As Double = geofencetripsummary.getIdling(plateno)

                                            Dim totalTimeIdling As Double = Fix(rowIdlingTime)
                                            Dim tMins, fSecs, fHours, fMins As Double
                                            Dim Idling As String
                                            tMins = "0" & Fix(totalTimeIdling / 60)
                                            fSecs = "0" & totalTimeIdling - (tMins * 60)
                                            fHours = "0" & Fix(totalTimeIdling / 3600)
                                            fMins = "0" & tMins - (fHours * 60)
                                            Idling = fHours.ToString("00") & ":" & fMins.ToString("00") & ":" & fSecs.ToString("00")


                                            If enterFirst = False Then
                                                enterFirst = True
                                                dailyPlateNo = plateno
                                                dailyOdometer = dailyOdometer + rowOdometer
                                                dailyIdlingTime = dailyIdlingTime + tMins
                                                dailyTripTime = dailyTripTime + rowTripTime
                                                firstdate = starttime
                                                lastDate = endtime
                                            Else
                                                If dailyPlateNo = plateno Then
                                                    dailyOdometer = dailyOdometer + rowOdometer
                                                    dailyIdlingTime = dailyIdlingTime + tMins
                                                    dailyTripTime = dailyTripTime + rowTripTime
                                                    lastDate = endtime
                                                Else
                                                    firstdate = endtime
                                                    enterFirst = False
                                                    dailyPlateNo = plateno
                                                    dailyOdometer = 0
                                                    dailyIdlingTime = 0
                                                    dailyTripTime = 0
                                                    dailyOdometer = dailyOdometer + rowOdometer
                                                    dailyIdlingTime = dailyIdlingTime + tMins
                                                    dailyTripTime = dailyTripTime + rowTripTime
                                                End If
                                            End If
                                            GeofenceFound = True
                                            count += 1
                                            GeofenceTripRow = t.NewRow
                                            GeofenceTripRow(0) = count
                                            GeofenceTripRow(1) = plateno
                                            GeofenceTripRow(2) = OnDateTime
                                            GeofenceTripRow(3) = dsGeofenceTrip.Tables(0).Rows(i)("datetime")
                                            GeofenceTripRow(4) = rowTripTime
                                            GeofenceTripRow(5) = tMins 'Idling
                                            GeofenceTripRow(6) = rowOdometer
                                            GeofenceTripRow(7) = geofencename
                                            GeofenceTripRow(8) = "<a rel=""balloon3"" href=""GussmannMap.aspx?userid=" & userid & "&x=" & ylon & "&y=" & xlat & """ target=""_blank""><img style=""border:solid 0 red;"" src=""images/gussmannmaps.gif"" title=""View map in Gussmann Maps"" onmouseover='gmapmouseover(" & userid & "," & ylon & "," & xlat & ");'/></a><a href=""http://maps.google.com/maps?f=q&hl=en&q=" & Math.Round(xlat, 6) & " + " & Math.Round(ylon, 4) & "&om=1&t=k"" target=""_blank""><img style=""border:solid 0 red;"" src=""images/googlemaps.gif"" title=""View map in Google Maps""/></a>"
                                            GeofenceTripRow(9) = "<a href=""ChartReport.aspx?plateno=" & plateno & "&begindate=" & Convert.ToDateTime(OnDateTime).ToString("yyyy/MM/dd 00:00:00") & "&enddate=" & Convert.ToDateTime(OnDateTime).ToString("yyyy/MM/dd 23:59:59") & "&rstartdate=" & OnDateTime & "&renddate=" & dsGeofenceTrip.Tables(0).Rows(i - 1)("datetime") & """ target=""_blank"">view</a>"
                                            'GeofenceTripRow(10) = "<a title='Displays this vehicle history on map' href='GMap.aspx?bdt=" & Convert.ToDateTime(starttime).AddMinutes(-30).ToString("yyyy-MM-dd HH:mm:ss") & "&edt=" & Convert.ToDateTime(endtime).AddMinutes(30).ToString("yyy-MM-dd HH:mm:ss") & "&plateno=" & plateno & "&ms=" & "16," & xlat & "," & ylon & "&si=" & "vht" & "&ran=" & (New Random).Next() & "' target='_Blank' style='font-family:Trebuchet MS ;size:11px;color: #5f7afc;'>" & "Trail" & "<a>"
                                            GeofenceTripRow(10) = "<a title='Displays this vehicle history on map' href='gmap.aspx?bdt=" & Convert.ToDateTime(starttime).AddMinutes(-30).ToString("yyyy-MM-dd HH:mm:ss") & "&edt=" & Convert.ToDateTime(endtime).AddMinutes(30).ToString("yyy-MM-dd HH:mm:ss") & "&plateno=" & plateno & "&ms=" & "16," & xlat & "," & ylon & "&si=" & "vht" & "&ran=" & (New Random).Next() & "&MapDirect=" & "Trail" & "' target='_Blank' style='font-family:Trebuchet MS ;size:11px;color: #5f7afc;'>" & "Trail" & "<a>"
                                            GeofenceTripRow(11) = dsGeofenceTrip.Tables(0).Rows(i)("userid")

                                            t.Rows.Add(GeofenceTripRow)
                                            Dim cmd1 As SqlCommand = New SqlCommand("insert into  public_geofence_history (plateno,id,intimestamp,outtimestamp,inlat,inlon,inspeed,outspeed,inodometer,outodometer,outlat,outlon,idlingtime,mileage,userid)values('" & plateno & "','" & gid & "','" & OnDateTime & "','" & dsGeofenceTrip.Tables(0).Rows(i)("datetime") & "','" & onlat & "','" & onlon & "','" & speed & "','" & dsGeofenceTrip.Tables(0).Rows(i)("speed") & "','" & odomeeter & "','" & dsGeofenceTrip.Tables(0).Rows(i)("gps_odometer") & "','" & dsGeofenceTrip.Tables(0).Rows(i)("lat") & "','" & dsGeofenceTrip.Tables(0).Rows(i)("lon") & "','" & tMins & "','" & rowOdometer & "','" & GeofenceTripRow(11) & "')", conn)
                                            Dim res As Integer
                                            ' WriteLog2("Errr22 " & t.Rows.Count)
                                            conn.Open()
                                            Try
                                                res = cmd1.ExecuteNonQuery()

                                            Catch ex As Exception
                                                'Response.Write(ex.Message)
                                            Finally
                                                conn.Close()
                                            End Try


                                            geofenceComplete = False




                                        End If
                                    Case 4
                                        If geofenceComplete = True Then
                                            'If DateDiff(DateInterval.Minute, Convert.ToDateTime(OnDateTime), Convert.ToDateTime(dsGeofenceTrip.Tables(0).Rows(i)("datetime"))) > 5 Then

                                            '  Dim plateno As String = dr("plateno")
                                            Dim starttime As String = OnDateTime
                                            Dim endtime As String = dsGeofenceTrip.Tables(0).Rows(i)("datetime")

                                            '  Dim GeofenceTripRow As DataRow

                                            Dim geofencetripsummary As New RefuelBeta(plateno, starttime, endtime)
                                            Dim rowOdometer As Double = CDbl(geofencetripsummary.fuelOdometerTotal).ToString("0.00")
                                            Dim rowFuelConsumption As Double = CDbl(geofencetripsummary.fuelConsumptionTotal).ToString("0.00")
                                            Dim rowTripTime As Double = DateDiff(DateInterval.Minute, Convert.ToDateTime(starttime), Convert.ToDateTime(endtime))
                                            Dim rowIdlingTime As Double = geofencetripsummary.getIdling(plateno)

                                            Dim totalTimeIdling As Double = Fix(rowIdlingTime)
                                            Dim tMins, fSecs, fHours, fMins As Double
                                            Dim Idling As String
                                            tMins = "0" & Fix(totalTimeIdling / 60)
                                            fSecs = "0" & totalTimeIdling - (tMins * 60)
                                            fHours = "0" & Fix(totalTimeIdling / 3600)
                                            fMins = "0" & tMins - (fHours * 60)
                                            Idling = fHours.ToString("00") & ":" & fMins.ToString("00") & ":" & fSecs.ToString("00")


                                            If enterFirst = False Then
                                                enterFirst = True
                                                dailyPlateNo = plateno
                                                dailyOdometer = dailyOdometer + rowOdometer
                                                dailyIdlingTime = dailyIdlingTime + tMins
                                                dailyTripTime = dailyTripTime + rowTripTime
                                                firstdate = starttime
                                                lastDate = endtime
                                            Else
                                                If dailyPlateNo = plateno Then
                                                    dailyOdometer = dailyOdometer + rowOdometer
                                                    dailyIdlingTime = dailyIdlingTime + tMins
                                                    dailyTripTime = dailyTripTime + rowTripTime
                                                    lastDate = endtime
                                                Else
                                                    firstdate = endtime
                                                    enterFirst = False
                                                    dailyPlateNo = plateno
                                                    dailyOdometer = 0
                                                    dailyIdlingTime = 0
                                                    dailyTripTime = 0
                                                    dailyOdometer = dailyOdometer + rowOdometer
                                                    dailyIdlingTime = dailyIdlingTime + tMins
                                                    dailyTripTime = dailyTripTime + rowTripTime
                                                End If
                                            End If
                                            GeofenceFound = True
                                            count += 1
                                            GeofenceTripRow = t.NewRow
                                            GeofenceTripRow(0) = count
                                            GeofenceTripRow(1) = plateno
                                            GeofenceTripRow(2) = OnDateTime
                                            GeofenceTripRow(3) = dsGeofenceTrip.Tables(0).Rows(i)("datetime")
                                            GeofenceTripRow(4) = rowTripTime
                                            GeofenceTripRow(5) = tMins 'Idling
                                            GeofenceTripRow(6) = rowOdometer
                                            GeofenceTripRow(7) = geofencename
                                            GeofenceTripRow(8) = "<a rel=""balloon3"" href=""GussmannMap.aspx?userid=" & userid & "&x=" & ylon & "&y=" & xlat & """ target=""_blank""><img style=""border:solid 0 red;"" src=""images/gussmannmaps.gif"" title=""View map in Gussmann Maps"" onmouseover='gmapmouseover(" & userid & "," & ylon & "," & xlat & ");'/></a><a href=""http://maps.google.com/maps?f=q&hl=en&q=" & Math.Round(xlat, 6) & " + " & Math.Round(ylon, 4) & "&om=1&t=k"" target=""_blank""><img style=""border:solid 0 red;"" src=""images/googlemaps.gif"" title=""View map in Google Maps""/></a>"
                                            GeofenceTripRow(9) = "<a href=""ChartReport.aspx?plateno=" & plateno & "&begindate=" & Convert.ToDateTime(OnDateTime).ToString("yyyy/MM/dd 00:00:00") & "&enddate=" & Convert.ToDateTime(OnDateTime).ToString("yyyy/MM/dd 23:59:59") & "&rstartdate=" & OnDateTime & "&renddate=" & dsGeofenceTrip.Tables(0).Rows(i - 1)("datetime") & """ target=""_blank"">view</a>"
                                            'GeofenceTripRow(10) = "<a title='Displays this vehicle history on map' href='GMap.aspx?bdt=" & Convert.ToDateTime(starttime).AddMinutes(-30).ToString("yyyy-MM-dd HH:mm:ss") & "&edt=" & Convert.ToDateTime(endtime).AddMinutes(30).ToString("yyy-MM-dd HH:mm:ss") & "&plateno=" & plateno & "&ms=" & "16," & xlat & "," & ylon & "&si=" & "vht" & "&ran=" & (New Random).Next() & "' target='_Blank' style='font-family:Trebuchet MS ;size:11px;color: #5f7afc;'>" & "Trail" & "<a>"
                                            GeofenceTripRow(10) = "<a title='Displays this vehicle history on map' href='gmap.aspx?bdt=" & Convert.ToDateTime(starttime).AddMinutes(-30).ToString("yyyy-MM-dd HH:mm:ss") & "&edt=" & Convert.ToDateTime(endtime).AddMinutes(30).ToString("yyy-MM-dd HH:mm:ss") & "&plateno=" & plateno & "&ms=" & "16," & xlat & "," & ylon & "&si=" & "vht" & "&ran=" & (New Random).Next() & "&MapDirect=" & "Trail" & "' target='_Blank' style='font-family:Trebuchet MS ;size:11px;color: #5f7afc;'>" & "Trail" & "<a>"
                                            GeofenceTripRow(11) = dsGeofenceTrip.Tables(0).Rows(i)("userid")

                                            t.Rows.Add(GeofenceTripRow)

                                            Dim cmd1 As SqlCommand = New SqlCommand("insert into  public_geofence_history (plateno,id,intimestamp,outtimestamp,inlat,inlon,inspeed,outspeed,inodometer,outodometer,outlat,outlon,idlingtime,mileage,userid)values('" & plateno & "','" & gid & "','" & OnDateTime & "','" & dsGeofenceTrip.Tables(0).Rows(i)("datetime") & "','" & onlat & "','" & onlon & "','" & speed & "','" & dsGeofenceTrip.Tables(0).Rows(i)("speed") & "','" & odomeeter & "','" & dsGeofenceTrip.Tables(0).Rows(i)("gps_odometer") & "','" & dsGeofenceTrip.Tables(0).Rows(i)("lat") & "','" & dsGeofenceTrip.Tables(0).Rows(i)("lon") & "','" & tMins & "','" & rowOdometer & "','" & GeofenceTripRow(11) & "')", conn)
                                            Dim res As Integer

                                            conn.Open()
                                            Try
                                                res = cmd1.ExecuteNonQuery()

                                            Catch ex As Exception
                                                ' Response.Write(ex.Message)
                                            Finally
                                                conn.Close()
                                            End Try
                                        End If
                                        geofenceComplete = False
                                        If geofenceComplete = False Then
                                            OnIgnition = dsGeofenceTrip.Tables(0).Rows(i)("ignition_sensor")
                                            OnDateTime = dsGeofenceTrip.Tables(0).Rows(i)("datetime")
                                            gid = currentgeofenceid
                                            onlat = dsGeofenceTrip.Tables(0).Rows(i)("lat")
                                            onlon = dsGeofenceTrip.Tables(0).Rows(i)("lon")
                                            speed = dsGeofenceTrip.Tables(0).Rows(i)("speed")
                                            odomeeter = dsGeofenceTrip.Tables(0).Rows(i)("gps_odometer")
                                            geofenceComplete = True
                                            geofencename = prevgeofencename
                                        End If

                                    Case Else
                                        'r(4) = "Error"
                                        'r(5) = 4
                                End Select
                            Else
                                Select Case currentgeofencestatus
                                    Case 0
                                        'r(4) = "--"
                                        'r(5) = 0
                                    Case 1
                                        'r(4) = "--"
                                        'r(5) = 0
                                    Case 2
                                        'r(4) = rs(map("Geofence Layer").LabelField)
                                        'r(5) = 2
                                    Case 3
                                        'r(4) = "--"
                                        'r(5) = 0
                                    Case Else
                                        'r(4) = "Error"
                                        'r(5) = 4
                                End Select
                            End If

                            prevgeofencestatus = currentgeofencestatus
                            prevgeofenceid = currentgeofenceid

                        Next

                    End If
                    '###########################################################################################################################

                End If
            Next

            'End While

            GridView2.DataSource = t
            GridView2.DataBind()
            'GridView1.DataSource = t
            'GridView1.DataBind()
        Catch ex As Exception
            WriteLog2("Errr " & ex.Message)
        End Try
        'GridView1.Visible = True
        'GridView1.Columns(10).Visible = True
        'Return GeofenceTripTable
    End Sub
    Protected Sub WriteLog2(ByVal message As String)
        Try
            Dim sw As New StreamWriter(Server.MapPath("") & "\GetSFLog.txt", FileMode.Append)
            sw.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") & " - " & message)
            sw.Close()
        Catch ex As Exception

        End Try
    End Sub
    Private Sub LoadGeofence(ByVal map As AspMap.Map, ByVal plateno As String)
        Try

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand = New SqlCommand("select * from geofence where accesstype='1' order by geofencename", conn)

            Dim dr As SqlDataReader

            Dim privateGeofenceLayer As New AspMap.DynamicLayer()
            privateGeofenceLayer.LayerType = LayerType.mcPolygonLayer

            Try
                conn.Open()
                dr = cmd.ExecuteReader()
                While dr.Read()
                    Try
                        If (dr("geofencetype") = False) Then
                            Dim circleShape As New AspMap.Shape
                            Dim values As String() = dr("data").Split(",")

                            If (values.Length = 3) Then
                                circleShape.MakeCircle(Convert.ToDouble(values(0)), Convert.ToDouble(values(1)), Convert.ToInt32(values(2)) / 111120.0)

                                privateGeofenceLayer.AddShape(circleShape, dr("geofencename").ToString().ToUpper(), dr("geofenceid"))
                            End If
                        Else
                            Dim polygonShape As New AspMap.Shape
                            polygonShape.ShapeType = ShapeType.mcPolygonShape

                            Dim shpPoints As New AspMap.Points()
                            Dim points() As String = dr("data").Split(";")
                            Dim values() As String

                            For i As Integer = 0 To points.Length - 1
                                values = points(i).Split(",")
                                If (values.Length = 2) Then
                                    shpPoints.AddPoint(Convert.ToDouble(values(0)), Convert.ToDouble(values(1)))
                                End If
                            Next

                            polygonShape.AddPart(shpPoints)

                            privateGeofenceLayer.AddShape(polygonShape, dr("geofencename").ToString().ToUpper(), dr("geofenceid"))
                        End If
                    Catch ex As Exception
                        Response.Write(ex.Message)
                    End Try
                End While

            Catch ex As Exception
                Response.Write(ex.Message)
            Finally
                conn.Close()
            End Try

            map.AddLayer(privateGeofenceLayer)
            map(0).Name = "Geofence Layer"


        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub


End Class
