using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaseWebSite.Models;
using System.Data;
using System.Net;
using System.Xml;
using System.Threading;

namespace BaseWebSite
{
    public partial class PurchaseApprove : System.Web.UI.Page
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

            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();

            if (!Page.IsPostBack)
            {
                if (Request.QueryString["odeme_tipi_id"] != null && Request.QueryString["odeme_tipi_id"].ToString() != "")
                {
                    odeme_tipi_id = Convert.ToInt32(Request.QueryString["odeme_tipi_id"].ToString());
                }

                BindControls();
                this.UyelikDurumu.Text = gnlDB.UyelikDurumuTextBelirle(odeme_tipi_id, BaseDB.SessionContext.Current.ActiveUser.UserUid);

                gnl_uyelik_odeme_tipleri_tanimlari uyelik_odeme_tipleri = gnlDB.UyelikOdemeTipiGetir(odeme_tipi_id);
                decimal anket_ucret = Convert.ToDecimal(uyelik_odeme_tipleri.ucret.ToString());
                this.Ucret.Text = anket_ucret.ToString();
            }

            if (rdHavale.Checked)
            {
                div_kk.Visible = false;
                div_havale.Visible = true;
                this.UyariOdemeTipi.Text = "Banka Havalesi İle Ödeme Tipini Seçtiniz.";
            }
            else if (rdKrediKarti.Checked)
            {
                this.div_kk.Visible = true;
                div_havale.Visible = false;
                this.UyariOdemeTipi.Text = "Kredi Kart İle Ödeme Tipini Seçtiniz.";
            }

        }


        protected void BindControls()
        {
            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();
            gnl_uye_kullanicilar member_users = gnlDB.GetMemberUsers(BaseDB.SessionContext.Current.ActiveUser.UserUid);
            gnl_users kullanicilar = gnlDB.GetUsers(BaseDB.SessionContext.Current.ActiveUser.UserUid);

            if (member_users != null)
            {
                this.chkSirket.Disabled = true;
                if (member_users.ad != null) this.Name.Text = member_users.ad;
                if (member_users.soyad != null) this.Surname.Text = member_users.soyad;
                if (member_users.adres != null) this.Adres.Text = member_users.adres;
                
                if (member_users.sirket_adi != null) this.SirketName.Text = member_users.sirket_adi;

                if (member_users.vergi_dairesi != null) this.VergiDairesi.Text = member_users.vergi_dairesi;
                
                if (member_users.vergi_no != null) this.VergiNo.Text = member_users.vergi_no;
                if (member_users.telefonu != null) this.Telefon.Text = member_users.telefonu;
                if (member_users.cep_telefonu != null) this.CepTelefonu.Text = member_users.cep_telefonu;

                if (member_users.grup_sirket_tipi_id != null)
                {
                    if (member_users.grup_sirket_tipi_id == 1)
                        this.chkSirket.Checked = true;
                }
            }
        }

        protected void PurchaseButton_Click(object sender, EventArgs e)
        {
            this.ErrorMessages.Text = "";
            this.has_error.Value = "0";

            if (!chkKabul.Checked)
            {
                this.ErrorMessages.Text = "Please specify the Membership Terms and Survey Responsibilities by checking the box.";
                this.has_error.Value = "1";
                return;
            }

            if (Adres.Text.Trim() == "")
            {
                this.ErrorMessages.Text = "Lütfen Adres,Şehir Bilgilerini Boş Geçmeyiniz.";
                this.has_error.Value = "1";
                return;
            }

            if (rdKrediKarti.Checked)
            {
                if (txt_kredi_karti.Text.Trim() == "" || txt_cvv.Text.Trim() == "")
                {
                    this.ErrorMessages.Text = "Lütfen Kredi Kartı , Kart Sahibi ve CVV Bilgilerini Boş Geçmeyiniz.";
                    this.has_error.Value = "1";
                    return;
                }

                if (!BaseClasses.BaseFunctions.getInstance().IsNumeric(txt_kredi_karti.Text) || !BaseClasses.BaseFunctions.getInstance().IsNumeric(txt_cvv.Text))
                {
                    this.ErrorMessages.Text = "Lütfen Kredi Kartı ve CVV Bilgilerine Sayısal Değer Giriniz.";
                    this.has_error.Value = "1";
                    return;
                }




                if (txt_kredi_karti.Text.Trim().Length != 16 || txt_cvv.Text.Trim().Length != 3)
                {
                    this.ErrorMessages.Text = "Lütfen Kredi Kartı (16) ve CVV (3) Bilgilerine Belirtilen Uzunlukta Karakter Giriniz.";
                    this.has_error.Value = "1";
                    return;
                }
            }

            //if (!ASPxCaptcha1.IsValid)
            //{
            //    this.ErrorMessages.Text = "Doğrulama Kodunu yanlış girdiniz.Lütfen tekrar giriniz.";
            //    this.has_error.Value = "1";
            //    return;
            //}



            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();
            gnl_uye_kullanicilar member_users = gnlDB.GetMemberUsers(BaseDB.SessionContext.Current.ActiveUser.UserUid);

            if (member_users != null)
            {
                string islem_no = "";

                try
                {
                    gnl_uyelik_paket_alimlari paket_alimi = new gnl_uyelik_paket_alimlari();
                    gnlDB.UyePaketAlimiEkle(paket_alimi);

                    gnl_uyelik_odeme_tipleri_tanimlari uyelik_odeme_tipleri = gnlDB.UyelikOdemeTipiGetir(odeme_tipi_id);
                    decimal anket_ucret = Convert.ToDecimal(uyelik_odeme_tipleri.ucret.ToString());
                    int uyelik_sure = Convert.ToInt32(uyelik_odeme_tipleri.sure_ay.ToString());
                    int anket_sayisi = Convert.ToInt32(uyelik_odeme_tipleri.anket_sayisi.ToString());
                    int katilimci_sayisi = Convert.ToInt32(uyelik_odeme_tipleri.katilimci_sayisi.ToString());

                    paket_alimi.anket_basina_katilimci_sayisi = katilimci_sayisi;
                    paket_alimi.anket_sayisi = anket_sayisi;
                    paket_alimi.grup_uid = member_users.grup_uid;
                    paket_alimi.odeme_tipi_id = odeme_tipi_id;
                    paket_alimi.paket_alim_tarihi = DateTime.Now;
                    paket_alimi.paket_fiyati = anket_ucret;
                    paket_alimi.paket_suresi = uyelik_sure;
                    paket_alimi.user_uid = member_users.user_uid;
                    paket_alimi.adres = Adres.Text;
                    paket_alimi.cep_telefonu = member_users.cep_telefonu;
                    paket_alimi.telefonu = member_users.telefonu;
                    paket_alimi.grup_sirket_tipi_id = member_users.grup_sirket_tipi_id;
                    if (chkKabul.Checked)
                    {
                        paket_alimi.kabul_edildi = true;

                        System.IO.StreamReader template1 = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"Anket\Templates\UyelikKosullariTemplate.html");
                        string Body1 = template1.ReadToEnd();
                        template1.Close();

                        paket_alimi.UyelikKosullari = Body1;

                        System.IO.StreamReader template2 = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"Anket\Templates\Anket123Sorumluluklari.html");
                        string Body2 = template2.ReadToEnd();
                        template2.Close();

                        paket_alimi.SurveyResponsibilities = Body2;

                    }
                    else
                    {
                        paket_alimi.kabul_edildi = false;
                        paket_alimi.UyelikKosullari = null;
                        paket_alimi.SurveyResponsibilities = null;
                    }

                    if (rdHavale.Checked)
                        paket_alimi.odeme_sekli_id = 1; 
                    else if (rdKrediKarti.Checked)
                        paket_alimi.odeme_sekli_id = 2;

                    if (member_users.grup_sirket_tipi_id != null && member_users.grup_sirket_tipi_id == 1)
                    {
                        paket_alimi.vergi_dairesi = member_users.vergi_dairesi;
                        paket_alimi.vergi_no = member_users.vergi_no;
                        paket_alimi.sirket_adi = member_users.sirket_adi;
                    }


                    DateTime dt = DateTime.Now.AddMonths(uyelik_sure);
                    int kalan_anket_sayisi = 0;

                    dt = DateTime.Now.AddMonths(uyelik_sure).Date;
                    kalan_anket_sayisi = (member_users.kalan_anket_sayisi.HasValue && (gnlDB.IsPurchasedUser(BaseDB.SessionContext.Current.ActiveUser.UserUid))) ? Convert.ToInt32(member_users.kalan_anket_sayisi) + ((gnlDB.IsPurchasedUser(BaseDB.SessionContext.Current.ActiveUser.UserUid)) ? anket_sayisi : 0) : anket_sayisi;

                    paket_alimi.uyelik_bitis_tarihi = dt;

                    var amount = anket_ucret;
                    var description = "Survey Uyelik Suresi : " + uyelik_sure.ToString() + " , Survey Sayısı : " + anket_sayisi.ToString() + " , Survey Katılımcı Sayısı : " + katilimci_sayisi.ToString();

                    //Banka havalesi için yapıldı.Kredi Kartı entegrasyonunda değiştirilecek

                    if (rdHavale.Checked)
                    {
                        try
                        {
                            gnlDB.Kaydet();
                            DataSet ds_result_havale = new DataSet();

                            ds_result_havale = BaseDB.DBManager.AppConnection.GetDataSet("select * from gnl_paket_alimlari_v where id='" + paket_alimi.id + "'");
                            string paket_durumu = "Paket Tipi : ";
                            if (ds_result_havale.Tables[0].Rows.Count > 0)
                            {

                                if (ds_result_havale.Tables[0].Rows[0]["anket_sayisi"] != System.DBNull.Value)
                                    paket_durumu += ds_result_havale.Tables[0].Rows[0]["anket_sayisi"].ToString()+" Survey ";

                                if (ds_result_havale.Tables[0].Rows[0]["odeme_tipi"] != System.DBNull.Value)
                                    paket_durumu += " - " + ds_result_havale.Tables[0].Rows[0]["odeme_tipi"].ToString();

                                if (ds_result_havale.Tables[0].Rows[0]["odeme_sekli"] != System.DBNull.Value)
                                    paket_durumu += " - " + ds_result_havale.Tables[0].Rows[0]["odeme_sekli"].ToString();

                                if (ds_result_havale.Tables[0].Rows[0]["paket_fiyati_str"] != System.DBNull.Value)
                                    paket_durumu += " - " + ds_result_havale.Tables[0].Rows[0]["paket_fiyati_str"].ToString();
                            }

                            string applicationPath = "";
                            if (System.Web.HttpContext.Current.Request.ApplicationPath == "/")
                                applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/";
                            else
                                applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/" + System.Web.HttpContext.Current.Request.ApplicationPath + "/";

                            gnlDB.banka_havalesi_onay_maili_gonder(applicationPath, Guid.Parse(member_users.user_uid.ToString()), member_users.email, member_users.ad, member_users.soyad, paket_durumu);
                            Response.Redirect("RegisterationInfoPage.aspx?tip=6", false);
                        }
                        catch (Exception exp)
                        {
                            Response.Redirect("RegisterationInfoPage.aspx?tip=5&state=BNK_HVL_" + exp.Message,false);
                        }
                    }
                    else if(rdKrediKarti.Checked)
                    {
                        //var dictionary = new Dictionary<string,string>();
                        //string sid = System.Configuration.ConfigurationSettings.AppSettings["PurchaseSID"].ToString();
                        //dictionary.Add("sid", sid);
                        //dictionary.Add("mode", "2CO");
                        //dictionary.Add("li_0_type", "product");
                        //dictionary.Add("li_0_name", "Survey " + anket_sayisi.ToString());
                        //dictionary.Add("li_0_quantity", "1");
                        //dictionary.Add("li_0_price", amount.ToString().Replace(",", "."));
                        //dictionary.Add("li_0_tangible", "N");
                        //string url = gnlDB.PurchaseLink(dictionary);
                        //HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                        //HttpWebResponse response = (HttpWebResponse)request.GetResponse();    
                        //XmlDocument doc = new XmlDocument();
                        //doc.Load(response.GetResponseStream());
                        //XmlNode kodNode = doc.SelectSingleNode("//Cevap/Msg/Kod");
                        //XmlNode statusNode = doc.SelectSingleNode("//Cevap/Msg/Status");
                        //XmlNode provNo = doc.SelectSingleNode("//Cevap/Msg/ProvNo");
                        //XmlNode mesajNo = doc.SelectSingleNode("//Cevap/Msg/Mesaj");

                        //if (pymnt != null)
                        //{
                        //    var pymntID = pymnt.id;
                        //    var state = pymnt.state;

                        //        if (state.Trim().ToLower().Equals("approved"))
                        //        {
                        //            if (rdKrediKarti.Checked) paket_alimi.aktive_edildi = true;
                        //            gnlDB.Kaydet();

                        //            try
                        //            {
                        //                if (rdKrediKarti.Checked)
                        //                {
                        //                    uye_kullanicilar.uye_bitis_tarihi = dt;
                        //                    uye_kullanicilar.uye_baslangic_tarihi = (uye_kullanicilar.uye_baslangic_tarihi.HasValue) ? Convert.ToDateTime(uye_kullanicilar.uye_baslangic_tarihi.ToString(), new System.Globalization.CultureInfo("tr-TR")) : DateTime.Now;
                        //                    uye_kullanicilar.son_odeme_tipi_id = odeme_tipi_id;
                        //                    uye_kullanicilar.kalan_anket_sayisi = kalan_anket_sayisi;
                        //                    uye_kullanicilar.aktif = true;
                        //                    gnlDB.Kaydet();

                        //                    DataSet ds_result_havale = new DataSet();

                        //                    ds_result_havale = BaseDB.DBManager.AppConnection.GetDataSet("select * from gnl_paket_alimlari_v where id='" + paket_alimi.id + "'");
                        //                    string paket_durumu = "Paket Tipi : ";
                        //                    if (ds_result_havale.Tables[0].Rows.Count > 0)
                        //                    {
                        //                        if (ds_result_havale.Tables[0].Rows[0]["odeme_tipi"] != System.DBNull.Value)
                        //                            paket_durumu += ds_result_havale.Tables[0].Rows[0]["odeme_tipi"].ToString();

                        //                        if (ds_result_havale.Tables[0].Rows[0]["odeme_sekli"] != System.DBNull.Value)
                        //                            paket_durumu += " - " + ds_result_havale.Tables[0].Rows[0]["odeme_sekli"].ToString();

                        //                        if (ds_result_havale.Tables[0].Rows[0]["paket_fiyati_str"] != System.DBNull.Value)
                        //                            paket_durumu += " - " + ds_result_havale.Tables[0].Rows[0]["paket_fiyati_str"].ToString();

                        //                        if (ds_result_havale.Tables[0].Rows[0]["islem_id"] != System.DBNull.Value)
                        //                        {
                        //                            paket_durumu += " - İşlem No : " + ds_result_havale.Tables[0].Rows[0]["islem_id"].ToString();
                        //                            islem_no = ds_result_havale.Tables[0].Rows[0]["islem_id"].ToString();
                        //                        }
                        //                    }

                        //                    string applicationPath = "";
                        //                    if (System.Web.HttpContext.Current.Request.ApplicationPath == "/")
                        //                        applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/";
                        //                    else
                        //                        applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/" + System.Web.HttpContext.Current.Request.ApplicationPath + "/";

                        //                    gnlDB.kredi_karti_onay_maili_gonder(applicationPath, Guid.Parse(uye_kullanicilar.user_uid.ToString()), uye_kullanicilar.email, uye_kullanicilar.ad, uye_kullanicilar.soyad, paket_durumu);
                        //                }
                        //            }
                        //            catch (Exception ex)
                        //            {

                        //            }
                        //            finally 
                        //            {
                        //                Response.Redirect("RegisterationInfoPage.aspx?tip=4&islem_no=" + islem_no);   
                        //            }
                        //        }
                        //        else
                        //        {
                        //            Response.Redirect("RegisterationInfoPage.aspx?tip=5&state=" + state);
                        //        }

                        //}
                        //else
                        //{
                        //    Response.Redirect("RegisterationInfoPage.aspx?tip=5");
                        //}
                    }

                }
                catch (Exception exp)
                {
                    Response.Redirect("RegisterationInfoPage.aspx?tip=5&state=BNK_PRCH_"+exp.Message);
                }
            }
            else
            {
                Response.Redirect("RegisterationInfoPage.aspx?tip=5&state=ÜyeBulunamadı");
            }

        }
      
       
    }
}