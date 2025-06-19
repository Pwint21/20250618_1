<%@ Page Language="vb" Debug="true" AutoEventWireup="false" Inherits="YTLWebApplication.Main" CodeBehind="Main.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head>
    <title>YTL - AVLS</title>
    <style type="text/css" title="currentStyle">
        @import "cssfiles/demo_page.css";
        @import "cssfiles/demo_table_jui.css";
        @import "cssfiles/themes/redmond/jquery-ui-1.8.4.custom.css";
        @import "cssfiles/jquery-ui.css";
        @import "cssfiles/jquery.ui.dialog.css";
        @import "cssfiles/TableTools.css";
        @import "cssfiles/ColVis.css";
    </style>
    <style type="text/css">
        .hor-minimalist-b {
            font-family: "Bell MT";
            font-size: 14px;
            background: #fff;
            margin: 0px;
            width: 520px;
            border-collapse: collapse;
            text-align: left;
            height: 156px;
        }

            .hor-minimalist-b th {
                font-size: 16px;
                font-weight: bold;
                color: #039;
                padding: 5px 15px;
            }

            .hor-minimalist-b td {
                font-size: 14px;
                color: #669;
                padding: 4px 15px;
            }

            .hor-minimalist-b tbody tr:hover td {
                color: #009;
            }

        .textbox1 {
            height: 20px;
            width: 180px;
            border-right: #cbd6e4 1px solid;
            border-top: #cbd6e4 1px solid;
            border-left: #cbd6e4 1px solid;
            color: #0b3d62;
            border-bottom: #cbd6e4 1px solid;
        }
    </style>
    <link rel="shortcut icon" href="images/gussmann.ico" />
    <link type="text/css" href="cssfiles/main2.css" rel="stylesheet" />
    <script type="text/javascript" src="jsfiles/jquery.js"></script>
    <script type="text/javascript" src="jsfiles/jquery-ui.js"></script>
    <link rel="stylesheet" type="text/css" href="cssfiles/jquery.gritter.css" />
    <link href="cssfiles/style.css" rel="stylesheet" type="text/css" />
    <link href="cssfiles/demos22.css" rel="stylesheet" type="text/css" />
    <link href="cssfiles/jquery.ui.dialog.css" rel="stylesheet" type="text/css" />
    <script src="jsfiles/soundmanager.js" type="text/javascript"></script>
    <script type="text/javascript">
        var mySound;
        // directory where SM2 .SWFs live
        soundManager.url = 'jsfiles/';
        soundManager.debugMode = false;
        soundManager.onready(function () {
            mySound = soundManager.createSound(
                {
                    id: 'aSound',
                    url: 'cssfiles/audiofiles/beep.mp3'
                });
        });
        soundManager.ontimeout(function () { });
        $(function () {
            if ($.browser.msie && $.browser.version.substr(0, 1) < 7) {
                $('li').has('ul').mouseover(function () {
                    $(this).children('ul').show();
                }).mouseout(function () {
                    $(this).children('ul').hide();
                });
            }
        });
        function init() {
            document.getElementById("mainframe").height = $(window).height() - 32 + "px";
            getNumber();
        }
        function resize() {
            document.getElementById("mainframe").height = $(window).height() - 32 + "px";
        }
        function getNumber() {
            $.get('NumberOfAlerts.aspx?r=' + Math.random(), function (data) {
                if (data != "0") {
                    document.getElementById("atag").innerHTML = "Web Alerts(" + data + ")";
                }
                else {
                    document.getElementById("atag").innerHTML = "Web Alerts";
                }
            });
        }
        window.setInterval(getNumber, 60000);
    </script>
    <script type="text/javascript" src="jsfiles/jquery.gritter.js"></script>
    <script type="text/javascript">
        var count = 0;
        var id;
        var playalertsound = true;
        var muteunmuteimg = "images/unmute.png";
        var muteunmutetitle = "Click to mute alert sounds.";

        function FixSound(imgt) {
            if (imgt.src.substr(imgt.src.lastIndexOf("/") + 1) == "mute.png") {
                playalertsound = true;
                muteunmuteimg = "images/unmute.png";
                muteunmutetitle = "Click to mute alert sounds.";
                $("#gritter-notice-wrapper .mute").each(function (i, value) {
                    value.src = "images/unmute.png";
                    value.title = "Click to mute alert sound.";
                });
            }
            else {
                playalertsound = false;
                muteunmuteimg = "images/mute.png";
                muteunmutetitle = "Click to unmute alert sounds.";
                $("#gritter-notice-wrapper .mute").each(function (i, value) {
                    value.src = "images/mute.png";
                    value.title = "Click to unmute alert sound.";
                });
            }
        }

        function callit() {
            document.getElementById("mainframe").src = "WebAlerts.aspx";
        }

        function callme() {
            var x = <%=nid %>;
            if (count == 0) {
                id = x;
            }
            count += 1;
            var resp;
            var cvalue = getCookie($("#hiduserid").val() + "webalerts");
            var lastXhr = $.getJSON("GetLatest.aspx?id=" + id + "&r=" + Math.random(), resp, function (data) {
                if (data.length > 0 && cvalue == 1) {
                    if (playalertsound) {
                        mySound.play();
                    }
                    for (var i = 0; i < data.length; i++) {
                        id = data[i][0];
                        var plateno = data[i][1];
                        var alertname = data[i][3];
                        var datetime = data[i][2];
                        var animStyle = "slide";
                        var imagepath = "";
                        if (alertname == "PANIC" || alertname == "POWERCUT") {
                            imagepath = "images/n2.png";
                        }
                        else {
                            imagepath = "images/n1.png";
                        }
                        if (alertname.indexOf("Geofence out") >= 0) {
                            alertname = data[i][4] + " Out"
                        }
                        else if (alertname.indexOf("Geofence In") >= 0) {
                            alertname = data[i][4] + " In"
                        }
                        else if (alertname.indexOf("Idling") >= 0) {
                            alertname = alertname + "(" + data[i][4].split(";")[1] + " Mins)";
                        }
                        else if (alertname.indexOf("OVERSPEED") >= 0) {
                            alertname = alertname + "(" + data[i][4] + " Km/h)";
                        }

                        var ttle = '<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\"  style=\"text-align:left;width:90%; cursor:pointer;\" ><tr><td colspan=\"2\"><span onclick=\"javascript:callit()\" style=\"cursor:pointer;text-align:center;\">' + alertname + '</span></td><td colspan=\"2\" style=\"text-align:right;"><img class=\"mute\" onclick=\"javascript:FixSound(this)\" style=\"cursor:pointer;display:none;\" title=\"' + muteunmutetitle + '\" src=\"' + muteunmuteimg + '\" /></td></tr></table>';

                        // var ttle='<span onclick=\"javascript:callit()\" style=\"cursor:pointer;\">' + alertname  + '</span>' ;
                        var datatext = '<span onclick=\"javascript:callit()\" style=\"cursor:pointer;\">' + plateno + '</span><br/><span onclick=\"javascript:callit()\" style=\"cursor:pointer;\">' + datetime + '</span>';

                        var unique_id = $.gritter.add({
                            title: ttle,
                            text: datatext,
                            image: imagepath,
                            sticky: true,
                            time: '',
                            class_name: 'my-sticky-class',
                            before_open: function () {
                                if ($('.gritter-item-wrapper').length >= 4) {
                                    var uuid = parseInt($('.gritter-item-wrapper')[0].id.split("-")[2]);
                                    $.gritter.remove(uuid, {
                                        fade: false,
                                        speed: 'fast'
                                    });
                                }
                            }
                        });



                        setTimeout(function () {
                            $.gritter.remove(unique_id, {
                                fade: true,
                                speed: 'fast'
                            });
                        }, 30000);
                    }
                }

            });
        }

        window.setInterval(callme, 60000);
        var result = 0;
        $(function () {
            var cvalue = getCookie($("#hiduserid").val() + "webalerts");
            if (cvalue == 1) {
                $("#imgsubscribe").show();
                $("#imgUnsubscribe").hide();
            }
            else {
                $("#imgUnsubscribe").show();
                $("#imgsubscribe").hide();
            }

            $("#div-pass").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
                width: 520,
                minHeight: 250,
                height: 250,
                title: "Password Change",
                buttons: {
                    //                     Change:function(){
                    //                       changepassword();
                    //                     },
                    Ok: function () {
                        result = 1;
                        $(this).dialog("close");
                    }

                }
            });

            $("#div-pass").on("dialogbeforeclose", function (event, ui) {
                result = 1;
                if (result == 0) {
                    alertbox("<b>Please change Password to enter in to the site.</b>");
                    return false;
                }
            });

            $("#dialog-confirm").dialog({
                resizable: false,
                draggable: false,
                height: 140,
                modal: true,
                autoOpen: false,
                buttons: {
                    "Yes": function () {
                        confirmresult = true;
                        $(this).dialog("close");
                    },
                    No: function () {
                        confirmresult = false;
                        $(this).dialog("close");

                    }
                }
            });

            $("#dialog:ui-dialog").dialog("destroy");

            $("#dialog-alert").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
                buttons: {
                    Ok: function () {
                        $(this).dialog("close");
                    }
                }
            });

            // checkpassword();

        });

        function mysubmit() {
            var newpass = document.getElementById("newpass").value;
            var confmpass = document.getElementById("confpass").value;

            if (document.getElementById("newpass").value == "") {
                alertbox("Please Enter New Pasword");
                return false;
            }
            else if (document.getElementById("confpass").value == "") {
                alertbox("Please again enter Pasword to Conform");
                return false;
            }
            else if (newpass.length < 6 || newpass.length > 10) {
                alertbox("Password Should be in between 6  to  10 Characters ");
                return false;
            }

            else if (newpass != confmpass) {
                alertbox("New Password and Conform password Should be same");
                return false;
            }

            else
                return true;
        }


        function changepassword() {

            var pwd = document.getElementById("newpass").value;
            var res = mysubmit();
            if (res == true) {
                $.getJSON('updatepassword.aspx?pwd=' + pwd + '&opr=change&r=' + Math.random(), null, function (json) {
                    if (json.d == 1) {
                        result = 1;

                        $("#div-pass").dialog("close");

                    }
                    else if (json.d == 2) {
                        alertbox("New Password Should not be the Old Password ");
                    }
                    else if (json.d == 3) {
                        alertbox("Sorry Password Not Updates please try Again ");
                    }
                });
            }
        }

        function checkpassword() {
            $.getJSON('updatepassword.aspx?opr=check&r=' + Math.random(), null, function (json) {
                if (json.d == 1) {

                }
                else {
                    document.getElementById("mainframe").src = "PasswordManagement.aspx";
                    $("#div-pass").dialog("open");
                }
            });
        }

        function unsubscribealerts(value) {
            setCookie($("#hiduserid").val() + "webalerts", value, 360);
            if (value == 1) {
                $("#imgsubscribe").show();
                $("#imgUnsubscribe").hide();
            }
            else {
                $("#imgUnsubscribe").show();
                $("#imgsubscribe").hide();
            }
        }
        function setCookie(cname, cvalue, exdays) {
            var d = new Date();
            d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
            var expires = "expires=" + d.toGMTString();
            document.cookie = cname + "=" + cvalue + "; " + expires;
        }
        function getCookie(cname) {
            var name = cname + "=";
            var ca = document.cookie.split(';');
            for (var i = 0; i < ca.length; i++) {
                var c = ca[i].trim();
                if (c.indexOf(name) == 0)
                    return c.substring(name.length, c.length);
            }
            return "1";
        }


        function confirmbox(confirmMessage) {
            confirmresult = false;
            document.getElementById("displayc").innerHTML = confirmMessage;
            $("#dialog-confirm").dialog("open");
        }
        function alertbox(message) {
            document.getElementById("displayp").innerHTML = message;
            $("#dialog-alert").dialog("open");
        }


    </script>
