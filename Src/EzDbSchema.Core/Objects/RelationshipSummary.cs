namespace EzDbSchema.Core.Objects
{
    /// <summary>
    /// Represents a summary of relationships with the same constraint name
    /// </summary>
    public class RelationshipSummary
    {
        /// <summary>
        /// Gets or sets the entity associated with this relationship summary
        /// </summary>
        public IEntity Entity { get; set; }
        
        /// <summary>
        /// Gets or sets the list of from property names
        /// </summary>
        public List<string> FromPropertyName { get; set; } = new List<string>();
        
        /// <summary>
        /// Gets or sets the list of from column names
        /// </summary>
        public List<string> FromColumnName { get; set; } = new List<string>();
        
        /// <summary>
        /// Gets or sets the list of to property names
        /// </summary>
        public List<string> ToPropertyName { get; set; } = new List<string>();
        
        /// <summary>
        /// Gets or sets the list of to column names
        /// </summary>
        public List<string> ToColumnName { get; set; } = new List<string>();
        
        /// <summary>
        /// Gets or sets the list of to column properties
        /// </summary>
        public List<IProperty> ToColumnProperties { get; set; } = new List<IProperty>();
        
        /// <summary>
        /// Gets or sets the list of to object property names
        /// </summary>
        public List<string> ToObjectPropertyName { get; set; } = new List<string>();
        
        /// <summary>
        /// Gets or sets the list of relationship types
        /// </summary>
        public List<string> Types { get; set; } = new List<string>();
        
        /// <summary>
        /// Gets or sets the list of multiplicity types
        /// </summary>
        public List<RelationshipMultiplicityType> MultiplicityTypes { get; set; } = new List<RelationshipMultiplicityType>();
        
        /// <summary>
        /// Gets or sets the from table name
        /// </summary>
        public string FromTableName { get; set; } = "";
        
        /// <summary>
        /// Gets or sets the constraint name
        /// </summary>
        public string ConstraintName { get; set; } = "";
        
        /// <summary>
        /// Gets or sets the to table name
        /// </summary>
        public string ToTableName { get; set; } = "";
        
        /// <summary>
        /// Gets or sets the primary table name
        /// </summary>
        public string PrimaryTableName { get; set; } = "";
        
        /// <summary>
        /// Gets or sets the list of from column properties
        /// </summary>
        public List<IProperty> FromColumnProperties { get; set; } = new List<IProperty>();
        
        /// <summary>
        /// Gets or sets the multiplicity type
        /// </summary>
        public RelationshipMultiplicityType MultiplicityType { get; set; } = RelationshipMultiplicityType.Unknown;
        
        /// <summary>
        /// Gets or sets the list of from object property names
        /// </summary>
        public List<string> FromObjectPropertyName { get; set; } = new List<string>();
        
        /// <summary>
        /// Gets or sets the relationship type
        /// </summary>
        public string Type { get; set; } = "";
        
        /// <summary>
        /// Gets or sets a value indicating there is a multiplicity type warning.  This means that of the relationships that participate, they have the following pattern:
        ///     ((ret.MultiplicityType == RelationshipMultiplicityType.ManyToZeroOrOne) || (ret.MultiplicityType == RelationshipMultiplicityType.ManyToOne)) &&
        ///      ((relationship.MultiplicityType == RelationshipMultiplicityType.ManyToOne) || (relationship.MultiplicityType == RelationshipMultiplicityType.ManyToZeroOrOne))
        ///   OR ((ret.MultiplicityType == RelationshipMultiplicityType.ZeroOrOneToMany) || (ret.MultiplicityType == RelationshipMultiplicityType.OneToMany)) &&
        ///      ((relationship.MultiplicityType == RelationshipMultiplicityType.OneToMany) || (relationship.MultiplicityType == RelationshipMultiplicityType.ZeroOrOneToMany))
        /// </summary>
        /// <value>
        ///   <c>true</c> if [multiplicity type warning]; otherwise, <c>false</c>.
        /// </value>
        public bool MultiplicityTypeWarning { get; set; } = false;

        /// <summary>
        /// Gets a unique column name for the relationship
        /// </summary>
        /// <param name="useFromColumn">Whether to use the from column (true) or to column (false)</param>
        /// <returns>A unique column name</returns>
        public string ToUniqueColumnName(bool useFromColumn = true)
        {
            return useFromColumn 
                ? string.Join("_", FromColumnName) 
                : string.Join("_", ToColumnName);
        }
    }
}
