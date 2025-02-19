using System;
using EzDbSchema.Core.Interfaces;
using Newtonsoft.Json;

namespace EzDbSchema.Core.Templates
{
    public class RelationshipTemplateView
    {
        private readonly IRelationship _relationship;

        public RelationshipTemplateView(IRelationship relationship)
        {
            _relationship = relationship;
        }

        [JsonProperty("constraintName")]
        public string ConstraintName => _relationship.ConstraintName;

        [JsonProperty("fromTableName")]
        public string FromTableName => _relationship.FromTableName;

        [JsonProperty("fromPropertyName")]
        public string FromPropertyName => _relationship.FromPropertyName;

        [JsonProperty("fromColumnName")]
        public string FromColumnName => _relationship.FromColumnName;

        [JsonProperty("toTableName")]
        public string ToTableName => _relationship.ToTableName;

        [JsonProperty("toPropertyName")]
        public string ToPropertyName => _relationship.ToPropertyName;

        [JsonProperty("toColumnName")]
        public string ToColumnName => _relationship.ToColumnName;

        [JsonProperty("relationshipType")]
        public string RelationshipType => _relationship.RelationshipType;

        [JsonProperty("fromFullName")]
        public string FromFullName => $"{FromTableName}.{FromPropertyName}";

        [JsonProperty("toFullName")]
        public string ToFullName => $"{ToTableName}.{ToPropertyName}";

        [JsonProperty("isOneToOne")]
        public bool IsOneToOne => _relationship.RelationshipType.Equals("One to One", StringComparison.OrdinalIgnoreCase);

        [JsonProperty("isOneToMany")]
        public bool IsOneToMany => _relationship.RelationshipType.Equals("One to Many", StringComparison.OrdinalIgnoreCase);

        [JsonProperty("isManyToOne")]
        public bool IsManyToOne => _relationship.RelationshipType.Equals("Many to One", StringComparison.OrdinalIgnoreCase);

        [JsonProperty("isManyToMany")]
        public bool IsManyToMany => _relationship.RelationshipType.Equals("Many to Many", StringComparison.OrdinalIgnoreCase);
    }
}
