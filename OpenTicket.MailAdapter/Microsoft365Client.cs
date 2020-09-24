using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Security;
using OpenTicket.Domain.Command;
using OpenTicket.Helper;
using OpenTicket.Domain.MailClient;

namespace OpenTicket.MailAdapter
{
    public class Microsoft365Client : ImapClient
    {
        public Microsoft365Client(EmailAccount account) : base(account)
        { }

        public override async Task AuthenticateAsync(CancellationToken cancellationToken)
        {
            if (!(Account is QueryEmailAccounts.Result externalAccount))
                throw new InvalidOperationException("Not valid M365 account");
            
            if (string.IsNullOrEmpty(externalAccount.AccessToken) || string.IsNullOrEmpty(externalAccount.RefreshToken))
                throw new InvalidOperationException($"No token granted for email account: {externalAccount.Email}");

            var refreshTokenRequest = new RefreshTokenRequest
            {
                ClientId = externalAccount.ClientId,
                Secret = externalAccount.Secret,
                Tenant = externalAccount.Tenant,
                RefreshToken = externalAccount.RefreshToken
            };
            refreshTokenRequest.Scopes.AddRange(M365Helper.EmailScopes);
            var token = await OAuth2Helper.RefreshTokenAsync(refreshTokenRequest);
            var auth = new SaslMechanismOAuth2(externalAccount.Email, token.AccessToken);
            await Client.AuthenticateAsync(auth, cancellationToken);
        }
    }
}
