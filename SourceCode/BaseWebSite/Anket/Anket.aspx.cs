using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaseWebSite.Models;
using System.Data;
using System.Web.UI.HtmlControls;
using BaseClasses;
using System.Collections;

namespace BaseWebSite.Survey
{
    public partial class Survey : System.Web.UI.Page
    {
        public Guid anket_uid
        {
            get { return (ViewState["anket_uid"] != null ? Guid.Parse(ViewState["anket_uid"].ToString()) : Guid.Empty); }
            set { ViewState["anket_uid"] = value; }
        }

        public int soru_sayisi
        {
            get { return (ViewState["soru_sayisi"] != null ? Convert.ToInt32(ViewState["soru_sayisi"].ToString()) : 1); }
            set { ViewState["soru_sayisi"] = value; }
        }

        private string key
        {
            get { return (ViewState["key"] != null ? Convert.ToString(ViewState["key"]) : ""); }
            set { ViewState["key"] = value; }
        }

        private string test_key
        {
            get { return (ViewState["test_key"] != null ? Convert.ToString(ViewState["test_key"]) : ""); }
            set { ViewState["test_key"] = value; }
        }

        public string TemaHeader
        {
            get { return (ViewState["TemaHeader"] != null ? Convert.ToString(ViewState["TemaHeader"]) : ""); }
            set { ViewState["TemaHeader"] = value; }
        }

        public string TemaFooter
        {
            get { return (ViewState["TemaFooter"] != null ? Convert.ToString(ViewState["TemaFooter"]) : ""); }
            set { ViewState["TemaFooter"] = value; }
        }

        private string Preview
        {
            get { return (ViewState["Preview"] != null ? Convert.ToString(ViewState["Preview"]) : ""); }
            set { ViewState["Preview"] = value; }
        }

        private string Test
        {
            get { return (ViewState["Test"] != null ? Convert.ToString(ViewState["Test"]) : ""); }
            set { ViewState["Test"] = value; }
        }

        private string CookieKey
        {
            get { return (ViewState["CookieKey"] != null ? Convert.ToString(ViewState["CookieKey"]) : ""); }
            set { ViewState["CookieKey"] = value; }
        }

        public string path_url
        {
            get { return (ViewState["path_url"] != null ? Convert.ToString(ViewState["path_url"]) : ""); }
            set { ViewState["path_url"] = value; }
        }


        public bool anket_bitti_kontrol_et
        {
            get { return (ViewState["anket_bitti_kontrol_et"] != null ? Convert.ToBoolean(ViewState["anket_bitti_kontrol_et"]) : false); }
            set { ViewState["anket_bitti_kontrol_et"] = value; }
        }
       

        public PagedDataSource obj_ds;

        protected void Page_Load(object sender, EventArgs e)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            if (!IsPostBack)
            {

                if (Request.QueryString["anket_uid"] != null && Request.QueryString["anket_uid"].ToString() != "")
                {
                    anket_uid = Guid.Parse(Request.QueryString["anket_uid"].ToString());
                }

                if (Request.QueryString["key"] != null && Request.QueryString["key"].ToString() != "")
                {
                    key = Request.QueryString["key"].ToString();
                }

                if (Request.QueryString["test_key"] != null && Request.QueryString["test_key"].ToString() != "")
                {
                    test_key = Request.QueryString["test_key"].ToString();
                }

                if (Request.QueryString["Preview"] != null && Request.QueryString["Preview"].ToString() != "")
                {
                    Preview = Request.QueryString["Preview"].ToString();
                }

                if (Request.QueryString["Test"] != null && Request.QueryString["Test"].ToString() != "")
                {
                    Test = Request.QueryString["Test"].ToString();
                }

                
                if (System.Web.HttpContext.Current.Request.ApplicationPath == "/")
                    path_url = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/";
                else
                    path_url = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/" + System.Web.HttpContext.Current.Request.ApplicationPath + "/";

                this.ltlMesaj.Text = "Use 'Save' Button To Save Answers.";

                this.div_test_onay.Visible = false;

                if (Preview == "1")
                {
                    this.btnClose2.Visible = false;
                    this.btnFinish2.Visible = false;
                }
                InitiliazeCombos();
                if (Test == "1")
                {
                    this.btnClose2.Visible = false;
                    this.btnFinish2.Visible = false;
                    this.div_test_onay.Visible = true ;
                    this.ltlMesaj.Visible = false;
                }

                sbr_anket anket = ankDB.SurveyGetir(anket_uid);
                soru_sayisi = Convert.ToInt32(anket.sayfadaki_soru_sayisi.ToString());

                if (anket.anket_tipi_id == 1 && key == "" && Preview != "1")
                {
                    this.UyariMesaji.Text = "Lütfen E-posta adresinize gelen linki tıklayarak cevaplarınızı giriniz.";
                    this.uyari_mesaji.Visible = true;
                    this.error_message.Visible = false;
                    this.btnFinish2.Visible = false;
                    this.btnClose2.Visible = false;
                    this.rptSurveySayfalama.Visible = false;
                    return;
                }

                if (Preview!="1" && anket.anket_tipi_id == 1)
                {
                    sbr_anket_yayinlama_mail_gonderi_aktivasyon aktivasyon2 = ankDB.MailGonderiAktivasyonGetirKeyeGore(key);

                    if (aktivasyon2.ankete_girildi == null || aktivasyon2.ankete_girildi == false)
                    {
                        aktivasyon2.ankete_girildi = true;
                        aktivasyon2.ankete_giris_tarihi = DateTime.Now;
                        ankDB.Kaydet();
                    }
                }

                if(anket.anket_goruntulenen_ad_ekle!=null && anket.anket_goruntulenen_ad_ekle==true)
                    this.LinkButtonHeader.Text = (anket.anket_goruntulenen_ad.Length > 70) ? anket.anket_goruntulenen_ad.Substring(0, 67)+".." : anket.anket_goruntulenen_ad + " ";
                else
                    this.LinkButtonHeader.Text = (anket.anket_adi.Length > 70) ? anket.anket_adi.Substring(0, 67)+".." : anket.anket_adi + " ";

               

                if (anket.anket_tipi_id != 1)
                {
                    if (Request.Cookies["ankCookie_" + anket_uid.ToString()] != null)
                    {
                        if (Request.Cookies["ankCookie_" + anket_uid.ToString()]["visitKey"] != null)
                        {
                            key = Request.Cookies["ankCookie_" + anket_uid.ToString()]["visitKey"].ToString();
                            
                            if (key != "")
                            {
                                CookieKey = key;
                                sbr_anket_yayinlama_acik_anket_aktivasyon akt = ankDB.AcikSurveyAktivasyonGetirKeyeGore(anket_uid, key);
                                if (akt != null && akt.anket_bitirildi != null && akt.anket_bitirildi == true)
                                {
                                    this.btnFinish2.Visible = false;
                                    this.btnClose2.Visible = false;
                                    this.UyariMesaji.Text = "You Can See Your Answers.";
                                    this.uyari_mesaji.Visible = true;
                                    this.error_message.Visible = false;
                                    this.ltlMesaj.Visible = false;
                                    anket_bitti_kontrol_et = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        HttpCookie bsCookie = new HttpCookie("ankCookie_" + anket_uid.ToString());
                        bsCookie.Values["visitDate"] = DateTime.Now.ToString();
                        bsCookie.Values["visitKey"] = Guid.NewGuid().ToString();
                        bsCookie.Expires = DateTime.Now.AddDays(10);
                        key = bsCookie.Values["visitKey"].ToString();
                        HttpContext.Current.Response.Cookies.Add(bsCookie);
                    }
                }
                
            }

            this.CevapKotrolu.Text = "";
            this.error_message.Visible = false;

            if (this.hdn_kaydet_finish.Value == "1")
            {
                BindSurveySorulari(anket_uid);
                this.hdn_kaydet_finish.Value = "0";
            }

           
           
        }

        protected new void Page_LoadComplete(object sender, EventArgs e)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            this.uyari_mesaji.Visible = false;
            sbr_anket anket2 = ankDB.SurveyGetir(anket_uid);
            if (anket2.anket_tipi_id == 1 && key == "" && Preview!="1" )
            {
                this.UyariMesaji.Text = "Lütfen E-posta adresinize gelen linki tıklayarak cevaplarınızı giriniz.";
                this.uyari_mesaji.Visible = true;
                this.error_message.Visible = false;
                this.btnFinish2.Visible = false;
                this.btnClose2.Visible = false;
                this.rptSurveySayfalama.Visible = false;
                return;
            }

            
          
            BindSurveySorulari(anket_uid);
          

            FormuDuzenle();

            if (Test == "1")
            {
                this.BindTestSurveySonucu();
            }  
        }

        protected void InitiliazeCombos()
        {

           
            DataSet ds1 = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_ref_anket_test_sonucu ");
            this.ddlTestSonucu.DataSource = ds1;
            this.ddlTestSonucu.DataTextField = "anket_test_sonucu";
            this.ddlTestSonucu.DataValueField = "anket_test_sonucu_id";
            this.ddlTestSonucu.DataBind();

        }

        protected void BindTestSurveySonucu()
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            sbr_anket_test_mail_gonderi_kisi_tarihcesi test = ankDB.TestTarihceGetirKeyeGore(test_key);

            if (test != null)
            {
                if (test.anket_test_sonucu_id != null) this.ddlTestSonucu.SelectedValue = test.anket_test_sonucu_id.ToString();
                this.txtSurveyTestiSonucAciklamasi.Text = test.anket_test_sonucu;
            }
        
        }

