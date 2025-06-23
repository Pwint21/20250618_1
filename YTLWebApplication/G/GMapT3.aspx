<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.GMapT3" CodeBehind="GMapT3.aspx.vb" %>

<html>
<head>
    <title>Gussmann Maps</title>
    <style type="text/css" media="screen">
        @import "cssfiles/demo_page.css";
        @import "cssfiles/demo_table_jui.css";
        @import "cssfiles/themes/redmond/jquery-ui-1.8.4.custom.css";

        .dataTables_info {
            width: 25%;
            float: left;
        }

        #tabs {
            font-family: "Trebuchet MS", "Helvetica", "Arial", "Verdana", "sans-serif";
        }

        .t1Textbox {
            background: url('images/findIcon.png') no-repeat scroll right center #FFFFFF;
            border: 1px solid #DDD;
            border-radius: 5px;
            box-shadow: 0 0 5px #888;
            color: #666;
            float: left;
            padding: 5px 27px 5px 10px;
            width: 100%;
            outline: none;
        }
    </style>
    <script type="text/javascript" src="js/googana.js"></script>
    <link type="text/css" href="cssfiles/jquery-ui.css" rel="stylesheet" />
    <link href="cssfiles/css3-buttons.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery-ui-1.8.20.custom.min.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.dataTables.js"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/FixedColumns.js"></script>
    <link href="cssfiles/style.css" rel="stylesheet" type="text/css" />
    <link href="cssfiles/demos22.css" rel="stylesheet" type="text/css" />
    <link href="GMap/default.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="https://maps.googleapis.com/maps/api/js?v=3.38&client=gme-zigbeeautomation&sensor=false&libraries=drawing&channel=YTL"></script>
    <link href="GMap/gdropdown2.css" rel="stylesheet" type="text/css" />
    <script src="GMap/gdropdown2.js" type="text/javascript"></script>
    <script src="GMap/maplabel-compiled.js"></script>

    <style type="text/css">
        div.side-by-side {
            width: 100%;
            margin-bottom: 1em;
        }

            div.side-by-side > div {
                float: left;
                width: 50%;
            }

                div.side-by-side > div > em {
                    margin-bottom: 10px;
                    display: block;
                }

        .clearfix:after {
            content: "\0020";
            display: block;
            height: 0;
            clear: both;
            overflow: hidden;
            visibility: hidden;
        }

        .VehicleMarker_OVERLAY {
            border-width: 0px;
            border: none;
            position: absolute;
            padding: 0px 0px 0px 0px;
            margin: 0px 0px 0px 0px;
            z-index: 99 !important;
        }

        .container.no-print {
            right: 380px !important;
            left: unset !important;
        }
    </style>
    <style type="text/css">
        v\:* {
            behavior: url(#default#VML);
        }

        #srchu {
            position: absolute;
            top: 0px;
            right: 0px;
            ;
            width: 300px;
            height: 30px;
            background: #fc0;
            display: none;
        }

        .search {
            position: absolute;
            top: 6px;
            right: 8px;
            visibility: visible;
            height: 28px;
        }

        .hideshowdiv {
            position: absolute;
            top: 0px;
            right: 500px;
            visibility: visible;
        }

        .searchin {
            border: 1px solid #333;
            right: 140px;
            padding-left: 2px;
            font-size: 11px;
            width: 128px;
            height: 18px;
            margin-top: 3px;
            margin-right: 139px;
            background: #fff;
        }

        .searchbutton {
            width: 60px;
        }

        .mpo {
            cursor: pointer;
        }

        .button {
            font-weight: bold;
            font-size: 11px;
            text-transform: lowercase;
            color: #FFFFFF;
            background: #d71a1a;
            border-width: 1px;
            border-style: solid;
            border-top-color: #f05555;
            border-left-color: #f05555;
            border-right-color: #7c0808;
            border-bottom-color: #7c0808;
        }
    </style>
    <style type="text/css" media="print">
        .no-print {
            display: none;
        }
    </style>
    <style type="text/css">
        .ui-tabs .ui-tabs-nav li {
            margin: 0 7px 1px 0;
        }

        input.pravinstyle {
            border: 1px solid #c4c4c4;
            width: 157px;
            font-size: 13px;
            padding: 4px 4px 4px 4px;
            box-shadow: 0px 0px 8px #d9d9d9;
            -moz-box-shadow: 0px 0px 8px #d9d9d9;
            -webkit-box-shadow: 0px 0px 8px #d9d9d9;
        }

            input.pravinstyle:focus {
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
                font: normal 13px/100% Arial;
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
                background: -moz-linear-gradient(top, #9e9e9e, #454545);
                -ms-filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#9e9e9e', endColorstr='#454545'); /* ie8 */
            }

        .ui-autocomplete {
            max-height: 150px;
            max-width: 275px;
            overflow-y: auto;
            outline-color: Aqua; /* add padding to account for vertical scrollbar */
            padding-right: 15px;
        }
        /* IE 6 doesn't support max-height
	 * we use height instead, but this forces the menu to always be this tall
	 */
        * html .ui-autocomplete {
            height: 100px;
        }

        .imgloading {
            background: white url('images/loading.gif') right center no-repeat;
        }

        .tooltip {
            border: 1px solid #777;
            background-color: #FFFF77;
            padding: 2px 5px 2px 5px;
            font-size: 12px;
        }

        ​
    </style>

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
        }
    </script>
    <script type="text/javascript">
        var isOpen = true;
        var platesTable;

        jQuery.extend(jQuery.fn.dataTableExt.oSort, {
            "num-html-pre": function (a) {
                var x = parseFloat(a.toString().replace(/<.*?>/g, ""));
                if (isNaN(x)) {
                    if (a.toString().indexOf("fuel sensor problem") > 0) {
                        return -1;
                    }
                    else if (a.toString().indexOf("no fuel sensor") > 0) {
                        return -0.1;
                    }

                    return 0.0;
                }
                else {
                    return x;
                }
            },

            "num-html-asc": function (a, b) {
                return ((a < b) ? -1 : ((a > b) ? 1 : 0));
            },

            "num-html-desc": function (a, b) {
                return ((a < b) ? 1 : ((a > b) ? -1 : 0));
            }
        });

        $(document).ready(function () {
            $(".ossvis").hide();
            $(".osshide").show();

            $("#tabs-5").hide();
            if ($("#juname").val() == "MARTINYTL" || $("#juname").val() == "JULIANAYTL" || $("#juname").val() == "SPYON" || $("#juname").val() == "CHEEKEONGYTL" || $("#juname").val() == "SWEEHAR" || $("#juname").val() == "ADMIN" || $("#juname").val() == "CHAN_BS" || $("#juname").val() == "FSLEE_PG_TB" || $("#juname").val() == "JAYA" || $("#juname").val() == "JOELCHONG" || $("#juname").val() == "MHWOON_PR" || $("#juname").val() == "MSYAZWAN_PR" || $("#juname").val() == "KLWONG_LM" ) {
                $("#tabs-5").show();
            }

            $('#tabs').tabs({
                select: function (event, ui) {
                    $(".ui-tabs-panel").hide();
                    var tabNumber = ui.index;
                    $(".ossvis").hide();
                    $(".osshide").show();
                    if (jobmarkerCluster != null) {
                        jobmarkerCluster.clearMarkers();
                    }
                    ClearArrowVehicleMarker();

                    getUsers();
                    $("#jobtext").parent().parent().hide();
                    $("#platenotext").parent().parent().hide();
                    $("#intervalts").parent().parent().hide();
                    if (ui.tab.parentElement.id == "tabs-2") {
                        //is History Tab
                        $("#intervalts").parent().parent().show();
                    }
                    if (tabNumber == 3) {
                        $("#tabs-1").addClass("ui-state-default ui-corner-top ui-tabs-active ui-state-active");
                        $("#apk").click();
                        ShowVehiclePath(1);
                    }
                    else if (tabNumber == 4) {
                        $(".ossvis").show();
                        $(".osshide").hide();
                        $("#jobtext").parent().parent().show();
                        $("#platenotext").parent().parent().show();
                        var radioId = parseInt(tabNumber) + 1;
                        document.getElementById("radio" + radioId).checked = true;
                    }
                    else {
                        var radioId = parseInt(tabNumber) + 1;
                        document.getElementById("radio" + radioId).checked = true;
                    }
                }
            });
            $(".ui-tabs-panel").hide();
            platesTable = $('#platenos').dataTable({
                "bJQueryUI": true,
                "iDisplayLength": 1000,
                "sScrollY": getWindowHeight() - 415,
                "bLengthChange": false,
                "bScrollCollapse": true,
                "bAutoWidth": false,
                "aaSorting": [[1, "asc"]],

                "fnDrawCallback": function (oSettings) {
                    /* Need to redo the counters if filtered or sorted */
                    if (oSettings.bSorted || oSettings.bFiltered) {
                        if (oSettings.aoColumns[0].bVisible == true) {
                            for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                                $('td:eq(0)', oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
                            }
                        }
                    }
                },
                "aoColumnDefs": [
                    { "bVisible": false, "sWidth": "30px", "bSortable": false, "aTargets": [0] },
                    {
                        "bSortable": false, "bVisible": true, "sWidth": "135px", "sClass": "left", "sType": "num-html",
                        "fnRender": function (oData, sVal) {
                            switch (oData.aData[2]) {
                                case 1:
                                    vstatus = " style=\"Color:Blue;font:'Trebuchet MS', sans-serif;'\" ";
                                    break;
                                case 2:
                                    vstatus = " style=\"Color:Green;font:'Trebuchet MS', sans-serif;' \" ";
                                    break;
                                default:
                                    vstatus = " style=\"Color:Red;font:'Trebuchet MS', sans-serif;' \" ";

                            }
                            if (oData.aData[3] != "-") {
                                listItems = "<a onclick='javascript:showmyValue(this)' title=\"Click to view on Map\" class='aclass' value=" + oData.aData[1] + " " + vstatus + " >" + oData.aData[3] + " - " + oData.aData[1] + "</a>";
                            }
                            else {
                                listItems = "<a onclick='javascript:showmyValue(this)' title=\"Click to view on Map\" class='aclass' value=" + oData.aData[1] + " " + vstatus + " >" + oData.aData[1] + "</a>";
                            }

                            return listItems;

                        },

                        "aTargets": [1]
                    },

                    { "bVisible": false, "sWidth": "30px", "bSortable": false, "aTargets": [2] }
                ]
            });
            $(".display thead").hide();
            $(".display tfoot").hide();
            var count = 0;
            $(".ui-corner-bl").hide();
            $(".trigger").attr("title", "Open Vehicle Search");
            $(".trigger").click(function () {
                $(".jaffa").toggle("fast");
                $(this).toggleClass("active");
                platesTable.fnSort([[1, 'asc']]);
                if ($(this).hasClass("active")) {
                    //t1Textbox
                    $(".dataTables_filter").find('input[type="text"]').addClass("t1Textbox");
                    $(".dataTables_filter").find('input[type="text"]').focus();
                    $(".trigger").attr("title", "Close Vehicle Search");
                }
                else {
                    $(".trigger").attr("title", "Open Vehicle Search");
                }

                return false;
            });

        });
        function showmyValue(ctrl) {
            document.getElementById("plateno").value = ctrl.getAttribute("value"); //ctrl.innerHTML;
            document.getElementById("box1View").value = ctrl.getAttribute("value");  //ctrl.innerHTML;
            if (document.getElementById("radio1").checked) {
                osmMap.setZoom(15);
                gotoloc();
            }
            else if (document.getElementById("radio2").checked) {
                osmMap.setZoom(15);
                ShowVehiclePath(0);
            }
            else if (document.getElementById("radio3").checked) {
                osmMap.setZoom(15);
                ShowVehiclePath(3);
            }

        }
        function clickMe() {
            $(".trigger").click();
        }
    </script>
    <style type="text/css">
        #over_map {
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

        #uha {
            margin: 0 auto;
            padding: 0;
            width: 350px;
        }

        a:focus {
            outline: none;
        }

        #panel {
            display: none;
            font: 120%/100% Arial, Helvetica, sans-serif;
        }

        .btn-slide {
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

        .active {
            background-position: right 12px;
        }

        .vlabel {
            background: #FFFF00;
            padding: 2px;
            border: solid 1px black;
            font-family: Verdana;
            font-size: 11px;
        }

        div.side-by-side {
            width: 100%;
            margin-bottom: 1em;
        }

            div.side-by-side > div {
                float: left;
                width: 50%;
            }

                div.side-by-side > div > em {
                    margin-bottom: 10px;
                    display: block;
                }

        .clearfix:after {
            content: "\0020";
            display: block;
            height: 0;
            clear: both;
            overflow: hidden;
            visibility: hidden;
        }

        .aclass {
            cursor: pointer;
            font-size: 12px;
            padding: 2px;
            font-family: verdana;
        }

            .aclass hover {
                color: Red;
                cursor: pointer;
            }

        .dataTables_filter {
            width: 100%;
            float: left;
            text-align: center;
            font-size: 0;
        }
    </style>
    <link href="GMap/VehicleMarker.css" rel="stylesheet" type="text/css" />
    <link href="cssfiles/slide2.css?r=<%= Now.ToString("yyyy/MM/dd HH:mm:ss") %>" rel="stylesheet"
        type="text/css" />
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
        <link href="cssfiles/tooltip.css" rel="stylesheet" />
    <script src="jsfiles/tooltip.js"></script>
    <script src="GMap/richmarker.js?r=0.9876576" type="text/javascript"></script>
    <script src="GMap/markerclusternew.js?r=0.9876578" type="text/javascript"></script>
    <script src="GMap/GMapT3.js?r=<%= Now.ToString("yyyy/MM/dd HH:mm:ss") %>" type="text/javascript"></script>
    <%--<script src="GMap/GMapT1.js?r=<%= Now.ToString("yyyy/MM/dd HH:mm:ss") %>" type="text/javascript"></script>--%>
</head>
<body onresize="resize()" onload="javascript:callit()" style="margin: 0px; padding: 0px; overflow: hidden;">
    <form id="form1" runat="server">
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
            <div style="position: absolute; left: 45%; top: 45%; font-family: Verdana; font-size: 12px; font-weight: bold; color: #d71a1a;">
                <img alt="Loading Map..." title="Loading Map..." src="images/loading.gif" />&nbsp;Loading
            Map...
            </div>
        </div>
        <div id="uha" class="hideshowdiv no-print ">
            <div id="pp">
            </div>
            <a href="#" class="btn-slide slide" onclick="javascript:clickMe()">
                <img id="udimg" src="images/down.gif" alt="Toggle vehicle search" title="Toggle vehicle search"
                    style="visibility: hidden; border: 0px none FFFFFF" /></a>
        </div>
        <div id="overmap1" class="jaffa">
            <table id="" style="text-align: right; margin: 0px; padding-bottom: 1px; padding-right: 0px;">
                <tr>
                    <td colspan="6">
                        <input type="radio" id="radio1" name="radio" checked="true" style="display: none;" />
                        <input type="radio" id="radio2" name="radio" style="display: none;" />
                        <input type="radio" id="radio3" name="radio" style="display: none;" />
                        <input type="radio" id="radio5" name="radio" style="display: none;" />
                        <div id="tabs" style="font-size: 11px;">
                            <ul style="padding-left: 20px;">
                                <li id="tabs-1"><a id="apk" href="#tabs1">Current</a></li>
                                <li id="tabs-2"><a href="#tabs2">History</a></li>
                                <li id="tabs-3"><a href="#tabs3">Playback</a></li>
                                <li id="tabs-4"><a href="#tabs4">All</a></li>
                                <li id="tabs-5"><a href="#tabs5">Truck-Job</a></li>
                            </ul>
                            <div id="tabs1">
                            </div>
                            <div id="tabs2">
                            </div>
                            <div id="tabs3">
                            </div>
                            <div id="tabs5">
                            </div>
                        </div>
                    </td>
                </tr>

                <tr class="osshide">
                    <td align="right" style="width: 80px;">
                        <b style="color: #2500EA; font-size: 12px;">Begin :</b>
                    </td>
                    <td align="left">
                        <input type="text" id="begindate1" runat="server" style="width: 75px; position: relative; z-index: 100000;"
                            readonly="readonly" enableviewstate="false" />
                    </td>
                    <td align="left">
                        <b style="color: #2500EA; font-size: 12px;">HH:</b>
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
                        <b style="color: #2500EA; font-size: 12px;">MM:</b>
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
                <tr class="osshide">
                    <td align="right">
                        <b style="color: #2500EA; font-size: 12px;">End :</b>
                    </td>
                    <td align="left">
                        <input type="text" id="enddate1" runat="server" style="width: 75px; position: relative; z-index: 100000;"
                            readonly="readonly" enableviewstate="false" />
                    </td>
                    <td align="left">
                        <b style="color: #2500EA; font-size: 12px;">HH:</b>
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
                        <b style="color: #2500EA; font-size: 12px;">MM:</b>
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
                <tr class="ossvis">
                    <td align="right">
                        <b style="color: #2500EA; font-size: 12px;">Transporter : </b>
                    </td>
                    <td align="left" colspan="5">
                        <select style="width: 100%;" id="selectTType" onchange="javascript:getUsersWithTransporter()">
                            <option value="0">All</option>
                            <option value="1">Internal</option>
                            <option value="2">Exernal</option>
                        </select>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <b style="color: #2500EA; font-size: 12px;">User : </b>
                    </td>
                    <td align="left" colspan="5">
                        <select id="selectUser" style="width: 100%;" onchange="javascript:getGroups()">
                        </select>
                    </td>
                </tr>
                <tr class="ossvis">
                    <td align="right">
                        <b style="color: #2500EA; font-size: 12px;">Status : </b>
                    </td>
                    <td align="left" colspan="5">
                        <select style="width: 100%;" id="selectStatus" onchange="javascript:getPlatesWithStatus()">
                            <option value="0">All</option>
                            <option value="1">Delivery Completed</option>
                            <option value="3">In Progress</option>
                            <option value="5">Inside Geofence</option>
                        </select>
                    </td>
                </tr>
                <tr class="ossvis">
                    <td align="right">
                        <b style="color: #2500EA; font-size: 12px;">Truck Type : </b>
                    </td>
                    <td align="left" colspan="5">
                        <select style="width: 100%;" id="selectType" onchange="javascript:getPlatesWithStatus()">
                            <option value="ALL">All</option>
                            <option value="TANKER">Tanker</option>
                            <option value="CARGO">Cargo</option>
                            <option value="TIPPER">Tipper</option>
                        </select>
                    </td>
                </tr>
                <tr class="osshide">
                    <td align="right">
                        <b style="color: #2500EA; font-size: 12px;">Group : </b>
                    </td>
                    <td align="left" colspan="5">
                        <select style="width: 100%;" id="selectGroup" onchange="javascript:getPlates()">
                        </select>
                    </td>
                </tr>
                <tr class="osshide">
                    <td align="right"></td>
                    <td align="left" colspan="5">
                        <div>
                            <table border="0" cellpadding="0" cellspacing="0" class="display" id="platenos" style="font-family: Verdana;">
                                <thead style="text-align: left;">
                                    <tr>
                                        <th width="130px">Plate No
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                </tbody>
                                <tfoot style="text-align: left;">
                                    <tr>
                                        <th width="130px">Plate No
                                        </th>
                                    </tr>
                                </tfoot>
                            </table>
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <a class="trigger" href="#" style="z-index: 999; border-bottom-right-radius: 25px; border-top-right-radius: 25px;">
            <img src="images/fleeticon.png" style="height: 30px; width: 30px;" alt="Vehicle" /></a>
        <div class="demo">
            <div id="dialog-confirm" title="Confirmation ">
                <p id="displayc">
                    <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>
                </p>
            </div>
            <div id="dialog-message" title="Information">
                <p id="displayp">
                    <span class="ui-icon ui-icon-circle-check" style="float: left; margin: 0 7px 50px 0;"></span>
                </p>
            </div>
        </div>
        <input type="hidden" id="userid" name="userid" value="<%= Request.Cookies("userinfo")("userid") %>"
            style="width: 10px;" />
        <input type="hidden" id="ucheck" name="ucheck" value="<%= ucheck %>" />

        <input type="hidden" id="juname" name="juname" value="<%= juname%>" />

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
        <input type="hidden" name="box1View" value="" id="box1View" />
        <input type="hidden" name="ddluid" value="ALLUSERS" id="ddluid" />
        <input type="hidden" id="ddlgid" name="lati" value="" />
        <script type="text/javascript">
            var map = document.getElementById("map");
            map.style.height = getWindowHeight() + "px";
            map.style.width = getWindowWidth() + "px";
        </script>
        <style type="text/css">
            .panel-body {
                padding: 0px;
                width: 301px;
            }

            .panel-title {
                margin-top: 0;
                margin-bottom: 0;
                font-size: 16px;
                color: inherit;
                box-sizing: border-box;
                -webkit-box-sizing: border-box;
                box-sizing: border-box;
            }

            .panel-primary > .panel-heading {
                color: #fff;
                background-color: #337ab7;
                border-color: #337ab7;
                box-sizing: border-box;
                -webkit-box-sizing: border-box;
                box-sizing: border-box;
            }

            .panel-heading {
                padding: 10px 15px;
                border-bottom: 1px solid transparent;
                border-top-left-radius: 3px;
                border-top-right-radius: 3px;
                box-sizing: border-box;
                -webkit-box-sizing: border-box;
                box-sizing: border-box;
            }

            .panel-primary {
                border-color: #337ab7;
                background-color: white;
                box-sizing: border-box;
                -webkit-box-sizing: border-box;
                box-sizing: border-box;
            }

            .panel {
                margin-bottom: 20px;
                background-color: #fff;
                border: 1px solid transparent;
                border-radius: 4px;
                -webkit-box-shadow: 0 1px 1px rgba(0,0,0,.05);
                box-shadow: 0 1px 1px rgba(0,0,0,.05);
                box-sizing: border-box;
                -webkit-box-sizing: border-box;
                box-sizing: border-box;
            }
        </style>
    </form>
</body>
</html>