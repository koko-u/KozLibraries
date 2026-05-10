using System.Linq;
using KozLibraries.TagHelpers.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;

namespace KozLibraries.TagHelpers;

[HtmlTargetElement("input", Attributes = "asp-for, bs-valid")]
[HtmlTargetElement("textarea", Attributes = "asp-for, bs-valid")]
[HtmlTargetElement("select", Attributes = "asp-for, bs-valid")]
public sealed class BsValidationClassTagHelper(ILogger<BsValidationClassTagHelper> logger)
    : TagHelper
{
    [ViewContext]
    [HtmlAttributeNotBound]
    public required ViewContext ViewContext { get; set; }

    [HtmlAttributeName("asp-for")]
    public ModelExpression For { get; set; } = null!;

    [HtmlAttributeName("bs-valid")]
    public bool Enabled { get; set; }

    public override int Order => 1000;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        logger.LogDebug("bs-valid TagHelper invoked.");

        // bypass hidden inputs
        if (output.IsHiddenInput())
        {
            return;
        }

        // asp-for attribute name
        var fullName = ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(For.Name);
        logger.LogDebug($"Processing bs-valid for field: {fullName}");

        // ModelState error of attribute name
        if (!ViewContext.ViewData.ModelState.TryGetValue(fullName, out var modelStateEntry))
        {
            logger.LogDebug($"No model state entry found for field: {fullName}");
            return;
        }

        logger.LogDebug($"{fullName}'s validation state: {modelStateEntry.ValidationState}");
        logger.LogDebug(
            $"{fullName}'s validation error: {modelStateEntry.Errors.FirstOrDefault()?.ErrorMessage}"
        );
        if (
            modelStateEntry.ValidationState == ModelValidationState.Invalid
            || modelStateEntry.Errors.Count > 0
        )
        {
            output.AddCssClass("is-invalid", logger);
            output.Attributes.SetAttribute("aria-describedby", $"{fullName}_Feedback");
        }
        else
        {
            output.AddCssClass("is-valid", logger);
        }

        output.Attributes.RemoveAll("bs-valid");
    }
}
