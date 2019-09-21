<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Duyurular.aspx.cs" Inherits="BaseWebSite.Admin.Duyurular" ValidateRequest="false" %>

<%@ Register Src="~/BaseControls/RepeaterPaging.ascx" TagName="RepeaterPaging" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Notifications</title>
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

            $("input, textarea,  select").uniform();
            ddsmoothmenu.init({
                mainmenuid: "smoothmenu1",
                orientation: 'h',
                classname: 'ddsmoothmenu',
                contentsource: "markup"
            })
        });
    

        function ArsiveGonder(notification_uid) {
            __doPostBack('ArsiveGonder', notification_uid);
        }

        function ArsivdenCikart(notification_uid_uid) {
            __doPostBack('ArsivdenCikart', notification_uid);
        }
        function DuyuruEkle() {


            $("#dialogDuyuruEkle").dialog({
                height: 570,
                width: 750,
                modal: true,
                title: 'Add Notifications',
                buttons: {
                    "Kaydet": function () {
                        var subject = $("#txtbaslik").val(),
			                 notification = $("#txtDuyuru").val();

                        var message = "";
                        if (subject == "") message += "Please Fill Notification Subject Field...\n\r"
                        if (notification == "") message += "Please Fill Notification Field...\n\r"
                       
                        if (message != "") {
                            alert(message);
                        }
                        else {
                            __doPostBack('DuyuruOlustur', subject + '^#^' + notification);
                        }
                    },
                    "Cancel": function () {
                        $(this).dialog("close");

                    }
                }

            });
        }

        function DuyuruDuzenle(notification_uid) {


            $("#dialogDuyuruDuzenle").dialog({
                height: 570,
                width: 750,
                modal: true,
                title: 'Add Notification',
                buttons: {
                    "Kaydet": function () {
                        var subject = $("#txtbaslik_duzenle").val(),
			                 notification = $("#txtDuyuru_duzenle").val();

                        var message = "";
                        if (subject == "") message += "Please Fill Notification Subject Field...\n\r"
                        if (notification == "") message += "Please Fill Notification Field...\n\r"

                        if (message != "") {
                            alert(message);
                        }
                        else {
                            __doPostBack('DuyuruUpdate', subject + '^#^' + notification + '^#^' + notification_uid);
                        }
                    },
                    "Cancel": function () {
                        $(this).dialog("close");

                    }
                }

            });
        }

        function Duzenle(notification_uid) {
            __doPostBack('Duzenle', notification_uid);
        }

    </script>
    <div style="width: 100%; margin: auto;">
    <div id="dialogDuyuruEkle" title="Add Notification" style="display: none; font-size: 10pt">
            <table cellpadding="0" cellspacing="0" width="680px">
                <tr>
                    <td>
                        <asp:Label ID="lblbaslik" runat="server" Text="Notification Subject :"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtbaslik" runat="server"  Width="430px"
                            ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblDuyuru" runat="server" Text="Notification :"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtDuyuru" runat="server" TextMode="MultiLine" Rows="20" Width="430px"
                            ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
        <div id="dialogDuyuruDuzenle" title="Edit Notification" style="display: none; font-size: 10pt">
            <table cellpadding="0" cellspacing="0" width="680px">
                <tr>
                    <td>
                        <asp:Label ID="Label1" runat="server" Text="Notification Subject :"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtbaslik_duzenle" runat="server"  Width="430px"
                            ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label2" runat="server" Text="Notification :"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="txtDuyuru_duzenle" runat="server" TextMode="MultiLine" Rows="20" Width="430px"
                            ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
        <div id="mainTopSection">
            <div id="smoothmenu1" class="ddsmoothmenu">
                <ul class="mainMenu">
                    <li><a href="AdminMain.aspx">SURVEY PANEL</a></li>
                    <li ><a href="#" onclick="DuyuruEkle();return false;">ADD NOTIFICATION</a></li>
                </ul>
            </div>
        </div>
        <div id="mainCenter" style="height: 100%">
            <div class="boxCenter">
                        <div class="boxCenterHeaders">
                            Notifications
                        </div>
                        <div class="boxCenterContent">
                            <asp:Repeater ID="rptDuyurular" runat="server">
                                <HeaderTemplate>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <div class="surveyListCenter">
                                        <div class="surveyListLeft">
                                            <div class="surveyName">
                                                <a class="mediumUnderlineLink" href="#">
                                                    <%#DataBinder.Eval(Container.DataItem, "notification_subject")%></div>
                                            <div class="surveyType">
                                               
                                                </div>
                                                <div class="surveyListButtons">
                                                 <a class="duzenle linkButtonStyle" href="#" onclick="Duzenle('<%#DataBinder.Eval(Container.DataItem, "notification_uid")%>');">
                                        Düzenle</a>
                                                    <%# ArsiveGonder(DataBinder.Eval(Container.DataItem, "notification_uid"))%>
                                                </div>
                                                
                                          </div>
                                            <div class="surveyListRight">
                                            <div class="surveyDate">
                                                <%#DataBinder.Eval(Container.DataItem, "notification_creation_date")%>
                                                </div>
                                             <div class="surveyStatus">
                                                <%# DuyuruDurumu(DataBinder.Eval(Container.DataItem, "notification_statu"))%>
                                                </div>
                                        </div>
                                        </div>
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                     <div class="surveyListCenter lightBg">
                                        <div class="surveyListLeft">
                                            <div class="surveyName">
                                                <a class="mediumUnderlineLink" href="#">
                                                    <%#DataBinder.Eval(Container.DataItem, "notification_subject")%></div>
                                            <div class="surveyType">
                                               
                                                </div>
                                                <div class="surveyListButtons">
                                                 <a class="duzenle linkButtonStyle" href="#" onclick="Duzenle('<%#DataBinder.Eval(Container.DataItem, "notification_uid")%>');">
                                        Edit</a>
                                                    <%# ArsiveGonder(DataBinder.Eval(Container.DataItem, "notification_uid"))%>
                                                </div>
                                                
                                          </div>
                                            <div class="surveyListRight">
                                            <div class="surveyDate">
                                                <%#DataBinder.Eval(Container.DataItem, "notification_creation_date")%>
                                                </div>
                                             <div class="surveyStatus">
                                                <%# DuyuruDurumu(DataBinder.Eval(Container.DataItem, "notification_statu"))%>
                                                </div>
                                        </div>
                                    </div>
                                </AlternatingItemTemplate>
                            </asp:Repeater>
                        </div>
                        <uc1:RepeaterPaging ID="rptDuyurularPaging" runat="server" />
            </div>
        </div>
    </div>
</asp:Content>
