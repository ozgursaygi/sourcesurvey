using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BaseWebSite.Models;
using BaseClasses;
using System.Collections;

namespace BaseWebSite.Survey
{
    public partial class MainPage : System.Web.UI.Page
    {
        private int MenuId
        {
            get { return (ViewState["MenuId"] != null ? Convert.ToInt32(ViewState["MenuId"]) : 0); }
            set { ViewState["MenuId"] = value; }
        }

        private int anket_durumu_id
        {
            get { return (ViewState["anket_durumu_id"] != null ? Convert.ToInt32(ViewState["anket_durumu_id"]) : 0); }
            set { ViewState["anket_durumu_id"] = value; }
        }

        public string grup_uid
        {
            get { return (ViewState["grup_uid"] != null ? ViewState["grup_uid"].ToString() :""); }
            set { ViewState["grup_uid"] = value; }
        }


        public string user_uid
        {
            get { return (ViewState["user_uid"] != null ? ViewState["user_uid"].ToString() : ""); }
            set { ViewState["user_uid"] = value; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["menu_id"] != null && Request.QueryString["menu_id"].ToString() != "")
            {
                MenuId = Convert.ToInt32(Request.QueryString["menu_id"]);
            }
            else
            {
                MenuId = 1;
            }

            if(BaseDB.SessionContext.Current==null || BaseDB.SessionContext.Current.ActiveUser==null)
            {
                Response.Redirect("~/Login.aspx?menu_id=" + MenuId);
            }

            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            
            if (!IsPostBack)
            {
                this.anket_durumu_id = 0;
                if (Request.QueryString["anket_durumu_id"] != null && Request.QueryString["anket_durumu_id"].ToString() != "")
                {
                    anket_durumu_id = Convert.ToInt32(Request.QueryString["anket_durumu_id"]);
                }

                switch (anket_durumu_id)
                {
                    case 0:
                        this.LinkButtonHeader.Text = "All";
                        break;
                    case 1:
                        this.LinkButtonHeader.Text = "Opened";
                        break;
                    case 2:
                        this.LinkButtonHeader.Text = "Closed";
                        break;
                    case 3:
                        this.LinkButtonHeader.Text = "Archived";
                        break;
                    case 4:
                        this.LinkButtonHeader.Text = "Published";
                        break;
                }             
                
                InitiliazeCombos();
                BindSurveyler(anket_durumu_id);
            }
            
            if (Request["__EVENTTARGET"] == "Kapat")
            {
                string degerler = Request["__EVENTARGUMENT"].ToString();
                string[] arrDegerler = degerler.Replace("^#^", "^").Split('^');
                string anket_uid = arrDegerler[0];
                string aciklama = arrDegerler[1];

                ankDB.SurveyDurumuDegistir((int)BaseWebSite.Models.anket_durumu.Kapali, Guid.Parse(anket_uid), aciklama,BaseDB.SessionContext.Current.ActiveUser.UserUid);
            }

            if (Request["__EVENTTARGET"] == "ArsiveGonder")
            {
                string degerler = Request["__EVENTARGUMENT"].ToString();
                string[] arrDegerler = degerler.Replace("^#^", "^").Split('^');
                string anket_uid = arrDegerler[0];
                string aciklama = arrDegerler[1];

                ankDB.SurveyDurumuDegistir((int)BaseWebSite.Models.anket_durumu.Arsivde, Guid.Parse(anket_uid), aciklama, BaseDB.SessionContext.Current.ActiveUser.UserUid);
            }

            if (Request["__EVENTTARGET"] == "ArsivdenCikart")
            {
                string anket_uid = Request["__EVENTARGUMENT"].ToString();

                ankDB.SurveyDurumuDegistir((int)BaseWebSite.Models.anket_durumu.Acik, Guid.Parse(anket_uid), "Removed From Archive", BaseDB.SessionContext.Current.ActiveUser.UserUid);
            }

            
            FormuDuzenle();
        }

        protected new void Page_LoadComplete(object sender, EventArgs e)
        {
            BindSurveyler(anket_durumu_id);

            this.user_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid.ToString();

            if (ddlgrup.SelectedValue != null)
                this.grup_uid = ddlgrup.SelectedValue.ToString();
            else
                this.grup_uid = "";
        }

        protected void FormuDuzenle()
        {
            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();

            if (!gnlDB.HasUserGroup(BaseDB.SessionContext.Current.ActiveUser.UserUid))
            {
                this.ddlgrup.Visible = false;
            }
        }


        protected void InitiliazeCombos()
        {
            DataSet ds = BaseDB.DBManager.AppConnection.GetDataSet("select * from dbo.sbr_anket_kullanici_gruplari('" + BaseDB.SessionContext.Current.ActiveUser.UserUid + "')");
            this.ddlgrup.DataSource = ds;
            this.ddlgrup.DataTextField = "group_name";
            this.ddlgrup.DataValueField = "group_uid";
            this.ddlgrup.DataBind();
        }

