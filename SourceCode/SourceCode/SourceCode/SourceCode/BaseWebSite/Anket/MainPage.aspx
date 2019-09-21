<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="MainPage.aspx.cs" Inherits="BaseWebSite.Survey.MainPage" %>

<%@ Register Src="~/BaseControls/RepeaterPaging.ascx" TagName="RepeaterPaging" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>My Surveys</title>
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
            $(".datepicker").datepicker({ dateFormat: 'dd-mm-yy', onSelect: function (dateText, inst) {

            }
            });
            $(".phone").mask("(999)999-9999");
            $("input, textarea,  select").uniform();
            ddsmoothmenu.init({
                mainmenuid: "smoothmenu1",
                orientation: 'h',
                classname: 'ddsmoothmenu',
                contentsource: "markup"
            })
        });
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
        });

        $(document).ready(function () {
            if (document.getElementById('search_text').value != null && document.getElementById('search_text').value != undefined) {
                $("#txtSearchBox").val(document.getElementById('search_text').value);
            }
        });


        function pageLoad() {

            $(".numeric").numeric({ decimal: "," });
            $(".integer").numeric(false);
        }

        function ArsiveGonder(anket_uid) {
            $.ajax({
                type: 'POST',
                url: 'AnketAshx/IsGrupAdmin.ashx?user_uid=' + '<%=user_uid%>' + '&grup_uid=' + '<%=grup_uid%>',
                success: function (ajaxCevap) {
                    if (ajaxCevap == '1') {
                        $("#dialogArsiveGonder").dialog({
                            height: 250,
                            width: 550,
                            modal: true,
                            title: 'Send To Archive',
                            buttons: {
                                "Save": function () {
                                    var aciklama = $("#txtArsivAciklamasi").val();
                                    __doPostBack('ArsiveGonder', anket_uid + '^#^' + aciklama);
                                },
                                "Cancel": function () {
                                    $(this).dialog("close");

                                }
                            }

                        });
                    }
                    else {
                        alert('Arşive Gönderme yetkiniz bulunmamaktadır.Bu yetki Ekip yöneticilerine verilmiştir.');
                        return;
                    }
                }
            });

        }

        function ArsivdenCikart(anket_uid) {
            $.ajax({
                type: 'POST',
                url: 'AnketAshx/IsGrupAdmin.ashx?user_uid=' + '<%=user_uid%>' + '&grup_uid=' + '<%=grup_uid%>',
                success: function (ajaxCevap) {
                    if (ajaxCevap == '1') {
                        if (window.confirm("Do you want to remove from Archive?")) {
                            __doPostBack('ArsivdenCikart', anket_uid);
                        }
                    }
                    else {
                        alert('Arşivden Çıkartma yetkiniz bulunmamaktadır.Bu yetki Ekip yöneticilerine verilmiştir.');
                        return;
                    }
                }
            });

        }


        function Goruntule(anket_uid) {
            $("#dialogSurveyGoruntule").dialog({
                height: 550,
                width: 900,
                modal: true
            });


        }

        function Kapat(anket_uid) {

            $.ajax({
                type: 'POST',
                url: 'AnketAshx/IsGrupAdmin.ashx?user_uid=' + '<%=user_uid%>' + '&grup_uid=' + '<%=grup_uid%>',
                success: function (ajaxCevap) {
                    if (ajaxCevap == '1') {
                        $("#dialogSurveyKapat").dialog({
                            height: 250,
                            width: 550,
                            modal: true,
                            title: 'Close Survey',
                            buttons: {
                                "Save": function () {
                                    var aciklama = $("#txtaciklama").val();
                                    __doPostBack('Kapat', anket_uid + '^#^' + aciklama);
                                },
                                "Cancel": function () {
                                    $(this).dialog("close");

                                }
                            }

                        });
                    }
                    else {
                        alert('You are not authorized to close the survey. This authority has been given to the team managers.');
                        return;
                    }
                }
            });


        }

        function Raporla(anket_uid) {
            window.open('Raporlar/ReportViewer.aspx?anket_uid=' + anket_uid + "&grup_uid=<%=grup_uid%>", 'Rapor', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=1000,height=800')
        }


        function HizliAra() {
            __doPostBack('HizliAramadan', $("#txtSearchBox").val());
        }

    </script>
    <div style="width: 100%; margin: auto;">
        <div id="dialogSurveyKapat" title="Soru Ekle" style="display: none; font-size: 10pt">
            <table cellpadding="0" cellspacing="0" width="500px">
                <tr>
                    <td colspan="2">
                        If you Close this Survey It is removed from Live Surveys List 
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <img src="../Images/_spacer.gif" width="20px" alt="" />
                    </td>
                </tr>
                <tr>
                    <td width="150px">
                        <asp:Label ID="lblaciklama" runat="server" Text="Close Description :"></asp:Label>
                    </td>
                     <td width="350px">
                        <asp:TextBox ID="txtaciklama" runat="server" TextMode="MultiLine" Rows="4" Width="330px"
                            ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
        <div id="dialogArsiveGonder" title="Soru Ekle" style="display: none; font-size: 10pt">
            <table cellpadding="0" cellspacing="0" width="500px">
                <tr>
                    <td colspan="2">
                        When Survey Closed If this Survey is online it is removed from Online Surveys .
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <img src="../Images/_spacer.gif" width="20px" alt="" />
                    </td>
                </tr>
                <tr>
                    <td width="150px">
                        <asp:Label ID="Label1" runat="server" Text="Açıklama :"></asp:Label>
                    </td>
                    <td width="350px">
                        <asp:TextBox ID="txtArsivAciklamasi" runat="server" TextMode="MultiLine" Rows="4"
                            Width="330px" ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
        <div id="mainTopSection">
            <div id="smoothmenu1" class="ddsmoothmenu">
                <ul class="mainMenu">
                    <li><a href="AnketDashboard.aspx">BACK</a></li>
                </ul>
                <div class="searchSurvey">
                    <input id="txtSearchBox" type="text" />
                    <input type="button" id="ara" />
                    <asp:DropDownList ID="ddlgrup" runat="server" CssClass="textEntry" Width="200px"
                        AutoPostBack="true" OnSelectedIndexChanged="ddlgrup_SelectedIndexChanged">
                    </asp:DropDownList>
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
                            <asp:LinkButton ID="tumu" runat="server" CssClass="tumu" OnClick="tumu_Click">All</asp:LinkButton></li>
                        <li>
                            <asp:LinkButton ID="aciklar" CssClass="aciklar" runat="server" OnClick="aciklar_Click">Opened</asp:LinkButton></li>
                        <li>
                            <asp:LinkButton ID="yayindakiler" CssClass="yayindakiler" runat="server" OnClick="yayindakiler_Click">Published</asp:LinkButton></li>
                        <li>
                            <asp:LinkButton ID="kapalilar" CssClass="kapalilar" runat="server" OnClick="kapalilar_Click">Closed</asp:LinkButton></li>
                        <li>
                            <asp:LinkButton ID="arsiv" CssClass="arsiv" runat="server" OnClick="arsiv_Click">Archived</asp:LinkButton></li>
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
                        <asp:AsyncPostBackTrigger ControlID="arsiv" />
                        <asp:AsyncPostBackTrigger ControlID="yayindakiler" />
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
                                                <a class="mediumUnderlineLink" href="AnketDetayi.aspx?anket_uid=<%#DataBinder.Eval(Container.DataItem, "anket_uid")%>">
                                                    <%#DataBinder.Eval(Container.DataItem, "anket_adi")%></a></div>
                                            <div class="surveyType">
                                                <%#DataBinder.Eval(Container.DataItem, "group_name")%></div>
                                            <div >
                                                <div><img src="../Images/_spacer.gif" width="10px" alt="" /></div>
                                                <a class="raporla  linkButtonStyle" href="#" onclick="Raporla('<%#DataBinder.Eval(Container.DataItem, "anket_uid")%>');">
                                                    Report</a><a class="kapat  linkButtonStyle" href="#" onclick="Kapat('<%#DataBinder.Eval(Container.DataItem, "anket_uid")%>');">
                                                        Close</a>
                                                <%# ArsiveGonder(DataBinder.Eval(Container.DataItem, "anket_uid"))%>
                                                <div><img src="../Images/_spacer.gif" width="10px" alt="" /></div>
                                            </div>
                                        </div>
                                        <div class="surveyListRight">
                                            <div class="surveyDate">
                                                <%#DataBinder.Eval(Container.DataItem, "baslangic_tarihi_str")%>
                                                -
                                                <%#DataBinder.Eval(Container.DataItem, "bitis_tarihi_str")%></div>
                                            <div class="linkButtonStyle">
                                                <%# SurveyDurumu(DataBinder.Eval(Container.DataItem, "anket_durumu_id"))%></div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                    <div class="surveyList lightBg">
                                        <div class="surveyListLeft">
                                            <div class="surveyName">
                                                <a class="mediumUnderlineLink" href="AnketDetayi.aspx?anket_uid=<%#DataBinder.Eval(Container.DataItem, "anket_uid")%>">
                                                    <%#DataBinder.Eval(Container.DataItem, "anket_adi")%></a></div>
                                            <div class="surveyType">
                                                <%#DataBinder.Eval(Container.DataItem, "group_name")%></div>
                                            <div >
                                                <div><img src="../Images/_spacer.gif" width="10px" alt="" /></div>
                                                <a class="raporla  linkButtonStyle" href="#" onclick="Raporla('<%#DataBinder.Eval(Container.DataItem, "anket_uid")%>');">
                                                    Report</a> <a class="kapat  linkButtonStyle" href="#" onclick="Kapat('<%#DataBinder.Eval(Container.DataItem, "anket_uid")%>');">
                                                        Close</a>
                                                <%# ArsiveGonder(DataBinder.Eval(Container.DataItem, "anket_uid"))%>
                                                <div><img src="../Images/_spacer.gif" width="10px" alt="" /></div>
                                            </div>
                                        </div>
                                        <div class="surveyListRight">
                                            <div class="surveyDate">
                                                <%#DataBinder.Eval(Container.DataItem, "baslangic_tarihi_str")%>
                                                -
                                                <%#DataBinder.Eval(Container.DataItem, "bitis_tarihi_str")%></div>
                                            <div class="linkButtonStyle">
                                                <%# SurveyDurumu(DataBinder.Eval(Container.DataItem, "anket_durumu_id"))%></div>
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
