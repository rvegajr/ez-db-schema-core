using System;
using System.Collections.Generic;
using System.Linq;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Templates
{
    public static class DatabaseExtensions
    {
        public static DatabaseTemplateContext AsTemplateContext(this IDatabase database)
        {
            return new DatabaseTemplateContext(database);
        }

        public static IEnumerable<IEntity> GetTables(this IDatabase database)
        {
            if (database == null)
                return Enumerable.Empty<IEntity>();

            return database.Values.Where(e => 
                string.Equals(e.EntityType, "TABLE", StringComparison.OrdinalIgnoreCase));
        }

        public static IEnumerable<IEntity> GetViews(this IDatabase database)
        {
            if (database == null)
                return Enumerable.Empty<IEntity>();

            return database.Values.Where(e => 
                string.Equals(e.EntityType, "VIEW", StringComparison.OrdinalIgnoreCase));
        }

        public static IEnumerable<IEntity> GetEntitiesBySchema(this IDatabase database, string schema)
        {
            if (database == null || string.IsNullOrEmpty(schema))
                return Enumerable.Empty<IEntity>();

            return database.Values.Where(e => 
                string.Equals(e.DatabaseSchema, schema, StringComparison.OrdinalIgnoreCase));
        }

        public static IDictionary<string, EntityTemplateView> GetTemplateViews(this IDatabase database)
        {
            if (database == null)
                return new Dictionary<string, EntityTemplateView>();

            return database.ToDictionary(
                kvp => kvp.Key,
                kvp => new EntityTemplateView(kvp.Value)
            );
        }
    }
}
