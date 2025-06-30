<%@ Page Language="vb" AutoEventWireup="false" EnableEventValidation="false" Inherits="YTLWebApplication.AVLS.UnitManagement" Codebehind="UnitManagement.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Unit Management</title>
    <style type="text/css">
    .a{background-color: #F6F6F6;}
    .b{background-color: #EBEBEB;}
    .c{text-align: center;}
    </style>
    <script type="text/javascript" language="javascript">
    function checkall(chkobj)
	{
	    var chkvalue=chkobj.checked;
	    for(i = 0; i < document.forms[0].elements.length; i++) 
        {
            elm = document.forms[0].elements[i]
            if (elm.type == 'checkbox') 
            {
                document.forms[0].elements[i].checked =chkvalue;
            }
        }
    }
    function deleteconfirmation()
	{
	    var checked=false;
	    for(i = 0; i < document.forms[0].elements.length; i++) 
        {
           elm = document.forms[0].elements[i]
           if (elm.type == 'checkbox') 
            {
                if(elm.checked == true)
                {
                    checked=true;
                    break;
                }
            }
        }
        if(checked)
        {
		    var result=confirm("Are you delete checked units ?");
		    if(result)
		    {
		        return true;
		    }
		    return false;
		}
		else
		{
		    alert("Please select checkboxes");
		    return false;
		}
	}
    function mouseover(path)
    {
        document.getElementById("bigimage").src="vehicleimages/"+path;
    }
    function mouseout()
    {

    }   
    </script>

</head>
<body style="margin-left: 5px; margin-top: 0px; margin-bottom: 0px; margin-right: 5px;">
    <form id="Form1" method="post" runat="server">
        <center>
            <br />
            <img src="images/UnitMgmt.jpg" alt="Unit Management" />
            <br />
            <br />
            <table border="0" width="600px;" style="font-family: Verdana; font-size: 11px">
                <tr>
                    <td align="left">
                        <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="images/Delete.jpg" ToolTip="Delete Checked Units" /></td>
                    <td align="center">
                        <b style="color: #5f7afc;">Select User Name&nbsp;:&nbsp;</b>
                        <asp:DropDownList ID="ddlusers" runat="server" Width="200px" AutoPostBack="True"
                            Font-Size="12px" Font-Names="verdana" EnableViewState="False">
                            <asp:ListItem Value="--Select User Name--">--Select User Name--</asp:ListItem>
                            <asp:ListItem Value="--All Server 1 Users--">--All Server 1 Users--</asp:ListItem>
                            <asp:ListItem>--All Server 2 Users--</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td align="right">
                        <a href="javascript:print();">
                            <img alt="Print" src="images/print.gif" style="border: solid 0px blue;" /></a>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <a href="AddUnit.aspx">
                            <img src="images/Add.jpg" alt="Add New Unit" style="border: 0px; cursor: pointer"
                                title="Add New Unit" />
                        </a>
                    </td>
                </tr>
                <tr>
                    <td align="center" colspan="3">
                        <div style="font-family: Verdana; font-size: 11px;">
                            <br />
                            <asp:GridView ID="unitsgrid" runat="server" AutoGenerateColumns="False" HeaderStyle-Font-Size="12px"
                                HeaderStyle-ForeColor="#FFFFFF" HeaderStyle-BackColor="#465AE8" HeaderStyle-Font-Bold="True"
                                HeaderStyle-Height="22px" EnableViewState="False" HeaderStyle-HorizontalAlign="Center"
                                Width="600px">
                                <Columns>
                                    <asp:BoundField DataField="chk" HeaderText="<input type='checkbox' onclick='javascript:checkall(this);' />"
                                        HtmlEncode="False" >
                                      <ItemStyle  Width="20" />
                                    </asp:BoundField>
                                     <asp:BoundField DataField="sno" HeaderText="S No">
                                        <ItemStyle HorizontalAlign="center" Width="35" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="unitid" HeaderText="Unit ID" HtmlEncode="False"></asp:BoundField>
                                    <asp:BoundField DataField="versionid" HeaderText="Version ID" />
                                    <asp:BoundField DataField="password" HeaderText="Password" />
                                    <asp:BoundField DataField="simno" HeaderText="SIM No">
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:BoundField>
                                </Columns>
                                <AlternatingRowStyle BackColor="Lavender" />
                            </asp:GridView>
                            <br />
                        </div>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="images/Delete.jpg" ToolTip="Delete Checked Units" />
                    </td>
                    <td>
                    </td>
                    <td align="right">
                        <a href="AddUnit.aspx">
                            <img src="images/Add.jpg" alt="Add New Unit" style="border: 0px; vertical-align: top;
                                cursor: pointer" title="Add New Unit" />
                        </a>
                    </td>
                </tr>
            </table>
            <p style="margin-bottom: 15px; font-family: Verdana; font-size: 11px; color: #5373a2;">
                Copyright © 2013 Global Telematics Sdn Bhd. All rights reserved.<br />
                <br />
            </p>
        </center>
    </form>

     <form id="form2" runat="server">
        <center>
            <div id="Qwssss" runat ="server" >
                <br />
                <b style="font-family: Verdana; font-size: 18px; color: #5B7C97;"></b>
                <br />
                <br />
                <table>
                    <tr>
                        <td align="center">
                            <table style="font-family: Verdana; font-size: 11px;">
                                <tr>
                                    <td style="height: 20px; background-color: #5B7C97;" align="left">
                                        <b style="color: White;">&nbsp; :</b></td>
                                </tr>
                                <tr>
                                    <td style="width: 500px; border: solid 1px #5B7C97;">
                                        <table style="width: 500px;">
                                            <tr>
                                                <td colspan="3">
                                                    <br />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <b style="color: #5B7C97;"></b>
                                                </td>
                                                <td>
                                                    <b style="color: #5B7C97;">:</b></td>
                                                <td align="left">
                                                    <asp:TextBox ID="QueryTextBox" runat="server" Rows="4" TextMode="MultiLine" Width="420"></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="3" align="right">
                                                    <br />
                                                    <asp:Button ID="Button1" runat="server" Text="Submit" />&nbsp;&nbsp;&nbsp;&nbsp;</td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                            <%--<p style="margin-bottom: 15px; font-family: Verdana; font-size: 11px; color: #5373a2;">
                                Copyright &copy; 2009 Global Telematics Sdn Bhd. All rights reserved.
                            </p>--%>
                        </td>
                    </tr>
                    <tr>
                        <td align="left">
                            <center>
                                <asp:Label ID="Label1" runat="server" Text="" Font-Bold="True" Font-Names="Verdana"
                                    Font-Size="11px" ForeColor="Green" Visible="false"></asp:Label></center>
                            <div style="font-family: Verdana; font-size: 11px;">
                                <br />
                                <asp:GridView ID="GridView1" runat="server" Width="100%" HeaderStyle-Font-Size="12px"
                                    HeaderStyle-ForeColor="#FFFFFF" HeaderStyle-BackColor="#5B7C97" HeaderStyle-Font-Bold="True"
                                    Font-Bold="False" Font-Overline="False" EnableViewState="False" HeaderStyle-Height="22px"
                                    HeaderStyle-HorizontalAlign="Center" AutoGenerateColumns="true" BorderColor="White"
                                    BorderStyle="None" CellPadding="1" CellSpacing="1" BorderWidth="0">
                                    <AlternatingRowStyle CssClass="a" />
                                    <RowStyle CssClass="b" />
                                </asp:GridView>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
        </center>
    </form>

</body>
</html>
