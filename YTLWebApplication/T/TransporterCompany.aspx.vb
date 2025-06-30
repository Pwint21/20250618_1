Imports System.Data
Imports System.Data.SqlClient
Imports System.IO

Partial Class TransporterCompany
    Inherits System.Web.UI.Page
    Public sb As New StringBuilder
    Public opt As String
    Public companyid As String = "0"
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            'If Request.Cookies("userinfo") Is Nothing Then
            '    Response.Redirect("Login.aspx")
            'End If
        Catch ex As Exception
        End Try
        MyBase.OnInit(e)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try


        Catch ex As Exception

        End Try

    End Sub

    Protected Sub Fill()
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Try
            Dim cmd As SqlCommand = New SqlCommand("select userid,username  from userTBL where userid not in ( select ISNULL(item,0) from fn_getcompanylist(" & companyid & ")) order by username", conn)
            Dim dr As SqlDataReader
            Try
                conn.Open()
                'dr = cmd.ExecuteReader()
                'While dr.Read()
                '    AllTrasporters.Items.Add(New ListItem(dr("username").ToString().ToUpper(), dr("userid")))
                '    AllTrasporters.Attributes.Add("class", "list-group-item")
                'End While
                'dr.Close()
                cmd = New SqlCommand("select IsNull(shiptocode,'') shiptocode ,geofenceid ,geofencename  from geofence where geofenceid not in ( select ISNULL(item,0) from  fn_getgeofencelist(" & companyid & ")) and shiptocode not like'000%'  order by geofencename", conn)
                dr = cmd.ExecuteReader()
                Allgeofences.Items.Clear()
                While dr.Read()
                    Allgeofences.Items.Add(New ListItem(dr("geofencename").ToString().ToUpper() & " - " & dr("shiptocode").ToString().ToUpper(), dr("geofenceid")))
                    Allgeofences.Attributes.Add("class", "list-group-item")
                End While
            Catch ex As Exception

            Finally
                conn.Close()
            End Try


        Catch ex As Exception
            WriteLog(ex.Message)
            WriteLog(ex.StackTrace)
        End Try
    End Sub

    Sub WriteLog(ByVal message As String)
        Try
            If (message.Length > 0) Then
                Dim sw As New StreamWriter(HttpContext.Current.Server.MapPath("") & "/GetData.Log.txt", FileMode.Append)
                sw.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") & " - " & message)
                sw.Close()
            End If
        Catch ex As Exception

        End Try
    End Sub

End Class
