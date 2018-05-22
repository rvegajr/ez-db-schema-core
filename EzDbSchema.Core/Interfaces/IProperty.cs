using System;
namespace EzDbSchema.Core.Interfaces
{
    public interface IProperty : IJson<IProperty>
    {
        string Name { get; set; }
        string Type { get; set; }
        int MaxLength { get; set; }
        int Precision { get; set; }
        int Scale { get; set; }
        bool IsNullable { get; set; }
        bool IsKey { get; set; }
        int KeyOrder { get; set; }
        bool IsIdentity { get; set; }
        IEntity Parent { get; set; }
        IRelationshipList RelatedTo { get; set; }
        ICustomAttributes CustomAttributes { get; set; }
    }
}
