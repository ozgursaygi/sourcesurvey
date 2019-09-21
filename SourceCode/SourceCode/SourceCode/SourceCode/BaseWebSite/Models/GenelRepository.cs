using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;
using System.Text;

namespace BaseWebSite.Models
{
    public class GenelRepository : BaseDB.BaseRepository<GenelContainer1>
    {
        #region Ileti işlemleri
        public IQueryable<gnl_message> AllMessages()
        {
            return db.gnl_message;
        }
        public gnl_message GetMessage(Guid message_uid)
        {
            return db.gnl_message.SingleOrDefault(d => d.message_uid == message_uid);
        }

       
        public void AddMessage(gnl_message message)
        {
            message.message_uid = Guid.NewGuid();
            db.gnl_message.AddObject(message);
        }
        public void DeleteMessageRecord(gnl_message message)
        {
            db.gnl_message.DeleteObject(message);
        }

        public void DeleteMessage(Guid message_uid)
        {
            this.DeleteMessageRecord(this.GetMessage(message_uid));
        }
        public gnl_message_inbox GetMessageFromInbox(Guid message_uid, Guid alici_uid)
        {
            return db.gnl_message_inbox.SingleOrDefault(gnl => gnl.message_uid == message_uid && gnl.user_uid == alici_uid);
        }

        public void DeleteMessageFromInboxPerminantly(gnl_message_inbox gelen)
        {
            db.gnl_message_inbox.DeleteObject(gelen);
        }

        public static DataSet InboxMessages(Guid user_uid, string filter, bool deleted_flag = false)
        {
            string query =
            @"Select 
               [message_uid]
              ,[gonderen_adi]
              ,[user_uid]
              ,[recipient_type]
              ,[message_is_read]
              ,[read_date]
              ,[is_deleted]
              ,[message_subject]
              ,[message]
              ,[sent_date]
              ,[zamani]
              ,[to_alici_isimleri]
              ,[mesaj_ad]
            from 
                gnl_ileti_gelen_kutusu_v 
            Where ( user_uid='{0}')
                {1}
                And (IsNull(is_deleted,0)={2})
            ORDER BY 
                sent_date DESC";
            return BaseDB.DBManager.AppConnection.GetDataSet(string.Format(query, user_uid.ToString(), filter, deleted_flag ? "1" : "0"));
        }

        public static DataSet InboxMessagesDashBoard(Guid kullanici_uid, string filter, bool deleted_flag = false)
        {
            string query =
            @"Select Top 5
                [message_uid]
              ,[gonderen_adi]
              ,[user_uid]
              ,[recipient_type]
              ,[message_is_read]
              ,[read_date]
              ,[is_deleted]
              ,[message_subject]
              ,[message]
              ,[sent_date]
              ,[zamani]
              ,[to_alici_isimleri]
              ,[mesaj_ad]
            from 
                gnl_ileti_gelen_kutusu_v 
            Where ( user_uid='{0}')
                {1}
                And (IsNull(is_deleted,0)={2})
            ORDER BY 
                sent_date DESC";
            return BaseDB.DBManager.AppConnection.GetDataSet(string.Format(query, kullanici_uid.ToString(), filter, deleted_flag ? "1" : "0"));
        }
       
        public static DataSet SentMessages(Guid send_user_uid,bool deleted_flag = false)
        {
            string query =
            @"SELECT 
                [message_uid]
              ,[message_subject]
               ,[gonderen_adi]
              ,[message]
              ,[sent_date]
              ,[zamani]
              ,[to_alici_isimleri]
              ,[mesaj_ad]
              ,[is_deleted]
              ,[deleted_at]
              ,[deleted_by]
          FROM 
            [dbo].[gnl_ileti_giden_kutusu_v]
            Where ( send_user_uid='{0}')
                And (IsNull(is_deleted,0)={1})
                and (recipient_type <>'Z' )
            ORDER BY 
                sent_date DESC";
            return BaseDB.DBManager.AppConnection.GetDataSet(string.Format(query, send_user_uid.ToString(), deleted_flag ? "1" : "0"));
        }

