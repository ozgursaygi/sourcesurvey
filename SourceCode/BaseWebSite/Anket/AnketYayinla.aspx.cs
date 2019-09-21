using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;
using BaseWebSite.Models;

namespace BaseWebSite.Survey
{
    public partial class SurveyYayinla : System.Web.UI.Page
    {
        public Guid anket_uid
        {
            get { return (ViewState["anket_uid"] != null ? Guid.Parse(ViewState["anket_uid"].ToString()) : Guid.Empty); }
            set { ViewState["anket_uid"] = value; }
        }

        public string anket_url
        {
            get { return (ViewState["anket_url"] != null ? ViewState["anket_url"].ToString() : ""); }
            set { ViewState["anket_url"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (BaseDB.SessionContext.Current == null || BaseDB.SessionContext.Current.ActiveUser == null)
            {
                Response.Redirect("~/Login.aspx");
            }
            
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            if (!IsPostBack)
            {
                if (Request.QueryString["anket_uid"] != null && Request.QueryString["anket_uid"].ToString() != "")
                {                    
                    anket_uid = Guid.Parse(Request.QueryString["anket_uid"].ToString());
                    
                    sbr_anket anket = ankDB.SurveyGetir(anket_uid);
                    this.LinkButtonHeader.Text = (anket.anket_adi.Length > 60) ? anket.anket_adi.Substring(0, 57) + "..." : anket.anket_adi + " - " + ankDB.SurveyDurumu(Convert.ToInt32(anket.anket_durumu_id.ToString()));
                }

                InitiliazeCombos();
                FormuDuzenle();

            }

            if (Request["__EVENTTARGET"] == "AnaSayfa")
            {
                Response.Redirect("MainPage.aspx");
            }

            if (Request["__EVENTTARGET"] == "SurveyeDon")
            {
                Response.Redirect("AnketDetayi.aspx?anket_uid=" + anket_uid);
            }

            if (Request["__EVENTTARGET"] == this.UpdatePanelMailler.ClientID)
            {
                if (Request["__EVENTARGUMENT"] != null)
                    BindSurveyMailler(Guid.Parse(Request["__EVENTARGUMENT"].ToString()));
            }
            this.div_error_message.Visible = false;
            YayinBilgileriniHazirla();

            sbr_anket anket2 = ankDB.SurveyGetir(anket_uid);
            if (anket2!=null && anket2.grup_uid != null && anket2.grup_uid.HasValue)
            {
                if (BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_mail_gruplari where grup_uid='" + anket2.grup_uid.ToString() + "'").Tables[0].Rows.Count == 0)
                {
                    if (anket2.anket_tipi_id != null && anket2.anket_tipi_id == 1)
                    {
                        this.ErrorMessage.Text = "Lütfen E-Posta Gruplarınızı Tanımlayınız.";
                        this.div_error_message.Visible = true;
                        this.error_message.Visible = false;
                    }
                }
            }
        }

        protected new void Page_LoadComplete(object sender, EventArgs e)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            BindMailTarihcesi();
            sbr_anket anket = ankDB.SurveyGetir(anket_uid);
            if (ddlMailGrubu.SelectedValue != null && ddlMailGrubu.SelectedValue.ToString() != "" && anket.grup_uid != null && anket.grup_uid.ToString()!="")
            {
                this.ltlUyari.Text = "Gönderilebilecek Kalan Katılımcı Sayısı : " + ankDB.KalanKatilimciSurveySayisi(Guid.Parse(ddlMailGrubu.SelectedValue.ToString()), BaseDB.SessionContext.Current.ActiveUser.UserUid, anket_uid, Guid.Parse(anket.grup_uid.ToString())).ToString() + " .<br>Katılımcı sayısını aşan mailler gönderilmeyecektir.Gönderilen kişilere tekrar aynı anket gönderilmeyecektir.Aksi takdirde çift bilgi oluşacaktır.";
            }
            
        }

        protected void FormuDuzenle()
        { 
             SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            sbr_anket anket = ankDB.SurveyGetir(anket_uid);

            if (anket.anket_tipi_id != null && anket.anket_tipi_id == 1)
            {
                this.div_acik_anket.Visible = false;
                this.div_katilimci_anketi.Visible = true;
            }
            else
            {
                this.div_acik_anket.Visible = true;
                this.div_katilimci_anketi.Visible = false;
            }
        }

        protected void BindSurveyMailler(Guid tarihce_uid)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            DataSet ds = ankDB.MailGonderiKisiTarihcesiGetirDataSet(tarihce_uid);

            this.rptMailler.DataSource = ds;
            this.rptMailler.DataBind();
        }

        protected void BindMailTarihcesi()
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();


            DataSet ds = ankDB.MailGonderiTarihcesiDataSet(anket_uid);

            this.rptGonderiTarihcesi.DataSource = ds;
            this.rptGonderiTarihcesi.DataBind();
        }

