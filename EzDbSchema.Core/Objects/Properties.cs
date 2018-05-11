using System;
using System.Collections.Generic;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	public class PropertyDictionary : Dictionary<string, IProperty>, IPropertyDictionary
    {
    }

	public class PropertyList : List<IProperty>, IPropertyList
    {
    }

	public class PrimaryKeyProperties : List<IProperty>, IPrimaryKeyProperties
    {
        protected IEntity Entity;
        public PrimaryKeyProperties(IEntity Parent)
        {
            this.Entity = Parent;
        }
    }
}
