using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using BaseWebSite.Models;
using System.Data.OleDb;
using System.Data;

namespace BaseWebSite.Survey
{
    public partial class ExceldenAktar : System.Web.UI.Page
    {
        public Guid mail_grubu_uid
        {
            get { return (ViewState["mail_grubu_uid"] != null ? Guid.Parse(ViewState["mail_grubu_uid"].ToString()) : Guid.Empty); }
            set { ViewState["mail_grubu_uid"] = value; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (BaseDB.SessionContext.Current == null || BaseDB.SessionContext.Current.ActiveUser == null)
            {
                Response.Redirect("~/Login.aspx");
            }

            if (Request.QueryString["mail_grubu_uid"] != null && Request.QueryString["mail_grubu_uid"].ToString() != "")
            {
                mail_grubu_uid = Guid.Parse(Request.QueryString["mail_grubu_uid"].ToString());
                this.Label2.Text = BaseDB.DBManager.AppConnection.ExecuteSql("select mail_grubu_adi from sbr_mail_gruplari where mail_grubu_uid='" + mail_grubu_uid + "'");
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            HttpPostedFile postedFile = this.fileuploadExcel.PostedFile;
            if (postedFile != null && postedFile.ContentLength > 0)
            {
                FileInfo clientFileInfo = new FileInfo(postedFile.FileName);

                string extension = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf(".") + 1);
                if (extension.ToLower() == "xls" || extension.ToLower() == "xlsx")
                {

                    SurveyBusiness.anket_business crm = new SurveyBusiness.anket_business();

                    byte[] dosya = new byte[postedFile.InputStream.Length];
                    postedFile.InputStream.Read(dosya, 0, (int)postedFile.InputStream.Length);

                    SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();
                    sbr_mail_gruplari_dosyalari dosyalar = new sbr_mail_gruplari_dosyalari();
                    ankDB.MailDosyalariEkle(dosyalar);
                    dosyalar.dosya_eklenma_tarihi = DateTime.Now;
                    dosyalar.dosya_ekleyen_user_uid = BaseDB.SessionContext.Current.ActiveUser.UserUid;
                    dosyalar.dosya_uzanti_adi = clientFileInfo.Name;
                    dosyalar.mail_grubu_uid = mail_grubu_uid;
                    ankDB.Kaydet();
                    crm.SurveyExcelDosyaEkle(dosyalar.id, dosya);
                    string directory = "", file_name = "";
                    string path = ShowDocument(dosyalar.id, dosya, ref directory, ref file_name);
                    this.hdn_id.Value = dosyalar.id.ToString();

                    string ext = System.IO.Path.GetExtension(path);
                    string strconn = "";

                    if (ext.ToLower() == ".xlsx")
                        strconn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Mode=ReadWrite;Extended Properties=\"Excel 12.0 Xml;HDR=NO;IMEX=1\"";
                    else if (ext.ToLower() == ".xls")
                        strconn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Mode=ReadWrite;Extended Properties=\"Excel 8.0;HDR=NO;IMEX=1\"";
                    else if (ext.ToLower() == ".csv")
                        strconn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + directory + ";Mode=ReadWrite;Extended Properties=\"Text;HDR=NO;FMT=Delimited\"";
                    else
                        return;

                    OleDbConnection oconn = new OleDbConnection(strconn);
                    oconn.Open();
                    dynamic myTableName = oconn.GetSchema("Tables").Rows[0]["TABLE_NAME"];
                    if (ext.ToLower() == ".csv") myTableName = file_name;
                    OleDbCommand ocmd = new OleDbCommand("select * from [" + myTableName + "]", oconn);

                    DataTable dt1 = new DataTable();
                    dt1.Load(ocmd.ExecuteReader());

                    if ((ext.ToLower() == ".csv"))
                    {
                        foreach (DataRow dr in dt1.Rows)
                        {
                            string[] arr = null;

                            if (dr[0].ToString().Contains(',') && !dr[0].ToString().Contains(';'))
                                arr = dr[0].ToString().Split(',');
                            else if (dr[0].ToString().Contains(';') && !dr[0].ToString().Contains(','))
                                arr = dr[0].ToString().Split(';');
                            else
                                break;

                            if (arr.Length == 3 && arr[0] != null && arr[1] != null && arr[2] != null && arr[0].ToString() != "" && arr[1].ToString() != "" && arr[2].ToString() != "")
                            {

                                if (BaseClasses.BaseFunctions.getInstance().IsEmailValid(arr[2].ToString()))
                                {
                                    ankDB.MailListesiOlustur(arr[0].ToString(), arr[1].ToString(), arr[2].ToString(), mail_grubu_uid.ToString());
                                    ankDB.Kaydet();
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (DataRow dr in dt1.Rows)
                        {

                            if (dr[0] != null && dr[1] != null && dr[2] != null && dr[0].ToString() != "" && dr[1].ToString() != "" && dr[2].ToString() != "")
                            {

                                if (BaseClasses.BaseFunctions.getInstance().IsEmailValid(dr[2].ToString()))
                                {
                                    ankDB.MailListesiOlustur(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), mail_grubu_uid.ToString());
                                    ankDB.Kaydet();
                                }
                            }
                        }

                    }
                    oconn.Close();

                  
                }

                ClientScript.RegisterStartupScript(typeof(string), "Close", "<script> alert('Excelden Aktarma İşlemi Yapılmıştır.'); </script>");
            }
            else
            {
                BaseClasses.BaseFunctions functions = new BaseClasses.BaseFunctions();
                ClientScript.RegisterStartupScript(typeof(string), "Close", "<script> alert('Lütfen Dosyayı seçiniz.'); </script>");
            }
        }

        private string ShowDocument(Guid uid, byte[] dosya,ref string directory,ref string file_name)
        {
            SurveyRepository ankDB = RepositoryManager.GetRepository<SurveyRepository>();

            var dosyalar = ankDB.MailDosyaGetir(uid);

            string folder = Guid.NewGuid().ToString();
            Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + "Temp\\" + folder);
            string serverPath = System.AppDomain.CurrentDomain.BaseDirectory + "Temp\\" + folder + "\\" + dosyalar.dosya_uzanti_adi;
            directory = System.AppDomain.CurrentDomain.BaseDirectory + "Temp\\" + folder + "\\";
            file_name = dosyalar.dosya_uzanti_adi;
            FileStream stream = new FileStream(serverPath, FileMode.CreateNew);
            stream.Write(dosya, 0, (int)dosya.Length);
            stream.Close();

            return serverPath;
            //Response.Redirect("~/Temp/" + folder + "/" + dosyalar.dosya_uzanti_adi);
            //ClientScript.RegisterStartupScript(typeof(string), "ShowFile", "<script> ShowLoading(); window.open('~/Temp/" + folder + "/" + fileTable[0].dosya_adi + "', 'ShowFile'); window.close(); </script>");
        }

        //public DataSet GetExcel(string fileName)
        //{
        //    Application oXL;
        //    Workbook oWB;
        //    Worksheet oSheet;
        //    Range oRng;
        //    try
        //    {
        //        //  creat a Application object
        //        oXL = new Microsoft.Office.Interop.Excel.Application();
        //        //   get   WorkBook  object
        //        oWB = oXL.Workbooks.Open(fileName, Missing.Value, Missing.Value,
        //                Missing.Value, Missing.Value, Missing.Value,
        //                Missing.Value, Missing.Value, Missing.Value,
        //                Missing.Value, Missing.Value, Missing.Value, Missing.Value,
        //                Missing.Value, Missing.Value);

        //        //   get   WorkSheet object
        //        oSheet = (Microsoft.Office.Interop.Excel.Worksheet)oWB.Sheets[1];
        //        System.Data.DataTable dt = new System.Data.DataTable("dtExcel");
        //        DataSet ds = new DataSet();
        //        ds.Tables.Add(dt);
        //        DataRow dr;

        //        StringBuilder sb = new StringBuilder();
        //        int jValue = oSheet.UsedRange.Cells.Columns.Count;
        //        int iValue = oSheet.UsedRange.Cells.Rows.Count;
        //        //  get data columns
        //        for (int j = 1; j <= jValue; j++)
        //        {
        //            dt.Columns.Add("column" + j, System.Type.GetType("System.String"));
        //        }

        //        //  get data in cell
        //        for (int i = 1; i <= iValue; i++)
        //        {
        //            dr = ds.Tables["dtExcel"].NewRow();
        //            for (int j = 1; j <= jValue; j++)
        //            {
        //                oRng = (Microsoft.Office.Interop.Excel.Range)oSheet.Cells[i, j];
        //                string strValue = oRng.Text.ToString();
        //                dr["column" + j] = strValue;
        //            }
        //            ds.Tables["dtExcel"].Rows.Add(dr);
        //        }
        //        return ds;
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.Write(ex.Message);
        //        return null;
        //    }
        //    finally
        //    {
        //        Dispose();
        //    }
        //}


    }
}