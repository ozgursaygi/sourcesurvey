<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AnketBitti.aspx.cs" Inherits="BaseWebSite.Survey.AnketBitti" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<title>Survey</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link rel="stylesheet" type="text/css" href="../css/custom-theme/jquery-ui-1.8.20.custom.css" />
    <link rel="stylesheet" type="text/css" href="../css/ddsmoothmenu.css" />
    <link rel="stylesheet" type="text/css" href="../css/reset.css"/>
	<link rel="stylesheet" type="text/css" href="../css/style.css"/>
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/jquery-ui-1.8.20.custom.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            
        });

        
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div style="width: 100%; margin: auto;">
        <div style="padding: 0px 5px 0px 20px; width: 70%; height: 100%; margin: 40px; float: left;">
            <div id="divanket" style="padding: 10px 10px; width: 100%; height: 100%; border: 1px solid #dddddd;
                vertical-align: top; background-color: #f2f5f7;">
                <div class="ui-widget" id="sonuc_mesaji" runat="server" clientidmode="Static">
                                    <div class="ui-state-error ui-corner-all" style="padding: 15px;">
                                        <p>
                <asp:Literal ID="SonucMesaji" runat="server"></asp:Literal>
                </p>
                </div>
                </div>
            <br />
            <br />
            <div ><asp:HyperLink ID="hyplink" runat="server" ForeColor="Green"><asp:Literal ID="ltlLink" runat="server"></asp:Literal></asp:HyperLink></div>    
            <br />
            </div>
            
        </div>
        
    </div>
    </form>
</body>
</html>
