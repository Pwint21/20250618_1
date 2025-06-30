Imports System.Data.SqlClient

Partial Class updatevehiclerecentstatus
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim pno As String = Request.QueryString("pno")
        Dim vtype As String = Request.QueryString("type")
        Dim re As String = Request.QueryString("re")
        Dim status As String = Request.QueryString("status")
        Dim r As String = 0
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Try

            Dim cmd As New SqlCommand()
            cmd.Connection = conn
            conn.Open()
            If vtype = "1" Then
                cmd.CommandText = "delete from vehicle_status_tracked2 where plateno=@plateno"
                cmd.Parameters.AddWithValue("@plateno", pno)
                cmd.ExecuteNonQuery()
                r = 1
            Else
                cmd.CommandText = "sp_InsertVehicleTrack"
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.AddWithValue("@plateno", pno)
                cmd.Parameters.AddWithValue("@remarks", re)
                cmd.Parameters.AddWithValue("@status", status)
                Dim dr As SqlDataReader = cmd.ExecuteReader()
                If dr.Read() Then
                    If dr("result").ToString() = "1" Then
                        r = 1
                    Else
                        r = 0
                    End If
                End If
            End If
        Catch ex As Exception
            r = ex.Message & " - " & ex.StackTrace
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try
        Response.Write(r)
        Response.ContentType = "text/plain"
    End Sub
End Class
