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
    public partial class KullaniciGruplar : System.Web.UI.Page
    {
        private int grup_durumu_id
        {
            get { return (ViewState["grup_durumu_id"] != null ? Convert.ToInt32(ViewState["grup_durumu_id"]) : 0); }
            set { ViewState["grup_durumu_id"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (BaseDB.SessionContext.Current == null || BaseDB.SessionContext.Current.ActiveUser == null)
            {
                Response.Redirect("~/Login.aspx");
            }

            
            if (!IsPostBack)
            {
                if (Request.QueryString["grup_durumu_id"] != null && Request.QueryString["grup_durumu_id"].ToString() != "")
                {
                    grup_durumu_id = Convert.ToInt32(Request.QueryString["grup_durumu_id"].ToString());
                }

                BindGruplar(this.grup_durumu_id);
            }
            
        }

        protected new void Page_LoadComplete(object sender, EventArgs e)
        {
            BindGruplar(grup_durumu_id);
        }

        protected void BindGruplar(int grup_state_id)
        {
            GenelRepository ankDB = RepositoryManager.GetRepository<GenelRepository>();
            DataSet ds = ankDB.GetUserGroupDataSet(grup_state_id);

            this.rptGruplar.DataSource = ds;
            this.rptGruplar.DataBind();

            if(grup_state_id == 0)
                this.LinkButtonHeader.Text = "All";
            else if(grup_state_id == 1)
                this.LinkButtonHeader.Text = "Yönettiğim Ekip";
            else if (grup_state_id == 2)
                this.LinkButtonHeader.Text = "Üyesi Olduğum Ekipler";
        }

        protected void tumu_Click(object sender, EventArgs e)
        {
            this.LinkButtonHeader.Text = "Tümü";
            grup_durumu_id = 0;
            BindGruplar(0);
        }

        protected void yonettigim_Click(object sender, EventArgs e)
        {
            this.LinkButtonHeader.Text = "Yönettiğim Ekip";
            grup_durumu_id = 1;
            BindGruplar(1);
        }

        protected void uyeolunan_Click(object sender, EventArgs e)
        {
            this.LinkButtonHeader.Text = "Üyesi Olduğum Ekipler";
            grup_durumu_id = 2;
            BindGruplar(2);
        }

       
    }
}