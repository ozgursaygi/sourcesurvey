using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;
using System.Text;

namespace BaseWebSite.Models
{
    public class GeneralRepository 
    {
        #region Ileti işlemleri
        public IQueryable<gnl_ileti> TumIletiler()
        {
            return db.gnl_ileti;
        }
        public gnl_ileti IletiyiGetir(Guid ileti_uid)
        {
            return db.gnl_ileti.SingleOrDefault(d => d.ileti_uid == ileti_uid);
        }

       
        public void IletiEkle(gnl_ileti ileti)
        {
            ileti.ileti_uid = Guid.NewGuid();
            db.gnl_ileti.AddObject(ileti);
        }
        public void IletiKaydiniSil(gnl_ileti ileti)
        {
            db.gnl_ileti.DeleteObject(ileti);
        }

        public void IletiyiSil(Guid ileti_uid)
        {
            this.IletiKaydiniSil(this.IletiyiGetir(ileti_uid));
        }
        public gnl_ileti_gelen GelenKutusundakiIletiyiGetir(Guid ileti_uid, Guid alici_uid)
        {
            return db.gnl_ileti_gelen.SingleOrDefault(gnl => gnl.ileti_uid == ileti_uid && gnl.kullanici_uid == alici_uid);
        }

        public void GelenKutusundanFizikiOlarakSil(gnl_ileti_gelen gelen)
        {
            db.gnl_ileti_gelen.DeleteObject(gelen);
        }

        public static DataSet GelenMesajlar(Guid kullanici_uid, string filter, bool deleted_flag = false)
        {
            string query =
            @"Select 
               [ileti_uid]
              ,[gonderen_adi]
              ,[kullanici_uid]
              ,[alici_tipi]
              ,[ileti_okundumu]
              ,[okunma_tarihi]
              ,[is_deleted]
              ,[ileti_basligi]
              ,[ileti]
              ,[gonderim_tarihi]
              ,[zamani]
              ,[to_alici_isimleri]
              ,[mesaj_ad]
            from 
                gnl_ileti_gelen_kutusu_v 
            Where ( kullanici_uid='{0}')
                {1}
                And (IsNull(is_deleted,0)={2})
            ORDER BY 
                gonderim_tarihi DESC";
            return BaseDB.DBManager.AppConnection.GetDataSet(string.Format(query, kullanici_uid.ToString(), filter, deleted_flag ? "1" : "0"));
        }

        public static DataSet GelenMesajlarDashBoard(Guid kullanici_uid, string filter, bool deleted_flag = false)
        {
            string query =
            @"Select Top 5
               [ileti_uid]
              ,[gonderen_adi]
              ,[kullanici_uid]
              ,[alici_tipi]
              ,[ileti_okundumu]
              ,[okunma_tarihi]
              ,[is_deleted]
              ,[ileti_basligi]
              ,[ileti]
              ,[gonderim_tarihi]
              ,[zamani]
              ,[to_alici_isimleri]
              ,[mesaj_ad]
            from 
                gnl_ileti_gelen_kutusu_v 
            Where ( kullanici_uid='{0}')
                {1}
                And (IsNull(is_deleted,0)={2})
            ORDER BY 
                gonderim_tarihi DESC";
            return BaseDB.DBManager.AppConnection.GetDataSet(string.Format(query, kullanici_uid.ToString(), filter, deleted_flag ? "1" : "0"));
        }
       
        public static DataSet GidenMesajlar(Guid gonderen_kullanici_uid,bool deleted_flag = false)
        {
            string query =
            @"SELECT 
                [ileti_uid]
              ,[ileti_basligi]
               ,[gonderen_adi]
              ,[ileti]
              ,[gonderim_tarihi]
              ,[zamani]
              ,[to_alici_isimleri]
              ,[mesaj_ad]
              ,[is_deleted]
              ,[deleted_at]
              ,[deleted_by]
          FROM 
            [dbo].[gnl_ileti_giden_kutusu_v]
            Where ( gonderen_kullanici_uid='{0}')
                And (IsNull(is_deleted,0)={1})
                and (alici_tipi <>'Z' )
            ORDER BY 
                gonderim_tarihi DESC";
            return BaseDB.DBManager.AppConnection.GetDataSet(string.Format(query, gonderen_kullanici_uid.ToString(), deleted_flag ? "1" : "0"));
        }

