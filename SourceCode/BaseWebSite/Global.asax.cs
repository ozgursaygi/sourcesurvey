using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Globalization;
using BaseDB;

namespace BaseWebSite
{
    public class Global : System.Web.HttpApplication
    {

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup

        }

        void Application_End(object sender, EventArgs e)
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory + "Temp");
            foreach (System.IO.DirectoryInfo subFolder in di.GetDirectories())
            {
                try
                {
                    ClearDirectory(subFolder);
                    subFolder.Delete();
                }
                catch { }
            }
            ClearDirectory(di);
            //di = new System.IO.DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory + "ChartImages");
            //ClearDirectory(di);

            //di = new System.IO.DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory + "CaptchaImages");
            //ClearDirectory(di);

        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            Dictionary<object, object> secilenler = new Dictionary<object, object>();

            foreach (System.Collections.DictionaryEntry item in Context.Items)
            {
                if (item.Value is IDbConnection || item.Value is IRepository)
                {
                    System.Diagnostics.Debug.WriteLine("Disposing:" + item.Key.ToString());
                    ((IDisposable)item.Value).Dispose();
                    secilenler.Add(item.Key, item.Value);
                }
                else
                    System.Diagnostics.Debug.WriteLine("Not Touching:" + item.Key.ToString());
            }
            foreach (var item in secilenler)
                Context.Items[item.Key] = null;
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            string languagePref = "tr-TR";

            try
            {
                if (HttpContext.Current.Session != null)
                    languagePref = BaseDB.SessionContext.Current.UICulture;
            }
            catch { }
            if (languagePref == "")
                languagePref = "tr-TR";


            try
            {
                CultureInfo ci = new CultureInfo(languagePref);
                System.Threading.Thread.CurrentThread.CurrentUICulture = ci;
            }
            catch
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("tr-TR");
            }
        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started

        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

            if (HttpContext.Current != null && HttpContext.Current.User != null)
            {
                FormsAuthentication.SignOut();
            }
   
            //BaseClasses.SessionKeeper.RemoveSession(Session.SessionID);

        }

        private void ClearDirectory(System.IO.DirectoryInfo di)
        {
            foreach (System.IO.FileInfo fi in di.GetFiles())
            {
                try
                {
                    fi.Delete();
                }
                catch { }
            }
        }

    }
}
