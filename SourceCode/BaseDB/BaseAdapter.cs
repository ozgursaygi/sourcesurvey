using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace BaseDB
{
    public class BaseAdapter : IDbDataAdapter, IDisposable
    {
        private SqlDataAdapter adapter = null;

        public BaseAdapter()
        {
            adapter = new SqlDataAdapter();
            Adapter = adapter;

            Adapter.RowUpdating += new SqlRowUpdatingEventHandler(OnRowUpdating);
            Adapter.RowUpdated += new SqlRowUpdatedEventHandler(OnRowUpdated);

        }

        public BaseAdapter(string sqlQuery, MsSqlConnection conn)
        {
            adapter = new SqlDataAdapter(sqlQuery, conn.Connection);
            Adapter = adapter;

            Adapter.RowUpdating += new SqlRowUpdatingEventHandler(OnRowUpdating);
            Adapter.RowUpdated += new SqlRowUpdatedEventHandler(OnRowUpdated);

        }

        public SqlDataAdapter Adapter
        {
            get { return adapter; }
            set { adapter = value; }
        }

        public SqlCommand InsertCommand
        {
            get { return Adapter.InsertCommand; }
            set { Adapter.InsertCommand = value; }
        }

        public SqlCommand UpdateCommand
        {
            get { return Adapter.UpdateCommand; }
            set { Adapter.UpdateCommand = value; }
        }

        public SqlCommand SelectCommand
        {
            get { return Adapter.SelectCommand; }
            set { Adapter.SelectCommand = value; }
        }

        public SqlCommand DeleteCommand
        {
            get { return Adapter.DeleteCommand; }
            set { Adapter.DeleteCommand = value; }
        }

        public void Insert(System.Data.DataSet data)
        {
            Adapter.Update(data);
            //if (Adapter.InsertCommand != null) if (Adapter.InsertCommand.Transaction == null) Adapter.InsertCommand.Connection.Close();
            //if (Adapter.UpdateCommand != null) if (Adapter.UpdateCommand.Transaction == null) Adapter.UpdateCommand.Connection.Close();
        }

        public void Update(System.Data.DataSet data)
        {
            Adapter.Update(data);
            //if (Adapter.InsertCommand != null) if (Adapter.InsertCommand.Transaction == null) Adapter.InsertCommand.Connection.Close();
            //if (Adapter.UpdateCommand != null) if (Adapter.UpdateCommand.Transaction == null) Adapter.UpdateCommand.Connection.Close();
        }

        public void Delete()
        {
            Adapter.DeleteCommand.ExecuteNonQuery();
        }

        public void Fill(System.Data.DataSet data, string tableName)
        {
            Adapter.Fill(data, tableName);
            //Adapter.SelectCommand.Connection.Close();
        }

        public void Fill(System.Data.DataSet data)
        {
            Adapter.Fill(data);
            //Adapter.SelectCommand.Connection.Close();
        }

        public void TableMappings(string sourceTable, string dataSetTable)
        {
            Adapter.TableMappings.Add(sourceTable, dataSetTable);
        }

        protected static void OnRowUpdating(object sender, SqlRowUpdatingEventArgs args)
        {
            
            if (args.StatementType == StatementType.Delete)
            {
                     
            }
            else if (args.StatementType == StatementType.Insert)
            {
              
            }
            else if (args.StatementType == StatementType.Update)
            {
                
            }
        }

        protected static void OnRowUpdated(object sender, SqlRowUpdatedEventArgs args)
        {
            if (args.Status == UpdateStatus.ErrorsOccurred)
            {
               
            }
            else if (args.StatementType == StatementType.Insert)
            {
                

                /*foreach (DataColumn c in args.Row.Table.Columns)
                    logger.InfoFormat("\t{0}={1}", c.ColumnName, args.Row[c.ColumnName]);*/
            }
        }

        public void Dispose()
        {
            adapter.Dispose();
        }
    }
}
