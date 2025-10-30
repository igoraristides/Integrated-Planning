using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PlanejamentoIntegrado.Helpers;

[HtmlTargetElement("*", Attributes = "asp-controller")]
public class MenuActiveTagHelper : TagHelper
{
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    [HtmlAttributeName("asp-controller")]
    public string AspController { get; set; }

    [HtmlAttributeName("asp-action")]
    public string AspAction { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var currentController = ViewContext.RouteData.Values["controller"]?.ToString();
        var currentAction = ViewContext.RouteData.Values["action"]?.ToString();

        var isActive = string.Equals(currentController, AspController, StringComparison.OrdinalIgnoreCase);

        if (!string.IsNullOrWhiteSpace(AspAction))
            isActive &= string.Equals(currentAction, AspAction, StringComparison.OrdinalIgnoreCase);

        if (isActive)
        {
            var existingClass = output.Attributes["class"]?.Value?.ToString() ?? string.Empty;
            output.Attributes.SetAttribute("class", $"{existingClass} active");
        }
    }
}
