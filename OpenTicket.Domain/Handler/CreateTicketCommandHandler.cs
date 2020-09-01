using MediatR;
using OpenTicket.Domain.Command;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTicket.Domain.Handler
{
    public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand>
    {
        public Task<Unit> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
        {
            /**
             * TODO:
             * 1. Find customer matching customer email
             * 2. Create new customer when not available yet
             * 3. Insert into Tickets table
             */
            throw new NotImplementedException();
        }
    }
}
