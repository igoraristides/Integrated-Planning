using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using PlanejamentoIntegrado.Helpers;
using PlanejamentoIntegrado.Tests.Base;

namespace PlanejamentoIntegrado.Tests.Helpers;

public class AuthenticationHelperTests : BaseTest
{
    [Fact]
    public void CreatePrincipal_ValidUser_ReturnsClaimsPrincipalWithCorrectClaims()
    {
        var user = CreateTestUser();

        var principal = AuthenticationHelper.CreatePrincipal(user);

        Assert.NotNull(principal);
        Assert.Equal(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal.Identity.AuthenticationType
        );
        Assert.True(principal.Identity.IsAuthenticated);

        var nameIdentifierClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
        Assert.NotNull(nameIdentifierClaim);
        Assert.Equal(user.Id.ToString(), nameIdentifierClaim.Value);

        var nameClaim = principal.FindFirst(ClaimTypes.Name);
        Assert.NotNull(nameClaim);
        Assert.Equal(user.Name, nameClaim.Value);

        var roleClaim = principal.FindFirst(ClaimTypes.Role);
        Assert.NotNull(roleClaim);
        Assert.Equal(((int)user.ProfileId).ToString(), roleClaim.Value);
    }

    [Fact]
    public void CreatePrincipal_UserWithViewerProfile_CreatesCorrectRoleClaim()
    {
        var user = CreateTestUser();
        user.ProfileId = Enums.SharedEnums.UserProfile.Viewer;

        var principal = AuthenticationHelper.CreatePrincipal(user);

        var roleClaim = principal.FindFirst(ClaimTypes.Role);
        Assert.NotNull(roleClaim);
        Assert.Equal("1", roleClaim.Value);
    }

    [Fact]
    public void CreatePrincipal_UserWithAdminProfile_CreatesCorrectRoleClaim()
    {
        var user = CreateTestUser();
        user.ProfileId = Enums.SharedEnums.UserProfile.Admin;

        var principal = AuthenticationHelper.CreatePrincipal(user);

        var roleClaim = principal.FindFirst(ClaimTypes.Role);
        Assert.NotNull(roleClaim);
        Assert.Equal("0", roleClaim.Value); 
    }

    [Fact]
    public void CreatePrincipal_ValidUser_HasExactlyThreeClaims()
    {
        var user = CreateTestUser();

        var principal = AuthenticationHelper.CreatePrincipal(user);

        var claims = principal.Claims.ToList();
        Assert.Equal(3, claims.Count);

        Assert.Contains(claims, c => c.Type == ClaimTypes.NameIdentifier);
        Assert.Contains(claims, c => c.Type == ClaimTypes.Name);
        Assert.Contains(claims, c => c.Type == ClaimTypes.Role);
    }

    [Fact]
    public void CreatePrincipal_UserWithDifferentId_CreatesCorrectNameIdentifierClaim()
    {
        var user = CreateTestUser();
        user.Id = 999;

        var principal = AuthenticationHelper.CreatePrincipal(user);

        var nameIdentifierClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
        Assert.NotNull(nameIdentifierClaim);
        Assert.Equal("999", nameIdentifierClaim.Value);
    }

    [Fact]
    public void CreatePrincipal_UserWithDifferentName_CreatesCorrectNameClaim()
    {
        var user = CreateTestUser();
        user.Name = "Maria Teste";

        var principal = AuthenticationHelper.CreatePrincipal(user);

        var nameClaim = principal.FindFirst(ClaimTypes.Name);
        Assert.NotNull(nameClaim);
        Assert.Equal("Maria Teste", nameClaim.Value);
    }
}
