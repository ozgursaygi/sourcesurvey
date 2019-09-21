using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaseWebSite.Models;

namespace BaseWebSite.Admin
{
    public partial class YeniSablon : System.Web.UI.Page
    {
        public Guid sablon_uid
        {
            get { return (ViewState["sablon_uid"] != null ? Guid.Parse(ViewState["sablon_uid"].ToString()) : Guid.Empty); }
            set { ViewState["sablon_uid"] = value; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (BaseDB.SessionContext.Current == null || BaseDB.SessionContext.Current.ActiveUser == null)
            {
                Response.Redirect("~/Login.aspx");
            }

            if (!IsPostBack)
            {
                if (Request.QueryString["sablon_uid"] != null && Request.QueryString["sablon_uid"].ToString() != "")
                {
                    sablon_uid = Guid.Parse(Request.QueryString["sablon_uid"].ToString());
                }

                InitiliazeCombos();
                if (sablon_uid != null && sablon_uid != Guid.Empty && sablon_uid.ToString() != "") BindControls();


            }


            FormuDuzenle();

           
        }

        protected void FormuDuzenle()
        {
            
        }

        protected void InitiliazeCombos()
        {
            this.ddlKategori.DataSource = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_ref_anket_kategori");
            this.ddlKategori.DataTextField = "kategori_kodu";
            this.ddlKategori.DataValueField = "kategori_id";
            this.ddlKategori.DataBind();
        }


        protected void BindControls()
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            sbr_sablon sablon = ankDB.SablonGetir(sablon_uid);

            if (sablon.sablon_adi != null) this.txtSablonAdi.Text = sablon.sablon_adi;
            
            if (sablon.kategori_id != null) this.ddlKategori.SelectedValue = sablon.kategori_id.ToString();
            
        }

        protected void btnSablonOlustur_Click(object sender, EventArgs e)
        {
            

            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
            sbr_sablon sablon = new sbr_sablon();
            int sablon_durumu_id = -1;
            if (sablon_uid == null || sablon_uid == Guid.Empty)
            {
                ankDB.SablonEkle(sablon);
                sablon.sablon_durumu_id = (int)BaseWebSite.Models.sablon_durumu.Acik;
            }
            else
            {
                sablon = ankDB.SablonGetir(sablon_uid);
                sablon_durumu_id = Convert.ToInt32(sablon.sablon_durumu_id);
            }

            sablon.sablon_adi = this.txtSablonAdi.Text;
            
            sablon.kategori_id = Convert.ToInt32(ddlKategori.SelectedValue);
            
            sablon.olusturan_kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
            sablon.olusturma_tarihi = DateTime.Now;
            sablon.kullanici_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;

            sablon.grup_uid = Guid.Empty;


           

            ankDB.Kaydet();

            if (sablon_durumu_id == -1 || sablon_durumu_id != sablon.sablon_durumu_id)
            {
                sbr_sablon_durum_tarihcesi sablon_durumu = new sbr_sablon_durum_tarihcesi();

                ankDB.SablonDurumuEkle(sablon_durumu);
                sablon_durumu.sablon_durumu_id = sablon.sablon_durumu_id;
                sablon_durumu.sablon_uid = sablon.sablon_uid;
                sablon_durumu.durum_olusma_tarihi = DateTime.Now;
                sablon_durumu.durumu_olusturan_kullanici = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                ankDB.Kaydet();

                sablon.sablon_durumu_uid = sablon_durumu.sablon_durumu_uid;
                ankDB.Kaydet();
            }

           
            Response.Redirect("SablonDetayi.aspx?sablon_uid=" + sablon.sablon_uid);
        }
    }
}