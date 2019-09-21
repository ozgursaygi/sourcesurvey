using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaseWebSite.Models;
using System.IO;

namespace BaseWebSite
{
    public partial class ArkadasimaOner : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (BaseDB.SessionContext.Current == null || BaseDB.SessionContext.Current.ActiveUser == null)
            {
                Response.Redirect("~/Login.aspx");
            }
        }

        protected void Gonder_Click(object sender, EventArgs e)
        {
            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();

            this.has_error.Value = "0";
            if (this.Email.Text.Trim() == "")
            {
                this.ErrorMessage.Text = "Lütfen bir E-Posta Adresi giriniz.";
                this.has_error.Value = "1";
                return;
            }
            this.ErrorMessage.Text = "";

            string mailBody = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"Templates\ArkadasimaOnerMailTemplate.html").ReadToEnd();
            mailBody = mailBody.Replace("%%ad%%", BaseDB.SessionContext.Current.ActiveUser.UserNameAndSurname);
            mailBody = mailBody.Replace("%%message%%", this.Mesaj.Text);

            string applicationPath = "";
            if (System.Web.HttpContext.Current.Request.ApplicationPath == "/")
                applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/";
            else
                applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/" + System.Web.HttpContext.Current.Request.ApplicationPath + "/";

            mailBody = mailBody.Replace("%%path_url%%", applicationPath);


            #region mail gönderiliyor
            if (BaseClasses.BaseFunctions.getInstance().IsEmailValid(this.Email.Text))
            {
                BaseClasses.BaseFunctions.getInstance().SendSMTPMail(this.Email.Text, "", BaseDB.SessionContext.Current.ActiveUser.UserNameAndSurname +" suggest you a survey application", mailBody, "", null, "", "genel");

                gnl_suggest_to_friend suggest = new gnl_suggest_to_friend();
                gnlDB.SaveSuggest(suggest);
                suggest.suggesting_user_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                suggest.suggest_email = this.Email.Text;
                suggest.suggesting_message = this.Mesaj.Text;
                suggest.suggest_date = DateTime.Now;

                gnlDB.Kaydet();

                ClientScript.RegisterStartupScript(typeof(string), "Close", "<script> alert('Arkadaşınıza Önerme İşlemi Gerçekleştirilmiştir.Teşekkür Ederiz.'); </script>");
            }
            else
            {
                this.ErrorMessage.Text = "Lütfen Geçerli Bir E-Posta Adresi giriniz.";
                this.has_error.Value = "1";
                return;
            }
            #endregion

        }
    
    }
}