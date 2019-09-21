using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Microsoft.Reporting.WebForms;

namespace BaseWebSite.Anket.Raporlar
{
    public partial class AnketSoruTipi1CevapRaporu1 : System.Web.UI.Page
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

                if (ddlAnket.SelectedValue != null && ddlAnket.SelectedValue.ToString() != "")
                {
                    this.ddlSoru.DataSource = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_anket_soru_v where anket_uid in ('" + anket_uid + "') order by soru_sira_no");
                    this.ddlSoru.DataTextField = "soru_kisa";
                    this.ddlSoru.DataValueField = "soru_uid";
                    this.ddlSoru.DataBind();
                }

               ShowReport();
            }
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

            if (ddlAnket.SelectedValue != null && ddlAnket.SelectedValue.ToString() != "")
            {
                this.ddlSoru.DataSource = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_anket_soru_v where anket_uid in ('" + anket_uid + "') order by soru_sira_no");
                this.ddlSoru.DataTextField = "soru_kisa";
                this.ddlSoru.DataValueField = "soru_uid";
                this.ddlSoru.DataBind();
            }
        }

        private void ShowReport()
        {
            
                Parameter param_soru_uid = new Parameter("soru_uid", DbType.Guid, this.ddlSoru.SelectedValue.ToString());

                this.ObjectDataSource1.SelectParameters["soru_uid"] = param_soru_uid;
                this.ObjectDataSource1.DataBind();

                this.ObjectDataSource2.SelectParameters["soru_uid"] = param_soru_uid;
                this.ObjectDataSource2.DataBind();

                this.ObjectDataSource3.SelectParameters["soru_uid"] = param_soru_uid;
                this.ObjectDataSource3.DataBind();

                this.ObjectDataSource4.SelectParameters["soru_uid"] = param_soru_uid;
                this.ObjectDataSource4.DataBind();

                this.ObjectDataSource5.SelectParameters["soru_uid"] = param_soru_uid;
                this.ObjectDataSource5.DataBind();
            
                this.ObjectDataSource6.SelectParameters["soru_uid"] = param_soru_uid;
                this.ObjectDataSource6.DataBind();


                this.ObjectDataSource7.SelectParameters["soru_uid"] = param_soru_uid;
                this.ObjectDataSource7.DataBind();

                this.ObjectDataSource8.SelectParameters["soru_uid"] = param_soru_uid;
                this.ObjectDataSource8.DataBind();
            
                this.ObjectDataSource9.SelectParameters["soru_uid"] = param_soru_uid;
                this.ObjectDataSource9.DataBind();
            
                this.ObjectDataSource10.SelectParameters["soru_uid"] = param_soru_uid;
                this.ObjectDataSource10.DataBind();

                this.ObjectDataSource11.SelectParameters["soru_uid"] = param_soru_uid;
                this.ObjectDataSource11.DataBind();

                ReportViewer1.Reset();


                if (this.ddlSoru.SelectedValue != null && this.ddlSoru.SelectedValue.ToString() != "")
                {
                    string soru_tipi_id = BaseDB.DBManager.AppConnection.ExecuteSql("select soru_tipi_id from sbr_anket_soru_v where soru_uid in('" + this.ddlSoru.SelectedValue.ToString() + "')");

                    if (soru_tipi_id == "1")
                    {
                        ReportDataSource dt_source = new ReportDataSource("DataSet1", this.ObjectDataSource1);
                        ReportViewer1.LocalReport.DataSources.Add(dt_source);
                        this.ReportViewer1.LocalReport.ReportPath = "Anket\\Raporlar\\AnketSoruTipi1CevapRaporu.rdlc";

                    }
                    else if (soru_tipi_id == "2")
                    {
                        ReportDataSource dt_source = new ReportDataSource("DataSet1", this.ObjectDataSource2);
                        ReportViewer1.LocalReport.DataSources.Add(dt_source);
                        this.ReportViewer1.LocalReport.ReportPath = "Anket\\Raporlar\\AnketSoruTipi2CevapRaporu.rdlc";
                    }
                    else if (soru_tipi_id == "3")
                    {
                        ReportDataSource dt_source = new ReportDataSource("DataSet1", this.ObjectDataSource3);
                        ReportViewer1.LocalReport.DataSources.Add(dt_source);
                        this.ReportViewer1.LocalReport.ReportPath = "Anket\\Raporlar\\AnketSoruTipi3CevapRaporu.rdlc";
                    }
                    else if (soru_tipi_id == "4")
                    {
                        ReportDataSource dt_source = new ReportDataSource("DataSet1", this.ObjectDataSource4);
                        ReportViewer1.LocalReport.DataSources.Add(dt_source);
                        this.ReportViewer1.LocalReport.ReportPath = "Anket\\Raporlar\\AnketSoruTipi4CevapRaporu.rdlc";
                    }
                    else if (soru_tipi_id == "5")
                    {
                        ReportDataSource dt_source = new ReportDataSource("DataSet1", this.ObjectDataSource5);
                        ReportViewer1.LocalReport.DataSources.Add(dt_source);
                        this.ReportViewer1.LocalReport.ReportPath = "Anket\\Raporlar\\AnketSoruTipi5CevapRaporu.rdlc";
                    }
                    else if (soru_tipi_id == "6")
                    {
                        ReportDataSource dt_source = new ReportDataSource("DataSet1", this.ObjectDataSource6);
                        ReportViewer1.LocalReport.DataSources.Add(dt_source);
                        this.ReportViewer1.LocalReport.ReportPath = "Anket\\Raporlar\\AnketSoruTipi6CevapRaporu.rdlc";
                    }
                    else if (soru_tipi_id == "7")
                    {
                        ReportDataSource dt_source = new ReportDataSource("DataSet1", this.ObjectDataSource7);
                        ReportViewer1.LocalReport.DataSources.Add(dt_source);
                        this.ReportViewer1.LocalReport.ReportPath = "Anket\\Raporlar\\AnketSoruTipi7CevapRaporu.rdlc";
                    }
                    else if (soru_tipi_id == "8")
                    {
                        ReportDataSource dt_source = new ReportDataSource("DataSet1", this.ObjectDataSource8);
                        ReportViewer1.LocalReport.DataSources.Add(dt_source);
                        this.ReportViewer1.LocalReport.ReportPath = "Anket\\Raporlar\\AnketSoruTipi8CevapRaporu.rdlc";
                    }
                    else if (soru_tipi_id == "9")
                    {
                        ReportDataSource dt_source = new ReportDataSource("DataSet1", this.ObjectDataSource9);
                        ReportViewer1.LocalReport.DataSources.Add(dt_source);
                        this.ReportViewer1.LocalReport.ReportPath = "Anket\\Raporlar\\AnketSoruTipi9CevapRaporu.rdlc";
                    }
                    else if (soru_tipi_id == "10")
                    {
                        ReportDataSource dt_source = new ReportDataSource("DataSet1", this.ObjectDataSource10);
                        ReportViewer1.LocalReport.DataSources.Add(dt_source);
                        this.ReportViewer1.LocalReport.ReportPath = "Anket\\Raporlar\\AnketSoruTipi10CevapRaporu.rdlc";
                    }
                    else if (soru_tipi_id == "11")
                    {
                        ReportDataSource dt_source = new ReportDataSource("DataSet1", this.ObjectDataSource11);
                        ReportViewer1.LocalReport.DataSources.Add(dt_source);
                        this.ReportViewer1.LocalReport.ReportPath = "Anket\\Raporlar\\AnketSoruTipi11CevapRaporu.rdlc";
                    }
                }
                
                ReportViewer1.LocalReport.Refresh();
            
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

            if (ddlAnket.SelectedValue != null && ddlAnket.SelectedValue.ToString() != "")
            {
                this.ddlSoru.DataSource = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_anket_soru_v where anket_uid in ('" + ddlAnket.SelectedValue.ToString() + "') order by soru_sira_no");
                this.ddlSoru.DataTextField = "soru_kisa";
                this.ddlSoru.DataValueField = "soru_uid";
                this.ddlSoru.DataBind();
            }
            else
            {
                this.ddlSoru.DataSource = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_anket_soru_v where anket_uid in ('" + Guid.NewGuid() + "') order by soru_sira_no");
                this.ddlSoru.DataTextField = "soru_kisa";
                this.ddlSoru.DataValueField = "soru_uid";
                this.ddlSoru.DataBind();
            }

            ShowReport();
        }

        protected void ddlAnket_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlAnket.SelectedValue != null && ddlAnket.SelectedValue.ToString() != "")
            {
                this.ddlSoru.DataSource = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_anket_soru_v where anket_uid in ('" + ddlAnket.SelectedValue.ToString() + "') order by soru_sira_no");
                this.ddlSoru.DataTextField = "soru_kisa";
                this.ddlSoru.DataValueField = "soru_uid";
                this.ddlSoru.DataBind();
            }

            ShowReport();
        }

        protected void ddlSoru_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowReport();
        }
    }
}