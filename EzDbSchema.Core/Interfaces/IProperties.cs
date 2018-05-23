using System;
using System.Collections.Generic;

namespace EzDbSchema.Core.Interfaces
{
    public interface IPropertyDictionary : IDictionary<string, IProperty>, IEzObject, IEzObjectJson<IPropertyDictionary>
    {
    }

    public interface IPropertyList : IList<IProperty>, IEzObject, IEzObjectJson<IPropertyList>
    {
    }

    public interface IPrimaryKeyProperties : IPropertyList, IEzObject, IEzObjectJson<IPrimaryKeyProperties>
    {
    }
}
