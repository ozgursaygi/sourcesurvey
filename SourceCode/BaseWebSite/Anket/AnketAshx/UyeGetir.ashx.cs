using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace BaseWebSite.Survey
{
    /// <summary>
    /// Summary description for UyeGetir
    /// </summary>
    public class UyeGetir : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string q = "";
            if (System.Web.HttpContext.Current.Request.QueryString["term"] != null)
            {
                q = System.Web.HttpContext.Current.Request.Url.ToString().Split('?').FirstOrDefault(s => s.StartsWith("term=")).Substring(5);
                q = q.Replace('+', ' ');
                q = q.TrimEnd().TrimStart();
            }

            DataSet ds = BaseDB.DBManager.AppConnection.GetDataSet("Select user_uid,ad+' '+soyad as name From gnl_users where (ad+' '+soyad) like '%" + q + "%' and active=1");
            int index = 0;
            string result = "";
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                if (index > 0)
                    result += ",";

                result += "{ \"id\" :\"" + dr["user_uid"] + "\",\"value\":\"" + dr["name"] + "\"}";
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