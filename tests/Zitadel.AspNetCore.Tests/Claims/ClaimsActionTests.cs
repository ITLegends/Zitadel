using System.Security.Claims;
using System.Text.Json;

using Zitadel.Abstractions;
using Zitadel.AspNetCore.Claims;

namespace Zitadel.AspNetCore.Tests.Claims;

public class ClaimsActionTests
{
    [Fact]
    public void ShouldTransformProjectRolesIntoRoleClaims()
    {
        // Arrange
        const string claimsWithZitadelProjectRoles = """
                        {
                            "iss": "zitadel.localhost",
                            "urn:zitadel:iam:org:project:roles": {
                              "admin" : {
                                "334771488802539102" : "acme.sh"
                              },
                              "viewer" : {
                                "334771488802539102": "acme.sh",
                                "727577719387174842": "zitadel.localhost"
                              }
                            }
                        }
                        """;
        var jsonElement = JsonElement.Parse(claimsWithZitadelProjectRoles);

        var identity = new ClaimsIdentity();
        var sut = new ZitadelRoleClaimsAction();

        // Act
        sut.Run(jsonElement, identity, "zitadel.localhost");

        // Assert
        Assert.True(identity.HasClaim(ClaimTypes.Role, "admin"));
        Assert.True(identity.HasClaim(ClaimTypes.Role, "viewer"));
        Assert.True(identity.HasClaim(ZitadelClaimTypes.OrganizationRole("334771488802539102"), "admin"));
        Assert.True(identity.HasClaim(ZitadelClaimTypes.OrganizationRole("334771488802539102"), "viewer"));
        Assert.True(identity.HasClaim(ZitadelClaimTypes.OrganizationRole("727577719387174842"), "viewer"));
        Assert.False(identity.HasClaim(ZitadelClaimTypes.OrganizationRole("727577719387174842"), "admin"));
    }
    
    [Fact]
    public void ShouldDoNothingWhenZitadelRolesAreNotDefined()
    {
        // Arrange
        const string claimsWithoutZitadelProjectRoles = """
                                 {
                                     "iss": "zitadel.localhost"
                                 }
                                 """;
        var jsonElement = JsonElement.Parse(claimsWithoutZitadelProjectRoles);

        var identity = new ClaimsIdentity();
        var sut = new ZitadelRoleClaimsAction();

        // Act
        sut.Run(jsonElement, identity, "zitadel.localhost");

        // Assert
        Assert.False(identity.HasClaim(c => c.Type == ClaimTypes.Role));
    }
}
