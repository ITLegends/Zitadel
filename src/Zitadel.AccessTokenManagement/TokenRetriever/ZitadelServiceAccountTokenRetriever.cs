using Duende.AccessTokenManagement;
using Duende.AccessTokenManagement.DPoP;

using Microsoft.Extensions.Options;

using Zitadel.Abstractions.Credentials;

namespace Zitadel.AccessTokenManagement.TokenRetriever;

internal sealed class ZitadelServiceAccountTokenRetriever : AccessTokenRequestHandler.ITokenRetriever
{
    private readonly IClientCredentialsTokenManager _clientCredentialsTokenManager;
    private readonly ClientCredentialsClientName _clientName;
    private readonly IOptionsMonitor<ZitadelServiceAccount> _serviceAccountOptions;
    private readonly ITokenRequestCustomizer? _customizer;

    public ZitadelServiceAccountTokenRetriever(
        IClientCredentialsTokenManager clientCredentialsTokenManager,
        ClientCredentialsClientName clientName,
        IOptionsMonitor<ZitadelServiceAccount> serviceAccountOptions,
        ITokenRequestCustomizer? customizer = null)
    {
        _clientCredentialsTokenManager = clientCredentialsTokenManager;
        _clientName = clientName;
        _serviceAccountOptions = serviceAccountOptions;
        _customizer = customizer;
    }

    /// <inheritdoc />
    public async Task<TokenResult<AccessTokenRequestHandler.IToken>> GetTokenAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var serviceAccount = _serviceAccountOptions.Get(_clientName);

        // In case a PAT token is configured we return our configured token with a max lifetime value.
        if (serviceAccount.Pat is not null)
        {
            return TokenResult.Success<AccessTokenRequestHandler.IToken>(new ClientCredentialsToken
            {
                ClientId = ClientId.Parse(_clientName),
                AccessToken = AccessToken.Parse(serviceAccount.Pat.AccessToken),
                Expiration = DateTimeOffset.MaxValue,
                Scope = null,
                AccessTokenType = null,
                DPoPJsonWebKey = null,
            });
        }

        var baseParameters = new TokenRequestParameters
        {
            ForceTokenRenewal = request.GetForceRenewal(),
        };

        var parameters = _customizer != null
            ? await _customizer.Customize(ToHttpRequestContext(request), baseParameters, ct)
            : baseParameters;

        var getTokenResult = await _clientCredentialsTokenManager.GetAccessTokenAsync(_clientName, parameters, ct);

        if (getTokenResult.WasSuccessful(out var token, out var error)) return token;

        return error;
    }

    private static HttpRequestContext ToHttpRequestContext(HttpRequestMessage request)
    {
        return new HttpRequestContext
        {
            Method = request.Method.Method,
            RequestUri = request.RequestUri,
            Headers = request.Headers,
        };
    }
}
