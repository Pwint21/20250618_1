Imports System.Data.SqlClient
Imports System.Data

Partial Class UserPlantManagement
    Inherits System.Web.UI.Page

    Public message As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            If Request.Cookies("userinfo") Is Nothing Then
                Response.Redirect("Login.aspx")
            End If
            Dim userid As String = Request.Cookies("userinfo")("userid")
            ImageButton1.Attributes.Add("onclick", "return mysubmit()")
            If Page.IsPostBack = False Then
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim da As SqlDataAdapter
                Dim role = Request.Cookies("userinfo")("role")
                Dim userslist As String = Request.Cookies("userinfo")("userslist")
                da = New SqlDataAdapter("select userid,username from userTBL where companyname='YTL cement' order by username", conn)
                Dim ds As New DataSet()
                da.Fill(ds)
                If ds.Tables(0).Rows.Count > 0 Then
                    Dim i As Integer
                    For i = 0 To ds.Tables(0).Rows.Count - 1
                        ddluser.Items.Add(New ListItem(ds.Tables(0).Rows(i).Item("username"), ds.Tables(0).Rows(i).Item("userid")))
                    Next
                End If
            Else

            End If

        Catch ex As Exception
            message = ex.Message
        End Try
    End Sub

    Protected Sub ddluser_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddluser.SelectedIndexChanged
        Fill()
    End Sub

    Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
        Try
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
            Dim userslist As String = Request.Form("userslist")

            Dim cmd As SqlCommand = New SqlCommand("sp_UpdatePlantAssign", conn)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@userid", ddluser.SelectedValue)
            cmd.Parameters.AddWithValue("@plantids", userslist)
            conn.Open()
            cmd.ExecuteNonQuery()
            conn.Close()
            Fill()
            message = "Successfully updated"
        Catch ex As Exception
            message = ex.Message
            Response.Write(message)
        End Try
    End Sub

    Protected Sub Fill()
        Try
            If ddluser.SelectedValue <> "--Select User--" Then

                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection2"))
                Dim cmd As SqlCommand = New SqlCommand("select * from dbo.fn_GetAssignedPlants(" & ddluser.SelectedValue & ")", conn)
                Dim dr As SqlDataReader
                conn.Open()
                dr = cmd.ExecuteReader()
                While dr.Read()
                    ListBox2.Items.Add(New ListItem(dr("PV_DisplayName"), dr("id")))
                End While
                cmd = New SqlCommand("select PV_DisplayName ,id  from oss_plant_master  where id not in (select id from dbo.fn_GetAssignedPlants(" & ddluser.SelectedValue & "))", conn)
                dr = cmd.ExecuteReader()

                While dr.Read()
                    ListBox1.Items.Add(New ListItem(dr("PV_DisplayName"), dr("id")))
                End While

            End If
        Catch ex As Exception
            message = ex.Message
        End Try
    End Sub
End Class
