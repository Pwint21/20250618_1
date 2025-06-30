<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.uploadFuelExcel" Codebehind="uploadFuelExcel.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Upload Shell Fuel</title>
    <link href="cssfiles/style_avls.css" type="text/css" rel="stylesheet">
</head>
<body>
    <form id="frmUpload" enctype="multipart/form-data" runat="server">
        <center>
            <div>
                <br />
                <img src="images/UploadShellFuel.jpg" alt="Upload Shell Fuel" />
                <!-- <h1>Upload Shell Fuel</h1>  -->
                <br />
                <table style="width: 450px;">
                    <tr>
                        <td align="center">
                            <table>
                                <tr>
                                    <td class="tbl_frmFilter_head">Upload Shell Fuel Information :</td>
                                </tr>
                                <tr>
                                    <td>
                                        <table class="tbl_frmFilter" style="width: 420px;">
                                            <tr>
                                                <td>User Name</td>
                                                <td>:</td>   
                                                <td>
                                                <asp:DropDownList ID="ddlusername" runat="server" Width="200px" Font-Size="12px" Font-Names="verdana"
                                                    EnableViewState="true">
                                                    <asp:ListItem>--Select User Name--</asp:ListItem>
                                                </asp:DropDownList>
                                                &nbsp;&nbsp;&nbsp;<a href="FuelReceipt.aspx">Edit/Delete</a>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="white-space: nowrap;">Select Excel File</td>
                                                <td>:</td>
                                                <td>
                                                <asp:FileUpload ID="MyUpload" runat="server" Style="border: 1px solid #cbd6e4;
                                                    font-size: 10pt; color: #0b3d62;" Width="350px"
                                                    EnableViewState="true" />
                                                <span class="span_note">- Only Excel (*.xls), not include Excel 2007 (*.xlsx)<br />- Max filesize 500KB</span>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="3" style="text-align:center;">
                                                    <br /><asp:Label ID="lblError" runat="server" CssClass="txt_error"></asp:Label>&nbsp;
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="3" style="text-align:right;">
                                                    <asp:Button ID="btnUpload" runat="server" Text="Upload" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <p class="p_copyright">
                                Copyright &copy; 2009-<%=Now().ToString("yyyy")%> Global Telematics Sdn Bhd. All rights reserved.
                            </p>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div style="font-family: Verdana; font-size: 11px;">
                                <br />
                                <asp:GridView ID="gvExcel" runat="server" AutoGenerateColumns="True" Width="700px" HeaderStyle-Font-Size="12px"
                                    HeaderStyle-ForeColor="#FFFFFF" HeaderStyle-BackColor="#465AE8" HeaderStyle-Font-Bold="True"
                                    Font-Bold="False" Font-Overline="False" EnableViewState="False" HeaderStyle-Height="22px"
                                    HeaderStyle-HorizontalAlign="Center">
                                    <AlternatingRowStyle BackColor="Lavender" />
                                    <HeaderStyle BackColor="#465AE8" Font-Bold="True" Font-Size="12px" ForeColor="White"
                                        Height="22px" HorizontalAlign="Center" />
                                </asp:GridView>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
            <br /><br /><asp:Label ID="lblDesc" runat="server" Text="lblDesc" Font-Size="12px" Width="700px"></asp:Label>
        </center>
        
    </form>
</body>
</html>
