using System.Security.Claims;

using Duende.AspNetCore.Authentication.OAuth2Introspection;
using Duende.IdentityModel;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;

using Zitadel.Abstractions;
using Zitadel.AspNetCore.Authentication.Handler;
using Zitadel.AspNetCore.Authentication.Options;
using Zitadel.AspNetCore.Claims;
using Zitadel.AspNetCore.Options;

namespace Zitadel.AspNetCore.Extensions;

/// <summary>
/// Extensions for the <see cref="AuthenticationBuilder"/>
/// to add ZITADEL login capabilities.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Add ZITADEL authentication/authorization (via OpenIDConnect) to the application.
    /// This is commonly used when the application delivers server-side
    /// pages. This behaves like other external IDPs (e.g. AddGoogle, AddFacebook, ...).
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/> to configure.</param>
    /// <param name="configureOptions">
    /// An optional action to configure the oidc options
    /// (<see cref="OpenIdConnectOptions"/>).
    /// </param>
    /// <returns>The configured <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddZitadel(
        this AuthenticationBuilder builder,
        Action<OpenIdConnectOptions>? configureOptions = null)
        => builder.AddZitadel(ZitadelDefaults.AuthenticationScheme, configureOptions);

    /// <summary>
    /// Add ZITADEL authentication/authorization (via OpenIDConnect) to the application.
    /// This is commonly used when the application delivers server-side
    /// pages. This behaves like other external IDPs (e.g. AddGoogle, AddFacebook, ...).
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/> to configure.</param>
    /// <param name="authenticationScheme">The name for the authentication scheme.</param>
    /// <param name="configureOptions">
    /// An optional action to configure the oidc options
    /// (<see cref="OpenIdConnectOptions"/>).
    /// </param>
    /// <returns>The configured <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddZitadel(
        this AuthenticationBuilder builder,
        string authenticationScheme,
        Action<OpenIdConnectOptions>? configureOptions = null)
        => builder.AddZitadel(authenticationScheme, ZitadelDefaults.DisplayName, configureOptions);

    /// <summary>
    /// Add ZITADEL authentication/authorization (via OpenIDConnect) to the application.
    /// This is commonly used when the application delivers server-side
    /// pages. This behaves like other external IDPs (e.g. AddGoogle, AddFacebook, ...).
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/> to configure.</param>
    /// <param name="authenticationScheme">The name for the authentication scheme.</param>
    /// <param name="displayName">The display name for the authentication scheme.</param>
    /// <param name="configureOptions">
    /// An optional action to configure the oidc options
    /// (<see cref="OpenIdConnectOptions"/>).
    /// </param>
    /// <returns>The configured <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddZitadel(
        this AuthenticationBuilder builder,
        string authenticationScheme,
        string displayName,
        Action<OpenIdConnectOptions>? configureOptions = null)
    {
        builder.Services.AddTransient<IClaimsTransformation, ZitadelRoleClaimsTransformation>();
        return builder.AddOpenIdConnect(
            authenticationScheme,
            displayName,
            options =>
            {
                options.CallbackPath = ZitadelDefaults.CallbackPath;
                options.UsePkce = true;
                options.ResponseType = "code";
                options.GetClaimsFromUserInfoEndpoint = true;

                options.ClaimActions.MapUniqueJsonKey(ClaimTypes.NameIdentifier, JwtClaimTypes.Subject);
                options.ClaimActions.MapJsonKey(ZitadelClaimTypes.PrimaryDomain, ZitadelClaimTypes.PrimaryDomain);
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, JwtClaimTypes.Name);
                options.ClaimActions.MapJsonKey(ClaimTypes.GivenName, JwtClaimTypes.GivenName);
                options.ClaimActions.MapJsonKey(ClaimTypes.Surname, JwtClaimTypes.FamilyName);
                options.ClaimActions.MapJsonKey("nickname", JwtClaimTypes.NickName);
                options.ClaimActions.MapJsonKey("preferred_username", JwtClaimTypes.PreferredUserName);
                options.ClaimActions.MapJsonKey("gender", JwtClaimTypes.Gender);
                options.ClaimActions.MapJsonKey(ClaimTypes.Email, JwtClaimTypes.Email);
                options.ClaimActions.MapJsonKey("email_verified", JwtClaimTypes.EmailVerified);
                options.ClaimActions.MapJsonKey(ClaimTypes.Locality, JwtClaimTypes.Locale);
                options.ClaimActions.MapJsonKey("locale", JwtClaimTypes.Locale);
                options.ClaimActions.Add(new ZitadelRoleClaimsAction());
                options.ClaimActions.DeleteClaim(ZitadelClaimTypes.ProjectRoles);

                configureOptions?.Invoke(options);
            });
    }

    /// <summary>
    /// Add the ZITADEL introspection handler without caring for session handling.
    /// This is typically used by web APIs that only need to verify the access token that is presented.
    /// This handler can manage JWT as well as opaque access tokens.
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/> to configure.</param>
    /// <param name="configureOptions">
    /// An optional action to configure the ZITADEL handler options
    /// (<see cref="ZitadelIntrospectionOptions"/>).
    /// </param>
    /// <returns>The configured <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddZitadelIntrospection(
        this AuthenticationBuilder builder,
        Action<ZitadelIntrospectionOptions>? configureOptions = null)
        => builder.AddZitadelIntrospection(ZitadelDefaults.AuthenticationScheme, configureOptions);

    /// <summary>
    /// Add the ZITADEL introspection handler without caring for session handling.
    /// This is typically used by web APIs that only need to verify the access token that is presented.
    /// This handler can manage JWT as well as opaque access tokens.
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/> to configure.</param>
    /// <param name="authenticationScheme">The name for the authentication scheme.</param>
    /// <param name="configureOptions">
    /// An optional action to configure the ZITADEL handler options
    /// (<see cref="ZitadelIntrospectionOptions"/>).
    /// </param>
    /// <returns>The configured <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddZitadelIntrospection(
        this AuthenticationBuilder builder,
        string authenticationScheme,
        Action<ZitadelIntrospectionOptions>? configureOptions = null)
    {
        builder.Services.AddTransient<IClaimsTransformation, ZitadelRoleClaimsTransformation>();
        builder.Services.ConfigureOptions<ConfigureZitadelIntrospectionOptions>();
        builder.AddOAuth2Introspection(authenticationScheme);

        if (configureOptions != null)
        {
            builder.Services.Configure(authenticationScheme, configureOptions);
        }

        return builder;
    }

    /// <summary>
    /// Add a "fake" ZITADEL authentication. This should only be used for local
    /// development to fake an authentication/authorization. All calls are authenticated
    /// by default. If (e.g. for testing reasons) a specific call should NOT be authenticated,
    /// attach the header "x-zitadel-fake-auth" with the value "false" to the request.
    /// This specific request will then fail to authenticate.
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/> to configure.</param>
    /// <param name="configureOptions">Action to configure the <see cref="LocalFakeZitadelOptions"/>.</param>
    /// <returns>The configured <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddZitadelFake(
        this AuthenticationBuilder builder,
        Action<LocalFakeZitadelOptions>? configureOptions)
        => builder.AddZitadelFake(
            ZitadelDefaults.FakeAuthenticationScheme,
            configureOptions);

    /// <summary>
    /// Add a "fake" ZITADEL authentication. This should only be used for local
    /// development to fake an authentication/authorization. All calls are authenticated
    /// by default. If (e.g. for testing reasons) a specific call should NOT be authenticated,
    /// attach the header "x-zitadel-fake-auth" with the value "false" to the request.
    /// This specific request will then fail to authenticate.
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/> to configure.</param>
    /// <param name="authenticationScheme">The name for the authentication scheme to be used.</param>
    /// <param name="configureOptions">Action to configure the <see cref="LocalFakeZitadelOptions"/>.</param>
    /// <returns>The configured <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddZitadelFake(
        this AuthenticationBuilder builder,
        string authenticationScheme,
        Action<LocalFakeZitadelOptions>? configureOptions)
        => builder.AddZitadelFake(authenticationScheme, ZitadelDefaults.FakeDisplayName, configureOptions);

    /// <summary>
    /// Add a "fake" ZITADEL authentication. This should only be used for local
    /// development to fake an authentication/authorization. All calls are authenticated
    /// by default. If (e.g. for testing reasons) a specific call should NOT be authenticated,
    /// attach the header "x-zitadel-fake-auth" with the value "false" to the request.
    /// This specific request will then fail to authenticate.
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/> to configure.</param>
    /// <param name="authenticationScheme">The name for the authentication scheme to be used.</param>
    /// <param name="displayName">The display name for the authentication scheme.</param>
    /// <param name="configureOptions">Action to configure the <see cref="LocalFakeZitadelOptions"/>.</param>
    /// <returns>The configured <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddZitadelFake(
        this AuthenticationBuilder builder,
        string authenticationScheme,
        string displayName,
        Action<LocalFakeZitadelOptions>? configureOptions)
    {
        var options = new LocalFakeZitadelOptions();
        configureOptions?.Invoke(options);
        return builder.AddZitadelFake(authenticationScheme, displayName, options);
    }

    /// <summary>
    /// Add a "fake" ZITADEL authentication. This should only be used for local
    /// development to fake an authentication/authorization. All calls are authenticated
    /// by default. If (e.g. for testing reasons) a specific call should NOT be authenticated,
    /// attach the header "x-zitadel-fake-auth" with the value "false" to the request.
    /// This specific request will then fail to authenticate.
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/> to configure.</param>
    /// <param name="options">The <see cref="LocalFakeZitadelOptions"/> to use.</param>
    /// <returns>The configured <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddZitadelFake(
        this AuthenticationBuilder builder,
        LocalFakeZitadelOptions options)
        => builder.AddZitadelFake(
            ZitadelDefaults.FakeAuthenticationScheme,
            options);

    /// <summary>
    /// Add a "fake" ZITADEL authentication. This should only be used for local
    /// development to fake an authentication/authorization. All calls are authenticated
    /// by default. If (e.g. for testing reasons) a specific call should NOT be authenticated,
    /// attach the header "x-zitadel-fake-auth" with the value "false" to the request.
    /// This specific request will then fail to authenticate.
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/> to configure.</param>
    /// <param name="authenticationScheme">The name for the authentication scheme to be used.</param>
    /// <param name="options">The <see cref="LocalFakeZitadelOptions"/> to use.</param>
    /// <returns>The configured <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddZitadelFake(
        this AuthenticationBuilder builder,
        string authenticationScheme,
        LocalFakeZitadelOptions options)
        => builder.AddZitadelFake(authenticationScheme, ZitadelDefaults.FakeDisplayName, options);

    /// <summary>
    /// Add a "fake" ZITADEL authentication. This should only be used for local
    /// development to fake an authentication/authorization. All calls are authenticated
    /// by default. If (e.g. for testing reasons) a specific call should NOT be authenticated,
    /// attach the header "x-zitadel-fake-auth" with the value "false" to the request.
    /// This specific request will then fail to authenticate.
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/> to configure.</param>
    /// <param name="authenticationScheme">The name for the authentication scheme to be used.</param>
    /// <param name="displayName">The display name for the authentication scheme.</param>
    /// <param name="options">The <see cref="LocalFakeZitadelOptions"/> to use.</param>
    /// <returns>The configured <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddZitadelFake(
        this AuthenticationBuilder builder,
        string authenticationScheme,
        string displayName,
        LocalFakeZitadelOptions options)
        => builder.AddScheme<LocalFakeZitadelSchemeOptions, LocalFakeZitadelHandler>(
            authenticationScheme,
            displayName,
            o => o.FakeZitadelOptions = options);
}
