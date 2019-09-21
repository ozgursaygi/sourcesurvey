using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaseWebSite.Models;
using System.Data;
using System.Collections;

namespace BaseWebSite.Admin
{
    public partial class SablonTanimlari : System.Web.UI.Page
    {
        private int sablon_durumu_id
        {
            get { return (ViewState["sablon_durumu_id"] != null ? Convert.ToInt32(ViewState["sablon_durumu_id"]) : 0); }
            set { ViewState["sablon_durumu_id"] = value; }
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
                this.LinkButtonHeader.Text = "Tümü";
                this.sablon_durumu_id = 0;
                InitiliazeCombos();
                BindSurveyler(0);

            }


        }


        protected new void Page_LoadComplete(object sender, EventArgs e)
        {
            BindSurveyler(sablon_durumu_id);
        }

        protected void FormuDuzenle()
        {
            
        }


        protected void InitiliazeCombos()
        {
           
        }

        protected void BindSurveyler(int anket_durumu_id)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            string criteria = "";
            string text = "";

            if (Request["__EVENTTARGET"] == "HizliAramadan")
            {
                text = Request["__EVENTARGUMENT"].ToString();
                this.rptSurveylerPaging.SayfaNo = 0;
                this.search_text.Value = text;
            }

            if (text != "")
            {
                DataSet dscriteria = BaseDB.DBManager.AppConnection.ExecuteSP("Search", new ArrayList { "Content", "Object" }, new ArrayList { text, "sbr_sablon_v" });

                if (dscriteria.Tables[0].Rows.Count > 0)
                {
                    criteria = dscriteria.Tables[0].Rows[0]["Criteria"].ToString();
                }
            }

           
            DataSet ds = ankDB.SablonGetirDurumaGoreDataSet(anket_durumu_id, criteria, Guid.Empty);

            this.rptSurveylerPaging.dataSource = ds;
            this.rptSurveylerPaging.SayfaBuyuklugu = 10;
            this.rptSurveylerPaging.Bind();
            this.rptSurveyler.DataSource = this.rptSurveylerPaging.Sayfa;
            this.rptSurveyler.DataBind();
        }

        public string SablonDurumu(object anket_durum_tipi_id)
        {
            string result = "";

            if (anket_durum_tipi_id != null && anket_durum_tipi_id.ToString() == ((int)BaseWebSite.Models.sablon_durumu.Acik).ToString())
            {
                result = "<span class=\"acikButton\">Açık</span>";
            }
            else if (anket_durum_tipi_id != null && anket_durum_tipi_id.ToString() == ((int)BaseWebSite.Models.sablon_durumu.Kapali).ToString())
            {
                result = "<span class=\"yayindaButton\">Kapalı</span>";
            }
            

            return result;
        }

        protected void tumu_Click(object sender, EventArgs e)
        {
            this.LinkButtonHeader.Text = "Tümü";
            sablon_durumu_id = 0;
            this.rptSurveylerPaging.SayfaNo = 0;
            BindSurveyler(0);
        }

        
        protected void aciklar_Click(object sender, EventArgs e)
        {
            this.LinkButtonHeader.Text = "Açık Şablonlar";
            sablon_durumu_id = 1;
            this.rptSurveylerPaging.SayfaNo = 0;
            BindSurveyler(1);
        }

        protected void kapalilar_Click(object sender, EventArgs e)
        {
            this.LinkButtonHeader.Text = "Kapalı Şablonlar";
            sablon_durumu_id = 2;
            this.rptSurveylerPaging.SayfaNo = 0;
            BindSurveyler(2);
        }
    }
}