using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseDB
{
    public interface IRepository : IDisposable
    {
        void Kaydet();
    }
}
