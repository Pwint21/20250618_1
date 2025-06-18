Imports System.Data.SqlClient

Public Class GeofenceTrack
    Inherits System.Web.UI.Page
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

        Catch ex As Exception

        End Try
        MyBase.OnInit(e)
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Try
            Dim userid As String = Request.Cookies("userinfo")("userid")
            Dim role As String = Request.Cookies("userinfo")("role")
            Dim userslist As String = Request.Cookies("userinfo")("userslist")
            Dim cmd As SqlCommand = New SqlCommand("select geofenceid,geofencename,type,geoarea  from (select geofenceid,geofencename,0 as type,geoarea  from geofence where geofenceid in ('29563','24916','4457','24914','23582','17364','24915','14194','19585','25125','24912','4395','29564','29562','23581','29687')) T1 union (select geofenceid,geofencename,1 as type,geoarea  from geofence where geofenceid not in  ('29563','24916','4457','24914','23582','17364','24915','14194','19585','25125','24912','4395','29564','29562','23581','29687')) order by type,geofencename", conn)
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            ddlcustomerid.Items.Clear()
            ddlcustomerid.Items.Add(New ListItem("PLEASE SELECT CUSTOMER", "0"))
            While dr.Read()
                If Not dr("geoarea").ToString() = "-" Then
                    ddlcustomerid.Items.Add(New ListItem(dr("geofencename").ToString().ToUpper() + " - " + dr("geoarea").ToString().ToUpper(), dr("geofenceid")))
                Else
                    ddlcustomerid.Items.Add(New ListItem(dr("geofencename").ToString().ToUpper(), dr("geofenceid")))
                End If
            End While
        Catch ex As Exception
        Finally
            If conn.State = ConnectionState.Open Then
                conn.Close()
            End If
        End Try


    End Sub

End Class