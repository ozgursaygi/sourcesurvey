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
    
    public partial class sbr_anket_davet
    {
        public System.Guid davet_uid { get; set; }
        public Nullable<System.Guid> davet_eden_kullanici_uid { get; set; }
        public Nullable<System.Guid> davet_edilen_grup_uid { get; set; }
        public string davet_edilen_email { get; set; }
        public string davet_key { get; set; }
        public Nullable<System.DateTime> davet_tarihi { get; set; }
        public Nullable<bool> davet_kabul_edildi { get; set; }
        public Nullable<System.DateTime> davet_kabul_edilme_tarihi { get; set; }
        public Nullable<System.Guid> davet_kabul_eden_kullanici_uid { get; set; }
    }
}