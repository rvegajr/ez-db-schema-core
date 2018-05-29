using System;
using System.Collections.Generic;
using System.Xml;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	/// <summary></summary>
	public class EntityDictionary : Dictionary<string, IEntity>, IEntityDictionary
    {
        public static string ALIAS = "Entities";
        public EntityDictionary()
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

	public class EntityList : List<IEntity>, IEntityList
    {
        public static string ALIAS = "Entities";

        public EntityList()
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
    public class EntityNameList : List<string>, IEntityNameList
    {
        public static string ALIAS = "EntityNames";

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
    }
}
