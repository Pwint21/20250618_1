Imports System.Data.SqlClient
Imports System.Data

Namespace AVLS
    Partial Class GeofenceTypeManagment
        Inherits System.Web.UI.Page

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            Try

                If Request.Cookies("userinfo") Is Nothing Then
                    Response.Redirect("Login.aspx")
                End If

                Dim userid As String = Request.Cookies("userinfo")("userid")
                Dim role As String = Request.Cookies("userinfo")("role")
                Dim userslist As String = Request.Cookies("userinfo")("userslist")




            Catch ex As Exception


            End Try
            MyBase.OnInit(e)
        End Sub


        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Try

                If Page.IsPostBack = False Then
                    ImageButton1.Attributes.Add("onclick", "return deleteconfirmation();")
                    ImageButton2.Attributes.Add("onclick", "return deleteconfirmation();")
                    FillGrid()
                End If

            Catch ex As Exception

            End Try
        End Sub

        Private Sub FillGrid()
            Try


                Dim groupstable As New DataTable

                groupstable.Columns.Add(New DataColumn("sno"))
                groupstable.Columns.Add(New DataColumn("geofencetype"))
                groupstable.Columns.Add(New DataColumn("username"))
                groupstable.Columns.Add(New DataColumn("lastmodified"))

                Dim r As DataRow

                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))


                Dim cmd As SqlCommand = New SqlCommand("select geofencetype,id,isnull(t2.username,'-') username,modifiedDT from geofence_type t1 left outer join usertbl t2 on t1.userid=t2.userid", conn)
                    Dim dr As SqlDataReader




                    conn.Open()
                    dr = cmd.ExecuteReader()
                    Dim i As Int32 = 1
                    While dr.Read
                        r = groupstable.NewRow

                        r(0) = i.ToString()
                        r(1) = " <a href= UpdateGeofenceType.aspx?gid=" & dr("id") & "> " & dr("geofencetype") & " </a>"
                        r(2) = dr("username")
                        r(3) = Convert.ToDateTime(dr("modifiedDT")).ToString("yyyy/MM/dd HH:mm:ss")
                        groupstable.Rows.Add(r)
                        i = i + 1
                    End While

                    conn.Close()


                If groupstable.Rows.Count = 0 Then
                    r = groupstable.NewRow
                    r(0) = "--"
                    r(1) = "--"
                    r(2) = "--"
                    r(3) = "--"
                    groupstable.Rows.Add(r)
                End If

                groupsgrid.DataSource = groupstable
                groupsgrid.DataBind()

            Catch ex As Exception

            End Try
        End Sub

        Protected Sub ImageButton1_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton1.Click
            DeleteGroup()
        End Sub

        Protected Sub ImageButton2_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImageButton2.Click
            DeleteGroup()
        End Sub

        Protected Sub DeleteGroup()
            Try
                Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
                Dim cmd As SqlCommand
                Dim unitid As String = ""

                Dim groupides() As String = Request.Form("chk").Split(",")

                For i As Int32 = 0 To groupides.Length - 1

                    cmd = New SqlCommand("delete from vehicle_group where groupid='" & groupides(i) & "'", conn)
                    Try
                        conn.Open()
                        cmd.ExecuteNonQuery()
                    Catch ex As Exception

                    Finally
                        conn.Close()
                    End Try

                Next
                FillGrid()

            Catch ex As Exception

            End Try
        End Sub


    End Class

End Namespace