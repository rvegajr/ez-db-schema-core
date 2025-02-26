using EzDbSchema.Core.Enums;
using EzDbSchema.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace EzDbSchema.Core.Objects
{
    /// <summary>
    /// Implementation of IRelationshipGroup interface for grouping relationships
    /// </summary>
    public class RelationshipGroup : Dictionary<string, IRelationshipList>, IRelationshipGroup
    {
        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        public IDatabase Database { get; set; }

        /// <summary>
        /// Counts items in the relationship group that match the specified search term
        /// </summary>
        /// <param name="searchFor">The term to search for</param>
        /// <returns>Count of matching items</returns>
        public int CountItems(string searchFor)
        {
            return CountItems(RelationSearchField.ToTableName, searchFor);
        }

        /// <summary>
        /// Counts items in the relationship group that match the specified search field and term
        /// </summary>
        /// <param name="searchField">The field to search in</param>
        /// <param name="searchFor">The term to search for</param>
        /// <returns>Count of matching items</returns>
        public int CountItems(RelationSearchField searchField, string searchFor)
        {
            int count = 0;
            foreach (var relationships in Values)
            {
                foreach (var relationship in relationships)
                {
                    string value = null;
                    switch (searchField)
                    {
                        case RelationSearchField.ToTableName:
                            value = relationship?.ToTableName;
                            break;
                        case RelationSearchField.FromTableName:
                            value = relationship?.FromTableName;
                            break;
                        case RelationSearchField.ToColumnName:
                            value = relationship?.ToColumnName;
                            break;
                        case RelationSearchField.FromColumnName:
                            value = relationship?.FromColumnName;
                            break;
                        case RelationSearchField.ToFieldName:
                            value = relationship?.ToPropertyName;
                            break;
                        case RelationSearchField.FromFieldName:
                            value = relationship?.FromPropertyName;
                            break;
                    }
                    if (value?.Contains(searchFor, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
    }
}
