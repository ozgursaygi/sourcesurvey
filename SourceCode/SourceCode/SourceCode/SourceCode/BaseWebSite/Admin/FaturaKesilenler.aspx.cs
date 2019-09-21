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
    public partial class FaturaKesilenler : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (BaseDB.SessionContext.Current == null || BaseDB.SessionContext.Current.ActiveUser == null)
            {
                Response.Redirect("~/Login.aspx");
            }

            if (!IsPostBack)
            {
                BindPaketler();
            }
        }

        protected new void Page_LoadComplete(object sender, EventArgs e)
        {
            BindPaketler();
        }

        protected void BindPaketler()
        {
            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();
            DataSet ds = gnlDB.PaketAlimlariFaturaKesilenGetirDataSet();

            this.RepeaterPaging1.dataSource = ds;
            this.RepeaterPaging1.SayfaBuyuklugu = 10;
            this.RepeaterPaging1.Bind();
            this.rptSurveyler.DataSource = this.RepeaterPaging1.Sayfa;
            this.rptSurveyler.DataBind();
        }
    }
}