using System.Text.Json;
using System.Text.Json.Nodes;
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

        internal static string AsString(this JsonNode? node)
        {
            if (node == null) return string.Empty;
            
            var jsonString = node.ToJsonString();
            return jsonString
                .Replace(DOUBLE_QUOTE, DOUBLE_QUOTE_SUB)
                .Replace("\"", "")
                .Replace(DOUBLE_QUOTE_SUB, "\"")
                .Replace(DOUBLE_SLASH, @"\");
        }
    }
}