        protected void YayinBilgileriniHazirla()
        {
            string applicationPath = "";
            if (System.Web.HttpContext.Current.Request.ApplicationPath == "/")
                applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/";
            else
                applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/" + System.Web.HttpContext.Current.Request.ApplicationPath + "/";

            this.txtDirektLink.Text = applicationPath + "Anket/Anket.aspx?anket_uid=" + anket_uid;

            StringBuilder popup_str = new StringBuilder();

            popup_str.Append("<a href=\"" + applicationPath + "Anket/Anket.aspx?anket_uid=" + anket_uid + "\" onclick=\"window.open('" + applicationPath + "Survey/Survey.aspx?anket_uid=" + anket_uid + "', '','toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=1,resizable=1,width=1000,height=700');return false\" >Survey</a>");

            this.txtPopupLink.Text = popup_str.ToString();

            StringBuilder page_str = new StringBuilder();

            page_str.Append("<a href=\"" + applicationPath + "Anket/Anket.aspx?anket_uid=" + anket_uid + "\"  >Survey</a>");

            this.txtPageLink.Text = page_str.ToString();

            StringBuilder iframe_str = new StringBuilder();

            iframe_str.Append("<iframe frameborder=\"0\" width=\"100%\" height=\"700\" scrolling=\"auto\" allowtransparency=\"true\" src=\"" + applicationPath + "Survey/Survey.aspx?anket_uid=" + anket_uid + "\"><a href=\"" + applicationPath + "Survey/Survey.aspx?anket_uid=" + anket_uid + "\">Survey</a></iframe>");

            this.txtIframeLink.Text = iframe_str.ToString();
            anket_url = applicationPath + "Anket/Anket.aspx?anket_uid=" + anket_uid;

        }

        protected void InitiliazeCombos()
        {

            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            sbr_anket anket = ankDB.SurveyGetir(anket_uid);

            if (anket.grup_uid != null && anket.grup_uid.HasValue)
            {
                this.ddlMailGrubu.Visible = true;
                DataSet ds1 = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_mail_gruplari where grup_uid='" + anket.grup_uid.ToString() + "'");
                this.ddlMailGrubu.DataSource = ds1;
                this.ddlMailGrubu.DataTextField = "mail_grubu_adi";
                this.ddlMailGrubu.DataValueField = "mail_grubu_uid";
                this.ddlMailGrubu.DataBind();
            }


        }

        protected void btnSurveyGonder_Click(object sender, EventArgs e)
        {
            this.ErrorMessage.Text = "";
            this.div_error_message.Visible = false;
            if (ddlMailGrubu.SelectedValue == null || ddlMailGrubu.SelectedValue.ToString() == "")
            {
                this.ErrorMessage.Text = "Lütfen Gönderilecek Mail Grubunu Seçiniz.";
                this.div_error_message.Visible = true;
                return;
            }
            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            sbr_anket anket = ankDB.SurveyGetir(anket_uid);

            DateTime dt_noweksi1 = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day, new System.Globalization.CultureInfo("tr-TR")).AddDays(-1);
            
            if((Convert.ToDateTime(anket.bitis_tarihi, new System.Globalization.CultureInfo("tr-TR")) <= dt_noweksi1))
            {
                    this.ErrorMessage.Text = BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "206");
                    this.div_error_message.Visible = true;
                    return;
            }
            

            //bool mail_grubuna_gonderildi = ankDB.AktivasyonMailGrubaGonderildiMi(anket_uid, Guid.Parse(ddlMailGrubu.SelectedValue.ToString()));

            //if (mail_grubuna_gonderildi)
            //{
            //    this.ErrorMessage.Text = "Daha Önce bu Mail Grubuna ilgili anket gönderilmiştir.Gönderilen kişilere tekrar aynı anket gönderilmeyecektir.";
            //    this.div_error_message.Visible = true;
            //    return;
            //}

            if (txtMailMesaj.Text.Trim() == "" || txtMailSubject.Text.Trim() == "")
            {
                return;
            }

            int kalan_katilimci = ankDB.KalanKatilimciSurveySayisi(Guid.Parse(ddlMailGrubu.SelectedValue.ToString()), BaseDB.SessionContext.Current.ActiveUser.UserUid, anket_uid, Guid.Parse(anket.grup_uid.ToString()));
            if (kalan_katilimci <= 0)
            {
                this.ErrorMessage.Text = "Satın Aldığınız Paketteki Katılımcı Sayınızı Doldurdunuz Kalan Katılımcılara Mail Gönderilmeyecektir.";
                this.div_error_message.Visible = true;
                return;
            }

            Guid tarihce_uid = Guid.NewGuid();

            #region Mailler Gönderiliyor
            
            
            string anket_adi="";
            
            if(anket.anket_goruntulenen_ad_ekle!=null && anket.anket_goruntulenen_ad_ekle==true)
                anket_adi=anket.anket_goruntulenen_ad;
            else
                anket_adi=anket.anket_adi;

