<%@ Page Language="VB" AutoEventWireup="false" EnableEventValidation="true"
    Inherits="YTLWebApplication.AVLS.Login" Codebehind="Login.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>YTL - AVLS Secure Login</title>
    <meta name="keywords" content="vehicle tracking, vehicle tracking system, GPS tracking, GPS vehicle tracking, GPS tracking system, automatic vehicle location, tracking system,automatic vehicle locating system, AVLS, GSM GPRS tracking" />
    <meta name="description" content="AVLS is a low cost, features rich versatile system. It utilize both GPS & GSM technologies to track your vehicle at any time and any place via web based." />
    <meta http-equiv="X-Frame-Options" content="DENY" />
    <meta http-equiv="X-Content-Type-Options" content="nosniff" />
    <meta http-equiv="X-XSS-Protection" content="1; mode=block" />
    
    <script type="text/javascript">
        function getWindowWidth() { if (window.self && self.innerWidth) { return self.innerWidth; } if (document.documentElement && document.documentElement.clientWidth) { return document.documentElement.clientWidth; } return document.documentElement.offsetWidth; }
        function getWindowHeight() { if (window.self && self.innerHeight) { return self.innerHeight; } if (document.documentElement && document.documentElement.clientHeight) { return document.documentElement.clientHeight; } return document.documentElement.offsetHeight; }
        
        // SECURITY FIX: Enhanced client-side validation
        function enhancedValidation() {
            var username = document.getElementById("uname").value;
            var password = document.getElementById("password").value;
            
            // Basic validation
            if (username === "") {
                alert("Please enter user name");
                return false;
            }
            
            if (password === "") {
                alert("Please enter password");
                return false;
            }
            
            // SECURITY FIX: Length validation
            if (username.length > 50) {
                alert("Username too long");
                return false;
            }
            
            if (password.length > 100) {
                alert("Password too long");
                return false;
            }
            
            // SECURITY FIX: Check for suspicious characters
            var suspiciousPattern = /[<>\"'%;()&+\-\*/=]/;
            if (suspiciousPattern.test(username)) {
                alert("Invalid characters in username");
                return false;
            }
            
            // SECURITY FIX: Basic XSS prevention
            if (username.toLowerCase().indexOf('script') !== -1 || 
                username.toLowerCase().indexOf('javascript:') !== -1 ||
                password.toLowerCase().indexOf('script') !== -1) {
                alert("Invalid input detected");
                return false;
            }
            
            // Set window dimensions
            document.getElementById("w").value = getWindowWidth();
            document.getElementById("h").value = getWindowHeight();
            
            return true;
        }
        
        // SECURITY FIX: Prevent frame embedding
        if (window.parent.frames.length > 0) {
            window.parent.location = "login.aspx";
        }
    </script>
    
    <%  If foc <> "" Then%>
    <script type="text/javascript" language="javascript">
        if (window.parent.frames.length > 0) { window.parent.location = "login.aspx"; }
    </script>
    <%  End If%>
    
    <style type="text/css">
        body
        {
            font-size: 11px;
            font-family: Verdana, Arial, Helvetica, sans-serif;
            margin: 0px;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
        }
        .login-container
        {
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
            padding: 20px;
        }
        .login-box
        {
            background: rgba(255, 255, 255, 0.95);
            border-radius: 10px;
            box-shadow: 0 15px 35px rgba(0, 0, 0, 0.1);
            padding: 40px;
            max-width: 400px;
            width: 100%;
            backdrop-filter: blur(10px);
        }
        .logo
        {
            text-align: center;
            margin-bottom: 30px;
        }
        .logo img
        {
            max-width: 200px;
            height: auto;
        }
        .form-group
        {
            margin-bottom: 20px;
        }
        .form-group label
        {
            display: block;
            margin-bottom: 5px;
            font-weight: bold;
            color: #333;
        }
        .form-group input[type="text"], .form-group input[type="password"]
        {
            width: 100%;
            padding: 12px;
            border: 2px solid #ddd;
            border-radius: 5px;
            font-size: 14px;
            transition: border-color 0.3s;
            box-sizing: border-box;
        }
        .form-group input[type="text"]:focus, .form-group input[type="password"]:focus
        {
            border-color: #667eea;
            outline: none;
        }
        .button-group
        {
            display: flex;
            gap: 10px;
            justify-content: center;
            margin-top: 30px;
        }
        .btn
        {
            padding: 12px 30px;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 14px;
            font-weight: bold;
            transition: all 0.3s;
        }
        .btn-login
        {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
        }
        .btn-login:hover
        {
            transform: translateY(-2px);
            box-shadow: 0 5px 15px rgba(102, 126, 234, 0.4);
        }
        .btn-reset
        {
            background: #f8f9fa;
            color: #333;
            border: 2px solid #ddd;
        }
        .btn-reset:hover
        {
            background: #e9ecef;
        }
        .contact-info
        {
            text-align: center;
            margin-top: 30px;
            font-size: 12px;
            color: #666;
        }
        .contact-info a
        {
            color: #667eea;
            text-decoration: none;
        }
        .security-notice
        {
            background: #e8f4fd;
            border: 1px solid #bee5eb;
            border-radius: 5px;
            padding: 10px;
            margin-bottom: 20px;
            font-size: 12px;
            color: #0c5460;
        }
    </style>
</head>
<body>
    <form id="loginform" runat="server" defaultbutton="ImageButton1">
        <div class="login-container">
            <div class="login-box">
                <div class="logo">
                    <img src="images/ytl-logo.png" alt="YTL Logo" />
                </div>
                
                <div class="security-notice">
                    🔒 This is a secure login system. Your session will timeout after 30 minutes of inactivity.
                </div>
                
                <div class="form-group">
                    <label for="uname">Username</label>
                    <input id="uname" runat="server" type="text" tabindex="1" title="Enter your username" maxlength="50" autocomplete="username" />
                </div>
                
                <div class="form-group">
                    <label for="password">Password</label>
                    <input id="password" runat="server" type="password" tabindex="2" title="Enter your password" maxlength="100" autocomplete="current-password" />
                </div>
                
                <div class="button-group">
                    <asp:ImageButton ID="ImageButton1" runat="server" CssClass="btn btn-login" 
                        Text="Login" TabIndex="3" OnClientClick="return enhancedValidation()" />
                    <input type="reset" class="btn btn-reset" title="Clear form" tabindex="4" value="Reset" />
                </div>
                
                <div class="contact-info">
                    <strong>24 Hour Help Line Center:</strong><br />
                    <span style="color: #667eea;">+60 3625 79472</span> / 
                    <a href="mailto:info@g1.com.my">info@g1.com.my</a><br />
                    <strong>Tel:</strong> <span style="color: #667eea;">+60 3625 70509</span><br /><br />
                    
                    <div style="border-top: 1px solid #ddd; padding-top: 15px; margin-top: 15px;">
                        <strong>www.g1.com.my</strong><br />
                        Copyright © 2024 <span style="color: #667eea;">Global Telematics Sdn Bhd</span>. All rights reserved<br />
                        Powered by Integra ®<br />
                        Best viewed with Chrome/Firefox at 1366x768 resolution.
                    </div>
                </div>
            </div>
        </div>
        
        <!-- Hidden fields for tracking -->
        <input name="w" id="w" type="hidden" value="" />
        <input name="h" id="h" type="hidden" value="" />
        <input name="lat" id="lat" type="hidden" value="" />
        <input name="lon" id="lon" type="hidden" value="" />
        <input name="acc" id="acc" type="hidden" value="" />
    </form>
    
    <script type="text/javascript" language="javascript">
        // Set focus to username field
        document.getElementById("uname").focus();
        
        // Get geolocation if available
        function getLocation() {
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(showPosition, function(error) {
                    // Fail silently if geolocation is not available
                });
            }
        }
        
        function showPosition(position) {
            document.getElementById("lat").value = position.coords.latitude;
            document.getElementById("lon").value = position.coords.longitude;
            document.getElementById("acc").value = position.coords.accuracy;
        }
        
        getLocation();
    </script>
    
    <%  If foc <> "" Then%>
    <script type="text/javascript" language="javascript">
        var id = "<%=foc%>"; 
        var element = document.getElementById(id);
        if (element) {
            element.focus(); 
            element.select();
        }
    </script>
    <%  End If%>
    
    <%  If errormessage <> "" Then%>
    <script type="text/javascript" language="javascript">
        alert('<%=errormessage.Replace("'", "\'").Replace(vbCrLf, "\n")%>');
    </script>
    <%  End If%>
</body>
</html>