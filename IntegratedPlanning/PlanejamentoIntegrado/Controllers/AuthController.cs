using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PlanejamentoIntegrado.Helpers;
using PlanejamentoIntegrado.Models;
using PlanejamentoIntegrado.Services;

namespace PlanejamentoIntegrado.Controllers;

public class AuthController(IAuthService authService) : Controller
{
    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var user = await authService.Authenticate(model.Login, model.Password);

        if (user == null)
        {
            TempData["LoginError"] = "Credenciais inválidas";
            return View(model);
        }

        var principal = AuthenticationHelper.CreatePrincipal(user);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return RedirectToAction("Index", "CompareVariation");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}
