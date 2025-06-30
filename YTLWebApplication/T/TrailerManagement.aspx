<%@ Page Language="VB" AutoEventWireup="false"
    Inherits="YTLWebApplication.TrailerManagement" Codebehind="TrailerManagement.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Trailer Management</title>
    <style type="text/css" media="screen">
        @import "cssfiles/demo_page.css";
        @import "cssfiles/demo_table_jui.css";
        @import "cssfiles/themes/redmond/jquery-ui-1.8.4.custom.css";
        @import "cssfiles/jquery-ui.css";
        @import "cssfiles/jquery.ui.dialog.css";
        @import "cssfiles/TableTools.css";
        @import "cssfiles/ColVis.css";
    </style>
    <style type="text/css">
         tfoot input
        {
            margin: 0.2em 0;
            width: 100%;
            margin-left: -0.7em;
            color: #444;
        }
        .leftClass
        {
            text-align: left;
        }
         div.dataTables_wrapper .ui-widget-header
        {
            font-weight: normal;
            float: left;
            text-align: left;
        }
        .dataTables_wrapper .ui-toolbar
        {
            padding: 5px;
            width: 1230px;
        }
    </style>
    <style type="text/css">
        .chzn-container-single .chzn-single
        {
            height: 24px;
            line-height: 24px;
        }
        .chzn-search
        {
            width: 203px;
        }
        .chzn-container .chzn-results
        {
            max-height: 300px;
        }
    </style>
    <style type="text/css">
        .ui-button
        {
            font-size: 12px;
        }
    </style>
    <link type="text/css" href="cssfiles/css3-buttons.css" rel="stylesheet" />
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.js"></script>
    <link href="cssfiles/chosen.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="jsfiles/chosen.jquery.js"></script>
    <script src="jsfiles/jquery-ui.min.js" type="text/javascript"></script>
    <script type="text/javascript" language="javascript" src="jsfiles/jquery.dataTables.js"></script>
    <script type="text/javascript">
        function checkall(chkobj) {
            var chkvalue = chkobj.checked;
            for (i = 0; i < document.forms[0].elements.length; i++) {
                elm = document.forms[0].elements[i]
                if (elm.type == 'checkbox') {
                    document.forms[0].elements[i].checked = chkvalue;
                }
            }
        }
        function deleteconfirmation() {
            var checked = false;
            $(".group1:checked").each(function () {
                if (this.value != "on") {
                    checked = true;
                }
            });           
            if (checked) {
                var result = confirm("Are you sure you want to delete selected trailer(s) ?");
                if (result) {
                    return true;
                }
                return false;
            }
            else {
                alertbox("Please select checkboxes");
                return false;
            }
        }
        function alertbox(message) {
            if (message == "") {
                document.getElementById("displayp").innerHTML = message;

            }
            else {
                document.getElementById("displayp").innerHTML = message;
                $("#dialog-msg").dialog("open");
            }
        }
        function confirm(message) {

            document.getElementById("P1").innerHTML = message;
            $("#div1").dialog("open");

        }

        $(function () {


            $('#txtdttm1').datepicker({ dateFormat: 'yy/mm/dd', minDate: +1, changeMonth: true, changeYear: true, numberOfMonths: 2 });
            $('#txtdttm').datepicker({ dateFormat: 'yy/mm/dd', minDate: +1, changeMonth: true, changeYear: true, numberOfMonths: 2 });
            $('#txtroadtax').datepicker({ dateFormat: 'yy/mm/dd', minDate: +1, changeMonth: true, changeYear: true, numberOfMonths: 2 });
            $('#txtroadtax1').datepicker({ dateFormat: 'yy/mm/dd', minDate: +1, changeMonth: true, changeYear: true, numberOfMonths: 2 });
            $('#txtptest').datepicker({ dateFormat: 'yy/mm/dd', minDate: +1, changeMonth: true, changeYear: true, numberOfMonths: 2 });
            $('#txtptest1').datepicker({ dateFormat: 'yy/mm/dd', minDate: +1, changeMonth: true, changeYear: true, numberOfMonths: 2 });
            $('#txtinsurence').datepicker({ dateFormat: 'yy/mm/dd', minDate: +1, changeMonth: true, changeYear: true, numberOfMonths: 2 });
            $('#txtinsurence1').datepicker({ dateFormat: 'yy/mm/dd', minDate: +1, changeMonth: true, changeYear: true, numberOfMonths: 2 });

            $("#dialog:ui-dialog").dialog("destroy");

            $("#dialog-message1").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
                minWidth: 1000,
                minHeight: 529
            });
            $("#dialog-msg").dialog({
                autoOpen: false,
                resizable: false,
                height: 140,
                modal: true,
                buttons: {
                    "OK": function () {
                        $(this).dialog("close");
                    }

                }
            });
            $("#div1").dialog(
        {
            title: "Confirm",
            modal: true,
            resizable: false,
            width: 320,
            autoOpen: false,
            buttons: {
                "YES": function () {
                    DeleteToll();
                    $(this).dialog("close");
                },
                "NO": function () {
                    $(this).dialog("close");
                }
            }
        });
        });
        function DeleteToll() {
            var checked = false;
            var matches = [];
            $(".group1:checked").each(function () {
                if (this.value != "on") {
                    matches.push(this.value);
                }                
            }); 
            var lastXHR = $.get("TrailerMgmtJson.aspx?opr=4&ugData=" + matches , function (data) {
                if (data == "Yes") {
                    return false;
                }
                else {
                    alertbox("<b style='Color:Red;'>Sorry.Please try again..!!!</b>");
                    var x;
                    return false;
                }
            });
            refreshTable();
            //                checked = false;
            $(".group1:checked").each(function () {
                $(this).removeAttr('checked');
            });
        }
        function refreshTable() { 
            table = oTable.dataTable();
            table._fnProcessingDisplay(true);
            oSettings = table.fnSettings();
            $.getJSON('TrailerMgmtJson.aspx?u=' + $("#uid").val() + '&opr=1&r=' + $("#rle").val() + '&lst=' + $("#ulist").val() + '&r=' + Math.random(), null, function (json) {
                table.fnClearTable(this);
                for (var i = 0; i < json.length; i++) {
                    table.oApi._fnAddData(oSettings, json[i]);
                }
                oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
                table._fnProcessingDisplay(false);

                table.fnDraw();
            });
            return false;
        }
        function ExcelReport() {
            refreshTable();
            var excelformobj = document.getElementById("excelform");
            excelformobj.submit();
        }
        function AddData() {
            var emailRegEx = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
            if ($('#txttname').val() == "") {
                alertbox("Please enter Trailer number.");
                return false;
            }
            if ($('#txtdttm').val() == "") {
                alertbox("Please select Inspection Date.");
                return false;
            }

            if ($('#txtemailid').val() == "") {
                alertbox("Please enter emailid1.");
                return false;
            }

            if (!emailRegEx.test(document.getElementById("txtemailid").value)) {
                alertbox("Please valid enter emailid1.");
                return false;
            }

            if ($('#txtemailidCC').val() != "") {
                var arr;
                var emls = document.getElementById("txtemailidCC").value;
                if (emls.indexOf(",") > 0) {
                    var mar = new Array();
                    mar = emls.split(",");
                    arr = new Array();
                    for (var i = 0; i < mar.length; i++) {
                        arr.push($.trim(mar[i]));
                    }
                }
                else if (emls.indexOf(";") > 0) {
                    arr = new Array();
                    var mar = new Array();
                    mar = emls.split(";");
                    for (var i = 0; i < mar.length; i++) {
                        arr.push($.trim(mar[i]));
                    }
                }
                else {
                    arr = new Array();
                    arr.push(emls);
                }
                for (var i = 0; i < arr.length; i++) {
                    if (!emailRegEx.test(arr[i])) {
                        alertbox("Please valid enter EMail Id (CC).");
                        return false;
                    }
                }

            }
            var lastXHR = $.get("TrailerMgmtJson.aspx?opr=2&insdatetime=" + $('#txtdttm').val() + "&rtax=" + $('#txtroadtax').val() + "&pt=" + $('#txtptest').val() + "&insu=" + $('#txtinsurence').val() + "&tname=" + $('#txttname').val() + "&uid=" + $('#ddluser').val() + "&em1=" + $('#txtemailid').val() + "&emlcc=" + $('#txtemailidCC').val(), function (data) {
                if (data == "Yes") {
                    return false;
                }
                else {
                    alertbox("<b style='Color:Red;'>Sorry.Please try again..!!!</b>");
                    var x;
                    return false;
                }
            });
           
            $("#dialog-form").dialog("close");
            $('#txttname').val("");
            $('#txtdttm').val("");
            $('#ddluser').val("");
            refreshTable();
        }
        function UpdateData() {
            var emailRegEx = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
            if ($('#txttname1').val() == "") {
                alertbox("Please enter Trailer number.");
                return false;
            }
            else if ($('#txtdttm1').val() == "") {
                alertbox("Please select Inspection Date.");
                return false;
            } 
            if ($('#txtemailid1').val() == "") {
                alertbox("Please enter emailid1.");
                return false;
            }

            if (!emailRegEx.test(document.getElementById("txtemailid1").value)) {
                alertbox("Please valid enter EMail Id.");
                return false;
            }

            if ($('#txtemailidCC1').val() != "") {
                var arr;
                var emls = document.getElementById("txtemailidCC1").value;
                if (emls.indexOf(",") > 0) {
                    var mar = new Array();
                    mar = emls.split(",");
                    arr = new Array();
                    for (var i = 0; i < mar.length; i++) {
                        arr.push($.trim(mar[i]));
                    }
                }
                else if (emls.indexOf(";") > 0) {
                    arr = new Array();
                    var mar = new Array();
                    mar = emls.split(";");
                    for (var i = 0; i < mar.length; i++) {
                        arr.push($.trim(mar[i]));
                    }
                }
                else {
                    arr = new Array();
                    arr.push(emls);
                }
                for (var i = 0; i < arr.length; i++) {
                    if (!emailRegEx.test(arr[i])) {
                        alertbox("Please valid enter EMail Id (CC).");
                        return false;
                    }
                } 
            } 

            var lastXHR = $.get("TrailerMgmtJson.aspx?id=" + $("#txtid1").val() + "&opr=3&insdatetime=" + $('#txtdttm1').val() + "&rtax=" + $('#txtroadtax1').val() + "&pt=" + $('#txtptest1').val() + "&insu=" + $('#txtinsurence1').val() + "&tname=" + $('#txttname1').val() + "&uid=" + $('#ddluser1').val() + "&em1=" + $('#txtemailid1').val() + "&emlcc=" + $('#txtemailidCC1').val() , function (data) {
                if (data == "Yes") { 
                    return false;
                }
                else {
                    alertbox("<b style='Color:Red;'>Sorry.Please try again..!!!</b>"); 
                    var x;
                    return false;
                }
            });

            $("#dialog-form1").dialog("close");
            $('#txtid1').val("");
            $('#txtdttm1').val("");
            $('#txttname1').val("");
            $('#ddluser1').val("");
            refreshTable();
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

            $("#dialog:ui-dialog").dialog("destroy");

            $("#dialog-form1").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
                width: 390,
                buttons: {
                    Update: function () {
                        UpdateData();
                    },
                    Cancel: function () {
                        $(this).dialog("close");
                    }
                }
            });
            $("#dialog-form").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
                width: 390,
                buttons: {
                    Add: function () {
                        AddData();
                    },
                    Cancel: function () {
                        $(this).dialog("close");
                    }
                }
            });
            oTable = $('#examples').dataTable({
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
              
                "aaSorting": [[2, "asc"]],
                "iDisplayLength": 10,
                "aLengthMenu": [5,10, 25],
                "bLengthChange": true,
                "fnDrawCallback": function (oSettings) {
                    if (oSettings.bSorted || oSettings.bFiltered) {
                        if (oSettings.aoColumns[0].bVisible == true) {
                            for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                                $('td:eq(1)', oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
                            }
                        }
                    }
                },
                "sDom": '<"H"Cl<"MyButton">f>rt<"F"iTp>',
                "aoColumnDefs": [
                   { "bVisible": true, "bSortable": false, "aTargets": [0] },
                          { "bVisible": true, "bSortable": false, "aTargets": [1] },
 { "bVisible": true, "sClass": "leftClass", "bSortable": true, "aTargets": [2] },
  { "bVisible": true, "sClass": "leftClass", "bSortable": true, "aTargets": [3] },
   { "bVisible": true, "sClass": "leftClass", "bSortable": true, "aTargets": [4] },
  { "bVisible": true, "sClass": "leftClass", "bSortable": true, "aTargets": [5] },
   { "bVisible": true, "sClass": "leftClass", "bSortable": true, "aTargets": [6] }

       ]
            });

        });
        function openPopup(id, name, insdatetime, roadtax, pustest, insurence, uid, eml1, emlcc) {
            $('#txtid1').val(id);
            $('#txttname1').val(name);
            $('#txtdttm1').val(insdatetime);

            $('#txtroadtax1').val(roadtax);

            $('#txtptest1').val(pustest);

            $('#txtinsurence1').val(insurence);

            $('#ddluser1').val(uid);
            $('#txtemailid1').val(eml1);
            $('#txtemailidCC1').val(emlcc);

            $("#dialog-form1").dialog("open");

        }
        function openPopup1() {
            $('#txttname').val("");
            $('#txtdttm').val("");
            $('#ddluser').val("");
            $('#txtroadtax').val("");
            $('#txtptest').val("");
            $('#txtinsurence').val("");
            $('#txtemailid').val("");
            $('#txtemailidCC').val("");
            $("#dialog-form").dialog("open");
        }
      
      

    </script>
    <style type="text/css">
        .ui-button-text
        {
            font-size: 11px;
        }
        .ui-text
        {
            font-size: 10px;
        }
         tfoot input
        {
            margin: 0.2em 0;
            width: 100%;
            margin-left: -0.7em;
            color: #444;
        }
        .leftClass
        {
            text-align: left;
        }
    </style>
