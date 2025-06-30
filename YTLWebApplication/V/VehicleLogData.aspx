<%@ Page Language="vb" AutoEventWireup="false" %>

<%@ Import Namespace="Newtonsoft.Json" %>

<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="ADODB" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Drawing" %>
<script runat="server">
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Request.Cookies("userinfo") Is Nothing Then
            Response.Redirect("Login.aspx")
        End If
	
        Dim eList As New List(Of VehicleData)
        Dim address As String = ""
	
        Dim user_id As String
        Dim interval As Byte
        Dim plate_no As String
        Dim begin_time As String
        Dim end_time As String
        Dim ignition As Integer
        Dim show_address As Integer
        Dim format As String
        Dim speed As Integer
        Dim lat As String
        Dim lon As String
        Dim pto As Boolean
        user_id = Request.QueryString("user_id")
        plate_no = Request.QueryString("plate_no")
        interval = Request.QueryString("interval")
		
        begin_time = Request.QueryString("begin_time")
        end_time = Request.QueryString("end_time")
       
        ignition = -1
        show_address = Request.QueryString("show_address")
        format = Request.QueryString("format")
        speed = Request.QueryString("speed")
	
        lat = Request.QueryString("lat")
        lon = Request.QueryString("lon")
        Dim i As Int64 = 1

        Dim connstr As String
        connstr = System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection")
        Dim conn As New SqlConnection(connstr)
        Dim query As String
                                                         

        Dim locuid As String = ""
        Dim suserid As String
        Dim suser As String
        Dim nu As String
        Dim ign As String
        Dim tm As String
        Dim spd As String
        Dim odo As String
        Dim lt As String
        Dim ln As String
        Dim adr As String
        nu = Request.QueryString("nu")
        ign = Request.QueryString("ign")
        tm = Request.QueryString("tm")
        spd = Request.QueryString("spd")
        odo = Request.QueryString("odo")
        lt = Request.QueryString("lt")
        ln = Request.QueryString("ln")
        adr = Request.QueryString("adr")

        suserid = Request.QueryString("user_id")
        locuid = suserid
        If Request.QueryString("user_id") Is Nothing Then
            If Request.QueryString("user_id") <> "All" Then
                suserid = Request.Cookies("userinfo")("userid")
                locuid = "0"
            Else
                suserid = "0"
                locuid = "0"
            End If
            If Request.Cookies("userinfo")("role") = "Admin" Then
                suserid = "1990"
                locuid = "0"
            End If
        End If
        If suserid.IndexOf(",") > 0 Then
            Dim sgroupname As String() = suserid.Split(",")
            suser = sgroupname(0)
            locuid = suser
        End If
        Dim locobj As New Location(locuid)

        '		query &= " ORDER BY datetime"
                                                       
                                                         
        If (lat <> "") Then
            query = "SELECT TOP 1 convert(varchar(20),timestamp,120) as datetime, gps_av,speed,odometer,ignition,lat,lon,alarm FROM vehicle_tracked2_table " & _
             " WHERE plateno like '" & plate_no & "'"
        Else
            If (ignition > -1) Then
                query &= " AND ignition_sensor =" & ignition
            End If
            query = "SELECT DISTINCT convert(varchar(20),timestamp,120)  as datetime, gps_av,speed,odometer,ignition,lat,lon,alarm  FROM vehicle_history2_table " & _
             " WHERE plateno = '" & plate_no & "' AND timestamp BETWEEN '" & begin_time & "' AND'" & end_time & "' and gps_av='A'"
        End If
              
        Dim cmd As SqlCommand = New SqlCommand(query, conn)
        
        Dim dr As SqlDataReader
        
        Dim firstdatetime As DateTime
        Dim seconddatetime As DateTime
        Dim distance As Double
        Dim prev_address As String
        Dim prev_lat As String
        Dim prev_log As String
	
        Try
            conn.Open()
            dr = cmd.ExecuteReader()

            While dr.Read()
                seconddatetime = dr("datetime")
                If ((seconddatetime - firstdatetime).TotalMinutes >= interval) Then
                    firstdatetime = seconddatetime
                    If System.Convert.ToDouble(dr("speed")) >= speed Then
							
                        Dim myVehicleData As VehicleData = New VehicleData
                        myVehicleData.number = i
                        i = i + 1
                        myVehicleData.datetime = dr("datetime")
                        myVehicleData.gpsav = dr("gps_av")
                        myVehicleData.speed = System.Convert.ToDouble(dr("speed")).ToString("0.00")
                        myVehicleData.odometer = System.Convert.ToDouble(dr("odometer")).ToString("0.00")
                        myVehicleData.ignition = IIf(dr("ignition"), "On", "Off")
                        
		
                        address = ""
		
						
						
		                
                        If show_address = 1 Then
                            If dr("lat") = prev_lat And dr("lon") = prev_log Then
                                address = prev_address
                            Else
                                address = locobj.GetLocation(dr("lat"), dr("lon"))
                                prev_address = address
                                prev_lat = dr("lat")
                                prev_log = dr("lon")
                            End If
                        End If

                        myVehicleData.address = address
	                    

                        myVehicleData.lat = dr("lat")
                        myVehicleData.lon = dr("lon")
                        If (dr("alarm") = True) Then
                            myVehicleData.pto = "1"
                        ElseIf (dr("alarm") = False) Then
                            myVehicleData.pto = "0"
                        End If
		
                        eList.Add(myVehicleData)
                    End If
	
                End If
            End While

        Catch ex As Exception
            Response.Write(ex.Message)
        Finally
            conn.Close()
        End Try
        
        'Response.Write("connection open<br>")
        'Dim number As Integer
        'number = dr.FieldCount		
        'Response.Write("Fields : " & number & "<br>")
        '      If dr.HasRows Then
        '		While dr.Read()
        '          	Response.Write(dr("speed"))
        '      	End While
        'End If
        'Response.Write("connection close<br>")
        If format = "csv" Then
            Dim item As VehicleData
            begin_time = Replace(begin_time, ":", "")
            begin_time = Replace(begin_time, " ", "_")
            begin_time = Replace(begin_time, "/", "")
            end_time = Replace(end_time, ":", "")
            end_time = Replace(end_time, " ", "_")
            end_time = Replace(end_time, "/", "")
			
            Response.AddHeader("content-disposition", "attachment;filename=VehicleLogReport_" & begin_time & "_" & end_time & ".csv")
            Response.ContentType = "text/csv"
              Response.Write(""+ nu +","+ ign +","+ tm +",GPS AV,"+ spd +","+ odo  +","+ lt +","+ ln +",Address" & vbCrLf)
            'Response.Write("No,Ignition,Time,GPS AV,Speed,Odometer,Lat,Lon,Address" & vbCrLf)
            For Each item In eList
                If ((item.address.Split(",").Length = 2 And (item.address.Contains(">") = False)) Or item.address = "") Then
                    Response.Write(item.number & "," & item.ignition & "," & item.datetime & "," & item.gpsav & "," & item.speed & "," & item.odometer & "," & item.lat & "," & item.lon & "," & item.address.Replace(",", " ") & vbCrLf)
                ElseIf (item.address.Contains("/b") = True) Then
                    Response.Write(item.number & "," & item.ignition & "," & item.datetime & "," & item.gpsav & "," & item.speed & "," & item.odometer & "," & item.lat & "," & item.lon & "," & item.address.Split(">")(3).Split("<")(0).Trim() & vbCrLf)
                Else
                    Response.Write(item.number & "," & item.ignition & "," & item.datetime & "," & item.gpsav & "," & item.speed & "," & item.odometer & "," & item.lat & "," & item.lon & "," & item.address.Split(">")(2).Replace(",", " ").Trim() & vbCrLf)
             
                    
                End If
                
                  
                
            Next
        Else
            Dim jss As New Newtonsoft.Json.JsonSerializer()


            Dim json As String  = JsonConvert.SerializeObject(elist, Formatting.None)
         
            Response.ContentType = "application/json"
            Response.Write(json)
            'Dim p As String = "\\/Date\((\d+)\+\d+\)\\/"
            'Dim matchEvaluator As MatchEvaluator = New MatchEvaluator(AddressOf ConvertJsonDateToDateString)
         
            'Dim reg As New Regex(p)
        
            'ans = reg.Replace(ans, matchEvaluator)
         
        End If
    End Sub
    
    'Public Function ConvertJsonDateToDateString(ByVal m As Match) As String
    '    Dim result As String = ""
    '    Dim dt As DateTime = New DateTime(1970, 1, 1)
    '    dt = dt.AddMilliseconds(Long.Parse(m.Groups(1).Value))
    '    dt = dt.ToLocalTime()
    '    result = dt.ToString("yyyy-MM-dd HH:mm:ss")
    '    Return result
    'End Function
    
    Public Class VehicleData
        Public number As Int64
        Public datetime As String
        Public gpsav As String
        Public speed As Double
        Public odometer As Double
        Public ignition As String
        Public address As String
        Public lat As String
        Public lon As String
        Public pto As String
    End Class
	
       	
</script>
