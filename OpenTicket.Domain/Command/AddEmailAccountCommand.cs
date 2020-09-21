using MediatR;

namespace OpenTicket.Domain.Command
{
    public class AddEmailAccountCommand : MailClient.EmailAccount, IRequest<Unit>
    {
        internal const int DraftId = -1;
    }
}
