using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaseWebSite.Models;
using System.Data;

namespace BaseWebSite.Survey
{
    public partial class AnketiGoster : System.Web.UI.Page
    {
        public Guid anket_uid
        {
            get { return (ViewState["anket_uid"] != null ? Guid.Parse(ViewState["anket_uid"].ToString()) : Guid.Empty); }
            set { ViewState["anket_uid"] = value; }
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



            }

            BindSurveySorulari(anket_uid);
        }



        protected void BindSurveySorulari(Guid anket_uid)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            DataSet ds = ankDB.SurveySoruGetirSurveyGoreDataSet(anket_uid);

            this.rptSurveySorulari.DataSource = ds;
            this.rptSurveySorulari.DataBind();
        }

        public string SorulariOlustur(object soru_uid)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            return ankDB.SoruGorunumuOlustur(soru_uid);
        }
    }
}