using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Interfaces
{
    /// <summary>
    /// Interface for a group of relationships
    /// </summary>
    public interface IRelationshipGroup : IDictionary<string, IRelationshipList>
    {
        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
        IDatabase Database { get; set; }
    }
}
