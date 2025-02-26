using System;
using System.Collections.Generic;
using EzDbSchema.Core.Enums;

namespace EzDbSchema.Core.Interfaces
{
    public interface IRelationshipDictionary : IDictionary<string, IRelationship>, IEzObject
    {
    }

    public interface IRelationshipList : IList<IRelationship>, IEzObject
    {
        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        IDatabase Database { get; set; }

		IRelationshipList Fetch(RelationshipMultiplicityType TypeToFetch);
		int CountItems(string searchFor);
		int CountItems(RelationSearchField searchField, string searchFor);
        IRelationshipList FindItems(string searchFor);
        IRelationshipList FindItems(RelationSearchField searchField, string searchFor);

    }
    public interface IRelationshipReferenceList : IRelationshipList, IEzObject
    {
    }
    public interface IRelationshipGroups : IDictionary<string, IRelationshipList>, IEzObject
    {
        /// <summary>
        /// Gets or sets the database that contains these relationship groups.
        /// </summary>
        IDatabase Database { get; set; }
    }

}
