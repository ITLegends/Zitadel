using System.Text.Json;

using Grpc.Net.ClientFactory;

using Microsoft.Extensions.DependencyInjection;

using Zitadel.Abstractions;
using Zitadel.Abstractions.Credentials;
using Zitadel.AccessTokenManagement.Extensions;
using Zitadel.Auth.V1;

using OidcScopes = Duende.IdentityModel.OidcConstants.StandardScopes;

namespace Zitadel.ApiAccess.Examples;

public class MultipleCredentialsExample
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };
    
    private const string PatAccountName = "sa-pat";
    private const string PatClientName = "pat-client";

    private const string JwtAccountName = "sa-jwt";
    private const string JwtClientName = "jwt-client";

    /// <summary>
    /// Retrieves information about the current authenticated user from multiple credentials.
    /// This method uses two separate gRPC clients, one configured with a personal access token (PAT)
    /// and the other with a JSON Web Token (JWT), to fetch user details via the Zitadel AuthService.
    /// It outputs the retrieved user data in a serialized JSON format.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    public static async Task GetMyUsers()
    {
        var services = new ServiceCollection();
        
        ConfigurePatServiceAccount(services);
        ConfigureJwtServiceAccount(services);
        
        var sp = services.BuildServiceProvider();
        var factory = sp.GetRequiredService<GrpcClientFactory>();
        var patResult = await factory.CreateClient<AuthService.AuthServiceClient>(PatClientName).GetMyUserAsync(new GetMyUserRequest());
        var jwtResult = await factory.CreateClient<AuthService.AuthServiceClient>(JwtClientName) .GetMyUserAsync(new GetMyUserRequest());

        Console.WriteLine($"""
                           ========== MULTIPLE USERS RESULT #1 ================= 
                           {JsonSerializer.Serialize(patResult.User, SerializerOptions)}
                           
                           ========== MULTIPLE USERS RESULT #2 ================= 
                           {JsonSerializer.Serialize(jwtResult.User, SerializerOptions)}
                           """);
        Console.WriteLine();
    }

    private static void ConfigurePatServiceAccount(IServiceCollection services)
    {
        services.ConfigureZitadelServiceAccount(PatAccountName, c =>
        {
            c.Pat = new ZitadelServiceAccount.PatProfile
            {
                AccessToken = "myaccesstoken"
            };
        });
        
        services
            .AddGrpcClient<AuthService.AuthServiceClient>(PatClientName, c => c.Address = new Uri("https://zitadel-instance-abcdef.zitadel.cloud/"))
            .WithZitadelServiceAccount(PatAccountName);
    }

    private static void ConfigureJwtServiceAccount(IServiceCollection services)
    {
        services.ConfigureZitadelServiceAccount(JwtAccountName, c =>
        {
            c.TokenEndpoint = "https://zitadel-instance-abcdef.zitadel.cloud/oauth/v2/token";
            c.Scopes = [OidcScopes.OpenId, OidcScopes.Profile, ZitadelScopes.ZitadelProject];
            c.Jwt = new ZitadelServiceAccount.JwtProfile
            {
                KeyId = "123456789012345678",
                UserId = "123456789012345678",
                Key = "-----BEGIN RSA PRIVATE KEY-----\nMY SUPER SECRET KEY\n-----END RSA PRIVATE KEY-----\n",
            };
        });

        services
            .AddGrpcClient<AuthService.AuthServiceClient>(JwtClientName, c => c.Address = new Uri("https://zitadel-instance-abcdef.zitadel.cloud/"))
            .WithZitadelServiceAccount(JwtAccountName);
    }
}
