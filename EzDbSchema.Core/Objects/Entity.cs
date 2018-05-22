using System;
using System.Reflection;
using System.Text;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	public class Entity : IEntity
    {
        public Entity()
        {
			this.PrimaryKeys = new PrimaryKeyProperties(this);
        }
        public string Name { get; set; }
        public string Schema { get; set; }
        public string Type { get; set; }
        public string TemporalType { get; set; }
        public string ObjectState { get; set; }

		public IPropertyDictionary Properties { get; set; } = new PropertyDictionary();
		public IRelationshipList Relationships { get; set; } = new RelationshipList();
		public IPrimaryKeyProperties PrimaryKeys { get; set; }
		public ICustomAttributes CustomAttributes { get; set; }

        public bool IsTemporalView { get; set; }

        public string AsJson()
        {
            var sb = new StringBuilder();
            sb.Append("{");
            foreach (PropertyInfo propertyInfo in this.GetType().GetProperties())
                if (propertyInfo.CanRead)
                    sb.AppendJsonParm(propertyInfo.Name, propertyInfo.GetValue(this, null));
            sb.Append("}");
            return sb.ToString();
        }

        public IEntity FromJson(string jsonToConvert)
        {
            throw new NotImplementedException();
        }

        public bool HasPrimaryKeys()
        {
            foreach (var prop in Properties.Values)
                if (prop.IsKey) return true;
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
        }    }
}
