using System;
using System.Collections.Generic;
using EzDbSchema.Core.Enums;

namespace EzDbSchema.Core.Interfaces
{
    public interface IRelationshipDictionary : IDictionary<string, IRelationship>, IJson<IRelationshipDictionary>
    {
    }

    public interface IRelationshipList : IList<IRelationship>, IJson<IRelationshipList>
    {
		IRelationshipList Fetch(RelationshipType TypeToFetch);
		int CountItems(string searchFor);
		int CountItems(RelationSearchField searchField, string searchFor);
    }
}
