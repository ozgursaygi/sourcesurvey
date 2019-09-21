using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Web;
using System.Configuration;
using System.Data;
using System.Collections;

namespace BaseDB
{
    public class MsSqlConnection : BaseConnection
    {
        string connectionKey = "";
        public MsSqlConnection(string key = DBManager.ConnectionTypes.AppConnection)
        {
            connectionKey = key;
        }
        public override System.Data.Common.DbConnection CreateConnection()
        {
            string connectionString = ConfigurationManager.ConnectionStrings[connectionKey].ConnectionString;

            SqlConnection conn = new SqlConnection(connectionString);
            return (System.Data.Common.DbConnection)conn;
        }
        public new SqlConnection Connection
        {
            get { return (SqlConnection)base.Connection; }
            set { base.Connection = value; }
        }
        public override System.Data.Common.DbTransaction CreateTransactionContext(System.Data.IsolationLevel isolationLevel)
        {
            return (System.Data.Common.DbTransaction)(Connection.BeginTransaction(isolationLevel));
        }

        public DataSet GetDataSet(string sql, string srcTable = "data")
        {
            DataSet myDataSet = new DataSet();

            SqlCommand sqlCmd = new SqlCommand(sql, this.Connection);
            sqlCmd.CommandType = CommandType.Text;
            SqlDataAdapter mySqlDataAdapter = new SqlDataAdapter(sqlCmd);
            mySqlDataAdapter.Fill(myDataSet, srcTable);
            return myDataSet;
        }

        public DataSet GetDataSet(string sql, ArrayList parameterArr, ArrayList valueArr, string srcTable = "data")
        {
            DataSet myDataSet = new DataSet();

            SqlCommand sqlCmd = new SqlCommand(sql, this.Connection);
            sqlCmd.CommandType = CommandType.Text;
            for (int i = 0; i < parameterArr.Count; i++)
                sqlCmd.Parameters.AddWithValue("@" + parameterArr[i], valueArr[i]);
            SqlDataAdapter mySqlDataAdapter = new SqlDataAdapter(sqlCmd);
            mySqlDataAdapter.Fill(myDataSet, srcTable);
            return myDataSet;
        }

        public DataSet ExecuteSP(string spName, ArrayList parameterArr, ArrayList valueArr)
        {
            DataSet myDataSet = new DataSet();

            SqlCommand sqlCmd = new SqlCommand(spName, this.Connection);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            if (parameterArr != null && valueArr != null)
                for (int i = 0; i < parameterArr.Count; i++)
                    sqlCmd.Parameters.AddWithValue("@" + parameterArr[i], valueArr[i]);
            SqlDataAdapter mySqlDataAdapter = new SqlDataAdapter(sqlCmd);
            mySqlDataAdapter.Fill(myDataSet, "Table");
            return myDataSet;

        }

        public string ExecuteSql(string sql)
        {
            string result = "";
            SqlCommand sqlCmd = new SqlCommand(sql, this.Connection);
            sqlCmd.CommandType = CommandType.Text;
            object obj = sqlCmd.ExecuteScalar();
            if (obj != null)
                result = obj.ToString();
            return result;
        }

        public string ExecuteSql(string sql, ArrayList parameterArr, ArrayList valueArr)
        {
            string result = "";
            SqlCommand sqlCmd = new SqlCommand(sql, this.Connection);
            sqlCmd.CommandType = CommandType.Text;
            for (int i = 0; i < parameterArr.Count; i++)
                sqlCmd.Parameters.AddWithValue("@" + parameterArr[i], valueArr[i]);
            object obj = sqlCmd.ExecuteScalar();
            if (obj != null)
                result = obj.ToString();
            return result;
        }

        public System.Xml.XmlReader ExecuteSPForXml(string spName, ArrayList parameterArr, ArrayList valueArr)
        {
            DataSet myDataSet = new DataSet();

            SqlCommand sqlCmd = new SqlCommand(spName, this.Connection);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            for (int i = 0; i < parameterArr.Count; i++)
            {
                sqlCmd.Parameters.AddWithValue("@" + parameterArr[i], valueArr[i]);
            }
            System.Xml.XmlReader x = sqlCmd.ExecuteXmlReader();
            return x;
        }
    }
}
