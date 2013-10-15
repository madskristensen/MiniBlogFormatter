using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MiniBlogFormatter
{
    public static class FormatterHelpers
    {
        public static string FormatSlug(string slug)
        {
            string text = slug.ToLowerInvariant().Replace(" ", "-");
            return Regex.Replace(text, @"([^0-9a-z-\(\)])", string.Empty).Trim();
        }
    }
}
