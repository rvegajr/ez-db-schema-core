using System;
using System.Collections.Generic;

namespace EzDbSchema.Core.Interfaces
{
    public interface IDatabase : IEzObject, IJsonRenderable, IDictionary<string, IEntity>
    {
        // Database Properties
        string Name { get; set; }
        bool ShowWarnings { get; set; }
        bool AutoAddPrimaryKeys { get; set; }

        // Entity Collection
        IEntityDictionary Entities { get; set; }
        bool ContainsValue(IEntity entity);
        new IEntityNameList Keys { get; }
        new IEnumerable<IEntity> Values { get; }

        // Database Operations
        IDatabase Render(string entityName, string ConnectionString);
        IDatabaseObjectUpdates LastUpdates { get; set; }
    }
}
