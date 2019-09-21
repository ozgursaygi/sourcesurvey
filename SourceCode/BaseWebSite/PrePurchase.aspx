<%@ Page Title="Survey - Satın Al" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PrePurchase.aspx.cs" Inherits="BaseWebSite.PrePurchase" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
 <link rel="stylesheet" type="text/css" href="../css/custom-theme/jquery-ui-1.8.20.custom.css" />
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
     <script type="text/javascript">

         var _gaq = _gaq || [];
         _gaq.push(['_setAccount', 'UA-44470048-1']);
         _gaq.push(['_trackPageview']);

         (function () {
             var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
             ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
             var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
         })();


    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
//        $(function () {
//            Cufon.replace('.slideContent h1');
//            Cufon.replace('.slideContent h2');
//            Cufon.replace('.slideContent h3');
//            Cufon.replace('.homePageBox h2');
//        }
//        );
		</script>
        
         <asp:Literal ID="ltl_uyari" runat="server"></asp:Literal>
        
                <div class="priceList">
          <div class="priceListHeader">
            <table border="0" cellpadding="0" cellspacing="0" width="960">
              <tbody>
                <tr>
                  <td bgcolor="#203b4a" class="first-row" width="244"></td>
                  <td align="center" style="border-right:1px solid #2c4b5d" width="178"><div class="all-container pack-names">
                      <div class="pack-name-title"> Paket 1</div>
                      <div class="pack-price"> 29 TL</div>
                      <div class="sign-up-button-normal"><asp:LinkButton ID="lnk_odeme_tipi_1"  class="sign-up-button-normal-a" runat="server" onclick="lnk_odeme_tipi_1_Click">Satın Al</asp:LinkButton>
                      </div>
                    </div></td>
                  <td align="center" style="border-right:1px solid #2c4b5d" width="178"><div class="all-container pack-names">
                      <div class="pack-name-title"> Paket 2</div>
                      <div class="pack-price"> 49 TL</div>
                      <div class="sign-up-button-normal"><asp:LinkButton ID="lnk_odeme_tipi_2"  
                              class="sign-up-button-normal-a" runat="server" 
                              onclick="lnk_odeme_tipi_2_Click" >Satın Al</asp:LinkButton></div>
                    </div></td>
                  <td align="center" style="border-right:1px solid #2c4b5d" width="178"><div class="all-container pack-names">
                      <div class="pack-name-title"> Paket 3</div>
                      <div class="pack-price"> 109 TL</div>
                      <div class="sign-up-button-normal"><asp:LinkButton ID="lnk_odeme_tipi_3"  
                              class="sign-up-button-normal-a" runat="server" 
                              onclick="lnk_odeme_tipi_3_Click" >Satın Al</asp:LinkButton></div>
                    </div></td>
                  <td align="center" style="border-right:1px solid #2c4b5d" width="178"><div class="all-container pack-names">
                      <div class="pack-name-title"> Paket 4</div>
                      <div class="pack-price"> 189 TL</div>
                      <div class="sign-up-button-normal"><asp:LinkButton ID="lnk_odeme_tipi_4"  
                              class="sign-up-button-normal-a" runat="server" 
                              onclick="lnk_odeme_tipi_4_Click" >Satın Al</asp:LinkButton></div>
                    </div></td>
                </tr>
              </tbody>
            </table>
          </div>
          <div class="priceListContent">
            <table border="0" cellpadding="0" cellspacing="0" width="960">
              <tbody>
                <tr class="line">
                  <td class="first-row first-column-list" width="244"><div class="all-container-prices"> Yapılabilecek Survey Sayısı</div></td>
                  <td align="center" class="price-columns" width="178">5</td>
                  <td align="center" class="price-columns" width="178">10</td>
                  <td align="center" class="price-columns" width="178"><div class="all-container-prices">25</div></td>
                  <td align="center" class="price-columns" width="178"><div class="all-container-prices">50</div></td>
                </tr>
                <tr class="line">
                  <td class="first-row first-column-list"><div class="all-container-prices"> E-Posta Gönderimli Survey Katılımcı Sayısı</div></td>
                  <td align="center" class="price-columns"> 500</td>
                  <td align="center" class="price-columns"> 500</td>
                  <td align="center" class="price-columns" >500</td>
                  <td align="center" class="price-columns">500</td>
                </tr>
                <tr class="line">
                  <td class="first-row first-column-list"><div class="all-container-prices"> Açık Survey Katılımcı Sayısı</div></td>
                  <td align="center" class="price-columns">Sınırsız</td>
                  <td align="center" class="price-columns"> Sınırsız</td>
                  <td align="center" class="price-columns" >Sınırsız</td>
                  <td align="center" class="price-columns">Sınırsız</td>
                </tr>
                <tr class="line">
                  <td class="first-row first-column-list"><div class="all-container-prices">Süre</div></td>
                  <td align="center" class="price-columns">3 Aylık Üyelik</td>
                  <td align="center" class="price-columns">6 Aylık Üyelik</td>
                  <td align="center" class="price-columns">Yıllık Üyelik</td>
                  <td align="center" class="price-columns">Yıllık Üyelik</td>
                </tr>
                <tr class="line">
                  <td class="first-row first-column-list"><div class="all-container-prices"> Soru Sayısı</div></td>
                  <td align="center" class="price-columns">Sınırsız</td>
                  <td align="center" class="price-columns">Sınırsız</td>
                  <td align="center" class="price-columns">Sınırsız</td>
                  <td align="center" class="price-columns">Sınırsız</td>
                </tr>
                <tr class="line">
                  <td class="first-row first-column-list"><div class="all-container-prices"> Seçenek Sayısı</div></td>
                  <td align="center" class="price-columns">Sınırsız</td>
                  <td align="center" class="price-columns">Sınırsız</td>
                  <td align="center" class="price-columns">Sınırsız</td>
                  <td align="center" class="price-columns">Sınırsız</td>
                </tr>
                <tr class="line">
                  <td class="first-row first-column-list"><div class="all-container-prices"> Açık Survey</div></td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                </tr>
                <tr class="line">
                  <td class="first-row first-column-list"><div class="all-container-prices"> Katılımcı Gönderimli Survey</div></td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                </tr>
                <tr class="line">
                  <td class="first-row first-column-list"><div class="all-container-prices"> Raporlama</div></td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                </tr>
                <tr class="line">
                  <td class="first-row first-column-list"><div class="all-container-prices"> Surveylere Kendi Logonuzu Ekleme</div></td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                </tr>
                <tr class="line">
                  <td class="first-row first-column-list"><div class="all-container-prices"> E-Posta Adresleri, Adres Defteri</div></td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                </tr>
                <tr class="line">
                  <td class="first-row first-column-list"><div class="all-container-prices"> E-Posta Grupları Oluşturma</div></td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                </tr>
                <tr class="line">
                  <td class="first-row first-column-list"><div class="all-container-prices"> E-Posta Adreslerinin Dosyadan Aktarımı</div></td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                </tr>
                <tr class="line">
                  <td class="first-row first-column-list"><div class="all-container-prices"> Hazır Survey Soruları Alma</div></td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                </tr>
                <tr class="line">
                  <td class="first-row first-column-list"><div class="all-container-prices"> Varolan Surveyten Soru Alma</div></td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                </tr>
                <tr class="line">
                  <td class="first-row first-column-list"><div class="all-container-prices"> Tema Seçimi</div></td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                </tr>
                <tr class="line">
                  <td class="first-row first-column-list"><div class="all-container-prices"> Raporların Dosyaya Aktarımı(Excel, Pdf, Word)</div></td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                  <td align="center" class="price-columns checkbox-icon">&nbsp;</td>
                </tr>
              </tbody>
            </table>
              <table>
                  <tr>
                      <td>
                          <img src="Images/_spacer.gif" height="20px" />
                      </td>
                  </tr>
                  <tr>
                      <td><span style="font-size:17px"><b>
                          Not: Fiyatlarımıza K.D.V. Dahildir.<br />
                          Üyeliği devam eden kullanıcılar Yeni bir paket alıp üyeliklerini yenilediklerinde
                          eski satın aldıkları paketlerinde kalan kullanmadıkları anketler Yeni paketlerine
                          ilave edilecektir.</b></span>
                      </td>
                  </tr>
              </table>
          </div>
        </div>
</asp:Content>
