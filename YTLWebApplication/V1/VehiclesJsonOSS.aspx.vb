Imports AspMap
Imports ADODB
Imports System.Data
Imports System.Data.SqlClient
Imports Newtonsoft.Json
Imports System.Collections.Generic
Partial Class VehiclesJsonOSS
    Inherits System.Web.UI.Page
    Public map, tempmap As AspMap.Map
    Dim point As AspMap.Point

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            Dim plateno As String = Request.QueryString("plateno")
            Dim bdt As String = Request.QueryString("bdt")
            Dim edt As String = Request.QueryString("edt")
            Dim searchin As String = Request.QueryString("si")
            Dim uid As String = Request.QueryString("uid")
            Dim type As String = Request.QueryString("type")
            Dim transportertype As String = Request.QueryString("t")

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim username As String = Request.Cookies("userinfo")("username").ToString().ToUpper()
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            Dim dr As SqlDataReader
            Dim statusstate As String = "3" ' For In-Process

            Dim typecmd As String = ""

            If uid <> "ALLUSERS" Then
                If username = "MARTINYTL" Or username = "JULIANAYTL" Or username = "SPYON" Or username = "CHEEKEONGYTL" Then
                    role = "User"
                    userid = uid
                End If
            End If

            Dim OSSDict As New Dictionary(Of String, OssRecord)

            Dim cmd As New SqlCommand("select s.name,p.plateno,p.source_supply,p.status from (select * from oss_patch_out where weight_outtime between '" & Now.AddHours(-24).ToString("yyyy/MM/dd HH:mm:ss") & "' and '" & Now.ToString("yyyy/MM/dd HH:mm:ss") & "')  p  Left Outer Join  oss_ship_to_code s On s.shiptocode=p.destination_siteid   order by p.weight_outtime desc", conn2)
            ' Dim cmd As New SqlCommand("select s.name,p.plateno,p.source_supply,p.status from (select * from oss_patch_out where weight_outtime between '2012/01/02 12:38:22' and '2013/01/03 12:38:22'  and status='3')  p  Left Outer Join  oss_ship_to_code s On s.shiptocode=p.destination_siteid   order by p.weight_outtime desc", conn2)
            Try
                conn2.Open()
                dr = cmd.ExecuteReader()
                While dr.Read()
                    Try

                        Dim ossr As New OssRecord()

                        ossr.source = dr("source_supply")
                        ossr.destination = dr("name")
                        ossr.status = dr("status")

                        OSSDict.Add(dr("plateno"), ossr)
                    Catch ex As Exception

                    End Try
                End While

            Catch ex As Exception

            Finally
                conn2.Close()
            End Try
            If Not type = "ALL" Then
                typecmd = " and vt.type='" & type & "'"
            End If

            Dim da As SqlDataAdapter
            If transportertype = "0" Then
                If uid = "ALLUSERS" Then
                    If role = "User" Then
                        da = New SqlDataAdapter("select ut.username,vtt.panic,vtt.powercut,vtt.overspeed,vt.pto,vtt.alarm,vt.plateno,vt.groupname,vt.type,convert(varchar(19),vtt.timestamp,120) as datetime,gps_odometer,speed,gps_av,lat,lon,ignition_sensor,vt.smallimage,vt.bigimage,vtt.bearing,isnull(vt.pmid,'-') as pmid from  vehicle_tracked vtt,vehicleTBL vt,userTBL ut where vtt.plateno=vt.plateno and vt.userid=ut.userid and vt.userid='" & userid & "' " & typecmd & "", conn)
                    ElseIf role = "SuperUser" Or role = "Operator" Then
                        da = New SqlDataAdapter("select ut.username,vtt.panic,vtt.powercut,vtt.overspeed,vt.pto,vtt.alarm,vt.plateno,vt.groupname,vt.type,convert(varchar(19),vtt.timestamp,120) as datetime,gps_odometer,speed,gps_av,lat,lon,ignition_sensor,vt.smallimage,vt.bigimage,vtt.bearing,isnull(vt.pmid,'-') as pmid from  vehicle_tracked vtt,vehicleTBL vt,userTBL ut where vtt.plateno=vt.plateno and vt.userid=ut.userid and vt.userid in(" & userslist & ") " & typecmd & "", conn)
                    Else
                        da = New SqlDataAdapter("select ut.username,vtt.panic,vtt.powercut,vtt.overspeed,vt.pto,vtt.alarm,vt.plateno,vt.groupname,vt.type,convert(varchar(19),vtt.timestamp,120) as datetime,gps_odometer,speed,gps_av,lat,lon,ignition_sensor,vt.smallimage,vt.bigimage,vtt.bearing,isnull(vt.pmid,'-') as pmid from  vehicle_tracked vtt,vehicleTBL vt,userTBL ut where vtt.plateno=vt.plateno and vt.userid=ut.userid " & typecmd & "", conn)
                    End If
                Else
                    da = New SqlDataAdapter("select ut.username,vtt.panic,vtt.powercut,vtt.overspeed,vt.pto,vtt.alarm,vt.plateno,vt.groupname,vt.type,convert(varchar(19),vtt.timestamp,120) as datetime,gps_odometer,speed,gps_av,lat,lon,ignition_sensor,vt.smallimage,vt.bigimage,vtt.bearing,isnull(vt.pmid,'-') as pmid from  vehicle_tracked vtt,vehicleTBL vt,userTBL ut where vtt.plateno=vt.plateno and vt.userid=ut.userid and vt.userid='" & uid & "' " & typecmd & "", conn)
                End If
            Else
                If uid = "ALLUSERS" Then
                    If transportertype = "1" Then
                        da = New SqlDataAdapter("select ut.username,vtt.panic,vtt.powercut,vtt.overspeed,vt.pto,vtt.alarm,vt.plateno,vt.groupname,vt.type,convert(varchar(19),vtt.timestamp,120) as datetime,gps_odometer,speed,gps_av,lat,lon,ignition_sensor,vt.smallimage,vt.bigimage,vtt.bearing,isnull(vt.pmid,'-') as pmid from  vehicle_tracked vtt,vehicleTBL vt,userTBL ut,ytloss.dbo.oss_transporter_master m where vtt.plateno=vt.plateno and vt.userid=ut.userid and vt.transporter_id = m.transporterid and m.internaltype=1 and vt.userid in(" & userslist & ") " & typecmd & "", conn)
                    Else
                        da = New SqlDataAdapter("select ut.username,vtt.panic,vtt.powercut,vtt.overspeed,vt.pto,vtt.alarm,vt.plateno,vt.groupname,vt.type,convert(varchar(19),vtt.timestamp,120) as datetime,gps_odometer,speed,gps_av,lat,lon,ignition_sensor,vt.smallimage,vt.bigimage,vtt.bearing,isnull(vt.pmid,'-') as pmid from  vehicle_tracked vtt,vehicleTBL vt,userTBL ut,ytloss.dbo.oss_transporter_master m where vtt.plateno=vt.plateno and vt.userid=ut.userid and vt.transporter_id = m.transporterid and m.internaltype=0 and vt.userid in(" & userslist & ") " & typecmd & "", conn)
                    End If

                Else
                    da = New SqlDataAdapter("select ut.username,vtt.panic,vtt.powercut,vtt.overspeed,vt.pto,vtt.alarm,vt.plateno,vt.groupname,vt.type,convert(varchar(19),vtt.timestamp,120) as datetime,gps_odometer,speed,gps_av,lat,lon,ignition_sensor,vt.smallimage,vt.bigimage,vtt.bearing,isnull(vt.pmid,'-') as pmid from  vehicle_tracked vtt,vehicleTBL vt,userTBL ut where vtt.plateno=vt.plateno and vt.userid=ut.userid and vt.userid='" & uid & "' " & typecmd & "", conn)
                End If
            End If


            ' '' '' '' '' '' '' '' For My local System
            ' '' '' '' ''If plateno = "" Then
            ' '' '' '' ''    If role = "User" Then
            ' '' '' '' ''        da = New SqlDataAdapter("select ut.username,vtt.panic,vtt.powercut,vtt.overspeed,vtt.alarm,vt.plateno,vt.groupname,vt.type,convert(varchar(19),vtt.timestamp,120) as datetime,gps_odometer,speed,gps_av,lat,lon,ignition_sensor,vt.smallimage,vt.bigimage from  vehicle_tracked vtt,vehicleTBL vt,userTBL ut where vtt.plateno=vt.plateno and vt.userid=ut.userid and vt.userid='" & userid & "'", conn)
            ' '' '' '' ''    ElseIf role = "SuperUser" Or role = "Operator" Then
            ' '' '' '' ''        da = New SqlDataAdapter("select ut.username,vtt.panic,vtt.powercut,vtt.overspeed,vtt.alarm,vt.plateno,vt.groupname,vt.type,convert(varchar(19),vtt.timestamp,120) as datetime,gps_odometer,speed,gps_av,lat,lon,ignition_sensor,vt.smallimage,vt.bigimage from  vehicle_tracked vtt,vehicleTBL vt,userTBL ut where vtt.plateno=vt.plateno and vt.userid=ut.userid and vt.userid in(" & userslist & ")", conn)
            ' '' '' '' ''    Else
            ' '' '' '' ''        da = New SqlDataAdapter("select ut.username,vtt.panic,vtt.powercut,vtt.overspeed,vtt.alarm,vt.plateno,vt.groupname,vt.type,convert(varchar(19),vtt.timestamp,120) as datetime,gps_odometer,speed,gps_av,lat,lon,ignition_sensor,vt.smallimage,vt.bigimage from  vehicle_tracked vtt,vehicleTBL vt,userTBL ut where vtt.plateno=vt.plateno and vt.userid=ut.userid", conn)
            ' '' '' '' ''    End If
            ' '' '' '' ''Else
            ' '' '' '' ''    If searchin = "vht" Then
            ' '' '' '' ''        da = New SqlDataAdapter("select top 1 ut.username,vht.panic,vht.powercut,vht.overspeed,vht.alarm,vt.plateno,vt.groupname,vt.type,convert(varchar(19),vht.timestamp,120) as datetime,gps_odometer,speed,gps_av,lat,lon,ignition_sensor,smallimage,bigimage from  vehicle_history vht,vehicleTBL vt,userTBL ut where vt.userid=ut.userid and vht.plateno='" & plateno & "' and  gps_av = 'A' and vht.timestamp between '" & bdt & "' and '" & edt & "' and vt.plateno='" & plateno & "' order by vht.timestamp desc", conn)
            ' '' '' '' ''    Else
            ' '' '' '' ''        da = New SqlDataAdapter("select ut.username,vt.plateno,vht.alarm,vt.groupname,vt.type,convert(varchar(19),vht.timestamp,120) as datetime,gps_odometer,speed,gps_av,lat,lon,ignition_sensor,smallimage,bigimage from  vehicle_tracked vht,vehicleTBL vt,userTBL ut where vht.plateno='" & plateno & "' and vt.plateno='" & plateno & "' and vt.userid=ut.userid", conn)
            ' '' '' '' ''    End If
            ' '' '' '' ''End If
            Dim ds As New DataSet
            da.Fill(ds)

            Dim lat As Double = 0
            Dim lon As Double = 0


            Dim vehiclepoint As New Point
            Dim aa As New ArrayList()
            Dim status As Int16 = 0
            Dim ossRec As New OssRecord()
            For i As Int32 = 0 To ds.Tables(0).Rows.Count - 1
                Try
                    Dim a As New ArrayList()
                    lat = Math.Round(System.Convert.ToDouble(ds.Tables(0).Rows(i)("lat")), 6)
                    lon = Math.Round(System.Convert.ToDouble(ds.Tables(0).Rows(i)("lon")), 6)
                    status = 0
                    Dim ignition As Byte = 0
                    If ds.Tables(0).Rows(i)("ignition_sensor") = 1 Then
                        ignition = 1
                        If ds.Tables(0).Rows(i)("speed") > 0 Then
                            status = 2
                        Else
                            status = 1
                        End If

                    End If

                    Dim pto As Integer
                    Dim address As String = "--"
                    Dim speed As Double = 0
                    Try
                        speed = Convert.ToDouble(ds.Tables(0).Rows(i)("speed"))
                    Catch ex As Exception

                    End Try
                    a.Add(status)
                    a.Add(HttpUtility.HtmlEncode(ds.Tables(0).Rows(i)("username")))
                    a.Add(HttpUtility.HtmlEncode(ds.Tables(0).Rows(i)("plateno")))
                    a.Add(HttpUtility.HtmlEncode(ds.Tables(0).Rows(i)("groupname")))
                    a.Add(HttpUtility.HtmlEncode(ds.Tables(0).Rows(i)("type")))
                    a.Add(lat)
                    a.Add(lon)
                    a.Add(HttpUtility.HtmlEncode(address))
                    a.Add(ds.Tables(0).Rows(i)("datetime"))
                    a.Add(ds.Tables(0).Rows(i)("speed"))
                    a.Add(ignition)
                    a.Add(ds.Tables(0).Rows(i)("gps_odometer") / 100)
                    If ds.Tables(0).Rows(i)("pto") Then
                        'If True Then
                        If ds.Tables(0).Rows(i)("alarm") = "ON" Then
                            pto = 1
                        Else
                            pto = 0
                        End If
                    Else
                        pto = -1
                    End If
                    a.Add(pto)
                    If ds.Tables(0).Rows(i)("panic") = "ON" Then
                        If (Now - Convert.ToDateTime(ds.Tables(0).Rows(i)("datetime"))).TotalMinutes < 10 Then
                            a.Add(1)
                        Else
                            a.Add(0)
                        End If
                    Else
                        a.Add(0)
                    End If
                    If ds.Tables(0).Rows(i)("powercut") = "ON" Then
                        If (Now - Convert.ToDateTime(ds.Tables(0).Rows(i)("datetime"))).TotalHours < 24 Then
                            If Request.Cookies("userinfo")("customrole") = "LFSuperUser" Then
                                a.Add(0) ' Asked to disable all the Powercut alerts on the map
                                '  a.Add(1)
                            Else
                                a.Add(1)
                            End If
                        Else
                            a.Add(0)
                        End If
                    Else
                        a.Add(0)
                    End If
                    If ds.Tables(0).Rows(i)("overspeed") = "ON" Then
                        If (Now - Convert.ToDateTime(ds.Tables(0).Rows(i)("datetime"))).TotalMinutes < 10 Then
                            If 80 <= speed And speed <= 95 Then
                                If Request.Cookies("userinfo")("customrole") = "LFSuperUser" Then
                                    a.Add(0) ' Asked to disable all the overspeed alerts on the map
                                Else
                                    a.Add(1)
                                End If
                            Else
                                a.Add(0)
                            End If
                        Else
                            a.Add(0)
                        End If
                    Else
                        a.Add(0)
                    End If

                    If OSSDict.ContainsKey(ds.Tables(0).Rows(i)("plateno")) Then
                        ossRec = OSSDict.Item(ds.Tables(0).Rows(i)("plateno"))
                        a.Add(ossRec.source)
                        a.Add(ossRec.destination)
                        a.Add(ossRec.status)
                    Else
                        a.Add("-")
                        a.Add("-")
                        a.Add(0)
                    End If
                    a.Add(ds.Tables(0).Rows(i)("bearing"))
                    a.Add(ds.Tables(0).Rows(i)("pmid"))
                    aa.Add(a)

                Catch ex As Exception

                End Try
            Next

            Dim jss As New Newtonsoft.Json.JsonSerializer()
            Dim json As String = ""
            json = JsonConvert.SerializeObject(aa, Formatting.None)

            Response.ContentType = "text/plain"
            Response.Write(json)

        Catch ex As Exception

        End Try
    End Sub

    Private Structure OssRecord
        Dim source As String
        Dim destination As String
        Dim status As String
        Dim plateno As String
    End Structure
    Sub LoadMapLayers(ByVal map As AspMap.Map)
        Try
            map.AddLayer(Server.MapPath("maps/SmartPoints.shp"))
            map(0).Name = "MapPoints"
        Catch ex As Exception

        End Try
    End Sub

    Sub LoadUserPoints(ByVal map As AspMap.Map)
        Try
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            Dim adocon As ADODB.Connection = New ADODB.Connection()
            Dim userpointsrs As New ADODB.Recordset

            adocon.Open(System.Configuration.ConfigurationManager.AppSettings("sqlserverdsn"))
            userpointsrs.CursorLocation = CursorLocationEnum.adUseClient
            Dim query As String = "select distinct(poiname) as location, lat as y, lon as x from poi_new where (accesstype=0 or accesstype=2)"
            If role = "User" Then
                query = "select distinct(poiname) as location, lat as y, lon as x from poi_new where userid='" & userid & "' and (accesstype=0 or accesstype=2)"
            ElseIf role = "SuperUser" Or role = "Operator" Then
                query = "select distinct(poiname) as location, lat as y, lon as x from poi_new where userid in (" & userslist & ") and (accesstype=0 or accesstype=2)"
            End If

            userpointsrs.Open(query, adocon, CursorTypeEnum.adOpenKeyset, LockTypeEnum.adLockReadOnly, CommandTypeEnum.adCmdText)

            map.AddLayer(userpointsrs)
            map(0).Name = "UserPoints"

        Catch ex As Exception

        End Try
    End Sub
End Class
