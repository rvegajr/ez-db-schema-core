using System;
using System.Reflection;
using System.Text;
using System.Xml;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	public class DatabaseObjectUpdates : EzObject, IDatabaseObjectUpdates
    {
        public DatabaseObjectUpdates() : base()
        {
        }
        public static string ALIAS = "DatabaseObjectUpdates";

        public DateTime? LastCreated { get; set; } = null;
        public DateTime? LastModified { get; set; } = null;
        public string LastItemCreated { get; set; } = "";
        public string LastItemModified { get; set; } = "";
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

        public string AsXml()
        {
            var doc = new XmlDocument();
            return doc.AppendChild(doc.CreateElement("xml").AppendChild(AsXml(doc))).OuterXml;
        }

        public XmlNode AsXml(XmlDocument doc)
        {
            return this.AsXmlNode(doc, ALIAS);
        }

        public IDatabaseObjectUpdates FromJson(string Json)
        {
            throw new NotImplementedException();
        }
    }
}
