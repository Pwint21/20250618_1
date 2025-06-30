<%@ Page Language="vb" AutoEventWireup="false" EnableEventValidation="false" Inherits="YTLWebApplication.AVLS.UserManagement" Codebehind="UserManagement.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>User Management</title>

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
		    var result=confirm("Are you delete checked users ?");
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
    </script>

</head>
<body style="margin-left: 5px; margin-top: 0px; margin-bottom: 0px; margin-right: 5px;">
    <form id="vehicleform" runat="server">
        <center>
            <br />
            <img src="images/UserMgmt.jpg" alt="User Management" />
            <br />
            <br />
            <table border="0" width="800px;" style="font-family: Verdana; font-size: 11px">
                <tr>
                    <td align="left">
                        <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="images/Delete.jpg" ToolTip="Delete Checked Users" /></td>
                    <td align="center">
                        <b style="color: #5f7afc;">Select User Name&nbsp;:&nbsp;</b>
                        <asp:DropDownList ID="ddlusers" runat="server" Width="200px" AutoPostBack="True"
                            Font-Size="12px" Font-Names="verdana" EnableViewState="False">
                            <asp:ListItem Value="--Select User Name--">--Select User Name--</asp:ListItem>
                            <asp:ListItem Value="-- All Customers --">-- All Usernames ------------</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td align="right">
                        <a href="javascript:print();">
                            <img alt="Print" src="images/print.gif" style="border: solid 0px blue;" /></a>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <a href="AddUser.aspx">
                            <img src="images/Add.jpg" alt="Add New User" style="border: 0px; vertical-align: top;
                                cursor: pointer" title="Add New User" />
                        </a>
                    </td>
                </tr>
                <tr>
                    <td align="left" colspan="3">
                        <div style="font-family: Verdana; font-size: 11px;">
                            <asp:Label ID="Label1" runat="server" Font-Size="X-Small" Text="** User Level : "></asp:Label>&nbsp;
                            <asp:Label ID="Label4" runat="server" Font-Size="X-Small" Text="0 - All"></asp:Label>
                            &nbsp;
                            <asp:Label ID="Label2" runat="server" Font-Size="X-Small" Text="1 - Reports & Charts Only"></asp:Label>&nbsp;
                            &nbsp;<asp:Label ID="Label3" runat="server" Font-Size="X-Small" Text="2 - Charts Only"></asp:Label>
                            <br />
                            <asp:GridView ID="usersgrid" runat="server" AutoGenerateColumns="False" HeaderStyle-Font-Size="12px"
                                HeaderStyle-ForeColor="#FFFFFF" HeaderStyle-BackColor="#465AE8" HeaderStyle-Font-Bold="True"
                                HeaderStyle-Height="22px" EnableViewState="False" HeaderStyle-HorizontalAlign="Center"
                                Width="800px">
                                <Columns>
                                    <asp:BoundField DataField="chk" HeaderText="<input type='checkbox' onclick='javascript:checkall(this);' />"
                                        HtmlEncode="False">
                                        <ItemStyle  Width="20" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sno" HeaderText="S No">
                                        <ItemStyle HorizontalAlign="center" Width="35" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="username" HeaderText="User Name" HtmlEncode="False"></asp:BoundField>
                                    <asp:BoundField DataField="password" HeaderText="Password" />
                                    <asp:BoundField DataField="companyname" HeaderText="Company" />
                                    <asp:BoundField DataField="phoneno" HeaderText="Phone No" />
                                    <asp:BoundField DataField="address" HeaderText="Address" />
                                    <asp:BoundField DataField="role" HeaderText="Role" HtmlEncode="False">
                                        <ItemStyle Width="90" />
                                    </asp:BoundField>
                                   <asp:BoundField DataField="usertype" HeaderText="Level" /> 
                                   <asp:BoundField DataField="ERP" HeaderText="ERP" /> 
                                   <asp:BoundField DataField="Itinery" HeaderText="Itinery" />
                                   <asp:BoundField DataField="drcaccess" HeaderText="IVMS" />
                                   <asp:BoundField DataField="server" HeaderText="Server" HtmlEncode="False"></asp:BoundField>
                                </Columns>
                                <AlternatingRowStyle BackColor="Lavender" />
                            </asp:GridView>
                            <br />
                        </div>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="images/Delete.jpg" ToolTip="Delete Checked Users" />
                    </td>
                    <td>
                    </td>
                    <td align="right">
                        <a href="AddUser.aspx">
                            <img src="images/Add.jpg" alt="Add New User" style="border: 0px; vertical-align: top;
                                cursor: pointer" title="Add New User" />
                        </a>
                    </td>
                </tr>
            </table>
            <p style="margin-bottom: 15px; font-family: Verdana; font-size: 11px; color: #5373a2;">
                Copyright © 2013 Global Telematics Sdn Bhd. All rights reserved.
            </p>
            <br />
        </center>
    </form>
</body>
</html>
