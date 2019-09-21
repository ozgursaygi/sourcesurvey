using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaseWebSite.Models;

namespace BaseWebSite.Anket.Raporlar
{
    public partial class ReportViewer : System.Web.UI.Page
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

            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();


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

                ComboDoldur();
                this.ddlrapor.SelectedValue = "7";
                //this.iframeMap.Attributes["src"] = "KullaniciBazliAnketRapor.aspx?anket_uid=" + anket_uid + "&grup_uid=" + grup_uid; ;
                this.iframeMap.Attributes["src"] = "AcikAnketSoruTipileriCevapRaporu.aspx?anket_uid=" + anket_uid + "&grup_uid=" + grup_uid;
            }
        }

        protected void ComboDoldur()
        {
        //    ListItem item1 = new ListItem();
        //    item1.Value = "1";
        //    item1.Text = "Katılımcı Bazlı Anket Raporu";
        //    this.ddlrapor.Items.Add(item1);

        //    ListItem item2 = new ListItem();
        //    item2.Value = "2";
        //    item2.Text = "Katılımcı Bazlı Anket Giriş Raporu";
        //    this.ddlrapor.Items.Add(item2);

        //    ListItem item3 = new ListItem();
        //    item3.Value = "3";
        //    item3.Text = "Katılımcı Bazlı Anket Cevaplama Raporu";
        //    this.ddlrapor.Items.Add(item3);

        //    ListItem item4 = new ListItem();
        //    item4.Value = "4";
        //    item4.Text = "Katılımcı Bazlı Anket Bitirme Raporu";
        //    this.ddlrapor.Items.Add(item4);

        //    ListItem item5 = new ListItem();
        //    item5.Value = "5";
        //    item5.Text = "Katılımcı Anket Cevap Raporu";
        //    this.ddlrapor.Items.Add(item5);

            
        //    ListItem item8 = new ListItem();
        //    item8.Value = "8";
        //    item8.Text = "Katılımcı Durum Raporu";
        //    this.ddlrapor.Items.Add(item8);

            

            ListItem item7 = new ListItem();
            item7.Value = "7";
            item7.Text = "Open Survey Answer Report";
            this.ddlrapor.Items.Add(item7);


            ListItem item6 = new ListItem();
            item6.Value = "6";
            item6.Text = "Survey Report";
            this.ddlrapor.Items.Add(item6);


        }

        protected void ddlrapor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlrapor.SelectedValue == "1")
                this.iframeMap.Attributes["src"] = "KullaniciBazliAnketRapor.aspx?anket_uid=" + anket_uid + "&grup_uid=" + grup_uid;
            else if (ddlrapor.SelectedValue == "2")
                this.iframeMap.Attributes["src"] = "KullaniciBazliAnketGirisRaporu.aspx?anket_uid=" + anket_uid + "&grup_uid=" + grup_uid;
            else if (ddlrapor.SelectedValue == "3")
                this.iframeMap.Attributes["src"] = "KullaniciBazliAnketCevaplanmaRaporu.aspx?anket_uid=" + anket_uid + "&grup_uid=" + grup_uid;
            else if (ddlrapor.SelectedValue == "4")
                this.iframeMap.Attributes["src"] = "KullaniciBazliAnketBitirmeRaporu.aspx?anket_uid=" + anket_uid + "&grup_uid=" + grup_uid;
            else if (ddlrapor.SelectedValue == "5")
                this.iframeMap.Attributes["src"] = "AnketSoruTipileriCevapRaporu.aspx?anket_uid=" + anket_uid + "&grup_uid=" + grup_uid;
            else if (ddlrapor.SelectedValue == "6")
                this.iframeMap.Attributes["src"] = "AnketSoruCevapRaporuTumu.aspx?anket_uid=" + anket_uid + "&grup_uid=" + grup_uid;
            else if (ddlrapor.SelectedValue == "7")
                this.iframeMap.Attributes["src"] = "AcikAnketSoruTipileriCevapRaporu.aspx?anket_uid=" + anket_uid + "&grup_uid=" + grup_uid;
            else if (ddlrapor.SelectedValue == "8")
                this.iframeMap.Attributes["src"] = "KullaniciBazliAnketDurumRaporu.aspx?anket_uid=" + anket_uid + "&grup_uid=" + grup_uid;



            
        }

        
    }
}