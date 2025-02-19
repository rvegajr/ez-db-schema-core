using System;
using System.Collections.Generic;
using System.Linq;
using EzDbSchema.Core.Interfaces;
using EzDbSchema.Core.Templates;
using EzDbSchema.Core.Templates.Languages;

namespace EzDbSchema.Core.CodeGen
{
    public class EntityCodeGenInfo
    {
        private readonly IEntity _entity;
        private readonly ILanguageDefinition _language;

        public EntityCodeGenInfo(IEntity entity, string language = "C#")
        {
            _entity = entity;
            _language = LanguageProvider.GetLanguage(language);
        }

        // Additional ORM-specific metadata
        public bool HasCompositeIndex => _entity.Properties.Values.Any(p => p.IndexOrder > 0);
        public bool HasUniqueConstraints => _entity.Properties.Values.Any(p => p.IsUnique);
        public bool HasDefaultValues => _entity.Properties.Values.Any(p => p.HasDefaultValue);
        public bool HasComputedColumns => _entity.Properties.Values.Any(p => p.IsComputed);
        public bool HasConcurrencyToken => _entity.Properties.Values.Any(p => p.IsConcurrencyToken);
        
        // Database-specific features
        public bool HasTriggers => _entity.HasTriggers;
        public bool HasCheckConstraints => _entity.HasCheckConstraints;
        public bool HasForeignKeyConstraints => _entity.HasForeignKeyConstraints;
        public bool HasUniqueIndexes => _entity.HasUniqueIndexes;

        // Advanced relationship metadata
        public bool HasSelfReferencing => _entity.Relationships.Any(r => r.ToTableName == r.FromTableName);
        public bool HasRequiredRelationships => _entity.Relationships.Any(r => !r.IsOptional);
        public bool HasCascadeDelete => _entity.Relationships.Any(r => r.CascadeDelete);
        
        // Security and access patterns
        public bool RequiresAuthorization => _entity.RequiresAuthorization;
        public bool HasRowLevelSecurity => _entity.HasRowLevelSecurity;
        public bool HasDataEncryption => _entity.Properties.Values.Any(p => p.IsEncrypted);
        
        // Performance optimization hints
        public bool ShouldImplementCaching => _entity.Properties.Values.Count > 10 || _entity.IsFrequentlyAccessed;
        public bool RequiresIndexing => _entity.Properties.Values.Any(p => p.RequiresIndex);
        public bool RequiresPagination => _entity.IsLargeDataset;

        // API and service patterns
        public bool SupportsFullTextSearch => _entity.Properties.Values.Any(p => p.SupportsFullText);
        public bool RequiresValidation => _entity.Properties.Values.Any(p => p.HasValidationRules);
        public bool HasAttachments => _entity.Properties.Values.Any(p => p.IsFileReference);
        public bool RequiresAuditTrail => _entity.IsAuditable() || _entity.IsVersioned;

        // Integration patterns
        public bool IsEventSource => _entity.GeneratesEvents;
        public bool RequiresNotification => _entity.RequiresNotification;
        public bool HasExternalReferences => _entity.HasExternalReferences;
        public bool IsPartOfWorkflow => _entity.IsPartOfWorkflow;

        // Basic Names
        public string TableName => _entity.TableName;
        public string SchemaName => _entity.DatabaseSchema;
        public string FullName => $"{SchemaName}.{TableName}";
        
        // Formatted Names
        public string ClassName => HandlebarsHelpers.ToPascalCase(TableName);
        public string ClassNamePlural => HandlebarsHelpers.ToPlural(ClassName);
        public string VariableName => HandlebarsHelpers.ToCamelCase(TableName);
        public string VariableNamePlural => HandlebarsHelpers.ToPlural(VariableName);
        public string ConstantName => HandlebarsHelpers.ToConstantCase(TableName);
        public string RouteParameter => HandlebarsHelpers.ToKebabCase(TableName);

        // Primary Key Info
        public bool HasCompositePrimaryKey => _entity.Properties.Values.Count(p => p.IsPrimaryKey) > 1;
        public IEnumerable<PropertyCodeGenInfo> PrimaryKeys => 
            _entity.Properties.Values
                .Where(p => p.IsPrimaryKey)
                .OrderBy(p => p.PrimaryKeyOrder)
                .Select(p => new PropertyCodeGenInfo(p, _language.Name));

        public string PrimaryKeyType => HasCompositePrimaryKey 
            ? $"{ClassName}Key" 
            : PrimaryKeys.First().LanguageType;

        // Properties
        public IEnumerable<PropertyCodeGenInfo> Properties =>
            _entity.Properties.Values.Select(p => new PropertyCodeGenInfo(p, _language.Name));

        public IEnumerable<PropertyCodeGenInfo> NonKeyProperties =>
            _entity.GetNonKeyProperties().Select(p => new PropertyCodeGenInfo(p, _language.Name));

        public IEnumerable<PropertyCodeGenInfo> RequiredProperties =>
            Properties.Where(p => !p.IsNullable);

        public IEnumerable<PropertyCodeGenInfo> OptionalProperties =>
            Properties.Where(p => p.IsNullable);

        // Relationships
        public IEnumerable<RelationshipInfo> OneToManyRelationships =>
            _entity.GetOneToManyRelationships();

        public IEnumerable<RelationshipInfo> ManyToOneRelationships =>
            _entity.GetManyToOneRelationships();

        // Special Features
        public bool IsAuditable => _entity.IsAuditableEntity();
        public bool IsSoftDeletable => _entity.IsSoftDeletableEntity();
        public bool IsVersioned => _entity.IsVersioned;
        public bool HasCircularReferences => _entity.HasCircularReferences();

        // Validation Info
        public IEnumerable<ValidationRule> ValidationRules => GetValidationRules();

        // API Info
        public string ApiEndpoint => $"/api/{HandlebarsHelpers.ToKebabCase(ClassNamePlural)}";
        public string ApiTag => ClassNamePlural;
        public string ApiDescription => $"Operations for {ClassNamePlural}";

        // Database Info
        public bool IsView => _entity.EntityType.Equals("VIEW", StringComparison.OrdinalIgnoreCase);
        public bool IsTable => _entity.EntityType.Equals("TABLE", StringComparison.OrdinalIgnoreCase);
        public bool IsTemporalTable => _entity.IsTemporalView;

        // Language-specific
        public string ClassDeclaration => _language.GetClassDeclaration(ClassName);
        public string InterfaceDeclaration => _language.GetInterfaceDeclaration($"I{ClassName}");
        public string NamespaceDeclaration => _language.GetNamespace($"{SchemaName}.{ClassName}");

        private IEnumerable<ValidationRule> GetValidationRules()
        {
            var rules = new List<ValidationRule>();

            foreach (var prop in Properties)
            {
                if (!prop.IsNullable)
                    rules.Add(new ValidationRule("required", prop.PropertyName));

                if (prop.MaxLength > 0)
                    rules.Add(new ValidationRule("maxLength", prop.PropertyName, prop.MaxLength.ToString()));

                if (prop.IsNumeric && prop.Precision > 0)
                {
                    rules.Add(new ValidationRule("precision", prop.PropertyName, prop.Precision.ToString()));
                    rules.Add(new ValidationRule("scale", prop.PropertyName, prop.Scale.ToString()));
                }
            }

            return rules;
        }
    }

    public class ValidationRule
    {
        public string Type { get; }
        public string PropertyName { get; }
        public string Value { get; }

        public ValidationRule(string type, string propertyName, string value = null)
        {
            Type = type;
            PropertyName = propertyName;
            Value = value;
        }
    }
}
