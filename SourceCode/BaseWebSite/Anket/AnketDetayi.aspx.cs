using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaseWebSite.Models;
using System.Data;
using System.Text;

namespace BaseWebSite.Survey
{
    public partial class AnketDetayi : System.Web.UI.Page
    {
        public Guid anket_uid
        {
            get { return (ViewState["anket_uid"] != null ? Guid.Parse(ViewState["anket_uid"].ToString()) : Guid.Empty); }
            set { ViewState["anket_uid"] = value; }
        }

        public int anket_yayinda
        {
            get { return (ViewState["anket_yayinda"] != null ? Convert.ToInt32(ViewState["anket_yayinda"].ToString()) : 0); }
            set { ViewState["anket_yayinda"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (BaseDB.SessionContext.Current == null || BaseDB.SessionContext.Current.ActiveUser == null)
            {
                Response.Redirect("~/Login.aspx");
            }

            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();


            if (!IsPostBack)
            {
                if (Request.QueryString["anket_uid"] != null && Request.QueryString["anket_uid"].ToString() != "")
                {
                    anket_uid = Guid.Parse(Request.QueryString["anket_uid"].ToString());
                }


                sbr_anket anket = ankDB.SurveyGetir(anket_uid);
                
                string ayrac = (anket.anket_adi.Length >= 35) ? "..." : "";
                this.LinkButtonHeader.Text = anket.anket_adi.Substring(0, (anket.anket_adi.Length < 35) ? anket.anket_adi.Length : 35) + ayrac + " / " + SurveyDurumu(anket.anket_durumu_id);
                BindCombos();
                this.ddlsoru_tipi.Attributes.Add("onchange", "ddlsoru_tipi_changed()");
                this.ddlKategori.Attributes.Add("onchange", "ddlkategori_changed()");

                BindSurveySorulari(anket_uid);
            }
                  
    

            if (Request["__EVENTTARGET"] == "Kapat")
            {
                string degerler = Request["__EVENTARGUMENT"].ToString();
                string[] arrDegerler = degerler.Replace("^#^", "^").Split('^');
                string anket_uid = arrDegerler[0];
                string aciklama = arrDegerler[1];

                ankDB.SurveyDurumuDegistir((int)BaseWebSite.Models.anket_durumu.Kapali, Guid.Parse(anket_uid), aciklama, BaseDB.SessionContext.Current.ActiveUser.UserUid);
                sbr_anket anket = ankDB.SurveyGetir(Guid.Parse(anket_uid));
                this.LinkButtonHeader.Text = anket.anket_adi + " / Survey Soruları - " + SurveyDurumu(anket.anket_durumu_id);
            }

            if (Request["__EVENTTARGET"] == "ArsiveGonder")
            {
                string degerler = Request["__EVENTARGUMENT"].ToString();
                string[] arrDegerler = degerler.Replace("^#^", "^").Split('^');
                string anket_uid = arrDegerler[0];
                string aciklama = arrDegerler[1];

                ankDB.SurveyDurumuDegistir((int)BaseWebSite.Models.anket_durumu.Arsivde, Guid.Parse(anket_uid), aciklama, BaseDB.SessionContext.Current.ActiveUser.UserUid);
                sbr_anket anket = ankDB.SurveyGetir(Guid.Parse(anket_uid));
                this.LinkButtonHeader.Text = anket.anket_adi + " / Survey Soruları - " + SurveyDurumu(anket.anket_durumu_id);
            }

            if (Request["__EVENTTARGET"] == "ArsivdenCikart")
            {
                string anket_uid = Request["__EVENTARGUMENT"].ToString();

                ankDB.SurveyDurumuDegistir((int)BaseWebSite.Models.anket_durumu.Acik, Guid.Parse(anket_uid), "Removed From Archive", BaseDB.SessionContext.Current.ActiveUser.UserUid);
                sbr_anket anket = ankDB.SurveyGetir(Guid.Parse(anket_uid));
                this.LinkButtonHeader.Text = anket.anket_adi + " / Survey Soruları - " + SurveyDurumu(anket.anket_durumu_id);
            }

            if (Request["__EVENTTARGET"] == "SoruOlustur")
            {
                string degerler = Request["__EVENTARGUMENT"].ToString();

                string[] arrDegerler = degerler.Replace("^#^", "^").Split('^');

                string soru_tipi = arrDegerler[0];
                string soru = arrDegerler[1];
                string soru_secenekleri = arrDegerler[2];
                string cevap_kolonları = arrDegerler[3];
                
                string soru_zorunlu = arrDegerler[4];
                string soru_sayisal_ondalik = arrDegerler[5];
                string soru_tek_satir = arrDegerler[6];

                ankDB.SoruOlustur(anket_uid, soru_tipi, soru, soru_secenekleri, cevap_kolonları,  soru_zorunlu, soru_sayisal_ondalik, soru_tek_satir);
                chkTekSatir.Checked = true;
                this.rdTamSayi.Checked = true;
            }

            if (Request["__EVENTTARGET"] == "SoruUpdate")
            {
                string degerler = Request["__EVENTARGUMENT"].ToString();

                string[] arrDegerler = degerler.Replace("^#^", "^").Split('^');

                string soru_uid = arrDegerler[0];
                string soru_tipi = arrDegerler[1];
                string soru = arrDegerler[2];
                string soru_secenekleri = arrDegerler[3];
                string cevap_kolonları = arrDegerler[4];
                string soru_zorunlu = arrDegerler[5];
                string soru_sayisal_ondalik = arrDegerler[6];
                string soru_tek_satir = arrDegerler[7];

                if (soru_uid!="")
                    ankDB.SoruUpdate(Guid.Parse(soru_uid), soru_tipi, soru, soru_secenekleri, cevap_kolonları, soru_zorunlu, soru_sayisal_ondalik,soru_tek_satir);
            }

            if (Request["__EVENTTARGET"] == "SoruSirala")
            {
                string degerler = Request["__EVENTARGUMENT"].ToString();
                string[] arrDegerler = degerler.Split(',');

                foreach (string value in arrDegerler)
                { 
                    if(value.Trim()!="")
                    {
                        string[] arrValue=value.Split('=');

                        string id = arrValue[0].Trim();
                        string sira = arrValue[1].Trim();

                        if (id != "" && sira != "")
                        {
                            Guid try_result = Guid.Empty;

                            if (Guid.TryParse(id, out try_result) == true)
                            {
                                sbr_anket_sorulari anket_soru = ankDB.SurveySoruGetir(try_result);
                                if (anket_soru != null)
                                {
                                    anket_soru.soru_sira_no = Convert.ToInt32(sira);
                                    ankDB.Kaydet();
                                }
                            }
                        }
                    }
                
                }


                
            }

            if (Request["__EVENTTARGET"] == "SablondanGetir")
            {
                string sablon_uid = Request["__EVENTARGUMENT"].ToString();
                ankDB.SablonSorulariniKaydet(Guid.Parse(sablon_uid),this.anket_uid);
            }

            if (Request["__EVENTTARGET"] == "TestiGonder")
            {
                string mailler = Request["__EVENTARGUMENT"].ToString();
                TestiGonder(mailler);
            }

            if (Request["__EVENTTARGET"] == "SurveytenGetir")
            {
                string anket_uid = Request["__EVENTARGUMENT"].ToString();
                ankDB.SurveytenGetirSorulariKaydet(Guid.Parse(anket_uid),this.anket_uid);
            }

            if (Request["__EVENTTARGET"] == "Duzenle")
            {
                string soru_uid = Request["__EVENTARGUMENT"].ToString();
                SoruBilgileriniHazirla(soru_uid);
            }

            if (Request["__EVENTTARGET"] == "Sil")
            {
                string soru_uid = Request["__EVENTARGUMENT"].ToString();
                SoruSil(soru_uid);
            }

            if (Request["__EVENTTARGET"] == "Sablonlar")
            {
                Response.Redirect("SablonTanimlari.aspx");
            }

            
            BindSurveySoruSirala();
            

            FormuDuzenle();
            ltlMenu.Text = CreateMenu();
        }

        protected new void Page_LoadComplete(object sender, EventArgs e)
        {
            BindSurveySorulari(anket_uid);
        }


        protected void FormuDuzenle()
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            sbr_anket anket = ankDB.SurveyGetir(anket_uid);

            if (ankDB.SurveyIcinHerhangiBirCevapVerilmisMi(anket_uid))
                this.hdn_ankete_cevap_verildimi.Value = "1";
            else
                this.hdn_ankete_cevap_verildimi.Value = "0";

            if (anket != null && anket.anket_durumu_id != null && anket.anket_durumu_id.ToString()!="")
            {
                anket_yayinda = Convert.ToInt32(anket.anket_durumu_id.ToString());
            }

            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();

            if (gnlDB.IsGrupUserAdmin(Guid.Parse(anket.grup_uid.ToString()), BaseDB.SessionContext.Current.ActiveUser.UserUid))
            {
                this.hdn_is_grup_admin.Value = "1";
            }
        }

        protected void BindCombos()
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            this.ddlsoru_tipi.DataSource = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_ref_soru_tipi");
            this.ddlsoru_tipi.DataValueField = "soru_tipi_id";
            this.ddlsoru_tipi.DataTextField = "soru_tipi_kodu";
            this.ddlsoru_tipi.DataBind();
            this.ddlsoru_tipi.SelectedValue = "1";

            this.ddlSoruTipi_Duzenle.DataSource = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_ref_soru_tipi");
            this.ddlSoruTipi_Duzenle.DataValueField = "soru_tipi_id";
            this.ddlSoruTipi_Duzenle.DataTextField = "soru_tipi_kodu";
            this.ddlSoruTipi_Duzenle.DataBind();
            this.ddlSoruTipi_Duzenle.SelectedValue = "1";

            this.ddlKategori.DataSource = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_ref_anket_kategori");
            this.ddlKategori.DataTextField = "kategori_kodu";
            this.ddlKategori.DataValueField = "kategori_id";
            this.ddlKategori.DataBind();

            this.ddlSablonlar.DataSource = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_sablon where sablon_durumu_id=1 and kategori_id="+ddlKategori.SelectedValue);
            this.ddlSablonlar.DataValueField = "sablon_uid";
            this.ddlSablonlar.DataTextField = "sablon_adi";
            this.ddlSablonlar.DataBind();


            DataSet ds1 = BaseDB.DBManager.AppConnection.GetDataSet("select * from dbo.sbr_anket_kullanici_gruplari('" + BaseDB.SessionContext.Current.ActiveUser.UserUid + "') ");
            int i=0;
            string grp="";
            foreach (DataRow dr in ds1.Tables[0].Rows)
            {
                if (i == (ds1.Tables[0].Rows.Count - 1))
                {
                    grp += "'" + dr["group_uid"].ToString() + "'";
                }
                else
                {
                    grp += "'" + dr["group_uid"].ToString() + "'" + ",";
                }
                i++;
            }

            DataSet ds = ankDB.SurveyGetirDurumaGoreDataSet(0, "", grp);

            this.ddlSurveyler.DataSource = ds;
            this.ddlSurveyler.DataValueField = "anket_uid";
            this.ddlSurveyler.DataTextField = "anket_adi";
            this.ddlSurveyler.DataBind();
        }


