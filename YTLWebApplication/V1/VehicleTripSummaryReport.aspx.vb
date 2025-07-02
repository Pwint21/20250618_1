Imports System.Data.SqlClient
Imports AspMap
Imports ADODB
Imports System.Data
Partial Class VehicleTripSummaryReport
    Inherits System.Web.UI.Page
    Public show As Boolean = False
    Public ec As String = "false"
    Dim FooterOdometer, FooterFuelConsumption, FooterFuelCost, FooterIdlingTime, FooterTripTime As Double

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            Dim cmd As SqlCommand
            Dim dr As SqlDataReader

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            cmd = New SqlCommand("select userid, username from userTBL where role='User' order by username", conn)
            If role = "User" Then
                cmd = New SqlCommand("select userid, username from userTBL where userid='" & userid & "'", conn)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select userid, username from userTBL where userid in (" & userslist & ") order by username", conn)
            End If
            conn.Open()
            dr = cmd.ExecuteReader()
            While dr.Read()
                ddlUsername.Items.Add(New ListItem(dr("username"), dr("userid")))
            End While
            dr.Close()
            If role = "User" Then
                ddlUsername.Items.Remove("--Select User Name--")
                ddlUsername.SelectedValue = userid
                getPlateNo(userid)
            End If
            conn.Close()

        Catch ex As Exception


        End Try
        MyBase.OnInit(e)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            If Page.IsPostBack = False Then
                ImageButton1.Attributes.Add("onclick", "return mysubmit()")
                txtBeginDate.Value = Now().ToString("yyyy/MM/dd")
                txtEndDate.Value = Now().ToString("yyyy/MM/dd")
            End If

            lblSuspect.Visible = False

        Catch ex As Exception
        End Try
    End Sub
    Protected Sub getPlateNo(ByVal uid As String)
        Try
            If ddlUsername.SelectedValue <> "--Select User Name--" Then
                ddlpleate.Items.Clear()
                ddlpleate.Items.Add("--Select Plate No--")
                Dim cmd As SqlCommand
                Dim dr As SqlDataReader

                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                cmd = New SqlCommand("select plateno from vehicleTBL where userid='" & uid & "' order by plateno", conn)
                conn.Open()
                dr = cmd.ExecuteReader()
                While dr.Read()
                    ddlpleate.Items.Add(New ListItem(dr("plateno"), dr("plateno")))
                End While
                dr.Close()

                conn.Close()
            Else
                ddlpleate.Items.Clear()
                ddlpleate.Items.Add("--Select User Name--")
            End If
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub

    Protected Sub DisplaySummary()
        Try
            Dim begindatetime = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
            Dim enddatetime = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"

            Dim t As New DataTable
            t.Columns.Add(New DataColumn("No"))
            t.Columns.Add(New DataColumn("Begin Date Time"))
            t.Columns.Add(New DataColumn("End Date Time"))
            t.Columns.Add(New DataColumn("Trip Time"))
            t.Columns.Add(New DataColumn("Idling Time"))
            t.Columns.Add(New DataColumn("Mileage"))
            t.Columns.Add(New DataColumn("Fuel"))
            t.Columns.Add(New DataColumn("Fuel Cost"))
            t.Columns.Add(New DataColumn("Liter/KM"))
            t.Columns.Add(New DataColumn("KM/Liter"))
            t.Columns.Add(New DataColumn("Cost/liter"))
            t.Columns.Add(New DataColumn("Start Address"))
            t.Columns.Add(New DataColumn("Start Maps"))
            t.Columns.Add(New DataColumn("Stop Address"))
            t.Columns.Add(New DataColumn("Stop Maps"))

            Dim refuelbeta As New RefuelBetaTrip(ddlpleate.SelectedValue, begindatetime, enddatetime)
            Dim triptable As New DataTable
            triptable = refuelbeta.TripHour(ddlUsername.SelectedValue, ddlpleate.SelectedValue, begindatetime, enddatetime)
            Dim r As DataRow
            Dim rowOdometer, rowFuelConsumption, rowFuelCost, rowTripTime As Double
            Dim dailyOdometer, dailyFuelConsumption, dailyFuelCost, dailyIdlingTime, dailyTripTime As Double
            Dim rowdate As DateTime
            Dim firstDate, lastDate

            Dim userid As String = ddlUsername.SelectedValue
            Dim companyname As String = Request.Cookies("userinfo")("companyname")
            Dim locObj As New Location(userid)
            Dim lat As Double
            Dim lon As Double


            For i As Int32 = 0 To triptable.Rows.Count - 1

                Dim triphourly As New RefuelBetaT(ddlpleate.SelectedValue, triptable.Rows(i)("startdatetime"), triptable.Rows(i)("enddatetime"))
                rowOdometer = CDbl(triphourly.fuelOdometerTotal).ToString("0.00")
                rowFuelConsumption = CDbl(triphourly.fuelConsumptionTotal).ToString("0.00")
                rowFuelCost = CDbl(triphourly.fuelConsumptionTotal * CDbl(triphourly.DieselPrice(triptable.Rows(i)("startdatetime")))).ToString("0.00")
                rowTripTime = DateDiff(DateInterval.Minute, Convert.ToDateTime(triptable.Rows(i)("startdatetime")), Convert.ToDateTime(triptable.Rows(i)("enddatetime")))

                Dim totalTimeIdling As Double = Fix(triphourly.getIdling(ddlpleate.SelectedValue))
                Dim tMins, fSecs, fHours, fMins As Double
                Dim Idling As String
                tMins = "0" & Fix(totalTimeIdling / 60)
                fSecs = "0" & totalTimeIdling - (tMins * 60)
                fHours = "0" & Fix(totalTimeIdling / 3600)
                fMins = "0" & tMins - (fHours * 60)
                Idling = fHours.ToString("00") & ":" & fMins.ToString("00") & ":" & fSecs.ToString("00")

                If i = 0 Then
                    rowdate = Convert.ToDateTime(triptable.Rows(i)("startdatetime"))
                    dailyOdometer = dailyOdometer + rowOdometer
                    If rowOdometer > 5 And rowFuelCost >= 0 Then
                        dailyFuelConsumption = dailyFuelConsumption + rowFuelConsumption
                        dailyFuelCost = dailyFuelCost + rowFuelCost
                    End If
                    dailyIdlingTime = dailyIdlingTime + tMins
                    dailyTripTime = dailyTripTime + rowTripTime
                    firstDate = triptable.Rows(i)("startdatetime")
                Else
                    If rowdate.Date = (Convert.ToDateTime(triptable.Rows(i)("startdatetime"))).Date Then
                        dailyOdometer = dailyOdometer + rowOdometer
                        If rowOdometer > 5 And rowFuelCost >= 0 Then
                            dailyFuelConsumption = dailyFuelConsumption + rowFuelConsumption
                            dailyFuelCost = dailyFuelCost + rowFuelCost
                        End If
                        dailyIdlingTime = dailyIdlingTime + tMins
                        dailyTripTime = dailyTripTime + rowTripTime
                    Else
                        r = t.NewRow
                        r(0) = ""
                        r(1) = firstDate
                        r(2) = triptable.Rows(i - 1)("enddatetime")
                        r(3) = dailyTripTime & " Mins"
                        r(4) = dailyIdlingTime & " Mins"
                        r(5) = dailyOdometer & " KM"

                        If dailyOdometer < 5 Then
                            r(6) = "-"
                            r(7) = "-"
                            r(8) = "-"
                            r(9) = "-"
                            r(10) = "-"
                            r(11) = "-"
                            r(12) = "-"
                        Else
                            r(6) = dailyFuelConsumption & " L"
                            r(7) = dailyFuelCost & " MYR"
                            r(8) = CDbl(dailyFuelConsumption / dailyOdometer).ToString("0.00")
                            r(9) = CDbl(CDbl(dailyOdometer) / CDbl(dailyFuelConsumption)).ToString("0.00")
                            r(10) = CDbl(dailyFuelCost / dailyOdometer).ToString("0.00")
                        End If

                        t.Rows.Add(r)
                        firstDate = triptable.Rows(i)("startdatetime")
                        rowdate = triptable.Rows(i)("startdatetime")
                        dailyOdometer = 0
                        dailyFuelConsumption = 0
                        dailyFuelCost = 0
                        dailyIdlingTime = 0
                        dailyTripTime = 0
                        dailyOdometer = dailyOdometer + rowOdometer
                        If rowOdometer > 5 And rowFuelCost >= 0 Then
                            dailyFuelConsumption = dailyFuelConsumption + rowFuelConsumption
                            dailyFuelCost = dailyFuelCost + rowFuelCost
                        End If
                        dailyIdlingTime = dailyIdlingTime + tMins
                        dailyTripTime = dailyTripTime + rowTripTime
                    End If
                End If

                r = t.NewRow
                r(0) = i + 1
                r(1) = triptable.Rows(i)("startdatetime")
                r(2) = triptable.Rows(i)("enddatetime")
                r(3) = rowTripTime & " Mins"
                r(4) = tMins & " Mins"
                r(5) = rowOdometer & " KM"


                Dim lastlat As Double = triptable.Rows(i)("onlat")
                Dim lastlon As Double = triptable.Rows(i)("onlon")


                r(11) = locObj.GetLocation(lastlat, lastlon)





                r(12) = "<a href=""http://maps.google.com/maps?f=q&hl=en&q=" & Math.Round(CDbl(triptable.Rows(i)("onlat")), 6) & " + " & Math.Round(CDbl(triptable.Rows(i)("onlon")), 4) & "&om=1&t=k"" target=""_blank""><img style=""border:solid 0 red;"" src=""images/googlemaps.gif"" title=""View map in Google Maps""/></a>"
                Dim offlat As Double = triptable.Rows(i)("offlat")
                Dim offlon As Double = triptable.Rows(i)("offlon")
                r(13) = locObj.GetLocation(offlat, offlon)

                r(14) = "<a href=""http://maps.google.com/maps?f=q&hl=en&q=" & Math.Round(CDbl(triptable.Rows(i)("offlat")), 6) & " + " & Math.Round(CDbl(triptable.Rows(i)("offlon")), 4) & "&om=1&t=k"" target=""_blank""><img style=""border:solid 0 red;"" src=""images/googlemaps.gif"" title=""View map in Google Maps""/></a>"
                If rowOdometer < 5 Or rowFuelCost < 0 Or (rowOdometer > 0 And rowFuelCost <= 0) Then
                    r(6) = "-"
                    r(7) = "-"
                    r(8) = "-"
                    r(9) = "-"
                    r(10) = "-"
                Else
                    r(6) = rowFuelConsumption & " L"
                    r(7) = rowFuelCost & " MYR"
                    r(8) = CDbl(rowFuelConsumption / rowOdometer).ToString("0.00")
                    r(9) = CDbl(CDbl(triphourly.fuelOdometerTotal) / CDbl(triphourly.fuelConsumptionTotal)).ToString("0.00")
                    r(10) = (CDbl(rowFuelConsumption * CDbl(triphourly.DieselPrice(triptable.Rows(i)("startdatetime")))).ToString("0.00") / rowOdometer).ToString("0.00")

                End If

                If rowOdometer > 5 And rowFuelCost >= 0 Then
                    FooterOdometer = FooterOdometer + triphourly.fuelOdometerTotal
                    FooterFuelConsumption = FooterFuelConsumption + triphourly.fuelConsumptionTotal
                    FooterFuelCost = FooterFuelCost + rowFuelCost
                End If
                FooterIdlingTime = FooterIdlingTime + tMins
                FooterTripTime = FooterTripTime + rowTripTime
                t.Rows.Add(r)

                If i = triptable.Rows.Count - 1 And Convert.ToDateTime(triptable.Rows(0)("startdatetime")).Date <> Convert.ToDateTime(triptable.Rows(i)("enddatetime")).Date Then
                    r = t.NewRow
                    r(0) = ""
                    r(1) = firstDate
                    r(2) = triptable.Rows(i)("enddatetime")
                    r(3) = dailyTripTime & " Mins"
                    r(4) = dailyIdlingTime & " Mins"
                    r(5) = dailyOdometer & " KM"

                    If dailyOdometer < 5 Then
                        r(6) = "-"
                        r(7) = "-"
                        r(8) = "-"
                        r(9) = "-"
                        r(10) = "-"
                    Else
                        r(6) = dailyFuelConsumption & " L"
                        r(7) = dailyFuelCost & " MYR"
                        r(8) = CDbl(dailyFuelConsumption / dailyOdometer).ToString("0.00")
                        r(9) = CDbl(CDbl(dailyOdometer) / CDbl(dailyFuelConsumption)).ToString("0.00")
                        r(10) = CDbl(dailyFuelCost / dailyOdometer).ToString("0.00")
                    End If
                    t.Rows.Add(r)
                End If
            Next

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
                r(10) = "--"
                r(11) = "--"
                r(12) = "--"
                r(13) = "--"
                r(14) = "--"
                t.Rows.Add(r)
            End If

            GridView1.DataSource = t
            GridView1.DataBind()

            Session.Remove("exceltable")
            Session.Remove("exceltable2")
            Session.Remove("exceltable3")

            t.Columns.Remove(t.Columns("start maps"))
            t.Columns.Remove(t.Columns("stop maps"))

            Session("exceltable") = t

            'GridView1.Columns(6).Visible = False
            'GridView1.Columns(7).Visible = False
            'GridView1.Columns(8).Visible = False
            'GridView1.Columns(9).Visible = False
            'GridView1.Columns(10).Visible = False

            ec = "true"
            If GridView1.PageCount > 1 Then
                show = True
            End If

        Catch ex As Exception
            Response.Write(ex.Message)
        End Try

    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridView1.RowDataBound
        Try
            If e.Row.RowType = DataControlRowType.DataRow Then
                Dim minutesArray As String() = CStr(e.Row.Cells(3).Text).Split(" ")
                If Double.TryParse(e.Row.Cells(0).Text, 0) = False Then
                    'e.Row.Cells.Remove(e.Row.Cells(0))
                    'e.Row.Cells(0).ColumnSpan = 3
                    e.Row.Style.Add("background-color", "darkseagreen")
                    e.Row.Style.Add("color", "BLACK")
                    e.Row.Style.Add("font-weight", "Bold")
                    e.Row.Style.Add("BORDER-TOP", "BLACK 3px solid")
                    e.Row.Style.Add("BORDER-BOTTOM", "BLACK 3px solid")

                    'e.Row.Cells(3).Style.Add("BORDER-RIGHT", "#aaccee 10px solid")
                    'e.Row.Cells(3).Style.Add("padding-left", "50px")
                    If e.Row.Cells(0).Text <> "--" Then
                        If CDbl(minutesArray(0)) > 600 Then
                            e.Row.Style.Add("color", "BLACK")
                        End If
                    End If
                    'e.Row.Cells(0).Text = "Daily Trip Summary on " & ddlpleate.SelectedValue
                    'ElseIf CDbl(minutesArray(0)) > 240 And e.Row.Cells(0).Text <> "" Then
                    '    e.Row.Style.Add("background-color", "FIREBRICK")
                    '    e.Row.Style.Add("color", "WHITE")
                End If
            End If
            If e.Row.RowType = DataControlRowType.Footer Then
                e.Row.Style.Add("BORDER-BOTTOM", "BLACK 5px double")
                e.Row.Cells(3).Text = FooterTripTime & " Mins"
                e.Row.Cells(4).Text = FooterIdlingTime & " Mins"
                e.Row.Cells(5).Text = CDbl(FooterOdometer).ToString("0.00") & " KM"
                'e.Row.Cells(6).Text = CDbl(FooterFuelConsumption).ToString("0.00") & " L"
                'e.Row.Cells(7).Text = CDbl(FooterFuelCost).ToString("0.00") & " MYR"
                'e.Row.Cells(8).Text = CDbl(FooterFuelConsumption / FooterOdometer).ToString("0.00")
                'e.Row.Cells(9).Text = CDbl(FooterOdometer / FooterFuelConsumption).ToString("0.00")
                'e.Row.Cells(10).Text = CDbl(FooterFuelCost / FooterOdometer).ToString("0.00")
            End If
        Catch ex As SystemException
            Response.Write(ex.Message)
        End Try
    End Sub

    Protected Sub GridView1_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridView1.RowCreated
        If (e.Row.RowType = DataControlRowType.Header) Then
            Dim row As GridViewRow = New GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal)
            Dim cell As New TableCell()
            cell.BackColor = System.Drawing.Color.FromArgb(255, 13, 24, 106) '.FromArgb(255, 70, 90, 232) ' '
            cell.ForeColor = System.Drawing.Color.White '.FromArgb(232, 70, 89) '
            cell.Font.Bold = True
            cell.Font.Name = "Tahoma"
            'cell.Font.Size = 11
            cell.ColumnSpan = GridView1.Columns.Count
            cell.HorizontalAlign = HorizontalAlign.Center
            cell.Text = "Vehicle Trip Summary Table"
            row.Cells.Add(cell)

            GridView1.Controls(0).Controls.AddAt(0, row)
        End If
    End Sub

    Protected Sub ddlUsername_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlUsername.SelectedIndexChanged
        getPlateNo(ddlUsername.SelectedValue)
    End Sub

    Protected Sub ImageButton1_Click(sender As Object, e As System.EventArgs) Handles ImageButton1.Click
        DisplaySummary()
    End Sub
End Class



