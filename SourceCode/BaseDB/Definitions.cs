using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseDB
{
    
    public class Definitions
    {
	    public enum ConnectionState
	    {
		    Closed = 0,
		    Open = 1,
		    Connecting = 2,
		    Executing = 4,
		    Fetching = 8,
		    Broken = 16
	    }

	    public enum IsolationLevel
	    {
		    Chaos = 16,
		    ReadUncommitted = 256,
		    ReadCommitted = 4096,		
		    RepeatableRead = 65536,
		    Serializable = 1048576,
		    Snapshot = 16777216,
		    Unspecified = -1
	    }

	    public enum CommandBehavior
	    {
		    Default = 0,
		    SingleResult = 1,
		    SchemaOnly = 2,
		    KeyInfo = 4,
		    SingleRow = 8,
		    SequentialAccess = 16,
		    CloseConnection = 32		
	    }

        public enum CommandType
        {
            Text = 1,
            StoredProcedure = 4,
            TableDirect = 512

        }

        public enum UpdateRowSource
        {
            None = 0,
            OutputParameters = 1,
            FirstReturnedRecord = 2,
            Both = 3
        }
        public enum RecordOperations
        {
            Insert = 100,
            Update = 200,
            Delete = 300,
            Select = 400
        }

    }
}
