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
    
    public partial class gnl_mail_service
    {
        public System.Guid id { get; set; }
        public string eposta_to { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public Nullable<System.DateTime> eklemetarihi { get; set; }
        public Nullable<bool> gonderildi { get; set; }
        public Nullable<System.DateTime> gonderimtarihi { get; set; }
        public Nullable<System.Guid> anket_uid { get; set; }
    }
}
