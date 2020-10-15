using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using OpenTicket.Data.Entity;
using MailProtocolType = OpenTicket.Domain.MailClient.MailProtocolType;

namespace OpenTicket.Domain.Command
{
    public class AddExternalAccountCommand : IRequest
    {
        public MailProtocolType Protocol { get; set; }
        public string Name { get; set; }
        public string Identifier { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }
        
        public class Handler : IRequestHandler<AddExternalAccountCommand>
        {
            private readonly OpenTicketDbContext _db;
            private readonly IMapper _mapper;
            private readonly IValidator<AddExternalAccountCommand> _validator;

            public Handler(OpenTicketDbContext db, IMapper mapper, IValidator<AddExternalAccountCommand> validator)
            {
                _validator = validator ?? throw new ArgumentNullException(nameof(validator));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _db = db ?? throw new ArgumentNullException(nameof(db));
            }
            
            public async Task<Unit> Handle(AddExternalAccountCommand request, CancellationToken cancellationToken)
            {
                await _validator.ValidateAndThrowAsync(request, cancellationToken: cancellationToken);
                var entity = _mapper.Map<ExternalAccount>(request);
                await _db.ExternalAccounts.AddAsync(entity, cancellationToken);
                await _db.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }

        public class Validator : AbstractValidator<AddExternalAccountCommand>
        {
            public Validator()
            {
                RuleFor(p => p.Name).NotEmpty().MaximumLength(50);
                RuleFor(p => p.Identifier).NotEmpty().MaximumLength(250);
                RuleFor(p => p.ClientId).NotEmpty();
                RuleFor(p => p.Secret).NotEmpty();
            }
        }
    }
}