using System;
using System.Reflection;
using System.Text;
using System.Linq;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	public class Entity : EzObject, IEntity
    {
        internal static string ALIAS = "Entity";
        public Entity() : base()
        {
			this.PrimaryKeys = new PrimaryKeyProperties(this);
        }
        public override string DatabaseObjectName { get => DatabaseSchema + "." + TableName; }
        public IDatabase ParentDatabase { get; set; }
        public IDatabase Database => ParentDatabase;
        public string TableName { get; set; }
        public string TableAlias { get; set; }
        public string DatabaseSchema { get; set; }
        public string EntityType { get; set; }
        public string TemporalType { get; set; }
        public string EntityState { get; set; }

		public IPropertyDictionary Properties { get; set; } = new PropertyDictionary();
		public IRelationshipReferenceList Relationships { get; set; } = new RelationshipReferenceList();
        public IRelationshipGroups RelationshipGroups { get; set; } = new RelationshipGroups();

        public IPrimaryKeyProperties PrimaryKeys { get; set; }

        public bool IsTemporalView { get; set; }

        public bool HasPrimaryKeys()
        {
            foreach (var prop in Properties.Values)
                if (prop.IsPrimaryKey) return true;
            return false;
        }
        public bool IsAuditable()
        {
            return Properties.ContainsKey("Created")
                && Properties.ContainsKey("CreatedBy")
                && Properties.ContainsKey("Updated")
                && Properties.ContainsKey("UpdatedBy");
        }
        public bool isAuditablePropertyName(string propertyNameToCheck)
        {
            return ((propertyNameToCheck.Equals("Created"))
                || (propertyNameToCheck.Equals("CreatedBy"))
                || (propertyNameToCheck.Equals("Updated"))
                || (propertyNameToCheck.Equals("UpdatedBy")));
        }

        // Database Features
        public bool HasTriggers => Properties.Values.Any(p => p.IsComputed);
        public bool HasCheckConstraints => Properties.Values.Any(p => !string.IsNullOrEmpty(p.ValidationRules));
        public bool HasForeignKeyConstraints => Relationships.Count > 0;
        public bool HasUniqueIndexes => Properties.Values.Any(p => p.IsUnique);

        // Security Features
        public bool RequiresAuthorization => Properties.Values.Any(p => p.RequiresAuthorization);
        public bool HasRowLevelSecurity => Properties.Values.Any(p => p.RequiresAuthorization || p.RequiresEncryption);

        // Performance Features
        public bool IsFrequentlyAccessed => Properties.Values.Any(p => p.IsFrequentlyAccessed);
        public bool RequiresCaching => Properties.Values.Any(p => p.RequiresCaching);
        public string CacheStrategy => Properties.Values.Any(p => !string.IsNullOrEmpty(p.CacheStrategy)) 
            ? Properties.Values.First(p => !string.IsNullOrEmpty(p.CacheStrategy)).CacheStrategy 
            : null;

        public bool IsLargeDataset => Properties.Values.Count > 20 || Properties.Values.Any(p => p.MaxLength > 1000);

        // Advanced Features
        public bool IsVersioned => Properties.ContainsKey("Version") || Properties.ContainsKey("RowVersion");
        public bool GeneratesEvents => Properties.Values.Any(p => p.RequiresNotification || p.TrackChanges);
        public bool RequiresNotification => Properties.Values.Any(p => p.RequiresNotification);
        public bool HasExternalReferences => Properties.Values.Any(p => p.IsExternalReference);
        public bool IsPartOfWorkflow => Properties.Values.Any(p => p.RequiresAuthorization || p.RequiresValidation);
    }
}
