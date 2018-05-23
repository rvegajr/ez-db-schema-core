using System;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	public class DatabaseObjectUpdates : EzObject, IDatabaseObjectUpdates
    {
        public DatabaseObjectUpdates() : base()
        {
        }
        public DateTime? LastCreated { get; set; } = null;
        public DateTime? LastModified { get; set; } = null;
        public string LastItemCreated { get; set; } = "";
        public string LastItemModified { get; set; } = "";
        public string AsJson()
        {
            throw new NotImplementedException();
        }

        public IDatabase FromJson(string Json)
        {
            throw new NotImplementedException();
        }
    }
}
