Imports System.Data.SqlClient
Imports AspMap
Imports ADODB
Imports System.Data

Namespace AVLS
    Partial Class VehicleSpeedReport
        Inherits System.Web.UI.Page
        Public show As Boolean = False
        Public ec As String = "false"
        Public plateno As String

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            Try

                If Request.Cookies("userinfo") Is Nothing Then
                    Response.Redirect("Login.aspx")
                End If

                Dim cmd As SqlCommand
                Dim dr As SqlDataReader

                Dim userid As String = Request.Cookies("userinfo")("userid")
                Dim role As String = Request.Cookies("userinfo")("role")
                Dim userslist As String = Request.Cookies("userinfo")("userslist")

                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                cmd = New SqlCommand("select userid, username,dbip from userTBL where role='User' order by username", conn)
                If role = "User" Then
                    cmd = New SqlCommand("select userid, username, dbip from userTBL where userid='" & userid & "'", conn)
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    cmd = New SqlCommand("select userid, username, dbip from userTBL where userid in (" & userslist & ") order by username", conn)
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
            'MyBase.OnInit(e)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Try
                If Page.IsPostBack = False Then
                    txtBeginDate.Value = Now().ToString("yyyy/MM/dd")
                    txtEndDate.Value = Now().ToString("yyyy/MM/dd")
                    ImageButton1.Attributes.Add("onclick", "return mysubmit()")
                End If

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

        Protected Sub DisplayLogInformation()
            Try
                Dim userid As String = ddlUsername.SelectedValue

                Dim begindatetime = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
                Dim enddatetime = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"
                Dim maxspeed As Double = ddlspeed.SelectedValue

                Dim t As New DataTable
                t.Columns.Add(New DataColumn("Sno"))
                t.Columns.Add(New DataColumn("Date Time"))
                t.Columns.Add(New DataColumn("Speed (Km/h)"))
                t.Columns.Add(New DataColumn("Speed (Mph)"))
                t.Columns.Add(New DataColumn("Maps"))



                'Read data from database server
                'Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))


                Dim cmd As SqlCommand

                cmd = New SqlCommand("select distinct convert(varchar(19),timestamp,120) as datetime,speed,lat,lon from vehicle_history where plateno ='" & ddlpleate.SelectedValue & "' and (gps_av='A' or (gps_av='V' and ignition_sensor='0')) and timestamp between '" & begindatetime & "' and '" & enddatetime & "'and ignition_sensor='1' and speed > 0 ", conn)

                Dim dr As SqlDataReader


                conn.Open()
                dr = cmd.ExecuteReader()
                Dim i As Int32 = 1
                Dim r As DataRow
                'Dim rpdf As DataRow
                Dim speed As Double
                While dr.Read()
                    r = t.NewRow

                    speed = dr("speed")
                    If speed > maxspeed Then
                        r(0) = "<span style='color:red;'>" & i.ToString() & "</span>"
                        r(1) = "<span style='color:red;'>" & dr("datetime") & "</span>"
                        r(2) = "<span style='color:red;'>" & speed.ToString("0.#0") & "</span>"
                        r(3) = "<span style='color:red;'>" & (speed * 0.621371192).ToString("0.#0") & "</span>"
                    Else
                        r(0) = i.ToString()
                        r(1) = dr("datetime")
                        r(2) = speed.ToString("0.#0")
                        r(3) = (speed * 0.621371192).ToString("0.#0")



                    End If
                    r(4) = "<a rel='balloon1' href='GussmannMap.aspx?userid=" & userid & "&x=" & dr("lon") & "&y=" & dr("lat") & "' target='_blank'><img style='border:solid 0 red;' src='images/gussmannmaps.gif' title='View map in Gussmann Maps' onmouseover='mouseover(" & userid & "," & dr("lon") & "," & dr("lat") & ");'/></a>   <a href='http://maps.google.com/maps?f=q&hl=en&q=" & dr("lat") & " + " & dr("lon") & "&om=1&t=k' target='_blank'><img style='border:solid 0 red;' src='images/googlemaps1.gif' title='View map in Google Maps'/></a>"
                    t.Rows.Add(r)

                    i = i + 1
                End While
                If t.Rows.Count <= 0 Then
                    'No Records Found

                    r = t.NewRow
                    r(0) = "--"
                    r(1) = "--"
                    r(2) = "--"
                    r(3) = "--"
                    r(4) = "--"
                    t.Rows.Add(r)
                End If

                Session.Remove("exceltable")
                Session.Remove("exceltable2")
                Session.Remove("exceltable3")

                Session("exceltable") = t
                ViewState("exceltable") = t

                GridView1.PageSize = noofrecords.SelectedValue
                GridView1.DataSource = t
                GridView1.DataBind()
                ec = "true"

                If GridView1.PageCount > 1 Then
                    show = True
                End If
            Catch ex As SystemException
                Response.Write(ex.Message)
            Finally
            End Try

        End Sub

        Protected Sub DisplayLogInformationOverSpeed()
            Try
                Dim userid As String = ddlUsername.SelectedValue

                Dim begindatetime = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
                Dim enddatetime = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"
                Dim maxspeed As Double = ddlspeed.SelectedValue

                Dim t As New DataTable
                t.Columns.Add(New DataColumn("Sno"))
                t.Columns.Add(New DataColumn("Date Time"))
                t.Columns.Add(New DataColumn("Speed (Km/h)"))
                t.Columns.Add(New DataColumn("Speed (Mph)"))
                t.Columns.Add(New DataColumn("Maps"))



                'Read data from database server
                'Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))


                Dim cmd As SqlCommand
                cmd = New SqlCommand("select distinct convert(varchar(19),timestamp,120) as datetime,speed,lat,lon from vehicle_history where plateno ='" & ddlpleate.SelectedValue & "' and (gps_av='A' or (gps_av='V' and ignition_sensor='0')) and timestamp between '" & begindatetime & "' and '" & enddatetime & "'and ignition_sensor='1' and speed > " & maxspeed, conn)
                Dim dr As SqlDataReader
                Try

                    conn.Open()
                    dr = cmd.ExecuteReader()
                    Dim i As Int32 = 1
                    Dim r As DataRow
                    'Dim rpdf As DataRow
                    Dim speed As Double
                    While dr.Read()
                        r = t.NewRow

                        speed = dr("speed")

                        r(0) = "<span style='color:red;'>" & i.ToString() & "</span>"
                        r(1) = "<span style='color:red;'>" & dr("datetime") & "</span>"
                        r(2) = "<span style='color:red;'>" & speed.ToString("0.#0") & "</span>"
                        r(3) = "<span style='color:red;'>" & (speed * 0.621371192).ToString("0.#0") & "</span>"

                        r(4) = "<a rel='balloon1' href='GussmannMap.aspx?userid=" & userid & "&x=" & dr("lon") & "&y=" & dr("lat") & "' target='_blank'><img style='border:solid 0 red;' src='images/gussmannmaps.gif' title='View map in Gussmann Maps' onmouseover='mouseover(" & userid & "," & dr("lon") & "," & dr("lat") & ");'/></a>   <a href='http://maps.google.com/maps?f=q&hl=en&q=" & dr("lat") & " + " & dr("lon") & "&om=1&t=k' target='_blank'><img style='border:solid 0 red;' src='images/googlemaps1.gif' title='View map in Google Maps'/></a>"
                        t.Rows.Add(r)

                        i = i + 1
                    End While
                    If t.Rows.Count <= 0 Then
                        'No Records Found

                        r = t.NewRow
                        r(0) = "--"
                        r(1) = "--"
                        r(2) = "--"
                        r(3) = "--"
                        r(4) = "--"
                        t.Rows.Add(r)
                    End If
                Catch ex As Exception

                Finally
                    conn.Close()
                End Try

                Session.Remove("exceltable")
                Session.Remove("exceltable2")
                Session.Remove("exceltable3")

                Session("exceltable") = t
                ViewState("exceltable") = t

                GridView1.PageSize = noofrecords.SelectedValue
                GridView1.DataSource = t
                GridView1.DataBind()
                ec = "true"

                If GridView1.PageCount > 1 Then
                    show = True
                End If
            Catch ex As SystemException
                Response.Write(ex.Message)
            End Try

        End Sub

        Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridView1.PageIndexChanging
            GridView1.PageSize = noofrecords.SelectedValue
            'GridView1.DataSource = Session("exceltable")
            GridView1.DataSource = ViewState("exceltable")
            GridView1.PageIndex = e.NewPageIndex
            GridView1.DataBind()

            ec = "true"
            show = True

        End Sub

        Protected Sub ddlUsername_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlUsername.SelectedIndexChanged
            getPlateNo(ddlUsername.SelectedValue)
        End Sub

        Protected Sub ImageButton1_Click(sender As Object, e As System.EventArgs) Handles ImageButton1.Click
            If cbOverSpeed.Checked = False Then
                DisplayLogInformation()
            Else
                DisplayLogInformationOverSpeed()
            End If
        End Sub
    End Class
End Namespace
