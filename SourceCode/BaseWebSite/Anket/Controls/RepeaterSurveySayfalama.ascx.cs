using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace BaseWebSite.Survey.Controls
{
    public partial class RepeaterSurveySayfalama : System.Web.UI.UserControl
    {
        public int SayfaNo
        {
            get { return (ViewState["SayfaNo"] != null ? Convert.ToInt32(ViewState["SayfaNo"]) : 0); }
            set { ViewState["SayfaNo"] = value; }
        }

        public int SayfaSayisi
        {
            get { return (ViewState["SayfaSayisi"] != null ? Convert.ToInt32(ViewState["SayfaSayisi"]) : 0); }
            set { ViewState["SayfaSayisi"] = value; }
        }

        public int SayfaBuyuklugu
        {
            get { return (ViewState["SayfaBuyuklugu"] != null ? Convert.ToInt32(ViewState["SayfaBuyuklugu"]) : 0); }
            set { ViewState["SayfaBuyuklugu"] = value; }
        }

        public DataSet dataSource
        {
            get { return (ViewState["dataSource"] != null ? (DataSet)(ViewState["dataSource"]) : null); }
            set { ViewState["dataSource"] = value; }
        }

     

        public PagedDataSource Sayfa;

        public bool NextVisible
        {
            get { return (ViewState["NextVisible"] != null ? Convert.ToBoolean(ViewState["NextVisible"].ToString()) : false); }
            set { ViewState["NextVisible"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (NextVisible)
                this.lnknext.Visible = true;
            else
                this.lnknext.Visible = false;
        }

        public void Bind()
        {
            if (dataSource == null || dataSource.Tables.Count == 0 || dataSource.Tables[0].Rows.Count == 0)
            {
                this.paging.Visible = false;
                return;
            }
            else
            {
                this.paging.Visible = true;
            }

            Sayfa = new PagedDataSource();
            DataTable dataTable = new DataTable();
            Sayfa.DataSource = dataSource.Tables[0].DefaultView;
            Sayfa.AllowPaging = true;
            Sayfa.PageSize = this.SayfaBuyuklugu;
            SayfaSayisi = Sayfa.PageCount - 1;
            Sayfa.CurrentPageIndex = SayfaNo;
            int sf = (Sayfa.CurrentPageIndex == 0) ? 1 : (Sayfa.CurrentPageIndex + 1);
            LblDurum.Text = "Page " + sf.ToString() + " / " + Sayfa.PageCount.ToString();
            lblSoruSayisi.Text="Question Count " + (Sayfa.DataSourceCount).ToString();

            lnknext.Visible = !(Sayfa.IsLastPage);
            LnkPrev.Visible = !(Sayfa.IsFirstPage);
          

        }


        protected void rptPages_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            SayfaNo = Convert.ToInt32(e.CommandArgument);
            Bind();
        }

        protected void rptPages_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            LinkButton lnkbtnPage = (LinkButton)(e.Item.FindControl("lnkbtnPaging"));
            if (lnkbtnPage.CommandArgument.ToString() == SayfaNo.ToString())
            {
                lnkbtnPage.Enabled = false;
                lnkbtnPage.Font.Bold = true;
                lnkbtnPage.ForeColor = System.Drawing.Color.Red;

            }
        }

        protected void lnknext_Click(object sender, EventArgs e)
        {
            SayfaNo += 1;
            Bind();
        }

        protected void LnkPrev_Click(object sender, EventArgs e)
        {
            SayfaNo -= 1;
            Bind();
        }

    }
}