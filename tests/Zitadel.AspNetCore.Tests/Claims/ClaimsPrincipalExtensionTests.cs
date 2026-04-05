using System.Security.Claims;

using NSubstitute;

using Zitadel.AspNetCore.Extensions;

namespace Zitadel.AspNetCore.Tests.Claims;

public class ClaimsPrincipalExtensionsTest
{
    private readonly ClaimsPrincipal _claimsPrincipal = Substitute.For<ClaimsPrincipal>();

    public ClaimsPrincipalExtensionsTest()
    {
        _claimsPrincipal.IsInRole("positive").Returns(true);
        _claimsPrincipal.IsInRole("negative").Returns(false);
    }

    [Fact]
    public void IsInSingleRole()
    {
        bool actual = ClaimsPrincipalExtensions.IsInRole(_claimsPrincipal, ["positive"]);

        Assert.True(actual);
        _claimsPrincipal.Received(1).IsInRole("positive");
    }

    [Fact]
    public void IsInOneOfTheGivenRoles()
    {
        bool actual = ClaimsPrincipalExtensions.IsInRole(_claimsPrincipal, ["negative", "positive"]);

        Assert.True(actual);
        _claimsPrincipal.Received(1).IsInRole("positive");
        _claimsPrincipal.Received(1).IsInRole("negative");
    }

    [Fact]
    public void IsNotInRole()
    {
        bool actual = ClaimsPrincipalExtensions.IsInRole(_claimsPrincipal, ["negative"]);

        Assert.False(actual);
        _claimsPrincipal.Received(1).IsInRole("negative");
    }

    [Fact]
    public void IsNotInNoneOfTheGivenRoles()
    {
        bool actual =
            ClaimsPrincipalExtensions.IsInRole(_claimsPrincipal, ["negative", "negative", "negative"]);

        Assert.False(actual);
        _claimsPrincipal.Received(3).IsInRole("negative");
    }

    [Fact]
    public void IsFalseForNoGivenRoles()
    {
        bool actual = ClaimsPrincipalExtensions.IsInRole(_claimsPrincipal, []);

        Assert.False(actual);
        _claimsPrincipal.DidNotReceiveWithAnyArgs().IsInRole();
    }
}
