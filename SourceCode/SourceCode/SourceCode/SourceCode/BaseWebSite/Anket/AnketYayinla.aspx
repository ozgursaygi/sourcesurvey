<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="SurveyYayinla.aspx.cs" Inherits="BaseWebSite.Survey.SurveyYayinla" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>Publish Info</title>
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
            $(".sortable").sortable({
                revert: true
            });
            $("ul, li").disableSelection();
            
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="fb-root">
    </div>
    <script type="text/javascript">
        $(function () {

        });

        function SurveyeDon() {
            __doPostBack('SurveyeDon', 'SurveyeDon');
        }

        function Listele(tarihce_uid) {
            $("#dialogMailListesiGoruntule").dialog({
                height: 550,
                width: 900,
                modal: true
            });

            __doPostBack('<%=UpdatePanelMailler.ClientID%>', tarihce_uid);
        }


        $(document).ready(function () {

        });

        function OnaySurveyYayinla() {

            if (window.confirm('Do you want to publish Survey?')) {
                return true;
            }
            else {
                return false;
            }
        }

        function TekrarOnaySurveyYayinla() {

            if (window.confirm('From the Survey Settings until the time you have extended; People who have completed Survey can see their responses. Unfilled People can respond. E-Mail will not go again.')) {
                return true;
            }
            else {
                return false;
            }
        }
    </script>
    <div style="width: 100%; margin: auto;">
        <div id="mainTopSection">
            <div id="smoothmenu1" class="ddsmoothmenu">
                <ul class="mainMenu">
                    <li><a href="MainPage.aspx">MY SURVEYS</a></li>
                    <li><a href="#" onclick="SurveyeDon();return false;">BACK TO THE SURVEY</a></li>
                </ul>
            </div>
        </div>
        <div id="mainCenter" style="height: 100%">
            <div class="boxCenter">
                <div class="boxCenterHeaders">
                    <asp:LinkButton ID="LinkButtonHeader" Font-Size="23px" ForeColor="White" Font-Underline="false"
                        runat="server" Text="Publis Survey"></asp:LinkButton>
                </div>
                <div id="dialogMailListesiGoruntule" title="Gönderilen Mail Listesi" style="display: none;
                    font-size: 10pt">
                    <asp:UpdatePanel ID="UpdatePanelMailler" runat="server">
                        <ContentTemplate>
                            <table cellpadding="0" cellspacing="0" width="630px">
                                <tr>
                                    <asp:Repeater ID="rptMailler" runat="server">
                                        <HeaderTemplate>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <div class="surveyListCenter">
                                                <div class="surveyListLeft">
                                                    <div class="surveyName">
                                                        <%#DataBinder.Eval(Container.DataItem, "gonderilen_kisi_ismi")%>
                                                        -
                                                        <%#DataBinder.Eval(Container.DataItem, "gonderilen_email_adresi")%>
                                                        -
                                                        <%#DataBinder.Eval(Container.DataItem, "gonderi_tarihi_str")%></span>
                                                    </div>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                        <AlternatingItemTemplate>
                                            <div class="surveyListCenter">
                                                <div class="surveyListLeft">
                                                    <div class="surveyName">
                                                        <%#DataBinder.Eval(Container.DataItem, "gonderilen_kisi_ismi")%>
                                                        -
                                                        <%#DataBinder.Eval(Container.DataItem, "gonderilen_email_adresi")%>
                                                        -
                                                        <%#DataBinder.Eval(Container.DataItem, "gonderi_tarihi_str")%></span>
                                                    </div>
                                                </div>
                                            </div>
                                        </AlternatingItemTemplate>
                                        <FooterTemplate>
                                        </FooterTemplate>
                                    </asp:Repeater>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div><table><tr>
                            <td  colspan="3">
                                <img src="../Images/_spacer.gif" height="20px" width="20px" />
                            </td>
                        </tr></table></div>
                <div class="ui-widget" id="div_error_message" runat="server" clientidmode="Static">
                    <div class="ui-state-error ui-corner-all" style="padding: 15px;">
                        <p>
                            <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                        </p>
                    </div>
                </div>
                <div>
                    <table>
                        <tr>
                            <td colspan="3">
                                <img src="../Images/_spacer.gif" height="20px" width="20px" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="div_acik_anket" runat="server">
                    <table cellpadding="0" cellspacing="0" width="100%">
                        <tr>
                            <td>
                                <div class="accountInfo">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td valign="top" width="50%" colspan="2">
                                                <asp:Button ID="Button1" runat="server" Text="Publish Survey" OnClick="btnSurveyYayinla_Click"
                                                    OnClientClick="return OnaySurveyYayinla();" />
                                            </td>
                                        </tr>
                                         <tr>
                                            <td width="20px" colspan="2">
                                                <img src="../Images/_spacer.gif" height="20px" width="20px" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top" width="300px">
                                                <asp:Label ID="lblDirektLink" runat="server" AssociatedControlID="txtDirektLink">Direkt Link:</asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtDirektLink" runat="server" CssClass="textEntry" Width="600px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top" width="300px">
                                                <asp:Label ID="Label1" runat="server" AssociatedControlID="txtPopupLink">Popup Link:</asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPopupLink" runat="server" TextMode="MultiLine" Rows="10" CssClass="textEntry"
                                                    Width="600px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top" width="300px">
                                                <asp:Label ID="Label2" runat="server" AssociatedControlID="txtPageLink">Link:</asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtPageLink" runat="server" TextMode="MultiLine" Rows="5" CssClass="textEntry"
                                                    Width="600px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top" width="300px">
                                                <asp:Label ID="Label3" runat="server" AssociatedControlID="txtIframeLink">IFrame Link:</asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtIframeLink" runat="server" TextMode="MultiLine" Rows="10" CssClass="textEntry"
                                                    Width="600px"></asp:TextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="20px" colspan="2">
                                                <img src="../Images/_spacer.gif" height="20px" width="20px" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top" colspan="2">
                                                
                                                
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="20px" colspan="2">
                                                <img src="../Images/_spacer.gif" height="20px" width="20px" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top" width="50%" colspan="2">
                                                <asp:Button ID="btnSurveyYayinla" runat="server" Text="Publish Survey" OnClick="btnSurveyYayinla_Click"
                                                    OnClientClick="return OnaySurveyYayinla();" />
                                            </td>
                                        </tr>
                                        <tr><td colspan="2"><img src="../Images/_spacer.gif" height="20px" width="20px" /></td></tr></tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
                <div id="div_katilimci_anketi" runat="server">
                 <div class="ui-widget" id="error_message" runat="server" clientidmode="Static">
                                    <div class="ui-state-error ui-corner-all" style="padding: 15px;">
                                        <p>
                                            <asp:Literal ID="ltlUyari" runat="server"></asp:Literal>
                                            <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="failureNotification"
                                                ValidationGroup="ValidationGroup" />
                                    </div>
                                </div>
                    <table cellpadding="0" cellspacing="0" width="100%">
                       
                        <tr>
                            <td width="70%">
                               
                                <div>
                                    <table cellpadding="0" cellspacing="0">
                                     
                                        <tr>
                                            <td valign="top" width="200px">
                                                <asp:Label ID="Label4" runat="server" AssociatedControlID="ddlMailGrubu">Grup:</asp:Label>
                                            </td>
                                            <td>
                                                <asp:DropDownList ID="ddlMailGrubu" runat="server" CssClass="textEntry" Width="200px"
                                                    AutoPostBack="true" ClientIDMode="Static">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top" width="200px">
                                                <asp:Label ID="Label5" runat="server" AssociatedControlID="txtMailSubject">Davetlilere gidecek Konu:</asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMailSubject" runat="server" TextMode="SingleLine" CssClass="textEntry"
                                                    Width="400px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="SubjectRequired" runat="server" ControlToValidate="txtMailSubject"
                                                    CssClass="failureNotification" ErrorMessage="Lütfen Konu giriniz." ToolTip="Lütfen Konu giriniz."
                                                    ValidationGroup="ValidationGroup">*</asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top" width="200px">
                                                <asp:Label ID="Label6" runat="server" AssociatedControlID="txtMailMesaj">Davetlilere gidecek Mesaj:</asp:Label>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtMailMesaj" runat="server" TextMode="MultiLine" Rows="8" CssClass="textEntry"
                                                    Width="400px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtMailMesaj"
                                                    CssClass="failureNotification" ErrorMessage="Lütfen Mesaj giriniz." ToolTip="Lütfen Mesaj giriniz."
                                                    ValidationGroup="ValidationGroup">*</asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td width="20px" colspan="2">
                                                <img src="../Images/_spacer.gif" height="20px" width="20px" alt="" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top" colspan="2">
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <asp:Button ID="btnSurveyGonder" runat="server" Text="E-Posta Grubuna Gönder&Yayınla"
                                                                ValidationGroup="ValidationGroup" OnClick="btnSurveyGonder_Click"
                                                                OnClientClick="return OnaySurveyYayinla();" />
                                                        </td>
                                                        <td width="20px" colspan="2">
                                                            <img src="../Images/_spacer.gif" height="20px" width="20px" alt="" />
                                                        </td>
                                                        <td>
                                                            <asp:Button ID="btnDirektYayinla" runat="server" Text="Activate Survey Again"
                                                                OnClick="btnDirektYayinla_Click" Width="200px" OnClientClick="return TekrarOnaySurveyYayinla();" />
                                                        </td>
                                                    </tr>
                                                    <tr><td colspan="3"><img src="../Images/_spacer.gif" height="20px" width="20px" /></td></tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                            <td width="5%">
                              <img src="../Images/_spacer.gif" height="50px" width="50px" />
                            </td>
                            <td width="25%" valign="top">
                                <table>
                                    <tr>
                                        <td width="300px" colspan="3">
                                            Gönderi Tarihçesi
                                        </td>
                                    </tr>
                                </table>
                                <asp:Repeater ID="rptGonderiTarihcesi" runat="server">
                                    <HeaderTemplate>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <div class="surveyListCenter">
                                            <div class="surveyListLeft">
                                                <div class="surveyName">
                                                    <a class="mediumUnderlineLink" href="#" onclick="Listele('<%#DataBinder.Eval(Container.DataItem, "tarihce_uid")%>');return false;">
                                                        <%#DataBinder.Eval(Container.DataItem, "mail_grubu_adi")%>
                                                        -
                                                        <%#DataBinder.Eval(Container.DataItem, "gonderi_tarihi_str")%></a>
                                                </div>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                    <AlternatingItemTemplate>
                                        <div class="surveyListCenter lightBg">
                                            <div class="surveyListLeft">
                                                <div class="surveyName">
                                                    <a class="mediumUnderlineLink" href="#" onclick="Listele('<%#DataBinder.Eval(Container.DataItem, "tarihce_uid")%>');return false;">
                                                        <%#DataBinder.Eval(Container.DataItem, "mail_grubu_adi")%>
                                                        -
                                                        <%#DataBinder.Eval(Container.DataItem, "gonderi_tarihi_str")%></a>
                                                </div>
                                            </div>
                                        </div>
                                    </AlternatingItemTemplate>
                                    <FooterTemplate>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
