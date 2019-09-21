<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="YeniAnket.aspx.cs" Inherits="BaseWebSite.Survey.YeniAnket" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" type="text/css" href="../css/custom-theme/jquery-ui-1.8.20.custom.css" />
    <link rel="stylesheet" href="../css/uniform.default.css" type="text/css" media="screen" />
    <link rel="stylesheet" type="text/css" href="../css/ddsmoothmenu.css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/jquery-ui-1.8.20.custom.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.numeric.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.maskedinput-1.2.2.js" type="text/javascript"></script>
    <script src="../js/ddsmoothmenu.js" type="text/javascript"></script>
    <script src="../js/jquery.uniform.js" type="text/javascript" charset="utf-8"></script>
    <script src="../js/jquery.ae.image.resize.min.js" type="text/javascript"></script>
    <script src="../Scripts/JQ/jquery.ui.datepicker-tr.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(function () {
            ddsmoothmenu.init({
                mainmenuid: "smoothmenu1",
                orientation: 'h',
                classname: 'ddsmoothmenu',
                contentsource: "markup"
            })

            $("input, textarea, select, button").uniform();
            $(".resizeme").aeImageResize({ height: 120, width: 150 });

        });

        $(document).ready(function () {

            $('#error_message').hide();
            if ($('#has_error').val() == "1") {
                $('#error_message').show();
            }

            $("#div_gorunen_ad").hide();
            $("#div_anket_mesaji").hide();
            $("#div_anket_sonuc_mesaji").hide();

            $("#baslangic_tarihi").datepicker({ dateFormat: 'dd-mm-yy', onSelect: function (dateText, inst) {

            }
            });

            $("#bitis_tarihi").datepicker({ dateFormat: 'dd-mm-yy', onSelect: function (dateText, inst) {

            }
            });

            if ($("#ChkGorunenAd").attr('checked') == "checked") {
                $("#div_gorunen_ad").show();
            }
            else {
                $("#div_gorunen_ad").hide();
            }

            if ($("#ChkSurveyMesajiEkle").attr('checked') == "checked") {
                $("#div_anket_mesaji").show();
            }
            else {
                $("#div_anket_mesaji").hide();
            }

            if ($("#chkSurveySonucMetni").attr('checked') == "checked") {
                $("#div_anket_sonuc_mesaji").show();
            }
            else {
                $("#div_anket_sonuc_mesaji").hide();
            }
        });


        function pageLoad() {

        }

        function Kontrol() {
            if ($.trim($('#MainContent_txtSurveyAdi').val()) == "" || $.trim($('#baslangic_tarihi').val()) == "" || $.trim($('#bitis_tarihi').val()) == "" || $('#has_error').val() == "1") {
                $('#error_message').show();
            }
            else {
                $('#error_message').hide();
            }

        }

        function CheckGorunenAd() {
            if ($("#ChkGorunenAd").attr('checked') == "checked") {
                $("#div_gorunen_ad").show();
            }
            else {
                $("#div_gorunen_ad").hide();
            }
        }

        function ChkSurveyMesaji() {
            if ($("#ChkSurveyMesajiEkle").attr('checked') == "checked") {
                $("#div_anket_mesaji").show();
            }
            else {
                $("#div_anket_mesaji").hide();
            }
        }

        function ChkSurveySonucMetni() {
            if ($("#chkSurveySonucMetni").attr('checked') == "checked") {
                $("#div_anket_sonuc_mesaji").show();
            }
            else {
                $("#div_anket_sonuc_mesaji").hide();
            }
        }

        function checkFileExtension(elem) {
            var filePath = elem.value;

            if (filePath.indexOf('.') == -1)
                return false;

            var validExtensions = new Array();
            var ext = filePath.substring(filePath.lastIndexOf('.') + 1).toLowerCase();
            //Add valid extentions in this array
            validExtensions[0] = 'jpg';
            validExtensions[1] = 'jpeg';
            validExtensions[2] = 'png';
            validExtensions[3] = 'gif';


            for (var i = 0; i < validExtensions.length; i++) {
                if (ext == validExtensions[i])
                    return true;
            }

            alert('Please Select .jpg .jpeg .png add .gif files.');

            return false;
        }
    </script>
    <div style="width: 100%; margin: auto;">
        <div id="smoothmenu1" class="ddsmoothmenu">
            <ul class="mainMenu">
                <li><a href="AnketDashboard.aspx">BACK</a></li>
            </ul>
        </div>
        <div id="mainCenter">
            <div class="boxCenter">
                <div class="boxCenterHeaders">
                    New Survey Infos
                </div>
                <div class="boxCenterContent">
                
                    <table cellpadding="0" cellspacing="0">
                        <tr>
                            <td colspan="4">
                                <img src="../Images/_spacer.gif" alt="" width="10px" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <img src="../Images/_spacer.gif" alt="" width="10px" />
                            </td>
                            <td colspan="2">
                                <div class="ui-widget" id="error_message" runat="server" clientidmode="Static">
                                    <div class="ui-state-error ui-corner-all" style="padding: 15px;">
                                        <p>
                                            <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                                            <asp:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="ValidationGroup" />
                                        </p>
                                    </div>
                                </div>
                            </td>
                            <td>
                                <img src="../Images/_spacer.gif" alt="" width="10px" />
                            </td>
                        </tr>
                    
                        <tr>
                            <td colspan="4">
                             
                        <asp:Literal ID="ltl_ucretsiz_uyelik" runat="server" Visible="false"></asp:Literal>
                        <asp:Literal ID="ltl_anket_sayisi" runat="server" Visible="false"></asp:Literal>
                            <div style="padding:5px 5px 5px 5px;display:none;" >
                        <p class="warning" ><span class="bold"><b>Survey Type :</b></span><asp:Literal ID="ltl_anket_tipi_uyari" runat="server"></asp:Literal></p>
                        </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <img src="../Images/_spacer.gif" alt="" width="10px" />
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                            <label>
                                                Survey Name :
                                            </label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="pollSection">
                                                <asp:TextBox ID="txtSurveyAdi" runat="server" CssClass="textEntry" Width="380px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="NameRequired" runat="server" ControlToValidate="txtSurveyAdi"
                                                    ErrorMessage="Please Input Survey Name." ToolTip="Please Input Survey Name."
                                                    ValidationGroup="ValidationGroup" CssClass="failureNotification">*</asp:RequiredFieldValidator>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                            </td>
                            <td>
                                <img src="../Images/_spacer.gif" alt="" width="10px" />
                            </td>
                        </tr>
                        <tr id="div_grup" runat="server">
                            <td>
                                <img src="../Images/_spacer.gif" alt="" width="10px" />
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                            <label>
                                                Survey Type :
                                            </label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                         
                                            <div class="pollSection">
                                                <asp:DropDownList ID="ddlTip" runat="server" CssClass="textEntry" Width="450px" AutoPostBack="true">
                                                </asp:DropDownList>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                            <label>
                                                Team :
                                            </label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="pollSection">
                                                <asp:DropDownList ID="ddlgrup" runat="server" CssClass="textEntry" Width="350px" AutoPostBack="true">
                                                </asp:DropDownList>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                                <img src="../Images/_spacer.gif" alt="" width="10px" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <img src="../Images/_spacer.gif" alt="" width="10px" />
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                            <label for="baslangic_tarihi">
                                                Start Date :
                                            </label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="pollSection">
                                                <input type="text" id="baslangic_tarihi" runat="server" clientidmode="Static" readonly="readonly"
                                                    style="width: 190px" />
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="baslangic_tarihi"
                                                    CssClass="failureNotification" ErrorMessage="Please Input Start Date."
                                                    ToolTip="Please Input Start Date." ValidationGroup="ValidationGroup">*</asp:RequiredFieldValidator>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                            <label for="baslangic_tarihi">
                                                End Date :
                                            </label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="pollSection">
                                                <input type="text" id="bitis_tarihi" runat="server" clientidmode="Static" readonly="readonly"
                                                    style="width: 190px" /><asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server"
                                                        ControlToValidate="bitis_tarihi" CssClass="failureNotification" ErrorMessage="Please Input End Date."
                                                        ToolTip="Please Input End Date." ValidationGroup="ValidationGroup">*</asp:RequiredFieldValidator>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                                <img src="../Images/_spacer.gif" alt="" width="10px" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4">
                                <img src="../Images/_spacer.gif" alt="" width="10px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="lightBg">
                                <img src="../Images/_spacer.gif" alt="" width="10px" />
                            </td>
                            <td colspan="2">
                                <div class="lightBg" style="width: 100%">
                                    <table cellpadding="5" cellspacing="5">
                                        <tr>
                                            <td width="34%">
                                                <div class="themePreview">
                                                    <label>
                                                        <asp:RadioButton ID="rdSurveyTema1" runat="server" GroupName="tema" Checked="true" />
                                                        Thema 1</label>
                                                    <img src="../img/tema01-thumb2.jpg" alt="" width="300px" />
                                                </div>
                                            </td>
                                            <td>
                                                <img src="../Images/_spacer.gif" width="10px" alt="" />
                                            </td>
                                            <td width="33%">
                                                <div class="themePreview">
                                                    <label>
                                                        <asp:RadioButton ID="rdSurveyTema2" runat="server" GroupName="tema" />
                                                        Thema 2</label>
                                                    <img src="../img/tema02-thumb2.jpg" alt="" width="315px" />
                                                </div>
                                            </td>
                                            <td>
                                                <img src="../Images/_spacer.gif" width="10px" alt=""  />
                                            </td>
                                            <td width="33%">
                                                <div class="themePreview last">
                                                    <label>
                                                        <asp:RadioButton ID="rdSurveyTema3" runat="server" GroupName="tema" />
                                                        Thema 3
                                                    </label>
                                                    <img src="../img/tema03-thumb2.jpg" alt="" width="300px"/>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                            <td class="lightBg">
                                <img src="../Images/_spacer.gif" alt="" width="10px" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" class="lightBg">
                                <img src="../Images/_spacer.gif" alt="" width="10px" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4">
                                <img src="../Images/_spacer.gif" alt="" width="10px" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <img src="../Images/_spacer.gif" alt="" width="10px" />
                            </td>
                            <td colspan="2">
                                <input type="checkbox" id="ChkGorunenAd" runat="server" class="check" onclick="CheckGorunenAd();"
                                    clientidmode="Static" />Show Different Survey Name For Users
                                <div id="div_gorunen_ad">
                                    <table>
                                        <tr>
                                            <td>
                                                <div class="pollSection">
                                                    <asp:TextBox ID="txtGorunenAd" runat="server" CssClass="textEntry" Width="380px"></asp:TextBox>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                            <td>
                                <img src="../Images/_spacer.gif" alt="" width="10px" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4">
                                <img src="../Images/_spacer.gif" alt="" width="10px" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <img src="../Images/_spacer.gif" alt="" width="10px" />
                            </td>
                            <td colspan="2">
                                <label>
                                    <input type="checkbox" id="ChkSurveyMesajiEkle" runat="server" class="check" onclick="ChkSurveyMesaji();"
                                        clientidmode="Static" />Add Survey Message</label>
                                <div id="div_anket_mesaji">
                                    <div class="pollSection">
                                        <asp:TextBox ID="txtSurveyMesaji" runat="server" CssClass="textEntry" TextMode="MultiLine"
                                            Rows="8" Width="640px"></asp:TextBox>
                                    </div>
                                </div>
                            </td>
                            <td>
                                <img src="../Images/_spacer.gif" alt="" width="10px" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4">
                                <img src="../Images/_spacer.gif" alt="" width="10px" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <img src="../Images/_spacer.gif" alt="" width="10px" />
                            </td>
                            <td colspan="2">
                                <label>
                                    <input type="checkbox" id="chkSurveySonucMetni" runat="server" class="check" onclick="ChkSurveySonucMetni();"
                                        clientidmode="Static" />Add Survey Result Message</label>
                                <div id="div_anket_sonuc_mesaji">
                                    <div class="pollSection">
                                        <asp:TextBox ID="txtSurveySonucMesaji" runat="server" CssClass="textEntry" TextMode="MultiLine"
                                            Rows="8" Width="640px"></asp:TextBox>
                                    </div>
                                </div>
                            </td>
                            <td>
                                <img src="../Images/_spacer.gif" alt="" width="10px" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4">
                                <img src="../Images/_spacer.gif" alt="" width="10px" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <img src="../Images/_spacer.gif" alt="" width="10px" />
                            </td>
                            <td colspan="2">
                                <table>
                                    <tr>
                                        <td style="vertical-align: middle">
                                            Add Logo :
                                        </td>
                                        <td style="vertical-align: middle">
                                            <asp:FileUpload ID="fileupload1" runat="server" onchange="return checkFileExtension(this);" />
                                        </td>
                                        <td>
                                            <img src="../Images/_spacer.gif" alt="" width="10px" />
                                        </td>
                                        <td>
                                        <div id="image_div" runat="server">
                                            <asp:Image ID="Image1" runat="server" CssClass="resizeme"/>
                                            </div>
                                        </td>
                                        <td>
                                            <img src="../Images/_spacer.gif" alt="" width="10px" />
                                        </td>
                                        <td>
                                            <asp:Button ID="btnLogoyuSil" runat="server" Text="Delete Logo" OnClick="btnLogoyuSil_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                                <img src="../Images/_spacer.gif" alt="" width="10px" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4">
                                <img src="../Images/_spacer.gif" alt="" width="10px" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <img src="../Images/_spacer.gif" alt="" width="10px" />
                            </td>
                            <td colspan="2">
                                <table>
                                    <tr>
                                        <td>
                                            <label>
                                                Question Count Per Page :
                                            </label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="pollSection">
                                                <asp:DropDownList ID="ddlSoruSayisi" runat="server" CssClass="textEntry" Width="100px">
                                                    <asp:ListItem>1</asp:ListItem>
                                                    <asp:ListItem>2</asp:ListItem>
                                                    <asp:ListItem>3</asp:ListItem>
                                                    <asp:ListItem>4</asp:ListItem>
                                                    <asp:ListItem>5</asp:ListItem>
                                                    <asp:ListItem>6</asp:ListItem>
                                                    <asp:ListItem>7</asp:ListItem>
                                                    <asp:ListItem>8</asp:ListItem>
                                                    <asp:ListItem>9</asp:ListItem>
                                                    <asp:ListItem>10</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td>
                                <img src="../Images/_spacer.gif" alt="" width="10px" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <img src="../Images/_spacer.gif" alt="" width="10px" />
                            </td>
                            <td colspan="2">
                                <asp:Button ID="btnSurveyOlustur" runat="server" Text="Create" ValidationGroup="ValidationGroup"
                                    OnClick="btnSurveyOlustur_Click" OnClientClick="Kontrol();" />
                            </td>
                            <td>
                                <img src="../Images/_spacer.gif" alt="" width="10px" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>
    <input type="hidden" id="has_error" runat="server" clientidmode="Static" />
</asp:Content>