        protected void FormuDuzenle()
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            sbr_anket anket = ankDB.SurveyGetir(anket_uid);

            if (anket != null)
            {
                if (anket.anket_durumu_id != null)
                {
                    if(Preview!="1" && anket.anket_durumu_id!=4)
                        Response.Redirect("AnketBitti.aspx?tip=3&anket_uid=" + anket_uid + "&key=" + key);
                }

                string cevap_key = "";

                if (anket.anket_tipi_id == 1)
                {
                    cevap_key = key;
                }
                else
                {
                    if(CookieKey !="")
                        cevap_key = CookieKey;
                    else
                        cevap_key = HttpContext.Current.Session.SessionID;
                }

                int cevaplanan_soru_sayisi = ankDB.CevaplananSoruSayısı(anket_uid, cevap_key);
                int tum_soru_sayisi = ankDB.TumSoruSayısı(anket_uid, cevap_key);

                this.ltlCevapDurumu.Text = "Answers : " + cevaplanan_soru_sayisi.ToString() + "/" + tum_soru_sayisi.ToString();

                if (anket.logo == null)
                {
                    //Image1.Visible = false;
                    Image1.ImageUrl = "~/img/surveySampleLogo.png";
                }
                else
                {
                    Image1.ImageUrl = "~/BaseAshx/ShowImage.ashx?anket_uid=" + anket_uid;
                }

                if (rptSurveySayfalama.SayfaNo == 0 && anket.anket_durumu_id != null && anket.anket_durumu_id == 4)
                {
                    if (anket.anket_mesaji_ekle != null && anket.anket_mesaji_ekle == true)
                    {
                        this.ltlSurveyMesaji.Text = anket.anket_mesaji;
                    }
                    else
                    {
                        this.ltlSurveyMesaji.Text = "Thank You For Your Interest";
                    }
                }
                else
                {
                    this.ltlSurveyMesaji.Text = "";
                }

                
                    if (anket.anket_tema_id == 1)
                    {
                        Literal link = new Literal();
                        link.Text = "<link rel=\"stylesheet\" type=\"text/css\" href=\"../css/theme1.css\" />";
                        base.Page.Header.Controls.Add(link);
                        this.rptSurveySayfalama.LnkPrev.ImageUrl = "~/img/leftArrow.png";
                        this.rptSurveySayfalama.lnknext.ImageUrl = "~/img/rightArrow.png";
                    }
                    else if (anket.anket_tema_id == 2)
                    {
                        Literal link = new Literal();
                        link.Text = "<link rel=\"stylesheet\" type=\"text/css\" href=\"../css/theme2.css\" />";
                        base.Page.Header.Controls.Add(link);
                        this.rptSurveySayfalama.LnkPrev.ImageUrl = "~/img/leftArrow2.png";
                        this.rptSurveySayfalama.lnknext.ImageUrl = "~/img/rightArrow2.png";
                    }
                    else if (anket.anket_tema_id == 3)
                    {
                        Literal link = new Literal();
                        link.Text = "<link rel=\"stylesheet\" type=\"text/css\" href=\"../css/theme3.css\" />";
                        base.Page.Header.Controls.Add(link);
                        this.rptSurveySayfalama.LnkPrev.ImageUrl = "~/img/leftArrow3.png";
                        this.rptSurveySayfalama.lnknext.ImageUrl = "~/img/rightArrow3.png";
                    }
                    else
                    {
                        Literal link = new Literal();
                        link.Text = "<link rel=\"stylesheet\" type=\"text/css\" href=\"../css/theme1.css\" />";
                        base.Page.Header.Controls.Add(link);
                    }
                
            }
        }

        protected void BindSurveySorulari(Guid anket_uid)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            
            DataSet ds = ankDB.SurveySoruGetirSurveyGoreDataSet(anket_uid);

            if (ds.Tables[0].Rows.Count <= 0)
                return;

            this.rptSurveySayfalama.dataSource = ds;
            this.rptSurveySayfalama.SayfaBuyuklugu = soru_sayisi;
            this.rptSurveySayfalama.Bind();
            this.rptSurveySorulari.DataSource = this.rptSurveySayfalama.Sayfa;
            this.rptSurveySorulari.DataBind();

            this.obj_ds = this.rptSurveySayfalama.Sayfa;

            ArrayList arr = new ArrayList();
            
            foreach (DataRowView dr in this.rptSurveySayfalama.Sayfa)
            {
                arr.Add(dr["soru_uid"].ToString());
            }

            int index = 0;


            foreach (RepeaterItem item in rptSurveySorulari.Items)
            {
                Control cnt = null;
                cnt= item.FindControl("div_soru");
                    
                string soru_uid = arr[index].ToString();

                if (cnt != null)
                    SorulariOlustur(cnt, Guid.Parse(soru_uid),index);

                index++;
            }

            this.rptSurveySayfalama.lnknext.Visible = true;
            this.rptSurveySayfalama.LnkPrev.Visible = true;

            if(this.rptSurveySayfalama.Sayfa.IsLastPage)
                this.rptSurveySayfalama.lnknext.Visible = false;

            if (this.rptSurveySayfalama.Sayfa.IsFirstPage)
                this.rptSurveySayfalama.LnkPrev.Visible = false;

            sbr_anket anket = ankDB.SurveyGetir(anket_uid);
            Image1.ImageUrl = "~/BaseAshx/ShowImage.ashx?anket_uid=" + anket_uid;



