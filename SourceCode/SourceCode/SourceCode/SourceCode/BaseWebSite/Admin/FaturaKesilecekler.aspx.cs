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
    public partial class FaturaKesilecekler : System.Web.UI.Page
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
                            paket.fatura_kesildi = true;
                            gnlDB.Kaydet();

                            //DataSet ds_result_havale = new DataSet();

                            //ds_result_havale = BaseDB.DBManager.AppConnection.GetDataSet("select * from gnl_paket_alimlari_v where id='" + paket.id + "'");
                            //string paket_durumu = "Paket Tipi : ";
                            //if (ds_result_havale.Tables[0].Rows.Count > 0)
                            //{
                            //    if (ds_result_havale.Tables[0].Rows[0]["odeme_tipi"] != System.DBNull.Value)
                            //        paket_durumu += ds_result_havale.Tables[0].Rows[0]["odeme_tipi"].ToString();

                            //    if (ds_result_havale.Tables[0].Rows[0]["odeme_sekli"] != System.DBNull.Value)
                            //        paket_durumu += " - " + ds_result_havale.Tables[0].Rows[0]["odeme_sekli"].ToString();

                            //    if (ds_result_havale.Tables[0].Rows[0]["paket_fiyati_str"] != System.DBNull.Value)
                            //        paket_durumu += " - " + ds_result_havale.Tables[0].Rows[0]["paket_fiyati_str"].ToString();
                            //}

                            //string applicationPath = "";
                            //if (System.Web.HttpContext.Current.Request.ApplicationPath == "/")
                            //    applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/";
                            //else
                            //    applicationPath = "http://" + System.Web.HttpContext.Current.Request.ServerVariables["HTTP_HOST"] + "/" + System.Web.HttpContext.Current.Request.ApplicationPath + "/";

                            //gnlDB.uyelik_aktivasyon_maili_gonder(uye_kullanicilar.ad, uye_kullanicilar.soyad, uye_kullanicilar.email, applicationPath, Convert.ToDateTime(paket.paket_alim_tarihi.ToString()), paket.id, paket_durumu);
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
            DataSet ds = gnlDB.PaketAlimlariFaturaKesilecekGetirDataSet();

            this.RepeaterPaging1.dataSource = ds;
            this.RepeaterPaging1.SayfaBuyuklugu = 10;
            this.RepeaterPaging1.Bind();
            this.rptSurveyler.DataSource = this.RepeaterPaging1.Sayfa;
            this.rptSurveyler.DataBind();
        }
    }
}