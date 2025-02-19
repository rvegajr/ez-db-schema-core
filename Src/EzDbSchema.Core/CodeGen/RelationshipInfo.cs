using System;
using System.Collections.Generic;
using EzDbSchema.Core.Interfaces;
using EzDbSchema.Core.Templates;

namespace EzDbSchema.Core.CodeGen
{
    public enum RelationType
    {
        OneToOne,
        OneToMany,
        ManyToOne,
        ManyToMany
    }

    public class RelationshipInfo
    {
        public string VariableName => $"{FromClassName}_{FromPropertyClassName}_{ToClassName}_{ToPropertyClassName}";

        private readonly IRelationship _relationship;
        private readonly RelationType _type;

        public RelationshipInfo(IRelationship relationship, RelationType type)
        {
            _relationship = relationship;
            _type = type;
        }

        // Advanced Relationship Features
        public bool IsOptional => _relationship.IsOptional;
        public bool CascadeDelete => _relationship.CascadeDelete;
        public string CascadeAction => _relationship.CascadeAction;
        public bool IsSelfReferencing => _relationship.FromTableName == _relationship.ToTableName;
        
        // Performance Features
        public bool RequiresLazyLoading => _relationship.RequiresLazyLoading;
        public bool RequiresEagerLoading => _relationship.RequiresEagerLoading;
        public bool RequiresIndexing => _relationship.RequiresIndexing;
        
        // Validation Features
        public bool HasConstraints => _relationship.HasConstraints;
        public IEnumerable<string> Constraints => _relationship.Constraints;
        public bool RequiresReferentialIntegrity => _relationship.RequiresReferentialIntegrity;
        
        // Business Logic Hints
        public bool IsOwnership => _relationship.IsOwnership;
        public bool IsAggregation => _relationship.IsAggregation;
        public bool IsComposition => _relationship.IsComposition;
        public string BusinessRole => _relationship.BusinessRole;
        
        // API and Integration Features
        public bool IncludeInDefaultFetch => _relationship.IncludeInDefaultFetch;
        public bool RequiresAuthorization => _relationship.RequiresAuthorization;
        public bool GeneratesEvents => _relationship.GeneratesEvents;
        
        // Documentation
        public string Description => _relationship.Description;
        public string Notes => _relationship.Notes;
        public string Version => _relationship.Version;
        
        // Change Tracking
        public bool TrackChanges => _relationship.TrackChanges;
        public bool AuditChanges => _relationship.AuditChanges;
        public string ChangeValidation => _relationship.ChangeValidation;

        // Basic Info
        public string ConstraintName => _relationship.ConstraintName;
        public RelationType Type => _type;

        // From (Source) Entity Info
        public string FromTableName => _relationship.FromTableName;
        public string FromPropertyName => _relationship.FromPropertyName;
        public string FromClassName => HandlebarsHelpers.ToPascalCase(FromTableName);
        public string FromPropertyClassName => HandlebarsHelpers.ToPascalCase(FromPropertyName);

        // To (Target) Entity Info
        public string ToTableName => _relationship.ToTableName;
        public string ToPropertyName => _relationship.ToPropertyName;
        public string ToClassName => HandlebarsHelpers.ToPascalCase(ToTableName);
        public string ToPropertyClassName => HandlebarsHelpers.ToPascalCase(ToPropertyName);

        // Navigation Property Names
        public string NavigationPropertyName => Type switch
        {
            RelationType.OneToOne => ToClassName,
            RelationType.OneToMany => HandlebarsHelpers.ToPlural(ToClassName),
            RelationType.ManyToOne => ToClassName,
            RelationType.ManyToMany => HandlebarsHelpers.ToPlural(ToClassName),
            _ => ToClassName
        };

        public string InverseNavigationPropertyName => Type switch
        {
            RelationType.OneToOne => FromClassName,
            RelationType.OneToMany => FromClassName,
            RelationType.ManyToOne => HandlebarsHelpers.ToPlural(FromClassName),
            RelationType.ManyToMany => HandlebarsHelpers.ToPlural(FromClassName),
            _ => FromClassName
        };

        // Type Checks
        public bool IsOneToOne => Type == RelationType.OneToOne;
        public bool IsOneToMany => Type == RelationType.OneToMany;
        public bool IsManyToOne => Type == RelationType.ManyToOne;
        public bool IsManyToMany => Type == RelationType.ManyToMany;

        // Collection Type Helpers
        public bool IsCollection => IsOneToMany || IsManyToMany;
        public string CollectionType => "ICollection<>";

        // Foreign Key Info
        public string ForeignKeyPropertyName => HandlebarsHelpers.ToPascalCase(FromPropertyName);
        public string InverseForeignKeyPropertyName => HandlebarsHelpers.ToPascalCase(ToPropertyName);

        
        // Cascade Options
        public bool ShouldCascadeOnDelete => IsOneToMany || IsManyToOne;
        public string CascadeOption => ShouldCascadeOnDelete ? "Cascade" : "NoAction";

        // Relationship Configuration
        public bool RequiresJoinTable => IsManyToMany;
        public string JoinTableName => IsManyToMany 
            ? $"{FromTableName}To{ToTableName}" 
            : null;

        // Validation
        public bool IsRequired => !IsCollection;
    }
}
