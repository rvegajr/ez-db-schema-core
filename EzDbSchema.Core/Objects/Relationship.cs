using System;
using System.Reflection;
using System.Text;
using System.Xml;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	public class Relationship : EzObject, IRelationship
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
        public string PrimaryTableName { get; set; }
        [AsRef("Id")]
        public IEntity Parent { get; set; }

        public string AsXml()
        {
            return AsXml(new XmlDocument()).OuterXml;
        }

        public XmlNode AsXml(XmlDocument doc)
        {
            return this.AsXmlNode(doc, ALIAS);
        }


    }
}
