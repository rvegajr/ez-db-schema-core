using System;
using System.Linq;

namespace EzDbSchema.Core.Templates.Languages
{
    public class JavaLanguage : ILanguageDefinition
    {
        public string Name => "Java";
        public string FileExtension => ".java";

        public string GetDataType(string dbType, int? length = null, int? precision = null, int? scale = null)
        {
            return dbType.ToLowerInvariant() switch
            {
                "int" => "Integer",
                "bigint" => "Long",
                "smallint" => "Short",
                "tinyint" => "Byte",
                "bit" => "Boolean",
                "decimal" => "BigDecimal",
                "float" => "Float",
                "real" => "Double",
                "money" => "BigDecimal",
                "smallmoney" => "BigDecimal",
                "datetime" => "LocalDateTime",
                "datetime2" => "LocalDateTime",
                "date" => "LocalDate",
                "time" => "LocalTime",
                "char" => length == 1 ? "Character" : "String",
                "nchar" => length == 1 ? "Character" : "String",
                "varchar" => "String",
                "nvarchar" => "String",
                "text" => "String",
                "ntext" => "String",
                "binary" => "byte[]",
                "varbinary" => "byte[]",
                "image" => "byte[]",
                "uniqueidentifier" => "UUID",
                "xml" => "String",
                _ => "Object"
            };
        }

        public string GetDefaultValue(string dbType)
        {
            return dbType.ToLowerInvariant() switch
            {
                "int" => "0",
                "bigint" => "0L",
                "smallint" => "(short)0",
                "tinyint" => "(byte)0",
                "bit" => "false",
                "decimal" => "BigDecimal.ZERO",
                "float" => "0.0f",
                "real" => "0.0d",
                "money" => "BigDecimal.ZERO",
                "smallmoney" => "BigDecimal.ZERO",
                "datetime" => "LocalDateTime.now()",
                "datetime2" => "LocalDateTime.now()",
                "date" => "LocalDate.now()",
                "time" => "LocalTime.now()",
                "char" => "''",
                "nchar" => "''",
                "varchar" => "\"\"",
                "nvarchar" => "\"\"",
                "text" => "\"\"",
                "ntext" => "\"\"",
                "binary" => "new byte[0]",
                "varbinary" => "new byte[0]",
                "image" => "new byte[0]",
                "uniqueidentifier" => "UUID.randomUUID()",
                "xml" => "\"\"",
                _ => "null"
            };
        }

        public string GetNullableType(string type) => type;

        public string GetCollectionType(string type) => $"List<{type}>";

        public string GetImportStatement(string type) => $"import {type};";

        public string GetClassDeclaration(string className, string baseClass = null, string[] interfaces = null)
        {
            var inheritance = "";
            if (!string.IsNullOrEmpty(baseClass))
                inheritance += $" extends {baseClass}";
            if (interfaces?.Length > 0)
                inheritance += $" implements {string.Join(", ", interfaces)}";
            return $"public class {className}{inheritance}";
        }

        public string GetPropertyDeclaration(string type, string name, bool isNullable, bool isReadOnly = false)
        {
            var annotation = isNullable ? "@Nullable" : "@NotNull";
            return $"{annotation}\nprivate {type} {HandlebarsHelpers.ToCamelCase(name)};";
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
                $"{p.Type} {p.Name}")
                ?? Array.Empty<string>();
            return $"public {className}({string.Join(", ", paramList)})";
        }

        public string GetInterfaceDeclaration(string name, string[] baseInterfaces = null)
        {
            var inheritance = baseInterfaces?.Length > 0 
                ? $" extends {string.Join(", ", baseInterfaces)}"
                : "";
            return $"public interface {name}{inheritance}";
        }

        public string GetEnumDeclaration(string name) => $"public enum {name}";

        public string GetEnumMember(string name, string value = null)
        {
            return string.IsNullOrEmpty(value) ? name : $"{name}({value})";
        }

        public string GetComment(string text) => $"/**\n * {text}\n */";

        public string GetRegionStart(string name) => $"// region {name}";

        public string GetRegionEnd() => "// endregion";

        public string GetNamespace(string name) => $"package {name};";
    }
}
