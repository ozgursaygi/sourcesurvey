using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaseWebSite.Models;
using System.Data;
using System.Text;

namespace BaseWebSite.Admin
{
    public partial class SablonDetayi : System.Web.UI.Page
    {
        public Guid sablon_uid
        {
            get { return (ViewState["sablon_uid"] != null ? Guid.Parse(ViewState["sablon_uid"].ToString()) : Guid.Empty); }
            set { ViewState["sablon_uid"] = value; }
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
                if (Request.QueryString["sablon_uid"] != null && Request.QueryString["sablon_uid"].ToString() != "")
                {
                    sablon_uid = Guid.Parse(Request.QueryString["sablon_uid"].ToString());
                }


                sbr_sablon sablon = ankDB.SablonGetir(sablon_uid);
                string ayrac = (sablon.sablon_adi.Length >= 40) ? "..." : "";
                this.LinkButtonHeader.Text = sablon.sablon_adi.Substring(0, (sablon.sablon_adi.Length < 40) ? sablon.sablon_adi.Length : 40) + ayrac + " / " + SablonDurumu(sablon.sablon_durumu_id);

                BindCombos();
                this.ddlsoru_tipi.Attributes.Add("onchange", "ddlsoru_tipi_changed()");

                BindSablonSorulari(sablon_uid);
            }

         

            if (Request["__EVENTTARGET"] == "Sablonlar")
            {
                Response.Redirect("SablonTanimlari.aspx");
            }

            if (Request["__EVENTTARGET"] == "Kapat")
            {
                string degerler = Request["__EVENTARGUMENT"].ToString();
                string[] arrDegerler = degerler.Replace("^#^", "^").Split('^');
                string sablonuid = arrDegerler[0];
                string aciklama = arrDegerler[1];

                ankDB.SablonDurumuDegistir((int)BaseWebSite.Models.sablon_durumu.Kapali, Guid.Parse(sablonuid), aciklama);
                sbr_sablon sablon1 = ankDB.SablonGetir(Guid.Parse(sablonuid));
                this.LinkButtonHeader.Text = sablon1.sablon_adi + " / Şablon Soruları - " + SablonDurumu(sablon1.sablon_durumu_id);
            }

            if (Request["__EVENTTARGET"] == "SablonuAc")
            {
               
                ankDB.SablonDurumuDegistir((int)BaseWebSite.Models.sablon_durumu.Acik, sablon_uid, "");
                sbr_sablon sablon1 = ankDB.SablonGetir(sablon_uid);
                this.LinkButtonHeader.Text = sablon1.sablon_adi + " / Şablon Soruları - " + SablonDurumu(sablon1.sablon_durumu_id);
            }

            if (Request["__EVENTTARGET"] == "SoruOlustur")
            {
                string degerler = Request["__EVENTARGUMENT"].ToString();

                string[] arrDegerler = degerler.Replace("^#^", "^").Split('^');

                string soru_tipi = arrDegerler[0];
                string soru = arrDegerler[1];
                string soru_secenekleri = arrDegerler[2];
                string cevap_kolonları = arrDegerler[3];
                string soru_sayisal_ondalik = arrDegerler[4];
                string soru_tek_satir = arrDegerler[5];

                ankDB.SablonSoruOlustur(sablon_uid, soru_tipi, soru, soru_secenekleri, cevap_kolonları, soru_sayisal_ondalik, soru_tek_satir);
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
                string soru_sayisal_ondalik = arrDegerler[5];
                string soru_tek_satir = arrDegerler[6];

                if (soru_uid != "")
                    ankDB.SablonSoruUpdate(Guid.Parse(soru_uid), soru_tipi, soru, soru_secenekleri, cevap_kolonları,soru_sayisal_ondalik,soru_tek_satir);
            }

