using System;
using System.Collections.Generic;

namespace EzDbSchema.Core.Interfaces
{
    public interface IEntityDictionary : IDictionary<string, IEntity>, IEzObject, IEzObjectJson<IEntityDictionary>
    {
    }
    public interface IEntityList : IList<IEntity>, IEzObject, IEzObjectJson<IEntityList>
    {
    }
}
