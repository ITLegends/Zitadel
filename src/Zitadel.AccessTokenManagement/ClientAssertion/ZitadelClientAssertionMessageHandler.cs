using Duende.IdentityModel;
using Duende.IdentityModel.Client;

namespace Zitadel.AccessTokenManagement.ClientAssertion;

/// <summary>
/// This handler modifies the outgoing Duende client credential HTTP request parameters so that the parameter names
/// match with what Zitadel expects.
/// </summary>
internal sealed class ZitadelClientAssertionMessageHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request is not ProtocolRequest protocolRequest || protocolRequest.ClientAssertion.Type != OidcConstants.GrantTypes.JwtBearer || !protocolRequest.Parameters.ContainsKey("client_assertion"))
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var newParameters = new Parameters
        {
            { "grant_type", protocolRequest.ClientAssertion.Type },
            { "assertion", protocolRequest.ClientAssertion.Value },
            { "scope", string.Join(' ', protocolRequest.Parameters.GetValues("scope")) },
        };

        protocolRequest.Parameters = newParameters;
        request.Content = new FormUrlEncodedContent(protocolRequest.Parameters);

        return await base.SendAsync(request, cancellationToken);
    }
}
