using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Moq;
using PlanejamentoIntegrado.Helpers;

namespace PlanejamentoIntegrado.Tests.Helpers;

public class TagHelperTests
{
    private MenuActiveTagHelper CreateTagHelper(
        string currentController,
        string? currentAction = null
    )
    {
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
        );

        actionContext.RouteData.Values["controller"] = currentController;
        if (currentAction != null)
        {
            actionContext.RouteData.Values["action"] = currentAction;
        }

        var metadataProvider = new EmptyModelMetadataProvider();
        var viewData = new ViewDataDictionary(metadataProvider, new ModelStateDictionary());

        var viewContext = new ViewContext(
            actionContext,
            Mock.Of<IView>(),
            viewData,
            Mock.Of<ITempDataDictionary>(),
            TextWriter.Null,
            new HtmlHelperOptions()
        );

        return new MenuActiveTagHelper { ViewContext = viewContext };
    }

    [Fact]
    public void Process_WhenControllerMatches_AddsActiveClass()
    {
        var tagHelper = CreateTagHelper("Home");
        tagHelper.AspController = "Home";

        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString()
        );

        var output = new TagHelperOutput(
            "a",
            new TagHelperAttributeList { new TagHelperAttribute("class", "menu-item") },
            (result, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent())
        );

        tagHelper.Process(context, output);

        var classAttribute = output.Attributes["class"]?.Value?.ToString();
        Assert.Contains("active", classAttribute);
    }

    [Fact]
    public void Process_WhenControllerDoesNotMatch_DoesNotAddActiveClass()
    {
        var tagHelper = CreateTagHelper("Home");
        tagHelper.AspController = "About";

        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString()
        );

        var output = new TagHelperOutput(
            "a",
            new TagHelperAttributeList { new TagHelperAttribute("class", "menu-item") },
            (result, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent())
        );

        tagHelper.Process(context, output);

        var classAttribute = output.Attributes["class"]?.Value?.ToString();
        Assert.DoesNotContain("active", classAttribute);
    }

    [Fact]
    public void Process_WhenControllerAndActionMatch_AddsActiveClass()
    {
        var tagHelper = CreateTagHelper("Home", "Index");
        tagHelper.AspController = "Home";
        tagHelper.AspAction = "Index";

        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString()
        );

        var output = new TagHelperOutput(
            "a",
            new TagHelperAttributeList { new TagHelperAttribute("class", "menu-item") },
            (result, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent())
        );

        tagHelper.Process(context, output);

        var classAttribute = output.Attributes["class"]?.Value?.ToString();
        Assert.Contains("active", classAttribute);
    }

    [Fact]
    public void Process_WhenControllerMatchesButActionDoesNot_DoesNotAddActiveClass()
    {
        var tagHelper = CreateTagHelper("Home", "Index");
        tagHelper.AspController = "Home";
        tagHelper.AspAction = "About";

        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString()
        );

        var output = new TagHelperOutput(
            "a",
            new TagHelperAttributeList { new TagHelperAttribute("class", "menu-item") },
            (result, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent())
        );

        tagHelper.Process(context, output);

        var classAttribute = output.Attributes["class"]?.Value?.ToString();
        Assert.DoesNotContain("active", classAttribute);
    }

    [Fact]
    public void Process_WhenNoExistingClass_CreatesClassWithActive()
    {
        var tagHelper = CreateTagHelper("Home");
        tagHelper.AspController = "Home";

        var context = new TagHelperContext(
            new TagHelperAttributeList(),
            new Dictionary<object, object>(),
            Guid.NewGuid().ToString()
        );

        var output = new TagHelperOutput(
            "a",
            new TagHelperAttributeList(),
            (result, encoder) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent())
        );

        tagHelper.Process(context, output);

        var classAttribute = output.Attributes["class"]?.Value?.ToString();
        Assert.Contains("active", classAttribute);
    }
}
