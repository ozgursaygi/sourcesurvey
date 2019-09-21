using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Şu an için sadece Web Application Mode'unda çalışıyor ve instance'ını Http Session'ında tutuyor.
// Windows mode'unda da Singleton modeliyle çalışabilir.

namespace BaseDB
{
    [Serializable]
    public class BaseUserInfo
    {
        public Guid UserUid { get; set; }
        public string UserName { get; set; }
        public string UserNameAndSurname { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public Guid GrupUid { get; set; }
        public string GrupAdi { get; set; }
        public Guid SubeUid { get; set; }
        public int PersonelId { get; set; }
        public string UserEmail { get; set; }
        public bool IsSistemAdmin { get; set; }
    }
    [Serializable]
    public class SessionContext
    {
        private SessionContext()
        {
            //hide constructor
            LoginUser = new BaseUserInfo();
            _activeUser = null;
            ConnectionString = null;
        }
        public BaseUserInfo LoginUser { get; private set; }
        protected BaseUserInfo _activeUser;
        public BaseUserInfo ActiveUser
        {
            get
            {
                return _activeUser ?? LoginUser;
            }
        }


        public string UICulture { get; set; }
        public string Culture { get; set; }
        public string Theme { get;  set; }
        public string HostCompanyName { get; set; }
        public string ConnectionString { get; set; }

        public static SessionContext Current
        {
            get
            {
                return (SessionContext)System.Web.HttpContext.Current.Session["SessionContext"];
            }
        }
        
        public static SessionContext StartSession()
        {
            //clear current user object
            System.Web.HttpContext.Current.Session["SessionContext"] = null;

            SessionContext session = new SessionContext();
            System.Web.HttpContext.Current.Session["SessionContext"] = session;
            return session;
        }

        public void StartImpersination(Guid user_uid)
        {
            _activeUser = null;
            _activeUser = new BaseUserInfo();
            _activeUser.UserUid = user_uid;
        }
    }
}
