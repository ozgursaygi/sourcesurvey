<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="GrupKullanicilari.aspx.cs" Inherits="BaseWebSite.Survey.GrupKullanicilari" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>Kullanıcılar</title>
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
    <script type="text/javascript">

        $(function () {

        });

        function DavetEttiklerim() {
            $("#dialogDavetEttiklerim").dialog({
                height: 450,
                width: 750,
                modal: true
            });
            return false;
        }

        function DavetEt() {
            $("#dialogDavet").dialog({
                height: 200,
                width: 680,
                modal: true,
                title: 'Davet',
                buttons: {
                    "Kaydet": function () {
                        if (window.confirm('İlgili E-Posta adresini davet etmek istiyor musunuz?')) {
                            var eposta = $("#txteposta").val()
                            var message = "";

                            if (!validateEmail('txteposta')) {
                                alert("Lütfen geçerli bir e-posta adresi giriniz.");
                                return;
                            }

                            if (eposta == "") message += "Lütfen e-posta adresini giriniz.\n\r"

                            if (message != "") {
                                alert(message);
                            }
                            else {
                                __doPostBack('DavetEt', eposta);
                            }
                        }
                    },
                    "Vazgeç": function () {
                        $(this).dialog("close");

                    }
                }

            });
        }

        function YoneticiYap(grup_uid, kullanici_uid) {
            if (window.confirm("Yönetici Yapmak istiyor musunuz?")) {
                __doPostBack('YoneticiYap', grup_uid + '^#^' + kullanici_uid);
            }
        }

        function YoneticiliktenCikart(grup_uid, kullanici_uid) {
            if (window.confirm("Yöneticilikten Çıkartmak istiyor musunuz?")) {
                __doPostBack('YoneticiliktenCikart', grup_uid + '^#^' + kullanici_uid);
            }
        }

        $(document).ready(function () {
            
        });


        function pageLoad() {
            
        }

        function validateEmail(txtEmail) {
            var a = document.getElementById(txtEmail).value;
            var filter = /^[a-zA-Z0-9_.-]+@[a-zA-Z0-9]+[a-zA-Z0-9.-]+[a-zA-Z0-9]+.[a-z]{1,4}$/;
            if (filter.test(a)) {
                return true;
            }
            else {
                return false;
            }
        }
    </script>
    <div style="width: 100%; margin: auto;">
        <div id="dialogDavet" title="Soru Ekle" style="display: none; font-size: 10pt">
            <table cellpadding="0" cellspacing="0" width="630px">
                <tr>
                    <td colspan="2">
                        <img src="../Images/_spacer.gif" height="10px" width="10px" />
                    </td>
                </tr>
                <tr>
                    <td width="200px">
                        <asp:Label ID="lbldavet" runat="server" Text="Davet Edilecek E-Posta Adresi :"></asp:Label>
                    </td>
                    <td width="300px">
                        <asp:TextBox ID="txteposta" runat="server" Width="300px" ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
        <div id="dialogDavetEttiklerim" title="Davet Ettiklerim" style="display: none; font-size: 10pt">
            <table cellpadding="0" cellspacing="0" width="630px">
                <tr>
                    <td colspan="2">
                    <asp:Repeater ID="rptDavetEttiklerim" runat="server">
                        <HeaderTemplate>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <div class="surveyList">
                                <div class="surveyListLeft">
                                    <div class="surveyName">
                                        <a class="mediumUnderlineLink" href="#">
                                             <%#DataBinder.Eval(Container.DataItem, "davet_edilen_email")%>
                                        </a>
                                    </div>
                                    <div class="surveyType">
                                        
                                    </div>
                                </div>
                                <div class="surveyListRight">
                                    <div class="surveyDate">
                                        <%#DataBinder.Eval(Container.DataItem, "davet_tarihi_str")%>
                                    </div>
                                </div>
                             </div>
                        </ItemTemplate>
                        <AlternatingItemTemplate>
                            <div class="surveyList lightBg">
                                <div class="surveyListLeft">
                                    <div class="surveyName">
                                        <a class="mediumUnderlineLink" href="#">
                                            <%#DataBinder.Eval(Container.DataItem, "davet_edilen_email")%>
                                        </a>
                                    </div>
                                    <div class="surveyType">
                                    </div>
                                </div>
                                <div class="surveyListRight">
                                    <div class="surveyDate">
                                         <%#DataBinder.Eval(Container.DataItem, "davet_tarihi_str")%>
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
        <div id="mainTopSection">
            <div id="smoothmenu1" class="ddsmoothmenu">
                <asp:Literal ID="ltlMenu" runat="server"></asp:Literal>
            </div>
        </div>
        <div id="mainLeft">
            <div class="boxLeft">
                <div class="boxLeftHeaders">
                    Ekipler
                </div>
                <div class="boxLeftContent">
                    <ul class="surveyFilterMenu">
                        <li>
                            <asp:LinkButton ID="tumu" runat="server" CssClass="tumu" OnClick="tumu_Click">Tümü</asp:LinkButton></li>
                        <li>
                            <asp:LinkButton ID="yonettigim_gruplar" CssClass="aciklar" runat="server" OnClick="yonettigim_Click">Yönettiğim Ekip</asp:LinkButton></li>
                        <li>
                            <asp:LinkButton ID="uyeolunan_gruplar" CssClass="yayindakiler" runat="server" OnClick="uyeolunan_Click">Üyesi Olduğum Ekipler</asp:LinkButton></li>
                    </ul>
                </div>
            </div>
        </div>
        <div id="mainRight" style="height: 100%">
            <div class="boxRight">
                <div class="boxRightHeaders">
                    <asp:LinkButton ID="LinkButtonHeader" Font-Size="23px" ForeColor="White" Font-Underline="false"
                        runat="server"></asp:LinkButton>
                </div>
                <div class="boxRightContent">
                    <asp:Repeater ID="rptGrupKullanicilari" runat="server">
                        <HeaderTemplate>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <div class="surveyList">
                                <div class="surveyListLeft">
                                    <div class="surveyName">
                                        <a class="mediumUnderlineLink" href="GrupKullanicilari.aspx?grup_uid=<%#DataBinder.Eval(Container.DataItem, "grup_uid")%>">
                                            <%#DataBinder.Eval(Container.DataItem, "ad")%>
                                            <%#DataBinder.Eval(Container.DataItem, "soyad")%>
                                        </a>
                                    </div>
                                    <div class="surveyType">
                                        <%#DataBinder.Eval(Container.DataItem, "admin")%>
                                    </div>
                                    <div class="surveyListButtons">
                                     </div>
                                </div>
                                <div class="surveyListRight">
                                    <div class="surveyDate">
                                        <%#DataBinder.Eval(Container.DataItem, "email")%>
                                    </div>
                                </div>
                             </div>
                        </ItemTemplate>
                        <AlternatingItemTemplate>
                            <div class="surveyList lightBg">
                                <div class="surveyListLeft">
                                    <div class="surveyName">
                                        <a class="mediumUnderlineLink" href="GrupKullanicilari.aspx?grup_uid=<%#DataBinder.Eval(Container.DataItem, "grup_uid")%>">
                                            <%#DataBinder.Eval(Container.DataItem, "ad")%>
                                            <%#DataBinder.Eval(Container.DataItem, "soyad")%>
                                        </a>
                                    </div>
                                    <div class="surveyType">
                                        <%#DataBinder.Eval(Container.DataItem, "admin")%>
                                    </div>
                                    <div class="surveyListButtons">
                                     </div>
                                </div>
                                <div class="surveyListRight">
                                    <div class="surveyDate">
                                        <%#DataBinder.Eval(Container.DataItem, "email")%>
                                    </div>
                                </div>
                            </div>
                        </AlternatingItemTemplate>
                        <FooterTemplate>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
