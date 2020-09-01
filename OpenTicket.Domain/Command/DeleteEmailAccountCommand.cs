using MediatR;

namespace OpenTicket.Domain.Command
{
    public class DeleteEmailAccountCommand : IRequest<Unit>
    {
        public DeleteEmailAccountCommand(int id) => Id = id;

        public int Id { get; }
    }
}