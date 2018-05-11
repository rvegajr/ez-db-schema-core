using System;
namespace EzDbSchema.Core.Enums
{
    public enum RelationshipType
    {
        OneToMany,
        ZeroOrOneToMany,
        ManyToOne,
        ManyToZeroOrOne,
        OneToOne
    }
}
