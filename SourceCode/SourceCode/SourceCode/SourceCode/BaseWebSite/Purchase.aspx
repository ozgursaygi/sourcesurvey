<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Purchase.aspx.cs" Inherits="BaseWebSite.Purchase" %>

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
            $(function(){
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
                if ($.trim($('#Name').val()) == "" || $.trim($('#Surname').val()) == "" || $.trim($('#Ekip').val()) == "" || $.trim($('#Email').val()) == "" || $.trim($('#Adres').val()) == "" || $.trim($('#Telefon').val()) == "" || $.trim($('#CepTelefonu').val()) == "" || $.trim($('#dogum_tarihi').val()) == "" || $('#has_error').val() == "1") {
                    $('#div_error_message').show();
                }
                else {
                    $('#div_error_message').hide();
                }

            }
    </script>
    <div class="buySurveyForm">
        <div class="boxCenterHeaders">
            Satın Alma İşlemleri</div>
        
         <div class="boxCenterContent">
        <div class="ui-widget" id="div_error_message" runat="server" clientidmode="Static">
            <div class="ui-state-error ui-corner-all" style="padding: 15px;">
                <p>
                    <asp:Literal ID="ErrorMessage" runat="server"></asp:Literal>
                    <asp:ValidationSummary ID="RegisterUserValidationSummary" runat="server" CssClass="failureNotification"
                        ValidationGroup="RegisterUserValidationGroup" />
            </div>
        </div> 
        <p class="warning"><span class="bold">Uyarı :</span> Üyeliği devam eden kullanıcılar Yeni bir paket alıp üyeliklerini yenilediklerinde eski satın aldıkları paketlerinde kalan kullanmadıkları anketler Yeni paketlerine ilave edilecektir.</p>
            <p class="packageProperties"><span class="bold">Var Olan Paket Özellikleri :</span> <br/><asp:Label ID="VarOlanPaket" runat="server"></asp:Label></p>
               <p class="packageProperties"><span class="bold">Seçilen Pakete göre Yeni Hesap Durumunuz :</span><br/> <asp:Label ID="UyelikDurumu" runat="server" ForeColor="Red" Font-Bold="true" ></asp:Label></p>
            <p>
                <input type="checkbox" id="chkSirket" runat="server" class="check" onclick="CheckSirket();"
                    clientidmode="Static" />Bir şirket adına siparişte bulunuyorsanız, lütfen buraya
                tıklayınız.
            </p>
            <p>
                <asp:Label ID="NameLabel" runat="server" AssociatedControlID="Name">Ad:</asp:Label>
                <asp:TextBox ID="Name" runat="server" CssClass="textEntry" Enabled="false" ClientIDMode="Static"></asp:TextBox>
                <asp:RequiredFieldValidator ID="NameRequired" runat="server" ControlToValidate="Name"
                    CssClass="failureNotification" ErrorMessage="Lütfen Ad bilgisini doldurunuz."
                    ToolTip="Lütfen Ad bilgisini doldurunuz." ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
            </p>
            <p>
                <asp:Label ID="SurnameLabel" runat="server" AssociatedControlID="Surname">Soyad:</asp:Label>
                <asp:TextBox ID="Surname" runat="server" CssClass="textEntry" Enabled="false" ClientIDMode="Static"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="Surname"
                    CssClass="failureNotification" ErrorMessage="Lütfen Soyad bilgisini doldurunuz."
                    ToolTip="Lütfen Soyad bilgisini doldurunuz." ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
            </p>
            <p>
                <asp:Label ID="EkipLabel" runat="server" AssociatedControlID="Ekip">Ekip İsmi:</asp:Label>
                <asp:TextBox ID="Ekip" runat="server" CssClass="textEntry" Enabled="false" ClientIDMode="Static"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="Ekip"
                    CssClass="failureNotification" ErrorMessage="Lütfen Ekip İsmi bilgisini doldurunuz."
                    ToolTip="Lütfen Ekip İsmi bilgisini doldurunuz." ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
            </p>
            <div id="div_sirket" >
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
                <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email">E-posta:</asp:Label>
                <asp:TextBox ID="Email" runat="server" CssClass="textEntry" Enabled="false" ClientIDMode="Static"></asp:TextBox>
                <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email"
                    CssClass="failureNotification" ErrorMessage="Lütfen E-posta bilgisini doldurunuz."
                    ToolTip=" Lütfen E-posta bilgisini doldurunuz." ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="regexEmailValid" runat="server" CssClass="failureNotification"
                    ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ControlToValidate="Email"
                    ErrorMessage="Lütfen geçerli bir e-posta adresi giriniz."  ValidationGroup="RegisterUserValidationGroup"></asp:RegularExpressionValidator>
            </p>
            <p>
                <asp:Label ID="Label1" runat="server" AssociatedControlID="Telefon">Telefon:</asp:Label>
                <asp:TextBox ID="Telefon" runat="server" CssClass="phone"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="Telefon"
                    CssClass="failureNotification" ErrorMessage="Lütfen Telefon bilgisini doldurunuz."
                    ToolTip=" Lütfen Telefon bilgisini doldurunuz." ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                
            </p>
            <p>
                <asp:Label ID="Label2" runat="server" AssociatedControlID="CepTelefonu">Cep Telefonu:</asp:Label>
                <asp:TextBox ID="CepTelefonu" runat="server" CssClass="phone"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="CepTelefonu"
                    CssClass="failureNotification" ErrorMessage="Lütfen Cep Telefonu bilgisini doldurunuz."
                    ToolTip=" Lütfen Cep Telefonu bilgisini doldurunuz." ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
                
            </p>
            <%--<p>
                <asp:Label ID="Label4" runat="server" AssociatedControlID="ddlcinsiyet">Cinsiyet:</asp:Label>
                <asp:DropDownList ID="ddlcinsiyet" runat="server" CssClass="textEntry" Width="320px">
                </asp:DropDownList>
            </p>--%>
            <%--<p>
                <asp:Label ID="Label3" runat="server" AssociatedControlID="dogum_tarihi">Doğum Tarihi:</asp:Label>
                <input type="text" id="dogum_tarihi" runat="server" clientidmode="Static" 
                    readonly="readonly" />
                     <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="dogum_tarihi"
                    CssClass="failureNotification" ErrorMessage="Lütfen Doğum Tarihi bilgisini doldurunuz."
                    ToolTip="Lütfen Doğum Tarihi bilgisini doldurunuz." ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
            </p>--%>
            <p>
                <asp:Label ID="AdresLabel" runat="server" AssociatedControlID="Adres">Adres:</asp:Label>
                <asp:TextBox ID="Adres" runat="server" CssClass="textEntry" TextMode="MultiLine"
                    Rows="4" Columns="40"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="Adres"
                    CssClass="failureNotification" ErrorMessage="Lütfen Adres bilgisini doldurunuz."
                    ToolTip="Lütfen Adres bilgisini doldurunuz." ValidationGroup="RegisterUserValidationGroup">*</asp:RequiredFieldValidator>
            </p>
            <p>
                <asp:Label ID="Label5" runat="server" AssociatedControlID="UyelikBaslangic">Üyelik Başlangıcı:</asp:Label>
                <asp:TextBox ID="UyelikBaslangic" runat="server" CssClass="textEntry" Enabled="false"></asp:TextBox>
            </p>
            <p>
                <asp:Label ID="Label6" runat="server" AssociatedControlID="UyelikBitis">Üyelik Bitiş:</asp:Label>
                <asp:TextBox ID="UyelikBitis" runat="server" CssClass="textEntry" Enabled="false"></asp:TextBox>
            </p>
             
            <p>
                <asp:Label ID="Label7" runat="server" AssociatedControlID="ddlPaket">Üyelik Paket Seçimi:</asp:Label>
                <asp:DropDownList ID="ddlPaket" runat="server" CssClass="textEntry" 
                    Width="520px" onselectedindexchanged="ddlPaket_SelectedIndexChanged" AutoPostBack="true">
                </asp:DropDownList>
            </p>
             
              
            
           
        <p class="submitButton">
            <asp:Button ID="PurchaseButton" runat="server" Text="Devam Et" ValidationGroup="RegisterUserValidationGroup"
                OnClick="PurchaseButton_Click" Style="height: 26px" OnClientClick="Kontrol();" />
        </p>
    </div>
    </div>
        <input type="hidden" id="has_error" runat="server" clientidmode="Static" />
</asp:Content>
