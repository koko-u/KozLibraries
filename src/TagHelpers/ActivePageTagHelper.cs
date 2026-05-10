using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace KozLibraries.TagHelpers;

/// <summary>
/// Tag helper for marking active pages in navigation links.
/// </summary>
[HtmlTargetElement("a", Attributes = "asp-page, active-when-page")]
public sealed class ActivePageTagHelper : TagHelper
{
    [ViewContext]
    [HtmlAttributeNotBound]
    public required ViewContext ViewContext { get; set; }

    [HtmlAttributeName("asp-page")]
    public string? Page { get; set; }

    [HtmlAttributeName("active-when-page")]
    public bool Enabled { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var currentPage = ViewContext.RouteData.Values["page"]?.ToString();
        if (string.Equals(currentPage, this.Page, StringComparison.OrdinalIgnoreCase))
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

        output.Attributes.RemoveAll("active-when-page");
    }
}
