using System.Collections.Generic;

namespace OpenTicket.Data.Entity
{
    public class ExternalAccount
    {
        public int Id { get; set; }
        public MailProtocolType Protocol { get; set; }
        public string Name { get; set; }
        public string Identifier { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public virtual ICollection<EmailAccount> EmailAccounts { get; set; } = new HashSet<EmailAccount>();
    }
}