using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Configuration.Assemblies;

namespace BaseDB
{
    public class SectionManager
    {
        public SectionManager()
        {
        }

        public static string GetValue(string section, string key)
        {
            return System.Configuration.ConfigurationManager.AppSettings.GetValues(key)[0];
            
        }
    }
}
