using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

namespace BaseDB
{
    public abstract class BaseConnection : IDbConnection
    {
        protected static int instanceCounter = 0;
        protected static int connections = 0;
        protected static int connectionsOpened = 0;
        protected static int connectionsClosed = 0;

        private DbConnection connection = null;
        public abstract  DbConnection CreateConnection();
        
        public BaseConnection()
        {
            BaseDB.BaseConnection.instanceCounter++;
        }
        public bool isOpen
        {
            get { return (connection != null && (int)connection.State == (int)Definitions.ConnectionState.Open); }
        }

        public bool canClose
        {
            get { return (isOpen); }
        }
        public void Connect()
        {
            if (this.Connection == null)
            {
                this.Connection = CreateConnection();
                connections++;
            }
            Open();
        }

        public DbConnection Connection
        {
            get { return connection; }
            set { connection = value; }
        }

        public bool Open()
        {
            if (Connection != null && !isOpen)
            {
                Connection.Open();
                connectionsOpened++;

                if ((BaseDB.BaseConnection.connectionsOpened % 10) == 0)
                    BaseDB.BaseConnection.LogConnectionCounters();
                return true;
            }
            return false;
        }

        public bool Close()
        {
            if (canClose)
            {
                Connection.Close();
                connectionsClosed++;
                /*if ((BaseDB.BaseConnection.connectionsOpened % 10) == 0)
                    BaseDB.BaseConnection.LogConnectionCounters();*/
                return true;
            }
            return false;
        }

        #region Transaction stuff
        public abstract DbTransaction CreateTransactionContext(System.Data.IsolationLevel isolationLevel);
        public DbTransaction Tran { get; set; }
        public System.Data.IsolationLevel IsolationLevel
        {
            get { return (Tran != null) ? Tran.IsolationLevel : IsolationLevel.Unspecified; }
        }

        public void BeginTransaction()
        {
            this.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
        }

        public void BeginTransaction(System.Data.IsolationLevel isolationLevel)
        {
            if (Connection != null && isOpen)
                Tran = CreateTransactionContext(isolationLevel);
            else
                if(Open())
                    Tran = CreateTransactionContext(isolationLevel);
        }

        public void CommitTransaction()
        {
            Tran.Commit();
        }

        public void RollbackTransaction()
        {
            Tran.Rollback();
        }
        #endregion
        //diagnostics
        public static void LogConnectionCounters()
        {
           
        }

        public void Dispose()
        {
            Close();
            connection = null;
        }

    }
}
