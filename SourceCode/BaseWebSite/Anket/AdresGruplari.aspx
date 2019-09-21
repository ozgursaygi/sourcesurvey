<%@ Page Title="Mail Grupları" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="AdresGruplari.aspx.cs" Inherits="BaseWebSite.Survey.AdresGruplari" %>

<%@ Register Src="../BaseControls/RepeaterPaging.ascx" TagName="RepeaterPaging" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>E-Mail Groups</title>
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
                text: false,
                icons: {
                    primary: "ui-icon-anket_menu_search"
                }
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


        function MailGrubuEkle() {

            $("#dialogMailGrubuEkle").dialog({
                height: 300,
                width: 680,
                modal: true,
                title: 'Add E-Mail Group',
                buttons: {
                    "Kaydet": function () {
                        if (window.confirm('Do you want to create an e-mail group?')) {
                            var anket_grubu = $("#ddlanketgrup").val(),
			                 ad = $("#txtad").val();

                            var message = "";
                            if (ad == "") message += "E-mail group name can not be blank...\n\r"

                            if (message != "") {
                                alert(message);
                            }
                            else {
                                __doPostBack('MailGrubuEkle', anket_grubu + '^#^' + ad);
                            }
                        }
                    },
                    "Cancel": function () {
                        $(this).dialog("close");

                    }
                }

            });
        }

        function Export() {
            if ($("#ddlMailGrubu").val() == null || $("#ddlMailGrubu").val() == undefined || $("#ddlMailGrubu").val() == "") {
                alert('Please add an e-mail group.')
                return;
            }
            var id = $("#ddlMailGrubu").val();

            var returnVal = window.showModalDialog("ExceldenAktar.aspx?mail_grubu_uid=" + id, "File", "dialogHeight=450px;dialogWidth=750px;status=no");
            __doPostBack('ExceldenAktar', 'ExceldenAktar');
            return false;
        }

        function MailEkle() {

            if ($("#ddlMailGrubu").val() == null || $("#ddlMailGrubu").val() == undefined || $("#ddlMailGrubu").val() == "") {
                alert('Please add an e-mail group.')
                return;
            }

            $("#dialogMailEkle").dialog({
                height: 300,
                width: 680,
                modal: true,
                title: 'E-Posta Ekle',
                buttons: {
                    "Kaydet": function () {
                        if (window.confirm('Do you want to add an e-mail?')) {
                            var mailad = $("#txtmailad").val(),
			                 mailsoyad = $("#txtmailsoyad").val(),
                             mail = $("#txtmail").val();

                            if (!validateEmail('txtmail')) {
                                alert("Please add valid e-mail address .");
                                return;
                            }

                            var message = "";
                            if (mailad == "") message += "Name can not be blank...\n\r";
                            if (mailsoyad == "") message += "Surname can not be blank...\n\r";
                            if (mail == "") message += "E-mail can not be blank...\n\r";


                            if (message != "") {
                                alert(message);
                            }
                            else {
                                __doPostBack('MailEkle', mailad + '^#^' + mailsoyad + '^#^' + mail);
                            }
                        }
                    },
                    "Vazgeç": function () {
                        $(this).dialog("close");

                    }
                }

            });
        }

        function MailDuzenle(id) {

            $("#dialogMailDuzenle").dialog({
                height: 300,
                width: 680,
                modal: true,
                title: 'E-Posta Düzenle',
                buttons: {
                    "Kaydet": function () {
                        if (window.confirm('Do you want to update e-mail?')) {
                            var mailad = $("#txtmailad_duzenle").val(),
			                 mailsoyad = $("#txtmailsoyad_duzenle").val(),
                             mail = $("#txtmail_duzenle").val();

                            if (!validateEmail('txtmail_duzenle')) {
                                alert("v.");
                                return;
                            }

                            var message = "";
                            if (mailad == "") message +=  "Name can not be blank...\n\r";
                            if (mailsoyad == "") message += "Surname can not be blank...\n\r";
                            if (mail == "") message += "E-mail can not be blank...\n\r;


                            if (message != "") {
                                alert(message);
                            }
                            else {
                                __doPostBack('MailUpdate', id + '^#^' + mailad + '^#^' + mailsoyad + '^#^' + mail);
                            }
                        }
                    },
                    "Vazgeç": function () {
                        $(this).dialog("close");

                    }
                }

            });
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

        function HizliAra() {
            __doPostBack('HizliAramadan', $("#txtSearchBox").val());
        }

        function Duzenle(id) {

            __doPostBack('Duzenle', id);
        }

        function Sil(id) {
            if (window.confirm('Do you want to delete related e-mail?')) {
                __doPostBack('Sil', id);
            }
        }
    </script>
    <div style="width: 100%; margin: auto;">
        <div id="mainTopSection">
            <div id="smoothmenu1" class="ddsmoothmenu">
                <ul class="mainMenu">
                    <li><a href="AnketDashboard.aspx">BACK</a></li>
                    <li><a href="#" onclick="MailGrubuEkle()">ADD NEW E-MAIL GROUP</a></li>
                    <li><a href="#" onclick="MailEkle()">ADD E-MAIL</a></li>
                    <li><a href="#" onclick="Export()">IMPORT TO EXCEL</a></li>
                </ul>
                <div class="searchSurvey">
                    <input id="txtSearchBox" type="text" />
                    <input type="button" id="ara" />
                </div>
            </div>
        </div>
        <div>
            <table>
                <tr>
                    <td>
                        Team :
                        <asp:DropDownList ID="ddlgrup" runat="server" CssClass="textEntry" Width="200px"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlgrup_SelectedIndexChanged">
                        </asp:DropDownList>
                    </td>
                    <td>
                        E-mail Group :
                        <asp:DropDownList ID="ddlMailGrubu" runat="server" CssClass="textEntry" Width="200px"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlMailGrubu_SelectedIndexChanged"
                            ClientIDMode="Static">
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </div>
        <div>
            <table>
                <tr>
                    <td>
                        <img src="../Images/_spacer.gif" height="10px" />
                    </td>
                </tr>
            </table>
        </div>
        <div id="dialogMailGrubuEkle" title="Add E-mail Group" style="display: none; font-size: 10pt">
            <table cellpadding="0" cellspacing="0" width="630px">
                <tr id="div_grup" runat="server">
                    <td width="150px">
                        <asp:Label ID="lblgrup" runat="server" Text="Survey Group :"></asp:Label>
                    </td>
                    <td width="480px">
                        <asp:DropDownList ID="ddlanketgrup" runat="server" Width="200px" ClientIDMode="Static">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <img src="../Images/_spacer.gif" height="10px" alt="" />
                    </td>
                </tr>
                <tr>
                    <td width="150px">
                        <asp:Label ID="lblad" runat="server" Text="E-mail Group Name :"></asp:Label>
                    </td>
                    <td width="480px">
                        <asp:TextBox ID="txtad" runat="server" Width="430px" ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
        <div id="dialogMailEkle" title="E-Posta Ekle" style="display: none; font-size: 10pt">
            <table cellpadding="0" cellspacing="0" width="630px">
                <tr>
                    <td width="150px">
                        <asp:Label ID="Label4" runat="server" Text="E-mail Group :"></asp:Label>
                    </td>
                    <td width="480px">
                        <asp:Label ID="Label5" runat="server" ><%=mail_grubu_text%></asp:Label>
                    </td>
                </tr>
                 <tr>
                    <td colspan="2">
                        <img src="../Images/_spacer.gif" height="10px" alt="" />
                    </td>
                </tr>
                <tr>
                    <td width="150px">
                        <asp:Label ID="lblmailad" runat="server" Text="Name :"></asp:Label>
                    </td>
                    <td width="480px">
                        <asp:TextBox ID="txtmailad" runat="server" Width="430px" ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <img src="../Images/_spacer.gif" height="10px" alt="" />
                    </td>
                </tr>
                <tr>
                    <td width="150px">
                        <asp:Label ID="lblmailsoyad" runat="server" Text="Surname :"></asp:Label>
                    </td>
                    <td width="480px">
                        <asp:TextBox ID="txtmailsoyad" runat="server" Width="430px" ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <img src="../Images/_spacer.gif" height="10px" alt="" />
                    </td>
                </tr>
                <tr>
                    <td width="150px">
                        <asp:Label ID="lblmail" runat="server" Text="E-Mail :"></asp:Label>
                    </td>
                    <td width="480px">
                        <asp:TextBox ID="txtmail" runat="server" Width="430px" ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
        <div id="dialogMailDuzenle" title="E-Posta Düzenle" style="display: none; font-size: 10pt">
            <table cellpadding="0" cellspacing="0" width="630px">
                <tr>
                    <td width="150px">
                        <asp:Label ID="Label1" runat="server" Text="Name :"></asp:Label>
                    </td>
                    <td width="480px">
                        <asp:TextBox ID="txtmailad_duzenle" runat="server" Width="430px" ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <img src="../Images/_spacer.gif" height="10px" alt="" />
                    </td>
                </tr>
                <tr>
                    <td width="150px">
                        <asp:Label ID="Label2" runat="server" Text="Surname :"></asp:Label>
                    </td>
                    <td width="480px">
                        <asp:TextBox ID="txtmailsoyad_duzenle" runat="server" Width="430px" ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <img src="../Images/_spacer.gif" height="10px" alt="" />
                    </td>
                </tr>
                <tr>
                    <td width="150px">
                        <asp:Label ID="Label3" runat="server" Text="E-Mail :"></asp:Label>
                    </td>
                    <td width="480px">
                        <asp:TextBox ID="txtmail_duzenle" runat="server" Width="430px" ClientIDMode="Static"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
        <div id="mainCenter" style="height: 100%">
            <div class="boxCenter">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <div class="boxCenterHeaders">
                            <asp:LinkButton ID="LinkButtonHeader" Font-Size="23px" ForeColor="White" Font-Underline="false"
                                runat="server"></asp:LinkButton>
                        </div>
                        <div class="boxCenterContent">
                            <asp:Repeater ID="rptMailler" runat="server">
                                <HeaderTemplate>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <div class="surveyListCenter">
                                        <div class="surveyListLeft">
                                            <div class="surveyName">
                                                <a class="mediumUnderlineLink" href="#">
                                                    <%#DataBinder.Eval(Container.DataItem, "ad")%>
                                                    -
                                                    <%#DataBinder.Eval(Container.DataItem, "soyad")%></a></div>
                                            <div class="surveyType">
                                                <%#DataBinder.Eval(Container.DataItem, "email")%>
                                                <div class="surveyListButtons">
                                                    <a class="duzenle linkButtonStyle" href="#" onclick="Duzenle('<%#DataBinder.Eval(Container.DataItem, "id")%>');">
                                                        Update</a> <a class="sil linkButtonStyle" href="#" onclick="Sil('<%#DataBinder.Eval(Container.DataItem, "id")%>');">
                                                            Delete</a>
                                                </div>
                                            </div>
                                            <div class="surveyListRight">
                                            </div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                    <div class="surveyListCenter lightBg">
                                        <div class="surveyListLeft">
                                            <div class="surveyName">
                                                <a class="mediumUnderlineLink" href="#">
                                                    <%#DataBinder.Eval(Container.DataItem, "ad")%>
                                                    -
                                                    <%#DataBinder.Eval(Container.DataItem, "soyad")%></a></div>
                                            <div class="surveyType">
                                                <%#DataBinder.Eval(Container.DataItem, "email")%>
                                                <div class="surveyListButtons">
                                                    <a class="duzenle linkButtonStyle" href="#" onclick="Duzenle('<%#DataBinder.Eval(Container.DataItem, "id")%>');">
                                                        Update</a> <a class="sil linkButtonStyle" href="#" onclick="Sil('<%#DataBinder.Eval(Container.DataItem, "id")%>');">
                                                            Delete</a>
                                                </div>
                                            </div>
                                            <div class="surveyListRight">
                                            </div>
                                        </div>
                                    </div>
                                </AlternatingItemTemplate>
                            </asp:Repeater>
                        </div>
                        <uc1:RepeaterPaging ID="rptMaillerPaging" runat="server" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
    <input type="hidden" id="search_text" runat="server" clientidmode="Static" />
</asp:Content>
