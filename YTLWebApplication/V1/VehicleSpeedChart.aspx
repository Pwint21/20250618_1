<%@ Page Language="VB" AutoEventWireup="false"
    Inherits="YTLWebApplication.VehicleSpeedChart" Codebehind="VehicleSpeedChart.aspx.vb" %>

<%@ Register TagPrefix="chart" Namespace="ChartDirector" Assembly="netchartdir" %>
<%--<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">--%>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Vehicle Speed Chart</title>
    <style media="print" type="text/css">
        body
        {
            color: #000000;
            background: #ffffff;
            font-family: verdana,arial,sans-serif;
            font-size: 12pt;
        }
        #fcimg
        {
            display: none;
        }
    </style>
    <link rel="stylesheet" href="cssfiles/css3-buttons.css" type="text/css" media="screen" />
    <link type="text/css" href="cssfiles/jquery-ui.css" rel="stylesheet" />
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
        //var xvalue;
        //var ilon;
        //var ilat;
        //var y1 = 155;   // change the # on the left to adjuct the Y co-ordinate
        //(document.getElementById) ? dom = true : dom = false;

        //function hideIt() {
        //  if (dom) {document.getElementById("layer1").style.visibility='hidden';
        //  document.getElementById("mapdiv").style.visibility='hidden';}
        //  if (document.layers) {document.layers["layer1"].visibility='hide';
        //  document.layers["mapdiv"].visibility='hide';} }

        //function showIt(x) {

        //var rectarea=x;
        //var xyvalue=document.getElementById("xyvalue");
        //xyvalue.value =rectarea;
        //  if (dom) {document.getElementById("layer1").style.visibility='visible';
        //  document.getElementById("mapdiv").style.visibility='visible';}
        //  if (document.layers) {document.layers["layer1"].visibility='show';
        //  document.layers["mapdiv"].visibility='show';} 
        // mouseover();
        //  }

        //function placeIt() {
        //  if (dom && !document.all) {document.getElementById("layer1").style.top = window.pageYOffset + (window.innerHeight - (window.innerHeight-y1))}
        //  if (document.layers) {document.layers["layer1"].top = window.pageYOffset + (window.innerHeight - (window.innerHeight-y1))}
        //  if (document.all) {document.all["layer1"].style.top = document.body.scrollTop + (document.body.clientHeight - (document.body.clientHeight-y1));
        //  }
        //  window.setTimeout("placeIt()", 10); 
        //  
        //  }

        //onResize="window.location.href = window.location.href"

        //function addLoadEvent(func) {
        //  var oldonload = window.onload;
        //  if (typeof window.onload != 'function') {
        //    window.onload = func;
        //  } else {
        //    window.onload = function() {
        //      if (oldonload) {
        //        oldonload();
        //      }
        //      func();
        //    }
        //  }
        //}
        //addLoadEvent(function() {
        //  placeIt();
        // 
        //});

        //-->
        function mouseover() {

            var x;
            var y;
            var yvalue = document.getElementById("xyvalue").value;

            var stringvalue = document.getElementById("stringvalue").value;
            var xyvalues = stringvalue.split(";");

            xyvalues.reverse();

            for (i = 0; i < xyvalues.length; i++) {
                if (yvalue == i) {
                    var xy = xyvalues[i].split(",");
                    x = xy[0];
                    y = xy[1];
                    //alert(x + " " + y);
                }
            }


            document.getElementById("map").src = "images/maploading.gif";
            document.getElementById("map").src = "GussmannMap.aspx?x=" + x + "&y=" + y;
        }

        function click(type) {

            var x;
            var y;
            var yvalue = document.getElementById("xyvalue").value;

            var stringvalue = document.getElementById("stringvalue").value;
            var xyvalues = stringvalue.split(";");

            xyvalues.reverse();

            for (i = 0; i < xyvalues.length; i++) {
                if (yvalue == i) {
                    var xy = xyvalues[i].split(",");
                    x = xy[0];
                    y = xy[1];
                    //alert(x + " " + y);
                }
            }

            var googlemapsformobj = document.getElementById("googlemapsform");
            var googlemapsformobj1 = document.getElementById("googlemapsform1");
            //http://www.google.com/maps?q=http%3A%2F%2F202.71.100.82%2FAVLS4.0%2FShowVehicleIdlingInGoogle.aspx%3Fplateno%3D" + plateno + "%26bdt%3D" + bdt + "%26edt%3D" + edt
            if (type == 1) {

                var win = window.open("http://maps.google.com/maps?q=" + y + "+" + x, '_blank');
                if (win) {
                    //Browser has allowed it to be opened
                    win.focus();
                } else {
                    //Broswer has blocked it
                    alert('Please allow popups for this site');
                }

                //googlemapsformobj.action = "https://maps.google.com/maps?f=q&hl=en&q=" + y + " + " + x + "&om=1"
                //googlemapsformobj.submit();
            }
            if (type == 2) {
                googlemapsformobj1.action = "GoogleEarthMaps.aspx?x=" + x + "&y=" + y
                googlemapsformobj1.submit();
            }
            if (type == 3) {
                googlemapsformobj.action = "https://www.mapquest.com/maps/map.adp?searchtype=address&formtype=latlong&latlongtype=decimal&latitude=" + y + "&longitude=" + x + "&fname=Com"
                googlemapsformobj.submit();
            }
        }
    </script>
    <script type="text/javascript" language="javascript">
 var ec=<%=ec %>;
