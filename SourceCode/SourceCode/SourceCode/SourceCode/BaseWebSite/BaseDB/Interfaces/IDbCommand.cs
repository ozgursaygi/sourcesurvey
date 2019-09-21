using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace BaseDB
{
    public interface IDbCommand
    {
        System.Data.CommandType CommandType { set; }
        string CommandText { set; }

        int ExecuteNonQuery();
        string ExecuteScalar();
        void AddParameter(SqlCommand Command, string parameterName, SqlDbType dbType);
        void AddParameterWithValue(SqlCommand Command, string parameterName, SqlDbType dbType, object value);
    }
}
