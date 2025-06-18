<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="GeofenceTrack.aspx.vb" Inherits="YTLWebApplication.GeofenceTrack" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Customer Track</title>
     <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css" integrity="sha384-1q8mTJOASx8j1Au+a5WDVnPi2lkFfwwEAa8hDDdjZlpLegxhjVME1fgjWPGmkzs7" crossorigin="anonymous">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.3.0/css/font-awesome.css" 
  rel="stylesheet"  type='text/css'>
    <link href="css/select2.min.css" rel="stylesheet" />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.3/jquery.min.js"></script>
    <script src="js/select2.full.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/js/bootstrap.min.js" integrity="sha384-0mSbJDEHialfmuBBQP6A4Qrprq5OVfW37PRR3j5ELqxss1yVqOtnepnHVP9aJ7xS" crossorigin="anonymous"></script>
    <script src="https://maps.googleapis.com/maps/api/js?v=3&client=gme-zigbeeautomation&libraries=drawing&channel=YTL"></script>
    <script src="jsfiles/richmarker.js"></script>

    <style type="text/css">
        .panel {
            margin-bottom: 0px;
        }

        .panel-body {
            padding: 1px;
        }

        .label_content {
            position: relative;
            border-radius: 5px;
            padding: 5px;
            color: #ffffff;
            background-color: red;
            font-size: 16px;
            width: 100%;
            line-height: 16px;
            text-align: center;
        }

            .label_content:after {
                content: '';
                position: absolute;
                top: 100%;
                left: 50%;
                margin-left: -8px;
                width: 0;
                height: 0;
                border-top: solid 8px red;
                border-left: solid 8px transparent;
                border-right: solid 8px transparent;
            }

        .spncls {
            color: #fff;
            margin-top: 0;
            margin-bottom: 0;
            font-size: 16px;
            color: inherit;
        }
    </style>
</head>
<body style="margin: 0px;">
     <div class="container-fluid">
         <div class="row">
             <div class="col-md-4">
                 
             </div>

             <div class ="col-md-4">
                     <div class="input-group" style="float:right">
                         <span class="input-group-addon">Customer Site</span>
                         <select id="ddlcustomerid" class="form-control" runat ="server"></select>
                     </div>
                 
             </div>
             <div class="col-md-4">
                 <input type="button" class="btn btn-primary" value="Track" onclick="TrackCustomer()"/>
                   <label class="label" id="lastupdate" style="color:black;margin-top:5px;margin-left:5px"></label>
             </div>
         </div>
         <div class="row trackedgoes">

         </div>
         </div>
