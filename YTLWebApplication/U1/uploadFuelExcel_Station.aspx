<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.uploadFuelExcel_Station" Debug="true" Codebehind="uploadFuelExcel_Station.aspx.vb" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Upload Shell Fuel</title>
<meta name="viewport" content="initial-scale=1, maximum-scale=1, user-scalable=no">

   
    <link href="bootflat/css/site.min.css" rel="stylesheet" type="text/css" />  

<link href="https://fonts.googleapis.com/css?family=Open+Sans:400,300,600,800,700,400italic,600italic,700italic,800italic,300italic" rel="stylesheet" type="text/css">

    <script type="text/javascript" src="bootflat/js/jquery-1.10.1.min.js"></script>
    <script type="text/javascript" src="bootflat/js/site.min.js"></script>
    
    <script type="text/javascript">

        $(document).ready(function() {
        $("[data-toggle='tooltip']").tooltip(); 
    $("#loadingImage").hide();
    $(".spinner").hide();
        $("#btnUpload").click(function(){
            //$('#btnUpload').prop('disabled', true);
          $("#loadingImage").show();
          $(".spinner").show();
        });
    });
        
    </script>

<style type="text/css">
.center {
   width: 700px;
margin-top:30px;
  margin-left:auto;
margin-right:auto;
}
</style>

<style type="text/css">
.spinner {
  margin: 20px auto;
  width: 50px;
  height: 30px;
  text-align: center;
  font-size: 10px;
}

.spinner > div {
  background-color: #333;
  height: 50px;
  width: 6px;
  display: inline-block;
  
  -webkit-animation: stretchdelay 1.2s infinite ease-in-out;
  animation: stretchdelay 1.2s infinite ease-in-out;
}

.spinner .rect2 {
  -webkit-animation-delay: -1.1s;
  animation-delay: -1.1s;
  color: blue;
}

.spinner .rect3 {
  -webkit-animation-delay: -1.0s;
  animation-delay: -1.0s;
}

.spinner .rect4 {
  -webkit-animation-delay: -0.9s;
  animation-delay: -0.9s;
}

.spinner .rect5 {
  -webkit-animation-delay: -0.8s;
  animation-delay: -0.8s;
}

@-webkit-keyframes stretchdelay {
  0%, 40%, 100% { -webkit-transform: scaleY(0.4) }  
  20% { -webkit-transform: scaleY(1.0) }
}

@keyframes stretchdelay {
  0%, 40%, 100% { 
    transform: scaleY(0.4);
    -webkit-transform: scaleY(0.4);
  }  20% { 
    transform: scaleY(1.0);
    -webkit-transform: scaleY(1.0);
  }
}
</style>

