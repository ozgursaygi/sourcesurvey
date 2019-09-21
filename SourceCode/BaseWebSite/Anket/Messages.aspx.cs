using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaseWebSite.Models;
using System.Data;
using System.Collections;
using System.Text;

namespace BaseWebSite
{
    public partial class Messages : System.Web.UI.Page
    {
        public int mesaj_durumu_id
        {
            get { return (ViewState["mesaj_durumu_id"] != null ? Convert.ToInt32(ViewState["mesaj_durumu_id"]) : 0); }
            set { ViewState["mesaj_durumu_id"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (BaseDB.SessionContext.Current == null || BaseDB.SessionContext.Current.ActiveUser == null)
            {
                Response.Redirect("~/Login.aspx");
            }

            GenelRepository db = RepositoryManager.GetRepository<GenelRepository>();

            if (!IsPostBack)
            {
                this.mesaj_durumu_id = 1;
                if (Request.QueryString["mesaj_durumu_id"] != null && Request.QueryString["mesaj_durumu_id"].ToString() != "")
                {
                    mesaj_durumu_id = Convert.ToInt32(Request.QueryString["mesaj_durumu_id"]);
                }

                switch (mesaj_durumu_id)
                {
                    case 1:
                        this.LinkButtonHeader.Text = "Inbox";
                        break;
                }            

                BindMesajlar(mesaj_durumu_id);
            }

            if (Request["__EVENTTARGET"] == "MesajGonder")
            {
                string degerler = Request["__EVENTARGUMENT"].ToString();

                string[] arrDegerler = degerler.Replace("^#^", "^").Split('^');

                string subject = arrDegerler[0];
                string message_gnl = arrDegerler[1];

                try
                {
                    gnl_message ileti2 = new gnl_message();
                    this.UpdateModelToSystemAdmin(ileti2, subject, message_gnl);
                    if (ileti2.IsValid)
                    {
                        ileti2.Send();
                        db.AddMessage(ileti2);
                        db.Kaydet();


                        DataSet ds_sistem_kullanicilari = db.SistemAdminKullanicilar();
                        
                        ArrayList mail_arr = new ArrayList();
                        ArrayList mail_ad_arr = new ArrayList();
                        ArrayList mail_soyad_arr = new ArrayList();

                        foreach (DataRow dr in ds_sistem_kullanicilari.Tables[0].Rows)
                        {
                            if (dr["email"] != System.DBNull.Value && BaseClasses.BaseFunctions.getInstance().IsEmailValid(dr["email"].ToString()))
                            {
                                mail_arr.Add(dr["email"].ToString());
                                mail_ad_arr.Add(dr["ad"].ToString());
                                mail_soyad_arr.Add(dr["soyad"].ToString());
                            }
                        }

                        string applicationPath = "";
                        if (System.Web.HttpContext.Current.Request.ApplicationPath == "/")
                            applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/";
                        else
                            applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/" + System.Web.HttpContext.Current.Request.ApplicationPath + "/";

                        db.send_mail(mail_arr, mail_ad_arr, mail_soyad_arr, applicationPath, DateTime.Now, subject, message_gnl, BaseDB.SessionContext.Current.ActiveUser.Name, BaseDB.SessionContext.Current.ActiveUser.Surname);
                        ClientScript.RegisterClientScriptBlock(this.GetType(), "Redirect2", "alert('" + BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "203") + "')", true);
                        
                        BindMesajlar(mesaj_durumu_id);
                    }
                }
                catch (Exception exp)
                {
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "Redirect2", "alert('" + BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "204") + "')", true);
                }
            }

