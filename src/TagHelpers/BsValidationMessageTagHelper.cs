using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace KozLibraries.TagHelpers;

[HtmlTargetElement(Attributes = "asp-validation-for, bs-feedback")]
public sealed class BsValidationMessageTagHelper : TagHelper
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
        output.AddCssClass("invalid-feedback");
    }
}
