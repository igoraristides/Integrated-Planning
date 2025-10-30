using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using PlanejamentoIntegrado.Controllers;
using PlanejamentoIntegrado.Models;
using PlanejamentoIntegrado.Services;
using PlanejamentoIntegrado.Tests.Base;

namespace PlanejamentoIntegrado.Tests.Controllers;

public class AuthControllerTests : BaseTest
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly AuthController _controller;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<IAuthenticationService> _mockAuthenticationService;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _mockHttpContext = new Mock<HttpContext>();
        _mockAuthenticationService = new Mock<IAuthenticationService>();

        var mockUrlHelperFactory = new Mock<IUrlHelperFactory>();
        var mockUrlHelper = new Mock<IUrlHelper>();

        mockUrlHelperFactory
            .Setup(x => x.GetUrlHelper(It.IsAny<ActionContext>()))
            .Returns(mockUrlHelper.Object);

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider
            .Setup(x => x.GetService(typeof(IAuthenticationService)))
            .Returns(_mockAuthenticationService.Object);
        serviceProvider
            .Setup(x => x.GetService(typeof(IUrlHelperFactory)))
            .Returns(mockUrlHelperFactory.Object);

        _mockHttpContext.Setup(x => x.RequestServices).Returns(serviceProvider.Object);

        _controller = new AuthController(_mockAuthService.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object },
            TempData = new TempDataDictionary(
                _mockHttpContext.Object,
                Mock.Of<ITempDataProvider>()
            ),
            Url = mockUrlHelper.Object,
        };
    }

    [Fact]
    public void Login_Get_ReturnsView()
    {
        var result = _controller.Login();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Login_Post_ValidCredentials_RedirectsToDashboard()
    {
        var model = new LoginViewModel { Login = "teste", Password = "senha123" };
        var user = CreateTestUser();

        _mockAuthService.Setup(x => x.Authenticate(model.Login, model.Password)).ReturnsAsync(user);

        _mockAuthenticationService
            .Setup(x =>
                x.SignInAsync(
                    It.IsAny<HttpContext>(),
                    It.IsAny<string>(),
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties>()
                )
            )
            .Returns(Task.CompletedTask);

        var result = await _controller.Login(model);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("CompareVariation", redirectResult.ControllerName);
    }

    [Fact]
    public async Task Login_Post_InvalidCredentials_ReturnsViewWithError()
    {
        var model = new LoginViewModel { Login = "teste", Password = "senhaerrada" };

        _mockAuthService
            .Setup(x => x.Authenticate(model.Login, model.Password))
            .ReturnsAsync((User?)null);

        var result = await _controller.Login(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
        Assert.Equal("Credenciais inválidas", _controller.TempData["LoginError"]);
    }

    [Fact]
    public async Task Logout_RedirectsToLogin()
    {
        _mockAuthenticationService
            .Setup(x =>
                x.SignOutAsync(
                    It.IsAny<HttpContext>(),
                    It.IsAny<string>(),
                    It.IsAny<AuthenticationProperties>()
                )
            )
            .Returns(Task.CompletedTask);

        var result = await _controller.Logout();

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirectResult.ActionName);
    }

    [Fact]
    public void AccessDenied_ReturnsView()
    {
        var result = _controller.AccessDenied();

        Assert.IsType<ViewResult>(result);
    }
}
