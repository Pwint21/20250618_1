<%@ Page Language="VB" AutoEventWireup="false"
    Inherits="YTLWebApplication.VehicleViolationDailyReportGroup" EnableEventValidation="false" Codebehind="VehicleViolationDailyReportGroup.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
       <title> Vehicle Violation - Daily</title>
    <style type="text/css" media="screen">
        @import "css/demo_page.css";
        @import "css/demo_table_jui.css";
        @import "css/themes/redmond/jquery-ui-1.8.4.custom.css";
    </style>
    <script type="text/javascript" language="javascript" src="js/jquery.js"></script>
    <link type="text/css" href="css/jquery-ui.css" rel="stylesheet" />
    <link type="text/css" href="css/common1.css" rel="stylesheet" />
    <link href="css/css3-buttons.css" rel="stylesheet" type="text/css" />
    <link href="css/style.css" rel="stylesheet" type="text/css" />
    <link href="css/demos22.css" rel="stylesheet" type="text/css" />
    <link href="css/jquery.ui.dialog.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="js/jquery_ui.js"></script>
    <script type="text/javascript" language="javascript" src="js/jquery.dataTables.js"></script>
    <script src="js/DatePickerConv.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript">
        function refreshTable() {
            
            if ($("#ddlplateno").val() != null) {
                $("#examples_processing").css("visibility", "visible");
                if ($("#ddlplateno").val() == "-- ALL PLATE NOS --")
                    $('#tdplateno').text("Plate No: ALL PLATE NUMBERS");
                else
                    $('#tdplateno').text("Plate No: " + $("#ddlplateno").val());

                $('#tduser').text("User Name: " + $('#ddlusers option:selected').text());
                $('#tdfromto').text("From: " + $('#txtFromdate').val() + " 00:00:00 To: " + $('#txttodate').val() + " 23:59:59");

                $.get("GetViolationDailyReport.aspx?pno=" + $('#ddlplateno').val() + "&op=3&bdt=" + $("#txtFromdate").val() + "&edt=" + $("#txttodate").val() + "&uid=" + $("#ddlusers").val() + "&gid=" + $("#ddlgroup").val() + "", function (response) {

                    var json = JSON.parse(response);
                    table = oTable.dataTable();
                    table._fnProcessingDisplay(true);
                    oSettings = table.fnSettings();
                    table.fnClearTable(this);
                    for (var i = 0; i < json.length-1 ; i++) {
                        table.oApi._fnAddData(oSettings, json[i]);
                    }
                    oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
                    table._fnProcessingDisplay(false);
                    table.fnDraw();
                    if (json.length > 1) {
                        $("#thovsped").text(json[json.length - 1][0]);
                        $("#thspaboveninty").text(json[json.length - 1][1]);
                        $("#thidle").text(json[json.length - 1][2]);
                         
                        $("#thhbk").text(json[json.length - 1][3]);
                         
                        $("#thcntdrv").text(json[json.length - 1][5]);
                        $("#thTotDrv").text(json[json.length - 1][5]);
                        $("#thTotWork").text(json[json.length - 1][6]);
                        $("#thTotunsafework").text(json[json.length - 1][7]);
                        $("#thTotunsafedrive").text(json[json.length - 1][8]);
                        $("#thTotVio").text(json[json.length - 1][9]);
                        $("#thdistance").text(json[json.length - 1][10]);                      
                        $("#thmidnight").text(json[json.length - 1][11]);
                    }
                });

            }
            else {
                $('#tduser').text("User Name: ");
                $('#tdplateno').text("Plate No: ")
                $('#tdfromto').text("From: " + $('#txtFromdate').val() + " 00:00:00 To: " + $('#txttodate').val() + " 23:59:59");

            }

            
           



            //$.ajax({
            //    type: "POST",
            //    url: "VehicleViolationDailyReportGroup.aspx/GetViolationSummary",
            //    data: '{fromdate: \"' + $('#txtFromdate').val() + " 00:00:00" + '\",todate: \"' + $('#txttodate').val() + " 23:59:59" + '\",plateno: \"' + $('#ddlplateno').val() + '\",userid: \"' + $('#ddlusers').val() + '\",h:\"' + $('#h').val() + '\",h11:\"' + $('#h1').val() + '\",h2:\"' + $('#h2').val() + '\",groupid:\"' + $('#ddlgroup').val() + '\"}',
            //    contentType: "application/json; charset=utf-8",
            //    dataType: "json",
            //    success: function (response) {
            //        var json = response.d;
            //        table = oTable.dataTable();
            //        table._fnProcessingDisplay(true);
            //        oSettings = table.fnSettings();
            //        table.fnClearTable(this);
            //        for (var i = 0; i < response.length - 1; i++) {
            //            table.oApi._fnAddData(oSettings, response[i]);
            //        }
            //        oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
            //        table._fnProcessingDisplay(false);
            //        table.fnDraw();
            //        if (response.length > 1) {
            //            $("#thhwy").text(response[response.length - 1][3]);
            //            $("#thnonhwy").text(response[response.length - 1][4]);
            //            $("#thidle").text(response[response.length - 1][5]);
            //             $("#thsafeidle").text(response[response.length - 1][6]);
            //            $("#thhbk").text(response[response.length - 1][7]);
            //               $("#thdhacc").text(response[response.length - 1][8]);
            //            $("#thcntdrv").text(response[response.length - 1][9]);
            //            $("#thTotDrv").text(response[response.length - 1][10]);
            //            $("#thTotWork").text(response[response.length - 1][11]);
            //            $("#thTotVio").text(response[response.length - 1][12]);
            //            $("#thdistance").text(response[response.length - 1][13]);
            //        }
            //        return false;
            //    },
            //    failure: function (response) {
            //        alert("Error");
            //    }
            //});
        }
        $(document).ready(function () {

            $("#txtFromdate").datepicker({ dateFormat: 'yy/mm/dd', minDate: -180, maxDate: -1, changeMonth: true, changeYear: true, numberOfMonths: 2
            });

            $("#txttodate").datepicker({ dateFormat: 'yy/mm/dd', minDate: -180, maxDate: -1, changeMonth: true, changeYear: true, numberOfMonths: 2
            });

            $("#dialog-message").hide();
            oTable = $('#examples').dataTable({
                "bJQueryUI": true,
                "bProcessing": true,
                "sPaginationType": "full_numbers",
                "iDisplayLength": 20,
                "aaSorting": [[1, "asc"]],
                "aLengthMenu": [10, 25, 50, 100, 200, 500],
                "bLengthChange": false,
                "fnDrawCallback": function (oSettings) {
                    if (oSettings.bSorted || oSettings.bFiltered) {
                        if (oSettings.aoColumns[0].bVisible == true) {
                            for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                                $('td:eq(0)', oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
                            }
                        }
                    }
                },
               
                "aoColumnDefs": [{ "sClass": "left", "bVisible": true, "bSortable": false, "aTargets": [0] },
                    { "sClass": "right", "bVisible": true, "aTargets": [3] },
                    { "sClass": "right", "bVisible": true, "aTargets": [4] },
                    { "sClass": "right", "bVisible": true, "aTargets": [5] },
                    { "sClass": "right", "bVisible": true, "aTargets": [6] },
                    { "sClass": "right", "bVisible": true, "aTargets": [7] },
                    { "sClass": "right", "bVisible": true, "aTargets": [8] },
                    { "sClass": "right", "bVisible": true, "aTargets": [9] },
                    { "sClass": "right", "bVisible": true, "aTargets": [10] },
                    { "sClass": "right", "bVisible": true, "aTargets": [11] }
                     ]
            });


            
             refreshTable();
             LoadVehicleGroup();
            //LoadVehicles();
        });

        function LoadVehicleGroup() {
            
            if ($('#ddlusers').val() == "--Select User Name--") {
                $('#ddlgroup').empty().append('<option selected="selected" value="0">Select User Name</option>');
            }
            else {
                $('#ddlgroup').empty().append('<option selected="selected" value="0">Loading...</option>');

                $.get("GetViolationDailyReport.aspx?uid=" + $('#ddlusers').val() + "&op=1", function (response) {
                    $('#ddlgroup').html("");
                    $('#ddlgroup').append("<option value='ALL'>ALL</option>");
                    var json = JSON.parse(response);
                    for (var i = 0; i < json.length; i++) {
                        $('#ddlgroup').append("<option value='" + json[i][1] + "'>" + json[i][1] + "</option>");
                    }
                    LoadVehicles();
                  
                });


                //$.ajax({
                //    type: "POST",
                //    url: "VehicleViolationDailyReportGroup.aspx/LoadVehicleGroup",
                //    data: '{userId: ' + $('#ddlusers').val() + '}',
                //    contentType: "application/json; charset=utf-8",
                //    dataType: "json",
                //    success: OnLoadVehicleGroup,
                //    failure: function (response) {
                //        alert(response.d);
                //    }
                //});
            }
         }


        function LoadVehicles() {
            
            $('#ddlplateno').empty().append('<option selected="selected" value="0">Loading...</option>');

            $.get("GetViolationDailyReport.aspx?uid=" + $('#ddlusers').val() + "&op=2&groupid=" + $('#ddlgroup').val()+"", function (response) {
                $('#ddlplateno').html("");
                $('#ddlplateno').append("<option value='ALL'>ALL</option>");
                var json = JSON.parse(response);
                for (var i = 0; i < json.length; i++) {
                    $('#ddlplateno').append("<option value='" + json[i] + "'>" + json[i] + "</option>");
                }
            });
        }
        

        function PopulateControl(list, control) {
            if (list.length > 0) {
                control.removeAttr("disabled");
                
                control.empty().append('<option selected="selected" value="-- ALL PLATE NOS --">ALL</option>');
                $.each(list, function () {
                    control.append($("<option></option>").val(this['Value']).html(this['Text']));
                });
            }
            else {
                control.empty().append('<option selected="selected" value="0">No vehicles<option>');
            }
        }

        function PopulateControlGroup(list, control) {
            if (list.length > 0) {
                control.removeAttr("disabled");
                control.empty().append('<option selected="selected" value="0">--SELECT PLATE NO--</option>');
                control.empty().append('<option selected="selected" value="0">ALL</option>');
                $.each(list, function () {
                    control.append($("<option></option>").val(this['Value']).html(this['Text']));
                });
            }
            else {
                control.empty().append('<option selected="selected" value="0">No vehicles<option>');
             }
         }

        function openPopup(frdate, todate, plateno, user, type) {
           $("#hidtype").val(type);
              $("#deplateno").val(plateno);
                $("#defrom").val(frdate);
                  $("#deto").val(todate);
            var heading;
            if (type == "hwy")
                heading = "Speed Violation Details On HighWays";
            else if (type == "nonhwy")
                heading = "Speed Violation Details On Non-HighWays";
            else if (type == "idealing" || type=="safeidealing")
                heading = "Vehicle idling Details";
            else if (type == "Hbreak")
                heading = "Harsh Break Details";


            $("#dialog-message").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
                width: 980,
                minHeight: 600,
                height: 600,
                title: heading,
                buttons: {
                    print: function () {
                        var type= $("#hidtype").val();
                        var heading;
                        if (type == "hwy")
                            heading = "Speed Violation Details On HighWays";
                        else if (type == "nonhwy")
                            heading = "Speed Violation Details On Non-HighWays";
                        else if (type == "idealing" || type == "safeidealing")
                            heading = "Vehicle idling Details";
                        else if (type == "Hbreak")
                            heading = "Harsh Break Details";
                        window.open("ViolatiosPrint.aspx?title=" + heading, 'width=900,height=700,toolbar=0,menubar=0,location=0,status=0,scrollbars=1,resizable=0,left=0,top=0');
                    },

                    SaveExcel: function () {
                      DownloadDetailes();
                    },

                    "Close": function () {
                        $("#hor-minimalist-b").children().remove();
                        $(this).dialog("close");
                    }
                  
                }
            });
            var i = 0;
            $.ajax({

                type: "POST",
                url: "VehicleViolationDailyReportGroup.aspx/GetViolationDetails",
                data: '{fromdate: \"' + frdate + " 00:00:00" + '\",todate: \"' + todate + " 23:59:59" + '\",plateno: \"' + plateno + '\",userid: \"' + $('#ddlusers').val() + '\",type:\"' + type + '\",h:\"' + $('#h').val() + '\",h1:\"' + $('#h1').val() + '\",h2:\"' + $('#h2').val() + '\",h3:\"' + $('#h3').val() + '\",h4:\"' + $('#h4').val() + '\",h5:\"' + $('#h5').val() + '\",h6:\"' + $('#h6').val() + '\",h7:\"' + $('#h7').val() + '\",h8:\"' + $('#h8').val() + '\",h9:\"' + $('#h9').val() + '\",h10:\"' + $('#h10').val() + '\"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    $("#hor-minimalist-b").children().remove();
                    $("#hor-minimalist-b").append(response.d);
                    return false;
                },
                failure: function (response) {
                    alert("Failed");
                }

            });
            if (i == 0) {
                $("#hor-minimalist-b").append("<tr><td style='height: 400px;vertical-align:center;text-align:center;'><img  src='images/ajax-loader.gif'/></td></tr>");
                i = i + 1;
            }

            $("#dialog-message").dialog("open");

        }

      

        function ExcelReport() {

            var plateno = document.getElementById("ddlplateno").value;
              document.getElementById("fd").value = $('#txtFromdate').val() +" 00:00:00";
              document.getElementById("td").value = $('#txttodate').val()+" 23:59:59";
            document.getElementById("plateno").value = plateno;

            var excelformobj = document.getElementById("excelform");
            excelformobj.submit();

        }
        
        function DownloadDetailes() {
           var type= $("#hidtype").val();
          if (type == "hwy")
                heading = "Speed Violation Details On HighWays";
            else if (type == "nonhwy")
                heading = "Speed Violation Details On Non-HighWays";
            else if (type == "idealing" || type=="safeidealing")
                heading = "Vehicle idling Details";
            else if (type == "Hbreak")
                heading = "Harsh Break Details";
            else if (type == "Hacc")
                heading = "Harsh Acceleration Details";
                $("#detailedtitle").val(heading);
         var excelformobj = document.getElementById("detailedexcel");
            excelformobj.submit();
        }


        function Exporttopdf() {
            var plateno = document.getElementById("ddlplateno").value;
            var fromdate = $('#txtFromdate').val() + " 00:00:00";
            var todate = $('#txttodate').val() + " 23:59:59";
            var userid = $('#ddlusers').val();
            var username = $('#ddlusers option:selected').text();
            var groupid = $('#ddlgroup').val();
            $('#pdfuserid').val(userid);
            $('#pdffrmdt').val(fromdate);
            $('#pdftd').val(todate);
            $('#pdfplate').val(plateno);
            $('#pdfopr').val(0);
            $('#pdfusername').val(username);
            $('#pdfgroup').val(groupid);
            var pdfformobj = document.getElementById("pdf");
            pdfformobj.submit();
        }

    </script>
    <style type="text/css">
        .item
        {
            -webkit-border-radius: 5px;
            border-radius: 5px;
            display: block;
            border: 1px solid #AAA;
            padding: 8px;
            padding-left: 5px;
            padding-right: 10px;
            float: left;
            color: #777777;
            text-shadow: 1px 1px 2px rgba(255, 255, 255, 0.65);
            margin-right: 5px;
            max-width: .6+ */ background:px;
            margin-bottom: 10px;
            font-size: 11px;
        }
        
        .item.normal
        {
            background: -moz-linear-gradient(top, #FFFFFF 0%, #7589EE 100%); /* FF3.6+ */
            background: -webkit-gradient(linear, left top, left bottom, color-stop(0%,#FFFFFF), color-stop(100%,#7589EE)); /* Chrome,Safari4+ */
            background: -webkit-linear-gradient(top, #FFFFFF 0%,#7589EE 100%); /* Chrome10+,Safari5.1+ */
            background: -o-linear-gradient(top, #FFFFFF 0%,#7589EE 100%); /* Opera11.10+ */
            background: -ms-linear-gradient(top, #FFFFFF 0%,#7589EE 100%); /* IE10+ */
            background: linear-gradient(top, #FFFFFF 0%,#7589EE 100%); /* W3C */
            border: 1px solid #A0A0A0;
            vertical-align: baseline;
        }
        .item.active
        {
            background: -moz-linear-gradient(top, #CBE0F3 0%, #4994D7 2%, #066ECD 100%); /* FF3.6+ */
            background: -webkit-gradient(linear, left top, left bottom, color-stop(0%, #CBE0F3), color-stop(2%,#4994D7), color-stop(100%,#066ECD)); /* Chrome,Safari4+ */
            background: -webkit-linear-gradient(top, #CBE0F3 0%, #4994D7 2%, #066ECD 100%); /* Chrome10+,Safari5.1+ */
            background: -o-linear-gradient(top, #CBE0F3 0%, #4994D7 2%, #066ECD 100%); /* Opera11.10+ */
            background: -ms-linear-gradient(top, #CBE0F3 0%, #4994D7 2%, #066ECD 100%); /* IE10+ */
            background: linear-gradient(top, #CBE0F3 0%, #4994D7 2%, #066ECD 100%); /* W3C */
            filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#4994D7', endColorstr='#066ECD');
            -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorstr='#4994D7', endColorstr='#066ECD')";
            color: #FFF;
            border: 1px solid #03437C;
            text-shadow: 1px 1px 2px rgba(0, 0, 0, 0.5);
        }
        
        .item.active .value
        {
            font-size: 20px;
            display: block;
            margin-top: 0.2em;
        }
        
        .item.normal .value
        {
            color: #444444;
            font-size: 18px;
            display: block;
            margin-top: 0.2em;
        }
        
        .item.normal .value.small
        {
            font-size: 10px;
            margin-top: 0.1em;
        }
        
        .info_box
        {
            -webkit-border-radius: 5px;
            border-radius: 5px;
            background: #fff;
            border: 1px solid #AAA;
            padding: 4px;
            margin-top: 0.1em;
        }
        
        .dropdown
        {
            font-size: 12px;
            display: block;
            padding: 3px;
            border: solid 1px #AAA;
            width: 400px;
        }
        .gbutton
        {
            padding: 0.4em 1em 0.4em 20px;
            position: relative;
            text-decoration: none;
            width: 180px;
        }
        .gbutton span.ui-icon
        {
            left: 0.2em;
            margin: -8px 5px 0 0;
            position: absolute;
            top: 50%;
        }
        .textbox1
        {
            height: 20px;
            width: 180px;
            border-right: #cbd6e4 1px solid;
            border-top: #cbd6e4 1px solid;
            border-left: #cbd6e4 1px solid;
            color: #0b3d62;
            border-bottom: #cbd6e4 1px solid;
        }
        
        .hor-minimalist-b
        {
            font-family: "Verdana";
            font-size: 11px;
            background: #fff;
            margin: 5px;
            width: 99%;
            border-collapse: collapse;
            text-align: left;
        }
        .hor-minimalist-b th
        {
            font-size: 14px;
            font-weight: normal;
            color: #039;
            padding: 5px 3px;
            border-bottom: 2px solid #6678b1;
        }
        .hor-minimalist-b td
        {
            border-bottom: 1px solid #ccc;
            color: #669;
            padding: 6px 8px;
        }
        .hor-minimalist-b tbody tr:hover td
        {
            color: #009;
        }
         button, .button
        {
            width: inherit;
            height: inherit;
        }
    </style>
</head>
<body onload="conv()">
    <form id="form1" runat="server">
   <%-- <asp:Literal ID="Literal1" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, vvr%>" />
    <asp:Literal ID="Literal2" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, vvr1%>" />
    <asp:Literal ID="Literal3" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, ffm6%>" />
    <asp:Literal ID="Literal4" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, tit1%>" />
    <asp:Literal ID="Literal5" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, sPlateNo%>" />
    <asp:Literal ID="Literal6" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, vvr2%>" />
    <asp:Literal ID="Literal7" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, vvr3%>" />
    <asp:Literal ID="Literal10" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, vvr6%>" />
    <asp:Literal ID="Literal11" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, vvr7%>" />
    <asp:Literal ID="Literal12" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, ur2%>" />
    <asp:Literal ID="Literal13" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, vvr8%>" />
    <asp:Literal ID="Literal14" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, vvr9%>" />
   
    <asp:Literal ID="Literal16" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, vvr11%>" />
    <asp:Literal ID="Literal17" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, vvr12%>" />
    <asp:Literal ID="Literal18" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, vvr13%>" />
    <asp:Literal ID="Literal19" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, vvr14%>" />
    <asp:Literal ID="Literal20" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, vvr15%>" />
    <asp:Literal ID="Literal21" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, fur%>" />
    <asp:Literal ID="Literal22" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, fur1%>" />
    <asp:Literal ID="Literal23" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, vvr16%>" />
    <asp:Literal ID="Literal24" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, pt%>" />
    <asp:Literal ID="Literal25" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, btn7%>" />
   
    <asp:Literal ID="Literal8" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, sall%>" />
    <asp:Literal ID="Literal9" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, g6%>" />
    <asp:Literal ID="Literal30" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, sFil%>" />
    <asp:Literal ID="Literal31" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, g8%>" />
    <asp:Literal ID="Literal32" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, g9%>" />
    <asp:Literal ID="Literal33" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, g10%>" />
    <asp:Literal ID="Literal34" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, g7%>" />
    <asp:Literal ID="Literal35" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, lst103%>" />
    <asp:Literal ID="Literal36" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, poiu%>" />
    <asp:Literal ID="Literal37" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, lst114%>" />
    <asp:Literal ID="Literal38" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, lst122%>" />
    <asp:Literal ID="Literal39" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, lst112%>" />
    <asp:Literal ID="Literal40" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, lst140%>" />
    <asp:Literal ID="Literal41" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, vd4%>" />
    <asp:Literal ID="Literal42" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, sNo%>" />
    <asp:Literal ID="Literal43" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, fm13%>" />
    <asp:Literal ID="Literal44" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, sSpeed%>" />
    <asp:Literal ID="Literal45" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, sLocation%>" />
    <asp:Literal ID="Literal46" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, gr17%>" />
    <asp:Literal ID="Literal47" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, lst37%>" />
    <asp:Literal ID="Literal48" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, vd%>" />
    <asp:Literal ID="Literal49" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, vd1%>" />
    <asp:Literal ID="Literal50" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, vd2%>" />
    <asp:Literal ID="Literal51" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, vd3%>" />
    <asp:Literal ID="Literal52" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, btn1%>" />
    <asp:Literal ID="Literal53" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, tm14%>" />
    <asp:Literal ID="Literal54" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, vvdr%>" />
    <asp:Literal ID="Literal55" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, vm15%>" />
    <asp:Literal ID="Literal56" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, excel1%>" />
    <asp:Literal ID="Literal15" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, vm32%>" />
    <asp:Literal ID="Literal26" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, vm33%>" />
    <asp:Literal ID="Literal27" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, ext17%>" />
    <asp:Literal ID="Literal28" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, alt19%>" />
    <asp:Literal ID="Literal29" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, alt20%>" />
    <asp:Literal ID="Literal57" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, alt21%>" />
    <asp:Literal ID="Literal58" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, alt22%>" />
    <asp:Literal ID="Literal59" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, alt23%>" />--%>
    <center>
        <br />
         <div class="c1"> Vehicle Violation - Daily Report</div>
         <br />
         <br />
         <table  style="width:1100px" >
         <tr>
         <td style="width:20%;vertical-align:top;">
         <div class="item normal" style="clear:both; width: 200px; height: 310px;text-align:left;background: -webkit-linear-gradient(top, #FFFFFF 0%,#7589EE 100%);" >
                        <h4>Filter</h4>
                        <br />
                    	From Date
						<span class="value">
							<input type="text" id="txtFromdate" class="textbox1" value="<%= Now.AddDays(-1).ToString("yyyy/MM/dd") %>" style="width:191px;"    />
						</span>
                        	To Date
						<span class="value">
						<input type="text" id="txttodate" class="textbox1"  value="<%= Now.AddDays(-1).ToString("yyyy/MM/dd") %>" style="width:191px;"   />
						</span>
                    	User Name
						<span class="value">
							<select name="user_id" id="ddlusers" runat ="server" onchange="LoadVehicleGroup();" style="width:191px;" class="dropdown">
                           
							</select>
						</span>
             Group
             <span class="value">
							<select name="user_id" id="ddlgroup" runat ="server" onchange="LoadVehicles();" style="width:191px;" class="dropdown">
                           
							</select>
						</span>
						Plate No
						<span class="value">
							<select name="user_id" id="ddlplateno" runat ="server" style="width:191px;" onchange="refreshTable()" class="dropdown">
                           
							</select>
						</span>
                       <%-- <div style="text-align: right; margin-top: 15px; margin-left: 20px;">	
							<span  class="gbutton ui-state-default ui-corner-all" id="print_btn" style="cursor:pointer ;"  onclick="refreshTable()"><span class="ui-icon ui-icon-check"> </span>Generate</span>
                       </div>--%>
                        <div style="text-align: right; margin-top: 15px; margin-left: 0px;width:200px;">	
                       <%--<a href="javascript:Exporttopdf();" class="gbutton ui-state-default ui-corner-all" id="A1"><span class="ui-icon ui-icon-print"> </span>Print</a>--%>
                            <span  class="gbutton ui-state-default ui-corner-all" id="print_btn" style="cursor:pointer ;"  onclick="refreshTable()"><span class="ui-icon ui-icon-check"> </span>Generate</span>
					    <a href="javascript:ExcelReport();" class="gbutton ui-state-default ui-corner-all" id="export_btn" ><span class="ui-icon ui-icon-disk"></span>Save Excel</a>
                       
                       
                        </div>
        
	      </div>
                   
         </td>
         <td style="width:80%;vertical-align:top; padding-top:0px;">
          <div class="table info_box" id="data_log" style="width:1250px;padding-top:0px;">
          
            <table border="0" width="1230px;" style="font-family: Verdana; font-size: 11px">
            <tr><td>
                 <table   id="filterdetails" style="width:1060px;" class="hor-minimalist-b" >
                 <thead><tr><th style="text-align:left; width:40%;">Filtering Constraints</th><th style="text-align:left;width:55%;">Remarks</th></tr></thead>
                 <tr>
                  <td align="left" id="tduser">
                  
                  </td>
                  <td align="left" >
                  <b style="color:Blue;">TotVio  -</b> Total Violations,<b style="color:Blue;"> Distance -</b> Total Distance Traveled
                  </td>
                  </tr>   
                  <tr>
                  <td align="left" id="tdplateno">
                  
                  </td>
                  <td align="left" >
                 <b style="color:Blue;">Idle -</b> Idle Count,<b style="color:Blue;"> HBreak -</b> Harsh Break Count,  <b style="color:Blue;">CntDrv -</b> Continuous Drive More than 4 Hours
                  </td>
                  </tr>   
                  <tr>
                  <td align="left" id="tdfromto">
                  
                  </td>
                   <td align="left" >
                   <b style="color:Blue;">TotDrv Hrs -</b> Total Drive Hours,<b style="color:Blue;"> TotWork Hrs -</b> Total Working Hours
                  </td>
                  </tr>    
                  
                 </table>
                 </td>
                </tr>
                 <tr><td>
                        <div id="fw_container" style="width:1230px;">
                        <table cellpadding="0" cellspacing="0" border="0"  id="examples" style="width:1230px;font-family:Verdana;" class="display" >
                            <thead align="left">
                               <tr style="font-weight: bold;">
                                    <th style="width: 60px; ">
                                       No
                                    </th>
                                     <th style="width: 120px;">
                                      Violation Date
                                    </th>
                                    <th style="width: 80px; ">
                                        Plate No
                                    </th>
                                   <th>
                                       Group
                                   </th>
                                    <th style="width: 60px; ">
                                       Over Speed
                                    </th>
                                   <th style="width: 60px; ">
                                      speed >90
                                    </th>
                                     <th style="width: 30px; ">
                                       Idle
                                    </th>
                                    <th style="width: 30px; ">
                                       HBreak
                                    </th>
                                    <th style="width: 50px; ">
                                        CntDrv
                                    </th>
                                     <th style="width: 120px; ">
                                        TotDrv Hrs
                                    </th>
                                    <th style="width: 120px; ">
                                        TotWork Hrs
                                    </th>
                                    <th style="width: 120px; ">
                                        Frequency of > 14 Hrs Work
                                    </th>
                                     <th style="width: 120px; ">
                                        Frequency of > 10 Hrs Drive
                                    </th>
                                     <th style="width: 40px; ">
                                        TotVio
                                    </th>

                                    <th style="width: 50px; ">
                                        Distance (KM)
                                    </th>
                                    <th style="width: 80px; ">
                                        Mid-Night Count
                                    </th>
                                    
                                   
                                </tr>
                            </thead>
                            <tbody>
                            </tbody>
                            <tfoot align="left" style="height:30px;">
                               <tr style="font-weight: bold;font-size:12px;">
                                
                                    <th style="width: 60px; ">
                                    
                                    </th>
                                     <th style="width: 120px;">
                                     Total
                                    </th>
                                   <th>
                                    </th>
                                    <th style="width: 80px; ">
                                       
                                    </th>
                                   
                                    <th style="width: 60px; text-align :right;" id="thovsped">
                                      
                                    </th>
                                     <th style="width: 60px; text-align :right;" id="thspaboveninty">
                                   
                                    </th>
                                    <th style="width: 40px;text-align :right; " id="thidle">
                                        
                                    </th>
                                     
                                     <th style="width: 40px;text-align :right; " id="thhbk">
                                        
                                    </th>
                                    
                                    <th style="width: 80px; text-align :right;" id="thcntdrv"> 
                                        
                                    </th>
                                    <th style="width: 90px;text-align :right; " id="thTotDrv">
                                        
                                    </th>
                                    <th style="width: 90px; text-align :right;" id="thTotWork">
                                        
                                    </th>
                                   <th style="width: 90px;text-align :right; " id="thTotunsafework">
                                        
                                    </th>
                                    <th style="width: 90px; text-align :right;" id="thTotunsafedrive">
                                        
                                    </th>
                                     <th style="width: 40px;text-align :right; " id="thTotVio">
                                        
                                    </th>
                                    <th style="width: 50px;text-align :right; " id="thdistance">
                                       
                                    </th>
                                   <th style="width: 50px;text-align :right; " id="thmidnight">
                                       
                                    </th>
                                </tr>
                            </tfoot>
                        </table>
                         <table id="Table1" style="width:1060px;">
                           
                    </table>
                    </div>
                   </td></tr>
                  
                   </table>
         </div>   
         </td>
         </tr>
         </table>
         <div id="dialog-message" style="padding-top: 0px; padding-right: 0px; padding-bottom: 0px;
            font-size: 10px; padding-left: 0px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;">
            <div  style="width: 960px;" id="exceldiv" >
              <table   id="hor-minimalist-b" class="hor-minimalist-b">
                         
             </table>
            </div>
            </div>  

          </center>
          <input type="hidden" id="hiduser" name="hiduser" runat ="server"  />
      <input type ="hidden" id="hidtype"   />
    <input type="hidden" value="<%=lng %>" id="lanc" />
    <input type="hidden" id="h11" name="h11" value="Violation Date" />
    <input type="hidden" id="h" name="h" value="No" />
    <input type="hidden" id="h1" name="h1" value="Plate No" />
    <input type="hidden" id="h2" name="h2" value="Date" />
    <input type="hidden" id="h3" name="h3" value="Speed" />
    <input type="hidden" id="h4" name="h4" value="Location" />
    <input type="hidden" id="h5" name="h5" value="From Date" />
    <input type="hidden" id="h6" name="h6" value="To Date" />
    <input type="hidden" id="h7" name="h7" value="Duration" />
    <input type="hidden" id="h8" name="h8" value="From" />
    <input type="hidden" id="h9" name="h9" value="To" />
    <input type="hidden" id="h10" name="h10" value="No Data to Display...." />
    </form>
    <form id="excelform" method="get" action="ExcelReport.aspx">
    <input type="hidden" id="title" name="title" value="Vehicle Violation Daily Report" />
    <input type="hidden" id="plateno" name="plateno" value="" />
    <input type="hidden" id="titl1" name="titl1" value="Vehicle Violation - Daily Report" />
    <input type="hidden" id="plno" name="plno" value="Vehicle plate number" />
    <input type="hidden" id="rd" name="rd" value="Report Date" />
    <input type="hidden" id="fd" name="fd" value="" />
    <input type="hidden" id="td" name="td" value="" />
    </form>
    <form id="pdf" method="get" action="ConvertToPdf.aspx">
    <input type="hidden" id="pdftitle" name="pdftitle" value="Vehicle Violation Daily Report" />
    <input type="hidden" id="pdfuserid" name="pdfuserid" />
    <input type="hidden" id="pdffrmdt" name="pdffrmdt" />
    <input type="hidden" id="pdftd" name="pdftd" />
    <input type="hidden" id="pdfplate" name="pdfplate" />
        <input type="hidden" id="pdfgroup" name="pdfgroup" />
    <input type="hidden" id="pdfopr" name="pdfopr" />
    <input type="hidden" id="pdfusername" name="pdfusername" />
    </form>
     <form id="detailedexcel" method="get" action="ExcelDownload.aspx">
     <input type="hidden" id="detailedtitle" name="detailedtitle" />
    <input type="hidden" id="deplateno" name="deplateno" />
     <input type="hidden" id="defrom" name="defrom" />
    <input type="hidden" id="deto" name="deto" />
     
     </form>
</body>
</html>
