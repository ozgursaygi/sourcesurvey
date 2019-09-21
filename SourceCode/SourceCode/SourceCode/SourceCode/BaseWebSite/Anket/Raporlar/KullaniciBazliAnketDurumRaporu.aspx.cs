using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Microsoft.Reporting.WebForms;
using System.Collections;

namespace BaseWebSite.Anket.Raporlar
{
    public partial class KullaniciBazliAnketDurumRaporu : System.Web.UI.Page
    {
        public Guid anket_uid
        {
            get { return (ViewState["anket_uid"] != null ? Guid.Parse(ViewState["anket_uid"].ToString()) : Guid.Empty); }
            set { ViewState["anket_uid"] = value; }
        }

        public string grup_uid
        {
            get { return (ViewState["grup_uid"] != null ? ViewState["grup_uid"].ToString() : ""); }
            set { ViewState["grup_uid"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (BaseDB.SessionContext.Current == null || BaseDB.SessionContext.Current.ActiveUser == null)
            {
                Response.Redirect("~/Login.aspx");
            }


            if (!IsPostBack)
            {
                if (Request.QueryString["anket_uid"] != null && Request.QueryString["anket_uid"].ToString() != "")
                {
                    anket_uid = Guid.Parse(Request.QueryString["anket_uid"].ToString());
                }

                if (Request.QueryString["grup_uid"] != null && Request.QueryString["grup_uid"].ToString() != "")
                {
                    grup_uid = Request.QueryString["grup_uid"].ToString();
                }
                
                InitiliazeCombos();

                ddlgrup.SelectedValue = grup_uid;
                ddlAnket.SelectedValue = anket_uid.ToString();

                

            }

            ShowReport();
        }

        protected void InitiliazeCombos()
        {
            DataSet ds = BaseDB.DBManager.AppConnection.GetDataSet("select * from dbo.sbr_anket_kullanici_gruplari('" + BaseDB.SessionContext.Current.ActiveUser.UserUid + "')");
            this.ddlgrup.DataSource = ds;
            this.ddlgrup.DataTextField = "grup_adi";
            this.ddlgrup.DataValueField = "grup_uid";
            this.ddlgrup.DataBind();

            if (ddlgrup.SelectedValue != null && ddlgrup.SelectedValue.ToString() != "")
            {
                this.ddlAnket.DataSource = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_anket_v where grup_uid in ('" + grup_uid + "') and anket_tipi_id=1 order by anket_adi");
                this.ddlAnket.DataTextField = "anket_adi";
                this.ddlAnket.DataValueField = "anket_uid";
                this.ddlAnket.DataBind();
            }

           
        }

        private void ShowReport()
        {
            Parameter param_anket_uid2 = new Parameter("anket_uid", DbType.Guid, this.ddlAnket.SelectedValue.ToString());
            this.ObjectDataSource1.SelectParameters["anket_uid"] = param_anket_uid2;
            this.ObjectDataSource1.DataBind();

            ReportViewer1.LocalReport.Refresh();
            ReportViewer1.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(SetSubDataSource);
        }

        protected void ddlAnket_SelectedIndexChanged(object sender, EventArgs e)
        {

           

            ShowReport();
        }

        protected void ddlCevaplayan_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowReport();
        }

        protected void ddlgrup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlgrup.SelectedValue != null && ddlgrup.SelectedValue.ToString() != "")
            {
                this.ddlAnket.DataSource = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_anket_v where grup_uid in ('" + ddlgrup.SelectedValue.ToString() + "') and anket_tipi_id=1 order by anket_adi");
                this.ddlAnket.DataTextField = "anket_adi";
                this.ddlAnket.DataValueField = "anket_uid";
                this.ddlAnket.DataBind();
            }

            ShowReport();
        }

        public void SetSubDataSource(object sender, SubreportProcessingEventArgs e)
        {
            try
            {
                e.DataSources.Add(ReportViewer1.LocalReport.DataSources[1]);
            }
            catch(Exception exp)
            {
                string aa = exp.Message;
            }

        }
    }
}