using System;
using System.Collections.Generic;
using System.Text;
using EzDbSchema.Core.Extentions;
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

        public string AsJson()
        {
            var sb = new StringBuilder();
            sb.Append("{");
            foreach (var key in this.Keys)
            {
                sb.AppendJsonParm(key, this[key].AsJson());
            }
            sb.Append("}");
            return sb.ToString();
        }

        public IEntityDictionary FromJson(string jsonToConvert)
        {
            throw new NotImplementedException();
        }
    }

	public class EntityList : List<IEntity>
    {
    }
}
