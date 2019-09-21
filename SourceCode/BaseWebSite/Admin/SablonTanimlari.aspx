<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="SablonTanimlari.aspx.cs" Inherits="BaseWebSite.Admin.SablonTanimlari" %>

<%@ Register Src="~/BaseControls/RepeaterPaging.ascx" TagName="RepeaterPaging" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Şablonlar</title>
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
            $("#ara").button({
                text: false
            }).click(function () {
                HizliAra();
            });

            $("#txtSearchBox").keydown(function () {
                if (event.keyCode == '13') {
                    $("#ara").focus();
                }
            });

            $("input, textarea,  select").uniform();
        });

        $(document).ready(function () {
            if (document.getElementById('search_text').value != null && document.getElementById('search_text').value != undefined) {
                $("#txtSearchBox").val(document.getElementById('search_text').value);
            }
        });


        function pageLoad() {

        }


        function HizliAra() {
            __doPostBack('HizliAramadan', $("#txtSearchBox").val());
        }

    </script>
    <div style="width: 100%; margin: auto;">
        <div id="mainTopSection">
            <div id="smoothmenu1" class="ddsmoothmenu">
                <ul class="mainMenu">
                    <li><a href="AdminMain.aspx">ANKET PANELİ</a></li>
                    <li><a href="YeniSablon.aspx">ŞABLON EKLE</a></li>
                </ul>
                <div class="searchSurvey">
                    <input id="txtSearchBox" type="text" />
                    <input type="button" id="ara" />
                </div>
            </div>
        </div>
        <div id="mainLeft">
            <div class="boxLeft">
                <div class="boxLeftHeaders">
                    Folders
                </div>
                <div class="boxLeftContent">
                    <ul class="surveyFilterMenu">
                        <li>
                            <asp:LinkButton ID="tumu" CssClass="tumu" runat="server" OnClick="tumu_Click">Tüm Şablon Listesi</asp:LinkButton></li>
                        <li>
                            <asp:LinkButton ID="aciklar" CssClass="aciklar" runat="server" OnClick="aciklar_Click">Tüm Açık Şablonlar Listesi</asp:LinkButton></li>
                        <li>
                            <asp:LinkButton ID="kapalilar" CssClass="kapalilar" runat="server" OnClick="kapalilar_Click">Tüm Kapalı Şablonlar Listesi</asp:LinkButton></li>
                    </ul>
                </div>
            </div>
        </div>
        <div id="mainRight" style="height: 100%">
            <div class="boxRight">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="tumu" />
                        <asp:AsyncPostBackTrigger ControlID="aciklar" />
                        <asp:AsyncPostBackTrigger ControlID="kapalilar" />
                    </Triggers>
                    <ContentTemplate>
                        <div class="boxRightHeaders">
                            <asp:LinkButton ID="LinkButtonHeader" Font-Size="23px" ForeColor="White" Font-Underline="false"
                                runat="server"></asp:LinkButton>
                        </div>
                        <div class="boxRightContent">
                            <asp:Repeater ID="rptSurveyler" runat="server">
                                <HeaderTemplate>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <div class="surveyList">
                                        <div class="surveyListLeft">
                                            <div class="surveyName">
                                                <a class="mediumUnderlineLink" href="SablonDetayi.aspx?sablon_uid=<%#DataBinder.Eval(Container.DataItem, "sablon_uid")%>">
                                                    <%#DataBinder.Eval(Container.DataItem, "sablon_adi")%></a>
                                            </div>
                                            <div class="surveyType">
                                                <%#DataBinder.Eval(Container.DataItem, "kategori_kodu")%>
                                            </div>
                                            <div class="surveyListButtons">
                                             </div>
                                        </div>
                                        <div class="surveyListRight">
                                            <div class="surveyStatus">
                                                <%# SablonDurumu(DataBinder.Eval(Container.DataItem, "sablon_durumu_id"))%>
                                            </div>
                                        </div>
                                        </div>
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                    <div class="surveyList lightBg">
                                        <div class="surveyListLeft">
                                            <div class="surveyName">
                                                <a class="mediumUnderlineLink" href="SablonDetayi.aspx?sablon_uid=<%#DataBinder.Eval(Container.DataItem, "sablon_uid")%>">
                                                    <%#DataBinder.Eval(Container.DataItem, "sablon_adi")%></a>
                                            </div>
                                            <div class="surveyType">
                                                <%#DataBinder.Eval(Container.DataItem, "kategori_kodu")%>
                                            </div>
                                             <div class="surveyListButtons">
                                             </div>
                                        </div>
                                        <div class="surveyListRight">
                                            <div class="surveyStatus">
                                                <%# SablonDurumu(DataBinder.Eval(Container.DataItem, "sablon_durumu_id"))%>
                                            </div>
                                        </div>
                                    </div>
                                </AlternatingItemTemplate>
                            </asp:Repeater>
                        </div>
                        <uc1:RepeaterPaging ID="rptSurveylerPaging" runat="server" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
    <input type="hidden" id="search_text" runat="server" clientidmode="Static" />
</asp:Content>
