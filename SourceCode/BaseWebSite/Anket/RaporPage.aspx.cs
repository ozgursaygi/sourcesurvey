using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BaseWebSite.Survey
{
    public partial class RaporPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (BaseDB.SessionContext.Current == null || BaseDB.SessionContext.Current.ActiveUser == null)
            {
                Response.Redirect("~/Login.aspx");
            }

            if (Request["__EVENTTARGET"] == "AnaSayfa")
            {
                Response.Redirect("MainPage.aspx");
            }
        }

       protected void kullanici_bazli_anket_raporu_Click(object sender, EventArgs e)
        {
            this.LinkButtonHeader.Text = "Kullanıcı Bazlı Survey Raporu";
            this.iframeMap.Attributes["src"] = "Raporlar/KullaniciBazliAnketRapor.aspx";
        }

       protected void anket_giris_raporu_Click(object sender, EventArgs e)
       {
           this.LinkButtonHeader.Text = "Survey Entry Report";
           this.iframeMap.Attributes["src"] = "Raporlar/KullaniciBazliAnketGirisRaporu.aspx";
       }

       protected void anket_bitirme_raporu_Click(object sender, EventArgs e)
       {
           this.LinkButtonHeader.Text = "Survey Finished Report";
           this.iframeMap.Attributes["src"] = "Raporlar/KullaniciBazliAnketBitirmeRaporu.aspx";
       }

       protected void anket_cevaplanma_raporu_Click(object sender, EventArgs e)
       {
           this.LinkButtonHeader.Text = "Survey Answered Report";
           this.iframeMap.Attributes["src"] = "Raporlar/KullaniciBazliAnketCevaplanmaRaporu.aspx";
       }

       protected void anket_cevap_raporu_Click(object sender, EventArgs e)
       {
           this.LinkButtonHeader.Text = "Survey Answer Report";
           this.iframeMap.Attributes["src"] = "Raporlar/AnketSoruTipileriCevapRaporu.aspx";
       }
       protected void anket_cevap_raporu_tumu_Click(object sender, EventArgs e)
       {
           this.LinkButtonHeader.Text = "Survey Answer Report(All) ";
           this.iframeMap.Attributes["src"] = "Raporlar/AnketSoruCevapRaporuTumu.aspx";
       }
    }
}