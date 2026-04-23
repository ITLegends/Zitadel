using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using Duende.AccessTokenManagement;
using Duende.IdentityModel;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

using Zitadel.Abstractions.Credentials;

namespace Zitadel.AccessTokenManagement.ClientAssertion;

/// <summary>
/// Provides an implementation of the IClientAssertionService interface for managing client assertions
/// required by Zitadel's client credentials flow. This service is responsible for generating JSON Web
/// Tokens (JWTs) to authenticate requests between a client and Zitadel's identity infrastructure.
/// </summary>
/// <remarks>
/// The ZitadelClientAssertionService interacts with the Zitadel service account and client credentials
/// configuration to create JWT-based client assertions. It determines the necessary information such as
/// client ID, key ID, and user ID before signing the JWT accordingly using RSA-SHA256.
/// If the service account's key information or signing delegate is missing, the service will return null,
/// indicating the inability to generate a client assertion.
///
/// The signing delegate can be used to offload the signing of a JWT to an external vault.
/// </remarks>
internal sealed class ZitadelClientAssertionService : IClientAssertionService
{
    private readonly IOptionsMonitor<ClientCredentialsClient> _options;
    private readonly IOptionsMonitor<ZitadelServiceAccount> _saOptions;
    private readonly IServiceProvider _serviceProvider;

    public ZitadelClientAssertionService(
        IOptionsMonitor<ClientCredentialsClient> options,
        IOptionsMonitor<ZitadelServiceAccount> saOptions,
        IServiceProvider serviceProvider)
    {
        _options = options;
        _saOptions = saOptions;
        _serviceProvider = serviceProvider;
    }

    public async Task<Duende.IdentityModel.Client.ClientAssertion?> GetClientAssertionAsync(
        ClientCredentialsClientName? clientName = null,
        TokenRequestParameters? parameters = null,
        CancellationToken ct = default)
    {
        var clientCredentialsOptions = _options.Get(clientName);
        var serviceAccountOptions = _saOptions.Get(clientName);
        if (!clientCredentialsOptions.ClientId.HasValue || (serviceAccountOptions.Jwt == null)) return null;

        var now = DateTimeOffset.UtcNow;
        var headerBytes = JsonSerializer.SerializeToUtf8Bytes(new
        {
            alg = SecurityAlgorithms.RsaSha256,
            typ = JwtConstants.HeaderType,
            kid = serviceAccountOptions.Jwt.KeyId,
        });

        var payloadBytes = JsonSerializer.SerializeToUtf8Bytes(new
        {
            iss = serviceAccountOptions.Jwt.UserId,
            sub = serviceAccountOptions.Jwt.UserId,
            aud = serviceAccountOptions.Authority,
            iat = now.ToUnixTimeSeconds(),
            exp = now.AddMinutes(5).ToUnixTimeSeconds(),
        });

        var signingInput = $"{Base64UrlEncoder.Encode(headerBytes)}.{Base64UrlEncoder.Encode(payloadBytes)}";
        var signingInputBytes = Encoding.UTF8.GetBytes(signingInput);

        byte[] signature;

        if (serviceAccountOptions.Jwt.SignJwtAsync is not null)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();
            signature = await serviceAccountOptions.Jwt.SignJwtAsync(scope.ServiceProvider, signingInputBytes, ct);
        }
        else
        {
            signature = SignData(serviceAccountOptions.Jwt.Key!, signingInputBytes);
        }

        var jwt = $"{signingInput}.{Base64UrlEncoder.Encode(signature)}";
        var assertion = new Duende.IdentityModel.Client.ClientAssertion
        {
            Type = OidcConstants.GrantTypes.JwtBearer,
            Value = jwt,
        };

        return assertion;
    }

    private static byte[] SignData(string key, byte[] data)
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(key);

        return rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
}
