using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaseWebSite.Models
{
    public enum anket_durumu
    {
        Tumu = 0,
        Acik = 1,
        Kapali = 2,
        Arsivde = 3,
        Yayinda = 4
    }

    public enum sablon_durumu
    {
        Acik = 1,
        Kapali = 2
    }

    public enum PaymentMethod
    {
        credit_card,
        paypal
    }
}