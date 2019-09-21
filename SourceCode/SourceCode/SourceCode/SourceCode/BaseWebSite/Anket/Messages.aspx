<%@ Page Title="Mesajlar" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Messages.aspx.cs" Inherits="BaseWebSite.Messages" ValidateRequest="false" %>

<%@ Register Src="~/BaseControls/RepeaterPaging.ascx" TagName="RepeaterPaging" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Messages</title>
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
        var tip = "1";
        var glb_mesaj_durumu = 1;
        
        $(function () {
            function result(message) {
                $("#result_uyeler").append(message);
                $("#result_uyeler").attr("scrollTop", 0);
            }

            $("#uyeler").autocomplete({
                source: "AnketAshx/UyeGetir.ashx",
                minLength: 1,
                select: function (event, ui) {
                    result(ui.item ?
					"<li id=\"liuyeler_" + ui.item.id + "\" style=\"list-style: none;\"><label onclick=\"DeleteUyelerRow('" + ui.item.id + "')\" style=\"color:Red\"> X </label> <span class=\"smallUnderlineLink\" >" + ui.item.value + "</span></li>" : "");
                },
                close: function (event, ui) { $("#uyeler").val(''); }
            });
        });

        $(document).ready(function () {


            $('#txtMessage').keyup(function () {
                var len = this.value.length;
                if (len >= 400) {
                    this.value = this.value.substring(0, 400);
                }
                $('#charLeft').text(401 - len);
            });

            $('#txtUyelereMesaj').keyup(function () {
                var len = this.value.length;
                if (len >= 400) {
                    this.value = this.value.substring(0, 400);
                }
                $('#charLeftUye').text(401 - len);
            });

            $("#uyeler").width(337);
        });

        function GelenKutusu() {
            tip = "1";
            glb_mesaj_durumu = 1;
        }

        function GidenKutusu() {
            tip = "2";
            glb_mesaj_durumu = 1;
        }

        function SilinenGelenKutusu() {
            tip = "3";
            glb_mesaj_durumu = 0;
        }

        function SilinenGidenKutusu() {
            tip = "4";
            glb_mesaj_durumu = 0;
        }

        function DeleteUyelerRow(id) {
            $("#liuyeler_" + id).remove();
        }

        function pageLoad() {
            if (glb_mesaj_durumu == 1)
                $('.yetkibutton').show();
            else
                $('.yetkibutton').hide();

        }


        function Cevapla(message_uid) {

            $("#dialogMesajGonder").dialog({
                height: 400,
                width: 680,
                modal: true,
                title: 'Mesaj Gönder',
                buttons: {
                    "Send": function () {
                        if (window.confirm('Do you want to send message?')) {
                            var subject = $("#txtSubject").val(),
			                 message = $("#txtMessage").val();
                            subject = $.trim(subject);
                            message = $.trim(message);
                            var message = "";
                            if (subject == "") message += "Subject could not be blank...\n\r";
                            if (message == "") message += "Message could not be blank...\n\r";

                            if (message != "") {
                                alert(message);
                            }
                            else {
                                __doPostBack('Cevapla', subject + "^#^" + message + "^#^" + message_uid);
                            }
                        }
                    },
                    "Cancel": function () {
                        $(this).dialog("close");

                    }
                }

            });

        }

        function Sil(message_uid) {
            if (window.confirm('Do you want to delete message?')) {
                __doPostBack('Sil', message_uid + "^#^" + tip);
            }
        }



        function MesajGonder() {

            $("#dialogMesajGonder").dialog({
                height: 400,
                width: 680,
                modal: true,
                title: 'Mesaj Gönder',
                buttons: {
                    "Send": function () {
                        if (window.confirm('Do you want to send message?')) {
                            var subject = $("#txtSubject").val(),
			                 message = $("#txtMessage").val();
                            subject = $.trim(subject);
                            message = $.trim(message);
                            var message = "";
                            if (subject == "") message += "Subject could not be blank...\n\r";
                            if (message == "") message += "Message could not be blank...\n\r";

                            if (message != "") {
                                alert(message);
                            }
                            else {
                                __doPostBack('MesajGonder', subject + "^#^" + message);
                            }
                        }
                    },
                    "Cancel": function () {
                        $(this).dialog("close");

                    }
                }

            });
        }

        function UyelereMesajGonder() {

            $("#dialogUyelereMesajGonder").dialog({
                height: 570,
                width: 680,
                modal: true,
                title: 'Send Message',
                buttons: {
                    "Send": function () {
                        if (window.confirm('Do you want to send message?')) {

                            var uye = "";
                            $("#result_uyeler li").each(function (i) {
                                var id = "";
                                if ($(this).attr("id") != "")
                                    id = $(this).attr("id").split('_')[1];
                                else
                                    id = "";
                                if (i == 0) {
                                    uye = id;
                                }
                                else if (uye.indexOf(id) == -1) {
                                    uye = uye + "," + id;
                                }
                            });


                            var subject = $("#txtUyelereKonu").val(),
			                 message = $("#txtUyelereMesaj").val();
                            subject = $.trim(subject);
                            message = $.trim(message);

                            var tum_uyeler = "false";

                            if ($("#chkTumUyelere").attr('checked') == "checked") {
                                tum_uyeler = "true";
                            }

                            var message = "";
                            if (subject == "") message += "Subject could not be blank...\n\r";
                            if (message == "") message += "Message could not be blank...\n\r";

                            if (message != "") {
                                alert(message);
                            }
                            else {
                                __doPostBack('UyelereMesajGonder', subject + "^#^" + message + "^#^" + tum_uyeler + "^#^" + uye);
                            }
                        }
                    },
                    "Cancel": function () {
                        $(this).dialog("close");

                    }
                }

            });
        }

    
    </script>
    <div style="width: 100%; margin: auto;">
        <div id="dialogMesajGonder" title="Mesaj Gönder" style="display: none; font-size: 10pt">
            <table cellpadding="1" cellspacing="2" width="600">
                <tr>
                    <td align="right" class="style1">
                        &nbsp;
                    </td>
                    <td width="200">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:Label CssClass="labelStyle" ID="lblSubject" runat="server" Text="Subject :"></asp:Label>
                    </td>
                    <td width="200">
                        <asp:TextBox ID="txtSubject" runat="server" ClientIDMode="Static" Width="400px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:Label CssClass="labelStyle" ID="lblMessage" runat="server" Text="Message :"></asp:Label>
                    </td>
                    <td width="200">
                        <asp:TextBox ID="txtMessage" runat="server" ClientIDMode="Static" Width="400px" Rows="12"
                            TextMode="MultiLine"></asp:TextBox>
                        <br />
                        (Maximum Karakter: 400)<br />
                        <span id="charLeft"></span>
                    </td>
                </tr>
            </table>
        </div>
        <div id="dialogUyelereMesajGonder" title="Send Message" style="display: none; font-size: 10pt">
            <table cellpadding="1" cellspacing="2" width="600">
                <tr>
                    <td align="right" class="style1">
                        &nbsp;
                    </td>
                    <td width="200">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td valign="top">
                        <label for="uyeler_mesaj">
                            Related Members:
                        </label>
                    </td>
                    <td valign="top">
                        <div class="ui-widget">
                            <input id="uyeler" />
                        </div>
                        <div class="ui-widget" style="margin-top: 0px; font-family: Arial">
                            <div id="result_uyeler" style="height: 100px; width: 400px; overflow: auto;" class="ui-widget-content">
                            </div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td width="150px">
                        <asp:Label ID="Label14" runat="server"></asp:Label>
                    </td>
                    <td width="480px">
                        <asp:CheckBox runat="server" ClientIDMode="Static" ID="chkTumUyelere" Text="Satın Alan Tüm Üyelere Gönder" />
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:Label CssClass="labelStyle" ID="Label1" runat="server" Text="Subject :"></asp:Label>
                    </td>
                    <td width="200">
                        <asp:TextBox ID="txtUyelereKonu" runat="server" ClientIDMode="Static" Width="400px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        <asp:Label CssClass="labelStyle" ID="Label2" runat="server" Text="Message :"></asp:Label>
                    </td>
                    <td width="200">
                        <asp:TextBox ID="txtUyelereMesaj" runat="server" ClientIDMode="Static" Width="400px"
                            Rows="12" TextMode="MultiLine"></asp:TextBox>
                        <br />
                        (Maximum Karakter: 400)<br />
                        <span id="charLeftUye"></span>
                    </td>
                </tr>
            </table>
        </div>
        <div id="smoothmenu1" class="ddsmoothmenu">
            <asp:Literal ID="ltlMenu" runat="server"></asp:Literal>
        </div>
        <div id="mainLeft">
            <div class="boxLeft">
                <div class="boxLeftHeaders">
                    Folders
                </div>
                <div class="boxLeftContent">
                    <ul class="surveyFilterMenu">
                        <li>
                            <asp:LinkButton ID="gelen_kutusu" runat="server" CssClass="tumu" OnClientClick="GelenKutusu()"
                                OnClick="gelen_kutusu_Click">Inbox</asp:LinkButton></li>
                        <li>
                            <asp:LinkButton ID="giden_kutusu" CssClass="aciklar" runat="server" OnClientClick="GidenKutusu()"
                                OnClick="giden_kutusu_Click">Outbox</asp:LinkButton></li>
                        <li>
                            <asp:LinkButton ID="silinenler_gelenler" CssClass="yayindakiler" runat="server" OnClientClick="SilinenGelenKutusu()"
                                OnClick="silinenler_gelen_Click">Deleted Messages(Inbox)</asp:LinkButton></li>
                        <li>
                            <asp:LinkButton ID="silinenler_gidenler" CssClass="kapalilar" runat="server" OnClientClick="SilinenGidenKutusu()"
                                OnClick="silinenler_gidenler_Click">Deleted Messages(Outbox)</asp:LinkButton></li>
                    </ul>
                </div>
            </div>
        </div>
        <div id="mainRight" style="height: 100%">
            <div class="boxRight">
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="gelen_kutusu" />
                        <asp:AsyncPostBackTrigger ControlID="giden_kutusu" />
                        <asp:AsyncPostBackTrigger ControlID="silinenler_gelenler" />
                        <asp:AsyncPostBackTrigger ControlID="silinenler_gidenler" />
                    </Triggers>
                    <ContentTemplate>
                        <div class="boxRightHeaders">
                            <asp:LinkButton ID="LinkButtonHeader" Font-Size="23px" ForeColor="White" Font-Underline="false"
                                runat="server"></asp:LinkButton>
                        </div>
                        <div class="boxRightContent">
                            <asp:Repeater ID="rptMesajlar" runat="server">
                                <HeaderTemplate>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <div class="surveyList">
                                        <div class="surveyListLeft">
                                            <div class="surveyName">
                                                <a class="mediumUnderlineLink" href="#"><%#DataBinder.Eval(Container.DataItem, "mesaj_ad")%>
                                                -
                                                <%#DataBinder.Eval(Container.DataItem, "message_subject")%></a></div>
                                            <div class="surveyType">
                                                <%#DataBinder.Eval(Container.DataItem, "message")%></div>
                                            <div class="surveyListButtons">
                                                
                                                <a class="cevapla" href="#" onclick="Cevapla('<%#DataBinder.Eval(Container.DataItem, "message_uid")%>');">
                                                    Answer</a><div  class="yetkibutton" ><a class="sil linkButtonStyle" href="#" onclick="Sil('<%#DataBinder.Eval(Container.DataItem, "message_uid")%>');">
                                                        Delete</a>
                                                        </div>
                                            </div>
                                        </div>
                                        <div class="surveyListRight">
                                            <div class="surveyDate">
                                                <%#DataBinder.Eval(Container.DataItem, "sent_date")%></div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                    <div class="surveyList lightBg">
                                        <div class="surveyListLeft">
                                            <div class="surveyName">
                                                <a class="mediumUnderlineLink" href="#"><%#DataBinder.Eval(Container.DataItem, "mesaj_ad")%>
                                                -
                                                <%#DataBinder.Eval(Container.DataItem, "message_subject")%></a></div>
                                            <div class="surveyType">
                                                <%#DataBinder.Eval(Container.DataItem, "message")%></div>
                                            <div class="surveyListButtons">
                                            
                                                <a class="cevapla" href="#" onclick="Cevapla('<%#DataBinder.Eval(Container.DataItem, "message_uid")%>');">
                                                    Answer</a><div  class="yetkibutton" ><a class="sil linkButtonStyle" href="#" onclick="Sil('<%#DataBinder.Eval(Container.DataItem, "message_uid")%>');">
                                                        Delete</a>
                                                        </div>
                                            </div>
                                        </div>
                                        <div class="surveyListRight">
                                            <div class="surveyDate">
                                                <%#DataBinder.Eval(Container.DataItem, "sent_date")%></div>
                                        </div>
                                    </div>
                                </AlternatingItemTemplate>
                            </asp:Repeater>
                        </div>
                        <uc1:RepeaterPaging ID="rptMesajlarPaging" runat="server" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
    
</asp:Content>
