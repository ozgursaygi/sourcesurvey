using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing.Imaging;
using BaseWebSite.Models;
using System.IO;

namespace BaseWebSite
{
    public partial class RequestPassword : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
 
        }

        protected void PasswordRequest_Click(object sender, EventArgs e)
        {
            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();
            this.has_error.Value = "0";
            if (this.Email.Text.Trim() != "")
            {
                gnl_users user = gnlDB.GetUsersByEmail(this.Email.Text.Trim());

                //if (!ASPxCaptcha1.IsValid)
                //{
                //    this.ErrorMessage.Text = "Doğrulama Kodunu yanlış girdiniz.Lütfen tekrar giriniz.";
                //    this.has_error.Value = "1";
                //    return;
                //}

                if (user == null)
                {
                    this.ErrorMessage.Text = BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "14") + "<br>";
                    this.has_error.Value = "1";
                    return;
                }
                else
                {
                    BaseClasses.BaseLogin objLogin = new BaseClasses.BaseLogin();
                    string key = System.Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 10);
                    string sifre = System.Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 10);
                    string encriptedPassword = objLogin.EncriptText(sifre);

                    System.Text.Encoding enc = System.Text.Encoding.ASCII;
                    //byte[] byteArray = enc.GetBytes(encriptedPassword);

                    gnl_kullanici_sifre_sifirlama sifre_sifirlama = new gnl_kullanici_sifre_sifirlama();
                    gnlDB.SifreSifirlamaEkle(sifre_sifirlama);
                    sifre_sifirlama.sifirlama_istek_tarihi = DateTime.Now;
                    sifre_sifirlama.sifirlama_key = key;
                    sifre_sifirlama.sifre_sifirlanacak_email = this.Email.Text.Trim();
                    sifre_sifirlama.sifre = encriptedPassword;
                    gnlDB.Kaydet();


                    string mailBody = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"Templates\PasswordReminderMailTemplate.html").ReadToEnd();
                    mailBody = mailBody.Replace("%%email%%", this.Email.Text.Trim());
                    mailBody = mailBody.Replace("%%sifre%%", sifre);

                    string applicationPath = "";
                    if (System.Web.HttpContext.Current.Request.ApplicationPath == "/")
                        applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/";
                    else
                        applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/" + System.Web.HttpContext.Current.Request.ApplicationPath + "/";


                    mailBody = mailBody.Replace("%%path_url%%", applicationPath);
                    mailBody = mailBody.Replace("%%link%%", applicationPath + "Login.aspx?sifre_sifirlama_key=" + key);

                    #region mail gönderiliyor
                    if (BaseClasses.BaseFunctions.getInstance().IsEmailValid(this.Email.Text.Trim()))
                    {
                        BaseClasses.BaseFunctions.getInstance().SendSMTPMail(this.Email.Text.Trim(), "", "Yeni Şifre İşlemi", mailBody, "", null, "", "genel");
                    }
                    #endregion

                    this.ErrorMessage.Text = BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "103") + "<br>";
                    this.has_error.Value = "1";

                }
            }
        }

    }
}