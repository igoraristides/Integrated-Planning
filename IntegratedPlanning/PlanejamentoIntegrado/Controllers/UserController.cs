using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanejamentoIntegrado.Constants;
using PlanejamentoIntegrado.Models;
using PlanejamentoIntegrado.Services;

namespace PlanejamentoIntegrado.Controllers;

[Authorize]
public class UserController(IUserService userService) : Controller
{
    [Authorize(Roles = RoleNames.Admin)]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users = await userService.GetAll();
        return View(users);
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpGet]
    public IActionResult New()
    {
        return View();
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost]
    public async Task<IActionResult> New(UserViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await userService.Register(model);
        if (!result)
        {
            TempData["Error"] = MessageConstants.UserLoginAlreadyExists;
            return View(model);
        }

        TempData["Success"] = MessageConstants.UserCreatedSuccess;
        return RedirectToAction("Index");
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await userService.GetById(id);
        if (user == null)
        {
            TempData["Error"] = MessageConstants.UserNotFound;
            return RedirectToAction("Index");
        }

        return View(user);
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost]
    public async Task<IActionResult> Edit(UserViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await userService.Edit(model.Id, model);
        if (!response.Item1)
        {
            TempData["Error"] = response.Item2;
            return RedirectToAction("Index");
        }

        TempData["Success"] = MessageConstants.UserUpdatedSuccess;
        return RedirectToAction("Index");
    }

    [Authorize(Roles = RoleNames.Admin)]
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await userService.Delete(id);
        if (!success)
        {
            TempData["Error"] = MessageConstants.UserNotFound;
            return RedirectToAction("Index");
        }

        TempData["Success"] = MessageConstants.UserDeletedSuccess;
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            TempData["Error"] = MessageConstants.UserNotFound;
            return RedirectToAction("Login", "Auth");
        }

        var (success, error) = await userService.ChangePassword(
            userId,
            model.CurrentPassword,
            model.NewPassword
        );

        if (!success)
        {
            TempData["Error"] = error ?? MessageConstants.UnexpectedError;
            return View(model);
        }

        TempData["Success"] = MessageConstants.PasswordChangedSuccess;
        return RedirectToAction("Index", "CompareVariation");
    }
}
