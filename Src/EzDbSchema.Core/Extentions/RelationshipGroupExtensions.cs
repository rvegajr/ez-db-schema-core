using EzDbSchema.Core.Interfaces;
using EzDbSchema.Core.Objects;
using System.Linq;

namespace EzDbSchema.Core.Extentions
{
    /// <summary>
    /// Extension methods for IRelationshipGroup
    /// </summary>
    public static class RelationshipGroupExtensions
    {
        /// <summary>
        /// Creates a summary of the relationship group
        /// </summary>
        /// <param name="relationshipGroup">The relationship group</param>
        /// <returns>A summary of the relationship group</returns>
        public static RelationshipSummary AsSummary(this IRelationshipGroup relationshipGroup)
        {
            if (relationshipGroup == null || relationshipGroup.Count == 0)
                return new RelationshipSummary();

            // Get the first relationship list
            var firstList = relationshipGroup.Values.FirstOrDefault();
            if (firstList == null)
                return new RelationshipSummary();

            // Use the AsSummary method of the first relationship list
            return firstList.AsSummary();
        }
        
        /// <summary>
        /// Gets the unique column name from the relationship group
        /// </summary>
        /// <param name="relationshipGroup">The relationship group</param>
        /// <param name="includeTableName">Whether to include the table name</param>
        /// <returns>The unique column name</returns>
        public static string ToUniqueColumnName(this IRelationshipGroup relationshipGroup, bool includeTableName = true)
        {
            if (relationshipGroup == null || relationshipGroup.Count == 0)
                return string.Empty;

            // Get the first relationship list
            var firstList = relationshipGroup.Values.FirstOrDefault();
            if (firstList == null)
                return string.Empty;

            // Use the ToUniqueColumnName method of the first relationship list
            return firstList.ToUniqueColumnName(includeTableName);
        }
    }
}
