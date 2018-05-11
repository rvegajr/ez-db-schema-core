using System;
using System.Collections.Generic;

namespace EzDbSchema.Core.Interfaces
{
    public interface IRelationshipDictionary : IDictionary<string, IRelationship>
    {
    }

    public interface IRelationshipList : IList<IRelationship>
    {
    }
}
