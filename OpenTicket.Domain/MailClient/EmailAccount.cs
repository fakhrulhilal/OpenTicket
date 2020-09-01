using System.ComponentModel;

namespace OpenTicket.Domain.MailClient
{
    public abstract class EmailAccount
    {
        public string Email { get; set; }
        public string UserId { get; set; }
        public string Secret { get; set; }
        public string AccessToken { get; set; }
        public int ServerPort { get; set; }
        public string ServerAddress { get; set; }
        public string MailBox { get; set; }
        public bool UseSecureConnection { get; set; }
        public MailProtocolType Protocol { get; set; }
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
