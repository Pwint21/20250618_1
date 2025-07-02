<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.vehicleUsageReportLitePost" Codebehind="vehicleUsageReportLitePost.aspx.vb" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1">
      <title>Vehicle Usage Report</title>

    <link rel="stylesheet" href="cssfiles/css3-buttons.css" type="text/css" media="screen"/>
      <link type="text/css"  href="cssfiles/jquery-ui.css" rel="stylesheet" />
   
      <script type="text/javascript" src="jsfiles/jquery.min.js"></script>
      <script type="text/javascript" src="jsfiles/jquery-ui.min.js"></script>
      
      <script type="text/javascript" language="javascript">

          $(function () {
              $("#txtBeginDate").datepicker({ dateFormat: 'yy/mm/dd', minDate: -180, maxDate: +0, changeMonth: true, changeYear: true, numberOfMonths: 2
              });

              $("#txtEndDate").datepicker({ dateFormat: 'yy/mm/dd', minDate: -180, maxDate: +0, changeMonth: true, changeYear: true, numberOfMonths: 2

              });
          });
   
   var ec=<%=ec %>;
function mysubmit()
{
    var plateno=document.getElementById("ddlpleate").value;
    if (plateno=="--Select User Name--")
    {
        alert("Select User Name");
        return false;
     }   
    if (plateno=="--Select Plate No--")
    {
         alert("Select Plate Number");
         return false;         
    }
    var bigindatetime=document.getElementById("txtBeginDate").value;
    var enddatetime=document.getElementById("txtEndDate").value;
    
    var fdate=Date.parse(bigindatetime);
    var sdate=Date.parse(enddatetime);
    
    var diff=(sdate-fdate)*(1/(1000*60*60*24));
    var days=parseInt(diff)+1;
    if(days>93)
    {
        alert("Sorry you've selected "+days+" Maximum 93 days of report can be shown.");
         return false; 
    }
    else if(diff < 0)
    {
        alert("");
         return false; 
    }
    return true;
     
}
function ExcelReport()
{
    if(ec==true)
    {
        	//ec=false;
	var plateno=document.getElementById("ddlpleate").value;
       
        document.getElementById("plateno").value=plateno;

        var excelformobj=document.getElementById("excelform");
        excelformobj.submit();
    }
    else
    {
        alert("Please click submit");
    }
}
       

function mouseover(x,y)
{
   
}
function googlemouseover(x,y)
{
   
}

    </script>
