using System.Text;
using System.Text.RegularExpressions;

namespace Fixopolis.Application.Common;

public static class SlugHelper
{
    public static string GenerateSlug(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        text = text.ToLowerInvariant();
        
        text = RemoveDiacritics(text);
        
        text = Regex.Replace(text, @"[^a-z0-9\s-]", "");
        
        text = Regex.Replace(text, @"\s+", " ");
        
        text = text.Trim();
        
        text = Regex.Replace(text, @"\s+", "-");
        
        return text;
    }
    
    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new System.Text.StringBuilder();
        
        foreach (var c in normalizedString)
        {
            var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }
        
        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}