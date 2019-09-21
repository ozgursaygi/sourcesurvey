<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SablonGoster.aspx.cs" Inherits="BaseWebSite.Admin.SablonGoster" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Survey Soruları</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link rel="stylesheet" type="text/css" href="../css/reset.css" />
    <link rel="stylesheet" type="text/css" href="../css/style.css" />
    <link rel="stylesheet" type="text/css" href="../css/custom-theme/jquery-ui-1.8.20.custom.css" />
    <link rel="stylesheet" href="../css/uniform.default.css" type="text/css" media="screen" />
    <link rel="stylesheet" type="text/css" href="../css/ddsmoothmenu.css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/jquery-ui-1.8.20.custom.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.numeric.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.maskedinput-1.2.2.js" type="text/javascript"></script>
    <script src="../Scripts/JQ/jquery.ui.datepicker-tr.js" type="text/javascript"></script>
    <script src="../js/ddsmoothmenu.js" type="text/javascript"></script>
    <script src="../js/jquery.uniform.js" type="text/javascript" charset="utf-8"></script>
    <script type="text/javascript">
        $(function () {
            $("input, textarea  ").uniform();
        });

        $(document).ready(function () {
            $(".numeric").numeric({ decimal: "," });
            $(".integer").numeric(false);
            $(".datepicker").datepicker({ dateFormat: 'dd-mm-yy', onSelect: function (dateText, inst) {

            }
            });
            $(".phone").mask("(999)999-9999");
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="mainCenter" style="height: 100%">
        <div class="boxCenter">
            <div class="boxCenterHeaders">
                Şablon Bilgileri
            </div>
            <asp:Repeater ID="rptSablonSorulari" runat="server">
                <HeaderTemplate>
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="makeQuestion">
                        <div class="questionNumber">
                            Soru
                            <%#DataBinder.Eval(Container.DataItem, "RowNumber")%>
                            :
                        </div>
                        <div class="questionName">
                            <%#DataBinder.Eval(Container.DataItem, "soru")%>
                        </div>
                        <div class="questionContent">
                            <%# SablonSorulariOlustur(DataBinder.Eval(Container.DataItem, "soru_uid"))%>
                        </div>
                        <div class="surveyListButtons">
                        </div>
                    </div>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <div class="makeQuestion lightBg">
                        <div class="questionNumber">
                            Soru
                            <%#DataBinder.Eval(Container.DataItem, "RowNumber")%>
                            :
                        </div>
                        <div class="questionName">
                            <%#DataBinder.Eval(Container.DataItem, "soru")%>
                        </div>
                        <div class="questionContent">
                            <%# SablonSorulariOlustur(DataBinder.Eval(Container.DataItem, "soru_uid"))%>
                        </div>
                        <div class="surveyListButtons">
                        </div>
                    </div>
                </AlternatingItemTemplate>
            </asp:Repeater>
        </div>
    </div>
    </form>
</body>
</html>
