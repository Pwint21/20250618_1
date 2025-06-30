<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.TrailerManagementNew" Codebehind="TrailerManagementNew.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>TrailerManagement</title>
    <style type="text/css" media="screen">
        @import "css/demo_table_jui.css";
        @import "css/jquery-ui.css";
        @import "css/TableTools.css";
        @import "css/ColVis.css";
        @import "css/common1.css";
        @import "css/chosen.css";
    </style>

    <script type="text/javascript" src="js/googana.js"></script>
    <script type="text/javascript" language="javascript" src="js/jquery.js"></script>
    <script type="text/javascript" src="js/jquery_ui.js"></script>
    <script type="text/javascript" language="javascript" src="js/jquery.dataTables.js"></script>
    <script type="text/javascript" language="javascript" src="js/FixedColumns.js"></script>
    <style type="text/css">
        .dataTables_info
        {
            width: 32%;
            float: left;
        }
        
        
        .ui-widget-header
        {
            border: 0px solid #4297d7;
        }
        
        
        table.display thead th div.DataTables_sort_wrapper
        {
            position: relative;
            padding-right: 6px;
        }
        table.display tfoot th
        {
            padding: 0px 0px 0px 0px;
            padding-left: 3px;
        }
        table.display thead th
        {
            padding: 0px 0px 0px 0px;
            padding-left: 3px;
            height: 31px;
        }
        table.display td
        {
            padding: 2px 2px;
        }
        .dataTables_filter
        {
            width: 300px;
        }
        
        .MyButton
        {
            text-align: left;
            float: left;
            width: 350px;
        }
        
        
        .chzn-container .chzn-results
        {
            color: #4E6CA3;
        }
        .chzn-container .chzn-results
        {
            max-height: 300px;
        }
        
        .textbox1
        {
            height: 18px;
            width: 180px;
            border-right: #cbd6e4 1px solid;
            border-top: #cbd6e4 1px solid;
            border-left: #cbd6e4 1px solid;
            color: #0b3d62;
            border-bottom: #cbd6e4 1px solid;
        }
        .button
        {
            text-decoration: none;
            text-shadow: 0 1px 0 #fff;
            text-align: center;
            font: bold 11px Helvetica, Arial, sans-serif;
            color: #444;
            height: 24px;
            display: inline-block;
            width: 60px;
            margin: 5px;
            padding: 0px 3px 2px 2px;
            background: #F3F3F3;
            border: solid 1px #D9D9D9;
            border-radius: 2px;
            -webkit-border-radius: 2px;
            -moz-border-radius: 2px;
            -webkit-transition: border-color .20s;
            -moz-transition: border-color .20s;
            -o-transition: border-color .20s;
            transition: border-color .20s;
        }
        button
        {
            cursor: pointer;
        }
        a.button:active
        {
            border-color: #4B8DF8;
        }
        a.button:hover
        {
            color: White;
            text-shadow: 0 1px 0 #fff;
            border: 1px solid #2F5BB7 !important;
            background: #3F83F1;
            background: -webkit-linear-gradient(top, #4D90FE, #357AE8);
            background: -moz-linear-gradient(top, #4D90FE, #357AE8);
            background: -ms-linear-gradient(top, #4D90FE, #357AE8);
            background: -o-linear-gradient(top, #4D90FE, #357AE8);
        }
        a.button
        {
            text-align: center;
            font: bold 11px Helvetica, Arial, sans-serif;
            cursor: pointer;
            text-shadow: 0 1px 0 #fff;
            display: inline-block;
            width: 74px;
            border: 1px solid #3079ED !important;
            color: White;
            height: 14px;
            background: #4B8DF8;
            background: -webkit-linear-gradient(top, #4C8FFD, #4787ED);
            background: -moz-linear-gradient(top, #4C8FFD, #4787ED);
            background: -ms-linear-gradient(top, #4C8FFD, #4787ED);
            background: #4B8DF8;
            -webkit-transition: border .20s;
            -moz-transition: border .20s;
            -o-transition: border .20s;
            transition: border .20s;
            margin: 5px;
            padding: 3px 5px 5px 3px;
        }
        button, .button
        {
            width: inherit;
            height: inherit;
        }
        tfoot input {
    margin: 0.5em 0;
    width: 98%;
    color: #444;
}
        .dataTables_info {
    width: 24%;
    float: left;
}
    </style>
    <script type="text/javascript" language="javascript">


        function validate() {
            if (document.getElementById("ddluserid").value == "SELECT USERNAME") {
                alertbox("<%=Literal19.Text%>");
                return false;
            }
            else if (document.getElementById("txttrailerid").value == "") {
                alertbox("Please Enter Trailer ID..!");
                return false;
            }  
            else if (document.getElementById("txttrailerno").value == "" && $("#opr").val()=="0") {
                alertbox("Please Enter Trailer No..!");
                return false;
            }
            else {
                return true;
            }
        }

       function ExcelReport() {
            var excelformobj = document.getElementById("excelform");
            excelformobj.submit();

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


	function refreshTable() {
			table = oTable.dataTable();
			table._fnProcessingDisplay(true);
            oSettings = table.fnSettings();
      
			$.getJSON('GetTrailerData.aspx?u='+$('#ddluser1').val()+'&role='+$('#rle').val()+'&userslist='+$('#ulist').val()+'&r=' + Math.random(), null, function (json) {
				table.fnClearTable(this);

				for (var i = 0; i < json.aaData.length; i++) {
					table.oApi._fnAddData(oSettings, json.aaData[i]);
				}
				oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
				table._fnProcessingDisplay(false);

				table.fnDraw();
			});
			return false;
		}

     
  function DeleteTailer() {

            var checked = false;
            var matches = [];
            $(".chkTrailer:checked").each(function () {
                matches.push(this.value);
            });

      $.getJSON('GetTrailerData.aspx?ugData=' + matches + '&ddata=0&r=' + Math.random(), null, function (json) {
          var response = json.result;
          if (response != 0)
          {
              $("#dialog-message").dialog("close");                      
              refreshTable();
          }
       });	
          
            //                checked = false;
            $(".chkTrailer:checked").each(function () {
                $(this).removeAttr('checked');
            });
        }

        var oTable;
       
        
        $(document).ready(function () {
            $("#dialog-message").hide();
            $("#dialog:ui-dialog").dialog("destroy");
            $("#dialog-msg").dialog({
                autoOpen: false,
                resizable: false,
                height: 140,
                modal: true,
                buttons: {
                    "<%=Literal38.Text%>": function () {
                        $(this).dialog("close");
                    }

                }
            });


            $("#div1").dialog({
                resizable: false,
                draggable: false,
                height: 160,
                modal: true,
                autoOpen: false,
                buttons: {
                    "<%=Literal36.Text%>": function () {
                       
                        DeleteTailer();
                        $(this).dialog("close");
                    },
                    "<%=Literal37.Text%>": function () {
                       
                        $(this).dialog("close");

                    }
                }
            });

            $("#dialog-message").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
                width: 350,
                minHeight: 180,
                height: 180

            });

            fnFeaturesInit();
            oTable = $('#examples').dataTable({
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                "sAjaxSource": "GetTrailerData.aspx?u=<%=suserid %>&role="+$('#rle').val()+"&userslist="+$('#ulist').val()+"",
                "iDisplayLength": 100,
                "aLengthMenu": [10, 25, 50, 100, 200, 500],
                "bLengthChange": false,
                "fnDrawCallback": function (oSettings) {
                    if (oSettings.bSorted || oSettings.bFiltered) {
                        if (oSettings.aoColumns[0].bVisible == true) {
                            for (var i = 0, iLen = oSettings.aiDisplay.length; i < iLen; i++) {
                                $('td:eq(1)', oSettings.aoData[oSettings.aiDisplay[i]].nTr).html(i + 1);
                            }
                        }
                    }
                },
                "oLanguage": {
                    "oPaginate": {
                        "sNext": "<%=Literal25.Text %>",
                        "sFirst": "<%=Literal23.Text %>",
                        "sLast": "<%=Literal26.Text %>",
                        "sPrevious": "<%=Literal24.Text %>"
                    },
                    "sSearch": "<%=Literal9.Text %>",
                    "sEmptyTable": "<%=Literal21.Text %>",
                    "sInfo": "<%=Literal22.Text %>",
                    "sInfoFiltered": "<%=Literal43.Text%>",
                    "sZeroRecords":"<%=Literal44.Text%>",
                    "sInfoEmpty": "<%=Literal45.Text%>",
                    "sLoadingRecords":"<%=Literal46.Text%>",
                    "sProcessing": "<%=Literal47.Text%>",
                },
                "sDom": '<"H"Cl<"MyButton">f>rt<"F"iTp>',
                "aoColumnDefs": [

                          { "sClass": "center", "sWidth": "10px", "aTargets": [0], "bSortable": false, "bVisible": true,
                              "fnRender": function (oData, sVal) {
                                  return "<input type='checkbox' name='chk' class='chkTrailer' value='" + sVal + "' />";
                                 
                              }

                          },
                             { "bVisible": true, "bSortable": false, "sWidth": "40px", "aTargets": [1] },
                            { "bVisible": true, "sWidth": "80px", "bSortable": true, "aTargets": [2] },
                            { "bVisible": true, "sWidth": "80px", "bSortable": true, "aTargets": [3] },
                            { "bVisible": true, "sWidth": "90px", "bSortable": true, "aTargets": [4] }                          
                             ]
            });
            $("div.MyButton").html('<div><table><tr><td><%=Literal2.Text%> : </td><td><%=opt %></td></tr> </table></div>');
            jQuery(".chosen").data("placeholder", "<%=Literal39.Text%>...").chosen();
            //refreshTable();
        });
        function alertbox(message) {
            if (message == "") {
                document.getElementById("displayp").innerHTML = message;

            }
            else {
                document.getElementById("displayp").innerHTML = message;
                $("#dialog-msg").dialog("open");
            }
        }

        function openAddPopup() {
             $("#txttrailerno").val("");      
            Clearfields();
            $("#opr").val(0);
            $("#dialog-message").dialog({
                title: "Add Trailer",
                buttons: {
                    "<%=Literal14.Text%>": function () {
                        UpdateData("0");
                    },
                    "<%=Literal12.Text%>": function () {
                        $(this).dialog("close");

                    }
                }
            });

            $("#dialog-message").dialog("open");
        }
        function openUpdatePopup(id, trailerid,trailerno, userid) {          
            $("#opr").val(1);
            $("#ddluserid").val(userid);
            $("#txttrailerid").val(trailerid);
             $("#txttrailerno").val(trailerno);
            $("#dialog-message").dialog({
                title: "Update Trailer",
                buttons: {
                    "<%=Literal11.Text%>": function () {
                        UpdateData(id);
                    },
                    "<%=Literal12.Text%>": function () {
                        $(this).dialog("close");

                    }
                }
            });

            $("#dialog-message").dialog("open");
        }



        function UpdateData(id) {
            var res = validate();
            var opr = $("#opr").val();            
            if (res == true) {
            $.getJSON('GetTrailerData.aspx?u='+$('#ddluserid').val()+'&trailerid='+$('#txttrailerid').val()+'&trailerno='+$('#txttrailerno').val()+'&tid='+id+'&opr='+$('#opr').val()+'&r=' + Math.random(), null, function (json) {
                var response = json.result;                
                if (opr == 0 && response == 1) {
                          $("#dialog-message").dialog("close");
                        alertbox("<%=Literal32.Text%>");                       
                         refreshTable();
                    }
                else if (opr == 1 && response == 1) {
                      $("#dialog-message").dialog("close");
                        alertbox("<%=Literal31.Text%>");
                         refreshTable();
                }
                else if (response==2) {
                     $("#dialog-message").dialog("close");
                        alertbox("Trailer ID already exist...!");
                         refreshTable();
                }
                else {
                      $("#dialog-message").dialog("close");
                    alertbox("<%=Literal30.Text%>");
                    refreshTable();
                    }
                     
			});			
               
            }


        }
        function Clearfields() {
            if ($("#rle").val() != "User")
                $('#ddluserid').val("--Select User Name--");

            $("#txttrailerid").val("");           
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
            for (i = 0; i < document.forms[0].elements.length; i++) {
                elm = document.forms[0].elements[i]
                if (elm.type == 'checkbox') {
                    if (elm.checked == true) {
                        checked = true;
                        break;
                    }
                }
            }
            if (checked) {
                var result = confirm("Are you sure you want to delete selected Trailer(s) ?");
                if (result) {
                    return true;
                }
                return false;
            }
            else {
                alertbox("Please select atleast one Trailer");
                return false;
            }
        }

    </script>
    <link href="css/chosen.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="js/chosen.jquery.js"></script>
</head>
<body style="font-size: 11px; font-weight: normal; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
    <form id="form1" runat="server">
    <asp:Literal ID="Literal1" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, mPOIManagement%>" />
    <asp:Literal ID="Literal2" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, u1%>" />
    <asp:Literal ID="Literal3" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, sNo%>" />
    <asp:Literal ID="Literal4" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, tit3%>" />
    <asp:Literal ID="Literal5" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, tit1%>" />
    <asp:Literal ID="Literal6" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, sLat%>" />
    <asp:Literal ID="Literal7" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, sLon%>" />
    <asp:Literal ID="Literal8" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, g4%>" />
    <asp:Literal ID="Literal9" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, sFil%>" />
    <asp:Literal ID="Literal22" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, sall%>" />
    <asp:Literal ID="Literal21" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, g6%>" />
    <asp:Literal ID="Literal23" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, g10%>" />
    <asp:Literal ID="Literal24" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, g8%>" />
    <asp:Literal ID="Literal25" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, g7%>" />
    <asp:Literal ID="Literal26" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, g9%>" />
    <asp:Literal ID="Literal10" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, poi%>" />
    <asp:Literal ID="Literal11" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, btn6%>" />
    <asp:Literal ID="Literal12" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, btn1%>" />
    <asp:Literal ID="Literal13" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, aPoi%>" />
    <asp:Literal ID="Literal14" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, atp4%>" />
    <asp:Literal ID="Literal16" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, apoi1%>" />
    <asp:Literal ID="Literal17" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, btn5%>" />
    <asp:Literal ID="Literal18" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, apoi2%>" />
    <asp:Literal ID="Literal19" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, poiu%>" />
    <asp:Literal ID="Literal20" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, ent%>" />
    <asp:Literal ID="Literal27" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, msm2%>" />
    <asp:Literal ID="Literal28" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, msm3%>" />
    <asp:Literal ID="Literal29" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, lst27%>" />
    <asp:Literal ID="Literal30" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, apoi4%>" />
    <asp:Literal ID="Literal31" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, apoi5%>" />
    <asp:Literal ID="Literal32" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, apoi6%>" />
    <asp:Literal ID="Literal33" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, apoi7%>" />
    <asp:Literal ID="Literal34" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, apoi8%>" />
    <asp:Literal ID="Literal35" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, apoi9%>" />
    <asp:Literal ID="Literal36" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, apoi10%>" />
    <asp:Literal ID="Literal37" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, apoi11%>" />
    <asp:Literal ID="Literal38" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, vm28%>" />
    <asp:Literal ID="Literal39" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, lst99%>" />
    <asp:Literal ID="Literal40" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, lst100%>" />
    <asp:Literal ID="Literal43" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, alt19%>" />
    <asp:Literal ID="Literal44" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, alt20%>" />
    <asp:Literal ID="Literal45" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, alt21%>" />
    <asp:Literal ID="Literal46" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, alt22%>" />
    <asp:Literal ID="Literal47" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, alt23%>" />
    <asp:Literal ID="Literal15" Visible="false" runat="server" Text="<%$ Resources:chienvh.language, tm14%>" />
    <center>
        <br />
        <br />
        <div class="c1">
            &nbsp;Trailer Management </div>
        <br />
        <table style="font-family: Verdana; font-size: 11px; width: 1000px">
            <tr>
                <td align="left">
                      <a  class="button" title="Delete Trailer" style="width: 59px; ">
                        <span class="ui-button-text"  onclick="deleteconfirmation()" ><%=Literal17.Text%> </span></a>
                </td>
                <td align="center">
                </td>
                <td align="right" valign="middle" style="margin-right:0px;">
                    <a  class="button" title="Add New Trailer" style="width: 79px; " onclick="openAddPopup()">
                        <span class="ui-button-text"><%=Literal14.Text%> </span></a>
                        <a href="javascript:ExcelReport();" class="button" style="margin-right: 3px; width: 75px;">
                       Save Excel</a>
                </td>
            </tr>
            <tr>
                <td colspan="3" style="width: 1000px;">
                    <div  id="fw_container" >
                        <table  style="font-family: Verdana; font-size: 11px; ">
                            <tr>
                                <td colspan="3" align="center">
                                    <table cellpadding="0" cellspacing="0" border="0" class="display" id="examples"  style="font-family: Verdana; font-size: 11px; width: 1000px">
                                        <thead align="left">
                                            <tr>
                                              <th align="center" style="width: 20px;">
                                               <input type="checkbox" name="chk" class="chkTrailer"  onclick="javascript:checkall(this);" />
                                               </th>
                                                <th style="width: 50px;">
                                                 No
                                                </th>
                                                <th>
                                                   Trailer ID
                                                </th>
                                                <th>
                                                    Trailer NO
                                                </th>
                                                <th>
                                                   Modify DateTime
                                                </th>                                             
                                            </tr>
                                        </thead>
                                        <tbody>
                                        </tbody>
                                        <tfoot align="left">
                                            <tr >
                                                <th>
                                                   <input type="checkbox" id="Checkbox1" class="chkTrailer" onclick=" checkall(this)" />
                                                 </th>
                                              <th style="width: 50px;">
                                                 No
                                                </th>
                                                <th>
                                                   Trailer ID
                                                </th>
                                                <th>
                                                    Trailer NO
                                                </th>
                                                <th>
                                                   Modify DateTime
                                                </th>  
                                            </tr>
                                        </tfoot>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
            <tr>
                <td align="left">
                 <a  class="button" title="Delete Trailer" style="width: 59px; ">
                        <span class="ui-button-text"  onclick="deleteconfirmation()" ><%=Literal17.Text%> </span></a>
                    
                </td>
                <td align="center">
                </td>
                <td align="right" valign="middle" style="margin-right:0px;">
                      <a  class="button" title="Add New Trailer" style="width: 79px; " onclick="openAddPopup()">
                        <span class="ui-button-text"><%=Literal14.Text%> </span></a>
                </td>
            </tr>
        </table>
        <input type="hidden" id="opr" runat="server" />
        <input type="hidden" name="uid" value="" runat="server" id="uid" />
        <input type="hidden" name="rle" value="" runat="server" id="rle" />
        <input type="hidden" name="ulist" value="" runat="server" id="ulist" />
        <input type="hidden" id="gid" value="" runat="server" />
        <input type="hidden" value="" id="ss" runat="server" />
    </center>
    <div id="div1" title="<%=Literal35.Text%>">
        <p id="P1">
            <span class="ui-icon ui-icon-circle-check" style="float: left; margin: 0 7px 50px 0;">
            </span>
        </p>
    </div>
    <div id="dialog-msg" title="<%=Literal29.Text%>">
        <p id="displayp">
            <span class="ui-icon ui-icon-circle-check" style="float: left; margin: 0 7px 50px 0;">
            </span>
        </p>
    </div>
    <div id="dialog-message" title="Add Unit" align="center" style="padding-top: 1px;
        padding-right: 0px; padding-bottom: 0px; font-size: 11px; padding-left: 5px;
        font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
        <table border="0" cellpadding="1" cellspacing="1" style="width: 320px; font-size: 11px;
            font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Verdana,sans-serif;
            vertical-align: middle;">
            <br />
            <tr id="txttr" align="left">
                <td align="left">
                    <b style="color: #4E6CA3;">
                      User Name
                    </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:DropDownList ID="ddluserid" runat="server" Width="182px" Height="20px" TabIndex="10">
                    </asp:DropDownList>
                </td>
            </tr>   
              <tr align="left">
                <td align="left">
                    <b style="color: #4E6CA3;">
                       Trailer ID
                    </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:TextBox ID="txttrailerid" runat="Server" CssClass="textbox1" TabIndex="11" />
                </td>
            </tr>
                 <tr align="left" id="trtrailerno">
                <td align="left">
                    <b style="color: #4E6CA3;">
                       Trailer No
                    </b>
                </td>
                <td style="color: #4E6CA3;">
                    :
                </td>
                <td>
                    <asp:TextBox ID="txttrailerno" runat="Server" CssClass="textbox1" TabIndex="11" />
                </td>
            </tr>
        </table>
    </div>
    </form>
      <form id="excelform" method="get" action="ExcelReport.aspx">
    <input type="hidden" id="title" name="title" value="Trailer Management" />
    <input type="hidden" id="titl1" name="titl1" value="Trailer Management" />
    <input type="hidden" id="rd" name="rd" value="Report Date" />
  
   
    </form>
</body>
</html>
