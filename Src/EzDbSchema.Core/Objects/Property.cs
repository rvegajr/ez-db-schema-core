using System;
using System.Reflection;
using System.Text;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Interfaces;
using Newtonsoft.Json;

namespace EzDbSchema.Core.Objects
{
	public class Property : EzObject, IProperty
    {
        // Basic Properties
        public string PropertyName { get; set; }
        public string ColumnName { get; set; }
        public string ColumnAlias { get; set; }
        public string DataType { get; set; }
        public bool IsNullable { get; set; }
        public string DefaultValue { get; set; }
        public int MaxLength { get; set; }
        public int Precision { get; set; }
        public int Scale { get; set; }

        // Indexing and Uniqueness
        public int IndexOrder { get; set; }
        public bool IsUnique { get; set; }
        public bool HasDefaultValue { get; set; }
        public bool IsComputed { get; set; }
        public bool IsConcurrencyToken { get; set; }

        // Validation Rules
        public bool RequiresValidation { get; set; }
        public string ValidationRules { get; set; }
        public string ValidationMessage { get; set; }

        // Security Features
        public bool RequiresEncryption { get; set; }
        public string EncryptionType { get; set; }
        public bool RequiresMasking { get; set; }
        public string MaskingPattern { get; set; }
        public bool RequiresAuthorization { get; set; }
        public string AuthorizationRules { get; set; }

        // Documentation
        public string Description { get; set; }
        public string Notes { get; set; }
        public string Version { get; set; }

        // UI Hints
        public string DisplayName { get; set; }
        public string PlaceholderText { get; set; }
        public string HelpText { get; set; }
        public string InputType { get; set; }
        public bool IsVisible { get; set; }
        public bool IsEditable { get; set; }
        public bool IsRequired { get; set; }

        // External Integration
        public bool IsExternalReference { get; set; }
        public string ExternalSystem { get; set; }
        public string ExternalFormat { get; set; }

        // Performance Features
        public bool IsFrequentlyAccessed { get; set; }
        public bool RequiresCaching { get; set; }
        public string CacheStrategy { get; set; }

        // Change Tracking
        public bool TrackChanges { get; set; }
        public bool AuditChanges { get; set; }
        public string ChangeValidation { get; set; }

        internal static string ALIAS = "Property";

        public Property() : base()
        {
        }

        // UI Properties
        public bool IsReadOnly => !IsEditable;
        public bool IsHidden => !IsVisible;

        // File Properties
        public bool IsFileReference => DataType?.ToLower() == "file" || DataType?.ToLower() == "binary";
        public string FileType => IsFileReference ? ExternalFormat : null;

        // Security Properties
        public bool IsEncrypted => RequiresEncryption;

        // Additional Properties
        public bool RequiresIndex { get; set; }
        public bool SupportsFullText { get; set; }
        public string ComputedExpression { get; set; }
        public string RegexPattern { get; set; }
        public string MinValue { get; set; }
        public string MaxValue { get; set; }
        public bool IsSensitive { get; set; }
        public string SearchAnalyzer { get; set; }
        public string DisplayFormat { get; set; }
        public string InputMask { get; set; }
        public string Placeholder { get; set; }
        public bool RequiresNotification { get; set; }
        // Additional Database Properties
        public bool IsPrimaryKey { get; set; }
        public int PrimaryKeyOrder { get; set; }
        public bool IsIdentity { get; set; }

        // Relationships
        [AsRef("_id")]
        [JsonIgnore]
        public IEntity ParentEntity { get; set; }

        public override string DatabaseObjectName { get => ParentEntity.DatabaseSchema + "." + ParentEntity.TableName + "." + ColumnName; }

        [JsonProperty]
        [JsonConverter(typeof(WriteOnlyJsonConverter))]
		public IRelationshipList RelatedTo { get; set; } = new RelationshipList();
    }
}
