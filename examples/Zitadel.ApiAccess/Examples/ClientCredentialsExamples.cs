using System.Text.Json;

using Microsoft.Extensions.DependencyInjection;

using Zitadel.Abstractions;
using Zitadel.Abstractions.Credentials;
using Zitadel.AccessTokenManagement.Extensions;
using Zitadel.Auth.V1;

using OidcScopes = Duende.IdentityModel.OidcConstants.StandardScopes;

namespace Zitadel.ApiAccess.Examples;

public static class ClientCredentialsExamples
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

    /// <summary>
    /// Retrieves the details of the current authenticated user using client credentials.
    /// </summary>
    /// <returns>
    /// An asynchronous task that represents the operation. The task result contains the response
    /// for retrieving the user details, including user-specific information.
    /// </returns>
    public static async Task GetMyUser()
    {
        var services = new ServiceCollection();
        services.ConfigureZitadelServiceAccount(c =>
        {
            c.Authority = "https://zitadel-instance-abcdef.zitadel.cloud";
            c.Scopes = [OidcScopes.OpenId, OidcScopes.Profile, ZitadelScopes.ZitadelProject];
            c.ClientCredentials = new ZitadelServiceAccount.ClientCredentialsProfile
            {
                ClientId = "123456789012345678@yourproject",
                ClientSecret = "myclientsecret",
            };
        });

        services
            .AddGrpcClient<AuthService.AuthServiceClient>(c => c.Address = new Uri("https://zitadel-instance-abcdef.zitadel.cloud/"))
            .WithZitadelServiceAccount();

        var sp = services.BuildServiceProvider();
        var client = sp.GetRequiredService<AuthService.AuthServiceClient>();
        var result = await client.GetMyUserAsync(new GetMyUserRequest());

        Console.WriteLine($"""
                           ========== CLIENT CREDENTIALS RESULT ================= 
                           {JsonSerializer.Serialize(result.User, SerializerOptions)}
                           """);
        Console.WriteLine();
    }
}

