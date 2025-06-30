<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.UpdatePlateNoManagement" Codebehind="UpdatePlateNoManagement.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
   <script type="text/javascript" language="javascript">
function setHide()
{
	document.getElementById("rowSearch").style.display='none';
	document.getElementById("rowSearch2").style.display='none';
	document.getElementById("rowSearch3").style.display='none';
	document.getElementById("rowSearch4").style.display='none';
	document.getElementById("rowSearch5").style.display='none';
}

function setDisplay()
{
    document.getElementById("rowSearch").style.display='inline';
    document.getElementById("rowSearch2").style.display='inline';
   document.getElementById("rowSearch3").style.display='inline';
   document.getElementById("rowSearch4").style.display='inline';
   document.getElementById("rowSearch5").style.display='inline';   
}

function mysubmit()
{
    if(document.getElementById("txtNew").value=="")
         {
            alert("enter new plate number.");
            return false;
          }
    if
    {
        return confirm("Are you sure you want to update this vehicle?\nPress OK to proceed.");
    }
    return true;
      
}

</script>

    <title>Gussmann Technologies - Update Old Plate No.</title>
</head>
<body id="plateno" style="text-align: center" runat="server" bottommargin="1" leftmargin="1" rightmargin="1" topmargin="1">
    <form id="form1" runat="server" defaultbutton="ibBack">
        <br />
        <asp:Image ID="Image1" runat="server" ImageUrl="~/images/updateplatenumbermanagement.gif" /><br />
    <table width="900" align="center">
    <tr>
   <td align="left" bgcolor="#465ae8" style="width: 100%; height: 20px;">
       <asp:Label ID="Label1" runat="server" Text="Update Plate Number Management :" Font-Bold="True" Font-Names="Verdana" Font-Size="11px" ForeColor="White"></asp:Label></td>
    </tr>
        <tr>
            <td style="padding-right:5px; padding-top:5px;padding-bottom:5px; border: solid 1px #3952F9; width: 100%;" >
            <table width="100%"><tr id="rowType">
                                                    <td align="left" valign="middle">
                                                        <b style="color: #5f7afc">
                                                            <asp:Label ID="Label23" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="11px"
                                                                ForeColor="#5F7AFC" Text="User Name"></asp:Label></b></td>
                                                    <td valign="middle">
                                                        <b style="color: #5f7afc">:</b></td>
                                                    <td align="left" valign="middle">
                                                        <asp:DropDownList ID="ddlUsername" runat="server" AutoPostBack="True" Font-Names="verdana"
                                                            Font-Size="11px" Width="200px">
                                                            <asp:ListItem>--Select User Name--</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td align="left" valign="middle">
                                                        <asp:Label ID="lblOffset" runat="server" Font-Bold="True" Font-Names="Verdana"
                                                            Font-Size="11px" ForeColor="#5F7AFC" Text="Old Plate"></asp:Label></td>
                                                    <td align="left" valign="middle">
                                                        <asp:Label ID="Label10" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="11px"
                                                            ForeColor="#5F7AFC" Text=":"></asp:Label></td>
                                                    <td align="left" valign="middle">
                                                        <asp:DropDownList ID="ddlpleate" runat="server" Font-Names="verdana" Font-Size="11px"
                                                            Width="200px">
                                                            <asp:ListItem>--Select User Name--</asp:ListItem>
                                                        </asp:DropDownList></td>
                                                    <td align="left" valign="middle">
                                                        <asp:Label ID="Label13" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="11px"
                                                            ForeColor="#5F7AFC" Text="New Plate"></asp:Label></td>
                                                    <td align="left" valign="middle">
                                                        <asp:Label ID="Label15" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="11px"
                                                            ForeColor="#5F7AFC" Text=":"></asp:Label></td>
                                                    <td align="left" valign="middle">
                                                        <asp:TextBox ID="txtNew" runat="server" Width="183px" Font-Size="11px"></asp:TextBox></td>
                                                </tr>
            </table>
            </td>
        </tr>
   <tr>
                                    <td style="padding-right:5px; padding-top:5px;padding-bottom:5px; border: solid 1px #3952F9; width: 100%;" valign="top" >
                                        <table width="100%">
                                            <tbody>
                                                <tr></tr>
                                                <tr>
                                                    <td align="left" colspan="9" valign="middle">
                                                        <asp:CheckBoxList ID="cblTable" runat="server" Font-Names="Verdana" Font-Size="11px" RepeatColumns="4" Width="100%">
                                                            <asp:ListItem>vehicleTBL</asp:ListItem>
                                                            <asp:ListItem Enabled="False">vehicle_history</asp:ListItem>
                                                            <asp:ListItem Selected="True">vehicle_tracked</asp:ListItem>
                                                            <asp:ListItem Selected="True">fuel_tank_check</asp:ListItem>
                                                            <asp:ListItem Selected="True">fuel_tank_profile</asp:ListItem>
                                                            <asp:ListItem Selected="True">maintenance</asp:ListItem>
                                                            <asp:ListItem Selected="True">geofence_tracked</asp:ListItem>
                                                            <asp:ListItem Selected="True">geofence_trip_audit</asp:ListItem>
                                                            <asp:ListItem Selected="True">fuel</asp:ListItem>
                                                            <asp:ListItem Selected="True">panic_interval</asp:ListItem>
                                                            <asp:ListItem Selected="True">tollfare</asp:ListItem>
                                                            <asp:ListItem Selected="True">documents_date</asp:ListItem>
                                                            <asp:ListItem Selected="True">driver_assign</asp:ListItem>
                                                            <asp:ListItem Selected="True">operator_check_list</asp:ListItem>
                                                            <asp:ListItem Selected="True">trip_receipt</asp:ListItem>
                                                            <asp:ListItem Selected="True">vehicle_average_idling</asp:ListItem>
                                                            <asp:ListItem Selected="True">vehicle_servicing</asp:ListItem>
                                                            <asp:ListItem Selected="True">vehicle_g13e_data</asp:ListItem>
                                                            <asp:ListItem Selected="True">vehicle_geofence</asp:ListItem>
                                                            <asp:ListItem Selected="True">vehicle_idling_profile</asp:ListItem>
                                                            <asp:ListItem Selected="True">vehicle_incident</asp:ListItem>
                                                            <asp:ListItem Selected="True">vehicle_fuel_summ</asp:ListItem>
                                                            <asp:ListItem Selected="True">vehicle_idling_summ</asp:ListItem>
                                                            <asp:ListItem Selected="True">vehicle_refuel_summ</asp:ListItem>
                                                            <asp:ListItem Selected="True">instant_alert_settings</asp:ListItem>
                                                            <asp:ListItem Selected="True">sms_panic_dispatch_list</asp:ListItem>
                                                        </asp:CheckBoxList></td>
                                                </tr>
                                                
                                                </tbody> 
    </table>
