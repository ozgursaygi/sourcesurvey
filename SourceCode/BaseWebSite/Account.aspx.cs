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

namespace BaseWebSite
{
    public partial class Account : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (BaseDB.SessionContext.Current == null || BaseDB.SessionContext.Current.ActiveUser == null)
            {
                Response.Redirect("~/Login.aspx");
            }

            ChangePasswordLnk.NavigateUrl = "ChangePassword.aspx";

            this.ErrorMessage.Text = "";

            if (!Page.IsPostBack)
            {
                InitiliazeCombos();

                BindControls();
            }

            
            
        }

        protected new void Page_LoadComplete(object sender, EventArgs e)
        {
            BindPaketler(BaseDB.SessionContext.Current.ActiveUser.UserUid);
        }

        protected void BindPaketler(Guid user_uid)
        {
            GenelRepository ankDB = RepositoryManager.GetRepository<GenelRepository>();
            gnl_uye_kullanicilar member_users = ankDB.GetMemberUsers(BaseDB.SessionContext.Current.ActiveUser.UserUid);

            DataSet ds = ankDB.PaketAlimlariGetirDataSet(user_uid);

            this.rptPaketler.DataSource = ds;
            this.rptPaketler.DataBind();

            if (member_users != null && member_users.son_odeme_tipi_id.HasValue)
            {
                //this.VarOlanPaket.Text = ankDB.VarUyelikDurumuTextBelirle(BaseDB.SessionContext.Current.ActiveUser.UserUid);
            }
            else
            {
                //this.VarOlanPaket.Text = "Satın Alınmış Herhangi Bir Paket Bulunmamaktadır.";
            }

        }

        protected void InitiliazeCombos()
        {
            //this.ddlcinsiyet.DataSource = BaseDB.DBManager.AppConnection.GetDataSet("select * from gnl_ref_cinsiyet");
            //this.ddlcinsiyet.DataTextField = "cinsiyet";
            //this.ddlcinsiyet.DataValueField = "cinsiyet_id";
            //this.ddlcinsiyet.DataBind();
        }

        protected void BindControls()
        {
            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();
            gnl_uye_kullanicilar member_users = gnlDB.GetMemberUsers(BaseDB.SessionContext.Current.ActiveUser.UserUid);
            gnl_users users = gnlDB.GetUsers(BaseDB.SessionContext.Current.ActiveUser.UserUid);

            if (member_users != null)
            {
               // this.chkSirket.Disabled = true;
                if (member_users.ad != null) this.Name.Text = member_users.ad;
                if (member_users.soyad != null) this.Surname.Text = member_users.soyad;
                if (member_users.email != null) this.Email.Text = member_users.email;
                if (member_users.telefonu != null) this.Telefon.Text = member_users.telefonu;
                if (member_users.cep_telefonu != null) this.CepTelefonu.Text = member_users.cep_telefonu;
                if (member_users.adres != null) this.Adres.Text = member_users.adres;
                if (users != null && users.group_name != null) this.Ekip.Text = users.group_name;
                if (member_users.sirket_adi != null) this.SirketName.Text = member_users.sirket_adi;

                if (member_users.vergi_dairesi != null) this.VergiDairesi.Text = member_users.vergi_dairesi;
                if (member_users.vergi_no != null) this.VergiNo.Text = member_users.vergi_no;
                
                if (member_users.dogum_tarihi != null)
                {
                    string baslangic_gun = Convert.ToDateTime(member_users.dogum_tarihi, new System.Globalization.CultureInfo("tr-TR")).Day.ToString();
                    string baslangic_ay = Convert.ToDateTime(member_users.dogum_tarihi, new System.Globalization.CultureInfo("tr-TR")).Month.ToString();
                    string baslangic_yil = Convert.ToDateTime(member_users.dogum_tarihi, new System.Globalization.CultureInfo("tr-TR")).Year.ToString();

                    if (baslangic_gun.Length == 1) baslangic_gun = "0" + baslangic_gun;
                    if (baslangic_ay.Length == 1) baslangic_ay = "0" + baslangic_ay;

                    //this.dogum_tarihi.Value = baslangic_gun + "-" + baslangic_ay + "-" + baslangic_yil;
                }


                this.chkSirket.Checked = false;

                if (member_users.grup_sirket_tipi_id != null)
                {
                    //if (member_users.grup_sirket_tipi_id == 1)
                    //this.chkSirket.Checked = true;
                }
                
                if (member_users.son_odeme_tipi_id.HasValue)
                {
                    //this.div_ucretli_uye.Visible = true;
                }
                else
                {
                    //this.div_ucretli_uye.Visible = false;
                }
            }
            else
            {
                if (users != null && users.group_name != null) this.Ekip.Text = users.group_name;
                this.Email.Text = BaseDB.SessionContext.Current.ActiveUser.UserEmail;
                this.Name.Text = BaseDB.SessionContext.Current.ActiveUser.Name;
                this.Surname.Text = BaseDB.SessionContext.Current.ActiveUser.Surname;
                this.div_ucretsiz.Visible = false;
                this.div_ucretsiz_sirket.Visible = false;
                this.div_ucretli_uye.Visible = false;
                this.UpdateButton.Visible = false;
            }
           
        }

        protected void UpdateButton_Click(object sender, EventArgs e)
        {
            bool bos_var = false;
            string msg = "";
            this.has_error.Value = "0";
            if (SirketName.Text.Trim() == "")
            {
                msg += BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "110") + "<br>";
                bos_var = true;
            }

            if (VergiDairesi.Text.Trim() == "")
            {
                msg += BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "20") + "<br>";
                bos_var = true;
            }

            if (VergiNo.Text.Trim() == "")
            {
                msg += BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "21") + "<br>";
                bos_var = true;
            }

            if (this.chkSirket.Checked)
            {
                if (bos_var)
                {
                    this.ErrorMessage.Text = msg;
                    this.has_error.Value = "1";
                    return;
                }
                else
                {
                    this.ErrorMessage.Text = "";
                }
            }

           
            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();
            gnl_uye_kullanicilar member_users = gnlDB.GetMemberUsers(BaseDB.SessionContext.Current.ActiveUser.UserUid);

            if (member_users != null)
            {

                //if (!ASPxCaptcha1.IsValid)
                //{
                //    this.ErrorMessage.Text = "Doğrulama Kodunu yanlış girdiniz.Lütfen tekrar giriniz.";
                //    this.has_error.Value = "1";
                //    return;
                //}

                member_users.email = Email.Text.Trim();
                member_users.telefonu = Telefon.Text;
                member_users.cep_telefonu = CepTelefonu.Text;
                member_users.adres = Adres.Text;
                member_users.sirket_adi = SirketName.Text;
                member_users.vergi_dairesi = VergiDairesi.Text;
                member_users.vergi_no = VergiNo.Text;
                //uye_kullanicilar.cinsiyet = Convert.ToInt32(ddlcinsiyet.SelectedValue);
                //uye_kullanicilar.dogum_tarihi = Convert.ToDateTime(dogum_tarihi.Value, new System.Globalization.CultureInfo("tr-TR"));
                member_users.grup_adi = Ekip.Text;
                
                if (chkSirket.Checked)
                    member_users.grup_sirket_tipi_id = 1;
                else
                    member_users.grup_sirket_tipi_id = 0;

                gnlDB.Kaydet();

                gnl_user_groups grup = gnlDB.GetUserGroup(BaseDB.SessionContext.Current.ActiveUser.GrupUid);
                grup.group_name = Ekip.Text;
                gnlDB.Kaydet();

                this.ErrorMessage.Text = BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "24") + "<br>";
                this.has_error.Value = "1";
            }
        }

    }
}