using System;
using System.Collections.Generic;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	/// <summary></summary>
	public class EntityDictionary : Dictionary<string, IEntity>, IEntityDictionary
    {
		/// <summary></summary>
        public EntityDictionary()
        {
        }
    }

	public class EntityList : List<IEntity>
    {
    }
}
