using EzDbSchema.Core.Interfaces;
using EzDbSchema.Core.Objects;
using System.Collections.Generic;
using System.Linq;

namespace EzDbSchema.Core.Extentions
{
    /// <summary>
    /// Extension methods for IRelationshipList
    /// </summary>
    public static class RelationshipListExtensions
    {
        /// <summary>
        /// Creates a summary of the relationship list
        /// </summary>
        /// <param name="relationshipList">The relationship list to summarize</param>
        /// <returns>A RelationshipSummary object</returns>
        public static RelationshipSummary AsSummary(this IRelationshipList relationshipList)
        {
            if (relationshipList == null || relationshipList.Count == 0)
                return new RelationshipSummary();

            var firstRelationship = relationshipList.FirstOrDefault();
            if (firstRelationship == null)
                return new RelationshipSummary();

            var summary = new RelationshipSummary
            {
                ConstraintName = firstRelationship.ConstraintName,
                FromTableName = firstRelationship.FromTableName,
                ToTableName = firstRelationship.ToTableName,
                FromColumnName = new List<string>(),
                ToColumnName = new List<string>(),
                MultiplicityType = firstRelationship.MultiplicityType
            };

            foreach (var relationship in relationshipList)
            {
                if (!summary.FromColumnName.Contains(relationship.FromColumnName))
                    summary.FromColumnName.Add(relationship.FromColumnName);

                if (!summary.ToColumnName.Contains(relationship.ToColumnName))
                    summary.ToColumnName.Add(relationship.ToColumnName);
            }

            return summary;
        }

        /// <summary>
        /// Gets a unique column name for the relationship list
        /// </summary>
        /// <param name="relationshipList">The relationship list</param>
        /// <param name="useFromColumn">Whether to use the from column (true) or to column (false)</param>
        /// <returns>A unique column name</returns>
        public static string ToUniqueColumnName(this IRelationshipList relationshipList, bool useFromColumn = true)
        {
            if (relationshipList == null || relationshipList.Count == 0)
                return string.Empty;

            var summary = relationshipList.AsSummary();
            return useFromColumn 
                ? string.Join("_", summary.FromColumnName) 
                : string.Join("_", summary.ToColumnName);
        }
    }
}
