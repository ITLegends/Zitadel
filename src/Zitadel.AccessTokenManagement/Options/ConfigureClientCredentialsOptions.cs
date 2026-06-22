using Duende.AccessTokenManagement;

using Microsoft.Extensions.Options;

using Zitadel.Abstractions.Credentials;

namespace Zitadel.AccessTokenManagement.Options;

internal sealed class ConfigureClientCredentialsClient : IConfigureNamedOptions<ClientCredentialsClient>
{
    private readonly IOptionsMonitor<ZitadelServiceAccount> _serviceAccountOptions;
    private readonly ZitadelServiceAccountRegistry _serviceAccountRegistry;

    public ConfigureClientCredentialsClient(IOptionsMonitor<ZitadelServiceAccount> serviceAccountOptions, IOptions<ZitadelServiceAccountRegistry> serviceAccountRegistry)
    {
        _serviceAccountOptions = serviceAccountOptions;
        _serviceAccountRegistry = serviceAccountRegistry.Value;
    }

    public void Configure(ClientCredentialsClient client)
    {
        Configure(string.Empty, client);
    }

    public void Configure(string? name, ClientCredentialsClient client)
    {
        if(!_serviceAccountRegistry.IsServiceAccountRegistered(name ?? string.Empty)) return;
        var serviceAccount = _serviceAccountOptions.Get(name ?? string.Empty);

        if (serviceAccount.Pat is not null) return;
        if (!Uri.TryCreate(new Uri(serviceAccount.Authority), serviceAccount.TokenPath, out var tokenEndpoint)) return;
        
        client.TokenEndpoint = tokenEndpoint;
        client.Scope = Scope.Parse(string.Join(" ", serviceAccount.Scopes));

        if (serviceAccount.ClientCredentials is not null)
        {
            client.ClientId = ClientId.Parse(serviceAccount.ClientCredentials.ClientId);
            client.ClientSecret = ClientSecret.Parse(serviceAccount.ClientCredentials.ClientSecret);
        }

        if (serviceAccount.Jwt is not null)
        {
            client.ClientId = ClientId.Parse(serviceAccount.Jwt.UserId);
            client.HttpClientName = $"{name}-backchannel";
        }
    }
}
