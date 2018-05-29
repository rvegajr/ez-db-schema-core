using System;
using System.Collections.Generic;
using System.Xml;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	public class CustomAttributes : Dictionary<string, object>, ICustomAttributes
    {
        public static string ALIAS = "CustomAttributes";

        public CustomAttributes()
        {
            this.Id = this.GetId();
        }
        public int Id { get; set; }
        public XmlNode AsXml(XmlDocument doc)
        {
            return this.DictionaryAsXmlNode(doc, ALIAS);
        }

        public string AsXml()
        {
            return AsXml(new XmlDocument()).OuterXml;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Interface |
                           System.AttributeTargets.Property,
                           AllowMultiple = false)  // multiuse attribute  
    ]
    public class AsRef : System.Attribute
    {
        public AsRef(string referenceFieldName)
        {
            ReferenceFieldName = referenceFieldName;
        }
        public string ReferenceFieldName { get; set; }
    }
}
