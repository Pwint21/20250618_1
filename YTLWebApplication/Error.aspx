<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Error.aspx.vb" Inherits="YTLWebApplication.ErrorPage" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Error - YTL Web Application</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f5f5f5;
            margin: 0;
            padding: 20px;
        }
        .error-container {
            max-width: 600px;
            margin: 50px auto;
            background-color: white;
            padding: 30px;
            border-radius: 5px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            text-align: center;
        }
        .error-icon {
            font-size: 48px;
            color: #dc3545;
            margin-bottom: 20px;
        }
        .error-title {
            color: #dc3545;
            font-size: 24px;
            margin-bottom: 15px;
        }
        .error-message {
            color: #666;
            font-size: 16px;
            margin-bottom: 25px;
        }
        .btn {
            background-color: #007bff;
            color: white;
            padding: 10px 20px;
            text-decoration: none;
            border-radius: 4px;
            display: inline-block;
        }
        .btn:hover {
            background-color: #0056b3;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="error-container">
            <div class="error-icon">⚠</div>
            <h1 class="error-title">An Error Occurred</h1>
            <p class="error-message">
                We're sorry, but an unexpected error has occurred. Please try again later or contact support if the problem persists.
            </p>
            <a href="Login.aspx" class="btn">Return to Login</a>
        </div>
    </form>
</body>
</html>