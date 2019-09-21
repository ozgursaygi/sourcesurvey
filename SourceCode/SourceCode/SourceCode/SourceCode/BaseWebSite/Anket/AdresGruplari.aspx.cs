using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BaseWebSite.Models;
using System.Collections;
using System.Reflection;
using System.Text;
using System.IO;
using System.Threading;

namespace BaseWebSite.Survey
{
    public partial class AdresGruplari : System.Web.UI.Page
    {
        public string mail_grubu_uid
        {
            get { return (ViewState["mail_grubu_uid"] != null ? ViewState["mail_grubu_uid"].ToString() : ""); }
            set { ViewState["mail_grubu_uid"] = value; }
        }

        public string mail_grubu_text
        {
            get { return (ViewState["mail_grubu_text"] != null ? ViewState["mail_grubu_text"].ToString() : ""); }
            set { ViewState["mail_grubu_text"] = value; }
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
                
                InitiliazeCombos();

                if (Request.QueryString["mail_grubu_uid"] != null && Request.QueryString["mail_grubu_uid"].ToString() != "")
                {
                    mail_grubu_uid = Request.QueryString["mail_grubu_uid"].ToString();
                    ddlMailGrubu.SelectedValue = mail_grubu_uid;
                    mail_grubu_text = BaseDB.DBManager.AppConnection.ExecuteSql("select mail_grubu_adi from sbr_mail_gruplari where mail_grubu_uid='" + mail_grubu_uid + "'");
                }

            }

            if (Request["__EVENTTARGET"] == "AnaSayfa")
            {
                Response.Redirect("MainPage.aspx");
            }

            if (Request["__EVENTTARGET"] == "MailGrubuEkle")
            {
                string degerler = Request["__EVENTARGUMENT"].ToString();

                string[] arrDegerler = degerler.Replace("^#^", "^").Split('^');

                string grup = arrDegerler[0];
                string ad = arrDegerler[1];
               
                ankDB.MailGrubuOlustur(grup,ad);
                if (ddlgrup.SelectedValue != null && ddlgrup.SelectedValue != "")
                {
                    this.ddlMailGrubu.Visible = true;
                    DataSet ds1 = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_mail_gruplari where grup_uid='" + ddlgrup.SelectedValue + "'");
                    this.ddlMailGrubu.DataSource = ds1;
                    this.ddlMailGrubu.DataTextField = "mail_grubu_adi";
                    this.ddlMailGrubu.DataValueField = "mail_grubu_uid";
                    this.ddlMailGrubu.DataBind();
                }
                else
                {
                    this.ddlMailGrubu.Visible = false;
                }
            }

            if (Request["__EVENTTARGET"] == "MailEkle")
            {
                string degerler = Request["__EVENTARGUMENT"].ToString();

                string[] arrDegerler = degerler.Replace("^#^", "^").Split('^');

                string ad = arrDegerler[0];
                string soyad = arrDegerler[1];
                string mail = arrDegerler[2];

                ankDB.MailListesiOlustur(ad,soyad,mail,ddlMailGrubu.SelectedValue);
            }

            if (Request["__EVENTTARGET"] == "MailUpdate")
            {
                string degerler = Request["__EVENTARGUMENT"].ToString();

                string[] arrDegerler = degerler.Replace("^#^", "^").Split('^');

                string id = arrDegerler[0];
                string ad = arrDegerler[1];
                string soyad = arrDegerler[2];
                string mail = arrDegerler[3];

                if(id!="")
                    ankDB.MailListesiUpdate(Guid.Parse(id), ad, soyad, mail);
            }

            if (Request["__EVENTTARGET"] == "Duzenle")
            {
                string id = Request["__EVENTARGUMENT"].ToString();
                MailBilgileriniHazirla(id);
            }

            if (Request["__EVENTTARGET"] == "Sil")
            {
                string id = Request["__EVENTARGUMENT"].ToString();
                MailSil(id);
            }

        }

        protected new void Page_LoadComplete(object sender, EventArgs e)
        {
            BindMailGruplari();
            if (this.ddlMailGrubu.SelectedValue != null && this.ddlMailGrubu.SelectedValue!="") this.LinkButtonHeader.Text = this.ddlMailGrubu.SelectedItem.Text;
        }

