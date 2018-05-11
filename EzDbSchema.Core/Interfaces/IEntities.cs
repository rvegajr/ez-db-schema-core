using System;
using System.Collections.Generic;

namespace EzDbSchema.Core.Interfaces
{
    public interface IEntityDictionary : IDictionary<string, IEntity>
    {
    }
    public interface IEntityList : IList<IEntity>
    {
    }
}
