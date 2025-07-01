Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Xml
Imports System.IO
Imports System.Collections.Generic
Imports System.Data.SqlClient

Public Class VehicleLogData
    Inherits System.Web.UI.Page
    Public show As Boolean = False
    Public ec As String = "false"
    Public plateno As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Enable authentication check
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.StatusCode = 401
                Response.Write("Unauthorized")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Validate and sanitize all query string parameters
            Dim user_id As String = SecurityHelper.SanitizeString(Request.QueryString("user_id"), 50)
            Dim plate_no As String = SecurityHelper.SanitizeString(Request.QueryString("plate_no"), 20)
            Dim begin_time As String = SecurityHelper.SanitizeString(Request.QueryString("begin_time"), 50)
            Dim end_time As String = SecurityHelper.SanitizeString(Request.QueryString("end_time"), 50)
            Dim format As String = SecurityHelper.SanitizeString(Request.QueryString("format"), 10)
            Dim lat As String = SecurityHelper.SanitizeString(Request.QueryString("lat"), 20)
            Dim lon As String = SecurityHelper.SanitizeString(Request.QueryString("lon"), 20)

            ' SECURITY FIX: Validate numeric parameters
            Dim interval As Integer = 0
            Dim ignition As Integer = -1
            Dim show_address As Integer = 0
            Dim speed As Integer = 0

            If Not String.IsNullOrEmpty(Request.QueryString("interval")) Then
                Integer.TryParse(Request.QueryString("interval"), interval)
            End If
            If Not String.IsNullOrEmpty(Request.QueryString("ignition")) Then
                Integer.TryParse(Request.QueryString("ignition"), ignition)
            End If
            If Not String.IsNullOrEmpty(Request.QueryString("show_address")) Then
                Integer.TryParse(Request.QueryString("show_address"), show_address)
            End If
            If Not String.IsNullOrEmpty(Request.QueryString("speed")) Then
                Integer.TryParse(Request.QueryString("speed"), speed)
            End If

            ' SECURITY FIX: Validate inputs
            If Not SecurityHelper.ValidateUserId(user_id) AndAlso user_id <> "All" Then
                Response.StatusCode = 400
                Response.Write("Invalid user ID")
                Response.End()
                Return
            End If

            If Not SecurityHelper.ValidatePlateNumber(plate_no) Then
                Response.StatusCode = 400
                Response.Write("Invalid plate number")
                Response.End()
                Return
            End If

            If Not String.IsNullOrEmpty(begin_time) AndAlso Not SecurityHelper.ValidateDate(begin_time) Then
                Response.StatusCode = 400
                Response.Write("Invalid begin time")
                Response.End()
                Return
            End If

            If Not String.IsNullOrEmpty(end_time) AndAlso Not SecurityHelper.ValidateDate(end_time) Then
                Response.StatusCode = 400
                Response.Write("Invalid end time")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Validate coordinates if provided
            If Not String.IsNullOrEmpty(lat) AndAlso Not String.IsNullOrEmpty(lon) Then
                If Not SecurityHelper.ValidateCoordinate(lat, lon) Then
                    Response.StatusCode = 400
                    Response.Write("Invalid coordinates")
                    Response.End()
                    Return
                End If
            End If

            ' SECURITY FIX: Validate format parameter
            If Not String.IsNullOrEmpty(format) AndAlso format <> "csv" AndAlso format <> "json" Then
                Response.StatusCode = 400
                Response.Write("Invalid format")
                Response.End()
                Return
            End If

            ' Process the request with validated parameters
            ProcessVehicleLogData(user_id, plate_no, begin_time, end_time, interval, ignition, show_address, speed, format, lat, lon)

        Catch ex As Exception
            SecurityHelper.LogError("VehicleLogData Page_Load Error", ex, Server)
            Response.StatusCode = 500
            Response.Write("Internal server error")
            Response.End()
        End Try
    End Sub

    Private Sub ProcessVehicleLogData(user_id As String, plate_no As String, begin_time As String, end_time As String, interval As Integer, ignition As Integer, show_address As Integer, speed As Integer, format As String, lat As String, lon As String)
        Try
            Dim eList As New List(Of VehicleData)
            Dim address As String = ""

            Dim locuid As String = ""
            Dim suserid As String = user_id

            ' SECURITY FIX: Validate user access
            If String.IsNullOrEmpty(suserid) OrElse suserid = "All" Then
                suserid = SecurityHelper.ValidateAndGetUserId(Request)
                locuid = "0"
            Else
                locuid = suserid
            End If

            ' SECURITY FIX: Handle group users
            If suserid.IndexOf(",") > 0 Then
                Dim sgroupname As String() = suserid.Split(","c)
                If sgroupname.Length > 0 AndAlso SecurityHelper.ValidateUserId(sgroupname(0)) Then
                    suserid = sgroupname(0)
                    locuid = suserid
                Else
                    Response.StatusCode = 400
                    Response.Write("Invalid group user ID")
                    Response.End()
                    Return
                End If
            End If

            Dim locobj As New Location(locuid)

            Dim query As String
            Dim param As New Dictionary(Of String, Object)

            If Not String.IsNullOrEmpty(lat) Then
                ' SECURITY FIX: Use parameterized query for current location
                query = "SELECT TOP 1 convert(varchar(20),timestamp,120) as datetime, gps_av,speed,odometer,ignition,lat,lon,alarm FROM vehicle_tracked2_table WHERE plateno = @plateno"
                param.Add("@plateno", plate_no)
            Else
                ' SECURITY FIX: Use parameterized query for historical data
                query = "SELECT DISTINCT convert(varchar(20),timestamp,120) as datetime, gps_av,speed,odometer,ignition,lat,lon,alarm FROM vehicle_history2_table WHERE plateno = @plateno AND timestamp BETWEEN @begin_time AND @end_time and gps_av='A'"
                param.Add("@plateno", plate_no)
                param.Add("@begin_time", begin_time)
                param.Add("@end_time", end_time)

                If ignition > -1 Then
                    query &= " AND ignition_sensor = @ignition"
                    param.Add("@ignition", ignition)
                End If
            End If

            Dim dt As DataTable = SecurityHelper.ExecuteSecureQuery(query, param)

            Dim firstdatetime As DateTime
            Dim seconddatetime As DateTime
            Dim prev_address As String = ""
            Dim prev_lat As String = ""
            Dim prev_log As String = ""
            Dim i As Int64 = 1

            If dt.Rows.Count > 0 Then
                For Each dr As DataRow In dt.Rows
                    seconddatetime = dr("datetime")
                    
                    If ((seconddatetime - firstdatetime).TotalMinutes >= interval) Then
                        firstdatetime = seconddatetime
                        
                        If System.Convert.ToDouble(dr("speed")) >= speed Then
                            Dim myVehicleData As VehicleData = New VehicleData
                            myVehicleData.number = i
                            i = i + 1
                            myVehicleData.datetime = SecurityHelper.HtmlEncode(dr("datetime").ToString())
                            myVehicleData.gpsav = SecurityHelper.HtmlEncode(dr("gps_av").ToString())
                            myVehicleData.speed = System.Convert.ToDouble(dr("speed")).ToString("0.00")
                            myVehicleData.odometer = System.Convert.ToDouble(dr("odometer")).ToString("0.00")
                            myVehicleData.ignition = If(dr("ignition"), "On", "Off")

                            address = ""

                            If show_address = 1 Then
                                ' SECURITY FIX: Validate coordinates before processing
                                If SecurityHelper.ValidateCoordinate(dr("lat").ToString(), dr("lon").ToString()) Then
                                    If dr("lat").ToString() = prev_lat And dr("lon").ToString() = prev_log Then
                                        address = prev_address
                                    Else
                                        address = SecurityHelper.HtmlEncode(locobj.GetLocation(dr("lat"), dr("lon")))
                                        prev_address = address
                                        prev_lat = dr("lat").ToString()
                                        prev_log = dr("lon").ToString()
                                    End If
                                End If
                            End If

                            myVehicleData.address = address

                            ' SECURITY FIX: Validate coordinates
                            If SecurityHelper.ValidateCoordinate(dr("lat").ToString(), dr("lon").ToString()) Then
                                myVehicleData.lat = dr("lat").ToString()
                                myVehicleData.lon = dr("lon").ToString()
                            Else
                                myVehicleData.lat = "0"
                                myVehicleData.lon = "0"
                            End If

                            If Not IsDBNull(dr("alarm")) Then
                                If Convert.ToBoolean(dr("alarm")) Then
                                    myVehicleData.pto = "1"
                                Else
                                    myVehicleData.pto = "0"
                                End If
                            Else
                                myVehicleData.pto = "0"
                            End If

                            eList.Add(myVehicleData)
                        End If
                    End If
                Next
            End If

            ' SECURITY FIX: Output data based on format
            If format = "csv" Then
                OutputCSV(eList, begin_time, end_time)
            Else
                OutputJSON(eList)
            End If

        Catch ex As Exception
            SecurityHelper.LogError("ProcessVehicleLogData Error", ex, Server)
            Response.StatusCode = 500
            Response.Write("Internal server error")
            Response.End()
        End Try
    End Sub

    Private Sub OutputCSV(eList As List(Of VehicleData), begin_time As String, end_time As String)
        Try
            ' SECURITY FIX: Sanitize filename
            Dim sanitizedBeginTime As String = begin_time.Replace(":", "").Replace(" ", "_").Replace("/", "")
            Dim sanitizedEndTime As String = end_time.Replace(":", "").Replace(" ", "_").Replace("/", "")

            Response.AddHeader("content-disposition", "attachment;filename=VehicleLogReport_" & sanitizedBeginTime & "_" & sanitizedEndTime & ".csv")
            Response.ContentType = "text/csv"
            Response.Write("No,Ignition,Time,GPS AV,Speed,Odometer,Lat,Lon,Address" & vbCrLf)

            For Each item In eList
                ' SECURITY FIX: Sanitize CSV output
                Dim csvLine As String = String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                    item.number,
                    SecurityHelper.SanitizeForHtml(item.ignition),
                    SecurityHelper.SanitizeForHtml(item.datetime),
                    SecurityHelper.SanitizeForHtml(item.gpsav),
                    item.speed,
                    item.odometer,
                    item.lat,
                    item.lon,
                    SecurityHelper.SanitizeForHtml(item.address).Replace(",", " "))

                Response.Write(csvLine & vbCrLf)
            Next

        Catch ex As Exception
            SecurityHelper.LogError("OutputCSV Error", ex, Server)
            Response.StatusCode = 500
            Response.Write("Error generating CSV")
            Response.End()
        End Try
    End Sub

    Private Sub OutputJSON(eList As List(Of VehicleData))
        Try
            Dim json As String = Newtonsoft.Json.JsonConvert.SerializeObject(eList, Newtonsoft.Json.Formatting.None)
            Response.ContentType = "application/json"
            Response.Write(json)

        Catch ex As Exception
            SecurityHelper.LogError("OutputJSON Error", ex, Server)
            Response.StatusCode = 500
            Response.Write("Error generating JSON")
            Response.End()
        End Try
    End Sub

    Public Class VehicleData
        Public number As Int64
        Public datetime As String
        Public gpsav As String
        Public speed As String
        Public odometer As String
        Public ignition As String
        Public address As String
        Public lat As String
        Public lon As String
        Public pto As String
    End Class
End Class