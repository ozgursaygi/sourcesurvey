using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace BaseDB
{
    public class BaseCommand : IDbCommand, IDisposable
    { 
        private SqlCommand command = null;

        public BaseCommand(BaseConnection baseConn)
        {
            Command = (SqlCommand)baseConn.Connection.CreateCommand();
            Command.Connection = (SqlConnection)baseConn.Connection;
            if (baseConn.Tran != null)
                Command.Transaction = (SqlTransaction)(baseConn.Tran);
        }

        public SqlCommand Command
        {
            get { return command; }
            set { command = value; }
        }

        public System.Data.CommandType CommandType 
        {
            set { Command.CommandType = value; }
        }

        public string CommandText
        {
            set { Command.CommandText = value; }
        }

        public int ExecuteNonQuery()
        {
            int result = 0;
            try
            {
                result = Command.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                
                //return result;
            }
            //Command.Connection.Close();
            return result;
        }

        public string ExecuteScalar()
        {
            string result = "";
            try
            {
                object obj = Command.ExecuteScalar();
                if (obj != null)
                    result = obj.ToString();
            }
            catch(Exception exp)
            {
                
                //return result;
            }
            //Command.Connection.Close();
            return result;
        }

        public void AddParameter(SqlCommand command, string parameterName, SqlDbType dbType)
        {
            command.Parameters.Add(parameterName, dbType);
        }

        public void AddParameterWithValue(SqlCommand command, string parameterName, SqlDbType dbType,object value)
        {
            command.Parameters.AddWithValue(parameterName, value);
        }

        public void Dispose()
        {
            if (Command != null)
                Command.Dispose();
        }
    }
}
