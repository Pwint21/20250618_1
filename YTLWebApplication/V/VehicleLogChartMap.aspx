<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.VehicleLogChartMap" Codebehind="VehicleLogChartMap.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
	<title>Vehicle Log Chart Map</title>
     <style type="text/css" media="screen">
        @import "css/g1/jquery-ui-1.8.21.custom.css";
        @import "css/vehiclelogchartmap.css";
           
         </style>
         
	<script type="text/javascript" src="js/jquery-1.7.2.min.js"></script>
	<script type="text/javascript" src="js/jquery-ui-1.8.20.custom.min.js"></script>
	<script type='text/javascript' src='https://www.google.com/jsapi'></script>	
	<script type="text/javascript" src="js/infobubble-compiled.js"></script>
	<script type="text/javascript" src="js/highstock.src.js"></script>
	<script type="text/javascript" src="js/highcharts-more.js"></script>
	<script type="text/javascript" src="js/exporting.js"></script>
    <script type="text/javascript" src="js/Date.js"></script>
	<script type="text/javascript" src="https://maps.googleapis.com/maps/api/js?sensor=false"></script>
    				
	<style>
			#container {
				margin-left: 7px;
			}
			
			.date_field {
 			   display: inline;
    			width: 100px;
			}

			.speed_field {
 			   display: inline;
    		   width: 70px;
			}
	
			#report {
				position: absolute;
				top: 0px;
				margin-left: 305px;
			}
			
			.map {
				height: 260px;
				display: block;
			}

			.chart {
				min-width: 600px;
				height: 280px;
				padding: 0px;
				display: block;
				
			}		
		
			
			.table {
				height: 330px;
				height: 100%;
			}
			
			.ui-button {
			    float: left;
			}			
			
			.info_graph {
				width: 100%;
			}
			
			.progress-indicator {
			   top:0;
			   right:0;
			   width:100%;
			   height:100%;
			   position:fixed;
			   text-align:center;
			   /* IE filter */
			   filter: progid:DXImageTransform.Microsoft.Alpha(opacity=50);
			   -moz-opacity:0.5;    /* Mozilla extension */
			   -khtml-opacity:0.5;  /* Safari/Conqueror extension */
			   opacity:0.5; /* CSS3 */
			   z-index:1000;
			   background-color:white;
			   display:none;
			 }

			 .progress-indicator img {
			   margin-top:75px;
			 }			

			.gbutton {
			    padding: 0.4em 1em 0.4em 20px;
			    position: relative;
			    text-decoration: none;
			}	
	
			.gbutton span.ui-icon {
			    left: 0.2em;
			    margin: -8px 5px 0 0;
			    position: absolute;
			    top: 50%;
			}			
		
			.icon-ignition-0 {
				background-position: 0 -126px;
			}	

			.icon-ignition-1 {
				background-position: -208px -192px;
			}			
		
			.td_cell {
				text-align: center;
			}
			
			#total_odometer {
				font-weight: bold;
			}

			td {
				line-height: 12px;
			}	
	
			.limit_label {
				width: 30px;
			}	

			#data_log div div {
				overflow: hidden;
			}

			#user_lists {
				width: 280px;
			}
			
			#plate_no_lists {
				width: 280px;
			}
			
			.top-bar {
				margin-top: 5px;
				margin-bottom: 0px;
				margin-left: 5px;
			}

			.item {
    			max-width: 280px;
    		}
    		
			.item.normal.top-label {
				height: 24px;
				margin-bottom: 5px;
				margin-top: 5px;
			}

			.item.normal.time {
				width: 260px;
				height: 24px;
				padding-bottom: 0px;
				padding-top: 10px;
				
			}			
			.export-btns {
				float: right; 
				margin-top: 12px; 
				margin-right: 30px;
			}
			
			#btn-generate {
				display: block;
				text-align: center;
				width: 258px;
				font-size: 20px;
				
			}
			
			#datefrom, #dateto {
				font-size: 0.7em;
			}
			
			#datefromto {
				font-size: 0.7em;
			}

 .g1
{
    background-image: url(images/g.png);
    background-repeat: no-repeat;
    width: 16px;
    height: 16px;
    vertical-align:middle;
    display:inline-table;
}
.p1
{
    background-image: url(images/p.png);
    background-repeat: no-repeat;
    width: 16px;
    height: 16px;
    display:inline-table;
    vertical-align:middle;
}
.r1
{
    background-image: url(images/r.png);
    background-repeat: no-repeat;
    width: 13px;
    height: 13px;
    display:inline-table;
    vertical-align:middle;
}

	</style>
	<script type='text/javascript'>
	    google.load('visualization', '1', { packages: ['table', 'corechart'] });
    </script>	
</head>
<body>



	<div id="container">
		<div class="sidebar">
			<div class="header">			
				<h2>Vehicle Log Chart Map</h2>
			</div>
			<div class="filter_bar">

					<div class="item normal" style="clear:both; width: 300px; height: 210px;">
						Single Date Selection
						<span class="value" style=" padding-left: 30px;">
								<div id="datefromto"></div>
						</span>
					</div>
					<div class="item normal" style="clear:both; width: 300px;">
						Date Range Selection
						<span class="value" style=" padding-left: 30px;">

							<input type="text" id="datefrom" class="date_field" tabindex="-1" value="<%= Now.ToString("yyyy/MM/dd") %>" /> to <input type="text" id="dateto" class="date_field" tabindex="-1" value="<%= Now.ToString("yyyy/MM/dd") %>" /> 
						</span>
					</div>					
					<div class="item normal" style="clear:both;">
						User Name
						<span class="value">

							<select name="user_id" id="user_lists" runat ="server" onchange="LoadVehicles();">
                           
							</select>
						</span>
					</div>
					<div class="item normal" style="clear:both;">
						Plate Number
						<span class="value">
							<select name="plate_no" id="plate_no_lists">
							</select>
						</span>	
					</div>
<!--					
					<div class="item normal" style="clear:both; width: 160px;">
						Ignition
							<div id="ignitionset" style="width: 160px;">
							<input type="radio" name="ignition" value="-1" checked="checked" id="radio1" CHECKED/><label for="radio1">All</label>
							<input type="radio" name="ignition" value="1" id="radio2"/><label for="radio2">On Only</label>
							<input type="radio" name="ignition" value="0" id="radio3"/><label for="radio3">Off Only</label>
							</div>
					</div>	
