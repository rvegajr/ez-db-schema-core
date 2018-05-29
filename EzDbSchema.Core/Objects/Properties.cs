using System;
using System.Collections.Generic;
using System.Xml;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	public class PropertyDictionary : Dictionary<string, IProperty>, IPropertyDictionary
    {
        public static string ALIAS = "Properties";

        public PropertyDictionary()
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

    public class PropertyList : List<IProperty>, IPropertyList
    {
        public static string ALIAS = "Properties";

        public PropertyList()
        {
            this.Id = this.GetId();
        }
        public int Id { get; set; }
        public XmlNode AsXml(XmlDocument doc)
        {
            return this.ListAsXmlNode(doc, ALIAS);
        }

        public string AsXml()
        {
            return AsXml(new XmlDocument()).OuterXml;
        }

    }

    public class PrimaryKeyProperties : List<IProperty>, IPrimaryKeyProperties
    {
        public static string ALIAS = "PrimaryKeys";

        public PrimaryKeyProperties()
        {
            this.Id = this.GetId();
        }
        public int Id { get; set; }
        protected IEntity Entity;
        public PrimaryKeyProperties(IEntity Parent)
        {
            this.Entity = Parent;
        }

        public XmlNode AsXml(XmlDocument doc)
        {
            XmlNode nod = doc.CreateElement(XmlConvert.EncodeName(ALIAS));
            foreach (var item in this)
            {
                XmlNode refnod = nod.OwnerDocument.CreateElement("item");
                ((XmlElement)refnod).SetAttribute("ref", item.Id.ToSafeString());
                nod.AppendChild(refnod);

            }
            return nod;
        }

        public string AsXml()
        {
            return AsXml(new XmlDocument()).OuterXml;
        }

    }
}
