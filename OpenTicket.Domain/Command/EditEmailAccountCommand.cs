using MediatR;

namespace OpenTicket.Domain.Command
{
    public class EditEmailAccountCommand : QueryEmailAccountById.EmailAccount, IRequest<Unit>
    { }
}
