using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BaseWebSite
{
    public partial class PrePurchase : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void lnk_odeme_tipi_1_Click(object sender, EventArgs e)
        {
            if (BaseDB.SessionContext.Current == null)
            {
                this.ltl_uyari.Text = "<div style=\"padding:5px 5px 5px 5px\"><p class=\"warning\" ><span class=\"bold\"><b>Uyarı :</b></span>Satın Alma işlemini yapabilmeniz için Üye olmanız ve Kullanıcı Girişi yapmanız gerekmektedir.</p></div>";
            }
            else
            {
                Response.Redirect("Purchase.aspx?odeme_tipi_id=1");
            }
        }

        protected void lnk_odeme_tipi_2_Click(object sender, EventArgs e)
        {
            if (BaseDB.SessionContext.Current == null)
            {
                this.ltl_uyari.Text = "<div style=\"padding:5px 5px 5px 5px\"><p class=\"warning\" ><span class=\"bold\"><b>Uyarı :</b></span>Satın Alma işlemini yapabilmeniz için Üye olmanız ve Kullanıcı Girişi yapmanız gerekmektedir.</p></div>";
            }
            else
            {
                Response.Redirect("Purchase.aspx?odeme_tipi_id=2");
            }
        }

        protected void lnk_odeme_tipi_3_Click(object sender, EventArgs e)
        {
            if (BaseDB.SessionContext.Current == null)
            {
                this.ltl_uyari.Text = "<div style=\"padding:5px 5px 5px 5px\"><p class=\"warning\" ><span class=\"bold\"><b>Uyarı :</b></span>Satın Alma işlemini yapabilmeniz için Üye olmanız ve Kullanıcı Girişi yapmanız gerekmektedir.</p></div>";
            }
            else
            {
                Response.Redirect("Purchase.aspx?odeme_tipi_id=3");
            }
        }

        protected void lnk_odeme_tipi_4_Click(object sender, EventArgs e)
        {
            if (BaseDB.SessionContext.Current == null)
            {
                this.ltl_uyari.Text = "<div style=\"padding:5px 5px 5px 5px\"><p class=\"warning\" ><span class=\"bold\"><b>Uyarı :</b></span>Satın Alma işlemini yapabilmeniz için Üye olmanız ve Kullanıcı Girişi yapmanız gerekmektedir.</p></div>";
            }
            else
            {
                Response.Redirect("Purchase.aspx?odeme_tipi_id=4");
            }
        }

      
    }
}