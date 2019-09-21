using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseDB
{
    public interface IDbConnection : IDisposable
    {
        bool isOpen { get; }
        bool canClose { get; }
        bool Open();
        bool Close();
    }
}
