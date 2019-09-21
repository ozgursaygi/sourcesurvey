<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Anket.aspx.cs" Inherits="BaseWebSite.Survey.Survey"
    ValidateRequest="false" %>

<%@ Register Src="Controls/RepeaterSurveySayfalama.ascx" TagName="RepeaterSurveySayfalama"
    TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="Expires" content="-1" />
    <title>Survey</title>
    <link rel="stylesheet" type="text/css" href="../css/reset.css" />
    <link rel="stylesheet" type="text/css" href="../css/custom-theme/jquery-ui-1.8.20.custom.css" />
    <link rel="stylesheet" type="text/css" href="../css/uniform.default.css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/jquery-ui-1.8.20.custom.min.js" type="text/javascript"></script>
    <script src="../js/jquery.uniform.min.js" type="text/javascript" charset="utf-8"></script>
    <script src="../js/jquery.ae.image.resize.min.js" type="text/javascript"></script>
    <script type="text/javascript" charset="utf-8">
        $(function () {
            $("input , textarea, select").uniform();
            $(".resizeme").aeImageResize({ height: 100, width: 140 });
        });
    </script>
    <script src="../Scripts/JQValidationEngine/languages/jquery.validationEngine-tr.js"
        type="text/javascript" charset="utf-8"></script>
    <script src="../Scripts/JQValidationEngine/jquery.validationEngine.js" type="text/javascript"
        charset="utf-8"></script>
    <script src="../Scripts/jquery.numeric.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.maskedinput-1.2.2.js" type="text/javascript"></script>
    <script src="../Scripts/JQ/jquery.ui.datepicker-tr.js" type="text/javascript"></script>
    <link href="../Scripts/css/validationEngine.jquery.css" rel="stylesheet" type="text/css" />
    <meta http-equiv="Pragma" content="no-cache" />
    <meta http-equiv="Expires" content="-1" />
    <script type="text/javascript">
        function Kaydet() {
            document.getElementById('hdn_kaydet_finish').value = "1";
            if ($("#form1").validationEngine('attach')) {
                return true;
            }
            else {
                return false;
            }
        }

        function Finish() {
            document.getElementById('hdn_kaydet_finish').value = "1";
            if (window.confirm('Do you want to Finish Survey.')) {
                return true;
            }
            else {
                return false;
            }
        }

        function TestiOnayla() {
            __doPostBack('TestiOnayla', 'TestiOnayla');
        }

        $(document).ready(function () {
            $("#form1").validationEngine('attach');
            $(".numeric").numeric({ decimal: "," });
            $(".integer").numeric(false);
            $(".datepicker").datepicker({ dateFormat: 'dd-mm-yy', onSelect: function (dateText, inst) {

            }
            });
            $(".phone").mask("(999)999-9999");


        });
    </script>
