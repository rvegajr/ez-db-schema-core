using System;
using System.Collections.Generic;
using System.Text;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	/// <summary></summary>
	public abstract class Database : IDatabase, IJson<IDatabase>
    {
		private IEntityDictionary _entities = new EntityDictionary();

		/// <summary></summary>
        public Database()
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

        /// <summary></summary>
        public bool ShowWarnings { get; set; } = false;
		/// <summary></summary>
		public string Name { get; set; } = "";
		/// <summary></summary>
		public IDatabaseObjectUpdates LastUpdates { get; set; } = new DatabaseObjectUpdates();
		/// <summary></summary>
		public ICollection<string> Keys
        {
            get
            {
                return _entities.Keys;
            }
        }
	}
}
