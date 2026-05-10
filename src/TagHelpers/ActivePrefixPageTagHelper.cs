using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace KozLibraries.TagHelpers;

[HtmlTargetElement("a", Attributes = "active-when-prefix")]
public sealed class ActivePrefixPageTagHelper : TagHelper
{
    [ViewContext]
    [HtmlAttributeNotBound]
    public required ViewContext ViewContext { get; set; }
    
    [HtmlAttributeName("active-when-prefix")]
    public string? Prefix { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (string.IsNullOrWhiteSpace(Prefix))
        {
            return;
        }
        
        var currentPage = ViewContext.RouteData.Values["page"]?.ToString();
        if (string.IsNullOrWhiteSpace(currentPage))
        {
            return;
        }
        
        if (currentPage.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase))
        {
            if (output.Attributes.TryGetAttribute("class", out var classAttribute))
            {
                output.Attributes.SetAttribute("class", $"{classAttribute.Value} active");
            }
            else
            {
                output.Attributes.Add("class", "active");
            }
        }

        output.Attributes.RemoveAll("active-when-prefix");
    }
}
