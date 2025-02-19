# EzDbSchema CLI Tool

A command-line interface for EzDbSchema that allows you to analyze database schemas, generate code, and export schema documentation.

## Installation

### Using .NET Tool
```bash
dotnet tool install --global EzDbSchema.Cli
```

### Building from Source
```bash
git clone https://github.com/rvegajr/ez-db-schema-core.git
cd ez-db-schema-core/Src/EzDbSchema.Cli
dotnet build
dotnet pack
```

## Usage

### Basic Commands

1. **Analyze Database Schema**
```bash
ezdb analyze --connection "Server=myserver;Database=mydb;User Id=sa;Password=****;TrustServerCertificate=True"
```

2. **Generate Code**
```bash
ezdb generate --connection "..." --language csharp --output ./output
```

3. **Export Schema Documentation**
```bash
ezdb export --connection "..." --format markdown --output schema.md
```

### Command Options

#### Analyze Command
```bash
ezdb analyze [options]
```
Options:
- `--connection, -c`: Connection string (required)
- `--output, -o`: Output file path for schema JSON
- `--schema, -s`: Database schema name (default: dbo)
- `--verbose, -v`: Enable verbose output

#### Generate Command
```bash
ezdb generate [options]
```
Options:
- `--connection, -c`: Connection string (required)
- `--language, -l`: Target language (csharp, typescript, python, go, java)
- `--output, -o`: Output directory (required)
- `--template, -t`: Template directory
- `--namespace, -n`: Root namespace
- `--orm`: ORM framework (ef, typeorm, sqlalchemy)
- `--include-views`: Include database views
- `--generate-tests`: Generate unit tests

#### Export Command
```bash
ezdb export [options]
```
Options:
- `--connection, -c`: Connection string (required)
- `--format, -f`: Output format (json, markdown, html)
- `--output, -o`: Output file path
- `--include-relationships`: Include relationship diagrams
- `--include-constraints`: Include constraint details

### Examples

1. **Generate C# Entity Framework Models**
```bash
ezdb generate \
  --connection "Server=localhost;Database=northwind;Trusted_Connection=True;" \
  --language csharp \
  --orm ef \
  --output ./Models \
  --namespace MyApp.Domain \
  --include-views
```

2. **Generate TypeScript Models with TypeORM**
```bash
ezdb generate \
  --connection "..." \
  --language typescript \
  --orm typeorm \
  --output ./src/models
```

3. **Export Markdown Documentation**
```bash
ezdb export \
  --connection "..." \
  --format markdown \
  --output docs/schema.md \
  --include-relationships \
  --include-constraints
```

4. **Analyze and Save Schema**
```bash
ezdb analyze \
  --connection "..." \
  --output schema.json \
  --schema dbo \
  --verbose
```

## Configuration File

You can create an `ezdb.json` configuration file to store common settings:

```json
{
  "connectionStrings": {
    "dev": "Server=dev-db;Database=myapp;...",
    "prod": "Server=prod-db;Database=myapp;..."
  },
  "defaultLanguage": "csharp",
  "defaultOrm": "ef",
  "templates": {
    "path": "./templates",
    "customMappings": {
      "datetime": "DateTimeOffset",
      "varchar": "string"
    }
  },
  "output": {
    "path": "./generated",
    "cleanDirectory": true
  }
}
```

Then use it with:
```bash
ezdb generate --config ezdb.json --connection-name dev
```

## Templates

EzDbSchema CLI supports custom templates using Handlebars syntax. Create a template directory with:

```
templates/
  ├── entity/
  │   ├── class.hbs
  │   ├── interface.hbs
  │   └── dto.hbs
  ├── dbcontext/
  │   └── context.hbs
  └── helpers/
      └── types.hbs
```

Use custom templates with:
```bash
ezdb generate --template ./templates
```

## Environment Variables

- `EZDB_CONNECTION_STRING`: Default connection string
- `EZDB_OUTPUT_PATH`: Default output path
- `EZDB_TEMPLATE_PATH`: Default template path
- `EZDB_CONFIG_FILE`: Default config file path

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.
