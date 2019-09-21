using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

namespace BaseDB
{

    public interface ITransactionManager : IDisposable
    {
        IsolationLevel IsolationLevel { get; }
        SqlTransaction Tran { get; }

        void BeginTransaction(IsolationLevel isolationLevel);
        void CommitTransaction();
        void RollbackTransaction();

    }
}
