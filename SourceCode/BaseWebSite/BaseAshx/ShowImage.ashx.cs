using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data;

namespace BaseWebSite.BaseAshx
{
    /// <summary>
    /// Summary description for ShowImage
    /// </summary>
    public class ShowImage : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string anket_uid = "";
            if (context.Request.QueryString["anket_uid"] != null) anket_uid = context.Request.QueryString["anket_uid"].ToString();
            
            context.Response.ContentType = "image/jpeg";
            Stream strm = ShowEmpImage(anket_uid);

            if (strm != null)
            {
                byte[] buffer = new byte[4096];
                int byteSeq = strm.Read(buffer, 0, 4096);

                while (byteSeq > 0)
                {
                    context.Response.OutputStream.Write(buffer, 0, byteSeq);
                    byteSeq = strm.Read(buffer, 0, 4096);
                }
            }
        }

        public Stream ShowEmpImage(string anket_uid)
        {
            MemoryStream result_ms = new MemoryStream();
            DataSet ds = new DataSet();
           
           ds = BaseDB.DBManager.AppConnection.GetDataSet("select logo from sbr_anket where anket_uid='" + anket_uid + "'");
           
            try
            {
                result_ms = new MemoryStream((byte[])ds.Tables[0].Rows[0]["logo"]);
                
                return result_ms;
            }
            catch
            {
                return null;
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}