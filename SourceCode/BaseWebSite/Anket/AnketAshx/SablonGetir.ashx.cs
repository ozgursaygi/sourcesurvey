using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace BaseWebSite.Survey.SurveyAshx
{
    /// <summary>
    /// Summary description for SablonGetir
    /// </summary>
    public class SablonGetir : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string q = "";
            if (System.Web.HttpContext.Current.Request.QueryString["kategori_id"] != null)
            {
                q = System.Web.HttpContext.Current.Request.Url.ToString().Split('?').FirstOrDefault(s => s.StartsWith("kategori_id=")).Substring(12);
                q = q.Replace('+', ' ');
                q = q.TrimEnd().TrimStart();
            }

            DataSet ds = BaseDB.DBManager.AppConnection.GetDataSet("select * from sbr_sablon where sablon_durumu_id=1 and kategori_id=" + q);
            int index = 0;
            string result = "";
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                if (index > 0)
                    result += ",";

                result += "{ \"id\" :\"" + dr["sablon_uid"] + "\",\"value\":\"" + dr["sablon_adi"] + "\"}";
                index++;
            }

            result = "[" + result + "]";

            context.Response.Write(result);
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