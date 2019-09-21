<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SurveyTestSonuclari.aspx.cs"
    Inherits="BaseWebSite.Survey.SurveyTestSonuclari" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Test Sonuçları</title>
    <link rel="stylesheet" type="text/css" href="../css/custom-theme/jquery-ui-1.8.20.custom.css" />
    <link rel="stylesheet" href="../css/uniform.default.css" type="text/css" media="screen" />
    <link rel="stylesheet" type="text/css" href="../css/reset.css" />
    <link rel="stylesheet" type="text/css" href="../css/style.css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/jquery-ui-1.8.20.custom.min.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div style="width: 100%; margin: auto;">
        <div id="mainCenter" style="height: 100%">
            <div class="boxCenter">
            <div class="boxCenterHeaders">
            Test Sonuçları
            </div>
                <div class="boxCenterContent">
                    <asp:Repeater ID="rptTestSonuclari" runat="server">
                        <ItemTemplate>
                            <div class="surveyList">
                                <div class="surveyListLeft">
                                    <div class="surveyName">
                                        <%#DataBinder.Eval(Container.DataItem, "gonderilen_email_adresi")%>
                                        -
                                        <%#DataBinder.Eval(Container.DataItem, "anket_test_sonucu_durumu")%>
                                    </div>
                                    <div class="surveyType">
                                        <%#DataBinder.Eval(Container.DataItem, "anket_test_sonucu")%>
                                         <div class="surveyListButtons">
                                         </div>

                                    </div>
                                    <div class="surveyListRight">
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                        <AlternatingItemTemplate>
                            <div class="surveyList lightBg">
                                <div class="surveyListLeft">
                                    <div class="surveyName">
                                        <%#DataBinder.Eval(Container.DataItem, "gonderilen_email_adresi")%>
                                        -
                                        <%#DataBinder.Eval(Container.DataItem, "anket_test_sonucu_durumu")%>
                                    </div>
                                    <div class="surveyType">
                                        <%#DataBinder.Eval(Container.DataItem, "anket_test_sonucu")%>
                                        <div class="surveyListButtons">
                                         </div>
                                    </div>
                                    <div class="surveyListRight">
                                    </div>
                                </div>
                            </div>
                        </AlternatingItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </div>
    </form>
</body>
</html>
