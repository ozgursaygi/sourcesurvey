using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace BaseDB
{
    public class DBManager
    {
        public class ConnectionTypes
        {
            // yeni connection türleride enumerate edilmeli
            public const string AppConnection = "AppConnectionString";
            public const string LojConnection = "LojConnectionString";
            public static IEnumerable<string> ConnectionStrings()
            {
                yield return AppConnection;
                yield return LojConnection;
            }
        }
        /// <summary>
        /// Asıl Connection, uygulamada ikincil bir veritabanına bağlanılmadığı sürece bu kullanılır.
        /// </summary>
        /// <returns></returns>
        public static MsSqlConnection AppConnection
        {
            get
            {
                return DBManager.GetConnectionFromCurrentContext(DBManager.ConnectionTypes.AppConnection);
            }
        }
        public static MsSqlConnection LojConnection
        {
            get
            {
                return DBManager.GetConnectionFromCurrentContext(DBManager.ConnectionTypes.LojConnection);
            }
        }
        public static MsSqlConnection GetConnectionFromCurrentContext(string connectionKey)
        {
            MsSqlConnection conn;

            if (HttpContext.Current == null)
            {
                conn = new MsSqlConnection(connectionKey);
                conn.Connect();
                return conn;
            }

            if (HttpContext.Current.Items[connectionKey] == null)
            {
                conn = new MsSqlConnection(connectionKey);
                conn.Connect();
                HttpContext.Current.Items[connectionKey] = conn;
                return conn;
            }
            else
                return (MsSqlConnection)HttpContext.Current.Items[connectionKey];
        }
    }
}
