Imports System.Data
Imports System.Data.SqlClient

Partial Class TransporterSummary
    Inherits System.Web.UI.Page
    Public ec As String = "false"
    Public show As Boolean = False
    Public sb1 As New StringBuilder()
    Public sb2 As New StringBuilder()
    Public tot As Int16 = 0
    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        Try

            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If
        Catch ex As Exception

        Finally
            MyBase.OnInit(e)
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If

            ImageButton1.Attributes.Add("onclick", "return mysubmit()")
            If Page.IsPostBack = False Then
                txtBeginDate.Value = Now().AddDays(-1).ToString("yyyy/MM/dd")
            End If

        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub

    Protected Sub DisplayLogInformation()
        Try
            Dim begindatetime As String = txtBeginDate.Value & " 00:00:00" ' & ddlbh.SelectedValue & ":" & ddlbm.SelectedValue & ":00"
            Dim enddatetime As String = txtBeginDate.Value & " 23:59:59" ' & ddleh.SelectedValue & ":" & ddlem.SelectedValue & ":59"
            Dim colnms As String = ""
            Dim geonames As String = ""
            Dim t As New DataTable
            t.Columns.Add(New DataColumn("Username"))
            t.Columns.Add(New DataColumn("Plate No"))
            t.Columns.Add(New DataColumn("counter"))
            t.Columns.Add(New DataColumn("geofencename"))
            ec="true"
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand = New SqlCommand("sp_GetPlantSummaryAPK", conn)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@begindtime", begindatetime)
            cmd.Parameters.AddWithValue("@enddtime", enddatetime)
            cmd.Parameters.AddWithValue("@vtype", 0)
            Dim da As New SqlDataAdapter(cmd)
            Dim ds As New DataSet
            da.Fill(ds, "Tanker")

            cmd = New SqlCommand("sp_GetPlantSummaryAPK", conn)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@begindtime", begindatetime)
            cmd.Parameters.AddWithValue("@enddtime", enddatetime)
            cmd.Parameters.AddWithValue("@vtype", 1)
            da = New SqlDataAdapter(cmd)
            da.Fill(ds, "CARGO")

            Dim dtt As DataTable = ds.Tables(0)
            Dim dtC As DataTable = ds.Tables(1)
            dtt.Columns.RemoveAt(2)
            dtC.Columns.RemoveAt(2)

            dtt.Columns.RemoveAt(0)
            dtC.Columns.RemoveAt(0)

            dtC.Columns("geoname").ColumnName = "CARGO"
            dtt.Columns("geoname").ColumnName = "TANKER"

            gvCargo.DataSource = dtC
            gvTanker.DataSource = dtt

            gvCargo.DataBind()
            gvTanker.DataBind()


            gvCargo.Rows(0).BorderStyle = BorderStyle.Double
            gvTanker.Rows(0).BorderStyle = BorderStyle.Double

            gvCargo.Rows(1).BorderStyle = BorderStyle.Double
            gvTanker.Rows(1).BorderStyle = BorderStyle.Double
            Session.Remove("exceltable")

            Session("exceltable") = dtC
            Session("exceltable2") = dtt

        Catch ex As Exception
            Response.Write(ex.Message)
        End Try

    End Sub

    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ImageButton1.Click
        DisplayLogInformation()
    End Sub

    Protected Sub gvCargo_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            For i As Integer = 0 To e.Row.Cells.Count - 1
                Dim encoded As String = e.Row.Cells(i).Text
                e.Row.Cells(i).Text = Context.Server.HtmlDecode(encoded)
            Next
            Try
                If e.Row.RowType = DataControlRowType.Footer Then
                    For Each row As GridViewRow In gvCargo.Rows
                        For i As Integer = 6 To gvCargo.Columns.Count
                            If Not String.IsNullOrEmpty(row.Cells(i).Text) Then
                                tot = tot + Convert.ToInt32(row.Cells(i).Text)
                            End If

                            Dim label As Label = New Label()
                            e.Row.Cells(i).Text = tot.ToString()
                            label.Text = "Total" & " " & tot
                            e.Row.Cells(i).Controls.Add(label)
                        Next
                    Next
                End If
                'tot = tot + Convert.ToInt32(e.Row.Cells(1).Text)
            Catch ex As Exception
                Response.Write(ex.Message)
            End Try

        End If
    End Sub
    Protected Sub gvTanker_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            For i As Integer = 0 To e.Row.Cells.Count - 1
                Dim encoded As String = e.Row.Cells(i).Text
                e.Row.Cells(i).Text = Context.Server.HtmlDecode(encoded)
            Next
        End If
    End Sub
End Class
