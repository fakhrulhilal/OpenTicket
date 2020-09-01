using System;
using MediatR;
using OpenTicket.Domain.Command;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using OpenTicket.Data.Entity;

namespace OpenTicket.Domain.Mapper
{
    public class EditEmailAccountCommandHandler : IRequestHandler<EditEmailAccountCommand>
    {
        private readonly OpenTicketDbContext _db;
        private readonly IMapper _mapper;
        private readonly IValidator<EditEmailAccountCommand> _validator;

        public EditEmailAccountCommandHandler(OpenTicketDbContext db, IMapper mapper, IValidator<EditEmailAccountCommand> validator)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<Unit> Handle(EditEmailAccountCommand request, CancellationToken cancellationToken)
        {
            await _validator.ValidateAndThrowAsync(request, cancellationToken: cancellationToken);
            var emailAccount = await _db.EmailAccounts.FindAsync(request.Id);
            if (emailAccount == null) return Unit.Value;

            var entity = _mapper.Map<EmailAccount>(emailAccount);
            _db.EmailAccounts.Update(entity);
            await _db.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
