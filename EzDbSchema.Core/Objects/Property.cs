using System;
using System.Reflection;
using System.Text;
using System.Xml;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	public class Property : EzObject, IProperty
    {
        public static string ALIAS = "Property";

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
        [AsRef("_id")]
        public IEntity Parent { get; set; }
		public IRelationshipList RelatedTo { get; set; } = new RelationshipList();
		public ICustomAttributes CustomAttributes { get; set; } = new CustomAttributes();

        public string AsXml()
        {
            return AsXml(new XmlDocument()).OuterXml;
        }

        public XmlNode AsXml(XmlDocument doc)
        {
            return this.AsXmlNode(doc, ALIAS);
        }
        public void FromXml(string Xml)
        {
            var doc = (new XmlDocument());
            doc.LoadXml(Xml);
            FromXml(doc.FirstChild);
        }

        public XmlNode FromXml(XmlNode node)
        {
            this.FromXmlNode(node, ALIAS);
            return node;
        }
    }
}
