using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaseWebSite.Models;

namespace BaseWebSite
{
    public partial class PurchaseControl : System.Web.UI.Page
    {
        public int odeme_tipi_id
        {
            get { return (ViewState["odeme_tipi_id"] != null ? Convert.ToInt32(ViewState["odeme_tipi_id"].ToString()) : 0); }
            set { ViewState["odeme_tipi_id"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["odeme_tipi_id"] != null && Request.QueryString["odeme_tipi_id"].ToString() != "")
            {
                odeme_tipi_id = Convert.ToInt32(Request.QueryString["odeme_tipi_id"].ToString());
            }

            if (BaseDB.SessionContext.Current != null)
            {
                Response.Redirect("Purchase.aspx?odeme_tipi_id=" + odeme_tipi_id);
            }
        }

        protected void OnaylaButton_Click(object sender, EventArgs e)
        {
            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();
            
            if(this.Email.Text!="")
            {
                gnl_users user = gnlDB.GetUsersByEmail(this.Email.Text);

                if (user != null)
                {
                    if (BaseDB.SessionContext.Current == null)
                    {
                        Response.Redirect("Login.aspx?From=PurchaseControl&odeme_tipi_id=" + odeme_tipi_id);
                    }
                    else
                    {
                        if (BaseDB.SessionContext.Current.ActiveUser.UserUid == user.user_uid)
                        {
                            Response.Redirect("Purchase.aspx?odeme_tipi_id=" + odeme_tipi_id);
                        }
                        else
                        {
                            Response.Redirect("Login.aspx?From=PurchaseControl&odeme_tipi_id=" + odeme_tipi_id);
                        }
                    }
                }
                else
                {
                    Response.Redirect("Register.aspx?From=PurchaseControl&odeme_tipi_id=" + odeme_tipi_id);
                }
            }
        }
    }
}