using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using PlanejamentoIntegrado.Constants;
using PlanejamentoIntegrado.Controllers;
using PlanejamentoIntegrado.Models;
using PlanejamentoIntegrado.Services;
using PlanejamentoIntegrado.Tests.Base;

namespace PlanejamentoIntegrado.Tests.Controllers;

public class UserControllerTests : BaseTest
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _mockUserService = new Mock<IUserService>();

        var httpContext = new DefaultHttpContext();

        _controller = new UserController(_mockUserService.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext },
            TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>()),
        };
    }

    [Fact]
    public async Task Index_ReturnsViewWithUsers()
    {
        var users = new List<UserViewModel>
        {
            new UserViewModel
            {
                Id = 1,
                Name = "João",
                Login = "joao",
            },
            new UserViewModel
            {
                Id = 2,
                Name = "Maria",
                Login = "maria",
            },
        };

        _mockUserService.Setup(x => x.GetAll()).ReturnsAsync(users);

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<UserViewModel>>(viewResult.Model);
        Assert.Equal(2, model.Count);
    }

    [Fact]
    public void New_Get_ReturnsView()
    {
        var result = _controller.New();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task New_Post_ValidModel_RedirectsToIndex()
    {
        var model = new UserViewModel
        {
            Name = "João",
            Surname = "Silva",
            Email = "joao@teste.com",
            Login = "joao",
            Password = "senha123",
        };

        _mockUserService.Setup(x => x.Register(model)).ReturnsAsync(true);

        var result = await _controller.New(model);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal(MessageConstants.UserCreatedSuccess, _controller.TempData["Success"]);
    }

    [Fact]
    public async Task New_Post_InvalidModel_ReturnsViewWithModel()
    {
        var model = new UserViewModel();
        _controller.ModelState.AddModelError("Name", "Required");

        var result = await _controller.New(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
    }

    [Fact]
    public async Task New_Post_UserAlreadyExists_ReturnsViewWithError()
    {
        var model = new UserViewModel
        {
            Name = "João",
            Login = "joao.existente",
            Password = "senha123",
        };

        _mockUserService.Setup(x => x.Register(model)).ReturnsAsync(false);

        var result = await _controller.New(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
        Assert.Equal(MessageConstants.UserLoginAlreadyExists, _controller.TempData["Error"]);
    }

    [Fact]
    public async Task Edit_Get_ExistingUser_ReturnsViewWithUser()
    {
        var user = new UserViewModel
        {
            Id = 1,
            Name = "João",
            Login = "joao",
        };
        _mockUserService.Setup(x => x.GetById(1)).ReturnsAsync(user);

        var result = await _controller.Edit(1);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(user, viewResult.Model);
    }

    [Fact]
    public async Task Edit_Get_NonExistingUser_RedirectsToIndexWithError()
    {
        _mockUserService.Setup(x => x.GetById(999)).ReturnsAsync((UserViewModel?)null);

        var result = await _controller.Edit(999);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal(MessageConstants.UserNotFound, _controller.TempData["Error"]);
    }

    [Fact]
    public async Task Edit_Post_ValidModel_RedirectsToIndexWithSuccess()
    {
        var model = new UserViewModel
        {
            Id = 1,
            Name = "João Editado",
            Login = "joao.editado",
            Password = "senha123",
        };

        _mockUserService.Setup(x => x.Edit(1, model)).ReturnsAsync((true, null));

        var result = await _controller.Edit(model);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal(MessageConstants.UserUpdatedSuccess, _controller.TempData["Success"]);
    }

    [Fact]
    public async Task Edit_Post_InvalidModel_ReturnsViewWithModel()
    {
        var model = new UserViewModel { Id = 1 };
        _controller.ModelState.AddModelError("Name", "Required");

        var result = await _controller.Edit(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
    }

    [Fact]
    public async Task Edit_Post_ServiceError_RedirectsToIndexWithError()
    {
        var model = new UserViewModel
        {
            Id = 1,
            Name = "João",
            Password = "senha123",
        };
        var errorMessage = "Erro de validação";

        _mockUserService.Setup(x => x.Edit(1, model)).ReturnsAsync((false, errorMessage));

        var result = await _controller.Edit(model);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal(errorMessage, _controller.TempData["Error"]);
    }

    [Fact]
    public async Task Delete_ExistingUser_RedirectsToIndexWithSuccess()
    {
        _mockUserService.Setup(x => x.Delete(1)).ReturnsAsync(true);

        var result = await _controller.Delete(1);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal(MessageConstants.UserDeletedSuccess, _controller.TempData["Success"]);
    }

    [Fact]
    public async Task Delete_NonExistingUser_RedirectsToIndexWithError()
    {
        _mockUserService.Setup(x => x.Delete(999)).ReturnsAsync(false);

        var result = await _controller.Delete(999);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal(MessageConstants.UserNotFound, _controller.TempData["Error"]);
    }

    [Fact]
    public void ChangePassword_Get_ReturnsView()
    {
        var result = _controller.ChangePassword();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task ChangePassword_Post_ValidModel_RedirectsToDashboardWithSuccess()
    {
        var model = new ChangePasswordViewModel
        {
            CurrentPassword = "senhaAtual123",
            NewPassword = "novaSenha123",
            ConfirmPassword = "novaSenha123",
        };

        var mockUser = new ClaimsPrincipal(
            new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "1") })
        );

        _controller.ControllerContext.HttpContext.User = mockUser;

        _mockUserService
            .Setup(x => x.ChangePassword(1, model.CurrentPassword, model.NewPassword))
            .ReturnsAsync((true, null));

        var result = await _controller.ChangePassword(model);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("CompareVariation", redirectResult.ControllerName);
        Assert.Equal(MessageConstants.PasswordChangedSuccess, _controller.TempData["Success"]);
    }

    [Fact]
    public async Task ChangePassword_Post_InvalidModel_ReturnsViewWithModel()
    {
        var model = new ChangePasswordViewModel();
        _controller.ModelState.AddModelError("CurrentPassword", "Required");

        var result = await _controller.ChangePassword(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
    }

    [Fact]
    public async Task ChangePassword_Post_InvalidUserId_RedirectsToLoginWithError()
    {
        var model = new ChangePasswordViewModel
        {
            CurrentPassword = "senhaAtual123",
            NewPassword = "novaSenha123",
            ConfirmPassword = "novaSenha123",
        };

        var mockUser = new ClaimsPrincipal(
            new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "invalid_id") })
        );

        _controller.ControllerContext.HttpContext.User = mockUser;

        var result = await _controller.ChangePassword(model);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirectResult.ActionName);
        Assert.Equal("Auth", redirectResult.ControllerName);
        Assert.Equal(MessageConstants.UserNotFound, _controller.TempData["Error"]);
    }

    [Fact]
    public async Task ChangePassword_Post_ServiceError_ReturnsViewWithError()
    {
        var model = new ChangePasswordViewModel
        {
            CurrentPassword = "senhaAtual123",
            NewPassword = "novaSenha123",
            ConfirmPassword = "novaSenha123",
        };

        var mockUser = new ClaimsPrincipal(
            new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "1") })
        );

        _controller.ControllerContext.HttpContext.User = mockUser;

        _mockUserService
            .Setup(x => x.ChangePassword(1, model.CurrentPassword, model.NewPassword))
            .ReturnsAsync((false, MessageConstants.CurrentPasswordError));

        var result = await _controller.ChangePassword(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
        Assert.Equal(MessageConstants.CurrentPasswordError, _controller.TempData["Error"]);
    }

    [Fact]
    public async Task ChangePassword_Post_ServiceErrorWithNullMessage_ReturnsViewWithUnexpectedError()
    {
        var model = new ChangePasswordViewModel
        {
            CurrentPassword = "senhaAtual123",
            NewPassword = "novaSenha123",
            ConfirmPassword = "novaSenha123",
        };

        var mockUser = new ClaimsPrincipal(
            new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "1") })
        );

        _controller.ControllerContext.HttpContext.User = mockUser;

        _mockUserService
            .Setup(x => x.ChangePassword(1, model.CurrentPassword, model.NewPassword))
            .ReturnsAsync((false, null));

        var result = await _controller.ChangePassword(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
        Assert.Equal(MessageConstants.UnexpectedError, _controller.TempData["Error"]);
    }
}
