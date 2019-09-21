using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaseWebSite.Models
{
    public partial class gnl_message_inbox
    {
        public void IletiOkundu()
        {
            if (!this.message_is_read)
            {
                this.message_is_read = true;
                this.read_date = DateTime.Now;
            }
        }
        public void DeleteMessage(Guid silen_kullanici_uid)
        {
            this.is_deleted = true;
            this.deleted_at = DateTime.Now;
            this.deleted_by = silen_kullanici_uid;
        }
    }
}