</head>
<body style="margin: 0px; overflow: hidden;" onload="init();" onresize="resize();">
    <table width="100%" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td>
                <ul class="menu">
                    <%If username = "OPERATIONT1" Then%>
                    <li><a href="SmartFleetApk.aspx" target="mainframe">Smart Fleet</a> </li>
                    <li><a href="GMapT3.aspx" title="Google Map" target="mainframe">G Map</a></li>
                    <li><a href="#">OSS</a><ul>
                        <li><a href="OssManagementT.aspx" target="mainframe">OSS - Management</a></li>
                        <li><a href="GeofenceManagementPublic.aspx" target="mainframe">Public Geofence Management</a></li>
                        <li><a href="ShipToCodeManagementTest.aspx" target="mainframe">ShipToCode Management</a></li>
                    </ul>
                    </li>
                    <li><a href="#">Reports</a>
                        <ul>
                            <li><a href="VehicleLogReport.aspx" target="mainframe">Log</a></li>
                        </ul>
                    </li>
                    <%ElseIf username = "HASLIZA"%>

                    <%Else%>
                    <%If username = "BINTANG" Then%>
                    <li><a href="SmartFleetApk.aspx" target="mainframe">Smart Fleet</a> </li>
                    <%Else%>
                    <%If PubUserid = "2012" Or PubUserid = "156" Or PubUserid = "1997" Then%>
                    <li><a href="SmartFleetsek.aspx" target="mainframe">Smart Fleet</a> </li>
                    <% Else%>
                    <li><a href="SmartFleetApk.aspx" target="mainframe">Smart Fleet</a> </li>
                    <%End If%>
                    <li><a href="GMapT3.aspx" title="Google Map" target="mainframe">G Map</a></li>
                    <%If role = "Admin" Then%>

                    <li><a href="#">Admin Management</a><ul>
                        <%if Not (PubUserid = "7175" Or PubUserid = "0002") %>
                        <li><a href="#">Customer</a>
                            <ul>
                                <li><a href="TransporterCompany.aspx" target="mainframe">1.Create Admin</a></li>
                                <li><a href="TransporterUser.aspx" target="mainframe">2.Create Password</a> </li>
                                <%--  <li><a href="soldtomanagement.aspx" target="mainframe">3.Create Sold To Customer</a></li>
                                <li><a href="clientsoldtomanagement.aspx" target="mainframe">4.Create Password For Sold To Customer</a> </li>--%>
                            </ul>
                        </li>
                        <li><a href="CapitalManagement.aspx" target="mainframe">Capital Management</a> </li>
                        <%End If %>

                        <li><a href="#">Geofence Management</a>
                            <ul>
                                <li><a href="GeofenceManagementLafarge.aspx" target="mainframe">YTL</a> </li>
                                <li><a href="GeofenceManagementPrivate.aspx" target="mainframe">Private</a> </li>
                                <li><a href="GeofenceTypeManagment.aspx" target="mainframe">Geofence Type</a> </li>
                                <li><a href="GeofenceSummaryPostProcessPublic.aspx" target="mainframe">Post Process[Public]</a> </li>
                            </ul>
                        </li>
                        <li><a href="InstantAlertSettingsManagement.aspx" target="mainframe">Instant Alert Management</a>
                        </li>
                        <%if Not (PubUserid = "7175" Or PubUserid = "0002") %>
                        <li><a href="FuelFormulaManagement.aspx" target="mainframe">Fuel Formula Management</a>
                        </li>
                        <%End If%>

                        <li><a href="MapSettingsManagement.aspx" target="mainframe">Map Settings Management</a>
                        </li>
                        <li><a href="PasswordManagement.aspx" target="mainframe">Password Management</a>
                        </li>
                        <li><a href="POIManagement.aspx" target="mainframe">POI Management</a> </li>
                        <%if Not (PubUserid = "7175" Or PubUserid = "0002") %>
                        <li><a href="SMSSummaryReport.aspx" target="mainframe">SMS Dispatch Summary Report</a></li>

                        <li><a href="TrailerManagementNew.aspx" target="mainframe">Trailer Management</a>
                        </li>

                        <li><a href="#">Internal</a>
                            <ul>
                                <li><a href="http://lafargewebpp.avls.com.my/PostProcessWeb.aspx" target="_blank">Process Data</a></li>
                                <li><a href="hLogin.aspx" target="mainframe">Update History</a> </li>
                                <li><a href="UpdatePlatenoManagement.aspx" target="mainframe">Update Plateno</a>
                                </li>
                                <li><a href="FuelManagement.aspx" target="mainframe">Fuel Receipt Management</a>
                                </li>
                            </ul>
                        </li>
                        <%End If%>

                        <li><a href="#">User Management</a>
                            <ul>
                                <li><a href="OperatorManagement.aspx" target="mainframe">Operator</a> </li>
                                <li><a href="SuperUserManagement.aspx" target="mainframe">Superuser</a> </li>
                                <li><a href="SuperUserManagementL.aspx" target="mainframe">User to Multiple Superuser</a> </li>
                                <li><a href="UserManagement.aspx" target="mainframe">User</a> </li>
                            </ul>
                        </li>
                        <li><a href="#">Vehicle Management</a>
                            <ul>
                                <li><a href="GroupManagement.aspx" target="mainframe">Group</a> </li>
                                <li><a href="UnitManagement.aspx" target="mainframe">Unit</a> </li>
                                <li><a href="VehicleManagement.aspx" target="mainframe">Vehicle</a> </li>
                            </ul>
                        </li>
                    </ul>
                    </li>
                    <!--% ElseIf viewer = True Then%>
                    <li><a href="#">Management</a><ul>                        
                        <li><a href="PasswordManagement.aspx" target="mainframe">Password Management</a>
                        </li>                       
                    </ul>
                    </li-->

                    <%--  <li><a href="#">Delivery Apps</a>
                    <ul>
                        <li><a href="LafargeDriverSubmissionReport.aspx" target="mainframe">ePOD Management</a> </li>
                    </ul>
                    </li>--%>



                    <%Else%>
                    <li><a href="#">Management</a><ul>
                        <%If PubUserid = "2059" Then%>
                        <li><a href="#">Internal</a>
                            <ul>
                                <li><a href="http://lafargewebpp.avls.com.my/PostProcessWeb.aspx" target="_blank">Process Data</a></li>
                                <li><a href="hLogin.aspx" target="mainframe">Update History</a> </li>
                                <li><a href="FuelFormulaManagement.aspx" target="mainframe">Fuel Formula Management</a> </li>

                            </ul>
                        </li>
                        <%End If%>

                        <%If role = "Admin" Then%>
                        <li><a href="#">Alerts Setting</a>
                            <ul>
                                <%If PubUserid = "1967" Or PubUserid = "3342" Then%>
                                <li><a href="LafargeInstantAlertSettingsManagement.aspx" target="mainframe">Jamming</a>
                                </li>
                                <li><a href="LafargePrivateGeofenceAlertManagement.aspx" target="mainframe">Lafarge
                                    Private Geofence</a></li>
                                <li><a href="LafargeVDM.aspx" target="mainframe">Signal Loss & V-Data</a> </li>
                                <%End If%>
                                <li><a href="SMSNotificationManagement.aspx" target="mainframe">SMS Notification</a></li>

                            </ul>
                        </li>
                        <%End If%>

                        <%--Commented On Demo--%>
                        <%-- <%If customRole = "LFSuperUser" Then%>
                        <li><a href="#">Alerts Setting</a>
                            <ul>
                                <%If PubUserid = "1967" Or PubUserid = "3342" Then%>
                                <li><a href="LafargeInstantAlertSettingsManagement.aspx" target="mainframe">Jamming</a>
                                </li>
                                <li><a href="LafargePrivateGeofenceAlertManagement.aspx" target="mainframe">Lafarge
                                    Private Geofence</a></li>
                                <li><a href="LafargeVDM.aspx" target="mainframe">Signal Loss & V-Data</a> </li>
                                <%End If%>
                                <li><a href="SMSNotificationManagement.aspx" target="mainframe">SMS Notification</a></li>

                            </ul>
                        </li>
                        <%End If%>--%>
                        <%If PubUserid = "1967" Or PubUserid = "0002" Or PubUserid = "2068" Or PubUserid = "3342" Or PubUserid = "6967" Then%>
                        <li><a href="#">Customer</a>
                            <ul>
                                <li><a href="TransporterCompany.aspx" target="mainframe">1.Create Admin</a></li>
                                <li><a href="TransporterUser.aspx" target="mainframe">2.Create Password</a> </li>
                                <li><a href="soldtomanagement.aspx" target="mainframe">3.Create Sold To Customer</a></li>
                                <li><a href="clientsoldtomanagement.aspx" target="mainframe">4.Create Password For Sold To Customer</a> </li>
                            </ul>
                        </li>
                        <%End If%>
                        <%If customRole = "transporter" Or PubUserid = "1951" Or PubUserid = "2059" Or PubUserid = "1955" Or PubUserid = "6903" Then%>
                        <li><a href="GeofenceManagementPrivate.aspx" target="mainframe">Geofence - Private</a></li>
                        <%End If%>