</head>
<body style="background-color: #f1f2f6;">
    <form id="frmUpload" enctype="multipart/form-data" runat="server">
        
            <div class="example center">
                <div class="row">
                  <div >
                    <div class="panel panel-info">
                      <div class="panel-heading">
                        <h3 class="panel-title">Upload Fuel Receipt to System</h3>
                      </div>
                      <div class="panel-body">
                      
                         <!-- drop down menu -->
                                                    
                                                     <div class="row">
                                                      <div class="col-xs-12">
                                                        <label>User :</label><br />
                                                    <asp:DropDownList ID="ddlusername" class="btn btn-info dropdown-toggle" data-toggle="dropdown" runat="server" Width="200px" Font-Size="12px" Font-Names="verdana"
                                                    EnableViewState="true">
                                                    <asp:ListItem>--Select User Name--</asp:ListItem>
                                                </asp:DropDownList>
                                                </div>
                                                    </div>
                                                    
                                                    
                                                   <div class="row">
                                                      <div class="col-xs-12">
                                                        <label>Excel File :</label>
                                                                         <asp:FileUpload ID="MyUpload" runat="server" Style="border: 1px solid #cbd6e4; font-size: 10pt; color: #0b3d62;" Width="350px" EnableViewState="true" />
                                                      </div>
                                                    </div>
                                                    
                                                    
                                                    <div class="row">
                                                      <div class="col-xs-12">
                                                        <label>Date Time :</label><br />
                                                      <asp:DropDownList ID="ddlMonth" class="btn btn-info dropdown-toggle" data-toggle="dropdown" runat="server">
                                                        <asp:ListItem>Month</asp:ListItem>
                                                        <asp:ListItem Value="1">January</asp:ListItem>
                                                        <asp:ListItem Value="2">February</asp:ListItem>
                                                        <asp:ListItem Value="3">March</asp:ListItem>
                                                        <asp:ListItem Value="4">April</asp:ListItem>
                                                        <asp:ListItem Value="5">May</asp:ListItem>
                                                        <asp:ListItem Value="6">June</asp:ListItem>
                                                        <asp:ListItem Value="7">July</asp:ListItem>
                                                        <asp:ListItem Value="8">August</asp:ListItem>
                                                        <asp:ListItem Value="9">September</asp:ListItem>
                                                        <asp:ListItem Value="10">October</asp:ListItem>
                                                        <asp:ListItem Value="11">November</asp:ListItem>
                                                        <asp:ListItem Value="12">December</asp:ListItem>
                                                    </asp:DropDownList>
                                                    
                                                    
                                                    <asp:DropDownList ID="ddlYear" class="btn btn-info dropdown-toggle" data-toggle="dropdown" runat="server">
                                                        <asp:ListItem>Year</asp:ListItem>
                                                        <asp:ListItem>2015</asp:ListItem>
                                                        <asp:ListItem>2014</asp:ListItem>
                                                        <asp:ListItem>2013</asp:ListItem>
                                                    </asp:DropDownList>
                                                      
                                                       </div>
                                                    </div>
                                                    
                                                    <div class="row">
                                                      <div class="col-xs-12">
                                                        <label>Excel format From </label><br />
                                                            <div class="row">
                                                                      <div class="col-md-3">
                                                                        <div class="radio">
                                                                          <div class="iradio_flat" style="position: relative;"><input type="radio" runat="server" id="radiostation1" name="stationname" style="position: absolute; top: -20%; left: -20%; display: block; width: 140%; height: 140%; margin: 0px; padding: 0px; border: 0px; opacity: 0; background: rgb(255, 255, 255);"><ins class="iCheck-helper" style="position: absolute; top: -20%; left: -20%; display: block; width: 140%; height: 140%; margin: 0px; padding: 0px; border: 0px; opacity: 0; background: rgb(255, 255, 255);"></ins></div>
                                                                          <label for="radiostation1" data-toggle="tooltip" data-placement="bottom" title="" data-original-title="">Shell</label>
                                                                        </div>
                                                                      </div>
                                                                      <div class="col-md-3">
                                                                        <div class="radio">
                                                                          <div class="iradio_flat" style="position: relative;"><input type="radio" runat="server" id="radiostation2" name="stationname" style="position: absolute; top: -20%; left: -20%; display: block; width: 140%; height: 140%; margin: 0px; padding: 0px; border: 0px; opacity: 0; background: rgb(255, 255, 255);"><ins class="iCheck-helper" style="position: absolute; top: -20%; left: -20%; display: block; width: 140%; height: 140%; margin: 0px; padding: 0px; border: 0px; opacity: 0; background: rgb(255, 255, 255);"></ins></div>
                                                                          <label for="radiostation2" data-toggle="tooltip" data-placement="bottom" title="" data-original-title="">Petron</label>
                                                                        </div>
                                                                      </div>
                                                                      <div class="col-md-6">
                                                                        <div class="radio">
                                                                          <div class="iradio_flat" style="position: relative;"><input type="radio" runat="server" id="radiostation3" name="stationname" style="position: absolute; top: -20%; left: -20%; display: block; width: 140%; height: 140%; margin: 0px; padding: 0px; border: 0px; opacity: 0; background: rgb(255, 255, 255);"><ins class="iCheck-helper" style="position: absolute; top: -20%; left: -20%; display: block; width: 140%; height: 140%; margin: 0px; padding: 0px; border: 0px; opacity: 0; background: rgb(255, 255, 255);"></ins></div>
                                                                          <label for="radiostation3" data-toggle="tooltip" data-placement="bottom" title="" data-original-title="">Following guides provided as below</label>
                                                                        </div>
                                                                      </div>
                                                             </div>
                                                      </div>
                                                    </div>
                                                    
                                                    <div class="modal-footer">
                                                         <asp:Button ID="btnUpload" class="btn btn-primary"  runat="server" Text="Upload" />
                                                      </div>
                                                      
                                                      
                                                      <!--hr class="dashed" /-->
                                                      
                                                      
                                                      <!--div class="row">
                                                      <div class="col-xs-6">
                                                        <label>User ID :</label>
                                                                         <asp:TextBox ID="TextBox2" class="form-control"  placeholder="waiting for uploading..." runat="server"></asp:TextBox>
                                                      </div>
                                                      <div class="col-xs-6">
                                                        <label>Directory :</label>
                                                             <asp:TextBox ID="TextBox1" class="form-control" placeholder="waiting for uploading..." runat="server"></asp:TextBox>
                                                      </div>
                                                    </div>
                                                    
                                                    
                                                    <div class="modal-footer">
                                                        <asp:Button ID="btnProcess" class="btn btn-danger" runat="server" Text="Process" />
                                                         <asp:Image ID="loadingImage" runat="server" ImageUrl="~/images/information_balloon.png" />
                                                     </div-->
                                                    
                                                     
                                                    <div class="spinner">
                                                      <div class="rect1"></div>
                                                      <div class="rect2"></div>
                                                      <div class="rect3"></div>
                                                      <div class="rect4"></div>
                                                      <div class="rect5"></div>
                                                    </div> 
                                                      
                                                      <div  style="margin-left: auto; margin-right: auto;  width:600px;">
                                                      <span class="label label-danger"><%=s_excel%></span>
                                                      <span class="label label-danger"><%=s_record%></span>
                                                      <span class="label label-danger"><%=s_matching%></span>
                                                      <span class="label label-danger"><%=s_total%></span>
                                                        <span class="label label-danger"><%= s_error %></span></div>
                                                        </div>
                                                        
                                                       
                    </div>
                  </div>
                </div>
              </div>
              
               <div class="row center" >
              <div class="col-md-12">
                <div class="alert alert-warning alert-dismissable">
                  <button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>
                  <h3>Guides on uploading Refuel Receipt</h3>
                  <br />
                  <strong>File Format</strong> Only Excel (*.xls), not include Excel 2007 (*.xlsx).
                  <br />
                  <br />
                  <strong>Column Name</strong> Make sure this 6 columns are included in Excel and with the correct naming.
                  <img src="images/excel_example.png" />
                  
                  <br />
                  <br />
                  <strong>Sheet Name</strong> Make sure only 1 sheet is in the excel and named "Sheet1".
                  <img src="images/excel_example_1.png" />
                  
                  <br />
                  <br />
                  <strong>Timestamp Format</strong> Ensure cell format is set to "Date", with Type "M-DD-YY HH:MM".<br />Example: 3-14-01 13:30.
                  <br /><small>** If cannot be found, select  "Locale (location)" to English (U.S.).</small><br />
                   <img src="images/excel_example_2.png" />
                  <br /> 
                  
                </div>
              </div>
            </div>
   
    </form>
</body>
</html>
