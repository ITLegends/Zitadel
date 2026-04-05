using System.Net;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Zitadel.AccessTokenManagement.Extensions;
using Zitadel.AccessTokenManagement.Tests.TestSetup;

namespace Zitadel.AccessTokenManagement.Tests;

public class ZitadelServiceAccountTests
{
    [Fact]
    public async Task TokenShouldNotBeAttachedToOutgoingRequestForUnenrolledClient()
    {
        // Arrange
        var services = ZitadelServiceAccountTestSetup.CreateDefaultServiceCollection();
        services.ConfigureZitadelServiceAccount(ZitadelServiceAccountTestSetup.PatConfiguration);
        services.AddDownstreamHttpClient();
            
        var provider = services.BuildServiceProvider();
        var mock = provider.GetHttpMock()
            .ExpectUnauthenticatedDownstreamRequest();
        
        var client = provider.GetHttpClient();

        // Act
        var response = await client.GetAsync("/resource");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        mock.VerifyNoOutstandingExpectation();
    }
    
    [Fact]
    public async Task PatShouldBeAttachedToOutgoingRequest()
    {
        // Arrange
        var services = ZitadelServiceAccountTestSetup.CreateDefaultServiceCollection();
        services.ConfigureZitadelServiceAccount(ZitadelServiceAccountTestSetup.PatConfiguration);
        services.AddDownstreamHttpClient().WithZitadelServiceAccount();
        
        var provider = services.BuildServiceProvider();
        var mock = provider.GetHttpMock()
            .ExpectDownstreamRequestWithToken(ZitadelServiceAccountTestSetup.Pat);
        
        var client = provider.GetHttpClient();

        // Act
        var response = await client.GetAsync("/resource");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        mock.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task ClientCredentialsShouldRequestTokenAndAttachToOutgoingRequest()
    {
        // Arrange
        var services = ZitadelServiceAccountTestSetup.CreateDefaultServiceCollection();
        services.ConfigureZitadelServiceAccount(ZitadelServiceAccountTestSetup.ClientCredentialsConfiguration);
        services.AddDownstreamHttpClient().WithZitadelServiceAccount();
            
        var provider = services.BuildServiceProvider();
        var mock = provider.GetHttpMock()
            .ExpectClientCredentialsRequest()
            .ExpectDownstreamRequestWithToken(TokenEndpointMock.ValidAccessToken);
        
        var client = provider.GetHttpClient();

        // Act
        var response = await client.GetAsync("/resource");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        mock.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task JwtProfileShouldRequestTokenAndAttachToOutgoingRequest()
    {
        // Arrange
        var services = ZitadelServiceAccountTestSetup.CreateDefaultServiceCollection();
        services.ConfigureZitadelServiceAccount(ZitadelServiceAccountTestSetup.JwtProfileConfiguration);
        services.AddDownstreamHttpClient().WithZitadelServiceAccount();
            
        var provider = services.BuildServiceProvider();
        var mock = provider.GetHttpMock()
            .ExpectJwtProfileRequest()
            .ExpectDownstreamRequestWithToken(TokenEndpointMock.ValidAccessToken);
        
        var client = provider.GetHttpClient();

        // Act
        var response = await client.GetAsync("/resource");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        mock.VerifyNoOutstandingExpectation();
    }
    
    [Fact]
    public async Task TokenShouldBeCachedForSubsequentRequests()
    {
        // Arrange
        var services = ZitadelServiceAccountTestSetup.CreateDefaultServiceCollection();
        services.ConfigureZitadelServiceAccount(ZitadelServiceAccountTestSetup.ClientCredentialsConfiguration);
        services.AddDownstreamHttpClient().WithZitadelServiceAccount();
            
        var provider = services.BuildServiceProvider();
        var mock = provider.GetHttpMock()
            .ExpectClientCredentialsRequest(count: 1)
            .ExpectDownstreamRequestWithToken(TokenEndpointMock.ValidAccessToken, count: 3);
        
        var client = provider.GetHttpClient();

        // Act
        var response1 = await client.GetAsync("/a");
        var response2 = await client.GetAsync("/b");
        var response3 = await client.GetAsync("/c");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
        Assert.Equal(HttpStatusCode.OK, response3.StatusCode);
        mock.VerifyNoOutstandingExpectation();
    }

    [Fact]
    public async Task MultipleServiceAccountsShouldBeAbleToCoExist()
    {
        // Arrange
        var services = ZitadelServiceAccountTestSetup.CreateDefaultServiceCollection();
        services.ConfigureZitadelServiceAccount("JWT", ZitadelServiceAccountTestSetup.JwtProfileConfiguration);
        services.ConfigureZitadelServiceAccount("BASIC", ZitadelServiceAccountTestSetup.ClientCredentialsConfiguration);
        services.ConfigureZitadelServiceAccount("PAT", ZitadelServiceAccountTestSetup.PatConfiguration);
        
        services.AddDownstreamHttpClient("A").WithZitadelServiceAccount("JWT");
        services.AddDownstreamHttpClient("B").WithZitadelServiceAccount("BASIC");
        services.AddDownstreamHttpClient("C").WithZitadelServiceAccount("PAT");
            
        var provider = services.BuildServiceProvider();
        var mock = provider.GetHttpMock()
            // Expectations are ordered. 
            .ExpectJwtProfileRequest(count: 1)
            .ExpectDownstreamRequestWithToken(TokenEndpointMock.ValidAccessToken, count: 1)
            .ExpectClientCredentialsRequest(count: 1)
            .ExpectDownstreamRequestWithToken(TokenEndpointMock.ValidAccessToken, count: 1)
            .ExpectDownstreamRequestWithToken(ZitadelServiceAccountTestSetup.Pat, count: 1);
        
        var jwtClient = provider.GetHttpClient("A");
        var basicClient = provider.GetHttpClient("B");
        var patClient = provider.GetHttpClient("C");

        // Act
        var jwtResponse = await jwtClient.GetAsync("/resource");
        var basicResponse = await basicClient.GetAsync("/resource");
        var patResponse = await patClient.GetAsync("/resource");

        // Assert
        Assert.Equal(HttpStatusCode.OK, jwtResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, basicResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, patResponse.StatusCode);
        mock.VerifyNoOutstandingExpectation();
    }
    
    [Fact]
    public async Task InvalidServiceAccountSetupThrowsValidationException()
    {
        // Arrange
        var services = ZitadelServiceAccountTestSetup.CreateDefaultServiceCollection();
        services.ConfigureZitadelServiceAccount("INVALID", o => o.TokenEndpoint = ZitadelServiceAccountTestSetup.TokenEndpoint);
        services.AddDownstreamHttpClient().WithZitadelServiceAccount("INVALID");
            
        var provider = services.BuildServiceProvider();
        
        var client = provider.GetHttpClient();

        // Act
        var action = () => client.GetAsync("/resource");
        
        // Assert
        await Assert.ThrowsAsync<OptionsValidationException>(action);
    }
}
