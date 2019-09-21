<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="PurchaseApprove.aspx.cs" Inherits="BaseWebSite.PurchaseApprove" MaintainScrollPositionOnPostback="false" %>

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
<style type="text/css"> 
 
#loading { 
    width: 280px; 
    height: 100px; 
    position: absolute; 
    left: 50%; 
    top: 50%; 
    margin-top: -50px; 
    margin-left: -100px; 
    text-align: center; 
    background-color:#ffffff;
    color:Black;
    font-size:large;
    -webkit-border-radius: 10px;
        -moz-border-radius: 10px;
        border-radius: 10px;
        border: 2px solid #6C9733;
} 
 
</style> 
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
        });

        function CheckSirket() {
            if ($("#chkSirket").attr('checked') == "checked") {
                $("#div_sirket").show();
            }
            else {
                $("#div_sirket").hide();
            }
        }

        function GosterHtml(tip) {
            if (tip == 1) {
                window.open('Survey/ShowHtmlTemplate.aspx?tip=1', 'Üyelik', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=900,height=500');
            }
            else if (tip == 2) {
                window.open('Survey/ShowHtmlTemplate.aspx?tip=2', 'Üyelik', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=900,height=500');
            }
        }

        function sadeceSayi(evt) {
            evt = (evt) ? evt : window.event
            var charCode = (evt.which) ? evt.which : evt.keyCode
            if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                return false;
            }
            return true;
        }

        function PurchaseClick() {
            $('.submitButton').hide();
            $("#ajaxLoader").show();
            HideBackground();
            $("html, body").animate({ scrollTop: 0 }, "slow");
            $('#div_error_message').hide();
            if ($('#has_error').val() == "1") {
                $('#div_error_message').show();
            }
            return true;
        }

        function HideBackground() {
            document.getElementById("loading").style.display = "block";

            $('.buySurveyForm').css({ "opacity": "0.3" });


        }
    </script>
    <div id="loading" style="display:none"><br><br><span style="color:Black">İşleminiz Yapılıyor...Lütfen Bekleyiniz...</span></div>
    <div class="buySurveyForm">
        <div class="boxCenterHeaders">
            Satın Alma Onay İşlemleri</div>
        <div class="boxCenterContent">
            <div class="ui-widget" id="div_error_message" runat="server" clientidmode="Static">
                <div class="ui-state-error ui-corner-all" style="padding: 15px;">
                    <p>
                        <asp:Literal ID="ErrorMessages" runat="server"></asp:Literal>
                        <asp:ValidationSummary ID="RegisterUserValidationSummary" runat="server" CssClass="failureNotification"
                            ValidationGroup="RegisterUserValidationGroup" />
                </div>
            </div>
            <p>
                <input type="checkbox" id="chkSirket" runat="server" class="check" onclick="CheckSirket();"
                    clientidmode="Static" />Bir şirket adına siparişte bulunuyorsunuz
            </p>
            <p class="warning">
                <span class="bold">Uyarı :</span> Üyeliği devam eden kullanıcılar Yeni bir paket
                alıp üyeliklerini yenilediklerinde eski satın aldıkları paketlerinde kalan kullanmadıkları
                anketler Yeni paketlerine ilave edilecektir.</p>
            <p class="packageProperties">
                <span class="bold">Seçilen Pakete Göre Yeni Hesap Durumunuz:</span>
                <br />
                <asp:Label ID="UyelikDurumu" runat="server" ForeColor="Red" Font-Bold="true"></asp:Label></p>
            <p>
                <asp:Label ID="Label9" runat="server" AssociatedControlID="Name">Ad:</asp:Label>
                <asp:TextBox ID="Name" runat="server" CssClass="textEntry" Enabled="false" ReadOnly="true" ></asp:TextBox>
            </p>
            <p>
                <asp:Label ID="SurnameLabel" runat="server" AssociatedControlID="Surname">Soyad:</asp:Label>
                <asp:TextBox ID="Surname" runat="server" CssClass="textEntry" Enabled="false" ReadOnly="true"></asp:TextBox>
            </p>
            <p>
                <asp:Label ID="Label1" runat="server" AssociatedControlID="Telefon">Telefon:</asp:Label>
                <asp:TextBox ID="Telefon" runat="server" CssClass="textEntry" Enabled="false"></asp:TextBox>
            </p>
            <p>
                <asp:Label ID="Label2" runat="server" AssociatedControlID="CepTelefonu">Cep Telefonu:</asp:Label>
                <asp:TextBox ID="CepTelefonu" runat="server" CssClass="textEntry" Enabled="false"></asp:TextBox>
            </p>
            <div id="div_sirket">
                <p>
                    <asp:Label ID="SirketLabel" runat="server" AssociatedControlID="SirketName">Şirket Adı:</asp:Label>
                    <asp:TextBox ID="SirketName" runat="server" CssClass="textEntry" ClientIDMode="Static"
                        ReadOnly="true" Enabled="false"></asp:TextBox>
                </p>
                <p>
                    <asp:Label ID="VergiDairesiLabel" runat="server" AssociatedControlID="VergiDairesi">Vergi Dairesi:</asp:Label>
                    <asp:TextBox ID="VergiDairesi" runat="server" CssClass="textEntry" ClientIDMode="Static"
                        ReadOnly="true" Enabled="false"></asp:TextBox>
                </p>
                <p>
                    <asp:Label ID="VergiNoLabel" runat="server" AssociatedControlID="VergiNo">Vergi No:</asp:Label>
                    <asp:TextBox ID="VergiNo" runat="server" CssClass="textEntry" ClientIDMode="Static"
                        ReadOnly="true" Enabled="false"></asp:TextBox>
                </p>
            </div>
            <p>
                <asp:Label ID="AdresLabel" runat="server" AssociatedControlID="Adres">Fatura Adresi:</asp:Label>
                <asp:TextBox ID="Adres" runat="server" CssClass="textEntry" TextMode="MultiLine"
                    Rows="4" Columns="40" ></asp:TextBox>
                    
            </p>
             <p class="warning">
                Faturanız bu Adrese gönderilecektir.Lütfen Geçerli bir Fatura Adresi olduğunu kontrol ediniz.
                </p>
            <p>
                <asp:Label ID="UcretLabel" runat="server" AssociatedControlID="Ucret">Ücret (TL) :</asp:Label>
                <asp:TextBox ID="Ucret" runat="server" CssClass="textEntry" Enabled="false" ReadOnly="true"></asp:TextBox>
            </p>
            <p  >
            <asp:Label ID="Label7" runat="server" AssociatedControlID="rdHavale">Ödeme Tipi :</asp:Label>
            
               <asp:RadioButton ID="rdHavale" runat="server" CssClass="textEntry" Text="Banka Havalesi"
                    Width="400px" GroupName="OdemeSekli" Checked="true" AutoPostBack="true" />
                    <br />
                    </p>
                    <p  style="display:none">
                    <asp:Label ID="Label8" runat="server" AssociatedControlID="rdKrediKarti"></asp:Label>
               <asp:RadioButton ID="rdKrediKarti" runat="server" CssClass="textEntry" Text="Kredi Kartı"
                    Width="400px" GroupName="OdemeSekli" AutoPostBack="true"  />
                
               
            </p>
            <div id="div_havale" runat="server" class="packageProperties" >


            <p>
                <asp:Label ID="Label19" runat="server" AssociatedControlID="Label11"></asp:Label>
                <asp:Label ID="Label20" runat="server" ><b>DCM GRUP MİMARLIK İNŞAAT DIŞ TİCARET LTD.</b></asp:Label>
            </p>
            
             <p>
                <asp:Label ID="Label10" runat="server" AssociatedControlID="Label11"><b>Banka :</b></asp:Label>
                <asp:Label ID="Label11" runat="server" >YAPI KREDİ BANKASI</asp:Label>
            </p>
            <p>
                <asp:Label ID="Label14" runat="server" AssociatedControlID="Label15"><b>Şube :</b></asp:Label>
                <asp:Label ID="Label15" runat="server" >014 - BEŞİKTAŞ ŞUBESİ</asp:Label>
            </p>
            <p>
                <asp:Label ID="Label17" runat="server" AssociatedControlID="Label13"><b>IBAN :</b></asp:Label>
                <asp:Label ID="Label18" runat="server" >TR84 0006 7010 0000 0091 7907 53 (TL)</asp:Label>
            </p>
            <p>
                <asp:Label ID="Label12" runat="server" AssociatedControlID="Label13"><b>Hesap Nosu :</b></asp:Label>
                <asp:Label ID="Label13" runat="server" >91790753</asp:Label>
            </p>
            <p>
                <asp:Label ID="Label16" runat="server" AssociatedControlID="Label13"></asp:Label>
                <asp:Label ID="Label21" runat="server" > Not : Lütfen Ödeme Yaparken Açıklama Kısmına Yukarıda Yer Alan <b>Adınızı Soyadınızı ve "Survey Ödemesi"</b> yazınız.</asp:Label>
            </p>
            </div>
            <div id="div_kk" runat="server" class="packageProperties" style="display:none">
             <p  >
                <asp:Label ID="Label3" runat="server" AssociatedControlID="txt_kredi_karti">Kredi Kartı No :</asp:Label>
                <asp:TextBox ID="txt_kredi_karti" runat="server" CssClass="textEntry"  onKeyPress="return sadeceSayi(event)" ></asp:TextBox>
            </p>
               <p  >
                <asp:Label ID="Label4" runat="server" AssociatedControlID="txt_cvv">CVV :</asp:Label>
                <asp:TextBox ID="txt_cvv" runat="server" CssClass="textEntry"  onKeyPress="return sadeceSayi(event)" ></asp:TextBox>
            </p> 

              <p  >
                <asp:Label ID="Label5" runat="server" AssociatedControlID="ddl_son_kul_ay">Son Kul. Ay :</asp:Label>
                <asp:DropDownList ID="ddl_son_kul_ay" runat="server" 
                            TabIndex="7">
                            <asp:ListItem Value="01" Text="01" />
                            <asp:ListItem Value="02" Text="02" />
                            <asp:ListItem Value="03" Text="03" />
                            <asp:ListItem Value="04" Text="04" />
                            <asp:ListItem Value="05" Text="05" />
                            <asp:ListItem Value="06" Text="06" />
                            <asp:ListItem Value="07" Text="07" />
                            <asp:ListItem Value="08" Text="08" />
                            <asp:ListItem Value="09" Text="09" />
                            <asp:ListItem Value="10" Text="10" />
                            <asp:ListItem Value="11" Text="11" />
                            <asp:ListItem Value="12" Text="12" />
                        </asp:DropDownList>
            </p> 
            <p  >
                <asp:Label ID="Label6" runat="server" AssociatedControlID="ddl_son_kul_yil">Son Kul. Yıl :</asp:Label>
                <asp:DropDownList ID="ddl_son_kul_yil" runat="server" 
                            TabIndex="7">
                             <asp:ListItem Value="2013" Text="2013" />
                            <asp:ListItem Value="2014" Text="2014" />
                            <asp:ListItem Value="2015" Text="2015" />
                            <asp:ListItem Value="2016" Text="2016" />
                            <asp:ListItem Value="2017" Text="2017" />
                            <asp:ListItem Value="2018" Text="2018" />
                            <asp:ListItem Value="2019" Text="2019" />
                            <asp:ListItem Value="2020" Text="2020" />
                            <asp:ListItem Value="2021" Text="2021" />
                            <asp:ListItem Value="2022" Text="2022" />
                            <asp:ListItem Value="2023" Text="2023" />
                        </asp:DropDownList>
            </p> 
            <p  ><asp:RadioButton ID="rd_visa" runat="server" CssClass="textEntry" Text="Visa"
                    Width="200px" GroupName="KKTip" Checked="true"  />
                    <asp:RadioButton ID="rd_master" runat="server" CssClass="textEntry" Text="Master Card"
                    Width="200px" GroupName="KKTip" />
                    </p>
                    </div>
            <div  ><p ><asp:CheckBox runat="server" ID="chkKabul" /><span class="okudum">Okudum / Kabul Ediyorum</span> <a href="#" class="okudum_link" onclick="GosterHtml(1);return false;">Üyelik Koşulları</a> , <a href="#" class="okudum_link" onclick="GosterHtml(2);return false;">Survey Sorumlulukları</a></p></div>
            <p >
                <asp:Label ID="UyariOdemeTipi" runat="server" ForeColor="Red" Font-Bold="true" ></asp:Label>
            </p>
            <p  >
                <asp:Label ID="UyarıOnay" runat="server" ForeColor="Red" Font-Bold="true">Satın Alma İşlemini Kabul Ediyorsanız Lütfen Onaylayınız.</asp:Label>
            </p>
            <p class="submitButton"  >
                <asp:Button ID="PurchaseButton" runat="server" Text="Onayla/Öde" ValidationGroup="RegisterUserValidationGroup"
                    OnClick="PurchaseButton_Click" Style="height: 26px" OnClientClick="return PurchaseClick();" />
                     
            </p>
            <div id="ajaxLoader" style="display: none; float: right; clear: both; height: 26px;
                    margin-right: 190px;">
                    <img src="Images/ajax-loader.gif" alt="" height="26" />
                </div>
        </div>
    </div>
    <input type="hidden" id="has_error" runat="server" clientidmode="Static" />
</asp:Content>
