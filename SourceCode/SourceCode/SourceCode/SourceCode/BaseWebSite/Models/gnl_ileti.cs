using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaseWebSite.Models
{
    public partial class gnl_message
    {
        public const string ReceiverTypeTo = "T";
        public const string ReceiverTypeCc = "C";
        public bool AliciTarafindanOkundu(Guid receiver_uid)
        {
            
            gnl_message_inbox gelen = this.gnl_message_inbox.SingleOrDefault(g => g.user_uid == receiver_uid);

            if (gelen == null)
                return false;

            if (gelen.message_is_read == false)
            {
                gelen.message_is_read = true;
                gelen.read_date = DateTime.Now;
                return true;
            }
            return false;
        }
        public bool IsValid
        {
            get
            {
                if (String.IsNullOrWhiteSpace(this.message_subject) == true) return false;
                if (String.IsNullOrWhiteSpace(this.message) == true) return false;
                if (this.gnl_message_recipient.Count == 0) return false;
                return true;
            }
        }

        internal void AddReceiver(Guid[] userGuid, string receiverType)
        {
            if (userGuid == null)
                return;
            foreach (Guid kullanici in userGuid)
            {
                gnl_message_recipient alici = this.gnl_message_recipient.SingleOrDefault(a => a.user_uid == kullanici);
                if (alici == null)
                    this.gnl_message_recipient.Add(new gnl_message_recipient
                    {
                        user_uid = kullanici,
                        recipient_type = receiverType,
                        recipient_rank = 1
                    });
            }
        }

        internal void Send()
        {
            foreach (gnl_message_recipient alici in this.gnl_message_recipient)
            {
                var gelen = new gnl_message_inbox
                {
                    recipient_type = alici.recipient_type,
                    user_uid = alici.user_uid,
                    message_is_read = false,
                    is_deleted = false
                };
                this.gnl_message_inbox.Add(gelen);
            }
            this.sent_date = DateTime.Now;
        }

        internal void Sil(Guid deleted_user_uid)
        {
            this.is_deleted = true;
            this.deleted_at = DateTime.Now;
            this.deleted_by = deleted_user_uid;
        }

    }
}