        public void ileti_maili_gonder(ArrayList mail_arr, ArrayList mail_ad_arr, ArrayList mail_soyad_arr, string applicationPath, DateTime mesaj_gonderim_tarihi, string konu , string mesaj,string gonderen_ad,string gonderen_soyad)
        {
            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();

            System.IO.StreamReader template = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"Anket/Templates\MesajMailTemplate.html");
            string mailBody = template.ReadToEnd();
            template.Close();
            for (int i = 0; i < mail_arr.Count; i++)
            {
                string tempMailBody = mailBody;
                tempMailBody = tempMailBody.Replace("%%path_url%%", applicationPath);
                tempMailBody = tempMailBody.Replace("%%isim%%", mail_ad_arr[i].ToString() + " " + mail_soyad_arr[i].ToString());
                tempMailBody = tempMailBody.Replace("%%tarih%%", mesaj_gonderim_tarihi.ToString());
                tempMailBody = tempMailBody.Replace("%%konu%%", konu);
                tempMailBody = tempMailBody.Replace("%%mesaj%%", mesaj);
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


        public void uyelere_ileti_maili_gonder(ArrayList mail_arr, ArrayList mail_ad_arr, ArrayList mail_soyad_arr,ArrayList mail_user_uid_arr, string applicationPath, DateTime mesaj_gonderim_tarihi, string konu, string mesaj, string gonderen_ad, string gonderen_soyad)
        {
            System.IO.StreamReader template = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"Anket/Templates\MesajMailTemplate.html");
            string mailBody = template.ReadToEnd();
            template.Close();
            for (int i = 0; i < mail_arr.Count; i++)
            {
                gnl_ileti ileti = new gnl_ileti();
                ileti.ileti_basligi = konu;
                ileti.ileti = mesaj;
                ileti.gonderen_kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;

                Guid[] arr_guid = new Guid[1];
                arr_guid[0] = Guid.Parse(mail_user_uid_arr[i].ToString());

                ileti.AlicilarEkle(arr_guid, "Z");

                if (ileti.IsValid)
                {
                    ileti.Gonder();
                    this.IletiEkle(ileti);
                    this.Kaydet();
                }

                string tempMailBody = mailBody;
                tempMailBody = tempMailBody.Replace("%%path_url%%", applicationPath);
                tempMailBody = tempMailBody.Replace("%%isim%%", mail_ad_arr[i].ToString() + " " + mail_soyad_arr[i].ToString());
                tempMailBody = tempMailBody.Replace("%%tarih%%", mesaj_gonderim_tarihi.ToString());
                tempMailBody = tempMailBody.Replace("%%konu%%", konu);
                tempMailBody = tempMailBody.Replace("%%mesaj%%", mesaj);
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
        public gnl_kullanicilar KullaniciGetir(Guid user_uid)
        {
            return db.gnl_kullanicilar.SingleOrDefault(d => d.user_uid == user_uid);
        }

        public gnl_kullanicilar KullaniciGetirGrubaGore(Guid grup_uid)
        {
            return db.gnl_kullanicilar.SingleOrDefault(d => d.grup_uid == grup_uid);
        }

        public gnl_kullanicilar KullaniciGetirEmaileGore(string email)
        {
            return db.gnl_kullanicilar.SingleOrDefault(d => d.email == email);
        }

        public void KullaniciEkle(gnl_kullanicilar kullanici)
        {
            kullanici.user_uid = Guid.NewGuid();
            db.gnl_kullanicilar.AddObject(kullanici);
        }
        #endregion

        #region Üye Kullanıcı İşlemleri
        public gnl_uye_kullanicilar UyeKullaniciGetir(Guid user_uid)
        {
            return db.gnl_uye_kullanicilar.SingleOrDefault(d => d.user_uid == user_uid);
        }

        public gnl_uye_kullanicilar UyeKullaniciGetirGrubaGore(Guid grup_uid)
        {
            return db.gnl_uye_kullanicilar.SingleOrDefault(d => d.grup_uid == grup_uid);
        }

        public gnl_uye_kullanicilar UyeKullaniciGetirEmaileGore(string email)
        {
            return db.gnl_uye_kullanicilar.SingleOrDefault(d => d.email == email);
        }

        public void UyeKullaniciEkle(gnl_uye_kullanicilar kullanici)
        {
            kullanici.id = Guid.NewGuid();
            db.gnl_uye_kullanicilar.AddObject(kullanici);
        }

        public bool IsPurchasedUser(Guid user_uid)
        {
            bool result = false;

            gnl_uye_kullanicilar uye = this.UyeKullaniciGetir(user_uid);

            if (uye != null)
            {
                if (uye.aktif != false && uye.uye_baslangic_tarihi != null && uye.uye_bitis_tarihi != null)
                {
                    if (uye.uye_baslangic_tarihi <= DateTime.Now && uye.uye_bitis_tarihi >= DateTime.Now)
                        result = true;
                }
            }


            return result;
        }


        public bool IsPurchasedUserGrubaGore(Guid grup_uid)
        {
            bool result = false;

            gnl_uye_kullanicilar uye = this.UyeKullaniciGetirGrubaGore(grup_uid);

            if (uye != null)
            {
                if (uye.aktif != false && uye.uye_baslangic_tarihi != null && uye.uye_bitis_tarihi != null)
                {
                    if (uye.uye_baslangic_tarihi <= DateTime.Now && uye.uye_bitis_tarihi >= DateTime.Now)
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
                gnl_uye_kullanicilar uye = this.UyeKullaniciGetir(user_uid);

                if(uye!=null)
                {
                    if(uye.son_odeme_tipi_id!=null)
                        result = Convert.ToInt32(uye.son_odeme_tipi_id.ToString());
                }
            }
            
            return result;
        }

        public DataSet SistemAdminKullanicilar()
        {
            return BaseDB.DBManager.AppConnection.GetDataSet("Select * From gnl_kullanicilar Where aktif = 1 and is_sistem_admin=1");
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
                        gnl_uye_kullanicilar uyeler = gnlDB.UyeKullaniciGetir(try_result);
                        if (uyeler != null)
                        {
                            string applicationPath = "";
                            if (System.Web.HttpContext.Current.Request.ApplicationPath == "/")
                                applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/";
                            else
                                applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/" + System.Web.HttpContext.Current.Request.ApplicationPath + "/";

                            uyelik_bitis_bilgilendirme_maili_gonder(applicationPath,  Guid.Parse(uyeler.user_uid.ToString()), uyeler.email, uyeler.ad, uyeler.soyad, Convert.ToDateTime(uyeler.uye_bitis_tarihi.ToString(),new System.Globalization.CultureInfo("tr-TR")));
                            
                            gnl_uyelik_bitis_bilgilendirme_mail_gonderi_tarihcesi tarihce=new gnl_uyelik_bitis_bilgilendirme_mail_gonderi_tarihcesi();
                            
                            this.UyelikBitisBilgilendirmeMailGonderiEkle(tarihce);
                            tarihce.gonderen_user_uid = Guid.Empty;
                            tarihce.gonderilen_ad = uyeler.ad;
                            tarihce.gonderilen_email = uyeler.email;
                            tarihce.gonderilen_soyad = uyeler.soyad;
                            tarihce.gonderilme_tarihi = DateTime.Now;
                            tarihce.user_uid = uyeler.user_uid;

                            this.Kaydet();
                        }
                    }
                }

            }

        }

