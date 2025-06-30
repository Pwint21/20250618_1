Imports System.Data.SqlClient 
Imports System.Collections.Generic
Imports Newtonsoft.Json

Partial Class TrailerMgmtJson
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Try
            ' SECURITY FIX: Validate session
            If Not SecurityHelper.ValidateUserSession(Request, Session) Then
                Response.Write(JsonConvert.SerializeObject(New With {.error = "Unauthorized"}))
                Return
            End If

            ' SECURITY FIX: Validate and sanitize input parameters
            Dim opr As String = SecurityHelper.ValidateInput(Request.QueryString("opr"), "numeric")
            If String.IsNullOrEmpty(opr) Then
                Response.Write(JsonConvert.SerializeObject(New With {.error = "Invalid operation"}))
                Return
            End If

            Select Case opr
                Case "1"
                    FillVehiclesGrid()
                Case "2"
                    AddTrailerData()
                Case "3"
                    UpdateTrailerData()
                Case "4"
                    DeleteTrailerData()
                Case Else
                    Response.Write(JsonConvert.SerializeObject(New With {.error = "Invalid operation"}))
            End Select

        Catch ex As Exception
            SecurityHelper.LogError("TrailerMgmtJson Error", ex, Server)
            Response.Write(JsonConvert.SerializeObject(New With {.error = "Server error"}))
        End Try
    End Sub

    Private Sub FillVehiclesGrid()
        Try
            Dim userid As String = SecurityHelper.ValidateAndGetUserId(Request)
            Dim role As String = SecurityHelper.ValidateAndGetUserRole(Request)
            Dim userslist As String = SecurityHelper.ValidateAndGetUsersList(Request)

            Dim parameters As New Dictionary(Of String, Object)
            Dim query As String = ""

            If role = "User" Then
                query = "SELECT emailid1, emailid2, id, trailerNo, inspectionDate, roadtax, puspakom, insurance, u.username, u.userid FROM trailer t LEFT OUTER JOIN userTBL u ON u.userid = t.userid WHERE t.userid = @userid"
                parameters.Add("@userid", userid)
            ElseIf role = "SuperUser" Or role = "Operator" Then
                If SecurityHelper.IsValidUsersList(userslist) Then
                    Dim userIds() As String = userslist.Split(","c)
                    Dim paramNames As New List(Of String)
                    
                    For i As Integer = 0 To userIds.Length - 1
                        Dim paramName As String = "@userid" & i
                        paramNames.Add(paramName)
                        parameters.Add(paramName, userIds(i).Trim())
                    Next
                    
                    query = $"SELECT emailid1, emailid2, id, trailerNo, inspectionDate, roadtax, puspakom, insurance, u.username, u.userid FROM trailer t LEFT OUTER JOIN userTBL u ON u.userid = t.userid WHERE t.userid IN ({String.Join(",", paramNames)})"
                End If
            Else
                query = "SELECT emailid1, emailid2, id, trailerNo, inspectionDate, roadtax, puspakom, insurance, u.username, u.userid FROM trailer t LEFT OUTER JOIN userTBL u ON u.userid = t.userid"
            End If

            Dim trailerData As DataTable = SecurityHelper.ExecuteSecureQuery(query, parameters)
            Dim result As New List(Of Object())

            For Each row As DataRow In trailerData.Rows
                Dim trailerArray(9) As Object
                trailerArray(0) = $"<input type=""checkbox"" name=""chk"" class=""group1"" value=""{SecurityHelper.HtmlEncode(row("id").ToString())}""/>"
                trailerArray(1) = result.Count + 1
                trailerArray(2) = $"<span style='cursor:pointer;text-decoration:underline;' onclick=""javascript:openPopup('{SecurityHelper.HtmlEncode(row("id").ToString())}','{SecurityHelper.HtmlEncode(row("trailerNo").ToString())}','{SecurityHelper.HtmlEncode(Convert.ToDateTime(row("inspectionDate")).ToString("yyyy/MM/dd"))}','{SecurityHelper.HtmlEncode(If(IsDBNull(row("roadtax")), "", Convert.ToDateTime(row("roadtax")).ToString("yyyy/MM/dd")))}','{SecurityHelper.HtmlEncode(If(IsDBNull(row("puspakom")), "", Convert.ToDateTime(row("puspakom")).ToString("yyyy/MM/dd")))}','{SecurityHelper.HtmlEncode(If(IsDBNull(row("insurance")), "", Convert.ToDateTime(row("insurance")).ToString("yyyy/MM/dd")))}','{SecurityHelper.HtmlEncode(row("userid").ToString())}','{SecurityHelper.HtmlEncode(row("emailid1").ToString())}','{SecurityHelper.HtmlEncode(If(IsDBNull(row("emailid2")), "", row("emailid2").ToString()))}')"">{SecurityHelper.HtmlEncode(row("trailerNo").ToString().ToUpper())}</span>"
                trailerArray(3) = SecurityHelper.HtmlEncode(Convert.ToDateTime(row("inspectionDate")).ToString("yyyy/MM/dd"))
                trailerArray(4) = SecurityHelper.HtmlEncode(If(IsDBNull(row("roadtax")), "", Convert.ToDateTime(row("roadtax")).ToString("yyyy/MM/dd")))
                trailerArray(5) = SecurityHelper.HtmlEncode(If(IsDBNull(row("puspakom")), "", Convert.ToDateTime(row("puspakom")).ToString("yyyy/MM/dd")))
                trailerArray(6) = SecurityHelper.HtmlEncode(If(IsDBNull(row("insurance")), "", Convert.ToDateTime(row("insurance")).ToString("yyyy/MM/dd")))
                trailerArray(7) = SecurityHelper.HtmlEncode(row("username").ToString().ToUpper())
                trailerArray(8) = SecurityHelper.HtmlEncode(row("emailid1").ToString())
                trailerArray(9) = SecurityHelper.HtmlEncode(If(IsDBNull(row("emailid2")), "", row("emailid2").ToString()))
                
                result.Add(trailerArray)
            Next

            If result.Count = 0 Then
                Dim emptyArray(9) As Object
                For i As Integer = 0 To 9
                    emptyArray(i) = "--"
                Next
                result.Add(emptyArray)
            End If

            Session("exceltable") = trailerData
            Response.Write(JsonConvert.SerializeObject(result))

        Catch ex As Exception
            SecurityHelper.LogError("FillVehiclesGrid Error", ex, Server)
            Response.Write(JsonConvert.SerializeObject(New With {.error = "Data retrieval failed"}))
        End Try
    End Sub

    Private Sub AddTrailerData()
        Try
            ' SECURITY FIX: Validate all input parameters
            Dim uid As String = SecurityHelper.ValidateInput(Request.QueryString("uid"), "numeric")
            Dim tname As String = SecurityHelper.ValidateInput(Request.QueryString("tname"), "plateno")
            Dim insdatetime As String = Request.QueryString("insdatetime")
            Dim eml1 As String = Request.QueryString("em1")
            Dim emlcc As String = Request.QueryString("emlcc")
            Dim roadtax As String = Request.QueryString("rtax")
            Dim puspakam As String = Request.QueryString("pt")
            Dim insurance As String = Request.QueryString("insu")

            ' Validate required fields
            If String.IsNullOrEmpty(uid) OrElse String.IsNullOrEmpty(tname) OrElse String.IsNullOrEmpty(insdatetime) Then
                Response.Write("No")
                Return
            End If

            ' Validate dates
            If Not SecurityHelper.ValidateDate(insdatetime) Then
                Response.Write("No")
                Return
            End If

            ' Validate email
            If Not String.IsNullOrEmpty(eml1) AndAlso Not IsValidEmail(eml1) Then
                Response.Write("No")
                Return
            End If

            Dim parameters As New Dictionary(Of String, Object) From {
                {"@trailerNo", tname},
                {"@inspectionDate", Convert.ToDateTime(insdatetime)},
                {"@userid", uid},
                {"@emailid1", If(String.IsNullOrEmpty(eml1), DBNull.Value, eml1)},
                {"@emailid2", If(String.IsNullOrEmpty(emlcc), DBNull.Value, emlcc)},
                {"@roadtax", If(String.IsNullOrEmpty(roadtax), DBNull.Value, Convert.ToDateTime(roadtax))},
                {"@puspakom", If(String.IsNullOrEmpty(puspakam), DBNull.Value, Convert.ToDateTime(puspakam))},
                {"@insurance", If(String.IsNullOrEmpty(insurance), DBNull.Value, Convert.ToDateTime(insurance))}
            }

            Dim query As String = "INSERT INTO trailer(trailerNo, inspectionDate, RoadTax, Puspakom, Insurance, userid, emailid1, emailid2) VALUES(@trailerNo, @inspectionDate, @roadtax, @puspakom, @insurance, @userid, @emailid1, @emailid2)"
            
            Dim result As Integer = SecurityHelper.ExecuteSecureNonQuery(query, parameters)
            Response.Write(If(result > 0, "Yes", "No"))

        Catch ex As Exception
            SecurityHelper.LogError("AddTrailerData Error", ex, Server)
            Response.Write("No")
        End Try
    End Sub

    Private Sub UpdateTrailerData()
        Try
            ' SECURITY FIX: Validate all input parameters
            Dim id As String = SecurityHelper.ValidateInput(Request.QueryString("id"), "numeric")
            Dim insdatetime As String = Request.QueryString("insdatetime")
            Dim tname As String = SecurityHelper.ValidateInput(Request.QueryString("tname"), "plateno")
            Dim uid As String = SecurityHelper.ValidateInput(Request.QueryString("uid"), "numeric")
            Dim eml1 As String = Request.QueryString("em1")
            Dim emlcc As String = Request.QueryString("emlcc")
            Dim roadtax As String = Request.QueryString("rtax")
            Dim puspakam As String = Request.QueryString("pt")
            Dim insurance As String = Request.QueryString("insu")

            ' Validate required fields
            If String.IsNullOrEmpty(id) OrElse String.IsNullOrEmpty(tname) OrElse String.IsNullOrEmpty(insdatetime) Then
                Response.Write("No")
                Return
            End If

            ' Validate dates
            If Not SecurityHelper.ValidateDate(insdatetime) Then
                Response.Write("No")
                Return
            End If

            Dim parameters As New Dictionary(Of String, Object) From {
                {"@id", id},
                {"@trailerNo", tname},
                {"@inspectionDate", Convert.ToDateTime(insdatetime)},
                {"@userid", uid},
                {"@emailid1", If(String.IsNullOrEmpty(eml1), DBNull.Value, eml1)},
                {"@emailid2", If(String.IsNullOrEmpty(emlcc), DBNull.Value, emlcc)},
                {"@roadtax", If(String.IsNullOrEmpty(roadtax), DBNull.Value, Convert.ToDateTime(roadtax))},
                {"@puspakom", If(String.IsNullOrEmpty(puspakam), DBNull.Value, Convert.ToDateTime(puspakam))},
                {"@insurance", If(String.IsNullOrEmpty(insurance), DBNull.Value, Convert.ToDateTime(insurance))}
            }

            Dim query As String = "UPDATE trailer SET trailerNo = @trailerNo, emailid1 = @emailid1, emailid2 = @emailid2, inspectionDate = @inspectionDate, RoadTax = @roadtax, Puspakom = @puspakom, Insurance = @insurance, userid = @userid WHERE id = @id"
            
            Dim result As Integer = SecurityHelper.ExecuteSecureNonQuery(query, parameters)
            Response.Write(If(result > 0, "Yes", "No"))

        Catch ex As Exception
            SecurityHelper.LogError("UpdateTrailerData Error", ex, Server)
            Response.Write("No")
        End Try
    End Sub

    Private Sub DeleteTrailerData()
        Try
            Dim ugData As String = Request.QueryString("ugData")
            If String.IsNullOrEmpty(ugData) Then
                Response.Write("No")
                Return
            End If

            Dim trailerIds() As String = ugData.Split(","c)
            Dim deletedCount As Integer = 0

            For Each trailerId As String In trailerIds
                If trailerId <> "on" AndAlso SecurityHelper.ValidateInput(trailerId, "numeric") Then
                    Dim parameters As New Dictionary(Of String, Object) From {
                        {"@id", trailerId}
                    }
                    
                    Dim query As String = "DELETE FROM trailer WHERE id = @id"
                    Dim result As Integer = SecurityHelper.ExecuteSecureNonQuery(query, parameters)
                    If result > 0 Then
                        deletedCount += 1
                    End If
                End If
            Next

            Response.Write(If(deletedCount > 0, "Yes", "No"))

        Catch ex As Exception
            SecurityHelper.LogError("DeleteTrailerData Error", ex, Server)
            Response.Write("No")
        End Try
    End Sub

    Private Function IsValidEmail(email As String) As Boolean
        Try
            Dim emailRegex As New System.Text.RegularExpressions.Regex("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
            Return emailRegex.IsMatch(email)
        Catch
            Return False
        End Try
    End Function
End Class