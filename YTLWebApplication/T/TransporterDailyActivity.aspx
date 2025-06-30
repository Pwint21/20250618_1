<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.TransporterDailyActivity" Codebehind="TransporterDailyActivity.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Transporter Daily Activity</title>
    <style type="text/css" media="screen">
        @import "cssfiles/demo_page.css";
        @import "cssfiles/demo_table_jui.css";
        @import "cssfiles/themes/redmond/jquery-ui-1.8.4.custom.css";
        .dataTables_info
        {
            width: 25%;
            float: left;
        }
    </style>




    <link type="text/css" href="cssfiles/jquery-ui.css" rel="stylesheet" />
    <link href="cssfiles/css3-buttons.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery-ui-1.8.20.custom.min.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.dataTables.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/FixedColumns.js"></script>
    <script type="text/javascript">

        function getWindowWidth() { if (window.self && self.innerWidth) { return self.innerWidth; } if (document.documentElement && document.documentElement.clientWidth) { return document.documentElement.clientWidth; } return document.documentElement.offsetWidth; }
        function getWindowHeight() { if (window.self && self.innerHeight) { return self.innerHeight; } if (document.documentElement && document.documentElement.clientHeight) { return document.documentElement.clientHeight; } return document.documentElement.offsetHeight; }
		
        $(function () {
            $("#txtBeginDate").datepicker({
                dateFormat: 'yy/mm/dd', minDate: new Date(2013, 0, 01), maxDate: +0, changeMonth: true, changeYear: true, numberOfMonths: 2
            });

            $("#txtEndDate").datepicker({
                dateFormat: 'yy/mm/dd', minDate: new Date(2013, 0, 01), maxDate: +0, changeMonth: true, changeYear: true, numberOfMonths: 2

            });

            $("#dialog:ui-dialog").dialog("destroy");

            $("#dialog-message").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
                minWidth: getWindowWidth() - 60,
                minHeight: getWindowHeight() - 200
            });

            $("#idlingpage").width(getWindowWidth() - 65);
            $("#idlingpage").height(getWindowHeight() - 200);
      });
             
             function DisplayMap(intime,outtime,plateno) {
             document.getElementById("mappage").src = "GMap.aspx?bdt=" + intime + "&edt=" + outtime + "&plateno="+ plateno +"&scode=1&sf=0&r=" + Math.random();
                 document.getElementById("mappage").style.visibility = "visible";

          $("#dialog-message").dialog("open");  
            }  
    
     
function mysubmit()
{
    var username = document.getElementById("ddltransporter").value;
    if (username=="0")
    {
         alert("Please select transporter name");
         return false;         
    }
   
     
    
   
    var bigindatetime=document.getElementById("txtBeginDate").value+" "+document.getElementById("ddlbh").value+":"+document.getElementById("ddlbm").value;
    var enddatetime=document.getElementById("txtEndDate").value+" "+document.getElementById("ddleh").value+":"+document.getElementById("ddlem").value;
    
    var fdate=Date.parse(bigindatetime);
    var sdate=Date.parse(enddatetime);
    
    var diff=(sdate-fdate)*(1/(1000*60*60*24));
    var days=parseInt(diff)+1;
    if(days>5)
    {
        return confirm("You selected "+days+" days of data.So it will take more time to execute.\nAre you sure you want to proceed ? ");
    }
    return true;
     
}
var ec=<%=ec %>;
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
        function openpage(id,bdt,edt,tid) {
            openMap("ShowCompetitorInformation.aspx?geoid=" + id + "&transid="+ tid +"&bdt="+ bdt +"&edt="+ edt);
        }

        function openMap(message) {
            document.getElementById("idlingpage").style.visibility = "visible";
            document.getElementById("idlingpage").src = message;
            $("#dialog-message").dialog("open");
        }

  function fnFeaturesInit() {
		      
		        $('ul.limit_length>li').each(function (i) {
		            if (i > 10) {
		                this.style.display = 'none';
		            }
		        });

		        $('ul.limit_length').append('<li class="css_link">Show more<\/li>');
		        $('ul.limit_length li.css_link').click(function () {
		            $('ul.limit_length li').each(function (i) {
		                if (i > 5) {
		                    this.style.display = 'list-item';
		                }
		            });
		            $('ul.limit_length li.css_link').css('display', 'none');
		        });
		    }
            $(document).ready(function () {
		       fnFeaturesInit()
		        $('#examples').dataTable({
		            "bJQueryUI": true,
		            "sPaginationType": "full_numbers",
                    "iDisplayLength": 25,
                   "aLengthMenu": [ 10,25,50,100,200,500 ],
                    "fnDrawCallback": function (oSettings) {
                       if (oSettings.bSorted || oSettings.bFiltered) {
                        if (oSettings.aoColumns[0].bVisible == true) {
                            for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                                $('td:eq(0)', oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
                            }
                        }
                    }
                },
                   "aoColumnDefs": [
                          {   "bVisible": true, "bSortable": false, "aTargets": [0] } ,
                           
                           
                            
                    ]
                });

                $('#examples1').dataTable({
                    "bJQueryUI": true,
                    "sPaginationType": "full_numbers",
                    "iDisplayLength": 25,
                    "aLengthMenu": [10, 25, 50, 100, 200, 500],
                    "fnDrawCallback": function (oSettings) {
                        if (oSettings.bSorted || oSettings.bFiltered) {
                            if (oSettings.aoColumns[0].bVisible == true) {
                                for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                                    $('td:eq(0)', oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
                                }
                            }
                        }
                    },
                    "aoColumnDefs": [
                        { "bVisible": true, "bSortable": false, "aTargets": [0] },



                    ]
                });

                $('#examples2').dataTable({
                    "bJQueryUI": true,
                    "sPaginationType": "full_numbers",
                    "iDisplayLength": 25,
                    "aLengthMenu": [10, 25, 50, 100, 200, 500],
                    "fnDrawCallback": function (oSettings) {
                        if (oSettings.bSorted || oSettings.bFiltered) {
                            if (oSettings.aoColumns[0].bVisible == true) {
                                for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                                    $('td:eq(0)', oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
                                }
                            }
                        }
                    },
                    "aoColumnDefs": [
                        { "bVisible": true, "bSortable": false, "aTargets": [0] },



                    ]
                });

		          });


    </script>
    
