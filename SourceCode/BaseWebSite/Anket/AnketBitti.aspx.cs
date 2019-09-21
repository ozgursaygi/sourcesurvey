using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaseWebSite.Models;

namespace BaseWebSite.Survey
{
    public partial class AnketBitti : System.Web.UI.Page
    {
        public Guid anket_uid
        {
            get { return (ViewState["anket_uid"] != null ? Guid.Parse(ViewState["anket_uid"].ToString()) : Guid.Empty); }
            set { ViewState["anket_uid"] = value; }
        }

        private string key
        {
            get { return (ViewState["key"] != null ? Convert.ToString(ViewState["key"]) : ""); }
            set { ViewState["key"] = value; }
        }

        private string tip
        {
            get { return (ViewState["tip"] != null ? Convert.ToString(ViewState["tip"]) : ""); }
            set { ViewState["tip"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            if (!IsPostBack)
            {
                if (Request.QueryString["anket_uid"] != null && Request.QueryString["anket_uid"].ToString() != "")
                {
                    anket_uid = Guid.Parse(Request.QueryString["anket_uid"].ToString());
                }

                if (Request.QueryString["key"] != null && Request.QueryString["key"].ToString() != "")
                {
                    key = Request.QueryString["key"].ToString();
                }

                if (Request.QueryString["tip"] != null && Request.QueryString["tip"].ToString() != "")
                {
                    tip = Request.QueryString["tip"].ToString();
                }

                sbr_anket anket = ankDB.SurveyGetir(anket_uid);
                if (anket.anket_sonuc_mesaji_ekle == true)
                {
                    this.SonucMesaji.Text = anket.anket_sonuc_mesaji;
                }
                else
                {
                    this.SonucMesaji.Text = "Thank You for Replying to Survey.";
                }

                string applicationPath = "";
                if (System.Web.HttpContext.Current.Request.ApplicationPath == "/")
                    applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/";
                else
                    applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/" + System.Web.HttpContext.Current.Request.ApplicationPath + "/";


                this.hyplink.NavigateUrl = applicationPath+"Anket/Anket.aspx?anket_uid=" + anket_uid;
                this.ltlLink.Text = "Click To View Results .";

                if (anket.anket_tipi_id == 1)
                {
                    hyplink.Visible = false;
                    this.ltlLink.Visible = false;
                }


                if (tip == "1")
                {
                    sbr_anket_yayinlama_mail_gonderi_aktivasyon aktivasyon = ankDB.MailGonderiAktivasyonGetirKeyeGore(key);
                    aktivasyon.anket_bitirildi = true;
                    aktivasyon.anket_bitirilme_tarihi = DateTime.Now;
                    ankDB.Kaydet();
                }
                else if (tip == "2")
                {
                    sbr_anket_yayinlama_acik_anket_aktivasyon aktivasyon = new sbr_anket_yayinlama_acik_anket_aktivasyon();
                    ankDB.AcikSurveyAktivasyonEkle(aktivasyon);
                    aktivasyon.giris_key = key;
                    aktivasyon.anket_bitirildi = true;
                    aktivasyon.anket_bitirilme_tarihi = DateTime.Now;
                    aktivasyon.anket_uid = anket_uid;
                    aktivasyon.browser = HttpContext.Current.Request.Browser.Browser;
                    aktivasyon.host_name = HttpContext.Current.Request.UserHostName;
                    aktivasyon.platform = HttpContext.Current.Request.Browser.Platform;
                    aktivasyon.user_host_adres = HttpContext.Current.Request.UserHostAddress;
                    aktivasyon.versiyon = HttpContext.Current.Request.Browser.Version;
                    ankDB.Kaydet();
                }
                else if (tip == "3")
                {
                    this.SonucMesaji.Text = "This Survey is Not Live.Thank You.";
                }
                else if (tip == "4")
                {
                    if (anket.anket_sonuc_mesaji_ekle == true)
                    {
                        this.SonucMesaji.Text = anket.anket_sonuc_mesaji;
                    }
                    else
                    {
                        this.SonucMesaji.Text = "Thank You for Replying to Survey.";
                    }
                }
            }
        }

    }
}