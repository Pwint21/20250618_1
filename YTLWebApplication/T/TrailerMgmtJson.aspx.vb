Imports System.Data.SqlClient 
Imports System.Collections.Generic
Imports Newtonsoft.Json

Partial Class TrailerMgmtJson
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim opr As String = Request.QueryString("opr")
        Dim userid As String = Request.QueryString("u")
        Dim uid As String = Request.QueryString("uid")
        Dim id As String = Request.QueryString("id")
        Dim role As String = Request.QueryString("r")
        Dim ulist As String = Request.QueryString("lst")
        Dim trilerno As String = Request.QueryString("tname")
        Dim inspection As String = Request.QueryString("insdatetime")
        Dim rtax As String = Request.QueryString("rtax")
        Dim ptest As String = Request.QueryString("pt")
        Dim ins As String = Request.QueryString("Insu") 
        Dim emailid As String = Request.QueryString("em1")
        Dim emailid2 As String = Request.QueryString("emlcc")
        Dim reamrks As String = Request.QueryString("rem")
        Dim ugData As String = Request.QueryString("ugData")
        If opr = "1" Then
            FillVehiclesGrid(userid, role, ulist)
        ElseIf opr = 2 Then
            AddData(uid, trilerno, inspection, emailid, emailid2, rtax, ptest, ins)
        ElseIf opr = 3 Then
            UpdateData(id, inspection, trilerno, uid, emailid, emailid2, rtax, ptest, ins)
        ElseIf opr = 4 Then
            DeleteData(ugData)
        End If
    End Sub

    Public Function FillVehiclesGrid(ByVal ugData As String, ByVal role As String, ByVal userslist As String) As String
        Dim aa As New ArrayList()
        Dim a As ArrayList

        Dim r As DataRow
        Dim t As New DataTable
        t.Columns.Add(New DataColumn("chk"))
        t.Columns.Add(New DataColumn("S NO"))
        t.Columns.Add(New DataColumn("Name"))
        t.Columns.Add(New DataColumn("insdate"))
        t.Columns.Add(New DataColumn("taxdate"))
        t.Columns.Add(New DataColumn("pusdate"))
        t.Columns.Add(New DataColumn("insudate"))
        t.Columns.Add(New DataColumn("unm"))
        t.Columns.Add(New DataColumn("eml1"))
        t.Columns.Add(New DataColumn("eml2"))
        Dim userid As String = HttpContext.Current.Request.Cookies("userinfo")("userid")
        Dim rle As String = HttpContext.Current.Request.Cookies("userinfo")("role")
        Dim ulist As String = HttpContext.Current.Request.Cookies("userinfo")("userslist")

        Dim query As String = "select emailid1,emailid2,id,trailerNo,inspectionDate,roadtax,puspakom,insurance,u.username,u.userid from trailer t left outer join userTBL u on u.userid=t.userid"
        If rle = "User" Then
            query = "select emailid1,emailid2,id,trailerNo,inspectionDate,roadtax,puspakom,insurance,u.username,u.userid from (select * from trailer where userid='" & userid & "') t left outer join userTBL u on u.userid=t.userid"
        ElseIf rle = "SuperUser" Or rle = "Operator" Then
            query = "select emailid1,emailid2,id,trailerNo,inspectionDate,roadtax,puspakom,insurance,u.username,u.userid from (select * from trailer where  userid in (" & ulist & ")) t left outer join userTBL u on u.userid=t.userid"
        End If

        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim cmd As New SqlCommand(query, conn)
        Try
            conn.Open()
            Dim dr As SqlDataReader = cmd.ExecuteReader()
            Dim i As Int32 = 1
            Dim insDateTime As String = ""
            While (dr.Read())
                Try
                    insDateTime = Convert.ToDateTime(dr("inspectionDate")).ToString("yyyy/MM/dd")
                    r = t.NewRow
                    r(0) = "<input type=""checkbox"" name=""chk"" class=""group1"" value=""" & dr("id") & """/>"
                    r(1) = i.ToString()

                    r(8) = dr("emailid1").ToString()
                    If Not IsDBNull(dr("emailid2")) Then
                        r(9) = dr("emailid2").ToString()
                    Else
                        r(6) = ""
                    End If

                    r(3) = insDateTime
                    If Not IsDBNull(dr("roadtax")) Then
                        If dr("roadtax") = "1900-01-01 00:00:00.000" Then
                            r(4) = ""
                        Else
                            r(4) = Convert.ToDateTime(dr("roadtax")).ToString("yyyy/MM/dd")
                        End If
                    Else
                        r(4) = ""
                    End If
                    If Not IsDBNull(dr("puspakom")) Then
                        If dr("puspakom") = "1900-01-01 00:00:00.000" Then
                            r(5) = ""
                        Else
                            r(5) = Convert.ToDateTime(dr("puspakom")).ToString("yyyy/MM/dd")
                        End If
                    Else
                        r(5) = ""
                    End If
                    If Not IsDBNull(dr("insurance")) Then
                        If dr("insurance") = "1900-01-01 00:00:00.000" Then
                            r(6) = ""
                        Else
                            r(6) = Convert.ToDateTime(dr("insurance")).ToString("yyyy/MM/dd")
                        End If
                    Else
                        r(6) = ""
                    End If
                    r(2) = "<span style='cursor:pointer;text-decoration:underline;' onclick=""javascript :openPopup('" & dr("id") & "','" & dr("trailerNo") & "','" & insDateTime & "','" & r(4) & "','" & r(5) & "','" & r(6) & "','" & dr("userid") & "','" & r(8) & "','" & r(9) & "')"">" & dr("trailerNo").ToString.ToUpper() & "</span>"

                    r(7) = dr("username").ToString.ToUpper()

                    t.Rows.Add(r)
                    i = i + 1
                Catch ex As Exception

                End Try
            End While
            If t.Rows.Count = 0 Then
                r = t.NewRow
                r(0) = "--"
                r(1) = "--"
                r(2) = "--"
                r(3) = "--"
                r(4) = "--"
                r(5) = "--"
                r(6) = "--"
                r(7) = "--"
                r(8) = "--"
                r(9) = "--"
                t.Rows.Add(r)
            End If
            Session("exceltable") = t
            For j As Integer = 0 To t.Rows.Count - 1
                Try
                    a = New ArrayList
                    a.Add(t.DefaultView.Item(j)(0))
                    a.Add(t.DefaultView.Item(j)(1))
                    a.Add(t.DefaultView.Item(j)(2))
                    a.Add(t.DefaultView.Item(j)(3))
                    a.Add(t.DefaultView.Item(j)(4))
                    a.Add(t.DefaultView.Item(j)(5))
                    a.Add(t.DefaultView.Item(j)(6))
                    a.Add(t.DefaultView.Item(j)(7))
                    a.Add(t.DefaultView.Item(j)(8))
                    a.Add(t.DefaultView.Item(j)(9))
                    aa.Add(a)
                Catch ex As Exception

                End Try
            Next

            Dim json As String = JsonConvert.SerializeObject(aa, Formatting.None)
            Response.Write(json)


        Catch ex As Exception

        End Try
    End Function

    Public Sub AddData(ByVal uid As String, ByVal tname As String, ByVal insdatetime As String, ByVal eml1 As String, ByVal emlcc As String, ByVal raodtax As String, ByVal puspakam As String, ByVal insurence As String)
        Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
        Dim result As String = "0"
        Dim cmd As SqlCommand
        Try
            conn.Open()
            If raodtax <> "" Then
                raodtax = Convert.ToDateTime(raodtax).ToString("yyyy/MM/dd 00:00:00")
            End If
            If puspakam <> "" Then

                puspakam = Convert.ToDateTime(puspakam).ToString("yyyy/MM/dd 00:00:00")
            End If
            If insurence <> "" Then
                insurence = Convert.ToDateTime(insurence).ToString("yyyy/MM/dd 00:00:00")
            End If
            cmd = New SqlCommand("insert into trailer(trailerNo,inspectionDate, RoadTax, Puspakom ,Insurance,userid,emailid1,emailid2) values('" & tname & "','" & Convert.ToDateTime(insdatetime).ToString("yyyy/MM/dd 00:00:00") & "','" & raodtax & "','" & puspakam & "','" & insurence & "','" & uid & "','" & eml1 & "','" & emlcc & "')", conn)
            result = cmd.ExecuteNonQuery()
            If (result > 0) Then
                Response.Write("Yes")
            Else
                Response.Write("No")
            End If
        Catch ex As Exception
            Response.Write(cmd.CommandText)
        Finally
            conn.Close()
        End Try

    End Sub


    Public Sub UpdateData(ByVal id As String, ByVal insdatetime As String, ByVal tname As String, ByVal uid As String, ByVal eml1 As String, ByVal emlcc As String, ByVal raodtax As String, ByVal puspakam As String, ByVal insurence As String)
        Dim result As String = "0"
        Try
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))

            If raodtax <> "" Then
                raodtax = Convert.ToDateTime(raodtax).ToString("yyyy/MM/dd 00:00:00")
            End If
            If puspakam <> "" Then
                puspakam = Convert.ToDateTime(puspakam).ToString("yyyy/MM/dd 00:00:00")
            End If
            If insurence <> "" Then
                insurence = Convert.ToDateTime(insurence).ToString("yyyy/MM/dd 00:00:00")
            End If
            Dim cmd As New SqlCommand("update  trailer set trailerNo='" & tname & "',emailid1='" & eml1 & "',emailid2='" & emlcc & "',inspectionDate='" & Convert.ToDateTime(insdatetime).ToString("yyyy/MM/dd 00:00:00") & "',RoadTax='" & raodtax & "', Puspakom='" & puspakam & "',Insurance='" & insurence & "', userid='" & uid & "'  where id='" & id & "'", conn)

            Try
                conn.Open()
                result = cmd.ExecuteNonQuery().ToString()
                If (result > 0) Then
                    Response.Write("Yes")
                Else
                    Response.Write("No")
                End If
            Catch ex As Exception
                Response.Write(ex.Message)
            Finally
                conn.Close()
            End Try
        Catch ex As Exception

        End Try

    End Sub


    Public Sub DeleteData(ByVal ugData As String)
        Dim res As String = "0"
        Try
            Dim conn As New SqlConnection(System.Configuration.ConfigurationManager.AppSettings("sqlserverconnection"))
            Dim cmd As SqlCommand
            Dim Tides() As String = ugData.Split(",")
            For i As Int32 = 0 To Tides.Length - 1
                Try
                    conn.Open()
                    If Tides(i) = "on" Then

                    Else
                        cmd = New SqlCommand("delete from trailer where id='" & Tides(i) & "'", conn)
                        res = cmd.ExecuteNonQuery().ToString()
                    End If
                Catch ex As Exception
                    res = ex.Message
                Finally
                    conn.Close()
                End Try
            Next
            Response.Write("Yes")
        Catch ex As Exception
            res = ex.Message
        End Try 
    End Sub
End Class
