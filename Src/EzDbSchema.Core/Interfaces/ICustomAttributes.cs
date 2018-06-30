using System;
using System.Collections.Generic;

namespace EzDbSchema.Core.Interfaces
{
    public interface ICustomAttributes : IDictionary<string, object>, IEzObject, IXmlRenderable
    {
    }
}
