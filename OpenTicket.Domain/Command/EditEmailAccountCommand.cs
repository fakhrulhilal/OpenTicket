using System;
using System.Linq;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenTicket.Data.Entity;

namespace OpenTicket.Domain.Command
{
    public class EditEmailAccountCommand : MailClient.EmailAccount, IRequest<Unit>
    {
        public int Id { get; set; }
        
        public class Validator : AbstractValidator<EditEmailAccountCommand>
        {
            public Validator(OpenTicketDbContext db)
            {
                if (db == null) throw new ArgumentNullException(nameof(db));
                
                Include(new BaseValidator());
                RuleFor(p => p.Id).GreaterThan(0)
                    .Must(id => db.EmailAccounts.AsNoTracking().Any(ea => ea.Id == id))
                    .WithMessage(id => $"Email account with ID {id} must exist");
            }
        }
    }
}