            if (Request["__EVENTTARGET"] == "SoruSirala")
            {
                string degerler = Request["__EVENTARGUMENT"].ToString();
                string[] arrDegerler = degerler.Split(',');

                foreach (string value in arrDegerler)
                {
                    if (value.Trim() != "")
                    {
                        string[] arrValue = value.Split('=');

                        string id = arrValue[0].Trim();
                        string sira = arrValue[1].Trim();

                        if (id != "" && sira != "")
                        {
                            Guid try_result = Guid.Empty;

                            if (Guid.TryParse(id, out try_result) == true)
                            {
                                sbr_sablon_sorulari anket_soru = ankDB.SablonSoruGetir(try_result);
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


            BindSablonSoruSirala();


            FormuDuzenle();
            ltlMenu.Text = CreateMenu();

        }

        protected new void Page_LoadComplete(object sender, EventArgs e)
        {
            BindSablonSorulari(sablon_uid);
        }


        protected void FormuDuzenle()
        {
            
        }

        protected void BindCombos()
        {
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
        }


        protected void BindSablonSorulari(Guid sablon_uid)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            DataSet ds = ankDB.SablonSoruGetirSablonGoreDataSet(sablon_uid);

            this.rptSablonSorulari.DataSource = ds;
            this.rptSablonSorulari.DataBind();
        }

        protected void BindSablonSoruSirala()
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            DataSet ds = ankDB.SablonSoruGetirSablonGoreDataSet(sablon_uid);

            this.rptSira.DataSource = ds;
            this.rptSira.DataBind();
        }


        protected string SorulariOlustur(object soru_uid)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            return ankDB.SablonSoruGorunumuOlustur(soru_uid);

        }

        protected string CreateMenu()
        {

            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            sbr_sablon anket = ankDB.SablonGetir(sablon_uid);
            int anket_durumu_id = 0;

            if (anket != null)
            {
                if (anket.sablon_durumu_id != null)
                {
                    anket_durumu_id = Convert.ToInt32(anket.sablon_durumu_id.ToString());
                }
            }

            
            StringBuilder menu_output = new StringBuilder();

            menu_output.Append("<ul class=\"mainMenu\" >");
            menu_output.Append("<li ><a href=\"AdminMain.aspx\" >ANKET PANELİ</a></li>");
            menu_output.Append("<li ><a href=\"SablonTanimlari.aspx\" >ŞABLONLAR</a></li>");


            menu_output.Append("<li ><a href=\"#\">ŞABLON İŞLEMLERİ</a>");
            menu_output.Append("<ul >");
            menu_output.Append("<li class=\"first\"><a href=\"YeniSablon.aspx?sablon_uid=" + sablon_uid + "\">ŞABLON AYARLARI</a></li>");
            if (anket_durumu_id == 1)
            {
                menu_output.Append("<li ><a href=\"#\" onclick=\"SablonuKapat();return false;\" >ŞABLONU KAPAT</a></li>");
            }
            else if (anket_durumu_id == 2)
            {
                menu_output.Append("<li ><a href=\"#\" onclick=\"SablonuAc();return false;\" >ŞABLONU AÇ</a></li>");
            }
            menu_output.Append("</ul>");
            menu_output.Append("</li>");


            menu_output.Append("<li ><a href=\"#\" onclick=\"SoruEkleme();return false;\">SORU EKLE</a></li></li>");
            menu_output.Append("<li ><a href=\"#\" onclick=\"SoruSirala();return false;\">SORULARI SIRALA</a>");

            menu_output.Append("</ul>");
            return menu_output.ToString();
        }

        public string SablonDurumu(object anket_durum_tipi_id)
        {
            string result = "";

            if (anket_durum_tipi_id != null && anket_durum_tipi_id.ToString() == ((int)BaseWebSite.Models.sablon_durumu.Acik).ToString())
            {
                result = "Açık";
            }
            else if (anket_durum_tipi_id != null && anket_durum_tipi_id.ToString() == ((int)BaseWebSite.Models.sablon_durumu.Kapali).ToString())
            {
                result = "Kapalı";
            }
           

            return result;
        }

        protected void SoruBilgileriniHazirla(string soru_uid)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            if (soru_uid == "") return;

            sbr_sablon_sorulari soru = ankDB.SablonSoruGetir(Guid.Parse(soru_uid));

            if (soru.soru_tipi_id != null)
            {
                this.ddlSoruTipi_Duzenle.SelectedValue = soru.soru_tipi_id.ToString();

                if (soru.soru_tipi_id == 1)
                {
                    DataSet secenek = ankDB.SablonSoruTipi1SecenekGetirSoruyaGoreDataSet(Guid.Parse(soru_uid));

                    foreach (DataRow dr in secenek.Tables[0].Rows)
                    {
                        if (dr["soru_secenek_ad"] != System.DBNull.Value) this.txtsoru_secenekleri_duzenle.Text += dr["soru_secenek_ad"].ToString() + "\r\n";
                    }
                }
                else if (soru.soru_tipi_id == 2)
                {
                    DataSet secenek = ankDB.SablonSoruTipi2SecenekGetirSoruyaGoreDataSet(Guid.Parse(soru_uid));

                    foreach (DataRow dr in secenek.Tables[0].Rows)
                    {
                        if (dr["soru_secenek_ad"] != System.DBNull.Value) this.txtsoru_secenekleri_duzenle.Text += dr["soru_secenek_ad"].ToString() + "\r\n";
                    }
                }
                else if (soru.soru_tipi_id == 7)
                {
                    DataSet secenek = ankDB.SablonSoruTipi7SecenekGetirSoruyaGoreDataSet(Guid.Parse(soru_uid));

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
                    if (soru.soru_tek_satir != null && soru.soru_tek_satir == true)
                        this.chkTekSatir_duzenle.Checked = true;
                    else
                        this.chkTekSatir_duzenle.Checked = false;
                }
                else if (soru.soru_tipi_id == 3)
                {
                    DataSet secenek = ankDB.SablonSoruTipi3SecenekGetirSoruyaGoreDataSet(Guid.Parse(soru_uid));

                    foreach (DataRow dr in secenek.Tables[0].Rows)
                    {
                        if (dr["soru_secenek_ad"] != System.DBNull.Value) this.txtsoru_secenekleri_duzenle.Text += dr["soru_secenek_ad"].ToString() + "\r\n";
                    }

                    DataSet kolon = ankDB.SablonSoruTipi3SecenekKolonGetirSoruyaGoreDataSet(Guid.Parse(soru_uid));

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
            ankDB.SablonSoruSil(Guid.Parse(soru_uid));

            ankDB.SablonSoruTipi1SecenekSilSoruyaGore(Guid.Parse(soru_uid));
            ankDB.SablonSoruTipi2SecenekSilSoruyaGore(Guid.Parse(soru_uid));
            ankDB.SablonSoruTipi3SecenekSilSoruyaGore(Guid.Parse(soru_uid));
            ankDB.SablonSoruTipi3SecenekKolonSilSoruyaGore(Guid.Parse(soru_uid));
            ankDB.SablonSoruTipi4SecenekSilSoruyaGore(Guid.Parse(soru_uid));
            ankDB.SablonSoruTipi5SecenekSilSoruyaGore(Guid.Parse(soru_uid));
            ankDB.SablonSoruTipi6SecenekSilSoruyaGore(Guid.Parse(soru_uid));
            ankDB.SablonSoruTipi7SecenekSilSoruyaGore(Guid.Parse(soru_uid));

            ankDB.Kaydet();
        }
    }
}