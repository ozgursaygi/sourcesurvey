using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaseWebSite.Models;

namespace BaseWebSite.Survey
{
    public partial class AnketDashboard : System.Web.UI.Page
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

            if (Request["__EVENTTARGET"] == "DuyuruGoster")
            {
                string notification_uid = Request["__EVENTARGUMENT"].ToString();
                PrepareNotificationInfo(notification_uid);
            }

            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            this.ltlSurveyler.Text = ankDB.anket_dashbord_anket_durumu(BaseDB.SessionContext.Current.ActiveUser.GrupUid);
            this.ltlYayindakiler.Text = ankDB.anket_dashbord_yayindakiler(BaseDB.SessionContext.Current.ActiveUser.GrupUid);
            //this.ltlMailGruplari.Text = ankDB.anket_dashbord_mail_gruplari(BaseDB.SessionContext.Current.ActiveUser.GrupUid);
            //this.ltlMesajlar.Text = ankDB.anket_dashbord_mesajlar(BaseDB.SessionContext.Current.ActiveUser.UserUid);
            //this.ltlDuyurular.Text = ankDB.anket_dashbord_duyurular();
        }

        protected void PrepareNotificationInfo(string notification_uid)
        {
            GenelRepository ankDB = RepositoryManager.GetRepository<GenelRepository>();
            if (notification_uid == "") return;

            gnl_notification notification = ankDB.NotificationGet(Guid.Parse(notification_uid));
            
            if (notification.notification_subject != null) this.txtbaslik.Text = notification.notification_subject;
            if (notification.notification != null) this.txtDuyuru.Text = notification.notification;

            ClientScript.RegisterStartupScript(this.GetType(), "Redirect1", "<script>DuyuruGoster1('" + notification_uid + "')</script>");
        }
    }
}