        public void send_mail(ArrayList mail_arr, ArrayList mail_ad_arr, ArrayList mail_soyad_arr, string applicationPath, DateTime mesaj_gonderim_tarihi, string subject , string message_gnl,string gonderen_ad,string gonderen_soyad)
        {
            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();

            System.IO.StreamReader template = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"Survey/Templates\MesajMailTemplate.html");
            string mailBody = template.ReadToEnd();
            template.Close();
            for (int i = 0; i < mail_arr.Count; i++)
            {
                string tempMailBody = mailBody;
                tempMailBody = tempMailBody.Replace("%%path_url%%", applicationPath);
                tempMailBody = tempMailBody.Replace("%%isim%%", mail_ad_arr[i].ToString() + " " + mail_soyad_arr[i].ToString());
                tempMailBody = tempMailBody.Replace("%%tarih%%", mesaj_gonderim_tarihi.ToString());
                tempMailBody = tempMailBody.Replace("%%subject%%", subject);
                tempMailBody = tempMailBody.Replace("%%message_gnl%%", message_gnl);
                tempMailBody = tempMailBody.Replace("%%gonderen%%", gonderen_ad+" "+gonderen_soyad);

                try
                {
                    if (BaseClasses.BaseFunctions.getInstance().IsEmailValid(mail_arr[i].ToString().Trim()))
                    {
                        BaseClasses.BaseFunctions.getInstance().SendSMTPMail(mail_arr[i].ToString(), "", "Mesaj Bilgileri", tempMailBody, "", null, "", "genel");
                    }

                }
                catch (Exception exp)
                {
                    continue;
                }
            }
        }


