using System;
using System.Linq;

namespace EzDbSchema.Core.Templates.Languages
{
    public class PythonLanguage : ILanguageDefinition
    {
        public string Name => "Python";
        public string FileExtension => ".py";

        public string GetDataType(string dbType, int? length = null, int? precision = null, int? scale = null)
        {
            return dbType.ToLowerInvariant() switch
            {
                "int" => "int",
                "bigint" => "int",
                "smallint" => "int",
                "tinyint" => "int",
                "bit" => "bool",
                "decimal" => "Decimal",
                "float" => "float",
                "real" => "float",
                "money" => "Decimal",
                "smallmoney" => "Decimal",
                "datetime" => "datetime",
                "datetime2" => "datetime",
                "date" => "date",
                "time" => "time",
                "char" => "str",
                "nchar" => "str",
                "varchar" => "str",
                "nvarchar" => "str",
                "text" => "str",
                "ntext" => "str",
                "binary" => "bytes",
                "varbinary" => "bytes",
                "image" => "bytes",
                "uniqueidentifier" => "UUID",
                "xml" => "str",
                _ => "Any"
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
                "bit" => "False",
                "decimal" => "Decimal('0')",
                "float" => "0.0",
                "real" => "0.0",
                "money" => "Decimal('0')",
                "smallmoney" => "Decimal('0')",
                "datetime" => "datetime.now()",
                "datetime2" => "datetime.now()",
                "date" => "date.today()",
                "time" => "time()",
                "char" => "''",
                "nchar" => "''",
                "varchar" => "''",
                "nvarchar" => "''",
                "text" => "''",
                "ntext" => "''",
                "binary" => "b''",
                "varbinary" => "b''",
                "image" => "b''",
                "uniqueidentifier" => "uuid4()",
                "xml" => "''",
                _ => "None"
            };
        }

        public string GetNullableType(string type) => $"Optional[{type}]";

        public string GetCollectionType(string type) => $"List[{type}]";

        public string GetImportStatement(string type) => $"from {type.ToLower()} import {type}";

        public string GetClassDeclaration(string className, string baseClass = null, string[] interfaces = null)
        {
            var inheritance = "";
            if (!string.IsNullOrEmpty(baseClass) || (interfaces?.Length ?? 0) > 0)
            {
                var types = new[] { baseClass }
                    .Concat(interfaces ?? Array.Empty<string>())
                    .Where(t => !string.IsNullOrEmpty(t));
                inheritance = $"({string.Join(", ", types)})";
            }
            return $"class {className}{inheritance}:";
        }

        public string GetPropertyDeclaration(string type, string name, bool isNullable, bool isReadOnly = false)
        {
            var typeAnnotation = isNullable ? GetNullableType(type) : type;
            return $"{name}: {typeAnnotation}";
        }

        public string GetMethodDeclaration(string returnType, string name, MethodParameter[] parameters)
        {
            var paramList = parameters?.Select(p => 
            {
                var paramType = p.IsOptional ? GetNullableType(p.Type) : p.Type;
                return $"{p.Name}: {paramType}{(p.IsOptional ? $" = {p.DefaultValue}" : "")}";
            }) ?? Array.Empty<string>();
            
            return $"def {name}({string.Join(", ", paramList)}) -> {returnType}:";
        }

        public string GetConstructorDeclaration(string className, MethodParameter[] parameters)
        {
            var paramList = parameters?.Select(p => 
            {
                var paramType = p.IsOptional ? GetNullableType(p.Type) : p.Type;
                return $"{p.Name}: {paramType}{(p.IsOptional ? $" = {p.DefaultValue}" : "")}";
            }) ?? Array.Empty<string>();
            
            return $"def __init__(self, {string.Join(", ", paramList)}):";
        }

        public string GetInterfaceDeclaration(string name, string[] baseInterfaces = null)
        {
            var inheritance = baseInterfaces?.Length > 0 
                ? $"({string.Join(", ", baseInterfaces)})"
                : "";
            return $"class {name}{inheritance}:";
        }

        public string GetEnumDeclaration(string name) => $"class {name}(Enum):";

        public string GetEnumMember(string name, string value = null)
        {
            return string.IsNullOrEmpty(value) ? $"{name} = auto()" : $"{name} = {value}";
        }

        public string GetComment(string text) => $"\"\"\"\n{text}\n\"\"\"";

        public string GetRegionStart(string name) => $"# region {name}";

        public string GetRegionEnd() => "# endregion";

        public string GetNamespace(string name) => "";  // Python doesn't have traditional namespaces
    }
}