function download()
{
    if(ec==true)
    {
        var downloadformobj=document.getElementById("downloadform");
        downloadformobj.submit();
    }
    else
    {
        alert("First click submit button");
    }
}
function ExcelReport()
{
    if(ec==true)
    {
        var plateno=document.forms(0).ddlpleate.value;    
        var begindate=document.getElementById("txtBeginDate").value+" "+document.getElementById("ddlbh").value+":"+document.getElementById("ddlbm").value+":00";
        var enddate=document.getElementById("txtEndDate").value+" "+document.getElementById("ddleh").value+":"+document.getElementById("ddlem").value+":59";
        var driver=document.forms(0).ddldriver.value;
        var maxspeed=document.forms(0).ddlspeed.value;
      
      
        document.getElementById("plateno").value=plateno;
        document.getElementById("begindate").value=begindate;
        document.getElementById("enddate").value=enddate;
        document.getElementById("driver").value=driver;
        document.getElementById("maxspeed").value=maxspeed;
        
        var excelformobj=document.getElementById("excelform");
        excelformobj.submit();
    }
    else
    {
        alert("First click submit button");
    }
}
function mysubmit()
{
    var plateno=document.getElementById("ddlpleate").value;
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
    var days=parseInt(diff)+1;
    if(days>7)
    {
        return confirm("You selected "+days+" days of data.So it will take more time to execute.\nAre you sure you want to proceed ? ");
     
    }  
}
function loadlinechart()
{
alert('line');
}


function getWindowWidth(){if(window.self && self.innerWidth){return self.innerWidth;}if (document.documentElement && document.documentElement.clientWidth){return document.documentElement.clientWidth;}return document.documentElement.offsetWidth;}
function getWindowHeight(){if(window.self && self.innerHeight){return self.innerHeight;}if (document.documentElement && document.documentElement.clientHeight){return document.documentElement.clientHeight;}return document.documentElement.offsetHeight;}
  
    </script>
 </head>
