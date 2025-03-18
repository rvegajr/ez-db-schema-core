using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using EzDbSchema.Core.Enums;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Interfaces;
using Newtonsoft.Json;

namespace EzDbSchema.Core.Objects
{
	public class Relationship : EzObject, IRelationship
    {
        internal static string ALIAS = "Relationship";

        public Relationship() : base()
        {
            FromProperties = new List<IProperty>();
            ToProperties = new List<IProperty>();
        }
        // Basic Properties
        public string ConstraintName { get; set; }
        public string FromTableName { get; set; }
        public string FromPropertyName { get; set; }
        public string FromColumnName { get; set; }
        public string ToTableName { get; set; }
        public string ToPropertyName { get; set; }
        public string ToColumnName { get; set; }
        public string PrimaryTableName { get; set; }
        public string RelationshipType { get; set; }
        public RelationshipMultiplicityType MultiplicityType { get; set; } = RelationshipMultiplicityType.Unknown;

        // Entity References
        [JsonIgnore]
        public IEntity FromEntity { get; set; }
        [JsonIgnore]
        public IProperty FromProperty { get; set; }
        [JsonIgnore]
        public IEntity ToEntity { get; set; }
        [JsonIgnore]
        public IProperty ToProperty { get; set; }
        [AsRef("_id")]
        [JsonIgnore]
        public IEntity ParentEntity { get; set; }

        // Additional Properties
        public int ForeignKeyOrdinalPosition { get; set; } = 0;

        // Advanced Relationship Features
        public bool IsOptional { get; set; }
        public bool CascadeDelete { get; set; }
        public string CascadeAction { get; set; }

        // Performance Features
        public bool RequiresLazyLoading { get; set; }
        public bool RequiresEagerLoading { get; set; }
        public bool RequiresIndexing { get; set; }

        // Validation Features
        public bool HasConstraints { get; set; }
        public IEnumerable<string> Constraints { get; set; } = new List<string>();
        public bool RequiresReferentialIntegrity { get; set; }

        // Business Logic Hints
        public bool IsOwnership { get; set; }
        public bool IsAggregation { get; set; }
        public bool IsComposition { get; set; }
        public string BusinessRole { get; set; }

        // API and Integration Features
        public bool IncludeInDefaultFetch { get; set; }
        public bool RequiresAuthorization { get; set; }
        public bool GeneratesEvents { get; set; }

        // Documentation
        public string Description { get; set; }
        public string Notes { get; set; }
        public string Version { get; set; }

        // Change Tracking
        public bool TrackChanges { get; set; }
        public bool AuditChanges { get; set; }
        public string ChangeValidation { get; set; }

        // Composite Key Support
        public IList<IProperty> FromProperties { get; set; }
        public IList<IProperty> ToProperties { get; set; }
    }


}
