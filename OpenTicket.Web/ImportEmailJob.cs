using Hangfire.Server;
using MediatR;
using OpenTicket.Domain.Command;
using System;
using System.Threading.Tasks;

namespace OpenTicket.Web
{
    public class ImportEmailJob
    {
        private readonly IMediator _mediator;
        internal const string Name = "email-import";

        public ImportEmailJob(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task ExecuteAsync(PerformContext context) =>
            await _mediator.Send(new ImportEmailCommand(), context.CancellationToken.ShutdownToken);
    }
}