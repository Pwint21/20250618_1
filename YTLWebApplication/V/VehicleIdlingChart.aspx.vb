Imports System.Data.SqlClient
Imports ChartDirector

Partial Class VehicleIdlingChart
    Inherits System.Web.UI.Page
    Public xyvalues As String
    Public ilat, ilon As Double
    Public ec As String = "false"
    Public suser As String
    Public sgroup As String
    
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
            SecurityHelper.LogError("VehicleIdlingChart OnInit Error", ex, Server)
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
                
                ' SECURITY FIX: Validate query string parameters
                Dim userid As String = SecurityHelper.SanitizeString(Request.QueryString("u"), 50)
                Dim plateno As String = SecurityHelper.SanitizeString(Request.QueryString("p"), 20)

                If userid.IndexOf(",") > 0 Then
                    Dim sgroupname As String() = userid.Split(","c)
                    suser = SecurityHelper.SanitizeString(sgroupname(0), 50)
                    sgroup = SecurityHelper.SanitizeString(sgroupname(1), 50)
                End If

                If Not String.IsNullOrEmpty(suser) AndAlso SecurityHelper.ValidateUserId(suser) Then
                    Dim query As String = "select plateno from vehicleTBL where userid=@userid order by plateno"
                    Dim param As New Dictionary(Of String, Object) From {{"@userid", suser}}
                    Dim dt As DataTable = SecurityHelper.ExecuteSecureQuery(query, param)
                    
                    If dt.Rows.Count > 0 Then
                        For Each dr As DataRow In dt.Rows
                            ddlpleate.Items.Add(New ListItem(SecurityHelper.HtmlEncode(dr("plateno").ToString()), dr("plateno").ToString()))
                        Next
                    End If
                    
                    ddlUsername.SelectedValue = suser
                ElseIf Not String.IsNullOrEmpty(userid) AndAlso SecurityHelper.ValidateUserId(userid) Then
                    Dim query As String = "select plateno from vehicleTBL where userid=@userid order by plateno"
                    Dim param As New Dictionary(Of String, Object) From {{"@userid", userid}}
                    Dim dt As DataTable = SecurityHelper.ExecuteSecureQuery(query, param)
                    
                    If dt.Rows.Count > 0 Then
                        For Each dr As DataRow In dt.Rows
                            ddlpleate.Items.Add(New ListItem(SecurityHelper.HtmlEncode(dr("plateno").ToString()), dr("plateno").ToString()))
                        Next
                    End If
                    
                    ddlUsername.SelectedValue = userid
                End If
                
                If Not String.IsNullOrEmpty(plateno) AndAlso SecurityHelper.ValidatePlateNumber(plateno) Then
                    ddlpleate.SelectedValue = plateno
                End If

                ' SECURITY FIX: Validate date parameters
                If Not String.IsNullOrEmpty(plateno) Then
                    Dim begindatetime As String = SecurityHelper.SanitizeString(Request.QueryString("bdt"), 50)
                    Dim enddatetime As String = SecurityHelper.SanitizeString(Request.QueryString("edt"), 50)
                    
                    If SecurityHelper.ValidateDate(begindatetime) AndAlso SecurityHelper.ValidateDate(enddatetime) Then
                        txtBeginDate.Value = DateTime.Parse(begindatetime).ToString("yyyy/MM/dd")
                        txtEndDate.Value = DateTime.Parse(enddatetime).ToString("yyyy/MM/dd")
                        ddlUsername.SelectedValue = userid
                        ddlpleate.SelectedValue = plateno

                        DisplayChart(plateno, begindatetime, enddatetime, ddlUsername.SelectedValue)
                    End If
                End If
            End If
        Catch ex As Exception
            SecurityHelper.LogError("VehicleIdlingChart Page_Load Error", ex, Server)
            Response.Redirect("Error.aspx")
        End Try
    End Sub

    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ImageButton1.Click
        Try
            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If

            ' SECURITY FIX: Validate inputs
            If Not SecurityHelper.ValidatePlateNumber(ddlpleate.SelectedValue) Then
                Return
            End If

            If Not SecurityHelper.ValidateDate(txtBeginDate.Value) OrElse Not SecurityHelper.ValidateDate(txtEndDate.Value) Then
                Return
            End If

            Dim plateno As String = ddlpleate.SelectedValue
            Dim begindatetime As String = txtBeginDate.Value & " " & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
            Dim enddatetime As String = txtEndDate.Value & " " & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":00"
            Dim userid As String = ddlUsername.SelectedValue

            DisplayChart(plateno, begindatetime, enddatetime, userid)

        Catch ex As Exception
            SecurityHelper.LogError("ImageButton1_Click Error", ex, Server)
        End Try
    End Sub

    Protected Sub DisplayChart(ByVal plateno As String, ByVal begindatetime As String, ByVal enddatetime As String, ByVal userid As String)
        Try
            ' SECURITY FIX: Validate all inputs
            If Not SecurityHelper.ValidatePlateNumber(plateno) OrElse Not SecurityHelper.ValidateUserId(userid) Then
                Return
            End If

            If Not SecurityHelper.ValidateDate(begindatetime) OrElse Not SecurityHelper.ValidateDate(enddatetime) Then
                Return
            End If

            ' SECURITY FIX: Use parameterized query
            Dim query As String = "select distinct convert(varchar(19),timestamp,120) as datetime,speed,ignition,lon,lat from vehicle_history2 where plateno =@plateno and (gps_av='A' or (gps_av='V' and ignition='0')) and timestamp between @begindatetime and @enddatetime"
            Dim param As New Dictionary(Of String, Object) From {
                {"@plateno", plateno},
                {"@begindatetime", begindatetime},
                {"@enddatetime", enddatetime}
            }

            Dim stoptime As TimeSpan = New TimeSpan(0, 0, 0)
            Dim ideltime As TimeSpan = New TimeSpan(0, 0, 0)
            Dim movingtime As TimeSpan = New TimeSpan(0, 0, 0)

            Dim datavalues() As Double = {}
            Dim labelsvalues() As String = {}
            Dim colorsvalues() As Integer = {}

            Dim lon() As Double = {}
            Dim lat() As Double = {}

            Dim plon As Double
            Dim plat As Double

            Dim imagestatus As String = "no"
            Dim prevstatus As String = "stop"
            Dim prevtime As DateTime = DateTime.Parse(begindatetime)

            Dim currentstatus As String = "stop"
            Dim currenttime As DateTime = DateTime.Parse(begindatetime)

            Dim temptime As TimeSpan = New TimeSpan(0, 0, 0)
            Dim idling As TimeSpan = New TimeSpan(0, SecurityHelper.ValidateNumeric(ddlidling.SelectedValue, 1, 1440), 0)

            Dim dt As DataTable = SecurityHelper.ExecuteSecureQuery(query, param)
            Dim i As Integer = 0
            Dim enter As String = "no"

            If dt.Rows.Count > 0 Then
                For Each dr As DataRow In dt.Rows
                    imagestatus = "yes"

                    If enter = "no" Then
                        enter = "Yes"
                    End If
                    
                    currenttime = dr("datetime")
                    If dr("ignition") = True And dr("speed") <> 0 Then
                        currentstatus = "moving"
                    ElseIf dr("ignition") = True And dr("speed") = "0" Then
                        currentstatus = "idle"
                    Else
                        currentstatus = "stop"
                    End If

                    If prevstatus <> currentstatus Then
                        temptime = currenttime - prevtime
                        Dim minutes As Int16 = temptime.TotalMinutes()

                        Select Case prevstatus
                            Case "stop"
                                ' No action needed
                            Case "moving"
                                ' No action needed
                            Case "idle"
                                If temptime > New TimeSpan(0, 0, 0) Then
                                    If cbxidling.Checked = True Then
                                        If temptime > idling Then
                                            ReDim Preserve datavalues(i)
                                            ReDim Preserve labelsvalues(i)
                                            ReDim Preserve colorsvalues(i)
                                            ReDim Preserve lon(i)
                                            ReDim Preserve lat(i)

                                            datavalues(i) = Math.Round(temptime.TotalMinutes, 2)
                                            labelsvalues(i) = prevtime.ToString("yyyy/MM/dd HH:mm:ss") & " To " & currenttime.ToString("yyyy/MM/dd HH:mm:ss")
                                            colorsvalues(i) = &HBB0000
                                            
                                            ' SECURITY FIX: Validate coordinates
                                            If SecurityHelper.ValidateCoordinate(dr("lat").ToString(), dr("lon").ToString()) Then
                                                lon(i) = Math.Round(dr("lon"), 6)
                                                lat(i) = Math.Round(dr("lat"), 6)
                                            Else
                                                lon(i) = 0
                                                lat(i) = 0
                                            End If

                                            i += 1
                                        End If
                                    Else
                                        ReDim Preserve datavalues(i)
                                        ReDim Preserve labelsvalues(i)
                                        ReDim Preserve colorsvalues(i)
                                        ReDim Preserve lon(i)
                                        ReDim Preserve lat(i)
                                        
                                        datavalues(i) = Math.Round(temptime.TotalMinutes, 2)
                                        labelsvalues(i) = prevtime.ToString("yyyy/MM/dd HH:mm:ss") & " To " & currenttime.ToString("yyyy/MM/dd HH:mm:ss")
                                        
                                        If temptime > idling Then
                                            colorsvalues(i) = &H638BBC
                                        Else
                                            colorsvalues(i) = &HFF
                                        End If
                                        
                                        ' SECURITY FIX: Validate coordinates
                                        If SecurityHelper.ValidateCoordinate(dr("lat").ToString(), dr("lon").ToString()) Then
                                            lon(i) = Math.Round(dr("lon"), 6)
                                            lat(i) = Math.Round(dr("lat"), 6)
                                        Else
                                            lon(i) = 0
                                            lat(i) = 0
                                        End If

                                        i += 1
                                    End If
                                End If
                                
                                ' SECURITY FIX: Validate coordinates before assignment
                                If SecurityHelper.ValidateCoordinate(dr("lat").ToString(), dr("lon").ToString()) Then
                                    plon = Math.Round(dr("lon"), 6)
                                    plat = Math.Round(dr("lat"), 6)
                                End If
                        End Select

                        prevtime = currenttime
                        prevstatus = currentstatus
                    End If
                Next
            End If

            ' Handle final record
            If prevtime <> currenttime Then
                temptime = currenttime - prevtime
                Dim minutes As Int16 = temptime.TotalMinutes()

                Select Case prevstatus
                    Case "stop"
                        ' No action needed
                    Case "moving"
                        ' No action needed
                    Case "idle"
                        If temptime > New TimeSpan(0, 0, 0) Then
                            If cbxidling.Checked = True Then
                                If temptime > idling Then
                                    ReDim Preserve datavalues(i)
                                    ReDim Preserve labelsvalues(i)
                                    ReDim Preserve colorsvalues(i)
                                    ReDim Preserve lon(i)
                                    ReDim Preserve lat(i)

                                    datavalues(i) = Math.Round(temptime.TotalMinutes, 2)
                                    labelsvalues(i) = prevtime.ToString("yyyy/MM/dd HH:mm:ss") & " To " & currenttime.ToString("yyyy/MM/dd HH:mm:ss")
                                    colorsvalues(i) = &HBB0000
                                    lon(i) = Math.Round(plon, 6)
                                    lat(i) = Math.Round(plat, 6)
                                End If
                            Else
                                ReDim Preserve datavalues(i)
                                ReDim Preserve labelsvalues(i)
                                ReDim Preserve colorsvalues(i)
                                ReDim Preserve lon(i)
                                ReDim Preserve lat(i)

                                datavalues(i) = Math.Round(temptime.TotalMinutes, 2)
                                labelsvalues(i) = prevtime.ToString("yyyy/MM/dd HH:mm:ss") & " To " & currenttime.ToString("yyyy/MM/dd HH:mm:ss")
                                
                                If temptime > idling Then
                                    colorsvalues(i) = &H638BBC
                                Else
                                    colorsvalues(i) = &HFF
                                End If
                                
                                lon(i) = Math.Round(plon, 6)
                                lat(i) = Math.Round(plat, 6)
                            End If
                        End If
                End Select

                prevtime = currenttime
                prevstatus = currentstatus
            End If

            If imagestatus = "no" Then
                WebChartViewer1.Visible = False
                Image1.Visible = True
                Image1.ImageUrl = "~/images/NoDataWide.jpg"
                Return
            End If

            Array.Reverse(datavalues)
            Array.Reverse(labelsvalues)
            Array.Reverse(colorsvalues)

            Dim chight As Int64 = datavalues.Length * 30

            'Create a XYChart object
            Dim c As XYChart = New XYChart(730, chight + 100, &HFFFFFF, 0, 0)

            'Add a title to the chart
            c.addTitle(SecurityHelper.HtmlEncode(plateno) & " Idling Chart", "Verdana", 10)

            c.setPlotArea(290, 50, 410, chight)

            'Add a bar chart layer
            Dim layer As BarLayer = c.addBarLayer3(datavalues, colorsvalues)

            layer.set3D(6)
            'Swap the axis so that the bars are drawn horizontally
            c.swapXY(True)

            'Set the bar gap
            layer.setBarGap(0.3)

            'Use the format for the bar label
            layer.setAggregateLabelFormat(" {value}")

            'Set the bar label font
            layer.setAggregateLabelStyle("Verdana", 8)

            'Set the labels on the x axis
            Dim textbox As ChartDirector.TextBox = c.xAxis().setLabels(labelsvalues)

            'Set the x axis label font
            textbox.setFontStyle("Verdana")
            textbox.setFontSize(8)

            'Add titles to the axes
            c.xAxis().setTitle("Date Time")
            c.yAxis().setTitle("Time (Minutes)")

            'Output the chart
            WebChartViewer1.Image = c.makeWebImage(Chart.PNG)

            'Client side Javascript for tooltip
            Dim toolTip As String = "title='Date Time : {xLabel}  " & Environment.NewLine & "Minutes : {value} Minutes'"
            'Include tool tip for the chart
            WebChartViewer1.ImageMap = c.getHTMLImageMap("", "", toolTip)

            Dim popUp As String = ""
            Dim semi As String = ""
            For i = 0 To UBound(datavalues)
                If i <> 0 Then
                    semi = ";"
                End If
                popUp = popUp & semi & lon(i) & "," & lat(i)
            Next
            
            stringvalue.Value = popUp

            If imagestatus = "yes" Then
                Image1.Visible = False
                WebChartViewer1.Visible = True
                ec = "true"
                Session("Chart") = c.makeChart2(0)
            End If

        Catch ex As Exception
            SecurityHelper.LogError("DisplayChart Error", ex, Server)
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

End Class