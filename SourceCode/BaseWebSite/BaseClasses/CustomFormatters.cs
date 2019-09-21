using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseClasses
{
    public class CustomFormatters
    {
        public string format_phone(string phoneNumber)
        {
            if (phoneNumber.Length == 10)
                return string.Format("({0}){1}-{2}", phoneNumber.Substring(0, 3), phoneNumber.Substring(3, 3), phoneNumber.Substring(6, 4));
            return phoneNumber;
        }
    }
}
