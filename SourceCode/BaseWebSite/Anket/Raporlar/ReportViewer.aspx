<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportViewer.aspx.cs" Inherits="BaseWebSite.Anket.Raporlar.ReportViewer" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="Expires" content="-1" />
    <title>Raporlar</title>
    <link rel="stylesheet" type="text/css" href="../../css/custom-theme/jquery-ui-1.8.20.custom.css" />
    <link rel="stylesheet" href="../../css/uniform.default.css" type="text/css" media="screen" />
    <link rel="stylesheet" type="text/css" href="../../css/ddsmoothmenu.css" />
    <script src="../../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../../js/jquery-ui-1.8.20.custom.min.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery.numeric.js" type="text/javascript"></script>
    <script src="../../Scripts/jquery.maskedinput-1.2.2.js" type="text/javascript"></script>
    <script src="../../Scripts/JQ/jquery.ui.datepicker-tr.js" type="text/javascript"></script>
    <script src="../../js/ddsmoothmenu.js" type="text/javascript"></script>
    <script src="../../js/jquery.uniform.js" type="text/javascript" charset="utf-8"></script>
    <link rel="stylesheet" type="text/css" href="../../css/reset.css" />
    <link rel="stylesheet" type="text/css" href="../../css/style.css" />
    <script type="text/javascript">
        $(function () {

            $("input, textarea,  select").uniform();

        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div style="width: 100%; margin: auto;">
        <div>
            <table>
                <tr>
                    <td>
                        <img src="../../Images/_spacer.gif" height="20px" alt="" />
                    </td>
                </tr>
            </table>
        </div>
        <div id="mainTopSection">
            <div class="searchSurvey">
                <table width="500px"><tr><td width="20px"></td><td width="180px"><asp:Label runat="server" ID="lblLabel">Report Options :</asp:Label></td><td width="300px"><asp:DropDownList ID="ddlrapor" runat="server" CssClass="textEntry" Width="300px"
                    AutoPostBack="true" OnSelectedIndexChanged="ddlrapor_SelectedIndexChanged">
                </asp:DropDownList></td></tr></table>
                
            </div>
        </div>
        <div>
                <table>
                    <tr>
                        <td>
                            <img src="../../Images/_spacer.gif" height="40px" />
                        </td>
                    </tr>
                </table>
            </div>
            <hr />
        <div>
            
            <iframe id="iframeMap" runat="server" frameborder="0" height="650px" width="100%" scrolling="no">
            </iframe>
        </div>
    </div>
    </form>
</body>
</html>
