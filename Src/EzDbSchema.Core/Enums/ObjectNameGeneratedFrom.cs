namespace EzDbSchema.Core.Enums
{
    /// <summary>
    /// Specifies the source from which an object name is generated
    /// </summary>
    public enum ObjectNameGeneratedFrom
    {
        /// <summary>
        /// Generate object name from the join's from column name
        /// </summary>
        JoinFromColumnName,
        
        /// <summary>
        /// Generate object name from the unique column name in the target table
        /// </summary>
        ToUniqueColumnName,
        
        /// <summary>
        /// Generate object name from the target table name
        /// </summary>
        ToTableName,
        
        /// <summary>
        /// Generate object name from the source table name
        /// </summary>
        FromTableName,
        
        /// <summary>
        /// Generate object name from the join's to column name
        /// </summary>
        JoinToColumnName
    }
}
