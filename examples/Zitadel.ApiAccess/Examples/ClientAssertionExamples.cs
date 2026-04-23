using System.Text.Json;

using Azure.Identity;
using Azure.Security.KeyVault.Keys.Cryptography;

using Microsoft.Extensions.DependencyInjection;

using Zitadel.Abstractions;
using Zitadel.Abstractions.Credentials;
using Zitadel.AccessTokenManagement.Extensions;
using Zitadel.Auth.V1;

namespace Zitadel.ApiAccess.Examples;

using OidcScopes = Duende.IdentityModel.OidcConstants.StandardScopes;

public static class ClientAssertionExamples
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

    /// <summary>
    /// Retrieves the current user associated with the authenticated session
    /// using a locally stored key for client assertion authentication.
    /// </summary>
    /// <remarks>
    /// The key can be created and downloaded on the zitadel account page under keys. 
    /// </remarks>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    public static async Task GetMyUserWithLocalKey()
    {
        var services = new ServiceCollection();
        services.ConfigureZitadelServiceAccount(c =>
        {
            c.Authority = "https://zitadel-instance-abcdef.zitadel.cloud";
            c.Scopes = [OidcScopes.OpenId, OidcScopes.Profile, ZitadelScopes.ZitadelProject];
            c.Jwt = new ZitadelServiceAccount.JwtProfile
            {
                KeyId = "123456789012345678",
                UserId = "123456789012345678",
                Key = "-----BEGIN RSA PRIVATE KEY-----\nMY SUPER SECRET KEY\n-----END RSA PRIVATE KEY-----\n",
            };
        });


        services
            .AddGrpcClient<AuthService.AuthServiceClient>(c => c.Address = new Uri("https://zitadel-instance-abcdef.zitadel.cloud/"))
            .WithZitadelServiceAccount();

        var sp = services.BuildServiceProvider();
        var client = sp.GetRequiredService<AuthService.AuthServiceClient>();
        var result = await client.GetMyUserAsync(new GetMyUserRequest());

        Console.WriteLine($"""
                           ========== CLIENT ASSERTION RESULT ================= 
                           {JsonSerializer.Serialize(result.User, SerializerOptions)}
                           """);
        Console.WriteLine();
    }

    /// <summary>
    /// Retrieves the current user associated with the authenticated session
    /// using a key stored in an external vault for client assertion authentication.
    /// </summary>
    /// <remarks>
    /// Perfect for during development for devs that should not have the private key on their machine but still needs
    /// access to the Zitadel API.
    /// </remarks>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    public static async Task GetMyUserWithExternalKey()
    {
        var services = new ServiceCollection();
        services.ConfigureZitadelServiceAccount(c =>
        {
            c.Authority = "https://zitadel-instance-abcdef.zitadel.cloud";
            c.Scopes = [OidcScopes.OpenId, OidcScopes.Profile, ZitadelScopes.ZitadelProject];
            c.Jwt = new ZitadelServiceAccount.JwtProfile
            {
                KeyId = "123456789012345678",
                UserId = "123456789012345678",
                SignJwtAsync = async (_, data, ct) =>
                {
                    var keyId = "https://my-vault.vault.azure.net/keys/Zitadel/123456789012345678";
                    var cryptoClient = new CryptographyClient(new Uri(keyId), new AzureCliCredential());
                    return (await cryptoClient.SignDataAsync(SignatureAlgorithm.RS256, data, ct)).Signature;
                }
            };
        });

        services
            .AddGrpcClient<AuthService.AuthServiceClient>(c => c.Address = new Uri("https://zitadel-instance-abcdef.zitadel.cloud/"))
            .WithZitadelServiceAccount();

        var sp = services.BuildServiceProvider();
        var client = sp.GetRequiredService<AuthService.AuthServiceClient>();
        var result = await client.GetMyUserAsync(new GetMyUserRequest());

        Console.WriteLine($"""
                           ========== CLIENT ASSERTION WITH VAULT RESULT ================= 
                           {JsonSerializer.Serialize(result.User, SerializerOptions)}
                           """);
        Console.WriteLine();
    }
}

