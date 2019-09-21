<%@ Page Title="Survey - Yardım" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Help.aspx.cs" Inherits="BaseWebSite.Help" %>
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
         
         });

         $(document).ready(function () {

             
         });


         function HelpGoster(no) {
             if (no == 1) {
                 window.open('HELP/01_Survey_Nedir.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 2) {
                 window.open('HELP/02_Survey_Yapmanin_Onemi.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 3) {
                 window.open('HELP/03_Survey_Yapma_Kurallari.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 4) {
                  window.open('HELP/04_Survey 123_Nasıl_Satin_Alacagim.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             
             else if (no == 5) {
                 window.open('HELP/05_Survey_123_Paketlerinin_Ozellikleri_Nelerdir.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 6) {
                 window.open('HELP/06_Hesabimi_Gormek_Istiyorum.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 7) {
                  window.open('HELP/07_Survey_Uyeligimi_Yenileyebilirmiyim.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 8) {
                 window.open('HELP/08_Survey_123_Kullanim_Kosullari_Nelerdir.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 9) {
                 window.open('HELP/09_Survey_123_Sorumluluklari_Nedir.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 10) {
                  window.open('HELP/10_Survey_Sorularini_Nasil_Hazirlayacagim.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 11) {
                 window.open('HELP/11_Yeni_Survey_Hazirlamak.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 12) {
                 window.open('HELP/12_Survey_Soru_Turleri_Nelerdir_Soruyu_Nasil_Eklerim.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 13) {
                  window.open('HELP/13_Survey_Sorularini_Nasil_Duzeltirim_Veya_Nasil_Silebilirim.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 14) {
                 window.open('HELP/14_Survey_Sorularini_Survey_Yayinlanmadan_Nasil_On_Izleme_Yapabilirim.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 15) {
                 window.open('HELP/15_Survey_Temasini_Degistirebilirmiyim.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 16) {
                  window.open('HELP/16_Hazir_anket_Sorulari_varmi.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 17) {
                 window.open('HELP/17_Test_Surveyi_Ne_Demektir.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 18) {
                 window.open('HELP/18_Surveyi_Cevaplayacaklarin_Gorecegi_Survey_123_Sayfasi_Nasildir.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 19) {
                  window.open('HELP/19_Surveyi_Katilimcilara_Duyurarak_Nasil_Baslatacagim.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 20) {
                 window.open('HELP/20_Surveyi_Sonuclanmadan_Yanitlari_Izleyebilirmiyim.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 21) {
                 window.open('HELP/21_Devam_Eden_Surveyi_Bitmeden_Kapatabilirmiyim.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 22) {
                  window.open('HELP/22_Bir_Surveyi_Tekrar_Yapabilirmiyim.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 23) {
                 window.open('HELP/23_Survey_123_ANA_SAYFA_da_Yer_Alan_Gostergeler.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 24) {
                 window.open('HELP/24_Surveyten_Sorumlu_Personel_Isten_Ayrildi.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 25) {
                  window.open('HELP/25_Survey_Ekibi_Nedir.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
              else if (no == 26) {
                  window.open('HELP/26_Surveye_Davet_Ettigim_Kisileri_Nasil_Gorebilirim.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 27) {
                 window.open('HELP/27_Survey_Raporlarini_Nasil_Gorebilirim.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 28) {
                 window.open('HELP/28_Survey_Sonuclarini_Bilgisayarima_Transfer_Edebilirmiyim.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 29) {
                  window.open('HELP/29_eposta_adreslerimi_dosyadan_anket123_e_transfeedebilirmiyim.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 30) {
                 window.open('HELP/30_Survey_Katılımcilari_eposta_grubu_nedir.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 31) {
                 window.open('HELP/31_Survey_eposta_grubunu_nasil_hazirlayacagim.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 32) {
                  window.open('HELP/32_Mail_gruplarina_ekleme_yapabilirmiyim.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 33) {
                 window.open('HELP/33_eposta_adreslerinin_gizlilik_seviyesi_nedir.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 34) {
                 window.open('HELP/34_Acik_Survey_Nedir_Nasil_Yapilir.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
             else if (no == 35) {
                 window.open('HELP/35_Kapali_Surveyi_Acmak_Istiyorum.htm', 'Help', 'toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=800,height=600');
             }
         }
    </script>
    <div class="buySurveyForm">
        <div class="boxCenterHeaders">
            Yardım</div>
        
         <div class="boxCenterContent">
          <p>
                <a href="#" class="help_link"  onclick="HelpGoster(1)">Survey Nedir?</a>
            </p>
            <p>
                <a href="#" class="help_link" onclick="HelpGoster(2)">Survey Yapmanın Önemi</a>
            </p>

            <p>
                <a href="#" class="help_link"  onclick="HelpGoster(3)">Survey Yapma Kuralları</a>
            </p>
            <p>
                <a href="#" class="help_link" onclick="HelpGoster(4)">Survey Üyeliği Nasıl Satın Alacağım?</a>
            </p>

            <p>
                <a href="#" class="help_link"  onclick="HelpGoster(5)">Survey Paketlerinin Özellikleri Nelerdir ?</a>
            </p>
            <p>
                <a href="#" class="help_link" onclick="HelpGoster(6)">Hesabımı  Görmek İstiyorum?</a>
            </p>

            <p>
                <a href="#" class="help_link"  onclick="HelpGoster(7)">Survey Üyeliğimi Yenileyebilirmiyim?</a>
            </p>
            <p>
                <a href="#" class="help_link" onclick="HelpGoster(8)">Survey Kullanım Koşulları Nelerdir?</a>
            </p>

            <p>
                <a href="#" class="help_link"  onclick="HelpGoster(9)">Survey Sorumlulukları Nedir?</a>
            </p>
            <p>
                <a href="#" class="help_link" onclick="HelpGoster(10)">Survey Sorularını Nasıl Hazırlayacağım?</a>
            </p>

            <p>
                <a href="#" class="help_link"  onclick="HelpGoster(11)">Yeni Survey Hazırlamak</a>
            </p>
            <p>
                <a href="#" class="help_link" onclick="HelpGoster(12)">Survey Soru Türleri Nelerdir Soruyu Nasıl Eklerim?</a>
            </p>

            <p>
                <a href="#" class="help_link"  onclick="HelpGoster(13)">Survey Sorularını Nasıl Düzeltirim Veya Nasıl Silebilirim?</a>
            </p>
            <p>
                <a href="#" class="help_link" onclick="HelpGoster(14)">Survey Sorularını Survey Yayınlanmadan Nasıl Ön İzleme Yapabilirim?</a>
            </p>

            <p>
                <a href="#" class="help_link"  onclick="HelpGoster(15)">Survey Temasını Değiştirebilirmiyim?</a>
            </p>
            <p>
                <a href="#" class="help_link" onclick="HelpGoster(16)">Hazır Survey soruları var mı?</a>
            </p>

            <p>
                <a href="#" class="help_link"  onclick="HelpGoster(17)">Yayın Öncesi Ne Demektir?</a>
            </p>
            <p>
                <a href="#" class="help_link" onclick="HelpGoster(18)">Surveyi Cevaplayacakların Göreceği Survey Sayfası Nasıldır?</a>
            </p>

            <p>
                <a href="#" class="help_link"  onclick="HelpGoster(19)">Surveyi Katılımcılara Duyurarak Nasıl Başlatacağım?</a>
            </p>
            <p>
                <a href="#" class="help_link" onclick="HelpGoster(20)">Surveyi Sonuçlanmadan Yanıtları İzleyebilirmiyim?</a>
            </p>
            <p>
                <a href="#" class="help_link"  onclick="HelpGoster(21)">Devam Eden Surveyi Bitmeden Kapatabilirmiyim?</a>
            </p>
            <p>
                <a href="#" class="help_link" onclick="HelpGoster(22)">Bir Surveyi Tekrar Yapabilirmiyim?</a>
            </p>

            
            <p>
                <a href="#" class="help_link"  onclick="HelpGoster(23)">Survey ANA SAYFA da Yer Alan Göstergeler</a>
            </p>
            <p>
                <a href="#" class="help_link" onclick="HelpGoster(24)">Surveyten Sorumlu Personel İşten Ayrıldı, Haklarını Yeni Bir Kişiye Nasıl Vereceğim?</a>
            </p>

            <p>
                <a href="#" class="help_link"  onclick="HelpGoster(25)">Survey Ekibi  Nedir, Survey Yöneticisi Hakları Nelerdir?</a>
            </p>
            <p>
                <a href="#" class="help_link" onclick="HelpGoster(26)">Surveye Davet Ettiğim Kişileri Nasıl Görebilirim?</a>
            </p>

            <p>
                <a href="#" class="help_link"  onclick="HelpGoster(27)">Survey Raporlarını Nasıl Görebilirim?</a>
            </p>
            <p>
                <a href="#" class="help_link" onclick="HelpGoster(28)">Survey Sonuçlarını Bilgisayarıma Transfer Edebilirmiyim?</a>
            </p>

            <p>
                <a href="#" class="help_link"  onclick="HelpGoster(29)">E-Posta Adreslerimi Dosyadan Survey E Transfer Edebilirmiyim?</a>
            </p>
            <p>
                <a href="#" class="help_link" onclick="HelpGoster(30)">Survey Katılımcıları e-posta grubu nedir?</a>
            </p>
            <p>
                <a href="#" class="help_link"  onclick="HelpGoster(31)">Survey e-posta grubunu nasıl hazırlayacağım?</a>
            </p>
            <p>
                <a href="#" class="help_link" onclick="HelpGoster(32)">Mail gruplarına ekleme yapabilirmiyim, düzeltme yapabilirmiyim?</a>
            </p>
            <p>
                <a href="#" class="help_link" onclick="HelpGoster(33)">E-Posta Adreslerimin Gizlilik Seviyesi Nedir?</a>
            </p>
              <p>
                <a href="#" class="help_link" onclick="HelpGoster(34)">Açık Survey Nedir Nasıl Yapılır?</a>
            </p>
              <p>
                <a href="#" class="help_link" onclick="HelpGoster(35)">Kapalı Surveyi Açmak İstiyorum?</a>
            </p>
         </div>
         </div>
         
</asp:Content>
