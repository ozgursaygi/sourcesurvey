using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BaseWebSite.Models;

namespace BaseWebSite.Admin
{
    public partial class OdemeAktiveEt : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (BaseDB.SessionContext.Current == null || BaseDB.SessionContext.Current.ActiveUser == null)
            {
                Response.Redirect("~/Login.aspx");
            }

            if (!IsPostBack)
            {
                BindPaketler();
            }

            if (Request["__EVENTTARGET"] == "AktiveEt")
            {
                string id = "";
                if (Request["__EVENTARGUMENT"] != null)
                {
                    id = Request["__EVENTARGUMENT"].ToString();

                    if (id != "")
                    {
                        GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();
                        gnl_uyelik_paket_alimlari paket = gnlDB.UyePaketAlimiGetir(Guid.Parse(id));

                        if (paket != null)
                        {
                            gnl_uye_kullanicilar member_users = gnlDB.GetMemberUsers(Guid.Parse(paket.user_uid.ToString()));

                            member_users.uye_bitis_tarihi = DateTime.Now.AddMonths(Convert.ToInt32(paket.paket_suresi.ToString()));
                            member_users.uye_baslangic_tarihi = (member_users.uye_baslangic_tarihi.HasValue) ? Convert.ToDateTime(member_users.uye_baslangic_tarihi.ToString(), new System.Globalization.CultureInfo("tr-TR")) : DateTime.Now;
                            member_users.son_odeme_tipi_id = paket.odeme_tipi_id;
                            int kalan_anket_sayisi = (member_users.kalan_anket_sayisi.HasValue && (gnlDB.IsPurchasedUser(Guid.Parse(paket.user_uid.ToString())))) ? Convert.ToInt32(member_users.kalan_anket_sayisi) + ((gnlDB.IsPurchasedUser(Guid.Parse(paket.user_uid.ToString()))) ? Convert.ToInt32(paket.anket_sayisi.ToString()) : 0) : Convert.ToInt32(paket.anket_sayisi.ToString());

                            member_users.kalan_anket_sayisi = kalan_anket_sayisi;
                            member_users.aktif = true;
                            gnlDB.Kaydet();

                            paket.aktive_edildi = true;
                            gnlDB.Kaydet();

                            DataSet ds_result_havale = new DataSet();

                            ds_result_havale = BaseDB.DBManager.AppConnection.GetDataSet("select * from gnl_paket_alimlari_v where id='" + paket.id + "'");
                            string paket_durumu = "Paket Tipi : ";
                            if (ds_result_havale.Tables[0].Rows.Count > 0)
                            {
                                if (ds_result_havale.Tables[0].Rows[0]["anket_sayisi"] != System.DBNull.Value)
                                    paket_durumu += ds_result_havale.Tables[0].Rows[0]["anket_sayisi"].ToString() + " Survey ";

                                if (ds_result_havale.Tables[0].Rows[0]["odeme_tipi"] != System.DBNull.Value)
                                    paket_durumu += " - " + ds_result_havale.Tables[0].Rows[0]["odeme_tipi"].ToString();

                                if (ds_result_havale.Tables[0].Rows[0]["odeme_sekli"] != System.DBNull.Value)
                                    paket_durumu += " - " + ds_result_havale.Tables[0].Rows[0]["odeme_sekli"].ToString();

                                if (ds_result_havale.Tables[0].Rows[0]["paket_fiyati_str"] != System.DBNull.Value)
                                    paket_durumu += " - " + ds_result_havale.Tables[0].Rows[0]["paket_fiyati_str"].ToString();
                            }

                            string applicationPath = "";
                            if (System.Web.HttpContext.Current.Request.ApplicationPath == "/")
                                applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/";
                            else
                                applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/" + System.Web.HttpContext.Current.Request.ApplicationPath + "/";

                            gnlDB.uyelik_aktivasyon_maili_gonder(member_users.ad, member_users.soyad, member_users.email, applicationPath, Convert.ToDateTime(paket.paket_alim_tarihi.ToString()), paket.id,paket_durumu);
                        }
                    }
                }
            }
        }

        protected new void Page_LoadComplete(object sender, EventArgs e)
        {
            BindPaketler();
        }

        protected void BindPaketler()
        {
            GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();
            DataSet ds = gnlDB.PaketAlimlariHavaleAktiveOlacakGetirDataSet();

            this.RepeaterPaging1.dataSource = ds;
            this.RepeaterPaging1.SayfaBuyuklugu = 10;
            this.RepeaterPaging1.Bind();
            this.rptSurveyler.DataSource = this.RepeaterPaging1.Sayfa;
            this.rptSurveyler.DataBind();
        }
    }
}