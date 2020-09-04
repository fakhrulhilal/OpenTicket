using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenTicket.Data.Entity;
using OpenTicket.Domain.Command;

namespace OpenTicket.Domain.Handler
{
    public class EditEmailAccountCommandHandler : IRequestHandler<EditEmailAccountCommand>
    {
        private readonly OpenTicketDbContext _db;
        private readonly IMapper _mapper;
        private readonly IValidator<EditEmailAccountCommand> _validator;

        public EditEmailAccountCommandHandler(OpenTicketDbContext db, IMapper mapper,
            IValidator<EditEmailAccountCommand> validator)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<Unit> Handle(EditEmailAccountCommand request, CancellationToken cancellationToken)
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
                entity.ServerPort = 993;
                entity.ServerAddress = "outlook.office365.com";
                entity.UseSecureConnection = true;
            }
            _db.EmailAccounts.Update(entity);
            await _db.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
