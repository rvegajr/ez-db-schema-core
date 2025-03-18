using EzDbSchema.Core.Interfaces;
using EzDbSchema.Core.Objects;
using System.Collections.Generic;
using System.Linq;

namespace EzDbSchema.Core.Extentions
{
    /// <summary>
    /// Extension methods for IRelationshipList related to foreign keys
    /// </summary>
    public static class RelationshipListFKExtensions
    {
        /// <summary>
        /// Groups relationships by their foreign key name (constraint name)
        /// </summary>
        /// <param name="relationshipList">The relationship list to group</param>
        /// <returns>A dictionary of relationship lists grouped by foreign key name</returns>
        public static Dictionary<string, IRelationshipList> GroupByFKName(this IRelationshipList relationshipList)
        {
            if (relationshipList == null || relationshipList.Count == 0)
                return new Dictionary<string, IRelationshipList>();

            var result = new Dictionary<string, IRelationshipList>();
            
            // Group by constraint name which serves as the foreign key name
            var groupedByConstraintName = relationshipList
                .GroupBy(r => r.ConstraintName)
                .ToDictionary(g => g.Key, g => g.ToList());
            
            foreach (var group in groupedByConstraintName)
            {
                var fkName = group.Key;
                var relationships = group.Value;
                
                // Create a new relationship list for this group
                var relationshipListForGroup = new RelationshipList
                {
                    DatabaseObjectName = relationshipList.DatabaseObjectName,
                    Database = relationshipList.Database,
                    CustomAttributes = relationshipList.CustomAttributes,
                    IsEnabled = relationshipList.IsEnabled
                };
                
                // Add all relationships from this group to the new list
                foreach (var relationship in relationships)
                {
                    relationshipListForGroup.Add(relationship);
                }
                
                // Add the group to the result dictionary
                result.Add(fkName, relationshipListForGroup);
            }
            
            return result;
        }
    }
}
