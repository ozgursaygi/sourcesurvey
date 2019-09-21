<%@ Page Title="Hesap Bilgileri" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Account.aspx.cs" Inherits="BaseWebSite.Account" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" type="text/css" href="css/custom-theme/jquery-ui-1.8.20.custom.css" />
    <link rel="stylesheet" href="css/uniform.default.css" type="text/css" media="screen" />
    <link rel="stylesheet" type="text/css" href="css/ddsmoothmenu.css" />
    <script src="js/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="js/jquery-ui-1.8.20.custom.min.js" type="text/javascript"></script>
    <script src="Scripts/jquery.numeric.js" type="text/javascript"></script>
    <script src="Scripts/jquery.maskedinput-1.2.2.js" type="text/javascript"></script>
    <script src="js/ddsmoothmenu.js" type="text/javascript"></script>
    <script src="js/jquery.uniform.js" type="text/javascript" charset="utf-8"></script>
    <script src="js/jquery.ae.image.resize.min.js" type="text/javascript"></script>
    <script src="Scripts/JQ/jquery.ui.datepicker-tr.js" type="text/javascript"></script>
    <%--<script src="js/cufon-yui.js" type="text/javascript"></script>--%>
    <script src="js/Diavlo.font.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">

        $(function () {
            $("input, textarea, select, button").uniform();
        });

        $(document).ready(function () {
            $('#div_error_message').hide();
            if ($('#has_error').val() == "1") {
                $('#div_error_message').show();
            }

            CheckSirket();
            $("#dogum_tarihi").datepicker({ dateFormat: 'dd-mm-yy', onSelect: function (dateText, inst) {

            }
            });

            $(".phone").mask("(999)999-9999");
        });

        function CheckSirket() {
            if ($("#chkSirket").attr('checked') == "checked") {
                $("#div_sirket").show();
            }
            else {
                $("#div_sirket").hide();
                $("#GrupName").val("");
            }
        }

        function Kontrol() {

            if ($.trim($('#Name').val()) == "" || $.trim($('#Surname').val()) == "" || $.trim($('#Ekip').val()) == "" || $.trim($('#Email').val()) == "" || $.trim($('#Adres').val()) == "" || $.trim($('#dogum_tarihi').val()) == "" || $('#has_error').val() == "1") {
                $('#div_error_message').show();
            }
            else {
                $('#div_error_message').hide();
            }

        }
    </script>
    <div class="buySurveyForm">
        <div class="boxCenterHeaders">
            Acount Info</div>
        <div class="boxCenterContent">
            <div class="ui-widget" id="div_error_message" runat="server" clientidmode="Static">
                <div class="ui-state-error ui-corner-all" style="padding: 15px;">
                    <p>
                        <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                        <asp:ValidationSummary ID="RegisterUserValidationSummary" runat="server" CssClass="failureNotification"
                            ValidationGroup="RegisterUserValidationGroup" />
                    </p>
                </div>
            </div>
            <div id="div_ucretsiz_sirket" runat="server" visible="false" >
                <p>
                    <input type="checkbox" id="chkSirket" runat="server" class="check" onclick="CheckSirket();"
                        clientidmode="Static" />Bir şirket adına siparişte bulundunuz
                </p>
            </div>
            <p>
                <asp:Label ID="NameLabel" runat="server" AssociatedControlID="Name">Name:</asp:Label>
                <asp:TextBox ID="Name" runat="server" CssClass="textEntry" Enabled="false"></asp:TextBox>
                <asp:RequiredFieldValidator ID="NameRequired" runat="server" ControlToValidate="Name"
                    CssClass="failureNotification" ErrorMessage="Please Fill Name Info."
                    ToolTip="Please Fill Name Info." ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
            </p>
            <p>
                <asp:Label ID="SurnameLabel" runat="server" AssociatedControlID="Surname">Surname:</asp:Label>
                <asp:TextBox ID="Surname" runat="server" CssClass="textEntry" Enabled="false"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="Surname"
                    CssClass="failureNotification" ErrorMessage="Please Surname Info."
                    ToolTip="Please Fill Surname Info." ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
            </p>
            <p>
                <asp:Label ID="EkipLabel" Visible="false" runat="server" AssociatedControlID="Ekip">Team Name:</asp:Label>
                <asp:TextBox ID="Ekip" runat="server" CssClass="textEntry" Text="Survey Team" Visible="false" ></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="Ekip" Visible="false"
                    CssClass="failureNotification" ErrorMessage="Lütfen Ekip İsmi bilgisini doldurunuz."
                    ToolTip="Lütfen Ekip İsmi bilgisini doldurunuz." ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
            </p>
            <div id="div_sirket" style="display:none;">
                <p>
                    <asp:Label ID="SirketLabel" runat="server" AssociatedControlID="SirketName">Şirket Adı:</asp:Label>
                    <asp:TextBox ID="SirketName" runat="server" CssClass="textEntry" ClientIDMode="Static"></asp:TextBox>
                </p>
                <p>
                    <asp:Label ID="VergiDairesiLabel" runat="server" AssociatedControlID="VergiDairesi">Vergi Dairesi:</asp:Label>
                    <asp:TextBox ID="VergiDairesi" runat="server" CssClass="textEntry" ClientIDMode="Static"></asp:TextBox>
                </p>
                <p>
                    <asp:Label ID="VergiNoLabel" runat="server" AssociatedControlID="VergiNo">Vergi No:</asp:Label>
                    <asp:TextBox ID="VergiNo" runat="server" CssClass="textEntry" ClientIDMode="Static"></asp:TextBox>
                </p>
            </div>
            <p>
                <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email">E-mail:</asp:Label>
                <asp:TextBox ID="Email" runat="server" CssClass="textEntry" Enabled="false"></asp:TextBox>
                <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email"
                    CssClass="failureNotification" ErrorMessage="Please Fill Email Info."
                    ToolTip="Please Fill Email Info." ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="regexEmailValid" runat="server" CssClass="failureNotification"
                    ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ControlToValidate="Email"
                    ErrorMessage="Please Fill Valid Email Info." ValidationGroup="RegisterUserValidationGroup"></asp:RegularExpressionValidator>
            </p>
            <div id="div_ucretsiz" runat="server">
                <p>
                    <asp:Label ID="Label1" runat="server" AssociatedControlID="Telefon">Phone:</asp:Label>
                    <asp:TextBox ID="Telefon" runat="server" CssClass="phone"></asp:TextBox>
                     <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="Telefon"
                    CssClass="failureNotification" ErrorMessage="Please Fill Phone Info."
                    ToolTip="Please Fill Phone Info." ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                </p>
                <p>
                    <asp:Label ID="Label2" runat="server" AssociatedControlID="CepTelefonu">Mobile Phone:</asp:Label>
                    <asp:TextBox ID="CepTelefonu" runat="server" CssClass="phone"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="CepTelefonu"
                    CssClass="failureNotification" ErrorMessage="Please Fill Mobile Phone Info."
                    ToolTip="Please Fill Mobile Phone Info." ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                </p>
               <%-- <p>
                    <asp:Label ID="Label4" runat="server" AssociatedControlID="ddlcinsiyet">Cinsiyet:</asp:Label>
                    <asp:DropDownList ID="ddlcinsiyet" runat="server" CssClass="textEntry" Width="320px">
                    </asp:DropDownList>
                </p>
                <p>
                    <asp:Label ID="Label3" runat="server" AssociatedControlID="dogum_tarihi">Doğum Tarihi:</asp:Label>
                    <input type="text" id="dogum_tarihi" runat="server" clientidmode="Static" readonly="readonly" />
                </p>--%>
                <p>
                    <asp:Label ID="AdresLabel" runat="server" AssociatedControlID="Adres">Address:</asp:Label>
                    <asp:TextBox ID="Adres" runat="server" CssClass="textEntry" TextMode="MultiLine"
                        Rows="4" Columns="40"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="Adres"
                        CssClass="failureNotification" ErrorMessage="Please Fill Address Info."
                        ToolTip="Please Fill Address Info." ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                </p>
                   <%-- <p class="packageProperties">
                        <span class="bold">Var Olan Paket Özellikleri :</span>
                        <br />
                        <asp:Label ID="VarOlanPaket" runat="server" ForeColor="Red" Font-Bold="true"></asp:Label></p>--%>
            </div>
            <p>
               <span style="font-size:16px" >
                <asp:HyperLink ID="ChangePasswordLnk" runat="server" EnableViewState="false" CssClass="link">Change Password.</asp:HyperLink></span>
            </p>
            <p><br /></p>
           
            <p>
                <asp:Button ID="UpdateButton" runat="server" Text="Update" ValidationGroup="RegisterUserValidationGroup"
                    OnClick="UpdateButton_Click" Style="height: 26px" OnClientClick="Kontrol();" />
            </p>
            </div>
            </div>
            
            <div id="div_ucretli_uye" runat="server" visible="false">
                <div id="mainCenter" style="height: 100%">
                    <div class="boxCenter">
                        <div class="boxCenterHeaders">
                            Satın Alınan Üyelik Paketleri
                        </div>
                        <asp:Repeater ID="rptPaketler" runat="server">
                            <HeaderTemplate>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div class="surveyListCenter">
                                    <div class="surveyListLeft">
                                        <div class="surveyName">
                                            <%#DataBinder.Eval(Container.DataItem, "paket_alim_tarihi_str")%>
                                            -
                                            <%#DataBinder.Eval(Container.DataItem, "odeme_tipi")%>
                                        </div>
                                        <div class="surveyType">
                                            <%#DataBinder.Eval(Container.DataItem, "paket_fiyati_str")%>
                                            -
                                            <%#DataBinder.Eval(Container.DataItem, "anket_sayisi_str")%>
                                            -
                                            <%#DataBinder.Eval(Container.DataItem, "anket_basina_katilimci_sayisi_str")%>
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
                                            <%#DataBinder.Eval(Container.DataItem, "paket_alim_tarihi_str")%>
                                            -
                                            <%#DataBinder.Eval(Container.DataItem, "odeme_tipi")%>
                                        </div>
                                        <div class="surveyType">
                                            <%#DataBinder.Eval(Container.DataItem, "paket_fiyati_str")%>
                                            -
                                            <%#DataBinder.Eval(Container.DataItem, "anket_sayisi_str")%>
                                            -
                                            <%#DataBinder.Eval(Container.DataItem, "anket_basina_katilimci_sayisi_str")%>
                                        </div>
                                        <div class="surveyListRight">
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

    <input type="hidden" id="has_error" runat="server" clientidmode="Static" />
</asp:Content>
