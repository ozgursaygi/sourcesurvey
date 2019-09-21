using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace BaseDB
{

    public class BaseDataAccess : IDisposable
    {
        private MsSqlConnection _msConn;
        public MsSqlConnection MsConn { get { return _msConn; } set { _msConn = value; } }
        public SqlConnection conn { get { return MsConn.Connection; } }

        public string InsertSPName { get; set; }
        public string SelectSPName { get; set; }
        public string UpdateSPName { get; set; }
        public string DeleteSPName { get; set; }
        public string TableName { get; set; }
        public string PrimaryKeyName { get; set; }
        public string SchemaName { get; set; }
        public bool DeleteLogical { get; set; }
        public BaseAdapter BaseAdapter
        {
            get
            {
                if (_baseAdapter != null)
                    return _baseAdapter;
                _baseAdapter = new BaseAdapter();
                _baseAdapter.InsertCommand = InsertCommand();
                _baseAdapter.UpdateCommand = UpdateCommand();
                _baseAdapter.DeleteCommand = DeleteCommand();
                _baseAdapter.SelectCommand = SelectCommand();
                _baseAdapter.TableMappings("Table", TableName);
                return _baseAdapter;
            }
            set { _baseAdapter = value; }
        }
        private BaseAdapter _baseAdapter=null;

        public BaseDataAccess()
        {
            MsConn = DBManager.AppConnection;
            MsConn.Connect();
        }

        public BaseDataAccess(MsSqlConnection c)
        {
            MsConn = c;
        }
        
        private SqlCommand InsertCommand()
        {
            BaseCommand cmn = new BaseCommand(MsConn);
            cmn.CommandType = System.Data.CommandType.StoredProcedure;
            cmn.CommandText = InsertSPName;
            AddParameters(cmn.Command, BaseDB.Definitions.RecordOperations.Insert);
            return cmn.Command;
        }

        private SqlCommand UpdateCommand()
        {
            BaseCommand cmn = new BaseCommand(MsConn);
            cmn.CommandType = System.Data.CommandType.StoredProcedure;
            cmn.CommandText = UpdateSPName;
            AddParameters(cmn.Command, BaseDB.Definitions.RecordOperations.Update);
            return cmn.Command;
        }

        private SqlCommand DeleteCommand()
        {
            BaseCommand cmn = new BaseCommand(MsConn);
            cmn.CommandType = System.Data.CommandType.StoredProcedure;
            cmn.CommandText = DeleteSPName;
            return cmn.Command;
        }

        private SqlCommand SelectCommand()
        {
            BaseCommand cmn = new BaseCommand(MsConn);
            cmn.CommandType = System.Data.CommandType.StoredProcedure;
            cmn.CommandText = SelectSPName;
            return cmn.Command;
        }

        public virtual int Insert(DataSet data)
        {
            BaseAdapter.Insert(data);
            if (PrimaryKeyName != "")
            {
                try
                {
                    return Int32.Parse(data.Tables[TableName].Rows[0][PrimaryKeyName].ToString());
                }
                catch (Exception exp)
                {
                    return 0;
                }
            }
            else
            {
                throw new Exception("Primary Key'i olmayan tabloya kayıt girildi, ID yerine ne return etmeli?");
            }
        }

        public virtual void Update(DataSet data)
        {
            BaseAdapter.Update(data);
        }

        public virtual void Delete(int recordId, string deletedBy)
        {
            this.BaseAdapter.DeleteCommand.Parameters.Clear();
            this.BaseAdapter.DeleteCommand.Parameters.AddWithValue(PrimaryKeyName, recordId);
            if (deletedBy != "")
                this.BaseAdapter.DeleteCommand.Parameters.AddWithValue(BaseCommonConstants.DeletedBy, deletedBy);
            BaseAdapter.Delete();
        }

        public virtual void Delete(Guid record_uid, string deletedBy)
        {
            this.BaseAdapter.DeleteCommand.Parameters.Clear();
            this.BaseAdapter.DeleteCommand.Parameters.AddWithValue(PrimaryKeyName, record_uid);
            if (deletedBy != "")
                this.BaseAdapter.DeleteCommand.Parameters.AddWithValue("deleted_by", deletedBy);
            BaseAdapter.Delete();
        }
        
        public virtual void Delete(int recordId)
        {
          this.BaseAdapter.DeleteCommand.Parameters.Clear();
          this.BaseAdapter.DeleteCommand.Parameters.AddWithValue(PrimaryKeyName, recordId);
          BaseAdapter.Delete();
        }

        public virtual void Delete(Guid record_uid)
        {
          this.BaseAdapter.DeleteCommand.Parameters.Clear();
          this.BaseAdapter.DeleteCommand.Parameters.AddWithValue(PrimaryKeyName, record_uid);
          BaseAdapter.Delete();
        }
        /// <summary>
        /// gelen parametreleri bind eder
        /// </summary>
        /// <param name="args">(parameter name, parameter value), (parameter name, parameter value)</param>
      
        public virtual void Delete(params object[] args)
        {
          for (int i = 0; i < args.Length; i += 2)
            this.BaseAdapter.DeleteCommand.Parameters.AddWithValue((string)args[i], args[i + 1]);
          BaseAdapter.Delete();
        }

        public virtual DataSet Select(int recordId, DataSet Data)
        {
            this.BaseAdapter.SelectCommand.Parameters.AddWithValue(PrimaryKeyName, recordId);
            BaseAdapter.Fill(Data, TableName);
            return Data;
        }

        public virtual DataSet Select(Guid record_uid, DataSet Data)
        {
            this.BaseAdapter.SelectCommand.Parameters.AddWithValue(PrimaryKeyName, record_uid);
            BaseAdapter.Fill(Data, TableName);
            return Data;
        }
        /// <summary>
        /// gelen parametreleri bind edip, data'yı doldurur.
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="args">(parameter name, parameter value), (parameter name, parameter value)</param>
        /// <returns></returns>
        public virtual DataSet Select(DataSet Data, params object[] args)
        {
          for (int i = 0; i < args.Length; i+=2)
              this.BaseAdapter.SelectCommand.Parameters.AddWithValue((string)args[i], args[i+1]);
          BaseAdapter.Fill(Data, TableName);
          return Data;
        }

        public virtual DataSet Select(int recordId)
        {
            return this.Select(recordId, new DataSet());
        }

        public virtual DataSet Select(Guid record_uid)
        {
            return this.Select(record_uid, new DataSet());
        }

        protected virtual void AddParameters(SqlCommand cmd, BaseDB.Definitions.RecordOperations opType)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (_baseAdapter != null)
                _baseAdapter.Dispose();
            //şimdilik kapatma, application_endrequest kapatsın bakalım.
            //MsConn.Close();
        }


    }
}
