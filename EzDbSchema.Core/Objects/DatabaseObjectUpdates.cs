using System;
using System.Reflection;
using System.Text;
using System.Xml;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Extentions.Xml;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	public class DatabaseObjectUpdates : EzObject, IDatabaseObjectUpdates
    {
        public DatabaseObjectUpdates() : base()
        {
        }
        public static string ALIAS = "LastUpdates";

        public DateTime? LastCreated { get; set; } = null;
        public DateTime? LastModified { get; set; } = null;
        public string LastItemCreated { get; set; } = "";
        public string LastItemModified { get; set; } = "";
        public string AsXml()
        {
            var doc = new XmlDocument();
            return doc.AppendChild(doc.CreateElement("xml").AppendChild(AsXml(doc))).OuterXml;
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
