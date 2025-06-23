Imports System
Imports System.Data.SqlClient
Imports System.Data
Imports iTextSharp.text
Imports iTextSharp.text.html.simpleparser
Imports iTextSharp.text.pdf
Imports System.IO
Imports System.Net
Imports System.Collections.Generic

Partial Class GussCertificateNew
    Inherits System.Web.UI.Page
    Public Companyname As String

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.StatusCode = 401
                Response.Write("Unauthorized access")
                Return
            End If

            ' SECURITY FIX: Validate form inputs
            Dim userid As String = SecurityHelper.HtmlEncode(Request.Form("userid"))
            Companyname = SecurityHelper.HtmlEncode(Request.Form("companyname"))
            Dim plateno As String = SecurityHelper.HtmlEncode(Request.Form("plateno"))

            ' SECURITY FIX: Validate inputs
            If Not SecurityHelper.ValidateUserId(userid) Then
                Response.Write("Invalid user ID")
                Return
            End If

            If String.IsNullOrEmpty(Companyname) OrElse Companyname.Length > 100 Then
                Response.Write("Invalid company name")
                Return
            End If

            If Not String.IsNullOrEmpty(plateno) AndAlso plateno <> "ALL" AndAlso Not SecurityHelper.ValidatePlateNumber(plateno) Then
                Response.Write("Invalid plate number")
                Return
            End If

            ' SECURITY FIX: Rate limiting
            If SecurityHelper.IsRateLimited(Request.UserHostAddress, 5, 1) Then
                Response.StatusCode = 429
                Response.Write("Rate limit exceeded")
                Return
            End If

            ViewState("cname") = Companyname
            
            ' SECURITY FIX: Validate file path
            Dim filename As String = Server.MapPath("~/tmp_pdf/Certificate.pdf")
            If Not SecurityHelper.ValidateFilePath(filename) Then
                Response.Write("Invalid file path")
                Return
            End If

            ' SECURITY FIX: Ensure directory exists and is writable
            Dim directory As String = Path.GetDirectoryName(filename)
            If Not Directory.Exists(directory) Then
                Directory.CreateDirectory(directory)
            End If

            Dim document As New Document(PageSize.A4, 20.0F, 20.0F, 20.0F, 10.0F)
            Dim BigFont As Font = FontFactory.GetFont("Berlin Sans FB Demi", 45, Font.BOLD)
            Dim titleFont As Font = FontFactory.GetFont("Berlin Sans FB Demi", 11, Font.NORMAL)
            Dim tableHeader As Font = FontFactory.GetFont("Berlin Sans FB Demi", 12, Font.BOLD)

            Dim pdfWrite As PdfWriter = PdfWriter.GetInstance(document, New FileStream(filename, FileMode.Create))
            Dim ev As New itsEvents
            pdfWrite.PageEvent = ev
            document.Open()

            Dim bf As BaseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED)
            Dim cb As PdfContentByte = pdfWrite.DirectContent

            Dim p4 As Paragraph = New Paragraph()
            p4.IndentationLeft = 50

            Dim vehicleLsit As List(Of Vehiclelist) = GetVehiclesList(userid, plateno)
            Dim sno As Integer = 1
            Dim pno As Integer = 1
            Dim pagecnt As Integer = 1

            Dim table As New PdfPTable(3)
            table.TotalWidth = 450.0F
            table.LockedWidth = True
            Dim widths As Single() = New Single() {2.0F, 5.0F, 5.0F}
            table.SetWidths(widths)
            table.HorizontalAlignment = 0
            table.SpacingBefore = 10.0F
            table.SpacingAfter = 10.0F

            Dim cell As New PdfPCell
            Dim p5 As Paragraph = New Paragraph()

            If vehicleLsit.Count > 0 Then
                cell = New PdfPCell(New Phrase("Sno", tableHeader))
                cell.HorizontalAlignment = 1
                table.AddCell(cell)
                cell = New PdfPCell(New Phrase("Vehicle Number", tableHeader))
                cell.HorizontalAlignment = 1
                table.AddCell(cell)
                cell = New PdfPCell(New Phrase("Installation Date", tableHeader))
                cell.HorizontalAlignment = 1
                table.AddCell(cell)

                For i As Integer = 0 To vehicleLsit.Count - 1
                    cb.BeginText()
                    cb.SetFontAndSize(bf, 16)
                    cb.SetTextMatrix(140, 610)
                    cb.ShowText(Companyname)
                    cb.EndText()

                    If pagecnt > 1 Then
                        If pno > 28 Then
                            table.AddCell(sno.ToString())
                            table.AddCell(vehicleLsit(i).plateno)
                            table.AddCell(vehicleLsit(i).InstallDate)

                            p4.Add(table)
                            document.Add(p4)
                            document.NewPage()
                            table.DeleteBodyRows()
                            p4.Clear()
                            pno = 0
                            pagecnt = pagecnt + 1
                        Else
                            table.AddCell(sno.ToString())
                            table.AddCell(vehicleLsit(i).plateno)
                            table.AddCell(vehicleLsit(i).InstallDate)
                        End If
                    Else
                        If pno > 28 Then
                            table.AddCell(sno.ToString())
                            table.AddCell(vehicleLsit(i).plateno)
                            table.AddCell(vehicleLsit(i).InstallDate)

                            p4.Add(table)
                            document.Add(p4)
                            document.NewPage()
                            table.DeleteBodyRows()
                            p4.Clear()
                            pno = 0
                            pagecnt = pagecnt + 1
                        Else
                            table.AddCell(sno.ToString())
                            table.AddCell(vehicleLsit(i).plateno)
                            table.AddCell(vehicleLsit(i).InstallDate)
                        End If
                    End If
                    sno = sno + 1
                    pno = pno + 1
                Next
                
                If table.Rows.Count > 0 Then
                    p4.Add(table)
                    document.Add(p4)
                    table.DeleteBodyRows()
                    p4.Clear()
                End If
            End If

            document.Close()

            ' SECURITY FIX: Secure file download
            If File.Exists(filename) Then
                Dim fileInfo As New FileInfo(filename)
                If fileInfo.Length > 0 AndAlso fileInfo.Length < 10485760 Then ' 10MB limit
                    Dim myBuff As Byte() = File.ReadAllBytes(filename)
                    Response.ContentType = "application/pdf"
                    Response.AddHeader("content-length", myBuff.Length.ToString())
                    Response.AppendHeader("Content-Disposition", $"inline; filename={SecurityHelper.HtmlEncode(Companyname)}.pdf")
                    Response.BinaryWrite(myBuff)
                    Response.Flush()
                    Response.Close()
                    
                    ' SECURITY FIX: Clean up temporary file
                    Try
                        File.Delete(filename)
                    Catch
                        ' Ignore cleanup errors
                    End Try
                Else
                    Response.Write("File size error")
                End If
            Else
                Response.Write("File generation failed")
            End If

        Catch ex As Exception
            SecurityHelper.LogError("GussCertificateNew error", ex, Server)
            Response.Write("An error occurred while generating the certificate")
        End Try
    End Sub

    Public Function GetVehiclesList(ByVal userid As String, ByVal plateno As String) As List(Of Vehiclelist)
        Dim VList As New List(Of Vehiclelist)
        Try
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
                Dim cmd As SqlCommand
                
                ' SECURITY FIX: Use parameterized queries
                If plateno = "ALL" Then
                    cmd = New SqlCommand("SELECT t1.plateno, t1.type, t1.brand, t1.companyid, t2.name, t1.installdate FROM vehicleTBL t1 INNER JOIN customer t2 ON t1.companyid = t2.id WHERE t1.companyid = @userid ORDER BY t1.plateno", conn)
                    cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@userid", userid, SqlDbType.Int))
                Else
                    cmd = New SqlCommand("SELECT t1.plateno, t1.type, t1.brand, t1.companyid, t2.name, t1.installdate FROM vehicleTBL t1 INNER JOIN customer t2 ON t1.companyid = t2.id WHERE t1.companyid = @userid AND t1.plateno = @plateno ORDER BY t1.plateno", conn)
                    cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@userid", userid, SqlDbType.Int))
                    cmd.Parameters.Add(SecurityHelper.CreateSqlParameter("@plateno", plateno, SqlDbType.VarChar))
                End If

                conn.Open()
                Using dr As SqlDataReader = cmd.ExecuteReader()
                    Dim vDetails As Vehiclelist
                    While dr.Read
                        vDetails.plateno = SecurityHelper.HtmlEncode(dr("Plateno").ToString())
                        
                        If IsDBNull(dr("type")) Then
                            vDetails.type = ""
                        Else
                            vDetails.type = SecurityHelper.HtmlEncode(dr("type").ToString())
                        End If
                        
                        If IsDBNull(dr("brand")) Then
                            vDetails.Brand = ""
                        Else
                            vDetails.Brand = SecurityHelper.HtmlEncode(dr("brand").ToString())
                        End If
                        
                        If IsDBNull(dr("installdate")) Then
                            vDetails.InstallDate = ""
                        Else
                            Dim installDate As DateTime
                            If DateTime.TryParse(dr("installdate").ToString(), installDate) Then
                                vDetails.InstallDate = installDate.ToString("yyyy/MM/dd")
                            Else
                                vDetails.InstallDate = "Invalid Date"
                            End If
                        End If

                        vDetails.CompanyName = SecurityHelper.HtmlEncode(dr("name").ToString())
                        VList.Add(vDetails)
                    End While
                End Using
            End Using
        Catch ex As Exception
            SecurityHelper.LogError("GetVehiclesList error", ex, Server)
        End Try
        Return VList
    End Function

    Structure Vehiclelist
        Public plateno As String
        Public type As String
        Public Brand As String
        Public CompanyName As String
        Public InstallDate As String
    End Structure

    Public Class itsEvents
        Inherits PdfPageEventHelper
        
        Public Overrides Sub OnStartPage(ByVal writer As iTextSharp.text.pdf.PdfWriter, ByVal document As iTextSharp.text.Document)
            Try
                Dim strSelectUserListBuilder As StringBuilder = New StringBuilder()
                strSelectUserListBuilder.Append("<br/><br/><br/><br/>")
                Dim BigFont As Font = FontFactory.GetFont("Berlin Sans FB Demi", 72, Font.BOLD)
                Dim titleFont As Font = FontFactory.GetFont("Berlin Sans FB Demi", 11, Font.NORMAL)
                Dim tableHeader As Font = FontFactory.GetFont("Berlin Sans FB Demi", 12, Font.BOLD)
                
                ' SECURITY FIX: Validate image file paths
                Dim imageFile As String = HttpContext.Current.Server.MapPath("~/images/Certificate/logo-01.png")
                Dim imageFile1 As String = HttpContext.Current.Server.MapPath("~/images/Certificate/watermark-01.png")
                Dim imageFile2 As String = HttpContext.Current.Server.MapPath("~/images/Certificate/Certificate-01.png")
                Dim imageFile3 As String = HttpContext.Current.Server.MapPath("~/images/Certificate/CompanyChop-01.png")

                ' SECURITY FIX: Validate file existence before using
                If File.Exists(imageFile) Then
                    Dim myImage As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(imageFile)
                    myImage.ScaleToFit(160, 35)
                    myImage.SetAbsolutePosition(20, 770)
                    myImage.SpacingBefore = 50.0F
                    myImage.SpacingAfter = 10.0F
                    myImage.Alignment = Element.ALIGN_LEFT
                    document.Add(myImage)
                End If

                If File.Exists(imageFile1) Then
                    Dim myImage1 As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(imageFile1)
                    myImage1.SetAbsolutePosition(80, 100)
                    myImage1.SpacingBefore = 50.0F
                    myImage1.SpacingAfter = 10.0F
                    myImage1.Alignment = Element.ALIGN_CENTER
                    document.Add(myImage1)
                End If

                If File.Exists(imageFile2) Then
                    Dim myImage2 As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(imageFile2)
                    myImage2.ScaleToFit(201, 18)
                    myImage2.SetAbsolutePosition(230, 750)
                    myImage2.Alignment = Element.ALIGN_RIGHT
                    document.Add(myImage2)
                End If

                If File.Exists(imageFile3) Then
                    Dim myImage3 As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(imageFile3)
                    myImage3.ScaleToFit(89, 88)
                    myImage3.SetAbsolutePosition(470, 50)
                    myImage3.Alignment = Element.ALIGN_RIGHT
                    document.Add(myImage3)
                End If

                Dim htmlText As String = strSelectUserListBuilder.ToString()
                For Each element As iTextSharp.text.IElement In HTMLWorker.ParseToList(New StringReader(htmlText), Nothing)
                    document.Add(element)
                Next
                
                document.Add(Chunk.NEWLINE)
                document.Add(Chunk.NEWLINE)
                document.Add(Chunk.NEWLINE)

                document.Add(New Paragraph("Date Of Issue: " & DateTime.Now.ToString("dd MMM yyyy"), titleFont))
                document.Add(Chunk.NEWLINE)

                Dim p1 As Paragraph = New Paragraph("Gussmann Technologies Sdn Bhd hereby declares that below company has been using our GPS Tracking Solutions and the vehicles that fitted with GPS System are listed below:", titleFont)
                p1.IndentationLeft = 30
                document.Add(p1)
                document.Add(Chunk.NEWLINE)

                Dim p2 As Paragraph = New Paragraph("Company Name: ", titleFont)
                p2.IndentationLeft = 30
                document.Add(p2)
                document.Add(Chunk.NEWLINE)

                Dim p3 As Paragraph = New Paragraph("Vehicles List:", titleFont)
                p3.IndentationLeft = 30
                document.Add(p3)

                Dim bf As BaseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED)
                Dim cb As PdfContentByte = writer.DirectContent
                cb.BeginText()
                cb.SetFontAndSize(bf, 10)
                cb.SetTextMatrix(20, 50)
                cb.ShowText("Gussmann Technologies Sdn.Bhd")
                cb.EndText()

                Dim bf1 As BaseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED)
                Dim cb1 As PdfContentByte = writer.DirectContent
                cb1.BeginText()
                cb1.SetFontAndSize(bf1, 8)
                cb1.SetTextMatrix(20, 30)
                cb1.ShowText("51-3, JLN 5/18A, Taman Mastiara, Batu 5 ½, Jalan Ipoh, 51200 Kuala Lumpur.  www.g1.com.my   Tel:03 6257 0509    info@g1.com.my")
                cb1.EndText()

                Dim regnof As BaseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED)
                Dim regno As PdfContentByte = writer.DirectContent
                regno.BeginText()
                regno.SetFontAndSize(regnof, 6)
                regno.SetTextMatrix(140, 42)
                regno.ShowText("(307779-M)")
                regno.EndText()

                Dim gpsc As PdfContentByte = writer.DirectContent
                gpsc.BeginText()
                gpsc.SetFontAndSize(bf, 11)
                gpsc.SetTextMatrix(250, 735)
                gpsc.ShowText("( Of GPS Conformity )")
                gpsc.EndText()

            Catch ex As Exception
                ' SECURITY FIX: Log error but continue
                SecurityHelper.LogError("PDF generation error", ex, HttpContext.Current.Server)
            End Try
        End Sub
    End Class

End Class