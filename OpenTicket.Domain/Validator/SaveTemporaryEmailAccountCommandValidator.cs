using FluentValidation;
using OpenTicket.Domain.Command;
using OpenTicket.Domain.MailClient;

namespace OpenTicket.Domain.Validator
{
    public class SaveTemporaryEmailAccountCommandValidator : AbstractValidator<SaveTemporaryEmailAccountCommand>
    {
        public SaveTemporaryEmailAccountCommandValidator()
        {
            RuleFor(p => p.Email).NotEmpty().MaximumLength(100);
            RuleFor(p => p.UserId).NotEmpty().MaximumLength(250);
            RuleFor(p => p.Secret).NotEmpty();
            RuleFor(p => p.ServerAddress).NotEmpty().When(cmd => cmd.Protocol != MailProtocolType.M365);
            RuleFor(p => p.ServerPort).GreaterThan(0).When(cmd => cmd.Protocol != MailProtocolType.M365);
        }
    }
}