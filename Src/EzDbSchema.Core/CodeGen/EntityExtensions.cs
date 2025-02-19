using System;
using System.Collections.Generic;
using System.Linq;
using EzDbSchema.Core.Interfaces;
using EzDbSchema.Core.Templates;

namespace EzDbSchema.Core.CodeGen
{
    public static class EntityExtensions
    {
        public static EntityCodeGenInfo GetCodeGenInfo(this IEntity entity, string language = "C#")
        {
            return new EntityCodeGenInfo(entity, language);
        }

        public static bool HasCompositeKey(this IEntity entity)
        {
            return entity.Properties.Values.Count(p => p.IsPrimaryKey) > 1;
        }

        public static bool HasForeignKeys(this IEntity entity)
        {
            return entity.Relationships.Any(r => r.FromTableName == entity.TableName);
        }

        public static bool HasIncomingRelationships(this IEntity entity)
        {
            return entity.Relationships.Any(r => r.ToTableName == entity.TableName);
        }

        public static IEnumerable<IProperty> GetForeignKeyProperties(this IEntity entity)
        {
            var fkNames = entity.Relationships
                .Where(r => r.FromTableName == entity.TableName)
                .Select(r => r.FromPropertyName);
            return entity.Properties.Values.Where(p => fkNames.Contains(p.ColumnName));
        }

        public static IEnumerable<IProperty> GetNonKeyProperties(this IEntity entity)
        {
            var keyProps = entity.GetForeignKeyProperties().Select(p => p.ColumnName)
                .Concat(entity.Properties.Values.Where(p => p.IsPrimaryKey).Select(p => p.ColumnName));
            return entity.Properties.Values.Where(p => !keyProps.Contains(p.ColumnName));
        }

        public static bool IsAuditableEntity(this IEntity entity)
        {
            if (entity?.Properties?.Values == null) return false;
            var auditColumns = new[] { "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy" };
            return auditColumns.Any(col => 
                entity.Properties.Values.Any(p => p?.ColumnName != null && p.ColumnName.Equals(col, StringComparison.OrdinalIgnoreCase)));
        }

        public static bool IsSoftDeletableEntity(this IEntity entity)
        {
            if (entity?.Properties?.Values == null) return false;
            return entity.Properties.Values.Any(p => 
                p?.ColumnName != null && (p.ColumnName.Equals("IsDeleted", StringComparison.OrdinalIgnoreCase) ||
                p.ColumnName.Equals("DeletedAt", StringComparison.OrdinalIgnoreCase)));
        }

        public static bool IsVersionedEntity(this IEntity entity)
        {
            if (entity?.Properties?.Values == null) return false;
            return entity.Properties.Values.Any(p => 
                p?.ColumnName != null && (p.ColumnName.Equals("Version", StringComparison.OrdinalIgnoreCase) ||
                p.ColumnName.Equals("RowVersion", StringComparison.OrdinalIgnoreCase)));
        }

        public static IEnumerable<RelationshipInfo> GetOneToManyRelationships(this IEntity entity)
        {
            return entity.Relationships
                .Where(r => r.FromTableName == entity.TableName)
                .Select(r => new RelationshipInfo(r, RelationType.OneToMany));
        }

        public static IEnumerable<RelationshipInfo> GetManyToOneRelationships(this IEntity entity)
        {
            return entity.Relationships
                .Where(r => r.ToTableName == entity.TableName)
                .Select(r => new RelationshipInfo(r, RelationType.ManyToOne));
        }

        public static bool HasCircularReferences(this IEntity entity)
        {
            if (entity?.TableName == null || entity.Relationships == null) return false;
            var visited = new HashSet<string>();
            return HasCircularReferencesRecursive(entity, visited);
        }

        private static bool HasCircularReferencesRecursive(IEntity entity, HashSet<string> visited)
        {
            if (entity?.TableName == null || entity.Relationships == null || entity.ParentDatabase == null) return false;
            if (visited.Contains(entity.TableName))
                return true;

            visited.Add(entity.TableName);

            foreach (var relationship in entity.Relationships)
            {
                if (relationship == null || string.IsNullOrEmpty(relationship.FromTableName) || string.IsNullOrEmpty(relationship.ToTableName))
                    continue;

                if (relationship.FromTableName != entity.TableName)
                    continue;

                if (visited.Contains(relationship.ToTableName))
                    return true;

                var relatedEntity = entity.ParentDatabase[relationship.ToTableName];
                if (relatedEntity == null) continue;

                if (HasCircularReferencesRecursive(relatedEntity, visited))
                    return true;
            }

            return false;
        }
    }
}
