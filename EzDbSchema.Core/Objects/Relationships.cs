using System;
using System.Collections.Generic;
using System.Linq;
using EzDbSchema.Core.Enums;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
	
	public class RelationshipDictionary : Dictionary<string, IRelationship>, IRelationshipDictionary
    {
    }

	public class RelationshipList : List<IRelationship>, IRelationshipList
    {
		public IRelationshipList Fetch(RelationshipType TypeToFetch)
        {
			IRelationshipList retListRaw = new RelationshipList();
			if (TypeToFetch == RelationshipType.OneToMany) retListRaw = (EzDbSchema.Core.Objects.RelationshipList)this.Where(r => r.Type == "One to Many").ToList();
			else if (TypeToFetch == RelationshipType.ZeroOrOneToMany) retListRaw = (EzDbSchema.Core.Objects.RelationshipList)this.Where(r => ((r.Type == "One to Many") || (r.Type == "ZeroOrOne to Many"))).ToList();
			else if (TypeToFetch == RelationshipType.ManyToOne) retListRaw = (EzDbSchema.Core.Objects.RelationshipList)this.Where(r => r.Type == "Many to One").ToList();
			else if (TypeToFetch == RelationshipType.ManyToZeroOrOne) retListRaw = (EzDbSchema.Core.Objects.RelationshipList)this.Where(r => ((r.Type == "Many to ZeroOrOne") || (r.Type == "Many to One"))).ToList();
			else if (TypeToFetch == RelationshipType.OneToOne) retListRaw = (EzDbSchema.Core.Objects.RelationshipList)this.Where(r => (r.Type == "One to One")).ToList();
			IRelationshipList retList = new RelationshipList();
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
    }
}
