<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="KullaniciGruplar.aspx.cs" Inherits="BaseWebSite.Survey.KullaniciGruplar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <title>Kullanıcı İşlemleri</title>
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


        $(document).ready(function () {
        });


        function pageLoad() {

        }
    </script>
    <div style="width: 100%; margin: auto;">
        <div id="mainTopSection">
            <div id="smoothmenu1" class="ddsmoothmenu">
                <ul class="mainMenu">
                    <li><a href="AnketDashboard.aspx">BACK</a></li>
                </ul>
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
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="tumu" />
                        <asp:AsyncPostBackTrigger ControlID="yonettigim_gruplar" />
                        <asp:AsyncPostBackTrigger ControlID="uyeolunan_gruplar" />
                    </Triggers>
                    <ContentTemplate>
                        <div class="boxRightHeaders">
                            <asp:LinkButton ID="LinkButtonHeader" Font-Size="23px" ForeColor="White" Font-Underline="false"
                                runat="server"></asp:LinkButton>
                        </div>
                        <div class="boxRightContent">
                            <asp:Repeater ID="rptGruplar" runat="server">
                                <HeaderTemplate>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <div class="surveyList">
                                        <div class="surveyListLeft">
                                            <div class="surveyName">
                                                <a class="mediumUnderlineLink" href="GrupKullanicilari.aspx?grup_uid=<%#DataBinder.Eval(Container.DataItem, "grup_uid")%>">
                                                    <%#DataBinder.Eval(Container.DataItem, "grup_adi")%>
                                                </a>
                                            </div>
                                            <div class="surveyType">
                                                <%#DataBinder.Eval(Container.DataItem, "admin")%>
                                            </div>
                                     <div class="surveyListButtons">
                                     </div>
                                        </div>
                                    
                                    </div>
                                    
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                    <div class="surveyList lightBg">
                                            <div class="surveyListLeft">
                                                <div class="surveyName">
                                                    <a class="mediumUnderlineLink" href="GrupKullanicilari.aspx?grup_uid=<%#DataBinder.Eval(Container.DataItem, "grup_uid")%>">
                                                        <%#DataBinder.Eval(Container.DataItem, "grup_adi")%>
                                                    </a>
                                                </div>
                                                <div class="surveyType">
                                                    <%#DataBinder.Eval(Container.DataItem, "admin")%>
                                                </div>
                                                <div class="surveyListButtons">
                                     </div>
                                            </div>
                                        </div>
                                        
                                </AlternatingItemTemplate>
                            </asp:Repeater>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
</asp:Content>
