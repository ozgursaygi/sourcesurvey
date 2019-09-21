using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
using BaseDB;
using System.Web.Security;
using System.Drawing.Imaging;
using BaseWebSite.Models;
using System.Data;

namespace BaseWebSite
{
    public partial class Login : System.Web.UI.Page
    {
        private string DavetKey
        {
            get { return (ViewState["DavetKey"] != null ? Convert.ToString(ViewState["DavetKey"]) : ""); }
            set { ViewState["DavetKey"] = value; }
        }

        private string From
        {
            get { return (ViewState["From"] != null ? Convert.ToString(ViewState["From"]) : ""); }
            set { ViewState["From"] = value; }
        }

        private string sifre_sifirlama_key
        {
            get { return (ViewState["sifre_sifirlama_key"] != null ? Convert.ToString(ViewState["sifre_sifirlama_key"]) : ""); }
            set { ViewState["sifre_sifirlama_key"] = value; }
        }

        private int hatali_giris_sayisi
        {
            get { return (ViewState["hatali_giris_sayisi"] != null ? Convert.ToInt32(ViewState["hatali_giris_sayisi"]) : 0); }
            set { ViewState["hatali_giris_sayisi"] = value; }
        }

        public int odeme_tipi_id
        {
            get { return (ViewState["odeme_tipi_id"] != null ? Convert.ToInt32(ViewState["odeme_tipi_id"].ToString()) : 0); }
            set { ViewState["odeme_tipi_id"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
   
            
            if (!Page.IsPostBack)
            {

                if (Request.QueryString["DavetKey"] != null && Request.QueryString["DavetKey"].ToString() != "")
                {
                    DavetKey = Request.QueryString["DavetKey"].ToString();
                }

                if (Request.QueryString["From"] != null && Request.QueryString["From"].ToString() != "")
                {
                    From = Request.QueryString["From"].ToString();
                }

                if (Request.QueryString["odeme_tipi_id"] != null && Request.QueryString["odeme_tipi_id"].ToString() != "")
                {
                    odeme_tipi_id = Convert.ToInt32(Request.QueryString["odeme_tipi_id"].ToString());
                }

                if (Request.QueryString["sifre_sifirlama_key"] != null && Request.QueryString["sifre_sifirlama_key"].ToString() != "")
                {
                    sifre_sifirlama_key = Request.QueryString["sifre_sifirlama_key"].ToString();
                    GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();
                    gnlDB.SifreSifirla(sifre_sifirlama_key);
                }

                hatali_giris_sayisi = 0;
                //DevExpress.Web.ASPxEditors.ASPxCaptcha captcha = (DevExpress.Web.ASPxEditors.ASPxCaptcha)this.Login1.FindControl("ASPxCaptcha1");
                //captcha.Visible = false;
            }
            
            this.ErrorMessage.Text = "";
            this.has_error.Value = "0";
            if (From == "PurchaseControl")
            {
                this.ErrorMessage.Text = BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "11") + "<br>";
                this.has_error.Value = "1";
            }

         
        }

        protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
        {
            BaseClasses.BaseLogin objLogin = new BaseClasses.BaseLogin();
            string culture = "tr-TR";

            culture = "tr-TR";
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("tr-TR");
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("tr-TR");


            //DevExpress.Web.ASPxEditors.ASPxCaptcha captcha = (DevExpress.Web.ASPxEditors.ASPxCaptcha)this.Login1.FindControl("ASPxCaptcha1");

            //if (hatali_giris_sayisi >= 3)
            //{
            //    captcha.Visible = true;
            //    this.has_error2.Value = "0";
            //    if (!captcha.IsValid)
            //    {
            //        this.ErrorMessage.Text = "Doğrulama Kodunu yanlış girdiniz.Lütfen tekrar giriniz.";
            //        this.has_error2.Value = "1";
            //        return;
            //    }
            //}

            if (objLogin.UserValidaton(Login1.UserName, Login1.Password))
            {
                SessionContext newSession = SessionContext.StartSession();
                newSession.Culture = newSession.UICulture = culture;
                newSession.LoginUser.UserUid = objLogin.GetUserUid();
                newSession.LoginUser.UserNameAndSurname = objLogin.GetUserNameAndSurName();
                newSession.LoginUser.UserEmail = objLogin.GetEmail();
                newSession.LoginUser.GrupUid = objLogin.GetUserGrupUid();
                newSession.LoginUser.Name = objLogin.GetName();
                newSession.LoginUser.Surname = objLogin.GetSurname();
                newSession.LoginUser.IsSistemAdmin = objLogin.GetUserAdminType();
                
                BaseClasses.SessionKeeper.AddCurrentSession();
                BaseClasses.SessionKeeper.AddLoggedInUserToDataBase("login");

                string theme = "BaseBlue";
                newSession.Theme = theme;

                FormsAuthentication.SetAuthCookie(newSession.ActiveUser.UserUid.ToString(), false);

                SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
                ankDB.DavetKabulEt(DavetKey,BaseDB.SessionContext.Current.ActiveUser.UserUid);

                if (From == "PurchaseControl")
                {
                    Response.Redirect("Purchase.aspx?odeme_tipi_id=" + odeme_tipi_id);
                }
                else
                {
                    Response.Redirect("Anket/AnketDashboard.aspx?menu_id=3");
                }
            }
            else
            {
                this.ErrorMessage.Text = "Login Process cannot be performed. Please try again.";
                this.has_error.Value = "1";
                e.Authenticated = false;
                hatali_giris_sayisi += hatali_giris_sayisi + 1;
                //if (hatali_giris_sayisi<=3)
                //    captcha.Visible = false;
            }
        }

        protected void Login1_LoginError(object sender, EventArgs e)
        {

        }

        private bool UserValidation(string userName, string passWord)
        {
            return false;
        }
    }
}
