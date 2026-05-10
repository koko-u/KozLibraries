using System;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace KozLibraries.TagHelpers.Extensions;

internal static class TagHelperOutputExtension
{
    public static bool IsHiddenInput(this TagHelperOutput output)
    {
        if (!string.Equals(output.TagName, "input", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!output.Attributes.TryGetAttribute("type", out var attr))
        {
            return false;
        }

        return string.Equals(attr.Value?.ToString(), "hidden", StringComparison.OrdinalIgnoreCase);
    }
}
