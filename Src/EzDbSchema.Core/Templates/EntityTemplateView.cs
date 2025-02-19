using System;
using System.Collections.Generic;
using System.Linq;
using EzDbSchema.Core.Interfaces;
using Newtonsoft.Json;

namespace EzDbSchema.Core.Templates
{
    public class EntityTemplateView
    {
        private readonly IEntity _entity;

        public EntityTemplateView(IEntity entity)
        {
            _entity = entity;
        }

        [JsonProperty("tableName")]
        public string TableName => _entity.TableName;

        [JsonProperty("tableNameCamel")]
        public string TableNameCamelCase => HandlebarsHelpers.ToCamelCase(_entity.TableName);

        [JsonProperty("tableNamePascal")]
        public string TableNamePascalCase => HandlebarsHelpers.ToPascalCase(_entity.TableName);

        [JsonProperty("tableNamePlural")]
        public string TableNamePlural => HandlebarsHelpers.ToPlural(_entity.TableName);

        [JsonProperty("schema")]
        public string DatabaseSchema => _entity.DatabaseSchema;

        [JsonProperty("fullTableName")]
        public string FullTableName => $"{_entity.DatabaseSchema}.{_entity.TableName}";

        [JsonProperty("properties")]
        public IEnumerable<PropertyTemplateView> Properties => 
            _entity.Properties.Values.Select(p => new PropertyTemplateView(p));

        [JsonProperty("relationships")]
        public IEnumerable<RelationshipTemplateView> Relationships =>
            _entity.Relationships.Select(r => new RelationshipTemplateView(r));

        [JsonProperty("primaryKeys")]
        public IEnumerable<PropertyTemplateView> PrimaryKeys =>
            _entity.Properties.Values.Where(p => p.IsPrimaryKey)
                .OrderBy(p => p.PrimaryKeyOrder)
                .Select(p => new PropertyTemplateView(p));

        [JsonProperty("nonPrimaryKeys")]
        public IEnumerable<PropertyTemplateView> NonPrimaryKeys =>
            _entity.Properties.Values.Where(p => !p.IsPrimaryKey)
                .Select(p => new PropertyTemplateView(p));

        [JsonProperty("hasTimestamps")]
        public bool HasTimestamps => _entity.Properties.Values.Any(p => 
            p.ColumnName.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase) || 
            p.ColumnName.Equals("UpdatedAt", StringComparison.OrdinalIgnoreCase));

        [JsonProperty("hasRelationships")]
        public bool HasRelationships => _entity.Relationships?.Count > 0;

        [JsonProperty("hasProperties")]
        public bool HasProperties => _entity.Properties?.Count > 0;

        [JsonProperty("entityType")]
        public string EntityType => _entity.EntityType;

        [JsonProperty("isTable")]
        public bool IsTable => _entity.EntityType.Equals("TABLE", StringComparison.OrdinalIgnoreCase);

        [JsonProperty("isView")]
        public bool IsView => _entity.EntityType.Equals("VIEW", StringComparison.OrdinalIgnoreCase);
    }
}
