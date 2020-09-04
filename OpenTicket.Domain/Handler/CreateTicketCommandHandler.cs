using MediatR;
using OpenTicket.Domain.Command;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OpenTicket.Data.Entity;

namespace OpenTicket.Domain.Handler
{
    public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand>
    {
        private readonly OpenTicketDbContext _db;

        public CreateTicketCommandHandler(OpenTicketDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<Unit> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
        {
            var customer =
                await _db.Customers.FirstOrDefaultAsync(c => c.Email == request.CustomerEmail, cancellationToken);
            if (customer == null)
            {
                customer = new Customer
                {
                    Email = request.CustomerEmail,
                    DisplayName = request.CustomerName,
                    RegisteredDateTime = DateTime.UtcNow
                };
                await _db.Customers.AddAsync(customer, cancellationToken);
            }

            var ticket = new Ticket
            {
                Customer = customer,
                EmailAccountId = request.EmailAccountId,
                Title = request.Title,
                Question = request.Question,
                CreatedDateTime = DateTime.UtcNow
            };
            await _db.Tickets.AddAsync(ticket, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
