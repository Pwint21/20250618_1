<%@ Page Language="VB" AutoEventWireup="false"
    Inherits="YTLWebApplication.TransporterSummary" EnableViewState="false" EnableEventValidation="false" Codebehind="TransporterSummary.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>YTL - Transporter Summary Report</title>
    <style type="text/css" media="screen">
        @import "cssfiles/demo_page.css";
        @import "cssfiles/demo_table_jui.css";
        @import "cssfiles/themes/redmond/jquery-ui-1.8.4.custom.css";

        .dataTables_info {
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


        $(function () {
            $("#txtBeginDate").datepicker({
                dateFormat: 'yy/mm/dd', minDate: -180, maxDate: -1, changeMonth: true, changeYear: true, numberOfMonths: 2
            });

            $("#txtEndDate").datepicker({
                dateFormat: 'yy/mm/dd', minDate: -180, maxDate: +0, changeMonth: true, changeYear: true, numberOfMonths: 2

            });

            $("#dialog:ui-dialog").dialog("destroy");

            $("#dialog-message").dialog({
                resizable: false,
                draggable: false,
                modal: true,
                autoOpen: false,
                minWidth: 1000,
                minHeight: 529
            });
        });


        function openPopUp(plateno, intime, outtime, gid) {
            document.getElementById("mappage").src = "ViewDetails.aspx?bdt=" + intime + "&edt=" + outtime + "&pno=" + plateno + "&gid=" + gid + "&r=" + Math.random();
            document.getElementById("mappage").style.visibility = "visible";
            $("#dialog-message").dialog("open");
        }


        function mysubmit() {
            return true;

        }
        var ec =<%=ec %>;
        function ExcelReport() {
            if (ec == true) {

                var excelformobj = document.getElementById("excelform");
                excelformobj.submit();
            }
            else {
                alert("First click submit button");
            }
        }
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
        var oTable, oTable2;
        $(document).ready(function () {
            fnFeaturesInit()
            oTable = $('.display').dataTable({
                "bJQueryUI": true,
                "sPaginationType": "full_numbers",
                "iDisplayLength": 200,
                "aaSorting": [[1, "asc"]],
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
                    { "aDataSort": [1, 9], "aTargets": [1] },
                    { "aDataSort": [4, 1, 9], "aTargets": [4] }
                ]
            });
            oTable.fnSetColumnVis(2, false);

        });



    </script>
    <style type="text/css">
        table.display td {
            padding: 2px 2px;
        }
    </style>
</head>
<body id="index" style="font-size: 11px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
    <form id="Form1" method="post" runat="server">
        <center>
            <div>
                <br />
                <b style="font-family: Verdana; font-size: 20px; color: #38678B;">YTL - Transporter
                    Summary Report</b>
                <br />
                <br />
            </div>
            <table>
                <tr>
                    <td align="center">
                        <table style="font-family: Verdana; font-size: 11px;">
                            <tr>
                                <td colspan="2" style="height: 20px; background-color: #465ae8;" align="left">
                                    <b style="color: White;">&nbsp;YTL - Transporter Summary Report&nbsp;:</b>
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
                                                            style="color: #465AE8;">&nbsp;
                                                </td>
                                            </tr>
                                            <tr>
                                                <td align="center">
                                                    <br />
                                                </td>
                                                <td colspan="2" align="center">
                                                    <br />
                                                    <asp:Button ID="ImageButton1" class="action blue" runat="server" Text="Submit" ToolTip="Submit" />
                                                    <a href="javascript:ExcelReport();" class="button"><span class="ui-button-text" title="Download">
                                                        Download</a>
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
                        <div style="width: 100%;">
                            <%--  <%If ec = True Then%>
                            <%=sb1.ToString()%>
                            <%=sb2.ToString()%>
                            <%End If%>--%>
                            <asp:GridView runat="server" OnRowDataBound="gvCargo_RowDataBound" ID="gvCargo" CellPadding="4"
                                ForeColor="#333333"
                                GridLines="None">
                                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                <EditRowStyle BackColor="#999999" />
                                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                            </asp:GridView>
                            <br />
                            <br />
                            <br />
                            <asp:GridView runat="server" OnRowDataBound="gvTanker_RowDataBound" ID="gvTanker"
                                CellPadding="4" ForeColor="#333333"
                                GridLines="None">
                                <AlternatingRowStyle BackColor="White" />
                                <EditRowStyle BackColor="#7C6F57" />
                                <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                                <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
                                <RowStyle BackColor="#E3EAEB" />
                                <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
                            </asp:GridView>
                        </div>
                    </td>
                </tr>
            </table>
        </center>

    </form>
    <form id="excelform" method="get" action="ExcelReport.aspx">
        <input type="hidden" id="plateno" name="plateno" value="" />
        <input type="hidden" id="title" name="title" value="Transport Summary Report" />
    </form>
    <div class="demo">
        <div id="dialog-message" title="Information" style="padding-top: 1px; padding-right: 0px;
            padding-bottom: 0px; font-size: 10px; padding-left: 0px; font-weight: bold; font-family: Myriad Pro,Lucida Grande,Helvetica,Arial,sans-serif">
            <iframe id="mappage" name="mappage" src="" frameborder="0" scrolling="auto" height="500"
                width="998px" style="visibility: hidden; border: solid 1px #aac6ff;" />
        </div>
    </div>
</body>
</html>