            string applicationPath = "";
            if (System.Web.HttpContext.Current.Request.ApplicationPath == "/")
                applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/";
            else
                applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/" + System.Web.HttpContext.Current.Request.ApplicationPath + "/";

            bool herhangi_mail_gonderildi = ankDB.anket_yayinlama_maili_gonder(this.txtMailSubject.Text, this.txtMailMesaj.Text, applicationPath, Guid.Parse(ddlMailGrubu.SelectedValue.ToString()), anket_uid, tarihce_uid, anket_adi, BaseDB.SessionContext.Current.ActiveUser.UserUid, Guid.Parse(anket.grup_uid.ToString()));
            #endregion

            #region Tarihçe Oluşturuluyor
            if (herhangi_mail_gonderildi)
            {
                sbr_anket_yayinlama_mail_gonderi_tarihcesi tarihce = new sbr_anket_yayinlama_mail_gonderi_tarihcesi();
                ankDB.MailGonderiTarihcesiObjectEkle(tarihce);
                tarihce.tarihce_uid = tarihce_uid;
                tarihce.anket_uid = anket_uid;
                tarihce.gonderilen_mail_grubu_uid = Guid.Parse(ddlMailGrubu.SelectedValue.ToString());
                tarihce.gonderilen_mail_konusu = this.txtMailSubject.Text;
                tarihce.gonderilen_mail = this.txtMailMesaj.Text;
                tarihce.gonderen_user_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                tarihce.gonderi_tarihi = DateTime.Now;
                HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;
                tarihce.host_name = HttpContext.Current.Request.UserHostName;
                tarihce.user_host_adres = HttpContext.Current.Request.UserHostAddress;
                ankDB.Kaydet();
            }
            #endregion

            ankDB.SurveyDurumuDegistir((int)BaseWebSite.Models.anket_durumu.Yayinda, anket_uid, "Published", BaseDB.SessionContext.Current.ActiveUser.UserUid);
            this.LinkButtonHeader.Text = (anket.anket_adi.Length > 60) ? anket.anket_adi.Substring(0, 57) + "..." : anket.anket_adi + " - " + ankDB.SurveyDurumu(Convert.ToInt32(anket.anket_durumu_id.ToString()));


            if (herhangi_mail_gonderildi)
                ClientScript.RegisterClientScriptBlock(this.GetType(), "Mesaj", "alert('Mail Gönderme ve Survey Yayınlama İşlemi Gerçekleştirilmiştir.');", true);
        }

        protected void btnSurveyYayinla_Click(object sender, EventArgs e)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            sbr_anket anket = ankDB.SurveyGetir(anket_uid);

            DateTime dt_noweksi1 = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day, new System.Globalization.CultureInfo("tr-TR")).AddDays(-1);

            if ((Convert.ToDateTime(anket.bitis_tarihi, new System.Globalization.CultureInfo("tr-TR")) <= dt_noweksi1))
            {
                this.ErrorMessage.Text = BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "206");
                this.div_error_message.Visible = true;
                return;
            }

            ankDB.SurveyDurumuDegistir((int)BaseWebSite.Models.anket_durumu.Yayinda, anket_uid, "Published", BaseDB.SessionContext.Current.ActiveUser.UserUid);
            this.LinkButtonHeader.Text = (anket.anket_adi.Length > 60) ? anket.anket_adi.Substring(0, 57) + "..." : anket.anket_adi +" - " + ankDB.SurveyDurumu(Convert.ToInt32(anket.anket_durumu_id.ToString()));
            ClientScript.RegisterClientScriptBlock(this.GetType(), "Mesaj", "alert('Survey Published.');", true);
        }

        protected void btnDirektYayinla_Click(object sender, EventArgs e)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            sbr_anket anket = ankDB.SurveyGetir(anket_uid);

            DateTime dt_noweksi1 = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day, new System.Globalization.CultureInfo("tr-TR")).AddDays(-1);

            if ((Convert.ToDateTime(anket.bitis_tarihi, new System.Globalization.CultureInfo("tr-TR")) <= dt_noweksi1))
            {
                this.ErrorMessage.Text = BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "206");
                this.div_error_message.Visible = true;
                return;
            }

            ankDB.SurveyDurumuDegistir((int)BaseWebSite.Models.anket_durumu.Yayinda, anket_uid, "Published", BaseDB.SessionContext.Current.ActiveUser.UserUid);
            this.LinkButtonHeader.Text = (anket.anket_adi.Length > 60) ? anket.anket_adi.Substring(0, 57) + "..." : anket.anket_adi + " - " + ankDB.SurveyDurumu(Convert.ToInt32(anket.anket_durumu_id.ToString()));
            ClientScript.RegisterClientScriptBlock(this.GetType(), "Mesaj", "alert('Survey Published.');", true);
        }
        
    }
}