using System;
using System.Linq;

namespace EzDbSchema.Core.Templates.Languages
{
    public class CSharpLanguage : ILanguageDefinition
    {
        public string Name => "C#";
        public string FileExtension => ".cs";

        public string GetDataType(string dbType, int? length = null, int? precision = null, int? scale = null)
        {
            return dbType.ToLowerInvariant() switch
            {
                "int" => "int",
                "bigint" => "long",
                "smallint" => "short",
                "tinyint" => "byte",
                "bit" => "bool",
                "decimal" => "decimal",
                "float" => "float",
                "real" => "double",
                "money" => "decimal",
                "smallmoney" => "decimal",
                "datetime" => "DateTime",
                "datetime2" => "DateTime",
                "date" => "DateTime",
                "time" => "TimeSpan",
                "char" => length == 1 ? "char" : "string",
                "nchar" => length == 1 ? "char" : "string",
                "varchar" => "string",
                "nvarchar" => "string",
                "text" => "string",
                "ntext" => "string",
                "binary" => "byte[]",
                "varbinary" => "byte[]",
                "image" => "byte[]",
                "uniqueidentifier" => "Guid",
                "xml" => "string",
                _ => "object"
            };
        }

        public string GetDefaultValue(string dbType)
        {
            return dbType.ToLowerInvariant() switch
            {
                "int" => "0",
                "bigint" => "0L",
                "smallint" => "0",
                "tinyint" => "0",
                "bit" => "false",
                "decimal" => "0m",
                "float" => "0f",
                "real" => "0d",
                "money" => "0m",
                "smallmoney" => "0m",
                "datetime" => "DateTime.MinValue",
                "datetime2" => "DateTime.MinValue",
                "date" => "DateTime.MinValue",
                "time" => "TimeSpan.Zero",
                "char" => "''",
                "nchar" => "''",
                "varchar" => "string.Empty",
                "nvarchar" => "string.Empty",
                "text" => "string.Empty",
                "ntext" => "string.Empty",
                "binary" => "Array.Empty<byte>()",
                "varbinary" => "Array.Empty<byte>()",
                "image" => "Array.Empty<byte>()",
                "uniqueidentifier" => "Guid.Empty",
                "xml" => "string.Empty",
                _ => "null"
            };
        }

        public string GetNullableType(string type) => $"{type}?";

        public string GetCollectionType(string type) => $"ICollection<{type}>";

        public string GetImportStatement(string type) => $"using {type};";

        public string GetClassDeclaration(string className, string baseClass = null, string[] interfaces = null)
        {
            var inheritance = "";
            if (!string.IsNullOrEmpty(baseClass) || (interfaces?.Length ?? 0) > 0)
            {
                var types = new[] { baseClass }
                    .Concat(interfaces ?? Array.Empty<string>())
                    .Where(t => !string.IsNullOrEmpty(t));
                inheritance = $" : {string.Join(", ", types)}";
            }
            return $"public class {className}{inheritance}";
        }

        public string GetPropertyDeclaration(string type, string name, bool isNullable, bool isReadOnly = false)
        {
            var nullableType = isNullable && !type.EndsWith("?") && type != "string" ? GetNullableType(type) : type;
            return $"public {nullableType} {name} {{ get; {(isReadOnly ? "private " : "")}set; }}";
        }

        public string GetMethodDeclaration(string returnType, string name, MethodParameter[] parameters)
        {
            var paramList = parameters?.Select(p => 
                $"{p.Type} {p.Name}{(p.IsOptional ? $" = {p.DefaultValue}" : "")}")
                ?? Array.Empty<string>();
            return $"public {returnType} {name}({string.Join(", ", paramList)})";
        }

        public string GetConstructorDeclaration(string className, MethodParameter[] parameters)
        {
            var paramList = parameters?.Select(p => 
                $"{p.Type} {p.Name}{(p.IsOptional ? $" = {p.DefaultValue}" : "")}")
                ?? Array.Empty<string>();
            return $"public {className}({string.Join(", ", paramList)})";
        }

        public string GetInterfaceDeclaration(string name, string[] baseInterfaces = null)
        {
            var inheritance = baseInterfaces?.Length > 0 
                ? $" : {string.Join(", ", baseInterfaces)}"
                : "";
            return $"public interface {name}{inheritance}";
        }

        public string GetEnumDeclaration(string name) => $"public enum {name}";

        public string GetEnumMember(string name, string value = null)
        {
            return string.IsNullOrEmpty(value) ? name : $"{name} = {value}";
        }

        public string GetComment(string text) => $"/// <summary>\n/// {text}\n/// </summary>";

        public string GetRegionStart(string name) => $"#region {name}";

        public string GetRegionEnd() => "#endregion";

        public string GetNamespace(string name) => $"namespace {name}";
    }
}
