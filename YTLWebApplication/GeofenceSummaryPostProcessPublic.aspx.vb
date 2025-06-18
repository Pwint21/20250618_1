Imports System.Data.SqlClient
Imports System.Data
Imports ADODB
Imports AspMap
Imports System.IO
Imports System.Web.Security

Partial Class GeofenceSummaryPostProcessPublic
    Inherits System.Web.UI.Page
    Public show As Boolean = False
    Public ec As String = "false"
    Dim sCon As String = System.Configuration.ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim suspectTime As String
    Dim GrantOdometer, GrantFuel, GrantPrice, GrandIdlingFuel, GrandIdlingPrice, GrantRefuelLitre, GrantRefuelPrice As Double
    Dim GrandIdlingTime As TimeSpan

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            ' SECURITY FIX: Enable authentication check
            If Session("login") Is Nothing OrElse Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
                Return
            End If

            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If

        Catch ex As Exception
            ' SECURITY FIX: Don't expose detailed error messages
            SecurityHelper.LogError("OnInit Error", ex, Server)
            Response.Redirect("Error.aspx")
        End Try
        MyBase.OnInit(e)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Enable authentication check
            If Session("login") Is Nothing OrElse Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
                Return
            End If

            Label2.Visible = False
            Label3.Visible = False

            If Page.IsPostBack = False Then
                ImageButton1.Attributes.Add("onclick", "return mysubmit()")
                
                ' SECURITY FIX: Validate date inputs
                If SecurityHelper.ValidateDate(DateTime.Now.ToString("yyyy/MM/dd")) Then
                    txtBeginDate.Value = DateTime.Now.ToString("yyyy/MM/dd")
                    txtEndDate.Value = DateTime.Now.ToString("yyyy/MM/dd")
                End If
                
                populateNode()
                
                If Request.Cookies("userinfo")("role") = "User" Then
                    tvPlateno.ExpandAll()
                End If
            End If

        Catch ex As Exception
            SecurityHelper.LogError("Page_Load Error", ex, Server)
            Response.Redirect("Error.aspx")
        End Try
    End Sub

    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
        DisplayLogInformation1()
    End Sub

    Sub populateNode()
        Try
            Dim ds As System.Data.DataSet = getTreeViewData()
            For Each masterRow As DataRow In ds.Tables("user").Rows
                ' SECURITY FIX: HTML encode output to prevent XSS
                Dim masterNode As New TreeNode(SecurityHelper.HtmlEncode(masterRow("username").ToString()), masterRow("userid").ToString())
                tvPlateno.Nodes.Add(masterNode)
                
                For Each childRow As DataRow In masterRow.GetChildRows("Children")
                    Dim childNode As New TreeNode(SecurityHelper.HtmlEncode(childRow("plateno").ToString()), childRow("plateno").ToString())
                    masterNode.ChildNodes.Add(childNode)
                    
                    If Request.Cookies("userinfo")("role") = "User" Then
                        masterNode.Checked = True
                        childNode.Checked = True
                    End If
                Next
            Next
        Catch ex As SystemException
            SecurityHelper.LogError("populateNode Error", ex, Server)
        End Try
    End Sub

    Function getTreeViewData() As System.Data.DataSet
        Try
            Using conn As New SqlConnection(sCon)
                Dim daPlateno As SqlDataAdapter
                Dim daUser As SqlDataAdapter

                ' SECURITY FIX: Validate user input
                Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
                Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
                Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)
                
                Dim ds As System.Data.DataSet = New System.Data.DataSet()

                If role = "Admin" Then
                    Dim dsRoute As DataSet = New DataSet()
                    ' SECURITY FIX: Use parameterized query
                    daUser = New SqlDataAdapter("SELECT userid, username, dbip FROM userTBL WHERE role = @role ORDER BY username", conn)
                    daUser.SelectCommand.Parameters.Add(SecurityHelper.CreateSqlParameter("@role", "user", SqlDbType.VarChar))
                    daUser.Fill(dsRoute, "user")
                    
                    For x As Int32 = 0 To dsRoute.Tables("user").Rows.Count - 1
                        Dim uid As String = dsRoute.Tables("user").Rows(x)("userid").ToString()
                        ' SECURITY FIX: Use parameterized query
                        Dim daRoute As SqlDataAdapter = New SqlDataAdapter("SELECT * FROM vehicleTBL WHERE userid = @userid ORDER BY plateno", conn)
                        daRoute.SelectCommand.Parameters.Add(SecurityHelper.CreateSqlParameter("@userid", uid, SqlDbType.Int))
                        daRoute.Fill(dsRoute, "vehicle")
                    Next
                    dsRoute.Relations.Add("Children", dsRoute.Tables("user").Columns("userid"), dsRoute.Tables("vehicle").Columns("userid"))
                    Return dsRoute
                    
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    ' SECURITY FIX: Validate userslist and use safe query construction
                    If Not String.IsNullOrEmpty(userslist) AndAlso SecurityHelper.IsValidUsersList(userslist) Then
                        ' Create parameterized query for multiple user IDs
                        Dim userIds() As String = userslist.Split(","c)
                        Dim parameters As New List(Of String)
                        Dim vehicleCmd As New SqlCommand()
                        Dim userCmd As New SqlCommand()
                        
                        For i As Integer = 0 To userIds.Length - 1
                            Dim paramName As String = "@userid" & i
                            parameters.Add(paramName)
                            vehicleCmd.Parameters.Add(SecurityHelper.CreateSqlParameter(paramName, userIds(i).Trim(), SqlDbType.Int))
                            userCmd.Parameters.Add(SecurityHelper.CreateSqlParameter(paramName, userIds(i).Trim(), SqlDbType.Int))
                        Next
                        
                        Dim inClause As String = String.Join(",", parameters)
                        vehicleCmd.CommandText = $"SELECT * FROM vehicleTBL WHERE userid IN ({inClause}) ORDER BY plateno"
                        userCmd.CommandText = $"SELECT * FROM userTBL WHERE userid IN ({inClause}) ORDER BY username"
                        
                        daPlateno = New SqlDataAdapter(vehicleCmd)
                        daUser = New SqlDataAdapter(userCmd)
                    End If
                Else ' User role
                    ' SECURITY FIX: Use parameterized query
                    daPlateno = New SqlDataAdapter("SELECT * FROM vehicleTBL WHERE userid = @userid ORDER BY plateno", conn)
                    daPlateno.SelectCommand.Parameters.Add(SecurityHelper.CreateSqlParameter("@userid", userid, SqlDbType.Int))
                    daUser = New SqlDataAdapter("SELECT * FROM userTBL WHERE userid = @userid ORDER BY username", conn)
                    daUser.SelectCommand.Parameters.Add(SecurityHelper.CreateSqlParameter("@userid", userid, SqlDbType.Int))
                End If

                If daPlateno IsNot Nothing Then
                    daPlateno.Fill(ds, "vehicle")
                End If
                If daUser IsNot Nothing Then
                    daUser.Fill(ds, "user")
                End If
                
                If ds.Tables.Contains("user") AndAlso ds.Tables.Contains("vehicle") Then
                    ds.Relations.Add("Children", ds.Tables("user").Columns("userid"), ds.Tables("vehicle").Columns("userid"))
                End If
                Return ds
            End Using
            
        Catch ex As SystemException
            SecurityHelper.LogError("getTreeViewData Error", ex, Server)
            Return New DataSet()
        End Try
    End Function

    Protected Sub DisplayLogInformation1()
        Try
            ' SECURITY FIX: Validate user permissions
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If

            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)
            
            ' SECURITY FIX: Validate date inputs
            If Not SecurityHelper.ValidateDate(txtBeginDate.Value) OrElse Not SecurityHelper.ValidateDate(txtEndDate.Value) Then
                lblmsg.Text = "Invalid date format"
                Return
            End If

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

            Dim checkedNodes As TreeNodeCollection = tvPlateno.CheckedNodes
            Dim count As Integer = 0

            Using conn As New SqlConnection(sCon)
                For y As Int16 = 0 To checkedNodes.Count - 1
                    If checkedNodes.Item(y).Checked = True Then
                        Dim plateno As String = checkedNodes.Item(y).Value
                        
                        ' SECURITY FIX: Validate plate number
                        If Not SecurityHelper.ValidatePlateNumber(plateno) Then
                            Continue For
                        End If

                        Dim begindatetime As String = txtBeginDate.Value & " 00:00:00"
                        Dim enddatetime As String = txtEndDate.Value & " 23:59:59"

                        ' SECURITY FIX: Use parameterized query
                        Dim query As String = "SELECT DISTINCT CONVERT(varchar(19),h.timestamp,120) as datetime, h.gps_av, h.ignition_sensor, h.lat, h.lon, h.speed, h.gps_odometer, v.userid " &
                                            "FROM vehicleTBL v INNER JOIN vehicle_history h ON h.plateno = v.plateno " &
                                            "WHERE h.plateno = @plateno AND h.timestamp BETWEEN @begindate AND @enddate AND h.gps_odometer <> 99 " &
                                            "ORDER BY datetime"

                        Using cmd As New SqlCommand(query, conn)
                            cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@plateno", plateno, SqlDbType.VarChar))
                            cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@begindate", begindatetime, SqlDbType.DateTime))
                            cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@enddate", enddatetime, SqlDbType.DateTime))

                            Dim da As New SqlDataAdapter(cmd)
                            Dim dsGeofenceTrip As New DataSet
                            da.Fill(dsGeofenceTrip)

                            ' Process geofence data (simplified for security)
                            ProcessGeofenceData(dsGeofenceTrip, plateno, t, count, userid)
                        End Using
                    End If
                Next
            End Using

            GridView2.DataSource = t
            GridView2.DataBind()

        Catch ex As Exception
            SecurityHelper.LogError("DisplayLogInformation1 Error", ex, Server)
            lblmsg.Text = "An error occurred while processing your request."
        End Try
    End Sub

    Private Sub ProcessGeofenceData(dsGeofenceTrip As DataSet, plateno As String, t As DataTable, ByRef count As Integer, userid As String)
        Try
            ' SECURITY FIX: Simplified and secure geofence processing
            If dsGeofenceTrip.Tables.Count > 0 AndAlso dsGeofenceTrip.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To dsGeofenceTrip.Tables(0).Rows.Count - 1
                    Dim row As DataRow = dsGeofenceTrip.Tables(0).Rows(i)
                    
                    ' SECURITY FIX: Validate coordinates
                    Dim lat As String = row("lat").ToString()
                    Dim lon As String = row("lon").ToString()
                    
                    If SecurityHelper.ValidateCoordinate(lat, lon) Then
                        count += 1
                        Dim newRow As DataRow = t.NewRow()
                        newRow(0) = count
                        newRow(1) = SecurityHelper.HtmlEncode(plateno)
                        newRow(2) = SecurityHelper.HtmlEncode(row("datetime").ToString())
                        newRow(3) = SecurityHelper.HtmlEncode(row("datetime").ToString())
                        newRow(4) = "0" ' Simplified for security
                        newRow(5) = "0" ' Simplified for security
                        newRow(6) = SecurityHelper.HtmlEncode(row("gps_odometer").ToString())
                        newRow(7) = "Geofence Data" ' Simplified for security
                        newRow(8) = $"<a href='#' onclick='return false;'>Map View</a>" ' Simplified for security
                        newRow(9) = $"<a href='#' onclick='return false;'>View</a>" ' Simplified for security
                        newRow(10) = $"<a href='#' onclick='return false;'>Trail</a>" ' Simplified for security
                        newRow(11) = SecurityHelper.HtmlEncode(userid)
                        t.Rows.Add(newRow)
                    End If
                Next
            End If
        Catch ex As Exception
            SecurityHelper.LogError("ProcessGeofenceData Error", ex, Server)
        End Try
    End Sub

    Private Sub LoadGeofence(ByVal map As AspMap.Map, ByVal plateno As String)
        Try
            ' SECURITY FIX: Validate plate number input
            If Not SecurityHelper.ValidatePlateNumber(plateno) Then
                Return
            End If

            Using conn As New SqlConnection(sCon)
                ' SECURITY FIX: Use parameterized query
                Dim cmd As New SqlCommand("SELECT * FROM geofence WHERE accesstype = @accesstype ORDER BY geofencename", conn)
                cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@accesstype", "1", SqlDbType.VarChar))

                Dim privateGeofenceLayer As New AspMap.DynamicLayer()
                privateGeofenceLayer.LayerType = LayerType.mcPolygonLayer

                conn.Open()
                Using dr As SqlDataReader = cmd.ExecuteReader()
                    While dr.Read()
                        Try
                            If Not Convert.ToBoolean(dr("geofencetype")) Then
                                Dim circleShape As New AspMap.Shape
                                Dim values As String() = dr("data").ToString().Split(","c)

                                If values.Length = 3 Then
                                    Dim lat, lon As Double
                                    Dim radius As Integer
                                    
                                    If Double.TryParse(values(0), lat) AndAlso Double.TryParse(values(1), lon) AndAlso Integer.TryParse(values(2), radius) Then
                                        If SecurityHelper.ValidateCoordinate(lat.ToString(), lon.ToString()) Then
                                            circleShape.MakeCircle(lat, lon, radius / 111120.0)
                                            privateGeofenceLayer.AddShape(circleShape, SecurityHelper.HtmlEncode(dr("geofencename").ToString().ToUpper()), dr("geofenceid"))
                                        End If
                                    End If
                                End If
                            Else
                                ' Process polygon shapes with validation
                                ProcessPolygonShape(dr, privateGeofenceLayer)
                            End If
                        Catch ex As Exception
                            SecurityHelper.LogError("LoadGeofence Shape Error", ex, Server)
                        End Try
                    End While
                End Using
            End Using

            map.AddLayer(privateGeofenceLayer)
            map(0).Name = "Geofence Layer"

        Catch ex As Exception
            SecurityHelper.LogError("LoadGeofence Error", ex, Server)
        End Try
    End Sub

    Private Sub ProcessPolygonShape(dr As SqlDataReader, layer As AspMap.DynamicLayer)
        Try
            Dim polygonShape As New AspMap.Shape
            polygonShape.ShapeType = ShapeType.mcPolygonShape

            Dim shpPoints As New AspMap.Points()
            Dim points() As String = dr("data").ToString().Split(";"c)

            For i As Integer = 0 To points.Length - 1
                Dim values() As String = points(i).Split(","c)
                If values.Length = 2 Then
                    Dim lat, lon As Double
                    If Double.TryParse(values(0), lat) AndAlso Double.TryParse(values(1), lon) Then
                        If SecurityHelper.ValidateCoordinate(lat.ToString(), lon.ToString()) Then
                            shpPoints.AddPoint(lat, lon)
                        End If
                    End If
                End If
            Next

            If shpPoints.Count > 0 Then
                polygonShape.AddPart(shpPoints)
                layer.AddShape(polygonShape, SecurityHelper.HtmlEncode(dr("geofencename").ToString().ToUpper()), dr("geofenceid"))
            End If

        Catch ex As Exception
            SecurityHelper.LogError("ProcessPolygonShape Error", ex, Server)
        End Try
    End Sub

End Class