using System;
using System.Collections.Generic;

namespace EzDbSchema.Core.Interfaces
{
    public interface IEntityDictionary : IDictionary<string, IEntity>, IEzObject
    {
    }
    public interface IEntityList : IList<IEntity>, IEzObject
    {
    }
}
