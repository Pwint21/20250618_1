<%@ Page Language="vb" AutoEventWireup="false" EnableEventValidation="false" Inherits="YTLWebApplication.AVLS.VehicleManagement" Codebehind="VehicleManagement.aspx.vb" %>

<%@ Import Namespace="System.Data.SqlClient" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Vehicle Management</title>
    <link type="text/css" href="cssfiles/balloontip.css" rel="stylesheet" />
    <script type="text/javascript" src="jsfiles/balloontip.js"></script>
    <script type="text/javascript" language="javascript">
    var ec=<%=ec %>;
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
		    var result=confirm("Are you delete checked vehicles ?");
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
        document.getElementById("bigimage").src="vehiclebigimages\\"+path+"?rnd="+Math.random();
    }
    function ExcelReport()
{
    if(ec==true)
    {
        var excelformobj=document.getElementById("excelform");
        excelformobj.submit();
    }
    else
    {
        alert("First click submit button");
    }
}

    function ExcelReport2()
{
    if(ec==true)
    {
        var a = document.getElementById("hdExceltitle").value
        var b = document.getElementById("hdtotalTank1").value
        var c = document.getElementById("hdtotaltank2").value
        var d = document.getElementById("hdtotalTankNull").value
        var e = document.getElementById("hdtotalVehicle").value
        var f = document.getElementById("hdtotalPTO").value
        
        document.getElementById("title2").value = a;
        document.getElementById("Totaltank1").value = b;
        document.getElementById("Totaltank2").value = c;
        document.getElementById("TotalWithOutTank").value = d;
        document.getElementById("TotalVehicle").value = e; 
        document.getElementById("TotalPTO").value = f;

        var excelformobj=document.getElementById("excelform2");
        excelformobj.submit();
    }
    else
    {
        alert("First click submit button");
    }
}
    </script>
