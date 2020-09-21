using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace OpenTicket.Data.Entity
{
    public class EmailAccount
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public int? DraftId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? LastUpdateAccessToken { get; set; }
        public int ServerPort { get; set; }
        public string ServerAddress { get; set; }
        public string MailBox { get; set; }
        public bool UseSecureConnection { get; set; }
        public MailProtocolType Protocol { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
    }

    public enum MailProtocolType : short
    {
        [Description("IMAP")]
        Imap = 1,

        [Description("POP3")]
        Pop3,

        [Description("Microsoft 365")]
        M365
    }
}
