using System;
using System.Collections.Generic;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	public class PropertyDictionary : Dictionary<string, IProperty>, IPropertyDictionary
    {
        public PropertyDictionary()
        {
            this.Id = this.GetId();
        }
        public int Id { get; set; }
    }

    public class PropertyList : List<IProperty>, IPropertyList
    {
        public PropertyList()
        {
            this.Id = this.GetId();
        }
        public int Id { get; set; }
    }

    public class PrimaryKeyProperties : List<IProperty>, IPrimaryKeyProperties
    {
        public PrimaryKeyProperties()
        {
            this.Id = this.GetId();
        }
        public int Id { get; set; }
        protected IEntity Entity;
        public PrimaryKeyProperties(IEntity Parent)
        {
            this.Entity = Parent;
        }
    }
}
