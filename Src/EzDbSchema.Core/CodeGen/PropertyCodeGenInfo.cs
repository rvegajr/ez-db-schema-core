using System;
using System.Collections.Generic;
using System.Linq;
using EzDbSchema.Core.Interfaces;
using EzDbSchema.Core.Templates;
using EzDbSchema.Core.Templates.Languages;

namespace EzDbSchema.Core.CodeGen
{
    public class PropertyCodeGenInfo
    {
        private readonly IProperty _property;
        private readonly ILanguageDefinition _language;

        public PropertyCodeGenInfo(IProperty property, string language = "C#")
        {
            _property = property;
            _language = LanguageProvider.GetLanguage(language);
        }

        // Advanced Database Features
        public bool IsComputed => _property.IsComputed;
        public string ComputedExpression => _property.ComputedExpression;
        public bool IsUnique => _property.IsUnique;
        public bool HasDefaultValue => _property.HasDefaultValue;
        public string DefaultValue => _property.DefaultValue;
        public bool IsConcurrencyToken => _property.IsConcurrencyToken;
        public int IndexOrder => _property.IndexOrder;
        
        // Validation and Constraints
        public bool HasValidationRules => _property.HasValidationRules;
        public IEnumerable<string> ValidationRules => _property.ValidationRules?.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries) ?? Enumerable.Empty<string>();
        public string RegexPattern => _property.RegexPattern;
        public string MinValue => _property.MinValue;
        public string MaxValue => _property.MaxValue;
        
        // Security Features
        public bool IsEncrypted => _property.IsEncrypted;
        public string EncryptionType => _property.EncryptionType;
        public bool IsSensitive => _property.IsSensitive;
        public bool RequiresMasking => _property.RequiresMasking;
        
        // Search and Index Features
        public bool RequiresIndex => _property.RequiresIndex;
        public bool SupportsFullText => _property.SupportsFullText;
        public string SearchAnalyzer => _property.SearchAnalyzer;
        
        // UI/UX Hints
        public string DisplayFormat => _property.DisplayFormat;
        public string InputMask => _property.InputMask;
        public string Placeholder => _property.Placeholder;
        public bool IsReadOnly => _property.IsReadOnly;
        public bool IsHidden => _property.IsHidden;
        
        // Integration Features
        public bool IsFileReference => _property.IsFileReference;
        public string FileType => _property.FileType;
        public bool IsExternalReference => _property.IsExternalReference;
        public string ExternalSystem => _property.ExternalSystem;
        
        // Performance Hints
        public bool IsFrequentlyAccessed => _property.IsFrequentlyAccessed;
        public bool RequiresCaching => _property.RequiresCaching;
        public string CacheStrategy => _property.CacheStrategy;

        // Basic Info
        public string ColumnName => _property.ColumnName;
        public string DataType => _property.DataType;
        public bool IsNullable => _property.IsNullable;
        public bool IsPrimaryKey => _property.IsPrimaryKey;
        public int PrimaryKeyOrder => _property.PrimaryKeyOrder;
        public bool IsIdentity => _property.IsIdentity;
        public int MaxLength => _property.MaxLength;
        public int Precision => _property.Precision;
        public int Scale => _property.Scale;

        // Formatted Names
        public string PropertyName => HandlebarsHelpers.ToPascalCase(ColumnName);
        public string VariableName => HandlebarsHelpers.ToCamelCase(ColumnName);
        public string ConstantName => HandlebarsHelpers.ToConstantCase(ColumnName);
        public string ParameterName => HandlebarsHelpers.ToCamelCase(ColumnName);

        // Language-specific Types
        public string LanguageType => _language.GetDataType(DataType, MaxLength, Precision, Scale);
        public string NullableType => IsNullable ? _language.GetNullableType(LanguageType) : LanguageType;

        // Property Declaration
        public string Declaration => _language.GetPropertyDeclaration(LanguageType, PropertyName, IsNullable);
        public string BackingField => _language.Name switch
        {
            "C#" => $"private {NullableType} _{VariableName};",
            "TypeScript" => $"private _{VariableName}: {NullableType};",
            "Java" => $"private {NullableType} {VariableName};",
            _ => ""
        };

        // Documentation
        public string Comment => _language.GetComment(GetPropertyDescription());
        public string ValidationAttributes => GetValidationAttributes();

        // Type Information
        public bool IsNumeric => IsNumericType(DataType);
        public bool IsText => IsTextType(DataType);
        public bool IsDateTime => IsDateTimeType(DataType);
        public bool IsBoolean => IsBooleanType(DataType);
        public bool IsBinary => IsBinaryType(DataType);
        public bool IsEnum => false; // Could be enhanced with actual enum detection

        private string GetPropertyDescription()
        {
            var desc = $"{PropertyName} - {DataType}";
            if (MaxLength > 0) desc += $" (max length: {MaxLength})";
            if (Precision > 0) desc += $" (precision: {Precision}, scale: {Scale})";
            if (IsPrimaryKey) desc += " - Primary Key";
            if (IsIdentity) desc += " - Auto Increment";
            if (!IsNullable) desc += " - Required";
            return desc;
        }

        private string GetValidationAttributes()
        {
            var attrs = new System.Text.StringBuilder();

            switch (_language.Name)
            {
                case "C#":
                    if (!IsNullable) attrs.AppendLine("[Required]");
                    if (MaxLength > 0) attrs.AppendLine($"[MaxLength({MaxLength})]");
                    if (IsNumeric && Precision > 0)
                        attrs.AppendLine($"[Range(-{Math.Pow(10, Precision - Scale)}, {Math.Pow(10, Precision - Scale)})]");
                    break;

                case "TypeScript":
                    if (!IsNullable) attrs.AppendLine("@IsNotEmpty()");
                    if (MaxLength > 0) attrs.AppendLine($"@MaxLength({MaxLength})");
                    break;

                case "Java":
                    if (!IsNullable) attrs.AppendLine("@NotNull");
                    if (MaxLength > 0) attrs.AppendLine($"@Size(max = {MaxLength})");
                    break;
            }

            return attrs.ToString().TrimEnd();
        }

        private static bool IsNumericType(string dataType) =>
            dataType.ToLowerInvariant() switch
            {
                "int" or "bigint" or "smallint" or "tinyint" or "decimal" or
                "numeric" or "money" or "smallmoney" or "float" or "real" => true,
                _ => false
            };

        private static bool IsTextType(string dataType) =>
            dataType.ToLowerInvariant() switch
            {
                "char" or "nchar" or "varchar" or "nvarchar" or
                "text" or "ntext" => true,
                _ => false
            };

        private static bool IsDateTimeType(string dataType) =>
            dataType.ToLowerInvariant() switch
            {
                "datetime" or "datetime2" or "smalldatetime" or
                "date" or "time" or "timestamp" => true,
                _ => false
            };

        private static bool IsBooleanType(string dataType) =>
            dataType.ToLowerInvariant() switch
            {
                "bit" or "boolean" => true,
                _ => false
            };

        private static bool IsBinaryType(string dataType) =>
            dataType.ToLowerInvariant() switch
            {
                "binary" or "varbinary" or "image" => true,
                _ => false
            };
    }
}
