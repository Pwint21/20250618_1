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
            Dim userid As String = Request.Form("userid")
            Companyname = Request.Form("companyname")
            ViewState("cname") = Companyname
            Dim plateno As String = Request.Form("plateno")
            Dim document As New Document(PageSize.A4, 20.0F, 20.0F, 20.0F, 10.0F)
            Dim filename As String = "C:\inetpub\wwwroot\YTLNew\tmp_pdf\Certificate.pdf"
            Dim BigFont As Font = FontFactory.GetFont("Berlin Sans FB Demi", 45, Font.BOLD)
            Dim titleFont As Font = FontFactory.GetFont("Berlin Sans FB Demi", 11, Font.NORMAL)
            Dim tableHeader As Font = FontFactory.GetFont("Berlin Sans FB Demi", 12, Font.BOLD)
            'writer - have our own path!!! and see you have write permissions...
            Dim pdfWrite As PdfWriter = PdfWriter.GetInstance(document, New FileStream(filename, FileMode.Create))
            Dim ev As New itsEvents
            pdfWrite.PageEvent = ev
            document.Open()

            Dim bf As BaseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED)
            Dim cb As PdfContentByte = pdfWrite.DirectContent

            'document.Add(New Paragraph("Date Of Issue: " & DateTime.Now.ToString("dd MMM yyyy"), titleFont))
            'document.Add(Chunk.NEWLINE)
            'Dim p1 As Paragraph = New Paragraph("Gussmann Technologies Sdn Bhd hereby declares that below company has been using our GPS Tracking Solutions and the vehicles that fitted with GPS System are listed below:", titleFont)
            'p1.IndentationLeft = 30
            'document.Add(p1)
            'document.Add(Chunk.NEWLINE)
            'Dim p2 As Paragraph = New Paragraph("Company Name: " & Companyname, titleFont)
            'p2.IndentationLeft = 30
            'document.Add(p2)
            'document.Add(Chunk.NEWLINE)
            'Dim p3 As Paragraph = New Paragraph("Vehicle Plate Number:", titleFont)
            'p3.IndentationLeft = 30
            'document.Add(p3)
            Dim p4 As Paragraph = New Paragraph()
            p4.IndentationLeft = 50

            Dim vehicleLsit As List(Of Vehiclelist) = GetVehiclesList(userid, plateno)
            Dim sno As Integer = 1
            Dim pno As Integer = 1
            Dim pagecnt As Integer = 1
            Dim sb As New StringBuilder

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
                'cell = New PdfPCell(New Phrase("Brand", tableHeader))
                'cell.HorizontalAlignment = 1
                'table.AddCell(cell)

                For i As Integer = 0 To vehicleLsit.Count - 1
                    cb.BeginText()
                    cb.SetFontAndSize(bf, 16)
                    cb.SetTextMatrix(140, 610)
                    cb.ShowText(Companyname)
                    cb.EndText()
                    'If i > 0 Then

                    '    document.NewPage()
                    '    p5 = New Paragraph(vehicleLsit(i).plateno, BigFont)
                    '    p5.Alignment = Element.ALIGN_CENTER
                    '    document.Add(p5)
                    'Else

                    '    p5 = New Paragraph(vehicleLsit(i).plateno, BigFont)
                    '    p5.Alignment = Element.ALIGN_CENTER
                    '    document.Add(p5)
                    'End If


                    If pagecnt > 1 Then
                        If pno > 28 Then
                            table.AddCell(sno)
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
                            table.AddCell(sno)
                            table.AddCell(vehicleLsit(i).plateno)
                            table.AddCell(vehicleLsit(i).InstallDate)

                        End If
                    Else
                        If pno > 28 Then
                            table.AddCell(sno)
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
                            table.AddCell(sno)
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
            Dim myWeb As WebClient = New WebClient()
            Dim myBuff As Byte() = myWeb.DownloadData(filename)
            Response.ContentType = "application/pdf"
            Response.AddHeader("content-length", myBuff.Length.ToString())
            Response.AppendHeader("Content-Disposition", "inline; filename=" + Companyname + ".pdf")
            Response.BinaryWrite(myBuff)
            Response.Flush()
            Response.Close()
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub

    Public Function GetVehiclesList(ByVal userid As String, ByVal plateno As String) As List(Of Vehiclelist)
        Dim VList As New List(Of Vehiclelist)
        Try
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim dr As SqlDataReader
            Dim cmd As SqlCommand
            If plateno = "ALL" Then
                cmd = New SqlCommand("select t1.plateno,t1.type,t1.brand,t1.companyid,t2.name,t1.installdate  from vehicleTBL t1 inner join customer t2 on t1.companyid=t2.id where t1.companyid='" & userid & "' order by t1.plateno", conn)
            Else
                cmd = New SqlCommand("select t1.plateno,t1.type,t1.brand,t1.companyid,t2.name,t1.installdate  from vehicleTBL t1 inner join customer t2 on t1.companyid=t2.id where t1.companyid='" & userid & "' and t1.plateno='" & plateno & "' order by t1.plateno", conn)
            End If

            conn.Open()
            dr = cmd.ExecuteReader()
            Dim i As Int32 = 1

            Dim vDetails As Vehiclelist
            While dr.Read
                vDetails.plateno = dr("Plateno")
                If IsDBNull(dr("type")) Then
                    vDetails.type = ""
                Else
                    vDetails.type = dr("type")
                End If
                If IsDBNull(dr("brand")) Then
                    vDetails.Brand = ""
                Else
                    vDetails.Brand = dr("brand")
                End If
                If IsDBNull(dr("installdate")) Then
                    vDetails.InstallDate = ""
                Else
                    vDetails.InstallDate = Convert.ToDateTime(dr("installdate")).ToString("yyyy/MM/dd")
                End If


                vDetails.CompanyName = dr("name")


                VList.Add(vDetails)
            End While
        Catch ex As Exception

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


            Dim strSelectUserListBuilder As StringBuilder = New StringBuilder()
            strSelectUserListBuilder.Append("<br/><br/><br/><br/>")
            Dim BigFont As Font = FontFactory.GetFont("Berlin Sans FB Demi", 72, Font.BOLD)
            Dim titleFont As Font = FontFactory.GetFont("Berlin Sans FB Demi", 11, Font.NORMAL)
            Dim tableHeader As Font = FontFactory.GetFont("Berlin Sans FB Demi", 12, Font.BOLD)
            Dim imageFile As String = "C:\inetpub\wwwroot\YTLNew\images\Certificate\logo-01.png"
            Dim imageFile1 As String = "C:\inetpub\wwwroot\YTLNew\images\Certificate\watermark-01.png"
            Dim imageFile2 As String = "C:\inetpub\wwwroot\YTLNew\images\Certificate\Certificate-01.png"
            Dim imageFile3 As String = "C:\inetpub\wwwroot\YTLNew\images\Certificate\CompanyChop-01.png"

            Dim myImage As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(imageFile)
            Dim myImage1 As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(imageFile1)
            Dim myImage2 As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(imageFile2)
            Dim myImage3 As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(imageFile3)

            myImage.ScaleToFit(160, 35)
            myImage.SetAbsolutePosition(20, 770)
            myImage.SpacingBefore = 50.0F
            myImage.SpacingAfter = 10.0F
            myImage.Alignment = Element.ALIGN_LEFT
            document.Add(myImage)

            myImage1.SetAbsolutePosition(80, 100)
            myImage1.SpacingBefore = 50.0F
            myImage1.SpacingAfter = 10.0F
            myImage1.Alignment = Element.ALIGN_CENTER
            document.Add(myImage1)

            myImage2.ScaleToFit(201, 18)
            myImage2.SetAbsolutePosition(230, 750)

            myImage2.Alignment = Element.ALIGN_RIGHT
            document.Add(myImage2)

            myImage3.ScaleToFit(89, 88)
            myImage3.SetAbsolutePosition(470, 50)

            myImage3.Alignment = Element.ALIGN_RIGHT
            document.Add(myImage3)

            Dim htmlText As String = strSelectUserListBuilder.ToString()
            'make an arraylist ....with STRINGREADER since its no IO reading file...

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

        End Sub
    End Class
End Class



