using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Reflection;
using System.Collections;
using BaseDB;
using System.IO;
using System.Text;

namespace BaseWebSite.Models
{
    public class SurveyRepository : BaseDB.BaseRepository<SurveyContainer>
    {
        #region Survey İşlemleri
        public IQueryable<sbr_anket> TumSurveyler()
        {
            return db.sbr_anket;
        }
        public sbr_anket SurveyGetir(Guid anket_uid)
        {
            return db.sbr_anket.SingleOrDefault(d => d.anket_uid == anket_uid);
        }
        public void SurveyEkle(sbr_anket anket)
        {
            anket.anket_uid = Guid.NewGuid();
            anket.grup_uid = BaseDB.SessionContext.Current.ActiveUser.GrupUid;
            anket.kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
            db.sbr_anket.AddObject(anket);
        }
        public void SurveySil(sbr_anket anket)
        {
            db.sbr_anket.DeleteObject(anket);
        }

        public void SurveySil(Guid anket_uid)
        {
            this.SurveySil(this.SurveyGetir(anket_uid));
        }


        public DataSet SurveyGetirDurumaGoreDataSet(int anket_durum_id,string criteria,Guid grup_uid)
        {
            DataSet ds_result = new DataSet();

            if (anket_durum_id == 0)
            {
                if (grup_uid != Guid.Empty)
                    ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_anket_v where grup_uid in ('" + grup_uid + "')" + (criteria != "" ? " and ( " + criteria + " )" : "") + " order by olusturma_tarihi desc");
                else
                    ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_anket_v where kullanici_uid in ('" + BaseDB.SessionContext.Current.ActiveUser.UserUid + "')" + (criteria != "" ? " and ( " + criteria + " )" : "") + " order by olusturma_tarihi desc");
            }
            else
            {
                if (grup_uid != Guid.Empty)
                    ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_anket_v where grup_uid in ('" + grup_uid + "') and anket_durumu_id=" + anket_durum_id + (criteria != "" ? " and ( " + criteria + " )" : "") + " order by olusturma_tarihi desc");
                else
                    ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_anket_v where kullanici_uid in ('" + BaseDB.SessionContext.Current.ActiveUser.UserUid + "') and anket_durumu_id=" + anket_durum_id + (criteria != "" ? " and ( " + criteria + " )" : "") + " order by olusturma_tarihi desc");
            }
            return ds_result;
        }

        public DataSet SurveyGetirDurumaGoreDataSet(int anket_durum_id, string criteria, string grup_uid)
        {
            DataSet ds_result = new DataSet();

            if (anket_durum_id == 0)
            {
                if (grup_uid != Guid.Empty.ToString())
                    ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_anket_v where grup_uid in (" + grup_uid + ")" + (criteria != "" ? " and ( " + criteria + " )" : "") + " order by olusturma_tarihi desc");
                else
                    ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_anket_v where kullanici_uid in ('" + BaseDB.SessionContext.Current.ActiveUser.UserUid + "')" + (criteria != "" ? " and ( " + criteria + " )" : "") + " order by olusturma_tarihi desc");
            }
            else
            {
                if (grup_uid != Guid.Empty.ToString())
                    ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_anket_v where grup_uid in (" + grup_uid + ") and anket_durumu_id=" + anket_durum_id + (criteria != "" ? " and ( " + criteria + " )" : "") + " order by olusturma_tarihi desc");
                else
                    ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_anket_v where kullanici_uid in ('" + BaseDB.SessionContext.Current.ActiveUser.UserUid + "') and anket_durumu_id=" + anket_durum_id + (criteria != "" ? " and ( " + criteria + " )" : "") + " order by olusturma_tarihi desc");
            }
            return ds_result;
        }

        public void SurveyDurumuDegistir(int anket_durumu_id, Guid anket_uid,string aciklama,Guid olusturan_user_uid)
        {
            sbr_anket_durum_tarihcesi anket_durumu = new sbr_anket_durum_tarihcesi();

            this.SurveyDurumuEkle(anket_durumu);
            anket_durumu.anket_durumu_id = anket_durumu_id;
            anket_durumu.anket_uid = anket_uid;
            anket_durumu.durum_olusma_tarihi = DateTime.Now;
            anket_durumu.durumu_olusturan_kullanici = olusturan_user_uid;
            anket_durumu.durum_aciklamasi = aciklama;
            
            this.Kaydet();

            sbr_anket anket = this.SurveyGetir(anket_uid);
            anket.anket_durumu_id = anket_durumu_id;
            anket.anket_durumu_uid = anket_durumu.anket_durumu_uid;

            this.Kaydet();
        }

        public string SurveyDurumu(int anket_durum_tipi_id)
        {
            string result = "";

            if (anket_durum_tipi_id.ToString() == ((int)BaseWebSite.Models.anket_durumu.Acik).ToString())
            {
                result = "Open";
            }
            else if (anket_durum_tipi_id.ToString() == ((int)BaseWebSite.Models.anket_durumu.Kapali).ToString())
            {
                result = "Closed";
            }
            else if (anket_durum_tipi_id.ToString() == ((int)BaseWebSite.Models.anket_durumu.Arsivde).ToString())
            {
                result = "Archived";
            }
            else if (anket_durum_tipi_id.ToString() == ((int)BaseWebSite.Models.anket_durumu.Yayinda).ToString())
            {
                result = "Live";
            }

            return result;
        }

        public bool SurveyIcinHerhangiBirCevapVerilmisMi(Guid anket_uid)
        {
            bool anket_cevaplandimi = false;

            string kayit_sayisi_tip1 = BaseDB.DBManager.AppConnection.ExecuteSql("select count(*) from sbr_soru_tipi_1_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_1_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid  and sbr_anket_sorulari.anket_uid='" + anket_uid + "'");
            string kayit_sayisi_tip2 = BaseDB.DBManager.AppConnection.ExecuteSql("select count(*) from sbr_soru_tipi_2_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_2_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid  and sbr_anket_sorulari.anket_uid='" + anket_uid + "'");
            string kayit_sayisi_tip3 = BaseDB.DBManager.AppConnection.ExecuteSql("select count(*) from sbr_soru_tipi_3_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_3_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid  and sbr_anket_sorulari.anket_uid='" + anket_uid + "'");
            string kayit_sayisi_tip4 = BaseDB.DBManager.AppConnection.ExecuteSql("select count(*) from sbr_soru_tipi_4_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_4_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid  and sbr_anket_sorulari.anket_uid='" + anket_uid + "'");
            string kayit_sayisi_tip5 = BaseDB.DBManager.AppConnection.ExecuteSql("select count(*) from sbr_soru_tipi_5_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_5_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid  and sbr_anket_sorulari.anket_uid='" + anket_uid + "'");
            string kayit_sayisi_tip6 = BaseDB.DBManager.AppConnection.ExecuteSql("select count(*) from sbr_soru_tipi_6_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_6_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid  and sbr_anket_sorulari.anket_uid='" + anket_uid + "'");
            string kayit_sayisi_tip7 = BaseDB.DBManager.AppConnection.ExecuteSql("select count(*) from sbr_soru_tipi_7_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_7_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid  and sbr_anket_sorulari.anket_uid='" + anket_uid + "'");
            string kayit_sayisi_tip8 = BaseDB.DBManager.AppConnection.ExecuteSql("select count(*) from sbr_soru_tipi_8_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_8_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid  and sbr_anket_sorulari.anket_uid='" + anket_uid + "'");
            string kayit_sayisi_tip9 = BaseDB.DBManager.AppConnection.ExecuteSql("select count(*) from sbr_soru_tipi_9_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_9_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid  and sbr_anket_sorulari.anket_uid='" + anket_uid + "'");
            string kayit_sayisi_tip10 = BaseDB.DBManager.AppConnection.ExecuteSql("select count(*) from sbr_soru_tipi_10_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_10_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid  and sbr_anket_sorulari.anket_uid='" + anket_uid + "'");
            string kayit_sayisi_tip11 = BaseDB.DBManager.AppConnection.ExecuteSql("select count(*) from sbr_soru_tipi_11_cevaplari,sbr_anket_sorulari where sbr_soru_tipi_11_cevaplari.soru_uid=sbr_anket_sorulari.soru_uid  and sbr_anket_sorulari.anket_uid='" + anket_uid + "'");

            if ((kayit_sayisi_tip1 != "" && kayit_sayisi_tip1 != "0") || (kayit_sayisi_tip2 != "" && kayit_sayisi_tip2 != "0") || (kayit_sayisi_tip3 != "" && kayit_sayisi_tip3 != "0") || (kayit_sayisi_tip4 != "" && kayit_sayisi_tip4 != "0") || (kayit_sayisi_tip5 != "" && kayit_sayisi_tip5 != "0") || (kayit_sayisi_tip6 != "" && kayit_sayisi_tip6 != "0") || (kayit_sayisi_tip7 != "" && kayit_sayisi_tip7 != "0") || (kayit_sayisi_tip8 != "" && kayit_sayisi_tip8 != "0") || (kayit_sayisi_tip9 != "" && kayit_sayisi_tip9 != "0") || (kayit_sayisi_tip10 != "" && kayit_sayisi_tip10 != "0") || (kayit_sayisi_tip11 != "" && kayit_sayisi_tip11 != "0"))
                anket_cevaplandimi = true;

            return anket_cevaplandimi;
        }

        public int OlusturulanSurveySayisiGetir(Guid user_uid)
        {
            int result = 0;

            string kayit = BaseDB.DBManager.AppConnection.ExecuteSql("select count(*) from sbr_anket where kullanici_uid='"+user_uid+"'");

            if (kayit != "" && kayit != "0")
            {
                result = Convert.ToInt32(kayit);
            }

            return result;
        }

        public int OlusturulanSurveySayisiGetirGrubaGore(Guid grup_uid)
        {
            int result = 0;

            string kayit = BaseDB.DBManager.AppConnection.ExecuteSql("select count(*) from sbr_anket where grup_uid='" + grup_uid + "'");

            if (kayit != "" && kayit != "0")
            {
                result = Convert.ToInt32(kayit);
            }

            return result;
        }

        public void SurveyleriKapat()
        {
            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();
            DateTime dt_noweksi1 = Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day, new System.Globalization.CultureInfo("tr-TR")).AddDays(-1);
                
           DataSet ds = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_anket where (anket_durumu_id=" + ((int)BaseWebSite.Models.anket_durumu.Acik).ToString() + " or anket_durumu_id=" + ((int)BaseWebSite.Models.anket_durumu.Yayinda).ToString()+") and bitis_tarihi <=cast('"+dt_noweksi1.Year+"-"+dt_noweksi1.Month+"-"+dt_noweksi1.Day+"' as datetime)");

           foreach (DataRow dr in ds.Tables[0].Rows)
           {
               if (dr["kullanici_uid"] != System.DBNull.Value && dr["kullanici_uid"].ToString() != "")
               {
                   Guid try_result = new Guid();
                   if (Guid.TryParse(dr["kullanici_uid"].ToString(), out try_result))
                   {
                       gnl_uye_kullanicilar members = gnlDB.GetMemberUsers(try_result);
                       if (members != null)
                       {
                           Guid anket_uid = Guid.Parse(dr["anket_uid"].ToString());
                           sbr_anket anket = this.SurveyGetir(anket_uid);

                           this.SurveyDurumuDegistir(((int)BaseWebSite.Models.anket_durumu.Kapali), anket_uid, "Closed automatically by system on End Date.", Guid.Empty);

                           string applicationPath = "";
                           if (System.Web.HttpContext.Current.Request.ApplicationPath == "/")
                               applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/";
                           else
                               applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/" + System.Web.HttpContext.Current.Request.ApplicationPath + "/";

                           anket_kapanis_maili_gonder(applicationPath, anket_uid, anket.anket_adi, Guid.Parse(members.user_uid.ToString()), members.email, members.ad, members.soyad);
                       }
                       else
                       {
                           gnl_users kul = gnlDB.GetUsers(try_result);
                           if (kul != null)
                           {
                               Guid anket_uid = Guid.Parse(dr["anket_uid"].ToString());
                               sbr_anket anket = this.SurveyGetir(anket_uid);

                               this.SurveyDurumuDegistir(((int)BaseWebSite.Models.anket_durumu.Kapali), anket_uid, "Closed automatically by system on End Date.", Guid.Empty);

                               string applicationPath = "";
                               if (System.Web.HttpContext.Current.Request.ApplicationPath == "/")
                                   applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/";
                               else
                                   applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/" + System.Web.HttpContext.Current.Request.ApplicationPath + "/";

                               anket_kapanis_maili_gonder(applicationPath, anket_uid, anket.anket_adi, Guid.Parse(kul.user_uid.ToString()), kul.email, kul.name, kul.surname);
                           }
                       }
                   }
               }
           
           }

        }


      
        public void anket_kapanis_maili_gonder(string applicationPath, Guid anket_uid, string anket_adi, Guid user_uid, string user_email, string user_ad, string user_soyad)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();

