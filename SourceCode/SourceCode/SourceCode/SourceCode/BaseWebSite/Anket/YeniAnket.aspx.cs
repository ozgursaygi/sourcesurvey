using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaseWebSite.Models;
using System.Data;
using System.IO;

namespace BaseWebSite.Survey
{
    public partial class YeniAnket : System.Web.UI.Page
    {
        public Guid anket_uid
        {
            get { return (ViewState["anket_uid"] != null ? Guid.Parse(ViewState["anket_uid"].ToString()) : Guid.Empty); }
            set { ViewState["anket_uid"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ErrorMessage.Text = "";

            if (BaseDB.SessionContext.Current == null || BaseDB.SessionContext.Current.ActiveUser == null)
            {
                Response.Redirect("~/Login.aspx");
            }
            
            if (!IsPostBack)
            {
                if (Request.QueryString["anket_uid"] != null && Request.QueryString["anket_uid"].ToString() != "")
                {
                    anket_uid = Guid.Parse(Request.QueryString["anket_uid"].ToString());
                }

                InitiliazeCombos();
                if (anket_uid != null && anket_uid != Guid.Empty && anket_uid.ToString() != "")
                    BindControls();
            }
            
            if (Request["__EVENTTARGET"] == "AnaSayfa")
            {
                Response.Redirect("MainPage.aspx");
            }

            FormuDuzenle();
        }

        protected void FormuDuzenle()
        {
            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();

            if (!gnlDB.HasUserGroup(BaseDB.SessionContext.Current.ActiveUser.UserUid))
                this.div_grup.Visible = false;

            if (anket_uid == null || anket_uid == Guid.Empty || anket_uid.ToString() == "")
                this.btnLogoyuSil.Visible = false;

            Image1.Visible = false;
            this.image_div.Visible = false;

            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            sbr_anket anket = ankDB.SurveyGetir(anket_uid);

            if (anket != null)
            {
                if (anket.logo != null)
                {
                    Image1.ImageUrl = "~/BaseAshx/ShowImage.ashx?anket_uid=" + anket_uid;
                    Image1.Height = 100;
                    Image1.Visible = true;
                    this.image_div.Visible = true;
                }
                else
                {
                    Image1.Height = 0;
                    Image1.Visible = false;
                    this.image_div.Visible = false;
                }
            }

            if (ddlTip.SelectedValue == "1")
            {
                this.ltl_anket_tipi_uyari.Text = "<span style=\"color:red\">Survey With Send E-Mail</span> Select Type. Bu Survey Survey içinde tanımlı veya tanımlayacağınız e-posta gruplarına e-posta gönderilerek yapılır.";
            }
            else if (ddlTip.SelectedValue == "2")
            {
                this.ltl_anket_tipi_uyari.Text = "<span style=\"color:red\">Open Survey </span> Select Type. You can share or embed your site";
            }

            if (!gnlDB.IsPurchasedUser(BaseDB.SessionContext.Current.ActiveUser.UserUid))
            {
                this.ltl_ucretsiz_uyelik.Text = "<div style=\"padding:5px 5px 5px 5px\"><p class=\"warning\" ><span class=\"bold\"><b>Uyarı :</b></span>Ücretsiz Üyelikte kayıt olduğunuz tarihten itibaren 15 gün için anket tanımlayabilirsiniz.</p></div>";
            }

            
            gnl_uye_kullanicilar members = gnlDB.GetMemberUsersByGroup(Guid.Parse(ddlgrup.SelectedValue.ToString()));

            if (members != null)
            {
                this.ltl_anket_sayisi.Text = "<div style=\"padding:5px 5px 5px 5px\"><p class=\"packageProperties\" >Seçtiğiniz Ekibe bağlı olarak oluşturabileceğiniz Survey Sayısı : <b>" + members.kalan_anket_sayisi.ToString() + "</b></p></div>";
            }
            else
            {
                int anket_sayisi = gnlDB.OdemetipiSurveySayisiGetir(1);
                int olusturulan_anket_sayisi = ankDB.OlusturulanSurveySayisiGetirGrubaGore(Guid.Parse(ddlgrup.SelectedValue.ToString()));

                this.ltl_anket_sayisi.Text = "<div style=\"padding:5px 5px 5px 5px\"><p class=\"packageProperties\" >Seçtiğiniz Ekibe bağlı olarak oluşturabileceğiniz Survey Sayısı : <b>" + (anket_sayisi - olusturulan_anket_sayisi).ToString() + "</b></p></div>";
            }
            
        }

        protected void InitiliazeCombos()
        {
            //this.ddlKategori.DataSource = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_ref_anket_kategori");
            //this.ddlKategori.DataTextField = "kategori_kodu";
            //this.ddlKategori.DataValueField= "kategori_id";
            //this.ddlKategori.DataBind();

            this.ddlTip.DataSource = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_ref_anket_tipi");
            this.ddlTip.DataTextField = "anket_tipi";
            this.ddlTip.DataValueField = "anket_tipi_id";
            this.ddlTip.DataBind();

            this.ddlgrup.DataSource = BaseDB.DBManager.AppConnection.GetDataSet("select * from dbo.sbr_anket_kullanici_gruplari('" + BaseDB.SessionContext.Current.ActiveUser.UserUid + "') "); ;
            this.ddlgrup.DataTextField = "group_name";
            this.ddlgrup.DataValueField = "group_uid";
            this.ddlgrup.DataBind();
        }

        protected void BindControls()
        { 
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            sbr_anket anket = ankDB.SurveyGetir(anket_uid);

            if (anket.anket_adi!=null) this.txtSurveyAdi.Text = anket.anket_adi;
            if (anket.anket_mesaji!=null) this.txtSurveyMesaji.Text = anket.anket_mesaji;
            if (anket.anket_sonuc_mesaji!=null) this.txtSurveySonucMesaji.Text = anket.anket_sonuc_mesaji;
            if (anket.anket_goruntulenen_ad!=null) this.txtGorunenAd.Text = anket.anket_goruntulenen_ad;

            if (anket.anket_mesaji_ekle != null)
            {
                this.ChkSurveyMesajiEkle.Checked = Convert.ToBoolean(anket.anket_mesaji_ekle);
            }
            if (anket.anket_sonuc_mesaji_ekle!=null) this.chkSurveySonucMetni.Checked = Convert.ToBoolean(anket.anket_sonuc_mesaji_ekle);
            if (anket.anket_goruntulenen_ad_ekle!=null) this.ChkGorunenAd.Checked = Convert.ToBoolean(anket.anket_goruntulenen_ad_ekle);
            //if (anket.ip_kisitlamasi!=null) this.chkIpRestriction.Checked = Convert.ToBoolean(anket.ip_kisitlamasi);
            //if (anket.bitis_tarihinde_anketi_kaldir != null) this.chkBitisTarihindeSurveyiKapat.Checked = Convert.ToBoolean(anket.bitis_tarihinde_anketi_kaldir);
            
            
            if (anket.baslangic_tarihi != null)
            {
                string baslangic_gun = Convert.ToDateTime(anket.baslangic_tarihi, new System.Globalization.CultureInfo("tr-TR")).Day.ToString();
                string baslangic_ay = Convert.ToDateTime(anket.baslangic_tarihi, new System.Globalization.CultureInfo("tr-TR")).Month.ToString();
                string baslangic_yil = Convert.ToDateTime(anket.baslangic_tarihi, new System.Globalization.CultureInfo("tr-TR")).Year.ToString();

                if (baslangic_gun.Length == 1) baslangic_gun = "0" + baslangic_gun;
                if (baslangic_ay.Length == 1) baslangic_ay = "0" + baslangic_ay;

                this.baslangic_tarihi.Value = baslangic_gun + "-" + baslangic_ay + "-" + baslangic_yil;

            }

            if (anket.bitis_tarihi != null)
            {

                string bitis_gun = Convert.ToDateTime(anket.bitis_tarihi, new System.Globalization.CultureInfo("tr-TR")).Day.ToString();
                string bitis_ay = Convert.ToDateTime(anket.bitis_tarihi, new System.Globalization.CultureInfo("tr-TR")).Month.ToString();
                string bitis_yil = Convert.ToDateTime(anket.bitis_tarihi, new System.Globalization.CultureInfo("tr-TR")).Year.ToString();

                if (bitis_gun.Length == 1) bitis_gun = "0" + bitis_gun;
                if (bitis_ay.Length == 1) bitis_ay = "0" + bitis_ay;

                this.bitis_tarihi.Value = bitis_gun + "-" + bitis_ay + "-" + bitis_yil;
            }

            //if (anket.kategori_id != null) this.ddlKategori.SelectedValue = anket.kategori_id.ToString();
            if (anket.anket_tipi_id != null) this.ddlTip.SelectedValue = anket.anket_tipi_id.ToString();
            if (anket.sayfadaki_soru_sayisi != null) this.ddlSoruSayisi.SelectedValue = anket.sayfadaki_soru_sayisi.ToString();
            //if (anket.soru_cevaplama_zorunlulugu != null) this.ddlCevaplamaTipi.SelectedValue = anket.soru_cevaplama_zorunlulugu.ToString();

            if (anket.grup_uid != null)
            {
                GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();

                if (gnlDB.HasUserGroup(BaseDB.SessionContext.Current.ActiveUser.UserUid))
                    this.ddlgrup.SelectedValue = anket.grup_uid.ToString();
            }

            if (ankDB.SurveyIcinHerhangiBirCevapVerilmisMi(anket_uid))
                this.ddlTip.Enabled = false;

            if (anket.anket_tema_id == 1)
                this.rdSurveyTema1.Checked=true;
            else if (anket.anket_tema_id == 2)
                this.rdSurveyTema2.Checked = true;
            else if (anket.anket_tema_id == 3)
                this.rdSurveyTema3.Checked = true;

            if (anket.logo != null)
            {
                Image1.ImageUrl = "~/BaseAshx/ShowImage.ashx?anket_uid=" + anket_uid;
                Image1.Height = 100;
                Image1.Visible = true;
                this.image_div.Visible = true;
            }
            else
            {
                Image1.Height = 0;
                Image1.Visible = false;
                this.image_div.Visible = false;
            }
        }

        protected void btnSurveyOlustur_Click(object sender, EventArgs e)
        {
            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            gnl_uye_kullanicilar member_users = gnlDB.GetMemberUsersByGroup(Guid.Parse(ddlgrup.SelectedValue.ToString()));
            
            this.has_error.Value = "0";


            if (gnlDB.IsPurchasedUserGrubaGore(Guid.Parse(ddlgrup.SelectedValue.ToString())))
            {
                if ((Convert.ToDateTime(bitis_tarihi.Value, new System.Globalization.CultureInfo("tr-TR")) > Convert.ToDateTime(member_users.uye_bitis_tarihi, new System.Globalization.CultureInfo("tr-TR"))) || (Convert.ToDateTime(baslangic_tarihi.Value, new System.Globalization.CultureInfo("tr-TR")) > Convert.ToDateTime(member_users.uye_bitis_tarihi, new System.Globalization.CultureInfo("tr-TR"))))
                {
                    this.ErrorMessage.Text = BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "205");
                    this.has_error.Value = "1";
                    return;
                }

            }
            else
            {
                gnl_users user = gnlDB.GetUsersByGroup(Guid.Parse(ddlgrup.SelectedValue.ToString()));

                if (user != null)
                {
                    if (Convert.ToDateTime(bitis_tarihi.Value, new System.Globalization.CultureInfo("tr-TR"))< Convert.ToDateTime(baslangic_tarihi.Value, new System.Globalization.CultureInfo("tr-TR"))) 
                    {
                        this.ErrorMessage.Text = "End Date must be greater than Start Date";
                        this.has_error.Value = "1";
                        return;
                    }
                }
            }

            //if (Convert.ToDateTime(bitis_tarihi.Value, new System.Globalization.CultureInfo("tr-TR")) < Convert.ToDateTime(baslangic_tarihi.Value, new System.Globalization.CultureInfo("tr-TR")))
            //{
            //    this.ErrorMessage.Text = BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "10");
            //    this.has_error.Value = "1";
            //    return;
            //}

            DateTime dt_noweksi1 = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day, new System.Globalization.CultureInfo("tr-TR")).AddDays(-1);

            if (Convert.ToDateTime(bitis_tarihi.Value, new System.Globalization.CultureInfo("tr-TR")) <= Convert.ToDateTime(dt_noweksi1, new System.Globalization.CultureInfo("tr-TR")))
            {
                this.ErrorMessage.Text = "Survey End Date should be great than Today Date.";
                this.has_error.Value = "1";
                return;
            }

            if (!gnlDB.IsPurchasedUserGrubaGore(Guid.Parse(ddlgrup.SelectedValue.ToString())))
            {
                int anket_sayisi = gnlDB.OdemetipiSurveySayisiGetir(1);
                int olusturulan_anket_sayisi = ankDB.OlusturulanSurveySayisiGetirGrubaGore(Guid.Parse(ddlgrup.SelectedValue.ToString()));

                if (olusturulan_anket_sayisi >= anket_sayisi)
                {
                    this.ErrorMessage.Text = BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "202");
                    if (anket_uid==null || anket_uid==Guid.Empty) this.has_error.Value = "1";
                    SurveyGuncelle(anket_uid);
                    return;
                }
            }
            else
            {
                if (member_users != null)
                {
                    int kalan_anket_sayisi = 0;
                    if (member_users.kalan_anket_sayisi.HasValue)
                    {
                        kalan_anket_sayisi = Convert.ToInt32(member_users.kalan_anket_sayisi.ToString());
                    }

                    if (kalan_anket_sayisi <= 0)
                    {
                        this.ErrorMessage.Text = BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "202");
                        if (anket_uid == null || anket_uid == Guid.Empty) this.has_error.Value = "1";
                        SurveyGuncelle(anket_uid);
                        return;
                    }
                }
                else
                {
                    this.ErrorMessage.Text = BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "202");
                    if (anket_uid == null || anket_uid == Guid.Empty) this.has_error.Value = "1";
                    SurveyGuncelle(anket_uid);
                    return;
                }
            }

            
            int new_record = 0;
            
            sbr_anket anket = new sbr_anket();
            int anket_durumu_id = -1;
            if (anket_uid == null || anket_uid==Guid.Empty)
            {   
                ankDB.SurveyEkle(anket);
                new_record = 1;
                anket.anket_durumu_id = (int)BaseWebSite.Models.anket_durumu.Acik;
            }
            else
            {
                anket = ankDB.SurveyGetir(anket_uid);
                anket_durumu_id = Convert.ToInt32(anket.anket_durumu_id);
            }

            anket.anket_adi = this.txtSurveyAdi.Text;
            anket.anket_goruntulenen_ad_ekle = this.ChkGorunenAd.Checked;

            if (this.ChkGorunenAd.Checked)
                anket.anket_goruntulenen_ad = this.txtGorunenAd.Text;
            else
                anket.anket_goruntulenen_ad = "";

            anket.anket_mesaji_ekle = this.ChkSurveyMesajiEkle.Checked;

            if (this.ChkSurveyMesajiEkle.Checked)
                anket.anket_mesaji = this.txtSurveyMesaji.Text;
            else
                anket.anket_mesaji = "";

            anket.anket_sonuc_mesaji_ekle = this.chkSurveySonucMetni.Checked;

            if (this.chkSurveySonucMetni.Checked)
                anket.anket_sonuc_mesaji = this.txtSurveySonucMesaji.Text;
            else
                anket.anket_sonuc_mesaji = "";

            anket.baslangic_tarihi = Convert.ToDateTime(baslangic_tarihi.Value, new System.Globalization.CultureInfo("tr-TR"));
            anket.bitis_tarihi = Convert.ToDateTime(bitis_tarihi.Value, new System.Globalization.CultureInfo("tr-TR"));

            //anket.ip_kisitlamasi = this.chkIpRestriction.Checked;
            //anket.bitis_tarihinde_anketi_kaldir = this.chkBitisTarihindeSurveyiKapat.Checked;
            anket.ip_kisitlamasi = true;
            //anket.kategori_id = Convert.ToInt32(ddlKategori.SelectedValue);
            anket.anket_tipi_id = Convert.ToInt32(ddlTip.SelectedValue);
            anket.sayfadaki_soru_sayisi = Convert.ToInt32(ddlSoruSayisi.SelectedValue);
            //anket.soru_cevaplama_zorunlulugu = Convert.ToInt32(ddlCevaplamaTipi.SelectedValue);
            anket.olusturan_kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
            anket.olusturma_tarihi = DateTime.Now;
            anket.kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;


            if (gnlDB.HasUserGroup(BaseDB.SessionContext.Current.ActiveUser.UserUid))
                anket.grup_uid = Guid.Parse(ddlgrup.SelectedValue.ToString());
            else
                anket.grup_uid = Guid.Empty;


            if (rdSurveyTema1.Checked)
                anket.anket_tema_id = 1;
            else if (rdSurveyTema2.Checked)
                anket.anket_tema_id = 2;
            else if (rdSurveyTema3.Checked)
                anket.anket_tema_id = 3;
            
            ankDB.Kaydet();

            if (member_users != null)
            {
                if (new_record==1) member_users.kalan_anket_sayisi = member_users.kalan_anket_sayisi - 1;
                gnlDB.Kaydet();

                if (member_users.kalan_anket_sayisi == 0)
                {
                    string applicationPath = "";
                    if (System.Web.HttpContext.Current.Request.ApplicationPath == "/")
                        applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/";
                    else
                        applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/" + System.Web.HttpContext.Current.Request.ApplicationPath + "/";

                    if (new_record == 1) ankDB.anket_limit_bitis_maili_gonder(applicationPath, anket.anket_uid, "", Guid.Parse(member_users.user_uid.ToString()), member_users.email, member_users.ad, member_users.soyad);
                }
            }
            else
            {
                int anket_sayisi = gnlDB.OdemetipiSurveySayisiGetir(1);
                int olusturulan_anket_sayisi = ankDB.OlusturulanSurveySayisiGetirGrubaGore(Guid.Parse(ddlgrup.SelectedValue.ToString()));

                if (olusturulan_anket_sayisi - anket_sayisi == 0)
                {
                    string applicationPath = "";
                    if (System.Web.HttpContext.Current.Request.ApplicationPath == "/")
                        applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/";
                    else
                        applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/" + System.Web.HttpContext.Current.Request.ApplicationPath + "/";

                    if (new_record == 1) ankDB.anket_limit_bitis_maili_gonder(applicationPath, anket.anket_uid, "", BaseDB.SessionContext.Current.ActiveUser.UserUid, BaseDB.SessionContext.Current.ActiveUser.UserEmail, BaseDB.SessionContext.Current.ActiveUser.Name, BaseDB.SessionContext.Current.ActiveUser.Surname);
                }
            }

            if (anket_durumu_id == -1 || anket_durumu_id != anket.anket_durumu_id)
            {
                sbr_anket_durum_tarihcesi anket_durumu = new sbr_anket_durum_tarihcesi();

                ankDB.SurveyDurumuEkle(anket_durumu);
                anket_durumu.anket_durumu_id = anket.anket_durumu_id;
                anket_durumu.anket_uid = anket.anket_uid;
                anket_durumu.durum_olusma_tarihi = DateTime.Now;
                anket_durumu.durumu_olusturan_kullanici = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                ankDB.Kaydet();

                anket.anket_durumu_uid = anket_durumu.anket_durumu_uid;
                ankDB.Kaydet();
            }

            HttpPostedFile postedFile = this.fileupload1.PostedFile;
            if (postedFile != null && postedFile.ContentLength > 0)
            {
                string extension = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf(".") + 1);
                if (extension.ToLower() == "jpg" || extension.ToLower() == "png" || extension.ToLower() == "gif" || extension.ToLower() == "jpeg")
                {
                    FileInfo clientFileInfo = new FileInfo(postedFile.FileName);


                    SurveyBusiness.anket_business crm = new SurveyBusiness.anket_business();

                    byte[] dosya = new byte[postedFile.InputStream.Length];
                    postedFile.InputStream.Read(dosya, 0, (int)postedFile.InputStream.Length);

                    crm.SurveyLogoEkle(anket.anket_uid, dosya);
                }
            }
            Response.Redirect("AnketDetayi.aspx?anket_uid=" + anket.anket_uid);
        }


        protected void SurveyGuncelle(Guid anket_uid)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            if (anket_uid != null && anket_uid != Guid.Empty)
            {
                sbr_anket anket = new sbr_anket();
                anket = ankDB.SurveyGetir(anket_uid);
                anket.anket_adi = this.txtSurveyAdi.Text;
                anket.anket_goruntulenen_ad_ekle = this.ChkGorunenAd.Checked;

                if (this.ChkGorunenAd.Checked)
                    anket.anket_goruntulenen_ad = this.txtGorunenAd.Text;
                else
                    anket.anket_goruntulenen_ad = "";

                anket.anket_mesaji_ekle = this.ChkSurveyMesajiEkle.Checked;

                if (this.ChkSurveyMesajiEkle.Checked)
                    anket.anket_mesaji = this.txtSurveyMesaji.Text;
                else
                    anket.anket_mesaji = "";

                anket.anket_sonuc_mesaji_ekle = this.chkSurveySonucMetni.Checked;

                if (this.chkSurveySonucMetni.Checked)
                    anket.anket_sonuc_mesaji = this.txtSurveySonucMesaji.Text;
                else
                    anket.anket_sonuc_mesaji = "";

                anket.baslangic_tarihi = Convert.ToDateTime(baslangic_tarihi.Value, new System.Globalization.CultureInfo("tr-TR"));
                anket.bitis_tarihi = Convert.ToDateTime(bitis_tarihi.Value, new System.Globalization.CultureInfo("tr-TR"));

                anket.anket_tipi_id = Convert.ToInt32(ddlTip.SelectedValue);
                anket.sayfadaki_soru_sayisi = Convert.ToInt32(ddlSoruSayisi.SelectedValue);
                
                if (rdSurveyTema1.Checked)
                    anket.anket_tema_id = 1;
                else if (rdSurveyTema2.Checked)
                    anket.anket_tema_id = 2;
                else if (rdSurveyTema3.Checked)
                    anket.anket_tema_id = 3;

                ankDB.Kaydet();

            }
        }

        protected void btnLogoyuSil_Click(object sender, EventArgs e)
        {
            SurveyBusiness.anket_business crm = new SurveyBusiness.anket_business();
            crm.SurveyLogoyuSil(anket_uid);
            if (anket_uid != null && anket_uid != Guid.Empty && anket_uid.ToString() != "") BindControls();
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            sbr_anket anket = ankDB.SurveyGetir(anket_uid);

            Image1.Height = 0;
            Image1.Visible = false;
            this.image_div.Visible = false;
            
        }

       
    }
}