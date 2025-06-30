Imports AspMap
Imports System.Data
Imports System.Data.SqlClient

Partial Class TrailerReport
    Inherits System.Web.UI.Page
    Public ec As String = "false"
    Public show As Boolean = False
    Public sb1 As New StringBuilder()
    
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            ' SECURITY FIX: Enable authentication check
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If

            LoadUserData()

        Catch ex As Exception
            SecurityHelper.LogError("TrailerReport OnInit Error", ex, Server)
            Response.Redirect("Error.aspx")
        Finally
            MyBase.OnInit(e)
        End Try
    End Sub

    Private Sub LoadUserData()
        Try
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)

            Dim parameters As New Dictionary(Of String, Object)
            Dim userQuery As String = ""
            Dim vehicleQuery As String = ""

            ' Load users dropdown
            If role = "User" Then
                userQuery = "SELECT userid, username FROM userTBL WHERE userid = @userid ORDER BY username"
                parameters.Add("@userid", userid)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                DropDownList1.Items.Add(New ListItem("--Select User Name--", "--Select User Name--"))
                If SecurityHelper.IsValidUsersList(userslist) Then
                    Dim userIds() As String = userslist.Split(","c)
                    Dim paramNames As New List(Of String)
                    
                    For i As Integer = 0 To userIds.Length - 1
                        Dim paramName As String = "@userid" & i
                        paramNames.Add(paramName)
                        parameters.Add(paramName, userIds(i).Trim())
                    Next
                    
                    userQuery = $"SELECT userid, username FROM userTBL WHERE role='User' AND userid IN ({String.Join(",", paramNames)}) ORDER BY username"
                End If
            Else
                DropDownList1.Items.Add(New ListItem("--Select User Name--", "--Select User Name--"))
                userQuery = "SELECT userid, username FROM userTBL WHERE role='User' ORDER BY username"
            End If

            If Not String.IsNullOrEmpty(userQuery) Then
                Dim userData As DataTable = SecurityHelper.ExecuteSecureQuery(userQuery, parameters)
                
                For Each row As DataRow In userData.Rows
                    DropDownList1.Items.Add(New ListItem(SecurityHelper.HtmlEncode(row("username").ToString()), row("userid").ToString()))
                Next
            End If

            ' Load vehicles dropdown
            ddlplate.Items.Add(New ListItem("--Select Plate No--", "--Select Plate No--"))
            
            parameters.Clear()
            If role = "User" Then
                vehicleQuery = "SELECT plateno FROM vehicleTBL WHERE userid = @userid ORDER BY plateno"
                parameters.Add("@userid", userid)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                If SecurityHelper.IsValidUsersList(userslist) Then
                    Dim userIds() As String = userslist.Split(","c)
                    Dim paramNames As New List(Of String)
                    
                    For i As Integer = 0 To userIds.Length - 1
                        Dim paramName As String = "@userid" & i
                        paramNames.Add(paramName)
                        parameters.Add(paramName, userIds(i).Trim())
                    Next
                    
                    vehicleQuery = $"SELECT plateno FROM vehicleTBL WHERE userid IN ({String.Join(",", paramNames)}) ORDER BY plateno"
                End If
            Else
                vehicleQuery = "SELECT plateno FROM vehicleTBL ORDER BY plateno"
            End If

            If Not String.IsNullOrEmpty(vehicleQuery) Then
                Dim vehicleData As DataTable = SecurityHelper.ExecuteSecureQuery(vehicleQuery, parameters)
                
                For Each row As DataRow In vehicleData.Rows
                    ddlplate.Items.Add(New ListItem(SecurityHelper.HtmlEncode(row("plateno").ToString()), row("plateno").ToString()))
                Next
            End If

        Catch ex As Exception
            SecurityHelper.LogError("LoadUserData Error", ex, Server)
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If

            ImageButton1.Attributes.Add("onclick", "return mysubmit()")
            If Page.IsPostBack = False Then
                txtBeginDate.Value = Now().ToString("yyyy/MM/dd")
                txtEndDate.Value = Now().ToString("yyyy/MM/dd")
            Else
                LoadVehiclesForSelectedUser()
            End If

        Catch ex As Exception
            SecurityHelper.LogError("TrailerReport Page_Load Error", ex, Server)
            Response.Redirect("Error.aspx")
        End Try
    End Sub

    Private Sub LoadVehiclesForSelectedUser()
        Try
            Dim selectedUserId As String = Request.Form("DropDownList1")
            If String.IsNullOrEmpty(selectedUserId) OrElse selectedUserId = "--Select User Name--" Then
                Return
            End If

            ' SECURITY FIX: Validate user ID
            If Not SecurityHelper.ValidateUserId(selectedUserId) Then
                Return
            End If

            ddlplate.Items.Clear()
            ddlplate.Items.Add(New ListItem("--Select Plate No--", "--Select Plate No--"))

            Dim parameters As New Dictionary(Of String, Object) From {
                {"@userid", selectedUserId}
            }
            
            Dim query As String = "SELECT plateno FROM vehicleTBL WHERE userid = @userid ORDER BY plateno"
            Dim vehicleData As DataTable = SecurityHelper.ExecuteSecureQuery(query, parameters)
            
            For Each row As DataRow In vehicleData.Rows
                ddlplate.Items.Add(New ListItem(SecurityHelper.HtmlEncode(row("plateno").ToString()), row("plateno").ToString()))
            Next

        Catch ex As Exception
            SecurityHelper.LogError("LoadVehiclesForSelectedUser Error", ex, Server)
        End Try
    End Sub

    Protected Sub DisplayLogInformation()
        Try
            ' SECURITY FIX: Validate input parameters
            If Not SecurityHelper.ValidateDate(txtBeginDate.Value) OrElse Not SecurityHelper.ValidateDate(txtEndDate.Value) Then
                Return
            End If

            Dim begindatetime As String = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
            Dim enddatetime As String = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"
            Dim userid As String = DropDownList1.SelectedValue
            Dim plateno As String = ddlplate.SelectedValue

            ' SECURITY FIX: Validate plate number
            If Not SecurityHelper.ValidatePlateNumber(plateno) Then
                Return
            End If

            Dim t As New DataTable
            t.Columns.Add(New DataColumn("S No"))
            t.Columns.Add(New DataColumn("Plate No"))
            t.Columns.Add(New DataColumn("Date Time"))
            t.Columns.Add(New DataColumn("Trailer"))
            t.Columns.Add(New DataColumn("Duration (min)"))
            t.Columns.Add(New DataColumn("Trailer ID"))
            t.Columns.Add(New DataColumn("Trailer No"))
            t.Columns.Add(New DataColumn("Address"))

            Dim parameters As New Dictionary(Of String, Object) From {
                {"@plateno", plateno},
                {"@begindate", begindatetime},
                {"@enddate", enddatetime}
            }

            Dim query As String = "SELECT timestamp, h.plateno, h.trailer, h.trailerid, t.trailerno, h.lat, h.lon FROM vehicle_history2 h LEFT JOIN trailer2 t ON h.trailerid = t.trailerid WHERE h.plateno = @plateno AND timestamp BETWEEN @begindate AND @enddate ORDER BY timestamp"
            
            Dim trailerData As DataTable = SecurityHelper.ExecuteSecureQuery(query, parameters)
            
            Dim i As Int64 = 1
            Dim prevstatus As String = ""
            Dim prevtimestamp, currenttimestamp As String
            Dim currentstatus As String = ""
            Dim locObj As New Location(userid)

            For Each row As DataRow In trailerData.Rows
                Try
                    currentstatus = If(CBool(row("Trailer")), "Attach", "Detach")
                    currenttimestamp = Convert.ToDateTime(row("timestamp")).ToString("yyyy/MM/dd HH:mm:ss")

                    If prevstatus <> currentstatus Then
                        Dim r As DataRow = t.NewRow
                        r(0) = i.ToString()
                        r(1) = SecurityHelper.HtmlEncode(row("plateno").ToString())
                        r(2) = SecurityHelper.HtmlEncode(currenttimestamp)
                        r(3) = SecurityHelper.HtmlEncode(currentstatus)
                        
                        ' Calculate duration for detach events
                        If currentstatus = "Detach" AndAlso Not String.IsNullOrEmpty(prevtimestamp) Then
                            Try
                                r(4) = (Convert.ToDateTime(currenttimestamp) - Convert.ToDateTime(prevtimestamp)).TotalMinutes.ToString("0")
                            Catch
                                r(4) = "N/A"
                            End Try
                        Else
                            r(4) = "N/A"
                        End If

                        r(5) = SecurityHelper.HtmlEncode(If(IsDBNull(row("trailerid")), "", row("trailerid").ToString()))
                        r(6) = SecurityHelper.HtmlEncode(If(IsDBNull(row("trailerno")), "", row("trailerno").ToString()))
                        
                        ' Get location safely
                        Try
                            Dim lat As Double = CDbl(row("lat"))
                            Dim lon As Double = CDbl(row("lon"))
                            If SecurityHelper.ValidateCoordinate(lat.ToString(), lon.ToString()) Then
                                r(7) = SecurityHelper.HtmlEncode(locObj.GetLocation(lat, lon))
                            Else
                                r(7) = "Invalid coordinates"
                            End If
                        Catch
                            r(7) = "Location unavailable"
                        End Try

                        prevstatus = currentstatus
                        If prevstatus = "Attach" Then
                            prevtimestamp = currenttimestamp
                        End If

                        t.Rows.Add(r)
                        i += 1
                    End If
                Catch ex As Exception
                    SecurityHelper.LogError("DisplayLogInformation Row Processing Error", ex, Server)
                End Try
            Next

            If t.Rows.Count = 0 Then
                Dim r As DataRow = t.NewRow
                For j As Integer = 0 To 7
                    r(j) = "--"
                Next
                t.Rows.Add(r)
            End If

            Session.Remove("exceltable")
            Session("exceltable") = t
            GenerateHtmlTable(t)

        Catch ex As Exception
            SecurityHelper.LogError("DisplayLogInformation Error", ex, Server)
        End Try
    End Sub

    Private Sub GenerateHtmlTable(t As DataTable)
        Try
            If t.Rows.Count > 0 Then
                ec = "true"
                sb1.Length = 0
                sb1.Append("<table cellpadding=""0"" cellspacing=""0"" border=""0"" class=""display"" id=""examples"" style=""font-size: 10px;font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;"">")
                sb1.Append("<thead><tr><th>S No</th><th>Plate NO</th><th>Date Time</th><th>Trailer</th><th>Duration (mins)</th><th>Trailer ID</th><th>Trailer No</th><th>Location</th></tr></thead>")
                sb1.Append("<tbody>")

                For k As Integer = 0 To t.Rows.Count - 1
                    sb1.Append("<tr>")
                    For col As Integer = 0 To 7
                        sb1.Append("<td>")
                        sb1.Append(SecurityHelper.HtmlEncode(t.DefaultView.Item(k)(col).ToString()))
                        sb1.Append("</td>")
                    Next
                    sb1.Append("</tr>")
                Next

                sb1.Append("</tbody>")
                sb1.Append("<tfoot><tr><th>S No</th><th>Plate NO</th><th>Date Time</th><th>Trailer</th><th>Duration (mins)</th><th>Trailer ID</th><th>Trailer No</th><th>Location</th></tr></tfoot>")
                sb1.Append("</table>")
            End If
        Catch ex As Exception
            SecurityHelper.LogError("GenerateHtmlTable Error", ex, Server)
        End Try
    End Sub

    Protected Sub ImageButton1_Click(sender As Object, e As System.EventArgs) Handles ImageButton1.Click
        DisplayLogInformation()
    End Sub
End Class