Imports System.Data.SqlClient
Imports System.Collections.Generic

Partial Class UpdateVehicleInfo
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Write("Unauthorized")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Check user permissions
            Dim userRole As String = SecurityHelper.ValidateAndGetUserRole(Request)
            If userRole <> "Admin" AndAlso userRole <> "SuperUser" Then
                SecurityHelper.LogSecurityEvent("UNAUTHORIZED_VEHICLE_UPDATE", "User attempted to update vehicle info without permission")
                Response.Write("Insufficient permissions")
                Response.End()
                Return
            End If

            ' SECURITY FIX: Validate all input parameters
            Dim plateno As String = Request.QueryString("plateno")
            Dim groupname As String = Request.QueryString("groupname")
            Dim vehicleType As String = Request.QueryString("type")
            Dim brand As String = Request.QueryString("brand")
            Dim model As String = Request.QueryString("model")
            Dim speedlimit As String = Request.QueryString("speedlimit")
            Dim drivermobile As String = Request.QueryString("drivermobile")
            Dim odometer As String = Request.QueryString("odometer")
            Dim recdate As String = Request.QueryString("recdate")
            Dim pmid As String = Request.QueryString("pmid")
            Dim baseplant As String = Request.QueryString("baseplant")
            Dim permit As String = Request.QueryString("permit")

            ' Validate required fields
            If Not SecurityHelper.ValidatePlateNumber(plateno) Then
                Response.Write("Invalid plate number")
                Response.End()
                Return
            End If

            If Not String.IsNullOrEmpty(speedlimit) AndAlso Not SecurityHelper.ValidateNumeric(speedlimit, 0, 300) Then
                Response.Write("Invalid speed limit")
                Response.End()
                Return
            End If

            If Not String.IsNullOrEmpty(drivermobile) AndAlso Not SecurityHelper.ValidateInput(drivermobile, "mobile") Then
                Response.Write("Invalid mobile number")
                Response.End()
                Return
            End If

            If Not String.IsNullOrEmpty(odometer) AndAlso Not SecurityHelper.ValidateNumeric(odometer, 0, 9999999) Then
                Response.Write("Invalid odometer reading")
                Response.End()
                Return
            End If

            If Not String.IsNullOrEmpty(recdate) AndAlso Not SecurityHelper.ValidateDate(recdate) Then
                Response.Write("Invalid date")
                Response.End()
                Return
            End If

            Dim res As Integer = 0
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
                Dim query As String = "UPDATE vehicleTBL SET groupid = @groupid, type = @type, brand = @brand, model = @model, speedlimit = @speedlimit, drivermobile = @drivermobile, vehicleodometer = @vehicleodometer, VehicleOdoRecDate = @VehicleOdoRecDate, pmid = @pmid, baseplant = @baseplant, companyid = @companyid WHERE plateno = @plateno"
                
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@groupid", If(String.IsNullOrEmpty(groupname), DBNull.Value, groupname), SqlDbType.VarChar))
                    cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@type", If(String.IsNullOrEmpty(vehicleType), DBNull.Value, vehicleType), SqlDbType.VarChar))
                    cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@brand", If(String.IsNullOrEmpty(brand), DBNull.Value, brand), SqlDbType.VarChar))
                    cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@model", If(String.IsNullOrEmpty(model), DBNull.Value, model), SqlDbType.VarChar))
                    cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@speedlimit", If(String.IsNullOrEmpty(speedlimit), DBNull.Value, speedlimit), SqlDbType.Int))
                    cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@drivermobile", If(String.IsNullOrEmpty(drivermobile), DBNull.Value, drivermobile), SqlDbType.VarChar))
                    cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@vehicleodometer", If(String.IsNullOrEmpty(odometer), DBNull.Value, odometer), SqlDbType.Float))
                    cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@VehicleOdoRecDate", If(String.IsNullOrEmpty(recdate), DBNull.Value, recdate), SqlDbType.DateTime))
                    cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@pmid", If(String.IsNullOrEmpty(pmid), DBNull.Value, pmid), SqlDbType.VarChar))
                    cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@baseplant", If(String.IsNullOrEmpty(baseplant), DBNull.Value, baseplant), SqlDbType.VarChar))
                    cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@companyid", If(String.IsNullOrEmpty(permit), DBNull.Value, permit), SqlDbType.VarChar))
                    cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@plateno", plateno, SqlDbType.VarChar))

                    conn.Open()
                    res = cmd.ExecuteNonQuery()
                    
                    If res > 0 Then
                        SecurityHelper.LogSecurityEvent("VEHICLE_INFO_UPDATED", $"Vehicle info updated for plate: {plateno}")
                        UpdateData(plateno)
                    End If
                End Using
            End Using

            Response.Write(res.ToString())
            
        Catch ex As Exception
            SecurityHelper.LogError("UpdateVehicleInfo Error", ex, Server)
            Response.Write("Error occurred")
        End Try
    End Sub

    Private Sub UpdateData(p1 As String)
        Server.ScriptTimeout = 600000

        Try
            ' SECURITY FIX: Validate plate number
            If Not SecurityHelper.ValidatePlateNumber(p1) Then
                Return
            End If

            Dim vehicleDict As New Dictionary(Of String, String)
            Dim VehicleMOdoDict As New Dictionary(Of String, Integer)

            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
                Using conn2 As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
                    ' Get vehicle data
                    Dim query As String = "SELECT plateno FROM vehicleTBL WHERE plateno = @plateno"
                    Using cmd As New SqlCommand(query, conn)
                        cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@plateno", p1, SqlDbType.VarChar))
                        
                        conn.Open()
                        Using dr As SqlDataReader = cmd.ExecuteReader()
                            While dr.Read()
                                Try
                                    vehicleDict.Add(dr("plateno").ToString(), dr("plateno").ToString())
                                Catch ex As Exception
                                    ' Duplicate key, ignore
                                End Try
                            End While
                        End Using
                    End Using

                    Dim dt As New DataTable
                    Dim modo As New Odometer
                    Dim vehicleodometer, actodometer, maintainenceodometer As Double
                    Dim lastodometer As Double = 0

                    For Each key As String In vehicleDict.Keys
                        Try
                            Dim odometerQuery As String = "SELECT av.plateno, ISNULL(ao.absolute,0) absolute, ISNULL(ao.milage,0) milage, av.VehicleOdoRecDate, av.vehicleodometer FROM vehicle_odometer ao RIGHT OUTER JOIN vehicleTBL av ON CONVERT(varchar,ao.timestamp,106) = CONVERT(varchar,DATEADD(dd,1,av.VehicleOdoRecDate),106) AND ao.[plateno] = av.plateno WHERE av.plateno = @plateno"
                            Dim lastOdometerQuery As String = "SELECT TOP 1 absolute, afterodometer FROM vehicle_odometer WHERE plateno = @plateno ORDER BY timestamp DESC"

                            Using cmdm As New SqlCommand(odometerQuery, conn2)
                                cmdm.Parameters.Add(SecurityHelper.CreateSqlParameter("@plateno", key, SqlDbType.VarChar))
                                
                                Using cmdm1 As New SqlCommand(lastOdometerQuery, conn2)
                                    cmdm1.Parameters.Add(SecurityHelper.CreateSqlParameter("@plateno", key, SqlDbType.VarChar))

                                    conn2.Open()
                                    Using drm As SqlDataReader = cmdm.ExecuteReader()
                                        Using drm1 As SqlDataReader = cmdm1.ExecuteReader()
                                            If drm.Read() Then
                                                vehicleodometer = If(IsDBNull(drm("vehicleodometer")), 0, Convert.ToDouble(drm("vehicleodometer")))
                                                actodometer = Convert.ToDouble(drm("absolute")) - Convert.ToDouble(drm("milage"))
                                                
                                                If drm1.Read() Then
                                                    Dim recDate As DateTime = Convert.ToDateTime(drm("VehicleOdoRecDate"))
                                                    dt = modo.ProcessData(key, recDate.ToString("yyyy/MM/dd HH:mm:ss"), recDate.ToString("yyyy/MM/dd") & " 23:59:59")
                                                    
                                                    If dt.Rows.Count > 0 Then
                                                        If actodometer = 0 Then
                                                            maintainenceodometer = Convert.ToDouble(dt.Rows(0)("net")) + vehicleodometer
                                                        Else
                                                            maintainenceodometer = Convert.ToDouble(dt.Rows(0)("net")) + vehicleodometer + Convert.ToDouble(drm1("absolute")) - actodometer
                                                        End If
                                                    Else
                                                        maintainenceodometer = vehicleodometer + Convert.ToDouble(drm1("absolute")) - actodometer
                                                    End If

                                                    lastodometer = Convert.ToDouble(drm1("afterodometer"))
                                                Else
                                                    maintainenceodometer = vehicleodometer
                                                End If
                                            Else
                                                If drm1.Read() Then
                                                    maintainenceodometer = Convert.ToDouble(drm1("absolute"))
                                                Else
                                                    maintainenceodometer = 0
                                                End If
                                            End If
                                        End Using
                                    End Using
                                    
                                    If maintainenceodometer = -1 Then
                                        maintainenceodometer = 0
                                    Else
                                        maintainenceodometer = Convert.ToInt32(maintainenceodometer)
                                    End If
                                    
                                    VehicleMOdoDict.Add(key, CInt(maintainenceodometer))
                                End Using
                            End Using
                        Catch ex As Exception
                            WriteLog("Maintenance Odometer " & ex.Message)
                        Finally
                            If conn2.State = ConnectionState.Open Then
                                conn2.Close()
                            End If
                        End Try
                    Next

                    ' Update maintenance odometer
                    For Each plateno As String In VehicleMOdoDict.Keys
                        Try
                            Dim updateQuery As String = "UPDATE vehicleTBL SET modo = @modo WHERE plateno = @plateno"
                            Using updateCmd As New SqlCommand(updateQuery, conn)
                                updateCmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@modo", VehicleMOdoDict.Item(plateno).ToString(), SqlDbType.Int))
                                updateCmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@plateno", plateno, SqlDbType.VarChar))
                                
                                If conn.State = ConnectionState.Closed Then
                                    conn.Open()
                                End If
                                
                                Dim result As Integer = updateCmd.ExecuteNonQuery()
                                If result > 0 Then
                                    WriteLog("Updated MOdo: " & VehicleMOdoDict.Item(plateno).ToString() & " for the Plateno : " & plateno)
                                End If
                            End Using
                        Catch ex As Exception
                            WriteLog("During update " & ex.Message)
                        Finally
                            If conn.State = ConnectionState.Open Then
                                conn.Close()
                            End If
                        End Try
                    Next
                End Using
            End Using
        Catch ex As Exception
            WriteLog("Main " & ex.Message)
        End Try
    End Sub

    Private Sub WriteLog(p1 As String)
        ' SECURITY FIX: Secure logging
        Try
            SecurityHelper.LogSecurityEvent("VEHICLE_UPDATE_LOG", p1)
        Catch
            ' Fail silently
        End Try
    End Sub
End Class