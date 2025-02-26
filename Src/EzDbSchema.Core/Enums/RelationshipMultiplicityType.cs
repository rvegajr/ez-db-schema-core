namespace EzDbSchema.Core.Enums
{
    /// <summary>
    /// Specifies the multiplicity type of a relationship
    /// </summary>
    public enum RelationshipMultiplicityType
    {
        /// <summary>
        /// Unknown relationship type
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// One-to-One relationship
        /// </summary>
        OneToOne = 1,

        /// <summary>
        /// One-to-Many relationship
        /// </summary>
        OneToMany = 2,

        /// <summary>
        /// Zero-or-One-to-Many relationship
        /// </summary>
        ZeroOrOneToMany = 3,

        /// <summary>
        /// Many-to-One relationship
        /// </summary>
        ManyToOne = 4,

        /// <summary>
        /// Many-to-Zero-or-One relationship
        /// </summary>
        ManyToZeroOrOne = 5,

        /// <summary>
        /// Zero-or-One-to-One relationship
        /// </summary>
        ZeroOrOneToOne = 6,

        /// <summary>
        /// One-to-Zero-or-One relationship
        /// </summary>
        OneToZeroOrOne = 7,

        /// <summary>
        /// Many-to-Many relationship
        /// </summary>
        ManyToMany = 8
    }
}
