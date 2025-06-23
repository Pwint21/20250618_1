Imports System.Data.SqlClient
Partial Class GMapQuarry
    Inherits System.Web.UI.Page
    Public sb As StringBuilder
    Public plateno As String
    Public begindatetime As String
    Public enddatetime As String
    Public searchin As String
    Public puserid As String
    Public mapsettings As String = ""
    Public role As String
    Public jspage As String
    Public querystring As String = ""
    Public acode As String = "0"
    Public polygonid As String = ""
    Public scode As String = ""
    Public sf As String = ""
    Public markerlat As String = ""
    Public markerlon As String = ""
    Public reqfrom As String = ""
    Public mvals As String
    Public la As String = "N"
    Public ucheck As String = "False"

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            ucheck = "False"
            Dim userid As String = Request.Cookies("userinfo")("userid")
            If userid = "7030" Or userid = "7031" Or userid = "7032" Or userid = "7033" Or userid = "1933" Or userid = "6779" Or userid = "6835" Or userid = "1618" Or userid = "1911" Or userid = "6826" Then
                ucheck = "True"
            End If
        Catch ex As Exception

        End Try

        MyBase.OnInit(e)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ucheck = "False"
            reqfrom = Request.QueryString("from")
            If reqfrom = "Client" Then
                If Request.Cookies("Auserinfo") Is Nothing Then
                    Server.Transfer("Login.aspx")
                Else
                    puserid = Request.Cookies("Auserinfo")("userid")
                    sb = New StringBuilder()
                    mvals = Request.QueryString("plateno") & "," & Request.QueryString("uid") & "," & Request.QueryString("tr") & "," & Request.QueryString("sr") & "," & Request.QueryString("dn") & "," & Request.QueryString("wo") & "," & Request.QueryString("sc") & "," & Request.QueryString("sn") & "," & Request.QueryString("ata") & "," & Request.QueryString("distance")
                    markerlat = Request.QueryString("markerlat")
                    markerlon = Request.QueryString("markerlon")
                    querystring = Request.QueryString("qs")
                    acode = Request.QueryString("acode")
                    polygonid = Request.QueryString("id")
                    Session("MapRefresh") = "Y"
                    plateno = Request.QueryString("plateno")
                    begindatetime = Request.QueryString("bdt")
                    enddatetime = Request.QueryString("edt")
                    searchin = Request.QueryString("si")
                    scode = Request.QueryString("scode")
                    sf = Request.QueryString("sf")
                    begindate1.Value = Now.ToString("yyyy/MM/dd")
                    enddate1.Value = Now.ToString("yyyy/MM/dd")
                    reqfrom = Request.QueryString("from")
                    Dim userslist As String = Request.Cookies("userinfo")("userslist")
                    If sf = "1" Then
                        sf = "1"
                        begindatetime = Convert.ToDateTime(enddatetime).AddDays(-1).ToString("yyyy/MM/dd HH:mm:ss")
                    Else
                        sf = ""
                    End If

                    Dim userid As String = Request.Cookies("userinfo")("userid")
                    If userid = "7030" Or userid = "7031" Or userid = "7032" Or userid = "7033" Or userid = "1933" Or userid = "6779" Or userid = "6835" Or userid = "1618" Or userid = "1911" Or userid = "6826" Then
                        ucheck = "True"
                    End If
                    role = Request.Cookies("userinfo")("role")
                    jspage = userid
                    la = Request.Cookies("userinfo")("LA")
                    ' la = "'" & la & "'"
                    If Session("mapsettings") Is Nothing Or Session("mapsettings") = 0 Then
                        Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                        Dim cmd As SqlCommand = New SqlCommand("select * from map_settings where userid='" & userid & "'", conn)
                        Try
                            conn.Open()
                            Dim dr As SqlDataReader = cmd.ExecuteReader()
                            If dr.Read() Then
                                mapsettings = dr("zoomlevel") & "," & dr("lat") & "," & dr("lon")
                            End If
                        Catch ex As Exception
                        Finally
                            conn.Close()
                        End Try
                    Else
                        Select Case Session("mapsettings")
                            Case "1"
                                mapsettings = "12,5.808334,102.146759"
                            Case "2"
                                mapsettings = "12,3.117371,101.6833"
                            Case "3"
                                mapsettings = "9,3.504639,102.639771"
                            Case "4"
                                mapsettings = "9,3.117371,101.6833"
                            Case "5"
                                mapsettings = "12,5.324249,103.141022"
                            Case Else
                                Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                                Dim cmd As SqlCommand = New SqlCommand("select * from map_settings where userid='" & userid & "'", conn)
                                Try
                                    conn.Open()
                                    Dim dr As SqlDataReader = cmd.ExecuteReader()
                                    If dr.Read() Then
                                        mapsettings = dr("zoomlevel") & "," & dr("lat") & "," & dr("lon")
                                    End If
                                Catch ex As Exception
                                Finally
                                    conn.Close()
                                End Try
                        End Select
                    End If

                    Dim conn1 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                    Dim ds As New DataSet
                    Dim da As SqlDataAdapter
                    If role = "User" Then
                        da = New SqlDataAdapter("select userid,username from userTBL where userid='" & userid & "' order by username", conn1)
                    ElseIf role = "SuperUser" Or role = "Operator" Then
                        da = New SqlDataAdapter("select userid,username from userTBL where  userid in(" & userslist & ") order by username", conn1)
                    Else
                        da = New SqlDataAdapter("select userid,username from userTBL where role='User' order by username", conn1)
                    End If
                    da.Fill(ds)
                    sb.Append("<select style=""width:238px;"" id=""ddlplateno""  runat=""server"" data-placeholder=""Select Plate Number"" style=""width:350px;"" class=""chzn-select"" tabindex=""5"">")
                    sb.Append("<option value=""""></option>")

                    Dim vcmd As SqlCommand
                    Dim i As Integer
                    For i = 0 To ds.Tables(0).Rows.Count - 1
                        vcmd = New SqlCommand("select plateno from vehicleTBL where userid='" & ds.Tables(0).Rows(i)(0) & "' order by plateno", conn1)
                        conn1.Open()
                        sb.Append("<optgroup label=""" & ds.Tables(0).Rows(i)(1).ToString().ToUpper() & """>")
                        Dim dr As SqlDataReader = vcmd.ExecuteReader()
                        While dr.Read()
                            sb.Append("<option value=""" & dr("plateno") & """>" & dr("plateno") & "</option>")
                        End While
                        sb.Append("</optgroup>")
                        conn1.Close()
                        dr.Close()
                    Next
                    sb.Append("</select>")



                End If
            Else
                If Request.Cookies("userinfo") Is Nothing Then
                    Server.Transfer("Login.aspx")
                Else
                    sb = New StringBuilder()
                    puserid = Request.Cookies("userinfo")("userid")
                    mvals = Request.QueryString("plateno") & "," & Request.QueryString("uid") & "," & Request.QueryString("tr") & "," & Request.QueryString("sr") & "," & Request.QueryString("dn") & "," & Request.QueryString("wo") & "," & Request.QueryString("sc") & "," & Request.QueryString("sn") & "," & Request.QueryString("ata") & "," & Request.QueryString("distance")
                    markerlat = Request.QueryString("markerlat")
                    markerlon = Request.QueryString("markerlon")
                    querystring = Request.QueryString("qs")
                    acode = Request.QueryString("acode")
                    polygonid = Request.QueryString("id")
                    Session("MapRefresh") = "Y"
                    plateno = Request.QueryString("plateno")
                    begindatetime = Request.QueryString("bdt")
                    enddatetime = Request.QueryString("edt")
                    searchin = Request.QueryString("si")
                    scode = Request.QueryString("scode")
                    sf = Request.QueryString("sf")
                    begindate1.Value = Now.ToString("yyyy/MM/dd")
                    enddate1.Value = Now.ToString("yyyy/MM/dd")
                    reqfrom = Request.QueryString("from")
                    Dim userslist As String = Request.Cookies("userinfo")("userslist")
                    If sf = "1" Then
                        sf = "1"
                        begindatetime = Convert.ToDateTime(enddatetime).AddDays(-1).ToString("yyyy/MM/dd HH:mm:ss")
                    Else
                        sf = ""
                    End If

                    Dim userid As String = Request.Cookies("userinfo")("userid")
                    If userid = "7030" Or userid = "7031" Or userid = "7032" Or userid = "7033" Or userid = "1933" Or userid = "6779" Or userid = "6835" Or userid = "1618" Or userid = "1911" Or userid = "6826" Then
                        ucheck = "True"
                    End If
                    role = Request.Cookies("userinfo")("role")
                    jspage = userid
                    la = Request.Cookies("userinfo")("LA")
                    ' la = "'" & la & "'"
                    If Session("mapsettings") Is Nothing Or Session("mapsettings") = 0 Then
                        Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                        Dim cmd As SqlCommand = New SqlCommand("select * from map_settings where userid='" & userid & "'", conn)
                        Try
                            conn.Open()
                            Dim dr As SqlDataReader = cmd.ExecuteReader()
                            If dr.Read() Then
                                mapsettings = dr("zoomlevel") & "," & dr("lat") & "," & dr("lon")
                            End If
                        Catch ex As Exception
                        Finally
                            conn.Close()
                        End Try
                    Else
                        Select Case Session("mapsettings")
                            Case "1"
                                mapsettings = "12,5.808334,102.146759"
                            Case "2"
                                mapsettings = "12,3.117371,101.6833"
                            Case "3"
                                mapsettings = "9,3.504639,102.639771"
                            Case "4"
                                mapsettings = "9,3.117371,101.6833"
                            Case "5"
                                mapsettings = "12,5.324249,103.141022"
                            Case Else
                                Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                                Dim cmd As SqlCommand = New SqlCommand("select * from map_settings where userid='" & userid & "'", conn)
                                Try
                                    conn.Open()
                                    Dim dr As SqlDataReader = cmd.ExecuteReader()
                                    If dr.Read() Then
                                        mapsettings = dr("zoomlevel") & "," & dr("lat") & "," & dr("lon")
                                    End If
                                Catch ex As Exception
                                Finally
                                    conn.Close()
                                End Try
                        End Select
                    End If

                    Dim conn1 As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                    Dim ds As New DataSet
                    Dim da As SqlDataAdapter
                    If role = "User" Then
                        da = New SqlDataAdapter("select userid,username from userTBL where userid='" & userid & "' order by username", conn1)
                    ElseIf role = "SuperUser" Or role = "Operator" Then
                        da = New SqlDataAdapter("select userid,username from userTBL where  userid in(" & userslist & ") order by username", conn1)
                    Else
                        da = New SqlDataAdapter("select userid,username from userTBL where role='User' order by username", conn1)
                    End If
                    da.Fill(ds)
                    sb.Append("<select style=""width:238px;"" id=""ddlplateno""  runat=""server"" data-placeholder=""Select Plate Number"" style=""width:350px;"" class=""chzn-select"" tabindex=""5"">")
                    sb.Append("<option value=""""></option>")

                    Dim vcmd As SqlCommand
                    Dim i As Integer
                    For i = 0 To ds.Tables(0).Rows.Count - 1
                        vcmd = New SqlCommand("select plateno from vehicleTBL where userid='" & ds.Tables(0).Rows(i)(0) & "' order by plateno", conn1)
                        conn1.Open()
                        sb.Append("<optgroup label=""" & ds.Tables(0).Rows(i)(1).ToString().ToUpper() & """>")
                        Dim dr As SqlDataReader = vcmd.ExecuteReader()
                        While dr.Read()
                            sb.Append("<option value=""" & dr("plateno") & """>" & dr("plateno") & "</option>")
                        End While
                        sb.Append("</optgroup>")
                        conn1.Close()
                        dr.Close()
                    Next
                    sb.Append("</select>")



                End If
            End If


        Catch ex As Exception
        End Try
    End Sub
End Class
