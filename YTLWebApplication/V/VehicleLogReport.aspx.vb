Imports System.Data.SqlClient
Imports AspMap
Imports ADODB
Imports System.Data

Partial Class VehicleLogReport2
    Inherits System.Web.UI.Page

    Public show As Boolean = False
    Public ec As String = "false"
    Public plateno As String

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try

            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            Dim query As String
            Dim dt As DataTable

            'Dim userid As String = Request.Cookies("userinfo")("userid")
            'Dim role As String = Request.Cookies("userinfo")("role")
            'Dim userslist As String = Request.Cookies("userinfo")("userslist")

            Dim userid As String = HttpContext.Current.Session("userid").ToString()
            Dim role As String = HttpContext.Current.Session("role").ToString()
            Dim userslist As String = HttpContext.Current.Session("userslist").ToString()
            Dim param As New Dictionary(Of String, Object)
            'Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            If role = "User" Then
                query = "select userid, username, dbip from userTBL where userid=@userid"
                param.Add("@userid", userid)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                query = "select userid, username, dbip from userTBL WHERE userid IN (SELECT value FROM STRING_SPLIT(@userslist, ',')) order by username"
                param.Add("@userslist", userslist)
            Else
                query = "select userid, username,dbip from userTBL where role='User' order by username"
            End If
            dt = DatabaseHelper.ExecuteQuery(query, param)
            If dt.Rows.Count > 0 Then
                For Each dr As DataRow In dt.Rows
                    ddlUsername.Items.Add(New ListItem(dr("username"), dr("userid")))
                Next
            End If
            If role = "User" Then
                ddlUsername.Items.Remove("--Select User Name--")
                ddlUsername.SelectedValue = userid
                getPlateNo(userid)
            End If
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
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub getPlateNo(ByVal uid As String)
        Try
            If ddlUsername.SelectedValue <> "--Select User Name--" Then
                ddlpleate.Items.Clear()
                ddlpleate.Items.Add("--Select Plate No--")
                Dim query As String
                Dim dt As DataTable
                Dim param As New Dictionary(Of String, Object)
                'Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

                query = "select plateno from vehicleTBL where userid=@uid order by plateno"
                param.Add("@uid", uid)
                dt = DatabaseHelper.ExecuteQuery(query, param)
                If dt.Rows.Count > 0 Then
                    For Each dr As DataRow In dt.Rows
                        ddlpleate.Items.Add(New ListItem(dr("plateno"), dr("plateno")))
                    Next
                End If
            Else
                ddlpleate.Items.Clear()
                ddlpleate.Items.Add("--Select User Name--")
            End If
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub

    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        Try
            GridView1.PageSize = noofrecords.SelectedValue
            GridView1.DataSource = Session("exceltable")
            GridView1.PageIndex = e.NewPageIndex
            GridView1.DataBind()

            ec = "true"
            show = True

            CheckBox1.Visible = True
            CheckBox2.Visible = True

            If CheckBox1.Checked = False Then
                GridView1.Columns(1).Visible = True
            Else
                GridView1.Columns(1).Visible = False
            End If

            If CheckBox2.Checked = False Then
                GridView1.Columns(3).Visible = True
            Else
                GridView1.Columns(3).Visible = False
            End If

        Catch ex As Exception

        End Try
    End Sub

    Protected Sub ddlUsername_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlUsername.SelectedIndexChanged
        getPlateNo(ddlUsername.SelectedValue)
    End Sub

    Protected Sub ImageButton1_Click(sender As Object, e As System.EventArgs) Handles ImageButton1.Click
        DisplayLogInformation()
    End Sub

    Protected Sub DisplayLogInformation()

        On Error Resume Next
        If CheckBox1.Checked = False Then
            GridView1.Columns(2).Visible = True
            lblmessage.Visible = True
            lblmessage2.Visible = True
        Else
            GridView1.Columns(2).Visible = False
            lblmessage.Visible = False
            lblmessage2.Visible = False
        End If

        If CheckBox2.Checked = False Then
            GridView1.Columns(4).Visible = True
        Else
            GridView1.Columns(4).Visible = False
        End If

        Dim bdt As String = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
        Dim edt As String = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"

        Dim interval As Byte = ddlinterval.SelectedValue

        Dim t As New DataTable

        t.Columns.Add(New DataColumn("No"))
        t.Columns.Add(New DataColumn("Date Time"))
        t.Columns.Add(New DataColumn("GPS"))
        t.Columns.Add(New DataColumn("Speed"))
        t.Columns.Add(New DataColumn("Odometer"))
        t.Columns.Add(New DataColumn("Ignition"))
        t.Columns.Add(New DataColumn("Powercut"))
        t.Columns.Add(New DataColumn("PTO"))
        t.Columns.Add(New DataColumn("Address"))
        t.Columns.Add(New DataColumn("Nearest Town"))
        t.Columns.Add(New DataColumn("Lat"))
        t.Columns.Add(New DataColumn("Lon"))
        t.Columns.Add(New DataColumn("Maps"))



        Dim gpsandignitioncondition As String = "and gps_av='A' and ignition_sensor<>0"
        If CheckBox1.Checked = False And CheckBox2.Checked = False Then
            gpsandignitioncondition = ""
        ElseIf CheckBox1.Checked = False And CheckBox2.Checked = True Then
            gpsandignitioncondition = "and ignition_sensor<>0"
        ElseIf CheckBox1.Checked = True And CheckBox2.Checked = False Then
            gpsandignitioncondition = "and gps_av='A'"
        End If
        Dim param As New Dictionary(Of String, Object)
        Dim query As String = "select distinct convert(varchar(19),timestamp,120) as datetime,alarm,vt.pto,gps_av,speed,gps_odometer,ignition_sensor,round(lat,6) as lat,round(lon,6) as lon,powercut from vehicle_history vht Join  vehicleTBL vt on vt.plateno=vht.plateno and  vt.plateno =@ddlplate" & gpsandignitioncondition & " and timestamp between @bdt and @edt and gps_av<>'E'"
        param.Add("@ddlplate", ddlpleate.SelectedValue)
        param.Add("@bdt", bdt)
        param.Add("@edt", edt)
        Dim dt As DataTable = DatabaseHelper.ExecuteQuery(query, param)
        Dim dr As DataRow

        Dim r As DataRow

        Dim address As String = ""
        Dim lafargeGeofenceName As String = ""
        Dim privateGeofenceName As String = ""
        Dim publicGeofenceName As String = ""

        Dim mappoint As String = ""
        Dim poi As String = ""

        Dim lat As Double
        Dim lon As Double

        Dim i As Int64 = 1

        Dim userid As String = ddlUsername.SelectedValue
        Dim previousdatetime As DateTime
        Dim presentdatetime As DateTime
        Dim locObj As New Location(userid)

        If dt.Rows.Count > 0 Then
            For Each dr In dt.Rows
                presentdatetime = dr("datetime")


                If ((presentdatetime - previousdatetime).TotalMinutes >= interval) Then



                    previousdatetime = presentdatetime
                    r = t.NewRow

                    r(0) = i.ToString()

                    r(1) = dr("datetime")

                    r(2) = dr("gps_av")
                    r(3) = System.Convert.ToDouble(dr("speed")).ToString("0.00")
                    If System.Convert.ToDouble(dr("gps_odometer")) = 99 Then
                        r(4) = "-"
                    Else
                        r(4) = (System.Convert.ToDouble(dr("gps_odometer")) / 100.0).ToString("0.00")
                    End If

                    r(5) = "OFF"

                    If dr("ignition_sensor") = 1 Then
                        r(5) = "ON"
                    End If
                    r(6) = "No"
                    If dr("powercut") = 1 Then
                        r(6) = "Yes"
                    End If

                    r(7) = "--"

                    If dr("pto") Then
                        r(7) = dr("alarm")
                    End If


                    address = ""

                    lat = dr("lat")
                    lon = dr("lon")

                    r(8) = locObj.GetLocation(lat, lon)


                    r(9) = locObj.GetNearestTown(lat, lon)
                    r(10) = dr("lat")
                    r(11) = dr("lon")
                    r(12) = "<a href='http://maps.google.com/maps?f=q&hl=en&q=" & dr("lat") & " + " & dr("lon") & "&om=1&t=k' target='_blank'><img style='border:solid 0 red;' src='images/googlemaps1.gif' title='View map in Google Maps'/></a>   <a href='GoogleEarthMaps.aspx?x=" & dr("lon") & "&y=" & dr("lat") & "'><img style='border:solid 0 red;' src='images/googleearth1.gif' title='View map in GoogleEarth'/></a>"

                    t.Rows.Add(r)
                    i = i + 1
                End If
            Next
        End If
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
            t.Rows.Add(r)
        End If

        Session.Remove("exceltable")
        Session.Remove("exceltable2")
        Session.Remove("exceltable3")
        Session.Remove("tempTable")

        Session("exceltable") = t

        GridView1.PageSize = noofrecords.SelectedValue
        GridView1.DataSource = t
        GridView1.DataBind()
        ec = "true"
        CheckBox1.Visible = True
        CheckBox2.Visible = True

        If GridView1.PageCount > 1 Then
            show = True
        End If


    End Sub

End Class
