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
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")

            Dim cmd As SqlCommand
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim conn2 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim ds As New DataSet
            Dim da As SqlDataAdapter
            ' ddlGeofence.Items.Add(New ListItem("--All Geofences--", "--All Geofences--"))
            cmd = New SqlCommand("select * from userTBL where role='User' order by username", conn)
            ' da = New SqlDataAdapter("select geofencename,geofenceid from geofence where accesstype='1' order by geofencename", conn2)
            da = New SqlDataAdapter("select geofencename,geofenceid from geofence where accesstype='1' order by LTRIM(geofencename)", conn2)

            If role = "User" Then
                cmd = New SqlCommand("select * from userTBL where userid='" & userid & "' order by username", conn)

            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select * from userTBL where role='User' and userid in (" & userslist & ") order by username", conn)
                DropDownList1.Items.Add(New ListItem("--Select User Name--", "--Select User Name--"))
            Else
                DropDownList1.Items.Add(New ListItem("--Select User Name--", "--Select User Name--"))
            End If
            'da.Fill(ds)
            Dim count As Integer = 0
            'For count = 0 To ds.Tables(0).Rows.Count - 1
            '    ddlGeofence.Items.Add(New ListItem(ds.Tables(0).Rows(count)(0).ToString().ToUpper(), ds.Tables(0).Rows(count)(1)))
            'Next
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()


            While (dr.Read())
                DropDownList1.Items.Add((New ListItem(dr("username").ToString(), dr("userid").ToString())))
            End While
            DropDownList1.SelectedValue = Request.Form("DropDownList1")
            ddlplate.Items.Add(New ListItem("--Select Plate No--", "--Select Plate No--"))
            If role = "User" Then
                cmd = New SqlCommand("select * from vehicleTBL where userid='" & userid & "' order by plateno", conn)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                cmd = New SqlCommand("select * from vehicleTBL where userid in (" & userslist & ") order by plateno", conn)
            Else
                cmd = New SqlCommand("select * from vehicleTBL order by plateno", conn)
            End If

            dr = cmd.ExecuteReader()

            While (dr.Read())
                ddlplate.Items.Add((New ListItem(dr("plateno"), dr("plateno"))))
            End While

            ddlplate.SelectedValue = Request.Form("ddlplate")
            ' ddlGeofence.SelectedValue = Request.Form("ddlGeofence")
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
            Else
                Dim userid As String = Request.Cookies("userinfo")("userid")
                Dim role As String = Request.Cookies("userinfo")("role")
                Dim userslist As String = Request.Cookies("userinfo")("userslist")
                Dim dr As SqlDataReader
                Dim cmd As SqlCommand
                ddlplate.Items.Clear()

                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                ddlplate.Items.Add(New ListItem("--Select Plate No--", "--Select Plate No--"))
                cmd = New SqlCommand("select * from vehicleTBL where userid='" & Request.Form("DropDownList1") & "' order by plateno", conn)
                ' If Request.Form("DropDownList1") = "--Select User Name--" Then
                'If role = "SuperUser" Or role = "Operator" Then
                '        cmd = New SqlCommand("select * from vehicleTBL where userid in (" & userslist & ") order by plateno", conn)
                '    Else
                '        cmd = New SqlCommand("select * from vehicleTBL order by plateno", conn)
                '    End If
                ' End If
                Try
                    If conn.State <> ConnectionState.Open Then
                        conn.Open()
                    End If
                    dr = cmd.ExecuteReader()
                    While (dr.Read())
                        ddlplate.Items.Add((New ListItem(dr("plateno"), dr("plateno"))))
                    End While
                Catch ex As Exception
                Finally

                    conn.Close()
                End Try
                ddlplate.SelectedValue = Request.Form("ddlplate")
            End If


        Catch ex As Exception

        End Try
    End Sub

    Protected Sub DisplayLogInformation()
        Try

            Dim begindatetime As String = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
            Dim enddatetime As String = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"
            Dim userid As String = DropDownList1.SelectedValue
            Dim plateno As String = ddlplate.SelectedValue
            Dim uid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")



            Dim t As New DataTable
            t.Columns.Add(New DataColumn("S No"))
            t.Columns.Add(New DataColumn("Plate No"))
            t.Columns.Add(New DataColumn("Date Time"))
            t.Columns.Add(New DataColumn("Trailer"))
            t.Columns.Add(New DataColumn("Duration (min)"))
            t.Columns.Add(New DataColumn("Trailer ID"))
            t.Columns.Add(New DataColumn("Trailer No"))
            t.Columns.Add(New DataColumn("Address"))

            Dim query As String = ""
            Dim condition As String = ""
            Dim r As DataRow
            Dim i As Int64 = 1
            Dim prevstatus As String = ""
            Dim prevtimestamp, currenttimestamp As String
            prevtimestamp = ""
            currenttimestamp = ""
            Dim currentstatus As String = ""
            Dim locObj As New Location(userid)
            Dim lat As Double
            Dim lon As Double

            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Try


                Dim cmd As SqlCommand = New SqlCommand("select   timestamp,h.plateno,h.trailer,h.trailerid,t.trailerno,h.lat,h.lon from vehicle_history2 h  left join trailer2 t on h.trailerid=t.trailerid  where h.plateno ='" & ddlplate.SelectedValue & "' and timestamp between '" & begindatetime & "' and '" & enddatetime & "'  order by timestamp", conn)
                ' cmd = New SqlCommand("select  timestamp,h.plateno,h.trailer,t.trailerid,t.trailerno,h.lat,h.lon from vehicle_history2 h  join trailer2 t on h.trailerid=t.trailerid where h.plateno ='kdq7475' and timestamp between '2017-07-02 00:00:00' and '2017-07-02 23:59:59' order by timestamp ", conn)
                conn.Open()
                Dim dr As SqlDataReader = cmd.ExecuteReader()
                While dr.Read
                    If (dr("Trailer")) Then
                        currentstatus = "Attach"
                        currenttimestamp = Convert.ToDateTime(dr("timestamp")).ToString("yyyy/MM/dd HH:mm:ss")
                        'currenttimestamp = ""
                    Else
                        currentstatus = "Detach"
                        currenttimestamp = Convert.ToDateTime(dr("timestamp")).ToString("yyyy/MM/dd HH:mm:ss")
                    End If
                    If prevstatus <> currentstatus Then


                        r = t.NewRow
                        r(0) = i.ToString()

                        r(1) = dr("plateno")
                        r(2) = Convert.ToDateTime(dr("timestamp")).ToString("yyyy/MM/dd HH:mm:ss")
                        r(3) = False
                        If (dr("Trailer")) Then
                            r(3) = "Attach"
                        Else
                            r(3) = "Detach"
                        End If
                        If Not IsDBNull(dr("trailerid")) Then
                            r(4) = dr("trailerid")

                        Else
                            r(4) = ""
                        End If
                        Try
                            If currentstatus = "Detach" Then
                                If Not currenttimestamp = "" Then
                                    r(5) = (Convert.ToDateTime(currenttimestamp) - Convert.ToDateTime(prevtimestamp)).TotalMinutes.ToString("0")
                                Else
                                    r(5) = "N/A"
                                End If

                            End If
                        Catch ex As Exception
                            r(5) = "N/A"
                        End Try




                        If Not IsDBNull(dr("trailerno")) Then
                            r(6) = dr("trailerno")
                        Else
                            r(6) = ""
                        End If
                        lat = dr("lat")
                        lon = dr("lon")

                        r(7) = locObj.GetLocation(lat, lon)

                        prevstatus = currentstatus
                        If prevstatus = "Attach" Then
                            prevtimestamp = currenttimestamp
                        End If

                        t.Rows.Add(r)
                        i = i + 1
                    End If
                End While





            Catch ex As Exception
                Response.Write(ex.Message)
            Finally
                conn.Close()
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
                t.Rows.Add(r)
            End If
            Session.Remove("exceltable")
            Session("exceltable") = t
            sb1.Length = 0


            Session.Remove("exceltable")
            Session("exceltable") = t
            sb1.Length = 0
            If (t.Rows.Count > 0) Then
                ec = "true"

                sb1.Append("<table cellpadding=""0"" cellspacing=""0"" border=""0"" class=""display"" id=""examples"" style=""font-size: 10px;font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;"">")

                sb1.Append("<thead><tr><th>S No</th><th>Plate NO</th><th>Date Time</th><th>Trailer</th><th>Duration (mins)</th><th>Trailer ID</th><th>Trailer No</th><th>Location</th></tr></thead>")

                sb1.Append("<tbody>")
                Dim counter As Integer = 1
                For k As Integer = 0 To t.Rows.Count - 1
                    Try
                        sb1.Append("<tr>")
                        sb1.Append("<td>")
                        sb1.Append(t.DefaultView.Item(k)(0))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(k)(1))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(k)(2))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(k)(3))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(k)(5))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(k)(4))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(k)(6))
                        sb1.Append("</td><td>")
                        sb1.Append(t.DefaultView.Item(k)(7))
                        sb1.Append("</td></tr>")
                        counter += 1
                    Catch ex As Exception

                    End Try
                Next
                sb1.Append("</tbody>")
                sb1.Append("<tfoot><tr><th>S No</th><th>Plate NO</th><th>Date Time</th><th>Trailer</th><th>Duration (mins)</th><th>Trailer ID</th><th>Trailer No</th><th>Location</th></tr></tfoot>")
            Else

            End If

        Catch ex As Exception

        End Try

    End Sub




    Protected Sub ImageButton1_Click(sender As Object, e As System.EventArgs) Handles ImageButton1.Click
        DisplayLogInformation()
    End Sub
End Class
