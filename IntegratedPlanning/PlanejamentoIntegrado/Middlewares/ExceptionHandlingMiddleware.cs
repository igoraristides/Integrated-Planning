using Microsoft.AspNetCore.Mvc.ViewFeatures;
using PlanejamentoIntegrado.Constants;
using System.Net;
using System.Text.Json;

namespace PlanejamentoIntegrado.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exceção não tratada");

            if (context.Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var errorObj = new { error = MessageConstants.UnexpectedError };
                var json = JsonSerializer.Serialize(errorObj);

                await context.Response.WriteAsync(json);
                return;
            }

            var tempDataFactory = context.RequestServices.GetService<ITempDataDictionaryFactory>();
            var tempData = tempDataFactory?.GetTempData(context);

            if (tempData is not null)
            {
                tempData["GlobalError"] = MessageConstants.UnexpectedError;

                tempData.Save();
            }

            context.Response.Redirect(context.Request.Path + context.Request.QueryString);
        }
    }
}
