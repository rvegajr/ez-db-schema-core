using EzDbSchema.Core.Interfaces;
using EzDbSchema.Core.Objects;
using System;
using System.Linq;
using System.Collections.Generic;

namespace EzDbSchema.Core.Extentions
{
    /// <summary>
    /// Extension methods for database operations
    /// </summary>
    public static class DatabaseExtensions
    {
        /// <summary>
        /// Rebuilds all relationship pointers in the database
        /// </summary>
        /// <param name="database">The database to rebuild relationship pointers for</param>
        /// <param name="verbose">Whether to output verbose information</param>
        /// <returns>The database with rebuilt relationship pointers</returns>
        public static IDatabase RebuildAllRelationshipPointers(this IDatabase database, bool verbose = false)
        {
            if (database == null)
                return null;

            if (verbose)
                Console.WriteLine($"Rebuilding relationship pointers for database {database.Name}");

            // Rebuild entity references in properties
            foreach (var entity in database.Entities.Values)
            {
                foreach (var property in entity.Properties.Values)
                {
                    property.ParentEntity = entity;
                }
            }

            // Rebuild relationship pointers
            foreach (var entity in database.Entities.Values)
            {
                if (entity.RelationshipGroups != null)
                {
                    foreach (var relationshipGroup in entity.RelationshipGroups)
                    {
                        var groupKey = relationshipGroup.Key;
                        var relationshipList = relationshipGroup.Value;
                        
                        // Set database reference on the relationship group
                        if (relationshipGroup.Value is IRelationshipGroup rg)
                        {
                            rg.Database = database;
                        }
                        
                        // Set database reference on the relationship list
                        relationshipList.Database = database;
                        
                        // Process each relationship in the list
                        for (int i = 0; i < relationshipList.Count; i++)
                        {
                            var relationship = relationshipList[i];
                            
                            // Set parent entity
                            relationship.ParentEntity = entity;

                            // Set from entity and property
                            if (database.Entities.TryGetValue(relationship.FromTableName, out var fromEntity))
                            {
                                relationship.FromEntity = fromEntity;
                                if (fromEntity.Properties.TryGetValue(relationship.FromColumnName, out var fromProperty))
                                {
                                    relationship.FromProperty = fromProperty;
                                }
                            }

                            // Set to entity and property
                            if (database.Entities.TryGetValue(relationship.ToTableName, out var toEntity))
                            {
                                relationship.ToEntity = toEntity;
                                if (toEntity.Properties.TryGetValue(relationship.ToColumnName, out var toProperty))
                                {
                                    relationship.ToProperty = toProperty;
                                }
                            }
                        }
                    }
                }
            }

            if (verbose)
                Console.WriteLine($"Finished rebuilding relationship pointers for database {database.Name}");

            return database;
        }
    }
}
