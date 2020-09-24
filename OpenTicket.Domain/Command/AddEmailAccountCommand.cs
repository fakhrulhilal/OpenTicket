using FluentValidation;
using MediatR;
using OpenTicket.Domain.MailClient;

namespace OpenTicket.Domain.Command
{
    public class AddEmailAccountCommand : EmailAccount, IRequest<Unit>
    {
        internal const int DraftId = -1;

        public class Validator : AbstractValidator<AddEmailAccountCommand>
        {
            public Validator()
            {
                Include(new BaseValidator());
                bool IsLocalAccount(AddEmailAccountCommand cmd) => cmd.Protocol != MailProtocolType.M365;
                RuleFor(p => p.Password).NotEmpty().When(IsLocalAccount);
            }
        }
    }
}
