//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BaseWebSite.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class sbr_sablon_durum_tarihcesi
    {
        public System.Guid sablon_durumu_uid { get; set; }
        public Nullable<System.Guid> sablon_uid { get; set; }
        public Nullable<int> sablon_durumu_id { get; set; }
        public Nullable<System.Guid> durumu_olusturan_kullanici { get; set; }
        public Nullable<System.DateTime> durum_olusma_tarihi { get; set; }
        public string durum_aciklamasi { get; set; }
    }
}
