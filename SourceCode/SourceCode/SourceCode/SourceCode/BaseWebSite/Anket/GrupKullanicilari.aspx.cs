using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaseWebSite.Models;
using System.Data;
using System.IO;
using System.Text;

namespace BaseWebSite.Survey
{
    public partial class GrupKullanicilari : System.Web.UI.Page
    {
        public Guid grup_uid
        {
            get { return (ViewState["grup_uid"] != null ? Guid.Parse(ViewState["grup_uid"].ToString()) : Guid.Empty); }
            set { ViewState["grup_uid"] = value; }
        }

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

            if (Request["__EVENTTARGET"] == "KullaniciIslemleri")
            {
                Response.Redirect("KullaniciGruplar.aspx");
            }
            
            GenelRepository ankDB = RepositoryManager.GetRepository<GenelRepository>();

            if (!IsPostBack)
            {
                if (Request.QueryString["grup_uid"] != null && Request.QueryString["grup_uid"].ToString() != "")
                {
                    grup_uid = Guid.Parse(Request.QueryString["grup_uid"].ToString());
                }


                BindKullanicilar();
            }

            
            gnl_user_groups grup = ankDB.GetUserGroup(grup_uid);

            if (grup != null)
                this.LinkButtonHeader.Text = grup.group_name + " / User List";

            if (Request["__EVENTTARGET"] == "DavetEt")
            {
                string degerler = Request["__EVENTARGUMENT"].ToString();

                string[] arrDegerler = degerler.Replace("^#^", "^").Split('^');

                string eposta = arrDegerler[0];
                
                DavetEt(eposta);
            }

            if (Request["__EVENTTARGET"] == "YoneticiYap")
            {
                string degerler = Request["__EVENTARGUMENT"].ToString();

                string[] arrDegerler = degerler.Replace("^#^", "^").Split('^');

                string grupid = arrDegerler[0];
                string kullaniciid = arrDegerler[1];

                
                gnl_group_user_definitions grup_users = ankDB.GrupGetUser(Guid.Parse(grupid.ToString()), Guid.Parse(kullaniciid.ToString()));

                if (grup_users != null)
                {
                    grup_users.is_user_admin = true;
                    ankDB.Kaydet();
                }
                
            }

            if (Request["__EVENTTARGET"] == "YoneticiliktenCikart")
            {
                string degerler = Request["__EVENTARGUMENT"].ToString();

                string[] arrDegerler = degerler.Replace("^#^", "^").Split('^');

                string grupid = arrDegerler[0];
                string kullaniciid = arrDegerler[1];


                gnl_group_user_definitions group_users = ankDB.GrupGetUser(Guid.Parse(grupid.ToString()), Guid.Parse(kullaniciid.ToString()));

                if (group_users != null)
                {
                    group_users.is_user_admin = false;
                    ankDB.Kaydet();
                }

            }

            SurveyRepository sbrDB = RepositoryManager.GetRepository<SurveyRepository>();
            DataSet ds = sbrDB.DavetEttiklerimDataSet(grup_uid);

            this.rptDavetEttiklerim.DataSource = ds;
            this.rptDavetEttiklerim.DataBind();

            ltlMenu.Text = CreateMenu();
        }

        protected string CreateMenu()
        {
            bool is_grup_admin=false;
            System.Text.StringBuilder menu_output = new StringBuilder();
            menu_output.Append("<ul class=\"mainMenu\" >");
            menu_output.Append("<li ><a href=\"AnketDashboard.aspx\" >BACK</a></li>");
            menu_output.Append("<li ><a href=\"KullaniciGruplar.aspx\" >KULLANICI İŞLEMLERİ</a></li>");
             GenelRepository ankDB = RepositoryManager.GetRepository<GenelRepository>();
             if (ankDB.IsGrupUserAdmin(Guid.Parse(grup_uid.ToString()), BaseDB.SessionContext.Current.ActiveUser.UserUid))
             {
                 is_grup_admin = true;
             }

             if (is_grup_admin == true)
             {
                 menu_output.Append("<li ><a href=\"#\" onclick=\"DavetEt();return false;\">INVITE</a></li>");
                 menu_output.Append("<li ><a href=\"#\" onclick=\"DavetEttiklerim();return false;\">INVITATION</a></li>");
             }

            menu_output.Append("</ul>");
            return menu_output.ToString();
        }

