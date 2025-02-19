using EzDbSchema.Core.Objects;
using System;
using EzDbSchema.Core.Extentions;
using Newtonsoft.Json;

namespace EzDbSchema.Core.Interfaces
{
    /// <summary>
    /// Represents a property (column) in a database entity with its metadata, validation rules, and UI settings.
    /// </summary>
    public interface IProperty : IEzObject
    {
        // Basic Properties
        /// <summary>
        /// Gets or sets the property name used in code generation.
        /// </summary>
        string PropertyName { get; set; }
        /// <summary>
        /// Gets or sets the physical column name in the database.
        /// </summary>
        string ColumnName { get; set; }
        /// <summary>
        /// Gets or sets an optional alias for the column, useful in queries and views.
        /// </summary>
        string ColumnAlias { get; set; }
        /// <summary>
        /// Gets or sets the database data type of the property.
        /// </summary>
        string DataType { get; set; }
        /// <summary>
        /// Gets or sets whether the property can contain null values.
        /// </summary>
        bool IsNullable { get; set; }
        /// <summary>
        /// Gets or sets the default value for the property.
        /// </summary>
        string DefaultValue { get; set; }
        /// <summary>
        /// Gets or sets the maximum length for string/binary data types.
        /// </summary>
        int MaxLength { get; set; }
        /// <summary>
        /// Gets or sets the precision for numeric data types.
        /// </summary>
        int Precision { get; set; }
        /// <summary>
        /// Gets or sets the scale (decimal places) for numeric data types.
        /// </summary>
        int Scale { get; set; }

        // UI Properties
        /// <summary>
        /// Gets whether the property is read-only and cannot be modified.
        /// </summary>
        bool IsReadOnly { get; }
        /// <summary>
        /// Gets whether the property should be hidden from views and forms.
        /// </summary>
        bool IsHidden { get; }
        /// <summary>
        /// Gets or sets whether the property should be visible in views and forms.
        /// </summary>
        bool IsVisible { get; set; }
        /// <summary>
        /// Gets or sets whether the property can be edited in forms.
        /// </summary>
        bool IsEditable { get; set; }
        /// <summary>
        /// Gets or sets whether the property is required and must have a value.
        /// </summary>
        bool IsRequired { get; set; }
        /// <summary>
        /// Gets or sets the type of input control to use for this property (e.g., text, number, date).
        /// </summary>
        string InputType { get; set; }
        /// <summary>
        /// Gets or sets the friendly name to display in UI for this property.
        /// </summary>
        string DisplayName { get; set; }
        /// <summary>
        /// Gets or sets the placeholder text to show in input fields when empty.
        /// </summary>
        string PlaceholderText { get; set; }
        /// <summary>
        /// Gets or sets the help text or tooltip to show for this property.
        /// </summary>
        string HelpText { get; set; }

        // File Properties
        bool IsFileReference { get; }
        string FileType { get; }

        // Security Properties
        bool IsEncrypted { get; }
        bool RequiresEncryption { get; set; }
        string EncryptionType { get; set; }
        bool RequiresMasking { get; set; }
        string MaskingPattern { get; set; }
        bool RequiresAuthorization { get; set; }
        string AuthorizationRules { get; set; }

        // Database Properties
        bool IsPrimaryKey { get; set; }
        int PrimaryKeyOrder { get; set; }
        bool IsIdentity { get; set; }
        int IndexOrder { get; set; }
        bool IsUnique { get; set; }
        bool HasDefaultValue { get; set; }
        bool IsComputed { get; set; }
        bool IsConcurrencyToken { get; set; }

        // Validation Rules
        bool RequiresValidation { get; set; }
        string ValidationRules { get; set; }
        string ValidationMessage { get; set; }

        // Documentation
        string Description { get; set; }
        string Notes { get; set; }
        string Version { get; set; }

        // External Integration
        bool IsExternalReference { get; set; }
        string ExternalSystem { get; set; }
        string ExternalFormat { get; set; }

        // Performance Features
        bool IsFrequentlyAccessed { get; set; }
        bool RequiresCaching { get; set; }
        string CacheStrategy { get; set; }

        // Change Tracking
        bool TrackChanges { get; set; }
        bool AuditChanges { get; set; }
        string ChangeValidation { get; set; }

        // Additional Properties
        bool RequiresIndex { get; set; }
        bool SupportsFullText { get; set; }
        string ComputedExpression { get; set; }
        bool HasValidationRules => !string.IsNullOrEmpty(ValidationRules);
        string RegexPattern { get; set; }
        string MinValue { get; set; }
        string MaxValue { get; set; }
        bool IsSensitive { get; set; }
        string SearchAnalyzer { get; set; }
        string DisplayFormat { get; set; }
        string InputMask { get; set; }
        string Placeholder { get; set; }
        bool RequiresNotification { get; set; }

        // Relationships
        [AsRef("_id")]
        IEntity ParentEntity { get; set; }

        [JsonProperty]
        [JsonConverter(typeof(WriteOnlyJsonConverter))]
        IRelationshipList RelatedTo { get; set; }
    }
}