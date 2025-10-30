using Microsoft.AspNetCore.Authentication.Cookies;
using PlanejamentoIntegrado.Models;
using System.Security.Claims;

namespace PlanejamentoIntegrado.Helpers;

public static class AuthenticationHelper
{
    public static ClaimsPrincipal CreatePrincipal(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Role, ((int)user.ProfileId).ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(identity);
    }
}
