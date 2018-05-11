using System;
using System.Collections.Generic;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	public class RelationshipDictionary : Dictionary<string, IRelationship>, IRelationshipDictionary
    {
    }

	public class RelationshipList : List<IRelationship>, IRelationshipList
    {
    }
}
