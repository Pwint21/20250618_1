Imports AspMap
Imports System.Data.SqlClient
Imports System.Xml
Imports ADODB

Partial Class GussmannMap
    Inherits System.Web.UI.Page
    Dim map As AspMap.Map
    Dim z As Byte = 15
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Dim x As Double = System.Convert.ToDouble(Request.QueryString("x"))
            Dim y As Double = System.Convert.ToDouble(Request.QueryString("y"))
            Dim size As Byte = System.Convert.ToDouble(Request.QueryString("size"))
            Dim point As AspMap.Point = New AspMap.Point()
            point.X = x
            point.Y = y

            map = New AspMap.Map()
            map.BackColor = RGB(153, 179, 204)


            'If (Session("Map") Is Nothing) Or (Session("MiniMap") Is Nothing) Or (Session("MiniMap") = False) Then

            If size = "2" Then
                map.Width = 150
                map.Height = 150
            Else
                map.Width = 256
                map.Height = 256
            End If

            LoadMainLayers()
            LoadAreaLayers()
            LoadLineLayers()
            LoadPointLayers()
            'ShowUserPoints()
            ShowGF()
            'Session("MiniMap") = True
            'Session("Map") = map
            'Else
            'map = Session("Map")
            'End If

            If Not Request.Cookies("userinfo")("companyname") = "LAFARGE" Then
                ShowUserPoints()
            End If

            Dim rect As AspMap.Rectangle = New AspMap.Rectangle()
            rect.Left = x - 0.01
            rect.Top = y + 0.01
            rect.Right = x + 0.01
            rect.Bottom = y - 0.01

            map.FullExtent = rect


            Dim callout As Callout = map.Callouts.Add()
            callout.X = x
            callout.Y = y
            callout.Text = "Location"
            callout.Font.Size = 14
            callout.Font.Name = "Verdana"
            callout.Font.Outline = True
            callout.BackColor = RGB(255, 255, 128) ' yellow
            callout.LineColor = RGB(0, 0, 0) ' black

            'callout.XIndent = 10
            'callout.YIndent = 10

            map.ImageFormat = AspMap.ImageFormat.mcPNG
            Response.ContentType = "image/png"
            Response.BinaryWrite(map.Image)

		map.Dispose()

            'map.Callouts.Remove(0)

            'map.ImageFormat = AspMap.ImageFormat.mcPNG

            'Dim bmp As Bitmap = New Bitmap(New System.IO.MemoryStream(CType(map.Image, Byte())))

            'Dim tempbmp As Bitmap = New Bitmap(200, 200, System.Drawing.Imaging.PixelFormat.Format24bppRgb)
            'tempbmp.SetResolution(60, 60)

            'Dim copyright As String = "© 2007 - Gussmann Technologies"
            'Dim watermarkphoto As Graphics = Graphics.FromImage(tempbmp)
            'watermarkphoto.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias

            'watermarkphoto.DrawImage(bmp, 0, 0, 200, 200)

            'Dim myfont As System.Drawing.Font = New System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold)
            'Dim StrFormat As StringFormat = New StringFormat()
            'StrFormat.Alignment = StringAlignment.Center

            'Dim semiTransBrush2 As SolidBrush = New SolidBrush(Color.FromArgb(153, 0, 0, 0))
            'watermarkphoto.DrawString(copyright, myfont, semiTransBrush2, New PointF(128, 128), StrFormat)
            'Dim semiTransBrush As SolidBrush = New SolidBrush(Color.FromArgb(153, 255, 255, 255))
            'watermarkphoto.DrawString(copyright, myfont, semiTransBrush, New PointF(127, 127), StrFormat)

            'Response.ContentType = "image/png"
            'Dim mstream As New System.IO.MemoryStream()
            'tempbmp.Save(mstream, System.Drawing.Imaging.ImageFormat.Png)
            'mstream.WriteTo(Response.OutputStream)
            'Response.BinaryWrite(mstream.GetBuffer())
        Catch ex As Exception
            Response.Write(ex.Message)
	map.Dispose()
        End Try

    End Sub

    Sub LoadMainLayers()
        Try
            Dim mlayer As AspMap.Layer

            map.AddLayer(Server.MapPath("maps/MalaysiaAndSingapore/Malaysia.shp"))
            mlayer = map.Layer("Malaysia")

            mlayer.ShowLabels = False

            mlayer.Symbol.LineStyle = LineStyle.mcSolidLine
            mlayer.Symbol.FillStyle = FillStyle.mcSolidFill
            mlayer.Symbol.Size = 1
            mlayer.Symbol.LineColor = RGB(242, 239, 233)
            mlayer.Symbol.FillColor = RGB(242, 239, 233)


            map.AddLayer(Server.MapPath("maps/World/country.shp"))
            mlayer = map.Layer("country")

            mlayer.ShowLabels = False
            mlayer.LabelField = "cntry_name"

            mlayer.LabelStyle = LabelStyle.mcPolygonCenter
            mlayer.Symbol.LineStyle = LineStyle.mcSolidLine
            mlayer.Symbol.FillStyle = FillStyle.mcSolidFill
            mlayer.Symbol.Size = 1
            mlayer.Symbol.LineColor = RGB(153, 179, 204)
            'mlayer.Symbol.FillColor = RGB(242, 239, 233) 'Google
            mlayer.Symbol.FillColor = RGB(241, 238, 232) 'OpenStreetMap
            mlayer.LabelFont.Name = "Verdana"
            mlayer.LabelFont.Size = 14
            mlayer.LabelFont.Color = RGB(0, 0, 0)
            mlayer.LabelFont.Outline = True

            mlayer.Visible = True

            map.AddLayer(Server.MapPath("maps/World/level4.shp"))
            mlayer = map.Layer("level4")

            mlayer.ShowLabels = False
            mlayer.LabelField = "level_name"

            mlayer.LabelStyle = LabelStyle.mcPolygonCenter
            mlayer.Symbol.LineStyle = LineStyle.mcInvisibleLine
          

            mlayer.Symbol.FillStyle = FillStyle.mcSolidFill
            mlayer.Symbol.Size = 1
            mlayer.Symbol.LineColor = RGB(153, 179, 204)
            mlayer.Symbol.FillColor = RGB(242, 239, 233)
            mlayer.LabelFont.Name = "Verdana"
            mlayer.LabelFont.Size = 14
            mlayer.LabelFont.Color = RGB(0, 0, 0)
            mlayer.LabelFont.Outline = True


            map.AddLayer(Server.MapPath("maps/World/rivers.shp"))
            mlayer = map.Layer("rivers")

            mlayer.ShowLabels = True
            mlayer.LabelField = "NAME"

            mlayer.Symbol.LineStyle = LineStyle.mcSolidLine
            mlayer.Symbol.FillStyle = FillStyle.mcSolidFill
            mlayer.Symbol.LineColor = RGB(153, 179, 204)
            mlayer.Symbol.FillColor = RGB(153, 179, 204)
            mlayer.Symbol.Size = 3

            mlayer.LabelFont.Antialias = True
            mlayer.LabelFont.Name = "Tahoma"
            mlayer.LabelFont.Bold = True
            mlayer.LabelFont.Size = 12
            mlayer.LabelFont.Color = RGB(100, 150, 225)
            mlayer.LabelFont.Outline = True

            mlayer.Visible = False

        Catch ex As Exception

        End Try
    End Sub

    Sub LoadAreaLayers()
        Try

            'Create the XML Document
            Dim xmldoc As XmlDocument = New XmlDocument()
            Dim areanodelist As XmlNodeList
            Dim areanode As XmlNode

            Dim symbolnode As XmlNode
            Dim labelfontnode As XmlNode

            Dim subtype As String

            Dim mlayer As AspMap.Layer

            'Load the Xml file
            xmldoc.Load(Server.MapPath("Maps/Config/MapAreas.xml"))

            'Get the list of name nodes 
            areanodelist = xmldoc.SelectNodes("Areas/Area")

            For Each areanode In areanodelist
                'Get the type Attribute Value

                subtype = areanode.Attributes("type").Value
                symbolnode = areanode.ChildNodes(0)
                labelfontnode = areanode.ChildNodes(1)


                map.AddLayer(Server.MapPath("maps/MalaysiaAndSingapore/Areas/" & subtype & ".shp"))

                mlayer = map.Layer(0)

                mlayer.LabelField = "Label"
                mlayer.ShowLabels = Convert.ToBoolean(areanode.Attributes("showlabels").Value)

                mlayer.Symbol.FillColor = HexToInt(symbolnode.Attributes("fillcolor").Value)
                mlayer.Symbol.FillStyle = Convert.ToInt32(symbolnode.Attributes("fillstyle").Value)
                mlayer.Symbol.LineColor = HexToInt(symbolnode.Attributes("linecolor").Value)
                mlayer.Symbol.LineStyle = Convert.ToInt32(symbolnode.Attributes("linestyle").Value)

                mlayer.LabelFont.Antialias = Convert.ToBoolean(labelfontnode.Attributes("antialias").Value)
                mlayer.LabelFont.Bold = Convert.ToBoolean(labelfontnode.Attributes("bold").Value)
                mlayer.LabelFont.Name = labelfontnode.Attributes("name").Value
                mlayer.LabelFont.Size = labelfontnode.Attributes("size").Value
                mlayer.LabelFont.Color = HexToInt(labelfontnode.Attributes("color").Value)
                mlayer.LabelFont.Outline = labelfontnode.Attributes("outline").Value

                mlayer.Visible = Convert.ToBoolean(areanode.Attributes("visible").Value)

            Next

        Catch ex As Exception

        End Try
    End Sub

    Sub LoadLineLayers()
        Try

            'Create the XML Document
            Dim xmldoc As XmlDocument = New XmlDocument()
            Dim linenodelist As XmlNodeList
            Dim linenode As XmlNode

            Dim symbolnode As XmlNode
            Dim zoomnode As XmlNode
            Dim labelfontnode As XmlNode

            Dim subtype As String

            Dim mlayer As AspMap.Layer

            'Load the Xml file
            xmldoc.Load(Server.MapPath("Maps/Config/MapLines.xml"))

            'Get the list of name nodes 
            linenodelist = xmldoc.SelectNodes("Lines/Line")

            For Each linenode In linenodelist
                'Get the type Attribute Value

                subtype = linenode.Attributes("type").Value
                symbolnode = linenode.ChildNodes(0)
                zoomnode = linenode.ChildNodes(1).ChildNodes(z)
                labelfontnode = linenode.ChildNodes(2)


                map.AddLayer(Server.MapPath("maps/MalaysiaAndSingapore/Lines/" & subtype & ".shp"))

                mlayer = map.Layer(0)

                mlayer.LabelField = "Label"
                mlayer.ShowLabels = Convert.ToBoolean(zoomnode.Attributes("showlabels").Value)

                mlayer.Symbol.FillColor = HexToInt(symbolnode.Attributes("fillcolor").Value)
                mlayer.Symbol.InnerColor = HexToInt(symbolnode.Attributes("innercolor").Value)
                mlayer.Symbol.LineColor = HexToInt(symbolnode.Attributes("linecolor").Value)
                mlayer.Symbol.LineStyle = Convert.ToInt32(symbolnode.Attributes("linestyle").Value)
                mlayer.Symbol.Size = Convert.ToInt32(zoomnode.Attributes("size").Value)

                mlayer.LabelFont.Antialias = Convert.ToBoolean(labelfontnode.Attributes("antialias").Value)
                mlayer.LabelFont.Bold = Convert.ToBoolean(labelfontnode.Attributes("bold").Value)
                mlayer.LabelFont.Name = labelfontnode.Attributes("name").Value
                mlayer.LabelFont.Size = labelfontnode.Attributes("size").Value
                mlayer.LabelFont.Color = HexToInt(labelfontnode.Attributes("color").Value)
                mlayer.LabelFont.Outline = labelfontnode.Attributes("outline").Value

                mlayer.Visible = Convert.ToBoolean(zoomnode.Attributes("visible").Value)

            Next

        Catch ex As Exception

        End Try
    End Sub

    Sub LoadPointLayers()
        Try

            'Create the XML Document
            Dim xmldoc As XmlDocument = New XmlDocument()
            Dim pointnodelist As XmlNodeList
            Dim pointnode As XmlNode

            Dim symbolnode As XmlNode
            Dim labelfontnode As XmlNode

            Dim subtype As String

            Dim mlayer As AspMap.Layer

            'Load the Xml file
            xmldoc.Load(Server.MapPath("Maps/Config/MapPoints.xml"))

            'Get the list of name nodes 
            pointnodelist = xmldoc.SelectNodes("Points/Point")

            For Each pointnode In pointnodelist
                'Get the type Attribute Value

                If z >= Convert.ToByte(pointnode.Attributes("zoom").Value) Then

                    subtype = pointnode.Attributes("type").Value
                    symbolnode = pointnode.ChildNodes(0)
                    labelfontnode = pointnode.ChildNodes(1)

                    map.AddLayer(Server.MapPath("maps/MalaysiaAndSingapore/Points/" & subtype & ".shp"))

                    mlayer = map.Layer(0)
                    mlayer.LabelField = "Label"
                    mlayer.ShowLabels = True

                    mlayer.Symbol.PointStyle = PointStyle.mcBitmapPoint
                    mlayer.Symbol.Bitmap = Server.MapPath("images/mappoints/" & symbolnode.Attributes("bitmap").Value & "")
                    mlayer.Symbol.TransparentColor = RGB(255, 255, 255)
                    mlayer.Symbol.Size = symbolnode.Attributes("size").Value

                    mlayer.LabelFont.Name = labelfontnode.Attributes("name").Value
                    mlayer.LabelFont.Size = labelfontnode.Attributes("size").Value
                    mlayer.LabelFont.Color = HexToInt(labelfontnode.Attributes("color").Value)
                    mlayer.LabelFont.Outline = labelfontnode.Attributes("outline").Value

                    mlayer.Visible = Convert.ToBoolean(pointnode.Attributes("visible").Value)
                End If

            Next

        Catch ex As Exception

        End Try
    End Sub

    Sub AddMapLayer(ByVal Map, ByVal strLayerPath)

        If Not Map.AddLayer(strLayerPath) Then
            Throw New System.Exception("Attempt to add a layer has failed: " & strLayerPath)
        End If

    End Sub

    Sub ShowUserPoints()

        Try

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            'Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim connection As New Redirect(userid)
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings(connection.sqlConnection))

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

            For poitype As Int16 = 1 To 57

                poipoints = New AspMap.DynamicPoints
                dv = New DataView(ds.Tables(0), "type= " & poitype & "", "location", DataViewRowState.CurrentRows)

                For i As Int32 = 0 To dv.Count - 1
                    poipoints.AddPoint(dv.Item(i).Row()("x"), dv.Item(i).Row()("y"), dv.Item(i).Row()("location"))
                Next

                poipoints.Type = LayerType.mcPointLayer

                map.AddLayer(poipoints)

                Dim mlayer As AspMap.Layer
                map(0).Name = "UserPoints" & poitype

                mlayer = map.Layer("UserPoints" & poitype)
                mlayer.ShowLabels = True

                mlayer.Symbol.PointStyle = PointStyle.mcBitmapPoint
                mlayer.Symbol.Bitmap = Server.MapPath("images/" & poitype & ".bmp")
                mlayer.Symbol.TransparentColor = RGB(255, 255, 255)
                mlayer.Symbol.Size = 20

                mlayer.LabelFont.Name = "Tahoma"
                mlayer.LabelFont.Size = 13
                mlayer.LabelFont.Color = RGB(0, 0, 128)
                mlayer.LabelFont.Outline = True

                mlayer.Visible = True

            Next
        Catch ex As Exception

        End Try

    End Sub

    Sub ShowGF()
        Try
            Dim plateno As String = Request.QueryString("plateno")
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            Dim query As String

            If Request.Cookies("userinfo")("companyname") = "LAFARGE" Then
                query = "select geofencename,lat,lon,radius from  geofence_new_public"
            Else
                query = "select geofencename,lat,lon,radius from  geofence_new"
                If role = "User" Then
                    query = "select geofencename,lat,lon,radius from  geofence_new where userid='" & userid & "'"
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    query = "select geofencename,lat,lon,radius from  geofence_new where userid in(" & userslist & ")"
                End If
            End If

            Dim cmd As New SqlCommand(query, conn)
            Dim dr As SqlDataReader

            Dim radius As Byte

            Dim geofenceLayer As New AspMap.DynamicLayer
            geofenceLayer.LayerType = LayerType.mcPolygonLayer

            Try
                conn.Open()
                dr = cmd.ExecuteReader()
                While dr.Read()
                    Try
                        radius = dr("radius")
                        Dim circleshape As New AspMap.Shape
                        circleshape.MakeCircle(Convert.ToDouble(dr("lon")), Convert.ToDouble(dr("lat")), radius / (60 * 1852))

                        geofenceLayer.AddShape(circleshape, dr("geofencename"))
                    Catch ex As Exception

                    End Try
                End While
            Catch ex As Exception

            Finally
                conn.Close()
            End Try

            map.AddLayer(geofenceLayer)
            map(0).Name = "Geofences"

            Dim mlayer As AspMap.Layer
            mlayer = map.Layer("Geofences")

            mlayer.ShowLabels = True
            mlayer.LabelField = "label"

            mlayer.Symbol.FillColor = RGB(125, 255, 0)
            mlayer.Symbol.LineColor = RGB(0, 128, 0)
            mlayer.Symbol.Size = 2

            mlayer.LabelFont.Antialias = True
            mlayer.LabelFont.Name = "Tahoma"
            mlayer.LabelFont.Size = 14
            mlayer.LabelFont.Color = RGB(0, 0, 128)
            mlayer.LabelFont.Outline = True

            mlayer.Visible = True

            mlayer.Opacity = 0.2

        Catch ex As Exception

        End Try
    End Sub

    Shared Function HexToInt(ByVal value As String) As Int32
        Return RGB(System.Convert.ToInt32(value.Substring(0, 2), 16), System.Convert.ToInt32(value.Substring(2, 2), 16), System.Convert.ToInt32(value.Substring(4, 2), 16))
    End Function

End Class
