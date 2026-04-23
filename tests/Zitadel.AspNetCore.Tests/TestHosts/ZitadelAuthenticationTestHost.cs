using System.Collections;
using System.Security.Claims;
using System.Text;

using Duende.IdentityModel;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using RichardSzalay.MockHttp;

using Zitadel.Abstractions;
using Zitadel.Abstractions.Credentials;
using Zitadel.AspNetCore.Extensions;

namespace Zitadel.AspNetCore.Tests.TestHosts;

public static class ZitadelAuthenticationTestHost
{
    public const string UserId = "Test";
    public const string ViewerUserId = "TestViewer";
    public const string ValidToken = "VALID";
    public const string InvalidToken = "INVALID";
    public const string ViewerToken = "VIEWER";

    public const string AdminRole = "admin";
    public const string ViewerRole = "viewer";

    public static readonly string[] Roles = [AdminRole, ViewerRole];
    public readonly static Claim AdditionalClaim = new("foo", "bar");

    public const string Authority = "https://zitadel.localhost";
    public const string ClientId = "1234567890";
    public const string ClientSecret = "abcdefghijklm";
    public const string AppId = "1122334455667788";
    public const string KeyId = "0987654321";

    public const string ValidSignature = "ValidSignature";
    
    public static IHost CreateClientCredentialsHost()
    {
        return CreateHost(services => services
            .AddAuthentication(ZitadelDefaults.AuthenticationScheme)
            .AddZitadelIntrospection(options =>
            {
                options.Authority = Authority;
                options.ClientId = ClientId;
                options.ClientSecret = ClientSecret;
            }));
    }
    
    public static IHost CreateClientAssertionHost()
    {
        return CreateHost(services => services
            .AddAuthentication(ZitadelDefaults.AuthenticationScheme)
            .AddZitadelIntrospection(options =>
            {
                options.Authority = Authority;
                options.ClientId = ClientId;
                options.Jwt = new ZitadelApplication.JwtProfile
                {
                    AppId = AppId,
                    KeyId = KeyId,
                    SignJwtAsync = (_, _, _) => Task.FromResult(Encoding.UTF8.GetBytes(ValidSignature)),
                };
            }));
    }

    public static IHost CreateZitadelFakeHost()
    {
        return CreateHost(services => services
            .AddAuthentication(ZitadelDefaults.AuthenticationScheme)
            .AddZitadelFake(ZitadelDefaults.AuthenticationScheme, options =>
            {
                options.FakeZitadelId = UserId;
                options.AdditionalClaims = [AdditionalClaim];
                options.Roles = [ViewerRole];

                options.Events.OnZitadelFakeAuth = context =>
                {
                    if (context.FakeZitadelId == UserId)
                    {
                        context.AddRole(AdminRole);
                    }

                    return Task.CompletedTask;
                };
            }));
    }

    private static IHost CreateHost(Action<IServiceCollection> authenticationSetup)
    {
        var builder = WebApplication.CreateSlimBuilder();
        
        builder.Services.SetupIntrospectionClient();
        builder.Services.AddAuthorization();
        authenticationSetup.Invoke(builder.Services);

        builder.WebHost.UseTestServer();
        
        var app = builder.Build();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();
            
        app.MapGet("unauthed", GetAnonymousResponse).AllowAnonymous();
        app.MapGet("authed", GetAuthenticatedResponse).RequireAuthorization();
        app.MapGet("admin", GetAdminResponse).RequireAuthorization();
        return app;
    }

    private static void SetupIntrospectionClient(this IServiceCollection services)
    {
        services.AddSingleton<MockHttpMessageHandler>(_ => IntrospectionClientMock.SetupMock());
        
        services.ConfigureHttpClientDefaults(c => c.ConfigurePrimaryHttpMessageHandler<MockHttpMessageHandler>());
    }
    
    private static IResult GetAnonymousResponse() => Results.Ok(new AnonymousResponse("Pong"));
    private static IResult GetAuthenticatedResponse(HttpContext context)
    {
        var response = new AuthenticatedResponse(
            Ping: "Pong",
            AuthType: context.User.Identity?.AuthenticationType ?? string.Empty,
            UserId: context.User.FindFirstValue(JwtClaimTypes.Name) ?? string.Empty,
            Claims: context.User.Claims.Select(c => new KeyValuePair<string, string>(c.Type, c.Value)));
        return Results.Ok(response);
    }
    
    [Authorize(Roles = "admin")]
    private static IResult GetAdminResponse(HttpContext context) => GetAuthenticatedResponse(context);

    internal record AnonymousResponse(string Ping);
    internal record AuthenticatedResponse(string Ping, string AuthType, string UserId, IEnumerable<KeyValuePair<string, string>> Claims);
}

public class ZitadelAuthenticationTestHosts : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return [ZitadelAuthenticationTestHost.CreateClientCredentialsHost()];
        yield return [ZitadelAuthenticationTestHost.CreateClientAssertionHost()];
        yield return [ZitadelAuthenticationTestHost.CreateZitadelFakeHost()];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
