# AI Usage Guide for EzDbSchema

> This guide is specifically designed for AI models to effectively utilize the EzDbSchema library for database schema analysis and code generation.

This document provides structured information to help AI models effectively use the EzDbSchema library for code generation tasks.

## Core Concepts

### Security Features
```csharp
// Entity-level security
public interface IEntity
{
    bool RequiresAuthorization { get; }  // Entity requires auth
    bool HasRowLevelSecurity { get; }    // Row-level security enabled
    bool IsAuditable();                  // Supports audit trails
}

// Property-level security
public interface IProperty
{
    bool IsReadOnly { get; }     // Read-only access
    bool IsHidden { get; }       // Hidden from views
    bool IsVisible { get; set; } // Visible in UI
    bool IsEditable { get; set; }// Editable in forms
}
```

### UI Metadata
```csharp
public interface IProperty
{
    // UI Display
    string DisplayName { get; set; }      // Friendly name
    string PlaceholderText { get; set; }  // Input placeholder
    string HelpText { get; set; }         // Help tooltip
    string InputType { get; set; }        // Input field type
    
    // Validation
    bool IsRequired { get; set; }         // Required field
    int MaxLength { get; set; }           // Maximum length
    int Precision { get; set; }           // Numeric precision
    int Scale { get; set; }               // Decimal places
}
```

### Advanced Database Features
```csharp
public interface IEntity
{
    // Schema Information
    string DatabaseSchema { get; set; }   // Database schema
    string TemporalType { get; set; }     // Temporal table type
    bool IsTemporalView { get; set; }     // Is temporal view
    
    // Database Features
    bool HasTriggers { get; }             // Has triggers
    bool HasCheckConstraints { get; }     // Has check constraints
    bool HasForeignKeyConstraints { get; }// Has foreign keys
    bool HasUniqueIndexes { get; }        // Has unique indexes
}

// Advanced Relationship Features
public interface IRelationship
{
    string ConstraintName { get; set; }   // Constraint name
    bool IsOptional { get; set; }         // Optional relationship
    bool CascadeDelete { get; set; }      // Cascade deletes
    string CascadeAction { get; set; }    // Cascade action
    
    // Performance Features
    bool RequiresLazyLoading { get; set; }  // Use lazy loading
    bool RequiresEagerLoading { get; set; } // Use eager loading
    bool RequiresIndexing { get; set; }     // Needs indexing
}
```

### Database Schema Representation
The library represents database schemas as hierarchical objects with the following structure:
```csharp
IDatabase
└── Dictionary<string, IEntity>
    └── IEntity
        ├── string TableName
        ├── Dictionary<string, IProperty> Properties
        │   └── IProperty
        │       ├── string ColumnName
        │       ├── string PropertyName
        │       ├── bool IsNullable
        │       ├── bool IsIdentity
        │       ├── bool IsPrimaryKey
        │       ├── string DataType
        │       ├── int MaxLength
        │       ├── int Precision
        │       └── int Scale
        └── IRelationshipReferenceList Relationships
            └── IRelationship
                ├── string FromTableName
                ├── string ToTableName
                ├── string FromColumnName
                ├── string ToColumnName
                └── string Multiplicity
```

### Code Generation Context
The `DatabaseCodeGenInfo` class provides rich metadata about the database schema:
```csharp
DatabaseCodeGenInfo
├── IEnumerable<string> RequiredFrameworkFeatures
├── IDictionary<string, IEnumerable<string>> DependencyGraph
├── bool HasAuditableEntities
├── bool HasVersionedEntities
├── bool HasSoftDeletableEntities
└── bool HasCircularReferences
```

## Common Tasks and Code Patterns

### 1. Getting Database Schema
```csharp
// Best practice for connecting to database
var schema = new EzDbSchema.MsSql.Database().Render(
    "SchemaName",
    "Server=server;Database=db;User Id=user;Password=pass;TrustServerCertificate=True"
);

// Access entities and their properties
foreach (var entity in schema.Values)
{
    // Entity name
    string tableName = entity.TableName;
    
    // Properties (columns)
    foreach (var prop in entity.Properties.Values)
    {
        string columnName = prop.ColumnName;
        string dataType = prop.DataType;
        bool isNullable = prop.IsNullable;
    }
    
    // Relationships
    foreach (var rel in entity.Relationships)
    {
        string relatedTable = rel.ToTableName;
        string multiplicity = rel.Multiplicity;
    }
}
```

### 2. Detecting Database Patterns
```csharp
var codeGen = new DatabaseCodeGenInfo(schema);

// Check for common patterns
if (codeGen.HasAuditableEntities)
{
    // Generate audit trail functionality
    // Look for CreatedDate, ModifiedDate columns
}

if (codeGen.HasVersionedEntities)
{
    // Generate versioning support
    // Look for Version, RowVersion columns
}

if (codeGen.HasSoftDeletableEntities)
{
    // Generate soft delete functionality
    // Look for IsDeleted, DeletedDate columns
}
```

