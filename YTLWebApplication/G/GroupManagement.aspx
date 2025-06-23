<%@ Page Language="VB" AutoEventWireup="false" EnableEventValidation="false"
    Inherits="YTLWebApplication.AVLS.GroupManagement" Codebehind="GroupManagement.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Group Management</title>

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
		    var result=confirm("Are you delete checked groups ?");
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
<body style="margin-left: 5px; margin-top: 0px; margin-bottom: 10px; margin-right: 5px;">
    <form id="vehicleform" runat="server">
        <center>
            <br />
            <img src="images/GroupManagement.jpg" alt="Group Management" />
            <br />
            <br />
            <table border="0" width="600px;" style="font-family: Verdana; font-size: 11px">
                <tr>
                    <td align="left">
                        <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="images/Delete.jpg" ToolTip="Delete Checked Groups" /></td>
                    <td align="center">
                        <b style="color: #5f7afc;">Select User Name&nbsp;:&nbsp;</b>
                        <asp:DropDownList ID="ddlusers" runat="server" Width="200px" AutoPostBack="True"
                            Font-Size="12px" Font-Names="verdana" EnableViewState="False">
                           
                        </asp:DropDownList>
                    </td>
                    <td align="right">
                        <a href="javascript:print();">
                            <img alt="Print" src="images/print.gif" style="border: solid 0px blue;" /></a>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <a href="AddGroup.aspx">
                            <img src="images/Add.jpg" alt="Add New Unit" style="border: 0px; cursor: pointer"
                                title="Add New Group" />
                    </td>
                </tr>
                <tr>
                    <td align="left" colspan="3">
                        <div style="font-family: Verdana; font-size: 11px;">
                            <br />
                            <asp:GridView ID="groupsgrid" runat="server" AutoGenerateColumns="False" HeaderStyle-Font-Size="12px"
                                HeaderStyle-ForeColor="#FFFFFF" HeaderStyle-BackColor="#465AE8" HeaderStyle-Font-Bold="True"
                                HeaderStyle-Height="22px" EnableViewState="False" HeaderStyle-HorizontalAlign="Center"
                                Width="600px">
                                <Columns>
                                    <asp:BoundField DataField="chk" HeaderText="<input type='checkbox' onclick='javascript:checkall(this);' />"
                                        HtmlEncode="False">
                                        <ItemStyle  Width="20" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sno" HeaderText="S No">
                                        <ItemStyle HorizontalAlign="center" Width="35" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="groupname" HeaderText="Group Name" HtmlEncode="False"></asp:BoundField>
                                    <asp:BoundField DataField="username" HeaderText="User Name" />
                                    <asp:BoundField DataField="description" HeaderText="Description" />
                                </Columns>
                                <AlternatingRowStyle BackColor="Lavender" />
                            </asp:GridView>
                            <br />
                        </div>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="images/Delete.jpg" ToolTip="Delete Checked Groups" />
                    </td>
                    <td>
                    </td>
                    <td align="right">
                        <a href="AddGroup.aspx">
                            <img src="images/Add.jpg" alt="Add New Unit" style="border: 0px; cursor: pointer"
                                title="Add New Group" />
                    </td>
                </tr>
            </table>
            <p style="margin-bottom: 15px; font-family: Verdana; font-size: 11px; color: #5373a2;">
                Copyright © 2013 Global Telematics Sdn Bhd. All rights reserved.</p>
        </center>
    </form>
</body>
</html>