</head>
<body style="margin-top: 0px; margin-left: 0px; margin-right: 15px; margin-bottom: 0px;"
    onload="conv()">
    <center>
        <form id="Form1" method="post" runat="server">
         
 
   
        <div>
            <br />
            <b style="font-family: Verdana; font-size: 20px; color: #38678B;">Vehicle Usage Report</b>
            <br />
            <br />
            <table width="1200px">
                <tr>
                    <td align="center">
                        <table style="font-family: Verdana; font-size: 11px;">
                            <tr>
                                <td style="height: 20px; background-color: #465ae8;" align="left">
                                    <b style="color: White;">&nbsp;Vehicle Usage Report &nbsp;:</b>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 450px; border: solid 1px #3952F9;">
                                    <table style="width: 450px;">
                                        <tbody>
                                            <tr>
                                                <td align="left">
                                                    <b style="color: #5f7afc; font-family: Verdana; font-size: 11px;">From Date</b>
                                                </td>
                                                <td>
                                                    <b style="color: #5f7afc; font-family: Verdana; font-size: 11px;">:</b>
                                                </td>
                                                <td align="left">
                                                    <input readonly="readonly" style="width: 70px;" type="text" value="<%=strBeginDate%>"
                                                        id="txtBeginDate" runat="server" name="txtBeginDate" enableviewstate="false" />
                                                    <b style="color: #5f7afc; font-family: Verdana; font-size: 11px;">&nbsp;HH&nbsp;:&nbsp;</b>
                                                    <asp:DropDownList ID="ddlbh" runat="server" Width="42px" Font-Size="12px" Font-Names="verdana"
                                                        EnableViewState="False">
                                                        <asp:ListItem Value="00">00</asp:ListItem>
                                                        <asp:ListItem Value="01">01</asp:ListItem>
                                                        <asp:ListItem Value="02">02</asp:ListItem>
                                                        <asp:ListItem Value="03">03</asp:ListItem>
                                                        <asp:ListItem Value="04">04</asp:ListItem>
                                                        <asp:ListItem Value="05">05</asp:ListItem>
                                                        <asp:ListItem Value="06">06</asp:ListItem>
                                                        <asp:ListItem Value="07">07</asp:ListItem>
                                                        <asp:ListItem Value="08">08</asp:ListItem>
                                                        <asp:ListItem Value="09">09</asp:ListItem>
                                                        <asp:ListItem Value="10">10</asp:ListItem>
                                                        <asp:ListItem Value="11">11</asp:ListItem>
                                                        <asp:ListItem Value="12">12</asp:ListItem>
                                                        <asp:ListItem Value="13">13</asp:ListItem>
                                                        <asp:ListItem Value="14">14</asp:ListItem>
                                                        <asp:ListItem Value="15">15</asp:ListItem>
                                                        <asp:ListItem Value="16">16</asp:ListItem>
                                                        <asp:ListItem Value="17">17</asp:ListItem>
                                                        <asp:ListItem Value="18">18</asp:ListItem>
                                                        <asp:ListItem Value="19">19</asp:ListItem>
                                                        <asp:ListItem Value="20">20</asp:ListItem>
                                                        <asp:ListItem Value="21">21</asp:ListItem>
                                                        <asp:ListItem Value="22">22</asp:ListItem>
                                                        <asp:ListItem Value="23">23</asp:ListItem>
                                                    </asp:DropDownList>
                                                    <b style="color: #5f7afc; font-family: Verdana; font-size: 11px;">&nbsp;MM&nbsp;:&nbsp;</b>
                                                    <asp:DropDownList ID="ddlbm" runat="server" Width="42px" Font-Size="12px" Font-Names="verdana"
                                                        EnableViewState="False">
                                                        <asp:ListItem Value="00">00</asp:ListItem>
                                                        <asp:ListItem Value="01">01</asp:ListItem>
                                                        <asp:ListItem Value="02">02</asp:ListItem>
                                                        <asp:ListItem Value="03">03</asp:ListItem>
                                                        <asp:ListItem Value="04">04</asp:ListItem>
                                                        <asp:ListItem Value="05">05</asp:ListItem>
                                                        <asp:ListItem Value="06">06</asp:ListItem>
                                                        <asp:ListItem Value="07">07</asp:ListItem>
                                                        <asp:ListItem Value="08">08</asp:ListItem>
                                                        <asp:ListItem Value="09">09</asp:ListItem>
                                                        <asp:ListItem Value="10">10</asp:ListItem>
                                                        <asp:ListItem Value="11">11</asp:ListItem>
                                                        <asp:ListItem Value="12">12</asp:ListItem>
                                                        <asp:ListItem Value="13">13</asp:ListItem>
                                                        <asp:ListItem Value="14">14</asp:ListItem>
                                                        <asp:ListItem Value="15">15</asp:ListItem>
                                                        <asp:ListItem Value="16">16</asp:ListItem>
                                                        <asp:ListItem Value="17">17</asp:ListItem>
                                                        <asp:ListItem Value="18">18</asp:ListItem>
                                                        <asp:ListItem Value="19">19</asp:ListItem>
                                                        <asp:ListItem Value="20">20</asp:ListItem>
                                                        <asp:ListItem Value="21">21</asp:ListItem>
                                                        <asp:ListItem Value="22">22</asp:ListItem>
                                                        <asp:ListItem Value="23">23</asp:ListItem>
                                                        <asp:ListItem Value="24">24</asp:ListItem>
                                                        <asp:ListItem Value="25">25</asp:ListItem>
                                                        <asp:ListItem Value="26">26</asp:ListItem>
                                                        <asp:ListItem Value="27">27</asp:ListItem>
                                                        <asp:ListItem Value="28">28</asp:ListItem>
                                                        <asp:ListItem Value="29">29</asp:ListItem>
                                                        <asp:ListItem Value="30">30</asp:ListItem>
                                                        <asp:ListItem Value="31">31</asp:ListItem>
                                                        <asp:ListItem Value="32">32</asp:ListItem>
                                                        <asp:ListItem Value="33">33</asp:ListItem>
                                                        <asp:ListItem Value="34">34</asp:ListItem>
                                                        <asp:ListItem Value="35">35</asp:ListItem>
                                                        <asp:ListItem Value="36">36</asp:ListItem>
                                                        <asp:ListItem Value="37">37</asp:ListItem>
                                                        <asp:ListItem Value="38">38</asp:ListItem>
                                                        <asp:ListItem Value="39">39</asp:ListItem>
                                                        <asp:ListItem Value="40">40</asp:ListItem>
                                                        <asp:ListItem Value="41">41</asp:ListItem>
                                                        <asp:ListItem Value="42">42</asp:ListItem>
                                                        <asp:ListItem Value="43">43</asp:ListItem>
                                                        <asp:ListItem Value="44">44</asp:ListItem>
                                                        <asp:ListItem Value="45">45</asp:ListItem>
                                                        <asp:ListItem Value="46">46</asp:ListItem>
                                                        <asp:ListItem Value="47">47</asp:ListItem>
                                                        <asp:ListItem Value="48">48</asp:ListItem>
                                                        <asp:ListItem Value="49">49</asp:ListItem>
                                                        <asp:ListItem Value="50">50</asp:ListItem>
                                                        <asp:ListItem Value="51">51</asp:ListItem>
                                                        <asp:ListItem Value="52">52</asp:ListItem>
                                                        <asp:ListItem Value="53">53</asp:ListItem>
                                                        <asp:ListItem Value="54">54</asp:ListItem>
                                                        <asp:ListItem Value="55">55</asp:ListItem>
                                                        <asp:ListItem Value="56">56</asp:ListItem>
                                                        <asp:ListItem Value="57">57</asp:ListItem>
                                                        <asp:ListItem Value="58">58</asp:ListItem>
                                                        <asp:ListItem Value="59">59</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <b style="color: #5f7afc; font-family: Verdana; font-size: 11px;">To Date</b>
                                                </td>
                                                <td>
                                                    <b style="color: #5f7afc; font-family: Verdana; font-size: 11px;">:</b>
                                                </td>
                                                <td align="left">
                                                    <input style="width: 70px;" readonly="readonly" type="text" value="<%=strEndDate%>"
                                                        id="txtEndDate" runat="server" name="txtEndDate" enableviewstate="false" />&nbsp;<b
                                                            style="color: #5f7afc; font-family: Verdana; font-size: 11px;">&nbsp;HH&nbsp;:&nbsp;</b>
                                                    <asp:DropDownList ID="ddleh" runat="server" Width="42px" Font-Size="12px" Font-Names="verdana"
                                                        EnableViewState="False">
                                                        <asp:ListItem Value="00">00</asp:ListItem>
                                                        <asp:ListItem Value="01">01</asp:ListItem>
                                                        <asp:ListItem Value="02">02</asp:ListItem>
                                                        <asp:ListItem Value="03">03</asp:ListItem>
                                                        <asp:ListItem Value="04">04</asp:ListItem>
                                                        <asp:ListItem Value="05">05</asp:ListItem>
                                                        <asp:ListItem Value="06">06</asp:ListItem>
                                                        <asp:ListItem Value="07">07</asp:ListItem>
                                                        <asp:ListItem Value="08">08</asp:ListItem>
                                                        <asp:ListItem Value="09">09</asp:ListItem>
                                                        <asp:ListItem Value="10">10</asp:ListItem>
                                                        <asp:ListItem Value="11">11</asp:ListItem>
                                                        <asp:ListItem Value="12">12</asp:ListItem>
                                                        <asp:ListItem Value="13">13</asp:ListItem>
                                                        <asp:ListItem Value="14">14</asp:ListItem>
                                                        <asp:ListItem Value="15">15</asp:ListItem>
                                                        <asp:ListItem Value="16">16</asp:ListItem>
                                                        <asp:ListItem Value="17">17</asp:ListItem>
                                                        <asp:ListItem Value="18">18</asp:ListItem>
                                                        <asp:ListItem Value="19">19</asp:ListItem>
                                                        <asp:ListItem Value="20">20</asp:ListItem>
                                                        <asp:ListItem Value="21">21</asp:ListItem>
                                                        <asp:ListItem Value="22">22</asp:ListItem>
                                                        <asp:ListItem Value="23" Selected="True">23</asp:ListItem>
                                                    </asp:DropDownList>
                                                    <b style="color: #5f7afc; font-family: Verdana; font-size: 11px;">&nbsp;MM&nbsp;:&nbsp;</b>
                                                    <asp:DropDownList ID="ddlem" runat="server" Width="42px" Font-Size="12px" Font-Names="verdana"
                                                        EnableViewState="False">
                                                        <asp:ListItem Value="00">00</asp:ListItem>
                                                        <asp:ListItem Value="01">01</asp:ListItem>
                                                        <asp:ListItem Value="02">02</asp:ListItem>
                                                        <asp:ListItem Value="03">03</asp:ListItem>
                                                        <asp:ListItem Value="04">04</asp:ListItem>
                                                        <asp:ListItem Value="05">05</asp:ListItem>
                                                        <asp:ListItem Value="06">06</asp:ListItem>
                                                        <asp:ListItem Value="07">07</asp:ListItem>
                                                        <asp:ListItem Value="08">08</asp:ListItem>
                                                        <asp:ListItem Value="09">09</asp:ListItem>
                                                        <asp:ListItem Value="10">10</asp:ListItem>
                                                        <asp:ListItem Value="11">11</asp:ListItem>
                                                        <asp:ListItem Value="12">12</asp:ListItem>
                                                        <asp:ListItem Value="13">13</asp:ListItem>
                                                        <asp:ListItem Value="14">14</asp:ListItem>
                                                        <asp:ListItem Value="15">15</asp:ListItem>
                                                        <asp:ListItem Value="16">16</asp:ListItem>
                                                        <asp:ListItem Value="17">17</asp:ListItem>
                                                        <asp:ListItem Value="18">18</asp:ListItem>
                                                        <asp:ListItem Value="19">19</asp:ListItem>
                                                        <asp:ListItem Value="20">20</asp:ListItem>
                                                        <asp:ListItem Value="21">21</asp:ListItem>
                                                        <asp:ListItem Value="22">22</asp:ListItem>
                                                        <asp:ListItem Value="23">23</asp:ListItem>
                                                        <asp:ListItem Value="24">24</asp:ListItem>
                                                        <asp:ListItem Value="25">25</asp:ListItem>
                                                        <asp:ListItem Value="26">26</asp:ListItem>
                                                        <asp:ListItem Value="27">27</asp:ListItem>
                                                        <asp:ListItem Value="28">28</asp:ListItem>
                                                        <asp:ListItem Value="29">29</asp:ListItem>
                                                        <asp:ListItem Value="30">30</asp:ListItem>
                                                        <asp:ListItem Value="31">31</asp:ListItem>
                                                        <asp:ListItem Value="32">32</asp:ListItem>
                                                        <asp:ListItem Value="33">33</asp:ListItem>
                                                        <asp:ListItem Value="34">34</asp:ListItem>
                                                        <asp:ListItem Value="35">35</asp:ListItem>
                                                        <asp:ListItem Value="36">36</asp:ListItem>
                                                        <asp:ListItem Value="37">37</asp:ListItem>
                                                        <asp:ListItem Value="38">38</asp:ListItem>
                                                        <asp:ListItem Value="39">39</asp:ListItem>
                                                        <asp:ListItem Value="40">40</asp:ListItem>
                                                        <asp:ListItem Value="41">41</asp:ListItem>
                                                        <asp:ListItem Value="42">42</asp:ListItem>
                                                        <asp:ListItem Value="43">43</asp:ListItem>
                                                        <asp:ListItem Value="44">44</asp:ListItem>
                                                        <asp:ListItem Value="45">45</asp:ListItem>
                                                        <asp:ListItem Value="46">46</asp:ListItem>
                                                        <asp:ListItem Value="47">47</asp:ListItem>
                                                        <asp:ListItem Value="48">48</asp:ListItem>
                                                        <asp:ListItem Value="49">49</asp:ListItem>
                                                        <asp:ListItem Value="50">50</asp:ListItem>
                                                        <asp:ListItem Value="51">51</asp:ListItem>
                                                        <asp:ListItem Value="52">52</asp:ListItem>
                                                        <asp:ListItem Value="53">53</asp:ListItem>
                                                        <asp:ListItem Value="54">54</asp:ListItem>
                                                        <asp:ListItem Value="55">55</asp:ListItem>
                                                        <asp:ListItem Value="56">56</asp:ListItem>
                                                        <asp:ListItem Value="57">57</asp:ListItem>
                                                        <asp:ListItem Value="58">58</asp:ListItem>
                                                        <asp:ListItem Value="59" Selected="True">59</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <b style="color: #5f7afc">User Name</b>
                                                </td>
                                                <td>
                                                    <b style="color: #5f7afc">:</b>
                                                </td>
                                                <td align="left">
                                                    <asp:DropDownList ID="ddlUsername" runat="server" AutoPostBack="True" Font-Names="verdana"
                                                        Font-Size="12px" Width="259px">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <b style="color: #5f7afc;">Plate Number </b>
                                                </td>
                                                <td>
                                                    <b style="color: #5f7afc;">:</b>
                                                </td>
                                                <td align="left">
                                                    <asp:DropDownList ID="ddlpleate" runat="server" Width="259px" Font-Size="12px" Font-Names="verdana">
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center">
                                                    <br />
                                                </td>
                                                <td align="center" colspan="2">
                                                    <br />
                                                    <table border="0" cellpadding="0" cellspacing="0">
                                                        <tr>
                                                            <td>
                                                                <asp:Button ID="ImageButton1" class="action blue" runat="server" style="width:85px;" Text="Submit" ToolTip="Submit" />
                                                            </td>
                                                            <td>
                                                                <a href="javascript:ExcelReport();" class="button" style="vertical-align: top; width:auto;"><span class="ui-button-text" title="Save Excel">
                                                                    Download</a>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <%--<tr>
                    <td align="center">
                        <chart:WebChartViewer ID="WebChartViewer1" runat="server" Visible="False"></chart:WebChartViewer>
                        <asp:Image ID="Image1" runat="server" Visible="False" /><br />
                    </td>
                </tr>--%>
                <tr align="left">
                    <td align="center" colspan="2">
                        <asp:Repeater ID="customers" runat="server" EnableViewState="False">
                            <HeaderTemplate>
                                <table border="0" width="100%;" style="font-family: Verdana; font-size: 12px">
                                    <tr style="background-color: #465AE8; color: #FFFFFF">
                                        <th>
                                            Date
                                        </th>
                                        <th colspan="5">
                                            Stops
                                        </th>
                                        <th colspan="5">
                                            Idles
                                        </th>
                                    </tr>
                                    <tr style="background-color: #465AE8; color: #FFFFFF">
                                        <th>
                                        </th>
                                        <th>
                                            >15 Min
                                        </th>
                                        <th>
                                            >30 Min
                                        </th>
                                        <th>
                                            >60 Min
                                        </th>
                                        <th>
                                            >90 Min
                                        </th>
                                        <th>
                                            Total Time
                                        </th>
                                        <th>
                                            >15 Min
                                        </th>
                                        <th>
                                            >30 Min
                                        </th>
                                        <th>
                                            >60 Min
                                        </th>
                                        <th>
                                            >90 Min
                                        </th>
                                        <th>
                                            Total Time
                                        </th>
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr style="background-color: #E7E8F8; font-size: 11px;">
                                    <td align="center">
                                        <%#Container.DataItem(0)%>
                                    </td>
                                    <td align="center">
                                        <%#Container.DataItem(1)%>
                                    </td>
                                    <td align="center">
                                        <%#Container.DataItem(2)%>
                                    </td>
                                    <td align="center">
                                        <%#Container.DataItem(3)%>
                                    </td>
                                    <td align="center">
                                        <%#Container.DataItem(4)%>
                                    </td>
                                    <td align="center">
                                        <%#Container.DataItem(5)%>
                                    </td>
                                    <td align="center">
                                        <%#Container.DataItem(6)%>
                                    </td>
                                    <td align="center">
                                        <%#Container.DataItem(7)%>
                                    </td>
                                    <td align="center">
                                        <%#Container.DataItem(8)%>
                                    </td>
                                    <td align="center">
                                        <%#Container.DataItem(9)%>
                                    </td>
                                    <td align="center">
                                        <%#Container.DataItem(10)%>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <AlternatingItemTemplate>
                                <tr style="background-color: #FFFFFF; font-size: 11px;">
                                    <td align="center">
                                        <%#Container.DataItem(0)%>
                                    </td>
                                    <td align="center">
                                        <%#Container.DataItem(1)%>
                                    </td>
                                    <td align="center">
                                        <%#Container.DataItem(2)%>
                                    </td>
                                    <td align="center">
                                        <%#Container.DataItem(3)%>
                                    </td>
                                    <td align="center">
                                        <%#Container.DataItem(4)%>
                                    </td>
                                    <td align="center">
                                        <%#Container.DataItem(5)%>
                                    </td>
                                    <td align="center">
                                        <%#Container.DataItem(6)%>
                                    </td>
                                    <td align="center">
                                        <%#Container.DataItem(7)%>
                                    </td>
                                    <td align="center">
                                        <%#Container.DataItem(8)%>
                                    </td>
                                    <td align="center">
                                        <%#Container.DataItem(9)%>
                                    </td>
                                    <td align="center">
                                        <%#Container.DataItem(10)%>
                                    </td>
                                </tr>
                            </AlternatingItemTemplate>
                            <FooterTemplate>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                        <div style="font-family: Verdana; font-size: 11px;">
                            <asp:GridView ID="GridView2" runat="server" AllowPaging="True" Width="100%" PageSize="20"
                                HeaderStyle-Font-Size="12" HeaderStyle-ForeColor="#FFFFFF" HeaderStyle-BackColor="#465AE8"
                                HeaderStyle-Font-Bold="True" Font-Bold="False" Font-Overline="False" AutoGenerateColumns="False"
                                EnableViewState="False" HeaderStyle-Height="22px" HeaderStyle-HorizontalAlign="Center">
                                <PagerSettings PageButtonCount="5" />
                                <PagerStyle Font-Bold="True" Font-Names="Verdana" Font-Size="Small" HorizontalAlign="Center"
                                    VerticalAlign="Middle" BackColor="White" Font-Italic="False" Font-Overline="False"
                                    Font-Strikeout="False" />
                                <Columns>
                                    <asp:BoundField DataField="No of Stationaryes" HeaderText="No of Stops" HtmlEncode="false" />
                                    <asp:BoundField DataField="Total Stationary Time" HeaderText="Total Stops Time" HtmlEncode="false" />
                                    <asp:BoundField DataField="No of Idlings" HeaderText="No of Idles" HtmlEncode="false" />
                                    <asp:BoundField DataField="Total Idling Time" HeaderText="Total Idle Time" HtmlEncode="false" />
                                </Columns>
                                <HeaderStyle BackColor="#465AE8" Font-Bold="True" Font-Names="Verdana" Font-Size="10pt"
                                    ForeColor="White" />
                                <RowStyle HorizontalAlign="center" />
                                <AlternatingRowStyle BackColor="Lavender" HorizontalAlign="Center" />
                            </asp:GridView>
                            <br />
                            <asp:GridView ID="GridView3" runat="server" AllowPaging="True" Width="100%" PageSize="20"
                                HeaderStyle-Font-Size="12" HeaderStyle-ForeColor="#FFFFFF" HeaderStyle-BackColor="#465AE8"
                                HeaderStyle-Font-Bold="True" Font-Bold="False" Font-Overline="False" AutoGenerateColumns="False"
                                EnableViewState="False" HeaderStyle-Height="22px" HeaderStyle-HorizontalAlign="Center">
                                <PagerSettings PageButtonCount="5" />
                                <PagerStyle Font-Bold="True" Font-Names="Verdana" Font-Size="Small" HorizontalAlign="Center"
                                    VerticalAlign="Middle" BackColor="White" Font-Italic="False" Font-Overline="False"
                                    Font-Strikeout="False" />
                                <Columns>
                                    <asp:BoundField DataField="Stops" HeaderText="Stop" HtmlEncode="False" HeaderStyle-HorizontalAlign="Left" />
                                    <asp:BoundField DataField="Stop Summary" HeaderText=" " HtmlEncode="False" HeaderStyle-HorizontalAlign="Left" />
                                    <asp:BoundField DataField="Idle" HeaderText="Idle" HtmlEncode="False" HeaderStyle-HorizontalAlign="Left" />
                                    <asp:BoundField DataField="Idle Summary" HeaderText=" " HtmlEncode="False" HeaderStyle-HorizontalAlign="Left" />
                                    <asp:BoundField DataField="Travel" HeaderText="Travel" HtmlEncode="False" HeaderStyle-HorizontalAlign="Left" />
                                    <asp:BoundField DataField="Travel Summary" HeaderText=" " HtmlEncode="False" HeaderStyle-HorizontalAlign="Left" />
                                </Columns>
                                <HeaderStyle BackColor="#465AE8" Font-Bold="True" Font-Names="Verdana" Font-Size="10pt"
                                    ForeColor="White" Height="22px" HorizontalAlign="Center" />
                                <AlternatingRowStyle BackColor="#F4F4F4" />
                            </asp:GridView>
                            <br />
                            <asp:GridView ID="GridView1" runat="server" AllowPaging="True" Width="100%" PageSize="20"
                                AutoGenerateColumns="False" HeaderStyle-Font-Size="12" HeaderStyle-ForeColor="#FFFFFF"
                                HeaderStyle-BackColor="#465AE8" HeaderStyle-Font-Bold="True" Font-Bold="False"
                                Font-Overline="False" EnableViewState="False" HeaderStyle-Height="22px" HeaderStyle-HorizontalAlign="Center">
                                <PagerSettings PageButtonCount="5" />
                                <PagerStyle Font-Bold="True" Font-Names="Verdana" Font-Size="Small" HorizontalAlign="Center"
                                    VerticalAlign="Middle" BackColor="White" Font-Italic="False" Font-Overline="False"
                                    Font-Strikeout="False" />
                                <Columns>
                                    <asp:BoundField DataField="No" HeaderText="No" HtmlEncode="False">
                                        <ItemStyle Width="3px" HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Begin" HeaderText="Begin" HtmlEncode="False">
                                        <ItemStyle Width="80px" HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="End" HeaderText="End" HtmlEncode="False">
                                        <ItemStyle Width="80px" HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Address" HeaderText="Address" HtmlEncode="False">
                                        <ItemStyle Width="80px" HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Status" HeaderText="Status" HtmlEncode="False">
                                        <ItemStyle Width="80px" HorizontalAlign="Left" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Duration" HeaderText="Duration" HtmlEncode="False">
                                        <ItemStyle Width="80px" HorizontalAlign="Center" />
                                    </asp:BoundField>
                                    
                                </Columns>
                                <HeaderStyle BackColor="#465AE8" Font-Bold="True" Font-Names="Verdana" Font-Size="10pt"
                                    ForeColor="White" />
                                <AlternatingRowStyle BackColor="#F4F4F4" />
                            </asp:GridView>
                            <% If show = True Then%>
                            <label id="pages" style="font-family: Verdana; font-size: 11px; font-weight: bold;">
                                Pages</label>
                            <%End If%>
                            <br />
                            <br />
                            <br />
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        </form>
        <form id="excelform" name="excelform" method="get" action="ExcelReport.aspx">
        <input type="hidden" id="title" name="title" value="Vehicle_Usage_Report" />
        <input type="hidden" id="plateno" name="plateno" value="" />

          <input type="hidden" id="titl1" name="titl1" value="Vehicle Usage Report" />
    <input type="hidden" id="plno" name="plno" value="vehicle plate number" />
    <input type="hidden" id="rd" name="rd" value="Report Date" />
        </form>
    </center>
   
</body>
</html>
