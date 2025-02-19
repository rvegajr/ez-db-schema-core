using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Extentions.Objects;
using EzDbSchema.Core.Interfaces;
using Newtonsoft.Json;

namespace EzDbSchema.Core.Objects
{
	/// <summary></summary>
	public class Database : EzObject, IDatabase
    {
        internal static string ALIAS = "Schema";
        public override string DatabaseObjectName { get => Name; }
        private IEntityDictionary _entities = new EntityDictionary();

        // IDictionary implementation
        public ICollection<IEntity> Values => _entities.Values;
        public int Count => _entities.Count;
        public bool IsReadOnly => _entities.IsReadOnly;

        IEnumerable<IEntity> IDatabase.Values => _entities.Values;

        public void Add(KeyValuePair<string, IEntity> item) => _entities.Add(item.Key, item.Value);
        public void Clear() => _entities.Clear();
        public bool Contains(KeyValuePair<string, IEntity> item) => _entities.ContainsKey(item.Key) && _entities[item.Key].Equals(item.Value);
        public void CopyTo(KeyValuePair<string, IEntity>[] array, int arrayIndex)
        {
            int i = arrayIndex;
            foreach (var kvp in _entities)
            {
                array[i++] = new KeyValuePair<string, IEntity>(kvp.Key, kvp.Value);
            }
        }
        public bool Remove(KeyValuePair<string, IEntity> item)
        {
            if (Contains(item))
            {
                return _entities.Remove(item.Key);
            }
            return false;
        }
        public bool Remove(string key) => _entities.Remove(key);
        public bool TryGetValue(string key, out IEntity value) => _entities.TryGetValue(key, out value);
        public IEnumerator<KeyValuePair<string, IEntity>> GetEnumerator() => _entities.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _entities.GetEnumerator();

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
        
        public static IDatabase FromJson(string json, bool verbose = false)
        {
	        return JsonConvert.DeserializeObject<Database>(json, new JsonSerializerSettings {
		        PreserveReferencesHandling = PreserveReferencesHandling.All,
		        TypeNameHandling = TypeNameHandling.All
	        }).RebuildAllRelationshipPointers(verbose);
        }

        public static IDatabase FromJsonFile(string fileName, bool verbose = false)
        {
	        var settings = new JsonSerializerSettings
	        {
		        PreserveReferencesHandling = PreserveReferencesHandling.All,
		        TypeNameHandling = TypeNameHandling.All
	        };
	        return JsonConvert.DeserializeObject<IDatabase>(File.ReadAllText(fileName), settings).RebuildAllRelationshipPointers(verbose);
        }
        
        public void ToJsonFile(string FileName)
        {
            File.WriteAllText(FileName, this.AsJson());
        }

        /// <summary></summary>
        public bool ShowWarnings { get; set; } = false;

        /// <summary>If there are no primary keys for an entity,  database calls will have issues performing CRUD operations.  If an entity is rendered and has no primary key, 
        /// if this is set to 'true', then the schema renderer will automatically add a key for each field (which is the only way we can guaruntee uniqueness. 
        /// Default value is 'false'</summary>
        public bool AutoAddPrimaryKeys { get; set; } = false;

        /// <summary></summary>
        public string Name { get; set; } = "";
		/// <summary></summary>
		public IDatabaseObjectUpdates LastUpdates { get; set; } = new DatabaseObjectUpdates();
		/// <summary></summary>
		ICollection<string> IDictionary<string, IEntity>.Keys => _entities.Keys;

		/// <summary></summary>
		public IEntityNameList Keys => new EntityNameList(_entities.Keys);
	}
}
