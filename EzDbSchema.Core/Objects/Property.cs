using System;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	public class Property : IProperty
    {
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
    }
}
