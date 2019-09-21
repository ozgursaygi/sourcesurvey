using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BaseWebSite.Admin
{
    public partial class AdminMain : System.Web.UI.Page
    {
        private int MenuId
        {
            get { return (ViewState["MenuId"] != null ? Convert.ToInt32(ViewState["MenuId"]) : 0); }
            set { ViewState["MenuId"] = value; }
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

            if (BaseDB.SessionContext.Current == null || BaseDB.SessionContext.Current.ActiveUser == null)
            {
                Response.Redirect("~/Login.aspx?menu_id=" + MenuId);
            }

            if (!IsPostBack)
            {
                ComboDoldur();
                this.ddlListe.SelectedValue = "1";
                this.iframeMap.Attributes["src"] = "Raporlar/AnketListesiRaporu.aspx";
                LinkButtonHeader.Text = "Survey List";

                this.ddlanketler.DataSource = BaseDB.DBManager.AppConnection.GetDataSet("select olusturan_kullanici+' - '+anket_adi as adi,* from sbr_anket_v  order by olusturan_kullanici+' - '+anket_adi");
                this.ddlanketler.DataTextField = "adi";
                this.ddlanketler.DataValueField = "anket_uid";
                this.ddlanketler.DataBind();

                this.ddlanketler.Visible = false;
            }
        }

        
        protected void ComboDoldur()
        {
            ListItem item1 = new ListItem();
            item1.Value = "1";
            item1.Text = "Surveys List";
            this.ddlListe.Items.Add(item1);

            //ListItem item2 = new ListItem();
            //item2.Value = "2";
            //item2.Text = "Ödeme Aktivasyonu";
            //this.ddlListe.Items.Add(item2);

            ListItem item3 = new ListItem();
            item3.Value = "3";
            item3.Text = "Surveys";
            this.ddlListe.Items.Add(item3);

            //ListItem item4 = new ListItem();
            //item4.Value = "4";
            //item4.Text = "Üye Kullanıcılar (Satın Alma Yapanlar)";
            //this.ddlListe.Items.Add(item4);

            //ListItem item5 = new ListItem();
            //item5.Value = "5";
            //item5.Text = "Kullanıcılar";
            //this.ddlListe.Items.Add(item5);

            //ListItem item6 = new ListItem();
            //item6.Value = "6";
            //item6.Text = "Fatura Kesilecekler";
            //this.ddlListe.Items.Add(item6);

            //ListItem item7 = new ListItem();
            //item7.Value = "7";
            //item7.Text = "Fatura Kesilenler";
            //this.ddlListe.Items.Add(item7);
        }

        protected void ddlListe_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlListe.SelectedValue == "1")
            {
                this.iframeMap.Attributes["src"] = "Raporlar/AnketListesiRaporu.aspx";
                button_header.Visible = true;
                LinkButtonHeader.Text = "Survey List";
                this.ddlanketler.Visible = false;
            }
            else if (ddlListe.SelectedValue == "2")
            {
                this.iframeMap.Attributes["src"] = "OdemeAktiveEt.aspx";
                button_header.Visible = true;
                LinkButtonHeader.Text = "Ödeme Activatio";
                this.ddlanketler.Visible = false;
            }
            else if (ddlListe.SelectedValue == "3")
            {
                this.ddlanketler.Visible = true;
                if (ddlanketler.SelectedValue != null && ddlanketler.SelectedValue.ToString() != "")
                {
                    this.iframeMap.Attributes["src"] = "../Anket/AnketiGoster.aspx?anket_uid=" + ddlanketler.SelectedValue;
                    button_header.Visible = false;
                }
                
            }
            else if (ddlListe.SelectedValue == "4")
            {
                this.iframeMap.Attributes["src"] = "Raporlar/UyeKullanicilarRaporu.aspx";
                button_header.Visible = true;
                LinkButtonHeader.Text = "Üye Kullanıcılar (Satın Alma Yapanlar)";
                this.ddlanketler.Visible = false;
            }
            else if (ddlListe.SelectedValue == "5")
            {
                this.iframeMap.Attributes["src"] = "Raporlar/KullanicilarRaporu.aspx";
                button_header.Visible = true;
                LinkButtonHeader.Text = "Kullanıcılar";
                this.ddlanketler.Visible = false;
            }
            else if (ddlListe.SelectedValue == "6")
            {
                this.iframeMap.Attributes["src"] = "FaturaKesilecekler.aspx";
                button_header.Visible = true;
                LinkButtonHeader.Text = "Fatura Kesilecekler";
                this.ddlanketler.Visible = false;
            }
            else if (ddlListe.SelectedValue == "7")
            {
                this.iframeMap.Attributes["src"] = "FaturaKesilenler.aspx";
                button_header.Visible = true;
                LinkButtonHeader.Text = "Fatura Kesilenler";
                this.ddlanketler.Visible = false;
            }
        }

        protected void ddlanketler_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ddlanketler.Visible = true;
            if (ddlanketler.SelectedValue != null && ddlanketler.SelectedValue.ToString() != "")
            {
                button_header.Visible = false;
                this.iframeMap.Attributes["src"] = "../Anket/AnketiGoster.aspx?anket_uid=" + ddlanketler.SelectedValue;
            }
            
        }
    }
}