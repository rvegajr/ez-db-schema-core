namespace EzDbSchema.Core.Enums
{
    /// <summary>
    /// Specifies the field to search in a relationship
    /// </summary>
    public enum RelationSearchField
    {
        /// <summary>
        /// Search in the ToTableName field
        /// </summary>
        ToTableName,

        /// <summary>
        /// Search in the ToColumnName field
        /// </summary>
        ToColumnName,

        /// <summary>
        /// Search in the ToFieldName field
        /// </summary>
        ToFieldName,

        /// <summary>
        /// Search in the FromTableName field
        /// </summary>
        FromTableName,

        /// <summary>
        /// Search in the FromFieldName field
        /// </summary>
        FromFieldName,

        /// <summary>
        /// Search in the FromColumnName field
        /// </summary>
        FromColumnName
    }
}
