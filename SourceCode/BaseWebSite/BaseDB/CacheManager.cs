using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.Web;
using System.Data;

namespace BaseDB
{
    public class CacheManager
    {
        public static CacheDependency FileDependency()
        {
            return  new CacheDependency(
                HttpContext.Current.Server.MapPath("cache_dep.txt"));
        }

        public static DataSet GetDataSetFromCache(string keyName, string sql)
        {
            DataSet ds = (DataSet)HttpContext.Current.Cache[keyName];
            if (ds == null)
            {
                ds = BaseDB.DBManager.AppConnection.GetDataSet(sql);
                HttpContext.Current.Cache.Insert(keyName, ds,
                    FileDependency(), DateTime.Now.AddMinutes(1d),
                    System.Web.Caching.Cache.NoSlidingExpiration);
            }
            return ds;
        }

    }
}
