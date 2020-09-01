﻿using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using OpenTicket.Domain.Command;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using OpenTicket.Data.Entity;
using OpenTicket.Domain.MailClient;
using OpenTicket.Domain.Utility;

namespace OpenTicket.Domain.Handler
{
    public class ImportEmailCommandHandler : IRequestHandler<ImportEmailCommand>
    {
        private readonly OpenTicketDbContext _db;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IMailClientFactory[] _mailClientFactories;

        public ImportEmailCommandHandler(OpenTicketDbContext db, IMediator mediator, IMapper mapper, IEnumerable<IMailClientFactory> mailClientFactories)
        {
            var clientFactories = mailClientFactories as IMailClientFactory[] ?? mailClientFactories.ToArray();
            if (mailClientFactories == null || !clientFactories.Any()) throw new ArgumentNullException(nameof(mailClientFactories));
            _mailClientFactories = clientFactories;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<Unit> Handle(ImportEmailCommand request, CancellationToken cancellationToken)
        {
            var emailAccounts = (await _mediator.Send(new QueryEmailAccounts(), cancellationToken)).ToArray();
            foreach (var emailAccount in emailAccounts)
            {
                var mailClientFactory = _mailClientFactories.FirstOrDefault(factory => factory.SupportedProtocol == emailAccount.Protocol);
                if (mailClientFactory == null)
                    throw new InvalidOperationException(
                        $"No registered mail client for protocol {emailAccount.Protocol.Humanize()}");

                using (var mailClient = mailClientFactory.Build(emailAccount))
                {
                    /**
                     * TODO: 
                     * 1. Use mail client factory to build the client
                     * 2. Initialize connection to mail server
                     * 3. Fetch all new mail IDs
                     * 4. Fetch each mail message
                     * 5. Remove the email when requested
                     * 6. Create ticket from email
                     */
                }
            }
            return Unit.Value;
        }
    }
}
