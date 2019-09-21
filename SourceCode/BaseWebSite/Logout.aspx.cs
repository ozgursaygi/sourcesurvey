using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace BaseWebSite
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Page.Request.QueryString["Type"] == "1")
            {
                Session.Abandon();
                FormsAuthentication.SignOut();
                BaseClasses.SessionKeeper.AddLoggedInUserToDataBase("logout");
                Response.Redirect("Default.aspx");
            }
        }
    }
}