        public void uyelik_bitis_bilgilendirme_maili_gonder(string applicationPath, Guid user_uid, string user_email, string user_ad, string user_soyad,DateTime tarih)
        {
            AnketRepository ankDB = RepositoryManager.GetRepository<AnketRepository>();

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

        #region Kullanıcı Grup İşlemleri
        public gnl_kullanici_gruplari KullaniciGrubuGetir(Guid grup_uid)
        {
            return db.gnl_kullanici_gruplari.SingleOrDefault(d => d.grup_uid == grup_uid);
        }

        public void KullaniciGrubuEkle(gnl_kullanici_gruplari grup)
        {
            grup.grup_uid = Guid.NewGuid();
            db.gnl_kullanici_gruplari.AddObject(grup);
        }

        public DataSet KullaniciGrubuGetirDataSet(int grup_durum_id)
        {
            DataSet ds_result = new DataSet();
            if (grup_durum_id == 0)
                ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from gnl_kullanici_gruplari_v where kullanici_uid='" + BaseDB.SessionContext.Current.ActiveUser.UserUid + "' and aktif=1 and aktif_kullanici=1");
            else if(grup_durum_id == 1)
                ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from gnl_kullanici_gruplari_v where kullanici_uid='" + BaseDB.SessionContext.Current.ActiveUser.UserUid + "' and aktif=1 and aktif_kullanici=1 and is_admin=1");
            else if (grup_durum_id == 2)
                ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from gnl_kullanici_gruplari_v where kullanici_uid='" + BaseDB.SessionContext.Current.ActiveUser.UserUid + "' and aktif=1 and aktif_kullanici=1 and (is_admin is null or is_admin=0)");

            return ds_result;
        }

        public bool HasKullaniciGrup(Guid user_uid)
        {
            bool result = false;

            
            
            string ks = BaseDB.DBManager.AppConnection.ExecuteSql("select count(*) from dbo.sbr_anket_kullanici_gruplari('" + BaseDB.SessionContext.Current.ActiveUser.UserUid + "')");

            if (ks != "" && ks != "0")
                result = true;

            return result;
        }

        public DataSet GrubunKullanicilariGetirDataSet(Guid grup_uid)
        {
            DataSet ds_result = new DataSet();
            ds_result = BaseDB.DBManager.AppConnection.GetDataSet("select * from gnl_kullanici_gruplari_v where grup_uid='" + grup_uid + "' and aktif_kullanici=1");
            
            return ds_result;
        }
        #endregion

        #region Grup Kullanıcıları İşlemleri
        public void GrupKullaniciEkle(gnl_grup_kullanici_tanimlari grup)
        {
            grup.id = Guid.NewGuid();
            db.gnl_grup_kullanici_tanimlari.AddObject(grup);
        }

        public gnl_grup_kullanici_tanimlari GrupKullaniciGetir(Guid grup_uid, Guid kullanici_uid)
        {
            return db.gnl_grup_kullanici_tanimlari.SingleOrDefault(d => d.grup_uid == grup_uid && d.kullanici_uid == kullanici_uid);
        }

        public bool IsGrupKullaniciAdmin(Guid grup_uid, Guid kullanici_uid)
        {
            bool result=false;
            gnl_grup_kullanici_tanimlari gk = db.gnl_grup_kullanici_tanimlari.SingleOrDefault(d => d.grup_uid == grup_uid && d.kullanici_uid == kullanici_uid);

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
            gnl_kullanicilar kullanicilar = new gnl_kullanicilar();

            if (sifre != null && (sifre.sifirlama_kabul_edildi == null))
            {
                kullanicilar = this.KullaniciGetirEmaileGore(sifre.sifre_sifirlanacak_email);

                if (kullanicilar != null)
                {

                    sifre.sifirlama_kabul_edildi = true;
                    sifre.sifirlama_kabul_edilme_tarihi = DateTime.Now;
                    this.Kaydet();

                    kullanicilar.sifre = sifre.sifre;
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

            gnl_uye_kullanicilar uye_kullanicilar = this.UyeKullaniciGetir(user_id);
            DateTime dt = DateTime.Now.AddMonths(uyelik_sure);
            int kalan_anket_sayisi = 0;
            if (uye_kullanicilar != null)
            {
                dt = (uye_kullanicilar.uye_bitis_tarihi.HasValue) ? Convert.ToDateTime(uye_kullanicilar.uye_bitis_tarihi.ToString(), new System.Globalization.CultureInfo("tr-TR")).AddMonths(uyelik_sure).Date : DateTime.Now.AddMonths(uyelik_sure).Date;
                kalan_anket_sayisi = (uye_kullanicilar.kalan_anket_sayisi.HasValue && (this.IsPurchasedUser(user_id))) ? Convert.ToInt32(uye_kullanicilar.kalan_anket_sayisi) + ((this.IsPurchasedUser(user_id)) ? anket_sayisi : 0) : anket_sayisi;
            }
            else
            {
                kalan_anket_sayisi = anket_sayisi;
            }

            return "Üyelik Bitiş Tarihi : " + dt.Date.ToString() + " , Toplam Yapılacak Anket Sayısı : " + kalan_anket_sayisi.ToString() + " , Paket Ücreti : " + anket_ucret.ToString() + " TL";

        }

        public string VarUyelikDurumuTextBelirle(Guid user_id)
        {
            string result = "";
            DataSet ds = BaseDB.DBManager.AppConnection.GetDataSet("select gnl_uye_kullanicilar.*,'Üyelik Bitiş Tarihi '+cast(day(gnl_uye_kullanicilar.uye_bitis_tarihi) as varchar(2))+'/'+cast(month(gnl_uye_kullanicilar.uye_bitis_tarihi) as varchar(2))+'/'+cast(year(gnl_uye_kullanicilar.uye_bitis_tarihi) as varchar(4))+' - '+cast (gnl_uye_kullanicilar.kalan_anket_sayisi as varchar(20))+' Anket - Anket Başına '+cast (gnl_uyelik_odeme_tipleri_tanimlari.katilimci_sayisi as varchar(20)) +' Katılımcı'  as uyelik_durumu  from gnl_uye_kullanicilar,gnl_uyelik_odeme_tipleri_tanimlari where odeme_tipi_id=son_odeme_tipi_id  and user_uid='" + BaseDB.SessionContext.Current.ActiveUser.UserUid + "'");

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
            AnketRepository ankDB = RepositoryManager.GetRepository<AnketRepository>();

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
            AnketRepository ankDB = RepositoryManager.GetRepository<AnketRepository>();

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

        public int OdemetipiAnketSayisiGetir(int odeme_tipi_id)
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

        #region Duyurular
        public void DuyuruEkle(gnl_duyurular duyuru)
        {
            duyuru.duyuru_uid = Guid.NewGuid();
            db.gnl_duyurular.AddObject(duyuru);
        }
        public gnl_duyurular DuyuruGetir(Guid duyuru_uid)
        {
            return db.gnl_duyurular.SingleOrDefault(d => d.duyuru_uid == duyuru_uid);
        }

        public DataSet DuyuruDataSet()
        {
            return BaseDB.DBManager.AppConnection.GetDataSet("Select * From gnl_duyurular order by duyuru_tarihi desc");
        }

        public DataSet DuyuruDataSetDashBoard()
        {
            return BaseDB.DBManager.AppConnection.GetDataSet("Select top 5 * From gnl_duyurular where duyuru_durumu_id=1 order by duyuru_tarihi desc");
        }
        #endregion

        #region Arkadaşıma Öner
        public void ArkadasimaOnerEkle(gnl_arkadasima_oner oner)
        {
            oner.id = Guid.NewGuid();
            db.gnl_arkadasima_oner.AddObject(oner);
        }
        public gnl_arkadasima_oner ArkadasimaOnerGetir(Guid id)
        {
            return db.gnl_arkadasima_oner.SingleOrDefault(d => d.id == id);
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