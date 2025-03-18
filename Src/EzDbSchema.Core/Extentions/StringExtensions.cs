using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EzDbSchema.Core.Extentions
{
    /// <summary>
    /// Extension methods for string operations
    /// </summary>
    public static class StringExtensions
    {
        private static readonly Dictionary<string, string> Plurals = new Dictionary<string, string>
        {
            { "child", "children" },
            { "person", "people" },
            { "ox", "oxen" },
            { "man", "men" },
            { "woman", "women" },
            { "tooth", "teeth" },
            { "foot", "feet" },
            { "mouse", "mice" },
            { "criterion", "criteria" },
            { "life", "lives" }
        };

        /// <summary>
        /// Converts a plural word to its singular form
        /// </summary>
        /// <param name="str">The string to convert to singular</param>
        /// <returns>The singular form of the word</returns>
        public static string ToSingular(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            str = str.Trim();

            // Check for irregular singulars
            var singular = Plurals.FirstOrDefault(x => x.Value.Equals(str, StringComparison.OrdinalIgnoreCase)).Key;
            if (singular != null)
                return singular;

            // Handle common singular rules
            if (str.EndsWith("ies"))
                return str.Substring(0, str.Length - 3) + "y";
            if (str.EndsWith("ves"))
                return str.Substring(0, str.Length - 3) + "f";
            if (str.EndsWith("es") && (str.EndsWith("shes") || str.EndsWith("ches")))
                return str.Substring(0, str.Length - 2);
            if (str.EndsWith("s") && !str.EndsWith("ss"))
                return str.Substring(0, str.Length - 1);

            return str;
        }

        /// <summary>
        /// Converts a singular word to its plural form
        /// </summary>
        /// <param name="str">The string to convert to plural</param>
        /// <returns>The plural form of the word</returns>
        public static string ToPlural(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            str = str.Trim();

            // Check for irregular plurals
            if (Plurals.TryGetValue(str.ToLower(), out var plural))
                return plural;

            // Handle common plural rules
            if (str.EndsWith("y") && !str.EndsWith("ay") && !str.EndsWith("ey") && !str.EndsWith("oy"))
                return str.Substring(0, str.Length - 1) + "ies";
            if (str.EndsWith("f"))
                return str.Substring(0, str.Length - 1) + "ves";
            if (str.EndsWith("fe"))
                return str.Substring(0, str.Length - 2) + "ves";
            if (str.EndsWith("sh") || str.EndsWith("ch") || str.EndsWith("x") || str.EndsWith("s"))
                return str + "es";

            return str + "s";
        }

        /// <summary>
        /// Converts a string to snake case
        /// </summary>
        /// <param name="str">The string to convert</param>
        /// <returns>The snake case version of the string</returns>
        public static string ToSnakeCase(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            
            // Replace any non-letter/non-digit character with underscore
            var name = Regex.Replace(str, @"[^\w]", "_");
            
            // Insert underscore before capital letters (except first letter)
            name = Regex.Replace(name, @"([A-Z])", "_$1", RegexOptions.Compiled).TrimStart('_');
            
            // Convert to lowercase and replace multiple underscores with single
            return Regex.Replace(name.ToLower(), @"_+", "_");
        }

        /// <summary>
        /// Converts a database type to its JavaScript equivalent
        /// </summary>
        /// <param name="str">The database type string</param>
        /// <returns>The JavaScript type</returns>
        public static string ToJsType(this string str)
        {
            if (string.IsNullOrEmpty(str)) return "any";

            return str.ToLower() switch
            {
                "int" or "bigint" or "smallint" or "tinyint" or "decimal" or "numeric" or "float" or "real" => "number",
                "bit" or "boolean" => "boolean",
                "datetime" or "datetime2" or "date" or "time" or "timestamp" => "Date",
                "uniqueidentifier" => "string",
                _ => "string"
            };
        }

        /// <summary>
        /// Gets a stable hash code for a string that remains consistent across different .NET runtimes
        /// </summary>
        /// <param name="str">The string to hash</param>
        /// <returns>A stable hash code</returns>
        public static int GetStableHashCode(this string str)
        {
            if (string.IsNullOrEmpty(str)) return 0;

            unchecked
            {
                int hash = 23;
                foreach (char c in str)
                {
                    hash = hash * 31 + c;
                }
                return hash;
            }
        }

        /// <summary>
        /// Resolves environment variables in a path string
        /// </summary>
        /// <param name="path">The path string containing variables to resolve</param>
        /// <param name="getEnvironmentVariable">Optional function to get environment variables</param>
        /// <returns>The resolved path</returns>
        public static string ResolvePathVars(this string path, Func<string, string> getEnvironmentVariable = null)
        {
            if (string.IsNullOrEmpty(path)) return path;

            getEnvironmentVariable ??= Environment.GetEnvironmentVariable;

            return Regex.Replace(path, @"\{([^}]+)\}", match =>
            {
                var varName = match.Groups[1].Value;
                var value = getEnvironmentVariable(varName);
                return value ?? match.Value;
            });
        }
    }
}
