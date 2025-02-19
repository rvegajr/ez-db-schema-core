using System;
using System.Collections.Generic;

namespace EzDbSchema.Core.Interfaces
{
    public interface IPropertyDictionary : IDictionary<string, IProperty>, IEzObject
    {
    }

    public interface IPropertyList : IList<IProperty>, IEzObject
    {
    }

    public interface IPrimaryKeyProperties : IPropertyList, IEzObject
    {
    }
}
