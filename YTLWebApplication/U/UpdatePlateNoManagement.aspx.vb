Imports System.Data
Imports System.Data.SqlClient

Partial Class UpdatePlateNoManagement
    Inherits System.Web.UI.Page

    Public errormessage As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("../Login.aspx")
                Return
            End If

            If Page.IsPostBack = False Then
                plateno.Attributes.Add("onload", "setHide()")
                ibSubmit.Attributes.Add("onclick", "return mysubmit()")

                ' SECURITY FIX: Get validated user information
                Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
                Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
                Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)

                Using conn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
                    Dim query As String
                    Dim cmd As SqlCommand

                    If role = "User" Then
                        query = "SELECT userid, username, dbip FROM userTBL WHERE userid = @userid"
                        cmd = New SqlCommand(query, conn)
                        cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@userid", userid, SqlDbType.Int))
                    ElseIf role = "SuperUser" Or role = "Operator" Then
                        If SecurityHelper.IsValidUsersList(userslist) Then
                            ' Create parameterized query for multiple user IDs
                            Dim userIds() As String = userslist.Replace("'", "").Split(","c)
                            Dim parameters As New List(Of String)
                            cmd = New SqlCommand()
                            
                            For i As Integer = 0 To userIds.Length - 1
                                Dim paramName As String = "@userid" & i
                                parameters.Add(paramName)
                                cmd.Parameters.Add(SecurityHelper.CreateSqlParameter(paramName, userIds(i).Trim(), SqlDbType.Int))
                            Next
                            
                            Dim inClause As String = String.Join(",", parameters)
                            query = $"SELECT userid, username, dbip FROM userTBL WHERE userid IN ({inClause}) ORDER BY username"
                            cmd.CommandText = query
                            cmd.Connection = conn
                        Else
                            ' Fallback to single user
                            query = "SELECT userid, username, dbip FROM userTBL WHERE userid = @userid"
                            cmd = New SqlCommand(query, conn)
                            cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@userid", userid, SqlDbType.Int))
                        End If
                    Else
                        query = "SELECT userid, username, dbip FROM userTBL WHERE role = @role ORDER BY username"
                        cmd = New SqlCommand(query, conn)
                        cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@role", "User", SqlDbType.VarChar))
                    End If

                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        While dr.Read()
                            ddlUsername.Items.Add(New ListItem(SecurityHelper.HtmlEncode(dr("username").ToString()), dr("userid").ToString()))
                        End While
                    End Using
                End Using
            End If

        Catch ex As Exception
            SecurityHelper.LogError("UpdatePlateNoManagement Page_Load Error", ex, Server)
            Response.Redirect("../Error.aspx")
        End Try

    End Sub

    Protected Sub InsertUpdateTmpTable(ByVal userid As String, ByVal newPlateNo As String, ByVal oldPlateNo As String)
        Try
            ' SECURITY FIX: Validate inputs
            If Not SecurityHelper.ValidateUserId(userid) OrElse 
               Not SecurityHelper.ValidatePlateNumber(newPlateNo) OrElse 
               Not SecurityHelper.ValidatePlateNumber(oldPlateNo) Then
                Return
            End If

            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
                Dim query As String = "INSERT INTO plateno_upd_tmp(oldplateno, newplateno, insertdatetime) VALUES (@oldplateno, @newplateno, @insertdatetime)"
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@oldplateno", oldPlateNo, SqlDbType.VarChar))
                    cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@newplateno", newPlateNo, SqlDbType.VarChar))
                    cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@insertdatetime", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), SqlDbType.DateTime))

                    If oldPlateNo <> "--Select Plate No--" Then
                        conn.Open()
                        cmd.ExecuteNonQuery()
                        SecurityHelper.LogSecurityEvent("PLATE_UPDATE_LOGGED", $"Plate number update logged: {oldPlateNo} -> {newPlateNo}")
                    End If
                End Using
            End Using
        Catch ex As SystemException
            SecurityHelper.LogError("InsertUpdateTmpTable Error", ex, Server)
        End Try
    End Sub

    Protected Sub updateTable(ByVal userid As String, ByVal TableName As String, ByVal newPlateNo As String, ByVal oldPlateNo As String)
        Try
            ' SECURITY FIX: Validate inputs
            If Not SecurityHelper.ValidateUserId(userid) OrElse 
               Not SecurityHelper.ValidatePlateNumber(newPlateNo) OrElse 
               Not SecurityHelper.ValidatePlateNumber(oldPlateNo) OrElse
               String.IsNullOrEmpty(TableName) Then
                Return
            End If

            ' SECURITY FIX: Validate table name against whitelist
            Dim allowedTables() As String = {
                "vehicleTBL", "vehicle_tracked", "fuel_tank_check", "fuel_tank_profile",
                "maintenance", "geofence_tracked", "geofence_trip_audit", "fuel",
                "panic_interval", "tollfare", "documents_date", "driver_assign",
                "operator_check_list", "trip_receipt", "vehicle_average_idling",
                "vehicle_servicing", "vehicle_g13e_data", "vehicle_geofence",
                "vehicle_idling_profile", "vehicle_incident", "vehicle_fuel_summ",
                "vehicle_idling_summ", "vehicle_refuel_summ", "instant_alert_settings",
                "sms_panic_dispatch_list"
            }

            If Not allowedTables.Contains(TableName) Then
                SecurityHelper.LogSecurityEvent("INVALID_TABLE_ACCESS", $"Attempt to update invalid table: {TableName}")
                Return
            End If

            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
                Dim updateQuery As String = $"UPDATE {TableName} SET plateno = @newplateno WHERE plateno = @oldplateno"
                Using updateCmd As New SqlCommand(updateQuery, conn)
                    updateCmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@newplateno", newPlateNo, SqlDbType.VarChar))
                    updateCmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@oldplateno", oldPlateNo, SqlDbType.VarChar))

                    conn.Open()
                    Dim result As Integer = updateCmd.ExecuteNonQuery()

                    If result > 0 Then
                        ' Log the modification
                        Dim logQuery As String = "INSERT INTO vehicle_plateno_modified(OldPlateNo, NewPlateNo, ModifiedDate, ModifiedTable, Username, OldUnitId, NewUnitId) VALUES (@oldplateno, @newplateno, @modifieddate, @modifiedtable, @username, @oldunitid, @newunitid)"
                        Using logCmd As New SqlCommand(logQuery, conn)
                            logCmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@oldplateno", oldPlateNo, SqlDbType.VarChar))
                            logCmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@newplateno", newPlateNo, SqlDbType.VarChar))
                            logCmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@modifieddate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), SqlDbType.DateTime))
                            logCmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@modifiedtable", TableName, SqlDbType.VarChar))
                            logCmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@username", SecurityHelper.ValidateAndGetUserId(Request), SqlDbType.VarChar))
                            logCmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@oldunitid", "-", SqlDbType.VarChar))
                            logCmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@newunitid", "-", SqlDbType.VarChar))
                            
                            logCmd.ExecuteNonQuery()
                        End Using

                        SecurityHelper.LogSecurityEvent("PLATE_NUMBER_UPDATED", $"Table {TableName}: {oldPlateNo} -> {newPlateNo}")
                    End If
                End Using
            End Using

        Catch ex As SystemException
            SecurityHelper.LogError("updateTable Error", ex, Server)
        End Try
    End Sub


    Protected Sub ibBack_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibBack.Click
        Try
            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("../Login.aspx")
                Return
            End If

            plateno.Attributes.Remove("onload")

            Dim userid As String = ddlUsername.SelectedValue
            If Not SecurityHelper.ValidateUserId(userid) Then
                Return
            End If

            Dim selectedPlate As String = ddlpleate.SelectedValue
            If Not SecurityHelper.ValidatePlateNumber(selectedPlate) Then
                Return
            End If

            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
                For i As Int64 = 0 To cblTable.Items.Count - 1
                    If cblTable.Items(i).Selected = True Then
                        Dim tableName As String = cblTable.Items(i).Value
                        
                        ' SECURITY FIX: Validate table name
                        Dim allowedTables() As String = {
                            "vehicleTBL", "vehicle_tracked", "fuel_tank_check", "fuel_tank_profile",
                            "maintenance", "geofence_tracked", "geofence_trip_audit", "fuel",
                            "panic_interval", "tollfare", "documents_date", "driver_assign",
                            "operator_check_list", "trip_receipt", "vehicle_average_idling",
                            "vehicle_servicing", "vehicle_g13e_data", "vehicle_geofence",
                            "vehicle_idling_profile", "vehicle_incident", "vehicle_fuel_summ",
                            "vehicle_idling_summ", "vehicle_refuel_summ", "instant_alert_settings",
                            "sms_panic_dispatch_list"
                        }

                        If allowedTables.Contains(tableName) Then
                            Dim query As String = $"SELECT COUNT(plateno) as counter FROM {tableName} WHERE plateno = @plateno"
                            Using cmd As New SqlCommand(query, conn)
                                cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@plateno", selectedPlate, SqlDbType.VarChar))
                                
                                If conn.State = ConnectionState.Closed Then
                                    conn.Open()
                                End If
                                
                                Dim count As Object = cmd.ExecuteScalar()
                                DisplayRecords(i, cblTable.Items(i).Selected, If(count, 0))
                            End Using
                        End If
                    Else
                        DisplayBlank(i)
                    End If
                Next

                ' Get unit ID
                Dim unitQuery As String = "SELECT unitid FROM vehicleTBL WHERE plateno = @plateno"
                Using unitCmd As New SqlCommand(unitQuery, conn)
                    unitCmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@plateno", selectedPlate, SqlDbType.VarChar))
                    
                    If conn.State = ConnectionState.Closed Then
                        conn.Open()
                    End If
                    
                    Dim unitId As Object = unitCmd.ExecuteScalar()
                    lbl6.Text = SecurityHelper.HtmlEncode(If(unitId, "").ToString())
                End Using
            End Using
            
        Catch ex As SystemException
            SecurityHelper.LogError("ibBack_Click Error", ex, Server)
        End Try

    End Sub

    Protected Sub getPlateNo(ByVal uid As String)
        Try
            ' SECURITY FIX: Validate user ID
            If Not SecurityHelper.ValidateUserId(uid) Then
                Return
            End If

            If ddlUsername.SelectedValue <> "--Select User Name--" Then
                ddlpleate.Items.Clear()
                ddlpleate.Items.Add("--Select Plate No--")

                Using conn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
                    Dim query As String = "SELECT plateno FROM vehicleTBL WHERE userid = @userid ORDER BY plateno"
                    Using cmd As New SqlCommand(query, conn)
                        cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@userid", uid, SqlDbType.Int))
                        
                        conn.Open()
                        Using dr As SqlDataReader = cmd.ExecuteReader()
                            While dr.Read()
                                ddlpleate.Items.Add(New ListItem(SecurityHelper.HtmlEncode(dr("plateno").ToString()), dr("plateno").ToString()))
                            End While
                        End Using
                    End Using
                End Using
            Else
                ddlpleate.Items.Clear()
                ddlpleate.Items.Add("--Select User Name--")
            End If
        Catch ex As Exception
            SecurityHelper.LogError("getPlateNo Error", ex, Server)
        End Try
    End Sub

    Protected Sub DisplayRecords(ByVal counter As String, ByVal tableSelected As Boolean, ByVal platenoCounter As String)
        Dim displayValue As String = If(tableSelected, SecurityHelper.HtmlEncode(platenoCounter.ToString()), "-")
        
        Select Case counter
            Case 0 : lbl1.Text = displayValue
            Case 1 : lbl2.Text = displayValue
            Case 2 : lbl3.Text = displayValue
            Case 3 : lbl4.Text = displayValue
            Case 4 : lbl5.Text = displayValue
            Case 5 : lblMaintenance.Text = displayValue
            Case 6 : lblGeofencetracked.Text = displayValue
            Case 7 : lblGeofencetripaudit.Text = displayValue
            Case 8 : lblFuel.Text = displayValue
            Case 9 : lblPanicinterval.Text = displayValue
            Case 10 : lblTollfare.Text = displayValue
            Case 11 : lblDocumentdate.Text = displayValue
            Case 12 : lblDriverassign.Text = displayValue
            Case 13 : lblOperatorchecklist.Text = displayValue
            Case 14 : lblTripreceipt.Text = displayValue
            Case 15 : lblVehicleaverageidling.Text = displayValue
            Case 16 : lblVehicleservicing.Text = displayValue
            Case 17 : lblVehicleg13edata.Text = displayValue
            Case 18 : lblVehiclegeofence.Text = displayValue
            Case 19 : lblVehicleidlingprofile.Text = displayValue
            Case 20 : lblVehicleincident.Text = displayValue
            Case 21 : lblVehiclefuelsumm.Text = displayValue
            Case 22 : lblVehicleidlingsumm.Text = displayValue
            Case 23 : lblVehiclerefuelsumm.Text = displayValue
            Case 24 : lblInstantalertsettings.Text = displayValue
            Case 25 : lblSmspanicdispatchlist.Text = displayValue
        End Select
    End Sub

    Protected Sub DisplayBlank(ByVal counter As String)
        DisplayRecords(counter, False, "")
    End Sub

    Protected Sub ddlUsername_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlUsername.SelectedIndexChanged
        getPlateNo(ddlUsername.SelectedValue)
    End Sub

    Protected Sub ibSubmit_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibSubmit.Click
        Try
            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("../Login.aspx")
                Return
            End If

            plateno.Attributes.Remove("onload")
            
            Dim userid As String = ddlUsername.SelectedValue
            Dim newPlateNo As String = txtNew.Text.Trim()
            Dim oldPlateNo As String = ddlpleate.SelectedValue

            ' SECURITY FIX: Validate inputs
            If Not SecurityHelper.ValidateUserId(userid) Then
                errormessage = "Invalid user ID"
                Return
            End If

            If Not SecurityHelper.ValidatePlateNumber(newPlateNo) Then
                errormessage = "Invalid new plate number format"
                Return
            End If

            If Not SecurityHelper.ValidatePlateNumber(oldPlateNo) Then
                errormessage = "Invalid old plate number format"
                Return
            End If

            ' SECURITY FIX: Check user permissions
            Dim currentRole As String = SecurityHelper.ValidateAndGetUserRole(Request)
            If currentRole <> "Admin" AndAlso currentRole <> "SuperUser" Then
                SecurityHelper.LogSecurityEvent("UNAUTHORIZED_PLATE_UPDATE", $"User attempted to update plate number without permission")
                errormessage = "Insufficient permissions"
                Return
            End If

            For i As Int64 = 0 To cblTable.Items.Count - 1
                If cblTable.Items(i).Selected = True Then
                    updateTable(userid, cblTable.Items(i).Value, newPlateNo, oldPlateNo)
                End If
            Next
            
            InsertUpdateTmpTable(userid, newPlateNo, oldPlateNo)
            errormessage = "Updated. Plate number refreshed."
            getPlateNo(userid)
            
        Catch ex As SystemException
            errormessage = "An error occurred during update"
            SecurityHelper.LogError("ibSubmit_Click Error", ex, Server)
        End Try
    End Sub
End Class