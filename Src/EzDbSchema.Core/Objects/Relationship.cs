using System;
using System.Reflection;
using System.Text;
using System.Xml;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Extentions.Xml;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	public class Relationship : EzObject, IRelationship, IXmlRenderableInternal
    {
        public static string ALIAS = "Relationship";

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
        public RelationshipMultiplicityType MultiplicityType { get; set; } = RelationshipMultiplicityType.Unknown;
        public string PrimaryTableName { get; set; }
        public int FKOrdinalPosition { get; set; } = 0;
        [AsRef("_id")]
        public IEntity Parent { get; set; }

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

    public enum RelationshipMultiplicityType
    {
        Unknown,
        OneToOne,
        OneToMany,
        ZeroOrOneToMany,
        ManyToOne,
        ManyToZeroOrOne
    }
}
