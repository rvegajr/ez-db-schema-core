using System;
using System.Collections.Generic;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Extentions.Objects;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	public class PropertyDictionary : Dictionary<string, IProperty>, IPropertyDictionary
    {
        public string DatabaseObjectName { get; set; } = string.Empty;
        internal static string ALIAS = "Properties";

        public PropertyDictionary()
        {
            this._id = this.GetId();
            this.IsEnabled = true;
        }
        public int _id { get; set; }
        public bool IsEnabled { get; set; } = true;
        public ICustomAttributes CustomAttributes { get; set; }
        /// <summary>
        /// Gets or sets the entity that contains these properties.
        /// </summary>
        public IEntity Entity { get; set; }
    }

    public class PropertyList : List<IProperty>, IPropertyList
    {
        public string DatabaseObjectName { get; set; } = string.Empty;
        internal static string ALIAS = "Properties";

        public PropertyList()
        {
            this._id = this.GetId();
            this.IsEnabled = true;
        }
        public int _id { get; set; }
        public bool IsEnabled { get; set; } = true;
        public ICustomAttributes CustomAttributes { get; set; }

    }

    public class PrimaryKeyProperties : List<IProperty>, IPrimaryKeyProperties
    {
        public string DatabaseObjectName { get; set; } = string.Empty;

        internal static string ALIAS = "PrimaryKeys";

        public PrimaryKeyProperties()
        {
            this._id = this.GetId();
            this.IsEnabled = true;
        }
        public int _id { get; set; }
        public bool IsEnabled { get; set; } = true;
        public ICustomAttributes CustomAttributes { get; set; }

        protected IEntity Entity;
        public PrimaryKeyProperties(IEntity Parent)
        {
            this.Entity = Parent;
        }
    }
}
