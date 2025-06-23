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
        Try
            ' SECURITY FIX: Validate user session for non-client requests
            Dim reqfrom As String = SecurityHelper.HtmlEncode(Request.QueryString("from"))
            
            If reqfrom <> "Client" Then
                If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                    Response.StatusCode = 401
                    Response.Write("Unauthorized")
                    Return
                End If
            End If

            ' SECURITY FIX: Validate coordinate parameters
            Dim xStr As String = Request.QueryString("x")
            Dim yStr As String = Request.QueryString("y")
            Dim zStr As String = Request.QueryString("z")

            If Not Integer.TryParse(xStr, x) OrElse Not Integer.TryParse(yStr, y) OrElse Not Integer.TryParse(zStr, z) Then
                Response.StatusCode = 400
                Response.Write("Invalid coordinates")
                Return
            End If

            ' SECURITY FIX: Validate coordinate ranges
            If x < 0 OrElse y < 0 OrElse z < 0 OrElse z > 20 Then
                Response.StatusCode = 400
                Response.Write("Coordinates out of range")
                Return
            End If

            ' SECURITY FIX: Rate limiting
            If SecurityHelper.IsRateLimited(Request.UserHostAddress, 100, 1) Then
                Response.StatusCode = 429
                Response.Write("Rate limit exceeded")
                Return
            End If

            map = New AspMap.Map()
            Dim size As Int64 = Math.Pow(2, z) * 256
            map.Width = size
            map.Height = size

            tempmap = New AspMap.Map()
            tempmap.BackColor = RGB(0, 0, 0)
            tempmap.Width = 256
            tempmap.Height = 256

            If (Session("map") Is Nothing) Or (Session("MapRefresh") Is Nothing) Or (Session("MapRefresh") = "Y") Then
                LoadGeofenceLayer()
                
                ' SECURITY FIX: Validate plateno parameter
                Dim plateno As String = SecurityHelper.HtmlEncode(Request.QueryString("plateno"))
                If Not String.IsNullOrEmpty(plateno) AndAlso SecurityHelper.ValidatePlateNumber(plateno) Then
                    ' Load vehicle route if plateno is valid
                End If

                Session("map") = tempmap
                Session("MapRefresh") = "N"
            Else
                tempmap = Session("map")
            End If

            ' SECURITY FIX: Validate layer parameter
            Dim layerParam As String = SecurityHelper.HtmlEncode(Request.QueryString("l"))
            If SecurityHelper.ValidateNumeric(layerParam, 1, 3) Then
                Select Case layerParam
                    Case "1"
                        tempmap("Geofence Layer").Visible = True
                    Case "2"
                        tempmap("Geofence Layer").Visible = False
                    Case "3"
                        tempmap("Geofence Layer").Visible = False
                    Case Else
                        tempmap("Geofence Layer").Visible = True
                End Select
            Else
                tempmap("Geofence Layer").Visible = True
            End If

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

        Catch ex As Exception
            SecurityHelper.LogError("GussmannMaps error", ex, Server)
            Response.StatusCode = 500
            Response.Write("Internal server error")
        End Try
    End Sub

    Private Sub LoadGeofenceLayer()
        Try
            ' SECURITY FIX: Get validated user information
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)

            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
                Dim cmd As SqlCommand

                ' SECURITY FIX: Use parameterized queries based on role
                If role = "User" Then
                    cmd = New SqlCommand("SELECT geofencename as label, data, geofencetype FROM geofence WHERE userid = @userid OR accesstype = '1'", conn)
                    cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@userid", userid, SqlDbType.Int))
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    If Not String.IsNullOrEmpty(userslist) AndAlso SecurityHelper.IsValidUsersList(userslist) Then
                        ' Create parameterized query for multiple user IDs
                        Dim userIds() As String = userslist.Split(","c)
                        Dim parameters As New List(Of String)
                        cmd = New SqlCommand()
                        
                        For i As Integer = 0 To userIds.Length - 1
                            Dim paramName As String = "@userid" & i
                            parameters.Add(paramName)
                            cmd.Parameters.Add(SecurityHelper.CreateSqlParameter(paramName, userIds(i).Trim(), SqlDbType.Int))
                        Next
                        
                        Dim inClause As String = String.Join(",", parameters)
                        cmd.CommandText = $"SELECT geofencename as label, data, geofencetype FROM geofence WHERE userid IN ({inClause}) OR accesstype = '1'"
                        cmd.Connection = conn
                    Else
                        cmd = New SqlCommand("SELECT geofencename as label, data, geofencetype FROM geofence WHERE userid = @userid OR accesstype = '1'", conn)
                        cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@userid", userid, SqlDbType.Int))
                    End If
                Else
                    cmd = New SqlCommand("SELECT geofencename as label, data, geofencetype FROM geofence WHERE accesstype = '1'", conn)
                End If

                Dim dl As New AspMap.DynamicLayer
                dl.LayerType = LayerType.mcPolygonLayer

                conn.Open()
                Using dr As SqlDataReader = cmd.ExecuteReader()
                    While dr.Read()
                        Try
                            Dim geofenceType As Boolean = Convert.ToBoolean(dr("geofencetype"))
                            Dim data As String = dr("data").ToString()
                            Dim label As String = SecurityHelper.HtmlEncode(dr("label").ToString())

                            If Not geofenceType Then
                                ' Circle geofence
                                Dim lnltrd() As String = data.Split(","c)
                                If lnltrd.Length = 3 Then
                                    Dim lat, lon As Double
                                    Dim radius As Integer
                                    
                                    If Double.TryParse(lnltrd(1), lat) AndAlso Double.TryParse(lnltrd(0), lon) AndAlso Integer.TryParse(lnltrd(2), radius) Then
                                        If SecurityHelper.ValidateCoordinate(lat.ToString(), lon.ToString()) AndAlso radius > 0 AndAlso radius < 50000 Then
                                            Dim radians As Double = lat * (Math.PI / 180)
                                            lat = (Math.Log(Math.Abs(Math.Tan(radians) + (1 / Math.Cos(radians)))) * 180) / Math.PI

                                            Dim circleshp As New AspMap.Shape()
                                            circleshp.MakeCircle(lon, lat, radius / (60 * 1852))
                                            dl.AddShape(circleshp, label)
                                        End If
                                    End If
                                End If
                            Else
                                ' Polygon geofence
                                Dim ptslayer As New AspMap.Points
                                Dim shp As New AspMap.Shape
                                shp.ShapeType = ShapeType.mcPolygonShape

                                Dim pots() As String = data.Split(";"c)
                                Dim validPoints As Boolean = True
                                
                                For i As Integer = 0 To pots.Length - 1
                                    Dim vals() As String = pots(i).Split(","c)
                                    If vals.Length = 2 Then
                                        Dim lat, lon As Double
                                        If Double.TryParse(vals(1), lat) AndAlso Double.TryParse(vals(0), lon) Then
                                            If SecurityHelper.ValidateCoordinate(lat.ToString(), lon.ToString()) Then
                                                Dim radians As Double = lat * (Math.PI / 180)
                                                lat = (Math.Log(Math.Abs(Math.Tan(radians) + (1 / Math.Cos(radians)))) * 180) / Math.PI
                                                ptslayer.AddPoint(lon, lat)
                                            Else
                                                validPoints = False
                                                Exit For
                                            End If
                                        Else
                                            validPoints = False
                                            Exit For
                                        End If
                                    Else
                                        validPoints = False
                                        Exit For
                                    End If
                                Next
                                
                                If validPoints AndAlso ptslayer.Count > 2 Then
                                    shp.AddPart(ptslayer)
                                    dl.AddShape(shp, label)
                                End If
                            End If

                        Catch ex As Exception
                            SecurityHelper.LogError("Geofence processing error", ex, Server)
                            Continue While
                        End Try
                    End While
                End Using
            End Using

            tempmap.AddLayer(dl)
            tempmap(0).Name = "Geofence Layer"

            Dim mlayer As AspMap.Layer = tempmap.Layer("Geofence Layer")
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

        Catch ex As Exception
            SecurityHelper.LogError("LoadGeofenceLayer error", ex, Server)
        End Try
    End Sub

End Class