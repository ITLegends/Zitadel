using System.Net.Mime;
using System.Text;
using System.Text.Json;

using Duende.IdentityModel;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;

using RichardSzalay.MockHttp;

namespace Zitadel.AccessTokenManagement.Tests.TestSetup;

public static class TokenEndpointMock
{
    public const string ValidAccessToken = "VALID";

    public static MockHttpMessageHandler ExpectClientCredentialsRequest(this MockHttpMessageHandler handler, int count = 1)
    {
        var expectedAuthHeader = $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ZitadelServiceAccountTestSetup.ClientId}:{ZitadelServiceAccountTestSetup.ClientSecret}"))}";

        for (int i = 0; i < count; i++)
        {
            handler
                .Expect(ZitadelServiceAccountTestSetup.TokenEndpoint)
                .WithHeaders("Authorization", expectedAuthHeader)
                .WithFormData(new Dictionary<string, string>
                {
                    { "grant_type", OidcConstants.GrantTypes.ClientCredentials },
                    { "scope", string.Join(' ', ZitadelServiceAccountTestSetup.Scopes) },
                })
                .RespondWithToken();
        }

        return handler;
    }

    public static MockHttpMessageHandler ExpectJwtProfileRequest(this MockHttpMessageHandler handler, int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            handler
                .Expect(ZitadelServiceAccountTestSetup.TokenEndpoint)
                .WithFormData(new Dictionary<string, string>
                {
                    { "grant_type", OidcConstants.GrantTypes.JwtBearer },
                    { "scope", string.Join(' ', ZitadelServiceAccountTestSetup.Scopes) },
                })
                .With(HasValidJwtAssertion)
                .RespondWithToken();
        }

        return handler;
    }
    
    private static bool HasValidJwtAssertion(HttpRequestMessage message)
    {
        if (message.Content is not FormUrlEncodedContent) return false;
        using var reader = new FormReader(message.Content.ReadAsStream());
        var form = reader.ReadForm();
        var jwt = form["assertion"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(jwt)) return false;

        return jwt.EndsWith(Base64UrlEncoder.Encode(ZitadelServiceAccountTestSetup.ValidSignature));
    }

    private static void RespondWithToken(this MockedRequest request) =>
        request.Respond(
            MediaTypeNames.Application.Json,
            JsonSerializer.Serialize(new
            {
                access_token = ValidAccessToken,
                token_type = "Bearer",
                expires_in = 3600,
            }));
}
