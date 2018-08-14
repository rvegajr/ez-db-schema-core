using System;
namespace EzDbSchema.Core.Enums
{
    public enum RelationshipMultiplicityType
    {
        Unknown,
        OneToOne,
        OneToMany,
        ZeroOrOneToMany,
        ManyToOne,
        ManyToZeroOrOne,
        ZeroOrOneToOne,
        OneToZeroOrOne
    }
}
