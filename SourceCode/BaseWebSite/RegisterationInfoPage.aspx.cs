using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BaseWebSite
{
    public partial class RegisterationInfoPage : System.Web.UI.Page
    {
        private int tip
        {
            get { return (ViewState["tip"] != null ? Convert.ToInt32(ViewState["tip"]) : 0); }
            set { ViewState["tip"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            string islem_no = "", state="";
            if (!IsPostBack)
            {
                if (Request.QueryString["tip"] != null && Request.QueryString["tip"].ToString() != "")
                {
                    tip = Convert.ToInt32(Request.QueryString["tip"].ToString());
                }

                if (Request.QueryString["islem_no"] != null && Request.QueryString["islem_no"].ToString() != "")
                {
                    islem_no = Request.QueryString["islem_no"].ToString();
                }

                if (Request.QueryString["state"] != null && Request.QueryString["state"].ToString() != "")
                {
                    state = Request.QueryString["state"].ToString();
                }

            }

            if (tip == 1)
            {
                this.ErrorMessage.Text = BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "101") + "<br>"; 
            }
            else if (tip == 2)
            {
                this.ErrorMessage.Text = BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "12") + "<br>"; 
            }
            else if (tip == 3)
            {
                this.ErrorMessage.Text = BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "13") + "<br>"; 
            }
            else if (tip == 4)
            {
                this.ErrorMessage.Text = BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "102") + " Lütfen İşlem Numaranızı kaydediniz.İşlem No : " + islem_no + "<br>";
            }
            else if (tip == 5)
            {
                if (state != "")
                    state = " Durum Kodu : " + state;

                this.ErrorMessage.Text = BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "27") + state + "<br>";
            }
            else if (tip == 6)
            {
                this.ErrorMessage.Text = BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "104") + "<br>";
            }
        }
    }
}