            if (Request["__EVENTTARGET"] == "UyelereMesajGonder")
            {
                string degerler = Request["__EVENTARGUMENT"].ToString();

                string[] arrDegerler = degerler.Replace("^#^", "^").Split('^');

                string subject = arrDegerler[0];
                string message_gnl = arrDegerler[1];
                string tum_uyelere_gonder = arrDegerler[2];
                string gonderilecek_uyeler = arrDegerler[3];

                try
                {
                    this.UpdateModelUyelereMesajGonder(subject, message_gnl, tum_uyelere_gonder, gonderilecek_uyeler);
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "Redirect2", "alert('" + BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "203") + "')", true);
                    BindMesajlar(mesaj_durumu_id);

                }
                catch (Exception exp)
                {
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "Redirect2", "alert('" + BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "204") + "')", true);
                }
            }


            if (Request["__EVENTTARGET"] == "Cevapla")
            {

                string degerler = Request["__EVENTARGUMENT"].ToString();

                string[] arrDegerler = degerler.Replace("^#^", "^").Split('^');

                string subject = arrDegerler[0];
                string message_gnl = arrDegerler[1];
                string message_uid = arrDegerler[2];

                try
                {
                    if (message_uid != "")
                    {
                        
                        gnl_message ileti3 = db.GetMessage(Guid.Parse(message_uid));

                        gnl_message ileti2 = new gnl_message();
                        this.UpdateModelToUser(ileti2, subject, message_gnl,ileti3.send_user_uid);
                        if (ileti2.IsValid)
                        {
                            ileti2.Send();
                            db.AddMessage(ileti2);
                            db.Kaydet();


                            gnl_uye_kullanicilar uyeler = db.GetMemberUsers(ileti3.send_user_uid);


                            ArrayList mail_arr = new ArrayList();
                            ArrayList mail_ad_arr = new ArrayList();
                            ArrayList mail_soyad_arr = new ArrayList();

                            mail_arr.Add(uyeler.email);
                            mail_ad_arr.Add(uyeler.ad);
                            mail_soyad_arr.Add(uyeler.soyad);

                            string applicationPath = "";
                            if (System.Web.HttpContext.Current.Request.ApplicationPath == "/")
                                applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/";
                            else
                                applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/" + System.Web.HttpContext.Current.Request.ApplicationPath + "/";

                            db.send_mail(mail_arr, mail_ad_arr, mail_soyad_arr, applicationPath, DateTime.Now, subject, message_gnl, BaseDB.SessionContext.Current.ActiveUser.Name, BaseDB.SessionContext.Current.ActiveUser.Surname);
                            ClientScript.RegisterClientScriptBlock(this.GetType(), "Redirect2", "alert('" + BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "203") + "')", true);
                            BindMesajlar(mesaj_durumu_id);
                        }
                    }
                }
                catch (Exception exp)
                {
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "Redirect2", "alert('" + BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "204") + "')", true);
                }
            }


            if (Request["__EVENTTARGET"] == "Sil")
            {

                string degerler = Request["__EVENTARGUMENT"].ToString();

                string[] arrDegerler = degerler.Replace("^#^", "^").Split('^');

                string message_uid = arrDegerler[0];
                string tip = arrDegerler[1];

                try
                {
                    if (message_uid != "")
                    {
                        gnl_message ileti3 = db.GetMessage(Guid.Parse(message_uid));

                        if (ileti3 != null)
                        {
                            if (tip == "2")
                            {
                                ileti3.is_deleted = true;
                                ileti3.deleted_at = DateTime.Now;
                                ileti3.deleted_by = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                                db.Kaydet();
                            }

                            gnl_message_inbox ileti4 = db.GetMessageFromInbox(Guid.Parse(message_uid), BaseDB.SessionContext.Current.ActiveUser.UserUid);

                            if (ileti4 != null)
                            {
                                ileti4.is_deleted = true;
                                ileti4.deleted_at = DateTime.Now;
                                ileti4.deleted_by = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                                db.Kaydet();

                            }
                        }
                        
                        BindMesajlar(mesaj_durumu_id);

                    }
                }
                catch (Exception exp)
                {
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "Redirect2", "alert('" + BaseClasses.BaseFunctions.getInstance().GetAlertResource("tr-TR", "204") + "')", true);
                }
            }


            ltlMenu.Text = CreateMenu();
        }
        protected new void Page_LoadComplete(object sender, EventArgs e)
        {
            BindMesajlar(mesaj_durumu_id);

        }
        protected string CreateMenu()
        {
            
            System.Text.StringBuilder menu_output = new StringBuilder();

            menu_output.Append("<ul class=\"mainMenu\" >");
            menu_output.Append("<li ><a href=\"AnketDashboard.aspx\" >BACK</a></li>");
            menu_output.Append("<li ><a href=\"#\" onclick=\"MesajGonder();return false;\">SENT MESSAGES TO SURVEY ADMIN</a></li>");
            if (BaseDB.SessionContext.Current.ActiveUser.IsSistemAdmin == true)
            {
                menu_output.Append("<li ><a href=\"#\" onclick=\"UyelereMesajGonder();return false;\">SENT MESSAGES TO USER</a></li>");
            }
            
            menu_output.Append("</ul>");
            return menu_output.ToString();
        }

        private void UpdateModelToSystemAdmin(gnl_message message, string subject, string message_gnl)
        {
            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();

            message.message_subject = subject;
            message.message = message_gnl;
            message.send_user_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;


            DataSet ds_sistem_kullanicilari = gnlDB.SistemAdminKullanicilar();
            int i = 0;
            foreach (DataRow dr in ds_sistem_kullanicilari.Tables[0].Rows)
            {
                if (dr["email"] != System.DBNull.Value && BaseClasses.BaseFunctions.getInstance().IsEmailValid(dr["email"].ToString()))
                {
                    i++;
                }
            }

            Guid[] arr_guid = new Guid[i];

            i = 0;
            foreach (DataRow dr in ds_sistem_kullanicilari.Tables[0].Rows)
            {
                if (dr["email"] != System.DBNull.Value && BaseClasses.BaseFunctions.getInstance().IsEmailValid(dr["email"].ToString()))
                {
                    arr_guid[i] = Guid.Parse(dr["user_uid"].ToString());
                    i++;
                }
            }

            message.AddReceiver(arr_guid, gnl_message.ReceiverTypeTo);


        }

        private void UpdateModelUyelereMesajGonder(string subject,string message_gnl,string tum_uyelere_gonder,string uye_listesi)
        {
             GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();

            
            ArrayList mail_arr = new ArrayList();
            ArrayList mail_ad_arr = new ArrayList();
            ArrayList mail_soyad_arr = new ArrayList();
            ArrayList mail_user_uid_arr = new ArrayList();


            if (tum_uyelere_gonder == "true" || tum_uyelere_gonder == "True" || tum_uyelere_gonder == "TRUE")
            {
                DataSet ds_member_users = gnlDB.TumUyeKullanicilar();
                int i = 0;
                foreach (DataRow dr in ds_member_users.Tables[0].Rows)
                {
                    if (dr["email"] != System.DBNull.Value && BaseClasses.BaseFunctions.getInstance().IsEmailValid(dr["email"].ToString()) && dr["ad"] != System.DBNull.Value && dr["soyad"] != System.DBNull.Value)
                    {
                        i++;
                    }
                }

                Guid[] arr_guid = new Guid[i];

                foreach (DataRow dr in ds_member_users.Tables[0].Rows)
                {
                    if (dr["email"] != System.DBNull.Value && BaseClasses.BaseFunctions.getInstance().IsEmailValid(dr["email"].ToString()) && dr["ad"] != System.DBNull.Value && dr["soyad"] != System.DBNull.Value)
                    {
                        mail_arr.Add(dr["email"].ToString());
                        mail_ad_arr.Add(dr["ad"].ToString());
                        mail_soyad_arr.Add(dr["soyad"].ToString());
                        mail_user_uid_arr.Add(dr["user_uid"].ToString());
                    }
                }
            }
            else
            {
                
                if (uye_listesi != "")
                {
                    string[] arr_uye_listesi = uye_listesi.Split(',');

                    foreach (string user_uid in arr_uye_listesi)
                    {
                        if (user_uid != "")
                        {
                            gnl_users user = gnlDB.GetUsers(Guid.Parse(user_uid));

                            if (user != null)
                            {
                                if (user.email != null && BaseClasses.BaseFunctions.getInstance().IsEmailValid(user.email) && user.name!= null && user.surname != null)
                                {
                                    mail_arr.Add(user.email);
                                    mail_ad_arr.Add(user.name);
                                    mail_soyad_arr.Add(user.surname);
                                    mail_user_uid_arr.Add(user.user_uid);
                                }
                            }
                        }
                    }
                }
            }

            string applicationPath = "";
            if (System.Web.HttpContext.Current.Request.ApplicationPath == "/")
                applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/";
            else
                applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/" + System.Web.HttpContext.Current.Request.ApplicationPath + "/";

            gnlDB.send_message_to_members(mail_arr, mail_ad_arr, mail_soyad_arr,mail_user_uid_arr, applicationPath, DateTime.Now, subject, message_gnl, "Survey", "Sistem Yöneticisi");


        }

        private void UpdateModelToUser(gnl_message message, string subject, string message_gnl, Guid user_uid)
        {
            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();

            message.message_subject = subject;
            message.message = message_gnl;
            message.send_user_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;


            
            Guid[] arr_guid = new Guid[1];
            arr_guid[0] = user_uid;
            
            message.AddReceiver(arr_guid, gnl_message.ReceiverTypeTo);


        }

        protected void BindMesajlar(int mesaj_durumu_id)
        {
            GenelRepository ankDB = RepositoryManager.GetRepository<GenelRepository>();

            string filter = "";
            DataSet ds = new DataSet();
            if (mesaj_durumu_id == 1)
                ds = GenelRepository.InboxMessages(BaseDB.SessionContext.Current.ActiveUser.UserUid, filter);
            else if (mesaj_durumu_id == 2)
                ds = GenelRepository.SentMessages(BaseDB.SessionContext.Current.ActiveUser.UserUid);
            else if (mesaj_durumu_id == 3)
                ds = GenelRepository.InboxMessages(BaseDB.SessionContext.Current.ActiveUser.UserUid, filter,true);
            else if (mesaj_durumu_id == 4)
                ds = GenelRepository.SentMessages(BaseDB.SessionContext.Current.ActiveUser.UserUid,  true);

            this.rptMesajlarPaging.dataSource = ds;
            this.rptMesajlarPaging.SayfaBuyuklugu = 10;
            this.rptMesajlarPaging.Bind();
            this.rptMesajlar.DataSource = this.rptMesajlarPaging.Sayfa;
            this.rptMesajlar.DataBind();
        }

        protected void gelen_kutusu_Click(object sender, EventArgs e)
        {
            this.LinkButtonHeader.Text = "Inbox";
            this.rptMesajlarPaging.SayfaNo = 0;
            mesaj_durumu_id = 1;
            BindMesajlar(1);
            
        }

        protected void giden_kutusu_Click(object sender, EventArgs e)
        {
            this.LinkButtonHeader.Text = "Outbox";
            this.rptMesajlarPaging.SayfaNo = 0;
            mesaj_durumu_id = 2;
            BindMesajlar(2);
            
        }

        protected void silinenler_gelen_Click(object sender, EventArgs e)
        {
            this.LinkButtonHeader.Text = "Deleted Messages(Inbox)";
            this.rptMesajlarPaging.SayfaNo = 0;
            mesaj_durumu_id = 3;
            BindMesajlar(3);
            
        }

        protected void silinenler_gidenler_Click(object sender, EventArgs e)
        {
            this.LinkButtonHeader.Text = "Deleted Messages(Outbox)";
            this.rptMesajlarPaging.SayfaNo = 0;
            mesaj_durumu_id = 4;
            BindMesajlar(4);

        }

    }
}