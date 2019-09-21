using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BaseDB;

namespace BaseWebSite.Models
{
    public class RepositoryManager
    {
        public static T GetRepository<T>(string namedInstance = null) where T : IRepository, new()
        {
            if (string.IsNullOrEmpty(namedInstance))
                namedInstance = typeof(T).FullName;

            if (HttpContext.Current == null)
            {
                return new T();
            }
            if (HttpContext.Current.Items[namedInstance] == null)
            {
                T repoInstance = new T();
                HttpContext.Current.Items[namedInstance] = repoInstance;
                return repoInstance;
            }
            else
                return (T)HttpContext.Current.Items[namedInstance];
        }
    }
}