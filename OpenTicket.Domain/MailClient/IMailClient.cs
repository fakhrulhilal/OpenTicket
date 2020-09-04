using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTicket.Domain.MailClient
{
    public interface IMailClient : System.IDisposable
    {
        Task InitializeConnectionAsync(CancellationToken cancellationToken);
        Task AuthenticateAsync(CancellationToken cancellationToken);
        IEnumerable<IMailMessage> FetchNewMessages();
        Task DeleteAsync(IMailMessage message, CancellationToken cancellationToken);
    }
}
