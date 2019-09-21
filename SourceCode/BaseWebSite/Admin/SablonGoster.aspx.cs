using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaseWebSite.Models;
using System.Data;

namespace BaseWebSite.Admin
{
    public partial class SablonGoster : System.Web.UI.Page
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


              
            }

            BindSablonSorulari(sablon_uid);
            
        }

        protected void BindSablonSorulari(Guid sablon_uid)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            DataSet ds = ankDB.SablonSoruGetirSablonGoreDataSet(sablon_uid);

            this.rptSablonSorulari.DataSource = ds;
            this.rptSablonSorulari.DataBind();
        }

        protected string SablonSorulariOlustur(object soru_uid)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            return ankDB.SablonSoruGorunumuOlustur(soru_uid);

        }
    }
}