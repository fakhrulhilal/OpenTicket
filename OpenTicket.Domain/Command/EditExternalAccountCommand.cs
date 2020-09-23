using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenTicket.Data.Entity;

namespace OpenTicket.Domain.Command
{
    public class EditExternalAccountCommand : IRequest
    {
        public int Id { get; set; }
        public MailClient.MailProtocolType Protocol { get; set; }
        public string Name { get; set; }
        public string Identifier { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }

        public class Handler : IRequestHandler<EditExternalAccountCommand>
        {
            private readonly OpenTicketDbContext _db;
            private readonly IMapper _mapper;
            private readonly IValidator<EditExternalAccountCommand> _validator;

            public Handler(OpenTicketDbContext db, IMapper mapper, IValidator<EditExternalAccountCommand> validator)
            {
                _validator = validator ?? throw new ArgumentNullException(nameof(validator));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _db = db ?? throw new ArgumentNullException(nameof(db));
            }
            
            public async Task<Unit> Handle(EditExternalAccountCommand request, CancellationToken cancellationToken)
            {
                await _validator.ValidateAndThrowAsync(request, cancellationToken: cancellationToken);
                var entity = _mapper.Map<ExternalAccount>(request);
                if (string.IsNullOrWhiteSpace(entity.Secret))
                {
                    var existing = await _db.ExternalAccounts.AsNoTracking().SingleAsync(ea => ea.Id == entity.Id, cancellationToken);
                    entity.Secret = existing.Secret;
                }
                _db.ExternalAccounts.Update(entity);
                await _db.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }

        public class Validator : AbstractValidator<EditExternalAccountCommand>
        {
            public Validator(OpenTicketDbContext db)
            {
                if (db == null) throw new ArgumentNullException(nameof(db));

                RuleFor(p => p.Id)
                    .GreaterThan(0)
                    .MustAsync((id, token) =>
                        db.ExternalAccounts.AsNoTracking().AnyAsync(ea => ea.Id == id, token))
                    .WithMessage((cmd, id) => $"No external account with ID {id}");
                RuleFor(p => p.Name).NotEmpty().MaximumLength(50);
                RuleFor(p => p.Identifier).NotEmpty().MaximumLength(250);
                RuleFor(p => p.ClientId).NotEmpty();
            }
        }
    }
}