using System;
using System.Collections.Generic;
using System.Linq;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Templates
{
    public static class EntityExtensions
    {
        public static IEnumerable<IProperty> GetPropertiesByType(this IEntity entity, string dataType)
        {
            if (entity == null || string.IsNullOrEmpty(dataType))
                return Enumerable.Empty<IProperty>();

            return entity.Properties.Values.Where(p => 
                p.DataType.Equals(dataType, StringComparison.OrdinalIgnoreCase));
        }

        public static IEnumerable<IRelationship> GetRelationshipsByType(this IEntity entity, string relationType)
        {
            if (entity == null || string.IsNullOrEmpty(relationType))
                return Enumerable.Empty<IRelationship>();

            return entity.Relationships.Where(r => 
                r.RelationshipType.Equals(relationType, StringComparison.OrdinalIgnoreCase));
        }

        public static IEnumerable<IProperty> GetNonPrimaryKeyProperties(this IEntity entity)
        {
            if (entity == null)
                return Enumerable.Empty<IProperty>();

            return entity.Properties.Values.Where(p => !p.IsPrimaryKey);
        }

        public static IEnumerable<IProperty> GetPrimaryKeyProperties(this IEntity entity)
        {
            if (entity == null)
                return Enumerable.Empty<IProperty>();

            return entity.Properties.Values
                .Where(p => p.IsPrimaryKey)
                .OrderBy(p => p.PrimaryKeyOrder);
        }

        public static bool HasTimestamps(this IEntity entity)
        {
            if (entity == null)
                return false;

            return entity.Properties.Values.Any(p => 
                p.ColumnName.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase) || 
                p.ColumnName.Equals("UpdatedAt", StringComparison.OrdinalIgnoreCase));
        }

        public static EntityTemplateView AsTemplateView(this IEntity entity)
        {
            return new EntityTemplateView(entity);
        }
    }
}
