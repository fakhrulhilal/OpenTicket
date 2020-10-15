using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenTicket.Data.Entity;

namespace OpenTicket.Domain.Command
{
    public class DeleteEmailAccountCommand : IRequest<Unit>
    {
        public DeleteEmailAccountCommand(int id) => Id = id;

        private int Id { get; }

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
        
        public class Handler : IRequestHandler<DeleteEmailAccountCommand>
        {
            private readonly OpenTicketDbContext _db;
            private readonly IMapper _mapper;
            private readonly IValidator<DeleteEmailAccountCommand> _validator;

            public Handler(OpenTicketDbContext db, IMapper mapper, IValidator<DeleteEmailAccountCommand> validator)
            {
                _validator = validator ?? throw new ArgumentNullException(nameof(validator));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _db = db ?? throw new ArgumentNullException(nameof(db));
            }

            public async Task<Unit> Handle(DeleteEmailAccountCommand request, CancellationToken cancellationToken)
            {
                await _validator.ValidateAndThrowAsync(request, cancellationToken: cancellationToken);
                var emailAccount = await _db.EmailAccounts.FindAsync(request.Id);
                if (emailAccount == null) return Unit.Value;

                var entity = _mapper.Map<EmailAccount>(emailAccount);
                _db.EmailAccounts.Remove(entity);
                await _db.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}