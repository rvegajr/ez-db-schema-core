using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Extentions.Objects;
using EzDbSchema.Core.Extentions.Xml;
using EzDbSchema.Core.Interfaces;
using Newtonsoft.Json;

namespace EzDbSchema.Core.Objects
{
	/// <summary></summary>
	public class Database : EzObject, IDatabase, IXmlRenderableInternal
    {
        internal static string ALIAS = "Schema";

        private IEntityDictionary _entities = new EntityDictionary();

		/// <summary></summary>
        public Database() : base()
        {
			this.ShowWarnings = false;
        }

		/// <summary></summary>
		public IEntity this[string entityName]
        {
            get { return _entities[entityName]; }
            set { _entities[entityName] = value; }
        }

		/// <summary></summary>
		public IEntityDictionary Entities
        {
            get { return _entities; }
            set { _entities = value; }
        }

		/// <summary></summary>
		public void Add(string entityName, IEntity entity)
		{
			if (_entities.ContainsKey(entityName))
            {
                _entities.Remove(entityName);
            }
            _entities.Add(entityName, entity);
		}

		/// <summary></summary>
		public bool ContainsKey(string entityName)
		{
			return _entities.ContainsKey(entityName);
		}

		/// <summary></summary>
		public bool ContainsValue(IEntity entity)
		{
			return _entities.Values.Contains(entity);
		}

		/// <summary>This must be overridded by a classed that will inherit from this class</summary>
		public virtual IDatabase Render(string entityName, string ConnectionString)
		{
			throw new NotImplementedException();
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

        public void FromXml(string Xml)
        {
            var doc = (new XmlDocument());
            doc.LoadXml(Xml);
            FromXml(doc.FirstChild);
        }

        public XmlNode FromXml(XmlNode node)
        {
            ObjectExtensions.ClearRef();
            this.FromXmlNode(node, ALIAS);
            //restore referenced objects
            foreach(IEntity e in this.Entities.Values)
            {
                foreach (var p in e.Properties.Values) {
                    p.Parent = e;
                    foreach (var r in p.RelatedTo)
                        r.Parent = e;
                }
                var keys = new PrimaryKeyProperties();
                foreach (var k in e.PrimaryKeys)
                {
                    if (ObjectExtensions.RefObjectXref.ContainsKey(k._id * -1))
                    {
                        keys.Add((IProperty)ObjectExtensions.RefObjectXref[k._id * -1]);
                    }
                }
                e.PrimaryKeys = keys;
                var rel = new RelationshipReferenceList();
                foreach (var k in e.Relationships)
                {
                    if (ObjectExtensions.RefObjectXref.ContainsKey(k._id * -1))
                    {
                        rel.Add((IRelationship)ObjectExtensions.RefObjectXref[k._id * -1]);
                    }
                }
                e.Relationships = rel;
            }
            return node;
        }

        public string AsJson()
        {
            return JsonConvert.SerializeObject(
                this
                , Newtonsoft.Json.Formatting.Indented
                , new JsonSerializerSettings {
                    PreserveReferencesHandling = PreserveReferencesHandling.All,
                    TypeNameHandling = TypeNameHandling.All
                });
        }

        internal static IDatabase FromJson(string Json)
        {
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };
            return JsonConvert.DeserializeObject<Database>(Json, settings);
        }

        internal static IDatabase FromJsonFile(string FileName)
        {
            return JsonConvert.DeserializeObject<Database>(File.ReadAllText(FileName));
        }

        internal static T FromJson<T>(string Json)
        {
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };
            return JsonConvert.DeserializeObject<T>(Json, settings);
        }

        internal static T FromJsonFile<T>(string FileName)
        {
            return FromJson<T>(File.ReadAllText(FileName));
        }

        public void ToJsonFile(string FileName)
        {
            File.WriteAllText(FileName, this.AsJson());
        }

        /// <summary></summary>
        public bool ShowWarnings { get; set; } = false;
		/// <summary></summary>
		public string Name { get; set; } = "";
		/// <summary></summary>
		public IDatabaseObjectUpdates LastUpdates { get; set; } = new DatabaseObjectUpdates();
		/// <summary></summary>
		public IEntityNameList Keys
        {
            get
            {
                return new EntityNameList(_entities.Keys);
            }
        }
	}
}
