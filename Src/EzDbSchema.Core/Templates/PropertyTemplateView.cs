using System;
using System.Collections.Generic;
using System.Linq;
using EzDbSchema.Core.Interfaces;
using EzDbSchema.Core.Templates.Languages;
using Newtonsoft.Json;

namespace EzDbSchema.Core.Templates
{
    public class PropertyTemplateView
    {
        private readonly IProperty _property;
        private readonly ILanguageDefinition _language;

        public PropertyTemplateView(IProperty property, string language = "C#")
        {
            _property = property;
            _language = LanguageProvider.GetLanguage(language);
        }

        [JsonProperty("columnName")]
        public string ColumnName => _property.ColumnName;

        [JsonProperty("columnNameCamel")]
        public string ColumnNameCamelCase => HandlebarsHelpers.ToCamelCase(_property.ColumnName);

        [JsonProperty("columnNamePascal")]
        public string ColumnNamePascalCase => HandlebarsHelpers.ToPascalCase(_property.ColumnName);

        [JsonProperty("columnNameSnake")]
        public string ColumnNameSnakeCase => HandlebarsHelpers.ToSnakeCase(_property.ColumnName);

        [JsonProperty("dataType")]
        public string DataType => _property.DataType;

        [JsonProperty("languageType")]
        public string LanguageType => _language.GetDataType(_property.DataType, _property.MaxLength, _property.Precision, _property.Scale);

        [JsonProperty("nullableType")]
        public string NullableType => _language.GetNullableType(LanguageType);

        [JsonProperty("defaultValue")]
        public string DefaultValue => _language.GetDefaultValue(_property.DataType);

        [JsonProperty("propertyDeclaration")]
        public string PropertyDeclaration => _language.GetPropertyDeclaration(
            LanguageType,
            ColumnNamePascalCase,
            _property.IsNullable
        );

        [JsonProperty("formattedDataType")]
        public string FormattedDataType
        {
            get
            {
                if (_property.MaxLength > 0)
                    return $"{_property.DataType}({_property.MaxLength})";
                if (_property.Precision > 0)
                    return $"{_property.DataType}({_property.Precision},{_property.Scale})";
                return _property.DataType;
            }
        }

        [JsonProperty("maxLength")]
        public int MaxLength => _property.MaxLength;

        [JsonProperty("precision")]
        public int Precision => _property.Precision;

        [JsonProperty("scale")]
        public int Scale => _property.Scale;

        [JsonProperty("isNullable")]
        public bool IsNullable => _property.IsNullable;

        [JsonProperty("isPrimaryKey")]
        public bool IsPrimaryKey => _property.IsPrimaryKey;

        [JsonProperty("primaryKeyOrder")]
        public int PrimaryKeyOrder => _property.PrimaryKeyOrder;

        [JsonProperty("isIdentity")]
        public bool IsIdentity => _property.IsIdentity;

        [JsonProperty("fullColumnName")]
        public string FullColumnName => $"{_property.ParentEntity.TableName}.{_property.ColumnName}";

        [JsonProperty("hasRelationships")]
        public bool HasRelationships => _property.RelatedTo?.Count > 0;

        [JsonProperty("relationships")]
        public IEnumerable<RelationshipTemplateView> Relationships =>
            _property.RelatedTo.Select(r => new RelationshipTemplateView(r));

        [JsonProperty("comment")]
        public string Comment => _language.GetComment($"Property {ColumnName} of type {FormattedDataType}");

        [JsonProperty("validations")]
        public IDictionary<string, object> Validations => new Dictionary<string, object>
        {
            { "required", !IsNullable },
            { "maxLength", MaxLength > 0 ? MaxLength : (object)null },
            { "type", DataType },
            { "isPrimaryKey", IsPrimaryKey },
            { "isIdentity", IsIdentity },
            { "precision", Precision > 0 ? Precision : (object)null },
            { "scale", Scale > 0 ? Scale : (object)null }
        };
    }
}
