using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaseWebSite.Models;
using System.Data;

namespace BaseWebSite.Admin
{
    public partial class Duyurular : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (BaseDB.SessionContext.Current == null || BaseDB.SessionContext.Current.ActiveUser == null)
            {
                Response.Redirect("~/Login.aspx");
            }
            
            GenelRepository ankDB = RepositoryManager.GetRepository<GenelRepository>();

            if (!IsPostBack)
            {
                BindNotification();
            }

            if (Request["__EVENTTARGET"] == "ArsivdenCikart")
            {
                string notification_uid = Request["__EVENTARGUMENT"].ToString();

                gnl_notification notifications = ankDB.NotificationGet(Guid.Parse(notification_uid));
                notifications.notification_statu = 1;
                ankDB.Kaydet();
            }

            if (Request["__EVENTTARGET"] == "ArsiveGonder")
            {
                string notification_uid = Request["__EVENTARGUMENT"].ToString();

                gnl_notification notifications = ankDB.NotificationGet(Guid.Parse(notification_uid));
                notifications.notification_statu = 2;
                ankDB.Kaydet();
            }

            if (Request["__EVENTTARGET"] == "Duzenle")
            {
                string notification_uid = Request["__EVENTARGUMENT"].ToString();
                NotificationPrepare(notification_uid);
            }

            if (Request["__EVENTTARGET"] == "DuyuruOlustur")
            {
                string degerler = Request["__EVENTARGUMENT"].ToString();

                string[] arrDegerler = degerler.Replace("^#^", "^").Split('^');

                string subject = arrDegerler[0];
                string notif = arrDegerler[1];

                gnl_notification notifications = new gnl_notification();
                ankDB.NotificationAdd(notifications);
                notifications.notification_subject = subject;
                notifications.notification = notif;
                notifications.notification_statu = 1;
                notifications.notification_date = DateTime.Now;
                notifications.notification_creation_date = DateTime.Now;
                notifications.notification_created_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                ankDB.Kaydet();
            }

            if (Request["__EVENTTARGET"] == "DuyuruUpdate")
            {
                string degerler = Request["__EVENTARGUMENT"].ToString();

                string[] arrDegerler = degerler.Replace("^#^", "^").Split('^');

                string subject = arrDegerler[0];
                string notif = arrDegerler[1];
                string uid = arrDegerler[2];

                gnl_notification notifications = ankDB.NotificationGet(Guid.Parse(uid));
                notifications.notification_subject = subject;
                notifications.notification = notif;
                ankDB.Kaydet();
            }
        }

        protected new void Page_LoadComplete(object sender, EventArgs e)
        {
            BindNotification();

            
        }

        protected void BindNotification()
        {
            GenelRepository ankDB = RepositoryManager.GetRepository<GenelRepository>();
         

            DataSet ds = ankDB.NotificationDataSet();

            this.rptDuyurularPaging.dataSource = ds;
            this.rptDuyurularPaging.SayfaBuyuklugu = 10;
            this.rptDuyurularPaging.Bind();
            this.rptDuyurular.DataSource = this.rptDuyurularPaging.Sayfa;
            this.rptDuyurular.DataBind();
        }

        protected void NotificationPrepare(string notification_uid)
        {
            GenelRepository ankDB = RepositoryManager.GetRepository<GenelRepository>();
            if (notification_uid == "") return;

            gnl_notification notification = ankDB.NotificationGet(Guid.Parse(notification_uid));

            

            if (notification.notification_subject != null) this.txtbaslik_duzenle.Text = notification.notification_subject;
            if (notification.notification!= null) this.txtDuyuru_duzenle.Text = notification.notification;

            ClientScript.RegisterStartupScript(this.GetType(), "Redirect1", "<script>DuyuruDuzenle('" + notification_uid + "')</script>");
        }

        public string DuyuruDurumu(object status_type_id)
        {
            string result = "";

            if (status_type_id != null && status_type_id.ToString() == "1")
            {
                result = "<span class=\"acikButton\">Open</span>";
            }
            else if (status_type_id != null && status_type_id.ToString() == "2")
            {
                result = "<span class=\"arsivButton\">Archived</span>";
            }
            
            return result;
        }

        public string ArsiveGonder(object notification_uid)
        {
            string result = "";

            GenelRepository ankDB = RepositoryManager.GetRepository<GenelRepository>();
            gnl_notification notification = ankDB.NotificationGet(Guid.Parse(notification_uid.ToString()));

            if (notification != null)
            {
                if (notification.notification_statu == 2)
                {

                    result = "<a href=\"#\" class=\"arsivdenCikart\" onclick=\"RemoveFromArchive('" + notification_uid + "');\">Remove From Archive</a>";
                }
                else
                {
                    result = "<a href=\"#\" class=\"arsiveGonder\" onclick=\"ArsiveGonder('" + notification_uid + "');\">Send To Archive</a>";
                }
            }

            return result;
        }
    }
}