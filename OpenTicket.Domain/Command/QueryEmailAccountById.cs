using System;
using System.Linq;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenTicket.Data.Entity;

namespace OpenTicket.Domain.Command
{
    public class QueryEmailAccountById : IRequest<QueryEmailAccountById.EmailAccount>
    {
        public QueryEmailAccountById(int id) => Id = id;

        public int Id { get; }

        public class EmailAccount : MailClient.EmailAccount
        {
            public int Id { get; set; }
        }

        public class Validator : AbstractValidator<QueryEmailAccountById>
        {
            public Validator(OpenTicketDbContext db)
            {
                if (db == null) throw new ArgumentNullException(nameof(db));

                RuleFor(p => p.Id).Must(id => db.EmailAccounts.AsNoTracking().Any(ea => ea.Id == id))
                    .WithMessage(id => $"Email account with ID {id} must exist");
            }
        }
    }
}
