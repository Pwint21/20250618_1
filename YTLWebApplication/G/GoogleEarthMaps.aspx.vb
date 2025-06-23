Imports System.Xml

Partial Class GoogleEarthMaps
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.StatusCode = 401
                Response.Write("<b style='color:red;'>Unauthorized access</b>")
                Return
            End If

            ' SECURITY FIX: Validate coordinate inputs
            Dim xStr As String = Request.QueryString("x")
            Dim yStr As String = Request.QueryString("y")
            
            If Not SecurityHelper.ValidateCoordinate(xStr, yStr) Then
                Response.Write("<b style='color:red;'>Invalid coordinates provided</b>")
                Return
            End If

            Dim x As Double = System.Convert.ToDouble(xStr)
            Dim y As Double = System.Convert.ToDouble(yStr)

            ' SECURITY FIX: Additional coordinate range validation
            If x < -180 OrElse x > 180 OrElse y < -90 OrElse y > 90 Then
                Response.Write("<b style='color:red;'>Coordinates out of valid range</b>")
                Return
            End If

            ' SECURITY FIX: Create XML safely
            Dim xml As String = "<?xml version='1.0' encoding='utf-8' ?><kml xmlns='http://earth.google.com/kml/2.0'>"
            xml += "<Document><name>Vehicle Position</name><description>Click on icon to see vehicle position on map</description>"
            xml += "<Placemark><name></name><description><![CDATA[ ]]></description><Point><coordinates>" & x.ToString("F6") & "," & y.ToString("F6") & ",0</coordinates></Point></Placemark>"
            xml += "</Document></kml>"

            ' SECURITY FIX: Validate XML before processing
            Dim xmldoc As System.Xml.XmlDocument = New System.Xml.XmlDocument()
            Try
                xmldoc.LoadXml(xml)
            Catch xmlEx As XmlException
                Response.Write("<b style='color:red;'>Invalid XML generated</b>")
                SecurityHelper.LogError("GoogleEarthMaps XML error", xmlEx, Server)
                Return
            End Try

            ' SECURITY FIX: Set secure headers
            Response.Buffer = True
            Response.ContentType = "application/vnd.google-earth.kml+xml"
            Response.AddHeader("Content-Disposition", "attachment; filename=VehicleLocation.kml;")
            Response.AddHeader("X-Content-Type-Options", "nosniff")

            xmldoc.Save(Response.Output)

        Catch ex As Exception
            SecurityHelper.LogError("GoogleEarthMaps error", ex, Server)
            Response.Write("<b style='color:red;'>An error occurred while generating the KML file</b>")
        End Try
    End Sub

End Class