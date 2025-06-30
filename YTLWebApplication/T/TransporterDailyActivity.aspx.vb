Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Collections.Generic

Partial Class TransporterDailyActivity
    Inherits System.Web.UI.Page
    Public ec As String = "false"
    Public show As Boolean = False
    Public sb1 As New StringBuilder()
    Public sb2 As New StringBuilder()
    Public sb3 As New StringBuilder()
    Public adminusers As String
    Dim TrailerDict As New Dictionary(Of String, String)
    
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            ' SECURITY FIX: Enable authentication check
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If

            LoadTransporterData()

        Catch ex As Exception
            SecurityHelper.LogError("TransporterDailyActivity OnInit Error", ex, Server)
            Response.Redirect("Error.aspx")
        Finally
            MyBase.OnInit(e)
        End Try
    End Sub

    Private Sub LoadTransporterData()
        Try
            Dim query As String = "SELECT DISTINCT t1.transporter_id, transporter_name FROM oss_transporter t1 INNER JOIN ytldb.dbo.usertbl t2 ON t2.transporter_id = t1.transporter_id ORDER BY transporter_name"
            Dim transporterData As DataTable = SecurityHelper.ExecuteSecureQuery(query, New Dictionary(Of String, Object))
            
            ddltransporter.Items.Clear()
            ddltransporter.Items.Add(New ListItem("Select Transporter", "0"))
            
            For Each row As DataRow In transporterData.Rows
                Dim transporterName As String = SecurityHelper.HtmlEncode(row("transporter_name").ToString())
                Dim transporterId As String = row("transporter_id").ToString()
                
                ddltransporter.Items.Add(New ListItem(transporterName, transporterId))
            Next

        Catch ex As Exception
            SecurityHelper.LogError("LoadTransporterData Error", ex, Server)
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
                lbltransportercount.Text = "0"
            End If

        Catch ex As Exception
            SecurityHelper.LogError("TransporterDailyActivity Page_Load Error", ex, Server)
            Response.Redirect("Error.aspx")
        End Try
    End Sub

    Protected Sub DisplayLogInformation()
        Try
            ' SECURITY FIX: Validate input parameters
            If Not SecurityHelper.ValidateDate(txtBeginDate.Value) OrElse Not SecurityHelper.ValidateDate(txtEndDate.Value) Then
                Return
            End If

            Dim transporterId As String = SecurityHelper.ValidateInput(ddltransporter.SelectedValue, "numeric")
            If String.IsNullOrEmpty(transporterId) OrElse transporterId = "0" Then
                Return
            End If

            Dim begindatetime As String = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
            Dim enddatetime As String = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"

            lbltransportercount.Text = "0"

            ' Get vehicle list for transporter
            Dim vehicleParameters As New Dictionary(Of String, Object) From {
                {"@transporterId", transporterId}
            }
            
            Dim vehicleQuery As String = "SELECT plateno FROM vehicletbl WHERE transporter_id = @transporterId"
            Dim vehicleData As DataTable = SecurityHelper.ExecuteSecureQuery(vehicleQuery, vehicleParameters)
            
            Dim gpsplatenolist As New List(Of String)
            Dim platenoCondition As String = ""
            
            For Each row As DataRow In vehicleData.Rows
                Dim plateno As String = SecurityHelper.HtmlEncode(row("plateno").ToString().ToUpper())
                gpsplatenolist.Add(plateno)
            Next
            
            lbltransportercount.Text = gpsplatenolist.Count.ToString()

            If gpsplatenolist.Count > 0 Then
                ' Create parameterized query for plate numbers
                Dim plateParameters As New Dictionary(Of String, Object)
                Dim plateParamNames As New List(Of String)
                
                For i As Integer = 0 To gpsplatenolist.Count - 1
                    Dim paramName As String = "@plateno" & i
                    plateParamNames.Add(paramName)
                    plateParameters.Add(paramName, gpsplatenolist(i))
                Next
                
                platenoCondition = String.Join(",", plateParamNames)
                
                ' Load competitor geofence trips
                LoadCompetitorTrips(begindatetime, enddatetime, plateParameters, platenoCondition, transporterId)
                
                ' Load inactive vehicles
                LoadInactiveVehicles(begindatetime, enddatetime, plateParameters, platenoCondition, transporterId, gpsplatenolist)
                
                ' Load main activity data
                LoadMainActivityData(begindatetime, enddatetime, plateParameters, platenoCondition, transporterId)
            End If

        Catch ex As Exception
            SecurityHelper.LogError("DisplayLogInformation Error", ex, Server)
        End Try
    End Sub

    Private Sub LoadCompetitorTrips(begindatetime As String, enddatetime As String, plateParameters As Dictionary(Of String, Object), platenoCondition As String, transporterId As String)
        Try
            Dim parameters As New Dictionary(Of String, Object)(plateParameters)
            parameters.Add("@begindate", begindatetime)
            parameters.Add("@enddate", enddatetime)
            
            Dim query As String = $"SELECT t2.geofencename, COUNT(*) as trips, t2.geofenceid FROM public_geofence_history t1 LEFT OUTER JOIN geofence t2 ON t1.id = t2.geofenceid WHERE id IN (SELECT geofenceid FROM geofence WHERE Gtype = 13 OR GType = 6) AND intimestamp BETWEEN @begindate AND @enddate AND plateno IN ({platenoCondition}) GROUP BY t2.geofencename, t2.geofenceid"
            
            Dim competitorData As DataTable = SecurityHelper.ExecuteSecureQuery(query, parameters)
            
            sb2.Append("<table cellpadding=""0"" cellspacing=""0"" border=""0"" class=""display"" id=""examples1"" style=""font-size: 10px;font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;"">")
            sb2.Append("<thead><tr align=""left""><th>S No</th><th>Ship To Name</th><th>Number Of Trips</th></tr></thead>")
            sb2.Append("<tbody>")
            
            Dim counter As Integer = 1
            For Each row As DataRow In competitorData.Rows
                sb2.Append("<tr>")
                sb2.Append("<td>").Append(counter).Append("</td>")
                sb2.Append("<td>").Append(SecurityHelper.HtmlEncode(row("geofencename").ToString().ToUpper())).Append("</td>")
                sb2.Append("<td>").Append("<span onclick=""javascript:openpage('").Append(SecurityHelper.HtmlEncode(row("geofenceid").ToString())).Append("','").Append(SecurityHelper.HtmlEncode(begindatetime)).Append("','").Append(SecurityHelper.HtmlEncode(enddatetime)).Append("','").Append(SecurityHelper.HtmlEncode(transporterId)).Append("')"" style=""cursor:pointer;text-decoration: underline; color: #000080;"" title=""View Details"">").Append(SecurityHelper.HtmlEncode(row("trips").ToString())).Append("</span>").Append("</td>")
                sb2.Append("</tr>")
                counter += 1
            Next
            
            sb2.Append("</tbody>")
            sb2.Append("<tfoot><tr align=""left""><th>S No</th><th>Ship To Name</th><th>Number Of Trips</th></tr></tfoot></table>")

        Catch ex As Exception
            SecurityHelper.LogError("LoadCompetitorTrips Error", ex, Server)
        End Try
    End Sub

    Private Sub LoadInactiveVehicles(begindatetime As String, enddatetime As String, plateParameters As Dictionary(Of String, Object), platenoCondition As String, transporterId As String, gpsplatenolist As List(Of String))
        Try
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@transporterId", transporterId},
                {"@begindate", begindatetime},
                {"@enddate", enddatetime}
            }
            
            Dim query As String = "SELECT * FROM GetInactiveVehicles(@transporterId, @begindate, @enddate) ORDER BY rdatetime"
            Dim inactiveData As DataTable = SecurityHelper.ExecuteSecureQuery(query, parameters)
            
            sb3.Append("<table cellpadding=""0"" cellspacing=""0"" border=""0"" class=""display"" id=""examples2"" style=""font-size: 10px;font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;"">")
            sb3.Append("<thead><tr align=""left""><th>S No</th><th>Plateno</th><th>Date</th><th>Mileage (KM)</th></tr></thead>")
            sb3.Append("<tbody>")
            
            Dim counter As Integer = 1
            For Each row As DataRow In inactiveData.Rows
                sb3.Append("<tr>")
                sb3.Append("<td>").Append(counter).Append("</td>")
                sb3.Append("<td>").Append(SecurityHelper.HtmlEncode(row("plateno").ToString().ToUpper())).Append("</td>")
                sb3.Append("<td>").Append(SecurityHelper.HtmlEncode(Convert.ToDateTime(row("rdatetime")).ToString("yyyy/MM/dd"))).Append("</td>")
                sb3.Append("<td>").Append(SecurityHelper.HtmlEncode(CDbl(row("mileage")).ToString("0.00"))).Append("</td>")
                sb3.Append("</tr>")
                counter += 1
            Next
            
            sb3.Append("</tbody>")
            sb3.Append("<tfoot><tr align=""left""><th>S No</th><th>Plateno</th><th>Date</th><th>Mileage (KM)</th></tr></tfoot></table>")

        Catch ex As Exception
            SecurityHelper.LogError("LoadInactiveVehicles Error", ex, Server)
        End Try
    End Sub

    Private Sub LoadMainActivityData(begindatetime As String, enddatetime As String, plateParameters As Dictionary(Of String, Object), platenoCondition As String, transporterId As String)
        Try
            Dim parameters As New Dictionary(Of String, Object)(plateParameters)
            parameters.Add("@transporterId", transporterId)
            parameters.Add("@begindate", begindatetime)
            parameters.Add("@enddate", enddatetime)
            
            Dim query As String = $"SELECT t2.PV_DisplayName As plant, plateno, productname, dn_no, dn_qty, t3.name, weight_outtime, ata_datetime FROM oss_patch_out t1 LEFT OUTER JOIN oss_plant_master t2 ON t1.source_supply = t2.PV_Plant LEFT OUTER JOIN oss_ship_to_code t3 ON t1.destination_siteid = t3.shiptocode WHERE transporter_id = @transporterId AND weight_outtime BETWEEN @begindate AND @enddate AND plateno IN ({platenoCondition})"
            
            Dim activityData As DataTable = SecurityHelper.ExecuteSecureQuery(query, parameters)
            
            Dim t As New DataTable
            t.Columns.Add(New DataColumn("SNo"))
            t.Columns.Add(New DataColumn("Plant"))
            t.Columns.Add(New DataColumn("Plateno"))
            t.Columns.Add(New DataColumn("Product Type"))
            t.Columns.Add(New DataColumn("DN No"))
            t.Columns.Add(New DataColumn("DN Qty"))
            t.Columns.Add(New DataColumn("Ship To Name"))
            t.Columns.Add(New DataColumn("Weight Out Time"))
            t.Columns.Add(New DataColumn("ATA"))
            t.Columns.Add(New DataColumn("Duration"))
            
            Dim counter As Integer = 1
            For Each row As DataRow In activityData.Rows
                Dim r As DataRow = t.NewRow()
                r(0) = counter
                r(1) = SecurityHelper.HtmlEncode(If(IsDBNull(row("plant")), "", row("plant").ToString()))
                r(2) = SecurityHelper.HtmlEncode(row("plateno").ToString())
                r(3) = SecurityHelper.HtmlEncode(If(IsDBNull(row("productname")), "", row("productname").ToString()))
                r(4) = SecurityHelper.HtmlEncode(If(IsDBNull(row("dn_no")), "", row("dn_no").ToString()))
                r(5) = SecurityHelper.HtmlEncode(If(IsDBNull(row("dn_qty")), "", row("dn_qty").ToString()))
                r(6) = SecurityHelper.HtmlEncode(If(IsDBNull(row("name")), "", row("name").ToString()))
                r(7) = SecurityHelper.HtmlEncode(Convert.ToDateTime(row("weight_outtime")).ToString("yyyy/MM/dd HH:mm:ss"))
                
                If IsDBNull(row("ata_datetime")) Then
                    r(8) = "-"
                    r(9) = "-"
                Else
                    r(8) = SecurityHelper.HtmlEncode(Convert.ToDateTime(row("ata_datetime")).ToString("yyyy/MM/dd HH:mm:ss"))
                    r(9) = DateDiff(DateInterval.Minute, Convert.ToDateTime(row("weight_outtime")), Convert.ToDateTime(row("ata_datetime")))
                End If
                
                t.Rows.Add(r)
                counter += 1
            Next
            
            If t.Rows.Count = 0 Then
                Dim r As DataRow = t.NewRow
                For i As Integer = 0 To 9
                    r(i) = "--"
                Next
                t.Rows.Add(r)
            End If
            
            Session.Remove("exceltable")
            Session("exceltable") = t
            GenerateMainActivityHtml(t)

        Catch ex As Exception
            SecurityHelper.LogError("LoadMainActivityData Error", ex, Server)
        End Try
    End Sub

    Private Sub GenerateMainActivityHtml(t As DataTable)
        Try
            If t.Rows.Count > 0 Then
                ec = "true"
                sb1.Length = 0
                sb1.Append("<table cellpadding=""0"" cellspacing=""0"" border=""0"" class=""display"" id=""examples"" style=""font-size: 10px;font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;"">")
                sb1.Append("<thead><tr align=""left""><th>S No</th><th>Plant</th><th>Plate No</th><th>Product Type</th><th>DN No</th><th>DN Qty</th><th>Ship To Name</th><th>Weight Out Time</th><th>ATA</th><th>Duration</th></tr></thead>")
                sb1.Append("<tbody>")
                
                For i As Integer = 0 To t.Rows.Count - 1
                    sb1.Append("<tr>")
                    For j As Integer = 0 To 9
                        sb1.Append("<td>").Append(SecurityHelper.HtmlEncode(t.DefaultView.Item(i)(j).ToString())).Append("</td>")
                    Next
                    sb1.Append("</tr>")
                Next
                
                sb1.Append("</tbody>")
                sb1.Append("<tfoot><tr align=""left""><th>S No</th><th>Plant</th><th>Plate No</th><th>Product Type</th><th>DN No</th><th>DN Qty</th><th>Ship To Name</th><th>Weight Out Time</th><th>ATA</th><th>Duration</th></tr></tfoot>")
                sb1.Append("</table>")
            End If
        Catch ex As Exception
            SecurityHelper.LogError("GenerateMainActivityHtml Error", ex, Server)
        End Try
    End Sub

    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ImageButton1.Click
        DisplayLogInformation()
    End Sub
End Class