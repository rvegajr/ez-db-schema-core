using System;
using System.Collections.Generic;

namespace EzDbSchema.Core.Interfaces
{
    public interface IEntityDictionary : IDictionary<string, IEntity>, IJson<IEntityDictionary>
    {
    }
    public interface IEntityList : IList<IEntity>
    {
    }
}
