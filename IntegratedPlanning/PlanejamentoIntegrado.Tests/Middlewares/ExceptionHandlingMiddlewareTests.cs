using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using PlanejamentoIntegrado.Constants;
using PlanejamentoIntegrado.Middlewares;
using System.Text.Json;

namespace PlanejamentoIntegrado.Tests.Middlewares;

public class ExceptionHandlingMiddlewareTests
{
    private readonly Mock<ILogger<ExceptionHandlingMiddleware>> _mockLogger;
    private readonly Mock<RequestDelegate> _mockNext;
    private readonly ExceptionHandlingMiddleware _middleware;

    public ExceptionHandlingMiddlewareTests()
    {
        _mockLogger = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        _mockNext = new Mock<RequestDelegate>();
        _middleware = new ExceptionHandlingMiddleware(_mockNext.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Invoke_NoException_CallsNext()
    {
        var context = new DefaultHttpContext();
        _mockNext.Setup(x => x(context)).Returns(Task.CompletedTask);

        await _middleware.Invoke(context);

        _mockNext.Verify(x => x(context), Times.Once);
    }

    [Fact]
    public async Task Invoke_ExceptionThrown_AjaxRequest_ReturnsJsonError()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Requested-With"] = "XMLHttpRequest";
        context.Response.Body = new MemoryStream();

        var exception = new Exception("Teste de exceção");
        _mockNext.Setup(x => x(context)).ThrowsAsync(exception);

        await _middleware.Invoke(context);

        Assert.Equal(500, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var errorObj = JsonSerializer.Deserialize<JsonElement>(responseBody);

        Assert.Equal(MessageConstants.UnexpectedError, errorObj.GetProperty("error").GetString());

        _mockLogger.Verify(
            x =>
                x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Exceção não tratada")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task Invoke_ExceptionThrown_RegularRequest_RedirectsWithTempData()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/test";
        context.Request.QueryString = new QueryString("?param=value");

        var mockTempData = new Mock<ITempDataDictionary>();
        var mockTempDataFactory = new Mock<ITempDataDictionaryFactory>();

        mockTempDataFactory.Setup(x => x.GetTempData(context)).Returns(mockTempData.Object);

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider
            .Setup(x => x.GetService(typeof(ITempDataDictionaryFactory)))
            .Returns(mockTempDataFactory.Object);

        context.RequestServices = serviceProvider.Object;

        var exception = new Exception("Teste de exceção");
        _mockNext.Setup(x => x(context)).ThrowsAsync(exception);

        await _middleware.Invoke(context);

        mockTempData.VerifySet(
            x => x["GlobalError"] = MessageConstants.UnexpectedError,
            Times.Once
        );
        mockTempData.Verify(x => x.Save(), Times.Once);

        _mockLogger.Verify(
            x =>
                x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Exceção não tratada")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task Invoke_ExceptionThrown_NoTempDataFactory_DoesNotThrow()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/test";

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider
            .Setup(x => x.GetService(typeof(ITempDataDictionaryFactory)))
            .Returns((ITempDataDictionaryFactory?)null);

        context.RequestServices = serviceProvider.Object;

        var exception = new Exception("Teste de exceção");
        _mockNext.Setup(x => x(context)).ThrowsAsync(exception);

        await _middleware.Invoke(context);

        _mockLogger.Verify(
            x =>
                x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Exceção não tratada")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task Invoke_ExceptionThrown_NullTempData_DoesNotThrow()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/test";

        var mockTempDataFactory = new Mock<ITempDataDictionaryFactory>();
        mockTempDataFactory.Setup(x => x.GetTempData(context)).Returns((ITempDataDictionary?)null);

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider
            .Setup(x => x.GetService(typeof(ITempDataDictionaryFactory)))
            .Returns(mockTempDataFactory.Object);

        context.RequestServices = serviceProvider.Object;

        var exception = new Exception("Teste de exceção");
        _mockNext.Setup(x => x(context)).ThrowsAsync(exception);

        await _middleware.Invoke(context);

        _mockLogger.Verify(
            x =>
                x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Exceção não tratada")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
            Times.Once
        );
    }
}

