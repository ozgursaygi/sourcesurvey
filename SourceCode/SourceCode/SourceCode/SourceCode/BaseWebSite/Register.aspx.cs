using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BaseWebSite.Models;
using BaseClasses;
using System.Drawing.Imaging;
using System.IO;


namespace BaseWebSite
{
    public partial class Register : System.Web.UI.Page
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (BaseDB.SessionContext.Current != null && BaseDB.SessionContext.Current.ActiveUser != null)
            {
                Response.Redirect("~/Anket/AnketDashboard.aspx?menu_id=3");
            }

            this.ErrorMessage.Text = "";
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


            }

            if (DavetKey == "")
            {
                //this.ltl_ucretsiz_uyelik.Text = "<div style=\"padding:5px 5px 5px 5px\"><p class=\"warning\" ><span class=\"bold\"><b>Uyarı :</b></span>Warning.</p></div>";
            }

            if (From == "PurchaseControl")
            {
                this.ErrorMessage.Text = BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "100") + "<br>";
            }
        }

        protected void CreateUserButton_Click(object sender, EventArgs e)
        {
            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();
            SurveyRepository anlDB = RepositoryManager.GetRepository<SurveyRepository>();
            this.has_error.Value = "0";
            if (this.Email.Text.Trim() != "")
            {
                gnl_users user = gnlDB.GetUsersByEmail(this.Email.Text.Trim());

                if (!chkKabul.Checked)
                {
                    this.ErrorMessage.Text = "Lütfen Üyelik Koşullarını ve Survey Sorumluluklarını Okuduğunuzu Kutucuğu İşaretleyerek Belirtiniz.";
                    this.has_error.Value = "1";
                    return;
                }

                //if (!ASPxCaptcha1.IsValid)
                //{
                //    this.ErrorMessage.Text = "Doğrulama Kodunu yanlış girdiniz.Lütfen tekrar giriniz.";
                //    this.has_error.Value = "1";
                //    return;
                //}
                if (user != null)
                {
                    this.ErrorMessage.Text = BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "3") + "<br>";
                    this.has_error.Value = "1";
                    return;
                }
                else
                {
                    this.ErrorMessage.Text = "";
                    string encriptedPassword = "";

                    SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

                    sbr_anket_davet davet = ankDB.DavetGetir(DavetKey);

                    if (DavetKey != "" && davet == null)
                    {
                        this.ErrorMessage.Text = BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "25") + "<br>";
                        this.has_error.Value = "1";
                        return;
                    }
                    else if (DavetKey != "" && davet != null && davet.davet_edilen_email.Trim() != this.Email.Text.Trim())
                    {
                        this.ErrorMessage.Text = BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "26") + "<br>";
                        this.has_error.Value = "1";
                        return;
                    }
                    else
                    {
                        user = new gnl_users();
                        gnlDB.AddUsers(user);
                        user.name = this.Name.Text;
                        user.surname = this.Surname.Text;
                        user.email = this.Email.Text.Trim();
                        user.group_name = this.Email.Text.Trim();

                        BaseLogin objLogin = new BaseLogin();
                        encriptedPassword = objLogin.EncriptText(this.Password.Text.Trim());

                        System.Text.Encoding enc = System.Text.Encoding.ASCII;
                        //byte[] byteArray = enc.GetBytes(encriptedPassword);
                        user.password = encriptedPassword;

                        string key = System.Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 20);

                        user.activation_key = key;
                        user.activation_ok = false;

                        if (chkKabul.Checked)
                        {

                            System.IO.StreamReader template1 = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"Anket\Templates\UyelikKosullariTemplate.html");
                            string Body1 = template1.ReadToEnd();
                            template1.Close();

                            user.membership_conditions = Body1;

                            System.IO.StreamReader template2 = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"Anket\Templates\Anket123Sorumluluklari.html");
                            string Body2 = template2.ReadToEnd();
                            template2.Close();

                            user.survey_responsibilities = Body2;

                        }

                        gnlDB.Kaydet();
                        

                        #region User Group Create
                        gnl_user_groups grup = new gnl_user_groups();
                        gnlDB.AddUserGroup(grup);
                        grup.active = true;
                        grup.group_name= Ekip.Text;
                        gnlDB.Kaydet();
                        #endregion

                        #region Kullanıcı Grup Bilgileri Güncelleniyor
                        gnl_users user2 = gnlDB.GetUsers(user.user_uid);
                        user2.group_uid = grup.group_uid;
                        user2.group_name = grup.group_name;
                        gnlDB.Kaydet();
                        #endregion

                        #region Group Users 
                        gnl_group_user_definitions group_users = new gnl_group_user_definitions();
                        gnlDB.GroupAddUser(group_users);
                        group_users.active = true;
                        group_users.group_uid = grup.group_uid;
                        group_users.is_admin = true;
                        group_users.is_user_admin = false;
                        group_users.user_uid = user.user_uid;
                        gnlDB.Kaydet();
                        #endregion

                        #region Üye Kullanıcıların Kayıtları Oluşturuluyor

                        gnl_uye_kullanicilar member_users = gnlDB.GetMemberUsers(user.user_uid);

                        if (member_users == null)
                        {
                            member_users = new gnl_uye_kullanicilar();

                            gnlDB.AddMemberUsers(member_users);
                            member_users.email = Email.Text.Trim();
                            member_users.telefonu = "";
                            member_users.cep_telefonu = "";
                            member_users.adres = "";
                            member_users.user_uid = user.user_uid;
                            member_users.sirket_adi = "";
                            member_users.vergi_dairesi = "";
                            member_users.vergi_no = "";
                            //uye_kullanicilar.cinsiyet = Convert.ToInt32(ddlcinsiyet.SelectedValue);
                            //uye_kullanicilar.dogum_tarihi = Convert.ToDateTime(dogum_tarihi.Value, new System.Globalization.CultureInfo("tr-TR"));
                            member_users.ad = this.Name.Text;
                            member_users.soyad = this.Surname.Text;
                            member_users.grup_adi = this.Ekip.Text;
                            member_users.grup_uid = grup.group_uid;
                            member_users.aktif = true;
                            member_users.uye_baslangic_tarihi = DateTime.Now;
                            member_users.kalan_anket_sayisi = 1000000;
                            //if (chkSirket.Checked)
                            //    member_users.grup_sirket_tipi_id = 1;
                            //else

                            member_users.grup_sirket_tipi_id = 0;

                            gnlDB.Kaydet();
                        }
                        else
                        {
                            member_users.email = Email.Text.Trim();
                            member_users.telefonu = "";
                            member_users.cep_telefonu = "";
                            member_users.adres = "";
                            member_users.sirket_adi = "";
                            member_users.vergi_dairesi = "";
                            member_users.vergi_no = "";
                            //uye_kullanicilar.cinsiyet = Convert.ToInt32(ddlcinsiyet.SelectedValue);
                            //uye_kullanicilar.dogum_tarihi = Convert.ToDateTime(dogum_tarihi.Value, new System.Globalization.CultureInfo("tr-TR"));
                            member_users.ad = this.Name.Text;
                            member_users.soyad = this.Surname.Text;
                            member_users.ad = this.Name.Text;
                            member_users.soyad = this.Surname.Text;

                            member_users.grup_sirket_tipi_id = 0;

                            gnlDB.Kaydet();
                        }

                        #endregion

                        this.ErrorMessage.Text = BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "4");

                        string mailBody = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"Templates\ActivationMailTemplate.html").ReadToEnd();
                        mailBody = mailBody.Replace("%%ad%%", user.name+" "+user.surname);

                        string applicationPath = "";
                        if (System.Web.HttpContext.Current.Request.ApplicationPath == "/")
                            applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/";
                        else
                            applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/" + System.Web.HttpContext.Current.Request.ApplicationPath + "/";

                        mailBody = mailBody.Replace("%%path_url%%", applicationPath );
                        mailBody = mailBody.Replace("%%link%%", applicationPath + "Activation.aspx?key=" + key);

                        #region mail gönderiliyor
                        if (BaseClasses.BaseFunctions.getInstance().IsEmailValid(user.email.Trim()))
                        {
                            BaseClasses.BaseFunctions.getInstance().SendSMTPMail(user.email, "", "Survey Activation", mailBody, "", null, "", "genel");
                        }
                        #endregion

                        ankDB.DavetKabulEt(DavetKey, user.user_uid);

                        Response.Redirect("RegisterationInfoPage.aspx?tip=1");
                    }
                }
            }
        }
       
    }
}