        protected void BindSurveySorulari(Guid anket_uid)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            DataSet ds = ankDB.SurveySoruGetirSurveyGoreDataSet(anket_uid);

            this.rptSurveySorulari.DataSource = ds;
            this.rptSurveySorulari.DataBind();
        }

        protected void BindSurveySoruSirala()
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            DataSet ds = ankDB.SurveySoruGetirSurveyGoreDataSet(anket_uid);

            this.rptSira.DataSource = ds;
            this.rptSira.DataBind();
        }

        protected string CreateMenu()
        {

            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            sbr_anket anket = ankDB.SurveyGetir(anket_uid);
            int anket_durumu_id = 0;
            bool is_grup_admin=false;
 
            if (anket != null)
            {
                if (anket.anket_durumu_id != null)
                {
                    anket_durumu_id = Convert.ToInt32(anket.anket_durumu_id.ToString());
                }
            }

            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();

            if (gnlDB.IsGrupUserAdmin(Guid.Parse(anket.grup_uid.ToString()), BaseDB.SessionContext.Current.ActiveUser.UserUid))
            {
                is_grup_admin = true;
            }

            StringBuilder menu_output = new StringBuilder();

            menu_output.Append("<ul class=\"mainMenu\" >");
            menu_output.Append("<li ><a href=\"MainPage.aspx\" >BACK</a></li>");


            //if (is_grup_admin == true)
            {
                menu_output.Append("<li ><a href=\"#\">SURVEY PROCEDURES</a>");
                menu_output.Append("<ul >");
                menu_output.Append("<li class=\"first\"><a href=\"YeniAnket.aspx?anket_uid=" + anket_uid + "\">SETTINGS</a></li>");
                if (anket_durumu_id == 1 || anket_durumu_id == 4)
                {
                    menu_output.Append("<li ><a href=\"#\" onclick=\"SurveyiKapat();return false;\" >CLOSE SURVEY</a></li>");
                }
                if (anket_durumu_id == 2 || anket_durumu_id == 1 || anket_durumu_id == 4)
                {
                    menu_output.Append("<li ><a href=\"#\" onclick=\"ArsiveGonder();return false;\">SEND TO ARCHIVE</a></li>");
                }
                if (anket_durumu_id == 3)
                {
                    menu_output.Append("<li ><a href=\"#\" onclick=\"ArsivdenCikart();return false;\">REMOVE FROM ARCHIVE</a></li>");
                }
                //menu_output.Append("<li ><a href=\"#\" onclick=\"SablondanGetir();return false;\">GET FROM TEMPLATE</a></li>");
                menu_output.Append("<li class=\"last\"  onclick=\"SurveytenKopyala();return false;\"><a href=\"#\">COPY FROM SURVEY</a></li>");
                menu_output.Append("</ul>");
                menu_output.Append("</li>");

                //menu_output.Append("<li ><a href=\"#\">YAYIN ÖNCESİ</a>");
                //menu_output.Append("<ul >");
                //menu_output.Append("<li class=\"first\"><a href=\"#\" onclick=\"TestiGonder();return false;\">BAŞLAMADAN GÖRÜŞ AL</a></li>");
                //menu_output.Append("<li class=\"last\"><a href=\"#\" onclick=\"TestSonuclari();return false;\">DEĞERLENDİRME SONUCU</a></li>");
                //menu_output.Append("</ul>");
                //menu_output.Append("</li>");
            }

            menu_output.Append("<li ><a href=\"#\" onclick=\"OnIzleme();return false;\">DISPLAY SURVEY</a></li>");
            //if (is_grup_admin == true)
            {
                menu_output.Append("<li ><a href=\"AnketYayinla.aspx?anket_uid=" + anket_uid + "\">PUBLISH</a></li>");
                menu_output.Append("<li ><a href=\"#\" onclick=\"SoruEkleme();return false;\">ADD QUESTION</a></li></li>");
                menu_output.Append("<li ><a href=\"#\" onclick=\"SoruSirala();return false;\">RANK QUESTIONS</a>");
            }
            menu_output.Append("</ul>");
            return menu_output.ToString();
        }

        protected string SorulariOlustur(object soru_uid)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            return ankDB.SoruGorunumuOlustur(soru_uid);
        }

        public string SurveyDurumu(object anket_durum_tipi_id)
        {
            string result = "";

            if (anket_durum_tipi_id != null && anket_durum_tipi_id.ToString() == ((int)BaseWebSite.Models.anket_durumu.Acik).ToString())
            {
                result = "Open";
            }
            else if (anket_durum_tipi_id != null && anket_durum_tipi_id.ToString() == ((int)BaseWebSite.Models.anket_durumu.Kapali).ToString())
            {
                result = "Closed";
            }
            else if (anket_durum_tipi_id != null && anket_durum_tipi_id.ToString() == ((int)BaseWebSite.Models.anket_durumu.Arsivde).ToString())
            {
                result = "Archived";
            }
            else if (anket_durum_tipi_id != null && anket_durum_tipi_id.ToString() == ((int)BaseWebSite.Models.anket_durumu.Yayinda).ToString())
            {
                result = "Live";
            }

            return result;
        }

        protected void SoruBilgileriniHazirla(string soru_uid)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            if(soru_uid=="") return;
            
            sbr_anket_sorulari soru=ankDB.SurveySoruGetir(Guid.Parse(soru_uid));

            if (soru.soru_tipi_id != null) 
            {
                this.ddlSoruTipi_Duzenle.SelectedValue = soru.soru_tipi_id.ToString();
                if (soru.soru_zorunlu != null && soru.soru_zorunlu == true) 
                    this.chkZorunlu_duzenle.Checked = true;
                else
                    this.chkZorunlu_duzenle.Checked = false;

                if(soru.soru_tipi_id==1)
                {
                    DataSet secenek = ankDB.AcikAnketSoruTipi1SecenekGetirSoruyaGoreDataSet(Guid.Parse(soru_uid),anket_uid);

                    foreach (DataRow dr in secenek.Tables[0].Rows)
                    {
                        if (dr["soru_secenek_ad"] != System.DBNull.Value) this.txtsoru_secenekleri_duzenle.Text += dr["soru_secenek_ad"].ToString() + "\r\n";
                    }

                    
                }
                else if (soru.soru_tipi_id == 2)
                {
                    DataSet secenek = ankDB.AcikAnketSoruTipi2SecenekGetirSoruyaGoreDataSet(Guid.Parse(soru_uid));

                    foreach (DataRow dr in secenek.Tables[0].Rows)
                    {
                        if (dr["soru_secenek_ad"] != System.DBNull.Value) this.txtsoru_secenekleri_duzenle.Text += dr["soru_secenek_ad"].ToString() + "\r\n";
                    }
                }
                else if (soru.soru_tipi_id == 7)
                {
                    DataSet secenek = ankDB.AcikAnketSoruTipi7SecenekGetirSoruyaGoreDataSet(Guid.Parse(soru_uid));

                    foreach (DataRow dr in secenek.Tables[0].Rows)
                    {
                        if (dr["soru_secenek_ad"] != System.DBNull.Value) this.txtsoru_secenekleri_duzenle.Text += dr["soru_secenek_ad"].ToString() + "\r\n";
                    }
                }
                else if (soru.soru_tipi_id == 8)
                {
                    if (soru.soru_sayisal_ondalik != null && soru.soru_sayisal_ondalik == true)
                        this.rdOndalik_duzenle.Checked = true;
                    else
                        this.rdTamSayi_duzenle.Checked = true;
                }
                else if (soru.soru_tipi_id == 4)
                {
                    if (soru.soru_tek_satir != null && soru.soru_tek_satir== true)
                        this.chkTekSatir_duzenle.Checked = true;
                    else
                        this.chkTekSatir_duzenle.Checked = false;
                }
                else if (soru.soru_tipi_id == 3)
                {
                    DataSet secenek = ankDB.AcikAnketSoruTipi3SecenekGetirSoruyaGoreDataSet(Guid.Parse(soru_uid));

                    foreach (DataRow dr in secenek.Tables[0].Rows)
                    {
                        if (dr["soru_secenek_ad"] != System.DBNull.Value) this.txtsoru_secenekleri_duzenle.Text += dr["soru_secenek_ad"].ToString() + "\r\n";
                    }

                    DataSet kolon = ankDB.AcikAnketSoruTipi3SecenekKolonGetirSoruyaGoreDataSet(Guid.Parse(soru_uid));

                    foreach (DataRow dr in kolon.Tables[0].Rows)
                    {
                        if (dr["soru_secenek_kolon_ad"] != System.DBNull.Value) this.txtcevap_kolonları_duzenle.Text += dr["soru_secenek_kolon_ad"].ToString() + "\r\n";
                    }
                }
            }

            if (soru.soru != null) this.txtsoru_duzenle.Text = soru.soru;
            
            ClientScript.RegisterStartupScript(this.GetType(), "Redirect1", "<script>SoruDuzenle('" + soru_uid + "')</script>");
        }

        protected void SoruSil(string soru_uid)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            ankDB.SurveySoruSil(Guid.Parse(soru_uid));

            ankDB.AcikAnketSoruTipi1SecenekSilSoruyaGore(Guid.Parse(soru_uid));
            ankDB.AcikAnketSoruTipi2SecenekSilSoruyaGore(Guid.Parse(soru_uid));
            ankDB.AcikAnketSoruTipi3SecenekSilSoruyaGore(Guid.Parse(soru_uid));
            ankDB.AcikAnketSoruTipi3SecenekKolonSilSoruyaGore(Guid.Parse(soru_uid));
            ankDB.AcikAnketSoruTipi4SecenekSilSoruyaGore(Guid.Parse(soru_uid));
            ankDB.AcikAnketSoruTipi5SecenekSilSoruyaGore(Guid.Parse(soru_uid));
            ankDB.AcikAnketSoruTipi6SecenekSilSoruyaGore(Guid.Parse(soru_uid));
            ankDB.AcikAnketSoruTipi7SecenekSilSoruyaGore(Guid.Parse(soru_uid));

            ankDB.Kaydet();
        }

        protected void TestiGonder(string mailler)
        {
            
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            #region Mailler Gönderiliyor
            sbr_anket anket = ankDB.SurveyGetir(anket_uid);

            string anket_adi = "";

            if (anket.anket_goruntulenen_ad_ekle != null && anket.anket_goruntulenen_ad_ekle == true)
                anket_adi = anket.anket_goruntulenen_ad;
            else
                anket_adi = anket.anket_adi;

            string applicationPath = "";
            if (System.Web.HttpContext.Current.Request.ApplicationPath == "/")
                applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/";
            else
                applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/" + System.Web.HttpContext.Current.Request.ApplicationPath + "/";

            bool herhangi_mail_gonderildi = ankDB.anket_test_maili_gonder(mailler, applicationPath,anket_uid, anket_adi+" - Yayın Öncesi Görüşler");
            #endregion

            if (herhangi_mail_gonderildi)
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "Mesaj", "alert('Yayın Öncesi Survey Değerlendirme Gönderme İşlemi Gerçekleştirilmiştir.');", true);
            }
            else
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "Mesaj", "alert('Yayın Öncesi Survey Değerlendirme Gönderilemedi.Lütfen E-Posta adreslerini kontrol ediniz.');", true);
            }
        }

       

    }
}