using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EzDbSchema.Core.Templates
{
    public static class HandlebarsHelpers
    {
        private static readonly Dictionary<string, string> Plurals = new()
        {
            { "child", "children" },
            { "person", "people" },
            { "ox", "oxen" },
            { "man", "men" },
            { "woman", "women" },
            { "tooth", "teeth" },
            { "foot", "feet" },
            { "mouse", "mice" },
            { "criterion", "criteria" }
        };

        public static string ToCamelCase(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            var words = SplitIntoWords(str);
            return char.ToLowerInvariant(words[0][0]) + words[0][1..] +
                   string.Join("", words.Skip(1).Select(w => char.ToUpperInvariant(w[0]) + w[1..]));
        }

        public static string ToPascalCase(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            var words = SplitIntoWords(str);
            return string.Join("", words.Select(w => char.ToUpperInvariant(w[0]) + w[1..]));
        }

        public static string ToSnakeCase(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            var words = SplitIntoWords(str);
            return string.Join("_", words.Select(w => w.ToLower()));
        }

        public static string ToKebabCase(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            var words = SplitIntoWords(str);
            return string.Join("-", words.Select(w => w.ToLower()));
        }

        public static string ToConstantCase(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            var words = SplitIntoWords(str);
            return string.Join("_", words.Select(w => w.ToUpper()));
        }

        public static string ToPlural(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            str = str.Trim();

            // Check for irregular plurals
            if (Plurals.TryGetValue(str.ToLower(), out var plural))
                return plural;

            // Handle common plural rules
            if (str.EndsWith("y") && !IsVowel(str[^2]))
                return str[..^1] + "ies";
            if (str.EndsWith("s") || str.EndsWith("sh") || str.EndsWith("ch") ||
                str.EndsWith("x") || str.EndsWith("z"))
                return str + "es";
            if (str.EndsWith("f"))
                return str[..^1] + "ves";
            if (str.EndsWith("fe"))
                return str[..^2] + "ves";
            if (str.EndsWith("o") && !IsVowel(str[^2]))
                return str + "es";
            
            return str + "s";
        }

        public static string ToSingular(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            str = str.Trim();

            // Check for irregular singulars
            var singular = Plurals.FirstOrDefault(x => x.Value.Equals(str, StringComparison.OrdinalIgnoreCase)).Key;
            if (singular != null)
                return singular;

            // Handle common singular rules
            if (str.EndsWith("ies"))
                return str[..^3] + "y";
            if (str.EndsWith("ves"))
                return str[..^3] + "f";
            if (str.EndsWith("es") && (str.EndsWith("shes") || str.EndsWith("ches")))
                return str[..^2];
            if (str.EndsWith("s") && !str.EndsWith("ss"))
                return str[..^1];

            return str;
        }

        public static string ToTitleCase(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            var words = SplitIntoWords(str);
            return string.Join(" ", words.Select(w => char.ToUpperInvariant(w[0]) + w[1..].ToLower()));
        }

        public static string ToSentenceCase(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            var words = SplitIntoWords(str);
            return char.ToUpperInvariant(words[0][0]) + words[0][1..].ToLower() +
                   string.Join(" ", words.Skip(1).Select(w => w.ToLower()));
        }

        public static string Abbreviate(string str, int maxLength = 3)
        {
            if (string.IsNullOrEmpty(str)) return str;
            var words = SplitIntoWords(str);
            return string.Join("", words.Select(w => char.ToUpperInvariant(w[0])));
        }

        private static string[] SplitIntoWords(string str)
        {
            // Handle snake_case
            if (str.Contains('_'))
                return str.Split('_', StringSplitOptions.RemoveEmptyEntries);

            // Handle kebab-case
            if (str.Contains('-'))
                return str.Split('-', StringSplitOptions.RemoveEmptyEntries);

            // Handle camelCase and PascalCase
            return Regex.Split(str, @"(?<!^)(?=[A-Z])");
        }

        private static bool IsVowel(char c)
        {
            return "aeiouAEIOU".Contains(c);
        }

        public static string Quote(string str) => $"\"{str}\"";

        public static string SingleQuote(string str) => $"'{str}'";

        public static string Escape(string str) => 
            str?.Replace("\"", "\\\"").Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "\\r");

        public static string Join(IEnumerable<string> items, string separator = ", ") => 
            string.Join(separator, items);

        public static string Repeat(string str, int count) =>
            string.Concat(Enumerable.Repeat(str, count));

        public static string Pad(string str, int length, char padChar = ' ', bool padLeft = true) =>
            padLeft ? str.PadLeft(length, padChar) : str.PadRight(length, padChar);

        public static string Truncate(string str, int maxLength, string suffix = "...") =>
            str.Length <= maxLength ? str : str[..(maxLength - suffix.Length)] + suffix;
    }
}
