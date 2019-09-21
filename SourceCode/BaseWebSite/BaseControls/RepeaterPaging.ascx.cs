using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace BaseWebSite.BaseControls
{
    public partial class RepeaterPaging : System.Web.UI.UserControl
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

        protected void Page_Load(object sender, EventArgs e)
        {

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
            LblDurum.Text = "Sayfa " + sf.ToString() + " / " + Sayfa.PageCount.ToString() + " - Toplam Kayıt " + (Sayfa.DataSourceCount).ToString();

            lnknext.Enabled = !(Sayfa.IsLastPage);
            LnkPrev.Enabled = !(Sayfa.IsFirstPage);
            Lnkfirst.Enabled = !(Sayfa.IsFirstPage);
            LnkLast.Enabled = !(Sayfa.IsLastPage);

            ReadSayfa(Sayfa);

        }

        protected void ReadSayfa(PagedDataSource Sayfa)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("PageIndex");
            dt.Columns.Add("PageText");
            for (int i = 0; i <= Sayfa.PageCount - 1; i++)
            {
                DataRow dr = dt.NewRow();
                dr[0] = i;
                dr[1] = i + 1;
                dt.Rows.Add(dr);
            }

            rptPages.DataSource = dt;
            rptPages.DataBind();
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

        protected void Lnkfirst_Click(object sender, EventArgs e)
        {
            SayfaNo = 0;
            Bind();
        }

        protected void LnkLast_Click(object sender, EventArgs e)
        {
            SayfaNo = SayfaSayisi;
            Bind();
        }
    }
}