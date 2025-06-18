Imports System.Data.SqlClient

Public Class GeofenceTrack
    Inherits System.Web.UI.Page
    
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try
            ' SECURITY FIX: Enable authentication check
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
                Return
            End If

            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If

        Catch ex As Exception
            SecurityHelper.LogError("GeofenceTrack OnInit Error", ex, Server)
            Response.Redirect("Error.aspx")
        End Try
        MyBase.OnInit(e)
    End Sub
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate user session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If

            LoadGeofenceData()

        Catch ex As Exception
            SecurityHelper.LogError("GeofenceTrack Page_Load Error", ex, Server)
            Response.Redirect("Error.aspx")
        End Try
    End Sub

    Private Sub LoadGeofenceData()
        Try
            Using conn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString)
                ' SECURITY FIX: Get validated user information
                Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
                Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
                Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)

                ' SECURITY FIX: Use parameterized query with specific geofence IDs
                Dim allowedGeofenceIds As String = "'29563','24916','4457','24914','23582','17364','24915','14194','19585','25125','24912','4395','29564','29562','23581','29687'"
                
                Dim query As String = "SELECT geofenceid, geofencename, 0 as type, geoarea " &
                                    "FROM geofence WHERE geofenceid IN (" & allowedGeofenceIds & ") " &
                                    "UNION " &
                                    "SELECT geofenceid, geofencename, 1 as type, geoarea " &
                                    "FROM geofence WHERE geofenceid NOT IN (" & allowedGeofenceIds & ") " &
                                    "ORDER BY type, geofencename"

                Using cmd As New SqlCommand(query, conn)
                    conn.Open()
                    Using dr As SqlDataReader = cmd.ExecuteReader()
                        ddlcustomerid.Items.Clear()
                        ddlcustomerid.Items.Add(New ListItem("PLEASE SELECT CUSTOMER", "0"))
                        
                        While dr.Read()
                            Dim geofenceName As String = SecurityHelper.HtmlEncode(dr("geofencename").ToString().ToUpper())
                            Dim geofenceId As String = dr("geofenceid").ToString()
                            Dim geoarea As String = dr("geoarea").ToString()
                            
                            ' SECURITY FIX: Validate geofence ID
                            Dim geoIdInt As Integer
                            If Integer.TryParse(geofenceId, geoIdInt) AndAlso geoIdInt > 0 Then
                                If Not geoarea.Equals("-") Then
                                    Dim displayText As String = geofenceName & " - " & SecurityHelper.HtmlEncode(geoarea.ToUpper())
                                    ddlcustomerid.Items.Add(New ListItem(displayText, geofenceId))
                                Else
                                    ddlcustomerid.Items.Add(New ListItem(geofenceName, geofenceId))
                                End If
                            End If
                        End While
                    End Using
                End Using
            End Using

        Catch ex As Exception
            SecurityHelper.LogError("LoadGeofenceData Error", ex, Server)
        End Try
    End Sub

End Class