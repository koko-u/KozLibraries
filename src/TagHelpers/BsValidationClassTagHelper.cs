using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace KozLibraries.TagHelpers;

[HtmlTargetElement("input", Attributes = "asp-for, bs-valid")]
[HtmlTargetElement("textarea", Attributes = "asp-for, bs-valid")]
[HtmlTargetElement("select", Attributes = "asp-for, bs-valid")]
public sealed class BsValidationClassTagHelper : TagHelper
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
        // bypass hidden inputs
        if (output.IsHiddenInput())
        {
            return;
        }

        // asp-for attribute name
        var fullName = ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(For.Name);

        // ModelState error of attribute name
        if (!ViewContext.ViewData.ModelState.TryGetValue(fullName, out var modelStateEntry))
        {
            return;
        }

        if (
            modelStateEntry.ValidationState == ModelValidationState.Invalid
            || modelStateEntry.Errors.Count > 0
        )
        {
            output.AddCssClass("is-invalid");
        }
        else
        {
            output.AddCssClass("is-valid");
        }

        output.Attributes.RemoveAll("bs-valid");
    }
}
