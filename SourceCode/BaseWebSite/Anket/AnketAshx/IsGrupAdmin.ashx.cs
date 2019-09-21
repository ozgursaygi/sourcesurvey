using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BaseWebSite.Models;

namespace BaseWebSite.Survey.SurveyAshx
{
    /// <summary>
    /// Summary description for IsGrupAdmin
    /// </summary>
    public class IsGrupAdmin : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string user_uid = "";
            string grup_uid = "";
            string result = "0";
            if (context.Request.QueryString["user_uid"] != null) user_uid = context.Request.QueryString["user_uid"].ToString();
            if (context.Request.QueryString["grup_uid"] != null) grup_uid = context.Request.QueryString["grup_uid"].ToString();

            if (user_uid != "" && grup_uid != "")
            {
                GenelRepository gnlDB = RepositoryManager.GetRepository<GenelRepository>();

                if (gnlDB.IsGrupUserAdmin(Guid.Parse(grup_uid.ToString()), Guid.Parse(user_uid)))
                {
                    result = "1";
                }
            }

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