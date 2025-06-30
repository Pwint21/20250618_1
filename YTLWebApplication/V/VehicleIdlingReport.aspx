<%@ Page Language="VB" AutoEventWireup="false"
    Inherits="YTLWebApplication.VehicleIdlingReporttemp" Codebehind="VehicleIdlingReport.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Vehicle Idling Report</title>
     <style media="print" type="text/css">
body {color : #000000;background : #ffffff;font-family : verdana,arial,sans-serif;font-size : 12pt;}
#fcimg
{display : none;}

</style>
      <link rel="stylesheet" href="cssfiles/css3-buttons.css" type="text/css" media="screen"/>
      <link type="text/css"  href="cssfiles/jquery-ui.css" rel="stylesheet" />
      <link type="text/css" href="cssfiles/balloontip.css" rel="stylesheet" />
      <script type="text/javascript" src="jsfiles/jquery.min.js"></script>
      <script type="text/javascript" src="jsfiles/jquery-ui.min.js"></script>
      <script type="text/javascript" src="jsfiles/balloontip.js"></script>
          
          
    <script type="text/javascript" language="javascript">
       $(function () {
          $("#txtBeginDate").datepicker({ dateFormat: 'yy/mm/dd', minDate: -180, maxDate: +0, changeMonth: true, changeYear: true, numberOfMonths: 2
          });

          $("#txtEndDate").datepicker({ dateFormat: 'yy/mm/dd', minDate: -180, maxDate: +0, changeMonth: true, changeYear: true, numberOfMonths: 2

          });
      });
   </script>

    <script type="text/javascript" language="javascript">

  
			function SelectedIndexChanged(ctlId)
			{
   				var control = document.getElementById(ctlId+'DDList'); 
				var strSelText='';
				for(var i = 0; i < control.length; i ++)
				{ 
					if(control.options[i].selected)
					{
					if (control.options[i].text!="--Select Minutes--")
						strSelText +=control.options[i].value + ',';
					}
				}
				if (strSelText.length>0)
					strSelText=strSelText.substring(0,strSelText.length-1);
				var ddLabel = document.getElementById(ctlId+"DDLabel"); 

                if (strSelText=="")
                ddLabel.value="--Select Minutes--";
                else
                {
                ddLabel.value=strSelText;                                         
                }
			}

			function OpenListBox(ctlId)
			{
				var lstBox = document.getElementById(ctlId+"DDList");
				
				if (lstBox.style.visibility == "visible")				
				{ 
				  CloseListBox(ctlId) ;
				 }
				else
				{
					lstBox.style.visibility = "visible"; 				
					lstBox.style.height="200px";
				}
			}

			function CloseListBox(ctlId)
			{							
			   
				var panel = document.getElementById(ctlId+"Panel2");
				var tabl = document.getElementById(ctlId+"Table2");
				var lstBox = document.getElementById(ctlId+"DDList");
				lstBox.style.visibility = "hidden"; 
				lstBox.style.height="0px";
				panel.style.height=tabl.style.height;
				var ddLabel = document.getElementById(ctlId+"DDLabel"); 
                if (ddLabel.value=="")
                ddLabel.value="--Select Minutes--";
				
			}

    
var ec=<%=ec %>;
function mysubmit()
{
     var plateno=document.getElementById("ddlpleate").value;
    if (plateno=="--Select User Name--")
    {
        alert("Please select user name and vehicle plate number");
        return false;
     }   
    if (plateno=="--Select Plate No--")
    {
         alert("Please select vehicle plate number");
         return false;         
    }
    var bigindatetime=document.getElementById("txtBeginDate").value+" "+document.getElementById("ddlbh").value+":"+document.getElementById("ddlbm").value;
    var enddatetime=document.getElementById("txtEndDate").value+" "+document.getElementById("ddleh").value+":"+document.getElementById("ddlem").value;
    
    var fdate=Date.parse(bigindatetime);
    var sdate=Date.parse(enddatetime);
    
    var diff=(sdate-fdate)*(1/(1000*60*60*24));
    var days=parseInt(diff);
    if(days>31)
    {
        alert("Sorry, you've selected "+days+" days.\nMaximum 31 days of report can only be shown.");
         return false; 
    }
    else if(days<0){
        alert("Sorry, you've selected wrong begin and end date, please select again");
         return false; 
    }
    return true;
     
}
function ExcelReport()
{
    if(ec==true)
    {
        var plateno=document.getElementById("ddlpleate").value;
       
        document.getElementById("plateno").value=plateno;

        var excelformobj=document.getElementById("excelform");
        excelformobj.submit();
    }
    else
    {
        alert("First click submit button");
    }
}


	    

function ShowGoogleMaps()
{
    var plateno=document.forms(0).ddlpleate.value;
    if (plateno=="--Select Plate No--")
    {
         alert("Please select vehicle plate number");
         return false;         
    }
    else
    {
        var bdt = document.getElementById("txtBeginDate").value + "%2520" + document.getElementById("ddlbh").value + ":" + document.getElementById("ddlbm").value + ":00"
        var edt = document.getElementById("txtEndDate").value + "%2520" + document.getElementById("ddleh").value + ":" + document.getElementById("ddlem").value + ":59"

        var googlemapsformobj=document.getElementById("googlemapsform");        
        googlemapsformobj.action="https://www.google.com/maps?q=http%3A%2F%2Flafargev2.avls.com.my%2FShowVehicleIdlingInGoogle.aspx%3Fplateno%3D" + plateno + "%26bdt%3D" + bdt + "%26edt%3D" + edt 
        googlemapsformobj.submit();     
    }
      
}
function ShowGoogleEarth()
{
    var plateno=document.forms(0).ddlpleate.value;
    if (plateno=="--Select Plate No--")
    {
         alert("Please select vehicle plate number");
         return false;         
    }
    else
    {
        var bdt = document.getElementById("txtBeginDate").value + " " + document.getElementById("ddlbh").value + ":" + document.getElementById("ddlbm").value + ":00"
        var edt = document.getElementById("txtEndDate").value + " " + document.getElementById("ddleh").value + ":" + document.getElementById("ddlem").value + ":59"

        var googleearthformobj=document.getElementById("googleearthform");
        googleearthformobj.action="ShowVehicleIdlingInGoogle.aspx?plateno=" + plateno + "&bdt=" + bdt + "&edt=" + edt
        googleearthformobj.submit();     
    }
      
}
function mouseover(x,y)
{
    document.getElementById("mapimage").src="images/maploading.gif";
    document.getElementById("mapimage").src="GussmannMap.aspx?x="+x+"&y="+y;
}
function googlemouseover(x,y)
{
    document.getElementById("mapimage").src="images/maploading.gif";
    document.getElementById("mapimage").src="https://mt0.google.com/mt?x="+x+"&y="+y+"&zoom=10";
    alert(document.getElementById("mapimage").src);
}
 function cancel()
    {  
    
     var formobj=document.getElementById("Form1");
     formobj.reset();
    }
    </script>

    <style type="text/css">
        .style1
        {
            height: 20px;
            width: 468px;
        }
        .style2
        {
            width: 468px;
        }
    </style>

</head>
<body style="margin-left: 5px; margin-top: 0px; margin-bottom: 0px; margin-right: 5px;">
    <form id="Form1" method="post" runat="server">
        

       

        <center>
            <div>
                <br />
                 <b style="font-family: Verdana; font-size: 20px; color: #38678B;">Vehicle Idling Report</b>
                <br />
                <br />
               <table width="90%">
                    <tr>
                        <td align="center">
                            <table style="font-family: Verdana; font-size: 11px;">
                                <tr>
                                    <td style="background-color: #465ae8;" align="left" class="style1">
                                        <b style="color: White;">&nbsp;Vehicle Idling Report &nbsp;:</b></td>
                                </tr>
                                <tr>
                                    <td style="border: solid 1px #3952F9;" class="style2">
                                        <table style="width: 466px;">
                                            <tbody>
                                                <tr>
                                                    <td align="left">
                                                        <b style="color: #5f7afc;">Begin Date</b>
                                                    </td>
                                                    <td>
                                                        <b style="color: #5f7afc;">:</b>
                                                    </td>
                                                    <td align="left">
                                                        <input readonly="readonly" style="width: 70px;" type="text" value="<%=strBeginDate%>"
                                                            id="txtBeginDate" runat="server" name="txtBeginDate" enableviewstate="false" /><b style="color: #5f7afc;">&nbsp;Hour&nbsp;:&nbsp;</b>
                                                        <asp:DropDownList ID="ddlbh" runat="server" Width="42px" EnableViewState="False"
                                                            Font-Size="12px" Font-Names="verdana">
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
                                                        </asp:DropDownList><b style="color: #5f7afc;">&nbsp;Min&nbsp;:&nbsp;</b>
                                                        <asp:DropDownList ID="ddlbm" runat="server" Width="42px" EnableViewState="False"
                                                            Font-Size="12px" Font-Names="verdana">
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
                                                        <b style="color: #5f7afc;">End Date</b>
                                                    </td>
                                                    <td>
                                                        <b style="color: #5f7afc;">:</b>
                                                    </td>
                                                    <td align="left">
                                                        <input style="width: 70px;" readonly="readonly" type="text" value="<%=strEndDate%>"
                                                            id="txtEndDate" runat="server" name="txtEndDate" enableviewstate="false" /><b style="color: #5f7afc;">&nbsp;Hour&nbsp;:&nbsp;
                                                                <asp:DropDownList ID="ddleh" runat="server" Width="42px" 
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
                                                                </asp:DropDownList>&nbsp;Min&nbsp;:&nbsp;
                                                                    <asp:DropDownList ID="ddlem" runat="server" Width="42px" 
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
                                                                    </asp:DropDownList></b>
                                                    </td>
                                                </tr>
                                              <tr>
                                                    <td align="left">
                                                        <b style="color: #5f7afc">User Name</b></td>
                                                    <td>
                                                        <b style="color: #5f7afc">:</b></td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="ddlUsername" runat="server" AutoPostBack="True" Font-Names="verdana"
                                                            Font-Size="12px" Width="251px">
                                                            <asp:ListItem>--Select User Name--</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left">
                                                        <b style="color: #5f7afc;">Plate No </b>
                                                    </td>
                                                    <td>
                                                        <b style="color: #5f7afc;">:</b></td>
                                                    <td align="left">
                                                        <asp:DropDownList ID="ddlpleate" runat="server" Width="251px" Font-Size="12px" 
                                                            Font-Names="verdana">
                                                            <asp:ListItem>--Select Plate No--</asp:ListItem>
                                                            <asp:ListItem>--Select All Plate No--</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                 <tr>
                                                    <td align="left" style="height: 22px">
                                                        <b style="color: #5f7afc;">Idle minutes</b>
                                                    </td>
                                                     <td style="height: 21px">
                                                        <b style="color: #5f7afc;">:</b></td>
                                                    <td align="left" valign="top" style="height: 21px">
                                                        <asp:DropDownList ID="ddlminutes" runat="server" Width="200px" Font-Size="12px" Font-Names="verdana"
                                                            EnableViewState="False">
                                                            <asp:ListItem Value="1" Selected="True">--All Minutes--</asp:ListItem>
                                                            <asp:ListItem Value="1">&gt; 1 Minutes</asp:ListItem>
                                                            <asp:ListItem Value="3">&gt; 3 Minutes</asp:ListItem>
                                                            <asp:ListItem Value="5">&gt; 5 Minutes</asp:ListItem>
                                                            <asp:ListItem Value="10">&gt; 10 Minutes</asp:ListItem>
                                                            <asp:ListItem Value="15">&gt; 15 Minutes</asp:ListItem>
                                                            <asp:ListItem Value="20">&gt; 20 Minutes</asp:ListItem>
                                                            <asp:ListItem Value="25">&gt; 25 Minutes</asp:ListItem>
                                                            <asp:ListItem Value="30">&gt; 30 Minutes</asp:ListItem>
                                                            <asp:ListItem Value="40">&gt; 40 Minutes</asp:ListItem>
                                                            <asp:ListItem Value="45">&gt; 45 Minutes</asp:ListItem>
                                                            <asp:ListItem Value="50">&gt; 50 Minutes</asp:ListItem>
                                                            <asp:ListItem Value="60">&gt; 60 Minutes</asp:ListItem>
                                                            <asp:ListItem Value="90">&gt; 90 Minutes</asp:ListItem>
                                                            <asp:ListItem Value="120">&gt; 120 Minutes</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="left" style="height: 22px">
                                                        <b style="color: #5f7afc;">Records/Page</b>
                                                    </td>
                                                    <td style="height: 22px">
                                                        <b style="color: #5f7afc;">:</b></td>
                                                    <td align="left" style="height: 22px"  >
                                                        <asp:DropDownList ID="noofrecords" runat="server" Width="70px" Font-Size="12px" Font-Names="verdana"
                                                            EnableViewState="False"  >
                                                            <asp:ListItem Selected="True">100</asp:ListItem>
                                                            <asp:ListItem>500</asp:ListItem>
                                                            <asp:ListItem>1000</asp:ListItem>
                                                        </asp:DropDownList></td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                    <br />
                                                     
                                                            </td>

                                                    <td colspan="2" align="left" >
                                                     
                                                     
                                                        <br />
                                                         <asp:Button ID="ImageButton1" class="action blue" runat="server" Text="Submit"  ToolTip="Submit" />
                                                           
                                                     <a href="javascript:ExcelReport();" class="button" style="vertical-align:top;"><span class="ui-button-text ">Save Excel</span>  </a>
                                                       
                                                        <a href="javascript:print();" class="button" style="vertical-align:top; width:55px;"><span class="ui-button-text"
                                                        title="Print">Print</a>
                                                             <img src="images/GoogleMaps.gif" alt="Show Log Report in Google Maps" title="Show Log Report in Google Maps" style="cursor: pointer;" onclick="return ShowGoogleMaps();" />&nbsp;&nbsp;                                                            
                                                       <img src="images/GoogleEarth.gif" alt="Show Log Report in Google Earth" title="Show Log Report in Google Earth" style="cursor: pointer;" onclick="return ShowGoogleEarth();" />
                                                            
                                               
                                                       
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                           
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <br />
                        </td>
                    </tr>
                    <tr align="left">
                        <td>
                            <table width="100%">
                                <tr>
                                    <td align="center" colspan="2">
                                        <div style="font-family: Verdana; font-size: 11px;">
                                            <br />
                                            <asp:GridView ID="GridView2" runat="server" AutoGenerateColumns="true" Width="99%"
                                                HeaderStyle-Font-Size="12px" HeaderStyle-ForeColor="#FFFFFF" HeaderStyle-BackColor="#465AE8"
                                                HeaderStyle-Font-Bold="True" Font-Bold="False" Font-Overline="False" EnableViewState="False"
                                                HeaderStyle-Height="22px" HeaderStyle-HorizontalAlign="Center" BorderColor="#F0F0F0">
                                                <AlternatingRowStyle BackColor="Lavender" />
                                            </asp:GridView>
                                        </div>
                                        <br />
                                     
                                        <div style="font-family: Verdana; font-size: 11px;">
                                          
                                            <asp:GridView ID="GridView1" runat="server" AllowPaging="True" Width="90%" PageSize="20"
                                                AutoGenerateColumns="False" HeaderStyle-Font-Size="12px" HeaderStyle-ForeColor="#FFFFFF"
                                                HeaderStyle-BackColor="#465AE8" HeaderStyle-Font-Bold="True" Font-Bold="False"
                                                Font-Overline="False" EnableViewState="False" HeaderStyle-Height="22px" 
                                                HeaderStyle-HorizontalAlign="Center" BorderColor="#F0F0F0" BackColor="#99CCFF">
                                                <PagerSettings PageButtonCount="5" />
                                                <PagerStyle Font-Bold="True" Font-Names="Verdana" Font-Size="Small" HorizontalAlign="Center"
                                                    VerticalAlign="Middle" BackColor="White" Font-Italic="False" Font-Overline="False"
                                                    Font-Strikeout="False" />
                                                <Columns>
                                                    <asp:BoundField DataField="S No" HeaderText="No">
                                                        <ItemStyle Width="5px" HorizontalAlign="center" />
                                                    </asp:BoundField>
                                                     <asp:BoundField DataField="Plateno" HeaderText="Plate No">
                                                        <ItemStyle  HorizontalAlign="Left"  />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Begin Date Time" HeaderText="Begin" HtmlEncode="False">
                                                        <ItemStyle Width="135px" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="End Date Time" HeaderText="End" HtmlEncode="False">
                                                        <ItemStyle Width="135px" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="Duration" HeaderText="Duration" />
                                                    <asp:BoundField DataField="Address" HeaderText="Address" HtmlEncode="False" />
                                                    <asp:BoundField DataField="Nearest Town" HeaderText="Nearest Town" HtmlEncode="False">                                                   
                                                    <ItemStyle Width="150px" />
                                                    </asp:BoundField>
                                                     <asp:BoundField DataField="Lat" HeaderText="Lat" />
                                                      <asp:BoundField DataField="Lon" HeaderText="Lon" />
                                                    <asp:BoundField DataField="Distance Travelled before next idling (Kms)" HeaderText="Distance Travelled before next idling (Kms)" />
                                                    <asp:BoundField DataField="Time spent in last travel (Mins)" HeaderText="Time spent in last travel (Mins)" />
                                                    <asp:BoundField DataField="Maps" HeaderText="Maps" HtmlEncode="False">
                                                        <ItemStyle Width="100px" />
                                                    </asp:BoundField>
                                                </Columns>
                                                <AlternatingRowStyle BackColor="Lavender" />
                                            </asp:GridView>
                                            <% If show = True Then%>
                                            <center><label id="pages" style="font-family: Verdana; font-size: 11px; font-weight: bold;">
                                                Pages</label></center>
                                            <%End If%>
                                            <br />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
        </center>
        <table>
            <tr>
                <td>
                    <div id="balloon1" class="balloonstyle" style="width: 258px; vertical-align: middle;">
                        <img id="mapimage" src="images/maploading.gif" alt="" style="border: 1px solid silver;
                            width: 256px; height: 256px; vertical-align: middle;" />
                    </div>
                </td>
            </tr>
        </table>
    </form>
    <form id="excelform" method="get" action="idlingExcelReport.aspx">
        <input type="hidden" id="title" name="title" value="Vehicle Idling Report" />
        <input type="hidden" id="plateno" name="plateno" value="" />
    </form>
   
    <form id="googlemapsform" method="post" action="" target="_blank">
    </form>
    <form id="googleearthform" method="post" action="">
    </form>
</body>
</html>