-->					
					<div class="item normal" style="clear:both; width: 100px;">
						Address(Table only)
							<div id="addressset" style="width: 95px;">
								<input type="radio" name="show_address" value="0" checked="checked" id="add1" CHECKED/><label for="add1">Hide</label>
								<input type="radio" name="show_address" value="1" id="add2"/><label for="add2">Show</label>
							</div>	
					</div>	
					<div style="clear: both; border-bottom: 1px solid #ccc; width: 300px;">
						
					</div>
<!--											<a href="#" id="btn-generate" class="gbutton ui-state-default ui-corner-all">Generate on Screen</a>	
-->					
						<div style="text-align: left; margin-top: 15px; margin-left: 20px;">	
							<a href="#" class="gbutton ui-state-default ui-corner-all" id="print_btn"><span class="ui-icon ui-icon-print"> </span>Print</a>
							<a href="#" class="gbutton ui-state-default ui-corner-all" id="export_btn"><span class="ui-icon ui-icon-disk"></span>Export .CSV</a>
							<a href="#" class="gbutton ui-state-default ui-corner-all" id="export_kml_btn"><span class="ui-icon ui-icon-disk"></span>Export .KML</a>					
						</div>
					


			</div>
		
			
		</div>		
		<div id="report">
			<div class="info_graph">
				<div class="top-bar">
					<div class="item normal top-label">
						Speed Limit
						<select name="speed" class="speed_field" id="speed">
							<option value="0">All</option>
							<option value="60">&gt; 60</option>
							<option value="70">&gt; 70</option>					
							<option value="80">&gt; 80</option>
							<option value="90">&gt; 90</option>
							<option value="100">&gt; 100</option>
						</select>
					</div>
					<div class="item normal top-label">
						Idle Time
						<select name="idle_time" class="speed_field" id="idle_time">
							<option value="10">10 mins</option>
							<option value="20">20 mins</option>
							<option value="30">30 mins</option>
							<option value="60">1 hour</option>
						</select>
					</div>					
					<div class="item normal top-label time">
						Time : <label id="begin_time">&nbsp;</label> <strong>-</strong> <label id="end_time">&nbsp;</label>
					</div>
					<span class="field_total_odometer ui-state-default ui-corner-all" style="display:none">Total Odometer:</span> <label id="total_odometer" style="display:none">&nbsp;</label>

					<div class="export-btns">
					</div>		
				</div>

				<div class="chart info_box" id="chart_box" style="clear: both;"> <h2>Chart</h2></div>
						<div>
							<div class="map info_box" id="map_box"> <h2>Map</h2> </div>
				  		</div>
  						<div>
  							<div class="table info_box" id="data_log"> <h2>Data Log</h2> </div>
  						</div>
			</div>
			<div class="footer">
				Copyright <% =Year(Now) %> for Gussmann Technologies Sdn. Bhd. All rights reserved.
			</div>		
		</div>
	</div>
	<div id="filter_dialog" title="Report Filter">
	</div>	
	<div id="container" style="min-width: 400px; height: 400px; margin: 0 auto"></div>
	
    <form id="googleearthform" method="post" action="">
    </form>		
		
	<div class="progress-indicator" id="loading">
		<img src="images/ajax-loader.gif">
	</div>
    <script type="text/javascript" >
       
            function LoadVehicles() {
                $("#plate_no_lists").attr("disabled", "disabled");
                if ($('#user_lists').val() == "0") {
                    $('#plate_no_lists').empty().append('<option selected="selected" value="0">Please select username</option>');
                }
                else {
                    $('#plate_no_lists').empty().append('<option selected="selected" value="0">Loading...</option>');
                    $.ajax({
                        type: "POST",
                        url: "GetChartVehicle.aspx?u=" + $('#user_lists').val(),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: OnLoadVehicles,
                        failure: function (response) {
                            alert(response.d);
                        }
                    });
                }
            }
            function OnLoadVehicles(response) {
                PopulateControl(response, $("#plate_no_lists"));
            }

            function PopulateControl(list, control) {
                if (list.length > 0) {
                    control.removeAttr("disabled");
                    control.empty().append('<option selected="selected" value="0">-- SELECT VEHICLE --</option>');
                    for (var i = 0; i < list.length; i++) {
                        control.append('<option value="' + list[i] + '">' + list[i] + '</option>');
                    }
                }
                else {
                    control.empty().append('<option selected="selected" value="0">No vehicles<option>');
                }
            } 
       
    </script>

	
	<script>
	    var report_data;
	    var map;
	    var chart;
	    var line_route;
	    var prev_way_pts = [];
	    var prev_table_data = null;
	    var refresh_table = false;
	    var refresh_map = false;
	    var glb_table_data;
	    var gmap_js_load = false;
	    var glb_chart_data;
	    var gtime_range = [];
	    var speed = 0;
	    var show_address = 0;
	    var prev_marker;
	    var user_id;
	    var plate_no;
	    var highchart;
	    var infowindow;
	    var selecting_data;
	    var singleDateSelection = true;
	    var geocoder;
	    var ignition = -1;
	    var rows_data = {};
	    var platedate = [];
	    var pto;

	    $(function () {
	        var height = $(window).height();
	        $("#data_log").css("height", 450);

	        var height = $(window).height();
	        var width = $(window).width();
	        //$("#report").css("height", height - 90);
	        $("#report").css("width", width - 320);
	        $(window).resize(function () {
	            var height = $(window).height();
	            var width = $(window).width();
	            //$("#report").css("height", height - 90);
	            $("#report").css("width", width - 320);

	        });

	        /*	    $( "#accordion" ).accordion({ heightStyle: "fill", change: function( event, ui ) {
	        if($("#data_log").is(":visible")) { 
	        if(refresh_table)
	        drawTable();
	        } else {
	        if(refresh_map)
	        drawMap();
	        }
	    	
	        }});	
	        */
	        $("#data_log").wrap('<div/>').css({ 'overflow': 'hidden' }).parent().css({ 'display': 'block',
	            'overflow': 'hidden',
	            'height': function () { return $('#data_log', this).height(); },
	            'width': '100%',
	            'paddingBottom': '18px',
	            'paddingBottom': '15px'
	        }).resizable({ minHeight: 330, handles: 's' }).find('#data_log').css({ 'overflow-y': 'auto', 'height': '100%', 'weight': '100%' });





	        $('#export_btn').click(function () {

	            begin_time = $.datepicker.formatDate('yy/mm/dd', gtime_range[0]) + " " + gtime_range[0].getHours() + ":" + gtime_range[0].getMinutes() + ":" + gtime_range[0].getSeconds();
	            end_time = $.datepicker.formatDate('yy/mm/dd', gtime_range[1]) + " " + gtime_range[1].getHours() + ":" + gtime_range[1].getMinutes() + ":" + gtime_range[1].getSeconds();
	            window.location = "VehicleLogData.aspx?user_id=" + user_id + "&plate_no=" + plate_no + "&interval=0&begin_time=" + begin_time + "&end_time=" + end_time + "&ignition=" + ignition + "&show_address=" + show_address + "&speed=" + speed + "&format=csv";
	            return;
	        });

	        $('#print_btn').click(function () {

	            begin_time = $.datepicker.formatDate('yy/mm/dd', gtime_range[0]) + " " + gtime_range[0].getHours() + ":" + gtime_range[0].getMinutes() + ":" + gtime_range[0].getSeconds();
	            end_time = $.datepicker.formatDate('yy/mm/dd', gtime_range[1]) + " " + gtime_range[1].getHours() + ":" + gtime_range[1].getMinutes() + ":" + gtime_range[1].getSeconds();

	            window.open("VehicleLogPrint.aspx?user_id=" + user_id + "&plate_no=" + plate_no + "&interval=0&begin_time=" + begin_time + "&end_time=" + end_time + "&ignition=" + ignition + "&show_address=" + show_address + "&speed=" + speed + "&pto=" + pto, "Print");
	            return false;
	        });

	        $('#export_kml_btn').click(function () {
	            if (plate_no)
	                ShowGoogleEarth();
	            return false;
	        });



	        $('#speed').change(function () {
	            var highchart_line = [];
	            var highchart_speedlimitline = [];
	            $("#speed option:selected").each(function () {
	                $("#loading").show();
	                speed = parseInt($(this).val());
	                //filter for table data
	                var tbl_data = prepareTableData();
	                var check_timerange = (gtime_range.length == 2);
	                var start_time = null;
	                var end_time = null;
	                if (check_timerange) {
	                    start_time = gtime_range[0];
	                    end_time = gtime_range[1];
	                }

	                $.each(report_data, function (idx, value) {
	                 
	                    if (check_timerange) {
	                        if (!(start_time <= value.datetime && value.datetime <= end_time))
	                            return;
	                    }
	                    if (value.speed >= speed) {
	                       
	                        var row = [value.ignition, value.datetime, value.gpsav, value.speed, value.odometer, value.lat, value.lon];
	                        if (pto == 1) {
	                            if (value.pto == "1")
	                                row.push("On");
	                            else if (value.pto == "0")
	                                row.push("Off");
	                        }
	                        if (show_address == 1) {
	                            row.push(value.address);
	                        }

	                        row.push("");
	                        tbl_data.addRow(row);
	                    }
	                    //					highchart_speedlimitline.push({x: value.datetime, y: speed});		
	                });
	                if (tbl_data.getNumberOfRows() == 0) {
	                    var row = ["", null, 'No Record', 0, 0, null, null, ""];
	                    if (pto == 1) {
	                        row.push(null);
	                    }
	                    if (show_address == 1) {
	                        row.push(null);
	                    }
	                    tbl_data.addRow(row);
	                }
	                drawTable(tbl_data);

	                //draw limit line	

	                highchart.yAxis[0].removePlotLine();
	                highchart.yAxis[0].addPlotLine({
	                    value: speed,
	                    width: 2,
	                    color: 'red',
	                    dashStyle: 'dash',
	                    label: {
	                        text: 'Speed Limit ' + speed + "km/h",
	                        align: 'center'
	                    }
	                });

	                //				highchart.series[1].setData(highchart_speedlimitline, true);



	                $("#loading").hide();
	                return false;
	            });
	        });

	        // Dialog Link

	        //		$("#user_lists").change(function() {
	        //			selected_opt = $("#user_lists option:selected").val();
	        //			$("#plate_no_lists").get(0).options.length = 0;
	        //			jQuery.getJSON("PlatesData.aspx?userid=" + selected_opt, function(data){
	        //				$.each(data, function(idx, row) {
	        //					$("#plate_no_lists").get(0).options.add(new Option(row, row));
	        //				});
	        //				generate();	
	        //			});			
	        //		});



	        $("#plate_no_lists").change(function () {
	            generate();
	        });

	        $("input[name=ignition]").change(function () {

	            $.each($("input[name=ignition]"), function (index, value) {
	                if ($(value).attr("checked") == "checked") {
	                    ignition = $(value).val();
	                }
	            });


	            generate();
	        });

	        $("input[name=show_address]").change(function (elm) {
	            show_address = show_address == 0 ? 1 : 0;
	            if (show_address == 1) {

	                if (confirm("This process will take more than 2 mins to load. Are you sure?")) {
	                    generate();
	                } else {
	                    $("#add1").attr("checked", "checked");
	                    show_address = 0;
	                    return false;
	                }
	            }
	            else {
	                if (confirm("This process will take more than 2 mins to load. Are you sure?")) {
	                    generate();
	                } else {
	                    $("#add1").attr("checked", "checked");
	                    show_address = 1;
	                    return false;
	                }
	            }

	        });


	        $("#idle_time").change(function () {
	            generate();
	        });

	        $('#filter_link, ul#icons li, #print_btn, #export_btn, #export_kml_btn').hover(
			function () { $(this).addClass('ui-state-hover'); },
			function () { $(this).removeClass('ui-state-hover'); }
		);

	        $("#ignitionset").buttonset();
	        $("#addressset").buttonset();


	        /*		$('.ui-accordion-header').click(function() {
	        $(this).next().toggle('fast', function() {
	        var parent_elm = $(this).parent().parent();
	        var chart_elm = $("#chart_box");
	        if(parent_elm.width() == 450) {
	        chart_elm.offset({ top: 78, left: 60 });
	        chart_elm.width($(window).width() - 80);
	        parent_elm.width(30);
	        highchart.redraw();
	        } else {
	        chart_elm.offset({ top: 78, left: 480 });
	        chart_elm.width($(window).width() - 500);
	        parent_elm.width(450);
	        highchart.redraw();
	        }		
	        drawMap();
	        });
	        $("#map_icon").toggleClass(function() {
	        if ($(this).hasClass('ui-icon-triangle-1-s')) {
	        $(this).removeClass('ui-icon-triangle-1-s');
	        return 'ui-icon-triangle-1-e';
	        } else {
	        $(this).removeClass('ui-icon-triangle-1-e');
	        return 'ui-icon-triangle-1-s';
	        }
	        });
				
	        return false;
	        }).next().hide();			

	        */

	        var dates = $("#datefrom, #dateto").datepicker({
	            defaultDate: "today",
	            dateFormat: "yy/mm/dd",
	            changeMonth: false,
	            numberOfMonths: 2,
	            maxDate: "today",
	            onSelect: function (selectedDate) {
	                singleDateSelection = false;
	                var option = this.id == "datefrom" ? "minDate" : "maxDate",
						instance = $(this).data("datepicker"),
						date = $.datepicker.parseDate(
							instance.settings.dateFormat ||
							$.datepicker._defaults.dateFormat,
							selectedDate, instance.settings);
	                new_date = $.datepicker.parseDate(
							instance.settings.dateFormat ||
							$.datepicker._defaults.dateFormat,
							selectedDate, instance.settings);
	                console.log(dates.not(this));
	                console.log(option);
	                console.log(date);


	                if (option == "minDate") {
	                    new_date.setDate(new_date.getDate() + 3);
	                    dates.not(this).datepicker("option", "maxDate", new_date);
	                    dates.not(this).val(selectedDate);
	                    dates.not(this).datepicker("option", option, date);
	                }

	                if (dates.not(this).val() == "")
	                    dates.not(this).val(selectedDate);
	                if (option == "maxDate")
	                    generate();
	            }
	        });




	        $("#datefromto").datepicker({
	            defaultDate: "today",
	            dateFormat: "yy/mm/dd",
	            changeMonth: false,
	            numberOfMonths: 1,
	            maxDate: "today",
	            onSelect: function (selectedDate) {
	                dates.val(selectedDate);
	                var instance = $(this).data("datepicker");
	                var start_date = $.datepicker.parseDate(instance.settings.dateFormat ||
							$.datepicker._defaults.dateFormat,
							selectedDate, instance.settings);
	                var end_date = $.datepicker.parseDate(instance.settings.dateFormat ||
							$.datepicker._defaults.dateFormat,
							selectedDate, instance.settings);


	                end_date.setDate(end_date.getDate() + 3);
	                $("#dateto").datepicker("option", "minDate", start_date);
	                $("#dateto").datepicker("option", "maxDate", end_date);

	                singleDateSelection = true;
	                generate();
	            }
	        });

	        /*		function prepareLineData() {
	        var line_data = new google.visualization.DataTable();
	        line_data.addColumn('datetime', 'Time');
	        line_data.addColumn('number', 'Limit (km/h)');
	        line_data.addColumn('number', 'Speed (km/h)');
			
			
	        return line_data;
	        }
	        */
	        function prepareTableData() {
	            var tbl_data = new google.visualization.DataTable();

	            tbl_data.addColumn('string', 'Ignition');
	            tbl_data.addColumn('datetime', 'Time');
	            tbl_data.addColumn('string', 'GPS AV');
	            tbl_data.addColumn('number', 'Speed (km/h)');
	            tbl_data.addColumn('number', 'Odometer');
	            tbl_data.addColumn('string', 'Latitude');
	            tbl_data.addColumn('string', 'Longitude');
	            if (pto == 1) {
	                tbl_data.addColumn('string', 'PTO');
	            }
	            if (show_address == 1) {
	                tbl_data.addColumn('string', 'Address');
	            }
	            tbl_data.addColumn('string', 'Links');

	            return tbl_data;
	        }
	        function formatDate(date) {
	            var d1 = Date.parse(date);
	            var d = new Date(d1);
	            var curr_date = d.getDate();
	            if (curr_date.toString().length == 1) {
	                curr_date = "0" + curr_date;
	            }
	            var curr_month = d.getMonth();
	            curr_month++;
	            if (curr_month.toString().length == 1) {
	                curr_month = "0" + curr_month;
	            }
	            var curr_year = d.getFullYear();
	            var hours = d.getHours();
	            if (hours.toString().length == 1) {
	                hours = "0" + hours;
	            }
	            var min = d.getMinutes();
	            if (min.toString().length == 1) {
	                min = "0" + min;
	            }
	            var sec = d.getSeconds();
	            if (sec.toString().length == 1) {
	                sec = "0" + sec;
	            }
	            return curr_year + "-" + curr_month + "-" + curr_date + " " + hours + ":" + min + ":" + sec;
	        }

	        function JSONDateWithTime(dateStr) {
	            jsonDate = dateStr;
	            var d = new Date(parseInt(jsonDate.substring(6)));
	            var m, day;
	            m = d.getMonth() + 1;
	            if (m < 10)
	                m = '0' + m
	            if (d.getDate() < 10)
	                day = '0' + d.getDate()
	            else
	                day = d.getDate();
	            var formattedDate = m + "/" + day + "/" + d.getFullYear();
	            var hours = (d.getHours() < 10) ? "0" + d.getHours() : d.getHours();
	            var minutes = (d.getMinutes() < 10) ? "0" + d.getMinutes() : d.getMinutes();
	            var formattedTime = hours + ":" + minutes + ":" + d.getSeconds();
	            formattedDate = formattedDate + " " + formattedTime;
	            return formattedDate;
	        }

	        function populatedata(data, use_hash) {
	            report_data = data;
	            var highchart_line = [];
	            var highchart_speedlimitline = [];
	            var highchart_odometerline = [];
	            var highchart_ignition = [];
	            //var line_data = prepareLineData();

	            var tbl_data = prepareTableData();
	            gtime_range = [];
	            map = undefined;
	            var way_pts = [];
	            var min_odometer = 0;
	            var prev_odometer = 0;
	            var start_time = null;
	            var end_time = null;
	            var ignition_off_start_time = null;
	            var plot_start_time = null;
	            var orig_datetime = null;
	            var test_data = [];

	            $.each(data, function (index, value) {
	                if (min_odometer == 0)
	                    min_odometer = value.odometer;
	                //remove "/" from "/Date(12312312312)/"
	                orig_datetime = value.datetime;
	                datetime = value.datetime;
	                //	                value.datetime = eval("new " + datetime);
	                value.datetime = Date.fromString(datetime, { order: 'MDY', strict: false });

	                if (value.odometer < prev_odometer) {
	                    value.odometer = prev_odometer;
	                }

	                //line_data.addRow([value.datetime, speed, value.speed]);			
	                prev_odometer = value.odometer;
	                if (start_time == null)
	                    start_time = value.datetime;
	                end_time = value.datetime;

	                //	                speed = parseFloat(value.speed) == value.speed ? parseFloat(value.speed.toFixed(2)) : 0.0;
	                speed = value.speed;
	                //	                odometer = parseFloat(value.odometer) == value.odometer ? parseFloat(value.odometer.toFixed(2)) : 0.0;
	                odometer = value.odometer;
	                var row = [value.ignition, value.datetime, value.gpsav, speed, odometer, value.lat, value.lon];
	                if (pto == 1) {
	                    if (value.pto == "1")
	                        row.push("On");
	                    else if (value.pto == "0")
	                        row.push("Off");
	                }
	                if (show_address == 1) {
	                    row.push(value.address);
	                }

	                if (value.ignition == "Off") {
	                    if (ignition_off_start_time == null) {
	                        ignition_off_start_time = value.datetime;
	                    }
	                    plot_start_time = null;
	                }
	                if (value.ignition == "On") {
	                    if (ignition_off_start_time != null) {
	                        var band = {
	                            from: ignition_off_start_time,
	                            to: value.datetime,
	                            color: '#aaa'
	                        };
	                        //highchart.xAxis[0].addPlotBand(band);
	                    }
	                    ignition_off_start_time = null;
	                }
	                if (value.ignition == "On" && value.speed == 0) {


	                    if (plot_start_time == null) {
	                        plot_start_time = value.datetime;
	                    }
	                } else if (value.ignition == "On" && value.speed > 0) {
	                    if (plot_start_time != null) {
	                        var idle_limit = $("#idle_time option:selected").val(); //10 mins
	                        var idle_time = ((value.datetime - plot_start_time) / 60 / 1000);
	                        var idle_more = idle_time > idle_limit;
	                        if (idle_more) {
	                            var band = {
	                                from: plot_start_time,
	                                to: value.datetime,
	                                color: idle_time > (idle_limit * 2) ? '#Fcc' : '#FEE',
	                                label: {
	                                    rotation: 45,
	                                    text: "idle",
	                                    verticalAlign: "top"
	                                }
	                            };
	                            highchart.xAxis[0].addPlotBand(band);
	                        }
	                        plot_start_time = null;
	                    }
	                }
	                if (use_hash)
	                    highchart_line.push({ x: value.datetime, y: speed });
	                else
	                    highchart_line.push([value.datetime, speed]);
	                //console.log(Date.parse(value.datetime).toString());
	                //rows_data[Date.parse(value.datetime).toString()] = {y: speed, odometer: value.odometer, gpsav: value.gpsav, ignition: value.ignition, lat: value.lat, lon: value.lon }

	                //highchart_line.push({x: value.datetime, y: speed});
	                //highchart_line.push([value.datetime, speed, value.odometer, value.gpsav, value.ignition, value.lat, value.lon]);
	                //test_data.push([value.datetime, speed]);

	                if (use_hash)
	                    highchart_odometerline.push({ x: value.datetime, y: odometer });
	                else
	                    highchart_odometerline.push([value.datetime, odometer]);

	                //highchart_odometerline.push({x: value.datetime, y: odometer});
	                row.push("");
	                tbl_data.addRow(row);

	                way_pts.push([value.lat, value.lon, value.speed, value.ignition]);
	            });
	            gtime_range = [start_time, end_time];
	            highchart.series[0].setData(highchart_line, false);
	            highchart.series[1].setData(highchart_odometerline, false);
	            highchart.redraw();

	            drawTable(tbl_data);
	            drawMap(way_pts);

	        }



	        function drawMap(way_pts) {
	            if (typeof way_pts == 'undefined') {
	                way_pts = prev_way_pts;
	                refresh_map = false;
	            } else {
	                prev_way_pts = way_pts;
	                refresh_map = true;
	            }
	            if (typeof way_pts == 'undefined' || way_pts.length == 0) {
	                return false;
	            }

	            if ($("#map_box").is(":visible")) {
	                //lazy load map js

	                if (typeof line_route != 'undefined')
	                    line_route.setMap(null);
	                if (typeof prev_marker != 'undefined')
	                    prev_marker.setMap(null);
	                if (typeof map == 'undefined') {
	                    map = new google.maps.Map(document.getElementById('map_box'), { zoom: 13, streetViewControl: false, mapTypeId: google.maps.MapTypeId.ROADMAP });

	                    if (typeof infowindow == 'undefined') {
	                        infowindow = new InfoBubble({
	                            padding: 10,
	                            borderRadius: 4,
	                            arrowSize: 20,
	                            arrowPosition: 50,
	                            arrowStyle: 0,
	                            minHeight: 125,
	                            minWidth: 320,
	                            content: "<table class=info>" +
												"<tr><th>Speed</th><td id='speed_td'></td><th>Odometer</th><td id='odometer_td'></td></tr>" +
												"<tr><th>GPSAV</th><td id='gpsav_td'></td><th>Ignition</th><td id='ignition_td'></td></tr>" +
												"<tr><th>Address</th><td id='info_address_td' colspan='3'></td></tr>" +
												"</table>"
	                        });

	                        google.maps.event.addListener(infowindow, 'domready', function () {
	                            if ($("#speed_td").length == 0) {
	                                //delay 500 to fix bug on Chrome where popup shown but id element still not found
	                                setTimeout("prepare_info_window(selecting_data)", 800);
	                            } else {
	                                prepare_info_window(selecting_data);
	                            }
	                        });

	                    }
	                }
	                var lineSymbol = {
	                    path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
	                    scale: 2,
	                    strokeColor: '#0000FF'
	                };

	                line_route = new google.maps.Polyline({ strokeWeight: 3, strokeColor: "#0000FF", icons: [{
	                    icon: lineSymbol,
	                    offset: '0%',
	                    repeat: '500px'
	                }]
	                });
	                animatePath(line_route);
	                var glatlags = [];
	                $.each(way_pts, function (idx, value) {
	                    latlng = new google.maps.LatLng(value[0], value[1]);
	                    glatlags.push(latlng);
	                });
	                line_route.setPath(glatlags);
	                line_route.setMap(map);
	                fit_map_to_points(glatlags);
	            }

	        }

	        function animatePath(line) {
	            var count = 0;
	            offsetId = window.setInterval(function () {
	                count = (count + 1) % 200;

	                var icons = line.get('icons');
	                icons[0].offset = (count / 2) + '%';
	                line.set('icons', icons);
	            }, 200);
	        }

	        function ignition_format(dataTable, rowNum) {
	            if (rowNum > 0) {
	                var ignition = dataTable.getValue(rowNum, 0);
	                if (ignition)
	                    return '<span class="ui-icon ui-icon-key"></span>';
	            }
	            return '';
	        }


	        function drawTable(table_data) {
	            if (typeof table_data == 'undefined') {
	                table_data = prev_table_data;
	                refresh_table = false;
	            } else {
	                refresh_table = true;
	                prev_table_data = table_data;
	            }
	            var i = 0;
	            if ($("#data_log").is(":visible")) {

	                var formatter_date = new google.visualization.DateFormat({ pattern: "yyyy-MM-dd HH:mm:ss" });
	                formatter_date.format(table_data, 1);
	                var formatter_links = new google.visualization.PatternFormat("<a href='https://maps.google.com/maps?f=q&hl=en&q={0}+{1}&om=1&t=k' target='_blank'><img style='width: 20px; height: 15px;' src='images/googlemaps1.gif' title='View map in Google Maps'/></a>   <a href='GoogleEarthMaps.aspx?x={1}&y={0}'><img style='width: 15px; height: 15px;' src='images/googleearth1.gif' title='View map in GoogleEarth'/></a>");
	                if (pto == 1)
	                    formatter_links.format(table_data, [5, 6], (show_address == 1 ? 9 : 8));
	                else if (pto == 0)
	                    formatter_links.format(table_data, [5, 6], (show_address == 1 ? 8 : 7));
	                var formatter_speed = new google.visualization.ColorFormat();
	                formatter_speed.addGradientRange(80, 200, 'black', "#FFCCCC", "#FF0000");
	                formatter_speed.format(table_data, 3); // Apply formatter to second column			

	                var formatter_vehicle = new google.visualization.NumberFormat();
	                formatter_vehicle.format(table_data, 3); // Apply formatter to second column			
	                formatter_vehicle.format(table_data, 4); // Apply formatter to second column			

	                glb_table_data = table_data;
	                var table = new google.visualization.Table(document.getElementById('data_log'));

	                var view = new google.visualization.DataView(table_data);
	                view.setColumns([0, { calc: ignition_format, type: 'string', label: 'ignition'}]);


	                table.draw(table_data, { width: '100%', showRowNumber: true, allowHtml: true, 'cssClassNames': { tableCell: 'td_cell'} });
	            }

	            //			calculateOdometer(table_data);


	            /*			google.visualization.events.addListener(table, 'select',  function() {
	            if(typeof map != 'undefined') {
	            var selection = table.getSelection();
	            var message = '';
	            $.each(selection, function(i, item) {
	            if(item.row != null) {
	            if(typeof prev_marker != 'undefined')
	            prev_marker.setMap(null);
	            var lat = table_data.getFormattedValue(item.row, 5);
	            var lon = table_data.getFormattedValue(item.row, 6);
	            var geopos = new google.maps.LatLng(lat, lon);
	            map.setCenter(geopos);
	            map.setZoom(16);
	            prev_marker = new google.maps.Marker({
	            position: geopos, 
	            map: map,
	            animation: google.maps.Animation.BOUNCE,
	            title:"Speed : " + table_data.getFormattedValue(item.row, 3) + " km/h  Ignition : " + (table_data.getFormattedValue(item.row, 0) ? "On" : "Off")
	            });			
	            }
	            });
	            }
	            });
	            */

	        }

	        function fit_map_to_points(points) {
	            if (points.length > 0) {
	                var sw_lat = points[0].lat();
	                var sw_lng = points[0].lng();
	                var ne_lat = points[0].lat();
	                var ne_lng = points[0].lng();

	                for (var k = 1; k < points.length; k++) {
	                    if (points[k].lat() < sw_lat) { sw_lat = points[k].lat(); }
	                    if (points[k].lng() < sw_lng) { sw_lng = points[k].lng(); }
	                    if (points[k].lat() > ne_lat) { ne_lat = points[k].lat(); }
	                    if (points[k].lng() > ne_lng) { ne_lng = points[k].lng(); }
	                };

	                var single_start = new google.maps.LatLng(sw_lat, sw_lng);
	                var single_end = new google.maps.LatLng(ne_lat, ne_lng);
	                var route_bounds = new google.maps.LatLngBounds(single_start, single_end); ;
	                map.fitBounds(route_bounds);
	            }
	        }


	        //load user lists
	        //		if($("#user_lists").get(0).options.length == 0) {
	        //			jQuery.getJSON("UsersData.aspx", function(data){
	        //				$.each(data, function(idx, row) {
	        //					jQuery("#user_lists").get(0).options.add(new Option(row.name, row.user_id));
	        //				});
	        //				if($("#user_lists").get(0).options.length > 0) {
	        //					$("#user_lists").change();				
	        //				}
	        //			
	        //			});			
	        //		}		



	        function generate() {
	            initChart();

	            //$(this).attr("disabled", "true");
	            user_id = $("select[id=user_lists]").val();
	            if ($("select[name=plate_no]").val() != null) {
	                platedate = $("select[name=plate_no]").val().split("-");
	                plate_no = platedate[0];
	                pto = platedate[1];
	            }
	            var date_from = null;
	            var date_to = null;

	            if (singleDateSelection) {
	                date_from = $("#datefromto").val();
	                date_to = date_from;
	            } else {
	                date_from = $("#datefrom").val();
	                date_to = $("#dateto").val();

	            }
	            var use_hash = (date_from == date_to);



	            //			var date_from = $("#datefromto").val();
	            //			var date_to = $("#dateto").val();
	            //			var date_to = date_from;
	            if (date_from == "" || date_to == "") {
	                alert("Please select a date range");
	                return false;
	            }
	            $("#loading").show();
	            //+ " - " + $("input[name='ignition']:checked").length + " - " + $("input[name='ignition']:CHECKED").length+ " - " + $("input[CHECKED]").length
	            //var ignition = $("input[name='ignition'][CHECKED]").val();
	            showDateRange($.datepicker.parseDate('yy/mm/dd', date_from), $.datepicker.parseDate('yy/mm/dd', date_to));

	            //$("#ignition").text(ignition == "-1" ? "All" : (ignition == "1" ? "On Only" : "Off Only"));			

	            speed = 0;
	            $("#speed").val(0);

	            date_from = $.datepicker.formatDate('yy/mm/dd', $.datepicker.parseDate('yy/mm/dd', date_from));
	            date_to = $.datepicker.formatDate('yy/mm/dd', $.datepicker.parseDate('yy/mm/dd', date_to));
	            date_from += " 00:00:00";
	            date_to += " 23:59:59";
	            jQuery.getJSON("VehicleLogData.aspx?user_id=" + user_id + "&plate_no=" + plate_no + "&interval=0&begin_time=" + date_from + "&end_time=" + date_to + "&ignition=" + ignition + "&show_address=" + show_address + "&speed=0&pto=" + pto + "", function (data) {
	                if (data.length == 0)
	                    alert("No Data Available. Please provide another filter selections");
	                else {

	                    populatedata(data, use_hash);
	                    //highchart.xAxis[0].setExtremes(Date.parse(date_from + " 00:00:00"), Date.parse(date_to + " 23:59:59"), false);
	                }
	                $("#loading").hide();
	            });
	            return false;
	        }

	        function initChart() {

	            highchart = new Highcharts.StockChart({
	                chart: {
	                    renderTo: 'chart_box',
	                    spacingRight: 20,
	                    spacingTop: 6
	                },
	                global: {
	                    useUTC: false
	                },
	                exporting: { buttons: { exportButton: { enabled: false}} },
	                title: {
	                    text: ''
	                },
	                xAxis: {
	                    ordinal: false,
	                    showFirstLabel: true,
	                    showLastLabel: true,
	                    tickInterval: 3600 * 1000 * 2,
	                    type: 'datetime',
	                    dateTimeLabelFormats: {
	                        second: '%I:%M:%S%p',
	                        minute: '%I:%M%p',
	                        hour: '%I:%M%p',
	                        day: '%m-%d'
	                    },
	                    labels: {
	                        staggerLines: 2,
	                        formatter: function () {
	                            //some cleanup may be required, but this is the general form of the solution
	                            if (this.value) {
	                                date = new Date(this.value)
	                                if (date.getHours() == 0)
	                                    return date.getDate() + "/" + date.getMonth()
	                                else {
	                                    if (date.getHours() > 12)
	                                        return date.getHours() - 12 + "pm";
	                                    else
	                                        return date.getHours() + "am";
	                                }
	                            }
	                        }
	                    },
	                    events: {
	                        setExtremes: function (event) {
	                            highchart.showLoading();
	                        },
	                        afterSetExtremes: function (event) {
	                            if (event.userMin == event.dataMin && event.userMax == event.dataMax) {
	                                highchart.hideLoading();
	                                return true;
	                            }
	                            var tbl_data = prepareTableData();
	                            var way_pts = [];
	                            var event_min = event.userMin;
	                            var event_max = event.userMax;
	                            var start_time, end_time;
	                            if (isNaN(event_min) || isNaN(event_max)) {
	                                start_time = Date.parse($("#datefrom").val() + " 00:00:00");
	                                end_time = Date.parse($("#datefrom").val() + " 23:59:59");

	                                //return true;
	                            } else {
	                                start_time = parseFloat(event_min) == event_min ? new Date(event_min) : new Date(Date.parse(event_min.substring(0, 33)));
	                                end_time = parseFloat(event_max) == event_max ? new Date(event_max) : new Date(Date.parse(event_max.substring(0, 33)));
	                            }

	                            showDateRange(start_time, end_time);
	                            gtime_range = [start_time, end_time];
	                            $.each(report_data, function (idx, value) {

	                                if (start_time <= value.datetime && value.datetime <= end_time) {
	                                    if (value.speed >= speed) {
	                                        var row = [value.ignition, value.datetime, value.gpsav, value.speed, value.odometer, value.lat, value.lon];
	                                        if (pto == 1) {
	                                            if (value.pto == "1")
	                                                row.push("On");
	                                            else if (value.pto == "0")
	                                                row.push("Off");
	                                        }
	                                        if (show_address == 1) {
	                                            row.push(value.address);
	                                        }
	                                        row.push("");
	                                        tbl_data.addRow(row);
	                                        way_pts.push([value.lat, value.lon]);
	                                    }
	                                }
	                                if (value.datetime > end_time)
	                                    return;
	                            });
	                            drawTable(tbl_data);
	                            drawMap(way_pts);
	                            highchart.hideLoading();

	                        }
	                    }

	                },
	                yAxis: [{
	                    title: {
	                        text: 'Speed (KM/h)'
	                    },
	                    labels: {
	                        formatter: function () {
	                            return this.value + ' km/h';
	                        }
	                    }
	                }, {
	                    title: {
	                        text: 'Odometer'
	                    },
	                    labels: {
	                        formatter: function () {
	                            return this.value + ' km';
	                        }
	                    },
	                    opposite: true
	                }],
	                tooltip: {
	                    shared: true
	                },
	                legend: {
	                    enabled: false
	                },
	                series: [{
	                    type: 'area',
	                    name: 'Speed (KM/h)',
	                    allowPointSelect: true,
	                    events: {
	                        /*	                	click: function(e) {
	                        if(typeof prev_marker != 'undefined')
	                        prev_marker.setMap(null);

	                        var geopos = new google.maps.LatLng(e.point.lat, e.point.lon);
	                        map.setCenter(geopos);
	                        map.setZoom(16);	                	
	                        prev_marker = new google.maps.Marker({
	                        position: geopos, 
	                        map: map
		    				
	                        });
	                        //selecting_data = rows_data[e.point.x.toString()];
	                        infowindow.open(map, prev_marker);					
							
	                        }	            
	                        */
	                    },
	                    tooltip: {
	                        valueDecimals: 2
	                    }
	                }, {
	                    yAxis: 1,
	                    type: 'line',
	                    name: 'Odometer',
	                    marker: {
	                        enabled: false
	                    },
	                    dashStyle: 'shortdot',
	                    tooltip: {
	                        valueDecimals: 2
	                    }
	                }],
	                rangeSelector: {
	                    enabled: true,
	                    buttons: [{
	                        count: 3,
	                        type: 'hour',
	                        text: '3h'
	                    }, {
	                        count: 6,
	                        type: 'hour',
	                        text: '6h'
	                    }, {
	                        count: 12,
	                        type: 'hour',
	                        text: '12h'
	                    }, {
	                        count: 24,
	                        type: 'hour',
	                        text: '1d'
	                    }],
	                    inputEnabled: false
	                },
	                navigator: {
	                    xAxis: {
	                        ordinal: false,
	                        labels: {
	                            formatter: function () {
	                                //some cleanup may be required, but this is the general form of the solution
	                                if (this.value) {
	                                    date = new Date(this.value)
	                                    if (date.getHours() == 0)
	                                        return date.getDate() + "/" + date.getMonth()
	                                    else {
	                                        if (date.getHours() > 12)
	                                            return date.getHours() - 12 + "pm";
	                                        else
	                                            return date.getHours() + "am";
	                                    }
	                                }
	                            }
	                        }
	                    }
	                }
	            });

	        }
	        initChart();
	        //		open_filter_dialog();
	    });


	    function calculateOdometer(table_data) {

	        var start = table_data.getValue(0, 4);
	        var end = table_data.getValue(table_data.getNumberOfRows() - 1, 4);
	        var total_distance = "-";
	        if (start != null && end != null) {
	            total_distance = end - start;
	        }
	        $("#total_odometer").text(total_distance.toFixed(2) + " km");
	    }

	    function showDateRange(start_time, end_time) {
	        var date_from = formatDateTime(start_time);
	        var date_to = formatDateTime(end_time);
	        var arr = [];
	        arr = date_from.split(" ")[0].split("/");
	        if (arr[1].length == 1) {
	            arr[1] = "0" + arr[1];
	        }
	        if (arr[0].length == 1) {
	            arr[0] = "0" + arr[0];
	        }
	        var dt = arr[2] + "/" + arr[1] + "/" + arr[0] + " 00:00:00";
	        arr = [];
	        arr = date_to.split(" ")[0].split("/");
	        if (arr[1].length == 1) {
	            arr[1] = "0" + arr[1];
	        }
	        if (arr[0].length == 1) {
	            arr[0] = "0" + arr[0];
	        }
	        var dt1 = arr[2] + "/" + arr[1] + "/" + arr[0] + " 23:59:59";
	        $("#begin_time").text(dt);
	        $("#end_time").text(dt1);

	    }


	    function formatDateTime(date) {
	        var hour = date.getHours();
	        var ampm = "am"
	        if (hour > 12) {
	            hour = hour - 12;
	            ampm = "pm"
	        }
	        var month = date.getMonth();
	        return date.getDate() + "/" + (month+1) + "/" + date.getFullYear() + " " + hour + ":" + date.getMinutes() + ampm;
	        /*		return date.toString().substring(0, 24);
	        var time = date.toLocaleTimeString();
	        if(time == "00:00:00")
	        return 	date.toDateString();
	        if(navigator.userAgent.match(/iPad|iPhone|iPod/i) != null) {
	        return 	date.toDateString();
	        }
	        return getDate()date.toDateString() + " " + time;
	        */
	    }
	    /*			
	    function open_filter_dialog() {			
	    if($("#user_lists").get(0).options.length == 0) {
	    jQuery.getJSON("/UsersData.aspx", function(data){
	    $.each(data, function(idx, row) {
	    jQuery("#user_lists").get(0).options.add(new Option(row.name, row.user_id));
	    });
	    if($("#user_lists").get(0).options.length > 0) {
	    $("#user_lists").change();				
	    }
			
	    });			
	    }
	    $('#filter_dialog').dialog('open');
		
	    }	
	    */
	    function ShowGoogleMaps() {
	        //		if(gtime_range.length == 0) {
	        //			gtime_range= [chart.getVisibleChartRange().start, chart.getVisibleChartRange().end];
	        //		}				
	        begin_time = $.datepicker.formatDate('yy/mm/dd', gtime_range[0]) + " " + gtime_range[0].getHours() + ":" + gtime_range[0].getMinutes() + ":" + gtime_range[0].getSeconds();
	        end_time = $.datepicker.formatDate('yy/mm/dd', gtime_range[1]) + " " + gtime_range[1].getHours() + ":" + gtime_range[1].getMinutes() + ":" + gtime_range[1].getSeconds();


	        var googlemapsformobj = $("#googlemapsform");
	        googlemapsformobj.attr("action", "https://www.google.com/maps?q=http%3A%2F%2Ffuel.avls.com.my%2FShowVehicleHistoryInGoogle.aspx%3Fplateno%3D" + plate_no + "%26bdt%3D" + begin_time + "%26edt%3D" + end_time);
	        googlemapsformobj.submit();

	    }

	    function ShowGoogleEarth() {
	        //		if(gtime_range.length == 0) {
	        //			gtime_range= [chart.getVisibleChartRange().start, chart.getVisibleChartRange().end];
	        //		}				
	        begin_time = $.datepicker.formatDate('yy/mm/dd', gtime_range[0]) + " " + gtime_range[0].getHours() + ":" + gtime_range[0].getMinutes() + ":" + gtime_range[0].getSeconds();
	        end_time = $.datepicker.formatDate('yy/mm/dd', gtime_range[1]) + " " + gtime_range[1].getHours() + ":" + gtime_range[1].getMinutes() + ":" + gtime_range[1].getSeconds();

	        var googleearthformobj = $("#googleearthform");

	        googleearthformobj.attr("action", "ShowVehicleHistoryInGoogle.aspx?userid=" + user_id + "&plateno=" + plate_no + "&bdt=" + begin_time + "&edt=" + end_time);
	        //googleearthformobj.action="ShowVehicleHistoryInGoogleEarth.aspx?plateno=" + plateno + "&bdt=" + bdt + "&edt=" + edt
	        googleearthformobj.submit();

	    }


	    function prepare_info_window(value) {
	        console.log(value);
	        $("#speed_td").html(value.y.toFixed(2));

	        $("#odometer_td").html(value.odometer);
	        $("#gpsav_td").html(value.gpsav);
	        $("#ignition_td").html(value.ignition);

	        if (typeof geocoder == 'undefined')
	            geocoder = new google.maps.Geocoder();

	        geocoder.geocode({ latLng: new google.maps.LatLng(value.lat, value.lon) }, function (results, status) {
	            if (status == google.maps.GeocoderStatus.OK) {
	                if (results.length > 0) {
	                    $("#info_address_td").text(results[0].formatted_address);
	                }
	            }
	        });
	    }	

		
		
		
    </script>

		
</body>
</html>

