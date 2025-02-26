using EzDbSchema.Core.Enums;
using EzDbSchema.Core.Interfaces;
using EzDbSchema.Core.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EzDbSchema.Core.Extentions
{
    /// <summary>
    /// Extension methods for working with relationships
    /// </summary>
    public static class RelationshipExtensions2
    {
        /// <summary>
        /// Starting from a relationship, function will figure out what the name of the object should be for a foreign key, taking into account the potential names 
        /// </summary>
        /// <param name="entity">Entity so we know the perspective of the relationship</param>
        /// <param name="fkName">String that is the name of the foreign key we want to generate</param>
        /// <param name="generatedFrom">Source from which to generate the object name</param>
        /// <returns>The generated object name</returns>
        public static string GenerateObjectName(this IEntity entity, string fkName, ObjectNameGeneratedFrom generatedFrom)
        {
            var procName = $"RelationshipExtensions2.GenerateObjectName(entity='{entity?.TableName}', fkName='{fkName}')";

            string fieldName = "";
            try
            {
                if (entity?.RelationshipGroups == null || !entity.RelationshipGroups.ContainsKey(fkName))
                {
                    return fieldName;
                }

                var relationship = entity.RelationshipGroups[fkName];
                var relSummary = relationship.AsSummary();
                var entityName = $"{entity.DatabaseSchema}.{entity.TableName}";

                // Count how many relationships point to the same table
                int sameTableCount = 0;
                foreach (var rg in entity.RelationshipGroups.Values)
                {
                    var summary = rg.AsSummary();
                    if (summary.ToTableName.Equals(relSummary.ToTableName))
                    {
                        sameTableCount++;
                    }
                }

                string toTableNameSingular = relSummary.ToTableName.Replace($"{entity.DatabaseSchema}.", "").ToSingular();

                var altName = toTableNameSingular;
                if (relSummary.FromTableName.Equals(relSummary.ToTableName))
                {
                    generatedFrom = ObjectNameGeneratedFrom.ToUniqueColumnName;
                }

                switch (generatedFrom)
                {
                    case ObjectNameGeneratedFrom.JoinFromColumnName:
                        altName = string.Join(",", relSummary.FromColumnName);
                        break;
                    case ObjectNameGeneratedFrom.ToUniqueColumnName:
                        altName = relSummary.ToUniqueColumnName(false);
                        break;
                    case ObjectNameGeneratedFrom.JoinToColumnName:
                        altName = string.Join(",", relSummary.ToColumnName);
                        break;
                    case ObjectNameGeneratedFrom.ToTableName:
                        altName = relSummary.ToTableName.Replace($"{entity.DatabaseSchema}.", "").ToSingular();
                        break;
                    case ObjectNameGeneratedFrom.FromTableName:
                        altName = relSummary.FromTableName.Replace($"{entity.DatabaseSchema}.", "").ToSingular();
                        break;
                }

                fieldName = (entity.Properties.ContainsKey(toTableNameSingular) || 
                            entityName == relSummary.ToTableName || 
                            sameTableCount > 1) 
                                ? altName 
                                : toTableNameSingular;
            }
            catch (Exception ex)
            {
                throw new Exception($"{procName}. {ex.Message}");
            }
            
            return fieldName;
        }

        /// <summary>
        /// Determines if the relationship ends with a many multiplicity
        /// </summary>
        /// <param name="relationship">The relationship to check</param>
        /// <returns>True if the relationship ends with many, otherwise false</returns>
        public static bool EndsAsMany(this IRelationship relationship)
        {
            return relationship.MultiplicityType == RelationshipMultiplicityType.OneToMany ||
                   relationship.MultiplicityType == RelationshipMultiplicityType.ZeroOrOneToMany;
        }

        /// <summary>
        /// Determines if the relationship begins with a many multiplicity
        /// </summary>
        /// <param name="relationship">The relationship to check</param>
        /// <returns>True if the relationship begins with many, otherwise false</returns>
        public static bool BeginsAsMany(this IRelationship relationship)
        {
            return relationship.MultiplicityType == RelationshipMultiplicityType.ManyToOne ||
                   relationship.MultiplicityType == RelationshipMultiplicityType.ManyToZeroOrOne;
        }

        /// <summary>
        /// Determines if the relationship begins with a one multiplicity
        /// </summary>
        /// <param name="relationship">The relationship to check</param>
        /// <returns>True if the relationship begins with one, otherwise false</returns>
        public static bool BeginsAsOne(this IRelationship relationship)
        {
            return relationship.MultiplicityType == RelationshipMultiplicityType.OneToMany ||
                   relationship.MultiplicityType == RelationshipMultiplicityType.OneToOne ||
                   relationship.MultiplicityType == RelationshipMultiplicityType.OneToZeroOrOne;
        }

        /// <summary>
        /// Determines if the relationship begins with a zero-or-one multiplicity
        /// </summary>
        /// <param name="relationship">The relationship to check</param>
        /// <returns>True if the relationship begins with zero-or-one, otherwise false</returns>
        public static bool BeginsAsZeroOrOne(this IRelationship relationship)
        {
            return relationship.MultiplicityType == RelationshipMultiplicityType.ZeroOrOneToMany ||
                   relationship.MultiplicityType == RelationshipMultiplicityType.ZeroOrOneToOne;
        }

        /// <summary>
        /// Determines if the relationship ends with a one multiplicity
        /// </summary>
        /// <param name="relationship">The relationship to check</param>
        /// <returns>True if the relationship ends with one, otherwise false</returns>
        public static bool EndsAsOne(this IRelationship relationship)
        {
            return relationship.MultiplicityType == RelationshipMultiplicityType.ManyToOne ||
                   relationship.MultiplicityType == RelationshipMultiplicityType.OneToOne ||
                   relationship.MultiplicityType == RelationshipMultiplicityType.ZeroOrOneToOne;
        }

        /// <summary>
        /// Determines if the relationship ends with a zero-or-one multiplicity
        /// </summary>
        /// <param name="relationship">The relationship to check</param>
        /// <returns>True if the relationship ends with zero-or-one, otherwise false</returns>
        public static bool EndsAsZeroOrOne(this IRelationship relationship)
        {
            return relationship.MultiplicityType == RelationshipMultiplicityType.ManyToZeroOrOne ||
                   relationship.MultiplicityType == RelationshipMultiplicityType.OneToZeroOrOne;
        }
    }
}
