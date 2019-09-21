<%@ Page Title="Yönetici Paneli" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="AdminMain.aspx.cs" Inherits="BaseWebSite.Admin.AdminMain" %>

<%@ Register Src="~/BaseControls/RepeaterPaging.ascx" TagName="RepeaterPaging" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../Styles/cupertino/jquery-ui-1.8.17.custom.css" rel="stylesheet" type="text/css" />
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

        $(document).ready(function () {
        });


        $(function () {

            $("input, textarea,  select").uniform();

        });

        function pageLoad() {

        }



    </script>
    <div style="width: 100%; margin: auto;">
        <div id="mainTopSection">
            <div id="smoothmenu1" class="ddsmoothmenu">
                <ul class="mainMenu">
                    <%--<li><a href="SablonTanimlari.aspx">ŞABLONLAR</a></li>--%>
                    <%--<li><a href="Duyurular.aspx">NOTIFICATIONS</a></li>--%>
                </ul>
                <div class="searchSurvey">
                    <asp:DropDownList ID="ddlListe" runat="server" CssClass="textEntry" Width="200px"
                        AutoPostBack="true" OnSelectedIndexChanged="ddlListe_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>
                <div class="searchSurvey">
                    <asp:DropDownList ID="ddlanketler" runat="server" CssClass="textEntry" Width="400px"
                        AutoPostBack="true" 
                        onselectedindexchanged="ddlanketler_SelectedIndexChanged" >
                    </asp:DropDownList>
                </div>
                
            </div>
        </div>
        <div id="mainCenter" style="height: 100%">
            <div class="boxCenter">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    
                    <ContentTemplate>
                        <div class="boxCenterHeaders" id="button_header" runat="server">
                            <asp:LinkButton ID="LinkButtonHeader" Font-Size="23px" ForeColor="White" Font-Underline="false"
                                runat="server"></asp:LinkButton>
                        </div>
                        <div class="boxCenterContent">
                            <iframe id="iframeMap" runat="server" frameborder="0" height="550px" width="100%">
                            </iframe>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
        </div>
</asp:Content>
