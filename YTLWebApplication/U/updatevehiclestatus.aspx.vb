Imports System.Data.SqlClient

Partial Class updatevehiclestatus
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim s As String = Request.QueryString("s")
        Dim pno As String = Request.QueryString("pno")
        Dim u As String = Request.QueryString("u")
        Dim re As String = Request.QueryString("re")
        Dim r As Integer = 0
        Try
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim uname As String = ""
            Dim cmd1 As SqlCommand = New SqlCommand("select username from userTBL where userid='" & u & "'", conn)
            Dim dr As SqlDataReader
            conn.Open()
            dr = cmd1.ExecuteReader()
            If dr.Read() Then
                uname = dr("username")
            End If
            Dim d = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
            d = d.Replace("-", "/")
            If s <> "Select Status" Then
                Dim cmd As New SqlCommand("insert into maintenance (statusdate,timestamp,status,plateno,sourcename,officeremark) values('" + d + "','" + d + "','" + s + "','" + pno + "','" + uname + "','" + re + "')", conn)
                Try
                    r = cmd.ExecuteNonQuery()
                    Response.Write(r)
                Catch ex As Exception
                Finally
                    conn.Close()
                End Try
            Else
                Response.Write(0)
            End If
            If r <> 1 Then
                Response.Write(0)

            End If

        Catch ex As Exception
        End Try

    End Sub
End Class
