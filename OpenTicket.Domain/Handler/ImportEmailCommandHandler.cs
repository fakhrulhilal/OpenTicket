using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using OpenTicket.Domain.Command;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using OpenTicket.Domain.MailClient;
using OpenTicket.Domain.Utility;

namespace OpenTicket.Domain.Handler
{
    public class ImportEmailCommandHandler : IRequestHandler<ImportEmailCommand>
    {
        private readonly IMediator _mediator;
        private readonly IMailClientFactory[] _mailClientFactories;
        private readonly IMapper _mapper;

        public ImportEmailCommandHandler(IMediator mediator, IMapper mapper, IEnumerable<IMailClientFactory> mailClientFactories)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            var clientFactories = mailClientFactories as IMailClientFactory[] ?? mailClientFactories.ToArray();
            if (mailClientFactories == null || !clientFactories.Any()) throw new ArgumentNullException(nameof(mailClientFactories));
            _mailClientFactories = clientFactories;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<Unit> Handle(ImportEmailCommand request, CancellationToken cancellationToken)
        {
            var emailAccounts = (await _mediator.Send(new QueryEmailAccounts {IsActive = true}, cancellationToken))
                .ToArray();
            foreach (var emailAccount in emailAccounts)
            {
                var mailClientFactory = _mailClientFactories.FirstOrDefault(factory => factory.SupportedProtocol == emailAccount.Protocol);
                if (mailClientFactory == null)
                    throw new InvalidOperationException(
                        $"No registered mail client for protocol {emailAccount.Protocol.Humanize()}");

                using (var mailClient = mailClientFactory.Build(emailAccount))
                {
                    await mailClient.InitializeConnectionAsync(cancellationToken);
                    await mailClient.AuthenticateAsync(cancellationToken);
                    var messages = mailClient.FetchNewMessages();
                    foreach (var message in messages)
                    {
                        var command = _mapper.Map<CreateTicketCommand>(message);
                        command.EmailAccountId = emailAccount.Id;
                        await _mediator.Send(command, cancellationToken);
                        await mailClient.DeleteAsync(message, cancellationToken);
                    }
                }
            }
            return Unit.Value;
        }
    }
}
