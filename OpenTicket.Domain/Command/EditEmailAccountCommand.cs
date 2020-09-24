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

        public class Handler : IRequestHandler<EditEmailAccountCommand>
        {
            private readonly OpenTicketDbContext _db;
            private readonly IMapper _mapper;
            private readonly IValidator<EditEmailAccountCommand> _validator;

            public Handler(OpenTicketDbContext db, IMapper mapper,
                IValidator<EditEmailAccountCommand> validator)
            {
                _validator = validator ?? throw new ArgumentNullException(nameof(validator));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _db = db ?? throw new ArgumentNullException(nameof(db));
            }

            public async Task<Unit> Handle(EditEmailAccountCommand request,
                CancellationToken cancellationToken)
            {
                await _validator.ValidateAndThrowAsync(request, cancellationToken: cancellationToken);
                var existingEmailAccount = await _db.EmailAccounts.AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
                if (existingEmailAccount == null) return Unit.Value;

                var entity = _mapper.Map<EmailAccount>(request);
                if (string.IsNullOrEmpty(entity.Password))
                    entity.Password = existingEmailAccount.Password;
                if (entity.Protocol == MailProtocolType.M365)
                {
                    var draftEmailAccount = await _db.EmailAccounts.FirstOrDefaultAsync(
                        ea => ea.Email == request.Email && ea.DraftId == request.Id, cancellationToken);
                    if (draftEmailAccount == null)
                        throw new InvalidOperationException("Token not acquired");

                    entity.ServerPort = 993;
                    entity.ServerAddress = "outlook.office365.com";
                    entity.UseSecureConnection = true;
                    entity.AccessToken = draftEmailAccount.AccessToken;
                    entity.RefreshToken = draftEmailAccount.RefreshToken;
                    entity.LastUpdateAccessToken = draftEmailAccount.LastUpdateAccessToken;
                    _db.EmailAccounts.Remove(draftEmailAccount);
                }

                _db.EmailAccounts.Update(entity);
                await _db.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}