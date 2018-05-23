using System;
using System.Collections.Generic;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	/// <summary></summary>
	public class EntityDictionary : Dictionary<string, IEntity>, IEntityDictionary
    {
        public EntityDictionary()
        {
            this.Id = this.GetId();
        }
        public int Id { get; set; }
    }

	public class EntityList : List<IEntity>, IEntityList
    {
        public EntityList()
        {
            this.Id = this.GetId();
        }
        public int Id { get; set; }
    }
}