</body>
    <script type="text/javascript">
        function getWindowHeight() { if (window.self && self.innerHeight) { return self.innerHeight; } if (document.documentElement && document.documentElement.clientHeight) { return document.documentElement.clientHeight; } return document.documentElement.offsetHeight; }
        var height = 300;
        $(function () {
            $("#ddlcustomerid").select2();
            height = (getWindowHeight() - 60) / 2;
            if (height < 300) {
                height = 300;
            }
        });
        var trackedgeos = new Array();
        var mapstrack = new Array();
        var tempmap;
        var markers = [];
     
        function TrackCustomer() {
        
            var currentdate = new Date();
            var datetime = "Last Sync: " + currentdate.getDate() + "/"
                + (currentdate.getMonth() + 1) + "/"
                + currentdate.getFullYear() + " @ "
                + currentdate.getHours() + ":"
                + currentdate.getMinutes() + ":"
                + currentdate.getSeconds();
            $("#lastupdate").html(datetime);
            var geoid = $("#ddlcustomerid").val();
            var nextcustid;
            if (trackedgeos.length < 4) {
                if (!trackedgeos.includes(geoid)) {
                    trackedgeos.push(geoid);

                    $(".trackedgoes").append("<div class='col-xs-12 col-sm-6 col-md-6 col-lg-6 maintrack_" + geoid + "' style='padding: 5px'><div class='panel panel-primary'><div class='panel-heading'><h3 class='panel-title'>" + $("#ddlcustomerid option:selected").html() + "  &nbsp; (<span class='fa fa-truck' title='Numer of trucks in location'></span> -> <span class='spncls' id='spn_" + geoid + "'>0</span>)<span class='fa fa-close' onclick='RemoveTrack(" + geoid + ")' title='Remove Track' style='float:right;cursor:pointer' /></h3></div><div class='panel-body'><div id='map_" + geoid + "' style='height: " + height + "px;'></div></div></div></div>");
                    var options = {
                        // center: new google.maps.LatLng(center[index].lat, center[index].lon),
                        zoom: 16,
                        mapTypeId: google.maps.MapTypeId.SATELLITE,
                        streetViewControl: false,
                        mapTypeControl: true,
                        mapTypeControlOptions: {
                            style: google.maps.MapTypeControlStyle.DROPDOWN_MENU,
                            mapTypeIds: [
                                google.maps.MapTypeId.ROADMAP,
                                google.maps.MapTypeId.SATELLITE,
                                google.maps.MapTypeId.TERRAIN
                            ]
                        }
                    };
                    var m = new google.maps.Map(document.getElementById("map_" + geoid), options);
                    mapstrack.push({ "gid": geoid, "mapobj": m });
                    var resp;
                    var lastXhr = $.getJSON("GetGeofenceGeoJson.aspx?gid=" + geoid + "&r=" + Math.random(), resp, function (data) {
                        if (data.length > 0) {
                            createPolygon(data[0][4], data[0][2], data[0][3], data[0][1], data[0][0], data[0][5], data[0][6]);
                            for (var i = 0; i < mapstrack.length; i++) {
                                if (mapstrack[i].gid == geoid) {
                                    tempmap = mapstrack[i].mapobj;
                                    break;
                                }
                            }
                            tempmap.setCenter(new google.maps.LatLng(data[0][5], data[0][6]));
                        }
                        else {
                            return false;
                        }
                    });
                    GetVehiclesinGeo();
                }
                else {
                    alert("Selected customer is already in tracked list");
                }
            }
            else {
                alert("Maximum you can track 4 locations at a time");
            }
           
            
        }
        window.setInterval(GetVehiclesinGeo, 60000);
        function GetVehiclesinGeo() {
            var resp;
            if (trackedgeos.length > 0) {
                var currentdate = new Date();
                var datetime = "Last Sync: " + currentdate.getDate() + "/"
                    + (currentdate.getMonth() + 1) + "/"
                    + currentdate.getFullYear() + " @ "
                    + currentdate.getHours() + ":"
                    + currentdate.getMinutes() + ":"
                    + currentdate.getSeconds();
                $("#lastupdate").html(datetime);
                var lastXhr = $.getJSON("GetVehiclesInGeofence.aspx?gid=" + escape(trackedgeos.toString()) + "&r=" + Math.random(), resp, function (data) {
                    if (data.length > 0) {
                        deleteMarkers();
                        for (var i = 0; i < data.length; i++) {
                            var marker = new RichMarker({
                                flat: true,
                                draggable: false,
                                position: new google.maps.LatLng(data[i][3], data[i][4]),
                                content: '<div><div class="label_content">' + data[i][1] + '</div></div>'
                            });
                            for (var j = 0; j < mapstrack.length; j++) {
                                if (mapstrack[j].gid == data[i][0]) {
                                    tempmap = mapstrack[j].mapobj;
                                    break;
                                }
                            }
                            marker.setMap(tempmap);
                            var prevcount = parseInt($("#spn_" + data[i][0] + "").html());
                            $("#spn_" + data[i][0] + "").html(prevcount + 1);
                            markers.push(marker);
                        }
                    }
                    else {
                        return false;
                    }
                });
            }
           
        }

        function createPolygon(polygonpoints, id, geofencename, at, status) {

            var pts = polygonpoints;
            var pi = new Array();
            var pt = new Array();
            for (var i = 0; i < pts.split(";").length; i++) {
                pi[i] = pts.split(";")[i].split(",")[0] + "," + pts.split(";")[i].split(",")[1];
            }

            for (var i = 0; i < pts.split(";").length - 1; i++) {
                var lat = pi[i].split(",")[1];
                var lon = pi[i].split(",")[0];
                var mpoint = new google.maps.LatLng(lat, lon);
                var gpoint = mpoint;
                pt[i] = gpoint;
            }
            for (var i = 0; i < mapstrack.length; i++) {
                if (mapstrack[i].gid == id) {
                    tempmap = mapstrack[i].mapobj;
                    break;
                }
            }
            var polygon;
            polygon = new google.maps.Polygon({
                strokeColor: '#00FF00',
                strokeOpacity: 1,
                strokeWeight: 1,
                fillColor: '#00FF00',
                fillOpacity: 0.25,
                clickable: false,
                map: tempmap,
                zIndex: 2
            });
            polygon.setPaths(pt);
            return polygon;
        }

        function deleteMarkers() {
            for (var i = 0; i < trackedgeos.length; i++) {
                $("#spn_" + trackedgeos[i] + "").html(0);
            }
            if (markers) {
                for (i in markers) {
                    markers[i].setMap(null);
                }
                markers.length = 0;
            }
        }

        function RemoveTrack(geoid) {
            trackedgeos.pop(geoid);
            for (var i = 0; i < mapstrack.length; i++) {
                if (mapstrack[i].gid == geoid) {
                    mapstrack.pop(i);
                    break;
                }
            }
            $(".maintrack_" + geoid + "").remove();
        }
    </script>
</html>
