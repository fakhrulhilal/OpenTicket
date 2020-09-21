using MediatR;

namespace OpenTicket.Domain.Command
{
    public class EditEmailAccountCommand : MailClient.EmailAccount, IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
