using Duende.AspNetCore.Authentication.OAuth2Introspection;
using Duende.IdentityModel.Client;

using Microsoft.Extensions.Options;

using Zitadel.Abstractions;
using Zitadel.AspNetCore.Authentication.Options;
using Zitadel.AspNetCore.ClientAssertions;

namespace Zitadel.AspNetCore.Options;

internal sealed class ConfigureZitadelIntrospectionOptions : IConfigureNamedOptions<OAuth2IntrospectionOptions>
{
    private readonly IOptionsMonitor<ZitadelIntrospectionOptions> _zitadelOptions;

    public ConfigureZitadelIntrospectionOptions(IOptionsMonitor<ZitadelIntrospectionOptions> zitadelOptions)
    {
        _zitadelOptions = zitadelOptions;
    }

    public void Configure(OAuth2IntrospectionOptions options)
    {
        Configure(string.Empty, options);
    }

    public void Configure(string? name, OAuth2IntrospectionOptions options)
    {
        var zitadelOptions = _zitadelOptions.Get(name ?? string.Empty);

        options.ClientId = zitadelOptions.ClientId;

        options.AuthenticationType = zitadelOptions.AuthenticationType;
        options.Authority = zitadelOptions.Authority;
        options.CacheDuration = zitadelOptions.CacheDuration;
        options.CacheKeyGenerator = zitadelOptions.CacheKeyGenerator;
        options.CacheKeyPrefix = zitadelOptions.CacheKeyPrefix;
        options.ClaimsIssuer = zitadelOptions.ClaimsIssuer;
        options.DiscoveryPolicy = zitadelOptions.DiscoveryPolicy;
        options.Events = zitadelOptions.Events;
        options.EventsType = zitadelOptions.EventsType;
        options.ForwardAuthenticate = zitadelOptions.ForwardAuthenticate;
        options.ForwardChallenge = zitadelOptions.ForwardChallenge;
        options.ForwardDefault = zitadelOptions.ForwardDefault;
        options.ForwardForbid = zitadelOptions.ForwardForbid;
        options.ForwardDefaultSelector = zitadelOptions.ForwardDefaultSelector;
        options.ForwardSignIn = zitadelOptions.ForwardSignIn;
        options.ForwardSignOut = zitadelOptions.ForwardSignOut;
        options.IntrospectionEndpoint = zitadelOptions.IntrospectionEndpoint;
        options.NameClaimType = zitadelOptions.NameClaimType;
        options.RoleClaimType = ZitadelClaimTypes.ProjectRoles;
        options.SaveToken = zitadelOptions.SaveToken;
        options.SkipTokensWithDots = zitadelOptions.SkipTokensWithDots;
        options.TokenRetriever = zitadelOptions.TokenRetriever;
        options.TokenTypeHint = zitadelOptions.TokenTypeHint;

        if (zitadelOptions.JwtProfile is not null)
        {
            options.ClientSecret = null;
            options.ClientCredentialStyle = ClientCredentialStyle.PostBody;
            options.Events.OnUpdateClientAssertion += context =>
                DefaultZitadelClientAssertion.UpdateClientAssertion(
                    context,
                    zitadelOptions.JwtProfile,
                    context.HttpContext.RequestAborted);
        }
        else
        {
            options.ClientSecret = zitadelOptions.ClientSecret;
            options.ClientCredentialStyle = ClientCredentialStyle.AuthorizationHeader;
            options.AuthorizationHeaderStyle = BasicAuthenticationHeaderStyle.Rfc6749;
        }
    }
}
