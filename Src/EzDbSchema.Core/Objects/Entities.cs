using System;
using System.Collections.Generic;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Extentions.Objects;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	/// <summary></summary>
	public class EntityDictionary : Dictionary<string, IEntity>, IEntityDictionary
    {
        internal static string ALIAS = "Entities";
        public EntityDictionary()
        {
            this._id = this.GetId();
            this.IsEnabled = true;
        }
        public int _id { get; set; }
        public bool IsEnabled { get; set; } = true;
        public ICustomAttributes CustomAttributes { get; set; }
        public string DatabaseObjectName { get; set; } = string.Empty;
    }

    public class EntityList : List<IEntity>, IEntityList
    {
        internal static string ALIAS = "Entities";

        public EntityList()
        {
            this._id = this.GetId();
            this.IsEnabled = true;
        }

        public string DatabaseObjectName { get; set; } = string.Empty;
        public int _id { get; set; }
        public bool IsEnabled { get; set; } = true;
        public ICustomAttributes CustomAttributes { get; set; }
    }
    public class EntityNameList : List<string>, IEntityNameList
    {
        internal static string ALIAS = "EntityNames";

        public EntityNameList()
        {
        }

        public EntityNameList(IEnumerable<string> collection) : base(collection)
        {
        }
    }
}
