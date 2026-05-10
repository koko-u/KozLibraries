using System;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;

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

    public static void AddCssClass(
        this TagHelperOutput output,
        string cssClass,
        ILogger? logger = null
    )
    {
        if (!output.Attributes.TryGetAttribute("class", out var classAttr))
        {
            output.Attributes.SetAttribute("class", cssClass);
            return;
        }

        var classValue = classAttr.Value?.ToString() ?? string.Empty;
        logger?.LogDebug($"current class attributes: {classValue}");

        var classValues = classValue.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        if (classValues.Length == 0)
        {
            // no class attribute, then set cssClass
            output.Attributes.SetAttribute("class", cssClass);
            return;
        }

        if (!classValues.Contains(cssClass))
        {
            // classes not contains cssClass, then append cssClass
            output.Attributes.SetAttribute("class", $"{classValue} {cssClass}");
        }

        // classes already contains cssClass, do nothing
    }
}
