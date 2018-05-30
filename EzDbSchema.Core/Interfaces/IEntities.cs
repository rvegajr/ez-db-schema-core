using System;
using System.Collections.Generic;

namespace EzDbSchema.Core.Interfaces
{
    public interface IEntityDictionary : IDictionary<string, IEntity>, IEzObject, IXmlRenderable
    {
    }
    public interface IEntityList : IList<IEntity>, IEzObject, IXmlRenderable
    {
    }
    public interface IEntityNameList : IList<string>, IXmlRenderable
    {
    }
}