### 3. Handling Dependencies
```csharp
// Get dependency graph
var dependencies = codeGen.DependencyGraph;
// Returns: { "Order": ["Customer", "Product"], ... }

// Check for circular references
if (codeGen.HasCircularReferences)
{
    // Handle circular dependencies
    // Consider using lazy loading
    // Or breaking circular references in generated code
}
```

## Best Practices for Code Generation

1. **Schema Analysis**
   ```csharp
   // Always check for null and handle edge cases
   if (entity?.Properties?.Values != null)
   {
       foreach (var prop in entity.Properties.Values)
       {
           if (prop?.ColumnName == null) continue;
           // Process property
       }
   }
   ```

2. **Type Mapping**
   ```csharp
   // Common database to C# type mappings
   var typeMap = new Dictionary<string, string>
   {
       ["int"] = "int",
       ["bigint"] = "long",
       ["varchar"] = "string",
       ["datetime"] = "DateTime",
       ["bit"] = "bool"
   };
   ```

3. **Relationship Handling**
   ```csharp
   // Generate navigation properties
   if (relationship.Multiplicity == "OneToMany")
   {
       // Generate collection property
       $"public virtual ICollection<{toEntity}> {propertyName} {{ get; set; }}"
   }
   else
   {
       // Generate single navigation property
       $"public virtual {toEntity} {propertyName} {{ get; set; }}"
   }
   ```

## Common Pitfalls to Avoid

1. **Null Reference Handling**
   - Always check for null on `Properties`, `Relationships`, and their values
   - Use null-conditional operators (`?.`) and null-coalescing operators (`??`)

2. **Case Sensitivity**
   - Table and column names might be case-sensitive depending on the database
   - Use case-insensitive comparisons when appropriate: `string.Equals(x, y, StringComparison.OrdinalIgnoreCase)`

3. **Circular Dependencies**
   - Check `HasCircularReferences` before generating navigation properties
   - Consider using lazy loading or breaking circular references

4. **Data Type Mapping**
   - Handle all possible database types
   - Consider precision and scale for decimal types
   - Handle nullable types appropriately

## Schema JSON Structure
When dumping the schema to JSON for template engines like Handlebars:
```json
{
    "Name": "SchemaName",
    "Entities": {
        "TableName": {
            "TableName": "string",
            "Properties": {
                "ColumnName": {
                    "ColumnName": "string",
                    "PropertyName": "string",
                    "IsNullable": "bool",
                    "IsIdentity": "bool",
                    "IsPrimaryKey": "bool",
                    "DataType": "string",
                    "MaxLength": "int",
                    "Precision": "int",
                    "Scale": "int"
                }
            },
            "Relationships": [
                {
                    "FromTableName": "string",
                    "ToTableName": "string",
                    "FromColumnName": "string",
                    "ToColumnName": "string",
                    "Multiplicity": "string"
                }
            ],
            "HasCompositePrimaryKey": "bool",
            "IsAuditable": "bool",
            "IsVersioned": "bool",
            "IsSoftDeletable": "bool"
        }
    }
}
```

## Multi-Language Support

The library supports code generation in multiple programming languages through the `ILanguageDefinition` interface:

### Supported Languages
- C# (.cs)
- TypeScript (.ts)
- Python (.py)
- Go (.go)
- SQL (.sql)
- Java (.java)

### Language Features
```csharp
public interface ILanguageDefinition
{
    string Name { get; }
    string FileExtension { get; }
    string GetDataType(string dbType, int? length = null, int? precision = null, int? scale = null);
    string GetDefaultValue(string dbType);
    string GetNullableType(string type);
    string GetCollectionType(string type);
    string GetImportStatement(string import);
    string GetClassDeclaration(string name, string baseClass = null);
    string GetPropertyDeclaration(string name, string type, bool isNullable);
    string GetMethodDeclaration(string name, string returnType, params (string type, string name)[] parameters);
}
```

## ORM Template Support

The library includes built-in ORM templates for popular frameworks:

### Entity Framework Core (C#)
```csharp
// Entity configuration
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
    }
}
```

### TypeORM (TypeScript)
```typescript
@Entity("customers")
export class Customer {
    @PrimaryGeneratedColumn()
    id: number;

    @Column({ length: 100 })
    name: string;

    @ManyToOne(() => Order)
    orders: Order[];
}
```

### SQLAlchemy (Python)
```python
class Customer(Base):
    __tablename__ = "customers"
    
    id = Column(Integer, primary_key=True)
    name = Column(String(100), nullable=False)
    orders = relationship("Order", back_populates="customer")
```

## API Template Support

