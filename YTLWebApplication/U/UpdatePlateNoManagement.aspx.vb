Imports System.Data
Imports System.Data.SqlClient

Partial Class UpdatePlateNoManagement
    Inherits System.Web.UI.Page

    Public errormessage As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try

            If Page.IsPostBack = False Then
                plateno.Attributes.Add("onload", "setHide()")
                ibSubmit.Attributes.Add("onclick", "return mysubmit()")

                'If Session("login") = Nothing Then
                '    Response.Redirect("Login.aspx")
                'End If

                Dim cmd As SqlCommand
                Dim dr As SqlDataReader

                Dim userid As String = Session("userid")
                Dim role = Session("role")
                Dim userslist As String = Session("userslist")
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                cmd = New SqlCommand("select userid, username,dbip from userTBL where role='User' order by username", conn)
                If role = "User" Then
                    cmd = New SqlCommand("select userid, username, dbip from userTBL where userid='" & userid & "'", conn)
                ElseIf role = "SuperUser" Or role = "Operator" Then
                    cmd = New SqlCommand("select userid, username, dbip from userTBL where userid in (" & userslist & ") order by username", conn)
                End If
                conn.Open()
                dr = cmd.ExecuteReader()
                While dr.Read()
                    ddlUsername.Items.Add(New ListItem(dr("username"), dr("userid")))
                End While
                dr.Close()

                conn.Close()
            Else

            End If

        Catch ex As Exception
        End Try

    End Sub

    Protected Sub InsertUpdateTmpTable(ByVal userid As String, ByVal newPlateNo As String, ByVal oldPlateNo As String)
        Try
            Dim connection As New Redirect(userid)
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings(connection.sqlConnection))
            Dim cmd As SqlCommand
            '################### for updating plate number history at backend #####################
            conn.Open()
            cmd = New SqlCommand("Insert Into plateno_upd_tmp(oldplateno,newplateno,insertdatetime) Values ('" & oldPlateNo & "', '" & newPlateNo & "', '" & (DateTime.Now).ToString("yyyy/MM/dd HH:mm:ss") & "')", conn)
            If oldPlateNo <> "--Select Plate No--" Then
                cmd.ExecuteNonQuery()
            End If
            conn.Close()
            '##############################################################################
        Catch ex As SystemException
            Response.Write(ex.Message)
        End Try
    End Sub

    Protected Sub updateTable(ByVal userid As String, ByVal TableName As String, ByVal newPlateNo As String, ByVal oldPlateNo As String)
        Try
            Dim result As String
            Dim connection As New Redirect(userid)
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings(connection.sqlConnection))
            Dim cmd As SqlCommand

            If TableName = "fuel" Then
                Dim connFuel As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings(connection.sqlConnection))
                cmd = New SqlCommand("update " & TableName & " set plateno='" & newPlateNo & "' where plateno='" & oldPlateNo & "'", connFuel)
                connFuel.Open()
                result = cmd.ExecuteNonQuery()
                If result = -1 Then
                    cmd = New SqlCommand("Insert Into vehicle_plateno_modified(OldPlateNo, NewPlateNo, ModifiedDate, ModifiedTable, Username, OldUnitId, NewUnitId) Values ('" & oldPlateNo & "', '" & newPlateNo & "', '" & (DateTime.Now).ToString("yyyy/MM/dd HH:mm:ss") & "', '" & TableName & "', '" & Session("userid") & "', '" & "-" & "', '" & "-" & "')", connFuel)
                    cmd.ExecuteNonQuery()
                End If
                connFuel.Close()
            Else
                cmd = New SqlCommand("update " & TableName & " set plateno='" & newPlateNo & "' where plateno='" & oldPlateNo & "'", conn)
                conn.Open()
                result = cmd.ExecuteNonQuery()
                If result = -1 Then
                    cmd = New SqlCommand("Insert Into vehicle_plateno_modified(OldPlateNo, NewPlateNo, ModifiedDate, ModifiedTable, Username, OldUnitId, NewUnitId) Values ('" & oldPlateNo & "', '" & newPlateNo & "', '" & (DateTime.Now).ToString("yyyy/MM/dd HH:mm:ss") & "', '" & TableName & "', '" & Session("userid") & "', '" & "-" & "', '" & "-" & "')", conn)
                    cmd.ExecuteNonQuery()
                End If
                conn.Close()

            End If


        Catch ex As SystemException
            Response.Write(ex.Message)
        End Try
    End Sub


    Protected Sub ibBack_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibBack.Click
        Try

            plateno.Attributes.Remove("onload")

            Dim userid As String = ddlUsername.SelectedValue
            Dim connection As New Redirect(userid)
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings(connection.sqlConnection))
            Dim cmd As SqlCommand

            For i As Int64 = 0 To cblTable.Items.Count - 1
                If cblTable.Items(i).Selected = True Then
                    If cblTable.Items(i).Value = "fuel" Then
                        'Dim connFuel As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("Server=192.168.1.15;Database=avlsdev;User ID=sa;Password=baad0987654321;MultipleActiveResultSets=True;"))
                        'Dim connFuel As New SqlConnection("Server=192.168.1.155;Database=avlsdev;User ID=sa;Password=baad0987654321;MultipleActiveResultSets=True;")
                        Dim connFuel As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings(connection.sqlConnection))
                        cmd = New SqlCommand("select count(plateno) as counter from " & cblTable.Items(i).Value & " where plateno='" & ddlpleate.SelectedValue & "'", connFuel)
                        connFuel.Open()

                        DisplayRecords(i, cblTable.Items(i).Selected, cmd.ExecuteScalar())
                        connFuel.Close()
                    ElseIf cblTable.Items(i).Value = "plateno_upd_tmp" Then
                        'do nothing
                    Else
                        cmd = New SqlCommand("select count(plateno) as counter from " & cblTable.Items(i).Value & " where plateno='" & ddlpleate.SelectedValue & "'", conn)
                        conn.Open()

                        DisplayRecords(i, cblTable.Items(i).Selected, cmd.ExecuteScalar())
                        conn.Close()
                    End If
                    
                Else
                    DisplayBlank(i)
                End If
            Next

            cmd = New SqlCommand("select unitid from vehicleTBL where plateno='" & ddlpleate.SelectedValue & "'", conn)
            conn.Open()
            lbl6.Text = cmd.ExecuteScalar()
            conn.Close()
        Catch ex As SystemException
            Response.Write(ex.Message)
        End Try

    End Sub

    Protected Sub getPlateNo(ByVal uid As String)
        Try
            If ddlUsername.SelectedValue <> "--Select User Name--" Then
                ddlpleate.Items.Clear()
                ddlpleate.Items.Add("--Select Plate No--")
                Dim cmd As SqlCommand
                Dim dr As SqlDataReader
                Dim connection As New Redirect(ddlUsername.SelectedValue)
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings(connection.sqlConnection))

                cmd = New SqlCommand("select plateno from vehicleTBL where userid='" & uid & "' order by plateno", conn)
                conn.Open()
                dr = cmd.ExecuteReader()
                While dr.Read()
                    ddlpleate.Items.Add(New ListItem(dr("plateno"), dr("plateno")))
                End While
                dr.Close()

                conn.Close()
            Else
                ddlpleate.Items.Clear()
                ddlpleate.Items.Add("--Select User Name--")
            End If
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub

    Protected Sub DisplayRecords(ByVal counter As String, ByVal tableSelected As Boolean, ByVal platenoCounter As String)
        If counter = 0 And tableSelected = True Then
            lbl1.Text = platenoCounter
        ElseIf counter = 0 And tableSelected = False Then
            lbl1.Text = "-"
        ElseIf counter = 1 And tableSelected = True Then
            lbl2.Text = platenoCounter
        ElseIf counter = 2 And tableSelected = True Then
            lbl3.Text = platenoCounter
        ElseIf counter = 3 And tableSelected = True Then
            lbl4.Text = platenoCounter
        ElseIf counter = 4 And tableSelected = True Then
            lbl5.Text = platenoCounter
        ElseIf counter = 5 And tableSelected = True Then
            lblMaintenance.Text = platenoCounter
        ElseIf counter = 6 And tableSelected = True Then
            lblGeofencetracked.Text = platenoCounter
        ElseIf counter = 7 And tableSelected = True Then
            lblGeofencetripaudit.Text = platenoCounter
        ElseIf counter = 8 And tableSelected = True Then
            lblFuel.Text = platenoCounter
        ElseIf counter = 9 And tableSelected = True Then
            lblPanicinterval.Text = platenoCounter
        ElseIf counter = 10 And tableSelected = True Then
            lblTollfare.Text = platenoCounter
        ElseIf counter = 11 And tableSelected = True Then
            lblDocumentdate.Text = platenoCounter
        ElseIf counter = 12 And tableSelected = True Then
            lblDriverassign.Text = platenoCounter
        ElseIf counter = 13 And tableSelected = True Then
            lblOperatorchecklist.Text = platenoCounter
        ElseIf counter = 14 And tableSelected = True Then
            lblTripreceipt.Text = platenoCounter
        ElseIf counter = 15 And tableSelected = True Then
            lblVehicleaverageidling.Text = platenoCounter
        ElseIf counter = 16 And tableSelected = True Then
            lblVehicleservicing.Text = platenoCounter
        ElseIf counter = 17 And tableSelected = True Then
            lblVehicleg13edata.Text = platenoCounter
        ElseIf counter = 18 And tableSelected = True Then
            lblVehiclegeofence.Text = platenoCounter
        ElseIf counter = 19 And tableSelected = True Then
            lblVehicleidlingprofile.Text = platenoCounter
        ElseIf counter = 20 And tableSelected = True Then
            lblVehicleincident.Text = platenoCounter
        ElseIf counter = 21 And tableSelected = True Then
            lblVehiclefuelsumm.Text = platenoCounter
        ElseIf counter = 22 And tableSelected = True Then
            lblVehicleidlingsumm.Text = platenoCounter
        ElseIf counter = 23 And tableSelected = True Then
            lblVehiclerefuelsumm.Text = platenoCounter
        ElseIf counter = 24 And tableSelected = True Then
            lblInstantalertsettings.Text = platenoCounter
        ElseIf counter = 25 And tableSelected = True Then
            lblSmspanicdispatchlist.Text = platenoCounter
        End If
    End Sub

    Protected Sub DisplayBlank(ByVal counter As String)
        If counter = 0 Then
            lbl1.Text = "-"
        ElseIf counter = 0 Then
            lbl1.Text = "-"
        ElseIf counter = 1 Then
            lbl2.Text = "-"
        ElseIf counter = 2 Then
            lbl3.Text = "-"
        ElseIf counter = 3 Then
            lbl4.Text = "-"
        ElseIf counter = 4 Then
            lbl5.Text = "-"
        ElseIf counter = 5 Then
            lblMaintenance.Text = "-"
        ElseIf counter = 6 Then
            lblGeofencetracked.Text = "-"
        ElseIf counter = 7 Then
            lblGeofencetripaudit.Text = "-"
        ElseIf counter = 8 Then
            lblFuel.Text = "-"
        ElseIf counter = 9 Then
            lblPanicinterval.Text = "-"
        ElseIf counter = 10 Then
            lblTollfare.Text = "-"
        ElseIf counter = 11 Then
            lblDocumentdate.Text = "-"
        ElseIf counter = 12 Then
            lblDriverassign.Text = "-"
        ElseIf counter = 13 Then
            lblOperatorchecklist.Text = "-"
        ElseIf counter = 14 Then
            lblTripreceipt.Text = "-"
        ElseIf counter = 15 Then
            lblVehicleaverageidling.Text = "-"
        ElseIf counter = 16 Then
            lblVehicleservicing.Text = "-"
        ElseIf counter = 17 Then
            lblVehicleg13edata.Text = "-"
        ElseIf counter = 18 Then
            lblVehiclegeofence.Text = "-"
        ElseIf counter = 19 Then
            lblVehicleidlingprofile.Text = "-"
        ElseIf counter = 20 Then
            lblVehicleincident.Text = "-"
        ElseIf counter = 21 Then
            lblVehiclefuelsumm.Text = "-"
        ElseIf counter = 22 Then
            lblVehicleidlingsumm.Text = "-"
        ElseIf counter = 23 Then
            lblVehiclerefuelsumm.Text = "-"
        ElseIf counter = 24 Then
            lblInstantalertsettings.Text = "-"
        ElseIf counter = 25 Then
            lblSmspanicdispatchlist.Text = "-"
        End If
    End Sub

    Protected Sub ddlUsername_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlUsername.SelectedIndexChanged
        getPlateNo(ddlUsername.SelectedValue)
    End Sub

    Protected Sub ibSubmit_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibSubmit.Click
        Try
            plateno.Attributes.Remove("onload")
            Dim userid As String = ddlUsername.SelectedValue
            Dim newPlateNo As String = txtNew.Text.Trim()
            Dim oldPlateNo As String = ddlpleate.SelectedValue
            For i As Int64 = 0 To cblTable.Items.Count - 1
                If cblTable.Items(i).Selected = True Then
                    updateTable(userid, cblTable.Items(i).Value, newPlateNo, oldPlateNo)
                End If
            Next
            InsertUpdateTmpTable(userid, newPlateNo, oldPlateNo)
            errormessage = "updated. plateno refreshed."
            getPlateNo(userid)
            'ddlpleate.SelectedValue = newPlateNo
        Catch ex As SystemException
            errormessage = ex.Message
        End Try
    End Sub
End Class
