using System;
using System.Linq;

namespace EzDbSchema.Core.Templates.Languages
{
    public class GoLanguage : ILanguageDefinition
    {
        public string Name => "Go";
        public string FileExtension => ".go";

        public string GetDataType(string dbType, int? length = null, int? precision = null, int? scale = null)
        {
            return dbType.ToLowerInvariant() switch
            {
                "int" => "int",
                "bigint" => "int64",
                "smallint" => "int16",
                "tinyint" => "int8",
                "bit" => "bool",
                "decimal" => "decimal.Decimal",
                "float" => "float32",
                "real" => "float64",
                "money" => "decimal.Decimal",
                "smallmoney" => "decimal.Decimal",
                "datetime" => "time.Time",
                "datetime2" => "time.Time",
                "date" => "time.Time",
                "time" => "time.Time",
                "char" => length == 1 ? "rune" : "string",
                "nchar" => length == 1 ? "rune" : "string",
                "varchar" => "string",
                "nvarchar" => "string",
                "text" => "string",
                "ntext" => "string",
                "binary" => "[]byte",
                "varbinary" => "[]byte",
                "image" => "[]byte",
                "uniqueidentifier" => "uuid.UUID",
                "xml" => "string",
                _ => "interface{}"
            };
        }

        public string GetDefaultValue(string dbType)
        {
            return dbType.ToLowerInvariant() switch
            {
                "int" => "0",
                "bigint" => "0",
                "smallint" => "0",
                "tinyint" => "0",
                "bit" => "false",
                "decimal" => "decimal.Zero",
                "float" => "0.0",
                "real" => "0.0",
                "money" => "decimal.Zero",
                "smallmoney" => "decimal.Zero",
                "datetime" => "time.Now()",
                "datetime2" => "time.Now()",
                "date" => "time.Now()",
                "time" => "time.Now()",
                "char" => "''",
                "nchar" => "''",
                "varchar" => "\"\"",
                "nvarchar" => "\"\"",
                "text" => "\"\"",
                "ntext" => "\"\"",
                "binary" => "[]byte{}",
                "varbinary" => "[]byte{}",
                "image" => "[]byte{}",
                "uniqueidentifier" => "uuid.New()",
                "xml" => "\"\"",
                _ => "nil"
            };
        }

        public string GetNullableType(string type) => $"*{type}";

        public string GetCollectionType(string type) => $"[]${type}";

        public string GetImportStatement(string type) => $"import \"{type}\"";

        public string GetClassDeclaration(string className, string baseClass = null, string[] interfaces = null)
        {
            return $"type {className} struct";
        }

        public string GetPropertyDeclaration(string type, string name, bool isNullable, bool isReadOnly = false)
        {
            var finalType = isNullable ? GetNullableType(type) : type;
            return $"{HandlebarsHelpers.ToPascalCase(name)} {finalType} `json:\"{HandlebarsHelpers.ToCamelCase(name)}\"`";
        }

        public string GetMethodDeclaration(string returnType, string name, MethodParameter[] parameters)
        {
            var paramList = parameters?.Select(p => $"{p.Name} {p.Type}") ?? Array.Empty<string>();
            var returnStr = string.IsNullOrEmpty(returnType) || returnType == "void" ? "" : $" {returnType}";
            return $"func ({name}){returnStr}";
        }

        public string GetConstructorDeclaration(string className, MethodParameter[] parameters)
        {
            var paramList = parameters?.Select(p => $"{p.Name} {p.Type}") ?? Array.Empty<string>();
            return $"func New{className}({string.Join(", ", paramList)}) *{className}";
        }

        public string GetInterfaceDeclaration(string name, string[] baseInterfaces = null)
        {
            return $"type {name} interface";
        }

        public string GetEnumDeclaration(string name) => $"type {name} int";

        public string GetEnumMember(string name, string value = null)
        {
            return string.IsNullOrEmpty(value) ? $"{name} {name}Type = iota" : $"{name} {name}Type = {value}";
        }

        public string GetComment(string text) => $"// {text}";

        public string GetRegionStart(string name) => $"// region {name}";

        public string GetRegionEnd() => "// endregion";

        public string GetNamespace(string name) => $"package {name}";
    }
}
