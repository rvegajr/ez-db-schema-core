using System;
using System.Collections.Generic;

namespace EzDbSchema.Core.Interfaces
{
    public interface IDatabase : IEzObject, IXmlRenderable
    {
        IEntity this[string entityName] { get; set; }
        IEntityDictionary Entities { get; set; }
        void Add(string entityName, IEntity entity);
        bool ContainsKey(string entityName);
        bool ContainsValue(IEntity entity);
		IDatabase Render(string entityName, string ConnectionString);
		string Name { get; set; }
		IDatabaseObjectUpdates LastUpdates { get; set; }
        IEntityNameList Keys {get;}
		bool ShowWarnings { get; set; }
	}
}
