using System;
using System.Collections.Generic;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Extentions.Objects;
using EzDbSchema.Core.Interfaces;

namespace EzDbSchema.Core.Objects
{
    public enum ObjectType
    {
        Database,
        Entity,
        EntityDictionary,
        EntityList,
        Property,
        PropertyList,
        PropertyDictionary,
        Relationship,
        RelationshipList,
        RelationshipDictionary,
        CustomAttributes
    }
    /// <summary></summary>
    public class EzObject : IEzObject
    {
        public EzObject()
        {
            this._id = this.GetId();
            this.IsEnabled = true;
        }
        public int _id { get; set; }
        public bool IsEnabled { get; set; } = true;
        public ICustomAttributes CustomAttributes { get; set; }

        internal static object CreateInstance(string name)
        {
            switch (name)
            {
                case "Database": case "IDatabase": throw new Exception("Cannot create instance of Database"); 
                case "Entity": case "IEntity": return new Entity();
                case "EntityList": case "IEntityList": return new EntityList();
                case "EntityDictionary": case "IEntityDictionary": return new EntityDictionary();
                case "Property": case "IProperty": return new Property();
                case "PropertyList": case "IPropertyList": return new PropertyList();
                case "PropertyDictionary": case "IPropertyDictionary": return new PropertyDictionary();
                case "Relationship": case "IRelationship": return new Relationship();
                case "RelationshipList": case "IRelationshipList": return new RelationshipList();
                case "RelationshipReferenceList": case "IRelationshipReferenceList": return new RelationshipReferenceList();
                case "RelationshipDictionary": case "IRelationshipDictionary": return new RelationshipDictionary();
                case "CustomAttributes": case "ICustomAttributes": return new CustomAttributes();
            }
            return null;
        }
    }
}
