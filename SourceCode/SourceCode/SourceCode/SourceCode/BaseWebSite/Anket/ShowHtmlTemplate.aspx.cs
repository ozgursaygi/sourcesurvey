using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BaseWebSite.Survey
{
    public partial class ShowHtmlTemplate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string tip = "";
            if (Request.QueryString["tip"] != null && Request.QueryString["tip"].ToString() != "")
                tip = Request.QueryString["tip"].ToString();
            else
                tip = "";

            ShowFile(tip);
        }

        protected void ShowFile(string tip)
        {
            if (tip == "1")
            {
                System.IO.StreamReader template = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"Anket\Templates\UyelikKosullariTemplate.html");
                string mailBody = template.ReadToEnd();
                template.Close();

                Response.Write(mailBody);
            }
            else if (tip == "2")
            {
                System.IO.StreamReader template = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"Anket\Templates\Anket123Sorumluluklari.html");
                string mailBody = template.ReadToEnd();
                template.Close();

                Response.Write(mailBody);
            }
        }
    }
}