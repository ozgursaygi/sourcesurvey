 using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using BaseWebSite.Models;

namespace BaseWebSite
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        private int MenuId
        {
            get { return (ViewState["MenuId"] != null ? Convert.ToInt32(ViewState["MenuId"]) : 0); }
            set { ViewState["MenuId"] = value; }
        }

        public string ApplicationPath
        {
            get { if (System.Web.HttpContext.Current.Request.ApplicationPath == "/") return "https://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/"; else return "https://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/" + System.Web.HttpContext.Current.Request.ApplicationPath + "/"; }
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

            
            if (Request.Url.ToString().Contains("Default.aspx") || Request.Url.ToString().Contains("default.aspx") || Request.Url.ToString().Contains("DEFAULT.ASPX"))
            {
                this.div_default.Attributes.Add("class", "bgforSlider homePage");
            }


            if (BaseDB.SessionContext.Current != null)
            {
                this.div_login_display.Visible = false;
                this.div_user_display.Visible = true;
                this.UserNameSurname.Text = BaseDB.SessionContext.Current.ActiveUser.UserNameAndSurname.ToUpper();


                if (!BaseDB.SessionContext.Current.ActiveUser.IsSistemAdmin)
                {
                    ltlMenu.Text = CreateMenu(MenuId, 1, 0);
                }
                else
                {
                    ltlMenu.Text = CreateMenu(MenuId, 1, 1);
                }


            }
            else
            {
                this.div_login_display.Visible = true;
                this.div_user_display.Visible = false;
                this.UserNameSurname.Text = "";

                ltlMenu.Text = CreateMenu(MenuId, 2, 0);

            }

           
        }

        protected string CreateMenu(int menu_id,int tip,int sistem_admin)
        {
            StringBuilder menu_output = new StringBuilder();
            string menu_1_class="";
            string menu_2_class = "";
            string menu_3_class = "";
            string menu_4_class = "";
            string menu_5_class = "";
            string menu_6_class = "";
            string menu_7_class = "";
            string menu_8_class = "";
            string menu_9_class = "";
            string menu_10_class = "";

            switch (menu_id)
            {
               case 1: menu_1_class="class=\"first\"";
                  break;
               case 2: menu_2_class = "class=\"first\"";
                  break;
               case 3: menu_3_class = "class=\"first\"";
                  break;
               case 4: menu_4_class = "class=\"first\"";
                  break;
               case 5: menu_5_class = "class=\"first\"";
                  break;
               case 6: menu_6_class = "class=\"first\"";
                  break;
               case 7: menu_7_class = "class=\"first\"";
                  break;
               case 8: menu_8_class = "class=\"first\"";
                  break;
               case 9: menu_9_class = "class=\"first\"";
                  break;
               case 10: menu_10_class = "class=\"first\"";
                  break;
                
            }

            menu_output.Append("<ul class=\"headerMenu\" >");
            menu_output.Append("<li " + menu_1_class + "  ><a href=\"" + ResolveClientUrl("Default.aspx") + "\">Main Page</a></li>");
            //menu_output.Append("<li " + menu_2_class + "><a href=\"" + ResolveClientUrl("About.aspx?menu_id=2") + "\">Hakkında</a></li>");
            menu_output.Append("<li " + menu_3_class + "><a href=\"" + ResolveClientUrl("Anket/AnketDashboard.aspx?menu_id=3") + "\">My Surveys</a></li>");
            //if (tip != 1) menu_output.Append("<li " + menu_4_class + "><a href=\"" + ResolveClientUrl("Register.aspx?menu_id=4") + "\">Sign Up</a></li>");
            //menu_output.Append("<li " + menu_5_class + "><a href=\"" + ResolveClientUrl("PrePurchase.aspx?menu_id=5") + "\">Purchase</a></li>");
            //if (tip != 2) menu_output.Append("<li " + menu_6_class + "><a href=\"" + ResolveClientUrl("Anket/Messages.aspx?menu_id=6") + "\">Messages</a></li>");
            //if (tip != 2) menu_output.Append("<li " + menu_10_class + "><a href=\"" + ResolveClientUrl("ArkadasimaOner.aspx?menu_id=10") + "\">Arkadaşıma Öner</a></li>");
            //menu_output.Append("<li " + menu_7_class + "><a href=\"" + ResolveClientUrl("Help.aspx?menu_id=7") + "\">Help</a></li>");
            menu_output.Append("<li " + menu_9_class + "><a href=\"" + ResolveClientUrl("Iletisim.aspx?menu_id=9") + "\">Contact</a></li>");
            if (tip != 2 && sistem_admin == 1) menu_output.Append("<li " + menu_8_class + "><a href=\"" + ResolveClientUrl("Admin/AdminMain.aspx?menu_id=8") + "\">Admin Panel</a></li>");
            menu_output.Append("</ul>");
            return menu_output.ToString();
        }
    }
}
