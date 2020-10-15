using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenTicket.Data.Entity;

namespace OpenTicket.Domain.Command
{
    public class CreateTicketCommand : IRequest<Unit>
    {
        public string Title { get; set; }
        public string Question { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public int EmailAccountId { get; set; }
        
        public class Handler : IRequestHandler<CreateTicketCommand>
        {
            private readonly OpenTicketDbContext _db;

            public Handler(OpenTicketDbContext db) => _db = db ?? throw new ArgumentNullException(nameof(db));

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
}
