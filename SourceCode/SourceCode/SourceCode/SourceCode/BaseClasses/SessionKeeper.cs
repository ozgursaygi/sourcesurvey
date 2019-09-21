using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace BaseClasses
{
    [Serializable]
    public class SessionInfo
    {
        public string Key { get; set; }
        public string UsersHostAddress { get; set; }
        public string UsersHostName { get; set; }
        public string Browser { get; set; }
        public string Platform { get; set; }
        public string Version { get; set; }
        public DateTime StartDate { get; set; }
        public string ActiveUserName { get; set; }
        public string ActiveUserNameSurname { get; set; }
        public string ActiveUserPersonelId { get; set; }
        public string ActiveUserGrupId { get; set; }


    }

    public class SessionKeeper
    {
        private SessionKeeper() { }
        static private SessionKeeper theInstance;

        static public SessionKeeper Instance
        {
            get
            {
                if (SessionKeeper.theInstance == null)
                    SessionKeeper.theInstance = new SessionKeeper();
                return SessionKeeper.theInstance;
            }
        }
        private System.Collections.Generic.Dictionary<string, SessionInfo> _activeSessions = new System.Collections.Generic.Dictionary<string, SessionInfo>();

        public static Dictionary<string, SessionInfo> ActiveSessions
        {
            get
            {
                return Instance._activeSessions;
            }
        }

        public static void AddCurrentSession()
        {
            HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;

            SessionInfo si = new SessionInfo();
            si.Key = HttpContext.Current.Session.SessionID;
            si.UsersHostName = HttpContext.Current.Request.UserHostName;
            si.UsersHostAddress = HttpContext.Current.Request.UserHostAddress;
            si.Browser = bc.Browser;
            si.Platform = bc.Platform;
            si.Version = bc.Version;
            si.ActiveUserName = BaseDB.SessionContext.Current.LoginUser.UserName;
            si.StartDate = DateTime.Now;
            si.ActiveUserNameSurname = BaseDB.SessionContext.Current.LoginUser.UserNameAndSurname;
            si.ActiveUserPersonelId = BaseDB.SessionContext.Current.LoginUser.UserUid.ToString();
            si.ActiveUserGrupId = BaseDB.SessionContext.Current.LoginUser.GrupUid.ToString();
            
            RemoveSession(HttpContext.Current.Session.SessionID);
            Instance._activeSessions.Add(HttpContext.Current.Session.SessionID, si);
        }

        public static void AddLoggedInUserToDataBase(string type)
        {
            if (BaseDB.SessionContext.Current == null)
                return;

            HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;

            StringBuilder str = new StringBuilder();
            str.Append("'" + type + "'");
            str.Append(",");
            str.Append("'" + BaseDB.SessionContext.Current.LoginUser.UserUid.ToString() + "'");
            str.Append(",");
            str.Append("'" + BaseDB.SessionContext.Current.LoginUser.UserName + "'");
            str.Append(",");
            str.Append("'" + BaseDB.SessionContext.Current.LoginUser.UserNameAndSurname + "'");
            str.Append(",");
            str.Append("getdate()");
            str.Append(",");
            str.Append("'" + HttpContext.Current.Session.SessionID + "'");
            str.Append(",");
            str.Append("'" + HttpContext.Current.Request.UserHostName + "'");
            str.Append(",");
            str.Append("'" + HttpContext.Current.Request.UserHostAddress + "'");
            str.Append(",");
            str.Append("'" + bc.Browser + "'");
            str.Append(",");
            str.Append("'" + bc.Platform + "'");
            str.Append(",");
            str.Append("'" + bc.Version + "'");

            BaseDB.DBManager.AppConnection.ExecuteSql("insert into BaseLoggedInUsers values (" + str.ToString() + ")");
        }

        public static void RemoveSession(string SessionID)
        {
            try
            {
                Instance._activeSessions.Remove(SessionID);
            }
            catch
            {
            }
        }


    }
}
