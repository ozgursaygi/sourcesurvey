using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing.Imaging;
using System.Data;
using BaseWebSite.Models;
using BaseClasses;


namespace BaseWebSite
{
    public partial class Purchase : System.Web.UI.Page
    {
        public int odeme_tipi_id
        {
            get { return (ViewState["odeme_tipi_id"] != null ? Convert.ToInt32(ViewState["odeme_tipi_id"].ToString()) : 0); }
            set { ViewState["odeme_tipi_id"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (BaseDB.SessionContext.Current == null || BaseDB.SessionContext.Current.ActiveUser == null)
            {
                 Response.Redirect("~/Login.aspx");
            }

            this.ErrorMessage.Text = "";
            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();

            if (Request.QueryString["odeme_tipi_id"] != null && Request.QueryString["odeme_tipi_id"].ToString() != "")
            {
                odeme_tipi_id = Convert.ToInt32(Request.QueryString["odeme_tipi_id"].ToString());
            }

            if (!Page.IsPostBack)
            {
                InitiliazeCombos();
                BindControls();
                
                this.UyelikDurumu.Text = gnlDB.UyelikDurumuTextBelirle(Convert.ToInt32(this.ddlPaket.SelectedValue.ToString()), BaseDB.SessionContext.Current.ActiveUser.UserUid);
                this.chkSirket.Checked = true;
            }

            //**NOT**Kullanıcı Tablosundaki dont_activate_purchase alanı true ise kredi kartı ile satın alma işlemlerini boş geçecek
        }

        protected void InitiliazeCombos()
        {
            //this.ddlcinsiyet.DataSource = BaseDB.DBManager.AppConnection.GetDataSet("select * from gnl_ref_cinsiyet");
            //this.ddlcinsiyet.DataTextField = "cinsiyet";
            //this.ddlcinsiyet.DataValueField = "cinsiyet_id";
            //this.ddlcinsiyet.DataBind();

            this.ddlPaket.DataSource = BaseDB.DBManager.AppConnection.GetDataSet("select *,odeme_tipi+' - '+cast (anket_sayisi as varchar(20))+' Survey - Survey Başına '+cast (katilimci_sayisi as varchar(20)) +' Katılımcı' +' - Ücret '+cast(ucret as varchar(20))+' TL' as uyelik_durumu  from gnl_uyelik_odeme_tipleri_tanimlari  where uyelik_tip_id=2");
            this.ddlPaket.DataTextField = "uyelik_durumu";
            this.ddlPaket.DataValueField = "odeme_tipi_id";
            this.ddlPaket.DataBind();

            if (odeme_tipi_id == 1)
                ddlPaket.SelectedValue = "2";
            else if (odeme_tipi_id == 2)
                ddlPaket.SelectedValue = "3";
            else if (odeme_tipi_id == 3)
                ddlPaket.SelectedValue = "4";
            else if (odeme_tipi_id == 4)
                ddlPaket.SelectedValue = "5";

            ddlPaket.Enabled = false;
        }

        protected void BindControls()
        {
            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();
            gnl_uye_kullanicilar member_users = gnlDB.GetMemberUsers(BaseDB.SessionContext.Current.ActiveUser.UserUid);
            gnl_users user = gnlDB.GetUsers(BaseDB.SessionContext.Current.ActiveUser.UserUid);

            if (member_users != null)
            {
                if (member_users.ad != null) this.Name.Text = member_users.ad;
                if (member_users.soyad != null) this.Surname.Text = member_users.soyad;
                if (member_users.email != null) this.Email.Text = member_users.email;
                if (member_users != null) this.Telefon.Text = member_users.telefonu;
                if (member_users.cep_telefonu != null) this.CepTelefonu.Text = member_users.cep_telefonu;
                if (member_users.adres != null) this.Adres.Text = member_users.adres;
                if (member_users.grup_adi != null) this.Ekip.Text = member_users.grup_adi;

                if (member_users.sirket_adi != null) this.SirketName.Text = member_users.sirket_adi;

                if (member_users.vergi_dairesi != null) this.VergiDairesi.Text = member_users.vergi_dairesi;
                if (member_users.vergi_no!= null) this.VergiNo.Text = member_users.vergi_no;
                //if (uye_kullanicilar.cinsiyet != null) this.ddlcinsiyet.SelectedValue = uye_kullanicilar.cinsiyet.ToString();

                if (member_users.dogum_tarihi != null)
                {
                    string baslangic_gun = Convert.ToDateTime(member_users.dogum_tarihi, new System.Globalization.CultureInfo("tr-TR")).Day.ToString();
                    string baslangic_ay = Convert.ToDateTime(member_users.dogum_tarihi, new System.Globalization.CultureInfo("tr-TR")).Month.ToString();
                    string baslangic_yil = Convert.ToDateTime(member_users.dogum_tarihi, new System.Globalization.CultureInfo("tr-TR")).Year.ToString();

                    if (baslangic_gun.Length == 1) baslangic_gun = "0" + baslangic_gun;
                    if (baslangic_ay.Length == 1) baslangic_ay = "0" + baslangic_ay;

                    //this.dogum_tarihi.Value = baslangic_gun + "-" + baslangic_ay + "-" + baslangic_yil;
                }

                if (member_users.uye_baslangic_tarihi != null)
                {
                    string baslangic_gun = Convert.ToDateTime(member_users.uye_baslangic_tarihi, new System.Globalization.CultureInfo("tr-TR")).Day.ToString();
                    string baslangic_ay = Convert.ToDateTime(member_users.uye_baslangic_tarihi, new System.Globalization.CultureInfo("tr-TR")).Month.ToString();
                    string baslangic_yil = Convert.ToDateTime(member_users.uye_baslangic_tarihi, new System.Globalization.CultureInfo("tr-TR")).Year.ToString();

                    if (baslangic_gun.Length == 1) baslangic_gun = "0" + baslangic_gun;
                    if (baslangic_ay.Length == 1) baslangic_ay = "0" + baslangic_ay;

                    this.UyelikBaslangic.Text = baslangic_gun + "-" + baslangic_ay + "-" + baslangic_yil;
                }

                if (member_users.uye_bitis_tarihi!= null)
                {
                    string baslangic_gun = Convert.ToDateTime(member_users.uye_bitis_tarihi, new System.Globalization.CultureInfo("tr-TR")).Day.ToString();
                    string baslangic_ay = Convert.ToDateTime(member_users.uye_bitis_tarihi, new System.Globalization.CultureInfo("tr-TR")).Month.ToString();
                    string baslangic_yil = Convert.ToDateTime(member_users.uye_bitis_tarihi, new System.Globalization.CultureInfo("tr-TR")).Year.ToString();

                    if (baslangic_gun.Length == 1) baslangic_gun = "0" + baslangic_gun;
                    if (baslangic_ay.Length == 1) baslangic_ay = "0" + baslangic_ay;

                    this.UyelikBitis.Text = baslangic_gun + "-" + baslangic_ay + "-" + baslangic_yil;
                }

               

                if (member_users.grup_sirket_tipi_id != null)
                {
                    if (member_users.grup_sirket_tipi_id == 1)
                        this.chkSirket.Checked = true;
                }

                if (member_users.son_odeme_tipi_id.HasValue)
                {
                    this.VarOlanPaket.Text = gnlDB.VarUyelikDurumuTextBelirle(BaseDB.SessionContext.Current.ActiveUser.UserUid);
                }
                else
                {
                    this.VarOlanPaket.Text = "Satın Alınmış Herhangi Bir Paket Bulunmamaktadır.";
                }
            }
            else
            {
                this.Email.Text = BaseDB.SessionContext.Current.ActiveUser.UserEmail;
                this.Name.Text = BaseDB.SessionContext.Current.ActiveUser.Name;
                this.Surname.Text = BaseDB.SessionContext.Current.ActiveUser.Surname;
                if (user != null && user.group_name != null) this.Ekip.Text = user.group_name;
                this.VarOlanPaket.Text = "Satın Alınmış Herhangi Bir Paket Bulunmamaktadır.";
            }
        }

        protected void PurchaseButton_Click(object sender, EventArgs e)
        {
            bool bos_var = false;
            string msg = "";
            this.ErrorMessage.Text = "";
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
                    this.has_error.Value = "0";
                }
            }

            //if (!ASPxCaptcha1.IsValid)
            //{
            //    this.ErrorMessage.Text = "Doğrulama Kodunu yanlış girdiniz.Lütfen tekrar giriniz.";
            //    this.has_error.Value = "1";
            //    return;
            //}

            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();

            #region Üye Kullanıcıların Kayıtları Oluşturuluyor

            gnl_uye_kullanicilar member_users = gnlDB.GetMemberUsers(BaseDB.SessionContext.Current.ActiveUser.UserUid);

            if (member_users == null)
            {
                member_users = new gnl_uye_kullanicilar();

                gnlDB.AddMemberUsers(member_users);
                member_users.user_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                member_users.email = Email.Text.Trim();
                member_users.telefonu = Telefon.Text;
                member_users.cep_telefonu = CepTelefonu.Text;
                member_users.adres = Adres.Text;
                member_users.sirket_adi = SirketName.Text;
                member_users.vergi_dairesi = VergiDairesi.Text;
                member_users.vergi_no = VergiNo.Text;
                //uye_kullanicilar.cinsiyet = Convert.ToInt32(ddlcinsiyet.SelectedValue);
                //uye_kullanicilar.dogum_tarihi = Convert.ToDateTime(dogum_tarihi.Value, new System.Globalization.CultureInfo("tr-TR"));
                member_users.ad = this.Name.Text;
                member_users.soyad = this.Surname.Text;
                member_users.grup_adi = this.Ekip.Text;
                member_users.grup_uid = BaseDB.SessionContext.Current.ActiveUser.GrupUid;


                if (chkSirket.Checked)
                    member_users.grup_sirket_tipi_id = 1;
                else
                    member_users.grup_sirket_tipi_id = 0;

                gnlDB.Kaydet();
            }
            else
            {
                member_users.email = Email.Text.Trim();
                member_users.telefonu = Telefon.Text;
                member_users.cep_telefonu = CepTelefonu.Text;
                member_users.adres = Adres.Text;
                member_users.sirket_adi = SirketName.Text;
                member_users.vergi_dairesi = VergiDairesi.Text;
                member_users.vergi_no = VergiNo.Text;
                //uye_kullanicilar.cinsiyet = Convert.ToInt32(ddlcinsiyet.SelectedValue);
                //uye_kullanicilar.dogum_tarihi = Convert.ToDateTime(dogum_tarihi.Value, new System.Globalization.CultureInfo("tr-TR"));
                member_users.ad = this.Name.Text;
                member_users.soyad = this.Surname.Text;
                
                if (chkSirket.Checked)
                    member_users.grup_sirket_tipi_id = 1;
                else
                    member_users.grup_sirket_tipi_id = 0;

                gnlDB.Kaydet();
            }
            #endregion

            Response.Redirect("PurchaseApprove.aspx?odeme_tipi_id=" + Convert.ToInt32(this.ddlPaket.SelectedValue.ToString()));
        }

        protected void ddlPaket_SelectedIndexChanged(object sender, EventArgs e)
        {
            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();
            this.UyelikDurumu.Text = gnlDB.UyelikDurumuTextBelirle(Convert.ToInt32(this.ddlPaket.SelectedValue.ToString()), BaseDB.SessionContext.Current.ActiveUser.UserUid);
        }
    
    }
}