</head>
<body id="index" style="font-size: 11px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
    <form id="Form1" method="post" runat="server">
    <center>
        <div>
            <br />
            <b style="font-family: Verdana; font-size: 20px; color: #38678B;">Transporter Daily Activity</b>
            <br />
            <br />
        </div>
        <table>
            <tr>
                <td align="center">
                    <table style="font-family: Verdana; font-size: 11px;">
                        <tr>
                            <td colspan="2" style="height: 20px; background-color: #465ae8;" align="left">
                                <b style="color: White;">&nbsp;Transporter Daily Activity&nbsp;:</b>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 450px; border: solid 1px #5B7C97;">
                                <table style="width: 450px;">
                                    <tbody>
                                        <tr>
                                            <td align="left">
                                                <b style="color: #465AE8;">Begin Date</b>
                                            </td>
                                            <td>
                                                <b style="color: #465AE8;">:</b>
                                            </td>
                                            <td align="left" style="width: 326px">
                                                <input readonly="readonly" style="width: 70px;" type="text" value="<%=strBeginDate%>"
                                                    id="txtBeginDate" runat="server" name="txtBeginDate" enableviewstate="false" /><b
                                                        style="color: #465AE8;">&nbsp;Hour&nbsp;:&nbsp;
                                                        <asp:DropDownList ID="ddlbh" runat="server" Width="40px" EnableViewState="False">
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
                                                    </b><b style="color: #465AE8;">&nbsp;Min&nbsp;:&nbsp;
                                                        <asp:DropDownList ID="ddlbm" runat="server" Width="40px" EnableViewState="False">
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
                                                    </b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <b style="color: #465AE8;">End Date</b>
                                            </td>
                                            <td>
                                                <b style="color: #465AE8;">:</b>
                                            </td>
                                            <td align="left" style="width: 326px">
                                                <input style="width: 70px;" readonly="readonly" type="text" value="<%=strEndDate%>"
                                                    id="txtEndDate" runat="server" name="txtEndDate" enableviewstate="false" /><b style="color: #465AE8;">&nbsp;Hour&nbsp;:&nbsp;
                                                        <asp:DropDownList ID="ddleh" runat="server" Width="40px" EnableViewState="False">
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
                                                    </b><b style="color: #465AE8;">&nbsp;Min&nbsp;:&nbsp;
                                                        <asp:DropDownList ID="ddlem" runat="server" Width="40px" EnableViewState="False">
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
                                                    </b>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <b style="color: #465AE8;">Transporter</b>
                                            </td>
                                            <td>
                                                <b style="color: #465AE8;">:</b>
                                            </td>
                                            <td align="left" style="width: 326px">
                                                <asp:DropDownList ID="ddltransporter" runat="server" Width="248px" AutoPostBack="true" >
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                         <tr>
                                            <td align="left">
                                                <b style="color: #465AE8;">No. of vehicles</b>
                                            </td>
                                            <td>
                                                <b style="color: #465AE8;">:</b>
                                            </td>
                                            <td align="left" style="width: 326px">
                                              <asp:Label ID="lbltransportercount"  runat ="server" Text="0"></asp:Label>
                                            </td>
                                        </tr>
                                        
                                        <tr>
                                            <td align="center">
                                                <br />
                                            </td>
                                            <td colspan="2" align="center">
                                                <br />
                                                <asp:Button ID="ImageButton1" class="action blue" runat="server" Text="Submit" ToolTip="Submit" />
                                                <%--<a href="javascript:ExcelReport();" class="button"><span class="ui-button-text" title="Download">
                                                    Download</a>--%> 
                                                <a href="javascript:print();" class="button"><span class="ui-button-text"
                                                        title="Print">Print</a>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <table>
            <tr>
                <td colspan="3">
                    <div style="width: 1190px;">
                        <%If ec = True Then%>
                        <%=sb1.ToString()%>
                        <%End If%>
                    </div>
                </td>
            </tr>
        </table>
        <table>
            <tr>
                <td colspan="3"><center><b>Competitor Geofence Trips</b></center></td></tr>
            <tr>
                <td colspan="3">
                    <div style="width: 1190px;">
                        <%=sb2.ToString()%>
                    </div>
                </td>
            </tr>
        </table>

        <table>
            <tr>
                <td colspan="3"><center><b>In-Active Vehicles List</b></center></td></tr>
            <tr>
                <td colspan="3">
                    <div style="width: 1190px;">
                        <%=sb3.ToString()%>
                    </div>
                </td>
            </tr>
        </table>
    </center>
     
    </form>
     <form id="excelform" method="get" action="ExcelReport.aspx">
    <input type="hidden" id="title" name="title" value="YTL Trailer Detach Report" />
    <input type="hidden" id="plateno" name="plateno" value="" />
    </form>
    

     <div id="dialog-message" title="Trips's Information" style="padding-top: 1px; padding-right: 0px;
        padding-bottom: 0px; font-size: 10px; padding-left: 0px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
        <iframe id="idlingpage" name="idlingpage" src="" frameborder="0" scrolling="auto"
            height="512" width="1198px" style="visibility: hidden; border: solid 1px #aac6ff;"/>
    </div>
   
</body>
</html>