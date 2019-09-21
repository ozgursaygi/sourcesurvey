<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="AnketDashboard.aspx.cs" Inherits="BaseWebSite.Survey.AnketDashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link rel="stylesheet" type="text/css" href="../css/custom-theme/jquery-ui-1.8.20.custom.css" />
    <link rel="stylesheet" href="../css/uniform.default.css" type="text/css" media="screen" />
    <link rel="stylesheet" type="text/css" href="../css/ddsmoothmenu.css" />
    <style type="text/css">
        #sortable
        {
            list-style-type: none;
            margin: 0;
            padding: 0;
            width: 60%;
        }
        #sortable li
        {
            margin: 0 3px 3px 3px;
            padding: 0.4em;
            padding-left: 1.5em;
            font-size: 1.4em;
            height: 18px;
        }
        #sortable li span
        {
            position: absolute;
            margin-left: -1.3em;
        }
    </style>
    <style type="text/css">
        .column
        {
            width: 350px;
            float: left;
            padding-bottom: 100px;
            padding-left: 10px;
        }
        .portlet
        {
            margin: 0 1em 1em 0;
        }
        .portlet-header
        {
            margin: 0.3em;
            padding-bottom: 4px;
            padding-left: 0.2em;
        }
        .portlet-header .ui-icon
        {
            float: right;
        }
        .portlet-content
        {
            padding: 0.4em;
        }
        .ui-sortable-placeholder
        {
            border: 1px dotted black;
            visibility: visible !important;
            height: 50px !important;
        }
        .ui-sortable-placeholder *
        {
            visibility: hidden;
        }
    </style>
    <script src="../js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../js/jquery-ui-1.8.20.custom.min.js" type="text/javascript"></script>
    <script src="../js/ddsmoothmenu.js" type="text/javascript"></script>
    <script src="../js/jquery.uniform.js" type="text/javascript" charset="utf-8"></script>
    <script type="text/javascript">
        $(function () {
            $(".sortable").sortable({
                revert: true
            });
            $("ul, li").disableSelection();

            $(".column").sortable({
                connectWith: ".column"
            });

            $(".portlet").addClass("ui-widget ui-widget-content ui-helper-clearfix ui-corner-all")
			.find(".portlet-header")
				.addClass("ui-widget-header ui-corner-all")
				.prepend("<span class='ui-icon ui-icon-minusthick'></span>")
				.end()
			.find(".portlet-content");

            $(".portlet-header .ui-icon").click(function () {
                $(this).toggleClass("ui-icon-minusthick").toggleClass("ui-icon-plusthick");
                $(this).parents(".portlet:first").find(".portlet-content").toggle();
            });

            $(".column").disableSelection();

            $("input, textarea,  select").uniform();
        });

        function DuyuruGoster(duyuru_uid) {
            __doPostBack('DuyuruGoster', duyuru_uid);
        }

        function DuyuruGoster1(duyuru_uid) {


            $("#dialogDuyuru").dialog({
                height: 570,
                width: 750,
                modal: true,
                title: 'Notification'
            });
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="width: 100%; margin: auto;">
     <div id="dialogDuyuru" title="Duyuru Ekle" style="display: none; font-size: 10pt">
            <table cellpadding="0" cellspacing="0" width="680px">
                <tr>
                    <td>
                        <asp:Label ID="lblbaslik" runat="server" Text="Notification Subject :"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtbaslik" runat="server"  Width="430px"
                            ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td >
                        <asp:Label ID="lblDuyuru" runat="server" Text="Notification :"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtDuyuru" runat="server" TextMode="MultiLine" Rows="20" Width="430px"
                            ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
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
                    Functions
                </div>
                <div class="boxLeftContent">
                    <ul class="surveyFilterMenu">
                        <li>
                            <div id="draggable_coklu_secenek">
                                <a href="YeniAnket.aspx" class="tumu">Crate New Survey</a></div>
                        </li>
                        <li>
                            <div id="draggable_coklu_secenek_coklu_secim">
                                <a href="MainPage.aspx?menu_id=3" class="aciklar">Survey List</a></div>
                        </li>
                        <%--<li >
                            <div id="draggable_matris">
                                <a href="AdresGruplari.aspx" class="yayindakiler">E-Mail Groups</a></div>
                        </li>--%>
                       <%-- <li>
                            <div id="draggable_text">
                                <a href="KullaniciGruplar.aspx" class="kapalilar">User Operations</a></div>
                        </li>--%>
                        <%--   <li>
                            <div id="draggable_coklu_text">
                                <a href="RaporPage.aspx" class="arsiv">Raporlar</a></div>
                        </li>--%>
                    </ul>
                </div>
            </div>
        </div>
        <div id="mainRight" class="droppable" style="height: 100%">
            <div class="boxRight">
                <div class="boxRightHeaders">
                    Panels
                </div>
                <div class="demo">
                    <div class="column">
                        <div class="portlet">
                            <div class="portlet-header">
                                <a href="MainPage.aspx?menu_id=3">Surveys</a></div>
                            <div class="portlet-content">
                                <asp:Literal ID="ltlSurveyler" runat="server"></asp:Literal>
                            </div>
                        </div>
                       <%-- <div class="portlet">
                            <div class="portlet-header">
                                <a href="Messages.aspx?mesaj_durumu_id=1">Messages</a></div>
                            <div class="portlet-content">
                                <asp:Literal ID="ltlMesajlar" runat="server"></asp:Literal>
                            </div>
                        </div>--%>
                        <%--<div class="portlet">
                            <div class="portlet-header">
                                Notifications</div>
                            <div class="portlet-content">
                                <asp:Literal ID="ltlDuyurular" runat="server"></asp:Literal>
                            </div>
                        </div>--%>
                    </div>
                    <div class="column">
                    <div class="portlet">
                        <div class="portlet-header">
                            <a href="MainPage.aspx?anket_durumu_id=4">Published Surveys</a></div>
                        <div class="portlet-content">
                            <asp:Literal ID="ltlYayindakiler" runat="server"></asp:Literal></div>
                    </div>
                    <%--<div class="portlet">
                        <div class="portlet-header">
                            <a href="AdresGruplari.aspx">E-Mail Groups</a></div>
                        <div class="portlet-content">
                            <asp:Literal ID="ltlMailGruplari" runat="server"></asp:Literal></div>
                    </div>--%>
                </div>
                </div>
                
            </div>
        </div>
    </div>
</asp:Content>
