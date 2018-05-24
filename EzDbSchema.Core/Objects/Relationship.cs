using System;
using System.Reflection;
using System.Text;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	public class Relationship : EzObject, IRelationship
    {
        public Relationship() : base()
        {

        }
        public string Name { get; set; }
        public string FromTableName { get; set; }
        public string FromFieldName { get; set; }
        public string FromColumnName { get; set; }
        public string ToTableName { get; set; }
        public string ToFieldName { get; set; }
        public string ToColumnName { get; set; }
        public string Type { get; set; }
        public string PrimaryTableName { get; set; }
        public IEntity Parent { get; set; }

        public string AsJson()
        {
            var sb = new StringBuilder();
            sb.Append("{");
            foreach (PropertyInfo pi in this.GetType().GetProperties())
                if (!((pi.PropertyType.FullName.Contains("EzDbSchema")) || (pi.PropertyType.FullName.Contains("Collection"))))
                    sb.AppendJson(pi.Name, pi.GetValue(this, null));
            sb.Append("}");
            return sb.ToString();
        }

        public IRelationship FromJson(string Json)
        {
            throw new NotImplementedException();
        }
    }
}
