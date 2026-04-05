using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using Duende.AspNetCore.Authentication.OAuth2Introspection.Context;
using Duende.IdentityModel;
using Duende.IdentityModel.Client;

using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

using Zitadel.Abstractions.Credentials;

namespace Zitadel.AspNetCore.ClientAssertions;

internal static class DefaultZitadelClientAssertion
{
    public static async Task UpdateClientAssertion(UpdateClientAssertionContext context, ZitadelApplication.JwtProfile jwtProfile, CancellationToken ct)
    {
        var headerBytes = JsonSerializer.SerializeToUtf8Bytes(new
        {
            alg = SecurityAlgorithms.RsaSha256,
            typ = JwtConstants.HeaderType,
            kid = jwtProfile.KeyId,
        });

        var now = DateTimeOffset.UtcNow;
        var expiration = now.AddMinutes(55);
        var payloadBytes = JsonSerializer.SerializeToUtf8Bytes(new
        {
            iss = context.Options.ClientId,
            sub = context.Options.ClientId,
            aud = context.Options.Authority,
            iat = now.ToUnixTimeSeconds(),
            exp = expiration.ToUnixTimeSeconds(),
        });

        var signingInput = $"{Base64UrlEncoder.Encode(headerBytes)}.{Base64UrlEncoder.Encode(payloadBytes)}";
        var signingInputBytes = Encoding.UTF8.GetBytes(signingInput);

        byte[] signature = jwtProfile switch
        {
            { SignJwtAsync: not null } => await jwtProfile.SignJwtAsync(context.HttpContext.RequestServices, signingInputBytes, ct),
            { Key: not null } => SignData(jwtProfile.Key, signingInputBytes),
            _ => throw new ArgumentException("Either JWT key or signing delegate should be provided"),
        };

        var jwt = $"{signingInput}.{Base64UrlEncoder.Encode(signature)}";
        var assertion = new ClientAssertion
        {
            Type = OidcConstants.ClientAssertionTypes.JwtBearer,
            Value = jwt,
        };

        context.ClientAssertion = assertion;
        context.ClientAssertionExpirationTime = expiration.UtcDateTime;
    }

    private static byte[] SignData(string key, byte[] data)
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(key);

        return rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
}
