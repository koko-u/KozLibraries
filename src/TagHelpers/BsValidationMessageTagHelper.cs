using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;

namespace KozLibraries.TagHelpers;

[HtmlTargetElement(Attributes = "asp-validation-for, bs-feedback")]
public sealed class BsValidationMessageTagHelper(ILogger<BsValidationMessageTagHelper> logger)
    : TagHelper
{
    [ViewContext]
    [HtmlAttributeNotBound]
    public required ViewContext ViewContext { get; set; }

    [HtmlAttributeName("asp-validation-for")]
    public ModelExpression For { get; set; } = null!;

    [HtmlAttributeName("bs-feedback")]
    public bool Enabled { get; set; }

    public override int Order => 1000;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        logger.LogDebug("bs-feedback TagHelper invoked.");

        // asp-for attribute name
        var fullName = ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(For.Name);
        logger.LogDebug($"Processing bs-valid for field: {fullName}");

        output.Attributes.SetAttribute("id", $"{fullName}_Feedback");
        output.AddCssClass("invalid-feedback");
    }
}
