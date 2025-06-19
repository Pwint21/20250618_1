Imports System.IO
Namespace AVLS

    Partial Class Logout
        Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub


        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()
        End Sub

#End Region

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If (Not Request.Cookies("userinfo") Is Nothing) Then
                    Dim myCookie As HttpCookie
                    myCookie = New HttpCookie("userinfo")
                    myCookie.Expires = DateTime.Now.AddDays(-1D)
                    Response.Cookies.Add(myCookie)
                End If
                Session.Clear()
                If (Not Request.Cookies("accesslevel") Is Nothing) Then
                    Dim myCookie As HttpCookie
                    myCookie = New HttpCookie("accesslevel")
                    myCookie.Expires = DateTime.Now.AddDays(-1D)
                    Response.Cookies.Add(myCookie)
                End If
            Catch ex As Exception
                Session.Clear()
                Response.Cookies.Clear()
                Response.Cookies.Remove("userinfo")
            End Try
            Response.Redirect("login.aspx")
        End Sub

    End Class

End Namespace

