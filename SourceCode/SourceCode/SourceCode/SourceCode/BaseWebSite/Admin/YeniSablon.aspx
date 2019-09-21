<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="YeniSablon.aspx.cs" Inherits="BaseWebSite.Admin.YeniSablon" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../Styles/cupertino/jquery-ui-1.8.17.custom.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" type="text/css" href="../css/custom-theme/jquery-ui-1.8.20.custom.css" />
    <link rel="stylesheet" href="../css/uniform.default.css" type="text/css" media="screen" />
    <link rel="stylesheet" type="text/css" href="../css/ddsmoothmenu.css" />
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/jquery-ui-1.8.20.custom.min.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.numeric.js" type="text/javascript"></script>
    <script src="../Scripts/jquery.maskedinput-1.2.2.js" type="text/javascript"></script>
    <script src="../js/ddsmoothmenu.js" type="text/javascript"></script>
    <script src="../js/jquery.uniform.js" type="text/javascript" charset="utf-8"></script>
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

        });

        $(document).ready(function () {

            $('#error_message').hide();
            if ($('#has_error').val() == "1") {
                $('#error_message').show();
            }


        });

        function pageLoad() {

        }

        function Kontrol() {
            if ($.trim($('#MainContent_txtSablonAdi').val()) == "" ) {
                $('#error_message').show();
            }
            else {
                $('#error_message').hide();
            }

        }
    </script>
    <div style="width: 100%; margin: auto;">
        <div id="smoothmenu1" class="ddsmoothmenu">
            <ul class="mainMenu">
                <li><a href="AdminMain.aspx">YÖNETİM PANELİ</a></li>
                <li><a href="SablonTanimlari.aspx">ŞABLONLAR</a></li>
            </ul>
        </div>
        <div id="mainCenter">
            <div class="boxCenter">
                <div class="boxCenterHeaders">
                    Yeni Şablon Bilgileri
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
                                            <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="ValidationGroup" />
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
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                            <label>
                                                Şablon Adı :
                                            </label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="pollSection">
                                                <asp:TextBox ID="txtSablonAdi" runat="server" CssClass="textEntry" Width="380px"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="NameRequired" runat="server" ControlToValidate="txtSablonAdi"
                                                    CssClass="failureNotification" ErrorMessage="Lütfen Şablon Adı giriniz." ToolTip="Lütfen Şablon Adı giriniz."
                                                    ValidationGroup="ValidationGroup">*</asp:RequiredFieldValidator>
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
                                                Şablon Kategorisi:
                                            </label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="pollSection">
                                                <asp:DropDownList ID="ddlKategori" runat="server" CssClass="textEntry" Width="320px">
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
                                <asp:Button ID="btnSablonOlustur" runat="server" Text="Kaydet" ValidationGroup="ValidationGroup"
                                    OnClick="btnSablonOlustur_Click" OnClientClick="Kontrol();" />
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
</asp:Content>