        protected new void Page_LoadComplete(object sender, EventArgs e)
        {
            BindKullanicilar();
        }

        protected void BindKullanicilar()
        {
            GenelRepository ankDB = RepositoryManager.GetRepository<GenelRepository>();


            DataSet ds = ankDB.GetGroupUsersDataSet(grup_uid);

            this.rptGrupKullanicilari.DataSource = ds;
            this.rptGrupKullanicilari.DataBind();
        }

        protected void DavetEt(string eposta)
        {
            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();
            SurveyRepository sbrDB = RepositoryManager.GetRepository<SurveyRepository>();
            gnl_users users = gnlDB.GetUsers(BaseDB.SessionContext.Current.ActiveUser.UserUid);

            string key = System.Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 20);
            

            sbr_anket_davet davet = new sbr_anket_davet();
            sbrDB.DavetEkle(davet);

            davet.davet_eden_kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
            davet.davet_edilen_email = eposta.Trim();
            davet.davet_edilen_grup_uid = grup_uid;
            davet.davet_key = key;
            davet.davet_tarihi = DateTime.Now;
            sbrDB.Kaydet();

            string mailBody = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"Templates\DavetMailTemplate.html").ReadToEnd();
            mailBody = mailBody.Replace("%%ad%%", BaseDB.SessionContext.Current.ActiveUser.UserNameAndSurname);

            string applicationPath = "";
            if (System.Web.HttpContext.Current.Request.ApplicationPath == "/")
                applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/";
            else
                applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/" + System.Web.HttpContext.Current.Request.ApplicationPath + "/";

            mailBody = mailBody.Replace("%%path_url%%", applicationPath);
            mailBody = mailBody.Replace("%%link%%", applicationPath + "Davet.aspx?key=" + key);

            #region mail gönderiliyor 
            if (BaseClasses.BaseFunctions.getInstance().IsEmailValid(eposta.Trim().Trim()))
            {
                BaseClasses.BaseFunctions.getInstance().SendSMTPMail(eposta.Trim(), "", "Ekip Üyeliği Çağrısı", mailBody, "", null, "", "genel");
            }
            #endregion

        }

        public string YoneticiYap(object grup_uid, object kullanici_uid)
        {
            string result = "";

            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();
            
            gnl_group_user_definitions group_users = gnlDB.GrupGetUser(Guid.Parse(grup_uid.ToString()), Guid.Parse(kullanici_uid.ToString()));

            if (gnlDB.IsGrupUserAdmin(Guid.Parse(grup_uid.ToString()), BaseDB.SessionContext.Current.ActiveUser.UserUid))
            {
                if (group_users != null)
                {
                    if (group_users.is_admin == true)
                    {
                        result = "";
                    }
                    else if (group_users.is_user_admin == true)
                    {
                        result = "<a href=\"#\" class=\"smallUnderlineLink\" onclick=\"YoneticiliktenCikart('" + grup_uid + "','" + kullanici_uid + "');\">Remove From Management</a>";
                    }
                    else
                    {
                        result = "<a href=\"#\" class=\"smallUnderlineLink\" onclick=\"YoneticiYap('" + grup_uid + "','" + kullanici_uid + "');\">Add To Management</a>";
                    }
                }
            }

            return result;
        }

        protected void tumu_Click(object sender, EventArgs e)
        {
            Response.Redirect("KullaniciGruplar.aspx?grup_durumu_id=0");
        }

        protected void yonettigim_Click(object sender, EventArgs e)
        {
            Response.Redirect("KullaniciGruplar.aspx?grup_durumu_id=1");
        }

        protected void uyeolunan_Click(object sender, EventArgs e)
        {
            Response.Redirect("KullaniciGruplar.aspx?grup_durumu_id=2");
        }
    }
}