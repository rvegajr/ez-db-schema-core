using System;
using System.Collections.Generic;
using System.Linq;

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
    }
}