</td> 
   </tr>
        <tr>
            <td style="border-right: #3952f9 1px solid; padding-right: 5px; border-top: #3952f9 1px solid;
                padding-bottom: 5px; border-left: #3952f9 1px solid; width: 100%; padding-top: 5px;
                border-bottom: #3952f9 1px solid" height="10">
               <table width="100%">
               <tr>
                                                    <td align="left" valign="bottom" width="13%">
                                                        <asp:ImageButton ID="ibBack" runat="server" ImageUrl="~/images/Search.jpg" OnClick="ibBack_Click" CausesValidation="False" /></td>
                                                    <td colspan="2" align="right" valign="bottom">
                                                        &nbsp;<asp:Label ID="lblMessage" runat="server"  Font-Bold="True" Font-Names="Verdana" Font-Size="11px" ForeColor="Red" Visible="False"></asp:Label>
                                                        &nbsp; &nbsp; &nbsp; &nbsp;&nbsp;
                                                    </td>
                                                    <td align="right" colspan="1" valign="bottom">
                                                    </td>
                                                    <td align="right" colspan="1" valign="bottom">
                                                    </td>
                                                    <td align="right" colspan="1" valign="bottom">
                                                    </td>
                                                    <td align="right" colspan="1" valign="bottom">
                                                    </td>
                                                    <td align="right" colspan="1" valign="bottom">
                                                    </td>
                                                    <td align="right" colspan="1" valign="bottom">
                                                    <asp:ImageButton ID="ibSubmit" runat="server" ImageUrl="~/images/Submit_s.jpg" /></td>
                                                </tr>
               </table> 
            </td>
        </tr>
   </table>
        <br />
       <table width="900px" id="rowSearch" visible="false" align="center">
       <tr visible="false"><td bgcolor="#465ae8">
           <asp:Label ID="Label3" runat="server" Text="vehicle" Font-Bold="True" Font-Names="Verdana" Font-Size="11px" ForeColor="White"></asp:Label></td>
          <td bgcolor="#465ae8">
           <asp:Label ID="Label4" runat="server" Text="vehicle_history" Font-Bold="True" Font-Names="Verdana" Font-Size="11px" ForeColor="White"></asp:Label></td>
          <td bgcolor="#465ae8">
           <asp:Label ID="Label5" runat="server" Text="vehicle_tracked" Font-Bold="True" Font-Names="Verdana" Font-Size="11px" ForeColor="White"></asp:Label></td>
          <td bgcolor="#465ae8">
           <asp:Label ID="Label6" runat="server" Text="fuel_tank_check" Font-Bold="True" Font-Names="Verdana" Font-Size="11px" ForeColor="White"></asp:Label></td>
          <td bgcolor="#465ae8">
           <asp:Label ID="Label7" runat="server" Text="fuel_tank_profile" Font-Bold="True" Font-Names="Verdana" Font-Size="11px" ForeColor="White"></asp:Label></td>
           <td bgcolor="#465ae8">
               <asp:Label ID="Label12" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="11px"
                   ForeColor="Black" Text="Old Unit Id"></asp:Label></td>
                 
           </tr>
       <tr>
       <td valign="top" align="center" bgcolor="#ffffcc">
           <asp:Label ID="lbl1" runat="server" Font-Names="Verdana" Font-Size="11px"></asp:Label></td>
       
       <td valign="top" align="center" bgcolor="#ffffcc">
           <asp:Label ID="lbl2" runat="server" Font-Names="Verdana" Font-Size="11px"></asp:Label></td>
       
       <td valign="top" align="center" bgcolor="#ffffcc">
           <asp:Label ID="lbl3" runat="server" Font-Names="Verdana" Font-Size="11px"></asp:Label></td>
       
        <td valign="top" align="center" bgcolor="#ffffcc">
            <asp:Label ID="lbl4" runat="server" Font-Names="Verdana" Font-Size="11px"></asp:Label></td>
       
       <td valign="top" align="center" bgcolor="#ffffcc">
           <asp:Label ID="lbl5" runat="server" Font-Names="Verdana" Font-Size="11px"></asp:Label></td>
           <td valign="top" align="center" bgcolor="#ffcccc">
               <asp:Label ID="lbl6" runat="server" Font-Names="Verdana" Font-Size="11px"></asp:Label></td>
       
       </tr>
       </table> 
        <table width="900" id="rowSearch2" visible="false" align="center">
            <tr visible="false">
                <td bgcolor="#465ae8" align="center">
                    <asp:Label ID="Label17" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="11px"
                        ForeColor="White" Text="maintenance"></asp:Label></td>
                <td bgcolor="#465ae8" align="center">
                    <asp:Label ID="Label14" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="11px"
                        ForeColor="White" Text="geofence_tracked"></asp:Label></td>
                <td bgcolor="#465ae8" align="center">
                    <asp:Label ID="Label18" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="11px"
                        ForeColor="White" Text="geofence_trip_audit"></asp:Label></td>
                <td bgcolor="#465ae8" align="center">
                    <asp:Label ID="Label19" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="11px"
                        ForeColor="White" Text="fuel"></asp:Label></td>
                <td bgcolor="#465ae8" align="center">
                    <asp:Label ID="Label20" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="11px"
                        ForeColor="White" Text="panic_interval"></asp:Label></td>
                <td bgcolor="#465ae8" align="center">
                    <asp:Label ID="Label21" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="11px"
                        ForeColor="White" Text="tollfare"></asp:Label></td>
            </tr>
            <tr>
                <td valign="top" align="center" bgcolor="#ccffcc">
                    <asp:Label ID="lblMaintenance" runat="server" Font-Names="Verdana" Font-Size="11px"></asp:Label></td>
                <td valign="top" align="center" bgcolor="#ccffcc">
                    <asp:Label ID="lblGeofencetracked" runat="server" Font-Names="Verdana" Font-Size="11px"></asp:Label></td>
                <td valign="top" align="center" bgcolor="#ccffcc">
                    <asp:Label ID="lblGeofencetripaudit" runat="server" Font-Names="Verdana" Font-Size="11px"></asp:Label></td>
                <td valign="top" align="center" bgcolor="#ccffcc">
                    <asp:Label ID="lblFuel" runat="server" Font-Names="Verdana" Font-Size="11px"></asp:Label></td>
                <td valign="top" align="center" bgcolor="#ccffcc">
                    <asp:Label ID="lblPanicinterval" runat="server" Font-Names="Verdana" Font-Size="11px"></asp:Label></td>
                <td valign="top" align="center" bgcolor="#ccffcc">
                    <asp:Label ID="lblTollfare" runat="server" Font-Names="Verdana" Font-Size="11px"></asp:Label></td>
            </tr>
        </table>
        <table width="900" id="rowSearch3" visible="false" align="center">
            <tr visible="false">
                <td bgcolor="#465ae8" align="center">
                    <asp:Label ID="Label27" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="11px"
                        ForeColor="White" Text="document_date"></asp:Label></td>
                <td bgcolor="#465ae8" align="center">
                    <asp:Label ID="Label28" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="11px"
                        ForeColor="White" Text="driver_assign"></asp:Label></td>
                <td bgcolor="#465ae8" align="center">
                    <asp:Label ID="Label29" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="11px"
                        ForeColor="White" Text="operator_check_list"></asp:Label></td>
                <td bgcolor="#465ae8" align="center">
                    <asp:Label ID="Label30" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="11px"
                        ForeColor="White" Text="trip_receipt"></asp:Label></td>
                <td bgcolor="#465ae8" align="center">
                    <asp:Label ID="Label31" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="11px"
                        ForeColor="White" Text="vehicle_average_idling"></asp:Label></td>
            </tr>
            <tr>
                <td valign="top" align="center" bgcolor="#ffffcc">
                    <asp:Label ID="lblDocumentdate" runat="server" Font-Names="Verdana" Font-Size="11px"></asp:Label></td>
                <td valign="top" align="center" bgcolor="#ffffcc">
                    <asp:Label ID="lblDriverassign" runat="server" Font-Names="Verdana" Font-Size="11px"></asp:Label></td>
                <td valign="top" align="center" bgcolor="#ffffcc">
                    <asp:Label ID="lblOperatorchecklist" runat="server" Font-Names="Verdana" Font-Size="11px"></asp:Label></td>
                <td valign="top" align="center" bgcolor="#ffffcc">
                    <asp:Label ID="lblTripreceipt" runat="server" Font-Names="Verdana" Font-Size="11px"></asp:Label></td>
                <td valign="top" align="center" bgcolor="#ffffcc">
                    <asp:Label ID="lblVehicleaverageidling" runat="server" Font-Names="Verdana" Font-Size="11px"></asp:Label></td>
            </tr>
        </table>
        <table width="900" id="rowSearch4" visible="false" align="center">
            <tr visible="false">
                <td bgcolor="#465ae8" align="center">
                    <asp:Label ID="Label32" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="11px"
                        ForeColor="White" Text="vehicle_servicing"></asp:Label></td>
                <td bgcolor="#465ae8" align="center">
                    <asp:Label ID="Label38" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="11px"
                        ForeColor="White" Text="vehicle_g13e_data"></asp:Label></td>
                <td bgcolor="#465ae8" align="center">
                    <asp:Label ID="Label40" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="11px"
                        ForeColor="White" Text="vehicle_geofence"></asp:Label></td>
                <td bgcolor="#465ae8" align="center">
                    <asp:Label ID="Label41" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="11px"
                        ForeColor="White" Text="vehicle_idling_profile"></asp:Label></td><td bgcolor="#465ae8" align="center">
                            <asp:Label ID="Label42" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="11px"
                                ForeColor="White" Text="vehicle_incident"></asp:Label></td>
            </tr>
            <tr>
                <td valign="top" align="center" bgcolor="#ccffcc">
                    <asp:Label ID="lblVehicleservicing" runat="server" Font-Names="Verdana" Font-Size="11px"></asp:Label></td>
                <td valign="top" align="center" bgcolor="#ccffcc">
                    <asp:Label ID="lblVehicleg13edata" runat="server" Font-Names="Verdana" Font-Size="11px"></asp:Label></td>
                <td valign="top" align="center" bgcolor="#ccffcc">
                    <asp:Label ID="lblVehiclegeofence" runat="server" Font-Names="Verdana" Font-Size="11px"></asp:Label></td>
                <td valign="top" align="center" bgcolor="#ccffcc">
                    <asp:Label ID="lblVehicleidlingprofile" runat="server" Font-Names="Verdana" Font-Size="11px"></asp:Label></td><td valign="top" align="center" bgcolor="#ccffcc">
                        <asp:Label ID="lblVehicleincident" runat="server" Font-Names="Verdana" Font-Size="11px"></asp:Label></td>
            </tr>
        </table>
        <table width="900" id="rowSearch5" visible="false" align="center">
            <tr visible="false">
                <td bgcolor="#465ae8" align="center">
                    <asp:Label ID="Label48" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="11px"
                        ForeColor="White" Text="vehicle_fuel_summ"></asp:Label></td>
                <td bgcolor="#465ae8" align="center">
                    <asp:Label ID="Label49" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="11px"
                        ForeColor="White" Text="vehicle_idling_summ"></asp:Label></td>
                <td bgcolor="#465ae8" align="center">
                    <asp:Label ID="Label50" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="11px"
                        ForeColor="White" Text="vehicle_refuel_summ"></asp:Label></td><td bgcolor="#465ae8" align="center">
                            <asp:Label ID="Label22" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="11px"
                                ForeColor="White" Text="instant_alert_settings"></asp:Label></td>
                <td bgcolor="#465ae8" align="center">
                    <asp:Label ID="Label24" runat="server" Font-Bold="True" Font-Names="Verdana" Font-Size="11px"
                        ForeColor="White" Text="sms_panic_dispatch_list"></asp:Label></td>
            </tr>
            <tr>
                <td valign="top" align="center" bgcolor="#ffffcc">
                    <asp:Label ID="lblVehiclefuelsumm" runat="server" Font-Names="Verdana" Font-Size="11px"></asp:Label></td>
                <td valign="top" align="center" bgcolor="#ffffcc">
                    <asp:Label ID="lblVehicleidlingsumm" runat="server" Font-Names="Verdana" Font-Size="11px"></asp:Label></td>
                <td valign="top" align="center" bgcolor="#ffffcc">
                    <asp:Label ID="lblVehiclerefuelsumm" runat="server" Font-Names="Verdana" Font-Size="11px"></asp:Label></td><td valign="top" align="center" bgcolor="#ffffcc">
                        <asp:Label ID="lblInstantalertsettings" runat="server" Font-Names="Verdana" Font-Size="11px"></asp:Label></td>
                <td valign="top" align="center" bgcolor="#ffffcc">
                    <asp:Label ID="lblSmspanicdispatchlist" runat="server" Font-Names="Verdana" Font-Size="11px"></asp:Label></td>
            </tr>
        </table>
    </form>
</body>
</html>
