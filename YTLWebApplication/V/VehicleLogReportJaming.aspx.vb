Imports System.Data.SqlClient
Imports AspMap
Imports ADODB
Imports System.Data

Partial Class VehicleLogReportJaming
    Inherits System.Web.UI.Page

    Public show As Boolean = False
    Public ec As String = "false"
    Public plateno As String

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            ' SECURITY FIX: Enable authentication check
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If

            ' SECURITY FIX: Get validated user information
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)

            Dim query As String
            Dim param As New Dictionary(Of String, Object)
            
            If role = "User" Then
                query = "select userid, username, dbip from userTBL where userid=@userid"
                param.Add("@userid", userid)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                ' SECURITY FIX: Validate userslist and use safe query construction
                If SecurityHelper.IsValidUsersList(userslist) Then
                    Dim userIds() As String = userslist.Split(","c)
                    Dim parameters As New List(Of String)
                    
                    For i As Integer = 0 To userIds.Length - 1
                        Dim paramName As String = "@userid" & i
                        parameters.Add(paramName)
                        param.Add(paramName, userIds(i).Trim())
                    Next
                    
                    Dim inClause As String = String.Join(",", parameters)
                    query = $"select userid, username, dbip from userTBL WHERE userid IN ({inClause}) order by username"
                Else
                    query = "select userid, username, dbip from userTBL where userid=@userid"
                    param.Add("@userid", userid)
                End If
            Else
                query = "select userid, username,dbip from userTBL where role='User' order by username"
            End If
            
            Dim dt As DataTable = SecurityHelper.ExecuteSecureQuery(query, param)
            If dt.Rows.Count > 0 Then
                For Each dr As DataRow In dt.Rows
                    ddlUsername.Items.Add(New ListItem(SecurityHelper.HtmlEncode(dr("username").ToString()), dr("userid").ToString()))
                Next
            End If
            
            If role = "User" Then
                ddlUsername.Items.Remove("--Select User Name--")
                ddlUsername.SelectedValue = userid
                getPlateNo(userid)
            End If

        Catch ex As Exception
            SecurityHelper.LogError("VehicleLogReportJaming OnInit Error", ex, Server)
            Response.Redirect("Error.aspx")
        End Try
        MyBase.OnInit(e)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If

            If Page.IsPostBack = False Then
                ImageButton1.Attributes.Add("onclick", "return mysubmit()")
                
                ' SECURITY FIX: Validate date inputs
                If SecurityHelper.ValidateDate(DateTime.Now.ToString("yyyy/MM/dd")) Then
                    txtBeginDate.Value = DateTime.Now.ToString("yyyy/MM/dd")
                    txtEndDate.Value = DateTime.Now.ToString("yyyy/MM/dd")
                End If
            End If
        Catch ex As Exception
            SecurityHelper.LogError("VehicleLogReportJaming Page_Load Error", ex, Server)
            Response.Redirect("Error.aspx")
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
                
                Dim query As String = "select plateno from vehicleTBL where userid=@uid order by plateno"
                Dim param As New Dictionary(Of String, Object) From {{"@uid", uid}}
                
                Dim dt As DataTable = SecurityHelper.ExecuteSecureQuery(query, param)
                If dt.Rows.Count > 0 Then
                    For Each dr As DataRow In dt.Rows
                        ddlpleate.Items.Add(New ListItem(SecurityHelper.HtmlEncode(dr("plateno").ToString()), dr("plateno").ToString()))
                    Next
                End If
            Else
                ddlpleate.Items.Clear()
                ddlpleate.Items.Add("--Select User Name--")
            End If
        Catch ex As Exception
            SecurityHelper.LogError("getPlateNo Error", ex, Server)
        End Try
    End Sub

    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridView1.PageIndexChanging
        Try
            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If

            GridView1.PageSize = SecurityHelper.ValidateNumeric(noofrecords.SelectedValue, 1, 10000)
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
            SecurityHelper.LogError("GridView1_PageIndexChanging Error", ex, Server)
        End Try
    End Sub

    Protected Sub ddlUsername_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlUsername.SelectedIndexChanged
        Try
            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If
            
            getPlateNo(ddlUsername.SelectedValue)
        Catch ex As Exception
            SecurityHelper.LogError("ddlUsername_SelectedIndexChanged Error", ex, Server)
        End Try
    End Sub

    Protected Sub ImageButton1_Click(sender As Object, e As System.EventArgs) Handles ImageButton1.Click
        Try
            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If
            
            DisplayLogInformation()
        Catch ex As Exception
            SecurityHelper.LogError("ImageButton1_Click Error", ex, Server)
        End Try
    End Sub

    Protected Sub DisplayLogInformation()
        Try
            ' SECURITY FIX: Validate user permissions
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If

            ' SECURITY FIX: Validate inputs
            If Not SecurityHelper.ValidateDate(txtBeginDate.Value) OrElse Not SecurityHelper.ValidateDate(txtEndDate.Value) Then
                Return
            End If

            If Not SecurityHelper.ValidatePlateNumber(ddlpleate.SelectedValue) Then
                Return
            End If

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

            Dim interval As Byte = SecurityHelper.ValidateNumeric(ddlinterval.SelectedValue, 0, 1440)

            Dim t As New DataTable
            t.Columns.Add(New DataColumn("No"))
            t.Columns.Add(New DataColumn("Date Time"))
            t.Columns.Add(New DataColumn("GPS"))
            t.Columns.Add(New DataColumn("Speed"))
            t.Columns.Add(New DataColumn("Odometer"))
            t.Columns.Add(New DataColumn("Ignition"))
            t.Columns.Add(New DataColumn("PTO"))
            t.Columns.Add(New DataColumn("Jaming"))
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
            Dim query As String = "select distinct convert(varchar(19),timestamp,120) as datetime,alarm,vt.pto,gps_av,speed,gps_odometer,ignition_sensor,lat,lon,jaming from vehicle_history vht Join vehicleTBL vt on vt.plateno=vht.plateno and vt.plateno =@ddlplate" & gpsandignitioncondition & " and timestamp between @bdt and @edt"
            
            If CheckBox3.Checked = True Then
                query &= " and jaming=1"
            End If
            
            param.Add("@ddlplate", ddlpleate.SelectedValue)
            param.Add("@bdt", bdt)
            param.Add("@edt", edt)

            Dim dt As DataTable = SecurityHelper.ExecuteSecureQuery(query, param)
            Dim dr As DataRow
            Dim r As DataRow

            Dim address As String = ""
            Dim lat As Double
            Dim lon As Double
            Dim i As Int64 = 1

            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
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
                        r(1) = SecurityHelper.HtmlEncode(dr("datetime").ToString())
                        r(2) = SecurityHelper.HtmlEncode(dr("gps_av").ToString())
                        r(3) = System.Convert.ToDouble(dr("speed")).ToString("0.00")
                        r(4) = (System.Convert.ToDouble(dr("gps_odometer")) / 100.0).ToString("0.00")
                        
                        r(5) = "OFF"
                        If dr("ignition_sensor") = 1 Then
                            r(5) = "ON"
                        End If

                        r(6) = "--"
                        If dr("pto") Then
                            r(6) = SecurityHelper.HtmlEncode(dr("alarm").ToString())
                        End If

                        If Not IsDBNull(dr("jaming")) Then
                            If Convert.ToBoolean(dr("jaming")) Then
                                r(7) = "Yes"
                            Else
                                r(7) = "No"
                            End If
                        Else
                            r(7) = "No"
                        End If

                        address = ""
                        lat = dr("lat")
                        lon = dr("lon")

                        ' SECURITY FIX: Validate coordinates
                        If SecurityHelper.ValidateCoordinate(lat.ToString(), lon.ToString()) Then
                            r(8) = SecurityHelper.HtmlEncode(locObj.GetLocation(lat, lon))
                            r(9) = SecurityHelper.HtmlEncode(locObj.GetNearestTown(lat, lon))
                            r(10) = lat.ToString("0.000000")
                            r(11) = lon.ToString("0.000000")
                            r(12) = $"<a href='http://maps.google.com/maps?f=q&hl=en&q={lat}+{lon}&om=1&t=k' target='_blank'><img style='border:solid 0 red;' src='images/googlemaps1.gif' title='View map in Google Maps'/></a> <a href='GoogleEarthMaps.aspx?x={lon}&y={lat}'><img style='border:solid 0 red;' src='images/googleearth1.gif' title='View map in GoogleEarth'/></a>"
                        Else
                            r(8) = "--"
                            r(9) = "--"
                            r(10) = "--"
                            r(11) = "--"
                            r(12) = "--"
                        End If

                        t.Rows.Add(r)
                        i = i + 1
                    End If
                Next
            End If

            If t.Rows.Count = 0 Then
                r = t.NewRow
                For j As Integer = 0 To 12
                    r(j) = "--"
                Next
                t.Rows.Add(r)
            End If

            Session.Remove("exceltable")
            Session.Remove("exceltable2")
            Session.Remove("exceltable3")
            Session.Remove("tempTable")

            Session("exceltable") = t

            GridView1.PageSize = SecurityHelper.ValidateNumeric(noofrecords.SelectedValue, 1, 10000)
            GridView1.DataSource = t
            GridView1.DataBind()
            ec = "true"
            CheckBox1.Visible = True
            CheckBox2.Visible = True

            If GridView1.PageCount > 1 Then
                show = True
            End If

        Catch ex As Exception
            SecurityHelper.LogError("DisplayLogInformation Error", ex, Server)
        End Try
    End Sub

End Class