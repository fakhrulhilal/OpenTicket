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
    public class GetEmailAccountByIdQuery : IRequest<GetEmailAccountByIdQuery.Result>
    {
        public GetEmailAccountByIdQuery(int id) => Id = id;

        private int Id { get; }

        public class Result : MailClient.EmailAccount
        {
            public int Id { get; set; }
        }

        public class Validator : AbstractValidator<GetEmailAccountByIdQuery>
        {
            public Validator(OpenTicketDbContext db)
            {
                if (db == null) throw new ArgumentNullException(nameof(db));

                RuleFor(p => p.Id).Must(id => db.EmailAccounts.AsNoTracking().Any(ea => ea.Id == id))
                    .WithMessage(id => $"Email account with ID {id} must exist");
            }
        }
        
        public class Handler : IRequestHandler<GetEmailAccountByIdQuery, Result>
        {
            private readonly OpenTicketDbContext _db;
            private readonly IMapper _mapper;

            public Handler(OpenTicketDbContext db, IMapper mapper)
            {
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _db = db ?? throw new ArgumentNullException(nameof(db));
            }

            public async Task<Result> Handle(GetEmailAccountByIdQuery request,
                CancellationToken cancellationToken)
            {
                var emailAccount = await _db.EmailAccounts.AsNoTracking()
                    .FirstOrDefaultAsync(e => !e.DraftId.HasValue && e.Id == request.Id, cancellationToken);
                return _mapper.Map<Result>(emailAccount);
            }
        }
    }
}
