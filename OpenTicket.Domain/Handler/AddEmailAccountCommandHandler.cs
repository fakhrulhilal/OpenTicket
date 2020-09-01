using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using OpenTicket.Data.Entity;
using OpenTicket.Domain.Command;

namespace OpenTicket.Domain.Handler
{
    public class AddEmailAccountCommandHandler : IRequestHandler<AddEmailAccountCommand>
    {
        private readonly OpenTicketDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IValidator<AddEmailAccountCommand> _validator;

        public AddEmailAccountCommandHandler(OpenTicketDbContext dbContext,
            IValidator<AddEmailAccountCommand> validator, IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Unit> Handle(AddEmailAccountCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken: cancellationToken);
            var entity = _mapper.Map<EmailAccount>(request);
            if (entity.Protocol == MailProtocolType.M365)
            {
                entity.ServerPort = 993;
                entity.ServerAddress = "outlook.office365.com";
                entity.UseSecureConnection = true;
            }
            await _dbContext.EmailAccounts.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}