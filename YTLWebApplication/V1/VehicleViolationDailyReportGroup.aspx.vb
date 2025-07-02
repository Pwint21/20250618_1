Imports System
Imports System.Collections.Generic
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Configuration
Imports System.Data.SqlClient
Imports System.Data
Imports System.Diagnostics
Imports System.Web.Script.Services
Imports Newtonsoft.Json
Imports AspMap.Web.Map
Imports AspMap
Imports iTextSharp.text
Imports iTextSharp.text.html.simpleparser
Imports iTextSharp.text.pdf
Imports System.IO
Imports System.Net
Imports ASPNetMultiLanguage
Partial Class VehicleViolationDailyReportGroup
    Inherits System.Web.UI.Page
    Public ec As String = "false"
    Public lng As String
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    End Sub
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try

            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            Dim suserid As String = Request.QueryString("userid")
            hiduser.Value = userid

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand = New SqlCommand("select userid,username from userTBL where role='User' order by username", conn)
            Dim dr As SqlDataReader

            If role = "User" Then
                cmd = New SqlCommand("select userid,username from userTBL where userid='" & userid & "' order by username", conn)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select userid,username from userTBL where userid in(" & userslist & ") order by username", conn)
            End If

            conn.Open()
            dr = cmd.ExecuteReader()
            If (role <> "User") Then
                ddlusers.Items.Add(New WebControls.ListItem("--SELECT USERNAME--", "--Select User Name--"))
                ddlusers.Items.Add(New WebControls.ListItem("--ALL USERS--", "ALL"))
            End If

            While dr.Read()
                ddlusers.Items.Add(New WebControls.ListItem(dr("username").ToString().ToUpper(), dr("userid")))
            End While
            conn.Close()
        Catch ex As Exception
            Response.Write(ex.StackTrace)
        Finally

            MyBase.OnInit(e)

        End Try
    End Sub

    '<System.Web.Services.WebMethod()>
    'Public Shared Function LoadVehicleGroup(ByVal userId As Integer) As ArrayList
    '    Dim list As ArrayList = New ArrayList
    '    Try

    '        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
    '        Dim cmd As New SqlCommand("select groupid ,groupname  from vehicle_group where userid=@userID order by groupname", conn)

    '        cmd.Parameters.AddWithValue("@userID", userId)

    '        Try
    '            conn.Open()
    '            Dim dr As SqlDataReader = cmd.ExecuteReader
    '            While dr.Read
    '                list.Add(New WebControls.ListItem(dr("groupname").ToString().ToUpper(), dr("groupid").ToString))
    '            End While
    '        Catch ex As Exception

    '        Finally
    '            conn.Close()
    '        End Try

    '    Catch ex As Exception

    '    End Try
    '    Return list
    'End Function

    '<System.Web.Services.WebMethod()>
    'Public Shared Function LoadVehicles(ByVal groupid As String, ByVal userid As String) As ArrayList
    '    Dim list As ArrayList = New ArrayList
    '    Try

    '        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
    '        Dim cmd As New SqlCommand("select plateno,UPPER(plateno) as unitid from vehicleTBL where groupid=@groupID", conn)
    '        If groupid = "0" Then

    '            cmd.CommandText = "select plateno,UPPER(plateno) as unitid from vehicleTBL where Userid=" & userid & ""
    '        Else
    '            cmd.Parameters.AddWithValue("@groupID", groupid)
    '        End If



    '        Try
    '            conn.Open()
    '            Dim dr As SqlDataReader = cmd.ExecuteReader
    '            While dr.Read
    '                list.Add(New WebControls.ListItem(dr("unitid").ToString, dr("plateno").ToString))
    '            End While
    '        Catch ex As Exception

    '        Finally
    '            conn.Close()
    '        End Try

    '    Catch ex As Exception

    '    End Try
    '    Return list
    'End Function

    ''<System.Web.Services.WebMethod(EnableSession:=True)>
    ''<ScriptMethod(ResponseFormat:=ResponseFormat.Xml)>
    ''Public Shared Function GetViolationSummary(ByVal fromdate As String, ByVal todate As String, ByVal plateno As String, ByVal userid As String, ByVal h As String, ByVal h11 As String, ByVal h2 As String, ByVal groupid As String) As String
    ''    Dim aa As New ArrayList()
    ''    Dim a As ArrayList
    ''    Dim json As String = ""
    ''    Dim tothwy As Int32 = 0
    ''    Dim totnonhwy As Int32 = 0
    ''    Dim totidle As Int32 = 0
    ''    Dim totsafeidle As Int32 = 0
    ''    Dim CntDrv As Int32 = 0
    ''    Dim TotDrv As Double = 0
    ''    Dim TotWork As Double = 0
    ''    Dim Tothbk As Double = 0
    ''    Dim TotVio As Int32 = 0
    ''    Dim totDistance As Double = 0
    ''    Dim tothacc As Int32 = 0
    ''    Dim totdrhours As String
    ''    Dim totworkhours As String
    ''    Dim dutyhours As String
    ''    Dim iSpan As TimeSpan
    ''    Try
    ''        Dim groupcondition As String = ""
    ''        Dim vehiclestable As New DataTable
    ''        vehiclestable.Columns.Add(New DataColumn("S No"))
    ''        vehiclestable.Columns.Add(New DataColumn("Violation Date"))
    ''        vehiclestable.Columns.Add(New DataColumn("Plate No1"))
    ''        vehiclestable.Columns.Add(New DataColumn("Hwy"))
    ''        vehiclestable.Columns.Add(New DataColumn("NonHwy"))
    ''        vehiclestable.Columns.Add(New DataColumn("safeidle"))
    ''        vehiclestable.Columns.Add(New DataColumn("Idle"))
    ''        vehiclestable.Columns.Add(New DataColumn("Harsh Break"))
    ''        vehiclestable.Columns.Add(New DataColumn("Harsh Acceleration"))
    ''        vehiclestable.Columns.Add(New DataColumn("CntDrv"))
    ''        vehiclestable.Columns.Add(New DataColumn("TotDrv"))
    ''        vehiclestable.Columns.Add(New DataColumn("TotWork"))
    ''        vehiclestable.Columns.Add(New DataColumn("TotVio"))
    ''        vehiclestable.Columns.Add(New DataColumn("Distance"))
    ''        vehiclestable.Columns.Add(New DataColumn("On Time"))
    ''        vehiclestable.Columns.Add(New DataColumn("Off Time"))
    ''        vehiclestable.Columns.Add(New DataColumn("Duration"))
    ''        vehiclestable.Columns.Add(New DataColumn("Group"))

    ''        Dim cmd As New SqlCommand
    ''        cmd.CommandTimeout = 120
    ''        Dim r As DataRow
    ''        If Not userid = "--Select User Name--" Then
    ''            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
    ''            If groupid = "0" Then
    ''                If plateno = "-- ALL PLATE NOS --" Then
    ''                    cmd = New SqlCommand("select ROW_NUMBER() over (order by  timestamp) as sno, CONVERT(varchar(20),timestamp,106) as datetime, plateno,[dbo].[fn_Getgroupname](plateno) as Grp ,overSpeedHw, overSpeed, idling, contdrive, totaldriverhour, totalworkhour, totalvio, distance,hdec,safeidling,isnull(convert(varchar,starttime,108),'') as ontime,isnull(convert(varchar,offtime,108),'') as offtime,isnull(duration,0) duration ,case when substring(dbo.[fn_GetVersion](plateno),1,2) ='MT' then  hacc else '-1' end as hacc from dbo.ohsas_violation  where   userid='" & userid & "' and timestamp between  '" & fromdate & "'   and '" & todate & "' ", conn)
    ''                Else
    ''                    cmd = New SqlCommand("select ROW_NUMBER() over (order by  timestamp) as sno, CONVERT(varchar(20),timestamp,106) as datetime,plateno,[dbo].[fn_Getgroupname](plateno) as Grp ,overSpeedHw, overSpeed, idling, contdrive, totaldriverhour, totalworkhour, totalvio, distance,hdec,safeidling,isnull(convert(varchar,starttime,108),'') as ontime,isnull(convert(varchar,offtime,108),'') as offtime,isnull(duration,0) duration ,case when substring(dbo.[fn_GetVersion](plateno),1,2) ='MT' then  hacc else '-1' end as hacc from dbo.ohsas_violation where userid='" & userid & "' and  plateno='" & plateno & "' and timestamp between  '" & fromdate & "'   and '" & todate & "' ", conn)

    ''                End If
    ''            Else
    ''                If plateno = "-- ALL PLATE NOS --" Then
    ''                    cmd = New SqlCommand("select ROW_NUMBER() over (order by  timestamp) as sno, CONVERT(varchar(20),timestamp,106) as datetime, plateno,[dbo].[fn_Getgroupname](plateno) as Grp ,overSpeedHw, overSpeed, idling, contdrive, totaldriverhour, totalworkhour, totalvio, distance,hdec,safeidling,isnull(convert(varchar,starttime,108),'') as ontime,isnull(convert(varchar,offtime,108),'') as offtime,isnull(duration,0) duration ,case when substring(dbo.[fn_GetVersion](plateno),1,2) ='MT' then  hacc else '-1' end as hacc from dbo.ohsas_violation  where  plateno in (select plateno from vehicleTBL where groupid='" & groupid & "') and timestamp between  '" & fromdate & "'   and '" & todate & "' ", conn)

    ''                Else
    ''                    cmd = New SqlCommand("select ROW_NUMBER() over (order by  timestamp) as sno, CONVERT(varchar(20),timestamp,106) as datetime,plateno,[dbo].[fn_Getgroupname](plateno) as Grp ,overSpeedHw, overSpeed, idling, contdrive, totaldriverhour, totalworkhour, totalvio, distance,hdec,safeidling,isnull(convert(varchar,starttime,108),'') as ontime,isnull(convert(varchar,offtime,108),'') as offtime,isnull(duration,0) duration ,case when substring(dbo.[fn_GetVersion](plateno),1,2) ='MT' then  hacc else '-1' end as hacc from dbo.ohsas_violation where userid='" & userid & "' and  plateno='" & plateno & "' and timestamp between  '" & fromdate & "'   and '" & todate & "' ", conn)

    ''                End If
    ''            End If



    ''            Try
    ''                conn.Open()
    ''                Dim dr As SqlDataReader = cmd.ExecuteReader
    ''                While dr.Read

    ''                    If Convert.ToInt32(dr("totaldriverhour")) < 0 Then
    ''                        iSpan = TimeSpan.FromSeconds(0)
    ''                    Else
    ''                        iSpan = TimeSpan.FromSeconds(dr("totaldriverhour"))
    ''                    End If
    ''                    totdrhours = iSpan.Hours.ToString.PadLeft(2, "0"c) & ":" &
    ''                  iSpan.Minutes.ToString.PadLeft(2, "0"c) & ":" &
    ''                  iSpan.Seconds.ToString.PadLeft(2, "0"c)
    ''                    If Convert.ToInt32(dr("totalworkhour")) < 0 Then
    ''                        iSpan = TimeSpan.FromSeconds(0)
    ''                    Else
    ''                        iSpan = TimeSpan.FromSeconds(dr("totalworkhour"))
    ''                    End If
    ''                    totworkhours = iSpan.Hours.ToString.PadLeft(2, "0"c) & ":" &
    ''                  iSpan.Minutes.ToString.PadLeft(2, "0"c) & ":" &
    ''                  iSpan.Seconds.ToString.PadLeft(2, "0"c)

    ''                    r = vehiclestable.NewRow
    ''                    r(0) = dr("sno")
    ''                    r(1) = dr("datetime")
    ''                    r(2) = dr("plateno")
    ''                    r(3) = dr("overSpeedHw")
    ''                    r(4) = dr("overSpeed")

    ''                    If IsDBNull(dr("safeidling")) Then
    ''                        r(5) = 0
    ''                    Else
    ''                        r(5) = dr("safeidling")
    ''                    End If
    ''                    r(6) = dr("idling")
    ''                    r(7) = dr("hdec")
    ''                    If dr("hacc") <> -1 Then
    ''                        r(8) = dr("hacc")
    ''                    Else
    ''                        r(8) = "--"
    ''                    End If

    ''                    r(9) = dr("contdrive")
    ''                    r(10) = totdrhours
    ''                    r(11) = totworkhours
    ''                    r(12) = Convert.ToInt32(dr("totalvio"))
    ''                    r(13) = Convert.ToDouble(dr("distance")).ToString("0.00")
    ''                    r(14) = dr("ontime")
    ''                    r(15) = dr("offtime")

    ''                    If Convert.ToInt32(dr("duration")) < 0 Then
    ''                        iSpan = TimeSpan.FromMinutes(0)
    ''                    Else
    ''                        iSpan = TimeSpan.FromMinutes(dr("duration"))
    ''                    End If
    ''                    dutyhours = iSpan.Hours.ToString.PadLeft(2, "0"c) & ":" &
    ''                    iSpan.Minutes.ToString.PadLeft(2, "0"c) & ":" &
    ''                    iSpan.Seconds.ToString.PadLeft(2, "0"c)

    ''                    r(16) = dutyhours

    ''                    r(17) = dr("Grp").ToString().ToUpper()

    ''                    vehiclestable.Rows.Add(r)
    ''                    tothwy = tothwy + Int32.Parse(dr("overSpeedHw").ToString())
    ''                    totnonhwy = totnonhwy + Int32.Parse(dr("overSpeed").ToString())
    ''                    totidle = totidle + Int32.Parse(dr("idling").ToString())
    ''                    If IsDBNull(dr("safeidling")) Then
    ''                        totsafeidle = totsafeidle + 0
    ''                    Else
    ''                        totsafeidle = totsafeidle + Int32.Parse(dr("safeidling").ToString())
    ''                    End If

    ''                    Tothbk = Tothbk + Int32.Parse(dr("hdec").ToString())
    ''                    If dr("hacc") <> -1 Then
    ''                        tothacc = tothacc + Int32.Parse(dr("hacc").ToString())
    ''                    Else
    ''                        tothacc = tothacc + 0
    ''                    End If

    ''                    CntDrv = CntDrv + Int32.Parse(dr("contdrive").ToString())
    ''                    If Convert.ToInt32(dr("totaldriverhour")) < 0 Then
    ''                        TotDrv = TotDrv + 0
    ''                    Else
    ''                        TotDrv = TotDrv + Double.Parse(dr("totaldriverhour").ToString())
    ''                    End If

    ''                    TotWork = TotWork + Double.Parse(dr("totalworkhour").ToString())
    ''                    TotVio = TotVio + Int32.Parse(dr("totalvio").ToString())
    ''                    totDistance = totDistance + Convert.ToDouble(dr("distance")).ToString("0.00")

    ''                End While
    ''            Catch ex As Exception
    ''                WriteLog(ex.Message & ":" & ex.StackTrace)
    ''            Finally
    ''                conn.Close()
    ''            End Try
    ''        Else
    ''            r = vehiclestable.NewRow
    ''            r(0) = "--"
    ''            r(1) = "--"
    ''            r(2) = "--"
    ''            r(3) = "--"
    ''            r(4) = "--"
    ''            r(5) = "--"
    ''            r(6) = "--"
    ''            r(7) = "--"
    ''            r(8) = "--"
    ''            r(9) = "--"
    ''            r(10) = "--"
    ''            r(11) = "--"
    ''            r(12) = "--"
    ''            r(13) = "--"
    ''            r(14) = "--"
    ''            r(15) = "--"
    ''            r(16) = "--"
    ''            r(17) = "--"
    ''            vehiclestable.Rows.Add(r)
    ''        End If
    ''        HttpContext.Current.Session.Remove("exceltable")
    ''        HttpContext.Current.Session.Remove("exceltable2")
    ''        HttpContext.Current.Session.Remove("exceltable3")
    ''        HttpContext.Current.Session.Remove("tempTable")
    ''        vehiclestable.Columns(0).ColumnName = h.ToString()
    ''        vehiclestable.Columns(1).ColumnName = h2.ToString()
    ''        vehiclestable.Columns(2).ColumnName = h11.ToString()
    ''        HttpContext.Current.Session("exceltable") = vehiclestable
    ''        If (vehiclestable.Rows.Count > 0) Then
    ''            For j As Integer = 0 To vehiclestable.Rows.Count - 1
    ''                Try
    ''                    a = New ArrayList
    ''                    a.Add(vehiclestable.DefaultView.Item(j)(0))
    ''                    a.Add(vehiclestable.DefaultView.Item(j)(1))
    ''                    a.Add(vehiclestable.DefaultView.Item(j)(2))
    ''                    a.Add(vehiclestable.DefaultView.Item(j)(17))
    ''                    a.Add(vehiclestable.DefaultView.Item(j)(3))
    ''                    a.Add(vehiclestable.DefaultView.Item(j)(4))
    ''                    a.Add(vehiclestable.DefaultView.Item(j)(5))
    ''                    a.Add(vehiclestable.DefaultView.Item(j)(6))
    ''                    a.Add(vehiclestable.DefaultView.Item(j)(7))
    ''                    a.Add(vehiclestable.DefaultView.Item(j)(8))
    ''                    a.Add(vehiclestable.DefaultView.Item(j)(9))
    ''                    a.Add(vehiclestable.DefaultView.Item(j)(10))
    ''                    a.Add(vehiclestable.DefaultView.Item(j)(11))
    ''                    a.Add(vehiclestable.DefaultView.Item(j)(12))
    ''                    a.Add(vehiclestable.DefaultView.Item(j)(13))
    ''                    a.Add(vehiclestable.DefaultView.Item(j)(14))
    ''                    a.Add(vehiclestable.DefaultView.Item(j)(15))
    ''                    a.Add(vehiclestable.DefaultView.Item(j)(16))

    ''                    aa.Add(a)
    ''                Catch ex As Exception

    ''                End Try
    ''            Next
    ''            a = New ArrayList
    ''            a.Add("")
    ''            a.Add("Total")
    ''            a.Add("")
    ''            a.Add(tothwy.ToString())
    ''            a.Add(totnonhwy.ToString())
    ''            a.Add(totidle.ToString())
    ''            a.Add(totsafeidle.ToString())
    ''            a.Add(Tothbk.ToString())
    ''            a.Add(tothacc.ToString())
    ''            a.Add(CntDrv.ToString())

    ''            iSpan = TimeSpan.FromSeconds(TotDrv)
    ''            totdrhours = iSpan.Days.ToString.PadLeft(2, "0"c) & " Days " & iSpan.Hours.ToString.PadLeft(2, "0"c) & ":" &
    ''            iSpan.Minutes.ToString.PadLeft(2, "0"c) & ":" &
    ''            iSpan.Seconds.ToString.PadLeft(2, "0"c)
    ''            iSpan = TimeSpan.FromSeconds(TotWork)
    ''            totworkhours = iSpan.Days.ToString.PadLeft(2, "0"c) & " Days " & iSpan.Hours.ToString.PadLeft(2, "0"c) & ":" &
    ''            iSpan.Minutes.ToString.PadLeft(2, "0"c) & ":" &
    ''            iSpan.Seconds.ToString.PadLeft(2, "0"c)
    ''            a.Add(totdrhours.ToString())
    ''            a.Add(totworkhours.ToString())
    ''            a.Add(TotVio.ToString())
    ''            a.Add(totDistance.ToString("0.00"))
    ''            a.Add("")
    ''            a.Add("")
    ''            a.Add("")
    ''            aa.Add(a)
    ''        End If

    ''    Catch ex As Exception
    ''        aa.Add(ex.Message)
    ''    End Try

    ''    json = JsonConvert.SerializeObject(aa, Formatting.None)
    ''    Return json
    ''End Function
    Public Shared Function SetColumnsOrder(ByVal table As DataTable, ByVal ParamArray columnNames As [String]()) As DataTable
        For columnIndex As Integer = 0 To columnNames.Length - 1
            table.Columns(columnNames(columnIndex)).SetOrdinal(columnIndex)
        Next
        Return table
    End Function

    '<System.Web.Services.WebMethod()>
    'Public Shared Function GetViolationDetails(ByVal fromdate As String, ByVal todate As String, ByVal plateno As String, ByVal userid As String, ByVal type As String, ByVal h As String, ByVal h1 As String, ByVal h2 As String, ByVal h3 As String, ByVal h4 As String, ByVal h5 As String, ByVal h6 As String, ByVal h7 As String, ByVal h8 As String, ByVal h9 As String, ByVal h10 As String) As String
    '    Dim aa As New ArrayList()

    '    Dim a As ArrayList
    '    Dim json As String = ""
    '    Dim vehiclepoint As New AspMap.Point
    '    Dim prevstatus As String = "stop"
    '    Dim currentstatus As String = "stop"
    '    Dim tempprevtime As DateTime = fromdate
    '    Dim prevtime As DateTime = fromdate
    '    Dim currenttime As DateTime = fromdate
    '    Dim speedlimits As New ArrayList()
    '    Dim highwaylimit As Single
    '    Dim nonhighwaylimit As Single
    '    Dim i As Int16 = 0

    '    Dim sb As StringBuilder

    '    Dim mapping As New AspMap.Web.Map
    '    mapping.AddLayer(HttpContext.Current.Server.MapPath("maps/highwaylines.shp"))


    '    Try
    '        Dim groupcondition As String = ""
    '        Dim vehiclestable As New DataTable
    '        If (type <> "idealing" And type <> "safeidealing") Then
    '            vehiclestable.Columns.Add(New DataColumn("Sno"))
    '            vehiclestable.Columns.Add(New DataColumn("Plateno"))
    '            vehiclestable.Columns.Add(New DataColumn("Date"))
    '            vehiclestable.Columns.Add(New DataColumn("Location"))
    '            If type = "Hacc" Then
    '                vehiclestable.Columns.Add(New DataColumn("HA Speed"))
    '            Else
    '                vehiclestable.Columns.Add(New DataColumn("Speed"))
    '            End If

    '        Else
    '            vehiclestable.Columns.Add(New DataColumn("Sno"))
    '            vehiclestable.Columns.Add(New DataColumn("Plateno"))
    '            vehiclestable.Columns.Add(New DataColumn("From Date"))
    '            vehiclestable.Columns.Add(New DataColumn("To Date"))
    '            vehiclestable.Columns.Add(New DataColumn("Location"))
    '            vehiclestable.Columns.Add(New DataColumn("Duration"))
    '        End If



    '        Dim locobj As New Location(userid)
    '        Dim cmd As New SqlCommand
    '        Dim r As DataRow
    '        speedlimits = Gethighwaynonhighwayspeed(plateno)
    '        If DateDiff(DateInterval.Day, Convert.ToDateTime(fromdate), Convert.ToDateTime("2013/11/10 00:00:00")) > 0 Then
    '            highwaylimit = 75
    '            nonhighwaylimit = 60
    '        Else
    '            highwaylimit = speedlimits(0)
    '            nonhighwaylimit = speedlimits(1)
    '        End If

    '        If type <> "" Then
    '            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

    '            If type = "hwy" Then
    '                'cmd = New SqlCommand("select distinct h.plateno,convert(varchar(19),h.timestamp,121) timestamp,h.lat,h.lon,h.speed,h.ignition from vehicle_history2 h Left join vehicle_g13e_data g on h.plateno=g.plateno and h.timestamp=g.timestamp where h.speed>=75 and  h.timestamp between  '" & fromdate & "'   and '" & todate & "' and  h.plateno='" & plateno & "'", conn)
    '                If userid = "2897" Or userid = "10080" Then
    '                    cmd = New SqlCommand("select plateno,convert(varchar(19),timestamp,121) timestamp,lat ,lon ,convert(numeric(18,2),speed) speed from vehicle_history2 where  gps_av='A' and Hbreak<>1 and odometer > 0 and speed>='" & highwaylimit & "' and timestamp between  '" & fromdate & "'   and '" & todate & "'  and  plateno='" & plateno & "'", conn)
    '                Else
    '                    cmd = New SqlCommand("select plateno,convert(varchar(19),timestamp,121) timestamp,lat ,lon ,convert(numeric(18,2),speed) speed from vehicle_history2 where  gps_av='A'  and odometer > 0 and speed>='" & highwaylimit & "' and timestamp between  '" & fromdate & "'   and '" & todate & "'  and  plateno='" & plateno & "'", conn)
    '                End If

    '            ElseIf type = "nonhwy" Then
    '                If userid = "2897" Or userid = "10080" Then
    '                    cmd = New SqlCommand("select plateno,convert(varchar(19),timestamp,121) timestamp,lat ,lon ,convert(numeric(18,2),speed) speed from vehicle_history2 where  gps_av='A' and Hbreak<>1 and odometer > 0 and  timestamp between  '" & fromdate & "'   and '" & todate & "'  and  plateno='" & plateno & "'", conn)
    '                Else
    '                    cmd = New SqlCommand("select plateno,convert(varchar(19),timestamp,121) timestamp,lat ,lon ,convert(numeric(18,2),speed) speed from vehicle_history2 where  gps_av='A'  and odometer > 0 and  timestamp between  '" & fromdate & "'   and '" & todate & "'  and  plateno='" & plateno & "'", conn)

    '                End If


    '            ElseIf type = "idealing" Or type = "safeidealing" Then
    '                cmd = New SqlCommand("select plateno,convert(varchar(19),timestamp,121) timestamp,lat ,lon ,convert(numeric(18,2),speed) speed,ignition from vehicle_history2 where  plateno='" & plateno & "' and gps_av='A' and odometer > 0 and timestamp between  '" & fromdate & "'   and '" & todate & "' order by timestamp", conn)
    '            ElseIf type = "Hbreak" Then
    '                cmd = New SqlCommand("select plateno,convert(varchar(19),timestamp,121) timestamp,lat ,lon ,convert(numeric(18,2),speed) speed from vehicle_history2 where  Hbreak=1 and gps_av='A' and  timestamp between  '" & fromdate & "'   and '" & todate & "'  and  plateno='" & plateno & "'  order by timestamp", conn)
    '            ElseIf type = "Hacc" Then
    '                cmd = New SqlCommand("select plateno,convert(varchar(19),timestamp,121) timestamp,lat ,lon ,convert(numeric(18,2),haspeed) speed from vehicle_history2 where haspeed>8  and  timestamp between  '" & fromdate & "'   and '" & todate & "'  and  plateno='" & plateno & "'  order by timestamp", conn)

    '            End If
    '            Try
    '                conn.Open()
    '                Dim dr As SqlDataReader = cmd.ExecuteReader
    '                Dim sno As Int16 = 1
    '                Dim idlelat As Double = 0
    '                Dim idlelon As Double = 0

    '                While dr.Read

    '                    r = vehiclestable.NewRow
    '                    vehiclepoint.X = CType(dr(3), Double)
    '                    vehiclepoint.Y = CType(dr(2), Double)
    '                    Dim rs As AspMap.Recordset
    '                    rs = mapping("highwaylines").SearchByDistance(vehiclepoint, 150 / (60 * 1852), AspMap.SearchMethod.Intersect)
    '                    If type = "hwy" Then
    '                        If dr(4) >= highwaylimit Then
    '                            If (rs("tag") = "0") Then
    '                                r(0) = sno
    '                                r(1) = dr(0)
    '                                r(2) = dr(1)
    '                                r(3) = rs("label")
    '                                r(4) = dr(4)
    '                                vehiclestable.Rows.Add(r)
    '                                sno = sno + 1
    '                            End If
    '                        End If
    '                    ElseIf type = "nonhwy" Then
    '                        If dr(4) >= nonhighwaylimit Then
    '                            If (rs("tag") <> "0") Then
    '                                r(0) = sno
    '                                r(1) = dr(0)
    '                                r(2) = dr(1)
    '                                r(3) = locobj.GetLocation(dr(2), dr(3))
    '                                r(4) = dr(4)
    '                                vehiclestable.Rows.Add(r)
    '                                sno = sno + 1
    '                            End If
    '                        End If
    '                    ElseIf type = "Hbreak" Then
    '                        r(0) = sno
    '                        r(1) = dr(0)
    '                        r(2) = dr(1)
    '                        r(3) = locobj.GetLocation(dr(2), dr(3))
    '                        r(4) = dr(4)
    '                        vehiclestable.Rows.Add(r)
    '                        sno = sno + 1
    '                    ElseIf type = "Hacc" Then
    '                        r(0) = sno
    '                        r(1) = dr(0)
    '                        r(2) = dr(1)
    '                        r(3) = locobj.GetLocation(dr(2), dr(3))
    '                        r(4) = dr(4)
    '                        vehiclestable.Rows.Add(r)
    '                        sno = sno + 1
    '                    ElseIf type = "idealing" Then
    '                        currenttime = dr("timestamp")
    '                        If dr("ignition") = True And dr("speed") <> 0 Then
    '                            currentstatus = "moving"
    '                        ElseIf dr("ignition") = True And dr("speed") = "0" Then
    '                            If prevstatus <> "idle" Then
    '                                idlelat = CType(dr(2), Double)
    '                                idlelon = CType(dr(3), Double)
    '                            End If
    '                            currentstatus = "idle"
    '                        Else
    '                            currentstatus = "stop"
    '                        End If

    '                        If prevstatus <> currentstatus Then
    '                            Dim temptime As TimeSpan = tempprevtime - prevtime 'currenttime - prevtime
    '                            Dim minutes As Int16 = temptime.TotalMinutes()
    '                            Select Case prevstatus
    '                                Case "stop"

    '                                Case "moving"

    '                                Case "idle"
    '                                    If temptime.TotalMinutes >= 20 Then
    '                                        r(0) = sno
    '                                        r(1) = dr(0)
    '                                        r(2) = prevtime.ToString("yyyy-MM-dd HH:mm:ss")
    '                                        r(3) = tempprevtime.ToString("yyyy-MM-dd HH:mm:ss")
    '                                        r(4) = locobj.GetLocation(idlelat, idlelon)
    '                                        r(5) = minutes
    '                                        vehiclestable.Rows.Add(r)
    '                                        sno = sno + 1
    '                                    End If
    '                            End Select
    '                            prevtime = currenttime
    '                            prevstatus = currentstatus
    '                        End If

    '                        tempprevtime = currenttime

    '                    ElseIf type = "safeidealing" Then
    '                        currenttime = dr("timestamp")
    '                        If dr("ignition") = True And dr("speed") <> 0 Then
    '                            currentstatus = "moving"
    '                        ElseIf dr("ignition") = True And dr("speed") = "0" Then
    '                            If prevstatus <> "idle" Then
    '                                idlelat = CType(dr(2), Double)
    '                                idlelon = CType(dr(3), Double)
    '                            End If
    '                            currentstatus = "idle"
    '                        Else
    '                            currentstatus = "stop"
    '                        End If
    '                        Dim location = ""
    '                        If prevstatus <> currentstatus Then
    '                            Dim temptime As TimeSpan = tempprevtime - prevtime 'currenttime - prevtime
    '                            Dim minutes As Int16 = temptime.TotalMinutes()
    '                            Select Case prevstatus
    '                                Case "stop"

    '                                Case "moving"

    '                                Case "idle"
    '                                    If temptime.TotalMinutes >= 20 Then
    '                                        Try
    '                                            location = locobj.GetLocation(idlelat, idlelon)
    '                                            If location.Contains("geofence") Then

    '                                                r(0) = sno
    '                                                r(1) = dr(0)
    '                                                r(2) = prevtime.ToString("yyyy-MM-dd HH:mm:ss")
    '                                                r(3) = tempprevtime.ToString("yyyy-MM-dd HH:mm:ss")
    '                                                r(4) = location
    '                                                r(5) = minutes
    '                                                vehiclestable.Rows.Add(r)
    '                                                sno = sno + 1

    '                                            End If
    '                                        Catch ex As Exception

    '                                        End Try
    '                                    End If
    '                            End Select
    '                            prevtime = currenttime
    '                            prevstatus = currentstatus
    '                        End If

    '                        tempprevtime = currenttime

    '                    End If
    '                End While

    '                If type = "idealing" Then
    '                    r = vehiclestable.NewRow
    '                    Dim temptime As TimeSpan = tempprevtime - prevtime 'currenttime - prevtime
    '                    Dim minutes As Int16 = temptime.TotalMinutes()
    '                    Try


    '                        Select Case prevstatus
    '                            Case "stop"

    '                            Case "moving"

    '                            Case "idle"
    '                                If temptime.TotalMinutes >= 20 Then
    '                                    r(0) = sno
    '                                    r(1) = plateno
    '                                    r(2) = prevtime.ToString("yyyy-MM-dd HH:mm:ss")
    '                                    r(3) = tempprevtime.ToString("yyyy-MM-dd HH:mm:ss")
    '                                    r(4) = locobj.GetLocation(idlelat, idlelon)
    '                                    r(5) = minutes
    '                                    vehiclestable.Rows.Add(r)
    '                                    sno = sno + 1
    '                                End If
    '                        End Select
    '                    Catch ex As Exception
    '                        r(0) = sno
    '                        r(1) = ""
    '                        r(2) = ""
    '                        r(3) = ""
    '                        r(4) = ex.Message
    '                        r(5) = ""
    '                        vehiclestable.Rows.Add(r)
    '                        sno = sno + 1
    '                    End Try

    '                ElseIf type = "safeidealing" Then
    '                    Dim location = ""
    '                    r = vehiclestable.NewRow
    '                    Dim temptime As TimeSpan = tempprevtime - prevtime 'currenttime - prevtime
    '                    Dim minutes As Int16 = temptime.TotalMinutes()
    '                    Select Case prevstatus
    '                        Case "stop"

    '                        Case "moving"

    '                        Case "idle"
    '                            If temptime.TotalMinutes >= 20 Then
    '                                Try
    '                                    location = locobj.GetLocation(idlelat, idlelon)
    '                                    If location.Contains("geofence") Then
    '                                        r(0) = sno
    '                                        r(1) = plateno
    '                                        r(2) = prevtime.ToString("yyyy-MM-dd HH:mm:ss")
    '                                        r(3) = tempprevtime.ToString("yyyy-MM-dd HH:mm:ss")
    '                                        r(4) = location
    '                                        r(5) = minutes
    '                                        vehiclestable.Rows.Add(r)
    '                                        sno = sno + 1
    '                                    End If
    '                                Catch ex As Exception

    '                                End Try
    '                            End If
    '                    End Select
    '                End If
    '            Catch ex As Exception

    '            Finally
    '                conn.Close()
    '            End Try
    '        Else
    '            r = vehiclestable.NewRow
    '            r(0) = "--"
    '            r(1) = "--"
    '            r(2) = "--"
    '            r(3) = "--"
    '            r(4) = "--"
    '            r(5) = "--"
    '            vehiclestable.Rows.Add(r)
    '        End If

    '        If type = "Hacc" Then
    '            h3 = "HA Speed"
    '        End If


    '        sb = New StringBuilder()
    '        sb.Clear()

    '        If vehiclestable.Rows.Count > 0 Then
    '            If type <> "idealing" And type <> "safeidealing" Then
    '                sb.Append("<thead align='left' ><tr  ><td colspan='5' style='color:rgb(35, 135, 228)' >" & h1 & ": " & plateno & "</td></tr>")
    '                sb.Append("<tr  ><td colspan='5' style='color:rgb(35, 135, 228)' >" & h8 & ": " & DateTime.Parse(fromdate).ToString("yyyy-MM-dd HH:mm:ss") & " " & h9 & ": " & DateTime.Parse(todate).ToString("yyyy-MM-dd HH:mm:ss") & " </td></tr>")
    '                sb.Append("<tr ><th scope='col' style='width: 30px;'>S No</th><th  scope='col' style='width: 80px;'>Plateno</th><th   scope='col' style='width: 100px; '>" & h2 & "</th><th  scope='col' style='width: 30px;'>" & h3 & "</th><th   scope='col' style='width: 200px; '>" & h4 & "</th></tr></thead>")

    '                sb.Append("<tbody>")


    '                For j As Integer = 0 To vehiclestable.Rows.Count - 1
    '                    Try
    '                        sb.Append("<tr ><td style='width:30px;'>" & vehiclestable.DefaultView.Item(j)(0) & "</td>")
    '                        sb.Append("<td style='width: 80px;'>" & vehiclestable.DefaultView.Item(j)(1) & "</td>")
    '                        sb.Append("<td style='width: 100px;'>" & vehiclestable.DefaultView.Item(j)(2) & "</td>")
    '                        sb.Append("<td style='width: 30px;text-align:right;'>" & vehiclestable.DefaultView.Item(j)(4) & "</td>")
    '                        sb.Append("<td style='width:200px;'>" & vehiclestable.DefaultView.Item(j)(3) & "</td></tr>")
    '                        'a.Add(vehiclestable.DefaultView.Item(j)(0))
    '                        'a.Add(vehiclestable.DefaultView.Item(j)(1))
    '                        'a.Add(vehiclestable.DefaultView.Item(j)(2))
    '                        'a.Add(vehiclestable.DefaultView.Item(j)(3))
    '                        'a.Add(vehiclestable.DefaultView.Item(j)(4))
    '                        'a.Add(vehiclestable.DefaultView.Item(j)(5))

    '                        'aa.Add(a)
    '                    Catch ex As Exception

    '                    End Try
    '                Next
    '                sb.Append("</tbody>")
    '                sb.Append("<tfoot align='left'><tr ><th style='width: 30px;'>S No</th><th style='width: 80px;'>Plateno</th><th style='width: 100px; '> " & h2 & "</th><th style='width: 30px;'>" & h3 & "</th><th style='width: 200px; '>" & h4 & "</th></tr></tfoot>")
    '            Else
    '                sb.Append("<thead align='left'><tr ><td colspan='6' style='color:rgb(35, 135, 228)'>" & h1 & ": " & plateno & "</td></tr>")
    '                sb.Append("<tr ><td colspan='6' style='color:rgb(35, 135, 228)'>" & h8 & ": " & DateTime.Parse(fromdate).ToString("yyyy-MM-dd HH:mm:ss") & " " & h9 & ": " & DateTime.Parse(todate).ToString("yyyy-MM-dd HH:mm:ss") & " </td></tr>")
    '                sb.Append("<tr ><td colspan='6' style='color:rgb(35, 135, 228)'>Interval: 20 Min </td></tr>")
    '                sb.Append("<tr><th style='width: 40px;'>S No</th><th style='width: 80px;'>Plateno</th><th style='width: 100px; '>" & h5 & "</th><th style='width: 100px; '>" & h6 & "</th><th style='width:50px;'>" & h7 & "</th><th style='width: 200px; '>" & h4 & "</th></tr></thead>")
    '                sb.Append("<tbody>")

    '                For j As Integer = 0 To vehiclestable.Rows.Count - 1
    '                    Try
    '                        sb.Append("<tr ><td style='width:30px;'>" & vehiclestable.DefaultView.Item(j)(0) & "</td>")
    '                        sb.Append("<td style='width: 80px'>" & vehiclestable.DefaultView.Item(j)(1) & "</td>")
    '                        sb.Append("<td style='width: 100px;'>" & vehiclestable.DefaultView.Item(j)(2) & "</td>")
    '                        sb.Append("<td style='width: 100px;'>" & vehiclestable.DefaultView.Item(j)(3) & "</td>")
    '                        sb.Append("<td style='width: 50px;'>" & vehiclestable.DefaultView.Item(j)(5) & "Min </td>")
    '                        sb.Append("<td style='width:200px;'>" & vehiclestable.DefaultView.Item(j)(4) & "</td></tr>")



    '                        'a.Add(vehiclestable.DefaultView.Item(j)(0))
    '                        'a.Add(vehiclestable.DefaultView.Item(j)(1))
    '                        'a.Add(vehiclestable.DefaultView.Item(j)(2))
    '                        'a.Add(vehiclestable.DefaultView.Item(j)(3))
    '                        'a.Add(vehiclestable.DefaultView.Item(j)(4))
    '                        'a.Add(vehiclestable.DefaultView.Item(j)(5))

    '                        'aa.Add(a)
    '                    Catch ex As Exception

    '                    End Try
    '                Next
    '                sb.Append("</tbody>")
    '                sb.Append("<tfoot align='left'><tr ><th style='width: 40px;'>S No</th><th style='width: 80px;'>Plateno</th><th style='width:100px; '>" & h5 & "</th><th style='width: 100px; '>" & h6 & "</th><th style='width: 50px;'>" & h7 & "</th><th style='width: 200px; '>" & h4 & "</th></tr></tfoot>")

    '            End If
    '        Else
    '            sb.Append("<tr ><td style='width:60px;'>" & h10 & "......</td>")
    '        End If

    '        Dim dt As DataTable
    '        If type <> "idealing" Then
    '            dt = SetColumnsOrder(vehiclestable, New String() {"Sno", "Plateno", "Date", "Speed", "Location"})
    '        Else
    '            dt = SetColumnsOrder(vehiclestable, New String() {"Sno", "Plateno", "From date", "To Date", "Duration", "Location"})
    '        End If
    '        HttpContext.Current.Session("deatails") = dt
    '    Catch ex As Exception

    '    End Try
    '    'json = JsonConvert.SerializeObject(aa, Formatting.None)
    '    HttpContext.Current.Session("printdeatails") = sb.ToString()
    '    Return sb.ToString()
    'End Function


    Public Shared Function Gethighwaynonhighwayspeed(ByVal plateno As String) As ArrayList
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim dr As SqlClient.SqlDataReader
        Dim scmd As SqlCommand
        Dim i As Int16 = 0
        If conn.State = ConnectionState.Open Then
            conn.Close()
        End If
        Dim vehicles As ArrayList = New ArrayList()
        conn.Open()
        Try
            Dim ssql As String = "select highwaylimit,nonhighwaylimit  from vehicleTBL where plateno='" & plateno & "' "

            scmd = New SqlClient.SqlCommand(ssql, conn)
            dr = scmd.ExecuteReader
            While dr.Read
                vehicles.Add(dr("highwaylimit").ToString())
                vehicles.Add(dr("nonhighwaylimit").ToString())
                i = i + 1
            End While

            conn.Close()
        Catch ex As Exception
            conn.Close()
        End Try
        Return vehicles
    End Function

    Shared Sub WriteLog(ByVal message As String)
        Try
            If (message.Length > 0) Then
                Dim sw As New StreamWriter(HttpContext.Current.Server.MapPath("") & "/FuelTrackLog.txt", FileMode.Append)
                sw.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") & " - " & message)
                sw.Close()
            End If
        Catch ex As Exception

        End Try
    End Sub


End Class