<%--                        <%If PubUserid = "3106" Or PubUserid = "3339" Or PubUserid = "6959" Or PubUserid = "6966" Or PubUserid = "6968" Or PubUserid = "6984" Or PubUserid = "1968" Or PubUserid = "2068" Or PubUserid = "7121" Or PubUserid = "7317" Or PubUserid = "7319" Or PubUserid = "7329" Or PubUserid = "7050" Then%>--%>
                        <li><a href="drivermanagement.aspx" title="Vehicles" target="mainframe">Driver Management</a></li>
                        <li><a href="driverroleassignmanagement.aspx" title="Vehicles" target="mainframe">Driver Role Assign Management</a></li>
<%--                        <li><a href="DriverAssignmentManagement.aspx" title="Vehicles" target="mainframe">Driver Assignment</a></li>--%>
<%--                        <%End If%>--%>
                        <%If customRole = "LFSuperUser" Then%>
                        <li><a href="GeofenceManagementLafarge.aspx" target="mainframe">Geofence - Private [For
                            YTL]</a> </li>
                        <%End If%>
                        <%If JReport = True Then%>
                        <li><a href="RoadGeofenceManagement.aspx" target="mainframe">Geofence - Road</a></li>
                        <%End If%>
                        <li><a href="InstantAlertSettingsManagement.aspx" target="mainframe">Instant Alert Management</a>
                        </li>
                        <li><a href="MapSettingsManagement.aspx" target="mainframe">Map Settings Management</a>
                        </li>
                        <li><a href="PasswordManagement.aspx" target="mainframe">Password Management</a>
                        </li>
                        <li><a href="POIManagement.aspx" target="mainframe">POI Management</a> </li>
                        <%--Commented On Demo--%>
                        <%--  <%If Not IsSpeacialUsers Then%>
                        <li><a href="TrailerManagementNew.aspx" target="mainframe">Trailer Management</a></li>
                        <li><a href="drivermanagement.aspx" title="Vehicles" target="mainframe">Driver Management</a></li>

                        <%End If %>--%>
                        <li><a href="VehiclesListNew.aspx" title="Vehicles" target="mainframe">Vehicle Management</a></li>
                        <%If PubUserid = "6968" Then%>
                        <li><a href="DriverAssignmentManagement.aspx" title="Vehicles" target="mainframe">Driver Assignment</a></li>
                        <%End If%>
                        <%If PubUserid = "1967" Or PubUserid = "3342" Then%>


                        <%End If%>
                        <%If PubUserid = "1999" Then%>
                        <li><a href="uploadfuelexcel_station.aspx" target="mainframe">Upload Fuel Receipt</a></li>
                        <%End If%>
                        <%If username = "SPYON" Or username = "SWEEHAR" Or username = "Hazlina" Or username = "SWEEHAR" Then%>
                        <li><a href="UserPlantManagement.aspx" target="mainframe">User - Plant Management</a></li>
                        <%End If%>
                    </ul>
                    </li>
                    <%End If%>
                    <%If Not PubUserid = "0000" Then%>
                    <li><a href="#">OSS</a><ul>
                        <% If username = "MAXLAI" Or role = "Admin" Or username = "WONGWAIYEE" Then%>
                        <% If Not (PubUserid = "7175" Or PubUserid = "0002") Then %>
                        <li><a href="#">OSS - Dashbaord</a>
                            <ul>
                                <li><a href="OssDashboardDaily.aspx" target="mainframe">Single Day</a> </li>
                                <li><a href="OssDashboard.aspx" target="mainframe">Last 3 Days</a> </li>
                            </ul>
                        </li>
                        <li><a href="OssEta.aspx" target="mainframe">OSS - ATA</a></li>
                        <%End If%>

                        <li><a href="SmartOss.aspx" target="mainframe">Long Waiting Time Report</a></li>
                        <%End If%>
                        <li><a href="OssManagementT.aspx" target="mainframe">OSS - Management</a></li>
                        <%If (IsSpeacialUsers Or PubUserid = "7183") And PubUserid <> "2068" Then  %>
                        <li><a href="KPIAPK.aspx" target="mainframe">KPI - Vehicles Performence</a></li>
                        <%End If %>
                        <%If ytluser Or role = "Admin" Then  %>
                        <li><a href="OssGeofenceDiffReportNew.aspx" target="mainframe">Waiting Inside Plant</a></li>
                        <%End If %>
                        <%If PubUserid = "1918" Or PubUserid = "6998" Or PubUserid = "7141" Or PubUserid = "744" Or PubUserid = "750" Or PubUserid = "752" Or PubUserid = "753" Then  %>
                        <li><a href="OssGeofenceDiffReportNew.aspx" target="mainframe">Waiting Inside Plant</a></li>
                        <%End If %>

                        <% If ytluser Then%>
                        <li><a href="PlantVehicleSearch.aspx" target="mainframe">Search Vehicles Nearby Plant</a></li>
                        <%End If%>
                        <% If username = "SPYON" Then%>
                        <li><a href="TransporterDailyActivity.aspx" target="mainframe">Transporter Activity</a></li>

                        <%End If%>

                       <%-- <% If username = "SWEEHAR" Then%>
                        <li><a href="PlantVehicleSearch.aspx" target="mainframe">Search Vehicles Nearby Plant</a></li>
                        <li><a href="TransporterDailyActivity.aspx" target="mainframe">Transporter Activity Report</a></li>
                        <%End If%>--%>
                        <%--<li><a href="ShipToCodeManagement.aspx" target="mainframe">Ship To Code Management</a></li>--%>
                        <%If IsSpeacialUsers Then  %>
                        <li><a href="GeofenceManagementPublic.aspx" target="mainframe">Public Geofence Management</a></li>
                        <%End If %>
                        <% If username = "ADMIN" Then%>
                        <li><a href="ShipToCodeManagementTest.aspx" target="mainframe">ShipToCode Management</a></li>
                        <%End If%>
                        <% If username = "JULIANAYTL" Or username = "SPYON" Then%>
                        <li><a href="DeliveryReportYTL.aspx" target="mainframe">Delivery Report</a></li>
                        <%End If%>
                        <% If ytluser Then%>
                        <li><a href="OSSReportYTL.aspx" target="mainframe">Fleet Monitoring Report</a></li>
                        <li><a href="FleetMoniterReport.aspx" target="mainframe">Tanker Availability Report</a></li>
                        <% If Not (PubUserid = "2068" Or PubUserid = "7121") Then%>
                            <li><a href="orderCreation.aspx" target="mainframe">Delivery Order Creation</a></li>
                            <li><a href="orderDeliveryMoniterReport.aspx" target="mainframe">Order Monitoring Report</a></li>
                            <li><a href="ECRLReport.aspx" target="mainframe">ECRL Report</a></li>
                            <li><a href="OssDailyDispatchReport.aspx" target="mainframe">Dispatch Report</a></li>
                        <%End If %>
                        <%End If%>
                        <% If role = "Admin" Or ytluser Then%>
                            <% If Not (PubUserid = "2068" Or PubUserid = "7121") Then%>
                                <li><a href="ossmonthreport.aspx" target="mainframe">Monthly Turck Performance Report</a></li>
                                <li><a href="ossdayreport.aspx" target="mainframe">Daily Turck Performance Report</a></li>
                            <%End If %>
                        <li><a href="ossareacodemanagement.aspx" target="mainframe">OSS Area Code Management</a></li>
                        <%End If%>
                    </ul>
                    </li>
                    <%End If%>
                    <% If PubUserid <> "7121" Then%>
                    <%If username = "SANDY" Or username = "MEI@BINTANG" Or username = "SWEEHAR" Or username = "WONGWAIYEE" Or customRole <> "transporter" Then %>
                    <li><a href="#">DMS</a><ul>
                        <!--li><a href="#">Delivery Monitoring System (DMS)</a>
                            <ul-->
                        <li><a href="DMS.aspx" target="mainframe">Daily</a></li>
                        <%--<li><a href="PTOSummaryReportBetaP.aspx" target="mainframe">Summary</a></li>--%>
                        <!--/ul>
                        </li-->
                    </ul>
                    </li>
                    <%End If %>
                    <%End If%>
                    <li><a href="#">Reports</a>
                        <ul>
                            <%If ytluser Or username = "ADMIN" Then%>
                            <li><a href="drc/pages/charts/DriversSummary.aspx" target="_blank">DRC</a> </li>
                            <%End If%>
                            <li><a href="AllVehiclesSummaryReportOdo.aspx" target="mainframe">Odometer Summary Report</a></li>
                            <% If ytluser  %>
                            <li><a href="VehicleOverspeedReport.aspx" target="mainframe">Vehicle Overspeed Report</a></li>
                            <%End If %>
                            <% If ytluser And PubUserid <> "7121"  %>
                            <li><a href="dashboard/tracking.aspx" target="mainframe">BS - Terminal Tracking</a></li>
                            <%End If %>
                            <% If Not ytluser  %>
                            <%If Not IsSpeacialUsers Then%>
                            <%--<li><a href="LafargeTrailerDetachReport.aspx" target="mainframe">Trailer Detach Report</a></li>--%>
                            <%If username = "MEI@BINTANG" Then%>
                            <li><a href="AlertNewL1.aspx" target="mainframe">Alerts report</a></li>
                            <%End If%>
                            <%--<li><a href="VehicleHarshBreakingReport.aspx" target="mainframe">Harsh Breaking</a>--%>

                            <%If username = "PCLTANKIP" Or username = "PCLTANKRW" Or username = "PCLBAGRW" Or username = "PCLBAGMKS" Then%>
                            <li><a href="WorkOrderReport.aspx" target="mainframe">Work Order Report</a> </li>
                            <%End If%>
                            <%If JReport = True Then%>
                            <li><a href="journalreport2.aspx" target="mainframe">Journal - Report</a></li>
                            <%End If%>
                            <%-- <li><a href="#">Driver - Behaviour</a>
                                <ul>
                                    <%If username = "YINSON" Then%>
                                    <li><a href="VehicleViolationDailyReportGroup.aspx" target="mainframe">Daily</a></li>
                                    <li><a href="VehicleViolationMonthlyReportGroup.aspx" target="mainframe">Monthly</a></li>
                                    <%Else%>
                                    <%If customRole = "LFSuperUser" Or username = "GISJANICE" Or username = "AMALLIANCES" Then%>
                                    <li><a href="OhsasVehicleDailySpeedratings.aspx" target="mainframe">Daily</a></li>
                                    <%Else%>
                                    <li><a href="OhsasVehicleDaily.aspx" target="mainframe">Daily</a></li>
                                    <%End If%>

                                    <%If username = "MNIZAM" Then%>
                                    <li><a href="OhsasVehicleWeeklysummary.aspx" target="mainframe">Weekly</a></li>
                                    <%End If%>
                                    <%If username = "RODAGROUP" %>
                                    <li><a href="OhsasVehicleMonthlySpeedratings.aspx" target="mainframe">Monthly (Ratings)</a></li>
                                    <%End If%>
                                    <%If username = "RODAGROUP" Then

                                          End If%>
                                    <%If username = "BINTANG3(KW/RW/SA/JB)" Or username = "BINTANG" Or username = "BINTANGTIPPER" Or username = "CHEONG@BINTANG" Or username = "MEI@BINTANG" Or username = "KELVIN@BINTANG" Or username = "SAFETY@BINTANG" Or username = "BINTANGOPR" Then%>
                                    <li><a href="OhsasVehicleDailysummary.aspx" target="mainframe">Daily (Summary)</a></li>
                                    <li><a href="OhsasVehicleWeeklysummary.aspx" target="mainframe">Weekly (Summary)</a></li>
                                    <li><a href="DBMonthlyBin.aspx" target="mainframe">Monthly</a></li>
                                    <li><a href="OhsasVehicleMonthlySpeedratings.aspx" target="mainframe">Monthly (Ratings)</a></li>
                                    <%Else%>
                                    <%If customRole = "LFSuperUser" Or customRole = "gussmann" Or username = "AMALLIANCES" Then%>
                                    <li><a href="OhsasVehicleMonthlySpeedratings.aspx" target="mainframe">Monthly</a></li>
                                    <%ElseIf username = "CHIPSENGHENG" Or username = "JASA" Or username = "NORFAISALTAN" Then%>
                                    <li><a href="DBMonthlygroup.aspx" target="mainframe">Monthly</a></li>
                                    <%Else%>
                                    <li><a href="DBMonthly.aspx" target="mainframe">Monthly</a></li>
                                    <%End If%>

                                    <%End If%>
                                    <%End If%>
                                </ul>
                            </li>--%>
                            <%--<li><a href="fuelreportdailypost.aspx" target="mainframe">Fuel - Summary</a></li>--%>
                            <%End If%>

                            <%End If %>
                            <li><a href="#">Geofence - Summary</a>
                                <ul>
                                     <%If PubUserid = "7318" Then  %>
                                    <li><a href="PrivateGeofenceSummaryReportTquarry.aspx" target="mainframe">Transporter Private</a></li>
                                    <%End If%>
                                    <%If PubUserid = "7319" Or PubUserid = "7320" Then  %>
                                    <li><a href="PrivateGeofenceSummaryReportT.aspx" target="mainframe">Transporter Private</a></li>
                                    <%End If%>
                                    <%If PubUserid = "1918" Or PubUserid = "6998" Or PubUserid = "7141" Or PubUserid = "744" Or PubUserid = "750" Or PubUserid = "752" Or PubUserid = "753" Then  %>
                                    <li><a href="LafargeGeofenceSummaryReport.aspx" target="mainframe">YTL Private</a></li>

                                    <%Else%>
                                    <%If customRole = "LFSuperUser" Then%>
                                    <li><a href="LafargeGeofenceSummaryReport.aspx" target="mainframe">YTL Private</a></li>
                                    <%End If%>
                                    <%If IsSpeacialUsers Then%>
                                    <li><a href="PrivateGeofenceSummaryReportT.aspx" target="mainframe">Transporter Private</a></li>
                                    <%End If %>
                                    <%End If%>
                                    <li><a href="PublicGeofenceSummaryReport.aspx" target="mainframe">Public</a></li>
                                </ul>
                            </li>


                            <%If PubUserid = "3342" Then%>
                            <li><a href="TransporterSummary.aspx" target="mainframe">Transporter Summary </a></li>
                            <%End If%>


                            <%If PubUserid = "722" Then%>
                            <li><a href="SpeedSummaryMonthly.aspx" target="mainframe">Speed Summary </a></li>
                            <%End If%>
                            <%If Not PubUserid = "6826" Then%>
                            <li><a href="PTOSummaryReport.aspx" target="mainframe">PTO - Summary</a></li>
                            <%End If%>
                            <%If PubUserid = "7110" Then%>
                            <li><a href="dashboard/tracking.aspx" target="mainframe">ASSB Jobs Tracking</a></li>
                            <%End If%>

                            <li><a href="#">Vehicle</a>
                                <ul>
                                    <li><a href="VehicleDailyReport.aspx" target="mainframe">Daily</a></li>
                                    <li><a href="VehicleIdlingReport.aspx" target="mainframe">Idling</a></li>
                                    <%If PubUserid = "722" Then%>
                                    <li><a href="VehicleIdlingSummaryReport.aspx" target="mainframe">Idling Summary </a>
                                    </li>
                                    <%End If%>
                                    <li><a href="VehicleLogReport.aspx" target="mainframe">Log</a></li>
                                    <%If username = "YINSON" Or username = "YINSON(YTL)" Then%>
                                    <li><a href="VehicleLogChartMap.aspx" target="mainframe">Log (Chart + Map)</a> </li>
                                    <%End If%>
                                    <%If PubUserid = "1967" Then%>
                                    <li><a href="VehicleLogReportJaming.aspx" target="mainframe">Log - Jamming</a> </li>
                                    <%End If%>
                                    <li><a href="VehicleOdometerReport.aspx" target="mainframe">Odometer</a></li>
                                    <li><a href="VehicleSpeedReport.aspx" target="mainframe">Speed</a></li>
                                    <li><a href="TrailerReport.aspx" target="mainframe">Trailer</a></li>
                                    <li><a href="VehicleTripSummaryReport.aspx" target="mainframe">Trip Summary</a></li>
                                    <%If username = "PCLBAGRW" Or username = "PCLTANKIP" Or username = "PCLTANKRW" Or username = "PERCEPTIVESS" Then%>
                                    <li><a href="VehicleUsageReportLitePost.aspx" target="mainframe">Usage</a></li>
                                    <%Else%>
                                    <li><a href="VehicleUsageReport.aspx" target="mainframe">Usage</a></li>
                                    <%End If%>
                                </ul>
                            </li>
                            <%If Not IsSpeacialUsers Then %>
                            <%--Commented For Demo--%>
                            <%--<li><a href="servicingreport2.aspx" target="mainframe">Servicing Report</a></li>--%>
                            <%If PubUserid = "3129" Then%>
                            <li><a href="SmsOutboxReport.aspx" target="mainframe">SMS Outbox</a> </li>
                            <%End If%>
                            <%If PubUserid = "6795" Then%>
                            <li><a href="SmsOutboxReportNew.aspx" target="mainframe">SMS Outbox</a> </li>
                            <%End If%>
                            <%If role = "Admin" Then%>
                            <li><a href="ivmsunitlist.aspx" target="mainframe">IVMS Unit List</a> </li>
                            <%End If %>
                            <%End If %>

                            <%If username = "ADMIN" Or username = "SPYON" Or username = "SWEEHAR" Or username = "JULIANAYTL" Then%>
                            <li><a href="Userlogreport.aspx" target="mainframe">User Login Activity</a> </li>
                            <%End If%>
                        </ul>
                    </li>
                    <li><a href="#">Charts</a><ul>
                        <li><a href="VehicleFullMovementChart.aspx" target="mainframe">Vehicle Full Movement
                            Chart</a></li>
                        <li><a href="VehicleFullMovementChartOhsasFinal.aspx" target="mainframe">Continuous
                            Driving Chart</a></li>
                        <li><a href="VehicleIdlingChart.aspx" target="mainframe">Vehicle Idling Chart</a></li>
                        <li><a href="VehicleSpeedChart.aspx" target="mainframe">Vehicle Speed Chart</a></li>
                        <li><a href="VehicleUsageChart.aspx" target="mainframe">Vehicle Usage Chart</a></li>
                    </ul>
                    </li>


                    <%If Not PubUserid = "2068" Then %> 

                        <%If IsSpeacialUsers Then %>
                        <%If customRole = "LFSuperUser" Then%>

                        <li><a href="#">Delivery Apps</a>
                            <ul>
                                <li><a href="EPodManagementNew.aspx" target="mainframe">ePOD Management</a> </li>
                                <li><a href="clientloginreport.aspx" target="mainframe">Delivery Apps Login Report</a> </li>
                                <li><a href="ClientFeedBackReport.aspx" target="mainframe">Delivery Apps FeedBack Report</a> </li>
                            </ul>
                        </li>

                        <%Else  %>
                        <li><a href="#">Delivery Apps</a>
                            <ul>
                                <li><a href="LafargeDriverSubmissionReport.aspx" target="mainframe">ePOD Management</a> </li>
                            </ul>
                        </li>
                        <%End If%>
                        <%End If %>
                    <%End If%>
                    <%If PubUserid = "0002" Or PubUserid = "1031" Or PubUserid = "1967" Or PubUserid = "1968" Or PubUserid = "2068" Or PubUserid = "7050" Or PubUserid = "7060" Or PubUserid = "7073" Or PubUserid = "7110" Or PubUserid = "7111" Or ytluser Or PubUserid = "1918" Or PubUserid = "6998" Or PubUserid = "7141" Or PubUserid = "744" Or PubUserid = "750" Or PubUserid = "752" Or PubUserid = "753" Then%>
                    <li><a href="#">Live Track</a>
                        <ul>
                            <li><a href="LiveTrack.aspx" target="mainframe">Truck Tracking</a> </li>
                            <li><a href="geofencetrack.aspx" target="mainframe">Geofence Tracking</a> </li>
                        </ul>
                    </li>

                    <%Else%>
                    <li><a href="LiveTrack.aspx" target="mainframe">Live Track</a> </li>
                    <%End If%>




                    <li><a href="WebAlerts.aspx" target="mainframe" id="atag">Web Alerts</a> </li>
                    <%If role = "AdminViewer2" Or role = "AdminViewer1" Or role = "Admin" Then%>
                    <% If Not (PubUserid = "7175" Or PubUserid = "0002") Then %>
                    <li><a href="#">Services</a>
                        <ul>
                            <li><a href="AdminServiceMangement.aspx" target="mainframe">Services Management</a></li>
                            <li><a href="ClientServiceManagementLog.aspx" target="mainframe">Services Log Management</a></li>
                        </ul>
                    </li>
                    <%End if %>
                    <%If (PubUserid = "0002") %>
                    <li><a href="#">Services</a>
                        <ul>
                            <li><a href="ClientServiceManagementLog.aspx" target="mainframe">Services Log Management</a></li>

                        </ul>
                    </li>
                    <li><a href="#">Beta</a>
                        <ul>
                            <li><a href="AllVehiclesSummaryReportA.aspx" target="mainframe">Summary [ALL]</a></li>
                        </ul>
                    </li>
                    <%End if %>


                    <%ElseIf role = "User" Or role = "SuperUser"  Then%>
                    <%--<%If PubUserid = "1967" Then%>
                    <li><a href="#">Services</a>
                        <ul>
                            <li><a href="SuperServiceMangement.aspx" target="mainframe">Service Management</a></li>
                        </ul>
                    </li>
                    <%Else%>--%>
                    <%If PubUserid <> "2068" And PubUserid <> "7121" Then %>
                    <li><a href="#">Services</a>
                        <ul>
                            <li><a href="ClientServiceManagement.aspx" target="mainframe">Service Management</a></li>
                            <li><a href="ClientServiceManagementLog.aspx" target="mainframe">Services Log Management</a></li>
                        </ul>
                    </li>
                    <%End If %>
                     <%If PubUserid = "7429" Then%>
                     <li><a href="#">Dashboard &nbsp
                    </a>
                        <ul>
                            <li><a href="dashboard/PlantDashboard.aspx" target="_blank">YTL Dashboard</a></li>
                            <li><a href="WaitingTimeMapNew.aspx" target="mainframe">Dashboard [Plant - Map]</a></li>
                        </ul>
                    </li>
                      <%End If %>


                    <%If customRole = "LFSuperUser" Then%>
                    <%-- <li><a href="WaitingTimeDashBoard.aspx" style="color: Aqua ; font-size: 14px; font-weight: bold;" >Dashboard &nbsp 
                              <img src="images/new1.gif" alt="New DashBoard" /></a>
                                </li>--%>
                    <li><a href="#">Dashboard &nbsp
                    </a>
                        <ul>
                            <%--<% If username = "JULIANAYTL" Or username = "SPYON" Or username = "SWEEHAR" Or username = "JOELCHONG" Or username = "KHOOYTL" Then%>--%>
                            <li><a href="dashboard/PlantDashboard.aspx" target="_blank">YTL Dashboard</a></li>
                            <%If Not PubUserid = "7121" Then%>
                                <li><a href="dashboard/plantreports.aspx" target="mainframe">Loading Report</a></li>
                                <li><a href="dashboard/dispatchreport.aspx" target="mainframe">Dispatch Report</a></li>
                            <%End If %>
                            <%--<%End If %>--%>
                            <%--<li><a href="WaitingTimeDashBoard.aspx">Dashboard [Customer Site]</a></li>--%>
                            <%-- <li><a href="WaitingTimeDashBoardPlant.aspx" target="mainframe">Dashboard [Plant - Chart]</a></li>--%>
                            <li><a href="WaitingTimeMapNew.aspx" target="mainframe">Dashboard [Plant - Map]</a></li>
                            <li><a href="dashboard/AttendanceReport.aspx" target="mainframe">Plant Attendance Report</a></li>
                            <%--<li><a href="dashboard/AddTrucksNoJobs.aspx" target="mainframe">Manage Attendance Report</a></li>--%>
                            
                        </ul>
                    </li>
                    <% If username = "MAXLAI" Then%>
                    <li><a href="ClientDashBoard.aspx" target="mainframe" style="color: Aqua; font-size: 14px; font-weight: bold;">I-Board</a></li>
                    <%End If %>
                    <%ElseIf customRole = "LFUser2" Then%>
                    <li><a href="#" style="color: Aqua; font-size: 14px; font-weight: bold;">Dashboard &nbsp
                        <img src="images/new1.gif" alt="New DashBoard" /></a>
                        <ul>
                            <%If PubUserid = "1918" Or PubUserid = "6998" Or PubUserid = "7141" Or PubUserid = "744" Or PubUserid = "750" Or PubUserid = "752" Or PubUserid = "753" Then  %>
                            <%--'Not necessary--%>
                            <%Else  %>
                            <li><a href="WaitingTimeDashBoardPlant.aspx" target="mainframe">Dashboard [Plant - Chart]</a></li>
                            <%End If%>

                            <li><a href="WaitingTimeMapNew.aspx" target="mainframe">Dashboard [Plant - Map]</a></li>
                        </ul>
                    </li>
                    <%End If%>

                    <%If PubUserid = "1918" Or PubUserid = "6998" Or PubUserid = "7141" Or PubUserid = "744" Or PubUserid = "750" Or PubUserid = "752" Or PubUserid = "753" Or role = "Admin" Then  %>
                    <%--'Not necessary--%>
                    <%Else  %>
                    <li><a href="#">Beta</a>
                        <ul>
                            <%If PubUserid = "6822" Or PubUserid = "6823" Or PubUserid = "1999" Or PubUserid = "7012" Or PubUserid = "7044" Or PubUserid = "7045" Or PubUserid = "6943" Then%>
                            <li><a href="RefuelSummaryReportT.aspx" target="mainframe">Refuel Summary Report</a></li>
                            <%End If%>
                            <%If PubUserid = "7012" Or PubUserid = "7044" Or PubUserid = "7045" Or PubUserid = "6943" Then%>
                            <li><a href="AllVehiclesSummaryReportAClass.aspx" target="mainframe">Summary [ALL]</a></li>
                            <%End If%>
                            <%If PubUserid = "1978" Or PubUserid = "2008" Or PubUserid = "2009" Or PubUserid = "6822" Or PubUserid = "6782" Or PubUserid = "1032" Or PubUserid = "723" Or PubUserid = "7004" Or PubUserid = "1034" Then%>
                            <li><a href="AllVehiclesSummaryReportA.aspx" target="mainframe">Summary [ALL]</a></li>
                            <%End If%>

                            <li><a href="#">Maintenance</a><ul>
                                <li><a href="DocumentsDate.aspx" target="mainframe">Documents Date Management</a></li>
                                <li><a href="ServiceManagementDecode.aspx" target="mainframe">Servicing Management</a></li>
                                <li><a href="help.aspx" target="mainframe">Help</a></li>
                            </ul>
                            </li>
                        </ul>
                    </li>
                    <%End If%>
                    <%If Not PubUserid = "2068" %>
                     <li><a href="#">Beta</a>
                        <ul>
                           <%-- <%If PubUserid = "6822" Or PubUserid = "6823" Or PubUserid = "1999" Or PubUserid = "7012" Or PubUserid = "7044" Or PubUserid = "7045" Or PubUserid = "6943" Then%>
                            <li><a href="RefuelSummaryReportT.aspx" target="mainframe">Refuel Summary Report</a></li>
                            <%End If%>
                            <%If PubUserid = "7012" Or PubUserid = "7044" Or PubUserid = "7045" Or PubUserid = "6943" Then%>
                            <li><a href="AllVehiclesSummaryReportAClass.aspx" target="mainframe">Summary [ALL]</a></li>
                            <%End If%>
                            <%If PubUserid = "1978" Or PubUserid = "2008" Or PubUserid = "2009" Or PubUserid = "6822" Or PubUserid = "6782" Or PubUserid = "1032" Or PubUserid = "723" Or PubUserid = "7004" Or PubUserid = "1034" Then%>
                            <li><a href="AllVehiclesSummaryReportA.aspx" target="mainframe">Summary [ALL]</a></li>
                            <%End If%>--%>

                            <li><a href="#">Maintenance</a><ul>
                                <li><a href="DocumentsDate.aspx" target="mainframe">Documents Date Management</a></li>
                                <li><a href="ServiceManagementDecode.aspx" target="mainframe">Servicing Management</a></li>
                                <li><a href="help.aspx" target="mainframe">Help</a></li>
                            </ul>
                            </li>
                        </ul>
                    </li>
                    <%End If %>
                    <%If checkItenery Then%>
                    <li><a href="#">Itinerary</a>
                        <ul>
                            <li><a href="ViewerManagement.aspx" target="mainframe">Viewer Management</a></li>
                            <li><a href="ViewerVehicleManagement.aspx" target="mainframe">Viewer Vehicle Management</a></li>
                        </ul>
                    </li>
                    <%End If%>
                    <%End If%>
                    <%End If%>
                    <%End If%>







                    <%If username <> "HASLIZA" Then%>
                    <li><a href="images/Lafarge Beta V1.0.pdf" target="mainframe">
                        <img
                            src="images/pdf2.png" width="25" height="25" style="vertical-align: middle;"
                            alt="User Manual" /></a> </li>
                    <%End If%>


                    <%If PubUserid = "7050" Or PubUserid = "0002" Or PubUserid = "1968" Then%>
                    <li><a href="CustomerCertificatenew.aspx" target="_blank">&nbsp;<img src="images/cert.png"
                        width="25" height="25" style="vertical-align: middle;" alt="User Manual" /></a>
                        &nbsp;</a> </li>
                    <%Else %>
                    <li><a href="CustomerCertificatenew.aspx" target="_blank">&nbsp;<img src="images/cert.png"
                        width="25" height="25" style="vertical-align: middle;" alt="User Manual" /></a>
                        &nbsp;</a> </li>
                    <%End If%>



                    <li style="float: right; padding: 0px;"><a href="Logout.aspx" style="color: #ff0000">LOGOUT</a></li>
                    <li style="float: right; color: White;"><b><a href="#" style="color: White; cursor: default;">
                        <%=username%></a></b></li>
                    <li style="float: right; color: White;"><b><a href="#" style="color: White; cursor: pointer;">
                        <img src="images/subscribe_bell.png" id="imgsubscribe" style="width: 28px; height: 28px;"
                            title="Click Here to Unsubscribe WebAlerts" alt="" onclick="unsubscribealerts(0)" />
                        <img src="images/Unsubscribe_bell.png" id="imgUnsubscribe" style="width: 28px; height: 28px;"
                            title="Click Here to Subscribe WebAlerts" alt="" onclick="unsubscribealerts(1)" />
                    </a></b></li>
                </ul>
            </td>
        </tr>
        <tr>
            <td>
                <%If PubUserid = "2012" Or PubUserid = "156" Or PubUserid = "1997" Then%>
                <iframe id="mainframe" name="mainframe" frameborder="0" scrolling="auto" marginheight="0"
                    marginwidth="0" width="100%" height="100%" src="SmartFleetsek.aspx" style="width: 100%; overflow-y: auto; overflow-x: auto"></iframe>
                <%ElseIf username = "HASLIZA"%>
                <iframe id="mainframe" name="mainframe" frameborder="0" scrolling="auto" marginheight="0"
                    marginwidth="0" width="100%" height="100%" src="CustomerCertificatenew.aspx" style="width: 100%; overflow-y: auto; overflow-x: auto"></iframe>
                <% Else%>
                <iframe id="mainframe" name="mainframe" frameborder="0" scrolling="auto" marginheight="0"
                    marginwidth="0" width="100%" height="100%" src="SmartFleetApk.aspx" style="width: 100%; overflow-y: auto; overflow-x: auto"></iframe>
                <%End If%>
            </td>
        </tr>
    </table>
    <input type="hidden" id="hiduserid" runat="server" />
    <div id="div-pass" style="padding-top: 1px; padding-right: 0px; padding-bottom: 0px; font-size: 11px; padding-left: 0px; font-family: Myriad Pro,Lucida Grande,Helvetica,Verdana,sans-serif;">
        <%--<table class ="hor-minimalist-b" frame="hsides" >
            <tr><th colspan ="2" align="left" >Security Notice: Renew of Password</th> </tr>
            <tr><td  colspan ="2">
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                Dear Valued Customer, kindly change your password before continue using our services.
                Please contact our customer support or sales personal if you encounter any issue login to our system.
             </td></tr>
                <tr><th colspan ="2" align="left" >  <p id="P1"> 安全公告：密码更新 </p>  </th> </tr>
                <tr><td  colspan ="2">
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        尊敬的客户，请您更新您的密码以继续使用我们的服务。
           如果您遇到任何问题登录到我们的系统，请联系我们的客服或销售人员。
         </td></tr>
         <tr><th colspan ="2" align="left" >Notis Keselamatan: Pertukaran Kata Laluan Baru</th> </tr>
           <tr><td  colspan ="2">
        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        Pelanggan Yang Dihormati, sila tukar kata laluan baru anda sebeloum menggunakan perkhidmatan kita. 
         Sila hubungi perkhidmatan pelanggan atau sales kami jika anda ada masalah login sistem kita.
         </td></tr>
          <tr><th colspan ="2"> 
        Contact:
        <br />
        email: cs.gtsb@g1.com.my

        phone: 03-62570509
         </th> 
         </tr>
    <tr><td>New Password</td> <td><input type ="password" class ="textbox1"  id="newpass"/> </td></tr>
      <tr><td>Conform Password</td> <td><input type ="password" class ="textbox1" id="confpass" /> </td></tr>
    </table>--%>
        <table class="hor-minimalist-b">
            <tr>
                <td>Please change your password before it expire on <b style="color: Red;">14<sup>th</sup>
                    Feb 2014.</b><br />
                    <br />
                    Thank You..
                </td>
            </tr>
        </table>
    </div>
    <div class="demo" style="display: none">
        <div id="dialog-confirm" title="Confirmation ">
            <p id="displayc">
                <span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span>
            </p>
        </div>
        <div id="dialog-alert" title="Information">
            <p id="displayp">
                <span class="ui-icon ui-icon-circle-check" style="float: left; margin: 0 7px 50px 0;"></span>
            </p>
        </div>
    </div>
</body>
</html>


