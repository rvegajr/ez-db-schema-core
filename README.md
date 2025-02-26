# EZDBSchema - Easy Database Schema

A powerful .NET library that provides comprehensive database schema analysis and code generation capabilities. It allows you to extract complete database schemas including columns, relationships, and advanced features detection, making it perfect for code generation and database documentation tasks.

## Key Features

- **Complete Schema Analysis**: Extract tables, columns, relationships, and constraints
- **Smart Feature Detection**: Automatically identifies common patterns like:
  - Auditable entities (CreatedDate, ModifiedDate)
  - Versioned entities (Version, RowVersion)
  - Soft-deletable entities (IsDeleted, DeletedDate)
  - Composite keys
  - Circular references
- **Dependency Graph Generation**: Analyze and visualize table dependencies
- **Code Generation Support**: Generate boilerplate code for:
  - ORM entities
  - API controllers
  - Database contexts
  - Repository patterns
- **Multi-Framework Support**: Compatible with .NET Standard 2.1, .NET 6.0, 7.0, and 8.0
- **Version**: 8.3.1

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## AI Integration

To facilitate AI-powered development and code generation, we provide a comprehensive [`AI-USAGE.md`](AI-USAGE.md) guide. This documentation is specifically designed to help AI models understand and work with the EzDbSchema library effectively.

### AI Documentation Features

- **Core Concepts**: Detailed schema structure and class hierarchies
- **Multi-Language Support**: Integration details for C#, TypeScript, Python, Go, SQL, and Java
- **ORM Templates**: Support for Entity Framework Core, TypeORM, SQLAlchemy, and more
- **API Generation**: Templates for REST APIs, GraphQL schemas, and Swagger/OpenAPI
- **Security Features**: Authorization, row-level security, and audit trails
- **UI Metadata**: Display properties, validation rules, and form generation
- **Advanced Features**: Temporal tables, relationships, and performance optimizations

AI models can leverage this documentation to:
- Generate accurate database-first code
- Create type-safe entity models
- Build complete API endpoints
- Implement proper validation and security
- Follow best practices for each supported language and framework

## Use Cases

1. **Rapid Application Development**
   - Generate complete data access layers
   - Create API endpoints from database schema
   - Scaffold CRUD operations

2. **Database Documentation**
   - Generate comprehensive schema documentation
   - Visualize table relationships
   - Track schema changes

3. **Database Migration**
   - Analyze database dependencies
   - Plan migration strategies
   - Generate migration scripts

4. **Code Generation**
   - Create strongly-typed entity classes
   - Generate data transfer objects (DTOs)
   - Build repository interfaces and implementations

5. **Architecture Analysis**
   - Identify circular dependencies
   - Analyze table relationships
   - Detect common patterns

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.  

### Prerequisites
You will need MSSQL with some database installed.  If you need a sample database,  feel free to look for the [World Wide Importers](https://github.com/Microsoft/sql-server-samples/releases/tag/wide-world-importers-v1.0) samples.

### Using this project:

### Installation

```bash
Install-Package EzDbSchema.Core
```

### Basic Usage

```csharp
// Connect to database and get schema
var schema = new EzDbSchema.MsSql.Database().Render(
    "MySchema",
    "Server=myserver;Database=mydb;User Id=sa;Password=****;TrustServerCertificate=True"
);

// Generate code for entities
var codeGen = new DatabaseCodeGenInfo(schema);

// Get required features
var features = codeGen.RequiredFrameworkFeatures;
// Returns: ["Auditing", "Versioning", etc.]

// Get dependency graph
var dependencies = codeGen.DependencyGraph;
// Returns: { "Order": ["Customer", "Product"], ... }
```

### Schema Dump for Handlebars Development

```csharp
// Get complete schema with all properties
var schema = new EzDbSchema.MsSql.Database().Render("MySchema", connectionString);

// Dump schema to JSON (useful for Handlebars template development)
var schemaJson = Newtonsoft.Json.JsonConvert.SerializeObject(schema, Newtonsoft.Json.Formatting.Indented);
File.WriteAllText("schema.json", schemaJson);

// Available Schema Properties:
/*
{
    "Name": "MySchema",
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
                    "Scale": "int",
                    "HasValidationRules": "bool"
                }
            },
            "Relationships": [
                {
                    "FromTableName": "string",
                    "ToTableName": "string",
                    "FromColumnName": "string",
                    "ToColumnName": "string",
                    "RelationshipName": "string",
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
*/
```

### Advanced Features

```csharp
// Check for specific patterns
var hasAuditableEntities = codeGen.HasAuditableEntities;
var hasVersionedEntities = codeGen.HasVersionedEntities;
var hasSoftDelete = codeGen.HasSoftDeletableEntities;

// Generate API controllers
var apiCode = codeGen.GenerateApiControllers();

// Generate ORM entities
var ormCode = codeGen.GenerateOrmEntities();
```

### CLI Usage

1. Configure connection in `EzDbSchema.Cli/appsettings.json`:
```json
{
    "ConnectionString": "Server=myserver;Database=mydb;User Id=sa;Password=****;TrustServerCertificate=True"
}
```

2. Run the CLI:
```bash
dotnet EzDbSchema.Cli.dll --schema MySchema
```

This will generate `MySchema.db.json` with the complete schema analysis.

## Deployment

This project was design to be hosted and distributed with nuget.com.

## Built With

* [.net core](https://www.microsoft.com/net/learn/get-started) - The framework used

## Contributing

Please read [CONTRIBUTING.md](https://gist.github.com/rvegajr/651875c08acb76009e563db128f33e7e) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/rvegajr/tags). 

## Authors

* **Ricky Vega** - *Initial work* - [Noctusoft](https://github.com/rvegajr)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

Many thanks to the following projects that have helped in this project
* McMaster.Extensions.CommandLineUtils

## Release Notes

### V 8.3.1 (2025-02-26)
- Updated to target .NET 8.0
- Fixed RelationshipMultiplicityType enum ordering
- Improved nullable reference type handling

### V 8.1.0
- Nuget package upgrades, updated to .NET 8.0
- Update to Microsoft SqlClient
- Added Server Trust Cert setting

### V 7.0.0
- Nuget package upgrades, updated to .NET 7.0

### V 6.0.1
- Added the ability to tell the generator to not auto create the primary keys if they are missing

### V 6.0.0
- Migration to .NET 6.0
