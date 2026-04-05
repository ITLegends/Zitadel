using System.Net;

using RichardSzalay.MockHttp;

namespace Zitadel.AccessTokenManagement.Tests.TestSetup;

public static class DownstreamEndpointMock
{
    public static MockHttpMessageHandler ExpectDownstreamRequestWithToken(this MockHttpMessageHandler handler, string expectedAuthorizationHeader, int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            handler
                .Expect($"{ZitadelServiceAccountTestSetup.DownstreamApiBase}/*")
                .WithHeaders("Authorization", $"Bearer {expectedAuthorizationHeader}")
                .Respond(HttpStatusCode.OK);
        }

        return handler;
    }

    public static MockHttpMessageHandler ExpectUnauthenticatedDownstreamRequest(this MockHttpMessageHandler handler)
    {
        handler
            .Expect($"{ZitadelServiceAccountTestSetup.DownstreamApiBase}/*")
            .Respond(HttpStatusCode.Unauthorized);

        return handler;
    }
}
