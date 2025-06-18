Imports AspMap
Imports ADODB
Imports System.Data
Imports System.Data.SqlClient
Imports Newtonsoft.Json
Partial Class GetAddress
    Inherits System.Web.UI.Page
    Public map, tempmap As AspMap.Map
    Dim point As AspMap.Point
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim a As New ArrayList()
        Dim aa As New ArrayList()
        Try
            Dim vehiclepoint As New Point
            Dim address As String = "--"
            Dim lat As Double = Convert.ToDouble(Request.QueryString("lat"))
            Dim lon As Double = Convert.ToDouble(Request.QueryString("lon"))

            vehiclepoint.Y = lat
            vehiclepoint.X = lon
            map = New AspMap.Map()
            LoadMapLayers(map)
            LoadUserPoints(map)
            If lat <> 0 And lon <> 0 Then

                Dim rs As AspMap.Recordset

                rs = map("SmartLines").SearchByDistanceEx(vehiclepoint, 2000 / 111120, SearchMethod.mcInside, "", True)
                If (rs.RecordCount > 0) Then
                    Dim di As Double = map.ConvertDistance(map.MeasureDistance(vehiclepoint, rs.Shape.Centroid), 9102, 9036)
                    address = rs(0) & ". (" & di.ToString("0.000") & " KM)"
                End If

                rs = map("UserPoints").SearchByDistanceEx(vehiclepoint, 2000 / 111120, SearchMethod.mcInside, "", True)
                If (rs.RecordCount > 0) Then
                    rs.MoveFirst()
                    Dim location As String = rs.FieldValue("Location")
                    Dim addresspoint As AspMap.Point = New AspMap.Point
                    addresspoint.X = rs.FieldValue(2)
                    addresspoint.Y = rs.FieldValue(1)

                    Dim d As Double = map.ConvertDistance(map.MeasureDistance(vehiclepoint, addresspoint), 9102, 9036)

                    If location <> "" And address <> "" Then
                        address = address & "/" & location & " (" & d.ToString("0.000") & "KM)"
                    ElseIf location <> "" Then
                        address = location & " (" & d.ToString("0.000") & "KM)"
                    End If
                End If
            End If
            a.Add(address)
            aa.Add(a)
        Catch ex As Exception
            a.Add("--")
            aa.Add(a)
        End Try
        Response.ContentType = "text/plain"
        Response.Write(JsonConvert.SerializeObject(aa, Formatting.None))

    End Sub

    Sub LoadMapLayers(ByVal map As AspMap.Map)
        Try
            map.AddLayer(Server.MapPath("maps/SmartLines.shp"))
            map(0).Name = "SmartLines"
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