            System.IO.StreamReader template = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"Anket\Templates\AnketKapanisMailTemplate.html");
            string mailBody = template.ReadToEnd();
            template.Close();

            string tempMailBody = mailBody;
            tempMailBody = tempMailBody.Replace("%%path_url%%", applicationPath);
            tempMailBody = tempMailBody.Replace("%%isim%%", user_ad + " " + user_soyad);
            tempMailBody = tempMailBody.Replace("%%anket_ismi%%", anket_adi);

            try
            {
                if (BaseClasses.BaseFunctions.getInstance().IsEmailValid(user_email))
                {
                   BaseClasses.BaseFunctions.getInstance().SendSMTPMail(user_email, "", "Survey Kapanış Bilgisi", tempMailBody, "", null, "", "genel");
                   
                }

            }
            catch (Exception exp)
            {

            }
        }

        public void anket_limit_bitis_maili_gonder(string applicationPath, Guid anket_uid, string anket_adi, Guid user_uid, string user_email, string user_ad, string user_soyad)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();

            System.IO.StreamReader template = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"Anket\Templates\AnketLimitBitisiMailTemplate.html");
            string mailBody = template.ReadToEnd();
            template.Close();

            string tempMailBody = mailBody;
            tempMailBody = tempMailBody.Replace("%%path_url%%", applicationPath);
            tempMailBody = tempMailBody.Replace("%%isim%%", user_ad + " " + user_soyad);
            
            try
            {
                if (BaseClasses.BaseFunctions.getInstance().IsEmailValid(user_email))
                {
                    BaseClasses.BaseFunctions.getInstance().SendSMTPMail(user_email, "", "Survey Limiti Bitiş Bilgisi", tempMailBody, "", null, "", "genel");
                    
                }

            }
            catch (Exception exp)
            {

            }
        }

        public string anket_dashbord_anket_durumu(Guid grup_uid)
        {
            string result = "";

            string aciklar = BaseDB.DBManager.AppConnection.ExecuteSql("select count(*) from sbr_anket_v where grup_uid in ('" + grup_uid + "') and anket_durumu_id=" + ((int)BaseWebSite.Models.anket_durumu.Acik).ToString());
            string kapalilar = BaseDB.DBManager.AppConnection.ExecuteSql("select count(*) from sbr_anket_v where grup_uid in ('" + grup_uid + "') and anket_durumu_id=" + ((int)BaseWebSite.Models.anket_durumu.Kapali).ToString());
            string yayinda = BaseDB.DBManager.AppConnection.ExecuteSql("select count(*) from sbr_anket_v where grup_uid in ('" + grup_uid + "') and anket_durumu_id=" + ((int)BaseWebSite.Models.anket_durumu.Yayinda).ToString());
            string arsivde = BaseDB.DBManager.AppConnection.ExecuteSql("select count(*) from sbr_anket_v where grup_uid in ('" + grup_uid + "') and anket_durumu_id=" + ((int)BaseWebSite.Models.anket_durumu.Arsivde).ToString());

            StringBuilder str_result = new StringBuilder();
            str_result.Append("<a href=\"MainPage.aspx?anket_durumu_id=1\"> Opened Survey Count : <span class=\"failureNotification\">" + (aciklar == "" ? "0" : aciklar) + "<span></a><br><br>");
            str_result.Append("<a href=\"MainPage.aspx?anket_durumu_id=2\"> Closed Survey Count : <span class=\"failureNotification\">" + (kapalilar == "" ? "0" : kapalilar) + "<span></a><br><br>");
            str_result.Append("<a href=\"MainPage.aspx?anket_durumu_id=4\"> Published Survey Count : <span class=\"failureNotification\">" + (yayinda == "" ? "0" : yayinda) + "<span></a><br><br>");
            str_result.Append("<a href=\"MainPage.aspx?anket_durumu_id=3\"> Archived Survey Count : <span class=\"failureNotification\">" + (arsivde == "" ? "0" : arsivde) + "<span></a><br><br><br>");

            result = str_result.ToString();

            return result;
        }

        public string anket_dashbord_yayindakiler(Guid grup_uid)
        {
            string result = "";

            DataSet ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_anket_v where grup_uid in ('" + grup_uid + "') and anket_durumu_id=4 order by olusturma_tarihi desc");

            StringBuilder str_result = new StringBuilder();
            int i = 1;
            foreach (DataRow dr in ds_result.Tables[0].Rows)
            {
                str_result.Append("<span class=\"failureNotification\"> * </span><a href=\"AnketDetayi.aspx?anket_uid=" + dr["anket_uid"].ToString() + "\">" + dr["anket_adi"].ToString() + "</a><br><br>");
                i++;
            }


            if (ds_result.Tables[0].Rows.Count > 0)
                result = str_result.ToString();
            else
                result = "No Published Surveys.";

            return result;
        }

        public string anket_dashbord_mail_gruplari(Guid grup_uid)
        {
            string result = "";

            DataSet ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_mail_gruplari where grup_uid='" + grup_uid + "'");

            StringBuilder str_result = new StringBuilder();
            int i = 1;
            foreach (DataRow dr in ds_result.Tables[0].Rows)
            {
                str_result.Append("<span class=\"failureNotification\"> * </span><a href=\"AdresGruplari.aspx?mail_grubu_uid=" + dr["mail_grubu_uid"].ToString() + "\">" + dr["mail_grubu_adi"].ToString() + "</a><br><br>");
                i++;
            }

            if (ds_result.Tables[0].Rows.Count > 0)
                result = str_result.ToString();
            else
                result = "Not Defined Mail Group.";

            return result;
        }

        public string anket_dashbord_mesajlar(Guid user_uid)
        {
            string result = "";

            DataSet ds_result = GenelRepository.InboxMessagesDashBoard(user_uid, "");

            StringBuilder str_result = new StringBuilder();
            int i = 1;
            foreach (DataRow dr in ds_result.Tables[0].Rows)
            {
                str_result.Append("<span class=\"failureNotification\"> * </span><a href=\"Messages.aspx?mesaj_durumu_id=1\">" + dr["message_subject"].ToString() + " - " + dr["gonderim_tarihi"].ToString() + "</a><br><br>");
                i++;
            }

            if (ds_result.Tables[0].Rows.Count > 0)
                result = str_result.ToString();
            else
                result = "Incoming Mail Not Found.";

            return result;
        }

        public string anket_dashbord_duyurular()
        {
            string result = "";
            GenelRepository ankDB = RepositoryManager.GetRepository<GenelRepository>();
            DataSet ds_result = ankDB.NotificationDataSetDashBoard();

            StringBuilder str_result = new StringBuilder();
            int i = 1;
            foreach (DataRow dr in ds_result.Tables[0].Rows)
            {
                str_result.Append("<span class=\"failureNotification\"> * </span><a href=\"#\" onclick=\"DuyuruGoster('" + dr["notification_uid"].ToString() + "'); return false;\">" + dr["notification_basligi"].ToString() + "</a><br><br>");
                i++;
            }

            if (ds_result.Tables[0].Rows.Count > 0)
                result = str_result.ToString();
            else
                result = "Notification Not Found.";

            return result;
        }

        #endregion

        #region Survey Soruları
        public IQueryable<sbr_anket_sorulari> TumSurveySorulari()
        {
            return db.sbr_anket_sorulari;
        }
        public sbr_anket_sorulari SurveySoruGetir(Guid anket_soru_uid)
        {
            return db.sbr_anket_sorulari.SingleOrDefault(d => d.soru_uid == anket_soru_uid);
        }
        public void SurveySoruEkle(sbr_anket_sorulari anket_soru)
        {
            anket_soru.soru_uid = Guid.NewGuid();
            db.sbr_anket_sorulari.AddObject(anket_soru);
        }
        public void SurveySoruSil(sbr_anket_sorulari anket_soru)
        {
            db.sbr_anket_sorulari.DeleteObject(anket_soru);
        }

        public void SurveySoruSil(Guid anket_soru_uid)
        {
            this.SurveySoruSil(this.SurveySoruGetir(anket_soru_uid));
        }


        public DataSet SurveySoruGetirSurveyGoreDataSet(Guid anket_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select ROW_NUMBER() OVER(ORDER BY soru_sira_no) AS 'RowNumber',case when soru_zorunlu=1 then '*' else '' end zorunlu_soru,case when soru_zorunlu=1 then 'fldTitle require' else 'fldTitle' end zorunlu_class, * from ( select * from sbr_anket_soru_v where anket_uid='" + anket_uid + "' ) as table1 ORDER BY soru_sira_no");
            
            return ds_result;
        }

        public DataSet SurveySoruCevapDurumuDataSet(Guid anket_uid,string cevap_key)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select ROW_NUMBER() OVER(ORDER BY soru_sira_no) AS 'RowNumber',*,dbo.SoruCevaplandiMi(soru_uid,soru_tipi_id,'" + cevap_key + "') as cevap_durumu from ( select * from sbr_anket_soru_v where anket_uid='" + anket_uid + "') as table1");

            return ds_result;
        }


        public bool HepsiCevaplandimi(Guid anket_uid,string cevap_key)
        {
            bool result=false;

            string cevap_durumu = BaseDB.DBManager.AppConnection.ExecuteSql("select count(*) from (select ROW_NUMBER() OVER(ORDER BY soru_sira_no) AS 'RowNumber',*,dbo.SoruCevaplandiMi(soru_uid,soru_tipi_id,'" + cevap_key + "') as cevap_durumu from ( select * from sbr_anket_soru_v where anket_uid='" + anket_uid + "') as table1) as table2 where cevap_durumu=0 and soru_zorunlu=1");

            if (cevap_durumu != "" && cevap_durumu != "0")
                result = false;
            else
                result = true;

            return result;
        }

        public int CevaplananSoruSayısı(Guid anket_uid, string cevap_key)
        {

            string cevap_durumu = BaseDB.DBManager.AppConnection.ExecuteSql("select count(*) from (select ROW_NUMBER() OVER(ORDER BY soru_sira_no) AS 'RowNumber',*,dbo.SoruCevaplandiMiHepsi(soru_uid,soru_tipi_id,'" + cevap_key + "') as cevap_durumu from ( select * from sbr_anket_soru_v where anket_uid='" + anket_uid + "') as table1) as table2 where cevap_durumu>0");

            if (cevap_durumu == "")
                cevap_durumu = "0";
            
            return Convert.ToInt32(cevap_durumu);
        }

        public int TumSoruSayısı(Guid anket_uid, string cevap_key)
        {

            string tum_soru_sayisi = BaseDB.DBManager.AppConnection.ExecuteSql("select count(*) from sbr_anket_soru_v where anket_uid='" + anket_uid + "' ");

            if (tum_soru_sayisi == "")
                tum_soru_sayisi = "0";

            return Convert.ToInt32(tum_soru_sayisi);
        }

        public string SurveySoruMaxSiraNo(Guid anket_uid)
        {

            string max = BaseDB.DBManager.AppConnection.ExecuteSql("select max(soru_sira_no)+1 from sbr_anket_sorulari where anket_uid='" + anket_uid + "'");

            if (max == "")
                max = "1";

            return max;
        }

        public void SoruOlustur(Guid anket_uid, string soru_tipi, string soru, string soru_secenekleri, string cevap_kolonları, string soru_zorunlu,string soru_sayisal_ondalik,string tek_satir)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            if (soru_tipi == "1")
            {
                sbr_anket_sorulari anket_sorulari = new sbr_anket_sorulari();
                ankDB.SurveySoruEkle(anket_sorulari);
                anket_sorulari.anket_uid = anket_uid;
                anket_sorulari.soru = soru;
                anket_sorulari.soru_tipi_id = Convert.ToInt32(soru_tipi);
                //anket_sorulari.soru_sitili_id = Convert.ToInt32(soru_sitili);
                //anket_sorulari.soru_siralama_sekli_id = Convert.ToInt32(siralama_sekli);
                anket_sorulari.soru_olusturan_kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                anket_sorulari.soru_olusturma_tarihi = DateTime.Now;
                anket_sorulari.soru_sira_no = Convert.ToInt32(ankDB.SurveySoruMaxSiraNo(anket_uid));
                if (soru_zorunlu != null && (soru_zorunlu == "true" || soru_zorunlu == "True" || soru_zorunlu == "TRUE"))
                    anket_sorulari.soru_zorunlu = true;
                else
                    anket_sorulari.soru_zorunlu = false;
                ankDB.Kaydet();

                soru_secenekleri = soru_secenekleri.Replace("\r\n", "#");
                string[] soru_secenekleri_arr = soru_secenekleri.Split('#');

                foreach (string secenekler in soru_secenekleri_arr)
                {
                    if (secenekler.Trim() != "")
                    {
                        sbr_soru_tipi_1_secenekleri anket_secenekler = new sbr_soru_tipi_1_secenekleri();
                        ankDB.AcikAnketSoruTipi1SecenekEkle(anket_secenekler);
                        anket_secenekler.soru_secenek_ad = secenekler;
                        anket_secenekler.soru_uid = anket_sorulari.soru_uid;
                        anket_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi1SecenekMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }
                }
            }
            else if (soru_tipi == "2")
            {
                sbr_anket_sorulari anket_sorulari = new sbr_anket_sorulari();
                ankDB.SurveySoruEkle(anket_sorulari);
                anket_sorulari.anket_uid = anket_uid;
                anket_sorulari.soru = soru;
                anket_sorulari.soru_tipi_id = Convert.ToInt32(soru_tipi);
                //anket_sorulari.soru_siralama_sekli_id = Convert.ToInt32(siralama_sekli);
                anket_sorulari.soru_olusturan_kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                anket_sorulari.soru_olusturma_tarihi = DateTime.Now;
                anket_sorulari.soru_sira_no = Convert.ToInt32(ankDB.SurveySoruMaxSiraNo(anket_uid));
                if (soru_zorunlu != null && (soru_zorunlu == "true" || soru_zorunlu == "True" || soru_zorunlu == "TRUE"))
                    anket_sorulari.soru_zorunlu = true;
                else
                    anket_sorulari.soru_zorunlu = false;
               
                ankDB.Kaydet();

                soru_secenekleri = soru_secenekleri.Replace("\r\n", "#");
                string[] soru_secenekleri_arr = soru_secenekleri.Split('#');

                foreach (string secenekler in soru_secenekleri_arr)
                {
                    if (secenekler.Trim() != "")
                    {
                        sbr_soru_tipi_2_secenekleri anket_secenekler = new sbr_soru_tipi_2_secenekleri();
                        ankDB.AcikAnketSoruTipi2SecenekEkle(anket_secenekler);
                        anket_secenekler.soru_secenek_ad = secenekler;
                        anket_secenekler.soru_uid = anket_sorulari.soru_uid;
                        anket_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi2SecenekMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }
                }
            }
            else if (soru_tipi == "3")
            {
                sbr_anket_sorulari anket_sorulari = new sbr_anket_sorulari();
                ankDB.SurveySoruEkle(anket_sorulari);
                anket_sorulari.anket_uid = anket_uid;
                anket_sorulari.soru = soru;
                anket_sorulari.soru_tipi_id = Convert.ToInt32(soru_tipi);
                //anket_sorulari.soru_siralama_sekli_id = Convert.ToInt32(siralama_sekli);
                anket_sorulari.soru_olusturan_kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                anket_sorulari.soru_olusturma_tarihi = DateTime.Now;
                anket_sorulari.soru_sira_no = Convert.ToInt32(ankDB.SurveySoruMaxSiraNo(anket_uid));
                if (soru_zorunlu != null && (soru_zorunlu == "true" || soru_zorunlu == "True" || soru_zorunlu == "TRUE"))
                    anket_sorulari.soru_zorunlu = true;
                else
                    anket_sorulari.soru_zorunlu = false;
               
                ankDB.Kaydet();

                soru_secenekleri = soru_secenekleri.Replace("\r\n", "#");
                string[] soru_secenekleri_arr = soru_secenekleri.Split('#');

                foreach (string secenekler in soru_secenekleri_arr)
                {
                    if (secenekler.Trim() != "")
                    {
                        sbr_soru_tipi_3_secenekleri anket_secenekler = new sbr_soru_tipi_3_secenekleri();
                        ankDB.AcikAnketSoruTipi3SecenekEkle(anket_secenekler);
                        anket_secenekler.soru_secenek_ad = secenekler;
                        anket_secenekler.soru_uid = anket_sorulari.soru_uid;
                        anket_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi3SecenekMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }
                }

                cevap_kolonları = cevap_kolonları.Replace("\r\n", "#");
                string[] cevap_kolonları_arr = cevap_kolonları.Split('#');

                foreach (string kolonlar in cevap_kolonları_arr)
                {
                    if (kolonlar.Trim() != "")
                    {
                        sbr_soru_tipi_3_secenek_kolonlari anket_secenek_kolonlari = new sbr_soru_tipi_3_secenek_kolonlari();
                        ankDB.AcikAnketSoruTipi3SecenekKolonEkle(anket_secenek_kolonlari);
                        anket_secenek_kolonlari.soru_secenek_kolon_ad = kolonlar;
                        anket_secenek_kolonlari.soru_uid = anket_sorulari.soru_uid;
                        anket_secenek_kolonlari.soru_secenek_kolon_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi3SecenekKolonMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }
                }
            }
            else if (soru_tipi == "4")
            {
                sbr_anket_sorulari anket_sorulari = new sbr_anket_sorulari();
                ankDB.SurveySoruEkle(anket_sorulari);
                anket_sorulari.anket_uid = anket_uid;
                anket_sorulari.soru = soru;
                anket_sorulari.soru_tipi_id = Convert.ToInt32(soru_tipi);
                anket_sorulari.soru_olusturan_kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                anket_sorulari.soru_olusturma_tarihi = DateTime.Now;
                anket_sorulari.soru_sira_no = Convert.ToInt32(ankDB.SurveySoruMaxSiraNo(anket_uid));
                if (soru_zorunlu != null && (soru_zorunlu == "true" || soru_zorunlu == "True" || soru_zorunlu == "TRUE"))
                    anket_sorulari.soru_zorunlu = true;
                else
                    anket_sorulari.soru_zorunlu = false;

                if (tek_satir != null && (tek_satir == "true" || tek_satir == "True" || tek_satir == "TRUE"))
                    anket_sorulari.soru_tek_satir = true;
                else
                    anket_sorulari.soru_tek_satir = false;

                ankDB.Kaydet();

                sbr_soru_tipi_4_secenekleri anket_secenekler = new sbr_soru_tipi_4_secenekleri();
                ankDB.AcikAnketSoruTipi4SecenekEkle(anket_secenekler);
                anket_secenekler.soru_secenek_ad = soru;
                anket_secenekler.soru_uid = anket_sorulari.soru_uid;
                anket_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi4SecenekMaxSiraNo(anket_sorulari.soru_uid));
                ankDB.Kaydet();
            }
            else if (soru_tipi == "5")
            {
                sbr_anket_sorulari anket_sorulari = new sbr_anket_sorulari();
                ankDB.SurveySoruEkle(anket_sorulari);
                anket_sorulari.anket_uid = anket_uid;
                anket_sorulari.soru = soru;
                anket_sorulari.soru_tipi_id = Convert.ToInt32(soru_tipi);
                anket_sorulari.soru_olusturan_kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                anket_sorulari.soru_olusturma_tarihi = DateTime.Now;
                anket_sorulari.soru_sira_no = Convert.ToInt32(ankDB.SurveySoruMaxSiraNo(anket_uid));
                if (soru_zorunlu != null && (soru_zorunlu == "true" || soru_zorunlu == "True" || soru_zorunlu == "TRUE"))
                    anket_sorulari.soru_zorunlu = true;
                else
                    anket_sorulari.soru_zorunlu = false;

               
                ankDB.Kaydet();

                sbr_soru_tipi_5_secenekleri anket_secenekler = new sbr_soru_tipi_5_secenekleri();
                ankDB.AcikAnketSoruTipi5SecenekEkle(anket_secenekler);
                anket_secenekler.soru_secenek_ad = "Doğru";
                anket_secenekler.soru_uid = anket_sorulari.soru_uid;
                anket_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi5SecenekMaxSiraNo(anket_sorulari.soru_uid));
                ankDB.Kaydet();

                sbr_soru_tipi_5_secenekleri anket_secenekler2 = new sbr_soru_tipi_5_secenekleri();
                ankDB.AcikAnketSoruTipi5SecenekEkle(anket_secenekler2);
                anket_secenekler2.soru_secenek_ad = "Yanlış";
                anket_secenekler2.soru_uid = anket_sorulari.soru_uid;
                anket_secenekler2.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi5SecenekMaxSiraNo(anket_sorulari.soru_uid));
                ankDB.Kaydet();
            }
            else if (soru_tipi == "6")
            {
                sbr_anket_sorulari anket_sorulari = new sbr_anket_sorulari();
                ankDB.SurveySoruEkle(anket_sorulari);
                anket_sorulari.anket_uid = anket_uid;
                anket_sorulari.soru = soru;
                anket_sorulari.soru_tipi_id = Convert.ToInt32(soru_tipi);
                anket_sorulari.soru_olusturan_kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                anket_sorulari.soru_olusturma_tarihi = DateTime.Now;
                anket_sorulari.soru_sira_no = Convert.ToInt32(ankDB.SurveySoruMaxSiraNo(anket_uid));
                if (soru_zorunlu != null && (soru_zorunlu == "true" || soru_zorunlu == "True" || soru_zorunlu == "TRUE"))
                    anket_sorulari.soru_zorunlu = true;
                else
                    anket_sorulari.soru_zorunlu = false;

                
                ankDB.Kaydet();

                sbr_soru_tipi_6_secenekleri anket_secenekler = new sbr_soru_tipi_6_secenekleri();
                ankDB.AcikAnketSoruTipi6SecenekEkle(anket_secenekler);
                anket_secenekler.soru_secenek_ad = "Evet";
                anket_secenekler.soru_uid = anket_sorulari.soru_uid;
                anket_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi6SecenekMaxSiraNo(anket_sorulari.soru_uid));
                ankDB.Kaydet();

                sbr_soru_tipi_6_secenekleri anket_secenekler2 = new sbr_soru_tipi_6_secenekleri();
                ankDB.AcikAnketSoruTipi6SecenekEkle(anket_secenekler2);
                anket_secenekler2.soru_secenek_ad = "Hayır";
                anket_secenekler2.soru_uid = anket_sorulari.soru_uid;
                anket_secenekler2.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi6SecenekMaxSiraNo(anket_sorulari.soru_uid));
                ankDB.Kaydet();
            }
            else if (soru_tipi == "7")
            {
                sbr_anket_sorulari anket_sorulari = new sbr_anket_sorulari();
                ankDB.SurveySoruEkle(anket_sorulari);
                anket_sorulari.anket_uid = anket_uid;
                anket_sorulari.soru = soru;
                anket_sorulari.soru_tipi_id = Convert.ToInt32(soru_tipi);
               // anket_sorulari.soru_siralama_sekli_id = Convert.ToInt32(siralama_sekli);
                anket_sorulari.soru_olusturan_kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                anket_sorulari.soru_olusturma_tarihi = DateTime.Now;
                anket_sorulari.soru_sira_no = Convert.ToInt32(ankDB.SurveySoruMaxSiraNo(anket_uid));
                if (soru_zorunlu != null && (soru_zorunlu == "true" || soru_zorunlu == "True" || soru_zorunlu == "TRUE"))
                    anket_sorulari.soru_zorunlu = true;
                else
                    anket_sorulari.soru_zorunlu = false;

              

                ankDB.Kaydet();

                soru_secenekleri = soru_secenekleri.Replace("\r\n", "#");
                string[] soru_secenekleri_arr = soru_secenekleri.Split('#');

                foreach (string secenekler in soru_secenekleri_arr)
                {
                    if (secenekler.Trim() != "")
                    {
                        sbr_soru_tipi_7_secenekleri anket_secenekler = new sbr_soru_tipi_7_secenekleri();
                        ankDB.AcikAnketSoruTipi7SecenekEkle(anket_secenekler);
                        anket_secenekler.soru_secenek_ad = secenekler;
                        anket_secenekler.soru_uid = anket_sorulari.soru_uid;
                        anket_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi7SecenekMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }
                }
            }
            else if (soru_tipi == "8")
            {
                sbr_anket_sorulari anket_sorulari = new sbr_anket_sorulari();
                ankDB.SurveySoruEkle(anket_sorulari);
                anket_sorulari.anket_uid = anket_uid;
                anket_sorulari.soru = soru;
                anket_sorulari.soru_tipi_id = Convert.ToInt32(soru_tipi);
                anket_sorulari.soru_olusturan_kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                anket_sorulari.soru_olusturma_tarihi = DateTime.Now;
                anket_sorulari.soru_sira_no = Convert.ToInt32(ankDB.SurveySoruMaxSiraNo(anket_uid));
                if (soru_zorunlu != null && (soru_zorunlu == "true" || soru_zorunlu == "True" || soru_zorunlu == "TRUE"))
                    anket_sorulari.soru_zorunlu = true;
                else
                    anket_sorulari.soru_zorunlu = false;

                if (soru_sayisal_ondalik != null && (soru_sayisal_ondalik == "true" || soru_sayisal_ondalik == "True" || soru_sayisal_ondalik == "TRUE"))
                    anket_sorulari.soru_sayisal_ondalik = true;
                else
                    anket_sorulari.soru_sayisal_ondalik = false;

                

                ankDB.Kaydet();

                sbr_soru_tipi_8_secenekleri anket_secenekler = new sbr_soru_tipi_8_secenekleri();
                ankDB.AcikAnketSoruTipi8SecenekEkle(anket_secenekler);
                anket_secenekler.soru_secenek_ad = soru;
                anket_secenekler.soru_uid = anket_sorulari.soru_uid;
                anket_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi8SecenekMaxSiraNo(anket_sorulari.soru_uid));
                ankDB.Kaydet();
            }
            else if (soru_tipi == "9")
            {
                sbr_anket_sorulari anket_sorulari = new sbr_anket_sorulari();
                ankDB.SurveySoruEkle(anket_sorulari);
                anket_sorulari.anket_uid = anket_uid;
                anket_sorulari.soru = soru;
                anket_sorulari.soru_tipi_id = Convert.ToInt32(soru_tipi);
                anket_sorulari.soru_olusturan_kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                anket_sorulari.soru_olusturma_tarihi = DateTime.Now;
                anket_sorulari.soru_sira_no = Convert.ToInt32(ankDB.SurveySoruMaxSiraNo(anket_uid));
                if (soru_zorunlu != null && (soru_zorunlu == "true" || soru_zorunlu == "True" || soru_zorunlu == "TRUE"))
                    anket_sorulari.soru_zorunlu = true;
                else
                    anket_sorulari.soru_zorunlu = false;

              
                ankDB.Kaydet();

                sbr_soru_tipi_9_secenekleri anket_secenekler = new sbr_soru_tipi_9_secenekleri();
                ankDB.AcikAnketSoruTipi9SecenekEkle(anket_secenekler);
                anket_secenekler.soru_secenek_ad = soru;
                anket_secenekler.soru_uid = anket_sorulari.soru_uid;
                anket_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi9SecenekMaxSiraNo(anket_sorulari.soru_uid));
                ankDB.Kaydet();
            }
            else if (soru_tipi == "10")
            {
                sbr_anket_sorulari anket_sorulari = new sbr_anket_sorulari();
                ankDB.SurveySoruEkle(anket_sorulari);
                anket_sorulari.anket_uid = anket_uid;
                anket_sorulari.soru = soru;
                anket_sorulari.soru_tipi_id = Convert.ToInt32(soru_tipi);
                anket_sorulari.soru_olusturan_kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                anket_sorulari.soru_olusturma_tarihi = DateTime.Now;
                anket_sorulari.soru_sira_no = Convert.ToInt32(ankDB.SurveySoruMaxSiraNo(anket_uid));
                if (soru_zorunlu != null && (soru_zorunlu == "true" || soru_zorunlu == "True" || soru_zorunlu == "TRUE"))
                    anket_sorulari.soru_zorunlu = true;
                else
                    anket_sorulari.soru_zorunlu = false;


                ankDB.Kaydet();

                sbr_soru_tipi_10_secenekleri anket_secenekler = new sbr_soru_tipi_10_secenekleri();
                ankDB.AcikAnketSoruTipi10SecenekEkle(anket_secenekler);
                anket_secenekler.soru_secenek_ad = soru;
                anket_secenekler.soru_uid = anket_sorulari.soru_uid;
                anket_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi10SecenekMaxSiraNo(anket_sorulari.soru_uid));
                ankDB.Kaydet();
            }
            else if (soru_tipi == "11")
            {
                sbr_anket_sorulari anket_sorulari = new sbr_anket_sorulari();
                ankDB.SurveySoruEkle(anket_sorulari);
                anket_sorulari.anket_uid = anket_uid;
                anket_sorulari.soru = soru;
                anket_sorulari.soru_tipi_id = Convert.ToInt32(soru_tipi);
                anket_sorulari.soru_olusturan_kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                anket_sorulari.soru_olusturma_tarihi = DateTime.Now;
                anket_sorulari.soru_sira_no = Convert.ToInt32(ankDB.SurveySoruMaxSiraNo(anket_uid));
                if (soru_zorunlu != null && (soru_zorunlu == "true" || soru_zorunlu == "True" || soru_zorunlu == "TRUE"))
                    anket_sorulari.soru_zorunlu = true;
                else
                    anket_sorulari.soru_zorunlu = false;

          

                ankDB.Kaydet();

                sbr_soru_tipi_11_secenekleri anket_secenekler = new sbr_soru_tipi_11_secenekleri();
                ankDB.AcikAnketSoruTipi11SecenekEkle(anket_secenekler);
                anket_secenekler.soru_secenek_ad = soru;
                anket_secenekler.soru_uid = anket_sorulari.soru_uid;
                anket_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi11SecenekMaxSiraNo(anket_sorulari.soru_uid));
                ankDB.Kaydet();
            }
        }

        public void SoruUpdate(Guid soru_uid, string soru_tipi, string soru, string soru_secenekleri, string cevap_kolonları, string soru_zorunlu, string soru_sayisal_ondalik,string tek_satir)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            if (soru_tipi == "1")
            {
                sbr_anket_sorulari anket_sorulari = this.SurveySoruGetir(soru_uid);
                anket_sorulari.soru = soru;
                
                if (soru_zorunlu != null && (soru_zorunlu == "true" || soru_zorunlu == "True" || soru_zorunlu == "TRUE"))
                    anket_sorulari.soru_zorunlu = true;
                else
                    anket_sorulari.soru_zorunlu = false;

                ankDB.Kaydet();

                soru_secenekleri = soru_secenekleri.Replace("\r\n", "#");
                string[] soru_secenekleri_arr = soru_secenekleri.Split('#');
                this.AcikAnketSoruTipi1SecenekSilSoruyaGore(soru_uid);

                foreach (string secenekler in soru_secenekleri_arr)
                {
                    if (secenekler.Trim() != "")
                    {
                        sbr_soru_tipi_1_secenekleri anket_secenekler = new sbr_soru_tipi_1_secenekleri();
                        ankDB.AcikAnketSoruTipi1SecenekEkle(anket_secenekler);
                        anket_secenekler.soru_secenek_ad = secenekler;
                        anket_secenekler.soru_uid = anket_sorulari.soru_uid;
                        anket_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi1SecenekMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }
                }
            }
            else if (soru_tipi == "2")
            {
                sbr_anket_sorulari anket_sorulari = this.SurveySoruGetir(soru_uid);
                anket_sorulari.soru = soru;
                if (soru_zorunlu != null && (soru_zorunlu == "true" || soru_zorunlu == "True" || soru_zorunlu == "TRUE"))
                    anket_sorulari.soru_zorunlu = true;
                else
                    anket_sorulari.soru_zorunlu = false;

                ankDB.Kaydet();

                this.AcikAnketSoruTipi2SecenekSilSoruyaGore(soru_uid);
                soru_secenekleri = soru_secenekleri.Replace("\r\n", "#");
                string[] soru_secenekleri_arr = soru_secenekleri.Split('#');

                foreach (string secenekler in soru_secenekleri_arr)
                {
                    if (secenekler.Trim() != "")
                    {
                        sbr_soru_tipi_2_secenekleri anket_secenekler = new sbr_soru_tipi_2_secenekleri();
                        ankDB.AcikAnketSoruTipi2SecenekEkle(anket_secenekler);
                        anket_secenekler.soru_secenek_ad = secenekler;
                        anket_secenekler.soru_uid = anket_sorulari.soru_uid;
                        anket_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi2SecenekMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }
                }
            }
            else if (soru_tipi == "3")
            {
                sbr_anket_sorulari anket_sorulari = this.SurveySoruGetir(soru_uid);
                anket_sorulari.soru = soru;
                if (soru_zorunlu != null && (soru_zorunlu == "true" || soru_zorunlu == "True" || soru_zorunlu == "TRUE"))
                    anket_sorulari.soru_zorunlu = true;
                else
                    anket_sorulari.soru_zorunlu = false;

                ankDB.Kaydet();


                soru_secenekleri = soru_secenekleri.Replace("\r\n", "#");
                string[] soru_secenekleri_arr = soru_secenekleri.Split('#');
                
                this.AcikAnketSoruTipi3SecenekSilSoruyaGore(soru_uid);

                foreach (string secenekler in soru_secenekleri_arr)
                {
                    if (secenekler.Trim() != "")
                    {
                        sbr_soru_tipi_3_secenekleri anket_secenekler = new sbr_soru_tipi_3_secenekleri();
                        ankDB.AcikAnketSoruTipi3SecenekEkle(anket_secenekler);
                        anket_secenekler.soru_secenek_ad = secenekler;
                        anket_secenekler.soru_uid = anket_sorulari.soru_uid;
                        anket_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi3SecenekMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }
                }

                cevap_kolonları = cevap_kolonları.Replace("\r\n", "#");
                string[] cevap_kolonları_arr = cevap_kolonları.Split('#');

                this.AcikAnketSoruTipi3SecenekKolonSilSoruyaGore(soru_uid);

                foreach (string kolonlar in cevap_kolonları_arr)
                {
                    if (kolonlar.Trim() != "")
                    {
                        sbr_soru_tipi_3_secenek_kolonlari anket_secenek_kolonlari = new sbr_soru_tipi_3_secenek_kolonlari();
                        ankDB.AcikAnketSoruTipi3SecenekKolonEkle(anket_secenek_kolonlari);
                        anket_secenek_kolonlari.soru_secenek_kolon_ad = kolonlar;
                        anket_secenek_kolonlari.soru_uid = anket_sorulari.soru_uid;
                        anket_secenek_kolonlari.soru_secenek_kolon_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi3SecenekKolonMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }
                }
            }
            else if (soru_tipi == "4")
            {
                sbr_anket_sorulari anket_sorulari = this.SurveySoruGetir(soru_uid);
                anket_sorulari.soru = soru;
                if (soru_zorunlu != null && (soru_zorunlu == "true" || soru_zorunlu == "True" || soru_zorunlu == "TRUE"))
                    anket_sorulari.soru_zorunlu = true;
                else
                    anket_sorulari.soru_zorunlu = false;

                if (tek_satir != null && (tek_satir == "true" || tek_satir == "True" || tek_satir == "TRUE"))
                    anket_sorulari.soru_tek_satir = true;
                else
                    anket_sorulari.soru_tek_satir = false;

                ankDB.Kaydet();

                this.AcikAnketSoruTipi4SecenekSilSoruyaGore(soru_uid);

                sbr_soru_tipi_4_secenekleri anket_secenekler = new sbr_soru_tipi_4_secenekleri();
                ankDB.AcikAnketSoruTipi4SecenekEkle(anket_secenekler);
                anket_secenekler.soru_secenek_ad = soru;
                anket_secenekler.soru_uid = anket_sorulari.soru_uid;
                anket_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi4SecenekMaxSiraNo(anket_sorulari.soru_uid));
                ankDB.Kaydet();
            }
            else if (soru_tipi == "5")
            {
                sbr_anket_sorulari anket_sorulari = this.SurveySoruGetir(soru_uid);
                anket_sorulari.soru = soru;
                if (soru_zorunlu != null && (soru_zorunlu == "true" || soru_zorunlu == "True" || soru_zorunlu == "TRUE"))
                    anket_sorulari.soru_zorunlu = true;
                else
                    anket_sorulari.soru_zorunlu = false;

                ankDB.Kaydet();

                this.AcikAnketSoruTipi5SecenekSilSoruyaGore(soru_uid);

                sbr_soru_tipi_5_secenekleri anket_secenekler = new sbr_soru_tipi_5_secenekleri();
                ankDB.AcikAnketSoruTipi5SecenekEkle(anket_secenekler);
                anket_secenekler.soru_secenek_ad = "Doğru";
                anket_secenekler.soru_uid = anket_sorulari.soru_uid;
                anket_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi5SecenekMaxSiraNo(anket_sorulari.soru_uid));
                ankDB.Kaydet();

                sbr_soru_tipi_5_secenekleri anket_secenekler2 = new sbr_soru_tipi_5_secenekleri();
                ankDB.AcikAnketSoruTipi5SecenekEkle(anket_secenekler2);
                anket_secenekler2.soru_secenek_ad = "Yanlış";
                anket_secenekler2.soru_uid = anket_sorulari.soru_uid;
                anket_secenekler2.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi5SecenekMaxSiraNo(anket_sorulari.soru_uid));
                ankDB.Kaydet();
            }
            else if (soru_tipi == "6")
            {
                sbr_anket_sorulari anket_sorulari = this.SurveySoruGetir(soru_uid);
                anket_sorulari.soru = soru;
                if (soru_zorunlu != null && (soru_zorunlu == "true" || soru_zorunlu == "True" || soru_zorunlu == "TRUE"))
                    anket_sorulari.soru_zorunlu = true;
                else
                    anket_sorulari.soru_zorunlu = false;

                ankDB.Kaydet();

                this.AcikAnketSoruTipi6SecenekSilSoruyaGore(soru_uid);

                sbr_soru_tipi_6_secenekleri anket_secenekler = new sbr_soru_tipi_6_secenekleri();
                ankDB.AcikAnketSoruTipi6SecenekEkle(anket_secenekler);
                anket_secenekler.soru_secenek_ad = "Evet";
                anket_secenekler.soru_uid = anket_sorulari.soru_uid;
                anket_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi6SecenekMaxSiraNo(anket_sorulari.soru_uid));
                ankDB.Kaydet();

                sbr_soru_tipi_6_secenekleri anket_secenekler2 = new sbr_soru_tipi_6_secenekleri();
                ankDB.AcikAnketSoruTipi6SecenekEkle(anket_secenekler2);
                anket_secenekler2.soru_secenek_ad = "Hayır";
                anket_secenekler2.soru_uid = anket_sorulari.soru_uid;
                anket_secenekler2.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi6SecenekMaxSiraNo(anket_sorulari.soru_uid));
                ankDB.Kaydet();
            }
            else if (soru_tipi == "7")
            {
                sbr_anket_sorulari anket_sorulari = this.SurveySoruGetir(soru_uid);
                anket_sorulari.soru = soru;
                if (soru_zorunlu != null && (soru_zorunlu == "true" || soru_zorunlu == "True" || soru_zorunlu == "TRUE"))
                    anket_sorulari.soru_zorunlu = true;
                else
                    anket_sorulari.soru_zorunlu = false;

                ankDB.Kaydet();

                this.AcikAnketSoruTipi7SecenekSilSoruyaGore(soru_uid);
                soru_secenekleri = soru_secenekleri.Replace("\r\n", "#");
                string[] soru_secenekleri_arr = soru_secenekleri.Split('#');

                foreach (string secenekler in soru_secenekleri_arr)
                {
                    if (secenekler.Trim() != "")
                    {
                        sbr_soru_tipi_7_secenekleri anket_secenekler = new sbr_soru_tipi_7_secenekleri();
                        ankDB.AcikAnketSoruTipi7SecenekEkle(anket_secenekler);
                        anket_secenekler.soru_secenek_ad = secenekler;
                        anket_secenekler.soru_uid = anket_sorulari.soru_uid;
                        anket_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi7SecenekMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }
                }
            }
            else if (soru_tipi == "8")
            {
                sbr_anket_sorulari anket_sorulari = this.SurveySoruGetir(soru_uid);
                anket_sorulari.soru = soru;
                if (soru_zorunlu != null && (soru_zorunlu == "true" || soru_zorunlu == "True" || soru_zorunlu == "TRUE"))
                    anket_sorulari.soru_zorunlu = true;
                else
                    anket_sorulari.soru_zorunlu = false;

                if (soru_sayisal_ondalik != null && (soru_sayisal_ondalik == "true" || soru_sayisal_ondalik == "True" || soru_sayisal_ondalik == "TRUE"))
                    anket_sorulari.soru_sayisal_ondalik = true;
                else
                    anket_sorulari.soru_sayisal_ondalik = false;

                ankDB.Kaydet();

                this.AcikAnketSoruTipi8SecenekSilSoruyaGore(soru_uid);

                sbr_soru_tipi_8_secenekleri anket_secenekler = new sbr_soru_tipi_8_secenekleri();
                ankDB.AcikAnketSoruTipi8SecenekEkle(anket_secenekler);
                anket_secenekler.soru_secenek_ad = soru;
                anket_secenekler.soru_uid = anket_sorulari.soru_uid;
                anket_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi8SecenekMaxSiraNo(anket_sorulari.soru_uid));
                ankDB.Kaydet();
            }
            else if (soru_tipi == "9")
            {
                sbr_anket_sorulari anket_sorulari = this.SurveySoruGetir(soru_uid);
                anket_sorulari.soru = soru;
                if (soru_zorunlu != null && (soru_zorunlu == "true" || soru_zorunlu == "True" || soru_zorunlu == "TRUE"))
                    anket_sorulari.soru_zorunlu = true;
                else
                    anket_sorulari.soru_zorunlu = false;

                ankDB.Kaydet();

                this.AcikAnketSoruTipi9SecenekSilSoruyaGore(soru_uid);

                sbr_soru_tipi_9_secenekleri anket_secenekler = new sbr_soru_tipi_9_secenekleri();
                ankDB.AcikAnketSoruTipi9SecenekEkle(anket_secenekler);
                anket_secenekler.soru_secenek_ad = soru;
                anket_secenekler.soru_uid = anket_sorulari.soru_uid;
                anket_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi9SecenekMaxSiraNo(anket_sorulari.soru_uid));
                ankDB.Kaydet();
            }
            else if (soru_tipi == "10")
            {
                sbr_anket_sorulari anket_sorulari = this.SurveySoruGetir(soru_uid);
                anket_sorulari.soru = soru;
                if (soru_zorunlu != null && (soru_zorunlu == "true" || soru_zorunlu == "True" || soru_zorunlu == "TRUE"))
                    anket_sorulari.soru_zorunlu = true;
                else
                    anket_sorulari.soru_zorunlu = false;

                ankDB.Kaydet();

                this.AcikAnketSoruTipi10SecenekSilSoruyaGore(soru_uid);

                sbr_soru_tipi_10_secenekleri anket_secenekler = new sbr_soru_tipi_10_secenekleri();
                ankDB.AcikAnketSoruTipi10SecenekEkle(anket_secenekler);
                anket_secenekler.soru_secenek_ad = soru;
                anket_secenekler.soru_uid = anket_sorulari.soru_uid;
                anket_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi10SecenekMaxSiraNo(anket_sorulari.soru_uid));
                ankDB.Kaydet();
            }
            else if (soru_tipi == "11")
            {
                sbr_anket_sorulari anket_sorulari = this.SurveySoruGetir(soru_uid);
                anket_sorulari.soru = soru;
                if (soru_zorunlu != null && (soru_zorunlu == "true" || soru_zorunlu == "True" || soru_zorunlu == "TRUE"))
                    anket_sorulari.soru_zorunlu = true;
                else
                    anket_sorulari.soru_zorunlu = false;

                ankDB.Kaydet();

                this.AcikAnketSoruTipi11SecenekSilSoruyaGore(soru_uid);

                sbr_soru_tipi_11_secenekleri anket_secenekler = new sbr_soru_tipi_11_secenekleri();
                ankDB.AcikAnketSoruTipi11SecenekEkle(anket_secenekler);
                anket_secenekler.soru_secenek_ad = soru;
                anket_secenekler.soru_uid = anket_sorulari.soru_uid;
                anket_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi11SecenekMaxSiraNo(anket_sorulari.soru_uid));
                ankDB.Kaydet();
            }
        }

        public string SoruGorunumuOlustur(object soru_uid)
        {
            string result = "";
            Guid soru_id = Guid.Parse(soru_uid.ToString());

            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            sbr_anket_sorulari anket_soru = ankDB.SurveySoruGetir(soru_id);


            if (anket_soru.soru_tipi_id == 1)
            {
                DataSet ds_soru_tipi1_secenekler = ankDB.AcikAnketSoruTipi1SecenekGetirSoruyaGoreDataSet(soru_id,Guid.Empty);
                int index = 0;
                result = "<table>";
                foreach (DataRow dr in ds_soru_tipi1_secenekler.Tables[0].Rows)
                {

                    result += "<tr><td><label class=\"withRadio\"><input id=\"secenek_tip1_" + dr["soru_secenek_uid"].ToString() + "\" type=\"radio\" name=\"grup_" + dr["soru_uid"].ToString() + "\" runat=\"server\" />" + dr["soru_secenek_ad"].ToString() + "</label></td></tr>";
                    index++;
                }
                result += "</table>";

                if (ds_soru_tipi1_secenekler.Tables[0].Rows.Count <= 0) result = "";
            }
            else if (anket_soru.soru_tipi_id == 2)
            {
                DataSet ds_soru_tipi2_secenekler = ankDB.AcikAnketSoruTipi2SecenekGetirSoruyaGoreDataSet(soru_id);
                int index = 0;
                result = "<table>";
                foreach (DataRow dr in ds_soru_tipi2_secenekler.Tables[0].Rows)
                {
                    result += "<tr><td><label class=\"withCheckbox\"><input id=\"secenek_tip2_" + dr["soru_secenek_uid"].ToString() + "\" type=\"checkbox\" runat=\"server\" />" + dr["soru_secenek_ad"].ToString() + "</label></td></tr>";
                    index++;
                }
                result += "</table>";

                if (ds_soru_tipi2_secenekler.Tables[0].Rows.Count <= 0) result = "";
            }
            else if (anket_soru.soru_tipi_id == 3)
            {
                DataSet ds_soru_tipi3_secenekler = ankDB.AcikAnketSoruTipi3SecenekGetirSoruyaGoreDataSet(soru_id);
                DataSet ds_soru_tipi3_kolonlar = ankDB.AcikAnketSoruTipi3SecenekKolonGetirSoruyaGoreDataSet(soru_id);
                int index = 0;
                result = "<table>";
                foreach (DataRow dr in ds_soru_tipi3_secenekler.Tables[0].Rows)
                {
                    result += "<tr><td width=\"250px\">" + dr["soru_secenek_ad"].ToString() + "</td>";
                    string kolonlar = "";
                    foreach (DataRow dr2 in ds_soru_tipi3_kolonlar.Tables[0].Rows)
                    {
                        kolonlar += "<td>" + "<label class=\"withRadio\"><input id=\"secenek_tip3_" + dr["soru_secenek_uid"].ToString() + "_" + dr2["soru_secenek_kolon_uid"].ToString() + "\" type=\"radio\" name=\"grup_" + dr["soru_secenek_uid"].ToString() + "\" runat=\"server\" />" + dr2["soru_secenek_kolon_ad"].ToString() + "</td><td width=\"15px\"></td>";
                    }
                    result += kolonlar + "</tr>";

                    index++;
                }
                result += "</table>";

            }
            else if (anket_soru.soru_tipi_id == 4)
            {
                DataSet ds_soru_tipi4_secenekler = ankDB.AcikAnketSoruTipi4SecenekGetirSoruyaGoreDataSet(soru_id);
                int index = 0;

                if (anket_soru.soru_tek_satir != null && anket_soru.soru_tek_satir == true)
                {
                    result = "<table>";
                    foreach (DataRow dr in ds_soru_tipi4_secenekler.Tables[0].Rows)
                    {
                        result += "<tr><td><input id=\"secenek_tip4_" + dr["soru_secenek_uid"].ToString() + "\"  runat=\"server\"  type=\"text\" /></td></tr>";
                        index++;
                    }
                    result += "</table>";

                    if (ds_soru_tipi4_secenekler.Tables[0].Rows.Count <= 0) result = "";
                }
                else
                {
                    result = "<table>";
                    foreach (DataRow dr in ds_soru_tipi4_secenekler.Tables[0].Rows)
                    {
                        result += "<tr><td><textarea id=\"secenek_tip4_" + dr["soru_secenek_uid"].ToString() + "\" rows=\"3\" runat=\"server\"  cols=\"80\"></textarea></td></tr>";
                        index++;
                    }
                    result += "</table>";

                    if (ds_soru_tipi4_secenekler.Tables[0].Rows.Count <= 0) result = "";
                }
            }
            else if (anket_soru.soru_tipi_id == 7)
            {
                DataSet ds_soru_tipi7_secenekler = ankDB.AcikAnketSoruTipi7SecenekGetirSoruyaGoreDataSet(soru_id);
                int index = 0;

                result = "<table>";

                foreach (DataRow dr in ds_soru_tipi7_secenekler.Tables[0].Rows)
                {
                    result += "<tr><td><label>" + dr["soru_secenek_ad"].ToString() + "</label></td><td width=\"30px\"> : </td><td><input type=\"text\" id=\"secenek_tip7_" + dr["soru_secenek_uid"].ToString() + "\" runat=\"server\"  /></td></tr>";
                    index++;
                }

                result += "</table>";
                if (ds_soru_tipi7_secenekler.Tables[0].Rows.Count <= 0) result = "";
            }
            else if (anket_soru.soru_tipi_id == 5)
            {
                DataSet ds_soru_tipi5_secenekler = ankDB.AcikAnketSoruTipi5SecenekGetirSoruyaGoreDataSet(soru_id);
                int index = 0;
                result = "<table>";
                foreach (DataRow dr in ds_soru_tipi5_secenekler.Tables[0].Rows)
                {
                    result += "<tr><td><label class=\"withRadio\"><input id=\"secenek_tip5_" + dr["soru_secenek_uid"].ToString() + "\" type=\"radio\" name=\"grup_" + dr["soru_uid"].ToString() + "\" runat=\"server\" />" + dr["soru_secenek_ad"].ToString() + "</label></td></tr>";
                    index++;
                }
                result += "</table>";

                if (ds_soru_tipi5_secenekler.Tables[0].Rows.Count <= 0) result = "";
            }
            else if (anket_soru.soru_tipi_id == 6)
            {
                DataSet ds_soru_tipi6_secenekler = ankDB.AcikAnketSoruTipi6SecenekGetirSoruyaGoreDataSet(soru_id);
                int index = 0;
                result = "<table>";
                foreach (DataRow dr in ds_soru_tipi6_secenekler.Tables[0].Rows)
                {
                    result += "<tr><td><label class=\"withRadio\"><input id=\"secenek_tip6_" + dr["soru_secenek_uid"].ToString() + "\" type=\"radio\" name=\"grup_" + dr["soru_uid"].ToString() + "\" runat=\"server\" />" + dr["soru_secenek_ad"].ToString() + "</label></td></tr>";
                    index++;
                }
                result += "</table>";

                if (ds_soru_tipi6_secenekler.Tables[0].Rows.Count <= 0) result = "";
            }
            else if (anket_soru.soru_tipi_id == 8)
            {
                DataSet ds_soru_tipi8_secenekler = ankDB.AcikAnketSoruTipi8SecenekGetirSoruyaGoreDataSet(soru_id);
                int index = 0;
                result = "<table>";
                foreach (DataRow dr in ds_soru_tipi8_secenekler.Tables[0].Rows)
                {
                    string class_tip = "";
                    if (anket_soru.soru_sayisal_ondalik != null && anket_soru.soru_sayisal_ondalik == true)
                        class_tip = "numeric";
                    else
                        class_tip = "integer";

                    result += "<tr><td><input class=\"" + class_tip + "\" type=\"text\"  id=\"secenek_tip8_" + dr["soru_secenek_uid"].ToString() + "\"  runat=\"server\" style=\"text-align: right\"   /></td></tr>";

                    index++;
                }

                result += "</table>";

                if (ds_soru_tipi8_secenekler.Tables[0].Rows.Count <= 0) result = "";
            }
            else if (anket_soru.soru_tipi_id == 9)
            {
                DataSet ds_soru_tipi9_secenekler = ankDB.AcikAnketSoruTipi9SecenekGetirSoruyaGoreDataSet(soru_id);
                int index = 0;
                result = "<table>";
                foreach (DataRow dr in ds_soru_tipi9_secenekler.Tables[0].Rows)
                {
                    string class_tip = "datepicker";
                    result += "<tr><td><input class=\"" + class_tip + "\" type=\"text\"  id=\"secenek_tip9_" + dr["soru_secenek_uid"].ToString() + "\"  runat=\"server\"  readonly=\"readonly\" /></td></tr>";
                    index++;
                }
                result += "</table>";

                if (ds_soru_tipi9_secenekler.Tables[0].Rows.Count <= 0) result = "";
            }
            else if (anket_soru.soru_tipi_id == 10)
            {
                DataSet ds_soru_tipi10_secenekler = ankDB.AcikAnketSoruTipi10SecenekGetirSoruyaGoreDataSet(soru_id);
                int index = 0;
                result = "<table>";
                foreach (DataRow dr in ds_soru_tipi10_secenekler.Tables[0].Rows)
                {
                    string class_tip = "phone";
                    result += "<tr><td><input class=\"" + class_tip + "\" type=\"text\"  id=\"secenek_tip10_" + dr["soru_secenek_uid"].ToString() + "\"  runat=\"server\"  /></td></tr>";

                    index++;
                }

                result += "</table>";

                if (ds_soru_tipi10_secenekler.Tables[0].Rows.Count <= 0) result = "";
            }
            else if (anket_soru.soru_tipi_id == 11)
            {
                DataSet ds_soru_tipi11_secenekler = ankDB.AcikAnketSoruTipi11SecenekGetirSoruyaGoreDataSet(soru_id);
                int index = 0;
                result = "<table>";
                foreach (DataRow dr in ds_soru_tipi11_secenekler.Tables[0].Rows)
                {
                    string class_tip = "eposta";
                    result += "<tr><td><input class=\"" + class_tip + "\" type=\"text\"  id=\"secenek_tip11_" + dr["soru_secenek_uid"].ToString() + "\"  runat=\"server\"  /></td></tr>";

                    index++;
                }

                result += "</table>";

                if (ds_soru_tipi11_secenekler.Tables[0].Rows.Count <= 0) result = "";
            }
            return result;
        }
        #endregion

        #region Survey Soru Tipi 1 Optionsi
        public IQueryable<sbr_soru_tipi_1_secenekleri> TumAcikAnketSoruTipi1Secenekleri()
        {
            return db.sbr_soru_tipi_1_secenekleri;
        }
        public sbr_soru_tipi_1_secenekleri AcikAnketSoruTipi1SecenekGetir(Guid soru_secenek_uid)
        {
            return db.sbr_soru_tipi_1_secenekleri.SingleOrDefault(d => d.soru_secenek_uid == soru_secenek_uid);
        }
        public void AcikAnketSoruTipi1SecenekEkle(sbr_soru_tipi_1_secenekleri anket_soru_tipi1_secenek)
        {
            anket_soru_tipi1_secenek.soru_secenek_uid = Guid.NewGuid();
            db.sbr_soru_tipi_1_secenekleri.AddObject(anket_soru_tipi1_secenek);
        }
        public void AcikAnketSoruTipi1SecenekSil(sbr_soru_tipi_1_secenekleri anket_soru_tipi1_secenekleri)
        {
            db.sbr_soru_tipi_1_secenekleri.DeleteObject(anket_soru_tipi1_secenekleri);
        }

        public void AcikAnketSoruTipi1SecenekSil(Guid anket_soru_tipi1_secenek_uid)
        {
            this.AcikAnketSoruTipi1SecenekSil(this.AcikAnketSoruTipi1SecenekGetir(anket_soru_tipi1_secenek_uid));
        }

        public void AcikAnketSoruTipi1SecenekSilSoruyaGore(Guid soru_uid)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_soru_tipi_1_secenekleri where soru_uid='" + soru_uid + "'");
        }

        public DataSet AcikAnketSoruTipi1SecenekGetirSoruyaGoreDataSet(Guid soru_uid,Guid anket_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select dbo.SoruTipi1CevapOraniGetir('" + anket_uid + "',sbr_soru_tipi_1_secenekleri.soru_uid,sbr_soru_tipi_1_secenekleri.soru_secenek_uid) as cevap_oran,dbo.SoruTipi1CevapSayisiGetir('" + anket_uid + "',sbr_soru_tipi_1_secenekleri.soru_uid,sbr_soru_tipi_1_secenekleri.soru_secenek_uid) as cevap_sayisi,* from sbr_soru_tipi_1_secenekleri where soru_uid='" + soru_uid + "' order by soru_secenek_sira_no");

            return ds_result;
        }

        public string AcikAnketSoruTipi1SecenekMaxSiraNo(Guid soru_uid)
        {

            string max = BaseDB.DBManager.AppConnection.ExecuteSql("select max(soru_secenek_sira_no)+1 from sbr_soru_tipi_1_secenekleri where soru_uid='" + soru_uid + "' ");

            if (max == "")
                max = "1";

            return max;
        }
        #endregion

        #region Survey Soru Tipi 2 Optionsi
        public IQueryable<sbr_soru_tipi_2_secenekleri> TumAcikAnketSoruTipi2Secenekleri()
        {
            return db.sbr_soru_tipi_2_secenekleri;
        }
        public sbr_soru_tipi_2_secenekleri AcikAnketSoruTipi2SecenekGetir(Guid soru_secenek_uid)
        {
            return db.sbr_soru_tipi_2_secenekleri.SingleOrDefault(d => d.soru_secenek_uid == soru_secenek_uid);
        }
        public void AcikAnketSoruTipi2SecenekEkle(sbr_soru_tipi_2_secenekleri anket_soru_tipi2_secenek)
        {
            anket_soru_tipi2_secenek.soru_secenek_uid = Guid.NewGuid();
            db.sbr_soru_tipi_2_secenekleri.AddObject(anket_soru_tipi2_secenek);
        }
        public void AcikAnketSoruTipi2SecenekSil(sbr_soru_tipi_2_secenekleri anket_soru_tipi2_secenekleri)
        {
            db.sbr_soru_tipi_2_secenekleri.DeleteObject(anket_soru_tipi2_secenekleri);
        }

        public void AcikAnketSoruTipi2SecenekSil(Guid anket_soru_tipi2_secenek_uid)
        {
            this.AcikAnketSoruTipi2SecenekSil(this.AcikAnketSoruTipi2SecenekGetir(anket_soru_tipi2_secenek_uid));
        }


        public DataSet AcikAnketSoruTipi2SecenekGetirSoruyaGoreDataSet(Guid soru_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_soru_tipi_2_secenekleri where soru_uid='" + soru_uid + "' order by soru_secenek_sira_no");

            return ds_result;
        }

        public string AcikAnketSoruTipi2SecenekMaxSiraNo(Guid soru_uid)
        {

            string max = BaseDB.DBManager.AppConnection.ExecuteSql("select max(soru_secenek_sira_no)+1 from sbr_soru_tipi_2_secenekleri where soru_uid='" + soru_uid + "' ");

            if (max == "")
                max = "1";

            return max;
        }

        public void AcikAnketSoruTipi2SecenekSilSoruyaGore(Guid soru_uid)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_soru_tipi_2_secenekleri where soru_uid='" + soru_uid + "'");
        }

        #endregion

        #region Survey Soru Tipi 3 Optionsi
        public IQueryable<sbr_soru_tipi_3_secenekleri> TumAcikAnketSoruTipi3Secenekleri()
        {
            return db.sbr_soru_tipi_3_secenekleri;
        }
        public sbr_soru_tipi_3_secenekleri AcikAnketSoruTipi3SecenekGetir(Guid soru_secenek_uid)
        {
            return db.sbr_soru_tipi_3_secenekleri.SingleOrDefault(d => d.soru_secenek_uid == soru_secenek_uid);
        }
        public void AcikAnketSoruTipi3SecenekEkle(sbr_soru_tipi_3_secenekleri anket_soru_tipi3_secenek)
        {
            anket_soru_tipi3_secenek.soru_secenek_uid = Guid.NewGuid();
            db.sbr_soru_tipi_3_secenekleri.AddObject(anket_soru_tipi3_secenek);
        }
        public void AcikAnketSoruTipi3SecenekSil(sbr_soru_tipi_3_secenekleri anket_soru_tipi3_secenekleri)
        {
            db.sbr_soru_tipi_3_secenekleri.DeleteObject(anket_soru_tipi3_secenekleri);
        }

        public void AcikAnketSoruTipi3SecenekSil(Guid anket_soru_tipi3_secenek_uid)
        {
            this.AcikAnketSoruTipi3SecenekSil(this.AcikAnketSoruTipi3SecenekGetir(anket_soru_tipi3_secenek_uid));
        }


        public DataSet AcikAnketSoruTipi3SecenekGetirSoruyaGoreDataSet(Guid soru_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_soru_tipi_3_secenekleri where soru_uid='" + soru_uid + "' order by soru_secenek_sira_no");

            return ds_result;
        }

        public string AcikAnketSoruTipi3SecenekMaxSiraNo(Guid soru_uid)
        {

            string max = BaseDB.DBManager.AppConnection.ExecuteSql("select max(soru_secenek_sira_no)+1 from sbr_soru_tipi_3_secenekleri where soru_uid='" + soru_uid + "' ");

            if (max == "")
                max = "1";

            return max;
        }

        public void AcikAnketSoruTipi3SecenekSilSoruyaGore(Guid soru_uid)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_soru_tipi_3_secenekleri where soru_uid='" + soru_uid + "'");
        }

        #endregion

        #region Survey Soru Tipi 3 Optionsi Kolonlari
        public IQueryable<sbr_soru_tipi_3_secenek_kolonlari> TumAcikAnketSoruTipi3SecenekKolonlari()
        {
            return db.sbr_soru_tipi_3_secenek_kolonlari;
        }
        public sbr_soru_tipi_3_secenek_kolonlari AcikAnketSoruTipi3SecenekKolonlariGetir(Guid soru_secenek_kolon_uid)
        {
            return db.sbr_soru_tipi_3_secenek_kolonlari.SingleOrDefault(d => d.soru_secenek_kolon_uid == soru_secenek_kolon_uid);
        }
        public void AcikAnketSoruTipi3SecenekKolonEkle(sbr_soru_tipi_3_secenek_kolonlari anket_soru_tipi3_secenek_kolon)
        {
            anket_soru_tipi3_secenek_kolon.soru_secenek_kolon_uid = Guid.NewGuid();
            db.sbr_soru_tipi_3_secenek_kolonlari.AddObject(anket_soru_tipi3_secenek_kolon);
        }
        public void AcikAnketSoruTipi3SecenekKolonSil(sbr_soru_tipi_3_secenek_kolonlari anket_soru_tipi3_secenek_kolonlari)
        {
            db.sbr_soru_tipi_3_secenek_kolonlari.DeleteObject(anket_soru_tipi3_secenek_kolonlari);
        }

        public void AcikAnketSoruTipi3SecenekKolonSil(Guid anket_soru_tipi3_secenek_kolon_uid)
        {
            this.AcikAnketSoruTipi3SecenekKolonSil(this.AcikAnketSoruTipi3SecenekKolonlariGetir(anket_soru_tipi3_secenek_kolon_uid));
        }


        public DataSet AcikAnketSoruTipi3SecenekKolonGetirSoruyaGoreDataSet(Guid soru_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_soru_tipi_3_secenek_kolonlari where soru_uid='" + soru_uid + "' order by soru_secenek_kolon_sira_no");

            return ds_result;
        }

        public string AcikAnketSoruTipi3SecenekKolonMaxSiraNo(Guid soru_uid)
        {

            string max = BaseDB.DBManager.AppConnection.ExecuteSql("select max(soru_secenek_kolon_sira_no)+1 from sbr_soru_tipi_3_secenek_kolonlari where soru_uid='" + soru_uid + "' ");

            if (max == "")
                max = "1";

            return max;
        }

        public void AcikAnketSoruTipi3SecenekKolonSilSoruyaGore(Guid soru_uid)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_soru_tipi_3_secenek_kolonlari where soru_uid='" + soru_uid + "'");
        }
        #endregion

        #region Survey Soru Tipi 4 Optionsi
        public IQueryable<sbr_soru_tipi_4_secenekleri> TumAcikAnketSoruTipi4Secenekleri()
        {
            return db.sbr_soru_tipi_4_secenekleri;
        }
        public sbr_soru_tipi_4_secenekleri AcikAnketSoruTipi4SecenekGetir(Guid soru_secenek_uid)
        {
            return db.sbr_soru_tipi_4_secenekleri.SingleOrDefault(d => d.soru_secenek_uid == soru_secenek_uid);
        }
        public void AcikAnketSoruTipi4SecenekEkle(sbr_soru_tipi_4_secenekleri anket_soru_tipi4_secenek)
        {
            anket_soru_tipi4_secenek.soru_secenek_uid = Guid.NewGuid();
            db.sbr_soru_tipi_4_secenekleri.AddObject(anket_soru_tipi4_secenek);
        }
        public void AcikAnketSoruTipi4SecenekSil(sbr_soru_tipi_4_secenekleri anket_soru_tipi4_secenekleri)
        {
            db.sbr_soru_tipi_4_secenekleri.DeleteObject(anket_soru_tipi4_secenekleri);
        }

        public void AcikAnketSoruTipi4SecenekSil(Guid anket_soru_tipi4_secenek_uid)
        {
            this.AcikAnketSoruTipi4SecenekSil(this.AcikAnketSoruTipi4SecenekGetir(anket_soru_tipi4_secenek_uid));
        }


        public DataSet AcikAnketSoruTipi4SecenekGetirSoruyaGoreDataSet(Guid soru_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_soru_tipi_4_secenekleri where soru_uid='" + soru_uid + "' order by soru_secenek_sira_no");

            return ds_result;
        }

        public string AcikAnketSoruTipi4SecenekMaxSiraNo(Guid soru_uid)
        {

            string max = BaseDB.DBManager.AppConnection.ExecuteSql("select max(soru_secenek_sira_no)+1 from sbr_soru_tipi_4_secenekleri where soru_uid='" + soru_uid + "' ");

            if (max == "")
                max = "1";

            return max;
        }

        public void AcikAnketSoruTipi4SecenekSilSoruyaGore(Guid soru_uid)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_soru_tipi_4_secenekleri where soru_uid='" + soru_uid + "'");
        }
        #endregion

        #region Survey Soru Tipi 5 Optionsi
        public IQueryable<sbr_soru_tipi_5_secenekleri> TumAcikAnketSoruTipi5Secenekleri()
        {
            return db.sbr_soru_tipi_5_secenekleri;
        }
        public sbr_soru_tipi_5_secenekleri AcikAnketSoruTipi5SecenekGetir(Guid soru_secenek_uid)
        {
            return db.sbr_soru_tipi_5_secenekleri.SingleOrDefault(d => d.soru_secenek_uid == soru_secenek_uid);
        }
        public void AcikAnketSoruTipi5SecenekEkle(sbr_soru_tipi_5_secenekleri anket_soru_tipi5_secenek)
        {
            anket_soru_tipi5_secenek.soru_secenek_uid = Guid.NewGuid();
            db.sbr_soru_tipi_5_secenekleri.AddObject(anket_soru_tipi5_secenek);
        }
        public void AcikAnketSoruTipi5SecenekSil(sbr_soru_tipi_5_secenekleri anket_soru_tipi5_secenekleri)
        {
            db.sbr_soru_tipi_5_secenekleri.DeleteObject(anket_soru_tipi5_secenekleri);
        }

        public void AcikAnketSoruTipi5SecenekSil(Guid anket_soru_tipi5_secenek_uid)
        {
            this.AcikAnketSoruTipi5SecenekSil(this.AcikAnketSoruTipi5SecenekGetir(anket_soru_tipi5_secenek_uid));
        }


        public DataSet AcikAnketSoruTipi5SecenekGetirSoruyaGoreDataSet(Guid soru_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_soru_tipi_5_secenekleri where soru_uid='" + soru_uid + "' order by soru_secenek_sira_no");

            return ds_result;
        }

        public string AcikAnketSoruTipi5SecenekMaxSiraNo(Guid soru_uid)
        {

            string max = BaseDB.DBManager.AppConnection.ExecuteSql("select max(soru_secenek_sira_no)+1 from sbr_soru_tipi_5_secenekleri where soru_uid='" + soru_uid + "' ");

            if (max == "")
                max = "1";

            return max;
        }

        public void AcikAnketSoruTipi5SecenekSilSoruyaGore(Guid soru_uid)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_soru_tipi_5_secenekleri where soru_uid='" + soru_uid + "'");
        }
        #endregion

        #region Survey Soru Tipi 6 Optionsi
        public IQueryable<sbr_soru_tipi_6_secenekleri> TumAcikAnketSoruTipi6Secenekleri()
        {
            return db.sbr_soru_tipi_6_secenekleri;
        }
        public sbr_soru_tipi_6_secenekleri AcikAnketSoruTipi6SecenekGetir(Guid soru_secenek_uid)
        {
            return db.sbr_soru_tipi_6_secenekleri.SingleOrDefault(d => d.soru_secenek_uid == soru_secenek_uid);
        }
        public void AcikAnketSoruTipi6SecenekEkle(sbr_soru_tipi_6_secenekleri anket_soru_tipi6_secenek)
        {
            anket_soru_tipi6_secenek.soru_secenek_uid = Guid.NewGuid();
            db.sbr_soru_tipi_6_secenekleri.AddObject(anket_soru_tipi6_secenek);
        }
        public void AcikAnketSoruTipi6SecenekSil(sbr_soru_tipi_6_secenekleri anket_soru_tipi6_secenekleri)
        {
            db.sbr_soru_tipi_6_secenekleri.DeleteObject(anket_soru_tipi6_secenekleri);
        }

        public void AcikAnketSoruTipi6SecenekSil(Guid anket_soru_tipi6_secenek_uid)
        {
            this.AcikAnketSoruTipi6SecenekSil(this.AcikAnketSoruTipi6SecenekGetir(anket_soru_tipi6_secenek_uid));
        }


        public DataSet AcikAnketSoruTipi6SecenekGetirSoruyaGoreDataSet(Guid soru_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_soru_tipi_6_secenekleri where soru_uid='" + soru_uid + "' order by soru_secenek_sira_no");

            return ds_result;
        }

        public string AcikAnketSoruTipi6SecenekMaxSiraNo(Guid soru_uid)
        {

            string max = BaseDB.DBManager.AppConnection.ExecuteSql("select max(soru_secenek_sira_no)+1 from sbr_soru_tipi_6_secenekleri where soru_uid='" + soru_uid + "' ");

            if (max == "")
                max = "1";

            return max;
        }

        public void AcikAnketSoruTipi6SecenekSilSoruyaGore(Guid soru_uid)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_soru_tipi_6_secenekleri where soru_uid='" + soru_uid + "'");
        }
        #endregion

        #region Survey Soru Tipi 7 Optionsi
        public IQueryable<sbr_soru_tipi_7_secenekleri> TumAcikAnketSoruTipi7Secenekleri()
        {
            return db.sbr_soru_tipi_7_secenekleri;
        }
        public sbr_soru_tipi_7_secenekleri AcikAnketSoruTipi7SecenekGetir(Guid soru_secenek_uid)
        {
            return db.sbr_soru_tipi_7_secenekleri.SingleOrDefault(d => d.soru_secenek_uid == soru_secenek_uid);
        }
        public void AcikAnketSoruTipi7SecenekEkle(sbr_soru_tipi_7_secenekleri anket_soru_tipi7_secenek)
        {
            anket_soru_tipi7_secenek.soru_secenek_uid = Guid.NewGuid();
            db.sbr_soru_tipi_7_secenekleri.AddObject(anket_soru_tipi7_secenek);
        }
        public void AcikAnketSoruTipi7SecenekSil(sbr_soru_tipi_7_secenekleri anket_soru_tipi7_secenekleri)
        {
            db.sbr_soru_tipi_7_secenekleri.DeleteObject(anket_soru_tipi7_secenekleri);
        }

        public void AcikAnketSoruTipi7SecenekSil(Guid anket_soru_tipi7_secenek_uid)
        {
            this.AcikAnketSoruTipi7SecenekSil(this.AcikAnketSoruTipi7SecenekGetir(anket_soru_tipi7_secenek_uid));
        }


        public DataSet AcikAnketSoruTipi7SecenekGetirSoruyaGoreDataSet(Guid soru_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_soru_tipi_7_secenekleri where soru_uid='" + soru_uid + "' order by soru_secenek_sira_no");

            return ds_result;
        }

        public string AcikAnketSoruTipi7SecenekMaxSiraNo(Guid soru_uid)
        {

            string max = BaseDB.DBManager.AppConnection.ExecuteSql("select max(soru_secenek_sira_no)+1 from sbr_soru_tipi_7_secenekleri where soru_uid='" + soru_uid + "' ");

            if (max == "")
                max = "1";

            return max;
        }

        public void AcikAnketSoruTipi7SecenekSilSoruyaGore(Guid soru_uid)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_soru_tipi_7_secenekleri where soru_uid='" + soru_uid + "'");
        }
        #endregion

        #region Survey Soru Tipi 8 Optionsi
        public IQueryable<sbr_soru_tipi_8_secenekleri> TumAcikAnketSoruTipi8Secenekleri()
        {
            return db.sbr_soru_tipi_8_secenekleri;
        }
        public sbr_soru_tipi_8_secenekleri AcikAnketSoruTipi8SecenekGetir(Guid soru_secenek_uid)
        {
            return db.sbr_soru_tipi_8_secenekleri.SingleOrDefault(d => d.soru_secenek_uid == soru_secenek_uid);
        }
        public void AcikAnketSoruTipi8SecenekEkle(sbr_soru_tipi_8_secenekleri anket_soru_tipi8_secenek)
        {
            anket_soru_tipi8_secenek.soru_secenek_uid = Guid.NewGuid();
            db.sbr_soru_tipi_8_secenekleri.AddObject(anket_soru_tipi8_secenek);
        }
        public void AcikAnketSoruTipi8SecenekSil(sbr_soru_tipi_8_secenekleri anket_soru_tipi8_secenekleri)
        {
            db.sbr_soru_tipi_8_secenekleri.DeleteObject(anket_soru_tipi8_secenekleri);
        }

        public void AcikAnketSoruTipi8SecenekSil(Guid anket_soru_tipi8_secenek_uid)
        {
            this.AcikAnketSoruTipi8SecenekSil(this.AcikAnketSoruTipi8SecenekGetir(anket_soru_tipi8_secenek_uid));
        }


        public DataSet AcikAnketSoruTipi8SecenekGetirSoruyaGoreDataSet(Guid soru_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_soru_tipi_8_secenekleri where soru_uid='" + soru_uid + "' order by soru_secenek_sira_no");

            return ds_result;
        }

        public string AcikAnketSoruTipi8SecenekMaxSiraNo(Guid soru_uid)
        {

            string max = BaseDB.DBManager.AppConnection.ExecuteSql("select max(soru_secenek_sira_no)+1 from sbr_soru_tipi_8_secenekleri where soru_uid='" + soru_uid + "' ");

            if (max == "")
                max = "1";

            return max;
        }

        public void AcikAnketSoruTipi8SecenekSilSoruyaGore(Guid soru_uid)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_soru_tipi_8_secenekleri where soru_uid='" + soru_uid + "'");
        }
        #endregion

        #region Survey Soru Tipi 9 Optionsi
        public IQueryable<sbr_soru_tipi_9_secenekleri> TumAcikAnketSoruTipi9Secenekleri()
        {
            return db.sbr_soru_tipi_9_secenekleri;
        }
        public sbr_soru_tipi_9_secenekleri AcikAnketSoruTipi9SecenekGetir(Guid soru_secenek_uid)
        {
            return db.sbr_soru_tipi_9_secenekleri.SingleOrDefault(d => d.soru_secenek_uid == soru_secenek_uid);
        }
        public void AcikAnketSoruTipi9SecenekEkle(sbr_soru_tipi_9_secenekleri anket_soru_tipi9_secenek)
        {
            anket_soru_tipi9_secenek.soru_secenek_uid = Guid.NewGuid();
            db.sbr_soru_tipi_9_secenekleri.AddObject(anket_soru_tipi9_secenek);
        }
        public void AcikAnketSoruTipi9SecenekSil(sbr_soru_tipi_9_secenekleri anket_soru_tipi9_secenekleri)
        {
            db.sbr_soru_tipi_9_secenekleri.DeleteObject(anket_soru_tipi9_secenekleri);
        }

        public void AcikAnketSoruTipi9SecenekSil(Guid anket_soru_tipi9_secenek_uid)
        {
            this.AcikAnketSoruTipi9SecenekSil(this.AcikAnketSoruTipi9SecenekGetir(anket_soru_tipi9_secenek_uid));
        }


        public DataSet AcikAnketSoruTipi9SecenekGetirSoruyaGoreDataSet(Guid soru_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_soru_tipi_9_secenekleri where soru_uid='" + soru_uid + "' order by soru_secenek_sira_no");

            return ds_result;
        }

        public string AcikAnketSoruTipi9SecenekMaxSiraNo(Guid soru_uid)
        {

            string max = BaseDB.DBManager.AppConnection.ExecuteSql("select max(soru_secenek_sira_no)+1 from sbr_soru_tipi_9_secenekleri where soru_uid='" + soru_uid + "' ");

            if (max == "")
                max = "1";

            return max;
        }

        public void AcikAnketSoruTipi9SecenekSilSoruyaGore(Guid soru_uid)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_soru_tipi_9_secenekleri where soru_uid='" + soru_uid + "'");
        }
        #endregion

        #region Survey Soru Tipi 10 Optionsi
        public IQueryable<sbr_soru_tipi_10_secenekleri> TumAcikAnketSoruTipi10Secenekleri()
        {
            return db.sbr_soru_tipi_10_secenekleri;
        }
        public sbr_soru_tipi_10_secenekleri AcikAnketSoruTipi10SecenekGetir(Guid soru_secenek_uid)
        {
            return db.sbr_soru_tipi_10_secenekleri.SingleOrDefault(d => d.soru_secenek_uid == soru_secenek_uid);
        }
        public void AcikAnketSoruTipi10SecenekEkle(sbr_soru_tipi_10_secenekleri anket_soru_tipi10_secenek)
        {
            anket_soru_tipi10_secenek.soru_secenek_uid = Guid.NewGuid();
            db.sbr_soru_tipi_10_secenekleri.AddObject(anket_soru_tipi10_secenek);
        }
        public void AcikAnketSoruTipi10SecenekSil(sbr_soru_tipi_10_secenekleri anket_soru_tipi10_secenekleri)
        {
            db.sbr_soru_tipi_10_secenekleri.DeleteObject(anket_soru_tipi10_secenekleri);
        }

        public void AcikAnketSoruTipi10SecenekSil(Guid anket_soru_tipi10_secenek_uid)
        {
            this.AcikAnketSoruTipi10SecenekSil(this.AcikAnketSoruTipi10SecenekGetir(anket_soru_tipi10_secenek_uid));
        }


        public DataSet AcikAnketSoruTipi10SecenekGetirSoruyaGoreDataSet(Guid soru_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_soru_tipi_10_secenekleri where soru_uid='" + soru_uid + "' order by soru_secenek_sira_no");

            return ds_result;
        }

        public string AcikAnketSoruTipi10SecenekMaxSiraNo(Guid soru_uid)
        {

            string max = BaseDB.DBManager.AppConnection.ExecuteSql("select max(soru_secenek_sira_no)+1 from sbr_soru_tipi_10_secenekleri where soru_uid='" + soru_uid + "' ");

            if (max == "")
                max = "1";

            return max;
        }

        public void AcikAnketSoruTipi10SecenekSilSoruyaGore(Guid soru_uid)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_soru_tipi_10_secenekleri where soru_uid='" + soru_uid + "'");
        }
        #endregion

        #region Survey Soru Tipi 11 Optionsi
        public IQueryable<sbr_soru_tipi_11_secenekleri> TumAcikAnketSoruTipi11Secenekleri()
        {
            return db.sbr_soru_tipi_11_secenekleri;
        }
        public sbr_soru_tipi_11_secenekleri AcikAnketSoruTipi11SecenekGetir(Guid soru_secenek_uid)
        {
            return db.sbr_soru_tipi_11_secenekleri.SingleOrDefault(d => d.soru_secenek_uid == soru_secenek_uid);
        }
        public void AcikAnketSoruTipi11SecenekEkle(sbr_soru_tipi_11_secenekleri anket_soru_tipi11_secenek)
        {
            anket_soru_tipi11_secenek.soru_secenek_uid = Guid.NewGuid();
            db.sbr_soru_tipi_11_secenekleri.AddObject(anket_soru_tipi11_secenek);
        }
        public void AcikAnketSoruTipi11SecenekSil(sbr_soru_tipi_11_secenekleri anket_soru_tipi11_secenekleri)
        {
            db.sbr_soru_tipi_11_secenekleri.DeleteObject(anket_soru_tipi11_secenekleri);
        }

        public void AcikAnketSoruTipi11SecenekSil(Guid anket_soru_tipi11_secenek_uid)
        {
            this.AcikAnketSoruTipi11SecenekSil(this.AcikAnketSoruTipi11SecenekGetir(anket_soru_tipi11_secenek_uid));
        }


        public DataSet AcikAnketSoruTipi11SecenekGetirSoruyaGoreDataSet(Guid soru_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_soru_tipi_11_secenekleri where soru_uid='" + soru_uid + "' order by soru_secenek_sira_no");

            return ds_result;
        }

        public string AcikAnketSoruTipi11SecenekMaxSiraNo(Guid soru_uid)
        {

            string max = BaseDB.DBManager.AppConnection.ExecuteSql("select max(soru_secenek_sira_no)+1 from sbr_soru_tipi_11_secenekleri where soru_uid='" + soru_uid + "' ");

            if (max == "")
                max = "1";

            return max;
        }

        public void AcikAnketSoruTipi11SecenekSilSoruyaGore(Guid soru_uid)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_soru_tipi_11_secenekleri where soru_uid='" + soru_uid + "'");
        }
        #endregion

        #region Survey Soru Tipi 1 Cevaplari
        public sbr_soru_tipi_1_cevaplari AcikAnketSoruTipi1CevapGetir(Guid id)
        {
            return db.sbr_soru_tipi_1_cevaplari.SingleOrDefault(d => d.id == id);
        }
        
        

        public void AcikAnketSoruTipi1CevapEkle(sbr_soru_tipi_1_cevaplari anket_soru_tipi_1_cevaplari)
        {
            anket_soru_tipi_1_cevaplari.id = Guid.NewGuid();
            db.sbr_soru_tipi_1_cevaplari.AddObject(anket_soru_tipi_1_cevaplari);
        }
        public void AcikAnketSoruTipi1CevapSil(sbr_soru_tipi_1_cevaplari anket_soru_tipi1_cevaplari)
        {
            db.sbr_soru_tipi_1_cevaplari.DeleteObject(anket_soru_tipi1_cevaplari);
        }

        public void AcikAnketSoruTipi1CevapSil(Guid id)
        {
            this.AcikAnketSoruTipi1CevapSil(this.AcikAnketSoruTipi1CevapGetir(id));
        }


        public DataSet AcikAnketSoruTipi1CevapGetirSoruyaKeyeGoreDataSet(Guid soru_uid,string key)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select soru_secenek_uid,soru_uid,id,cevap_key,cevap from sbr_soru_tipi_1_cevaplari where soru_uid='" + soru_uid + "' and cevap_key='" + key + "'");

            return ds_result;
        }

        public sbr_soru_tipi_1_cevaplari AcikAnketSoruTipi1CevapGetirSoruyaSecenegeKeyeGore(Guid soru_uid, Guid secenek_uid, string key)
        {
            return db.sbr_soru_tipi_1_cevaplari.SingleOrDefault(d => d.soru_uid == soru_uid && d.soru_secenek_uid==secenek_uid && d.cevap_key==key);
        }

        public void AcikAnketSoruTipi1CevapSilSoruyaKeyeGore(Guid soru_uid,string key)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_soru_tipi_1_cevaplari where soru_uid='" + soru_uid + "' and cevap_key='" + key + "'");
        }
        #endregion

        #region Survey Soru Tipi 2 Cevaplari
        public sbr_soru_tipi_2_cevaplari AcikAnketSoruTipi2CevapGetir(Guid id)
        {
            return db.sbr_soru_tipi_2_cevaplari.SingleOrDefault(d => d.id == id);
        }
        public void AcikAnketSoruTipi2CevapEkle(sbr_soru_tipi_2_cevaplari anket_soru_tipi_2_cevaplari)
        {
            anket_soru_tipi_2_cevaplari.id = Guid.NewGuid();
            db.sbr_soru_tipi_2_cevaplari.AddObject(anket_soru_tipi_2_cevaplari);
        }
        public void AcikAnketSoruTipi2CevapSil(sbr_soru_tipi_2_cevaplari anket_soru_tipi2_cevaplari)
        {
            db.sbr_soru_tipi_2_cevaplari.DeleteObject(anket_soru_tipi2_cevaplari);
        }

        public void AcikAnketSoruTipi2CevapSil(Guid id)
        {
            this.AcikAnketSoruTipi2CevapSil(this.AcikAnketSoruTipi2CevapGetir(id));
        }

        public void AcikAnketSoruTipi2CevapSilSoruyaKeyeGore(Guid soru_uid, string key)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_soru_tipi_2_cevaplari where soru_uid='" + soru_uid + "' and cevap_key='" + key + "'");
        }

        public DataSet AcikAnketSoruTipi2CevapGetirSoruyaKeyeGoreDataSet(Guid soru_uid, string key)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select soru_secenek_uid,soru_uid,id,cevap_key,cevap from sbr_soru_tipi_2_cevaplari where soru_uid='" + soru_uid + "' and cevap_key='" + key + "'");

            return ds_result;
        }
        #endregion

        #region Survey Soru Tipi 3 Cevaplari
        public sbr_soru_tipi_3_cevaplari AcikAnketSoruTipi3CevapGetir(Guid id)
        {
            return db.sbr_soru_tipi_3_cevaplari.SingleOrDefault(d => d.id == id);
        }
        public void AcikAnketSoruTipi3CevapEkle(sbr_soru_tipi_3_cevaplari anket_soru_tipi_3_cevaplari)
        {
            anket_soru_tipi_3_cevaplari.id = Guid.NewGuid();
            db.sbr_soru_tipi_3_cevaplari.AddObject(anket_soru_tipi_3_cevaplari);
        }
        public void AcikAnketSoruTipi3CevapSil(sbr_soru_tipi_3_cevaplari anket_soru_tipi3_cevaplari)
        {
            db.sbr_soru_tipi_3_cevaplari.DeleteObject(anket_soru_tipi3_cevaplari);
        }

        public void AcikAnketSoruTipi3CevapSil(Guid id)
        {
            this.AcikAnketSoruTipi3CevapSil(this.AcikAnketSoruTipi3CevapGetir(id));
        }

        public void AcikAnketSoruTipi3CevapSilSoruyaKeyeGore(Guid soru_uid, string key)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_soru_tipi_3_cevaplari where soru_uid='" + soru_uid + "' and cevap_key='" + key + "'");
        }

        public DataSet AcikAnketSoruTipi3CevapGetirSoruyaKeyeGoreDataSet(Guid soru_uid, string key)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select soru_secenek_uid,soru_secenek_kolon_uid,soru_uid,id,cevap_key,cevap from sbr_soru_tipi_3_cevaplari where soru_uid='" + soru_uid + "' and cevap_key='" + key + "'");

            return ds_result;
        }
        #endregion

        #region Survey Soru Tipi 4 Cevaplari
        public sbr_soru_tipi_4_cevaplari AcikAnketSoruTipi4CevapGetir(Guid id)
        {
            return db.sbr_soru_tipi_4_cevaplari.SingleOrDefault(d => d.id == id);
        }
        public void AcikAnketSoruTipi4CevapEkle(sbr_soru_tipi_4_cevaplari anket_soru_tipi_4_cevaplari)
        {
            anket_soru_tipi_4_cevaplari.id = Guid.NewGuid();
            db.sbr_soru_tipi_4_cevaplari.AddObject(anket_soru_tipi_4_cevaplari);
        }
        public void AcikAnketSoruTipi4CevapSil(sbr_soru_tipi_4_cevaplari anket_soru_tipi4_cevaplari)
        {
            db.sbr_soru_tipi_4_cevaplari.DeleteObject(anket_soru_tipi4_cevaplari);
        }

        public void AcikAnketSoruTipi4CevapSil(Guid id)
        {
            this.AcikAnketSoruTipi4CevapSil(this.AcikAnketSoruTipi4CevapGetir(id));
        }

        public void AcikAnketSoruTipi4CevapSilSoruyaKeyeGore(Guid soru_uid, string key)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_soru_tipi_4_cevaplari where soru_uid='" + soru_uid + "' and cevap_key='" + key + "'");
        }

        public DataSet AcikAnketSoruTipi4CevapGetirSoruyaKeyeGoreDataSet(Guid soru_uid, string key)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select soru_secenek_uid,soru_uid,id,cevap_key,cevap from sbr_soru_tipi_4_cevaplari where soru_uid='" + soru_uid + "' and cevap_key='" + key + "'");

            return ds_result;
        }
        #endregion

        #region Survey Soru Tipi 7 Cevaplari
        public sbr_soru_tipi_7_cevaplari AcikAnketSoruTipi7CevapGetir(Guid id)
        {
            return db.sbr_soru_tipi_7_cevaplari.SingleOrDefault(d => d.id == id);
        }
        public void AcikAnketSoruTipi7CevapEkle(sbr_soru_tipi_7_cevaplari anket_soru_tipi_7_cevaplari)
        {
            anket_soru_tipi_7_cevaplari.id = Guid.NewGuid();
            db.sbr_soru_tipi_7_cevaplari.AddObject(anket_soru_tipi_7_cevaplari);
        }
        public void AcikAnketSoruTipi7CevapSil(sbr_soru_tipi_7_cevaplari anket_soru_tipi7_cevaplari)
        {
            db.sbr_soru_tipi_7_cevaplari.DeleteObject(anket_soru_tipi7_cevaplari);
        }

        public void AcikAnketSoruTipi7CevapSil(Guid id)
        {
            this.AcikAnketSoruTipi7CevapSil(this.AcikAnketSoruTipi7CevapGetir(id));
        }

        public void AcikAnketSoruTipi7CevapSilSoruyaKeyeGore(Guid soru_uid, string key)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_soru_tipi_7_cevaplari where soru_uid='" + soru_uid + "' and cevap_key='" + key + "'");
        }

        public DataSet AcikAnketSoruTipi7CevapGetirSoruyaKeyeGoreDataSet(Guid soru_uid, string key)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select soru_secenek_uid,soru_uid,id,cevap_key,cevap from sbr_soru_tipi_7_cevaplari where soru_uid='" + soru_uid + "' and cevap_key='" + key + "'");

            return ds_result;
        }
        #endregion

        #region Survey Soru Tipi 5 Cevaplari
        public sbr_soru_tipi_5_cevaplari AcikAnketSoruTipi5CevapGetir(Guid id)
        {
            return db.sbr_soru_tipi_5_cevaplari.SingleOrDefault(d => d.id == id);
        }
        public void AcikAnketSoruTipi5CevapEkle(sbr_soru_tipi_5_cevaplari anket_soru_tipi_5_cevaplari)
        {
            anket_soru_tipi_5_cevaplari.id = Guid.NewGuid();
            db.sbr_soru_tipi_5_cevaplari.AddObject(anket_soru_tipi_5_cevaplari);
        }
        public void AcikAnketSoruTipi5CevapSil(sbr_soru_tipi_5_cevaplari anket_soru_tipi5_cevaplari)
        {
            db.sbr_soru_tipi_5_cevaplari.DeleteObject(anket_soru_tipi5_cevaplari);
        }

        public void AcikAnketSoruTipi5CevapSilSoruyaKeyeGore(Guid soru_uid, string key)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_soru_tipi_5_cevaplari where soru_uid='" + soru_uid + "' and cevap_key='" + key + "'");
        }

        public void AcikAnketSoruTipi5CevapSil(Guid id)
        {
            this.AcikAnketSoruTipi5CevapSil(this.AcikAnketSoruTipi5CevapGetir(id));
        }


        public DataSet AcikAnketSoruTipi5CevapGetirSoruyaKeyeGoreDataSet(Guid soru_uid, string key)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select soru_secenek_uid,soru_uid,id,cevap_key,cevap from sbr_soru_tipi_5_cevaplari where soru_uid='" + soru_uid + "' and cevap_key='" + key + "'");

            return ds_result;
        }
        #endregion

        #region Survey Soru Tipi 6 Cevaplari
        public sbr_soru_tipi_6_cevaplari AcikAnketSoruTipi6CevapGetir(Guid id)
        {
            return db.sbr_soru_tipi_6_cevaplari.SingleOrDefault(d => d.id == id);
        }
        public void AcikAnketSoruTipi6CevapEkle(sbr_soru_tipi_6_cevaplari anket_soru_tipi_6_cevaplari)
        {
            anket_soru_tipi_6_cevaplari.id = Guid.NewGuid();
            db.sbr_soru_tipi_6_cevaplari.AddObject(anket_soru_tipi_6_cevaplari);
        }
        public void AcikAnketSoruTipi6CevapSil(sbr_soru_tipi_6_cevaplari anket_soru_tipi6_cevaplari)
        {
            db.sbr_soru_tipi_6_cevaplari.DeleteObject(anket_soru_tipi6_cevaplari);
        }

        public void AcikAnketSoruTipi6CevapSil(Guid id)
        {
            this.AcikAnketSoruTipi6CevapSil(this.AcikAnketSoruTipi6CevapGetir(id));
        }

        public void AcikAnketSoruTipi6CevapSilSoruyaKeyeGore(Guid soru_uid, string key)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_soru_tipi_6_cevaplari where soru_uid='" + soru_uid + "' and cevap_key='" + key + "'");
        }

        public DataSet AcikAnketSoruTipi6CevapGetirSoruyaKeyeGoreDataSet(Guid soru_uid, string key)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select soru_secenek_uid,soru_uid,id,cevap_key,cevap from sbr_soru_tipi_6_cevaplari where soru_uid='" + soru_uid + "' and cevap_key='" + key + "'");

            return ds_result;
        }
        #endregion

        #region Survey Soru Tipi 8 Cevaplari
        public sbr_soru_tipi_8_cevaplari AcikAnketSoruTipi8CevapGetir(Guid id)
        {
            return db.sbr_soru_tipi_8_cevaplari.SingleOrDefault(d => d.id == id);
        }
        public void AcikAnketSoruTipi8CevapEkle(sbr_soru_tipi_8_cevaplari anket_soru_tipi_8_cevaplari)
        {
            anket_soru_tipi_8_cevaplari.id = Guid.NewGuid();
            db.sbr_soru_tipi_8_cevaplari.AddObject(anket_soru_tipi_8_cevaplari);
        }
        public void AcikAnketSoruTipi8CevapSil(sbr_soru_tipi_8_cevaplari anket_soru_tipi8_cevaplari)
        {
            db.sbr_soru_tipi_8_cevaplari.DeleteObject(anket_soru_tipi8_cevaplari);
        }

        public void AcikAnketSoruTipi8CevapSil(Guid id)
        {
            this.AcikAnketSoruTipi8CevapSil(this.AcikAnketSoruTipi8CevapGetir(id));
        }

        public void AcikAnketSoruTipi8CevapSilSoruyaKeyeGore(Guid soru_uid, string key)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_soru_tipi_8_cevaplari where soru_uid='" + soru_uid + "' and cevap_key='" + key + "'");
        }

        public DataSet AcikAnketSoruTipi8CevapGetirSoruyaKeyeGoreDataSet(Guid soru_uid, string key)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select soru_secenek_uid,soru_uid,id,cevap_key,cevap from sbr_soru_tipi_8_cevaplari where soru_uid='" + soru_uid + "' and cevap_key='" + key + "'");

            return ds_result;
        }
        #endregion

        #region Survey Soru Tipi 9 Cevaplari
        public sbr_soru_tipi_9_cevaplari AcikAnketSoruTipi9CevapGetir(Guid id)
        {
            return db.sbr_soru_tipi_9_cevaplari.SingleOrDefault(d => d.id == id);
        }
        public void AcikAnketSoruTipi9CevapEkle(sbr_soru_tipi_9_cevaplari anket_soru_tipi_9_cevaplari)
        {
            anket_soru_tipi_9_cevaplari.id = Guid.NewGuid();
            db.sbr_soru_tipi_9_cevaplari.AddObject(anket_soru_tipi_9_cevaplari);
        }
        public void AcikAnketSoruTipi9CevapSil(sbr_soru_tipi_9_cevaplari anket_soru_tipi9_cevaplari)
        {
            db.sbr_soru_tipi_9_cevaplari.DeleteObject(anket_soru_tipi9_cevaplari);
        }

        public void AcikAnketSoruTipi9CevapSil(Guid id)
        {
            this.AcikAnketSoruTipi9CevapSil(this.AcikAnketSoruTipi9CevapGetir(id));
        }

        public void AcikAnketSoruTipi9CevapSilSoruyaKeyeGore(Guid soru_uid, string key)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_soru_tipi_9_cevaplari where soru_uid='" + soru_uid + "' and cevap_key='" + key + "'");
        }

        public DataSet AcikAnketSoruTipi9CevapGetirSoruyaKeyeGoreDataSet(Guid soru_uid, string key)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select soru_secenek_uid,soru_uid,id,cevap_key,cevap from sbr_soru_tipi_9_cevaplari where soru_uid='" + soru_uid + "' and cevap_key='" + key + "'");

            return ds_result;
        }
        #endregion

        #region Survey Soru Tipi 10 Cevaplari
        public sbr_soru_tipi_10_cevaplari AcikAnketSoruTipi10CevapGetir(Guid id)
        {
            return db.sbr_soru_tipi_10_cevaplari.SingleOrDefault(d => d.id == id);
        }
        public void AcikAnketSoruTipi10CevapEkle(sbr_soru_tipi_10_cevaplari anket_soru_tipi_10_cevaplari)
        {
            anket_soru_tipi_10_cevaplari.id = Guid.NewGuid();
            db.sbr_soru_tipi_10_cevaplari.AddObject(anket_soru_tipi_10_cevaplari);
        }
        public void AcikAnketSoruTipi10CevapSil(sbr_soru_tipi_10_cevaplari anket_soru_tipi10_cevaplari)
        {
            db.sbr_soru_tipi_10_cevaplari.DeleteObject(anket_soru_tipi10_cevaplari);
        }

        public void AcikAnketSoruTipi10CevapSil(Guid id)
        {
            this.AcikAnketSoruTipi10CevapSil(this.AcikAnketSoruTipi10CevapGetir(id));
        }

        public void AcikAnketSoruTipi10CevapSilSoruyaKeyeGore(Guid soru_uid, string key)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_soru_tipi_10_cevaplari where soru_uid='" + soru_uid + "' and cevap_key='" + key + "'");
        }

        public DataSet AcikAnketSoruTipi10CevapGetirSoruyaKeyeGoreDataSet(Guid soru_uid, string key)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select soru_secenek_uid,soru_uid,id,cevap_key,cevap from sbr_soru_tipi_10_cevaplari where soru_uid='" + soru_uid + "' and cevap_key='" + key + "'");

            return ds_result;
        }
        #endregion

        #region Survey Soru Tipi 11 Cevaplari
        public sbr_soru_tipi_11_cevaplari AcikAnketSoruTipi11CevapGetir(Guid id)
        {
            return db.sbr_soru_tipi_11_cevaplari.SingleOrDefault(d => d.id == id);
        }
        public void AcikAnketSoruTipi11CevapEkle(sbr_soru_tipi_11_cevaplari anket_soru_tipi_11_cevaplari)
        {
            anket_soru_tipi_11_cevaplari.id = Guid.NewGuid();
            db.sbr_soru_tipi_11_cevaplari.AddObject(anket_soru_tipi_11_cevaplari);
        }
        public void AcikAnketSoruTipi11CevapSil(sbr_soru_tipi_11_cevaplari anket_soru_tipi11_cevaplari)
        {
            db.sbr_soru_tipi_11_cevaplari.DeleteObject(anket_soru_tipi11_cevaplari);
        }

        public void AcikAnketSoruTipi11CevapSil(Guid id)
        {
            this.AcikAnketSoruTipi11CevapSil(this.AcikAnketSoruTipi11CevapGetir(id));
        }

        public void AcikAnketSoruTipi11CevapSilSoruyaKeyeGore(Guid soru_uid, string key)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_soru_tipi_11_cevaplari where soru_uid='" + soru_uid + "' and cevap_key='" + key + "'");
        }

        public DataSet AcikAnketSoruTipi11CevapGetirSoruyaKeyeGoreDataSet(Guid soru_uid, string key)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select soru_secenek_uid,soru_uid,id,cevap_key,cevap from sbr_soru_tipi_11_cevaplari where soru_uid='" + soru_uid + "' and cevap_key='" + key + "'");

            return ds_result;
        }
        #endregion

        #region Survey Durumu İşlemleri
        public IQueryable<sbr_anket_durum_tarihcesi> TumSurveyDurumlari()
        {
            return db.sbr_anket_durum_tarihcesi;
        }
        public sbr_anket_durum_tarihcesi SurveyDurumuGetir(Guid anket_durumu_uid)
        {
            return db.sbr_anket_durum_tarihcesi.SingleOrDefault(d => d.anket_durumu_uid == anket_durumu_uid);
        }
        public void SurveyDurumuEkle(sbr_anket_durum_tarihcesi anket_durumu)
        {
            anket_durumu.anket_durumu_uid = Guid.NewGuid();
            db.sbr_anket_durum_tarihcesi.AddObject(anket_durumu);
        }
        #endregion

        #region Survey Davet İşlemleri
        public sbr_anket_davet DavetGetir(Guid davet_uid)
        {
            return db.sbr_anket_davet.SingleOrDefault(d => d.davet_uid == davet_uid);
        }

        public sbr_anket_davet DavetGetir(string key)
        {
            return db.sbr_anket_davet.SingleOrDefault(d => d.davet_key == key);
        }

        public void DavetEkle(sbr_anket_davet davet)
        {
            davet.davet_uid = Guid.NewGuid();
            db.sbr_anket_davet.AddObject(davet);
        }


        public DataSet DavetEttiklerimDataSet(Guid grup_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_davet_ettiklerim_v where davet_edilen_grup_uid in ('"+grup_uid+"') and (davet_kabul_edildi is null) order by davet_tarihi ");
            
            return ds_result;
        }

        public void DavetKabulEt(string DavetKey,Guid user_id)
        {
            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();

            sbr_anket_davet davet = this.DavetGetir(DavetKey);

            if (davet != null && davet.davet_kabul_edildi == null)
            {

                DataSet ds_group_users = gnlDB.GetGroupUsersDataSet(Guid.Parse(davet.davet_edilen_grup_uid.ToString()));
                bool var = false;
                foreach (DataRow dr in ds_group_users.Tables[0].Rows)
                {
                    if (BaseDB.SessionContext.Current != null && BaseDB.SessionContext.Current.ActiveUser != null && Guid.Parse(dr["kullanici_uid"].ToString()) == user_id)
                    {
                        var = true;
                    }
                }

                if (var == false)
                {
                    davet.davet_kabul_edildi = true;
                    davet.davet_kabul_edilme_tarihi = DateTime.Now;
                    davet.davet_kabul_eden_kullanici_uid = user_id;
                    this.Kaydet();

                    gnl_group_user_definitions user_groups_invitation = new gnl_group_user_definitions();
                    gnlDB.GroupAddUser(user_groups_invitation);
                    user_groups_invitation.group_uid= davet.davet_edilen_grup_uid;
                    user_groups_invitation.user_uid = user_id;
                    user_groups_invitation.active = true;
                    user_groups_invitation.is_admin = false;
                    user_groups_invitation.is_user_admin = false;
                    gnlDB.Kaydet();
                }
            }
        }
        #endregion

        #region Mail Grupları
        public sbr_mail_gruplari MailGrubuGetir(Guid mail_grubu_uid)
        {
            return db.sbr_mail_gruplari.SingleOrDefault(d => d.mail_grubu_uid == mail_grubu_uid);
        }
        public void MailGrubuEkle(sbr_mail_gruplari mail_grubu)
        {
            mail_grubu.mail_grubu_uid = Guid.NewGuid();
            db.sbr_mail_gruplari.AddObject(mail_grubu);
        }
        public void MailGrubuSil(sbr_mail_gruplari mail_grubu)
        {
            mail_grubu.is_deleted = true;
            mail_grubu.deleted_date = DateTime.Now;
            this.Kaydet();
        }

        public void MailGrubuSil(Guid mail_grubu)
        {
            this.MailGrubuSil(this.MailGrubuGetir(mail_grubu));
        }

        public void MailGrubuOlustur(string grup, string ad)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            if (grup != "")
            {
                sbr_mail_gruplari mail_grubu = new sbr_mail_gruplari();
                ankDB.MailGrubuEkle(mail_grubu);
                mail_grubu.grup_uid = Guid.Parse(grup);
                mail_grubu.mail_grubu_adi = ad;
                mail_grubu.is_deleted = false;
                mail_grubu.olusturan_kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                mail_grubu.olusturma_tarihi=DateTime.Now;
                
                ankDB.Kaydet();   
            }
           
        }
        #endregion

        #region Mail Ekle
        public sbr_mail_gruplari_kullanici_listesi MailGetir(Guid id)
        {
            return db.sbr_mail_gruplari_kullanici_listesi.SingleOrDefault(d => d.id == id);
        }
        public void MailEkle(sbr_mail_gruplari_kullanici_listesi mail_listesi)
        {
            mail_listesi.id = Guid.NewGuid();
            db.sbr_mail_gruplari_kullanici_listesi.AddObject(mail_listesi);
        }
        public void MailSil(sbr_mail_gruplari_kullanici_listesi mail_listesi)
        {
            mail_listesi.is_deleted = true;
            mail_listesi.deleted_date = DateTime.Now;
            this.Kaydet();
        }

        public void MailSil(Guid id)
        {
            this.MailSil(this.MailGetir(id));
        }

        public void MailListesiOlustur(string ad, string soyad,string eposta,string grup)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            if (grup != "")
            {
                if (!IsMailListesiMailExists(eposta, Guid.Parse(grup)))
                {
                    sbr_mail_gruplari_kullanici_listesi mail_listesi = new sbr_mail_gruplari_kullanici_listesi();
                    ankDB.MailEkle(mail_listesi);
                    mail_listesi.mail_grubu_uid = Guid.Parse(grup);
                    mail_listesi.ad = ad;
                    mail_listesi.soyad = soyad;
                    mail_listesi.email = eposta;
                    mail_listesi.is_deleted = false;
                    mail_listesi.olusturan_kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                    mail_listesi.olusturma_tarihi = DateTime.Now;

                    ankDB.Kaydet();
                }
            }

        }

        public void MailListesiUpdate(Guid id, string ad, string soyad, string eposta)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            if (id != null)
            {
                sbr_mail_gruplari_kullanici_listesi mail_listesi = ankDB.MailGetir(id);
                mail_listesi.ad = ad;
                mail_listesi.soyad = soyad;
                mail_listesi.email = eposta;
                ankDB.Kaydet();
            }

        }
        public DataSet MailGetirDataSet(string criteria, Guid mail_grup_uid)
        {
            DataSet ds_result = new DataSet();

            if (mail_grup_uid != Guid.Empty)
                ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_mail_gruplari_kullanici_listesi where mail_grubu_uid in ('" + mail_grup_uid + "')" + (criteria != "" ? " and ( " + criteria + " )" : "") + " and (is_deleted is null or is_deleted=0) order by ad ");
            
            return ds_result;
        }

        public bool IsMailListesiMailExists(string eposta, Guid mail_grup_uid)
        {
            bool result = false;

            string kayit_sayisi = BaseDB.DBManager.AppConnection.ExecuteSql("select count(*) from sbr_mail_gruplari_kullanici_listesi where mail_grubu_uid in ('" + mail_grup_uid + "') and email='"+eposta.Trim()+"' and (is_deleted is null or is_deleted=0) ");

            if (kayit_sayisi != "" && kayit_sayisi != "0")
                result = true;

            return result;
        }

        #endregion

        #region Mail Dosyalari
        public sbr_mail_gruplari_dosyalari MailDosyaGetir(Guid id)
        {
            return db.sbr_mail_gruplari_dosyalari.SingleOrDefault(d => d.id == id);
        }
        public void MailDosyalariEkle(sbr_mail_gruplari_dosyalari mail)
        {
            mail.id = Guid.NewGuid();
            db.sbr_mail_gruplari_dosyalari.AddObject(mail);
        }

        public DataSet MailDosyaMaxGetir(Guid mail_grubu_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select top 1 * from sbr_mail_gruplari_dosyalari where mail_grubu_uid='" + mail_grubu_uid + "' order by dosya_eklenma_tarihi desc");

            return ds_result;
        }
        #endregion

        #region Survey Yayınlama Mail Gonderi Tarihcesi
        public sbr_anket_yayinlama_mail_gonderi_tarihcesi MailGonderiTarihcesiGetir(Guid tarihce_uid)
        {
            return db.sbr_anket_yayinlama_mail_gonderi_tarihcesi.SingleOrDefault(d => d.tarihce_uid == tarihce_uid);
        }
        public void MailGonderiTarihcesiEkle(sbr_anket_yayinlama_mail_gonderi_tarihcesi tarihce)
        {
            tarihce.tarihce_uid = Guid.NewGuid();
            db.sbr_anket_yayinlama_mail_gonderi_tarihcesi.AddObject(tarihce);
        }

        public void MailGonderiTarihcesiObjectEkle(sbr_anket_yayinlama_mail_gonderi_tarihcesi tarihce)
        {
            db.sbr_anket_yayinlama_mail_gonderi_tarihcesi.AddObject(tarihce);
        }

        public DataSet MailGonderiTarihcesiDataSet(Guid anket_uid)
        {

            return BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_anket_yayinlama_mail_gonderi_tarihcesi_v where anket_uid='" + anket_uid + "'");
        
        }

        #endregion

        #region Survey Yayınlama Mail Gonderi Kişi Tarihcesi
        public sbr_anket_yayinlama_mail_gonderi_kisi_tarihcesi MailGonderiKisiTarihcesiGetir(Guid id)
        {
            return db.sbr_anket_yayinlama_mail_gonderi_kisi_tarihcesi.SingleOrDefault(d => d.id == id);
        }
        public void MailGonderiKisiTarihcesiEkle(sbr_anket_yayinlama_mail_gonderi_kisi_tarihcesi tarihce)
        {
            tarihce.id = Guid.NewGuid();
            db.sbr_anket_yayinlama_mail_gonderi_kisi_tarihcesi.AddObject(tarihce);
        }

        public DataSet MailGonderiKisiTarihcesiGetirDataSet(Guid tarihce_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_anket_yayinlama_mail_gonderi_kisi_tarihcesi_v where tarihce_uid='" + tarihce_uid + "' ");

            return ds_result;
        }
        #endregion

        #region Survey Yayınlama Mail Gonderi Aktivasyon
        public sbr_anket_yayinlama_mail_gonderi_aktivasyon MailGonderiAktivasyonGetir(Guid id)
        {
            return db.sbr_anket_yayinlama_mail_gonderi_aktivasyon.SingleOrDefault(d => d.id == id);
        }

        public sbr_anket_yayinlama_mail_gonderi_aktivasyon MailGonderiAktivasyonGetirKeyeGore(string key)
        {
            return db.sbr_anket_yayinlama_mail_gonderi_aktivasyon.SingleOrDefault(d => d.anket_gonderilme_key == key);
        }

        public void MailGonderiAktivasyonEkle(sbr_anket_yayinlama_mail_gonderi_aktivasyon aktivasyon)
        {
            aktivasyon.id = Guid.NewGuid();
            db.sbr_anket_yayinlama_mail_gonderi_aktivasyon.AddObject(aktivasyon);
        }

        public bool AktivasyonMailGrubaGonderildiMi(Guid anket_uid, Guid grup_uid)
        {
            bool result = false;

            string ksayisi = BaseDB.DBManager.AppConnection.ExecuteSql("select count(*) from sbr_anket_yayinlama_mail_gonderi_aktivasyon where anket_uid='" + anket_uid + "' and anket_mail_grubu_uid='" + grup_uid.ToString() + "'");

            if (ksayisi != "" && ksayisi != "0")
                result = true;

            return result;
        }

        public int GrubaGonderilenMailSayisi(Guid anket_uid, Guid grup_uid)
        {
            int result = 0;

            string ksayisi = BaseDB.DBManager.AppConnection.ExecuteSql("select count(*) from sbr_anket_yayinlama_mail_gonderi_aktivasyon where anket_uid='" + anket_uid + "' and anket_mail_grubu_uid='" + grup_uid.ToString() + "'");

            if (ksayisi != "" && ksayisi != "0")
                result = Convert.ToInt32(ksayisi);

            return result;
        }

        public bool AktivasyonMailGonderildiMi(Guid anket_uid, Guid grup_uid,string mail)
        {
            bool result = false;

            string ksayisi = BaseDB.DBManager.AppConnection.ExecuteSql("select count(*) from sbr_anket_yayinlama_mail_gonderi_aktivasyon where anket_uid='" + anket_uid + "' and anket_gonderilen_email='" + mail + "'");

            if (ksayisi != "" && ksayisi != "0")
                result = true;

            return result;
        }

        public int KalanKatilimciSurveySayisi(Guid mail_grubu_uid,Guid user_uid,Guid anket_uid,Guid grup_uid)
        {
            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();
            gnl_uye_kullanicilar member_users = gnlDB.GetMemberUsersByGroup(grup_uid);
            int anket_sayisi = 0;

            DataSet ds_kullanici_gruplari = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_mail_gruplari where grup_uid ='" + grup_uid.ToString() + "'");

            int gonderilen_mail_sayisi = 0;

            foreach (DataRow dr in ds_kullanici_gruplari.Tables[0].Rows)
            {
                if (dr["mail_grubu_uid"] != System.DBNull.Value && dr["mail_grubu_uid"].ToString() != "")
                {
                    gonderilen_mail_sayisi += this.GrubaGonderilenMailSayisi(anket_uid,Guid.Parse(dr["mail_grubu_uid"].ToString()));
                }
            }

            if (!gnlDB.IsPurchasedUserGrubaGore(grup_uid))
            {
                anket_sayisi = gnlDB.OdemetipiKatilimciSayisiGetir(1);
                if (gonderilen_mail_sayisi >= anket_sayisi)
                {
                    anket_sayisi = 0;
                }
                else
                {
                    anket_sayisi = anket_sayisi - gonderilen_mail_sayisi;
                }
            }
            else
            {
                if (member_users != null && member_users.son_odeme_tipi_id != null)
                {
                    int son_odeme_tipi = Convert.ToInt32(member_users.son_odeme_tipi_id.ToString());
                    anket_sayisi = gnlDB.OdemetipiKatilimciSayisiGetir(son_odeme_tipi);

                    if (gonderilen_mail_sayisi >= anket_sayisi)
                    {
                        anket_sayisi = 0;
                    }
                    else
                    {
                        anket_sayisi = anket_sayisi - gonderilen_mail_sayisi;
                    }
                }
            }

            return anket_sayisi;
        }

        #endregion

        #region Survey Yayınlama mail Gönderimi
        public bool anket_yayinlama_maili_gonder(string subject,string message, string applicationPath, Guid mail_grubu_uid,Guid anket_uid,Guid tarihce_uid, string anket_adi,Guid user_uid,Guid grup_uid)
        {
            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();
            gnl_uye_kullanicilar member_users = gnlDB.GetMemberUsersByGroup(grup_uid);
            int anket_sayisi = 0;
            
            DataSet ds_kullanici_gruplari = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_mail_gruplari where grup_uid ='" + grup_uid.ToString() + "'");

            int gonderilen_mail_sayisi = 0;

            foreach (DataRow dr in ds_kullanici_gruplari.Tables[0].Rows)
            {
                if (dr["mail_grubu_uid"] != System.DBNull.Value && dr["mail_grubu_uid"].ToString() != "")
                {
                    gonderilen_mail_sayisi += this.GrubaGonderilenMailSayisi(anket_uid, Guid.Parse(dr["mail_grubu_uid"].ToString()));
                }
            }

            if (!gnlDB.IsPurchasedUser(user_uid))
            {
                anket_sayisi = gnlDB.OdemetipiKatilimciSayisiGetir(1);
                if (gonderilen_mail_sayisi >= anket_sayisi)
                {
                    anket_sayisi = 0;
                }
                else
                {
                    anket_sayisi = anket_sayisi - gonderilen_mail_sayisi;
                }
            }
            else
            {
                if (member_users != null && member_users.son_odeme_tipi_id != null)
                {
                    int son_odeme_tipi = Convert.ToInt32(member_users.son_odeme_tipi_id.ToString());
                    anket_sayisi = gnlDB.OdemetipiKatilimciSayisiGetir(son_odeme_tipi);

                    if (gonderilen_mail_sayisi >= anket_sayisi)
                    {
                        anket_sayisi = 0;
                    }
                    else
                    {
                        anket_sayisi = anket_sayisi - gonderilen_mail_sayisi;
                    }
                }
            }


            DataSet dsliste = this.MailGetirDataSet("",mail_grubu_uid);
            ArrayList mail_arr = new ArrayList();
            ArrayList mail_ad_arr = new ArrayList();
            ArrayList mail_soyad_arr = new ArrayList();
            
            if (dsliste.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in dsliste.Tables[0].Rows)
                {
                    if (anket_sayisi > 0)
                    {
                        if (dr["email"] != System.DBNull.Value && dr["email"].ToString() != "" && dr["ad"] != System.DBNull.Value && dr["ad"].ToString().Trim() != "" && dr["soyad"] != System.DBNull.Value && dr["soyad"].ToString().Trim() != "" )
                        {
                            if (!this.AktivasyonMailGonderildiMi(anket_uid, mail_grubu_uid, dr["email"].ToString()))
                            {
                                mail_arr.Add(dr["email"].ToString());
                                mail_ad_arr.Add(dr["ad"].ToString());
                                mail_soyad_arr.Add(dr["soyad"].ToString());
                                anket_sayisi -= 1;
                            }
                        }
                    }
                }
            }

            

            System.IO.StreamReader template = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"Anket\Templates\AnketYayinlamaMailTemplate.html");
            string mailBody = template.ReadToEnd();
            template.Close();
            bool herhangi_mail_gonderildi = false;
            for (int i = 0; i < mail_arr.Count; i++)
            {
                
                string tempMailBody = mailBody;
                tempMailBody = tempMailBody.Replace("%%path_url%%", applicationPath);
                tempMailBody = tempMailBody.Replace("%%isim%%", mail_ad_arr[i].ToString()+" "+mail_soyad_arr[i].ToString());
                tempMailBody = tempMailBody.Replace("%%subject%%", subject);
                tempMailBody = tempMailBody.Replace("%%message%%", message);
                
                string key = System.Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 20);
                string link = applicationPath + "Anket/Anket.aspx?anket_uid=" + anket_uid + "&key=" + key;
                tempMailBody = tempMailBody.Replace("%%link%%", link);

                if (this.AktivasyonMailGonderildiMi(anket_uid, mail_grubu_uid, mail_arr[i].ToString().Trim()))
                   continue;

                try
                {
                    if (BaseClasses.BaseFunctions.getInstance().IsEmailValid(mail_arr[i].ToString().Trim()))
                    {
                        //BaseClasses.BaseFunctions.getInstance().SendSMTPMail(mail_arr[i].ToString(), "", anket_adi, tempMailBody, "", null, "", "genel");
                        string yeni_anket_adi = anket_adi;
                        if (member_users != null)
                        {
                            if (member_users.sirket_adi.Trim() != "")
                            {
                                yeni_anket_adi = member_users.sirket_adi.ToString() + " tarafından " + mail_ad_arr[i].ToString() + " " + mail_soyad_arr[i].ToString() + " için Survey Daveti : " + anket_adi;
                                tempMailBody = tempMailBody.Replace("%%gonderen%%", member_users.sirket_adi.ToString() + " - " + member_users.ad + " " + member_users.soyad);
                            }
                            else
                            {
                                yeni_anket_adi = member_users.ad + " " + member_users.soyad + " tarafından " + mail_ad_arr[i].ToString() + " " + mail_soyad_arr[i].ToString() + " için Survey Daveti : " + anket_adi;
                                tempMailBody = tempMailBody.Replace("%%gonderen%%", member_users.ad + " " + member_users.soyad);
                            }
                        }
                        else
                        {
                            gnl_users user = gnlDB.GetUsersByGroup(grup_uid);
                            yeni_anket_adi = user.name + " " + user.surname + " tarafından " + mail_ad_arr[i].ToString() + " " + mail_soyad_arr[i].ToString() + " için Survey Daveti : " + anket_adi;
                            tempMailBody = tempMailBody.Replace("%%gonderen%%", user.name + " " + user.surname);
                        }


                        gnlDB.InsertToMailService(mail_arr[i].ToString(), yeni_anket_adi, tempMailBody,anket_uid);
                    }
                    
                }
                catch (Exception exp)
                {
                    continue;
                }

                if (BaseClasses.BaseFunctions.getInstance().IsEmailValid(mail_arr[i].ToString().Trim()))
                {
                    sbr_anket_yayinlama_mail_gonderi_kisi_tarihcesi tarihce = new sbr_anket_yayinlama_mail_gonderi_kisi_tarihcesi();
                    this.MailGonderiKisiTarihcesiEkle(tarihce);
                    tarihce.gonderilen_kisi_ismi = mail_ad_arr[i].ToString() + " " + mail_soyad_arr[i].ToString();
                    tarihce.gonderilen_email_adresi = mail_arr[i].ToString();
                    tarihce.tarihce_uid = tarihce_uid;
                    this.Kaydet();

                    sbr_anket_yayinlama_mail_gonderi_aktivasyon aktivasyon = new sbr_anket_yayinlama_mail_gonderi_aktivasyon();
                    this.MailGonderiAktivasyonEkle(aktivasyon);
                    aktivasyon.anket_gonderilen_email = mail_arr[i].ToString();
                    aktivasyon.anket_gonderilme_key = key;
                    aktivasyon.anket_gonderilme_tarihi = DateTime.Now;
                    aktivasyon.anket_uid = anket_uid;
                    aktivasyon.tarihce_uid = tarihce_uid;
                    aktivasyon.anket_gonderilen_ad = mail_ad_arr[i].ToString();
                    aktivasyon.anket_gonderilen_soyad = mail_soyad_arr[i].ToString();
                    aktivasyon.anket_mail_grubu_uid = mail_grubu_uid;
                    this.Kaydet();

                    herhangi_mail_gonderildi = true;
                }

                
            }

            return herhangi_mail_gonderildi;
        }
        #endregion

        #region Survey Test Mail Gonderi Kişi Tarihcesi
        public sbr_anket_test_mail_gonderi_kisi_tarihcesi TestMailGonderiKisiTarihcesiGetir(Guid id)
        {
            return db.sbr_anket_test_mail_gonderi_kisi_tarihcesi.SingleOrDefault(d => d.id == id);
        }
        public void TestMailGonderiKisiTarihcesiEkle(sbr_anket_test_mail_gonderi_kisi_tarihcesi tarihce)
        {
            tarihce.id = Guid.NewGuid();
            db.sbr_anket_test_mail_gonderi_kisi_tarihcesi.AddObject(tarihce);
        }

        public sbr_anket_test_mail_gonderi_kisi_tarihcesi TestTarihceGetirKeyeGore(string key)
        {
            return db.sbr_anket_test_mail_gonderi_kisi_tarihcesi.SingleOrDefault(d => d.anket_test_key == key);
        }

        public DataSet TestSurveySonuclariGetirDataSet(Guid anket_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_test_anket_gonderi_sonuclari_v where anket_uid='" + anket_uid + "'");

            return ds_result;
        }
        #endregion

        #region Survey Test Maili Gönder
        public bool anket_test_maili_gonder(string mailler, string applicationPath, Guid anket_uid, string anket_adi)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            mailler = mailler.Replace("\r\n", "#");
            string[] mail_arr = mailler.Split('#');


            System.IO.StreamReader template = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"Anket\Templates\AnketTestMailTemplate.html");
            string mailBody = template.ReadToEnd();
            template.Close();
            bool herhangi_mail_gonderildi = false;
            for (int i = 0; i < mail_arr.Length; i++)
            {

                string tempMailBody = mailBody;
                string key = System.Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 20);
                string link = applicationPath + "Anket/Anket.aspx?anket_uid=" + anket_uid + "&Preview=1&Test=1" + "&test_key=" + key;
                tempMailBody = tempMailBody.Replace("%%path_url%%", applicationPath);
                tempMailBody = tempMailBody.Replace("%%link%%", link);

                try
                {
                    if (BaseClasses.BaseFunctions.getInstance().IsEmailValid(mail_arr[i].ToString().Trim()))
                    {
                        BaseClasses.BaseFunctions.getInstance().SendSMTPMail(mail_arr[i].ToString(), "", anket_adi, tempMailBody, "", null, "", "genel");
                    }

                }
                catch (Exception exp)
                {
                    continue;
                }

                if (BaseClasses.BaseFunctions.getInstance().IsEmailValid(mail_arr[i].ToString().Trim()))
                {
                    sbr_anket_test_mail_gonderi_kisi_tarihcesi tarihce = new sbr_anket_test_mail_gonderi_kisi_tarihcesi();
                    TestMailGonderiKisiTarihcesiEkle(tarihce);
                    tarihce.gonderi_tarihi = DateTime.Now;
                    tarihce.gonderen_user_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                    tarihce.anket_test_key = key;
                    tarihce.anket_uid = anket_uid;
                    tarihce.gonderilen_email_adresi = mail_arr[i].ToString();
                    this.Kaydet();

                    herhangi_mail_gonderildi = true;
                }
                
            }

            return herhangi_mail_gonderildi;
        }
        #endregion

        #region Survey Yayınlama Açık Survey Aktivasyon
        public sbr_anket_yayinlama_acik_anket_aktivasyon AcikSurveyAktivasyonGetir(Guid id)
        {
            return db.sbr_anket_yayinlama_acik_anket_aktivasyon.SingleOrDefault(d => d.id == id);
        }

        public sbr_anket_yayinlama_acik_anket_aktivasyon AcikSurveyAktivasyonGetirKeyeGore(Guid anket_uid,string key)
        {
            return db.sbr_anket_yayinlama_acik_anket_aktivasyon.FirstOrDefault(d => d.giris_key == key && d.anket_uid == anket_uid);
        }

        public void AcikSurveyAktivasyonEkle(sbr_anket_yayinlama_acik_anket_aktivasyon aktivasyon)
        {
            aktivasyon.id = Guid.NewGuid();
            db.sbr_anket_yayinlama_acik_anket_aktivasyon.AddObject(aktivasyon);
        }
        #endregion

        #region Raporlar
        public DataSet KullaniciBazliSurveyRaporu(Guid anket_uid, Guid kullanici_email)
        {

            string sql = "SELECT     sbr_anket.anket_uid, sbr_anket.anket_adi, sbr_anket_sorulari.soru_uid, sbr_anket_sorulari.soru, sbr_anket_sorulari.soru_tipi_id,dbo.GetKullaniciBazliSurveyCevaplari(sbr_anket.anket_uid,sbr_anket_sorulari.soru_uid,sbr_anket_sorulari.soru_tipi_id,'" + kullanici_email + "') as cevap" +
                       "FROM       sbr_anket,sbr_anket_sorulari " +
                       "Where      sbr_anket.anket_uid = sbr_anket_sorulari.anket_uid "+
                       "And    sbr_anket.anket_uid='"+anket_uid+"'";

            return  BaseDB.DBManager.AppConnection.GetDataSet(sql);
        }

        #endregion

        #region Sablon Durumu İşlemleri
        public IQueryable<sbr_sablon_durum_tarihcesi> TumSablonDurumlari()
        {
            return db.sbr_sablon_durum_tarihcesi;
        }
        public sbr_sablon_durum_tarihcesi SablonDurumuGetir(Guid sablon_durumu_uid)
        {
            return db.sbr_sablon_durum_tarihcesi.SingleOrDefault(d => d.sablon_durumu_uid == sablon_durumu_uid);
        }
        public void SablonDurumuEkle(sbr_sablon_durum_tarihcesi sablon_durumu)
        {
            sablon_durumu.sablon_durumu_uid = Guid.NewGuid();
            db.sbr_sablon_durum_tarihcesi.AddObject(sablon_durumu);
        }
        #endregion

        #region Şablon İşlemleri
        public IQueryable<sbr_sablon> TumSablonler()
        {
            return db.sbr_sablon;
        }
        public sbr_sablon SablonGetir(Guid sablon_uid)
        {
            return db.sbr_sablon.SingleOrDefault(d => d.sablon_uid == sablon_uid);
        }
        public void SablonEkle(sbr_sablon sablon)
        {
            sablon.sablon_uid = Guid.NewGuid();
            sablon.kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
            db.sbr_sablon.AddObject(sablon);
        }
        public void SablonSil(sbr_sablon sablon)
        {
            db.sbr_sablon.DeleteObject(sablon);
        }

        public void SablonSil(Guid sablon_uid)
        {
            this.SablonSil(this.SablonGetir(sablon_uid));
        }

        public DataSet SablonGetirDurumaGoreDataSet(int anket_durum_id, string criteria, Guid grup_uid)
        {
            DataSet ds_result = new DataSet();

            if (anket_durum_id == 0)
            {
               ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_sablon_v where grup_uid in ('" + grup_uid + "')" + (criteria != "" ? " and ( " + criteria + " )" : "") + " order by olusturma_tarihi desc");
            }
            else
            {
               ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_sablon_v where grup_uid in ('" + grup_uid + "') and sablon_durumu_id=" + anket_durum_id + (criteria != "" ? " and ( " + criteria + " )" : "") + " order by olusturma_tarihi desc");
            }
            return ds_result;
        }

        public void SablonDurumuDegistir(int sablon_durumu_id, Guid sablon_uid, string aciklama)
        {
            sbr_sablon_durum_tarihcesi sablon_durumu = new sbr_sablon_durum_tarihcesi();

            this.SablonDurumuEkle(sablon_durumu);
            sablon_durumu.sablon_durumu_id = sablon_durumu_id;
            sablon_durumu.sablon_uid = sablon_uid;
            sablon_durumu.durum_olusma_tarihi = DateTime.Now;
            sablon_durumu.durumu_olusturan_kullanici = BaseDB.SessionContext.Current.ActiveUser.UserUid;
            sablon_durumu.durum_aciklamasi = aciklama;

            this.Kaydet();

            sbr_sablon sablon = this.SablonGetir(sablon_uid);
            sablon.sablon_durumu_id = sablon_durumu_id;
            sablon.sablon_durumu_uid = sablon_durumu.sablon_durumu_uid;

            this.Kaydet();
        }

        public string SablonDurumu(int sablon_durum_tipi_id)
        {
            string result = "";

            if (sablon_durum_tipi_id.ToString() == ((int)BaseWebSite.Models.sablon_durumu.Acik).ToString())
            {
                result = "Açık";
            }
            else if (sablon_durum_tipi_id.ToString() == ((int)BaseWebSite.Models.sablon_durumu.Kapali).ToString())
            {
                result = "Kapalı";
            }
           

            return result;
        }

    
        #endregion

        #region Sablon Soruları
        public IQueryable<sbr_sablon_sorulari> TumSablonSorulari()
        {
            return db.sbr_sablon_sorulari;
        }
        public sbr_sablon_sorulari SablonSoruGetir(Guid sablon_soru_uid)
        {
            return db.sbr_sablon_sorulari.SingleOrDefault(d => d.soru_uid == sablon_soru_uid);
        }
        public void SablonSoruEkle(sbr_sablon_sorulari sablon_soru)
        {
            sablon_soru.soru_uid = Guid.NewGuid();
            db.sbr_sablon_sorulari.AddObject(sablon_soru);
        }
        public void SablonSoruSil(sbr_sablon_sorulari sablon_soru)
        {
            db.sbr_sablon_sorulari.DeleteObject(sablon_soru);
        }

        public void SablonSoruSil(Guid sablon_soru_uid)
        {
            this.SablonSoruSil(this.SablonSoruGetir(sablon_soru_uid));
        }


        public DataSet SablonSoruGetirSablonGoreDataSet(Guid sablon_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select ROW_NUMBER() OVER(ORDER BY soru_sira_no) AS 'RowNumber',case when soru_zorunlu=1 then 'Bu soru zorunludur!' else '' end zorunlu_soru,case when soru_zorunlu=1 then 'fldTitle reuire' else 'fldTitle' end zorunlu_class,* from ( select * from sbr_sablon_soru_v where sablon_uid='" + sablon_uid + "' ) as table1 ORDER BY soru_sira_no");

            return ds_result;
        }

    
        public int TumSablonSoruSayısı(Guid sablon_uid, string cevap_key)
        {

            string tum_soru_sayisi = BaseDB.DBManager.AppConnection.ExecuteSql("select count(*) from sbr_sablon_soru_v where sablon_uid='" + sablon_uid + "' ");

            if (tum_soru_sayisi == "")
                tum_soru_sayisi = "0";

            return Convert.ToInt32(tum_soru_sayisi);
        }

        public string SablonSoruMaxSiraNo(Guid sablon_uid)
        {

            string max = BaseDB.DBManager.AppConnection.ExecuteSql("select max(soru_sira_no)+1 from sbr_sablon_sorulari where sablon_uid='" + sablon_uid + "'");

            if (max == "")
                max = "1";

            return max;
        }

        public void SablonSoruOlustur(Guid sablon_uid, string soru_tipi, string soru, string soru_secenekleri, string cevap_kolonları, string soru_sayisal_ondalik,string tek_satir)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            if (soru_tipi == "1")
            {
                sbr_sablon_sorulari sablon_sorulari = new sbr_sablon_sorulari();
                ankDB.SablonSoruEkle(sablon_sorulari);
                sablon_sorulari.sablon_uid = sablon_uid;
                sablon_sorulari.soru = soru;
                sablon_sorulari.soru_tipi_id = Convert.ToInt32(soru_tipi);
                //sablon_sorulari.soru_sitili_id = Convert.ToInt32(soru_sitili);
                //sablon_sorulari.soru_siralama_sekli_id = Convert.ToInt32(siralama_sekli);
                sablon_sorulari.soru_olusturan_kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                sablon_sorulari.soru_olusturma_tarihi = DateTime.Now;
                sablon_sorulari.soru_sira_no = Convert.ToInt32(ankDB.SablonSoruMaxSiraNo(sablon_uid));

                ankDB.Kaydet();

                soru_secenekleri = soru_secenekleri.Replace("\r\n", "#");
                string[] soru_secenekleri_arr = soru_secenekleri.Split('#');

                foreach (string secenekler in soru_secenekleri_arr)
                {
                    if (secenekler.Trim() != "")
                    {
                        sbr_sablon_soru_tipi_1_secenekleri sablon_secenekler = new sbr_sablon_soru_tipi_1_secenekleri();
                        ankDB.SablonSoruTipi1SecenekEkle(sablon_secenekler);
                        sablon_secenekler.soru_secenek_ad = secenekler;
                        sablon_secenekler.soru_uid = sablon_sorulari.soru_uid;
                        sablon_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.SablonSoruTipi1SecenekMaxSiraNo(sablon_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }
                }
            }
            else if (soru_tipi == "2")
            {
                sbr_sablon_sorulari sablon_sorulari = new sbr_sablon_sorulari();
                ankDB.SablonSoruEkle(sablon_sorulari);
                sablon_sorulari.sablon_uid = sablon_uid;
                sablon_sorulari.soru = soru;
                sablon_sorulari.soru_tipi_id = Convert.ToInt32(soru_tipi);
                //sablon_sorulari.soru_siralama_sekli_id = Convert.ToInt32(siralama_sekli);
                sablon_sorulari.soru_olusturan_kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                sablon_sorulari.soru_olusturma_tarihi = DateTime.Now;
                sablon_sorulari.soru_sira_no = Convert.ToInt32(ankDB.SablonSoruMaxSiraNo(sablon_uid));

                ankDB.Kaydet();

                soru_secenekleri = soru_secenekleri.Replace("\r\n", "#");
                string[] soru_secenekleri_arr = soru_secenekleri.Split('#');

                foreach (string secenekler in soru_secenekleri_arr)
                {
                    if (secenekler.Trim() != "")
                    {
                        sbr_sablon_soru_tipi_2_secenekleri sablon_secenekler = new sbr_sablon_soru_tipi_2_secenekleri();
                        ankDB.SablonSoruTipi2SecenekEkle(sablon_secenekler);
                        sablon_secenekler.soru_secenek_ad = secenekler;
                        sablon_secenekler.soru_uid = sablon_sorulari.soru_uid;
                        sablon_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.SablonSoruTipi2SecenekMaxSiraNo(sablon_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }
                }
            }
            else if (soru_tipi == "3")
            {
                sbr_sablon_sorulari sablon_sorulari = new sbr_sablon_sorulari();
                ankDB.SablonSoruEkle(sablon_sorulari);
                sablon_sorulari.sablon_uid = sablon_uid;
                sablon_sorulari.soru = soru;
                sablon_sorulari.soru_tipi_id = Convert.ToInt32(soru_tipi);
                //sablon_sorulari.soru_siralama_sekli_id = Convert.ToInt32(siralama_sekli);
                sablon_sorulari.soru_olusturan_kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                sablon_sorulari.soru_olusturma_tarihi = DateTime.Now;
                sablon_sorulari.soru_sira_no = Convert.ToInt32(ankDB.SablonSoruMaxSiraNo(sablon_uid));

                ankDB.Kaydet();

                soru_secenekleri = soru_secenekleri.Replace("\r\n", "#");
                string[] soru_secenekleri_arr = soru_secenekleri.Split('#');

                foreach (string secenekler in soru_secenekleri_arr)
                {
                    if (secenekler.Trim() != "")
                    {
                        sbr_sablon_soru_tipi_3_secenekleri sablon_secenekler = new sbr_sablon_soru_tipi_3_secenekleri();
                        ankDB.SablonSoruTipi3SecenekEkle(sablon_secenekler);
                        sablon_secenekler.soru_secenek_ad = secenekler;
                        sablon_secenekler.soru_uid = sablon_sorulari.soru_uid;
                        sablon_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.SablonSoruTipi3SecenekMaxSiraNo(sablon_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }
                }

                cevap_kolonları = cevap_kolonları.Replace("\r\n", "#");
                string[] cevap_kolonları_arr = cevap_kolonları.Split('#');

                foreach (string kolonlar in cevap_kolonları_arr)
                {
                    if (kolonlar.Trim() != "")
                    {
                        sbr_sablon_soru_tipi_3_secenek_kolonlari sablon_secenek_kolonlari = new sbr_sablon_soru_tipi_3_secenek_kolonlari();
                        ankDB.SablonSoruTipi3SecenekKolonEkle(sablon_secenek_kolonlari);
                        sablon_secenek_kolonlari.soru_secenek_kolon_ad = kolonlar;
                        sablon_secenek_kolonlari.soru_uid = sablon_sorulari.soru_uid;
                        sablon_secenek_kolonlari.soru_secenek_kolon_sira_no = Convert.ToInt32(ankDB.SablonSoruTipi3SecenekKolonMaxSiraNo(sablon_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }
                }
            }
            else if (soru_tipi == "4")
            {
                sbr_sablon_sorulari sablon_sorulari = new sbr_sablon_sorulari();
                ankDB.SablonSoruEkle(sablon_sorulari);
                sablon_sorulari.sablon_uid = sablon_uid;
                sablon_sorulari.soru = soru;
                sablon_sorulari.soru_tipi_id = Convert.ToInt32(soru_tipi);
                sablon_sorulari.soru_olusturan_kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                sablon_sorulari.soru_olusturma_tarihi = DateTime.Now;
                sablon_sorulari.soru_sira_no = Convert.ToInt32(ankDB.SablonSoruMaxSiraNo(sablon_uid));
                
                if (tek_satir != null && (tek_satir == "true" || tek_satir == "True" || tek_satir == "TRUE"))
                    sablon_sorulari.soru_tek_satir = true;
                else
                    sablon_sorulari.soru_tek_satir = false;

                ankDB.Kaydet();

                sbr_sablon_soru_tipi_4_secenekleri sablon_secenekler = new sbr_sablon_soru_tipi_4_secenekleri();
                ankDB.SablonSoruTipi4SecenekEkle(sablon_secenekler);
                sablon_secenekler.soru_secenek_ad = soru;
                sablon_secenekler.soru_uid = sablon_sorulari.soru_uid;
                sablon_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.SablonSoruTipi4SecenekMaxSiraNo(sablon_sorulari.soru_uid));
                ankDB.Kaydet();
            }
            else if (soru_tipi == "5")
            {
                sbr_sablon_sorulari sablon_sorulari = new sbr_sablon_sorulari();
                ankDB.SablonSoruEkle(sablon_sorulari);
                sablon_sorulari.sablon_uid = sablon_uid;
                sablon_sorulari.soru = soru;
                sablon_sorulari.soru_tipi_id = Convert.ToInt32(soru_tipi);
                sablon_sorulari.soru_olusturan_kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                sablon_sorulari.soru_olusturma_tarihi = DateTime.Now;
                sablon_sorulari.soru_sira_no = Convert.ToInt32(ankDB.SablonSoruMaxSiraNo(sablon_uid));

                ankDB.Kaydet();

                sbr_sablon_soru_tipi_5_secenekleri sablon_secenekler = new sbr_sablon_soru_tipi_5_secenekleri();
                ankDB.SablonSoruTipi5SecenekEkle(sablon_secenekler);
                sablon_secenekler.soru_secenek_ad = "Doğru";
                sablon_secenekler.soru_uid = sablon_sorulari.soru_uid;
                sablon_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.SablonSoruTipi5SecenekMaxSiraNo(sablon_sorulari.soru_uid));
                ankDB.Kaydet();

                sbr_sablon_soru_tipi_5_secenekleri sablon_secenekler2 = new sbr_sablon_soru_tipi_5_secenekleri();
                ankDB.SablonSoruTipi5SecenekEkle(sablon_secenekler2);
                sablon_secenekler2.soru_secenek_ad = "Yanlış";
                sablon_secenekler2.soru_uid = sablon_sorulari.soru_uid;
                sablon_secenekler2.soru_secenek_sira_no = Convert.ToInt32(ankDB.SablonSoruTipi5SecenekMaxSiraNo(sablon_sorulari.soru_uid));
                ankDB.Kaydet();
            }
            else if (soru_tipi == "6")
            {
                sbr_sablon_sorulari sablon_sorulari = new sbr_sablon_sorulari();
                ankDB.SablonSoruEkle(sablon_sorulari);
                sablon_sorulari.sablon_uid = sablon_uid;
                sablon_sorulari.soru = soru;
                sablon_sorulari.soru_tipi_id = Convert.ToInt32(soru_tipi);
                sablon_sorulari.soru_olusturan_kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                sablon_sorulari.soru_olusturma_tarihi = DateTime.Now;
                sablon_sorulari.soru_sira_no = Convert.ToInt32(ankDB.SablonSoruMaxSiraNo(sablon_uid));

                ankDB.Kaydet();

                sbr_sablon_soru_tipi_6_secenekleri sablon_secenekler = new sbr_sablon_soru_tipi_6_secenekleri();
                ankDB.SablonSoruTipi6SecenekEkle(sablon_secenekler);
                sablon_secenekler.soru_secenek_ad = "Evet";
                sablon_secenekler.soru_uid = sablon_sorulari.soru_uid;
                sablon_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.SablonSoruTipi6SecenekMaxSiraNo(sablon_sorulari.soru_uid));
                ankDB.Kaydet();

                sbr_sablon_soru_tipi_6_secenekleri sablon_secenekler2 = new sbr_sablon_soru_tipi_6_secenekleri();
                ankDB.SablonSoruTipi6SecenekEkle(sablon_secenekler2);
                sablon_secenekler2.soru_secenek_ad = "Hayır";
                sablon_secenekler2.soru_uid = sablon_sorulari.soru_uid;
                sablon_secenekler2.soru_secenek_sira_no = Convert.ToInt32(ankDB.SablonSoruTipi6SecenekMaxSiraNo(sablon_sorulari.soru_uid));
                ankDB.Kaydet();
            }
            else if (soru_tipi == "7")
            {
                sbr_sablon_sorulari sablon_sorulari = new sbr_sablon_sorulari();
                ankDB.SablonSoruEkle(sablon_sorulari);
                sablon_sorulari.sablon_uid = sablon_uid;
                sablon_sorulari.soru = soru;
                sablon_sorulari.soru_tipi_id = Convert.ToInt32(soru_tipi);
                //sablon_sorulari.soru_siralama_sekli_id = Convert.ToInt32(siralama_sekli);
                sablon_sorulari.soru_olusturan_kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                sablon_sorulari.soru_olusturma_tarihi = DateTime.Now;
                sablon_sorulari.soru_sira_no = Convert.ToInt32(ankDB.SablonSoruMaxSiraNo(sablon_uid));

                ankDB.Kaydet();

                soru_secenekleri = soru_secenekleri.Replace("\r\n", "#");
                string[] soru_secenekleri_arr = soru_secenekleri.Split('#');

                foreach (string secenekler in soru_secenekleri_arr)
                {
                    if (secenekler.Trim() != "")
                    {
                        sbr_sablon_soru_tipi_7_secenekleri sablon_secenekler = new sbr_sablon_soru_tipi_7_secenekleri();
                        ankDB.SablonSoruTipi7SecenekEkle(sablon_secenekler);
                        sablon_secenekler.soru_secenek_ad = secenekler;
                        sablon_secenekler.soru_uid = sablon_sorulari.soru_uid;
                        sablon_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.SablonSoruTipi7SecenekMaxSiraNo(sablon_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }
                }
            }
            else if (soru_tipi == "8")
            {
                sbr_sablon_sorulari sablon_sorulari = new sbr_sablon_sorulari();
                ankDB.SablonSoruEkle(sablon_sorulari);
                sablon_sorulari.sablon_uid = sablon_uid;
                sablon_sorulari.soru = soru;
                sablon_sorulari.soru_tipi_id = Convert.ToInt32(soru_tipi);
                sablon_sorulari.soru_olusturan_kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                sablon_sorulari.soru_olusturma_tarihi = DateTime.Now;
                sablon_sorulari.soru_sira_no = Convert.ToInt32(ankDB.SablonSoruMaxSiraNo(sablon_uid));

                if (soru_sayisal_ondalik != null && (soru_sayisal_ondalik == "true" || soru_sayisal_ondalik == "True" || soru_sayisal_ondalik == "TRUE"))
                    sablon_sorulari.soru_sayisal_ondalik = true;
                else
                    sablon_sorulari.soru_sayisal_ondalik = false;

                ankDB.Kaydet();

                sbr_sablon_soru_tipi_8_secenekleri sablon_secenekler = new sbr_sablon_soru_tipi_8_secenekleri();
                ankDB.SablonSoruTipi8SecenekEkle(sablon_secenekler);
                sablon_secenekler.soru_secenek_ad = soru;
                sablon_secenekler.soru_uid = sablon_sorulari.soru_uid;
                sablon_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.SablonSoruTipi8SecenekMaxSiraNo(sablon_sorulari.soru_uid));
                ankDB.Kaydet();
            }
            else if (soru_tipi == "9")
            {
                sbr_sablon_sorulari sablon_sorulari = new sbr_sablon_sorulari();
                ankDB.SablonSoruEkle(sablon_sorulari);
                sablon_sorulari.sablon_uid = sablon_uid;
                sablon_sorulari.soru = soru;
                sablon_sorulari.soru_tipi_id = Convert.ToInt32(soru_tipi);
                sablon_sorulari.soru_olusturan_kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                sablon_sorulari.soru_olusturma_tarihi = DateTime.Now;
                sablon_sorulari.soru_sira_no = Convert.ToInt32(ankDB.SablonSoruMaxSiraNo(sablon_uid));


                ankDB.Kaydet();

                sbr_sablon_soru_tipi_9_secenekleri sablon_secenekler = new sbr_sablon_soru_tipi_9_secenekleri();
                ankDB.SablonSoruTipi9SecenekEkle(sablon_secenekler);
                sablon_secenekler.soru_secenek_ad = soru;
                sablon_secenekler.soru_uid = sablon_sorulari.soru_uid;
                sablon_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.SablonSoruTipi8SecenekMaxSiraNo(sablon_sorulari.soru_uid));
                ankDB.Kaydet();
            }
            else if (soru_tipi == "10")
            {
                sbr_sablon_sorulari sablon_sorulari = new sbr_sablon_sorulari();
                ankDB.SablonSoruEkle(sablon_sorulari);
                sablon_sorulari.sablon_uid = sablon_uid;
                sablon_sorulari.soru = soru;
                sablon_sorulari.soru_tipi_id = Convert.ToInt32(soru_tipi);
                sablon_sorulari.soru_olusturan_kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                sablon_sorulari.soru_olusturma_tarihi = DateTime.Now;
                sablon_sorulari.soru_sira_no = Convert.ToInt32(ankDB.SablonSoruMaxSiraNo(sablon_uid));

                ankDB.Kaydet();

                sbr_sablon_soru_tipi_10_secenekleri sablon_secenekler = new sbr_sablon_soru_tipi_10_secenekleri();
                ankDB.SablonSoruTipi10SecenekEkle(sablon_secenekler);
                sablon_secenekler.soru_secenek_ad = soru;
                sablon_secenekler.soru_uid = sablon_sorulari.soru_uid;
                sablon_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.SablonSoruTipi8SecenekMaxSiraNo(sablon_sorulari.soru_uid));
                ankDB.Kaydet();
            }
            else if (soru_tipi == "11")
            {
                sbr_sablon_sorulari sablon_sorulari = new sbr_sablon_sorulari();
                ankDB.SablonSoruEkle(sablon_sorulari);
                sablon_sorulari.sablon_uid = sablon_uid;
                sablon_sorulari.soru = soru;
                sablon_sorulari.soru_tipi_id = Convert.ToInt32(soru_tipi);
                sablon_sorulari.soru_olusturan_kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                sablon_sorulari.soru_olusturma_tarihi = DateTime.Now;
                sablon_sorulari.soru_sira_no = Convert.ToInt32(ankDB.SablonSoruMaxSiraNo(sablon_uid));

                ankDB.Kaydet();

                sbr_sablon_soru_tipi_11_secenekleri sablon_secenekler = new sbr_sablon_soru_tipi_11_secenekleri();
                ankDB.SablonSoruTipi11SecenekEkle(sablon_secenekler);
                sablon_secenekler.soru_secenek_ad = soru;
                sablon_secenekler.soru_uid = sablon_sorulari.soru_uid;
                sablon_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.SablonSoruTipi8SecenekMaxSiraNo(sablon_sorulari.soru_uid));
                ankDB.Kaydet();
            }
        }

        public void SablonSoruUpdate(Guid soru_uid, string soru_tipi, string soru, string soru_secenekleri, string cevap_kolonları, string soru_sayisal_ondalik,string tek_satir)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            if (soru_tipi == "1")
            {
                sbr_sablon_sorulari sablon_sorulari = this.SablonSoruGetir(soru_uid);
                sablon_sorulari.soru = soru;
                ankDB.Kaydet();

                soru_secenekleri = soru_secenekleri.Replace("\r\n", "#");
                string[] soru_secenekleri_arr = soru_secenekleri.Split('#');
                this.SablonSoruTipi1SecenekSilSoruyaGore(soru_uid);

                foreach (string secenekler in soru_secenekleri_arr)
                {
                    if (secenekler.Trim() != "")
                    {
                        sbr_sablon_soru_tipi_1_secenekleri sablon_secenekler = new sbr_sablon_soru_tipi_1_secenekleri();
                        ankDB.SablonSoruTipi1SecenekEkle(sablon_secenekler);
                        sablon_secenekler.soru_secenek_ad = secenekler;
                        sablon_secenekler.soru_uid = sablon_sorulari.soru_uid;
                        sablon_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.SablonSoruTipi1SecenekMaxSiraNo(sablon_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }
                }
            }
            else if (soru_tipi == "2")
            {
                sbr_sablon_sorulari sablon_sorulari = this.SablonSoruGetir(soru_uid);
                sablon_sorulari.soru = soru;
                ankDB.Kaydet();

                this.SablonSoruTipi2SecenekSilSoruyaGore(soru_uid);
                soru_secenekleri = soru_secenekleri.Replace("\r\n", "#");
                string[] soru_secenekleri_arr = soru_secenekleri.Split('#');

                foreach (string secenekler in soru_secenekleri_arr)
                {
                    if (secenekler.Trim() != "")
                    {
                        sbr_sablon_soru_tipi_2_secenekleri sablon_secenekler = new sbr_sablon_soru_tipi_2_secenekleri();
                        ankDB.SablonSoruTipi2SecenekEkle(sablon_secenekler);
                        sablon_secenekler.soru_secenek_ad = secenekler;
                        sablon_secenekler.soru_uid = sablon_sorulari.soru_uid;
                        sablon_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.SablonSoruTipi2SecenekMaxSiraNo(sablon_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }
                }
            }
            else if (soru_tipi == "3")
            {
                sbr_sablon_sorulari sablon_sorulari = this.SablonSoruGetir(soru_uid);
                sablon_sorulari.soru = soru;
                ankDB.Kaydet();


                soru_secenekleri = soru_secenekleri.Replace("\r\n", "#");
                string[] soru_secenekleri_arr = soru_secenekleri.Split('#');

                this.SablonSoruTipi3SecenekSilSoruyaGore(soru_uid);

                foreach (string secenekler in soru_secenekleri_arr)
                {
                    if (secenekler.Trim() != "")
                    {
                        sbr_sablon_soru_tipi_3_secenekleri sablon_secenekler = new sbr_sablon_soru_tipi_3_secenekleri();
                        ankDB.SablonSoruTipi3SecenekEkle(sablon_secenekler);
                        sablon_secenekler.soru_secenek_ad = secenekler;
                        sablon_secenekler.soru_uid = sablon_sorulari.soru_uid;
                        sablon_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.SablonSoruTipi3SecenekMaxSiraNo(sablon_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }
                }

                cevap_kolonları = cevap_kolonları.Replace("\r\n", "#");
                string[] cevap_kolonları_arr = cevap_kolonları.Split('#');

                this.SablonSoruTipi3SecenekKolonSilSoruyaGore(soru_uid);

                foreach (string kolonlar in cevap_kolonları_arr)
                {
                    if (kolonlar.Trim() != "")
                    {
                        sbr_sablon_soru_tipi_3_secenek_kolonlari sablon_secenek_kolonlari = new sbr_sablon_soru_tipi_3_secenek_kolonlari();
                        ankDB.SablonSoruTipi3SecenekKolonEkle(sablon_secenek_kolonlari);
                        sablon_secenek_kolonlari.soru_secenek_kolon_ad = kolonlar;
                        sablon_secenek_kolonlari.soru_uid = sablon_sorulari.soru_uid;
                        sablon_secenek_kolonlari.soru_secenek_kolon_sira_no = Convert.ToInt32(ankDB.SablonSoruTipi3SecenekKolonMaxSiraNo(sablon_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }
                }
            }
            else if (soru_tipi == "4")
            {
                sbr_sablon_sorulari sablon_sorulari = this.SablonSoruGetir(soru_uid);
                sablon_sorulari.soru = soru;


                if (tek_satir != null && (tek_satir == "true" || tek_satir == "True" || tek_satir == "TRUE"))
                    sablon_sorulari.soru_tek_satir = true;
                else
                    sablon_sorulari.soru_tek_satir = false;

                ankDB.Kaydet();

                this.SablonSoruTipi4SecenekSilSoruyaGore(soru_uid);

                sbr_sablon_soru_tipi_4_secenekleri sablon_secenekler = new sbr_sablon_soru_tipi_4_secenekleri();
                ankDB.SablonSoruTipi4SecenekEkle(sablon_secenekler);
                sablon_secenekler.soru_secenek_ad = soru;
                sablon_secenekler.soru_uid = sablon_sorulari.soru_uid;
                sablon_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.SablonSoruTipi4SecenekMaxSiraNo(sablon_sorulari.soru_uid));
                ankDB.Kaydet();
            }
            else if (soru_tipi == "5")
            {
                sbr_sablon_sorulari sablon_sorulari = this.SablonSoruGetir(soru_uid);
                sablon_sorulari.soru = soru;
                ankDB.Kaydet();

                this.SablonSoruTipi5SecenekSilSoruyaGore(soru_uid);

                sbr_sablon_soru_tipi_5_secenekleri sablon_secenekler = new sbr_sablon_soru_tipi_5_secenekleri();
                ankDB.SablonSoruTipi5SecenekEkle(sablon_secenekler);
                sablon_secenekler.soru_secenek_ad = "Doğru";
                sablon_secenekler.soru_uid = sablon_sorulari.soru_uid;
                sablon_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.SablonSoruTipi5SecenekMaxSiraNo(sablon_sorulari.soru_uid));
                ankDB.Kaydet();

                sbr_sablon_soru_tipi_5_secenekleri sablon_secenekler2 = new sbr_sablon_soru_tipi_5_secenekleri();
                ankDB.SablonSoruTipi5SecenekEkle(sablon_secenekler2);
                sablon_secenekler2.soru_secenek_ad = "Yanlış";
                sablon_secenekler2.soru_uid = sablon_sorulari.soru_uid;
                sablon_secenekler2.soru_secenek_sira_no = Convert.ToInt32(ankDB.SablonSoruTipi5SecenekMaxSiraNo(sablon_sorulari.soru_uid));
                ankDB.Kaydet();
            }
            else if (soru_tipi == "6")
            {
                sbr_sablon_sorulari sablon_sorulari = this.SablonSoruGetir(soru_uid);
                sablon_sorulari.soru = soru;
                ankDB.Kaydet();

                this.SablonSoruTipi6SecenekSilSoruyaGore(soru_uid);

                sbr_sablon_soru_tipi_6_secenekleri sablon_secenekler = new sbr_sablon_soru_tipi_6_secenekleri();
                ankDB.SablonSoruTipi6SecenekEkle(sablon_secenekler);
                sablon_secenekler.soru_secenek_ad = "Evet";
                sablon_secenekler.soru_uid = sablon_sorulari.soru_uid;
                sablon_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.SablonSoruTipi6SecenekMaxSiraNo(sablon_sorulari.soru_uid));
                ankDB.Kaydet();

                sbr_sablon_soru_tipi_6_secenekleri sablon_secenekler2 = new sbr_sablon_soru_tipi_6_secenekleri();
                ankDB.SablonSoruTipi6SecenekEkle(sablon_secenekler2);
                sablon_secenekler2.soru_secenek_ad = "Hayır";
                sablon_secenekler2.soru_uid = sablon_sorulari.soru_uid;
                sablon_secenekler2.soru_secenek_sira_no = Convert.ToInt32(ankDB.SablonSoruTipi6SecenekMaxSiraNo(sablon_sorulari.soru_uid));
                ankDB.Kaydet();
            }
            else if (soru_tipi == "7")
            {
                sbr_sablon_sorulari sablon_sorulari = this.SablonSoruGetir(soru_uid);
                sablon_sorulari.soru = soru;
                ankDB.Kaydet();

                this.SablonSoruTipi7SecenekSilSoruyaGore(soru_uid);
                soru_secenekleri = soru_secenekleri.Replace("\r\n", "#");
                string[] soru_secenekleri_arr = soru_secenekleri.Split('#');

                foreach (string secenekler in soru_secenekleri_arr)
                {
                    if (secenekler.Trim() != "")
                    {
                        sbr_sablon_soru_tipi_7_secenekleri sablon_secenekler = new sbr_sablon_soru_tipi_7_secenekleri();
                        ankDB.SablonSoruTipi7SecenekEkle(sablon_secenekler);
                        sablon_secenekler.soru_secenek_ad = secenekler;
                        sablon_secenekler.soru_uid = sablon_sorulari.soru_uid;
                        sablon_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.SablonSoruTipi7SecenekMaxSiraNo(sablon_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }
                }
            }
            else if (soru_tipi == "8")
            {
                sbr_sablon_sorulari sablon_sorulari = this.SablonSoruGetir(soru_uid);
                sablon_sorulari.soru = soru;

                if (soru_sayisal_ondalik != null && (soru_sayisal_ondalik == "true" || soru_sayisal_ondalik == "True" || soru_sayisal_ondalik == "TRUE"))
                    sablon_sorulari.soru_sayisal_ondalik = true;
                else
                    sablon_sorulari.soru_sayisal_ondalik = false;

                ankDB.Kaydet();

                this.SablonSoruTipi8SecenekSilSoruyaGore(soru_uid);

                sbr_sablon_soru_tipi_8_secenekleri sablon_secenekler = new sbr_sablon_soru_tipi_8_secenekleri();
                ankDB.SablonSoruTipi8SecenekEkle(sablon_secenekler);
                sablon_secenekler.soru_secenek_ad = soru;
                sablon_secenekler.soru_uid = sablon_sorulari.soru_uid;
                sablon_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.SablonSoruTipi8SecenekMaxSiraNo(sablon_sorulari.soru_uid));
                ankDB.Kaydet();
            }
            else if (soru_tipi == "9")
            {
                sbr_sablon_sorulari sablon_sorulari = this.SablonSoruGetir(soru_uid);
                sablon_sorulari.soru = soru;


                ankDB.Kaydet();

                this.SablonSoruTipi9SecenekSilSoruyaGore(soru_uid);

                sbr_sablon_soru_tipi_9_secenekleri sablon_secenekler = new sbr_sablon_soru_tipi_9_secenekleri();
                ankDB.SablonSoruTipi9SecenekEkle(sablon_secenekler);
                sablon_secenekler.soru_secenek_ad = soru;
                sablon_secenekler.soru_uid = sablon_sorulari.soru_uid;
                sablon_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.SablonSoruTipi9SecenekMaxSiraNo(sablon_sorulari.soru_uid));
                ankDB.Kaydet();
            }
            else if (soru_tipi == "10")
            {
                sbr_sablon_sorulari sablon_sorulari = this.SablonSoruGetir(soru_uid);
                sablon_sorulari.soru = soru;


                ankDB.Kaydet();

                this.SablonSoruTipi10SecenekSilSoruyaGore(soru_uid);

                sbr_sablon_soru_tipi_10_secenekleri sablon_secenekler = new sbr_sablon_soru_tipi_10_secenekleri();
                ankDB.SablonSoruTipi10SecenekEkle(sablon_secenekler);
                sablon_secenekler.soru_secenek_ad = soru;
                sablon_secenekler.soru_uid = sablon_sorulari.soru_uid;
                sablon_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.SablonSoruTipi10SecenekMaxSiraNo(sablon_sorulari.soru_uid));
                ankDB.Kaydet();
            }
            else if (soru_tipi == "11")
            {
                sbr_sablon_sorulari sablon_sorulari = this.SablonSoruGetir(soru_uid);
                sablon_sorulari.soru = soru;


                ankDB.Kaydet();

                this.SablonSoruTipi11SecenekSilSoruyaGore(soru_uid);

                sbr_sablon_soru_tipi_11_secenekleri sablon_secenekler = new sbr_sablon_soru_tipi_11_secenekleri();
                ankDB.SablonSoruTipi11SecenekEkle(sablon_secenekler);
                sablon_secenekler.soru_secenek_ad = soru;
                sablon_secenekler.soru_uid = sablon_sorulari.soru_uid;
                sablon_secenekler.soru_secenek_sira_no = Convert.ToInt32(ankDB.SablonSoruTipi11SecenekMaxSiraNo(sablon_sorulari.soru_uid));
                ankDB.Kaydet();
            }
        }

        public string SablonSoruGorunumuOlustur(object soru_uid)
        {
            string result = "";
            Guid soru_id = Guid.Parse(soru_uid.ToString());

            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            sbr_sablon_sorulari anket_soru = ankDB.SablonSoruGetir(soru_id);


            if (anket_soru.soru_tipi_id == 1)
            {
                DataSet ds_soru_tipi1_secenekler = ankDB.SablonSoruTipi1SecenekGetirSoruyaGoreDataSet(soru_id);
                int index = 0;
                result = "<table>";
                foreach (DataRow dr in ds_soru_tipi1_secenekler.Tables[0].Rows)
                {

                    result += "<tr><td><label class=\"withRadio\"><input id=\"secenek_tip1_" + dr["soru_secenek_uid"].ToString() + "\" type=\"radio\" name=\"grup_" + dr["soru_uid"].ToString() + "\" runat=\"server\" />" + dr["soru_secenek_ad"].ToString() + "</label></td></tr>";
                    index++;
                }
                result += "</table>";

                if (ds_soru_tipi1_secenekler.Tables[0].Rows.Count <= 0) result = "";
            }
            else if (anket_soru.soru_tipi_id == 2)
            {
                DataSet ds_soru_tipi2_secenekler = ankDB.SablonSoruTipi2SecenekGetirSoruyaGoreDataSet(soru_id);
                int index = 0;
                result = "<table>";
                foreach (DataRow dr in ds_soru_tipi2_secenekler.Tables[0].Rows)
                {
                    result += "<tr><td><label class=\"withCheckbox\"><input id=\"secenek_tip2_" + dr["soru_secenek_uid"].ToString() + "\" type=\"checkbox\" runat=\"server\" />" + dr["soru_secenek_ad"].ToString() + "</label></td></tr>";
                    index++;
                }
                result += "</table>";

                if (ds_soru_tipi2_secenekler.Tables[0].Rows.Count <= 0) result = "";
            }
            else if (anket_soru.soru_tipi_id == 3)
            {
                DataSet ds_soru_tipi3_secenekler = ankDB.SablonSoruTipi3SecenekGetirSoruyaGoreDataSet(soru_id);
                DataSet ds_soru_tipi3_kolonlar = ankDB.SablonSoruTipi3SecenekKolonGetirSoruyaGoreDataSet(soru_id);
                int index = 0;
                result = "<table>";
                foreach (DataRow dr in ds_soru_tipi3_secenekler.Tables[0].Rows)
                {
                    result += "<tr><td width=\"250px\">" + dr["soru_secenek_ad"].ToString() + "</td>";
                    string kolonlar = "";
                    foreach (DataRow dr2 in ds_soru_tipi3_kolonlar.Tables[0].Rows)
                    {
                        kolonlar += "<td>" + "<label class=\"withRadio\"><input id=\"secenek_tip3_" + dr["soru_secenek_uid"].ToString() + "_" + dr2["soru_secenek_kolon_uid"].ToString() + "\" type=\"radio\" name=\"grup_" + dr["soru_secenek_uid"].ToString() + "\" runat=\"server\" />" + dr2["soru_secenek_kolon_ad"].ToString() + "</td><td width=\"15px\"></td>";
                    }
                    result += kolonlar + "</tr>";

                    index++;
                }
                result += "</table>";

            }
            else if (anket_soru.soru_tipi_id == 4)
            {
                DataSet ds_soru_tipi4_secenekler = ankDB.SablonSoruTipi4SecenekGetirSoruyaGoreDataSet(soru_id);
                int index = 0;

                if (anket_soru.soru_tek_satir != null && anket_soru.soru_tek_satir == true)
                {
                    result = "<table>";
                    foreach (DataRow dr in ds_soru_tipi4_secenekler.Tables[0].Rows)
                    {
                        result += "<tr><td><input id=\"secenek_tip4_" + dr["soru_secenek_uid"].ToString() + "\"  runat=\"server\"  type=\"text\" /></td></tr>";
                        index++;
                    }
                    result += "</table>";

                    if (ds_soru_tipi4_secenekler.Tables[0].Rows.Count <= 0) result = "";
                }
                else
                {
                    result = "<table>";
                    foreach (DataRow dr in ds_soru_tipi4_secenekler.Tables[0].Rows)
                    {
                        result += "<tr><td><textarea id=\"secenek_tip4_" + dr["soru_secenek_uid"].ToString() + "\" rows=\"3\" runat=\"server\"  cols=\"80\"></textarea></td></tr>";
                        index++;
                    }
                    result += "</table>";

                    if (ds_soru_tipi4_secenekler.Tables[0].Rows.Count <= 0) result = "";
                }
            }
            else if (anket_soru.soru_tipi_id == 7)
            {
                DataSet ds_soru_tipi7_secenekler = ankDB.SablonSoruTipi7SecenekGetirSoruyaGoreDataSet(soru_id);
                int index = 0;

                result = "<table>";

                foreach (DataRow dr in ds_soru_tipi7_secenekler.Tables[0].Rows)
                {
                    result += "<tr><td><label>" + dr["soru_secenek_ad"].ToString() + "</label></td><td width=\"30px\"> : </td><td><input type=\"text\" id=\"secenek_tip7_" + dr["soru_secenek_uid"].ToString() + "\" runat=\"server\"  /></td></tr>";
                    index++;
                }

                result += "</table>";
                if (ds_soru_tipi7_secenekler.Tables[0].Rows.Count <= 0) result = "";
            }
            else if (anket_soru.soru_tipi_id == 5)
            {
                DataSet ds_soru_tipi5_secenekler = ankDB.SablonSoruTipi5SecenekGetirSoruyaGoreDataSet(soru_id);
                int index = 0;
                result = "<table>";
                foreach (DataRow dr in ds_soru_tipi5_secenekler.Tables[0].Rows)
                {
                    result += "<tr><td><label class=\"withRadio\"><input id=\"secenek_tip5_" + dr["soru_secenek_uid"].ToString() + "\" type=\"radio\" name=\"grup_" + dr["soru_uid"].ToString() + "\" runat=\"server\" />" + dr["soru_secenek_ad"].ToString() + "</label></td></tr>";
                    index++;
                }
                result += "</table>";

                if (ds_soru_tipi5_secenekler.Tables[0].Rows.Count <= 0) result = "";
            }
            else if (anket_soru.soru_tipi_id == 6)
            {
                DataSet ds_soru_tipi6_secenekler = ankDB.SablonSoruTipi6SecenekGetirSoruyaGoreDataSet(soru_id);
                int index = 0;
                result = "<table>";
                foreach (DataRow dr in ds_soru_tipi6_secenekler.Tables[0].Rows)
                {
                    result += "<tr><td><label class=\"withRadio\"><input id=\"secenek_tip6_" + dr["soru_secenek_uid"].ToString() + "\" type=\"radio\" name=\"grup_" + dr["soru_uid"].ToString() + "\" runat=\"server\" />" + dr["soru_secenek_ad"].ToString() + "</label></td></tr>";
                    index++;
                }
                result += "</table>";

                if (ds_soru_tipi6_secenekler.Tables[0].Rows.Count <= 0) result = "";
            }
            else if (anket_soru.soru_tipi_id == 8)
            {
                DataSet ds_soru_tipi8_secenekler = ankDB.SablonSoruTipi8SecenekGetirSoruyaGoreDataSet(soru_id);
                int index = 0;
                result = "<table>";
                foreach (DataRow dr in ds_soru_tipi8_secenekler.Tables[0].Rows)
                {
                    string class_tip = "";
                    if (anket_soru.soru_sayisal_ondalik != null && anket_soru.soru_sayisal_ondalik == true)
                        class_tip = "numeric";
                    else
                        class_tip = "integer";

                    result += "<tr><td><input class=\"" + class_tip + "\" type=\"text\"  id=\"secenek_tip8_" + dr["soru_secenek_uid"].ToString() + "\"  runat=\"server\" style=\"text-align: right\"   /></td></tr>";

                    index++;
                }

                result += "</table>";

                if (ds_soru_tipi8_secenekler.Tables[0].Rows.Count <= 0) result = "";
            }
            else if (anket_soru.soru_tipi_id == 9)
            {
                DataSet ds_soru_tipi9_secenekler = ankDB.SablonSoruTipi9SecenekGetirSoruyaGoreDataSet(soru_id);
                int index = 0;
                result = "<table>";
                foreach (DataRow dr in ds_soru_tipi9_secenekler.Tables[0].Rows)
                {
                    string class_tip = "datepicker";
                    result += "<tr><td><input class=\"" + class_tip + "\" type=\"text\"  id=\"secenek_tip9_" + dr["soru_secenek_uid"].ToString() + "\"  runat=\"server\"  readonly=\"readonly\" /></td></tr>";
                    index++;
                }
                result += "</table>";

                if (ds_soru_tipi9_secenekler.Tables[0].Rows.Count <= 0) result = "";
            }
            else if (anket_soru.soru_tipi_id == 10)
            {
                DataSet ds_soru_tipi10_secenekler = ankDB.SablonSoruTipi10SecenekGetirSoruyaGoreDataSet(soru_id);
                int index = 0;
                result = "<table>";
                foreach (DataRow dr in ds_soru_tipi10_secenekler.Tables[0].Rows)
                {
                    string class_tip = "phone";
                    result += "<tr><td><input class=\"" + class_tip + "\" type=\"text\"  id=\"secenek_tip10_" + dr["soru_secenek_uid"].ToString() + "\"  runat=\"server\"  /></td></tr>";

                    index++;
                }

                result += "</table>";

                if (ds_soru_tipi10_secenekler.Tables[0].Rows.Count <= 0) result = "";
            }
            else if (anket_soru.soru_tipi_id == 11)
            {
                DataSet ds_soru_tipi11_secenekler = ankDB.SablonSoruTipi11SecenekGetirSoruyaGoreDataSet(soru_id);
                int index = 0;
                result = "<table>";
                foreach (DataRow dr in ds_soru_tipi11_secenekler.Tables[0].Rows)
                {
                    string class_tip = "eposta";
                    result += "<tr><td><input class=\"" + class_tip + "\" type=\"text\"  id=\"secenek_tip11_" + dr["soru_secenek_uid"].ToString() + "\"  runat=\"server\"  /></td></tr>";

                    index++;
                }

                result += "</table>";

                if (ds_soru_tipi11_secenekler.Tables[0].Rows.Count <= 0) result = "";
            }
            return result;
        }
        #endregion

        #region Sablon Soru Tipi 1 Optionsi
        public IQueryable<sbr_sablon_soru_tipi_1_secenekleri> TumSablonSoruTipi1Secenekleri()
        {
            return db.sbr_sablon_soru_tipi_1_secenekleri;
        }
        public sbr_sablon_soru_tipi_1_secenekleri SablonSoruTipi1SecenekGetir(Guid soru_secenek_uid)
        {
            return db.sbr_sablon_soru_tipi_1_secenekleri.SingleOrDefault(d => d.soru_secenek_uid == soru_secenek_uid);
        }
        public void SablonSoruTipi1SecenekEkle(sbr_sablon_soru_tipi_1_secenekleri sablon_soru_tipi1_secenek)
        {
            sablon_soru_tipi1_secenek.soru_secenek_uid = Guid.NewGuid();
            db.sbr_sablon_soru_tipi_1_secenekleri.AddObject(sablon_soru_tipi1_secenek);
        }
        public void SablonSoruTipi1SecenekSil(sbr_sablon_soru_tipi_1_secenekleri sablon_soru_tipi1_secenekleri)
        {
            db.sbr_sablon_soru_tipi_1_secenekleri.DeleteObject(sablon_soru_tipi1_secenekleri);
        }

        public void SablonSoruTipi1SecenekSil(Guid sablon_soru_tipi1_secenek_uid)
        {
            this.SablonSoruTipi1SecenekSil(this.SablonSoruTipi1SecenekGetir(sablon_soru_tipi1_secenek_uid));
        }

        public void SablonSoruTipi1SecenekSilSoruyaGore(Guid soru_uid)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_sablon_soru_tipi_1_secenekleri where soru_uid='" + soru_uid + "'");
        }

        public DataSet SablonSoruTipi1SecenekGetirSoruyaGoreDataSet(Guid soru_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_sablon_soru_tipi_1_secenekleri where soru_uid='" + soru_uid + "' order by soru_secenek_sira_no");

            return ds_result;
        }

        public string SablonSoruTipi1SecenekMaxSiraNo(Guid soru_uid)
        {

            string max = BaseDB.DBManager.AppConnection.ExecuteSql("select max(soru_secenek_sira_no)+1 from sbr_sablon_soru_tipi_1_secenekleri where soru_uid='" + soru_uid + "' ");

            if (max == "")
                max = "1";

            return max;
        }
        #endregion

        #region Sablon Soru Tipi 2 Optionsi
        public IQueryable<sbr_sablon_soru_tipi_2_secenekleri> TumSablonSoruTipi2Secenekleri()
        {
            return db.sbr_sablon_soru_tipi_2_secenekleri;
        }
        public sbr_sablon_soru_tipi_2_secenekleri SablonSoruTipi2SecenekGetir(Guid soru_secenek_uid)
        {
            return db.sbr_sablon_soru_tipi_2_secenekleri.SingleOrDefault(d => d.soru_secenek_uid == soru_secenek_uid);
        }
        public void SablonSoruTipi2SecenekEkle(sbr_sablon_soru_tipi_2_secenekleri sablon_soru_tipi2_secenek)
        {
            sablon_soru_tipi2_secenek.soru_secenek_uid = Guid.NewGuid();
            db.sbr_sablon_soru_tipi_2_secenekleri.AddObject(sablon_soru_tipi2_secenek);
        }
        public void SablonSoruTipi2SecenekSil(sbr_sablon_soru_tipi_2_secenekleri sablon_soru_tipi2_secenekleri)
        {
            db.sbr_sablon_soru_tipi_2_secenekleri.DeleteObject(sablon_soru_tipi2_secenekleri);
        }

        public void SablonSoruTipi2SecenekSil(Guid sablon_soru_tipi2_secenek_uid)
        {
            this.SablonSoruTipi2SecenekSil(this.SablonSoruTipi2SecenekGetir(sablon_soru_tipi2_secenek_uid));
        }


        public DataSet SablonSoruTipi2SecenekGetirSoruyaGoreDataSet(Guid soru_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_sablon_soru_tipi_2_secenekleri where soru_uid='" + soru_uid + "' order by soru_secenek_sira_no");

            return ds_result;
        }

        public string SablonSoruTipi2SecenekMaxSiraNo(Guid soru_uid)
        {

            string max = BaseDB.DBManager.AppConnection.ExecuteSql("select max(soru_secenek_sira_no)+1 from sbr_sablon_soru_tipi_2_secenekleri where soru_uid='" + soru_uid + "' ");

            if (max == "")
                max = "1";

            return max;
        }

        public void SablonSoruTipi2SecenekSilSoruyaGore(Guid soru_uid)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_sablon_soru_tipi_2_secenekleri where soru_uid='" + soru_uid + "'");
        }

        #endregion

        #region Sablon Soru Tipi 3 Optionsi
        public IQueryable<sbr_sablon_soru_tipi_3_secenekleri> TumSablonSoruTipi3Secenekleri()
        {
            return db.sbr_sablon_soru_tipi_3_secenekleri;
        }
        public sbr_sablon_soru_tipi_3_secenekleri SablonSoruTipi3SecenekGetir(Guid soru_secenek_uid)
        {
            return db.sbr_sablon_soru_tipi_3_secenekleri.SingleOrDefault(d => d.soru_secenek_uid == soru_secenek_uid);
        }
        public void SablonSoruTipi3SecenekEkle(sbr_sablon_soru_tipi_3_secenekleri sablon_soru_tipi3_secenek)
        {
            sablon_soru_tipi3_secenek.soru_secenek_uid = Guid.NewGuid();
            db.sbr_sablon_soru_tipi_3_secenekleri.AddObject(sablon_soru_tipi3_secenek);
        }
        public void SablonSoruTipi3SecenekSil(sbr_sablon_soru_tipi_3_secenekleri sablon_soru_tipi3_secenekleri)
        {
            db.sbr_sablon_soru_tipi_3_secenekleri.DeleteObject(sablon_soru_tipi3_secenekleri);
        }

        public void SablonSoruTipi3SecenekSil(Guid sablon_soru_tipi3_secenek_uid)
        {
            this.SablonSoruTipi3SecenekSil(this.SablonSoruTipi3SecenekGetir(sablon_soru_tipi3_secenek_uid));
        }


        public DataSet SablonSoruTipi3SecenekGetirSoruyaGoreDataSet(Guid soru_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_sablon_soru_tipi_3_secenekleri where soru_uid='" + soru_uid + "' order by soru_secenek_sira_no");

            return ds_result;
        }

        public string SablonSoruTipi3SecenekMaxSiraNo(Guid soru_uid)
        {

            string max = BaseDB.DBManager.AppConnection.ExecuteSql("select max(soru_secenek_sira_no)+1 from sbr_sablon_soru_tipi_3_secenekleri where soru_uid='" + soru_uid + "' ");

            if (max == "")
                max = "1";

            return max;
        }

        public void SablonSoruTipi3SecenekSilSoruyaGore(Guid soru_uid)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_sablon_soru_tipi_3_secenekleri where soru_uid='" + soru_uid + "'");
        }

        #endregion

        #region Sablon Soru Tipi 3 Optionsi Kolonlari
        public IQueryable<sbr_sablon_soru_tipi_3_secenek_kolonlari> TumSablonSoruTipi3SecenekKolonlari()
        {
            return db.sbr_sablon_soru_tipi_3_secenek_kolonlari;
        }
        public sbr_sablon_soru_tipi_3_secenek_kolonlari SablonSoruTipi3SecenekKolonlariGetir(Guid soru_secenek_kolon_uid)
        {
            return db.sbr_sablon_soru_tipi_3_secenek_kolonlari.SingleOrDefault(d => d.soru_secenek_kolon_uid == soru_secenek_kolon_uid);
        }
        public void SablonSoruTipi3SecenekKolonEkle(sbr_sablon_soru_tipi_3_secenek_kolonlari sablon_soru_tipi3_secenek_kolon)
        {
            sablon_soru_tipi3_secenek_kolon.soru_secenek_kolon_uid = Guid.NewGuid();
            db.sbr_sablon_soru_tipi_3_secenek_kolonlari.AddObject(sablon_soru_tipi3_secenek_kolon);
        }
        public void SablonSoruTipi3SecenekKolonSil(sbr_sablon_soru_tipi_3_secenek_kolonlari sablon_soru_tipi3_secenek_kolonlari)
        {
            db.sbr_sablon_soru_tipi_3_secenek_kolonlari.DeleteObject(sablon_soru_tipi3_secenek_kolonlari);
        }

        public void SablonSoruTipi3SecenekKolonSil(Guid sablon_soru_tipi3_secenek_kolon_uid)
        {
            this.SablonSoruTipi3SecenekKolonSil(this.SablonSoruTipi3SecenekKolonlariGetir(sablon_soru_tipi3_secenek_kolon_uid));
        }


        public DataSet SablonSoruTipi3SecenekKolonGetirSoruyaGoreDataSet(Guid soru_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_sablon_soru_tipi_3_secenek_kolonlari where soru_uid='" + soru_uid + "' order by soru_secenek_kolon_sira_no");

            return ds_result;
        }

        public string SablonSoruTipi3SecenekKolonMaxSiraNo(Guid soru_uid)
        {

            string max = BaseDB.DBManager.AppConnection.ExecuteSql("select max(soru_secenek_kolon_sira_no)+1 from sbr_sablon_soru_tipi_3_secenek_kolonlari where soru_uid='" + soru_uid + "' ");

            if (max == "")
                max = "1";

            return max;
        }

        public void SablonSoruTipi3SecenekKolonSilSoruyaGore(Guid soru_uid)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_sablon_soru_tipi_3_secenek_kolonlari where soru_uid='" + soru_uid + "'");
        }
        #endregion

        #region Sablon Soru Tipi 4 Optionsi
        public IQueryable<sbr_sablon_soru_tipi_4_secenekleri> TumSablonSoruTipi4Secenekleri()
        {
            return db.sbr_sablon_soru_tipi_4_secenekleri;
        }
        public sbr_sablon_soru_tipi_4_secenekleri SablonSoruTipi4SecenekGetir(Guid soru_secenek_uid)
        {
            return db.sbr_sablon_soru_tipi_4_secenekleri.SingleOrDefault(d => d.soru_secenek_uid == soru_secenek_uid);
        }
        public void SablonSoruTipi4SecenekEkle(sbr_sablon_soru_tipi_4_secenekleri sablon_soru_tipi4_secenek)
        {
            sablon_soru_tipi4_secenek.soru_secenek_uid = Guid.NewGuid();
            db.sbr_sablon_soru_tipi_4_secenekleri.AddObject(sablon_soru_tipi4_secenek);
        }
        public void SablonSoruTipi4SecenekSil(sbr_sablon_soru_tipi_4_secenekleri sablon_soru_tipi4_secenekleri)
        {
            db.sbr_sablon_soru_tipi_4_secenekleri.DeleteObject(sablon_soru_tipi4_secenekleri);
        }

        public void SablonSoruTipi4SecenekSil(Guid sablon_soru_tipi4_secenek_uid)
        {
            this.SablonSoruTipi4SecenekSil(this.SablonSoruTipi4SecenekGetir(sablon_soru_tipi4_secenek_uid));
        }


        public DataSet SablonSoruTipi4SecenekGetirSoruyaGoreDataSet(Guid soru_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_sablon_soru_tipi_4_secenekleri where soru_uid='" + soru_uid + "' order by soru_secenek_sira_no");

            return ds_result;
        }

        public string SablonSoruTipi4SecenekMaxSiraNo(Guid soru_uid)
        {

            string max = BaseDB.DBManager.AppConnection.ExecuteSql("select max(soru_secenek_sira_no)+1 from sbr_sablon_soru_tipi_4_secenekleri where soru_uid='" + soru_uid + "' ");

            if (max == "")
                max = "1";

            return max;
        }

        public void SablonSoruTipi4SecenekSilSoruyaGore(Guid soru_uid)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_sablon_soru_tipi_4_secenekleri where soru_uid='" + soru_uid + "'");
        }
        #endregion

        #region Sablon Soru Tipi 5 Optionsi
        public IQueryable<sbr_sablon_soru_tipi_5_secenekleri> TumSablonSoruTipi5Secenekleri()
        {
            return db.sbr_sablon_soru_tipi_5_secenekleri;
        }
        public sbr_sablon_soru_tipi_5_secenekleri SablonSoruTipi5SecenekGetir(Guid soru_secenek_uid)
        {
            return db.sbr_sablon_soru_tipi_5_secenekleri.SingleOrDefault(d => d.soru_secenek_uid == soru_secenek_uid);
        }
        public void SablonSoruTipi5SecenekEkle(sbr_sablon_soru_tipi_5_secenekleri sablon_soru_tipi5_secenek)
        {
            sablon_soru_tipi5_secenek.soru_secenek_uid = Guid.NewGuid();
            db.sbr_sablon_soru_tipi_5_secenekleri.AddObject(sablon_soru_tipi5_secenek);
        }
        public void SablonSoruTipi5SecenekSil(sbr_sablon_soru_tipi_5_secenekleri sablon_soru_tipi5_secenekleri)
        {
            db.sbr_sablon_soru_tipi_5_secenekleri.DeleteObject(sablon_soru_tipi5_secenekleri);
        }

        public void SablonSoruTipi5SecenekSil(Guid sablon_soru_tipi5_secenek_uid)
        {
            this.SablonSoruTipi5SecenekSil(this.SablonSoruTipi5SecenekGetir(sablon_soru_tipi5_secenek_uid));
        }


        public DataSet SablonSoruTipi5SecenekGetirSoruyaGoreDataSet(Guid soru_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_sablon_soru_tipi_5_secenekleri where soru_uid='" + soru_uid + "' order by soru_secenek_sira_no");

            return ds_result;
        }

        public string SablonSoruTipi5SecenekMaxSiraNo(Guid soru_uid)
        {

            string max = BaseDB.DBManager.AppConnection.ExecuteSql("select max(soru_secenek_sira_no)+1 from sbr_sablon_soru_tipi_5_secenekleri where soru_uid='" + soru_uid + "' ");

            if (max == "")
                max = "1";

            return max;
        }

        public void SablonSoruTipi5SecenekSilSoruyaGore(Guid soru_uid)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_sablon_soru_tipi_5_secenekleri where soru_uid='" + soru_uid + "'");
        }
        #endregion

        #region Sablon Soru Tipi 6 Optionsi
        public IQueryable<sbr_sablon_soru_tipi_6_secenekleri> TumSablonSoruTipi6Secenekleri()
        {
            return db.sbr_sablon_soru_tipi_6_secenekleri;
        }
        public sbr_sablon_soru_tipi_6_secenekleri SablonSoruTipi6SecenekGetir(Guid soru_secenek_uid)
        {
            return db.sbr_sablon_soru_tipi_6_secenekleri.SingleOrDefault(d => d.soru_secenek_uid == soru_secenek_uid);
        }
        public void SablonSoruTipi6SecenekEkle(sbr_sablon_soru_tipi_6_secenekleri sablon_soru_tipi6_secenek)
        {
            sablon_soru_tipi6_secenek.soru_secenek_uid = Guid.NewGuid();
            db.sbr_sablon_soru_tipi_6_secenekleri.AddObject(sablon_soru_tipi6_secenek);
        }
        public void SablonSoruTipi6SecenekSil(sbr_sablon_soru_tipi_6_secenekleri sablon_soru_tipi6_secenekleri)
        {
            db.sbr_sablon_soru_tipi_6_secenekleri.DeleteObject(sablon_soru_tipi6_secenekleri);
        }

        public void SablonSoruTipi6SecenekSil(Guid sablon_soru_tipi6_secenek_uid)
        {
            this.SablonSoruTipi6SecenekSil(this.SablonSoruTipi6SecenekGetir(sablon_soru_tipi6_secenek_uid));
        }


        public DataSet SablonSoruTipi6SecenekGetirSoruyaGoreDataSet(Guid soru_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_sablon_soru_tipi_6_secenekleri where soru_uid='" + soru_uid + "' order by soru_secenek_sira_no");

            return ds_result;
        }

        public string SablonSoruTipi6SecenekMaxSiraNo(Guid soru_uid)
        {

            string max = BaseDB.DBManager.AppConnection.ExecuteSql("select max(soru_secenek_sira_no)+1 from sbr_sablon_soru_tipi_6_secenekleri where soru_uid='" + soru_uid + "' ");

            if (max == "")
                max = "1";

            return max;
        }

        public void SablonSoruTipi6SecenekSilSoruyaGore(Guid soru_uid)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_sablon_soru_tipi_6_secenekleri where soru_uid='" + soru_uid + "'");
        }
        #endregion

        #region Sablon Soru Tipi 7 Optionsi
        public IQueryable<sbr_sablon_soru_tipi_7_secenekleri> TumSablonSoruTipi7Secenekleri()
        {
            return db.sbr_sablon_soru_tipi_7_secenekleri;
        }
        public sbr_sablon_soru_tipi_7_secenekleri SablonSoruTipi7SecenekGetir(Guid soru_secenek_uid)
        {
            return db.sbr_sablon_soru_tipi_7_secenekleri.SingleOrDefault(d => d.soru_secenek_uid == soru_secenek_uid);
        }
        public void SablonSoruTipi7SecenekEkle(sbr_sablon_soru_tipi_7_secenekleri sablon_soru_tipi7_secenek)
        {
            sablon_soru_tipi7_secenek.soru_secenek_uid = Guid.NewGuid();
            db.sbr_sablon_soru_tipi_7_secenekleri.AddObject(sablon_soru_tipi7_secenek);
        }
        public void SablonSoruTipi7SecenekSil(sbr_sablon_soru_tipi_7_secenekleri sablon_soru_tipi7_secenekleri)
        {
            db.sbr_sablon_soru_tipi_7_secenekleri.DeleteObject(sablon_soru_tipi7_secenekleri);
        }

        public void SablonSoruTipi7SecenekSil(Guid sablon_soru_tipi7_secenek_uid)
        {
            this.SablonSoruTipi7SecenekSil(this.SablonSoruTipi7SecenekGetir(sablon_soru_tipi7_secenek_uid));
        }


        public DataSet SablonSoruTipi7SecenekGetirSoruyaGoreDataSet(Guid soru_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_sablon_soru_tipi_7_secenekleri where soru_uid='" + soru_uid + "' order by soru_secenek_sira_no");

            return ds_result;
        }

        public string SablonSoruTipi7SecenekMaxSiraNo(Guid soru_uid)
        {

            string max = BaseDB.DBManager.AppConnection.ExecuteSql("select max(soru_secenek_sira_no)+1 from sbr_sablon_soru_tipi_7_secenekleri where soru_uid='" + soru_uid + "' ");

            if (max == "")
                max = "1";

            return max;
        }

        public void SablonSoruTipi7SecenekSilSoruyaGore(Guid soru_uid)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_sablon_soru_tipi_7_secenekleri where soru_uid='" + soru_uid + "'");
        }
        #endregion

        #region Sablon Soru Tipi 8 Optionsi
        public IQueryable<sbr_sablon_soru_tipi_8_secenekleri> TumSablonSoruTipi8Secenekleri()
        {
            return db.sbr_sablon_soru_tipi_8_secenekleri;
        }
        public sbr_sablon_soru_tipi_8_secenekleri SablonSoruTipi8SecenekGetir(Guid soru_secenek_uid)
        {
            return db.sbr_sablon_soru_tipi_8_secenekleri.SingleOrDefault(d => d.soru_secenek_uid == soru_secenek_uid);
        }
        public void SablonSoruTipi8SecenekEkle(sbr_sablon_soru_tipi_8_secenekleri sablon_soru_tipi8_secenek)
        {
            sablon_soru_tipi8_secenek.soru_secenek_uid = Guid.NewGuid();
            db.sbr_sablon_soru_tipi_8_secenekleri.AddObject(sablon_soru_tipi8_secenek);
        }
        public void SablonSoruTipi8SecenekSil(sbr_sablon_soru_tipi_8_secenekleri sablon_soru_tipi8_secenekleri)
        {
            db.sbr_sablon_soru_tipi_8_secenekleri.DeleteObject(sablon_soru_tipi8_secenekleri);
        }

        public void SablonSoruTipi8SecenekSil(Guid sablon_soru_tipi8_secenek_uid)
        {
            this.SablonSoruTipi8SecenekSil(this.SablonSoruTipi8SecenekGetir(sablon_soru_tipi8_secenek_uid));
        }


        public DataSet SablonSoruTipi8SecenekGetirSoruyaGoreDataSet(Guid soru_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_sablon_soru_tipi_8_secenekleri where soru_uid='" + soru_uid + "' order by soru_secenek_sira_no");

            return ds_result;
        }

        public string SablonSoruTipi8SecenekMaxSiraNo(Guid soru_uid)
        {

            string max = BaseDB.DBManager.AppConnection.ExecuteSql("select max(soru_secenek_sira_no)+1 from sbr_sablon_soru_tipi_8_secenekleri where soru_uid='" + soru_uid + "' ");

            if (max == "")
                max = "1";

            return max;
        }

        public void SablonSoruTipi8SecenekSilSoruyaGore(Guid soru_uid)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_sablon_soru_tipi_8_secenekleri where soru_uid='" + soru_uid + "'");
        }
        #endregion

        #region Sablon Soru Tipi 9 Optionsi
        public IQueryable<sbr_sablon_soru_tipi_9_secenekleri> TumSablonSoruTipi9Secenekleri()
        {
            return db.sbr_sablon_soru_tipi_9_secenekleri;
        }
        public sbr_sablon_soru_tipi_9_secenekleri SablonSoruTipi9SecenekGetir(Guid soru_secenek_uid)
        {
            return db.sbr_sablon_soru_tipi_9_secenekleri.SingleOrDefault(d => d.soru_secenek_uid == soru_secenek_uid);
        }
        public void SablonSoruTipi9SecenekEkle(sbr_sablon_soru_tipi_9_secenekleri sablon_soru_tipi9_secenek)
        {
            sablon_soru_tipi9_secenek.soru_secenek_uid = Guid.NewGuid();
            db.sbr_sablon_soru_tipi_9_secenekleri.AddObject(sablon_soru_tipi9_secenek);
        }
        public void SablonSoruTipi9SecenekSil(sbr_sablon_soru_tipi_9_secenekleri sablon_soru_tipi9_secenekleri)
        {
            db.sbr_sablon_soru_tipi_9_secenekleri.DeleteObject(sablon_soru_tipi9_secenekleri);
        }

        public void SablonSoruTipi9SecenekSil(Guid sablon_soru_tipi9_secenek_uid)
        {
            this.SablonSoruTipi9SecenekSil(this.SablonSoruTipi9SecenekGetir(sablon_soru_tipi9_secenek_uid));
        }


        public DataSet SablonSoruTipi9SecenekGetirSoruyaGoreDataSet(Guid soru_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_sablon_soru_tipi_9_secenekleri where soru_uid='" + soru_uid + "' order by soru_secenek_sira_no");

            return ds_result;
        }

        public string SablonSoruTipi9SecenekMaxSiraNo(Guid soru_uid)
        {

            string max = BaseDB.DBManager.AppConnection.ExecuteSql("select max(soru_secenek_sira_no)+1 from sbr_sablon_soru_tipi_9_secenekleri where soru_uid='" + soru_uid + "' ");

            if (max == "")
                max = "1";

            return max;
        }

        public void SablonSoruTipi9SecenekSilSoruyaGore(Guid soru_uid)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_sablon_soru_tipi_9_secenekleri where soru_uid='" + soru_uid + "'");
        }
        #endregion

        #region Sablon Soru Tipi 10 Optionsi
        public IQueryable<sbr_sablon_soru_tipi_10_secenekleri> TumSablonSoruTipi10Secenekleri()
        {
            return db.sbr_sablon_soru_tipi_10_secenekleri;
        }
        public sbr_sablon_soru_tipi_10_secenekleri SablonSoruTipi10SecenekGetir(Guid soru_secenek_uid)
        {
            return db.sbr_sablon_soru_tipi_10_secenekleri.SingleOrDefault(d => d.soru_secenek_uid == soru_secenek_uid);
        }
        public void SablonSoruTipi10SecenekEkle(sbr_sablon_soru_tipi_10_secenekleri sablon_soru_tipi10_secenek)
        {
            sablon_soru_tipi10_secenek.soru_secenek_uid = Guid.NewGuid();
            db.sbr_sablon_soru_tipi_10_secenekleri.AddObject(sablon_soru_tipi10_secenek);
        }
        public void SablonSoruTipi10SecenekSil(sbr_sablon_soru_tipi_10_secenekleri sablon_soru_tipi10_secenekleri)
        {
            db.sbr_sablon_soru_tipi_10_secenekleri.DeleteObject(sablon_soru_tipi10_secenekleri);
        }

        public void SablonSoruTipi10SecenekSil(Guid sablon_soru_tipi10_secenek_uid)
        {
            this.SablonSoruTipi10SecenekSil(this.SablonSoruTipi10SecenekGetir(sablon_soru_tipi10_secenek_uid));
        }


        public DataSet SablonSoruTipi10SecenekGetirSoruyaGoreDataSet(Guid soru_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_sablon_soru_tipi_10_secenekleri where soru_uid='" + soru_uid + "' order by soru_secenek_sira_no");

            return ds_result;
        }

        public string SablonSoruTipi10SecenekMaxSiraNo(Guid soru_uid)
        {

            string max = BaseDB.DBManager.AppConnection.ExecuteSql("select max(soru_secenek_sira_no)+1 from sbr_sablon_soru_tipi_10_secenekleri where soru_uid='" + soru_uid + "' ");

            if (max == "")
                max = "1";

            return max;
        }

        public void SablonSoruTipi10SecenekSilSoruyaGore(Guid soru_uid)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_sablon_soru_tipi_10_secenekleri where soru_uid='" + soru_uid + "'");
        }
        #endregion

        #region Sablon Soru Tipi 11 Optionsi
        public IQueryable<sbr_sablon_soru_tipi_11_secenekleri> TumSablonSoruTipi11Secenekleri()
        {
            return db.sbr_sablon_soru_tipi_11_secenekleri;
        }
        public sbr_sablon_soru_tipi_11_secenekleri SablonSoruTipi11SecenekGetir(Guid soru_secenek_uid)
        {
            return db.sbr_sablon_soru_tipi_11_secenekleri.SingleOrDefault(d => d.soru_secenek_uid == soru_secenek_uid);
        }
        public void SablonSoruTipi11SecenekEkle(sbr_sablon_soru_tipi_11_secenekleri sablon_soru_tipi11_secenek)
        {
            sablon_soru_tipi11_secenek.soru_secenek_uid = Guid.NewGuid();
            db.sbr_sablon_soru_tipi_11_secenekleri.AddObject(sablon_soru_tipi11_secenek);
        }
        public void SablonSoruTipi11SecenekSil(sbr_sablon_soru_tipi_11_secenekleri sablon_soru_tipi11_secenekleri)
        {
            db.sbr_sablon_soru_tipi_11_secenekleri.DeleteObject(sablon_soru_tipi11_secenekleri);
        }

        public void SablonSoruTipi11SecenekSil(Guid sablon_soru_tipi11_secenek_uid)
        {
            this.SablonSoruTipi11SecenekSil(this.SablonSoruTipi11SecenekGetir(sablon_soru_tipi11_secenek_uid));
        }


        public DataSet SablonSoruTipi11SecenekGetirSoruyaGoreDataSet(Guid soru_uid)
        {
            DataSet ds_result = new DataSet();

            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_sablon_soru_tipi_11_secenekleri where soru_uid='" + soru_uid + "' order by soru_secenek_sira_no");

            return ds_result;
        }

        public string SablonSoruTipi11SecenekMaxSiraNo(Guid soru_uid)
        {

            string max = BaseDB.DBManager.AppConnection.ExecuteSql("select max(soru_secenek_sira_no)+1 from sbr_sablon_soru_tipi_11_secenekleri where soru_uid='" + soru_uid + "' ");

            if (max == "")
                max = "1";

            return max;
        }

        public void SablonSoruTipi11SecenekSilSoruyaGore(Guid soru_uid)
        {
            BaseDB.DBManager.AppConnection.ExecuteSql("delete from sbr_sablon_soru_tipi_11_secenekleri where soru_uid='" + soru_uid + "'");
        }
        #endregion

        #region Survey ve Şablondan Getir Suruları Kaydet
        public void SurveytenGetirSorulariKaydet(Guid anket_uid,Guid ana_anket_uid)
        {

            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            sbr_anket anket = ankDB.SurveyGetir(anket_uid);

            DataSet ds_anket_sorlari = ankDB.SurveySoruGetirSurveyGoreDataSet(anket_uid);

            foreach (DataRow dr_anket_sorulari in ds_anket_sorlari.Tables[0].Rows)
            {
                if (dr_anket_sorulari["soru_uid"] != null && dr_anket_sorulari["soru_uid"].ToString() != "")
                {
                    sbr_anket_sorulari anket_sorulari = new sbr_anket_sorulari();
                    ankDB.SurveySoruEkle(anket_sorulari);
                    anket_sorulari.anket_uid = ana_anket_uid;
                    if (dr_anket_sorulari["soru"] != null) anket_sorulari.soru = dr_anket_sorulari["soru"].ToString();
                    anket_sorulari.soru_olusturan_kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                    anket_sorulari.soru_olusturma_tarihi = DateTime.Now;
                    if (dr_anket_sorulari["soru_siralama_sekli_id"] != null && dr_anket_sorulari["soru_siralama_sekli_id"].ToString() != "") anket_sorulari.soru_siralama_sekli_id = Convert.ToInt32(dr_anket_sorulari["soru_siralama_sekli_id"].ToString());
                    if (dr_anket_sorulari["soru_tipi_id"] != null && dr_anket_sorulari["soru_tipi_id"].ToString() != "") anket_sorulari.soru_tipi_id = Convert.ToInt32(dr_anket_sorulari["soru_tipi_id"].ToString());
                    anket_sorulari.soru_sira_no = Convert.ToInt32(ankDB.SurveySoruMaxSiraNo(ana_anket_uid));
                    if (dr_anket_sorulari["soru_sayisal_ondalik"] != null && (dr_anket_sorulari["soru_sayisal_ondalik"].ToString() == "true" || dr_anket_sorulari["soru_sayisal_ondalik"].ToString() == "True" || dr_anket_sorulari["soru_sayisal_ondalik"].ToString() == "TRUE")) anket_sorulari.soru_sayisal_ondalik = true;
                    if (dr_anket_sorulari["soru_tek_satir"] != null && (dr_anket_sorulari["soru_tek_satir"].ToString() == "true" || dr_anket_sorulari["soru_tek_satir"].ToString() == "True" || dr_anket_sorulari["soru_tek_satir"].ToString() == "TRUE")) anket_sorulari.soru_tek_satir = true;
                    ankDB.Kaydet();

                    DataSet anket_soru_tipi_1_secenekleri = ankDB.AcikAnketSoruTipi1SecenekGetirSoruyaGoreDataSet(Guid.Parse(dr_anket_sorulari["soru_uid"].ToString()),Guid.Empty);
                    foreach (DataRow dr_anket_soru_tipi_1_secenekleri in anket_soru_tipi_1_secenekleri.Tables[0].Rows)
                    {
                        sbr_soru_tipi_1_secenekleri soru_tipi_1_secenekleri = new sbr_soru_tipi_1_secenekleri();
                        ankDB.AcikAnketSoruTipi1SecenekEkle(soru_tipi_1_secenekleri);
                        soru_tipi_1_secenekleri.soru_uid = anket_sorulari.soru_uid;
                        if (dr_anket_soru_tipi_1_secenekleri["soru_secenek_ad"] != null && dr_anket_soru_tipi_1_secenekleri["soru_secenek_ad"].ToString() != "") soru_tipi_1_secenekleri.soru_secenek_ad = dr_anket_soru_tipi_1_secenekleri["soru_secenek_ad"].ToString();
                        soru_tipi_1_secenekleri.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi1SecenekMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }

                    DataSet anket_soru_tipi_2_secenekleri = ankDB.AcikAnketSoruTipi2SecenekGetirSoruyaGoreDataSet(Guid.Parse(dr_anket_sorulari["soru_uid"].ToString()));
                    foreach (DataRow dr_anket_soru_tipi_2_secenekleri in anket_soru_tipi_2_secenekleri.Tables[0].Rows)
                    {
                        sbr_soru_tipi_2_secenekleri soru_tipi_2_secenekleri = new sbr_soru_tipi_2_secenekleri();
                        ankDB.AcikAnketSoruTipi2SecenekEkle(soru_tipi_2_secenekleri);
                        soru_tipi_2_secenekleri.soru_uid = anket_sorulari.soru_uid;
                        if (dr_anket_soru_tipi_2_secenekleri["soru_secenek_ad"] != null && dr_anket_soru_tipi_2_secenekleri["soru_secenek_ad"].ToString() != "") soru_tipi_2_secenekleri.soru_secenek_ad = dr_anket_soru_tipi_2_secenekleri["soru_secenek_ad"].ToString();
                        soru_tipi_2_secenekleri.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi2SecenekMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }

                    DataSet anket_soru_tipi_3_secenekleri = ankDB.AcikAnketSoruTipi3SecenekGetirSoruyaGoreDataSet(Guid.Parse(dr_anket_sorulari["soru_uid"].ToString()));
                    foreach (DataRow dr_anket_soru_tipi_3_secenekleri in anket_soru_tipi_3_secenekleri.Tables[0].Rows)
                    {
                        sbr_soru_tipi_3_secenekleri soru_tipi_3_secenekleri = new sbr_soru_tipi_3_secenekleri();
                        ankDB.AcikAnketSoruTipi3SecenekEkle(soru_tipi_3_secenekleri);
                        soru_tipi_3_secenekleri.soru_uid = anket_sorulari.soru_uid;
                        if (dr_anket_soru_tipi_3_secenekleri["soru_secenek_ad"] != null && dr_anket_soru_tipi_3_secenekleri["soru_secenek_ad"].ToString() != "") soru_tipi_3_secenekleri.soru_secenek_ad = dr_anket_soru_tipi_3_secenekleri["soru_secenek_ad"].ToString();
                        soru_tipi_3_secenekleri.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi3SecenekMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }

                    DataSet anket_soru_tipi_3_secenekleri_kolonlari = ankDB.AcikAnketSoruTipi3SecenekKolonGetirSoruyaGoreDataSet(Guid.Parse(dr_anket_sorulari["soru_uid"].ToString()));
                    foreach (DataRow dr_anket_soru_tipi_3_secenekleri_kolonlari in anket_soru_tipi_3_secenekleri_kolonlari.Tables[0].Rows)
                    {
                        sbr_soru_tipi_3_secenek_kolonlari soru_tipi_3_secenekleri_kolonlari = new sbr_soru_tipi_3_secenek_kolonlari();
                        ankDB.AcikAnketSoruTipi3SecenekKolonEkle(soru_tipi_3_secenekleri_kolonlari);
                        soru_tipi_3_secenekleri_kolonlari.soru_uid = anket_sorulari.soru_uid;
                        if (dr_anket_soru_tipi_3_secenekleri_kolonlari["soru_secenek_kolon_ad"] != null && dr_anket_soru_tipi_3_secenekleri_kolonlari["soru_secenek_kolon_ad"].ToString() != "") soru_tipi_3_secenekleri_kolonlari.soru_secenek_kolon_ad = dr_anket_soru_tipi_3_secenekleri_kolonlari["soru_secenek_kolon_ad"].ToString();
                        soru_tipi_3_secenekleri_kolonlari.soru_secenek_kolon_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi3SecenekKolonMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }

                    DataSet anket_soru_tipi_4_secenekleri = ankDB.AcikAnketSoruTipi4SecenekGetirSoruyaGoreDataSet(Guid.Parse(dr_anket_sorulari["soru_uid"].ToString()));
                    foreach (DataRow dr_anket_soru_tipi_4_secenekleri in anket_soru_tipi_4_secenekleri.Tables[0].Rows)
                    {
                        sbr_soru_tipi_4_secenekleri soru_tipi_4_secenekleri = new sbr_soru_tipi_4_secenekleri();
                        ankDB.AcikAnketSoruTipi4SecenekEkle(soru_tipi_4_secenekleri);
                        soru_tipi_4_secenekleri.soru_uid = anket_sorulari.soru_uid;
                        if (dr_anket_soru_tipi_4_secenekleri["soru_secenek_ad"] != null && dr_anket_soru_tipi_4_secenekleri["soru_secenek_ad"].ToString() != "") soru_tipi_4_secenekleri.soru_secenek_ad = dr_anket_soru_tipi_4_secenekleri["soru_secenek_ad"].ToString();
                        soru_tipi_4_secenekleri.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi4SecenekMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }

                    DataSet anket_soru_tipi_5_secenekleri = ankDB.AcikAnketSoruTipi5SecenekGetirSoruyaGoreDataSet(Guid.Parse(dr_anket_sorulari["soru_uid"].ToString()));
                    foreach (DataRow dr_anket_soru_tipi_5_secenekleri in anket_soru_tipi_5_secenekleri.Tables[0].Rows)
                    {
                        sbr_soru_tipi_5_secenekleri soru_tipi_5_secenekleri = new sbr_soru_tipi_5_secenekleri();
                        ankDB.AcikAnketSoruTipi5SecenekEkle(soru_tipi_5_secenekleri);
                        soru_tipi_5_secenekleri.soru_uid = anket_sorulari.soru_uid;
                        if (dr_anket_soru_tipi_5_secenekleri["soru_secenek_ad"] != null && dr_anket_soru_tipi_5_secenekleri["soru_secenek_ad"].ToString() != "") soru_tipi_5_secenekleri.soru_secenek_ad = dr_anket_soru_tipi_5_secenekleri["soru_secenek_ad"].ToString();
                        soru_tipi_5_secenekleri.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi5SecenekMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }

                    DataSet anket_soru_tipi_6_secenekleri = ankDB.AcikAnketSoruTipi6SecenekGetirSoruyaGoreDataSet(Guid.Parse(dr_anket_sorulari["soru_uid"].ToString()));
                    foreach (DataRow dr_anket_soru_tipi_6_secenekleri in anket_soru_tipi_6_secenekleri.Tables[0].Rows)
                    {
                        sbr_soru_tipi_6_secenekleri soru_tipi_6_secenekleri = new sbr_soru_tipi_6_secenekleri();
                        ankDB.AcikAnketSoruTipi6SecenekEkle(soru_tipi_6_secenekleri);
                        soru_tipi_6_secenekleri.soru_uid = anket_sorulari.soru_uid;
                        if (dr_anket_soru_tipi_6_secenekleri["soru_secenek_ad"] != null && dr_anket_soru_tipi_6_secenekleri["soru_secenek_ad"].ToString() != "") soru_tipi_6_secenekleri.soru_secenek_ad = dr_anket_soru_tipi_6_secenekleri["soru_secenek_ad"].ToString();
                        soru_tipi_6_secenekleri.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi6SecenekMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }

                    DataSet anket_soru_tipi_7_secenekleri = ankDB.AcikAnketSoruTipi7SecenekGetirSoruyaGoreDataSet(Guid.Parse(dr_anket_sorulari["soru_uid"].ToString()));
                    foreach (DataRow dr_anket_soru_tipi_7_secenekleri in anket_soru_tipi_7_secenekleri.Tables[0].Rows)
                    {
                        sbr_soru_tipi_7_secenekleri soru_tipi_7_secenekleri = new sbr_soru_tipi_7_secenekleri();
                        ankDB.AcikAnketSoruTipi7SecenekEkle(soru_tipi_7_secenekleri);
                        soru_tipi_7_secenekleri.soru_uid = anket_sorulari.soru_uid;
                        if (dr_anket_soru_tipi_7_secenekleri["soru_secenek_ad"] != null && dr_anket_soru_tipi_7_secenekleri["soru_secenek_ad"].ToString() != "") soru_tipi_7_secenekleri.soru_secenek_ad = dr_anket_soru_tipi_7_secenekleri["soru_secenek_ad"].ToString();
                        soru_tipi_7_secenekleri.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi7SecenekMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }

                    DataSet anket_soru_tipi_8_secenekleri = ankDB.AcikAnketSoruTipi8SecenekGetirSoruyaGoreDataSet(Guid.Parse(dr_anket_sorulari["soru_uid"].ToString()));
                    foreach (DataRow dr_anket_soru_tipi_8_secenekleri in anket_soru_tipi_8_secenekleri.Tables[0].Rows)
                    {
                        sbr_soru_tipi_8_secenekleri soru_tipi_8_secenekleri = new sbr_soru_tipi_8_secenekleri();
                        ankDB.AcikAnketSoruTipi8SecenekEkle(soru_tipi_8_secenekleri);
                        soru_tipi_8_secenekleri.soru_uid = anket_sorulari.soru_uid;
                        if (dr_anket_soru_tipi_8_secenekleri["soru_secenek_ad"] != null && dr_anket_soru_tipi_8_secenekleri["soru_secenek_ad"].ToString() != "") soru_tipi_8_secenekleri.soru_secenek_ad = dr_anket_soru_tipi_8_secenekleri["soru_secenek_ad"].ToString();
                        soru_tipi_8_secenekleri.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi8SecenekMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }

                    DataSet anket_soru_tipi_9_secenekleri = ankDB.AcikAnketSoruTipi9SecenekGetirSoruyaGoreDataSet(Guid.Parse(dr_anket_sorulari["soru_uid"].ToString()));
                    foreach (DataRow dr_anket_soru_tipi_9_secenekleri in anket_soru_tipi_9_secenekleri.Tables[0].Rows)
                    {
                        sbr_soru_tipi_9_secenekleri soru_tipi_9_secenekleri = new sbr_soru_tipi_9_secenekleri();
                        ankDB.AcikAnketSoruTipi9SecenekEkle(soru_tipi_9_secenekleri);
                        soru_tipi_9_secenekleri.soru_uid = anket_sorulari.soru_uid;
                        if (dr_anket_soru_tipi_9_secenekleri["soru_secenek_ad"] != null && dr_anket_soru_tipi_9_secenekleri["soru_secenek_ad"].ToString() != "") soru_tipi_9_secenekleri.soru_secenek_ad = dr_anket_soru_tipi_9_secenekleri["soru_secenek_ad"].ToString();
                        soru_tipi_9_secenekleri.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi9SecenekMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }

                    DataSet anket_soru_tipi_10_secenekleri = ankDB.AcikAnketSoruTipi10SecenekGetirSoruyaGoreDataSet(Guid.Parse(dr_anket_sorulari["soru_uid"].ToString()));
                    foreach (DataRow dr_anket_soru_tipi_10_secenekleri in anket_soru_tipi_10_secenekleri.Tables[0].Rows)
                    {
                        sbr_soru_tipi_10_secenekleri soru_tipi_10_secenekleri = new sbr_soru_tipi_10_secenekleri();
                        ankDB.AcikAnketSoruTipi10SecenekEkle(soru_tipi_10_secenekleri);
                        soru_tipi_10_secenekleri.soru_uid = anket_sorulari.soru_uid;
                        if (dr_anket_soru_tipi_10_secenekleri["soru_secenek_ad"] != null && dr_anket_soru_tipi_10_secenekleri["soru_secenek_ad"].ToString() != "") soru_tipi_10_secenekleri.soru_secenek_ad = dr_anket_soru_tipi_10_secenekleri["soru_secenek_ad"].ToString();
                        soru_tipi_10_secenekleri.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi10SecenekMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }

                    DataSet anket_soru_tipi_11_secenekleri = ankDB.AcikAnketSoruTipi11SecenekGetirSoruyaGoreDataSet(Guid.Parse(dr_anket_sorulari["soru_uid"].ToString()));
                    foreach (DataRow dr_anket_soru_tipi_11_secenekleri in anket_soru_tipi_11_secenekleri.Tables[0].Rows)
                    {
                        sbr_soru_tipi_11_secenekleri soru_tipi_11_secenekleri = new sbr_soru_tipi_11_secenekleri();
                        ankDB.AcikAnketSoruTipi11SecenekEkle(soru_tipi_11_secenekleri);
                        soru_tipi_11_secenekleri.soru_uid = anket_sorulari.soru_uid;
                        if (dr_anket_soru_tipi_11_secenekleri["soru_secenek_ad"] != null && dr_anket_soru_tipi_11_secenekleri["soru_secenek_ad"].ToString() != "") soru_tipi_11_secenekleri.soru_secenek_ad = dr_anket_soru_tipi_11_secenekleri["soru_secenek_ad"].ToString();
                        soru_tipi_11_secenekleri.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi11SecenekMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }
                }
            }

        }

        public void SablonSorulariniKaydet(Guid sablon_uid,Guid ana_anket_uid)
        {

            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            sbr_sablon sablon = ankDB.SablonGetir(sablon_uid);

            DataSet ds_sablon_sorlari = ankDB.SablonSoruGetirSablonGoreDataSet(sablon_uid);

            foreach (DataRow dr_sablon_sorulari in ds_sablon_sorlari.Tables[0].Rows)
            {
                if (dr_sablon_sorulari["soru_uid"] != null && dr_sablon_sorulari["soru_uid"].ToString() != "")
                {
                    sbr_anket_sorulari anket_sorulari = new sbr_anket_sorulari();
                    ankDB.SurveySoruEkle(anket_sorulari);
                    anket_sorulari.anket_uid = ana_anket_uid;
                    if (dr_sablon_sorulari["soru"] != null) anket_sorulari.soru = dr_sablon_sorulari["soru"].ToString();
                    anket_sorulari.soru_olusturan_kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                    anket_sorulari.soru_olusturma_tarihi = DateTime.Now;
                    if (dr_sablon_sorulari["soru_siralama_sekli_id"] != null && dr_sablon_sorulari["soru_siralama_sekli_id"].ToString() != "") anket_sorulari.soru_siralama_sekli_id = Convert.ToInt32(dr_sablon_sorulari["soru_siralama_sekli_id"].ToString());
                    if (dr_sablon_sorulari["soru_tipi_id"] != null && dr_sablon_sorulari["soru_tipi_id"].ToString() != "") anket_sorulari.soru_tipi_id = Convert.ToInt32(dr_sablon_sorulari["soru_tipi_id"].ToString());
                    if (dr_sablon_sorulari["soru_sayisal_ondalik"] != null && (dr_sablon_sorulari["soru_sayisal_ondalik"].ToString() == "true" || dr_sablon_sorulari["soru_sayisal_ondalik"].ToString() == "True" || dr_sablon_sorulari["soru_sayisal_ondalik"].ToString() == "TRUE")) anket_sorulari.soru_sayisal_ondalik = true;
                    if (dr_sablon_sorulari["soru_tek_satir"] != null && (dr_sablon_sorulari["soru_tek_satir"].ToString() == "true" || dr_sablon_sorulari["soru_tek_satir"].ToString() == "True" || dr_sablon_sorulari["soru_tek_satir"].ToString() == "TRUE")) anket_sorulari.soru_tek_satir = true;
                    anket_sorulari.soru_sira_no = Convert.ToInt32(ankDB.SurveySoruMaxSiraNo(ana_anket_uid));
                    ankDB.Kaydet();

                    DataSet sablon_soru_tipi_1_secenekleri = ankDB.SablonSoruTipi1SecenekGetirSoruyaGoreDataSet(Guid.Parse(dr_sablon_sorulari["soru_uid"].ToString()));
                    foreach (DataRow dr_sablon_soru_tipi_1_secenekleri in sablon_soru_tipi_1_secenekleri.Tables[0].Rows)
                    {
                        sbr_soru_tipi_1_secenekleri soru_tipi_1_secenekleri = new sbr_soru_tipi_1_secenekleri();
                        ankDB.AcikAnketSoruTipi1SecenekEkle(soru_tipi_1_secenekleri);
                        soru_tipi_1_secenekleri.soru_uid = anket_sorulari.soru_uid;
                        if (dr_sablon_soru_tipi_1_secenekleri["soru_secenek_ad"] != null && dr_sablon_soru_tipi_1_secenekleri["soru_secenek_ad"].ToString() != "") soru_tipi_1_secenekleri.soru_secenek_ad = dr_sablon_soru_tipi_1_secenekleri["soru_secenek_ad"].ToString();
                        soru_tipi_1_secenekleri.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi1SecenekMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }

                    DataSet sablon_soru_tipi_2_secenekleri = ankDB.SablonSoruTipi2SecenekGetirSoruyaGoreDataSet(Guid.Parse(dr_sablon_sorulari["soru_uid"].ToString()));
                    foreach (DataRow dr_sablon_soru_tipi_2_secenekleri in sablon_soru_tipi_2_secenekleri.Tables[0].Rows)
                    {
                        sbr_soru_tipi_2_secenekleri soru_tipi_2_secenekleri = new sbr_soru_tipi_2_secenekleri();
                        ankDB.AcikAnketSoruTipi2SecenekEkle(soru_tipi_2_secenekleri);
                        soru_tipi_2_secenekleri.soru_uid = anket_sorulari.soru_uid;
                        if (dr_sablon_soru_tipi_2_secenekleri["soru_secenek_ad"] != null && dr_sablon_soru_tipi_2_secenekleri["soru_secenek_ad"].ToString() != "") soru_tipi_2_secenekleri.soru_secenek_ad = dr_sablon_soru_tipi_2_secenekleri["soru_secenek_ad"].ToString();
                        soru_tipi_2_secenekleri.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi2SecenekMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }

                    DataSet sablon_soru_tipi_3_secenekleri = ankDB.SablonSoruTipi3SecenekGetirSoruyaGoreDataSet(Guid.Parse(dr_sablon_sorulari["soru_uid"].ToString()));
                    foreach (DataRow dr_sablon_soru_tipi_3_secenekleri in sablon_soru_tipi_3_secenekleri.Tables[0].Rows)
                    {
                        sbr_soru_tipi_3_secenekleri soru_tipi_3_secenekleri = new sbr_soru_tipi_3_secenekleri();
                        ankDB.AcikAnketSoruTipi3SecenekEkle(soru_tipi_3_secenekleri);
                        soru_tipi_3_secenekleri.soru_uid = anket_sorulari.soru_uid;
                        if (dr_sablon_soru_tipi_3_secenekleri["soru_secenek_ad"] != null && dr_sablon_soru_tipi_3_secenekleri["soru_secenek_ad"].ToString() != "") soru_tipi_3_secenekleri.soru_secenek_ad = dr_sablon_soru_tipi_3_secenekleri["soru_secenek_ad"].ToString();
                        soru_tipi_3_secenekleri.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi3SecenekMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }

                    DataSet sablon_soru_tipi_3_secenekleri_kolonlari = ankDB.SablonSoruTipi3SecenekKolonGetirSoruyaGoreDataSet(Guid.Parse(dr_sablon_sorulari["soru_uid"].ToString()));
                    foreach (DataRow dr_sablon_soru_tipi_3_secenekleri_kolonlari in sablon_soru_tipi_3_secenekleri_kolonlari.Tables[0].Rows)
                    {
                        sbr_soru_tipi_3_secenek_kolonlari soru_tipi_3_secenekleri_kolonlari = new sbr_soru_tipi_3_secenek_kolonlari();
                        ankDB.AcikAnketSoruTipi3SecenekKolonEkle(soru_tipi_3_secenekleri_kolonlari);
                        soru_tipi_3_secenekleri_kolonlari.soru_uid = anket_sorulari.soru_uid;
                        if (dr_sablon_soru_tipi_3_secenekleri_kolonlari["soru_secenek_kolon_ad"] != null && dr_sablon_soru_tipi_3_secenekleri_kolonlari["soru_secenek_kolon_ad"].ToString() != "") soru_tipi_3_secenekleri_kolonlari.soru_secenek_kolon_ad = dr_sablon_soru_tipi_3_secenekleri_kolonlari["soru_secenek_kolon_ad"].ToString();
                        soru_tipi_3_secenekleri_kolonlari.soru_secenek_kolon_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi3SecenekKolonMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }

                    DataSet sablon_soru_tipi_4_secenekleri = ankDB.SablonSoruTipi4SecenekGetirSoruyaGoreDataSet(Guid.Parse(dr_sablon_sorulari["soru_uid"].ToString()));
                    foreach (DataRow dr_sablon_soru_tipi_4_secenekleri in sablon_soru_tipi_4_secenekleri.Tables[0].Rows)
                    {
                        sbr_soru_tipi_4_secenekleri soru_tipi_4_secenekleri = new sbr_soru_tipi_4_secenekleri();
                        ankDB.AcikAnketSoruTipi4SecenekEkle(soru_tipi_4_secenekleri);
                        soru_tipi_4_secenekleri.soru_uid = anket_sorulari.soru_uid;
                        if (dr_sablon_soru_tipi_4_secenekleri["soru_secenek_ad"] != null && dr_sablon_soru_tipi_4_secenekleri["soru_secenek_ad"].ToString() != "") soru_tipi_4_secenekleri.soru_secenek_ad = dr_sablon_soru_tipi_4_secenekleri["soru_secenek_ad"].ToString();
                        soru_tipi_4_secenekleri.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi4SecenekMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }

                    DataSet sablon_soru_tipi_5_secenekleri = ankDB.SablonSoruTipi5SecenekGetirSoruyaGoreDataSet(Guid.Parse(dr_sablon_sorulari["soru_uid"].ToString()));
                    foreach (DataRow dr_sablon_soru_tipi_5_secenekleri in sablon_soru_tipi_5_secenekleri.Tables[0].Rows)
                    {
                        sbr_soru_tipi_5_secenekleri soru_tipi_5_secenekleri = new sbr_soru_tipi_5_secenekleri();
                        ankDB.AcikAnketSoruTipi5SecenekEkle(soru_tipi_5_secenekleri);
                        soru_tipi_5_secenekleri.soru_uid = anket_sorulari.soru_uid;
                        if (dr_sablon_soru_tipi_5_secenekleri["soru_secenek_ad"] != null && dr_sablon_soru_tipi_5_secenekleri["soru_secenek_ad"].ToString() != "") soru_tipi_5_secenekleri.soru_secenek_ad = dr_sablon_soru_tipi_5_secenekleri["soru_secenek_ad"].ToString();
                        soru_tipi_5_secenekleri.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi5SecenekMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }

                    DataSet sablon_soru_tipi_6_secenekleri = ankDB.SablonSoruTipi6SecenekGetirSoruyaGoreDataSet(Guid.Parse(dr_sablon_sorulari["soru_uid"].ToString()));
                    foreach (DataRow dr_sablon_soru_tipi_6_secenekleri in sablon_soru_tipi_6_secenekleri.Tables[0].Rows)
                    {
                        sbr_soru_tipi_6_secenekleri soru_tipi_6_secenekleri = new sbr_soru_tipi_6_secenekleri();
                        ankDB.AcikAnketSoruTipi6SecenekEkle(soru_tipi_6_secenekleri);
                        soru_tipi_6_secenekleri.soru_uid = anket_sorulari.soru_uid;
                        if (dr_sablon_soru_tipi_6_secenekleri["soru_secenek_ad"] != null && dr_sablon_soru_tipi_6_secenekleri["soru_secenek_ad"].ToString() != "") soru_tipi_6_secenekleri.soru_secenek_ad = dr_sablon_soru_tipi_6_secenekleri["soru_secenek_ad"].ToString();
                        soru_tipi_6_secenekleri.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi6SecenekMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }

                    DataSet sablon_soru_tipi_7_secenekleri = ankDB.SablonSoruTipi7SecenekGetirSoruyaGoreDataSet(Guid.Parse(dr_sablon_sorulari["soru_uid"].ToString()));
                    foreach (DataRow dr_sablon_soru_tipi_7_secenekleri in sablon_soru_tipi_7_secenekleri.Tables[0].Rows)
                    {
                        sbr_soru_tipi_7_secenekleri soru_tipi_7_secenekleri = new sbr_soru_tipi_7_secenekleri();
                        ankDB.AcikAnketSoruTipi7SecenekEkle(soru_tipi_7_secenekleri);
                        soru_tipi_7_secenekleri.soru_uid = anket_sorulari.soru_uid;
                        if (dr_sablon_soru_tipi_7_secenekleri["soru_secenek_ad"] != null && dr_sablon_soru_tipi_7_secenekleri["soru_secenek_ad"].ToString() != "") soru_tipi_7_secenekleri.soru_secenek_ad = dr_sablon_soru_tipi_7_secenekleri["soru_secenek_ad"].ToString();
                        soru_tipi_7_secenekleri.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi7SecenekMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }

                    DataSet sablon_soru_tipi_8_secenekleri = ankDB.SablonSoruTipi8SecenekGetirSoruyaGoreDataSet(Guid.Parse(dr_sablon_sorulari["soru_uid"].ToString()));
                    foreach (DataRow dr_sablon_soru_tipi_8_secenekleri in sablon_soru_tipi_8_secenekleri.Tables[0].Rows)
                    {
                        sbr_soru_tipi_8_secenekleri soru_tipi_8_secenekleri = new sbr_soru_tipi_8_secenekleri();
                        ankDB.AcikAnketSoruTipi8SecenekEkle(soru_tipi_8_secenekleri);
                        soru_tipi_8_secenekleri.soru_uid = anket_sorulari.soru_uid;
                        if (dr_sablon_soru_tipi_8_secenekleri["soru_secenek_ad"] != null && dr_sablon_soru_tipi_8_secenekleri["soru_secenek_ad"].ToString() != "") soru_tipi_8_secenekleri.soru_secenek_ad = dr_sablon_soru_tipi_8_secenekleri["soru_secenek_ad"].ToString();
                        soru_tipi_8_secenekleri.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi8SecenekMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }

                    DataSet sablon_soru_tipi_9_secenekleri = ankDB.SablonSoruTipi9SecenekGetirSoruyaGoreDataSet(Guid.Parse(dr_sablon_sorulari["soru_uid"].ToString()));
                    foreach (DataRow dr_sablon_soru_tipi_9_secenekleri in sablon_soru_tipi_9_secenekleri.Tables[0].Rows)
                    {
                        sbr_soru_tipi_9_secenekleri soru_tipi_9_secenekleri = new sbr_soru_tipi_9_secenekleri();
                        ankDB.AcikAnketSoruTipi9SecenekEkle(soru_tipi_9_secenekleri);
                        soru_tipi_9_secenekleri.soru_uid = anket_sorulari.soru_uid;
                        if (dr_sablon_soru_tipi_9_secenekleri["soru_secenek_ad"] != null && dr_sablon_soru_tipi_9_secenekleri["soru_secenek_ad"].ToString() != "") soru_tipi_9_secenekleri.soru_secenek_ad = dr_sablon_soru_tipi_9_secenekleri["soru_secenek_ad"].ToString();
                        soru_tipi_9_secenekleri.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi9SecenekMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }

                    DataSet sablon_soru_tipi_10_secenekleri = ankDB.SablonSoruTipi10SecenekGetirSoruyaGoreDataSet(Guid.Parse(dr_sablon_sorulari["soru_uid"].ToString()));
                    foreach (DataRow dr_sablon_soru_tipi_10_secenekleri in sablon_soru_tipi_10_secenekleri.Tables[0].Rows)
                    {
                        sbr_soru_tipi_10_secenekleri soru_tipi_10_secenekleri = new sbr_soru_tipi_10_secenekleri();
                        ankDB.AcikAnketSoruTipi10SecenekEkle(soru_tipi_10_secenekleri);
                        soru_tipi_10_secenekleri.soru_uid = anket_sorulari.soru_uid;
                        if (dr_sablon_soru_tipi_10_secenekleri["soru_secenek_ad"] != null && dr_sablon_soru_tipi_10_secenekleri["soru_secenek_ad"].ToString() != "") soru_tipi_10_secenekleri.soru_secenek_ad = dr_sablon_soru_tipi_10_secenekleri["soru_secenek_ad"].ToString();
                        soru_tipi_10_secenekleri.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi10SecenekMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }

                    DataSet sablon_soru_tipi_11_secenekleri = ankDB.SablonSoruTipi11SecenekGetirSoruyaGoreDataSet(Guid.Parse(dr_sablon_sorulari["soru_uid"].ToString()));
                    foreach (DataRow dr_sablon_soru_tipi_11_secenekleri in sablon_soru_tipi_11_secenekleri.Tables[0].Rows)
                    {
                        sbr_soru_tipi_11_secenekleri soru_tipi_11_secenekleri = new sbr_soru_tipi_11_secenekleri();
                        ankDB.AcikAnketSoruTipi11SecenekEkle(soru_tipi_11_secenekleri);
                        soru_tipi_11_secenekleri.soru_uid = anket_sorulari.soru_uid;
                        if (dr_sablon_soru_tipi_11_secenekleri["soru_secenek_ad"] != null && dr_sablon_soru_tipi_11_secenekleri["soru_secenek_ad"].ToString() != "") soru_tipi_11_secenekleri.soru_secenek_ad = dr_sablon_soru_tipi_11_secenekleri["soru_secenek_ad"].ToString();
                        soru_tipi_11_secenekleri.soru_secenek_sira_no = Convert.ToInt32(ankDB.AcikAnketSoruTipi11SecenekMaxSiraNo(anket_sorulari.soru_uid));
                        ankDB.Kaydet();
                    }
                }
            }

        }
        #endregion
    }
}