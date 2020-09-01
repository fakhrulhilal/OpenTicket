using FluentValidation;
using OpenTicket.Domain.Command;

namespace OpenTicket.Domain.Validator
{
    public class QueryEmailAccountByIdValidator : AbstractValidator<QueryEmailAccountById>
    {
        public QueryEmailAccountByIdValidator()
        {
            RuleFor(p => p.Id).GreaterThan(0);
        }
    }
}
