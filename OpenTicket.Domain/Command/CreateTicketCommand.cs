using MediatR;

namespace OpenTicket.Domain.Command
{
    public class CreateTicketCommand : IRequest<Unit>
    {
        public string Title { get; set; }
        public string Question { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public int EmailAccountId { get; set; }
    }
}
