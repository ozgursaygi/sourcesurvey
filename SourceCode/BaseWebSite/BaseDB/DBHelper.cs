using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;

namespace BaseDB
{
    public class DBHelper
    {
        public static DataSet GetDataSet(string sql)
        {
            DataSet dataSet = new DataSet();
            using (BaseDB.BaseAdapter baseAdapter = new BaseDB.BaseAdapter())
            {
                using (BaseDB.BaseDataAccess baseDataAccess = new BaseDB.BaseDataAccess())
                {
                    using (BaseDB.BaseCommand cmd = new BaseDB.BaseCommand(baseDataAccess.MsConn))
                    {
                        cmd.CommandText = sql;
                        cmd.CommandType = System.Data.CommandType.Text;
                        baseAdapter.SelectCommand = cmd.Command;
                        baseAdapter.Fill(dataSet, "Table");
                    }
                }
            }
            return dataSet;
        }

        public static DataSet ExecuteSP(string spName, ArrayList parameterArr, ArrayList valueArr)
        {

            DataSet Data = new DataSet();
            
            using (BaseDB.BaseAdapter baseAdapter = new BaseDB.BaseAdapter())
            {
                using (BaseDB.BaseDataAccess baseDataAccess = new BaseDB.BaseDataAccess())
                {
                    using (BaseDB.BaseCommand cmd = new BaseDB.BaseCommand(baseDataAccess.MsConn))
                    {
                        cmd.CommandText = spName;
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        for (int i = 0; i < parameterArr.Count; i++)
                        {
                            cmd.Command.Parameters.AddWithValue("@" + parameterArr[i], valueArr[i]);
                        }

                        baseAdapter.SelectCommand = cmd.Command;
                        baseAdapter.Fill(Data, "Table");
                    }
                }
            }
            return Data;
        }

        public static string ExecuteSql(string sql)
        {
            string retVal;
            using (BaseDB.BaseDataAccess baseDataAccess = new BaseDB.BaseDataAccess())
            {
                using (BaseDB.BaseCommand cmd = new BaseDB.BaseCommand(baseDataAccess.MsConn))
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = System.Data.CommandType.Text;
                    retVal = cmd.ExecuteScalar();
                }
            }
            return retVal;
        }
        public static string GetFieldValue(string tableName, string criteria, string field)
        {
            string retVal;
            using (BaseDB.BaseDataAccess baseDataAccess = new BaseDB.BaseDataAccess())
            {
                using (BaseDB.BaseCommand cmd = new BaseDB.BaseCommand(baseDataAccess.MsConn))
                {

                    if (string.Compare(criteria, "") != 0)
                        cmd.CommandText = "Select " + field + " From " + tableName + " Where " + criteria;
                    else
                        cmd.CommandText = "Select " + field + " From " + tableName;

                    cmd.CommandType = System.Data.CommandType.Text;
                    retVal = cmd.ExecuteScalar();
                }
            }
            return retVal;
        }
        public static int GetMaxId(string tableName, string field)
        {
            string result = "";
            using (BaseDB.BaseDataAccess baseDataAccess = new BaseDB.BaseDataAccess())
            {
                using (BaseDB.BaseCommand cmd = new BaseDB.BaseCommand(baseDataAccess.MsConn))
                {
                    cmd.CommandText = "Select MAX(" + field + ") From " + tableName;
                    cmd.CommandType = System.Data.CommandType.Text;
                    result = cmd.ExecuteScalar();

                    if (result == "")
                        result = "0";
                }
            }
            return Int32.Parse(result);
        }
        public static System.Xml.XmlReader ExecuteSPForXml(string spName, ArrayList parameterArr, ArrayList valueArr)
        {
            using (BaseDB.BaseAdapter baseAdapter = new BaseDB.BaseAdapter())
            {
                using (BaseDB.BaseDataAccess baseDataAccess = new BaseDB.BaseDataAccess())
                {
                    using (BaseDB.BaseCommand cmd = new BaseDB.BaseCommand(baseDataAccess.MsConn))
                    {
                        cmd.CommandText = spName;
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        for (int i = 0; i < parameterArr.Count; i++)
                        {
                            cmd.Command.Parameters.AddWithValue("@" + parameterArr[i], valueArr[i]);
                        }

                        baseAdapter.SelectCommand = cmd.Command;
                        System.Xml.XmlReader x = cmd.Command.ExecuteXmlReader();
                        return x;

                    }
                }
            }
        }

    }
}
