using System.Security.Claims;

using Zitadel.Abstractions;
using Zitadel.AspNetCore.Claims;

namespace Zitadel.AspNetCore.Tests.Claims;

public class ClaimsTransformerTests
{
    [Fact]
    public async Task ShouldTransformProjectRolesIntoRoleClaims()
    {
        // Arrange
        const string rolesJson = """
                                 {
                                   "admin" : {
                                     "334771488802539102" : "acme.sh"
                                   },
                                   "viewer" : {
                                     "334771488802539102": "acme.sh",
                                     "727577719387174842": "zitadel.cloud"
                                   }
                                 }
                                 """;

        var identity = new ClaimsIdentity(
        [
            new Claim(ZitadelClaimTypes.ProjectRoles, rolesJson),
        ]);

        var principal = new ClaimsPrincipal(identity);
        var sut = new ZitadelRoleClaimsTransformation();

        // Act
        var result = await sut.TransformAsync(principal);

        // Assert
        Assert.True(result.IsInRole("admin"));
        Assert.True(result.IsInRole("viewer"));
        Assert.True(result.HasClaim(ZitadelClaimTypes.OrganizationRole("334771488802539102"), "admin"));
        Assert.True(result.HasClaim(ZitadelClaimTypes.OrganizationRole("334771488802539102"), "viewer"));
        Assert.True(result.HasClaim(ZitadelClaimTypes.OrganizationRole("727577719387174842"), "viewer"));
        Assert.False(result.HasClaim(ZitadelClaimTypes.OrganizationRole("727577719387174842"), "admin"));
    }
    
    [Fact]
    public async Task ShouldNotDuplicateRolesOnSubsequentInvocations()
    {
        // Arrange
        var rolesJson = """
                        {
                          "admin" : {
                            "334771488802539102" : "acme.sh"
                          },
                          "viewer" : {
                            "334771488802539102": "acme.sh",
                            "727577719387174842": "zitadel.cloud"
                          }
                        }
                        """;

        var identity = new ClaimsIdentity(
        [
            new Claim(ZitadelClaimTypes.ProjectRoles, rolesJson),
        ]);

        var principal = new ClaimsPrincipal(identity);
        var sut = new ZitadelRoleClaimsTransformation();

        // Act
        var result = await sut.TransformAsync(principal);
        result = await sut.TransformAsync(result);

        // Assert
        Assert.Single(result.FindAll(ClaimTypes.Role), c => c.Value == "admin");
        Assert.Single(result.FindAll(ClaimTypes.Role), c => c.Value == "viewer");
        Assert.Single(result.FindAll(ZitadelClaimTypes.OrganizationRole("334771488802539102")), c => c.Value == "admin");
        Assert.Single(result.FindAll(ZitadelClaimTypes.OrganizationRole("334771488802539102")), c => c.Value == "viewer");
    }
    
    [Fact]
    public async Task ShouldDoNothingWhenZitadelRolesAreNotDefined()
    {
        // Arrange
        var identity = new ClaimsIdentity();
        var principal = new ClaimsPrincipal(identity);
        var sut = new ZitadelRoleClaimsTransformation();

        // Act
        var result = await sut.TransformAsync(principal);

        // Assert
        Assert.False(result.HasClaim(c => c.Type == ClaimTypes.Role));
    }
}
