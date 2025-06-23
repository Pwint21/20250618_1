Imports System.Xml
Partial Class GoogleEarthMaps
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            Dim x As Double = System.Convert.ToDouble(Request.QueryString("x"))
            Dim y As Double = System.Convert.ToDouble(Request.QueryString("y"))


            Dim xml As String = "<?xml version='1.0' encoding='utf-8' ?><kml xmlns='http://earth.google.com/kml/2.0'>"


            xml += "<Document><name>Vehicle Position</name><description>Click on icon to see vehicle position on map</description>"
            xml += "<Placemark><name></name><description><![CDATA[ ]]></description><Point><coordinates>" & x & "," & y & ",0</coordinates></Point></Placemark>"
            xml += "</Document></kml>"

            Dim xmldoc As System.Xml.XmlDocument = New System.Xml.XmlDocument()
            xmldoc.LoadXml(xml)

            Response.Buffer = True
            Response.ContentType = "application/vnd.google-earth.kml+xml"
            Response.AddHeader("Content-Disposition", "attachment; filename=VehicleLocation.kml;")

            xmldoc.Save(Response.Output)

        Catch ex As Exception
            Response.Write("<b style='color:red;'>" & ex.Message & "</b>")
        End Try
    End Sub
End Class
