using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using EzDbSchema.Core.Enums;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Extentions.Objects;
using EzDbSchema.Core.Extentions.Xml;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	
	public class RelationshipDictionary : Dictionary<string, IRelationship>, IRelationshipDictionary, IXmlRenderableInternal
    {
        internal static string ALIAS = "Relationships";

        public RelationshipDictionary()
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

    public class RelationshipReferenceList : RelationshipList, IRelationshipReferenceList, IXmlRenderableInternal
    {
        public new static string ALIAS = "Relationships";
        public new XmlNode AsXml(XmlDocument doc)
        {
            XmlNode nod = doc.CreateElement(XmlConvert.EncodeName(ALIAS));
            foreach (var item in this)
            {
                XmlNode refnod = nod.OwnerDocument.CreateElement(Relationship.ALIAS);
                ((XmlElement)refnod).SetAttribute("ref", item._id.ToSafeString());
                nod.AppendChild(refnod);

            }
            return nod;
        }

        public new string AsXml()
        {
            return AsXml(new XmlDocument()).OuterXml;
        }

        public new void FromXml(string Xml)
        {
            var doc = (new XmlDocument());
            doc.LoadXml(Xml);
            FromXml(doc.FirstChild);
        }

        public new XmlNode FromXml(XmlNode node)
        {
            this.ListFromXmlNodeList(node.ChildNodes, ALIAS);
            return node;
        }
    }
    /// <summary>
    /// A group of relationship lists that are group by foriegn key name,  this allows the schema to represent composite foriegn keys
    /// </summary>
    public class RelationshipGroups : Dictionary<string, IRelationshipList>, IRelationshipGroups, IXmlRenderableInternal
    {
        internal static string ALIAS = "RelationshipGroups";

        public RelationshipGroups()
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
    public class RelationshipList : List<IRelationship>, IRelationshipList, IXmlRenderableInternal
    {
        internal static string ALIAS = "RelatedTo";

        public RelationshipList()
        {
            this._id = this.GetId();
            this.IsEnabled = true;
        }
        public int _id { get; set; }
        public bool IsEnabled { get; set; } = true;
        public ICustomAttributes CustomAttributes { get; set; }

        public IRelationshipList Fetch(RelationshipMultiplicityType TypeToFetch)
        {
            var retListRaw = new List<IRelationship>();
			if (TypeToFetch == RelationshipMultiplicityType.OneToMany) retListRaw = this.Where(r => r.Type == "One to Many").ToList();
			else if (TypeToFetch == RelationshipMultiplicityType.ZeroOrOneToMany) retListRaw = this.Where(r => ((r.Type == "One to Many") || (r.Type == "ZeroOrOne to Many"))).ToList();
			else if (TypeToFetch == RelationshipMultiplicityType.ManyToOne) retListRaw = this.Where(r => r.Type == "Many to One").ToList();
			else if (TypeToFetch == RelationshipMultiplicityType.ManyToZeroOrOne) retListRaw = this.Where(r => ((r.Type == "Many to ZeroOrOne") || (r.Type == "Many to One"))).ToList();
			else if (TypeToFetch == RelationshipMultiplicityType.OneToOne) retListRaw = this.Where(r => (r.Type == "One to One")).ToList();
            else if (TypeToFetch == RelationshipMultiplicityType.OneToZeroOrOne) retListRaw = this.Where(r => ((r.Type == "One to ZeroOrOne") || (r.Type == "One to One"))).ToList();
            else if (TypeToFetch == RelationshipMultiplicityType.ZeroOrOneToOne) retListRaw = this.Where(r => ((r.Type == "ZeroOrOne to One") || (r.Type == "One to One"))).ToList();

            var retList = new RelationshipList();
            if (retListRaw.Count > 0)
            {
                //this should sort equal field names before non equal field names,  this will handle relationship with duplicate names correctly
                foreach (var item in retListRaw) if (item.ToFieldName.Equals(item.FromFieldName)) retList.Add(item);
                foreach (var item in retListRaw) if (!item.ToFieldName.Equals(item.FromFieldName)) retList.Add(item);
            }

            return retList;
        }

		public int CountItems(string searchFor)
        {
            return CountItems(RelationSearchField.ToTableName, searchFor);
        }

        public int CountItems(RelationSearchField searchField, string searchFor)
        {
			var list = this;
            if (searchField == RelationSearchField.ToTableName) return list.Count(r => r.ToTableName == searchFor);
            else if (searchField == RelationSearchField.ToColumnName) return list.Count(r => r.ToColumnName == searchFor);
            else if (searchField == RelationSearchField.ToFieldName) return list.Count(r => r.ToFieldName == searchFor);
            else if (searchField == RelationSearchField.FromTableName) return list.Count(r => r.FromTableName == searchFor);
            else if (searchField == RelationSearchField.FromFieldName) return list.Count(r => r.FromFieldName == searchFor);
            else if (searchField == RelationSearchField.FromColumnName) return list.Count(r => r.FromColumnName == searchFor);
            else return 0;
        }

        public IRelationshipList FindItems(string searchFor)
        {
            return FindItems(RelationSearchField.ToTableName, searchFor);
        }

        public IRelationshipList FindItems(RelationSearchField searchField, string searchFor)
        {
            var list = new RelationshipList();
            foreach (var item in this)
            {
                if ((searchField == RelationSearchField.ToTableName) && (item.ToTableName == searchFor)) list.Add(item);
                else if ((searchField == RelationSearchField.ToColumnName) && (item.ToColumnName == searchFor)) list.Add(item);
                else if ((searchField == RelationSearchField.ToFieldName) && (item.ToFieldName == searchFor)) list.Add(item);
                else if ((searchField == RelationSearchField.FromTableName) && (item.FromTableName == searchFor)) list.Add(item);
                else if ((searchField == RelationSearchField.FromFieldName) && (item.FromFieldName == searchFor)) list.Add(item);
                else if ((searchField == RelationSearchField.FromColumnName) && (item.FromColumnName == searchFor)) list.Add(item);
            }
            return list;
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
            this.ListFromXmlNodeList(node.ChildNodes, ALIAS);
            return node;
        }
    }
}
