using System;

namespace EzDbSchema.Core.Interfaces
{
    public interface IDatabaseObjectUpdates : IJson<IDatabaseObjectUpdates>
    {
        DateTime? LastCreated { get; set; }
        DateTime? LastModified { get; set; }
        string LastItemCreated { get; set; }
        string LastItemModified { get; set; }
    }
}