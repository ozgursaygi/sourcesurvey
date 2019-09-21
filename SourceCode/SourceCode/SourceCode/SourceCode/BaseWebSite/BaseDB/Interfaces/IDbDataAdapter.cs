using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace BaseDB
{
    public interface IDbDataAdapter
    {
        SqlCommand InsertCommand { get; set; }
        SqlCommand UpdateCommand { get; set; }
        SqlCommand SelectCommand { get; set; }
        SqlCommand DeleteCommand { get; set; }
        void Insert(System.Data.DataSet data);
        void Update(System.Data.DataSet data);
        void Delete();
        void Fill(System.Data.DataSet data,string tableName);
        void Fill(System.Data.DataSet data);
        void TableMappings(string sourceTable, string dataSetTable);
        
    }
}
