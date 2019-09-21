using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing.Imaging;
using System.Threading;
using BaseDB;
using System.Web.Security;
using BaseWebSite.Models;

namespace BaseWebSite
{
    public partial class Activation : System.Web.UI.Page
    {
        private string key
        {
            get { return (ViewState["key"] != null ? Convert.ToString(ViewState["key"]) : ""); }
            set { ViewState["key"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
            

                if (Request.QueryString["key"] != null && Request.QueryString["key"].ToString() != "")
                {
                    key = Request.QueryString["key"].ToString();
                }
            }

            this.ErrorMessage.Text = "";
        }

        protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
        {
            BaseClasses.BaseLogin objLogin = new BaseClasses.BaseLogin();
            string culture = "tr-TR";

            culture = "tr-TR";
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("tr-TR");
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("tr-TR");

            //DevExpress.Web.ASPxEditors.ASPxCaptcha captcha = (DevExpress.Web.ASPxEditors.ASPxCaptcha)this.Login1.FindControl("ASPxCaptcha1");
            this.has_error2.Value = "0";

            //if (!captcha.IsValid)
            //{
            //    this.ErrorMessage.Text = "Doğrulama Kodunu yanlış girdiniz.Lütfen tekrar giriniz.";
            //    this.has_error.Value = "1";
            //    return;
            //}
            if (objLogin.UserValidatonWithAktivasyonKey(Login1.UserName, Login1.Password,key))
            {
                Guid user_id = objLogin.GetUserUid();

                GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();
                gnl_users user = gnlDB.GetUsers(user_id);

                if (user != null)
                {
                    if (user.activation_key == key && user.activation_ok == true)
                    {
                        Response.Redirect("RegisterationInfoPage.aspx?tip=2");
                    }
                    else if (user.activation_key == key && user.activation_ok != true)
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

                        user.activation_ok = true;
                        user.active = true;

                        gnlDB.Kaydet();
                        Response.Redirect("Anket/AnketDashboard.aspx?menu_id=3");
                    }
                    else if (user.activation_key != key)
                    {
                        e.Authenticated = false;
                        this.ErrorMessage.Text = BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "22") + "<br>";
                        this.has_error.Value = "1";
                    }
                    else
                    {
                        e.Authenticated = false;
                        this.ErrorMessage.Text = BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "22") + "<br>";
                        this.has_error.Value = "1";
                    }
                }
                else
                {

                    e.Authenticated = false;
                    this.ErrorMessage.Text = BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "22") + "<br>";
                    this.has_error.Value = "1";
                }
            }
            else
            {
                this.ErrorMessage.Text = "Activation Failed.Please try again.";
                this.has_error.Value = "1";
                e.Authenticated = false;
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