        protected void InitiliazeCombos()
        {
            DataSet ds = BaseDB.DBManager.AppConnection.GetDataSet("select * from dbo.sbr_anket_kullanici_gruplari('" + BaseDB.SessionContext.Current.ActiveUser.UserUid + "') ");
            this.ddlgrup.DataSource = ds;
            this.ddlgrup.DataTextField = "group_name";
            this.ddlgrup.DataValueField = "group_uid";
            this.ddlgrup.DataBind();


            
            if (ddlgrup.SelectedValue != null && ddlgrup.SelectedValue != "")
            {
                this.ddlMailGrubu.Visible = true;
                DataSet ds1 = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_mail_gruplari where grup_uid='" + ddlgrup.SelectedValue + "'");
                this.ddlMailGrubu.DataSource = ds1;
                this.ddlMailGrubu.DataTextField = "mail_grubu_adi";
                this.ddlMailGrubu.DataValueField = "mail_grubu_uid";
                this.ddlMailGrubu.DataBind();
            }
            else
            {
                this.ddlMailGrubu.Visible = false;
            }

            DataSet ds3 = BaseDB.DBManager.AppConnection.GetDataSet("select * from dbo.sbr_anket_kullanici_gruplari('" + BaseDB.SessionContext.Current.ActiveUser.UserUid + "')  where (is_admin=1 or is_user_admin=1)");
            this.ddlanketgrup.DataSource = ds3;
            this.ddlanketgrup.DataTextField = "group_name";
            this.ddlanketgrup.DataValueField = "group_uid";
            this.ddlanketgrup.DataBind();

            if (ds3.Tables[0].Rows.Count == 0)
            {
                this.ddlgrup.Visible = false;
                this.div_grup.Visible = false;
            }
            else
            {
                this.ddlgrup.Visible = true;
                this.div_grup.Visible = true;
            }

        }

        protected void BindMailGruplari()
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            string criteria = "";
            string text = "";

            if (Request["__EVENTTARGET"] == "HizliAramadan")
            {
                text = Request["__EVENTARGUMENT"].ToString();
                this.rptMaillerPaging.SayfaNo = 0;
                this.search_text.Value = text;
            }

            if (text != "")
            {
                DataSet dscriteria = BaseDB.DBManager.AppConnection.ExecuteSP("Search", new ArrayList { "Content", "Object" }, new ArrayList { text, "sbr_mail_gruplari_kullanici_listesi" });

                if (dscriteria.Tables[0].Rows.Count > 0)
                {
                    criteria = dscriteria.Tables[0].Rows[0]["Criteria"].ToString();
                }
            }

            Guid grup_uid = Guid.Empty;

            if (ddlMailGrubu.SelectedValue != null && ddlMailGrubu.SelectedValue.ToString() != "")
            {
                grup_uid = Guid.Parse(ddlMailGrubu.SelectedValue.ToString());
                mail_grubu_text = BaseDB.DBManager.AppConnection.ExecuteSql("select mail_grubu_adi from sbr_mail_gruplari where mail_grubu_uid='" + grup_uid + "'");
            }

            DataSet ds = ankDB.MailGetirDataSet(criteria, grup_uid);

            this.rptMaillerPaging.dataSource = ds;
            this.rptMaillerPaging.SayfaBuyuklugu = 10;
            this.rptMaillerPaging.Bind();
            this.rptMailler.DataSource = this.rptMaillerPaging.Sayfa;
            this.rptMailler.DataBind();
        }

        protected void MailBilgileriniHazirla(string id)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            if (id == "") return;

            sbr_mail_gruplari_kullanici_listesi mail = ankDB.MailGetir(Guid.Parse(id));

            if (mail.ad != null) this.txtmailad_duzenle.Text = mail.ad;
            if (mail.soyad != null) this.txtmailsoyad_duzenle.Text = mail.soyad;
            if (mail.email != null) this.txtmail_duzenle.Text = mail.email;

            ClientScript.RegisterStartupScript(this.GetType(), "Redirect1", "<script>MailDuzenle('" + id + "')</script>");
        }

        protected void MailSil(string id)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            ankDB.MailSil(Guid.Parse(id));
            ankDB.Kaydet();
        }

        protected void ddlgrup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlgrup.SelectedValue != null && ddlgrup.SelectedValue != "")
            {
                this.ddlMailGrubu.Visible = true;
                DataSet ds1 = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_mail_gruplari where grup_uid='" + ddlgrup.SelectedValue + "'");
                this.ddlMailGrubu.DataSource = ds1;
                this.ddlMailGrubu.DataTextField = "mail_grubu_adi";
                this.ddlMailGrubu.DataValueField = "mail_grubu_uid";
                this.ddlMailGrubu.DataBind();
            }
            else
            {
                this.ddlMailGrubu.Visible = false;
            }
            BindMailGruplari();
        }

        protected void ddlMailGrubu_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindMailGruplari();
        }

        



    }
}