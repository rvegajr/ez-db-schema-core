using System;
using System.Collections.Generic;

namespace EzDbSchema.Core.Interfaces
{
    public interface IPropertyDictionary : IDictionary<string, IProperty>, IEzObject
    {
        /// <summary>
        /// Gets or sets the entity that contains these properties.
        /// </summary>
        IEntity Entity { get; set; }
    }

    public interface IPropertyList : IList<IProperty>, IEzObject
    {
    }

    public interface IPrimaryKeyProperties : IPropertyList, IEzObject
    {
    }
}
