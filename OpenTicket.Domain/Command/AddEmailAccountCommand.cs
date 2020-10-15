using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenTicket.Data.Entity;
using EmailAccount = OpenTicket.Domain.MailClient.EmailAccount;
using MailProtocolType = OpenTicket.Domain.MailClient.MailProtocolType;

namespace OpenTicket.Domain.Command
{
    public class AddEmailAccountCommand : EmailAccount, IRequest<Unit>
    {
        internal const int DraftId = -1;

        public class Validator : AbstractValidator<AddEmailAccountCommand>
        {
            public Validator()
            {
                Include(new BaseValidator());
                bool IsLocalAccount(AddEmailAccountCommand cmd) => cmd.Protocol != MailProtocolType.M365;
                RuleFor(p => p.Password).NotEmpty().When(IsLocalAccount);
            }
        }

        public class Handler : IRequestHandler<AddEmailAccountCommand>
        {
            private readonly OpenTicketDbContext _dbContext;
            private readonly IMapper _mapper;
            private readonly IValidator<AddEmailAccountCommand> _validator;

            public Handler(OpenTicketDbContext dbContext,
                IValidator<AddEmailAccountCommand> validator, IMapper mapper)
            {
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _validator = validator ?? throw new ArgumentNullException(nameof(validator));
                _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            }

            public async Task<Unit> Handle(AddEmailAccountCommand request,
                CancellationToken cancellationToken)
            {
                await _validator.ValidateAndThrowAsync(request, cancellationToken: cancellationToken);
                var entity = _mapper.Map<Data.Entity.EmailAccount>(request);
                if (entity.Protocol == Data.Entity.MailProtocolType.M365)
                {
                    var draftEmailAccount =
                        await _dbContext.EmailAccounts.FirstOrDefaultAsync(
                            ea => ea.Email == request.Email && ea.DraftId == DraftId,
                            cancellationToken);
                    if (draftEmailAccount == null)
                        throw new InvalidOperationException("Token not acquired");
                    draftEmailAccount.ServerPort = 993;
                    draftEmailAccount.ServerAddress = "outlook.office365.com";
                    draftEmailAccount.UseSecureConnection = true;
                    draftEmailAccount.DraftId = null;
                    // TODO: populate other settings
                    draftEmailAccount.IsActive = entity.IsActive;
                    _dbContext.EmailAccounts.Update(draftEmailAccount);
                }
                else
                {
                    await _dbContext.EmailAccounts.AddAsync(entity, cancellationToken);
                }

                await _dbContext.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}