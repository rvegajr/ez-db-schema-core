using EzDbSchema.Core.Enums;
using System;
using System.Collections.Generic;
namespace EzDbSchema.Core.Interfaces
{
    /// <summary>
    /// Represents a relationship between database entities, including foreign key constraints and performance settings.
    /// </summary>
    public interface IRelationship : IEzObject
    {
        // Basic Properties
        /// <summary>
        /// Gets or sets the name of the database constraint that defines this relationship.
        /// </summary>
        string ConstraintName { get; set; }
        /// <summary>
        /// Gets or sets the name of the table that contains the foreign key.
        /// </summary>
        string FromTableName { get; set; }
        /// <summary>
        /// Gets or sets the property name in the source entity that represents this relationship.
        /// </summary>
        string FromPropertyName { get; set; }
        /// <summary>
        /// Gets or sets the name of the referenced table.
        /// </summary>
        string ToTableName { get; set; }
        /// <summary>
        /// Gets or sets the property name in the target entity that represents this relationship.
        /// </summary>
        string ToPropertyName { get; set; }
        /// <summary>
        /// Gets or sets the name of the foreign key column in the source table.
        /// </summary>
        string FromColumnName { get; set; }
        /// <summary>
        /// Gets or sets the name of the referenced column in the target table.
        /// </summary>
        string ToColumnName { get; set; }
        /// <summary>
        /// Gets or sets the parent entity that owns this relationship.
        /// </summary>
        IEntity ParentEntity { get; set; }
        /// <summary>
        /// Gets or sets the name of the primary table in the relationship.
        /// </summary>
        string PrimaryTableName { get; set; }
        /// <summary>
        /// Gets or sets the type of relationship (e.g., OneToOne, OneToMany, ManyToMany).
        /// </summary>
        string RelationshipType { get; set; }
        /// <summary>
        /// Gets or sets the multiplicity type of the relationship.
        /// </summary>
        RelationshipMultiplicityType MultiplicityType { get; set; }

        // Entity and Property References
        /// <summary>
        /// Gets or sets the entity that contains the foreign key.
        /// </summary>
        IEntity FromEntity { get; set; }

        /// <summary>
        /// Gets or sets the property that contains the foreign key.
        /// </summary>
        IProperty FromProperty { get; set; }

        /// <summary>
        /// Gets or sets the referenced entity.
        /// </summary>
        IEntity ToEntity { get; set; }

        /// <summary>
        /// Gets or sets the referenced property.
        /// </summary>
        IProperty ToProperty { get; set; }

        // Advanced Relationship Features
        /// <summary>
        /// Gets or sets whether this relationship is optional (allows null foreign keys).
        /// </summary>
        bool IsOptional { get; set; }
        /// <summary>
        /// Gets or sets whether deletes should cascade through this relationship.
        /// </summary>
        bool CascadeDelete { get; set; }
        /// <summary>
        /// Gets or sets the cascade action to perform (e.g., Cascade, SetNull, NoAction).
        /// </summary>
        string CascadeAction { get; set; }

        // Performance Features
        /// <summary>
        /// Gets or sets whether this relationship should be lazy loaded to improve performance.
        /// </summary>
        bool RequiresLazyLoading { get; set; }
        /// <summary>
        /// Gets or sets whether this relationship should be eager loaded to improve performance.
        /// </summary>
        bool RequiresEagerLoading { get; set; }
        /// <summary>
        /// Gets or sets whether this relationship requires an index for better query performance.
        /// </summary>
        bool RequiresIndexing { get; set; }

        // Validation Features
        bool HasConstraints { get; set; }
        IEnumerable<string> Constraints { get; set; }
        bool RequiresReferentialIntegrity { get; set; }

        // Business Logic Hints
        bool IsOwnership { get; set; }
        bool IsAggregation { get; set; }
        bool IsComposition { get; set; }
        string BusinessRole { get; set; }

        // API and Integration Features
        bool IncludeInDefaultFetch { get; set; }
        bool RequiresAuthorization { get; set; }
        bool GeneratesEvents { get; set; }

        // Documentation
        string Description { get; set; }
        string Notes { get; set; }
        string Version { get; set; }

        // Change Tracking
        bool TrackChanges { get; set; }
        bool AuditChanges { get; set; }
        string ChangeValidation { get; set; }

        // Composite Key Support
        /// <summary>
        /// Gets or sets the collection of source properties for composite key relationships.
        /// </summary>
        IList<IProperty> FromProperties { get; set; }

        /// <summary>
        /// Gets or sets the collection of target properties for composite key relationships.
        /// </summary>
        IList<IProperty> ToProperties { get; set; }
    }

}
