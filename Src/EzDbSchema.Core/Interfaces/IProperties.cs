using System;
using System.Collections.Generic;

namespace EzDbSchema.Core.Interfaces
{
    public interface IPropertyDictionary : IDictionary<string, IProperty>, IEzObject, IXmlRenderable
    {
    }

    public interface IPropertyList : IList<IProperty>, IEzObject, IXmlRenderable
    {
    }

    public interface IPrimaryKeyProperties : IPropertyList, IEzObject, IXmlRenderable
    {
    }
}
