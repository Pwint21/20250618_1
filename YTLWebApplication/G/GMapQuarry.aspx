<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.GMapQuarry" Codebehind="GMapQuarry.aspx.vb" %>

<html>
<head>
    <title>Gussmann Maps</title>
    <link type="text/css" href="cssfiles/jquery-ui.css" rel="stylesheet" />
    <script type="text/javascript" src="jsfiles/jquery.min.js"></script>
    <script type="text/javascript" src="jsfiles/jquery-ui.min.js"></script>
    <link href="GMap/chosen/chosen.css" rel="stylesheet" type="text/css" />
    <script src="GMap/chosen/chosen.jquery.js" type="text/javascript"></script>
    <link href="cssfiles/css3-buttons.css" rel="stylesheet" type="text/css" />
    <link href="cssfiles/style.css" rel="stylesheet" type="text/css" />
    <link href="cssfiles/demos22.css" rel="stylesheet" type="text/css" />
    <link href="cssfiles/jquery.ui.dialog.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.effects.core.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.effects.slide.js"></script>
    <link href="GMap/default.css" rel="stylesheet" type="text/css" />
     <script type="text/javascript" src="https://maps.googleapis.com/maps/api/js?v=3.28&client=gme-zigbeeautomation&libraries=geometry,drawing&channel=YTL"></script>     <link href="GMap/gdropdown.css" rel="stylesheet" type="text/css" />
    <script src="GMap/gdropdown.js" type="text/javascript"></script>
        <script src="GMap/maplabel-compiled.js"></script>
    <style type="text/css">
        div.side-by-side
        {
            width: 100%;
            margin-bottom: 1em;
        }
        div.side-by-side > div
        {
            float: left;
            width: 50%;
        }
        div.side-by-side > div > em
        {
            margin-bottom: 10px;
            display: block;
        }
        .clearfix:after
        {
            content: "\0020";
            display: block;
            height: 0;
            clear: both;
            overflow: hidden;
            visibility: hidden;
        }
    </style>
    <style type="text/css">
 v\:* {behavior:url(#default#VML);}
 #srchu{position: absolute; top: 0px; right: 0px; ; width: 300px;height:30px; background: #fc0; display:none;}
.search{ position: absolute; top: 6px; 
right: 8px;
visibility:visible ;
            height: 28px;
        }
  .hideshowdiv{ position:absolute;
                       top: 0px;                     
                       right:500px;
 visibility:visible ;        
        }
         
.searchin {border: 1px solid #333;right: 140px;
            padding-left:2px; font-size: 11px; width: 128px; 
height:18px;
            margin-top: 3px;margin-right: 139px; background: #fff;
        }
    
.searchbutton {width: 60px;}
.mpo {cursor: pointer;}
.button {font-weight: bold;font-size: 11px;text-transform:lowercase;color: #FFFFFF;background: #d71a1a;border-width: 1px;border-style: solid;border-top-color: #f05555;border-left-color: #f05555;border-right-color: #7c0808;border-bottom-color: #7c0808;}
    </style>
    <style type="text/css" media="print">
        .no-print
        {
            display: none;
        }
    </style>
    <style type="text/css">
        
        input.pravinstyle
        {
            border: 1px solid #c4c4c4;
            width: 157px;           
            font-size: 13px;
            padding: 4px 4px 4px 4px;
           
            box-shadow: 0px 0px 8px #d9d9d9;
            -moz-box-shadow: 0px 0px 8px #d9d9d9;
            -webkit-box-shadow: 0px 0px 8px #d9d9d9;
        }
        
        input.pravinstyle:focus
        {
            outline: none;
            border: 1px solid #7bc1f7;
            box-shadow: 0px 0px 8px #7bc1f7;
            -moz-box-shadow: 0px 0px 8px #7bc1f7;
            -webkit-box-shadow: 0px 0px 8px #7bc1f7;
        }
   .searchform {
	display: inline-block;
	zoom: 1; /* ie7 hack for display:inline-block */
	*display: inline;
	border: solid 1px #d2d2d2;
	padding: 3px 3px;
	
	
	background: #f1f1f1;
	background: -webkit-gradient(linear, left top, left bottom, from(#fff), to(#ededed));
	background: #f1f1f1;
	 width: auto;
  }
.searchform input {
	font: normal 13px/100%  Arial;
}
.searchform .searchfield {
	background: #fff;
	padding: 6px 6px 6px 8px;
	width: 300px;
	border: solid 1px #bcbbbb;
	outline: none;

	
	-moz-box-shadow: inset 0 1px 2px rgba(0,0,0,.2);
	-webkit-box-shadow: inset 0 1px 2px rgba(0,0,0,.2);
	box-shadow: inset 0 1px 2px rgba(0,0,0,.2);
}
.searchform .searchbutton {
	color: #fff;
	border: solid 1px #494949;
	font-size: 11px;
	height: 28px;
	width: 27px;
	text-shadow: 0 1px 1px rgba(0,0,0,.6);



	background: #5f5f5f;
	background: -webkit-gradient(linear, left top, left bottom, from(#9e9e9e), to(#454545));
	background: -moz-linear-gradient(top,  #9e9e9e,  #454545);

	-ms-filter:  progid:DXImageTransform.Microsoft.gradient(startColorstr='#9e9e9e', endColorstr='#454545'); /* ie8 */
}
    .ui-autocomplete
        {
            max-height: 150px;
            max-width: 275px;
            overflow-y: auto;
            outline-color: Aqua; /* add padding to account for vertical scrollbar */
            padding-right: 15px;
        }
        /* IE 6 doesn't support max-height
	 * we use height instead, but this forces the menu to always be this tall
	 */
        * html .ui-autocomplete
        { 
          height:100px;
        }
     .imgloading
        {
            background: white url('images/loading.gif') right center no-repeat;
        }
		.tooltip{
			border: 1px solid #777;
			background-color: #FFFF77;
			padding:2px 5px 2px 5px;
			font-size: 12px;
		}        
        ​</style>

    <style>
            .hideen {
                visibility: hidden;
                /* margin-top:-80px;*/
            }

            .opacity {
                /* IE 8 */
                -ms-filter: "progid:DXImageTransform.Microsoft.Alpha(Opacity=30)";
                /* IE 5-7 */
                filter: alpha(opacity=30);
                /* Netscape */
                -moz-opacity: 0.3;
                /* Safari 1.x */
                -khtml-opacity: 0.3;
                opacity: 0.3;
                pointer-events: none;
            }

            #floatingBarsG {
                position: fixed;
                top: 50%;
                left: 50%;
                /*width: 100%;
            height: 100%;
         
            left: 25%;*/
            }
        </style>
    <%--     Script Files for Map Marker --%>
    <script type="text/javascript">
        function getWindowWidth() { if (window.self && self.innerWidth) { return self.innerWidth; } if (document.documentElement && document.documentElement.clientWidth) { return document.documentElement.clientWidth; } return document.documentElement.offsetWidth; }
        function getWindowHeight() { if (window.self && self.innerHeight) { return self.innerHeight; } if (document.documentElement && document.documentElement.clientHeight) { return document.documentElement.clientHeight; } return document.documentElement.offsetHeight; }
        function resize() {
            var map = document.getElementById("map");
            map.style.height = getWindowHeight() + "px";
            map.style.width = getWindowWidth() + "px";
            document.getElementById("uha").style.right = parseInt(map.style.width) / 3;
            document.getElementById("search1").style.right = "8px";
        }

        
    </script>
    <script type="text/javascript">
        var isOpen = true;

        jQuery(document).ready(function () {
            jQuery('#toggle-caption').click(function () {
                if (isOpen) {
                    $('#caption').slideRightShow();
                    isOpen = false;
                } else {
                    $('#caption').slideRightHide();
                    isOpen = true;
                }

            });

        });
        

       
    </script>
    <style type="text/css">
        #over_map
        {
            z-index: 99;
            background: #EEE;
            -webkit-border-radius: 10px;
            border-radius: 10px;
            display: block;
            border: 1px solid #AAA;
            border-right: 1px solid #AAA;
            box-shadow: 2px 2px 3px #888;
            border-bottom: 1px solid #888;
            border-right: 1px solid #888;
            border-left: 1px solid #888;
        }
        #uha
        {
            margin: 0 auto;
            padding: 0;
            width: 350px;
        }
        a:focus
        {
            outline: none;
        }
        #panel
        {
            display: none;
            font: 120%/100% Arial, Helvetica, sans-serif;
        }
        
        .btn-slide
        {
            text-align: center;
            width: 50px;
            height: 20px;
            padding: 0px 10px 0 0;
            margin: 0px 130px;
            display: block;
            font: bold 120%/100% Arial, Helvetica, sans-serif;
            color: #fff;
            text-decoration: none;
        }
        .active
        {
            background-position: right 12px;
        }
        .vlabel
        {
            background: #FFFF00;
            padding: 2px;
            border: solid 1px black;
            font-family: Verdana;
            font-size: 11px;
        }

        .gm-style-mtc > button {
            height: fit-content;
            width: fit-content;
        }

        .loader-container {
            width: 100%;
            height: 100%;
            position: absolute;
            display: none;
            justify-content: center;
            z-index: 1;
        }

        .loader {
            border: 10px solid #f3f3f3; /* Light grey */
            border-top: 10px solid #3498db; /* Blue */
            border-radius: 50%;
            width: 50px;
            height: 50px;
            animation: spin 2s linear infinite;
            top: 15px;
            position: absolute;
        }

        @keyframes spin {
            0% {
                transform: rotate(0deg);
            }

            100% {
                transform: rotate(360deg);
            }
        }
    </style>
    <link href="GMap/VehicleMarker.css" rel="stylesheet" type="text/css" />
    <!--[if lt IE 9]>
    <style type="text/css">
    .chat-bubble-arrow-border {
    bottom:-36px;
    }
    .chat-bubble-arrow {
    bottom:-34px;
    }   
    .b-arrow{
    bottom:-30px;
    }
     .r-arrow-border {
    bottom:-31px;
    }
    .r-arrow{
    bottom:-30px;
    }
    .g-arrow{
   bottom:-30px;
    }
    </style>    
<![endif]-->
    <script src="GMap/richmarker.js" type="text/javascript"></script>
    <script src="GMap/markerclusterer.js" type="text/javascript"></script>
   
    <script src="GMap/GMapQuarry.js?r=<%= Now.ToString("yyyy/MM/dd HH:mm:ss") %>" type="text/javascript"></script>
</head>
<body onresize="resize()" onload="callit()" style="margin: 0px; padding: 0px; overflow: hidden;">
    <form id="form1" runat="server">
        <div class="loader-container">
            <div class="loader"></div>
        </div>
        <div id="floatingBarsG" class="hideen" style="z-index: 999; opacity: 2;">
            <center>
            <div class="row">
            
            <div class ="col-md-12">
                  <div class="box box-primary">
                <div class="box-header">
                  <h3 class="box-title">Loading</h3>
                </div>
               <!-- /.box-body -->
                <!-- Loading (remove the following to stop the loading)-->
               <%-- <div class="overlay">
                  <i class="fa fa-refresh fa-spin"></i>
                </div>--%>
                <!-- end loading -->
              </div><!-- /.box -->
        
            </div>
            
                   </div>
                </center>
        </div>
    <div id="map" style="width: 700px; height: 500px; border: 0px solid silver; cursor: pointer">
        <div style="position: absolute; left: 45%; top: 45%; font-family: Verdana; font-size: 12px;
            font-weight: bold; color: #d71a1a;">
            <img alt="Loading Map..." title="Loading Map..." src="images/loading.gif" />&nbsp;Loading
            Map...
        </div>
    </div>
    <div id="uha" class="hideshowdiv no-print " style="font-size:12px">
        <div id="panel">
            <table id="over_map" style="text-align: right; margin: 0px; padding-bottom: 1px;
                padding-right: 0px;">
                <tr>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <b style="color: #5f7afc;">Begin Date : </b>
                    </td>
                    <td align="left">
                        <input type="text" id="begindate1" runat="server" style="width: 75px;" readonly="readonly"
                            enableviewstate="false" />
                    </td>
                    <td align="left">
                        <b style="color: #5f7afc;">HH: </b>&nbsp;
                    </td>
                    <td align="left">
                        <asp:DropDownList ID="ddlbh" runat="server" Width="44px" EnableViewState="False">
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
                        &nbsp;
                    </td>
                    <td align="left">
                        <b style="color: #5f7afc;">MM: </b>&nbsp;
                    </td>
                    <td align="left">
                        <asp:DropDownList ID="ddlbm" runat="server" EnableViewState="False">
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
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <b style="color: #5f7afc;">End Date : </b>
                    </td>
                    <td align="left">
                        <input type="text" id="enddate1" runat="server" style="width: 75px;" readonly="readonly"
                            enableviewstate="false" />
                    </td>
                    <td align="left">
                        <b style="color: #5f7afc;">HH: </b>&nbsp;
                    </td>
                    <td align="left">
                        <asp:DropDownList ID="ddleh" runat="server" Width="44px" EnableViewState="False">
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
                        &nbsp;
                    </td>
                    <td align="left">
                        <b style="color: #5f7afc;">MM: </b>&nbsp;
                    </td>
                    <td align="left">
                        <asp:DropDownList ID="ddlem" runat="server" EnableViewState="False">
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
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <b style="color: #5f7afc;">Plate No : </b>
                    </td>
                    <td align="left" colspan="5">
                        <%=sb.ToString()%>
                    </td>
                </tr>
                <tr>
                  
                    <td align="left" colspan="6" style="padding-left: 20px;">
                        <button class="action blue" style="background-color: #4A8CF8;" title="Show Vehicle Current Location"
                            id="Button1" onclick="return gotoloc();">
                            Current</button>
                        <button class="action blue" style="background-color: #4A8CF8;" title="Show Vehicle History"
                            id="Save" onclick="return ShowVehiclePath(0);">
                            History</button>
                        <button class="action blue" style="background-color: #4A8CF8;" title="Show All Vehicles"
                            id="Button2" onclick="return ShowVehiclePath(1);">
                            ShowAll</button>
                            <button class="action blue" style="background-color: #4A8CF8;" title="Simulate Selected vehicle"
                            id="Button3" onclick="return ShowVehiclePath(3);">
                            Simulate</button>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                    </td>
                    <td>
                    </td>
                    <td>
                    </td>
                </tr>
            </table>
        </div>
        <a href="#" class="btn-slide slide">
            <img id="udimg" src="images/down.gif" alt="Open vehicle search" title="Open vehicle search"
                style="visibility: hidden; border: 0px none FFFFFF" /></a>
    </div>
    <div id='search1' class='search nan1 no-print ' style="visibility: hidden;">
        <div class='searchform'>
            <table border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td> <input class="searchfield pravinstyle" style="height:28px;"  id='searchtest' name='searchtest' type="text"
                value='Search here..' title='Enter the text you want to search' onfocus="WaterMark(this, event);"
                onblur="WaterMark(this, event);" />
                    </td>
                     <td>
                      <input id="gotoposbtn" class="searchbutton" type="button" value="Go" title="Go to the location [Lat,Lon]"
                style="margin-left: 3px;color: Yellow; background-color: #5F5F5F; cursor: pointer;" onclick="javascript:gotopos()" />
                    </td>
                </tr>
            </table>      
        </div>
    </div>
    <div class="demo">
        <div id="dialog-confirm" title="Confirmation ">
            <p id="displayc">
                <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>
            </p>
        </div>
        <div id="dialog-message" title="Information">
            <p id="displayp">
                <span class="ui-icon ui-icon-circle-check" style="float: left; margin: 0 7px 50px 0;">
                </span>
            </p>
        </div>
    </div>
    <input type="hidden" id="userid" name="userid" value="<%= puserid  %>"
        style="width: 10px;" />
      <input type="hidden" id="ucheck" name="ucheck" value="<%= ucheck %>" />
        <input type="hidden" id="LA" name="LA" value="<%= la %>" />
    <input type="hidden" id="plateno" name="plateno" value="<%=plateno %>" />
    <input type="hidden" id="bdt" name="bdt" value="<%=begindatetime %>" />
    <input type="hidden" id="edt" name="edt" value="<%=enddatetime %>" />
    <input type="hidden" id="si" name="si" value="<%=searchin %>" />
    <input type="hidden" id="mvals" name="mvals" value="<%= mvals %>" />
    <input type="hidden" id="role" name="rolw" value="<%=role %>" />
    <input type="hidden" id="scode" name="scode" value="<%= scode %>" />
    <input type="hidden" id="sf" name="sf" value="<%= sf %>" />
    <input type="hidden" id="mapsettings" name="mapsettings" value="<%=mapsettings %>" />
    <input type="hidden" id="acode" name="acode" value="<%= acode %>" />
    <input type="hidden" id="markerlat" name="markerlat" value="<%= markerlat %>" />
    <input type="hidden" id="markerlon" name="markerlon" value="<%= markerlon %>" />
    <input type="hidden" name="gname" value="" id="gname" />
    <input type="hidden" id="lati" name="lati" value="" />
    <input type="hidden" id="lan" name="lan" value="" />
    <input type="hidden" id="polypts" name="polypts" value="" />
    <input type="hidden" id="circlepts" name="circlepts" value="" />
    <input type="hidden" id="poinm" name="poinm" value="" />
    <input type="hidden" name="poidet" value="" id="poidet" />
    <input type="hidden" id="querystring" name="querystring" value="<%= querystring %>" />
    <input type="hidden" id="polygonid" name="polygonid" value="<%= polygonid %>" />
        <input type="hidden" id="reqfrom" name="reqfrom" value="<%= reqfrom  %>" />
    <script type="text/javascript">
        var map = document.getElementById("map");
        map.style.height = getWindowHeight() + "px";
        map.style.width = getWindowWidth() + "px";
    </script>
    </form>
</body>
</html>