<body style="
    font-family: Verdana; font-size: 5px;">
    <div id="layer1" style="visibility: hidden; position: absolute; top: 0px; right: 10px;">
        <div style="border: solid 1px silver; width: 120px; background-color: White; filter: progid:DXImageTransform.Microsoft.Shadow(color=gray,direction=135,Strength=5);">
            <a style="cursor: pointer;" href='javascript:click("1");'>
                <img style='border: solid 0 red;' alt="" src='images/googlemaps1.gif' title='View map in Google Maps' /></a>
            <a style="cursor: pointer;" href='javascript:click("2");'>
                <img style='border: solid 0 red;' alt="" src='images/googleearth1.gif' title='View map in GoogleEarth' /></a>
            <a style="cursor: pointer;" href='javascript:click("3");'>
                <img style='border: solid 0 red;' src='images/mapquestmaps.gif' title='View map in MAPQUEST Maps'
                    alt="" /></a>&nbsp;&nbsp;&nbsp; <a style="cursor: pointer;" href='javascript:hideIt();'>
                        <img style='border: solid 0 red; vertical-align: top; padding: 0px;' src='images/cross.gif'
                            alt="Close" title="Close" /></a>
        </div>
        <div style="height: 1px; width: 120px; padding: 0px;">
        </div>
        <div id="mapdiv" style="width: 258px; vertical-align: middle; padding: 5px; visibility: hidden;
            border: 1px solid black; font: normal 12px Verdana; line-height: 18px; z-index: 100;
            background-color: white; filter: progid:DXImageTransform.Microsoft.Shadow(color=gray,direction=135,Strength=5);">
            <img id="map" src="images/maploading.gif" alt="" style="border: 1px solid silver;
                width: 256px; height: 256px; vertical-align: middle;" />
        </div>
    </div>
    <form id="Form1" method="post" runat="server" style="font-family: Verdana; font-size: 11px;">
    <center>
        <div>
            <div>
                <br />
                <b style="font-family: Verdana; font-size: 20px; color: #38678B;">Vehicle Speed Chart
                </b>
                <br />
                <br />
            </div>
            <table>
                <tr>
                    <td align="center">
                        <table style="font-family: Verdana; font-size: 11px;">
                            <tr>
                                <td style="height: 20px; background-color: #465ae8;" align="left">
                                    <b style="color: White;">&nbsp;Vehicle Speed Chart &nbsp;:</b>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 420px; border: solid 1px #3952F9;">
                                    <table style="width: 420px;">
                                        <tbody>
                                            <tr>
                                                <td align="left">
                                                    <b style="color: #5f7afc; font-family: Verdana; font-size: 11px;">Begin Date</b>
                                                </td>
                                                <td>
                                                    <b style="color: #5f7afc; font-family: Verdana; font-size: 11px;">:</b>
                                                </td>
                                                <td align="left">
                                                    <input readonly="readonly" style="width: 70px;" type="text" value="<%=strBeginDate%>"
                                                        id="txtBeginDate" runat="server" name="txtBeginDate" enableviewstate="false" />&nbsp;
                                                    <b style="color: #5f7afc; font-family: Verdana; font-size: 11px;">Hour :&nbsp; </b>
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
                                                    </asp:DropDownList>
                                                    <b style="color: #5f7afc; font-family: Verdana; font-size: 11px;">&nbsp;Min&nbsp;: &nbsp;</b><asp:DropDownList
                                                        ID="ddlbm" runat="server" Width="42px" EnableViewState="False" Font-Size="12px"
                                                        Font-Names="verdana">
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
                                                    <b style="color: #5f7afc; font-family: Verdana; font-size: 11px;">End Date</b>
                                                </td>
                                                <td>
                                                    <b style="color: #5f7afc; font-family: Verdana; font-size: 11px;">:</b>
                                                </td>
                                                <td align="left">
                                                    <input style="width: 70px;" readonly="readonly" type="text" value="<%=strEndDate%>"
                                                        id="txtEndDate" runat="server" name="txtEndDate" enableviewstate="false" /><b style="color: #5f7afc;
                                                            font-family: Verdana; font-size: 11px;">&nbsp;&nbsp; Hour :&nbsp; </b>
                                                    <asp:DropDownList ID="ddleh" runat="server" Width="42px" EnableViewState="False"
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
                                                        <asp:ListItem Value="23" Selected="True">23</asp:ListItem>
                                                    </asp:DropDownList>
                                                    <b style="color: #5f7afc; font-family: Verdana; font-size: 11px;">&nbsp;Min : &nbsp;</b><asp:DropDownList
                                                        ID="ddlem" runat="server" Width="42px" Font-Size="12px" Font-Names="verdana"
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
                                                    <b style="color: #5f7afc; font-size: 11px; font-family: Verdana;">User Name</b>
                                                </td>
                                                <td>
                                                    <b style="color: #5f7afc; font-size: 11px; font-family: Verdana;">:</b>
                                                </td>
                                                <td align="left">
                                                    <asp:DropDownList ID="ddlUsername" runat="server" AutoPostBack="True" Font-Names="verdana"
                                                        Font-Size="12px" Width="256px">
                                                        <asp:ListItem>--Select User Name--</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <b style="color: #5f7afc; font-family: Verdana; font-size: 11px;">Plate No </b>
                                                </td>
                                                <td>
                                                    <b style="color: #5f7afc; font-family: Verdana; font-size: 11px;">:</b>
                                                </td>
                                                <td align="left">
                                                    <asp:DropDownList ID="ddlpleate" runat="server" Width="256px">
                                                        <asp:ListItem>--Select Plate No--</asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="left">
                                                    <b style="color: #5f7afc; font-family: Verdana; font-size: 11px;">Max Speed Limit</b>
                                                </td>
                                                <td>
                                                    <b style="color: #5f7afc; font-family: Verdana; font-size: 11px;">:</b>
                                                </td>
                                                <td align="left">
                                                    <asp:DropDownList ID="ddlspeed" runat="server" EnableViewState="False">
                                                        <asp:ListItem Value="10">10</asp:ListItem>
                                                        <asp:ListItem Value="20">20</asp:ListItem>
                                                        <asp:ListItem Value="30">30</asp:ListItem>
                                                        <asp:ListItem Value="40">40</asp:ListItem>
                                                        <asp:ListItem Value="50">50</asp:ListItem>
                                                        <asp:ListItem Value="60">60</asp:ListItem>
                                                        <asp:ListItem Value="70">70</asp:ListItem>
                                                        <asp:ListItem Value="80" Selected="True">80</asp:ListItem>
                                                        <asp:ListItem Value="90">90</asp:ListItem>
                                                        <asp:ListItem Value="100">100</asp:ListItem>
                                                        <asp:ListItem Value="110">110</asp:ListItem>
                                                        <asp:ListItem Value="120">120</asp:ListItem>
                                                        <asp:ListItem Value="130">130</asp:ListItem>
                                                        <asp:ListItem Value="140">140</asp:ListItem>
                                                        <asp:ListItem Value="150">150</asp:ListItem>
                                                        <asp:ListItem Value="160">160</asp:ListItem>
                                                        <asp:ListItem Value="170">170</asp:ListItem>
                                                        <asp:ListItem Value="180">180</asp:ListItem>
                                                    </asp:DropDownList>
                                                    <b style="color: #5f7afc; font-family: Verdana; font-size: 11px;">Km/h</b>&nbsp;
                                                    <b style="color: #5f7afc; font-family: Verdana; font-size: 11px;">Over Speed</b>
                                                    <asp:CheckBox ID="cbxoverspeed" runat="server" ToolTip="check to display only over speed list" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center">
                                                    <br />
                                                </td>
                                                <td colspan="2" align="left">
                                                    <br />
                                                    <asp:Button ID="ImageButton1" class="action blue" runat="server" Text="Submit" ToolTip="Submit" />
                                                    <a href="javascript:download();" class="button">Download </a>
                                                    <button class="action blue" title="Print" id="Print" runat="server" onclick="javascript:print();">
                                                        Print</button>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </td>
                            </tr>
                        </table>
                     <%--   <p style="font-family: Verdana; font-size: 11px; color: #5373a2;">
                            Copyright © 2013 Global Telematics Sdn Bhd. All rights reserved.</p>--%>
                    </td>
                </tr>
                <tr>
                    <td>
                        <br />
                    </td>
                </tr>
                <tr align="left">
                    <td align="center" colspan="2">
                        <chart:WebChartViewer ID="WebChartViewer1" runat="server" Visible="False" />
                        <asp:Image ID="Image1" runat="server" Visible="False" />
                    </td>
                </tr>
            </table>
        </div>
    </center>
    <input id="xyvalue" type="hidden" runat="server" value="" />
    <input id="stringvalue" type="hidden" runat="server" value="" />
    </form>
    <form id="downloadform" name="downloadform" method="get" action="DownloadChart.aspx">
    <input type="hidden" id="title" name="title" value="Vehicle Speed Chart" />
    </form>
    <form id="googlemapsform" method="post" action="" target="_blank">
    </form>
    <form id="googlemapsform1" method="post" action="">
    </form>
    <form id="excelform" name="excelform" method="get" action="VehicleSpeedExcelReport.aspx">
    <%-- <input type="hidden" id="title" name="title" value="" />--%>
    <input type="hidden" id="plateno" name="plateno" value="" />
    <input type="hidden" id="begindate" name="begindate" value="" />
    <input type="hidden" id="enddate" name="enddate" value="" />
    <input type="hidden" id="driver" name="driver" value="" />
    <input type="hidden" id="maxspeed" name="maxspeed" value="" />
    </form>
</body>
</html>
