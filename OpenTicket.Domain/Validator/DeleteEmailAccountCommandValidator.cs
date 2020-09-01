using FluentValidation;
using OpenTicket.Domain.Command;

namespace OpenTicket.Domain.Validator
{
    public class DeleteEmailAccountCommandValidator : AbstractValidator<DeleteEmailAccountCommand>
    {
        public DeleteEmailAccountCommandValidator()
        {
            RuleFor(p => p.Id).GreaterThan(0);
        }
    }
}
