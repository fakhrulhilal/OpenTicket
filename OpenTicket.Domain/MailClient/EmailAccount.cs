using System.ComponentModel;
using FluentValidation;

namespace OpenTicket.Domain.MailClient
{
    public abstract class EmailAccount
    {
        public int? ExternalAccountId { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ServerPort { get; set; }
        public string ServerAddress { get; set; }
        public string MailBox { get; set; }
        public bool UseSecureConnection { get; set; }
        public MailProtocolType Protocol { get; set; }

        public class BaseValidator : AbstractValidator<EmailAccount>
        {
            public BaseValidator()
            {
                bool IsLocalAccount(EmailAccount cmd) => cmd.Protocol != MailProtocolType.M365;
                RuleFor(p => p.Email).NotEmpty().MaximumLength(100);
                RuleFor(p => p.Username).NotEmpty().When(IsLocalAccount).MaximumLength(250);
                RuleFor(p => p.ServerAddress).NotEmpty().When(IsLocalAccount);
                RuleFor(p => p.ServerPort).GreaterThan(0).When(IsLocalAccount);
                RuleFor(p => p.ExternalAccountId).NotNull().When(cmd => !IsLocalAccount(cmd));
            }
        }
    }

    public enum MailProtocolType : short
    {
        [Description("IMAP")] Imap = 1,

        [Description("POP3")] Pop3,

        [Description("Microsoft 365")] M365
    }
}