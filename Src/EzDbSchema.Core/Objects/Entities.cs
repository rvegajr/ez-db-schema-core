using System;
using System.Collections.Generic;
using System.Xml;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Extentions.Objects;
using EzDbSchema.Core.Extentions.Xml;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	/// <summary></summary>
	public class EntityDictionary : Dictionary<string, IEntity>, IEntityDictionary, IXmlRenderableInternal
    {
        internal static string ALIAS = "Entities";
        public EntityDictionary()
        {
            this._id = this.GetId();
            this.IsEnabled = true;
        }
        public int _id { get; set; }
        public bool IsEnabled { get; set; } = true;
        public ICustomAttributes CustomAttributes { get; set; }

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

    public class EntityList : List<IEntity>, IEntityList, IXmlRenderableInternal
    {
        internal static string ALIAS = "Entities";

        public EntityList()
        {
            this._id = this.GetId();
            this.IsEnabled = true;
        }
        public int _id { get; set; }
        public bool IsEnabled { get; set; } = true;
        public ICustomAttributes CustomAttributes { get; set; }

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
            return node;
        }
    }
    public class EntityNameList : List<string>, IEntityNameList, IXmlRenderableInternal
    {
        internal static string ALIAS = "EntityNames";

        public EntityNameList()
        {
        }

        public EntityNameList(IEnumerable<string> collection) : base(collection)
        {
        }

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
            return node;
        }
    }
}