</head>
<body class="theme">
    <form id="form1" runat="server" class="topLabel">
    <div id="wrapper" class="theme1">
        <div id="header">
            <div class="surveyLogo">
                <div class="surveyLogoInner">
                    <asp:Image ID="Image1" runat="server" CssClass="resizeme" /></div>
            </div>
            <div class="surveyTitle">
                <asp:Literal ID="LinkButtonHeader" runat="server"></asp:Literal></div>
                
           
        </div>
        <div id="main">
         <div>
             <table cellpadding="0" cellspacing="0" width="100%">
                 <tr>
                     <td width="20px">
                         <img src="../Images/_spacer.gif" height="20px" width="20px" alt="" />
                     </td>
                 </tr>
             </table>
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td width="20px">
                            <img src="../Images/_spacer.gif" height="20px" width="20px" alt="" />
                        </td>
                        <td align="center">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <div class="ui-widget" id="uyari_mesaji" runat="server" clientidmode="Static" >
                                            <div class="ui-state-error ui-corner-all" style="padding: 15px;width:650px">
                                                <p>
                                                    <asp:Literal ID="UyariMesaji" runat="server"></asp:Literal>
                                                </p>
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td width="20px">
                            <img src="../Images/_spacer.gif" height="20px" width="20px" alt="" />
                        </td>
                        <td align="center">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td><asp:LinkButton ID="LinkButton1" runat="server"></asp:LinkButton></td>
                                    <td>
                                        
                                        <asp:Literal ID="ltlSurveyMesaji" runat="server"></asp:Literal>
                                    </td>
                                    
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td width="20px">
                            <img src="../Images/_spacer.gif" height="20px" width="20px" alt="" />
                        </td>
                        <td>
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <div class="ui-widget" id="error_message" style="padding: 15px;" runat="server" clientidmode="Static">
                                            <div class="ui-state-error ui-corner-all" style="padding: 15px;width:650px" >
                                                <p>
                                                    <asp:Literal ID="CevapKotrolu" runat="server"></asp:Literal>
                                                </p>
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
            <asp:Repeater ID="rptSurveySorulari" runat="server">
                <ItemTemplate>
                    <div class="surveyQuestion">
                        <div class="surveyQuestionId">
                            <%#DataBinder.Eval(Container.DataItem, "RowNumber")%>.
                        </div>
                        <div class="surveyQuestionContent">
                            <div class="surveyQuestionTitle">
                                <label style="color:#CD746B" ><%#DataBinder.Eval(Container.DataItem, "zorunlu_soru")%></label>
                                    <%#DataBinder.Eval(Container.DataItem, "soru")%>
                            </div>
                            <div class="surveyQuestionAnswer">
                                <div id="div_soru" runat="server">
                                </div>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        <div id="footer">
            <uc1:RepeaterSurveySayfalama ID="rptSurveySayfalama" runat="server" />
            <div class="totalQuestionNumber">
                <asp:Literal ID="ltlCevapDurumu" runat="server"></asp:Literal>   -  * Mandatory Questions </div>
                 
            <div class="surveyButtons">
                <asp:Button id="btnFinish2" class="surveySave" type="button" 
                    OnClientClick="return Kaydet();" runat="server"
                    Text="Save"  style="border-style:none;" onclick="btnFinish2_Click"  />
                <asp:Button id="btnClose2" class="surveyFinish" type="button" 
                    OnClientClick="return Finish();" runat="server"
                    Text="Bitir"  style="border-style:none;" onclick="btnClose2_Click"   />
            </div>
            <div class="totalQuestionNumber" style="padding-left:64px"><asp:Literal ID="ltlMesaj" runat="server"></asp:Literal></div>
        </div>
        <div> <img src="../Images/_spacer.gif" height="10px" width="10px" alt="" /></div>
        <div class="logo" style="text-align:center" > <a  href="<%=path_url%>/Default.aspx" target="_blank"><img alt="" src="../img/surveySampleLogo.png" width="50px" /></a> </div>
        <div> <img src="../Images/_spacer.gif" height="10px" width="10px" alt="" /></div>
        <div runat="server" id="div_test_onay" style="padding:10px">
            <table>
            <tr>
                    <td colspan="2">
            <div class="totalQuestionNumber" style="font-weight:bold">Lütfen soruları içerik ve şık olarak değerlendiriniz.Cevaplamanız sadece çalışılırlığını kontrol etmek amaçlıdır.Cevaplamanız değerlendirmeye alınmaz.</div>            
                        
                    </td>
                </tr>
                <tr><td colspan="2"><img src="../Images/_spacer.gif" height="10px" width="10px" alt="" /></td></tr>
                <tr>
                    <td>
                        <asp:Label ID="Label2" runat="server">Değerlendirme Sonucu:</asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlTestSonucu" runat="server" CssClass="textEntry" Width="150px">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label1" runat="server">Değerlendirme Açıklaması :</asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtSurveyTestiSonucAciklamasi" runat="server" CssClass="textEntry"
                            Width="300" Rows="6" Columns="20" TextMode="MultiLine"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                    <td>
                        <asp:Button id="btnTestiOnayla1" type="button"  runat="server"
                            Text="Değerlendirmeyi Onayla" onclick="btnTestiOnayla1_Click" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <input type="hidden" id="hdn_kaydet_finish" runat="server" clientidmode="Static" />
    </form>
</body>
</html>
