using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaseWebSite.Models;

namespace BaseWebSite
{
    public partial class Davet : System.Web.UI.Page
    {
        private string key
        {
            get { return (ViewState["key"] != null ? Convert.ToString(ViewState["key"]) : ""); }
            set { ViewState["key"] = value; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["key"] != null && Request.QueryString["key"].ToString() != "")
                {
                    key = Request.QueryString["key"].ToString();
                }

                GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();
                SurveyRepository anlDB = RepositoryManager.GetRepository<SurveyRepository>();

                sbr_anket_davet davet = anlDB.DavetGetir(key);

                if (davet.davet_kabul_edildi == null || davet.davet_kabul_edildi == false)
                {
                    gnl_users user = gnlDB.GetUsersByEmail(davet.davet_edilen_email);

                    if (user != null)
                    {
                        Response.Redirect("Login.aspx?DavetKey=" + davet.davet_key);
                    }
                    else
                    {
                        Response.Redirect("Register.aspx?DavetKey=" + davet.davet_key);
                    }
                }
                else
                {
                    Response.Redirect("RegisterationInfoPage.aspx?tip=3");
                }
            }
        }
    }
}