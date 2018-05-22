using System;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	public class Relationship : IRelationship, IJson<IRelationship>
    {
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

    }
}
