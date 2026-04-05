using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

using Duende.IdentityModel;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

using RichardSzalay.MockHttp;

using Zitadel.AspNetCore.Tests.TestData;

namespace Zitadel.AspNetCore.Tests.TestHosts;

public static class IntrospectionClientMock
{
    public static MockHttpMessageHandler SetupMock()
    {
        var handler = new MockHttpMessageHandler();

        handler
            .SetupDiscoveryResponses()
            .SetupClientAssertionResponses()
            .SetupClientCredentialResponses();
        
        // Default introspection response
        handler
            .When("https://zitadel.localhost/oauth/v2/introspect")
            .Respond(_ => new HttpResponseMessage(HttpStatusCode.Unauthorized));
        
        return handler;
    }

    private static Dictionary<string, object> GetActiveToken(bool isAdmin)
    {
        return new Dictionary<string, object>
        {
            ["active"] = true,
            ["aud"] = ZitadelAuthenticationTestHost.ClientId,
            ["iss"] = ZitadelAuthenticationTestHost.Authority,
            ["clientId"] = ZitadelAuthenticationTestHost.ClientId,
            ["exp"] = DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeSeconds(),
            ["iat"] = DateTimeOffset.UtcNow.AddMinutes(-30).ToUnixTimeSeconds(),
            ["jti"] = "9203190123901",
            ["nbf"] = DateTimeOffset.UtcNow.AddMinutes(-30).ToUnixTimeSeconds(),
            ["scope"] = "openid",
            ["tokenType"] = "Bearer",
            ["username"] = "text@example.com",
            ["name"] = isAdmin ? ZitadelAuthenticationTestHost.UserId : ZitadelAuthenticationTestHost.ViewerUserId,
            ["urn:zitadel:iam:org:project:roles"] = isAdmin ? ZitadelRoleData.AdminAndViewerRoles : ZitadelRoleData.ViewerRole,
            ["foo"] = "bar",
        };
    }

    private static Dictionary<string, object> GetInactiveToken()
    {
        return new Dictionary<string, object>
        {
            ["active"] = false,
        };
    }

    private static MockHttpMessageHandler SetupDiscoveryResponses(this MockHttpMessageHandler handler)
    {
        handler
            .When("https://zitadel.localhost/.well-known/openid-configuration")
            .Respond(MediaTypeNames.Application.Json, IntrospectionData.DiscoveryDocumentResponse); 
        
        handler
            .When("https://zitadel.localhost/oauth/v2/keys")
            .Respond(MediaTypeNames.Application.Json, IntrospectionData.KeysResponse);
        
        return handler;
    }

    private static MockHttpMessageHandler SetupClientAssertionResponses(this MockHttpMessageHandler handler)
    {
        handler
            .When("https://zitadel.localhost/oauth/v2/introspect")
            .WithFormData(new Dictionary<string, string>
            {
                { "token", ZitadelAuthenticationTestHost.ValidToken }, 
                { "token_type_hint", "access_token" },
                { "client_id", ZitadelAuthenticationTestHost.ClientId },
                { "client_assertion_type", OidcConstants.ClientAssertionTypes.JwtBearer },
            })
            .With(IsValidJwt)
            .RespondAsJson(GetActiveToken(isAdmin: true));

        handler
            .When("https://zitadel.localhost/oauth/v2/introspect")
            .WithFormData(new Dictionary<string, string>
            {
                { "token", ZitadelAuthenticationTestHost.ViewerToken }, 
                { "token_type_hint", "access_token" },
                { "client_id", ZitadelAuthenticationTestHost.ClientId },
                { "client_assertion_type", OidcConstants.ClientAssertionTypes.JwtBearer},
            })
            .With(IsValidJwt)
            .RespondAsJson(GetActiveToken(isAdmin: false));
        
        handler
            .When("https://zitadel.localhost/oauth/v2/introspect")
            .WithFormData(new Dictionary<string, string>
            {
                { "token", ZitadelAuthenticationTestHost.InvalidToken }, 
                { "token_type_hint", "access_token" },
                { "client_id", ZitadelAuthenticationTestHost.ClientId },
                { "client_assertion_type", OidcConstants.ClientAssertionTypes.JwtBearer },
            })
            .With(IsValidJwt)
            .RespondAsJson(GetInactiveToken());
        
        return handler;
    }

    private static MockHttpMessageHandler SetupClientCredentialResponses(this MockHttpMessageHandler handler)
    {
        var authorizationHeaderPayload = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ZitadelAuthenticationTestHost.ClientId}:{ZitadelAuthenticationTestHost.ClientSecret}"));
        var expectedAuthorizationHeader = $"{AuthenticationSchemes.Basic} {authorizationHeaderPayload}";

        handler
            .When("https://zitadel.localhost/oauth/v2/introspect")
            .WithHeaders(HeaderNames.Authorization, expectedAuthorizationHeader)
            .WithFormData(new Dictionary<string, string>
            {
                { "token", ZitadelAuthenticationTestHost.ValidToken }, 
                { "token_type_hint", "access_token" }
            })
            .RespondAsJson(GetActiveToken(isAdmin: true));
        
        handler
            .When("https://zitadel.localhost/oauth/v2/introspect")
            .WithHeaders(HeaderNames.Authorization, expectedAuthorizationHeader)
            .WithFormData(new Dictionary<string, string>
            {
                { "token", ZitadelAuthenticationTestHost.ViewerToken }, 
                { "token_type_hint", "access_token" }
            })
            .RespondAsJson(GetActiveToken(isAdmin: false));
        
        handler
            .When("https://zitadel.localhost/oauth/v2/introspect")
            .WithHeaders(HeaderNames.Authorization, expectedAuthorizationHeader)
            .WithFormData(new Dictionary<string, string>
            {
                { "token", ZitadelAuthenticationTestHost.InvalidToken }, 
                { "token_type_hint", "access_token" }
            })
            .RespondAsJson(GetInactiveToken());

        return handler;
    }

    private static bool IsValidJwt(HttpRequestMessage message)
    {
        if (message.Content is not FormUrlEncodedContent) return false;
        using var reader = new FormReader(message.Content.ReadAsStream());
        var form = reader.ReadForm();
        var jwt = form["client_assertion"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(jwt)) return false;

        return jwt.EndsWith(Base64UrlEncoder.Encode(ZitadelAuthenticationTestHost.ValidSignature));
    }

     
    private static MockedRequest RespondAsJson<T>(this MockedRequest mockedRequest, T data) where T : class
    {
        return mockedRequest.Respond(MediaTypeNames.Application.Json, JsonSerializer.Serialize(data));
    }
}
