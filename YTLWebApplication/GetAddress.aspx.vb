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
            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                a.Add("Unauthorized")
                aa.Add(a)
                Response.ContentType = "application/json"
                Response.Write(JsonConvert.SerializeObject(aa, Formatting.None))
                Return
            End If

            ' SECURITY FIX: Validate coordinate inputs
            Dim latStr As String = Request.QueryString("lat")
            Dim lonStr As String = Request.QueryString("lon")
            
            If Not SecurityHelper.ValidateCoordinate(latStr, lonStr) Then
                a.Add("Invalid coordinates")
                aa.Add(a)
                Response.ContentType = "application/json"
                Response.Write(JsonConvert.SerializeObject(aa, Formatting.None))
                Return
            End If

            Dim vehiclepoint As New Point
            Dim address As String = "--"
            Dim lat As Double = Convert.ToDouble(latStr)
            Dim lon As Double = Convert.ToDouble(lonStr)

            vehiclepoint.Y = lat
            vehiclepoint.X = lon
            map = New AspMap.Map()
            LoadMapLayers(map)
            LoadUserPoints(map)
            
            If lat <> 0 And lon <> 0 Then
                Dim rs As AspMap.Recordset

                ' Search SmartLines layer
                rs = Nothing
                Try
                    rs = map("SmartLines").SearchByDistanceEx(vehiclepoint, 2000 / 111120, SearchMethod.mcInside, "", True)
                    If Not rs.EOF Then
                        Dim di As Double = map.ConvertDistance(map.MeasureDistance(vehiclepoint, rs.Shape.Centroid), 9102, 9036)
                        address = SecurityHelper.HtmlEncode(rs(0).ToString()) & ". (" & di.ToString("0.000") & " KM)"
                    End If
                Catch ex As Exception
                    SecurityHelper.LogError("SmartLines Search Error", ex, Server)
                End Try

                ' Search UserPoints layer
                rs = Nothing
                Try
                    rs = map("UserPoints").SearchByDistanceEx(vehiclepoint, 2000 / 111120, SearchMethod.mcInside, "", True)
                    If Not rs.EOF Then
                        rs.MoveFirst()
                        Dim location As String = SecurityHelper.HtmlEncode(rs.FieldValue("Location").ToString())
                        Dim addresspoint As AspMap.Point = New AspMap.Point
                        addresspoint.X = Convert.ToDouble(rs.FieldValue(2))
                        addresspoint.Y = Convert.ToDouble(rs.FieldValue(1))

                        Dim d As Double = map.ConvertDistance(map.MeasureDistance(vehiclepoint, addresspoint), 9102, 9036)

                        If Not String.IsNullOrEmpty(location) And Not String.IsNullOrEmpty(address) Then
                            address = address & "/" & location & " (" & d.ToString("0.000") & "KM)"
                        ElseIf Not String.IsNullOrEmpty(location) Then
                            address = location & " (" & d.ToString("0.000") & "KM)"
                        End If
                    End If
                Catch ex As Exception
                    SecurityHelper.LogError("UserPoints Search Error", ex, Server)
                End Try
            End If
            
            a.Add(address)
            aa.Add(a)
            
        Catch ex As Exception
            SecurityHelper.LogError("GetAddress Error", ex, Server)
            a.Add("--")
            aa.Add(a)
        End Try
        
        Response.ContentType = "application/json"
        Response.Write(JsonConvert.SerializeObject(aa, Formatting.None))
    End Sub

    Sub LoadMapLayers(ByVal map As AspMap.Map)
        Try
            map.AddLayer(Server.MapPath("maps/SmartLines.shp"))
            map(0).Name = "SmartLines"
        Catch ex As Exception
            SecurityHelper.LogError("LoadMapLayers Error", ex, Server)
        End Try
    End Sub

    Sub LoadUserPoints(ByVal map As AspMap.Map)
        Try
            ' SECURITY FIX: Get validated user information
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)

            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
                Dim query As String
                Dim cmd As SqlCommand

                ' SECURITY FIX: Use parameterized queries based on role
                If role = "Admin" Then
                    query = "SELECT DISTINCT poiname as location, lat as y, lon as x FROM poi_new WHERE accesstype IN (0, 2)"
                    cmd = New SqlCommand(query, conn)
                ElseIf role = "User" Then
                    query = "SELECT DISTINCT poiname as location, lat as y, lon as x FROM poi_new WHERE userid = @userid AND accesstype IN (0, 2)"
                    cmd = New SqlCommand(query, conn)
                    cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@userid", userid, SqlDbType.Int))
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    If SecurityHelper.IsValidUsersList(userslist) Then
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
                        query = $"SELECT DISTINCT poiname as location, lat as y, lon as x FROM poi_new WHERE userid IN ({inClause}) AND accesstype IN (0, 2)"
                        cmd.CommandText = query
                        cmd.Connection = conn
                    Else
                        Return ' Invalid users list
                    End If
                Else
                    Return ' Invalid role
                End If

                conn.Open()
                Using dr As SqlDataReader = cmd.ExecuteReader()
                    ' Create ADO recordset for AspMap compatibility
                    Dim adocon As New ADODB.Connection()
                    Dim userpointsrs As New ADODB.Recordset()
                    
                    adocon.Open(System.Configuration.ConfigurationManager.AppSettings("sqlserverdsn"))
                    userpointsrs.CursorLocation = CursorLocationEnum.adUseClient
                    userpointsrs.Open(query, adocon, CursorTypeEnum.adOpenKeyset, LockTypeEnum.adLockReadOnly, CommandTypeEnum.adCmdText)

                    map.AddLayer(userpointsrs)
                    map(0).Name = "UserPoints"
                End Using
            End Using

        Catch ex As Exception
            SecurityHelper.LogError("LoadUserPoints Error", ex, Server)
        End Try
    End Sub
End Class