        protected void BindSurveyler(int anket_durumu_id)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            string criteria = "";
            string text="";

            if (Request["__EVENTTARGET"] == "HizliAramadan")
            {
                text = Request["__EVENTARGUMENT"].ToString();
                 this.rptSurveylerPaging.SayfaNo = 0;
                this.search_text.Value = text;
            }

            if (text != "")
            {
                DataSet dscriteria = BaseDB.DBManager.AppConnection.ExecuteSP("Search", new ArrayList { "Content", "Object" }, new ArrayList { text, "sbr_anket_v" });

                if (dscriteria.Tables[0].Rows.Count > 0)
                {
                    criteria = dscriteria.Tables[0].Rows[0]["Criteria"].ToString();
                }
            }

            Guid grup_uid = Guid.Empty;
            
            if (ddlgrup.SelectedValue != null && ddlgrup.SelectedValue.ToString() != "")
            {
                grup_uid = Guid.Parse(ddlgrup.SelectedValue.ToString());
            }

            DataSet ds = ankDB.SurveyGetirDurumaGoreDataSet(anket_durumu_id,criteria,grup_uid);

            this.rptSurveylerPaging.dataSource = ds;
            this.rptSurveylerPaging.SayfaBuyuklugu = 10;
            this.rptSurveylerPaging.Bind();
            this.rptSurveyler.DataSource = this.rptSurveylerPaging.Sayfa;
            this.rptSurveyler.DataBind();
        }

        protected void tumu_Click(object sender, EventArgs e)
        {
            this.LinkButtonHeader.Text = "All";
            anket_durumu_id = 0;
            BindSurveyler(0);
        }

        protected void olcum_anketleri_Click(object sender, EventArgs e)
        {
            this.LinkButtonHeader.Text = "Surveys";
            anket_durumu_id = 1;
            BindSurveyler(1);
        }

        protected void aciklar_Click(object sender, EventArgs e)
        {
            this.LinkButtonHeader.Text = "Open Surveys";
            this.rptSurveylerPaging.SayfaNo = 0;
            anket_durumu_id = 1;
            BindSurveyler(1);
        }

        protected void kapalilar_Click(object sender, EventArgs e)
        {
            this.LinkButtonHeader.Text = "Closed Surveys";
            this.rptSurveylerPaging.SayfaNo = 0;
            anket_durumu_id = 2;
            BindSurveyler(2);
        }

        protected void arsiv_Click(object sender, EventArgs e)
        {
            this.LinkButtonHeader.Text = "Archived Surveys";
            this.rptSurveylerPaging.SayfaNo = 0;
            anket_durumu_id = 3;
            BindSurveyler(3);
        }

        protected void yayindakiler_Click(object sender, EventArgs e)
        {
            this.LinkButtonHeader.Text = "Published Surveys";
            this.rptSurveylerPaging.SayfaNo = 0;
            anket_durumu_id = 4;
            BindSurveyler(4);
        }

        public string SurveyDurumu(object anket_durum_tipi_id)
        {
            string result = "";

            if (anket_durum_tipi_id != null && anket_durum_tipi_id.ToString() == ((int)BaseWebSite.Models.anket_durumu.Acik).ToString())
            {
                result = "<span class=\"ButtonStyle\">Open</span>";
            }
            else if (anket_durum_tipi_id != null && anket_durum_tipi_id.ToString() == ((int)BaseWebSite.Models.anket_durumu.Kapali).ToString())
            {
                result = "<span class=\"ButtonStyle\">Closed</span>";
            }
            else if (anket_durum_tipi_id != null && anket_durum_tipi_id.ToString() == ((int)BaseWebSite.Models.anket_durumu.Arsivde).ToString())
            {
                result = "<span class=\"ButtonStyle\">Archived</span>";
            }
            else if (anket_durum_tipi_id != null && anket_durum_tipi_id.ToString() == ((int)BaseWebSite.Models.anket_durumu.Yayinda).ToString())
            {
                result = "<span class=\"ButtonStyle\">Published</span>";
            }

            return result;
        }

        protected void ddlgrup_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSurveyler(anket_durumu_id);
        }

 

        protected string SorulariOlustur(object soru_uid)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            return ankDB.SoruGorunumuOlustur(soru_uid);
        }

        public string ArsiveGonder(object anket_uid)
        {
            string result = "";

            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            sbr_anket anket = ankDB.SurveyGetir(Guid.Parse(anket_uid.ToString()));

            if (anket != null)
            {
                if (anket.anket_durumu_id == (int)BaseWebSite.Models.anket_durumu.Arsivde)
                {

                    result = "<a href=\"#\" class=\"arsivdenCikart linkButtonStyle\" onclick=\"ArsivdenCikart('" + anket_uid + "');\">Remove From Archive</a>";
                }
                else
                {
                    result = "<a href=\"#\" class=\"arsiveGonder linkButtonStyle\" onclick=\"ArsiveGonder('" + anket_uid + "');\">Send To Archive</a>";
                }
            }

            return result;
        }
    }
}