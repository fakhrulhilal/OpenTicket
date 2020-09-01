using MediatR;
using OpenTicket.Data.Entity;

namespace OpenTicket.Domain.Command
{
    public class AddEmailAccountCommand : IRequest<Unit>
    {
        public string Email { get; set; }
        public string UserId { get; set; }
        public string Secret { get; set; }
        public MailProtocolType Protocol { get; set; }
        public int ServerPort { get; set; }
        public string ServerAddress { get; set; }
        public string MailBox { get; set; }
        public bool UseSecureConnection { get; set; }
    }
}
