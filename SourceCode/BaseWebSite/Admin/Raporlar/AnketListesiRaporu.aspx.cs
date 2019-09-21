using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace BaseWebSite.Admin.Raporlar
{
    public partial class AnketListesiRaporu : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (BaseDB.SessionContext.Current == null || BaseDB.SessionContext.Current.ActiveUser == null)
            {
                Response.Redirect("~/Login.aspx");
            }

            if (!IsPostBack)
            {
                InitiliazeCombos();
                ShowReport();
            }
        }

        protected void InitiliazeCombos()
        {
            DataSet ds = BaseDB.DBManager.AppConnection.GetDataSet("select 0 as anket_durumu_id,'All' as anket_durumu from sbr_ref_anket_durumu union select anket_durumu_id,anket_durumu from sbr_ref_anket_durumu");
            this.ddlSurveyDurumu.DataSource = ds;
            this.ddlSurveyDurumu.DataTextField = "anket_durumu";
            this.ddlSurveyDurumu.DataValueField = "anket_durumu_id";
            this.ddlSurveyDurumu.DataBind();
        }

        private void ShowReport()
        {
            Parameter param_anket_durumu_id = new Parameter("anket_durumu_id", DbType.Int32, this.ddlSurveyDurumu.SelectedValue.ToString());

            this.ObjectDataSource1.SelectParameters["anket_durumu_id"] = param_anket_durumu_id;
            this.ObjectDataSource1.DataBind();

            ReportViewer1.LocalReport.Refresh();
        }

        protected void ddlSurveyDurumu_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowReport();
        }
    }
}