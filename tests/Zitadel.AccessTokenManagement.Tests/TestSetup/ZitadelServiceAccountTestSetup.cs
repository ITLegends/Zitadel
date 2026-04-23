using System.Text;

using Microsoft.Extensions.DependencyInjection;

using RichardSzalay.MockHttp;

using Zitadel.Abstractions.Credentials;

namespace Zitadel.AccessTokenManagement.Tests.TestSetup;

public static class ZitadelServiceAccountTestSetup
{
    public const string Authority = "https://zitadel.localhost/";
    public const string TokenEndpoint = "https://zitadel.localhost/oauth/v2/token";
    public const string DownstreamApiBase = "https://example.localhost";
    public const string DefaultDownstreamClientName = "downstream";

    public const string Pat = "my-personal-access-token";

    public const string ClientId = "1234567890";
    public const string ClientSecret = "abcdefghijklm";

    public const string UserId = "9876543210";
    public const string KeyId = "key-1234567890";
    public const string ValidSignature = "ValidSignature";

    public static readonly string[] Scopes = ["openid"];
    
    public static readonly Action<ZitadelServiceAccount> PatConfiguration = o =>
    {
        o.Pat = new ZitadelServiceAccount.PatProfile { AccessToken = Pat };
    };

    public static readonly Action<ZitadelServiceAccount> ClientCredentialsConfiguration = o =>
    {
        o.Authority = Authority;
        o.Scopes = Scopes;
        o.ClientCredentials = new ZitadelServiceAccount.ClientCredentialsProfile
        {
            ClientId = ClientId, ClientSecret = ClientSecret,
        };
    };
    
    public static readonly Action<ZitadelServiceAccount> JwtProfileConfiguration = o =>
    {
        o.Authority = Authority;
        o.Scopes = Scopes;
        o.Jwt = new ZitadelServiceAccount.JwtProfile
        {
            UserId = UserId,
            KeyId = KeyId,
            SignJwtAsync = (_, _, _) => Task.FromResult(Encoding.UTF8.GetBytes(ValidSignature)),
        };
    };
    
    public static IServiceCollection CreateDefaultServiceCollection()
    {
        var serviceCollection = new ServiceCollection();
        
        serviceCollection.AddSingleton(_ => new MockHttpMessageHandler());
        serviceCollection.ConfigureHttpClientDefaults(b => b.ConfigurePrimaryHttpMessageHandler<MockHttpMessageHandler>());
        return serviceCollection;
    }

    public static IHttpClientBuilder AddDownstreamHttpClient(this IServiceCollection services, string clientName = DefaultDownstreamClientName)
    {
        return services.AddHttpClient(clientName, client => client.BaseAddress = new Uri(DownstreamApiBase));
    }
    
    public static HttpClient GetHttpClient(this IServiceProvider provider, string clientName = DefaultDownstreamClientName)
    {
        return provider
            .GetRequiredService<IHttpClientFactory>()
            .CreateClient(clientName);
    }

    public static MockHttpMessageHandler GetHttpMock(this IServiceProvider provider)
    {
        return provider.GetRequiredService<MockHttpMessageHandler>();
    }
}
