using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenTicket.Data.Entity;

namespace OpenTicket.Domain.Command
{
    public class SaveEmailAccountTokenCommand : IRequest<Unit>
    {
        public string Email { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        
        public class Handler : IRequestHandler<SaveEmailAccountTokenCommand>
        {
            private readonly OpenTicketDbContext _db;

            public Handler(OpenTicketDbContext db)
            {
                _db = db ?? throw new ArgumentNullException(nameof(db));
            }

            public async Task<Unit> Handle(SaveEmailAccountTokenCommand request, CancellationToken cancellationToken)
            {
                if (request == null) throw new ArgumentNullException(nameof(request));
                var draftEmailAccount =
                    await _db.EmailAccounts.FirstOrDefaultAsync(
                        ea => ea.Email == request.Email && ea.DraftId.HasValue, cancellationToken);
                if (draftEmailAccount == null) return Unit.Value;

                draftEmailAccount.AccessToken = request.AccessToken;
                draftEmailAccount.RefreshToken = request.RefreshToken;
                draftEmailAccount.LastUpdateAccessToken = DateTime.UtcNow;
                _db.EmailAccounts.Update(draftEmailAccount);
                await _db.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}