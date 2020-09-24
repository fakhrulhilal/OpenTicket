using System;
using System.Linq;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenTicket.Data.Entity;

namespace OpenTicket.Domain.Command
{
    public class DeleteEmailAccountCommand : IRequest<Unit>
    {
        public DeleteEmailAccountCommand(int id) => Id = id;

        public int Id { get; }

        public class Validator : AbstractValidator<DeleteEmailAccountCommand>
        {
            public Validator(OpenTicketDbContext db)
            {
                if (db == null) throw new ArgumentNullException(nameof(db));

                RuleFor(p => p.Id).GreaterThan(0)
                    .Must(id => db.EmailAccounts.AsNoTracking().Any(ea => ea.Id == id))
                    .WithMessage(id => $"Email account with ID {id} must exist");
            }
        }
    }
}