using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BaseDB
{
    class TypedDataSetConvertor
    {
        /// <summary>
        /// Helper methods and functions.
        /// </summary>
        /// <typeparam name="T">A strongly type DataTable.
        /// A DataTable of type T will be returned from the DataSet.
        /// </typeparam>
        public static class DataSetAdapter<T>
                                  where T : DataTable, new()
        {
            /// <summary>
            /// Convert the first DataTable from a DataSet to a
            /// strongly-typed data table.
            /// </summary>
            public static T convert(DataSet dataSet)
            {
                if (dataSet == null)
                    return null;
                if (dataSet.Tables.Count == 0)
                    return null;
                DataTable dataTable = dataSet.Tables[0];
                return convert(dataTable);
            }
            /// <summary>
            /// Convert an ordinary DataTable to a strongly-typed
            /// data table.
            /// </summary>
            public static T convert(DataTable dataTable)
            {
                if (dataTable == null)
                    return null;
                T stronglyTyped = new T();
                // add data from the regular DataTable to the
                // strongly typed DataTable.
                stronglyTyped.Merge(dataTable);
                return stronglyTyped;
            }
        }
    }
}
