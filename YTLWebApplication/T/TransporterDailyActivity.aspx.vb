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
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            Dim cmd As SqlCommand

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))

            cmd = New SqlCommand("select distinct t1.transporter_id,transporter_name  from oss_transporter t1 inner join ytldb.dbo.usertbl t2 on t2.transporter_id=t1.transporter_id order by transporter_name", conn)
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            ddltransporter.Items.Clear()
            ddltransporter.Items.Add(New ListItem("Select Transporter", "0"))
            While (dr.Read())
                ddltransporter.Items.Add(New ListItem(dr("transporter_name").ToString(), Convert.ToInt32(dr("transporter_id"))))
            End While
            ddltransporter.SelectedValue = Request.Form("ddltransporter")

            conn.Close()

        Catch ex As Exception

        Finally
            MyBase.OnInit(e)
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            ImageButton1.Attributes.Add("onclick", "return mysubmit()")
            If Page.IsPostBack = False Then
                txtBeginDate.Value = Now().ToString("yyyy/MM/dd")
                txtEndDate.Value = Now().ToString("yyyy/MM/dd")
                lbltransportercount.Text = 0
            End If


        Catch ex As Exception

        End Try
    End Sub

    Protected Sub DisplayLogInformation()
        Try

            Dim begindatetime As String = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
            Dim enddatetime As String = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"
            Dim userid As String = ddltransporter.SelectedValue
            Dim tbdt, tedt As String
            Dim uid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            lbltransportercount.Text = 0

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))


            Dim r As DataRow
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
            Dim gpsplatenolist As New List(Of String)
            Dim ossplatenolist As New List(Of String)
            Dim dr3 As SqlDataReader

            Dim cmd, cmd2, cmd3 As SqlCommand
            Dim counter As Int16 = 0
            Dim platenocmd As String = ""
            Try
                conn.Open()
                cmd = New SqlCommand("select plateno from vehicletbl where transporter_id='" & ddltransporter.SelectedValue & "'", conn)
                Dim dr As SqlDataReader = cmd.ExecuteReader()
                While dr.Read()
                    gpsplatenolist.Add(dr("plateno").ToString().ToUpper())
                    If platenocmd = "" Then
                        platenocmd = " and plateno in ('" & dr("plateno") & "'"
                    Else
                        platenocmd = platenocmd & ",'" & dr("plateno") & "'"
                    End If
                    counter += 1
                End While
                If Not platenocmd = "" Then
                    platenocmd = platenocmd & ")"
                End If
                lbltransportercount.Text = counter
                dr.Close()
                cmd.CommandText = "select t2.geofencename,count(*) as trips,t2.geofenceid from public_geofence_history t1 left outer join geofence t2 on t1.id=t2.geofenceid where id in (select geofenceid from geofence where Gtype=13 or GType=6) and intimestamp between '" & begindatetime & "' and '" & enddatetime & "'  " & platenocmd & "  group by t2.geofencename,t2.geofenceid"
                dr = cmd.ExecuteReader()
                sb2.Append("<table cellpadding=""0"" cellspacing=""0"" border=""0"" class=""display"" id=""examples1"" style=""font-size: 10px;font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;"">")
                sb2.Append("<thead><tr align=""left""><th>S No</th><th>Ship To Name</th><th>Number Of Trips</th></tr></thead>")
                sb2.Append("<tbody>")
                counter = 1
                While dr.Read()
                    sb2.Append("<tr>")
                    sb2.Append("<td>")
                    sb2.Append(counter)
                    sb2.Append("</td><td>")
                    sb2.Append(dr("geofencename").ToString().ToUpper())
                    sb2.Append("</td><td>")
                    sb2.Append("<span onclick=""javascript:openpage('" & dr("geofenceid") & "','" & begindatetime & "','" & enddatetime & "','" & ddltransporter.SelectedValue & "') "" style=""cursor:pointer;text-decoration: underline; color: #000080;"" title=""View Details"">" & dr("trips") & "</span>")

                    sb2.Append("</td></tr>")
                    counter += 1
                End While
                sb2.Append("</tbody>")
                sb2.Append("<tfoot><tr align=""left""><th>S No</th><th>Ship To Name</th><th>Number Of Trips</th></tr></tfoot></table>")
                dr.Close()
                cmd.CommandText = "select * from GetInactiveVehicles('" & ddltransporter.SelectedValue & "','" & begindatetime & "','" & enddatetime & "') order by rdatetime"

                'cmd.CommandText = "select plateno,timestamp,mileage from vehicle_fuel_summary where timestamp between '" & begindatetime & "' and '" & enddatetime & "'  " & platenocmd & "  and mileage<20"
                dr = cmd.ExecuteReader()

                sb3.Append("<table cellpadding=""0"" cellspacing=""0"" border=""0"" class=""display"" id=""examples2"" style=""font-size: 10px;font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;"">")
                sb3.Append("<thead><tr align=""left""><th>S No</th><th>Plateno</th><th>Date</th><th>Mileage (KM)</th></tr></thead>")
                sb3.Append("<tbody>")
                counter = 1
                While dr.Read()
                    sb3.Append("<tr>")
                    sb3.Append("<td>")
                    sb3.Append(counter)
                    sb3.Append("</td><td>")
                    sb3.Append(dr("plateno").ToString().ToUpper())
                    sb3.Append("</td><td>")
                    sb3.Append(Convert.ToDateTime(dr("rdatetime")).ToString("yyyy/MM/dd"))
                    sb3.Append("</td><td>")
                    sb3.Append(CDbl(dr("mileage")).ToString("0.00"))
                    sb3.Append("</td></tr>")
                    counter += 1
                End While


                Try
                    conn2.Open()
                    Dim days As Integer = (Convert.ToDateTime(enddatetime) - Convert.ToDateTime(begindatetime)).TotalDays
                    Dim daycount As Integer = 0
                    cmd3 = New SqlCommand()
                    cmd3.Connection = conn2
                    While daycount < days
                        tbdt = Convert.ToDateTime(begindatetime).AddDays(daycount).ToString("yyyy/MM/dd HH:mm:ss")
                        tedt = Convert.ToDateTime(tbdt).ToString("yyyy/MM/dd 23:59:59")
                        cmd3.CommandText = "select plateno from oss_patch_out where transporter_id='" & ddltransporter.SelectedValue & "' and weight_outtime between '" & tbdt & "' and '" & tedt & "'"
                        dr3 = cmd3.ExecuteReader()
                        ossplatenolist = New List(Of String)
                        While dr3.Read()
                            ossplatenolist.Add(dr3("plateno").ToString().ToUpper())
                        End While
                        dr3.Close()
                        For Each platestr As String In gpsplatenolist
                            If Not ossplatenolist.Contains(platestr) Then
                                sb3.Append("<tr>")
                                sb3.Append("<td>")
                                sb3.Append(counter)
                                sb3.Append("</td><td>")
                                sb3.Append(platestr)
                                sb3.Append("</td><td>")
                                sb3.Append(Convert.ToDateTime(tbdt).ToString("yyyy/MM/dd"))
                                sb3.Append("</td><td>")
                                sb3.Append("No Job")
                                sb3.Append("</td></tr>")
                                counter += 1
                            End If
                        Next
                        daycount = daycount + 1
                    End While
                Catch ex As Exception
                    Response.Write(ex.Message)
                Finally
                    If conn2.State = ConnectionState.Open Then
                        conn2.Close()
                    End If
                End Try





                sb3.Append("</tbody>")
                sb3.Append("<tfoot><tr align=""left""><th>S No</th><th>Plateno</th><th>Date</th><th>Mileage (KM)</th></tr></tfoot></table>")
                dr.Close()
            Catch ex As Exception
                Response.Write(ex.Message & " : " & cmd.CommandText)
            Finally
                If conn.State = ConnectionState.Open Then
                    conn.Close()
                End If
            End Try


            Try
                conn2.Open()
                cmd2 = New SqlCommand(" Select  t2.PV_DisplayName As plant,plateno,productname,dn_no,dn_qty,t3.name,weight_outtime,ata_datetime from oss_patch_out t1 left outer join oss_plant_master t2 On t1.source_supply=t2.PV_Plant left outer join oss_ship_to_code t3 On t1.destination_siteid=t3.shiptocode where transporter_id=" & ddltransporter.SelectedValue & " and weight_outtime between '" & begindatetime & "' and '" & enddatetime & "'  " & platenocmd & "", conn2)
                Dim dr2 As SqlDataReader = cmd2.ExecuteReader()
                counter = 1
                While dr2.Read()
                    r = t.NewRow()
                    r(0) = counter
                    r(1) = dr2("plant")
                    r(2) = dr2("plateno")
                    r(3) = dr2("productname")
                    r(4) = dr2("dn_no")
                    r(5) = dr2("dn_qty")
                    r(6) = dr2("name")
                    r(7) = Convert.ToDateTime(dr2("weight_outtime")).ToString("yyyy/MM/dd HH:mm:ss")
                    If IsDBNull(dr2("ata_datetime")) Then
                        r(8) = "-"
                        r(9) = "-"
                    Else
                        r(8) = Convert.ToDateTime(dr2("ata_datetime")).ToString("yyyy/MM/dd HH:mm:ss")
                        r(9) = DateDiff(DateInterval.Minute, Convert.ToDateTime(dr2("weight_outtime")), Convert.ToDateTime(dr2("ata_datetime")))
                    End If
                    t.Rows.Add(r)
                    counter += 1
                End While
            Catch ex As Exception
                Response.Write(ex.Message & " : " & cmd.CommandText)
            Finally
                If conn2.State = ConnectionState.Open Then
                    conn2.Close()
                End If
            End Try




            If t.Rows.Count = 0 Then
                r = t.NewRow
                r(0) = "--"
                r(1) = "--"
                r(2) = "--"
                r(3) = "--"
                r(4) = "--"
                r(5) = "--"
                r(6) = "--"
                r(7) = "--"
                r(8) = "--"
                r(9) = "--"
                t.Rows.Add(r)
            End If

            Session.Remove("exceltable")
            Session("exceltable") = t
            sb1.Length = 0


            If (t.Rows.Count > 0) Then
                ec = "true"

                sb1.Append("<table cellpadding=""0"" cellspacing=""0"" border=""0"" class=""display"" id=""examples"" style=""font-size: 10px;font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;"">")

                sb1.Append("<thead><tr align=""left""><th>S No</th><th>Plant</th><th>Plate No</th><th>Product Type</th><th>DN No</th><th>DN Qty</th><th>Ship To Name</th><th>Weight Out Time</th><th>ATA</th><th>Duration</th></tr></thead>")

                sb1.Append("<tbody>")

                For i As Integer = 0 To t.Rows.Count - 1
                    Try
                        sb1.Append("<tr>")
                        sb1.Append("<td>")
                        sb1.Append(t.DefaultView.Item(i)(0))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(1))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(2))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(3))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(4))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(5))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(6))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(7))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(8))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(i)(9))
                        sb1.Append("</td></tr>")
                        counter += 1
                    Catch ex As Exception
                        Response.Write(ex.Message)
                    End Try
                Next
                sb1.Append("</tbody>")
                sb1.Append("<tfoot><tr align=""left""><th>S No</th><th>Plant</th><th>Plate No</th><th>Product Type</th><th>DN No</th><th>DN Qty</th><th>Ship To Name</th><th>Weight Out Time</th><th>ATA</th><th>Duration</th></tr></tfoot>")
            Else

            End If





        Catch ex As Exception
            Response.Write(ex.Message)
        End Try

    End Sub




    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ImageButton1.Click
        DisplayLogInformation()
    End Sub
End Class