            if (anket.anket_tipi_id == 1)
            {
                sbr_anket_yayinlama_mail_gonderi_aktivasyon aktivasyon = ankDB.MailGonderiAktivasyonGetirKeyeGore(key);

                if (Preview != "1")
                {
                    if (aktivasyon.anket_bitirildi != null || aktivasyon.anket_bitirildi == true)
                    {
                        this.btnFinish2.Visible = false;
                        this.btnClose2.Visible = false;
                        this.UyariMesaji.Text = "You Can See Your Answers.";
                        this.uyari_mesaji.Visible = true;
                        this.error_message.Visible = false;
                        this.ltlMesaj.Visible = false;
                        anket_bitti_kontrol_et = true;
                    }
                }
            }
            else
            {
                if (Request.Cookies["ankCookie_" + anket_uid.ToString()] != null)
                {
                    if (Request.Cookies["ankCookie_" + anket_uid.ToString()]["visitKey"] != null)
                    {
                        key = Request.Cookies["ankCookie_" + anket_uid.ToString()]["visitKey"].ToString();

                        if (key != "")
                        {
                            CookieKey = key;
                            sbr_anket_yayinlama_acik_anket_aktivasyon akt = ankDB.AcikSurveyAktivasyonGetirKeyeGore(anket_uid, key);
                            if (akt != null && akt.anket_bitirildi != null && akt.anket_bitirildi == true)
                            {
                                this.btnFinish2.Visible = false;
                                this.btnClose2.Visible = false;
                                this.UyariMesaji.Text = "You Can See Your Answers.";
                                this.uyari_mesaji.Visible = true;
                                this.error_message.Visible = false;
                                this.ltlMesaj.Visible = false;
                                anket_bitti_kontrol_et = true;
                            }
                        }
                    }
                }
            }
        }

        protected void SorulariOlustur(Control cnt, Guid soru_uid,int index)
        {
            Guid soru_id = Guid.Parse(soru_uid.ToString());

            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            sbr_anket_sorulari anket_soru = ankDB.SurveySoruGetir(soru_id);
            sbr_anket anket = ankDB.SurveyGetir(anket_uid);

            string cevap_key = "";

            if (anket.anket_tipi_id == 1)
            {
                cevap_key = key;
            }
            else
            {
                if (CookieKey != "")
                    cevap_key = CookieKey;
                else
                    cevap_key = HttpContext.Current.Session.SessionID;
            }

            string required_class ="";
            
            if (anket_soru.soru_tipi_id == 1)
            {
                DataSet ds_soru_tipi1_secenekler = ankDB.AcikAnketSoruTipi1SecenekGetirSoruyaGoreDataSet(soru_id,anket_uid);

                HtmlTable tbl = new HtmlTable();
                foreach (DataRow dr in ds_soru_tipi1_secenekler.Tables[0].Rows)
                {
                    HtmlTableRow row = new HtmlTableRow();
                    HtmlTableCell cell=new HtmlTableCell();
                    row.Cells.Add(cell);
                    HtmlInputRadioButton radio=new HtmlInputRadioButton();
                    radio.ID = "secenek_tip1_" + dr["soru_secenek_uid"].ToString();
                    radio.Name = "grup_" + dr["soru_uid"].ToString();
                    radio.Attributes["runat"] = "server";
                    radio.Attributes["class"] = "radio";
                    cell.Controls.Add(radio);

                    HtmlGenericControl label = new HtmlGenericControl("label");
                    label.InnerHtml = dr["soru_secenek_ad"].ToString();
                    label.Attributes["class"] = "radioEntry";
                    label.Attributes["for"] = "rptSurveySorulari_secenek_tip1_" + dr["soru_secenek_uid"].ToString() + "_" + index.ToString();
                    cell.Controls.Add(label);

                    if (anket.anket_tipi_id != 1)
                    {
                        HtmlTableCell cell2 = new HtmlTableCell();
                        HtmlGenericControl label_yuzde = new HtmlGenericControl("label");
                        label_yuzde.InnerHtml = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span style=\"color:Red\">" + (dr["cevap_oran"] != System.DBNull.Value ? dr["cevap_oran"].ToString() : "0") + "%&nbsp;&nbsp;(" + (dr["cevap_sayisi"] != System.DBNull.Value ? dr["cevap_sayisi"].ToString() : "0") + ")</span>";
                        label_yuzde.Attributes["class"] = "radioEntry";
                        label_yuzde.Attributes["for"] = "rptSurveySorulari_secenek_tip1_" + dr["soru_secenek_uid"].ToString() + "_" + index.ToString();
                        cell2.Controls.Add(label_yuzde);
                        row.Cells.Add(cell2);
                    }

                    tbl.Rows.Add(row);
                }

                cnt.Controls.Add(tbl);

                DataSet ds_soru_tipi1_cevaplar = ankDB.AcikAnketSoruTipi1CevapGetirSoruyaKeyeGoreDataSet(soru_id, cevap_key);
                foreach (DataRow dr in ds_soru_tipi1_cevaplar.Tables[0].Rows)
                {
                    string id = "secenek_tip1_" + dr["soru_secenek_uid"].ToString();
                    HtmlInputRadioButton radio = (HtmlInputRadioButton)BaseClasses.BaseFunctions.getInstance().FindControlInRepeater(id, this.rptSurveySorulari);

                    if (Convert.ToBoolean(dr["cevap"].ToString()) == true)
                    {
                        radio.Checked = true;
                    }
                    else
                    {
                        radio.Checked = false;
                    }
                }

              
            }

            else if (anket_soru.soru_tipi_id == 2)
            {
                DataSet ds_soru_tipi2_secenekler = ankDB.AcikAnketSoruTipi2SecenekGetirSoruyaGoreDataSet(soru_id);
                HtmlTable tbl = new HtmlTable();
                foreach (DataRow dr in ds_soru_tipi2_secenekler.Tables[0].Rows)
                {
                    HtmlTableRow row = new HtmlTableRow();
                    HtmlTableCell cell = new HtmlTableCell();
                    row.Cells.Add(cell);
                    HtmlInputCheckBox check = new HtmlInputCheckBox();
                    check.ID = "secenek_tip2_" + dr["soru_secenek_uid"].ToString();
                    check.Attributes["tag"]="grup_" + dr["soru_uid"].ToString();
                    check.Attributes["runat"] = "server";
                    check.Attributes["class"] = "checkbox";
                    
                    cell.Controls.Add(check);

                    HtmlGenericControl label = new HtmlGenericControl("label");
                    label.InnerHtml = dr["soru_secenek_ad"].ToString();
                    label.Attributes["class"] = "radioEntry";
                    label.Attributes["for"] = "rptSurveySorulari_secenek_tip2_" + dr["soru_secenek_uid"].ToString() + "_" + index.ToString();
                    cell.Controls.Add(label);
                    tbl.Rows.Add(row);
                }
                cnt.Controls.Add(tbl);

                DataSet ds_soru_tipi2_cevaplar = ankDB.AcikAnketSoruTipi2CevapGetirSoruyaKeyeGoreDataSet(soru_id, cevap_key);
                foreach (DataRow dr in ds_soru_tipi2_cevaplar.Tables[0].Rows)
                {
                    string id = "secenek_tip2_" + dr["soru_secenek_uid"].ToString();
                    HtmlInputCheckBox check = (HtmlInputCheckBox)BaseClasses.BaseFunctions.getInstance().FindControlInRepeater(id, this.rptSurveySorulari);

                    if (Convert.ToBoolean(dr["cevap"].ToString()) == true)
                    {
                        check.Checked = true;
                    }
                    else
                    {
                        check.Checked = false;
                    }
                }
            }
            if (anket_soru.soru_tipi_id == 3)
            {

                int i = 1;
                DataSet ds_soru_tipi3_secenekler = ankDB.AcikAnketSoruTipi3SecenekGetirSoruyaGoreDataSet(soru_id);
                DataSet ds_soru_tipi3_kolonlar = ankDB.AcikAnketSoruTipi3SecenekKolonGetirSoruyaGoreDataSet(soru_id);
                HtmlTable tbl = new HtmlTable();
                tbl.Attributes["class"] = "display";
                foreach (DataRow dr in ds_soru_tipi3_secenekler.Tables[0].Rows)
                {
                    HtmlTableRow row = new HtmlTableRow();

                    if ((i % 2) == 1)
                        row.Attributes["class"] = "odd";
                    else
                        row.Attributes["class"] = "even";
                    i++;
                    HtmlTableCell cell = new HtmlTableCell();
                    row.Cells.Add(cell);
                    
                    HtmlGenericControl label = new HtmlGenericControl("label");
                    label.InnerHtml = dr["soru_secenek_ad"].ToString();
                    label.Attributes["class"] = "radioEntry";
                    
                    cell.Controls.Add(label);

                    foreach (DataRow dr2 in ds_soru_tipi3_kolonlar.Tables[0].Rows)
                    {
                        HtmlTableCell cell2 = new HtmlTableCell();
                        row.Cells.Add(cell2);

                        HtmlInputRadioButton radio = new HtmlInputRadioButton();
                        radio.ID = "secenek_tip3_" + dr["soru_secenek_uid"].ToString() + "_" + dr2["soru_secenek_kolon_uid"].ToString();
                        radio.Name = "grup_" + dr["soru_secenek_uid"].ToString();
                        radio.Attributes["runat"] = "server";
                        radio.Attributes["class"] = "radio";
                        cell2.Controls.Add(radio);

                        HtmlGenericControl label2 = new HtmlGenericControl("label");
                        label2.InnerHtml = dr2["soru_secenek_kolon_ad"].ToString();
                        label2.Attributes["class"] = "radioEntry";
                        label2.Attributes["for"] = "rptSurveySorulari_secenek_tip3_" + dr["soru_secenek_uid"].ToString() + "_" + dr2["soru_secenek_kolon_uid"].ToString() + "_" + index.ToString();
                        cell2.Controls.Add(label2);

                    }

                   

                    tbl.Rows.Add(row);
                }
                cnt.Controls.Add(tbl);


                DataSet ds_soru_tipi3_cevaplar = ankDB.AcikAnketSoruTipi3CevapGetirSoruyaKeyeGoreDataSet(soru_id, cevap_key);
                foreach (DataRow dr in ds_soru_tipi3_cevaplar.Tables[0].Rows)
                {
                    string id = "secenek_tip3_" + dr["soru_secenek_uid"].ToString() + "_" + dr["soru_secenek_kolon_uid"].ToString();
                    HtmlInputRadioButton radio = (HtmlInputRadioButton)BaseClasses.BaseFunctions.getInstance().FindControlInRepeater(id, this.rptSurveySorulari);

                    if (Convert.ToBoolean(dr["cevap"].ToString()) == true)
                    {
                        radio.Checked = true;
                    }
                    else
                    {
                        radio.Checked = false;
                    }
                }
            }
            else if (anket_soru.soru_tipi_id == 4)
            {
                DataSet ds_soru_tipi4_secenekler = ankDB.AcikAnketSoruTipi4SecenekGetirSoruyaGoreDataSet(soru_id);
                HtmlTable tbl = new HtmlTable();
                foreach (DataRow dr in ds_soru_tipi4_secenekler.Tables[0].Rows)
                {
                    HtmlTableRow row = new HtmlTableRow();
                    HtmlTableCell cell = new HtmlTableCell();
                    cell.Width = "460px";
                    row.Cells.Add(cell);
                    
                    
                    if (anket_soru.soru_tek_satir != null && anket_soru.soru_tek_satir == true)
                    {
                        HtmlInputText txt = new HtmlInputText();
                        txt.ID = "secenek_tip4_" + dr["soru_secenek_uid"].ToString();
                        txt.Attributes["runat"] = "server";
                        txt.Attributes["class"] = "medium";
                        txt.Name = "grup_" + dr["soru_uid"].ToString();
                        cell.Controls.Add(txt);
                        
                    }
                    else
                    {
                        HtmlTextArea txtarea = new HtmlTextArea();
                        txtarea.ID = "secenek_tip4_" + dr["soru_secenek_uid"].ToString();
                        txtarea.Attributes["runat"] = "server";
                        txtarea.Attributes["class"] = "medium";
                        txtarea.Name = "grup_" + dr["soru_uid"].ToString();
                        txtarea.Rows = 5;
                        txtarea.Cols = 80;
                        cell.Controls.Add(txtarea);
                        
                    }

                    
                    tbl.Rows.Add(row);
                }
                cnt.Controls.Add(tbl);

                DataSet ds_soru_tipi4_cevaplar = ankDB.AcikAnketSoruTipi4CevapGetirSoruyaKeyeGoreDataSet(soru_id, cevap_key);
                foreach (DataRow dr in ds_soru_tipi4_cevaplar.Tables[0].Rows)
                {

                    string id = "secenek_tip4_" + dr["soru_secenek_uid"].ToString();
                    if (anket_soru.soru_tek_satir != null && anket_soru.soru_tek_satir == true)
                    {
                        
                        HtmlInputText text = (HtmlInputText)BaseClasses.BaseFunctions.getInstance().FindControlInRepeater(id, this.rptSurveySorulari);
                        text.Value = dr["cevap"].ToString();
                    }
                    else
                    {
                        HtmlTextArea text = (HtmlTextArea)BaseClasses.BaseFunctions.getInstance().FindControlInRepeater(id, this.rptSurveySorulari);
                        text.Value = dr["cevap"].ToString();
                    }
                }
            }
            else if (anket_soru.soru_tipi_id == 7)
            {

                DataSet ds_soru_tipi7_secenekler = ankDB.AcikAnketSoruTipi7SecenekGetirSoruyaGoreDataSet(soru_id);
                HtmlTable tbl = new HtmlTable();
                foreach (DataRow dr in ds_soru_tipi7_secenekler.Tables[0].Rows)
                {
                    HtmlTableRow row = new HtmlTableRow();
                    
                    HtmlTableCell cell = new HtmlTableCell();
                    row.Cells.Add(cell);

                    HtmlGenericControl span = new HtmlGenericControl("span");
                    span.InnerHtml = dr["soru_secenek_ad"].ToString();
                    span.Attributes["class"] = "radioEntry";
                    cell.Controls.Add(span);

                    HtmlTableCell cell2 = new HtmlTableCell();
                    row.Cells.Add(cell2);

                    HtmlGenericControl span2 = new HtmlGenericControl("span");
                    span2.InnerHtml = " : ";
                    span2.Attributes["class"] = "radioEntry";
                    cell.Controls.Add(span2);

                    HtmlTableCell cell3 = new HtmlTableCell();
                    cell3.Width = "460px";
                    row.Cells.Add(cell3);


                    HtmlInputText tarea = new HtmlInputText();
                    tarea.ID = "secenek_tip7_" + dr["soru_secenek_uid"].ToString();
                    tarea.Attributes["runat"] = "server";
                    tarea.Attributes["class"] = "medium";
                    tarea.Name = "grup_" + dr["soru_uid"].ToString();
                    cell3.Controls.Add(tarea);
                    tbl.Rows.Add(row);
                }
                cnt.Controls.Add(tbl);

                DataSet ds_soru_tipi7_cevaplar = ankDB.AcikAnketSoruTipi7CevapGetirSoruyaKeyeGoreDataSet(soru_id, cevap_key);
                foreach (DataRow dr in ds_soru_tipi7_cevaplar.Tables[0].Rows)
                {

                    string id = "secenek_tip7_" + dr["soru_secenek_uid"].ToString();
                    HtmlInputText text = (HtmlInputText)BaseClasses.BaseFunctions.getInstance().FindControlInRepeater(id, this.rptSurveySorulari);
                    text.Value = dr["cevap"].ToString();
                }
               
            }
           else if (anket_soru.soru_tipi_id == 5)
            {
               
                DataSet ds_soru_tipi5_secenekler = ankDB.AcikAnketSoruTipi5SecenekGetirSoruyaGoreDataSet(soru_id);
                HtmlTable tbl = new HtmlTable();
                foreach (DataRow dr in ds_soru_tipi5_secenekler.Tables[0].Rows)
                {
                    HtmlTableRow row = new HtmlTableRow();
                    HtmlTableCell cell = new HtmlTableCell();
                    row.Cells.Add(cell);
                    HtmlInputRadioButton radio = new HtmlInputRadioButton();
                    radio.ID = "secenek_tip5_" + dr["soru_secenek_uid"].ToString();
                    radio.Name = "grup_" + dr["soru_uid"].ToString();
                    radio.Attributes["runat"] = "server";
                    radio.Attributes["class"] = "radio";
                    cell.Controls.Add(radio);

                    HtmlGenericControl label = new HtmlGenericControl("label");
                    label.InnerHtml = dr["soru_secenek_ad"].ToString();
                    label.Attributes["class"] = "radioEntry";
                    label.Attributes["for"] = "rptSurveySorulari_secenek_tip5_" + dr["soru_secenek_uid"].ToString() + "_" + index.ToString();
                    cell.Controls.Add(label);
                    tbl.Rows.Add(row);
                }
                cnt.Controls.Add(tbl);

                DataSet ds_soru_tipi5_cevaplar = ankDB.AcikAnketSoruTipi5CevapGetirSoruyaKeyeGoreDataSet(soru_id, cevap_key);
                foreach (DataRow dr in ds_soru_tipi5_cevaplar.Tables[0].Rows)
                {

                    string id = "secenek_tip5_" + dr["soru_secenek_uid"].ToString();
                    HtmlInputRadioButton radio = (HtmlInputRadioButton)BaseClasses.BaseFunctions.getInstance().FindControlInRepeater(id, this.rptSurveySorulari);

                    if (Convert.ToBoolean(dr["cevap"].ToString()) == true)
                    {
                        radio.Checked = true;
                    }
                    else
                    {
                        radio.Checked = false;
                    }
                }
            }
            else if (anket_soru.soru_tipi_id == 6)
            {

                DataSet ds_soru_tipi6_secenekler = ankDB.AcikAnketSoruTipi6SecenekGetirSoruyaGoreDataSet(soru_id);
                HtmlTable tbl = new HtmlTable();
                foreach (DataRow dr in ds_soru_tipi6_secenekler.Tables[0].Rows)
                {
                    HtmlTableRow row = new HtmlTableRow();
                    HtmlTableCell cell = new HtmlTableCell();
                    row.Cells.Add(cell);
                    HtmlInputRadioButton radio = new HtmlInputRadioButton();
                    radio.ID = "secenek_tip6_" + dr["soru_secenek_uid"].ToString();
                    radio.Name = "grup_" + dr["soru_uid"].ToString();
                    radio.Attributes["runat"] = "server";
                    radio.Attributes["class"] = "radio";
                    cell.Controls.Add(radio);

                    HtmlGenericControl label = new HtmlGenericControl("label");
                    label.InnerHtml = dr["soru_secenek_ad"].ToString();
                    label.Attributes["class"] = "radioEntry";
                    label.Attributes["for"] = "rptSurveySorulari_secenek_tip6_" + dr["soru_secenek_uid"].ToString() + "_" + index.ToString();
                    cell.Controls.Add(label);
                    tbl.Rows.Add(row);
                }
                cnt.Controls.Add(tbl);

                DataSet ds_soru_tipi6_cevaplar = ankDB.AcikAnketSoruTipi6CevapGetirSoruyaKeyeGoreDataSet(soru_id, cevap_key);
                foreach (DataRow dr in ds_soru_tipi6_cevaplar.Tables[0].Rows)
                {

                    string id = "secenek_tip6_" + dr["soru_secenek_uid"].ToString();
                    HtmlInputRadioButton radio = (HtmlInputRadioButton)BaseClasses.BaseFunctions.getInstance().FindControlInRepeater(id, this.rptSurveySorulari);

                    if (Convert.ToBoolean(dr["cevap"].ToString()) == true)
                    {
                        radio.Checked = true;
                    }
                    else
                    {
                        radio.Checked = false;
                    }
                }
            }
            else if (anket_soru.soru_tipi_id == 8)
            {

                if (anket_soru.soru_sayisal_ondalik != null && anket_soru.soru_sayisal_ondalik == true)
                    required_class = "numeric";
                else
                    required_class = "integer";

                DataSet ds_soru_tipi8_secenekler = ankDB.AcikAnketSoruTipi8SecenekGetirSoruyaGoreDataSet(soru_id);
                HtmlTable tbl = new HtmlTable();
                foreach (DataRow dr in ds_soru_tipi8_secenekler.Tables[0].Rows)
                {
                    HtmlTableRow row = new HtmlTableRow();
                    HtmlTableCell cell = new HtmlTableCell();
                    row.Cells.Add(cell);
                    HtmlInputText sayisal = new HtmlInputText();
                    sayisal.ID = "secenek_tip8_" + dr["soru_secenek_uid"].ToString();
                    sayisal.Attributes["runat"] = "server";
                    sayisal.Attributes["class"] = required_class;
                    sayisal.Attributes["style"] = "text-align: right";
                    sayisal.Name = "grup_" + dr["soru_uid"].ToString();
                    cell.Controls.Add(sayisal);
                    tbl.Rows.Add(row);
                }
                cnt.Controls.Add(tbl);

                DataSet ds_soru_tipi8_cevaplar = ankDB.AcikAnketSoruTipi8CevapGetirSoruyaKeyeGoreDataSet(soru_id, cevap_key);
                foreach (DataRow dr in ds_soru_tipi8_cevaplar.Tables[0].Rows)
                {

                    string id = "secenek_tip8_" + dr["soru_secenek_uid"].ToString();
                    HtmlInputText sayisal = (HtmlInputText)BaseClasses.BaseFunctions.getInstance().FindControlInRepeater(id, this.rptSurveySorulari);
                    
                    if(anket_soru.soru_sayisal_ondalik!=null && anket_soru.soru_sayisal_ondalik==true)
                        sayisal.Value = dr["cevap"].ToString();
                    else
                        sayisal.Value = Convert.ToInt32(dr["cevap"].ToString().Substring(0,dr["cevap"].ToString().Replace('.',',').IndexOf(','))).ToString();
                }
            }
            else if (anket_soru.soru_tipi_id == 9)
            {

                DataSet ds_soru_tipi9_secenekler = ankDB.AcikAnketSoruTipi9SecenekGetirSoruyaGoreDataSet(soru_id);
                HtmlTable tbl = new HtmlTable();
                foreach (DataRow dr in ds_soru_tipi9_secenekler.Tables[0].Rows)
                {
                    HtmlTableRow row = new HtmlTableRow();
                    HtmlTableCell cell = new HtmlTableCell();
                    row.Cells.Add(cell);
                    HtmlInputText sayisal = new HtmlInputText();
                    sayisal.ID = "secenek_tip9_" + dr["soru_secenek_uid"].ToString();
                    sayisal.Attributes["runat"] = "server";
                    sayisal.Attributes["class"] = "datepicker";
                    sayisal.Attributes["readonly"] = "readonly";
                    sayisal.Name = "grup_" + dr["soru_uid"].ToString();
                    cell.Controls.Add(sayisal);
                    tbl.Rows.Add(row);
                }
                cnt.Controls.Add(tbl);

                DataSet ds_soru_tipi9_cevaplar = ankDB.AcikAnketSoruTipi9CevapGetirSoruyaKeyeGoreDataSet(soru_id, cevap_key);
                foreach (DataRow dr in ds_soru_tipi9_cevaplar.Tables[0].Rows)
                {

                    string id = "secenek_tip9_" + dr["soru_secenek_uid"].ToString();
                    HtmlInputText tarih = (HtmlInputText)BaseClasses.BaseFunctions.getInstance().FindControlInRepeater(id, this.rptSurveySorulari);

                    if (dr["cevap"] != System.DBNull.Value  && dr["cevap"].ToString() != "")
                    {
                        try
                        {
                            string bitis_gun = Convert.ToDateTime(dr["cevap"].ToString(), new System.Globalization.CultureInfo("tr-TR")).Day.ToString();
                            string bitis_ay = Convert.ToDateTime(dr["cevap"].ToString(), new System.Globalization.CultureInfo("tr-TR")).Month.ToString();
                            string bitis_yil = Convert.ToDateTime(dr["cevap"].ToString(), new System.Globalization.CultureInfo("tr-TR")).Year.ToString();

                            if (bitis_gun.Length == 1) bitis_gun = "0" + bitis_gun;
                            if (bitis_ay.Length == 1) bitis_ay = "0" + bitis_ay;

                            tarih.Value = bitis_gun + "-" + bitis_ay + "-" + bitis_yil;
                        }
                        catch (Exception exp)
                        { 
                        
                        }
                    }
                }
            }
            else if (anket_soru.soru_tipi_id == 10)
            {
                DataSet ds_soru_tipi10_secenekler = ankDB.AcikAnketSoruTipi10SecenekGetirSoruyaGoreDataSet(soru_id);
                HtmlTable tbl = new HtmlTable();
                foreach (DataRow dr in ds_soru_tipi10_secenekler.Tables[0].Rows)
                {
                    HtmlTableRow row = new HtmlTableRow();
                    HtmlTableCell cell = new HtmlTableCell();
                    row.Cells.Add(cell);
                    HtmlInputText sayisal = new HtmlInputText();
                    sayisal.ID = "secenek_tip10_" + dr["soru_secenek_uid"].ToString();
                    sayisal.Attributes["runat"] = "server";
                    sayisal.Attributes["class"] = "phone";
                    sayisal.Name = "grup_" + dr["soru_uid"].ToString();
                    cell.Controls.Add(sayisal);
                    tbl.Rows.Add(row);
                }
                cnt.Controls.Add(tbl);

                DataSet ds_soru_tipi10_cevaplar = ankDB.AcikAnketSoruTipi10CevapGetirSoruyaKeyeGoreDataSet(soru_id, cevap_key);
                foreach (DataRow dr in ds_soru_tipi10_cevaplar.Tables[0].Rows)
                {

                    string id = "secenek_tip10_" + dr["soru_secenek_uid"].ToString();
                    HtmlInputText tel = (HtmlInputText)BaseClasses.BaseFunctions.getInstance().FindControlInRepeater(id, this.rptSurveySorulari);
                    
                    if (dr["cevap"] != System.DBNull.Value )
                    {
                        tel.Value = dr["cevap"].ToString();
                    }
                }
            }
            else if (anket_soru.soru_tipi_id == 11)
            {
                DataSet ds_soru_tipi11_secenekler = ankDB.AcikAnketSoruTipi11SecenekGetirSoruyaGoreDataSet(soru_id);
                HtmlTable tbl = new HtmlTable();
                foreach (DataRow dr in ds_soru_tipi11_secenekler.Tables[0].Rows)
                {
                    HtmlTableRow row = new HtmlTableRow();
                    HtmlTableCell cell = new HtmlTableCell();
                    cell.Width = "460px";
                    row.Cells.Add(cell);
                    HtmlInputText sayisal = new HtmlInputText();
                    sayisal.ID = "secenek_tip11_" + dr["soru_secenek_uid"].ToString();
                    sayisal.Attributes["runat"] = "server";
                    sayisal.Attributes["class"] = "eposta validate[custom[email]] medium";
                    sayisal.Name = "grup_" + dr["soru_uid"].ToString();
                    cell.Controls.Add(sayisal);
                    tbl.Rows.Add(row);
                }
                cnt.Controls.Add(tbl);

                DataSet ds_soru_tipi11_cevaplar = ankDB.AcikAnketSoruTipi11CevapGetirSoruyaKeyeGoreDataSet(soru_id, cevap_key);
                foreach (DataRow dr in ds_soru_tipi11_cevaplar.Tables[0].Rows)
                {

                    string id = "secenek_tip11_" + dr["soru_secenek_uid"].ToString();
                    HtmlInputText tel = (HtmlInputText)BaseClasses.BaseFunctions.getInstance().FindControlInRepeater(id, this.rptSurveySorulari);

                    if (dr["cevap"] != System.DBNull.Value)
                    {
                        tel.Value = dr["cevap"].ToString();
                    }
                }
            }
        }

        protected void Kaydet()
        {
            string result = "";
            

            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            DataSet ds_anket = ankDB.SurveySoruGetirSurveyGoreDataSet(anket_uid);
            sbr_anket anket = ankDB.SurveyGetir(anket_uid);

            string cevap_key = "", cevaplayan_ad = "", cavaplayan_soyad = "", cevaplayan_email = "";

            if (anket.anket_tipi_id == 1)
            {
                cevap_key = key;
                sbr_anket_yayinlama_mail_gonderi_aktivasyon aktivasyon =  ankDB.MailGonderiAktivasyonGetirKeyeGore(key);
                cevaplayan_ad = aktivasyon.anket_gonderilen_ad;
                cavaplayan_soyad = aktivasyon.anket_gonderilen_soyad;
                cevaplayan_email = aktivasyon.anket_gonderilen_email;
            }
            else
            {
                if (CookieKey != "")
                    cevap_key = CookieKey;
                else
                    cevap_key = HttpContext.Current.Session.SessionID;
            }

            foreach (DataRowView dr_anket in obj_ds)
            {
                Guid soru_id = Guid.Parse(dr_anket["soru_uid"].ToString());
                int soru_tipi_id = Int32.Parse(dr_anket["soru_tipi_id"].ToString());

                if (soru_tipi_id == 1)
                {
                    DataSet ds_soru_tipi1_secenekler = ankDB.AcikAnketSoruTipi1SecenekGetirSoruyaGoreDataSet(soru_id,Guid.Empty);
                    ankDB.AcikAnketSoruTipi1CevapSilSoruyaKeyeGore(soru_id, cevap_key);

                    foreach (DataRow dr in ds_soru_tipi1_secenekler.Tables[0].Rows)
                    {
                        string id = "secenek_tip1_" + dr["soru_secenek_uid"].ToString();
                        HtmlInputRadioButton radio = (HtmlInputRadioButton)BaseClasses.BaseFunctions.getInstance().FindControlInRepeater(id, this.rptSurveySorulari);
                    
                        sbr_soru_tipi_1_cevaplari cevaplar=new sbr_soru_tipi_1_cevaplari();
                        ankDB.AcikAnketSoruTipi1CevapEkle(cevaplar);

                        if (radio.Checked==true)
                            cevaplar.cevap = true;
                        else
                            cevaplar.cevap = false;

                        cevaplar.cavaplama_tarihi = DateTime.Now;
                        cevaplar.soru_secenek_uid = Guid.Parse(dr["soru_secenek_uid"].ToString());

                        HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;
                        SessionInfo si = new SessionInfo();
                        cevaplar.session_id = HttpContext.Current.Session.SessionID;
                        cevaplar.host_name = HttpContext.Current.Request.UserHostName;
                        cevaplar.user_host_adres = HttpContext.Current.Request.UserHostAddress;
                        cevaplar.browser = bc.Browser;
                        cevaplar.platform = bc.Platform;
                        cevaplar.version = bc.Version;
                        cevaplar.soru_uid = soru_id;
                        cevaplar.cevap_key = cevap_key;
                        cevaplar.cevaplayan_ad = cevaplayan_ad;
                        cevaplar.cevaplayan_soyad = cavaplayan_soyad;
                        cevaplar.cevaplayan_email = cevaplayan_email;
                        ankDB.Kaydet();
            
                    }
                }
                else if (soru_tipi_id == 2)
                {
                    DataSet ds_soru_tipi2_secenekler = ankDB.AcikAnketSoruTipi2SecenekGetirSoruyaGoreDataSet(soru_id);
                    ankDB.AcikAnketSoruTipi2CevapSilSoruyaKeyeGore(soru_id, cevap_key);

                    foreach (DataRow dr in ds_soru_tipi2_secenekler.Tables[0].Rows)
                    {
                        string id = "secenek_tip2_" + dr["soru_secenek_uid"].ToString();
                        HtmlInputCheckBox check = (HtmlInputCheckBox)BaseClasses.BaseFunctions.getInstance().FindControlInRepeater(id, this.rptSurveySorulari);
                        
                        sbr_soru_tipi_2_cevaplari cevaplar = new sbr_soru_tipi_2_cevaplari();
                        ankDB.AcikAnketSoruTipi2CevapEkle(cevaplar);

                        if (check.Checked == true)
                            cevaplar.cevap = true;
                        else
                            cevaplar.cevap = false;

                        cevaplar.cavaplama_tarihi = DateTime.Now;
                        cevaplar.soru_secenek_uid = Guid.Parse(dr["soru_secenek_uid"].ToString());

                        HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;
                        SessionInfo si = new SessionInfo();
                        cevaplar.session_id = HttpContext.Current.Session.SessionID;
                        cevaplar.host_name = HttpContext.Current.Request.UserHostName;
                        cevaplar.user_host_adres = HttpContext.Current.Request.UserHostAddress;
                        cevaplar.browser = bc.Browser;
                        cevaplar.platform = bc.Platform;
                        cevaplar.version = bc.Version;
                        cevaplar.soru_uid = soru_id;
                        cevaplar.cevap_key = cevap_key;
                        cevaplar.cevaplayan_ad = cevaplayan_ad;
                        cevaplar.cevaplayan_soyad = cavaplayan_soyad;
                        cevaplar.cevaplayan_email = cevaplayan_email;
                        ankDB.Kaydet();

                    }
                }
                else if (soru_tipi_id == 3)
                {
                    DataSet ds_soru_tipi3_secenekler = ankDB.AcikAnketSoruTipi3SecenekGetirSoruyaGoreDataSet(soru_id);
                    DataSet ds_soru_tipi3_kolonlar = ankDB.AcikAnketSoruTipi3SecenekKolonGetirSoruyaGoreDataSet(soru_id);
                    ankDB.AcikAnketSoruTipi3CevapSilSoruyaKeyeGore(soru_id, cevap_key);
                    foreach (DataRow dr in ds_soru_tipi3_secenekler.Tables[0].Rows)
                    {
                        foreach (DataRow dr2 in ds_soru_tipi3_kolonlar.Tables[0].Rows)
                        {
                            string id = "secenek_tip3_" + dr["soru_secenek_uid"].ToString() + "_" + dr2["soru_secenek_kolon_uid"].ToString();

                            HtmlInputRadioButton radio = (HtmlInputRadioButton)BaseClasses.BaseFunctions.getInstance().FindControlInRepeater(id, this.rptSurveySorulari);
                            
                            sbr_soru_tipi_3_cevaplari cevaplar = new sbr_soru_tipi_3_cevaplari();
                            ankDB.AcikAnketSoruTipi3CevapEkle(cevaplar);

                            if (radio.Checked == true)
                                cevaplar.cevap = true;
                            else
                                cevaplar.cevap = false;

                            cevaplar.cavaplama_tarihi = DateTime.Now;
                            cevaplar.soru_secenek_uid = Guid.Parse(dr["soru_secenek_uid"].ToString());
                            cevaplar.soru_secenek_kolon_uid = Guid.Parse(dr2["soru_secenek_kolon_uid"].ToString());

                            HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;
                            SessionInfo si = new SessionInfo();
                            cevaplar.session_id = HttpContext.Current.Session.SessionID;
                            cevaplar.host_name = HttpContext.Current.Request.UserHostName;
                            cevaplar.user_host_adres = HttpContext.Current.Request.UserHostAddress;
                            cevaplar.browser = bc.Browser;
                            cevaplar.platform = bc.Platform;
                            cevaplar.version = bc.Version;
                            cevaplar.soru_uid = soru_id;
                            cevaplar.cevap_key = cevap_key;
                            cevaplar.cevaplayan_ad = cevaplayan_ad;
                            cevaplar.cevaplayan_soyad = cavaplayan_soyad;
                            cevaplar.cevaplayan_email = cevaplayan_email;
                            ankDB.Kaydet();
                        }
                    }
                }
                else if (soru_tipi_id == 4)
                {
                    DataSet ds_soru_tipi4_secenekler = ankDB.AcikAnketSoruTipi4SecenekGetirSoruyaGoreDataSet(soru_id);
                    ankDB.AcikAnketSoruTipi4CevapSilSoruyaKeyeGore(soru_id, cevap_key);
                    foreach (DataRow dr in ds_soru_tipi4_secenekler.Tables[0].Rows)
                    {
                        string id = "secenek_tip4_" + dr["soru_secenek_uid"].ToString();
                        HtmlInputText text = new HtmlInputText();
                        HtmlTextArea text_area = new HtmlTextArea();

                        if (dr_anket["soru_tek_satir"] != System.DBNull.Value && (dr_anket["soru_tek_satir"].ToString() == "true" || dr_anket["soru_tek_satir"].ToString() == "True" || dr_anket["soru_tek_satir"].ToString() == "TRUE"))
                        {
                            text = (HtmlInputText)BaseClasses.BaseFunctions.getInstance().FindControlInRepeater(id, this.rptSurveySorulari);
                        }
                        else
                        {
                            text_area = (HtmlTextArea)BaseClasses.BaseFunctions.getInstance().FindControlInRepeater(id, this.rptSurveySorulari);
                        }
   
                        sbr_soru_tipi_4_cevaplari cevaplar = new sbr_soru_tipi_4_cevaplari();
                        ankDB.AcikAnketSoruTipi4CevapEkle(cevaplar);

                        if (dr_anket["soru_tek_satir"] != System.DBNull.Value && (dr_anket["soru_tek_satir"].ToString() == "true" || dr_anket["soru_tek_satir"].ToString() == "True" || dr_anket["soru_tek_satir"].ToString() == "TRUE"))
                        {
                            cevaplar.cevap = text.Value.ToString();
                        }
                        else
                        {
                            cevaplar.cevap = text_area.Value.ToString();
                        }
                        cevaplar.cavaplama_tarihi = DateTime.Now;
                        cevaplar.soru_secenek_uid = Guid.Parse(dr["soru_secenek_uid"].ToString());

                        HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;
                        SessionInfo si = new SessionInfo();
                        cevaplar.session_id = HttpContext.Current.Session.SessionID;
                        cevaplar.host_name = HttpContext.Current.Request.UserHostName;
                        cevaplar.user_host_adres = HttpContext.Current.Request.UserHostAddress;
                        cevaplar.browser = bc.Browser;
                        cevaplar.platform = bc.Platform;
                        cevaplar.version = bc.Version;
                        cevaplar.soru_uid = soru_id;
                        cevaplar.cevap_key = cevap_key;
                        cevaplar.cevaplayan_ad = cevaplayan_ad;
                        cevaplar.cevaplayan_soyad = cavaplayan_soyad;
                        cevaplar.cevaplayan_email = cevaplayan_email;
                        ankDB.Kaydet();

                    }
                }
                else if (soru_tipi_id == 7)
                {
                    DataSet ds_soru_tipi7_secenekler = ankDB.AcikAnketSoruTipi7SecenekGetirSoruyaGoreDataSet(soru_id);
                    ankDB.AcikAnketSoruTipi7CevapSilSoruyaKeyeGore(soru_id, cevap_key);
                    foreach (DataRow dr in ds_soru_tipi7_secenekler.Tables[0].Rows)
                    {
                        string id = "secenek_tip7_" + dr["soru_secenek_uid"].ToString();
                        HtmlInputText text = (HtmlInputText)BaseClasses.BaseFunctions.getInstance().FindControlInRepeater(id, this.rptSurveySorulari);

                        sbr_soru_tipi_7_cevaplari cevaplar = new sbr_soru_tipi_7_cevaplari();
                        ankDB.AcikAnketSoruTipi7CevapEkle(cevaplar);
                        cevaplar.cevap = text.Value.ToString();
                        cevaplar.cavaplama_tarihi = DateTime.Now;
                        cevaplar.soru_secenek_uid = Guid.Parse(dr["soru_secenek_uid"].ToString());

                        HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;
                        SessionInfo si = new SessionInfo();
                        cevaplar.session_id = HttpContext.Current.Session.SessionID;
                        cevaplar.host_name = HttpContext.Current.Request.UserHostName;
                        cevaplar.user_host_adres = HttpContext.Current.Request.UserHostAddress;
                        cevaplar.browser = bc.Browser;
                        cevaplar.platform = bc.Platform;
                        cevaplar.version = bc.Version;
                        cevaplar.soru_uid = soru_id;
                        cevaplar.cevap_key = cevap_key;
                        cevaplar.cevaplayan_ad = cevaplayan_ad;
                        cevaplar.cevaplayan_soyad = cavaplayan_soyad;
                        cevaplar.cevaplayan_email = cevaplayan_email;
                        ankDB.Kaydet();

                    }
                }
                else if (soru_tipi_id == 5)
                {
                    DataSet ds_soru_tipi5_secenekler = ankDB.AcikAnketSoruTipi5SecenekGetirSoruyaGoreDataSet(soru_id);
                    ankDB.AcikAnketSoruTipi5CevapSilSoruyaKeyeGore(soru_id, cevap_key);

                    foreach (DataRow dr in ds_soru_tipi5_secenekler.Tables[0].Rows)
                    {
                        string id = "secenek_tip5_" + dr["soru_secenek_uid"].ToString();
                        HtmlInputRadioButton radio = (HtmlInputRadioButton)BaseClasses.BaseFunctions.getInstance().FindControlInRepeater(id, this.rptSurveySorulari);
                        sbr_soru_tipi_5_cevaplari cevaplar = new sbr_soru_tipi_5_cevaplari();
                        ankDB.AcikAnketSoruTipi5CevapEkle(cevaplar);

                        if (radio.Checked == true)
                            cevaplar.cevap = true;
                        else
                            cevaplar.cevap = false;

                        cevaplar.cavaplama_tarihi = DateTime.Now;
                        cevaplar.soru_secenek_uid = Guid.Parse(dr["soru_secenek_uid"].ToString());

                        HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;
                        SessionInfo si = new SessionInfo();
                        cevaplar.session_id = HttpContext.Current.Session.SessionID;
                        cevaplar.host_name = HttpContext.Current.Request.UserHostName;
                        cevaplar.user_host_adres = HttpContext.Current.Request.UserHostAddress;
                        cevaplar.browser = bc.Browser;
                        cevaplar.platform = bc.Platform;
                        cevaplar.version = bc.Version;
                        cevaplar.soru_uid = soru_id;
                        cevaplar.cevap_key = cevap_key;
                        cevaplar.cevaplayan_ad = cevaplayan_ad;
                        cevaplar.cevaplayan_soyad = cavaplayan_soyad;
                        cevaplar.cevaplayan_email = cevaplayan_email;
                        ankDB.Kaydet();

                    }
                }
                else if (soru_tipi_id == 6)
                {
                    DataSet ds_soru_tipi6_secenekler = ankDB.AcikAnketSoruTipi6SecenekGetirSoruyaGoreDataSet(soru_id);
                    ankDB.AcikAnketSoruTipi6CevapSilSoruyaKeyeGore(soru_id, cevap_key);

                    foreach (DataRow dr in ds_soru_tipi6_secenekler.Tables[0].Rows)
                    {
                        string id = "secenek_tip6_" + dr["soru_secenek_uid"].ToString();
                        HtmlInputRadioButton radio = (HtmlInputRadioButton)BaseClasses.BaseFunctions.getInstance().FindControlInRepeater(id, this.rptSurveySorulari);
                        sbr_soru_tipi_6_cevaplari cevaplar = new sbr_soru_tipi_6_cevaplari();
                        ankDB.AcikAnketSoruTipi6CevapEkle(cevaplar);

                        if (radio.Checked == true)
                            cevaplar.cevap = true;
                        else
                            cevaplar.cevap = false;

                        cevaplar.cavaplama_tarihi = DateTime.Now;
                        cevaplar.soru_secenek_uid = Guid.Parse(dr["soru_secenek_uid"].ToString());

                        HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;
                        SessionInfo si = new SessionInfo();
                        cevaplar.session_id = HttpContext.Current.Session.SessionID;
                        cevaplar.host_name = HttpContext.Current.Request.UserHostName;
                        cevaplar.user_host_adres = HttpContext.Current.Request.UserHostAddress;
                        cevaplar.browser = bc.Browser;
                        cevaplar.platform = bc.Platform;
                        cevaplar.version = bc.Version;
                        cevaplar.soru_uid = soru_id;
                        cevaplar.cevap_key = cevap_key;
                        cevaplar.cevaplayan_ad = cevaplayan_ad;
                        cevaplar.cevaplayan_soyad = cavaplayan_soyad;
                        cevaplar.cevaplayan_email = cevaplayan_email;
                        ankDB.Kaydet();

                    }
                }
                else if (soru_tipi_id == 8)
                {
                    DataSet ds_soru_tipi8_secenekler = ankDB.AcikAnketSoruTipi8SecenekGetirSoruyaGoreDataSet(soru_id);
                    ankDB.AcikAnketSoruTipi8CevapSilSoruyaKeyeGore(soru_id, cevap_key);
                    foreach (DataRow dr in ds_soru_tipi8_secenekler.Tables[0].Rows)
                    {
                        string id = "secenek_tip8_" + dr["soru_secenek_uid"].ToString();
                        HtmlInputText sayisal = (HtmlInputText)BaseClasses.BaseFunctions.getInstance().FindControlInRepeater(id, this.rptSurveySorulari);

                        sbr_soru_tipi_8_cevaplari cevaplar = new sbr_soru_tipi_8_cevaplari();
                        ankDB.AcikAnketSoruTipi8CevapEkle(cevaplar);

                        decimal sayisal_cevap = 0;


                        if (sayisal.Value != null && sayisal.Value.ToString() != "")
                            sayisal_cevap = Convert.ToDecimal(sayisal.Value);
                        else
                            sayisal_cevap = Convert.ToDecimal("0");
                        
                        cevaplar.cevap = sayisal_cevap;
                        
                        cevaplar.cavaplama_tarihi = DateTime.Now;
                        cevaplar.soru_secenek_uid = Guid.Parse(dr["soru_secenek_uid"].ToString());

                        HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;
                        SessionInfo si = new SessionInfo();
                        cevaplar.session_id = HttpContext.Current.Session.SessionID;
                        cevaplar.host_name = HttpContext.Current.Request.UserHostName;
                        cevaplar.user_host_adres = HttpContext.Current.Request.UserHostAddress;
                        cevaplar.browser = bc.Browser;
                        cevaplar.platform = bc.Platform;
                        cevaplar.version = bc.Version;
                        cevaplar.soru_uid = soru_id;
                        cevaplar.cevap_key = cevap_key;
                        cevaplar.cevaplayan_ad = cevaplayan_ad;
                        cevaplar.cevaplayan_soyad = cavaplayan_soyad;
                        cevaplar.cevaplayan_email = cevaplayan_email;
                        ankDB.Kaydet();

                    }
                }
                else if (soru_tipi_id == 9)
                {
                    DataSet ds_soru_tipi9_secenekler = ankDB.AcikAnketSoruTipi9SecenekGetirSoruyaGoreDataSet(soru_id);
                    ankDB.AcikAnketSoruTipi9CevapSilSoruyaKeyeGore(soru_id, cevap_key);
                    foreach (DataRow dr in ds_soru_tipi9_secenekler.Tables[0].Rows)
                    {
                        string id = "secenek_tip9_" + dr["soru_secenek_uid"].ToString();
                        HtmlInputText tarih = (HtmlInputText)BaseClasses.BaseFunctions.getInstance().FindControlInRepeater(id, this.rptSurveySorulari);

                        sbr_soru_tipi_9_cevaplari cevaplar = new sbr_soru_tipi_9_cevaplari();
                        ankDB.AcikAnketSoruTipi9CevapEkle(cevaplar);

                        if (tarih.Value != null && tarih.Value.ToString() != "")
                            cevaplar.cevap = Convert.ToDateTime(tarih.Value, new System.Globalization.CultureInfo("tr-TR"));
                        
                        cevaplar.cavaplama_tarihi = DateTime.Now;
                        cevaplar.soru_secenek_uid = Guid.Parse(dr["soru_secenek_uid"].ToString());

                        HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;
                        SessionInfo si = new SessionInfo();
                        cevaplar.session_id = HttpContext.Current.Session.SessionID;
                        cevaplar.host_name = HttpContext.Current.Request.UserHostName;
                        cevaplar.user_host_adres = HttpContext.Current.Request.UserHostAddress;
                        cevaplar.browser = bc.Browser;
                        cevaplar.platform = bc.Platform;
                        cevaplar.version = bc.Version;
                        cevaplar.soru_uid = soru_id;
                        cevaplar.cevap_key = cevap_key;
                        cevaplar.cevaplayan_ad = cevaplayan_ad;
                        cevaplar.cevaplayan_soyad = cavaplayan_soyad;
                        cevaplar.cevaplayan_email = cevaplayan_email;
                        ankDB.Kaydet();

                    }
                }
                else if (soru_tipi_id == 10)
                {
                    DataSet ds_soru_tipi10_secenekler = ankDB.AcikAnketSoruTipi10SecenekGetirSoruyaGoreDataSet(soru_id);
                    ankDB.AcikAnketSoruTipi10CevapSilSoruyaKeyeGore(soru_id, cevap_key);
                    foreach (DataRow dr in ds_soru_tipi10_secenekler.Tables[0].Rows)
                    {
                        string id = "secenek_tip10_" + dr["soru_secenek_uid"].ToString();
                        HtmlInputText tel = (HtmlInputText)BaseClasses.BaseFunctions.getInstance().FindControlInRepeater(id, this.rptSurveySorulari);

                        sbr_soru_tipi_10_cevaplari cevaplar = new sbr_soru_tipi_10_cevaplari();
                        ankDB.AcikAnketSoruTipi10CevapEkle(cevaplar);

                        if (tel.Value != null && tel.Value.ToString() != "")
                            cevaplar.cevap = tel.Value.Replace('(', ' ').Replace(')', ' ').Replace('-', ' ').Trim();

                        cevaplar.cavaplama_tarihi = DateTime.Now;
                        cevaplar.soru_secenek_uid = Guid.Parse(dr["soru_secenek_uid"].ToString());

                        HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;
                        SessionInfo si = new SessionInfo();
                        cevaplar.session_id = HttpContext.Current.Session.SessionID;
                        cevaplar.host_name = HttpContext.Current.Request.UserHostName;
                        cevaplar.user_host_adres = HttpContext.Current.Request.UserHostAddress;
                        cevaplar.browser = bc.Browser;
                        cevaplar.platform = bc.Platform;
                        cevaplar.version = bc.Version;
                        cevaplar.soru_uid = soru_id;
                        cevaplar.cevap_key = cevap_key;
                        cevaplar.cevaplayan_ad = cevaplayan_ad;
                        cevaplar.cevaplayan_soyad = cavaplayan_soyad;
                        cevaplar.cevaplayan_email = cevaplayan_email;
                        ankDB.Kaydet();

                    }
                }
                else if (soru_tipi_id == 11)
                {
                    DataSet ds_soru_tipi11_secenekler = ankDB.AcikAnketSoruTipi11SecenekGetirSoruyaGoreDataSet(soru_id);
                    ankDB.AcikAnketSoruTipi11CevapSilSoruyaKeyeGore(soru_id, cevap_key);
                    foreach (DataRow dr in ds_soru_tipi11_secenekler.Tables[0].Rows)
                    {
                        string id = "secenek_tip11_" + dr["soru_secenek_uid"].ToString();
                        HtmlInputText tel = (HtmlInputText)BaseClasses.BaseFunctions.getInstance().FindControlInRepeater(id, this.rptSurveySorulari);

                        sbr_soru_tipi_11_cevaplari cevaplar = new sbr_soru_tipi_11_cevaplari();
                        ankDB.AcikAnketSoruTipi11CevapEkle(cevaplar);

                        if (tel.Value != null && tel.Value.ToString() != "" && BaseClasses.BaseFunctions.getInstance().IsEmailValid(tel.Value.ToString()))
                            cevaplar.cevap = tel.Value.ToString();

                        cevaplar.cavaplama_tarihi = DateTime.Now;
                        cevaplar.soru_secenek_uid = Guid.Parse(dr["soru_secenek_uid"].ToString());

                        HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;
                        SessionInfo si = new SessionInfo();
                        cevaplar.session_id = HttpContext.Current.Session.SessionID;
                        cevaplar.host_name = HttpContext.Current.Request.UserHostName;
                        cevaplar.user_host_adres = HttpContext.Current.Request.UserHostAddress;
                        cevaplar.browser = bc.Browser;
                        cevaplar.platform = bc.Platform;
                        cevaplar.version = bc.Version;
                        cevaplar.soru_uid = soru_id;
                        cevaplar.cevap_key = cevap_key;
                        cevaplar.cevaplayan_ad = cevaplayan_ad;
                        cevaplar.cevaplayan_soyad = cavaplayan_soyad;
                        cevaplar.cevaplayan_email = cevaplayan_email;
                        ankDB.Kaydet();

                    }
                }
            }

            if (anket.anket_tipi_id == 1)
            {
                sbr_anket_yayinlama_mail_gonderi_aktivasyon aktivasyon2 = ankDB.MailGonderiAktivasyonGetirKeyeGore(key);

                if (aktivasyon2.ankete_cevap_verildi == null || aktivasyon2.ankete_cevap_verildi == false)
                {
                    aktivasyon2.ankete_cevap_verildi = true;
                    aktivasyon2.ankete_cevap_verilme_tarihi = DateTime.Now;
                    ankDB.Kaydet();
                }
            }
        }

        protected void btnFinish2_Click(object sender, EventArgs e)
        {
            Kaydet();
            BindSurveySorulari(anket_uid);

            if (!this.rptSurveySayfalama.Sayfa.IsLastPage)
                this.rptSurveySayfalama.SayfaNo += 1;

            this.rptSurveySayfalama.Bind();
            BindSurveySorulari(anket_uid);
        }

        protected void btnClose2_Click(object sender, EventArgs e)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            Kaydet();
            sbr_anket anket = ankDB.SurveyGetir(anket_uid);

            BindSurveySorulari(anket_uid);

            string cevap_key = "";

            if (anket.anket_tipi_id == 1)
            {
                cevap_key = key;
            }
            else
            {
                if (CookieKey != "")
                    cevap_key = CookieKey;
                else
                    cevap_key = HttpContext.Current.Session.SessionID;
            }

            bool hepsi_cevaplandi = ankDB.HepsiCevaplandimi(anket_uid, cevap_key);

            if (hepsi_cevaplandi == true && anket.anket_tipi_id == 1)
            {
                Response.Redirect("AnketBitti.aspx?tip=1&anket_uid=" + anket_uid + "&key=" + key);
            }
            else if (hepsi_cevaplandi == true && anket.anket_tipi_id != 1)
            {
                if (Request.Cookies["ankCookie_" + anket_uid.ToString()] != null)
                {
                    if (Request.Cookies["ankCookie_" + anket_uid.ToString()]["visitKey"] != null)
                    {
                        key = Request.Cookies["ankCookie_" + anket_uid.ToString()]["visitKey"].ToString();
                        CookieKey = key;
                    }
                }

                Response.Redirect("AnketBitti.aspx?tip=2&anket_uid=" + anket_uid + "&key=" + key);
            }
            else
            {
                string str = "";
                DataSet ds_cevap_durumu = ankDB.SurveySoruCevapDurumuDataSet(anket_uid, cevap_key);
                foreach (DataRow dr in ds_cevap_durumu.Tables[0].Rows)
                {
                    if (dr["cevap_durumu"] != System.DBNull.Value && dr["soru_zorunlu"] != System.DBNull.Value && dr["cevap_durumu"].ToString() == "0" && dr["soru_zorunlu"].ToString() == "True")
                        str += "Lütfen " + dr["RowNumber"].ToString() + ". Soruyu Cevaplayınız<br>";
                }

                this.CevapKotrolu.Text += str;
                this.error_message.Visible = true;
            }
        }

        protected void btnTestiOnayla1_Click(object sender, EventArgs e)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            sbr_anket_test_mail_gonderi_kisi_tarihcesi test = ankDB.TestTarihceGetirKeyeGore(test_key);

            if (test != null)
            {
                if (ddlTestSonucu.SelectedValue != null && ddlTestSonucu.SelectedValue.ToString() != "")
                {
                    test.anket_test_sonucu_id = Convert.ToInt32(ddlTestSonucu.SelectedValue.ToString());
                    test.anket_test_sonucu = txtSurveyTestiSonucAciklamasi.Text;
                    ankDB.Kaydet();
                }
            }

        }
    }
}