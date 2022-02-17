using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace VShop.SharedKernel.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static string ToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            Match startUnderscores = Regex.Match(input, @"^_+");

            return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }
        
        public static string ToPascalCase(this string input)
        {
            if (!input.Contains(' ')) input = Regex.Replace(input, "(?<=[a-z])(?=[A-Z])", " ");
            
            return CultureInfo.CurrentCulture.TextInfo
                .ToTitleCase(input.ToLower())
                .Replace(" ", string.Empty)
                .Replace("_", string.Empty);
        }
        
        public static bool IsBase64String(this string value)
        {
            Span<byte> buffer = new(new byte[value.Length]);
            
            return Convert.TryFromBase64String(value, buffer, out int _);
        }
    }
}