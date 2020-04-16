using System;
using System.Collections.Generic;
using System.Xml;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Extentions.Objects;
using EzDbSchema.Core.Extentions.Xml;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	public class PropertyDictionary : Dictionary<string, IProperty>, IPropertyDictionary, IXmlRenderableInternal
    {
        internal static string ALIAS = "Properties";

        public PropertyDictionary()
        {
            this._id = this.GetId();
        }
        public int _id { get; set; }
        public XmlNode AsXml(XmlDocument doc)
        {
            return this.DictionaryAsXmlNode(doc, ALIAS);
        }

        public string AsXml()
        {
            return AsXml(new XmlDocument()).OuterXml;
        }

        public void FromXml(string Xml)
        {
            var doc = (new XmlDocument());
            doc.LoadXml(Xml);
            FromXml(doc.FirstChild);
        }

        public XmlNode FromXml(XmlNode node)
        {
            this.DictionaryFromXmlNodeList(node.ChildNodes, ALIAS);
            return node;
        }
    }

    public class PropertyList : List<IProperty>, IPropertyList, IXmlRenderableInternal
    {
        internal static string ALIAS = "Properties";

        public PropertyList()
        {
            this._id = this.GetId();
        }
        public int _id { get; set; }
        public XmlNode AsXml(XmlDocument doc)
        {
            return this.ListAsXmlNode(doc, ALIAS);
        }

        public string AsXml()
        {
            return AsXml(new XmlDocument()).OuterXml;
        }

        public void FromXml(string Xml)
        {
            var doc = (new XmlDocument());
            doc.LoadXml(Xml);
            FromXml(doc.FirstChild);
        }

        public XmlNode FromXml(XmlNode node)
        {
            this.ListFromXmlNodeList(node.ChildNodes, ALIAS);
            return node;
        }
    }

    public class PrimaryKeyProperties : List<IProperty>, IPrimaryKeyProperties, IXmlRenderableInternal
    {
        internal static string ALIAS = "PrimaryKeys";

        public PrimaryKeyProperties()
        {
            this._id = this.GetId();
        }
        public int _id { get; set; }
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
                XmlNode refnod = nod.OwnerDocument.CreateElement(Property.ALIAS);
                ((XmlElement)refnod).SetAttribute("ref", item._id.ToSafeString());
                nod.AppendChild(refnod);

            }
            return nod;
        }

        public string AsXml()
        {
            return AsXml(new XmlDocument()).OuterXml;
        }

        public void FromXml(string Xml)
        {
            var doc = (new XmlDocument());
            doc.LoadXml(Xml);
            FromXml(doc.FirstChild);
        }

        public XmlNode FromXml(XmlNode node)
        {
            this.ListFromXmlNodeList(node.ChildNodes, ALIAS);
            return node;
        }
    }
}