</head>
<body style="margin-left: 5px; margin-top: 0px; margin-bottom: 0px; margin-right: 5px;">
    <form id="vehicleform" runat="server">
    <center>
        <br />
        <img src="images/VehicleMgmt.jpg" alt="Vehicle Management" />
        <br />
        <br />
        <table border="0" width="95%" style="font-family: Verdana; font-size: 11px">
            <tr>
                <td align="left" style="width: 20%">
                    <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="images/Delete.jpg" ToolTip="Delete Checked Vehicles" />
                </td>
                <td align="center" style="width: 60%">
                    <b style="color: #5f7afc;">Select User Name&nbsp;:&nbsp;</b>
                    <asp:DropDownList ID="ddlusers" runat="server" Width="200px" AutoPostBack="True"
                        Font-Size="12px" Font-Names="verdana" EnableViewState="False">
                        <asp:ListItem Value="--Select User Name--">--Select User Name--</asp:ListItem>
                        <asp:ListItem Value="--All Server 1 Users--">--All Server 1 Users--</asp:ListItem>
                        <asp:ListItem>--All Server 2 Users--</asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td align="right" style="width: 20%">
                    <a href="javascript:print();">
                        <img alt="Print" src="images/print.gif" style="border: solid 0px blue;" /></a>
               <%--     <a href="javascript:ExcelReport2();">
                        <img alt="Save Billing to Excel file" title="Save Billing to Excel file" src="images/saveBilling.jpg"
                            style="border: solid 0px blue;" /></a>--%>
                    <asp:ImageButton ID="btnImage" ToolTip = "Save Billing to Excel file" ImageUrl = "images/saveBilling.jpg" runat="server" style="border: solid 0px blue;"/>
                            <a href="javascript:ExcelReport();">
                                <img alt="Save to Excel file" title="Save to Excel file" src="images/saveExcel.jpg"
                                    style="border: solid 0px blue;" /></a> &nbsp;&nbsp;<a href="AddVehicle.aspx"><img
                                        src="images/Add.jpg" alt="Add New Vehicle" style="border: 0px; cursor: pointer"
                                        title="Add New Vehicle" />
                                    </a>
                </td>
            </tr>
            <tr>
                <td align="left" colspan="3">
                    <div style="font-family: Verdana; font-size: 11px;">
                        <br />
                        <asp:GridView ID="vehiclesgrid" runat="server" AutoGenerateColumns="False" HeaderStyle-Font-Size="12px"
                            HeaderStyle-ForeColor="#FFFFFF" HeaderStyle-BackColor="#465AE8" HeaderStyle-Font-Bold="True"
                            HeaderStyle-Height="22px" EnableViewState="False" HeaderStyle-HorizontalAlign="Center"
                            Width="100%" EnableModelValidation="True">
                            <Columns>
                                <asp:BoundField DataField="chk" HeaderText="&lt;input type='checkbox' onclick='javascript:checkall(this);' /&gt;"
                                    HtmlEncode="False">
                                    <ItemStyle Width="20px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="sno" HeaderText="No" />
                                <asp:BoundField DataField="username" HeaderText="Username" />
                                <asp:BoundField DataField="plateno" HeaderText="Plate No" HtmlEncode="False">
                                    <ItemStyle Width="150px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="plate no" HeaderText="Plate No" HtmlEncode="False">
                                    <ItemStyle Width="150px" />
                                </asp:BoundField>
                                <asp:BoundField DataField="unitid" HeaderText="Unit ID" HtmlEncode="False" />
                                <asp:BoundField DataField="type" HeaderText="Type" />
                                <asp:BoundField DataField="color" HeaderText="Color" />
                                <asp:BoundField DataField="model" HeaderText="Model" />
                                <asp:BoundField DataField="brand" HeaderText="Brand" />
                                <asp:BoundField DataField="groupname" HeaderText="Group Name" />
                                <asp:BoundField DataField="speed" HeaderText="Speed" />
                                <asp:BoundField DataField="tank1" HeaderText="Tank 1" />
                                <asp:BoundField DataField="tank2" HeaderText="Tank 2" />
                                <asp:BoundField DataField="portno" HeaderText="Port" />
                                <asp:BoundField DataField="PTO" HeaderText="PTO" />
                                <asp:BoundField DataField="WeightSensor" HeaderText="Weight" />
                                <asp:BoundField DataField="Immobilizer" HeaderText="Immo" />
                                <asp:BoundField DataField="Roaming" HeaderText="Roaming" />
                                 <asp:BoundField DataField="Permit" HeaderText="Permit" />
                                <asp:BoundField DataField="installdate" HeaderText="Installation Date">
                                    <ItemStyle Width="80px" HorizontalAlign="Center" />
                                </asp:BoundField>
                            </Columns>
                            <AlternatingRowStyle BackColor="Lavender" />
                            <HeaderStyle BackColor="#465AE8" Font-Bold="True" Font-Size="12px" ForeColor="White"
                                Height="22px" HorizontalAlign="Center" />
                        </asp:GridView>
                        <br />
                    </div>
                </td>
            </tr>
            <tr>
                <td align="left">
                    <asp:ImageButton ID="ImageButton2" runat="server" ImageUrl="images/Delete.jpg" ToolTip="Delete Checked Vehicles" />
                </td>
                <td>
                </td>
                <td align="right">
                    <a href="AddVehicle.aspx">
                        <img src="images/Add.jpg" alt="Add New Vehicle" style="border: 0px; cursor: pointer"
                            title="Add New Vehicle" />
                    </a>
                </td>
            </tr>
        </table>
        <p style="margin-bottom: 15px; font-family: Verdana; font-size: 11px; color: #5373a2;">
            Copyright ?2008 Global Telematics Sdn Bhd. All rights reserved.
        </p>
    </center>
    <div id="balloon1" class="balloonstyle" style="width: 102px; vertical-align: middle;">
        <img id="bigimage" src="vehiclebigimages/bigvehicle.gif" alt="" style="border: 1px solid silver;
            width: 100px; height: 100px; vertical-align: middle;" />
    </div>
    <asp:HiddenField ID="hdExceltitle" runat="server" />
    <asp:HiddenField ID="hdtotalTank1" runat="server" />
    <asp:HiddenField ID="hdtotaltank2" runat="server" />
    <asp:HiddenField ID="hdtotalTankNull" runat="server" />
    <asp:HiddenField ID="hdtotalWithOutTank" runat="server" />
    <asp:HiddenField ID="hdtotalPTO" runat="server" />
    <asp:HiddenField ID="hdtotalVehicle" runat="server" />
    </form>
    <form id="excelform" method="get" action="ExcelReport_new.aspx">
    <input type="hidden" id="title" name="title" value="Vehicle Management" />
    <input type="hidden" id="type" name="type" value="1" />
    </form>
    <form id="excelform2" method="get" action="GenerateInvoice.aspx">
    </form>
</body>
</html>
