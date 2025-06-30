Imports System.Data
Imports System.Data.OleDb
Imports System.Data.SqlClient

Partial Class uploadFuelExcel
    Inherits System.Web.UI.Page

    Protected Sub btnUpload_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpload.Click
        lblError.Text = ""
        lblDesc.Text = ""
        Try
            If Not (MyUpload.PostedFile Is Nothing) And (MyUpload.PostedFile.ContentLength > 0) Then
                Dim ex1 As Exception
                Dim nMaxFileSize As Int32
                nMaxFileSize = 600 * 1024
                If (ddlusername.SelectedValue = "--Select User Name--") Then
                    ex1 = New Exception("Please select User Name")
                    Throw ex1
                End If

                If (MyUpload.PostedFile.ContentLength > nMaxFileSize) Then
                    ex1 = New Exception("Only file size up to 600KB can be uploaded")
                    Throw ex1
                End If

                'Response.Write(MyUpload.PostedFile.ContentType)
                Dim strExt As String = System.IO.Path.GetExtension(MyUpload.PostedFile.FileName).ToLower
                If (strExt <> ".xls") Then
                    ex1 = New Exception("Only Excel file (*.xls) is allowed")
                    Throw ex1
                End If

                'If (MyUpload.PostedFile.ContentType <> "application/vnd.ms-excel") Then
                '    ex1 = New Exception("Only Excel file (*.xls) is allowed")
                '   Throw ex1
                'End If

                Dim strFileName As String
                Dim strPath As String
                Dim userid As String = ddlusername.SelectedValue
                Dim username As String = ddlusername.SelectedItem.Text

                strPath = Server.MapPath("fuelexcel\")
                strFileName = username & "_" & DateTime.Now.ToString("yyyyMMdd_HHmmss") & ".xls"

                MyUpload.PostedFile.SaveAs(strPath & strFileName)

                excel2SQL(strPath & strFileName, userid)
            Else
                lblError.Text = "No file selected to upload"
            End If
        Catch ex As Exception
            lblError.Text = "File Error: "& ex.Message
            'Exit Sub
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'If Session("login") = Nothing Then
        '    Response.Redirect("Login.aspx")
        'End If

        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("fuelmaster"))
        Dim cmd As SqlCommand
        Dim dr As SqlDataReader
        Dim strSQL As String

        strSQL = "SELECT userid, username,dbip FROM userTBL WHERE role='User' ORDER BY username"

        conn.Open()
        cmd = New SqlCommand(strSQL, conn)
        dr = cmd.ExecuteReader()
        While dr.Read()
            ddlusername.Items.Add(New ListItem(dr("username"), dr("userid")))
        End While
        dr.Close()
        conn.Close()

        lblDesc.Text = ""

        'Response.Write(Server.MapPath("fuelexcel\"))
    End Sub

    Sub excel2SQL(ByVal strExcelFile As String, ByVal strUserID As String)
        If (strExcelFile = "") Or (struserid = "") Then
            Exit Sub
        End If
        Dim dtTimer As New DateTime()

        'For OLE DB
        Dim strConn As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & strExcelFile & ";Extended Properties=Excel 8.0"
        Dim oleConn As OleDbConnection
        Dim oleCmd As OleDbCommand
        Dim oleDr As OleDbDataReader

        'For SQL DB

        Dim connMaster As SqlConnection
        Dim connLocal As SqlConnection
        Dim cmd As SqlCommand
        Dim drSQL As SqlDataReader

        Dim arrSheet As String()
        Dim strSQL As String
        Dim strOut As String
        Dim strList As String = ""

        Dim strPlateNo As String
        Dim strPlateNew As String
        Dim strNewDate As String
        Dim arrDate As String()
        Dim arrYMD As String()
        Dim nCost As Double
        Dim strUserSub as String
        Dim strStatus as String

        Dim i As Int16

        Try
            'Connect to Excel
            oleConn = New OleDbConnection(strConn)
            oleConn.Open()

            'Connect to SQL 'all save into master
            connMaster = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("fuelmaster"))
            'connLocal = New SqlConnection(objconn.getConnectionString(strUserID, False))
            connLocal = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            Response.Write("<br>call conn str")

            connMaster.Open()
            connLocal.Open()

            Response.Write("<br>connect to DB")

            'Get list of Sheet name
            arrSheet = getSheetList(oleConn)

            'Query each excel sheet.
            'Only read first tab
            lblDesc.Text = "<br><br>Sheet name: " & arrSheet(0) & "<br>------------------<br>"

            strSQL = "SELECT `Transaction Date/Time`,`Vehicle Reg# No#`,Quantity, `Unit Price` FROM " & arrSheet(0) & " ORDER BY `Vehicle Reg# No#`"
            'strSQL = "SELECT * FROM " & arrSheet(i)
            oleCmd = New OleDbCommand(strSQL, oleConn)

            'strList = strList &"<br>Select from excel"
            Response.Write("<br>select from excel")

            'Line by line reading
            oleDr = oleCmd.ExecuteReader()

            'gvExcel
            Dim tblData As New DataTable
            Dim rowData As DataRow
            tblData.Rows.Clear()
            tblData.Columns.Add(New DataColumn("#"))
            tblData.Columns.Add(New DataColumn("Plate No"))
            tblData.Columns.Add(New DataColumn("timestamp"))
            tblData.Columns.Add(New DataColumn("Station"))
            tblData.Columns.Add(New DataColumn("Fuel"))
            tblData.Columns.Add(New DataColumn("Liters"))
            tblData.Columns.Add(New DataColumn("Cost"))
            tblData.Columns.Add(New DataColumn("Status"))

            strPlateNo = ""
            strPlateNew = ""
            strUserSub = ""
            strStatus = ""

            strList = strList &"<br>Start looping in excel..."

            While oleDr.Read()
                If (strPlateNo <> oleDr("Vehicle Reg# No#").ToString.Replace(" ", "")) Then
                    strPlateNo = oleDr("Vehicle Reg# No#").ToString.Replace(" ", "")

                    If strUserID = "1029" Then
                        strSQL = "SELECT TOP 1 plateno, userid FROM vehicleTBL WHERE plateno LIKE '%" & strPlateNo & "%'"
                        strUserSub = ""
                    Else
                        strSQL = "SELECT TOP 1 plateno,userid FROM vehicleTBL WHERE userid='" & strUserID & "' AND plateno LIKE '%" & strPlateNo & "%'"
                        strUserSub = ""
                    End If

                    'response.write("<br>sql: "& strSQL)

                    'strSQL = "SELECT TOP 1 plateno FROM vehicleTBL WHERE userid='" & strUserID & "' AND plateno LIKE '%" & strPlateNo & "%'"
                    cmd = New SqlCommand(strSQL, connLocal)
                    drSQL = cmd.ExecuteReader()

                    If (drSQL.Read()) Then
                        strPlateNew = drSQL("plateno")
                        strUserSub = drSQL("userid") 'added
                    Else
                        strPlateNew = "111111_"& strPlateNo
                        strUserSub = "" 'added
                    End If

                    'strList = strList &"<br>sql: "& strPlateNew

                    drSQL.Close()
                    drSQL = Nothing
                End If

                'arrDate = oleDr("Transaction Date/Time").ToString.Split(" ")
                'date=arrdate(0), time=arrdate(1)
                'arrYMD = arrDate(0).Split("/") '2/25/2010
                'strNewDate = arrYMD(2) & "-" & arrYMD(0) & "-" & arrYMD(1) & " " & arrDate(1)

                strNewDate = oleDr("Transaction Date/Time").ToString

                Try
                    nCost = oleDr("Quantity") * oleDr("Unit Price")
                    strOut = "INSERT INTO fuel (userid,plateno,timestamp,stationcode,fueltype,liters,cost) VALUES " &
                             "('" & strUserSub & "', '" & strPlateNew & "','" & strNewDate & "','','Diesel','" & oleDr("Quantity").ToString & "','" & Format(nCost, "##0.00") & "')"
                    'Response.Write(Format(oleDr("Transaction Date/Time"), "yyyy-MM-dd HH:mm:ss") & " - " & oleDr("Card Number") & " - " & oleDr("Transaction Amount") & "<br>")

                    If strPlateNew.IndexOf("111111") = -1 Then
                        'If strPlateNew ="111111" Then                    
                        cmd = New SqlCommand(strOut, connMaster)
                        cmd.ExecuteNonQuery()
                        strStatus = "Execute"
                        'Response.Write(strOut & "<br /><br />")
                    End If

                    'cmd = New SqlCommand(strOut, connMaster)
                    'cmd.ExecuteNonQuery()
                    ''Response.Write(strOut & "<br /><br />")

                    'r = userstable.NewRow
                    rowData = tblData.NewRow
                    rowData(0) = i + 1
                    rowData(1) = strPlateNew
                    rowData(2) = strNewDate
                    rowData(3) = ""
                    rowData(4) = "Diesel"
                    rowData(5) = oleDr("Quantity").ToString
                    rowData(6) = Format(nCost, "##0.00")
                    rowData(7) = strStatus
                    tblData.Rows.Add(rowData)

                    i = i + 1
                Catch ex As Exception
                    strList = strList & "Err: " & ex.Message & " - " & strPlateNew & "<br />"
                Finally
                    'Response.Write(strOut & "<br />")
                End Try
            End While

            oleDr.Close()
            oleDr = Nothing
            gvExcel.DataSource = tblData
            gvExcel.DataBind()

        Catch ex As Exception
            strList = strList & "<br />Err: " & ex.Message & "<br />"
        Finally
            'clean up
            If (Not (oleDr) Is Nothing) Then
                oleDr.Close()
                oleDr.Dispose()
                oleDr = Nothing
            End If
            If (Not (oleCmd) Is Nothing) Then
                oleCmd.Dispose()
                oleCmd = Nothing
            End If
            If (Not (oleConn) Is Nothing) Then
                oleConn.Close()
                oleConn.Dispose()
                oleConn = Nothing
            End If

            If (Not (connMaster) Is Nothing) Then
                connMaster.Close()
                connMaster.Dispose()
                connMaster = Nothing
            End If

            If (Not (connLocal) Is Nothing) Then
                connLocal.Close()
                connLocal.Dispose()
                connLocal = Nothing
            End If

            lblError.Text = lblError.Text & "Done " & (i - 1) & "<br />time spent : " & DateTime.Now().Subtract(dtTimer).Milliseconds & " ms <br />"
            lblDesc.Text = strList
        End Try
    End Sub

    Sub excel2SQL_multi_tab(ByVal strExcelFile As String, ByVal strUserID As String)
        If (strExcelFile = "") Or (strUserID = "") Then
            Exit Sub
        End If
        Dim dtTimer As New DateTime()

        'For OLE DB
        Dim strConn As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & strExcelFile & ";Extended Properties=Excel 8.0"
        Dim oleConn As OleDbConnection
        Dim oleCmd As OleDbCommand
        Dim oleDr As OleDbDataReader

        'For SQL DB
        Dim conn As SqlConnection = New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("fuelmaster"))
        Dim cmd As SqlCommand
        Dim drSQL As SqlDataReader

        Dim arrSheet As String()
        Dim strSQL As String
        Dim strOut As String
        Dim strList As String

        Dim strPlateNo As String
        Dim strPlateNew As String
        Dim strNewDate As String
        Dim arrDate As String()
        Dim arrYMD As String()

        Dim i As Int16

        Try
            'Connect to Excel
            oleConn = New OleDbConnection(strConn)
            oleConn.Open()

            'Connect to SQL 
            conn.Open()

            'Get list of Sheet name
            arrSheet = getSheetList(oleConn)

            'Loop for every sheet
            Do While (i < arrSheet.Length)
                'Query each excel sheet.

                Response.Write("<br><br>Sheet name: " & arrSheet(i) & "<br>------------------<br>")

                strSQL = "SELECT `Transaction Date/Time`,`Vehicle Reg# No#`,Quantity, GPS, `Unit Price`, `Transaction Amount` FROM " & arrSheet(i) & " ORDER BY `Vehicle Reg# No#`"
                'strSQL = "SELECT * FROM " & arrSheet(i)
                oleCmd = New OleDbCommand(strSQL, oleConn)

                'Line by line reading
                oleDr = oleCmd.ExecuteReader()

                strPlateNo = ""
                strPlateNew = ""
                'strUserID = ""

                While oleDr.Read()
                    'If (strPlateNo <> oleDr("Vehicle Reg# No#").ToString.Replace(" ", "")) Then
                    'strPlateNo = oleDr("Vehicle Reg# No#").ToString.Replace(" ", "")

                    'strSQL = "SELECT TOP 1 userid, plateno FROM vehicleTBL WHERE plateno LIKE '%" & strPlateNo & "%'"
                    'cmd = New SqlCommand(strSQL, conn)
                    'drSQL = cmd.ExecuteReader()

                    'If (drSQL.Read()) Then
                    '   strUserID = drSQL("userid")
                    '   strPlateNew = drSQL("plateno")
                    'Else
                    '   strUserID = "Nobody"
                    '   strPlateNew = strPlateNo
                    'End If
                    'End If

                    strPlateNew = oleDr("Vehicle Reg# No#").ToString.Replace(" ", "")

                    arrDate = oleDr("Transaction Date/Time").ToString.Split(" ")
                    'date=arrdate(0), time=arrdate(1)
                    arrYMD = arrDate(0).Split("/") '2/25/2010
                    strNewDate = arrYMD(2) & "-" & arrYMD(1) & "-" & arrYMD(0) & " " & arrDate(1)

                    Try
                        strOut = "INSERT INTO fuel2 (userid,plateno,timestamp,stationcode,fueltype,liters,cost) VALUES " &
                                 "('" & strUserID & "', '" & strPlateNew & "','" & strNewDate & "','','Diesel','" & oleDr("Quantity").ToString & "','" & oleDr("Unit Price").ToString & "')"
                        'Response.Write(Format(oleDr("Transaction Date/Time"), "yyyy-MM-dd HH:mm:ss") & " - " & oleDr("Card Number") & " - " & oleDr("Transaction Amount") & "<br>")

                        cmd = New SqlCommand(strOut, conn)
                        cmd.ExecuteNonQuery()
                    Catch ex As Exception
                        Response.Write(ex.Message & "<br>")
                    End Try
                    Response.Write(strOut & "<br>")
                End While

                i = i + 1
                oleDr.Close()
            Loop

        Catch ex As Exception
            'lblError.Text = ex.Message
        Finally
            'clean up
            If (Not (oleDr) Is Nothing) Then
                oleDr.Close()
                oleDr.Dispose()
            End If
            If (Not (oleCmd) Is Nothing) Then
                oleCmd.Dispose()
            End If
            If (Not (oleConn) Is Nothing) Then
                oleConn.Close()
                oleConn.Dispose()
            End If
            Response.Write("finally <br>")
            Response.Write("time spent : " & DateTime.Now().Subtract(dtTimer).Milliseconds & " ms")

        End Try
    End Sub

    Private Function getSheetList(ByRef oleConn As OleDbConnection) As String()
        Dim dt As DataTable

        Try
            ' Get the data table containg the schema guid.
            dt = oleConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, Nothing)
            If (dt Is Nothing) Then
                Return Nothing
            End If

            Dim excelSheets(dt.Rows.Count) As String
            Dim i As Integer = 0
            ' Add the sheet name to the string array.
            For Each row As DataRow In dt.Rows
                excelSheets(i) = "[" & row("TABLE_NAME").ToString.Replace("'", "") & "]"
                i = (i + 1)
            Next

            'Response.Write("List all sheet...<br>")
            'i = 0
            'Do While (i < excelSheets.Length)
            'Query each excel sheet.
            'Response.Write(excelSheets(i) & "<br>")
            'i = i + 1
            'Loop

            Return excelSheets
        Catch ex As Exception
            Response.Write("ERROR :" & ex.Message & "<br />")
            Return Nothing
        Finally
            If (Not (dt) Is Nothing) Then
                dt.Dispose()
            End If
        End Try
    End Function

End Class
