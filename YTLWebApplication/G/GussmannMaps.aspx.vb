Imports System.Drawing
Imports System
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports System.Drawing.Drawing2D
Imports System.Data.SqlClient
Imports AspMap
Imports System.IO
Imports ADODB

Partial Class GussmannMaps
    Inherits System.Web.UI.Page
    Public map, tempmap As AspMap.Map
    Dim point As AspMap.Point
    Dim x As Integer
    Dim y As Integer
    Dim z As Integer
    Dim rs As ADODB.Recordset
    Public errormessage As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim reqfrom As String = ""
        reqfrom = Request.QueryString("from")
        If reqfrom <> "Client" Then
            x = System.Convert.ToInt32(Request.QueryString("x"))
            y = System.Convert.ToInt32(Request.QueryString("y"))
            z = System.Convert.ToInt32(Request.QueryString("z"))

            map = New AspMap.Map()
            Dim size As Int64 = Math.Pow(2, z) * 256
            map.Width = size
            map.Height = size

            tempmap = New AspMap.Map()
            tempmap.BackColor = RGB(0, 0, 0)
            tempmap.Width = 256
            tempmap.Height = 256


            If (Session("map") Is Nothing) Or (Session("MapRefresh") Is Nothing) Or (Session("MapRefresh") = "Y") Then

                'ShowUserPoints()
                'LoadPolygonGeofenceLayer()
                'LoadCircleGeofenceLayer()
                LoadGeofenceLayer()
                If Request.QueryString("plateno") <> Nothing Then
                    '  ShowVehicleRoute()
                End If

                Session("map") = tempmap
                Session("MapRefresh") = "N"

            Else
                tempmap = Session("map")
            End If

            'tempmap("UserPoints1").Visible = True
            Select Case Request.QueryString("l")
                Case "1"
                    tempmap("Geofence Layer").Visible = True
                    'For poitype As Int16 = 1 To 55
                    '    tempmap("UserPoints" & poitype).Visible = False
                    'Next
                Case "2"
                    'For poitype As Int16 = 1 To 55
                    '    tempmap("UserPoints" & poitype).Visible = True
                    'Next
                    tempmap("Geofence Layer").Visible = False
                Case "3"
                    tempmap("Geofence Layer").Visible = False
                    'For poitype As Int16 = 1 To 55
                    '    tempmap("UserPoints" & poitype).Visible = False
                    'Next
                Case Else
                    tempmap("Geofence Layer").Visible = True
                    'For poitype As Int16 = 1 To 55
                    '    tempmap("UserPoints" & poitype).Visible = True
                    'Next
            End Select


            Dim temprect As AspMap.Rectangle = New AspMap.Rectangle()

            temprect.Left = -180
            temprect.Top = 180
            temprect.Right = 180
            temprect.Bottom = -180

            map.FullExtent = temprect
            temprect.Deflate(0.0000000001, 0.0000000001)
            map.Extent = temprect

            Dim rect As AspMap.Rectangle = New AspMap.Rectangle()

            point = map.ToMapPoint((x * 256), (y * 256))

            rect.Left = point.X
            rect.Top = point.Y

            point = map.ToMapPoint((x * 256 + 255), (y * 256 + 255))

            rect.Right = point.X
            rect.Bottom = point.Y

            tempmap.FullExtent = rect
            rect.Deflate(0.0000000001, 0.0000000001)
            tempmap.Extent = rect

            tempmap.ImageFormat = AspMap.ImageFormat.mcPNG

            Dim simg() As Byte = tempmap.Image

            Dim dimg((simg.Length - 1) + 13) As Byte
            Array.Copy(simg, dimg, 813)
            Dim mybytes() As Byte = {0, 0, 0, 1, 116, 82, 78, 83, 0, 64, 230, 216, 102}
            Array.Copy(mybytes, 0, dimg, 813, 13)
            Array.Copy(simg, 813, dimg, 826, simg.Length - 813)

            Response.ContentType = "image/png"
            Response.BinaryWrite(dimg)
        End If

    End Sub


    Sub ShowUserPoints()
        Try
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            Dim da As SqlDataAdapter = New SqlDataAdapter("select distinct(poiname) as location, lat as y, lon as x,poitype as type from poi_new", conn)

            If role = "User" Then
                da = New SqlDataAdapter("select distinct(poiname) as location, lat as y, lon as x,poitype as type from poi_new where userid='" & userid & "' and (accesstype=0 or accesstype=2)", conn)
            ElseIf role = "SuperUser" Then
                da = New SqlDataAdapter("select distinct(poiname) as location, lat as y, lon as x,poitype as type from poi_new where userid in(" & userslist & ") and (accesstype=0 or accesstype=2)", conn)
            End If

            Dim ds As New DataSet
            da.Fill(ds)

            Dim poipoints As AspMap.DynamicPoints
            Dim dv As DataView

            Dim latitude As Double = 0
            Dim radians As Double = 0

            For poitype As Int16 = 1 To 55

                poipoints = New AspMap.DynamicPoints
                dv = New DataView(ds.Tables(0), "type= " & poitype & "", "location", DataViewRowState.CurrentRows)

                For i As Int32 = 0 To dv.Count - 1
                    latitude = dv.Item(i).Row()("y")
                    radians = latitude * (Math.PI / 180)
                    latitude = (Math.Log(Math.Abs(Math.Tan(radians) + (1 / Math.Cos(radians)))) * 180) / Math.PI
                    poipoints.AddPoint(dv.Item(i).Row()("x"), latitude, dv.Item(i).Row()("location"))
                Next

                poipoints.Type = LayerType.mcPointLayer

                tempmap.AddLayer(poipoints)

                Dim mlayer As AspMap.Layer
                tempmap(0).Name = "UserPoints" & poitype

                mlayer = tempmap.Layer("UserPoints" & poitype)
                mlayer.ShowLabels = True

                mlayer.Symbol.PointStyle = PointStyle.mcBitmapPoint
                mlayer.Symbol.Bitmap = Server.MapPath("images/" & poitype & ".bmp")
                mlayer.Symbol.TransparentColor = RGB(255, 255, 255)
                mlayer.Symbol.Size = 20

                mlayer.LabelFont.Antialias = True
                mlayer.LabelFont.Name = "Tahoma"
                mlayer.LabelFont.Size = 13
                mlayer.LabelFont.Color = RGB(0, 0, 128)
                mlayer.LabelFont.Outline = True

                mlayer.Visible = True

            Next
        Catch ex As Exception

        End Try

    End Sub

    Sub ShowVehicleRoute()

        Dim plateno As String = Request.QueryString("plateno")
        Dim begindatetime As String = Request.QueryString("bdt")
        Dim enddatetime As String = Request.QueryString("edt")

        If Not plateno = "" Then

            Dim mlayer As AspMap.Layer

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim da As SqlDataAdapter
            Dim ds As DataSet

            'Vehicle Route Layer

            da = New SqlDataAdapter("select distinct convert(varchar(19),timestamp,120) as datetime,lat as y,lon as x,ignition_sensor,bearing from vehicle_history where plateno ='" & plateno & "' and gps_av = 'A' and timestamp between '" & begindatetime & "' and '" & enddatetime & "'", conn)


            ds = New DataSet
            da.Fill(ds)

            Dim vehicleroutepoints As New AspMap.DynamicPoints
            Dim ignitiononpoints As New AspMap.DynamicPoints
            Dim ignitionoffpoints As New AspMap.DynamicPoints

            Dim latitude As Double = 0
            Dim radians As Double = 0

            If ds.Tables(0).Rows.Count > 0 Then
                For i As Int64 = 0 To ds.Tables(0).Rows.Count - 1
                    latitude = ds.Tables(0).Rows(i)("y")
                    radians = latitude * (Math.PI / 180)
                    latitude = (Math.Log(Math.Abs(Math.Tan(radians) + (1 / Math.Cos(radians)))) * 180) / Math.PI
                    vehicleroutepoints.AddPoint(ds.Tables(0).Rows(i)("x"), latitude, ds.Tables(0).Rows(i)("datetime"))
                    If ds.Tables(0).Rows(i)("ignition_sensor") = "1" Then
                        ignitiononpoints.AddPoint(ds.Tables(0).Rows(i)("x"), latitude, ds.Tables(0).Rows(i)("datetime"))
                    Else
                        ignitionoffpoints.AddPoint(ds.Tables(0).Rows(i)("x"), latitude, ds.Tables(0).Rows(i)("datetime"))
                    End If
                Next
            Else

            End If
            vehicleroutepoints.Type = LayerType.mcLineLayer

            tempmap.AddLayer(vehicleroutepoints)

            Dim vehicleroutelayer = tempmap.Layer(0)
            vehicleroutelayer.ShowLabels = False

            vehicleroutelayer.Symbol.LineStyle = LineStyle.mcDashRoadLine
            vehicleroutelayer.Symbol.LineColor = RGB(198, 0, 0)
            vehicleroutelayer.Symbol.InnerColor = RGB(244, 150, 92)
            vehicleroutelayer.Symbol.Size = 4


            'Vehicle Ignition ON Points Layer
            ignitiononpoints.Type = LayerType.mcPointLayer

            tempmap.AddLayer(ignitiononpoints)
            tempmap(0).Name = "VehicleIgnitionOnPointsLayer"

            mlayer = tempmap.Layer("VehicleIgnitionOnPointsLayer")
            mlayer.ShowLabels = True

            mlayer.Symbol.Size = 8
            mlayer.Symbol.PointStyle = PointStyle.mcSquareWithSmallCenter
            mlayer.Symbol.FillColor = RGB(0, 225, 0)
            mlayer.Symbol.LineColor = RGB(10, 10, 10)
            mlayer.Symbol.InnerColor = RGB(10, 10, 10)

            mlayer.LabelFont.Antialias = True
            mlayer.LabelFont.Name = "Verdana"
            mlayer.LabelFont.Size = 13
            mlayer.LabelFont.Color = RGB(0, 128, 0)
            mlayer.LabelFont.Bold = True
            mlayer.LabelFont.Outline = True

            mlayer.Visible = True



            'Vehicle Ignition OFF Points Layer
            ignitionoffpoints.Type = LayerType.mcPointLayer

            tempmap.AddLayer(ignitionoffpoints)
            tempmap(0).Name = "VehicleIgnitionOffPointsLayer"

            mlayer = tempmap.Layer("VehicleIgnitionOffPointsLayer")
            mlayer.ShowLabels = True

            mlayer.Symbol.Size = 8
            mlayer.Symbol.PointStyle = PointStyle.mcSquareWithSmallCenter
            mlayer.Symbol.FillColor = RGB(255, 0, 0)
            mlayer.Symbol.LineColor = RGB(10, 10, 10)
            mlayer.Symbol.InnerColor = RGB(10, 10, 10)

            mlayer.LabelFont.Antialias = True
            mlayer.LabelFont.Name = "Verdana"
            mlayer.LabelFont.Size = 13
            mlayer.LabelFont.Color = RGB(225, 0, 128)
            mlayer.LabelFont.Bold = True
            mlayer.LabelFont.Outline = True

            mlayer.Visible = True

        End If
    End Sub

    Sub LoadCircleGeofenceLayer()
        Try
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            ' Dim connection As New Redirect(userid)

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As New SqlCommand("select geofencename as label,data from  geofence where geofencetype='0'", conn)

            If role = "User" Then
                cmd = New SqlCommand("select geofencename as label,data from  geofence where geofencetype='0' and userid='" & userid & "' Or (accesstype='1' and geofencetype='0')", conn)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select geofencename as label,data from  geofence where  geofencetype='0' userid in(" & userslist & ") Or (accesstype='1' and geofencetype='0')", conn)
            End If

            Dim dr As SqlDataReader

            Dim circlepoints As New AspMap.DynamicPoints
            circlepoints.Type = LayerType.mcPointLayer
            Try
                conn.Open()
                dr = cmd.ExecuteReader()
                Dim latitude As Double = 0
                Dim radians As Double = 0
                While dr.Read()
                    Try
                        latitude = dr("data").ToString().Split(",")(1)
                        radians = latitude * (Math.PI / 180)
                        latitude = (Math.Log(Math.Abs(Math.Tan(radians) + (1 / Math.Cos(radians)))) * 180) / Math.PI
                        circlepoints.AddPoint(dr("data").ToString().Split(",")(0), latitude, dr("label"))
                    Catch ex As Exception

                    End Try
                End While
                conn.Close()
            Catch ex As Exception

            End Try
            tempmap.AddLayer(circlepoints)

            Dim mlayer As AspMap.Layer
            tempmap(0).Name = "Circle"

            mlayer = tempmap.Layer("Circle")
            mlayer.ShowLabels = True
            mlayer.LabelField = "label"

            mlayer.LabelFont.Antialias = True
            mlayer.LabelFont.Name = "Tahoma"
            mlayer.LabelFont.Size = 13
            mlayer.LabelFont.Color = RGB(0, 0, 128)
            mlayer.LabelFont.Outline = True
            mlayer.LabelFont.Bold = True

            mlayer.Visible = True


        Catch ex As Exception

        End Try
    End Sub

    Sub LoadPolygonGeofenceLayer()
        Try
            Dim plateno As String = Request.QueryString("plateno")
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            ' Dim connection As New Redirect(userid)

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As New SqlCommand("select geofencename as label,data from  geofence where geofencetype='1'", conn)

            If role = "User" Then
                cmd = New SqlCommand("select geofencename as label,data from  geofence where geofencetype='1' and userid='" & userid & "' Or (geofencetype='1' and accesstype='1')", conn)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select geofencename as label,data from  geofence where  geofencetype='1' userid in(" & userslist & ") Or (geofencetype='1' and accesstype='1')", conn)
            End If

            Dim dr As SqlDataReader

            Dim polygonpoints As New AspMap.DynamicPoints
            polygonpoints.Type = LayerType.mcPointLayer
            Try
                conn.Open()
                dr = cmd.ExecuteReader()
                Dim latitude As Double = 0
                Dim radians As Double = 0
                While dr.Read()
                    Try
                        latitude = dr("data").ToString().Split(";")(0).Split(",")(1)
                        radians = latitude * (Math.PI / 180)
                        latitude = (Math.Log(Math.Abs(Math.Tan(radians) + (1 / Math.Cos(radians)))) * 180) / Math.PI
                        polygonpoints.AddPoint(dr("data").ToString().Split(";")(0).Split(",")(0), latitude, dr("label"))
                    Catch ex As Exception

                    End Try
                End While
                conn.Close()
            Catch ex As Exception

            End Try
            tempmap.AddLayer(polygonpoints)

            Dim mlayer As AspMap.Layer
            tempmap(0).Name = "Polygon"

            mlayer = tempmap.Layer("Polygon")
            mlayer.ShowLabels = True
            mlayer.LabelField = "label"

            mlayer.LabelFont.Antialias = True
            mlayer.LabelFont.Name = "Tahoma"
            mlayer.LabelFont.Size = 20
            mlayer.LabelFont.Color = RGB(0, 0, 128)
            mlayer.LabelFont.Outline = True
            mlayer.LabelFont.Bold = True

            mlayer.Visible = True

        Catch ex As Exception

        End Try
    End Sub

    Private Sub LoadGeofenceLayer()
        Try
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            ' Dim connection As New Redirect(userid)

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As New SqlCommand("select geofencename as label,data,geofencetype from  geofence", conn)

            If role = "User" Then
                cmd = New SqlCommand("select geofencename as label,data,geofencetype from  geofence where userid='" & userid & "' Or (accesstype='1')", conn)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select geofencename as label,data,geofencetype from  geofence where userid in(" & userslist & ") Or (accesstype='1')", conn)
            End If

            Dim dr As SqlDataReader

            Dim dl As New AspMap.DynamicLayer
            dl.LayerType = LayerType.mcPolygonLayer

            Try
                conn.Open()
                dr = cmd.ExecuteReader()
                Dim latitude As Double = 0
                Dim radians As Double = 0
                While dr.Read()
                    Try
                        If Not dr("geofencetype") Then
                            Dim lnltrd() As String = dr("data").ToString().Split(",")
                            latitude = lnltrd(1)
                            radians = latitude * (Math.PI / 180)
                            latitude = (Math.Log(Math.Abs(Math.Tan(radians) + (1 / Math.Cos(radians)))) * 180) / Math.PI

                            Dim circleshp As New AspMap.Shape()
                            circleshp.MakeCircle(lnltrd(0), latitude, lnltrd(2) / (60 * 1852))
                            dl.AddShape(circleshp, dr("label"))
                            ' pointslayer.AddPoint(dr("data").ToString().Split(",")(0), latitude, dr("label"))
                        Else
                            Try
                                Dim data As String = dr("data")
                                Dim ptslayer As New AspMap.Points
                                Dim shp As New AspMap.Shape
                                shp.ShapeType = ShapeType.mcPolygonShape

                                Dim pots() As String = data.Split(";")
                                Dim vals() As String
                                For i As Integer = 0 To pots.Length - 1
                                    vals = pots(i).Split(",")

                                    latitude = vals(1)
                                    radians = latitude * (Math.PI / 180)
                                    latitude = (Math.Log(Math.Abs(Math.Tan(radians) + (1 / Math.Cos(radians)))) * 180) / Math.PI
                                    ptslayer.AddPoint(vals(0), latitude)
                                Next
                                shp.AddPart(ptslayer)
                                dl.AddShape(shp, dr("label"))
                                ' pointslayer.AddPoint(shp.Centroid.X, latitude, dr("label"))
                            Catch ex As Exception

                            End Try

                        End If

                    Catch ex As Exception

                    End Try
                End While
                conn.Close()
            Catch ex As Exception

            End Try
            tempmap.AddLayer(dl)

            tempmap(0).Name = "Geofence Layer"

            Dim mlayer As AspMap.Layer
            mlayer = tempmap.Layer("Geofence Layer")

            mlayer.ShowLabels = True
            mlayer.LabelField = "label"

            mlayer.Symbol.FillStyle = FillStyle.mcTransparentFill
            mlayer.Symbol.LineStyle = LineStyle.mcInvisibleLine

            mlayer.LabelFont.Antialias = True
            mlayer.LabelFont.Name = "Tahoma"
            mlayer.LabelFont.Size = 14
            mlayer.LabelFont.Color = RGB(0, 0, 128)
            mlayer.LabelFont.Outline = True

            mlayer.Visible = True

            'mlayer.Opacity = 0.25

        Catch ex As Exception

        End Try
    End Sub

End Class


