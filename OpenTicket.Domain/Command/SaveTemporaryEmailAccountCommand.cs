using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenTicket.Data.Entity;

namespace OpenTicket.Domain.Command
{
    public class SaveTemporaryEmailAccountCommand : MailClient.EmailAccount, IRequest
    {
        public int? Id { get; set; }
        
        public class Handler : IRequestHandler<SaveTemporaryEmailAccountCommand>
        {
            private readonly OpenTicketDbContext _db;
            private readonly IMapper _mapper;

            public Handler(OpenTicketDbContext db, IMapper mapper)
            {
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _db = db ?? throw new ArgumentNullException(nameof(db));
            }
            
            public async Task<Unit> Handle(SaveTemporaryEmailAccountCommand request,
                CancellationToken cancellationToken)
            {
                var emailAccount = _mapper.Map<EmailAccount>(request);
                bool isNewAccount = request.Id == AddEmailAccountCommand.DraftId;
                var pendingEmailAccount = isNewAccount
                    ? await _db.EmailAccounts.AsNoTracking().FirstOrDefaultAsync(
                        ea => ea.DraftId == request.Id && ea.Email == request.Email, cancellationToken)
                    : await _db.EmailAccounts.AsNoTracking()
                        .FirstOrDefaultAsync(ea => ea.DraftId == request.Id, cancellationToken);
                if (emailAccount.Protocol == MailProtocolType.M365)
                {
                    emailAccount.ServerAddress = "outlook.office365.com";
                    emailAccount.ServerPort = 993;
                    emailAccount.UseSecureConnection = true;
                }
                if (pendingEmailAccount != null)
                {
                    emailAccount.Id = pendingEmailAccount.Id;
                    _db.EmailAccounts.Update(emailAccount);
                }
                else
                {
                    await _db.EmailAccounts.AddAsync(emailAccount, cancellationToken);
                }
                await _db.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}