        public void send_message_to_members(ArrayList mail_arr, ArrayList mail_ad_arr, ArrayList mail_soyad_arr,ArrayList mail_user_uid_arr, string applicationPath, DateTime mesaj_gonderim_tarihi, string subject, string message_gnl, string gonderen_ad, string gonderen_soyad)
        {
            System.IO.StreamReader template = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"Survey/Templates\MesajMailTemplate.html");
            string mailBody = template.ReadToEnd();
            template.Close();
            for (int i = 0; i < mail_arr.Count; i++)
            {
                gnl_message message = new gnl_message();
                message.message_subject = subject;
                message.message = message_gnl;
                message.send_user_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;

                Guid[] arr_guid = new Guid[1];
                arr_guid[0] = Guid.Parse(mail_user_uid_arr[i].ToString());

                message.AddReceiver(arr_guid, "Z");

                if (message.IsValid)
                {
                    message.Send();
                    this.AddMessage(message);
                    this.Kaydet();
                }

                string tempMailBody = mailBody;
                tempMailBody = tempMailBody.Replace("%%path_url%%", applicationPath);
                tempMailBody = tempMailBody.Replace("%%isim%%", mail_ad_arr[i].ToString() + " " + mail_soyad_arr[i].ToString());
                tempMailBody = tempMailBody.Replace("%%tarih%%", mesaj_gonderim_tarihi.ToString());
                tempMailBody = tempMailBody.Replace("%%subject%%", subject);
                tempMailBody = tempMailBody.Replace("%%message_gnl%%", message_gnl);
                tempMailBody = tempMailBody.Replace("%%gonderen%%", gonderen_ad + " " + gonderen_soyad);

                try
                {
                    if (BaseClasses.BaseFunctions.getInstance().IsEmailValid(mail_arr[i].ToString().Trim()))
                    {
                        BaseClasses.BaseFunctions.getInstance().SendSMTPMail(mail_arr[i].ToString(), "", "Mesaj Bilgileri", tempMailBody, "", null, "", "genel");
                    }

                }
                catch (Exception exp)
                {
                    continue;
                }
            }
        }

        #endregion

        #region Kullanıcı Register İşlemleri
        public gnl_users GetUsers(Guid user_uid)
        {
            return db.gnl_users.SingleOrDefault(d => d.user_uid == user_uid);
        }

        public gnl_users GetUsersByGroup(Guid group_uid)
        {
            return db.gnl_users.SingleOrDefault(d => d.group_uid == group_uid);
        }

        public gnl_users GetUsersByEmail(string email)
        {
            return db.gnl_users.SingleOrDefault(d => d.email == email);
        }

        public void AddUsers(gnl_users user)
        {
            user.user_uid = Guid.NewGuid();
            db.gnl_users.AddObject(user);
        }
        #endregion

        #region Üye Kullanıcı İşlemleri
        public gnl_uye_kullanicilar GetMemberUsers(Guid user_uid)
        {
            return db.gnl_uye_kullanicilar.SingleOrDefault(d => d.user_uid == user_uid);
        }

        public gnl_uye_kullanicilar GetMemberUsersByGroup(Guid grup_uid)
        {
            return db.gnl_uye_kullanicilar.SingleOrDefault(d => d.grup_uid == grup_uid);
        }

        public gnl_uye_kullanicilar GetMemberUsersByMail(string email)
        {
            return db.gnl_uye_kullanicilar.SingleOrDefault(d => d.email == email);
        }

        public void AddMemberUsers(gnl_uye_kullanicilar user)
        {
            user.id = Guid.NewGuid();
            db.gnl_uye_kullanicilar.AddObject(user);
        }

        public bool IsPurchasedUser(Guid user_uid)
        {
            bool result = false;

            gnl_uye_kullanicilar member = this.GetMemberUsers(user_uid);

            if (member != null)
            {
                if (member.aktif != false && member.uye_baslangic_tarihi != null && member.uye_bitis_tarihi != null)
                {
                    if (member.uye_baslangic_tarihi <= DateTime.Now && member.uye_bitis_tarihi >= DateTime.Now)
                        result = true;
                }
            }


            return result;
        }


        public bool IsPurchasedUserGrubaGore(Guid grup_uid)
        {
            bool result = false;

            gnl_uye_kullanicilar member = this.GetMemberUsersByGroup(grup_uid);

            if (member != null)
            {
                if (member.aktif != false && member.uye_baslangic_tarihi != null && member.uye_bitis_tarihi != null)
                {
                    if (member.uye_baslangic_tarihi <= DateTime.Now && member.uye_bitis_tarihi >= DateTime.Now)
                        result = true;
                }
            }


            return result;
        }

        public int UyeKullaniciOdemeTipi(Guid user_uid)
        {
            int result = -1;
            if (this.IsPurchasedUser(user_uid))
            {
                gnl_uye_kullanicilar member = this.GetMemberUsers(user_uid);

                if(member != null)
                {
                    if(member.son_odeme_tipi_id!=null)
                        result = Convert.ToInt32(member.son_odeme_tipi_id.ToString());
                }
            }
            
            return result;
        }

        public DataSet SistemAdminKullanicilar()
        {
            return BaseDB.DBManager.AppConnection.GetDataSet("Select * From gnl_users Where aktif = 1 and is_sistem_admin=1");
        }

        public DataSet TumUyeKullanicilar()
        {
            return BaseDB.DBManager.AppConnection.GetDataSet("Select * From gnl_uye_kullanicilar Where aktif = 1 ");
        }



        public void UyelikBitisBilgilendirmeNotuGonder()
        {
            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();
            DateTime dt_noweksi1 = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day, new System.Globalization.CultureInfo("tr-TR")).AddDays(7);
            DateTime dt_noweksi2 = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day, new System.Globalization.CultureInfo("tr-TR")).AddDays(8);

            DateTime dt_noweksi3 = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day, new System.Globalization.CultureInfo("tr-TR"));
            DateTime dt_noweksi4 = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day, new System.Globalization.CultureInfo("tr-TR")).AddDays(1);

            DataSet ds = BaseDB.DBManager.AppConnection.GetDataSet("select * from gnl_uye_kullanicilar where uye_bitis_tarihi >=cast('" + dt_noweksi1.Year + "-" + dt_noweksi1.Month + "-" + dt_noweksi1.Day + "' as datetime) and uye_bitis_tarihi <=cast('" + dt_noweksi2.Year + "-" + dt_noweksi2.Month + "-" + dt_noweksi2.Day + "' as datetime) and not exists (select * from gnl_uyelik_bitis_bilgilendirme_mail_gonderi_tarihcesi where gnl_uyelik_bitis_bilgilendirme_mail_gonderi_tarihcesi.user_uid=gnl_uye_kullanicilar.user_uid and gonderilme_tarihi >=cast('" + dt_noweksi3.Year + "-" + dt_noweksi3.Month + "-" + dt_noweksi3.Day + "' as datetime) and gonderilme_tarihi <=cast('" + dt_noweksi4.Year + "-" + dt_noweksi4.Month + "-" + dt_noweksi4.Day + "' as datetime))");

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                if (dr["user_uid"] != System.DBNull.Value && dr["user_uid"].ToString() != "")
                {
                    Guid try_result = new Guid();
                    if (Guid.TryParse(dr["user_uid"].ToString(), out try_result))
                    {
                        gnl_uye_kullanicilar members = gnlDB.GetMemberUsers(try_result);
                        if (members != null)
                        {
                            string applicationPath = "";
                            if (System.Web.HttpContext.Current.Request.ApplicationPath == "/")
                                applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/";
                            else
                                applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/" + System.Web.HttpContext.Current.Request.ApplicationPath + "/";

                            uyelik_bitis_bilgilendirme_maili_gonder(applicationPath,  Guid.Parse(members.user_uid.ToString()), members.email, members.ad, members.soyad, Convert.ToDateTime(members.uye_bitis_tarihi.ToString(),new System.Globalization.CultureInfo("tr-TR")));
                            
                            gnl_uyelik_bitis_bilgilendirme_mail_gonderi_tarihcesi tarihce=new gnl_uyelik_bitis_bilgilendirme_mail_gonderi_tarihcesi();
                            
                            this.UyelikBitisBilgilendirmeMailGonderiEkle(tarihce);
                            tarihce.gonderen_user_uid = Guid.Empty;
                            tarihce.gonderilen_ad = members.ad;
                            tarihce.gonderilen_email = members.email;
                            tarihce.gonderilen_soyad = members.soyad;
                            tarihce.gonderilme_tarihi = DateTime.Now;
                            tarihce.user_uid = members.user_uid;

                            this.Kaydet();
                        }
                    }
                }

            }

        }

        public void uyelik_bitis_bilgilendirme_maili_gonder(string applicationPath, Guid user_uid, string user_email, string user_ad, string user_soyad,DateTime tarih)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            System.IO.StreamReader template = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"Templates\UyelikBitisBilgilendirmeMailTemplate.html");
            string mailBody = template.ReadToEnd();
            template.Close();

            string tempMailBody = mailBody;
            tempMailBody = tempMailBody.Replace("%%path_url%%", applicationPath);
            tempMailBody = tempMailBody.Replace("%%isim%%", user_ad + " " + user_soyad);
            tempMailBody = tempMailBody.Replace("%%tarih%%", tarih.ToString());
            
            try
            {
                if (BaseClasses.BaseFunctions.getInstance().IsEmailValid(user_email))
                {
                    BaseClasses.BaseFunctions.getInstance().SendSMTPMail(user_email, "", "Üyelik Bitiş Bilgilendirme Notu", tempMailBody, "", null, "", "genel");
                }

            }
            catch (Exception exp)
            {

            }
        }
        #endregion

        #region User Groups 
        public gnl_user_groups GetUserGroup(Guid group_uid)
        {
            return db.gnl_user_groups.SingleOrDefault(d => d.group_uid == group_uid);
        }

        public void AddUserGroup(gnl_user_groups group)
        {
            group.group_uid = Guid.NewGuid();
            db.gnl_user_groups.AddObject(group);
        }

        public DataSet GetUserGroupDataSet(int group_state_id)
        {
            DataSet ds_result = new DataSet();
            if (group_state_id == 0)
                ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from gnl_kullanici_gruplari_v where kullanici_uid='" + BaseDB.SessionContext.Current.ActiveUser.UserUid + "' and active=1 and aktif_kullanici=1");
            else if(group_state_id == 1)
                ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from gnl_kullanici_gruplari_v where kullanici_uid='" + BaseDB.SessionContext.Current.ActiveUser.UserUid + "' and active=1 and aktif_kullanici=1 and is_admin=1");
            else if (group_state_id == 2)
                ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from gnl_kullanici_gruplari_v where kullanici_uid='" + BaseDB.SessionContext.Current.ActiveUser.UserUid + "' and active=1 and aktif_kullanici=1 and (is_admin is null or is_admin=0)");

            return ds_result;
        }

        public bool HasUserGroup(Guid user_uid)
        {
            bool result = false;

            
            
            string ks = BaseDB.DBManager.AppConnection.ExecuteSql("select count(*) from dbo.sbr_anket_kullanici_gruplari('" + BaseDB.SessionContext.Current.ActiveUser.UserUid + "')");

            if (ks != "" && ks != "0")
                result = true;

            return result;
        }

        public DataSet GetGroupUsersDataSet(Guid group_uid)
        {
            DataSet ds_result = new DataSet();
            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from gnl_kullanici_gruplari_v where grup_uid='" + group_uid + "' and aktif_kullanici=1");
            
            return ds_result;
        }
        #endregion

        #region Group User 
        public void GroupAddUser(gnl_group_user_definitions group)
        {
            group.id = Guid.NewGuid();
            db.gnl_group_user_definitions.AddObject(group);
        }

        public gnl_group_user_definitions GrupGetUser(Guid group_uid, Guid user_uid)
        {
            return db.gnl_group_user_definitions.SingleOrDefault(d => d.group_uid == group_uid && d.user_uid == user_uid);
        }

        public bool IsGrupUserAdmin(Guid group_uid, Guid user_uid)
        {
            bool result=false;
            gnl_group_user_definitions gk = db.gnl_group_user_definitions.SingleOrDefault(d => d.group_uid == group_uid && d.user_uid == user_uid);

            if (gk != null)
            {
                if (gk.is_admin == true)
                    result = true;
            }

            return result;
        }
        #endregion

        #region Şifre Sıfırlama
        public void SifreSifirlamaEkle(gnl_kullanici_sifre_sifirlama sifre)
        {
            sifre.id = Guid.NewGuid();
            db.gnl_kullanici_sifre_sifirlama.AddObject(sifre);
        }

        public gnl_kullanici_sifre_sifirlama SifreSifirlamaGetir(Guid id)
        {
            return db.gnl_kullanici_sifre_sifirlama.SingleOrDefault(d => d.id == id);
        }

        public gnl_kullanici_sifre_sifirlama SifreSifirlamaGetir(string key)
        {
            return db.gnl_kullanici_sifre_sifirlama.SingleOrDefault(d => d.sifirlama_key == key);
        }

        public void SifreSifirla(string sifre_sifirlama_key)
        {
            gnl_kullanici_sifre_sifirlama sifre = this.SifreSifirlamaGetir(sifre_sifirlama_key);
            gnl_users user = new gnl_users();

            if (sifre != null && (sifre.sifirlama_kabul_edildi == null))
            {
                user = this.GetUsersByEmail(sifre.sifre_sifirlanacak_email);

                if (user != null)
                {

                    sifre.sifirlama_kabul_edildi = true;
                    sifre.sifirlama_kabul_edilme_tarihi = DateTime.Now;
                    this.Kaydet();

                    user.password = sifre.sifre;
                    this.Kaydet();
                }

            }
        }

        #endregion

        #region Üyelik Paket İşlemleri
        public void UyePaketAlimiEkle(gnl_uyelik_paket_alimlari paket)
        {
            paket.id = Guid.NewGuid();
            db.gnl_uyelik_paket_alimlari.AddObject(paket);
        }

        public gnl_uyelik_paket_alimlari UyePaketAlimiGetir(Guid id)
        {
            return db.gnl_uyelik_paket_alimlari.SingleOrDefault(d => d.id == id);
        }

        public string UyelikDurumuTextBelirle(int odeme_tipi, Guid user_id)
        {
            gnl_uyelik_odeme_tipleri_tanimlari uyelik_odeme_tipleri = this.UyelikOdemeTipiGetir(odeme_tipi);

            int uyelik_sure = Convert.ToInt32(uyelik_odeme_tipleri.sure_ay.ToString());
            int anket_sayisi = Convert.ToInt32(uyelik_odeme_tipleri.anket_sayisi.ToString());
            decimal anket_ucret = Convert.ToDecimal(uyelik_odeme_tipleri.ucret.ToString());

            gnl_uye_kullanicilar member_users = this.GetMemberUsers(user_id);
            DateTime dt = DateTime.Now.AddMonths(uyelik_sure);
            int kalan_anket_sayisi = 0;
            if (member_users != null)
            {
                dt = (member_users.uye_bitis_tarihi.HasValue) ? Convert.ToDateTime(member_users.uye_bitis_tarihi.ToString(), new System.Globalization.CultureInfo("tr-TR")).AddMonths(uyelik_sure).Date : DateTime.Now.AddMonths(uyelik_sure).Date;
                kalan_anket_sayisi = (member_users.kalan_anket_sayisi.HasValue && (this.IsPurchasedUser(user_id))) ? Convert.ToInt32(member_users.kalan_anket_sayisi) + ((this.IsPurchasedUser(user_id)) ? anket_sayisi : 0) : anket_sayisi;
            }
            else
            {
                kalan_anket_sayisi = anket_sayisi;
            }

            return "Üyelik Bitiş Tarihi : " + dt.Date.ToString() + " , Toplam Yapılacak Survey Sayısı : " + kalan_anket_sayisi.ToString() + " , Paket Ücreti : " + anket_ucret.ToString() + " TL";

        }

        public string VarUyelikDurumuTextBelirle(Guid user_id)
        {
            string result = "";
            DataSet ds = BaseDB.DBManager.AppConnection.GetDataSet("select gnl_uye_kullanicilar.*,'Üyelik Bitiş Tarihi '+cast(day(gnl_uye_kullanicilar.uye_bitis_tarihi) as varchar(2))+'/'+cast(month(gnl_uye_kullanicilar.uye_bitis_tarihi) as varchar(2))+'/'+cast(year(gnl_uye_kullanicilar.uye_bitis_tarihi) as varchar(4))+' - '+cast (gnl_uye_kullanicilar.kalan_anket_sayisi as varchar(20))+' Survey - Survey Başına '+cast (gnl_uyelik_odeme_tipleri_tanimlari.katilimci_sayisi as varchar(20)) +' Katılımcı'  as uyelik_durumu  from gnl_uye_kullanicilar,gnl_uyelik_odeme_tipleri_tanimlari where odeme_tipi_id=son_odeme_tipi_id  and user_uid='" + BaseDB.SessionContext.Current.ActiveUser.UserUid + "'");

            if (ds.Tables[0].Rows.Count > 0)
            {
                result = ds.Tables[0].Rows[0]["uyelik_durumu"].ToString();
            }

            if (!IsPurchasedUser(user_id))
            {
                result = "Üyeliğiniz Bitmiştir.Yeni bir paket alıp kullanmaya devam edebilirsiniz.";
            }

            return result;
        }

        public DataSet PaketAlimlariGetirDataSet(Guid user_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from gnl_paket_alimlari_v where aktive_edildi=1 and user_uid='" + user_uid + "' order by paket_alim_tarihi");
            
            return ds_result;
        }

        public DataSet PaketAlimlariHavaleAktiveOlacakGetirDataSet()
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from gnl_paket_alimlari_v where (aktive_edildi is null or aktive_edildi=0) order by paket_alim_tarihi desc");

            return ds_result;
        }

        public DataSet PaketAlimlariFaturaKesilecekGetirDataSet()
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from gnl_paket_alimlari_v where aktive_edildi=1 and (fatura_kesildi is null or fatura_kesildi=0) order by paket_alim_tarihi desc");

            return ds_result;
        }

        public DataSet PaketAlimlariFaturaKesilenGetirDataSet()
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from gnl_paket_alimlari_v where aktive_edildi=1 and fatura_kesildi=1 order by paket_alim_tarihi desc");

            return ds_result;
        }

        public void banka_havalesi_onay_maili_gonder(string applicationPath, Guid user_uid, string user_email, string user_ad, string user_soyad,string paket_durumu)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            System.IO.StreamReader template = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"Templates\HavaleOdemeMailTemplate.html");
            string mailBody = template.ReadToEnd();
            template.Close();

            string tempMailBody = mailBody;
            tempMailBody = tempMailBody.Replace("%%path_url%%", applicationPath);
            tempMailBody = tempMailBody.Replace("%%ad%%", user_ad + " " + user_soyad);
            tempMailBody = tempMailBody.Replace("%%paket_durumu%%", paket_durumu);
            
            try
            {
                if (BaseClasses.BaseFunctions.getInstance().IsEmailValid(user_email))
                {
                    BaseClasses.BaseFunctions.getInstance().SendSMTPMail(user_email, "", "Banka Havalesi İle Paket Alım Bilgisi", tempMailBody, "", null, "", "genel");
                }

            }
            catch (Exception exp)
            {

            }
        }

        public void kredi_karti_onay_maili_gonder(string applicationPath, Guid user_uid, string user_email, string user_ad, string user_soyad, string paket_durumu)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            System.IO.StreamReader template = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"Templates\KrediKartiOdemeMailTemplate.html");
            string mailBody = template.ReadToEnd();
            template.Close();

            string tempMailBody = mailBody;
            tempMailBody = tempMailBody.Replace("%%path_url%%", applicationPath);
            tempMailBody = tempMailBody.Replace("%%ad%%", user_ad + " " + user_soyad);
            tempMailBody = tempMailBody.Replace("%%paket_durumu%%", paket_durumu);

            try
            {
                if (BaseClasses.BaseFunctions.getInstance().IsEmailValid(user_email))
                {
                    BaseClasses.BaseFunctions.getInstance().SendSMTPMail(user_email, "", "Kredi Kartı İle Paket Alım Bilgisi", tempMailBody, "", null, "", "genel");
                }

            }
            catch (Exception exp)
            {

            }
        }
        #endregion

        #region Üyelik Ödeme Tipleri
        public gnl_uyelik_odeme_tipleri_tanimlari UyelikOdemeTipiGetir(int odeme_tipi_id)
        {
            return db.gnl_uyelik_odeme_tipleri_tanimlari.SingleOrDefault(d => d.odeme_tipi_id == odeme_tipi_id);
        }

        public int OdemetipiSurveySayisiGetir(int odeme_tipi_id)
        {
            int result = 0;
            gnl_uyelik_odeme_tipleri_tanimlari uyelik = db.gnl_uyelik_odeme_tipleri_tanimlari.SingleOrDefault(d => d.odeme_tipi_id == odeme_tipi_id);

            if (uyelik != null)
            {
                result = Convert.ToInt32(uyelik.anket_sayisi.ToString());
            }

            return result;
        }

        public int OdemetipiKatilimciSayisiGetir(int odeme_tipi_id)
        {
            int result = 0;
            gnl_uyelik_odeme_tipleri_tanimlari uyelik = db.gnl_uyelik_odeme_tipleri_tanimlari.SingleOrDefault(d => d.odeme_tipi_id == odeme_tipi_id);

            if (uyelik != null)
            {
                result = Convert.ToInt32(uyelik.katilimci_sayisi.ToString());
            }

            return result;
        }

        #endregion

        #region Üyelik Aktivasyon Mail Gönderimi

        public void UyelikMailGonderiEkle(gnl_uyelik_aktivasyon_mail_gonderi_tarihcesi tarihce)
        {
            tarihce.id = Guid.NewGuid();
            db.gnl_uyelik_aktivasyon_mail_gonderi_tarihcesi.AddObject(tarihce);
        }

        public void uyelik_aktivasyon_maili_gonder(string ad, string soyad, string email, string applicationPath, DateTime paket_alim_tarihi,Guid paket_uid,string paket_durumu)
        {
            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();

            ArrayList mail_arr = new ArrayList();
            ArrayList mail_ad_arr = new ArrayList();
            ArrayList mail_soyad_arr = new ArrayList();

            mail_arr.Add(email);
            mail_ad_arr.Add(ad);
            mail_soyad_arr.Add(soyad);


            System.IO.StreamReader template = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"Admin/Templates\OdemeAktivasyonMailTemplate.html");
            string mailBody = template.ReadToEnd();
            template.Close();
            for (int i = 0; i < mail_arr.Count; i++)
            {

                string tempMailBody = mailBody;
                tempMailBody = tempMailBody.Replace("%%path_url%%", applicationPath);
                tempMailBody = tempMailBody.Replace("%%isim%%", mail_ad_arr[i].ToString() + " " + mail_soyad_arr[i].ToString());
                tempMailBody = tempMailBody.Replace("%%tarih%%", paket_alim_tarihi.ToString());
                tempMailBody = tempMailBody.Replace("%%paket_durumu%%", paket_durumu);

                try
                {
                    if (BaseClasses.BaseFunctions.getInstance().IsEmailValid(mail_arr[i].ToString().Trim()))
                    {
                        BaseClasses.BaseFunctions.getInstance().SendSMTPMail(mail_arr[i].ToString(), "", "Üyelik Aktivasyon Bilgileri", tempMailBody, "", null, "", "genel");
                    }

                }
                catch (Exception exp)
                {
                    continue;
                }

                if (BaseClasses.BaseFunctions.getInstance().IsEmailValid(mail_arr[i].ToString().Trim()))
                {
                    gnl_uyelik_aktivasyon_mail_gonderi_tarihcesi tarihce = new gnl_uyelik_aktivasyon_mail_gonderi_tarihcesi();
                    this.UyelikMailGonderiEkle(tarihce);
                    tarihce.gonderilen_ad = mail_ad_arr[i].ToString();
                    tarihce.gonderilen_soyad = mail_soyad_arr[i].ToString();
                    tarihce.gonderilen_email = mail_arr[i].ToString();
                    tarihce.gonderilme_tarihi = DateTime.Now;
                    tarihce.paket_uid = paket_uid;
                    tarihce.gonderen_user_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                    this.Kaydet();
                }


            }
        }
        #endregion

        #region Üyelik Bitis Bilgilendirme Mail Gönderimi

        public void UyelikBitisBilgilendirmeMailGonderiEkle(gnl_uyelik_bitis_bilgilendirme_mail_gonderi_tarihcesi tarihce)
        {
            tarihce.id = Guid.NewGuid();
            db.gnl_uyelik_bitis_bilgilendirme_mail_gonderi_tarihcesi.AddObject(tarihce);
        }


        #endregion

        #region Notification
        public void NotificationAdd(gnl_notification notification)
        {
            notification.notification_uid = Guid.NewGuid();
            db.gnl_notification.AddObject(notification);
        }
        public gnl_notification NotificationGet(Guid notification_uid)
        {
            return db.gnl_notification.SingleOrDefault(d => d.notification_uid == notification_uid);
        }

        public DataSet NotificationDataSet()
        {
            return BaseDB.DBManager.AppConnection.GetDataSet("Select * From gnl_notification order by notification_date desc");
        }

        public DataSet NotificationDataSetDashBoard()
        {
            return BaseDB.DBManager.AppConnection.GetDataSet("Select top 5 * From gnl_notification where notification_statu=1 order by notification_date desc");
        }
        #endregion

        #region Suggest
        public void SaveSuggest(gnl_suggest_to_friend suggest)
        {
            suggest.id = Guid.NewGuid();
            db.gnl_suggest_to_friend.AddObject(suggest);
        }
        public gnl_suggest_to_friend GetSuggestToFriend(Guid id)
        {
            return db.gnl_suggest_to_friend.SingleOrDefault(d => d.id == id);
        }
        #endregion

        #region Mail Service
        public void MailServiceEkle(gnl_mail_service mail_service)
        {
            mail_service.id = Guid.NewGuid();
            db.gnl_mail_service.AddObject(mail_service);
        }
        
        public void InsertToMailService(string to, string subject, string body,Guid anket_uid)
        {
            gnl_mail_service mail_service=new gnl_mail_service();
            this.MailServiceEkle(mail_service);

            mail_service.eposta_to = to;
            mail_service.subject = subject;
            mail_service.body = body;
            mail_service.eklemetarihi = DateTime.Now;
            mail_service.gonderildi = false;
            mail_service.anket_uid = anket_uid;

            this.Kaydet();
            
        }
        #endregion

        #region Kredi Kartı İşlemleri
        public String ConstructQueryString(Dictionary<string, string> parameters)
        {
            List<string> Items = new List<string>();

            foreach (var name in parameters)
                Items.Add(String.Concat(name.Key, "=", name.Value));

            return String.Join("&", Items.ToArray());
        }

        public String PurchaseLink(Dictionary<string, string> args)
        {
            //bankaya göre değiştirilecek
            StringBuilder Link = new StringBuilder("http://www.dummylink090998j.com/hha?");
            String Parameters = ConstructQueryString(args);
            Link.Append(Parameters);
            return Link.ToString();
        }
        #endregion
    }
}