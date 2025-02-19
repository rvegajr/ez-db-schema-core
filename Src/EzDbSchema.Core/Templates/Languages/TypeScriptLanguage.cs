using System;
using System.Collections.Generic;
using System.Linq;

namespace EzDbSchema.Core.Templates.Languages
{
    public class TypeScriptLanguage : ILanguageDefinition
    {
        private static readonly Dictionary<string, string> TypeMappings = new()
        {
            { "int", "number" },
            { "bigint", "number" },
            { "smallint", "number" },
            { "tinyint", "number" },
            { "decimal", "number" },
            { "numeric", "number" },
            { "money", "number" },
            { "float", "number" },
            { "real", "number" },
            { "bit", "boolean" },
            { "datetime", "Date" },
            { "datetime2", "Date" },
            { "date", "Date" },
            { "time", "string" },
            { "char", "string" },
            { "nchar", "string" },
            { "varchar", "string" },
            { "nvarchar", "string" },
            { "text", "string" },
            { "ntext", "string" },
            { "binary", "Buffer" },
            { "varbinary", "Buffer" },
            { "image", "Buffer" },
            { "uniqueidentifier", "string" }
        };

        public string Name => "TypeScript";
        public string FileExtension => ".ts";

        public string GetDataType(string sqlType, int? maxLength = null, int? precision = null, int? scale = null)
        {
            if (TypeMappings.TryGetValue(sqlType.ToLower(), out var tsType))
                return tsType;
            return "any";
        }

        public string GetCollectionType(string elementType)
        {
            return $"{elementType}[]";
        }

        public string GetImportStatement(string module)
        {
            return $"import {{ {module} }} from './{module}';";
        }

        public string GetNullableType(string type)
        {
            return $"{type} | null";
        }

        public string GetDefaultValue(string type)
        {
            return type.ToLower() switch
            {
                "number" => "0",
                "boolean" => "false",
                "date" => "new Date()",
                "string" => "\"\"",
                "buffer" => "Buffer.alloc(0)",
                _ => "null"
            };
        }

        public string GetClassDeclaration(string className, string baseClass = "", string[] interfaces = null)
        {
            var inheritance = "";
            if (!string.IsNullOrEmpty(baseClass))
                inheritance = $" extends {baseClass}";
            if (interfaces?.Length > 0)
                inheritance += $" implements {string.Join(", ", interfaces)}";

            return $"export class {className}{inheritance}";
        }

        public string GetInterfaceDeclaration(string interfaceName, string[] extends = null)
        {
            var inheritance = extends?.Length > 0 ? $" extends {string.Join(", ", extends)}" : "";
            return $"export interface {interfaceName}{inheritance}";
        }

        public string GetPropertyDeclaration(string type, string name, bool isNullable, bool isReadOnly)
        {
            return $"{(isReadOnly ? "readonly " : "")}{name}{(isNullable ? "?" : "")}: {type};";
        }

        public string GetNamespace(string namespaceName)
        {
            return $"// Module: {namespaceName}";
        }

        public string GetComment(string comment)
        {
            return $"/** {comment} */";
        }

        public string GetMethodDeclaration(string returnType, string name, MethodParameter[] parameters)
        {
            var paramList = string.Join(", ", parameters.Select(p => 
                $"{p.Name}{(p.IsOptional ? "?" : "")}: {p.Type}"));
            return $"{name}({paramList}): {returnType}";
        }

        public string GetConstructorDeclaration(string className, MethodParameter[] parameters)
        {
            var paramList = string.Join(", ", parameters.Select(p => 
                $"{p.Name}{(p.IsOptional ? "?" : "")}: {p.Type}"));
            return $"constructor({paramList})";
        }

        public string GetEnumDeclaration(string enumName)
        {
            return $"export enum {enumName}";
        }

        public string GetEnumMember(string name, string value)
        {
            return $"{name} = {value}";
        }

        public string GetRegionStart(string regionName)
        {
            return $"// #region {regionName}";
        }

        public string GetRegionEnd()
        {
            return "// #endregion";
        }

        public string GetParameterDeclaration(string type, string name, bool isOptional = false)
        {
            return $"{name}{(isOptional ? "?" : "")}: {type}";
        }

        public string GetGenericType(string baseType, params string[] typeParameters)
        {
            return $"{baseType}<{string.Join(", ", typeParameters)}>";
        }

        public string GetAsyncMethodDeclaration(string returnType, string name, string parameters)
        {
            return $"async {name}({parameters}): Promise<{returnType}>";
        }

        public string GetDecorator(string name, string parameters = "")
        {
            return string.IsNullOrEmpty(parameters) 
                ? $"@{name}" 
                : $"@{name}({parameters})";
        }
    }
}
