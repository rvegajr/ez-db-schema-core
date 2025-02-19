using System;
using System.Reflection;
using System.Text;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	public class DatabaseObjectUpdates : EzObject, IDatabaseObjectUpdates
    {
        public DatabaseObjectUpdates() : base()
        {
        }
        internal static string ALIAS = "LastUpdates";

        public DateTime? LastCreated { get; set; } = null;
        public DateTime? LastModified { get; set; } = null;
        public string LastItemCreated { get; set; } = "";
        public string LastItemModified { get; set; } = "";
    }
}
