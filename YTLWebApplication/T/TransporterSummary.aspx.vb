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
            ' SECURITY FIX: Enable authentication check
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If

        Catch ex As Exception
            SecurityHelper.LogError("TransporterSummary OnInit Error", ex, Server)
            Response.Redirect("Error.aspx")
        Finally
            MyBase.OnInit(e)
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Redirect("Login.aspx")
                Return
            End If

            ImageButton1.Attributes.Add("onclick", "return mysubmit()")
            If Page.IsPostBack = False Then
                txtBeginDate.Value = Now().AddDays(-1).ToString("yyyy/MM/dd")
            End If

        Catch ex As Exception
            SecurityHelper.LogError("TransporterSummary Page_Load Error", ex, Server)
            Response.Redirect("Error.aspx")
        End Try
    End Sub

    Protected Sub DisplayLogInformation()
        Try
            ' SECURITY FIX: Validate date input
            If Not SecurityHelper.ValidateDate(txtBeginDate.Value) Then
                Return
            End If

            Dim begindatetime As String = txtBeginDate.Value & " 00:00:00"
            Dim enddatetime As String = txtBeginDate.Value & " 23:59:59"
            
            ec = "true"
            
            Dim parameters As New Dictionary(Of String, Object) From {
                {"@begindtime", begindatetime},
                {"@enddtime", enddatetime}
            }

            ' Load Tanker data
            parameters.Add("@vtype", 0)
            Dim tankerQuery As String = "EXEC sp_GetPlantSummaryAPK @begindtime, @enddtime, @vtype"
            Dim tankerData As DataTable = SecurityHelper.ExecuteSecureQuery(tankerQuery, parameters)

            ' Load Cargo data
            parameters("@vtype") = 1
            Dim cargoQuery As String = "EXEC sp_GetPlantSummaryAPK @begindtime, @enddtime, @vtype"
            Dim cargoData As DataTable = SecurityHelper.ExecuteSecureQuery(cargoQuery, parameters)

            ' Process and bind data
            ProcessAndBindData(tankerData, cargoData)

        Catch ex As Exception
            SecurityHelper.LogError("DisplayLogInformation Error", ex, Server)
        End Try
    End Sub

    Private Sub ProcessAndBindData(tankerData As DataTable, cargoData As DataTable)
        Try
            ' Remove unnecessary columns and rename
            If tankerData.Columns.Count > 2 Then
                tankerData.Columns.RemoveAt(2)
                tankerData.Columns.RemoveAt(0)
                tankerData.Columns("geoname").ColumnName = "TANKER"
            End If

            If cargoData.Columns.Count > 2 Then
                cargoData.Columns.RemoveAt(2)
                cargoData.Columns.RemoveAt(0)
                cargoData.Columns("geoname").ColumnName = "CARGO"
            End If

            gvCargo.DataSource = cargoData
            gvTanker.DataSource = tankerData

            gvCargo.DataBind()
            gvTanker.DataBind()

            ' Set border styles for first two rows
            If gvCargo.Rows.Count > 1 Then
                gvCargo.Rows(0).BorderStyle = BorderStyle.Double
                gvCargo.Rows(1).BorderStyle = BorderStyle.Double
            End If

            If gvTanker.Rows.Count > 1 Then
                gvTanker.Rows(0).BorderStyle = BorderStyle.Double
                gvTanker.Rows(1).BorderStyle = BorderStyle.Double
            End If

            Session.Remove("exceltable")
            Session.Remove("exceltable2")
            Session("exceltable") = cargoData
            Session("exceltable2") = tankerData

        Catch ex As Exception
            SecurityHelper.LogError("ProcessAndBindData Error", ex, Server)
        End Try
    End Sub

    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ImageButton1.Click
        DisplayLogInformation()
    End Sub

    Protected Sub gvCargo_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        Try
            If e.Row.RowType = DataControlRowType.DataRow Then
                For i As Integer = 0 To e.Row.Cells.Count - 1
                    Dim encoded As String = SecurityHelper.HtmlEncode(e.Row.Cells(i).Text)
                    e.Row.Cells(i).Text = encoded
                Next
            End If
        Catch ex As Exception
            SecurityHelper.LogError("gvCargo_RowDataBound Error", ex, Server)
        End Try
    End Sub
    
    Protected Sub gvTanker_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        Try
            If e.Row.RowType = DataControlRowType.DataRow Then
                For i As Integer = 0 To e.Row.Cells.Count - 1
                    Dim encoded As String = SecurityHelper.HtmlEncode(e.Row.Cells(i).Text)
                    e.Row.Cells(i).Text = encoded
                Next
            End If
        Catch ex As Exception
            SecurityHelper.LogError("gvTanker_RowDataBound Error", ex, Server)
        End Try
    End Sub
End Class