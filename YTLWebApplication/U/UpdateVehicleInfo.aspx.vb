Imports System.Data.SqlClient
Imports System.Collections.Generic
Partial Class UpdateVehicleInfo
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim res As Integer = 0
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim cmd As New SqlCommand("update  vehicleTBL set groupid='" & Request.QueryString("groupname") & "',type='" & Request.QueryString("type") & "',brand='" & Request.QueryString("brand") & "',model='" & Request.QueryString("model") & "',speedlimit='" & Request.QueryString("speedlimit") & "',drivermobile='" & Request.QueryString("drivermobile") & "',vehicleodometer='" & Request.QueryString("odometer") & "',VehicleOdoRecDate='" & Request.QueryString("recdate") & "',pmid='" & Request.QueryString("pmid") & "',baseplant='" & Request.QueryString("baseplant") & "',companyid='" & Request.QueryString("permit") & "' where plateno='" & Request.QueryString("plateno") & "'", conn)

        Try
            conn.Open()
            res = cmd.ExecuteNonQuery()
            If res > 0 Then
                UpdateData(Request.QueryString("plateno"))
            End If
        Catch ex As Exception

        Finally
            conn.Close()
        End Try
    End Sub

    Private Sub UpdateData(p1 As String)
        Server.ScriptTimeout = 600000

        Try
            Dim dr, drm, drm1 As SqlDataReader
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As New SqlCommand("select plateno from vehicleTBL where plateno='" & p1 & "'", conn)
            Dim vehicleDict As New Dictionary(Of String, String)
            Dim VehicleMOdoDict As New Dictionary(Of String, Integer)
            Try
                conn.Open()
                dr = cmd.ExecuteReader()
                While dr.Read()
                    Try
                        vehicleDict.Add(dr("plateno"), dr("plateno"))
                    Catch ex As Exception

                    End Try
                End While

            Catch ex As Exception
                WriteLog("Vehicle Table" & ex.Message)
            Finally
                conn.Close()
            End Try

            Dim dt As New DataTable
            Dim modo As New Odometer
            Dim vehicleodometer, actodometer, maintainenceodometer As Double
            Dim lastodometer As Double = 0

            For Each key As String In vehicleDict.Keys
                Try
                    Dim cmdm As SqlCommand = New SqlCommand("select av.plateno,isnull(ao.absolute,0) absolute ,isnull(ao.milage,0) milage,av.VehicleOdoRecDate,av.vehicleodometer from vehicle_odometer ao right outer join vehicleTBL av on convert(varchar,ao.timestamp,106)=convert(varchar,dateadd(dd,1,av.VehicleOdoRecDate),106) and ao.[plateno]=av.plateno   where av.plateno ='" & key & "'", conn2)
                    Dim cmdm1 As SqlCommand = New SqlCommand("select top 1 absolute,afterodometer from vehicle_odometer where plateno = '" & key & "'  order by timestamp desc ", conn2)

                    conn2.Open()
                    drm = cmdm.ExecuteReader()
                    drm1 = cmdm1.ExecuteReader()

                    If drm.Read() Then
                        vehicleodometer = drm("vehicleodometer")
                        actodometer = drm("absolute") - drm("milage")
                        If drm1.Read() Then
                            dt = modo.ProcessData(key, Convert.ToDateTime(drm("VehicleOdoRecDate")).ToString("yyyy/MM/dd HH:mm:ss"), Convert.ToDateTime(drm("VehicleOdoRecDate")).ToString("yyyy/MM/dd") & " 23:59:59")
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
                    If maintainenceodometer = -1 Then
                        maintainenceodometer = 0
                    Else
                        maintainenceodometer = Convert.ToInt32(maintainenceodometer)
                    End If
                    drm.Close()
                    drm1.Close()
                    VehicleMOdoDict.Add(key, maintainenceodometer)
                Catch ex As Exception
                    WriteLog("Maintainence Odometer " & ex.Message)
                Finally
                    conn2.Close()
                End Try
            Next

            For Each plateno As String In VehicleMOdoDict.Keys
                Try
                    cmd = New SqlCommand("update vehicleTBL set modo='" & VehicleMOdoDict.Item(plateno).ToString() & "' where plateno='" & plateno & "'", conn)
                    If conn.State = ConnectionState.Closed Then
                        conn.Open()
                    End If
                    Dim result As Integer = cmd.ExecuteNonQuery()
                    If result > 0 Then
                        WriteLog("Updated MOdo: " & VehicleMOdoDict.Item(plateno).ToString() & " for the Plateno : " & plateno)
                    End If

                    ' WriteLog(cmd.CommandText)
                Catch ex As Exception
                    WriteLog("During update " & ex.Message)
                Finally
                    conn.Close()
                    cmd.Clone()
                End Try
            Next
        Catch ex As Exception
            WriteLog("Main " & ex.Message)
        End Try
    End Sub

    Private Sub WriteLog(p1 As String)
        ' Response.Write(p1)
    End Sub
End Class
