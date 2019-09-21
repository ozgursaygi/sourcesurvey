using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaseWebSite.Models;
using BaseClasses;
using System.Drawing.Imaging;

namespace BaseWebSite
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (BaseDB.SessionContext.Current == null || BaseDB.SessionContext.Current.ActiveUser == null)
                Response.Redirect("Default.aspx");

            this.FailureText.Text = "";   
        }


        protected void ChangePasswordButton_Click(object sender, EventArgs e)
        {
            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();
            this.has_error.Value = "0";
            if (this.CurrentPassword.Text.Trim() != "" )
            {
                gnl_users user = gnlDB.GetUsers(BaseDB.SessionContext.Current.ActiveUser.UserUid);

                //if (!ASPxCaptcha1.IsValid)
                //{
                //    this.FailureText.Text = "Doğrulama Kodunu yanlış girdiniz.Lütfen tekrar giriniz.";
                //    this.has_error.Value = "1";
                //    return;
                //}

                if (user == null)
                {
                    this.FailureText.Text = BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "5");
                    this.has_error.Value = "1";
                    return;
                }
                else
                {
                    this.FailureText.Text = "";
                    string encriptedPassword = "";

                    string encriptedNewPassword = "";


                    BaseLogin objLogin = new BaseLogin();
                    encriptedPassword = objLogin.EncriptText(this.CurrentPassword.Text);

                    System.Text.Encoding enc = System.Text.Encoding.ASCII;
                    //byte[] byteArray = enc.GetBytes(encriptedPassword);

                    if (objLogin.PassWordCompare(user.password , encriptedPassword))
                    {
                        encriptedNewPassword = objLogin.EncriptText(this.NewPassword.Text);

                        System.Text.Encoding encr = System.Text.Encoding.ASCII;
                        //byte[] byteArray2 = enc.GetBytes(encriptedNewPassword);
                        user.password = encriptedNewPassword;
                    }
                    else
                    {
                        this.FailureText.Text = BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "8");
                        this.has_error.Value = "1";
                        return;
                    }
                    

                    gnlDB.Kaydet();

                    this.FailureText.Text = BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "7");
                    this.has_error.Value = "1";
                }
            }
        }

    }
}
