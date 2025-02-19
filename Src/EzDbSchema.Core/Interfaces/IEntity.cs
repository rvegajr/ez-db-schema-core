using System;
namespace EzDbSchema.Core.Interfaces
{
    /// <summary>
    /// Represents a database entity (table or view) with its properties, relationships, and metadata.
    /// </summary>
    public interface IEntity : IEzObject
    {
        // Basic Properties
        /// <summary>
        /// Gets or sets the parent database that contains this entity.
        /// </summary>
        IDatabase ParentDatabase { get; set; }
        /// <summary>
        /// Gets or sets the physical table name in the database.
        /// </summary>
        string TableName { get; set; }
        /// <summary>
        /// Gets or sets an optional alias for the table, useful in queries and code generation.
        /// </summary>
        string TableAlias { get; set; }
        /// <summary>
        /// Gets or sets the current state of the entity (e.g., Active, Deleted, etc.).
        /// </summary>
        string EntityState { get; set; }
        /// <summary>
        /// Gets or sets the database schema name (e.g., dbo, public) that contains this entity.
        /// </summary>
        string DatabaseSchema { get; set; }
        /// <summary>
        /// Gets or sets the temporal type of the entity if it supports temporal features.
        /// </summary>
        string TemporalType { get; set; }
        /// <summary>
        /// Gets or sets the type of the entity (e.g., Table, View, etc.).
        /// </summary>
        string EntityType { get; set; }
        /// <summary>
        /// Gets or sets whether this entity is a temporal view.
        /// </summary>
        bool IsTemporalView { get; set; }

        // Database Features
        /// <summary>
        /// Gets whether this entity has any database triggers defined.
        /// </summary>
        bool HasTriggers { get; }
        /// <summary>
        /// Gets whether this entity has any check constraints defined.
        /// </summary>
        bool HasCheckConstraints { get; }
        /// <summary>
        /// Gets whether this entity has any foreign key constraints defined.
        /// </summary>
        bool HasForeignKeyConstraints { get; }
        /// <summary>
        /// Gets whether this entity has any unique indexes defined.
        /// </summary>
        bool HasUniqueIndexes { get; }
        /// <summary>
        /// Determines whether this entity has any primary keys defined.
        /// </summary>
        /// <returns>True if the entity has one or more primary keys; otherwise, false.</returns>
        bool HasPrimaryKeys();

        // Security Features
        /// <summary>
        /// Gets whether this entity requires authorization for access.
        /// </summary>
        bool RequiresAuthorization { get; }
        /// <summary>
        /// Gets whether this entity implements row-level security.
        /// </summary>
        bool HasRowLevelSecurity { get; }
        /// <summary>
        /// Determines whether this entity supports auditing features.
        /// </summary>
        /// <returns>True if the entity has auditing capabilities; otherwise, false.</returns>
        bool IsAuditable();
        /// <summary>
        /// Checks if a given property name is an auditable property.
        /// </summary>
        /// <param name="propertyNameToCheck">The name of the property to check.</param>
        /// <returns>True if the property is auditable; otherwise, false.</returns>
        bool isAuditablePropertyName(string propertyNameToCheck);

        // Performance Features
        bool IsFrequentlyAccessed { get; }
        bool RequiresCaching { get; }
        string CacheStrategy { get; }
        bool IsLargeDataset { get; }

        // Advanced Features
        bool IsVersioned { get; }
        bool GeneratesEvents { get; }
        bool RequiresNotification { get; }
        bool HasExternalReferences { get; }
        bool IsPartOfWorkflow { get; }

        // Collections
        IPropertyDictionary Properties { get; set; }
        IRelationshipReferenceList Relationships { get; set; }
        IRelationshipGroups RelationshipGroups { get; set; }
        IPrimaryKeyProperties PrimaryKeys { get; set; }
    }
}
