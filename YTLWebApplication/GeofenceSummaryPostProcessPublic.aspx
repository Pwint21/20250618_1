<%@ Page Language="VB" AutoEventWireup="false"
    Inherits="YTLWebApplication.GeofenceSummaryPostProcessPublic" Codebehind="GeofenceSummaryPostProcessPublic.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title> GeofenceSummaryPostProcess</title>
    <style type="text/css" media="screen">
      
    
        @import "css/jquery-ui.css";
      
    </style>
    <script type="text/javascript" language="javascript" src="js/jquery.js"></script>
    <script src="js/jquery_ui.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript" src="js/calendar.js"></script>

     <script type="text/javascript">


        $(function () {
            $("#txtBeginDate").datepicker({ dateFormat: 'yy/mm/dd', minDate: -180, maxDate: +0, changeMonth: true, changeYear: true, numberOfMonths: 2
            });

            $("#txtEndDate").datepicker({ dateFormat: 'yy/mm/dd', minDate: -180, maxDate: +0, changeMonth: true, changeYear: true, numberOfMonths: 2

            });

        

          
        });
  
var ec=<%=ec %>;

function treeViewCheck()
{
// obj gives us the node on which check or uncheck operation has performed 

var obj = window.event.srcElement; 

var treeNodeFound = false; 

var checkedState; 

//checking whether obj consists of checkbox or not 

if (obj.tagName == "INPUT" && obj.type == "checkbox") {

//easier to read

var treeNode = obj; 

//record the checked state of the TreeNode 

checkedState = treeNode.checked; 

//work our way back to the parent <table> element 

do {

obj = obj.parentElement; 

} while (obj.tagName != "TABLE") 

//keep track of the padding level for comparison with any children 

var parentTreeLevel = obj.rows[0].cells.length; 

var parentTreeNode = obj.rows[0].cells[0]; 

//get all the TreeNodes inside the TreeView (the parent <div>) 

var tables = obj.parentElement.getElementsByTagName("TABLE"); 

//checking for any node is checked or unchecked during operation 

if(obj.tagName == "TABLE") {


// if any node is unchecked then their parent node are unchecked 

if (!treeNode.checked) {

//head1 gets the parent node of the unchecked node 

var head1 = obj.parentElement.previousSibling; 

if(head1.tagName == "TABLE") {

//checks for the input tag which consists of checkbox 

var matchElement1 = head1.getElementsByTagName("INPUT"); 

//matchElement1[0] gives us the checkbox and it is unchecked 

matchElement1[0].checked = false; 

}

else {

head1 = obj.parentElement.previousSibling;

} 

if(head1.tagName == "TABLE") {

//head2 gets the parent node of the unchecked node 

var head2 = obj.parentElement.parentElement.previousSibling; 

if(head2.tagName == "TABLE") {

//checks for the input tag which consists of checkbox 

var matchElement2 = head2.getElementsByTagName("INPUT"); 

matchElement2[0].checked = false; 

}

}

else {

head2 = obj.parentElement.previousSibling;

} 

if(head2.tagName == "TABLE") {

//head3 gets the parent node of the unchecked node 

var head3 = obj.parentElement.parentElement.parentElement.previousSibling; 

if(head3.tagName == "TABLE") {

//checks for the input tag which consists of checkbox 

var matchElement3 = head3.getElementsByTagName("INPUT"); 

matchElement3[0].checked = false; 

}

}

else {

head3 = obj.parentElement.previousSibling;

} 

if(head3.tagName == "TABLE") { 

//head4 gets the parent node of the unchecked node 

var head4 = obj.parentElement.parentElement.parentElement.parentElement.previousSibling; 

if(head4 != null) { 

if(head4.tagName == "TABLE") { 

//checks for the input tag which consists of checkbox 

var matchElement4 = head4.getElementsByTagName("INPUT"); 

matchElement4[0].checked = false; 

}

}

} 

} //end if - unchecked

//total number of TreeNodes 

var numTables = tables.length 

if (numTables >= 1) {

//cycle through all the TreeNodes 

//until we find the TreeNode we checked 

for (i=0; i < numTables; i++) {

if (tables[i] == obj) {

treeNodeFound = true; 

i++;

if (i == numTables) {

//if we're on the last 

//TreeNode then stop 

//return;

break;

}

} 

if (treeNodeFound == true) {

var childTreeLevel = tables[i].rows[0].cells.length; 

//if the current node is under the parent 

//the level will be deeper (greater) 

if (childTreeLevel > parentTreeLevel) { 

//jump to the last cell... it contains the checkbox 

var cell = tables[i].rows[0].cells[childTreeLevel - 1]; 

//set the checkbox to match the checkedState 

//of the TreeNode that was clicked 

var inputs = cell.getElementsByTagName("INPUT"); 

inputs[0].checked = checkedState; 

} 

else { 

//if any of the preceding TreeNodes are not deeper stop 

//return; 

break;

}

} //end if 

}//end for 

} //end if - numTables >= 1

// if all child nodes are checked then their parent node is checked

if (treeNode.checked) {

var chk1 = true;

var head1 = obj.parentElement.previousSibling;

var pTreeLevel1 = obj.rows[0].cells.length;

if(head1.tagName == "TABLE") {

var tbls = obj.parentElement.getElementsByTagName("TABLE");

var tblsCount = tbls.length;

for (i=0; i < tblsCount; i++) {

var childTreeLevel = tbls[i].rows[0].cells.length;

if (childTreeLevel = pTreeLevel1) {

var chld = tbls[i].getElementsByTagName("INPUT");

if (chld[0].checked == false) {

chk1 = false;

break;

}

}

}

var nd = head1.getElementsByTagName("INPUT");

nd[0].checked = chk1;

}

else {

head1 = obj.parentElement.previousSibling;

}

var chk2 = true;

if(head1.tagName == "TABLE") {

var head2 = obj.parentElement.parentElement.previousSibling; 

if(head2.tagName == "TABLE") {

var tbls = head1.parentElement.getElementsByTagName("TABLE");

var pTreeLevel2 = head1.rows[0].cells.length;

var tblsCount = tbls.length;

for (i=0; i < tblsCount; i++) {

var childTreeLevel = tbls[i].rows[0].cells.length;

if (childTreeLevel = pTreeLevel2) {

var chld = tbls[i].getElementsByTagName("INPUT");

if (chld[0].checked == false) {

chk2 = false;

break;

}

}

}

var nd = head2.getElementsByTagName("INPUT");

nd[0].checked = (chk2 && chk1);

}

}

else {

head2 = obj.parentElement.previousSibling;

}

var chk3 = true;

if(head2.tagName == "TABLE") {

var head3 = obj.parentElement.parentElement.parentElement.previousSibling; 

if(head3.tagName == "TABLE") {

var tbls = head2.parentElement.getElementsByTagName("TABLE");

var pTreeLevel3 = head2.rows[0].cells.length;

var tblsCount = tbls.length;

for (i=0; i < tblsCount; i++) {

var childTreeLevel = tbls[i].rows[0].cells.length;

if (childTreeLevel = pTreeLevel3) {

var chld = tbls[i].getElementsByTagName("INPUT");

if (chld[0].checked == false) {

chk3 = false;

break;

}

}

}

var nd = head3.getElementsByTagName("INPUT");

nd[0].checked = (chk3 && chk2 && chk1);

}

}

else {

head3 = obj.parentElement.previousSibling;

}

var chk4 = true;

if(head3.tagName == "TABLE") {

var head4 = obj.parentElement.parentElement.parentElement.parentElement.previousSibling; 

if(head4.tagName == "TABLE") {

var tbls = head3.parentElement.getElementsByTagName("TABLE");

var pTreeLevel4 = head3.rows[0].cells.length;

var tblsCount = tbls.length;

for (i=0; i < tblsCount; i++) {

var childTreeLevel = tbls[i].rows[0].cells.length;

if (childTreeLevel = pTreeLevel4) {

var chld = tbls[i].getElementsByTagName("INPUT");

if (chld[0].checked == false) {

chk4 = false;

break;

}

}

}

var nd = head4.getElementsByTagName("INPUT");

nd[0].checked = (chk4 && chk3 && chk2 && chk1);

}

}

}//end if - checked

} //end if - tagName = TABLE

} //end if

} //end function


   
   
   function client_OnTreeNodeChecked()
    {
        var obj = window.event.srcElement;
        var treeNodeFound = false;
        var checkedState;
        if (obj.tagName == "INPUT" && obj.type == "checkbox") 
        {
            var treeNode = obj;
            checkedState = treeNode.checked;
            do
            {
                obj = obj.parentElement;
            } while (obj.tagName != "TABLE")
            
            var parentTreeLevel = obj.rows[0].cells.length;
            var parentTreeNode = obj.rows[0].cells[0];
            var tables = obj.parentElement.getElementsByTagName("TABLE");
            var numTables = tables.length
            if (numTables >= 1)
            {
                for (i=0; i < numTables; i++)
                {
                    if (tables[i] == obj)
                    {
                        treeNodeFound = true;
                        i++;
                        if (i == numTables)
                        {
                            return;
                        }
                    }
                    if (treeNodeFound == true)
                    {
                        var childTreeLevel = tables[i].rows[0].cells.length;
                        if (childTreeLevel > parentTreeLevel)
                        {
                            var cell = tables[i].rows[0].cells[childTreeLevel - 1];
                            var inputs = cell.getElementsByTagName("INPUT");
                            inputs[0].checked = checkedState;
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
        }
    }
 

    function CheckBoxListSelect(cbControl, state)
    {    
       var chkBoxList = document.getElementById(cbControl);
        var chkBoxCount= chkBoxList.getElementsByTagName("input");
        for(var i=0;i<chkBoxCount.length;i++) 
        {
            chkBoxCount[i].checked = state;
        }
        
        return false; 
    }

function mysubmit() 
{
    var plateno=document.getElementById("ddlpleate").value;
    if (plateno=="--Select Plate No--")
    {
         alert("Please select vehicle plate number");
         return false;         
    }
    return true;
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

function ShowCalendar(strTargetDateField, intLeft, intTop)
{
    txtTargetDateField = strTargetDateField;
    var bsmallCalendar=document.getElementById("beginCalendar");
    var divTWCalendarobj=document.getElementById("divTWCalendar");
    divTWCalendarobj.style.visibility = 'visible';
    divTWCalendarobj.style.left = bsmallCalendar.offsetLeft+"px";
    divTWCalendarobj.style.top = (bsmallCalendar.offsetTop+20)+"px";
       selecteddate(txtTargetDateField);  
}

    </script>

</head>
<body style="margin-left: 5px; margin-top: 0px; margin-bottom: 0px; margin-right: 5px;">
    <form id="Form1" method="post" runat="server">
	<script type="text/javascript" language="javascript">DrawCalendarLayout();</script>
            <center>
            <div>
                <br />
                <table>
                    <tr>
                        <td align="center">
                            <table style="font-family: Verdana; font-size: 11px;">
                                <tr>
                                    <td style="height: 20px; background-color: #465ae8;" align="left">
                                        <b style="color: White;">&nbsp;GeofenceSummaryPostProcess &nbsp;:</b></td>
                                </tr>
                                <tr>
                                    <td style="width: 420px; border: solid 1px #3952F9;">
                                        <table style="width: 420px;">
                                            <tbody>
                                                <tr>
                                                    <td align="left">
                                                        <b style="color: #5f7afc;">Date Time</b>&nbsp;</td>
                                                    <td>
                                                        <b style="color: #5f7afc;">:</b>
                                                    </td>
                                                    <td align="left">
                                                        <input readonly="readonly" style="width: 70px;" type="text" value="<%=strBeginDate%>"
                                                            id="txtBeginDate" runat="server" name="txtBeginDate" enableviewstate="false" />&nbsp;
                                                            <input readonly="readonly" style="width: 70px;" type="text" value="<%=strBeginDate%>"
                                                            id="txtEndDate" runat="server" name="txtEndDate" enableviewstate="false" />
                                                            <asp:Label ID="lblmsg" Text="" runat="server" ></asp:Label>
                                                   <%--     <a
                                                                            href="javascript:ShowCalendar('txtBeginDate', 250, 250);" style="text-decoration: none">
                                                                            <img id="beginCalendar" alt="Show calendar control" height="14" src="images/Calendar.jpg"
                                                                                style="border-top: #005583 1px solid; border-left: #005583 1px solid; border-bottom: #005583 1px solid;
                                                                                position: relative" title="Show calendar control" width="19" /></a>   --%>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        <b style="color: #5f7afc;">Plate No </b>
                                                    </td>
                                                    <td>
                                                        <b style="color: #5f7afc;">:</b></td>
                                                    <td align="left">
                                                        <asp:Panel ID="Panel1" runat="server" BorderColor="Gray" BorderStyle="Solid" BorderWidth="1px"
                                                            Height="200px" ScrollBars="Vertical" Width="280px">
                                                            <asp:TreeView ID="tvPlateno" runat="server" ExpandDepth="0" ForeColor="Black" onclick="treeViewCheck(event);"
                                                                ShowCheckBoxes="All" ShowLines="True">
                                                                <ParentNodeStyle Font-Bold="True" />
                                                                <RootNodeStyle Font-Bold="True" />
                                                                <LeafNodeStyle Font-Italic="True" />
                                                            </asp:TreeView>
                                                        </asp:Panel>
                                                    </td>
                                                </tr>
                                                
                                                <tr>
                                                    <td align="center">
                                                        <br />
                                                   
                                                      <a href="../ERP.aspx">
                                                            <img src="images/back.jpg" alt="Back" style="border: 0px; vertical-align: top; cursor: pointer"
                                                                title="Back" />
                                                        </a>
                                                    </td>
                                                    <td colspan="2" align="center">
                                                        <br />
                                                        <asp:ImageButton ID="ImageButton1" runat="server" ImageUrl="images/Submit_s.jpg"
                                                            ToolTip="Submit"></asp:ImageButton>
                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <a href="javascript:ExcelReport();">
                                                            <img alt="Save to Excel file" title="Save to Excel file" src="images/SaveExcel.jpg"
                                                                style="border: solid 0px blue;" /></a>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                                        </td>
                                </tr>
                            </table>
                            <p style="font-family: Verdana; font-size: 11px; color: #5373a2;">
                                Copyright © 2013 Global Telematics Sdn Bhd. All rights reserved.</p>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;</td>
                    </tr>
                    <tr>
                        <td align="left">
                            &nbsp;<asp:Label ID="Label3" runat="server" Font-Names="Arial" Font-Size="X-Small" ForeColor="Red"
                                Text="*  RM/KM will be higher if idling time is too long for the selected period. " Visible="False"></asp:Label><br />
                            <asp:Label ID="Label2" runat="server" Font-Names="Arial" Font-Size="X-Small" ForeColor="Red"
                                Text="** Minimum distance of 50KM is required for effective calculation on RM/KM." Visible="False"></asp:Label></td>
                    </tr>
                   
                    <tr align="center">
                        <td>
                            <table>
                                <tr>
                                    <td align="left">
                                        <div style="font-family: Verdana; font-size: 11px;">
                                            <br />
                                            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" EnableViewState="False"
                                                Font-Bold="False" Font-Overline="False" HeaderStyle-BackColor="#465AE8" HeaderStyle-Font-Bold="True"
                                                HeaderStyle-Font-Size="12px" HeaderStyle-ForeColor="#FFFFFF" HeaderStyle-Height="22px"
                                                HeaderStyle-HorizontalAlign="Center"  
                                                PageSize="1" Width="100%" ShowFooter="True">
                                                <PagerSettings PageButtonCount="5" />
                                                <PagerStyle BackColor="White" Font-Bold="True" Font-Italic="False" Font-Names="Verdana"
                                                    Font-Overline="False" Font-Size="Small" Font-Strikeout="False" HorizontalAlign="Center"
                                                    VerticalAlign="Middle" />
                                                <Columns>
                                                    <asp:BoundField DataField="No" HeaderText="No" HtmlEncode="False">
                                                        <ItemStyle Width="3px" />
                                                    </asp:BoundField>
                                                     <asp:BoundField DataField="User Name" HeaderText="User Name" HtmlEncode="False" HeaderStyle-Width="180px">
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Plate No" HeaderText="Plate No" HtmlEncode="False">
                                                        <ItemStyle />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Begin Date Time" HeaderText="Going-In Time" HtmlEncode="False" HeaderStyle-Width="140px">
                                                        <ItemStyle HorizontalAlign="center" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="End Date Time" HeaderText="Going-Out  Time" HtmlEncode="False" HeaderStyle-Width="140px">
                                                        <ItemStyle HorizontalAlign="center"/>
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Trip Time" HeaderText="Stay Time" HtmlEncode="False" HeaderStyle-Width="70px">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                    </asp:BoundField>
                                                   <asp:BoundField DataField="Idling Time" HeaderText="Idling Time" HtmlEncode="False" HeaderStyle-Width="90px">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                    </asp:BoundField> 
                                                    <asp:BoundField DataField="Mileage" HeaderText="Mileage" HtmlEncode="False" HeaderStyle-Width="70px">
                                                        <ItemStyle HorizontalAlign="Right" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Geofence" HeaderText="Geofence" HtmlEncode="False">
                                                        <ItemStyle HorizontalAlign="Left" />
                                                    </asp:BoundField>
                                                     <asp:BoundField DataField="Maps" HeaderText="Maps" HtmlEncode="False">
                                                        <ItemStyle Width="80px" HorizontalAlign="center"  />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="View" HeaderText="View" HtmlEncode="False">
                                                        <ItemStyle Width="40px" HorizontalAlign="center"  />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Trail" HeaderText="Trail" HtmlEncode="False">
                                                        <ItemStyle Width="40px" HorizontalAlign="center"  />
                                                    </asp:BoundField>
                                                   
                                                     
                                                </Columns>
                                                <AlternatingRowStyle BackColor="Lavender" />
                                                <HeaderStyle BackColor="#465AE8" Font-Bold="True" Font-Size="12px" ForeColor="White"
                                                    Height="22px" HorizontalAlign="Center" />
                                                <FooterStyle BackColor="#C0C0FF" ForeColor="Black" Font-Bold="True" HorizontalAlign="Right" />
                                            </asp:GridView>
                                            <% If show = True Then%>
                                            <center>
                                                <label id="pages" style="font-family: Verdana; font-size: 11px; font-weight: bold;">
                                                    Pages</label></center>
                                            <%End If%>
                                            <br />

                                            
                                        </div>
                                    </td>
                                </tr>
                                <asp:GridView ID="GridView2" runat="server">
                                </asp:GridView>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
        </center>
    </form>
    <form id="excelform" method="get" action="ExcelReport.aspx">
        <input type="hidden" id="title" name="title" value="All Vehicles Daily Summary Report" />
        <input type="hidden" id="plateno" name="plateno" value="" />
    </form>
</body>
</html>
