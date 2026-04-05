using Microsoft.Extensions.Options;

using Zitadel.Abstractions.Credentials;

namespace Zitadel.AccessTokenManagement.Options;

internal sealed class ZitadelServiceAccountValidator : IValidateOptions<ZitadelServiceAccount>
{
    public ValidateOptionsResult Validate(string? name, ZitadelServiceAccount options)
    {
        var failures = GetValidationFailures(options).ToArray();
        return failures.Length == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(failures);
    }

    private static IEnumerable<string> GetValidationFailures(ZitadelServiceAccount options)
    {
        return CheckOnlyOneProfileDefined(options)
            .Concat(CheckProperties(options))
            .Concat(CheckPat(options.Pat))
            .Concat(CheckClientCredentials(options.ClientCredentials))
            .Concat(CheckJwtProfile(options.Jwt));
    }

    private static IEnumerable<string> CheckProperties(ZitadelServiceAccount options)
    {
        if (string.IsNullOrEmpty(options.TokenEndpoint) && options.Pat == null) yield return "Token endpoint cannot be null or empty";
        if (!Uri.IsWellFormedUriString(options.TokenEndpoint, UriKind.Absolute) && options.Pat == null) yield return "Please provide a well-formed token endpoint";
        if (options.Scopes.Length == 0 && options.Pat == null) yield return "Scopes cannot empty";
    }

    private static IEnumerable<string> CheckOnlyOneProfileDefined(ZitadelServiceAccount options)
    {
        var definedProfiles = 0;
        if (options.Jwt != null) definedProfiles++;
        if (options.ClientCredentials != null) definedProfiles++;
        if (options.Pat != null) definedProfiles++;

        if (definedProfiles == 0) yield return "Please configure an authentication method for the service account";
        if (definedProfiles > 1) yield return "A maximum of 1 authentication methods can be configured per service account";
    }

    private static IEnumerable<string> CheckPat(ZitadelServiceAccount.PatProfile? patProfile)
    {
        if (patProfile == null || !string.IsNullOrWhiteSpace(patProfile.AccessToken)) yield break;

        yield return "Configured Pat access token cannot be empty or whitespace";
    }

    private static IEnumerable<string> CheckClientCredentials(
        ZitadelServiceAccount.ClientCredentialsProfile? clientCredentials)
    {
        if (clientCredentials == null) yield break;

        if (string.IsNullOrWhiteSpace(clientCredentials.ClientId)) yield return "Client id cannot be empty or whitespace";
        if (string.IsNullOrWhiteSpace(clientCredentials.ClientSecret)) yield return "Client secret cannot be empty or whitespace";
    }

    private static IEnumerable<string> CheckJwtProfile(ZitadelServiceAccount.JwtProfile? jwtProfile)
    {
        if (jwtProfile == null) yield break;

        if (string.IsNullOrWhiteSpace(jwtProfile.UserId)) yield return "User id cannot be empty or whitespace";
        if (string.IsNullOrWhiteSpace(jwtProfile.KeyId)) yield return "Key id cannot be empty or whitespace";
        if (string.IsNullOrWhiteSpace(jwtProfile.Key) && jwtProfile.SignJwtAsync == null) yield return "Please provide either a private key or a signing delegate to offload the signing of the JWT";
    }
}
