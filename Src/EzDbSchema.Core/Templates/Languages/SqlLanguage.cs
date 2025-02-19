using System;
using System.Linq;

namespace EzDbSchema.Core.Templates.Languages
{
    public class SqlLanguage : ILanguageDefinition
    {
        public string Name => "SQL";
        public string FileExtension => ".sql";

        public string GetDataType(string dbType, int? length = null, int? precision = null, int? scale = null)
        {
            var type = dbType.ToUpperInvariant();
            if (length.HasValue && (type.Contains("CHAR") || type.Contains("BINARY")))
                return $"{type}({length})";
            if (precision.HasValue)
                return scale.HasValue ? $"{type}({precision},{scale})" : $"{type}({precision})";
            return type;
        }

        public string GetDefaultValue(string dbType)
        {
            return dbType.ToLowerInvariant() switch
            {
                "int" => "0",
                "bigint" => "0",
                "smallint" => "0",
                "tinyint" => "0",
                "bit" => "0",
                "decimal" => "0.0",
                "float" => "0.0",
                "real" => "0.0",
                "money" => "0.0",
                "smallmoney" => "0.0",
                "datetime" => "GETDATE()",
                "datetime2" => "GETDATE()",
                "date" => "GETDATE()",
                "time" => "GETDATE()",
                "char" => "''",
                "nchar" => "N''",
                "varchar" => "''",
                "nvarchar" => "N''",
                "text" => "''",
                "ntext" => "N''",
                "binary" => "0x",
                "varbinary" => "0x",
                "image" => "0x",
                "uniqueidentifier" => "NEWID()",
                "xml" => "''",
                _ => "NULL"
            };
        }

        public string GetNullableType(string type) => type;

        public string GetCollectionType(string type) => type;

        public string GetImportStatement(string type) => "";

        public string GetClassDeclaration(string className, string baseClass = null, string[] interfaces = null)
        {
            return $"CREATE TABLE {className}";
        }

        public string GetPropertyDeclaration(string type, string name, bool isNullable, bool isReadOnly = false)
        {
            return $"{name} {type}{(isNullable ? "" : " NOT NULL")}";
        }

        public string GetMethodDeclaration(string returnType, string name, MethodParameter[] parameters)
        {
            var paramList = parameters?.Select(p => 
                $"@{p.Name} {p.Type}{(p.IsOptional ? $" = {p.DefaultValue}" : "")}")
                ?? Array.Empty<string>();
            return $"CREATE PROCEDURE {name} {string.Join(", ", paramList)}";
        }

        public string GetConstructorDeclaration(string className, MethodParameter[] parameters)
        {
            return GetClassDeclaration(className);
        }

        public string GetInterfaceDeclaration(string name, string[] baseInterfaces = null)
        {
            return $"CREATE VIEW {name}";
        }

        public string GetEnumDeclaration(string name)
        {
            return $"CREATE TYPE {name} AS ENUM";
        }

        public string GetEnumMember(string name, string value = null)
        {
            return $"'{name}'";
        }

        public string GetComment(string text) => $"-- {text}";

        public string GetRegionStart(string name) => $"-- region {name}";

        public string GetRegionEnd() => "-- endregion";

        public string GetNamespace(string name) => $"USE {name}";
    }
}
