using System;
using System.Collections.Generic;

namespace OpenTicket.Data.Entity
{
    public class EmailAccount
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AccessToken { get; set; }
        public DateTime? LastUpdateAccessToken { get; set; }
        public int ServerPort { get; set; }
        public string ServerAddress { get; set; }
        public string MailBox { get; set; }
        public bool UseSecureConnection { get; set; }
        public short Protocol { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
    }
}
