using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseDB;
using System.Data.SqlClient;
using System.Data;

namespace SurveyDataAccess
{
    public class anket_dataaccess : BaseDB.BaseDataAccess
    {
        public void SurveyExcelDosyaEkle(Guid id, byte[] dosya)
        {
            BaseDB.BaseAdapter Adapter = new BaseAdapter();
            BaseCommand cmn = new BaseCommand(MsConn);
            cmn.CommandType = System.Data.CommandType.Text;
            cmn.CommandText = "Update sbr_mail_gruplari_dosyalari set dosya = @dosya where id='" + id + "'";
            SqlParameter UploadedImage = new SqlParameter("@dosya", SqlDbType.Image, dosya.Length);
            UploadedImage.Value = dosya;
            cmn.Command.Parameters.Add(UploadedImage);
            cmn.Command.ExecuteNonQuery();
        }

        public void SurveyLogoEkle(Guid anket_uid, byte[] logo)
        {
            BaseDB.BaseAdapter Adapter = new BaseAdapter();
            BaseCommand cmn = new BaseCommand(MsConn);
            cmn.CommandType = System.Data.CommandType.Text;
            cmn.CommandText = "Update sbr_anket set logo = @logo where anket_uid='" + anket_uid + "'";
            SqlParameter UploadedImage = new SqlParameter("@logo", SqlDbType.Image, logo.Length);
            UploadedImage.Value = logo;
            cmn.Command.Parameters.Add(UploadedImage);
            cmn.Command.ExecuteNonQuery();
        }

        public void SurveyLogoyuSil(Guid anket_uid)
        {
            BaseDB.BaseAdapter Adapter = new BaseAdapter();
            BaseCommand cmn = new BaseCommand(MsConn);
            cmn.CommandType = System.Data.CommandType.Text;
            cmn.CommandText = "Update sbr_anket set logo = null where anket_uid='" + anket_uid + "'";
            cmn.Command.ExecuteNonQuery();
        }
    }
}
