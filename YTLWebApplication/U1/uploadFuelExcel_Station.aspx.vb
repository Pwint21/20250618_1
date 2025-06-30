Imports System.Data
Imports System.Data.OleDb
Imports System.Data.SqlClient

Partial Class uploadFuelExcel_Station
    Inherits System.Web.UI.Page

    Public s_matching As String = ""
    Public s_excel As String = ""
    Public s_record As String = ""
    Public s_insert As String = ""
    Public s_total As String = ""
    Public s_error As String = ""

    Protected Sub btnUpload_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpload.Click

        Dim oWatch As New System.Diagnostics.Stopwatch
        oWatch.Start()


        Try
            If Not (MyUpload.PostedFile Is Nothing) And (MyUpload.PostedFile.ContentLength > 0) Then
                Dim ex1 As Exception
                Dim nMaxFileSize As Int32
                nMaxFileSize = 20000 * 1024
                If (ddlusername.SelectedValue = "--Select User Name--") Then
                    ex1 = New Exception("Please select User Name")
                    Throw ex1
                End If

                If (MyUpload.PostedFile.ContentLength > nMaxFileSize) Then
                    ex1 = New Exception("Only file size up to 20MB can be uploaded")
                    Throw ex1
                End If

                Dim strExt As String = System.IO.Path.GetExtension(MyUpload.PostedFile.FileName).ToLower

                If (strExt = ".xls") Or (strExt = ".csv") Or (strExt = ".xlsx") Then
                    'do nothing
                Else
                    ex1 = New Exception("Only Excel file (*.xls) is allowed")
                    Throw ex1
                End If

                Dim strFileName As String
                Dim strPath As String
                Dim userid As String = ddlusername.SelectedValue
                Dim username As String = ddlusername.SelectedItem.Text

                strPath = Server.MapPath("fuelexcel\")
                strFileName = username & "_" & DateTime.Now.ToString("yyyyMMdd_HHmmss") & ".xls"

                TextBox1.Text = strPath & strFileName
                TextBox2.Text = userid

                MyUpload.PostedFile.SaveAs(strPath & strFileName)
                'excel2SQL(strPath & strFileName, userid)

                'excel2SQLRowSet(strPath & strFileName, userid)
            Else
                s_error = "No file selected to upload"
            End If
        Catch ex As Exception
            s_error = "File Error: " & ex.Message
            'Exit Sub
        End Try

        'oWatch.Stop()
        's_error = CDbl(oWatch.ElapsedMilliseconds.ToString()) / 1000 & " secs"

        'oWatch.Reset()
        'oWatch.Start()

        If radiostation1.Checked Then
            excel2SQLRowShell(TextBox1.Text, TextBox2.Text)
        ElseIf radiostation2.Checked Then
            excel2SQLRowPetron(TextBox1.Text, TextBox2.Text)
        ElseIf radiostation3.Checked Then
            excel2SQLRowSet(TextBox1.Text, TextBox2.Text)
        Else
            Response.Write("none")
        End If


        oWatch.Stop()
        s_total = " Time Spend " & CDbl(oWatch.ElapsedMilliseconds.ToString()) / 1000 & " seconds"

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            'Response.Cookies("userinfo")("userid") = "1697"
            'Response.Cookies("userinfo")("role") = "SuperUser"
            'Response.Cookies("userinfo")("userslist") = "1769,1771,1768,1770,1773,1774,1772,1775,1710,1706,1750,1705,1751,1756,1762,1725,1755,1752,1761,1758,1733,1764,1759,1753,1709,1707,1708,1711,1757,1754,1763,1749,1704,1760,1703,1726,1778,1779,1780"

            If Page.IsPostBack = False Then
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As SqlCommand
                Dim dr As SqlDataReader
                Dim strSQL As String

                If Request.Cookies("userinfo")("role") = "User" Then
                    strSQL = "SELECT userid, username,dbip FROM userTBL WHERE userid='" & Request.Cookies("userinfo")("userid") & "' ORDER BY username"
                ElseIf Request.Cookies("userinfo")("role") = "Operator" Or Request.Cookies("userinfo")("role") = "SuperUser" Then
                    strSQL = "SELECT userid, username,dbip FROM userTBL WHERE userid IN (" & Request.Cookies("userinfo")("userslist") & ") ORDER BY username"
                Else
                    strSQL = "SELECT userid, username,dbip FROM userTBL WHERE role='User' ORDER BY username"
                End If

                conn.Open()
                cmd = New SqlCommand(strSQL, conn)
                dr = cmd.ExecuteReader()
                While dr.Read()
                    ddlusername.Items.Add(New ListItem(dr("username"), dr("userid")))
                End While
                dr.Close()
                conn.Close()

                If ddlusername.Items.Count = 1 Then

                End If

            End If

        Catch ex As SystemException
            Response.Write(ex.Message)
        End Try
    End Sub


    Sub excel2SQLRowSet(ByVal strExcelFile As String, ByVal strUserID As String)
        Try

            Dim r As DataRow

            Dim t As New DataTable
            t.Columns.Add(New DataColumn("userid"))
            t.Columns.Add(New DataColumn("plateno"))
            t.Columns.Add(New DataColumn("timestamp"))
            t.Columns.Add(New DataColumn("stationcode"))
            t.Columns.Add(New DataColumn("fueltype"))
            t.Columns.Add(New DataColumn("liters"))
            t.Columns.Add(New DataColumn("cost"))
            t.Columns.Add(New DataColumn("insertdate"))
            t.Columns.Add(New DataColumn("insertby"))
            t.Columns.Add(New DataColumn("insertmethod"))

            t.Columns(2).DataType = GetType(DateTime)
            t.Columns("liters").DataType = GetType(Double)
            t.Columns("cost").DataType = GetType(Double)

            Dim t2 As New DataTable
            t2.Columns.Add(New DataColumn("userid"))
            t2.Columns.Add(New DataColumn("plateno"))
            t2.Columns.Add(New DataColumn("timestamp"))
            t2.Columns.Add(New DataColumn("stationcode"))
            t2.Columns.Add(New DataColumn("fueltype"))
            t2.Columns.Add(New DataColumn("liters"))
            t2.Columns.Add(New DataColumn("cost"))
            t2.Columns.Add(New DataColumn("insertdate"))
            t2.Columns.Add(New DataColumn("insertby"))
            t2.Columns.Add(New DataColumn("insertmethod"))

            t2.Columns(2).DataType = GetType(DateTime)
            t2.Columns("liters").DataType = GetType(Double)
            t2.Columns("cost").DataType = GetType(Double)

            If (strExcelFile = "") Or (strUserID = "") Then
                Exit Sub
            End If
            Dim dtTimer As New DateTime()

            Dim conn As SqlConnection
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim connLocal As SqlConnection
            connLocal = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnectionLocalDB"))

            Dim da As SqlDataAdapter
            Dim ds As New Data.DataSet

            Dim arrSheet As String()
            arrSheet = getSheetList(strExcelFile)

            da = New SqlDataAdapter("SELECT * FROM OPENROWSET('Microsoft.ACE.OLEDB.12.0', 'Excel 12.0;Database=" & strExcelFile & "; HDR=YES','select timestamp, replace(plateno,'' '','''') as plateno, liters, stationcode,cost,fueltype from " & arrSheet(0) & "') as f", connLocal)
            da.Fill(ds)

            Dim daVehicle As SqlDataAdapter
            Dim dsVehicle As New Data.DataSet

            'Dim daVehicleString As String
            'If Request.Cookies("userinfo")("role") = "User" Then
            '    daVehicleString = "SELECT plateno, userid from vehicleTBL where userid in (" & Request.Cookies("userinfo")("userid") & ") order by plateno"
            'ElseIf Request.Cookies("userinfo")("role") = "Operator" Or Request.Cookies("userinfo")("role") = "SuperUser" Then
            '    daVehicleString = "SELECT plateno, userid from vehicleTBL where userid in (" & Request.Cookies("userinfo")("userslist") & ") order by plateno"
            'Else
            '    daVehicleString = "SELECT plateno, userid from vehicleTBL where role='User' order by plateno"
            'End If

            daVehicle = New SqlDataAdapter("SELECT plateno, userid from vehicleTBL where userid ='" & ddlusername.SelectedValue & "' order by plateno", conn)
            daVehicle.Fill(dsVehicle)


            Dim dc As New System.Collections.Generic.Dictionary(Of String, String)
            For Each rt As DataRow In dsVehicle.Tables(0).Rows
                dc.Add(rt.Item(0).ToString(), "")
            Next


            Dim i As Int64 = 0
            Dim matched As Int64 = 0
            Dim founded As Int64 = 0
            Dim table1Count As Int64 = 0
            Dim table2Count As Int64 = 0

            Dim arrDate As String()
            Dim arrYMD As String()
            Dim strNewDate As String

            Dim PlateNoFound As Boolean

            For i = 0 To ds.Tables(0).Rows.Count - 1

                Dim foundRow() As DataRow
                foundRow = dsVehicle.Tables(0).Select("plateno like  '%" & ds.Tables(0).Rows(i)("plateno") & "%'")

                'response.write("plateNoFound=" & ds.Tables(0).Rows(i)("plateno") & " System=" & dsvehicle.tables(0).rows.count & " FoundRow=" & foundrow.length & "<br/>")

                'PlateNoFound = dc.ContainsKey(ds.Tables(0).Rows(i)("plateno"))

                'If PlateNoFound = True Then
                'founded += 1



                If foundRow.Length > 0 Then
                    matched += 1

                    r = t.NewRow
                    r(0) = foundRow(0)("userid")
                    r(1) = foundRow(0)("plateno")
                    r(2) = Convert.ToDateTime(ds.Tables(0).Rows(i)("timestamp")).ToString("yyyy/MM/dd HH:mm:ss")
                    r(3) = ds.Tables(0).Rows(i)("stationcode")
                    r(4) = ds.Tables(0).Rows(i)("fueltype")
                    r(5) = ds.Tables(0).Rows(i)("liters")
                    r(6) = Format(ds.Tables(0).Rows(i)("cost"), "##0.00")
                    r(7) = Convert.ToDateTime(DateTime.Now).ToString("yyyy/MM/dd HH:mm:ss")
                    r(8) = Request.Cookies("userinfo")("username")
                    r(9) = "bulk"
                    t.Rows.Add(r)

                    If Convert.ToDateTime(ds.Tables(0).Rows(i)("timestamp")).Month = ddlMonth.SelectedValue Then
                        table1Count += 1
                    End If


                    'add into another table

                    arrDate = Convert.ToDateTime(ds.Tables(0).Rows(i)("timestamp")).ToString("yyyy/MM/dd HH:mm:ss").Split(" ")

                    If arrDate(0).Contains("/") Then
                        arrYMD = arrDate(0).Split("/")
                    Else
                        arrYMD = arrDate(0).Split("-")
                    End If


                    strNewDate = arrYMD(2) & "-" & arrYMD(0) & "-" & arrYMD(1) & " " & arrDate(1)

                    If TestDateTime(strNewDate) = True Then

                        r = t2.NewRow
                        r(0) = foundRow(0)("userid")
                        r(1) = foundRow(0)("plateno")
                        r(2) = Convert.ToDateTime(strNewDate).ToString("yyyy/MM/dd HH:mm:ss")
                        r(3) = ds.Tables(0).Rows(i)("stationcode")
                        r(4) = ds.Tables(0).Rows(i)("fueltype")
                        r(5) = ds.Tables(0).Rows(i)("liters")
                        r(6) = Format(ds.Tables(0).Rows(i)("cost"), "##0.00")
                        r(7) = Convert.ToDateTime(DateTime.Now).ToString("yyyy/MM/dd HH:mm:ss")
                        r(8) = Request.Cookies("userinfo")("username")
                        r(9) = "bulk"
                        t2.Rows.Add(r)

                        If Convert.ToDateTime(strNewDate).Month = ddlMonth.SelectedValue Then
                            table2Count += 1
                        End If

                    End If

                End If
                'End If

            Next


            Dim sql_bulkCopy As New SqlBulkCopy(conn)

            conn.Open()

            sql_bulkCopy.ColumnMappings.Add("userid", "userid")
            sql_bulkCopy.ColumnMappings.Add("plateno", "plateno")
            sql_bulkCopy.ColumnMappings.Add("timestamp", "timestamp")
            sql_bulkCopy.ColumnMappings.Add("stationcode", "stationcode")
            sql_bulkCopy.ColumnMappings.Add("fueltype", "fueltype")
            sql_bulkCopy.ColumnMappings.Add("liters", "liters")
            sql_bulkCopy.ColumnMappings.Add("cost", "cost")
            sql_bulkCopy.ColumnMappings.Add("insertdate", "insertdate")
            sql_bulkCopy.ColumnMappings.Add("insertby", "insertby")
            sql_bulkCopy.ColumnMappings.Add("insertmethod", "insertmethod")

            sql_bulkCopy.DestinationTableName = "fuel"
            sql_bulkCopy.BulkCopyTimeout = 600
            sql_bulkCopy.BatchSize = 3000

            If table1Count >= table2Count Then
                sql_bulkCopy.WriteToServer(t) 'previously t
            Else
                sql_bulkCopy.WriteToServer(t2) 'previously t2
            End If

            sql_bulkCopy.Close()

            conn.Close()


            s_matching = "Processed " & matched & " records"
            s_record = "Found " & (i) & " Records"


        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub

    Sub excel2SQLRowPetron(ByVal strExcelFile As String, ByVal strUserID As String)
        Try

            Dim r As DataRow

            Dim t As New DataTable
            t.Columns.Add(New DataColumn("userid"))
            t.Columns.Add(New DataColumn("plateno"))
            t.Columns.Add(New DataColumn("timestamp"))
            t.Columns.Add(New DataColumn("stationcode"))
            t.Columns.Add(New DataColumn("fueltype"))
            t.Columns.Add(New DataColumn("liters"))
            t.Columns.Add(New DataColumn("cost"))
            t.Columns.Add(New DataColumn("insertdate"))
            t.Columns.Add(New DataColumn("insertby"))
            t.Columns.Add(New DataColumn("insertmethod"))

            t.Columns(2).DataType = GetType(DateTime)
            t.Columns("liters").DataType = GetType(Double)
            t.Columns("cost").DataType = GetType(Double)

            Dim t2 As New DataTable
            t2.Columns.Add(New DataColumn("userid"))
            t2.Columns.Add(New DataColumn("plateno"))
            t2.Columns.Add(New DataColumn("timestamp"))
            t2.Columns.Add(New DataColumn("stationcode"))
            t2.Columns.Add(New DataColumn("fueltype"))
            t2.Columns.Add(New DataColumn("liters"))
            t2.Columns.Add(New DataColumn("cost"))
            t2.Columns.Add(New DataColumn("insertdate"))
            t2.Columns.Add(New DataColumn("insertby"))
            t2.Columns.Add(New DataColumn("insertmethod"))

            t2.Columns(2).DataType = GetType(DateTime)
            t2.Columns("liters").DataType = GetType(Double)
            t2.Columns("cost").DataType = GetType(Double)

            If (strExcelFile = "") Or (strUserID = "") Then
                Exit Sub
            End If
            Dim dtTimer As New DateTime()

            Dim conn As SqlConnection
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim connLocal As SqlConnection
            connLocal = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnectionLocalDB"))

            Dim da As SqlDataAdapter
            Dim ds As New Data.DataSet

            Dim arrSheet As String()
            arrSheet = getSheetList(strExcelFile)

            da = New SqlDataAdapter("SELECT * FROM OPENROWSET('Microsoft.ACE.OLEDB.12.0', 'Excel 12.0;Database=" & strExcelFile & "; HDR=YES','select * from " & arrSheet(0) & "') as f", connLocal)
            da.Fill(ds)

            Dim daVehicle As SqlDataAdapter
            Dim dsVehicle As New Data.DataSet

            'Dim daVehicleString As String
            'If Request.Cookies("userinfo")("role") = "User" Then
            '    daVehicleString = "SELECT plateno, userid from vehicleTBL where userid in (" & Request.Cookies("userinfo")("userid") & ") order by plateno"
            'ElseIf Request.Cookies("userinfo")("role") = "Operator" Or Request.Cookies("userinfo")("role") = "SuperUser" Then
            '    daVehicleString = "SELECT plateno, userid from vehicleTBL where userid in (" & Request.Cookies("userinfo")("userslist") & ") order by plateno"
            'Else
            '    daVehicleString = "SELECT plateno, userid from vehicleTBL where role='User' order by plateno"
            'End If

            daVehicle = New SqlDataAdapter("SELECT plateno, userid from vehicleTBL where userid ='" & ddlusername.SelectedValue & "' order by plateno", conn)
            daVehicle.Fill(dsVehicle)


            Dim dc As New System.Collections.Generic.Dictionary(Of String, String)
            For Each rt As DataRow In dsVehicle.Tables(0).Rows
                dc.Add(rt.Item(0).ToString(), "")
            Next


            Dim i As Int64 = 0
            Dim matched As Int64 = 0
            Dim founded As Int64 = 0
            Dim table1Count As Int64 = 0
            Dim table2Count As Int64 = 0

            Dim arrDate As String()
            Dim arrYMD As String()
            Dim strNewDate As String

            Dim PlateNoFound As Boolean

            For i = 0 To ds.Tables(0).Rows.Count - 1

                Dim foundRow() As DataRow
                foundRow = dsVehicle.Tables(0).Select("plateno like  '%" & CStr(ds.Tables(0).Rows(i)("Vehicle No")).Replace(" ", "") & "%'")

                'response.write("plateNoFound=" & ds.Tables(0).Rows(i)("plateno") & " System=" & dsvehicle.tables(0).rows.count & " FoundRow=" & foundrow.length & "<br/>")

                'PlateNoFound = dc.ContainsKey(ds.Tables(0).Rows(i)("plateno"))

                'If PlateNoFound = True Then
                'founded += 1



                If foundRow.Length > 0 Then
                    matched += 1

                    Try

                        If Convert.ToDateTime(ds.Tables(0).Rows(i)("Transaction Date")).Month = ddlMonth.SelectedValue Then
                            r = t.NewRow
                            r(0) = foundRow(0)("userid")
                            r(1) = foundRow(0)("plateno")
                            'Response.Write("~~" & ds.Tables(0).Rows(i)("Transaction Date") & " " & ds.Tables(0).Rows(i)("Transaction Time") & "~~")
                            r(2) = Convert.ToDateTime(ds.Tables(0).Rows(i)("Transaction Date") & " " & ds.Tables(0).Rows(i)("Transaction Time") & ":00").ToString("yyyy/MM/dd HH:mm:ss")
                            r(3) = ds.Tables(0).Rows(i)("Station Name")
                            r(4) = ds.Tables(0).Rows(i)("Product")
                            r(5) = ds.Tables(0).Rows(i)("Quantity")
                            r(6) = Format(ds.Tables(0).Rows(i)("Transaction Amount"), "##0.00")
                            r(7) = Convert.ToDateTime(DateTime.Now).ToString("yyyy/MM/dd HH:mm:ss")
                            r(8) = Request.Cookies("userinfo")("username")
                            r(9) = "bulk petron"
                            t.Rows.Add(r)
                            table1Count += 1
                            'Response.Write("t_datetime=" & r(2) & ", MonthCount=" & Convert.ToDateTime(ds.Tables(0).Rows(i)("Transaction Date")).Month & "<br/>")
                        End If

                    Catch ex As Exception
                        'Response.Write("@" & ex.message & "<br/>")
                    End Try
                    'add into another table
                    'Response.Write("Q")
                    Try

                        ''arrDate = ds.Tables(0).Rows(i)("Transaction Date").split("/")
                        ' ''arrDate = Convert.ToDateTime(ds.Tables(0).Rows(i)("Transaction Date") & " " & ds.Tables(0).Rows(i)("Transaction Time")).ToString("yyyy/MM/dd HH:mm:ss").Split(" ")
                        ' ''Response.Write("arrDate=" & Convert.ToDateTime(ds.Tables(0).Rows(i)("Transaction Date") & " " & ds.Tables(0).Rows(i)("Transaction Time")).ToString("yyyy/MM/dd HH:mm:ss") & "<br/>")
                        ''If arrDate(0).Contains("/") Then
                        ''    arrYMD = arrDate(0).Split("/")
                        ''Else
                        ''    arrYMD = arrDate(0).Split("-")
                        ''End If

                        arrYMD = ds.Tables(0).Rows(i)("Transaction Date").split("/")

                        strNewDate = arrYMD(2) & "-" & arrYMD(1) & "-" & arrYMD(0) & " " & ds.Tables(0).Rows(i)("Transaction Time")

                    Catch ex As Exception
                        Response.Write("#" & ex.Message & "<br/>")
                    End Try

                    If TestDateTime(strNewDate) = True Then

                        If Convert.ToDateTime(strNewDate).Month = ddlMonth.SelectedValue Then
                            r = t2.NewRow
                            r(0) = foundRow(0)("userid")
                            r(1) = foundRow(0)("plateno")
                            r(2) = Convert.ToDateTime(strNewDate).ToString("yyyy/MM/dd HH:mm:ss")
                            r(3) = ds.Tables(0).Rows(i)("Station Name")
                            r(4) = ds.Tables(0).Rows(i)("Product")
                            r(5) = ds.Tables(0).Rows(i)("Quantity")
                            r(6) = Format(ds.Tables(0).Rows(i)("Transaction Amount"), "##0.00")
                            r(7) = Convert.ToDateTime(DateTime.Now).ToString("yyyy/MM/dd HH:mm:ss")
                            r(8) = Request.Cookies("userinfo")("username")
                            r(9) = "bulk petron"
                            t2.Rows.Add(r)
                            'Response.Write("t2_datetime=" & r(2) & ", MonthCount=" & Convert.ToDateTime(strNewDate).Month & "<br/>")
                            table2Count += 1
                        End If

                    End If

                End If
                'End If

            Next


            Dim sql_bulkCopy As New SqlBulkCopy(conn)

            conn.Open()

            sql_bulkCopy.ColumnMappings.Add("userid", "userid")
            sql_bulkCopy.ColumnMappings.Add("plateno", "plateno")
            sql_bulkCopy.ColumnMappings.Add("timestamp", "timestamp")
            sql_bulkCopy.ColumnMappings.Add("stationcode", "stationcode")
            sql_bulkCopy.ColumnMappings.Add("fueltype", "fueltype")
            sql_bulkCopy.ColumnMappings.Add("liters", "liters")
            sql_bulkCopy.ColumnMappings.Add("cost", "cost")
            sql_bulkCopy.ColumnMappings.Add("insertdate", "insertdate")
            sql_bulkCopy.ColumnMappings.Add("insertby", "insertby")
            sql_bulkCopy.ColumnMappings.Add("insertmethod", "insertmethod")

            sql_bulkCopy.DestinationTableName = "fuel"
            sql_bulkCopy.BulkCopyTimeout = 600
            sql_bulkCopy.BatchSize = 3000

            'Response.Write("t=" & t.Rows.Count & ", t2=" & t2.Rows.Count)

            If table1Count >= table2Count Then
                sql_bulkCopy.WriteToServer(t) 'previously t
                s_matching = "Processed " & table1Count & " records"
                s_record = "Found " & (i) & " Records"
            Else
                sql_bulkCopy.WriteToServer(t2) 'previously t2
                s_matching = "Processed " & table2Count & " records"
                s_record = "Found " & (i) & " Records"
            End If

            sql_bulkCopy.Close()

            conn.Close()





        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub

    Sub excel2SQLRowShell(ByVal strExcelFile As String, ByVal strUserID As String)
        Try

            Dim r As DataRow


            Dim t As New DataTable
            t.Columns.Add(New DataColumn("userid"))
            t.Columns.Add(New DataColumn("plateno"))
            t.Columns.Add(New DataColumn("timestamp"))
            t.Columns.Add(New DataColumn("stationcode"))
            t.Columns.Add(New DataColumn("fueltype"))
            t.Columns.Add(New DataColumn("liters"))
            t.Columns.Add(New DataColumn("cost"))
            t.Columns.Add(New DataColumn("insertdate"))
            t.Columns.Add(New DataColumn("insertby"))
            t.Columns.Add(New DataColumn("insertmethod"))

            t.Columns(2).DataType = GetType(DateTime)
            t.Columns("liters").DataType = GetType(Double)
            t.Columns("cost").DataType = GetType(Double)

            Dim t2 As New DataTable
            t2.Columns.Add(New DataColumn("userid"))
            t2.Columns.Add(New DataColumn("plateno"))
            t2.Columns.Add(New DataColumn("timestamp"))
            t2.Columns.Add(New DataColumn("stationcode"))
            t2.Columns.Add(New DataColumn("fueltype"))
            t2.Columns.Add(New DataColumn("liters"))
            t2.Columns.Add(New DataColumn("cost"))
            t2.Columns.Add(New DataColumn("insertdate"))
            t2.Columns.Add(New DataColumn("insertby"))
            t2.Columns.Add(New DataColumn("insertmethod"))

            t2.Columns(2).DataType = GetType(DateTime)
            t2.Columns("liters").DataType = GetType(Double)
            t2.Columns("cost").DataType = GetType(Double)

            If (strExcelFile = "") Or (strUserID = "") Then
                Exit Sub
            End If
            Dim dtTimer As New DateTime()

            Dim conn As SqlConnection
            conn = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim connLocal As SqlConnection
            connLocal = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnectionLocalDB"))

            Dim arrSheet As String()
            arrSheet = getSheetList(strExcelFile)

            'Response.Write(arrSheet(0))

            Dim da As SqlDataAdapter
            Dim ds As New Data.DataSet


            da = New SqlDataAdapter("SELECT * FROM OPENROWSET('Microsoft.ACE.OLEDB.12.0', 'Excel 12.0;Database=" & strExcelFile & "; HDR=YES','select * from " & CStr(arrSheet(0)).Replace("$]", "$A13:AF]") & " where volume is not null') as f", connLocal)
            da.Fill(ds)

            Dim daVehicle As SqlDataAdapter
            Dim dsVehicle As New Data.DataSet

            'Dim daVehicleString As String
            'If Request.Cookies("userinfo")("role") = "User" Then
            '    daVehicleString = "SELECT plateno, userid from vehicleTBL where userid in (" & Request.Cookies("userinfo")("userid") & ") order by plateno"
            'ElseIf Request.Cookies("userinfo")("role") = "Operator" Or Request.Cookies("userinfo")("role") = "SuperUser" Then
            '    daVehicleString = "SELECT plateno, userid from vehicleTBL where userid in (" & Request.Cookies("userinfo")("userslist") & ") order by plateno"
            'Else
            '    daVehicleString = "SELECT plateno, userid from vehicleTBL where role='User' order by plateno"
            'End If

            daVehicle = New SqlDataAdapter("SELECT plateno, userid from vehicleTBL where userid ='" & ddlusername.SelectedValue & "' order by plateno", conn)
            daVehicle.Fill(dsVehicle)


            Dim dc As New System.Collections.Generic.Dictionary(Of String, String)
            For Each rt As DataRow In dsVehicle.Tables(0).Rows
                dc.Add(rt.Item(0).ToString(), "")
            Next


            Dim i As Int64 = 0
            Dim matched As Int64 = 0
            Dim founded As Int64 = 0
            Dim table1Count As Int64 = 0
            Dim table2Count As Int64 = 0

            Dim arrDate As String()
            Dim arrYMD As String()
            Dim strNewDate As String

            Dim PlateNoFound As Boolean

            For i = 0 To ds.Tables(0).Rows.Count - 1

                Dim foundRow() As DataRow

                Try
                    foundRow = dsVehicle.Tables(0).Select("plateno like  '%" & CStr(ds.Tables(0).Rows(i)("Licence Number")).Replace(" ", "") & "%'")
                Catch ex As Exception
                    Response.Write("@@@")
                End Try


                'response.write("plateNoFound=" & ds.Tables(0).Rows(i)("plateno") & " System=" & dsvehicle.tables(0).rows.count & " FoundRow=" & foundrow.length & "<br/>")

                'PlateNoFound = dc.ContainsKey(ds.Tables(0).Rows(i)("plateno"))

                'If PlateNoFound = True Then
                'founded += 1

                If foundRow.Length > 0 Then
                    matched += 1

                    Try



                        If Convert.ToDateTime(ds.Tables(0).Rows(i)("Transaction Date")).Month = ddlMonth.SelectedValue Then
                            r = t.NewRow
                            r(0) = foundRow(0)("userid")
                            r(1) = foundRow(0)("plateno")
                            r(2) = Convert.ToDateTime(ds.Tables(0).Rows(i)("Transaction Date") & " " & ds.Tables(0).Rows(i)("Transaction Time") & ":00").ToString("yyyy/MM/dd HH:mm:ss")
                            r(3) = ds.Tables(0).Rows(i)("Site")
                            r(4) = ds.Tables(0).Rows(i)("Product Name")
                            r(5) = ds.Tables(0).Rows(i)("Volume")
                            r(6) = Format(ds.Tables(0).Rows(i)("Net Amount (local currency)"), "##0.00")
                            r(7) = Convert.ToDateTime(DateTime.Now).ToString("yyyy/MM/dd HH:mm:ss")
                            r(8) = Request.Cookies("userinfo")("username")
                            r(9) = "bulk Shell"
                            t.Rows.Add(r)
                            table1Count += 1
                        End If

                    Catch ex As Exception
                        'Response.Write("@" & ex.message & "<br/>")
                    End Try
                    'add into another table
                    'Response.Write("Q")
                    Try

                        ''arrDate = ds.Tables(0).Rows(i)("Transaction Date").split("/")
                        ' ''arrDate = Convert.ToDateTime(ds.Tables(0).Rows(i)("Transaction Date") & " " & ds.Tables(0).Rows(i)("Transaction Time")).ToString("yyyy/MM/dd HH:mm:ss").Split(" ")
                        ' ''Response.Write("arrDate=" & Convert.ToDateTime(ds.Tables(0).Rows(i)("Transaction Date") & " " & ds.Tables(0).Rows(i)("Transaction Time")).ToString("yyyy/MM/dd HH:mm:ss") & "<br/>")
                        ''If arrDate(0).Contains("/") Then
                        ''    arrYMD = arrDate(0).Split("/")
                        ''Else
                        ''    arrYMD = arrDate(0).Split("-")
                        ''End If

                        arrYMD = ds.Tables(0).Rows(i)("Transaction Date").split("-")

                        strNewDate = arrYMD(2) & "-" & arrYMD(1) & "-" & arrYMD(0) & " " & ds.Tables(0).Rows(i)("Transaction Time")

                    Catch ex As Exception
                        Response.Write("#" & ex.Message & "<br/>")
                    End Try

                    Try

                        If TestDateTime(strNewDate) = True Then

                            If Convert.ToDateTime(strNewDate).Month = ddlMonth.SelectedValue Then
                                r = t2.NewRow
                                r(0) = foundRow(0)("userid")
                                r(1) = foundRow(0)("plateno")
                                r(2) = Convert.ToDateTime(strNewDate).ToString("yyyy/MM/dd HH:mm:ss")
                                r(3) = ds.Tables(0).Rows(i)("Site")
                                r(4) = CStr(ds.Tables(0).Rows(i)("Product Name")).Replace("FuelSave ", "")
                                r(5) = ds.Tables(0).Rows(i)("Volume")
                                r(6) = Format(CDbl(ds.Tables(0).Rows(i)("Net Amount (local currency)")), "##0.00")
                                r(7) = Convert.ToDateTime(DateTime.Now).ToString("yyyy/MM/dd HH:mm:ss")
                                r(8) = Request.Cookies("userinfo")("username")
                                r(9) = "bulk shell"
                                t2.Rows.Add(r)
                                table2Count += 1
                            End If

                        End If

                    Catch ex As Exception

                    End Try

                End If
                'End If

            Next

            Try

                Dim sql_bulkCopy As New SqlBulkCopy(conn)

                conn.Open()

                sql_bulkCopy.ColumnMappings.Add("userid", "userid")
                sql_bulkCopy.ColumnMappings.Add("plateno", "plateno")
                sql_bulkCopy.ColumnMappings.Add("timestamp", "timestamp")
                sql_bulkCopy.ColumnMappings.Add("stationcode", "stationcode")
                sql_bulkCopy.ColumnMappings.Add("fueltype", "fueltype")
                sql_bulkCopy.ColumnMappings.Add("liters", "liters")
                sql_bulkCopy.ColumnMappings.Add("cost", "cost")
                sql_bulkCopy.ColumnMappings.Add("insertdate", "insertdate")
                sql_bulkCopy.ColumnMappings.Add("insertby", "insertby")
                sql_bulkCopy.ColumnMappings.Add("insertmethod", "insertmethod")

                sql_bulkCopy.DestinationTableName = "fuel"
                sql_bulkCopy.BulkCopyTimeout = 600
                sql_bulkCopy.BatchSize = 3000

                If table1Count >= table2Count Then
                    sql_bulkCopy.WriteToServer(t) 'previously t
                    s_matching = "Processed " & table1Count & " records"
                    s_record = "Found " & (i) & " Records"

                Else
                    sql_bulkCopy.WriteToServer(t2) 'previously t2
                    s_matching = "Processed " & table2Count & " records"
                    s_record = "Found " & (i) & " Records"

                End If

                sql_bulkCopy.Close()

                conn.Close()


            Catch ex As Exception
                Response.Write("Insert=" & ex.Message)
            End Try




        Catch ex As Exception
            Response.Write("Last" & ex.Message)
        End Try
    End Sub


    'Sub excel2SQL(ByVal strExcelFile As String, ByVal strUserID As String)
    '    'On Error Resume Next
    '    Try
    '        If (strExcelFile = "") Or (strUserID = "") Then
    '            Exit Sub
    '        End If
    '        Dim dtTimer As New DateTime()
    '        'For OLE DB
    '        Dim strConn As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & strExcelFile & ";Extended Properties=Excel 8.0"
    '        Dim oleConn As OleDbConnection
    '        Dim oleCmd As OleDbCommand
    '        Dim oleDr As OleDbDataReader

    '        'For SQL DB
    '        'Dim objconn As New MyConn
    '        Dim connMaster As SqlConnection
    '        Dim connLocal As SqlConnection
    '        Dim cmd As SqlCommand
    '        Dim drSQL As SqlDataReader

    '        Dim arrSheet As String()
    '        Dim strSQL As String
    '        Dim strOut As String
    '        Dim strList As String = ""

    '        Dim strPlateNo As String
    '        Dim strPlateNew As String
    '        Dim strNewDate As String
    '        Dim strStation As String
    '        Dim strNetAmount As String
    '        Dim strProduct As String
    '        Dim arrDate As String()
    '        Dim arrYMD As String()
    '        Dim nCost As Double
    '        Dim strUserSub As String
    '        Dim strStatus As String

    '        Dim strFldNameDate As String
    '        Dim strFldNameReg As String
    '        Dim strFldQun As String
    '        Dim strFldPrice As String
    '        Dim strFldStation As String
    '        Dim strFIdNetAmount As String
    '        Dim strFIdProduct As String

    '        Dim insertCount As Double = 0

    '        Dim i As Int16

    '        'Try
    '        'Connect to Excel
    '        oleConn = New OleDbConnection(strConn)
    '        oleConn.Open()

    '        'Connect to SQL 'all save into master
    '        'connMaster = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("fuelmaster"))
    '        connMaster = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnectionLocal"))
    '        'connLocal = New SqlConnection(objconn.getConnectionString(strUserID, False))
    '        'Dim connection As New Redirect(strUserID)
    '        'connLocal = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings(connection.sqlConnection))
    '        connLocal = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnectionLocal"))

    '        connMaster.Open()
    '        connLocal.Open()

    '        'Get list of Sheet name
    '        arrSheet = getSheetList(oleConn)
    '        Response.Write("C")

    '        'lblDesc.Text = "<br><br>Sheet name: " & arrSheet(0) & "<br>------------------<br>"

    '        strFldNameDate = "Transaction Date & Time"

    '        strFldNameReg = "Licence Number"
    '        strFldQun = "Volume"
    '        strFldPrice = "Unit Price"
    '        strFldStation = "Site"
    '        strFIdNetAmount = "Net Amount (local currency)"
    '        strFIdProduct = "Product Name"
    '        Response.Write("D")
    '        strSQL = "SELECT `" & strFldNameDate & "`,`" & strFldNameReg & "`,`" & strFldQun & "`, `" & strFldPrice & "`, `" & strFldStation & "`, `" & strFIdNetAmount & "`, `" & strFIdProduct & "` FROM " & arrSheet(0) & " "

    '        oleCmd = New OleDbCommand(strSQL, oleConn)

    '        oleDr = oleCmd.ExecuteReader()
    '        Response.Write("E")
    '        'gvExcel
    '        Dim tblData As New DataTable
    '        Dim rowData As DataRow
    '        tblData.Rows.Clear()
    '        tblData.Columns.Add(New DataColumn("#"))
    '        tblData.Columns.Add(New DataColumn("Plate No"))
    '        tblData.Columns.Add(New DataColumn("timestamp"))
    '        tblData.Columns.Add(New DataColumn("Station"))
    '        tblData.Columns.Add(New DataColumn("Fuel"))
    '        tblData.Columns.Add(New DataColumn("Liters"))
    '        tblData.Columns.Add(New DataColumn("Cost"))
    '        tblData.Columns.Add(New DataColumn("Status"))

    '        Dim tblData2 As New DataTable
    '        tblData2.Rows.Clear()
    '        tblData2.Columns.Add(New DataColumn("#"))
    '        tblData2.Columns.Add(New DataColumn("Plate No"))
    '        tblData2.Columns.Add(New DataColumn("timestamp"))
    '        tblData2.Columns.Add(New DataColumn("Station"))
    '        tblData2.Columns.Add(New DataColumn("Fuel"))
    '        tblData2.Columns.Add(New DataColumn("Liters"))
    '        tblData2.Columns.Add(New DataColumn("Cost"))
    '        tblData2.Columns.Add(New DataColumn("Status"))

    '        strPlateNo = ""
    '        strPlateNew = ""
    '        strUserSub = ""
    '        strStatus = ""
    '        strStation = ""
    '        strNetAmount = ""
    '        strProduct = ""

    '        strList = strList & "<br>Start looping in excel...<br />"

    '        Dim validCount As Double = 0
    '        Dim errorCount As Double = 0
    '        Dim noMatchCount As Double = 0
    '        Response.Write("F")
    '        While oleDr.Read()

    '            If (strPlateNo <> oleDr(strFldNameReg).ToString.Replace(" ", "")) Then
    '                strPlateNo = oleDr(strFldNameReg).ToString.Replace(" ", "")

    '                strSQL = "SELECT TOP 1 plateno, userid FROM vehicleTBL WHERE plateno LIKE '%" & strPlateNo & "%'"
    '                strUserSub = ""

    '                cmd = New SqlCommand(strSQL, connLocal)
    '                drSQL = cmd.ExecuteReader()

    '                If (drSQL.Read()) Then
    '                    strPlateNew = drSQL("plateno")
    '                    strUserSub = drSQL("userid")
    '                Else
    '                    strPlateNew = "111111_" & strPlateNo
    '                    strUserSub = ""
    '                End If

    '                drSQL.Close()
    '                drSQL = Nothing

    '            End If

    '            strNewDate = oleDr(strFldNameDate).ToString()
    '            strStation = oleDr(strFldStation).ToString()
    '            strNetAmount = oleDr(strFIdNetAmount).ToString()
    '            strProduct = oleDr(strFIdProduct).ToString()

    '            nCost = CDbl(strNetAmount)

    '            If TestDateTime(strNewDate) = True Then
    '                Dim rowDateTime As DateTime = Convert.ToDateTime(strNewDate)
    '                If strPlateNew.IndexOf("111111") = 0 Then
    '                    noMatchCount += 1
    '                End If
    '                If rowDateTime.Month = ddlMonth.SelectedValue Then
    '                    If strPlateNew.IndexOf("111111") = -1 Then
    '                        validCount += 1
    '                    End If
    '                Else
    '                    errorCount += 1
    '                End If
    '            End If

    '            arrDate = oleDr(strFldNameDate).ToString.Split(" ")
    '            arrYMD = arrDate(0).Split("/")

    '            If UBound(arrDate) > 1 Then
    '                If strPlateNew.IndexOf("111111") = -1 Then
    '                    strNewDate = arrYMD(2) & "-" & arrYMD(1) & "-" & arrYMD(0) & " " & arrDate(1) & " " & arrDate(2)

    '                    rowData = tblData.NewRow
    '                    rowData(0) = i + 1
    '                    rowData(1) = strPlateNew
    '                    rowData(2) = strNewDate
    '                    rowData(3) = strStation
    '                    rowData(4) = strProduct
    '                    rowData(5) = oleDr(strFldQun).ToString
    '                    rowData(6) = Format(nCost, "##0.00")
    '                    rowData(7) = strStatus
    '                    tblData.Rows.Add(rowData)

    '                    strNewDate = arrYMD(2) & "-" & arrYMD(0) & "-" & arrYMD(1) & " " & arrDate(1) & " " & arrDate(2)

    '                    rowData = tblData2.NewRow
    '                    rowData(0) = i + 1
    '                    rowData(1) = strPlateNew
    '                    rowData(2) = strNewDate
    '                    rowData(3) = strStation
    '                    rowData(4) = strProduct
    '                    rowData(5) = oleDr(strFldQun).ToString
    '                    rowData(6) = Format(nCost, "##0.00")
    '                    rowData(7) = strStatus
    '                    tblData2.Rows.Add(rowData)

    '                    i = i + 1
    '                End If
    '            Else
    '                If strPlateNew.IndexOf("111111") = -1 Then
    '                    strNewDate = arrYMD(2) & "-" & arrYMD(1) & "-" & arrYMD(0) & " " & arrDate(1)

    '                    rowData = tblData.NewRow
    '                    rowData(0) = i + 1
    '                    rowData(1) = strPlateNew
    '                    rowData(2) = strNewDate
    '                    rowData(3) = strStation
    '                    rowData(4) = strProduct
    '                    rowData(5) = oleDr(strFldQun).ToString
    '                    rowData(6) = Format(nCost, "##0.00")
    '                    rowData(7) = strStatus
    '                    tblData.Rows.Add(rowData)

    '                    strNewDate = arrYMD(2) & "-" & arrYMD(0) & "-" & arrYMD(1) & " " & arrDate(1)

    '                    rowData = tblData2.NewRow
    '                    rowData(0) = i + 1
    '                    rowData(1) = strPlateNew
    '                    rowData(2) = strNewDate
    '                    rowData(3) = strStation
    '                    rowData(4) = strProduct
    '                    rowData(5) = oleDr(strFldQun).ToString
    '                    rowData(6) = Format(nCost, "##0.00")
    '                    rowData(7) = strStatus
    '                    tblData2.Rows.Add(rowData)

    '                    i = i + 1
    '                End If
    '            End If

    '        End While


    '        Dim RecordTable As New DataTable
    '        If validCount > errorCount Then
    '            RecordTable = tblData
    '        Else
    '            RecordTable = tblData2
    '        End If

    '        'Response.Write(RecordTable.Rows.Count)
    '        For w As Int32 = 0 To RecordTable.Rows.Count - 1
    '            If TestDateTime(RecordTable.Rows(w)("timestamp")) Then
    '                If Convert.ToDateTime(RecordTable.Rows(w)("timestamp")).Month = ddlMonth.SelectedValue Then
    '                    Try
    '                        nCost = RecordTable.Rows(w)("Cost")
    '                        strOut = "INSERT INTO fuel_test (userid,plateno,timestamp,stationcode,fueltype,liters,cost) VALUES " & _
    '                                         "('" & strUserSub & "', '" & RecordTable.Rows(w)("Plate No") & "','" & Convert.ToDateTime(RecordTable.Rows(w)("timestamp")).ToString("yyyy/MM/dd HH:mm:ss") & "','" & RecordTable.Rows(w)("Station") & "','" & RecordTable.Rows(w)("Fuel") & "','" & RecordTable.Rows(w)("Liters") & "','" & Format(nCost, "##0.00") & "')"
    '                        cmd = New SqlCommand(strOut, connMaster)
    '                        insertCount = cmd.ExecuteNonQuery()
    '                    Catch ex As SystemException
    '                        Response.Write("@@@" & ex.Message & "<br/>")
    '                    End Try
    '                End If
    '            End If
    '        Next

    '        oleDr.Close()
    '        oleDr = Nothing
    '        'gvExcel.DataSource = tblData
    '        'gvExcel.DataBind()

    '        'gvExcel2.DataSource = tblData2
    '        'gvExcel2.DataBind()

    '        If (Not (oleDr) Is Nothing) Then
    '            oleDr.Close()
    '            oleDr.Dispose()
    '            oleDr = Nothing
    '        End If
    '        If (Not (oleCmd) Is Nothing) Then
    '            oleCmd.Dispose()
    '            oleCmd = Nothing
    '        End If
    '        If (Not (oleConn) Is Nothing) Then
    '            oleConn.Close()
    '            oleConn.Dispose()
    '            oleConn = Nothing
    '        End If

    '        If (Not (connMaster) Is Nothing) Then
    '            connMaster.Close()
    '            connMaster.Dispose()
    '            connMaster = Nothing
    '        End If

    '        If (Not (connLocal) Is Nothing) Then
    '            connLocal.Close()
    '            connLocal.Dispose()
    '            connLocal = Nothing
    '        End If

    '        'End Try
    '    Catch ex As SystemException
    '        Response.Write(ex.Message)
    '    End Try
    'End Sub

    Private Function getSheetList(ByVal strExcelFile As String) As String()

        Dim strConn As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & strExcelFile & ";Extended Properties=Excel 12.0"
        Dim oleConn As OleDbConnection
        Dim oleCmd As OleDbCommand
        Dim oleDr As OleDbDataReader

        oleConn = New OleDbConnection(strConn)
        oleConn.Open()

        Dim dt As DataTable

        Try
            ' Get the data table containg the schema guid.
            dt = oleConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, Nothing)
            If (dt Is Nothing) Then
                Return Nothing
            End If

            Dim excelSheets(dt.Rows.Count) As String
            Dim i As Double = 0
            ' Add the sheet name to the string array.
            For Each row As DataRow In dt.Rows
                excelSheets(i) = "[" & row("TABLE_NAME").ToString.Replace("'", "") & "]"
                i = (i + 1)
            Next

            Return excelSheets
        Catch ex As Exception
            Response.Write("ERROR :" & ex.Message & "<br />")
            Return Nothing
        Finally

            oleConn.Close()

            If (Not (dt) Is Nothing) Then
                dt.Dispose()
            End If
        End Try
    End Function

    Public Shared Function TestDateTime(ByVal parseDateTime As String) As Boolean
        Dim Result As String = ""

        Dim Test As DateTime
        If DateTime.TryParse(parseDateTime, Test) Then
            Result = True

        Else
            Result = False

        End If

        Return Result
    End Function

    Protected Sub btnProcess_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnProcess.Click

        excel2SQLRowSet(TextBox1.Text, TextBox2.Text)

    End Sub
End Class
