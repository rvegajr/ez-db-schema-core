using System.Json;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("EzDbSchema.MsSql")]
[assembly: InternalsVisibleTo("EzDbCodeGen.Core")]

namespace EzDbSchema.Core.Extentions.Json
{
    internal static class JsonExtensions
    {
        private const string DOUBLE_QUOTE_SUB = @"_$$_";
        private const string DOUBLE_QUOTE = @"""""";
        private const string DOUBLE_SLASH = @"\\";

        internal static string AsString(this JsonValue obj)
        {
            return obj.ToString().Replace(DOUBLE_QUOTE, DOUBLE_QUOTE_SUB).Replace("\"", "").Replace(DOUBLE_QUOTE_SUB, "\"").Replace(DOUBLE_SLASH, @"\");
        }
    }
}