</head>
<body onload="refreshTable()" style="font-size: 11px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
    <form id="form1" runat="server">
    <div>
        <center>
            <br />
            <b style="font-family: Verdana; font-size: 20px; color: #38678B;">
                Trailer Management</b>
            <table style="font-family: Verdana; font-size: 11px; width: 1250px">
                <tr>
                    <td align="left">
                        <input type="button" id="ImageButton1" class="action blue" runat="server" value="Delete"
                            title="Delete Checked Trailer(s)" onclick="javascript:deleteconfirmation()" style="margin-left: 0px;" />
                    </td>
                    <td align="center">
                    </td>
                    <td align="right" valign="middle">
                    <a href="#" onclick="javascript:ExcelReport()" class="button" title="Download All Settings"
                            style="width: 59px; margin-right: 0px;"><span class="ui-button-text">Download </span>
                        </a>
                        <a href="#" onclick="javascript:openPopup1()" class="button" title="Add Trailer"
                            style="width: 59px; margin-right: 0px;"><span class="ui-button-text">Add </span>
                        </a>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                         <div style="width: 1250px;">
                            <table cellpadding="0" cellspacing="0" border="0" class="display" id="examples" style="font-size: 11px;
                                font-weight: normal; font-family: Verdana;width :1240px;" >
                                <thead>
                                    <tr style="text-align: left">
                                        <th style="width: 15px;">
                                            <input type="checkbox" name="chk" class="group1" onclick="javascript:checkall(this);" value="on" />
                                        </th>
                                        <th style="width: 50px;">
                                            SNo
                                        </th>
                                        <th style="width: 140px;">
                                            Trailer Number
                                        </th>
                                        <th style="width: 140px;">
                                            Inspection Date
                                        </th>
                                         <th style="width: 140px;">
                                           Road Tax
                                        </th>
                                        <th style="width: 140px;">
                                          Puspakam Test
                                        </th>
                                        <th style="width: 140px;">
                                            Insurence
                                        </th> 
                                        <th style="width: 150px;">
                                            User Name
                                        </th>
                                         <th style="width: 60px;">
                                           EMailId
                                        </th>
                                         <th >
                                            EMail Id (CC)
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                </tbody>
                                <tfoot>
                                    <tr style="text-align: left">
                                        <th style="width: 15px;">
                                            <input type="checkbox" name="chk" class="group1" onclick="javascript:checkall(this);" value="on"  />
                                        </th>
                                           <th style="width: 50px;">
                                            SNo
                                        </th>
                                        <th style="width: 140px;">
                                            Trailer Number
                                        </th>
                                        <th style="width: 140px;">
                                            Inspection Date
                                        </th>
                                         <th style="width: 140px;">
                                           Road Tax
                                        </th>
                                        <th style="width: 140px;">
                                          Puspakam Test
                                        </th>
                                        <th style="width: 140px;">
                                            Insurence
                                        </th> 
                                        <th style="width: 150px;">
                                            User Name
                                        </th>
                                         <th style="width: 60px;">
                                           EMailId
                                        </th>
                                         <th >
                                            EMail Id (CC)
                                        </th>
                                    </tr>
                                </tfoot>
                            </table>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <input type="button" id="ImageButton2" class="action blue" runat="server" value="Delete"
                            title="Delete Checked Trailer(s)" onclick="javascript:deleteconfirmation()" style="margin-left: 0px;" />
                    </td>
                    <td align="center">
                    </td>
                    <td align="right" valign="middle">
                        <a href="#" onclick="javascript:openPopup1()" class="button" title="Add Trailer"
                            style="width: 59px; margin-right: 0px;"><span class="ui-button-text">Add </span>
                        </a>
                    </td>
                </tr>
            </table>
        </center>
    </div>
    <div id="dialog-form" title="Add Trailer" style="padding-top: 1px; padding-right: 0px;
        padding-bottom: 0px; font-size: 10px; padding-left: 0px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;">
        <table border="0" cellpadding="1" cellspacing="1" style="font-size: 11px; font-weight: bold;">
            <br />
            <tr>
                <td align="left">
                    <b style="color: #4E6CA3;">User</b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:DropDownList runat="server" ID="ddluser" style="width:154px;">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td align="left">
                    <b style="color: #4E6CA3;">Trailer Number</b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:TextBox ID="txttname" runat="server" Width="151px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="left">
                    <b style="color: #4E6CA3;">Inspection Date</b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtdttm" Width="151px" ReadOnly="true" />
                </td>
            </tr>
              <tr>
                <td align="left">
                    <b style="color: #4E6CA3;">
                      Road Tax</b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtroadtax" Width="151px" ReadOnly="true" />
                </td>
            </tr>
            <tr>
                <td align="left">
                    <b style="color: #4E6CA3;">
                       Puspakam Test</b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtptest" Width="151px" ReadOnly="true" />
                </td>
            </tr>
            <tr>
                <td align="left">
                    <b style="color: #4E6CA3;">
                        Insurence</b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtinsurence" Width="151px" ReadOnly="true" />
                </td>
            </tr>
             <tr>
                    <td align="left">
                        <b style="color: #4E6CA3;">Email Id</b>
                    </td>
                    <td style="color: #4E6CA3;">
                        :
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtemailid" Width="234px" />
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <b style="color: #4E6CA3;">Email Id (CC)</b>
                    </td>
                    <td style="color: #4E6CA3;">
                        :
                    </td>
                    <td>
                    <asp:TextBox runat="server" ID="txtemailidCC" Width="234px"   />
                    </td>
                </tr>
        </table>
    </div>
    <div id="dialog-form1" title="Update Trailer" style="padding-top: 1px; padding-right: 0px;
        padding-bottom: 0px; font-size: 10px; padding-left: 0px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif;">
        <table border="0" cellpadding="1" cellspacing="1" style="font-size: 11px; font-weight: bold;">
            <br />
            <tr style="display: none;">
                <td colspan="3">
                    <input type="text" name="t1" value="" id="txtid1" />
                </td>
            </tr>
            <tr>
                <td align="left">
                    <b style="color: #4E6CA3;">User</b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:DropDownList runat="server" ID="ddluser1" style="width:154px;">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td align="left">
                    <b style="color: #4E6CA3;">Trailer Number</b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:TextBox ID="txttname1" runat="server" Width="151px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="left">
                    <b style="color: #4E6CA3;">Inspection Date</b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtdttm1" ReadOnly="true"  Width="151px" />
                </td>
            </tr>
              <tr>
                <td align="left">
                    <b style="color: #4E6CA3;">
                      Road Tax</b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtroadtax1" Width="151px" ReadOnly="true" />
                </td>
            </tr>
            <tr>
                <td align="left">
                    <b style="color: #4E6CA3;">
                       Puspakam Test</b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtptest1" Width="151px" ReadOnly="true" />
                </td>
            </tr>
            <tr>
                <td align="left">
                    <b style="color: #4E6CA3;">
                        Insurence</b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtinsurence1" Width="151px" ReadOnly="true" />
                </td>
            </tr>
             <tr>
                    <td align="left">
                        <b style="color: #4E6CA3;">Email Id</b>
                    </td>
                    <td style="color: #4E6CA3;">
                        :
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtemailid1" Width="234px" />
                    </td>
                </tr>
                <tr>
                    <td align="left">
                        <b style="color: #4E6CA3;">Email Id (CC)</b>
                    </td>
                    <td style="color: #4E6CA3;">
                        :
                    </td>
                    <td>
                    <asp:TextBox runat="server" ID="txtemailidCC1" Width="234px"   />
                    </td>
                </tr>
        </table>
    </div>
     <input type="hidden" name="uid" value="" runat="server" id="uid" />
        <input type="hidden" name="rle" value="" runat="server" id="rle" />
        <input type="hidden" name="ulist" value="" runat="server" id="ulist" />
    <div id="div1" title="Confirm" style="font-size: 11px; font-weight: normal;">
        <p id="P1">
            <span class="ui-icon ui-icon-circle-check" style="float: left; margin: 0 7px 50px 0;">
            </span>
        </p>
    </div>
    <div id="dialog-msg" title="Alert" style="font-size: 11px; font-weight: normal;">
        <p id="displayp">
            <span class="ui-icon ui-icon-circle-check" style="float: left; font-size: 7px; margin: 0 7px 50px 0;">
            </span>
        </p>
    </div>
    
    </form>
    <form id="excelform" method="get" action="ExcelReport.aspx">
    <input type="hidden" id="title" name="title" value="Trailer Alerts Settings" />
    <input type="hidden" id="plateno" name="plateno" value="" />
    </form>
</body>
</html>
