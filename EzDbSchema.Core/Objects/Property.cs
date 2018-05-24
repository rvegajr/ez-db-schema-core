using System;
using System.Reflection;
using System.Text;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	public class Property : EzObject, IProperty
    {
        public Property() : base()
        {

        }
        public string Name { get; set; }
        public string Type { get; set; }
        public int MaxLength { get; set; }
        public int Precision { get; set; }
        public int Scale { get; set; }
        public bool IsNullable { get; set; }
        public bool IsKey { get; set; }
        public int KeyOrder { get; set; }
        public bool IsIdentity { get; set; }
        public IEntity Parent { get; set; }
		public IRelationshipList RelatedTo { get; set; } = new RelationshipList();
		public ICustomAttributes CustomAttributes { get; set; } = new CustomAttributes();

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

        public IProperty FromJson(string Json)
        {
            throw new NotImplementedException();
        }
    }
}
