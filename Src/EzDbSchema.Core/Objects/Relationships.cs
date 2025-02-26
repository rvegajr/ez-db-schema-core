using System;
using System.Collections.Generic;
using System.Linq;
using EzDbSchema.Core.Enums;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Extentions.Objects;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	
	public class RelationshipDictionary : Dictionary<string, IRelationship>, IRelationshipDictionary
    {
        public string DatabaseObjectName { get => null; }

        internal static string ALIAS = "Relationships";

        public RelationshipDictionary()
        {
            this._id = this.GetId();
            this.IsEnabled = true;
        }
        public int _id { get; set; }
        public bool IsEnabled { get; set; } = true;
        public ICustomAttributes CustomAttributes { get; set; }
    }

    public class RelationshipReferenceList : RelationshipList, IRelationshipReferenceList
    {
        public new static string ALIAS = "Relationships";
    }
    /// <summary>
    /// A group of relationship lists that are group by foriegn key name,  this allows the schema to represent composite foriegn keys
    /// </summary>
    public class RelationshipGroups : Dictionary<string, IRelationshipList>, IRelationshipGroups
    {
        public string DatabaseObjectName { get; set; } = string.Empty;
        internal static string ALIAS = "RelationshipGroups";

        public RelationshipGroups()
        {
            this._id = this.GetId();
            this.IsEnabled = true;
        }
        public int _id { get; set; }
        public bool IsEnabled { get; set; } = true;
        public ICustomAttributes CustomAttributes { get; set; }
        /// <summary>
        /// Gets or sets the database that contains these relationship groups.
        /// </summary>
        public IDatabase Database { get; set; }
    }
    public class RelationshipList : List<IRelationship>, IRelationshipList
    {
        public string DatabaseObjectName { get; set; } = string.Empty;

        internal static string ALIAS = "RelatedTo";

        public RelationshipList()
        {
            this._id = this.GetId();
            this.IsEnabled = true;
        }
        public int _id { get; set; }
        public bool IsEnabled { get; set; } = true;
        public IDatabase Database { get; set; }
        public ICustomAttributes CustomAttributes { get; set; }

        public IRelationshipList Fetch(RelationshipMultiplicityType TypeToFetch)
        {
            var retListRaw = new List<IRelationship>();
			if (TypeToFetch == RelationshipMultiplicityType.OneToMany) retListRaw = this.Where(r => r.RelationshipType == "One to Many").ToList();
			else if (TypeToFetch == RelationshipMultiplicityType.ZeroOrOneToMany) retListRaw = this.Where(r => ((r.RelationshipType == "One to Many") || (r.RelationshipType == "ZeroOrOne to Many"))).ToList();
			else if (TypeToFetch == RelationshipMultiplicityType.ManyToOne) retListRaw = this.Where(r => r.RelationshipType == "Many to One").ToList();
			else if (TypeToFetch == RelationshipMultiplicityType.ManyToZeroOrOne) retListRaw = this.Where(r => ((r.RelationshipType == "Many to ZeroOrOne") || (r.RelationshipType == "Many to One"))).ToList();
			else if (TypeToFetch == RelationshipMultiplicityType.OneToOne) retListRaw = this.Where(r => (r.RelationshipType == "One to One")).ToList();
            else if (TypeToFetch == RelationshipMultiplicityType.OneToZeroOrOne) retListRaw = this.Where(r => ((r.RelationshipType == "One to ZeroOrOne") || (r.RelationshipType == "One to One"))).ToList();
            else if (TypeToFetch == RelationshipMultiplicityType.ZeroOrOneToOne) retListRaw = this.Where(r => ((r.RelationshipType == "ZeroOrOne to One") || (r.RelationshipType == "One to One"))).ToList();

            var retList = new RelationshipList();
            if (retListRaw.Count > 0)
            {
                //this should sort equal field names before non equal field names,  this will handle relationship with duplicate names correctly
                foreach (var item in retListRaw) if (item.ToPropertyName.Equals(item.FromPropertyName)) retList.Add(item);
                foreach (var item in retListRaw) if (!item.ToPropertyName.Equals(item.FromPropertyName)) retList.Add(item);
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
            else if (searchField == RelationSearchField.ToFieldName) return list.Count(r => r.ToPropertyName == searchFor);
            else if (searchField == RelationSearchField.FromTableName) return list.Count(r => r.FromTableName == searchFor);
            else if (searchField == RelationSearchField.FromFieldName) return list.Count(r => r.FromPropertyName == searchFor);
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
                else if ((searchField == RelationSearchField.ToFieldName) && (item.ToPropertyName == searchFor)) list.Add(item);
                else if ((searchField == RelationSearchField.FromTableName) && (item.FromTableName == searchFor)) list.Add(item);
                else if ((searchField == RelationSearchField.FromFieldName) && (item.FromPropertyName == searchFor)) list.Add(item);
                else if ((searchField == RelationSearchField.FromColumnName) && (item.FromColumnName == searchFor)) list.Add(item);
            }
            return list;
        }
    }
}