The library provides templates for generating API endpoints and documentation:

### REST API (Multiple Languages)
```csharp
// C# ASP.NET Core
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
    {
        // Implementation
    }
}
```

### GraphQL Schema
```graphql
type Customer {
    id: ID!
    name: String!
    orders: [Order!]
}

type Query {
    customers: [Customer!]!
    customer(id: ID!): Customer
}
```

### Swagger/OpenAPI Documentation
```yaml
paths:
  /api/customers:
    get:
      summary: Get all customers
      responses:
        '200':
          description: List of customers
          content:
            application/json:
              schema:
                type: array
                items:
                  $ref: '#/components/schemas/Customer'
```

## Template Generation

### Template Context
```csharp
public class DatabaseTemplateContext
{
    public string DatabaseName { get; set; }
    public IEnumerable<EntityTemplateView> Tables { get; }
    public IEnumerable<EntityTemplateView> Views { get; }
    public IDictionary<string, IEnumerable<EntityTemplateView>> EntitiesBySchema { get; }
}

public class EntityTemplateView
{
    public string TableName { get; }
    public string SchemaName { get; }
    public string ClassName { get; }
    public string ClassNamePlural { get; }
    public IEnumerable<PropertyTemplateView> Properties { get; }
    public IEnumerable<RelationshipTemplateView> Relationships { get; }
    public bool HasPrimaryKey { get; }
    public bool HasCompositeKey { get; }
    public bool IsAuditable { get; }
    public bool IsVersioned { get; }
}
```

### Template Generation Options
```csharp
public class GeneratorOptions
{
    public string TemplateDirectory { get; set; }  // Template location
    public string OutputDirectory { get; set; }    // Output location
    public string Language { get; set; }           // Target language
    public bool IncludeViews { get; set; }        // Include views
    public bool GenerateTests { get; set; }       // Generate tests
    public IDictionary<string, string> CustomOptions { get; set; }
}
```

### Custom Template Functions
```handlebars
{{! Entity helpers }}
{{#if (isAuditable entity)}}
    // Add audit fields
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
{{/if}}

{{! Relationship helpers }}
{{#each (getOneToManyRelationships entity)}}
    public virtual ICollection<{{toEntity}}> {{propertyName}} { get; set; }
{{/each}}

{{! Type mapping helpers }}
{{getLanguageType property.DataType}}
{{getNullableType property.DataType property.IsNullable}}
```

## Example Template Variables
When using template engines, these are common variables to consider:
```handlebars
{{#each Entities}}
    Table: {{TableName}}
    {{#each Properties}}
        Column: {{ColumnName}}
        Type: {{DataType}}
        IsNullable: {{IsNullable}}
        // Additional ORM-specific metadata
        HasIndex: {{HasIndex}}
        IsUnique: {{IsUnique}}
        HasDefaultValue: {{HasDefaultValue}}
        IsComputed: {{IsComputed}}
        IsConcurrencyToken: {{IsConcurrencyToken}}
    {{/each}}
    {{#each Relationships}}
        Related Table: {{ToTableName}}
        Type: {{Multiplicity}}
        // Additional relationship metadata
        IsCascadeDelete: {{IsCascadeDelete}}
        IsRequired: {{IsRequired}}
        ForeignKeyName: {{ForeignKeyName}}
    {{/each}}
    // Entity-level metadata
    HasTriggers: {{HasTriggers}}
    HasCheckConstraints: {{HasCheckConstraints}}
    HasForeignKeyConstraints: {{HasForeignKeyConstraints}}
    HasUniqueIndexes: {{HasUniqueIndexes}}
{{/each}}
```

## JSON Serialization and File Operations

### Serializing Database Schema
```csharp
// Save schema to JSON
string jsonSchema = database.AsJson();
File.WriteAllText("schema.json", jsonSchema);

// Load schema from JSON
var database = new Database();
database.FromJson(jsonSchema);

// Load schema from JSON file
database.FromJsonFile("schema.json");
```

### Schema File Structure
```json
{
    "DatabaseName": "MyDatabase",
    "Entities": {
        "Customer": {
            "TableName": "Customers",
            "SchemaName": "dbo",
            "Properties": {
                "Id": {
                    "ColumnName": "Id",
                    "DataType": "int",
                    "IsIdentity": true,
                    "IsPrimaryKey": true
                },
                "Name": {
                    "ColumnName": "Name",
                    "DataType": "nvarchar",
                    "MaxLength": 100,
                    "IsRequired": true
                }
            },
            "Relationships": [
                {
                    "Name": "FK_Orders_Customer",
                    "FromTable": "Orders",
                    "ToTable": "Customers",
                    "FromColumn": "CustomerId",
                    "ToColumn": "Id",
                    "Multiplicity": "OneToMany"
                }
            ]
        }
    }
}
