using System;
using System.Collections.Generic;
using System.Linq;
using EzDbSchema.Core.Interfaces;
using EzDbSchema.Core.Templates;

namespace EzDbSchema.Core.CodeGen
{
    public class DatabaseCodeGenInfo
    {
        private readonly IDatabase _database;
        private readonly string _language;

        public DatabaseCodeGenInfo(IDatabase database, string language = "C#")
        {
            _database = database;
            _language = language;
        }

        // Basic Database Info
        public IEnumerable<EntityCodeGenInfo> Entities => 
            _database.Values.Select(e => new EntityCodeGenInfo(e, _language));

        public IEnumerable<EntityCodeGenInfo> Tables => 
            Entities.Where(e => e.IsTable);

        public IEnumerable<EntityCodeGenInfo> Views => 
            Entities.Where(e => e.IsView);

        // Schema Organization
        public IEnumerable<string> Schemas =>
            _database.Values.Select(e => e.DatabaseSchema).Distinct();

        public IDictionary<string, IEnumerable<EntityCodeGenInfo>> EntitiesBySchema =>
            Entities.GroupBy(e => e.SchemaName)
                   .ToDictionary(g => g.Key, g => g.AsEnumerable());

        // Dependency Analysis
        public IEnumerable<EntityCodeGenInfo> RootEntities =>
            Entities.Where(e => !e.ManyToOneRelationships.Any());

        public IEnumerable<EntityCodeGenInfo> LeafEntities =>
            Entities.Where(e => !e.OneToManyRelationships.Any());

        public IDictionary<string, IEnumerable<string>> DependencyGraph =>
            BuildDependencyGraph();

        // Framework-Specific Info
        public string DefaultNamespace { get; set; } = "YourNamespace";
        public string ProjectName { get; set; } = "YourProject";
        public string DatabaseName => _database.Values.FirstOrDefault()?.ParentDatabase?.ToString() ?? "Database";

        // API Layer Info
        public string ApiVersion { get; set; } = "v1";
        public string BaseApiPath => $"/api/{ApiVersion}";
        public IEnumerable<string> ApiTags => Entities.Select(e => e.ApiTag).Distinct();

        // Common Patterns Detection
        public bool HasAuditableEntities => _database.Values.Any(e => e.IsAuditableEntity());
        public bool HasSoftDeletableEntities => _database.Values.Any(e => e.IsSoftDeletableEntity());
        public bool HasVersionedEntities => _database.Values.Any(e => e.IsVersionedEntity());
        public bool HasCircularReferences
        {
            get
            {
                foreach (var entity in _database.Values)
                {
                    if (entity?.TableName == null || entity.Relationships == null) continue;
                    if (entity.HasCircularReferences()) return true;
                }
                return false;
            }
        }

        // Framework Features
        public IEnumerable<string> RequiredFrameworkFeatures => GetRequiredFeatures();
        public IEnumerable<string> SuggestedNuGetPackages => GetSuggestedPackages();
        public IDictionary<string, object> FrameworkSpecificConfig => GetFrameworkConfig();

        // Code Organization
        public IEnumerable<string> SuggestedProjectStructure => GetProjectStructure();
        public IDictionary<string, string> FileNameMapping => GetFileNameMapping();

        private IDictionary<string, IEnumerable<string>> BuildDependencyGraph()
        {
            var graph = new Dictionary<string, IEnumerable<string>>();
            if (_database?.Values == null) return graph;

            foreach (var entity in _database.Values)
            {
                if (entity?.TableName == null || entity.Relationships == null) continue;

                var dependencies = entity.Relationships
                    .Where(r => r != null && 
                           r.FromTableName == entity.TableName && 
                           !string.IsNullOrEmpty(r.ToTableName))
                    .Select(r => r.ToTableName)
                    .ToList();

                if (dependencies.Any())
                {
                    graph[entity.TableName] = dependencies;
                }
            }
            return graph;
        }

        private IEnumerable<string> GetRequiredFeatures()
        {
            var features = new HashSet<string>();
            if (_database?.Values == null) return features;

            foreach (var entity in _database.Values)
            {
                if (entity?.Properties?.Values == null) continue;

                if (entity.Properties.Values.Any(p => p?.ColumnName == "CreatedDate"))
                {
                    features.Add("Auditing");
                    break;
                }
            }

            return features;
        }

        private IEnumerable<string> GetSuggestedPackages()
        {
            var packages = new HashSet<string>();
            
            switch (_language)
            {
                case "C#":
                    packages.Add("Microsoft.EntityFrameworkCore");
                    if (HasAuditableEntities)
                        packages.Add("Microsoft.EntityFrameworkCore.Auditing");
                    if (HasSoftDeletableEntities)
                        packages.Add("Softdelete.EFCore");
                    break;

                case "TypeScript":
                    packages.Add("@nestjs/typeorm");
                    packages.Add("typeorm");
                    if (HasAuditableEntities)
                        packages.Add("nestjs-audit");
                    break;

                case "Python":
                    packages.Add("sqlalchemy");
                    packages.Add("alembic");
                    if (HasAuditableEntities)
                        packages.Add("sqlalchemy-utils");
                    break;
            }

            return packages;
        }

        private IDictionary<string, object> GetFrameworkConfig()
        {
            return _language switch
            {
                "C#" => new Dictionary<string, object>
                {
                    { "TargetFramework", "net7.0" },
                    { "Nullable", true },
                    { "ImplicitUsings", true }
                },
                "TypeScript" => new Dictionary<string, object>
                {
                    { "Module", "CommonJS" },
                    { "Target", "ES2022" },
                    { "Strict", true }
                },
                "Python" => new Dictionary<string, object>
                {
                    { "PythonVersion", "3.9" },
                    { "UseTyping", true },
                    { "AsyncIO", true }
                },
                _ => new Dictionary<string, object>()
            };
        }

        private IEnumerable<string> GetProjectStructure()
        {
            var structure = new List<string>
            {
                "src/",
                "├── domain/",
                "│   ├── entities/",
                "│   ├── interfaces/",
                "│   └── value-objects/",
                "├── infrastructure/",
                "│   ├── persistence/",
                "│   └── configuration/",
                "├── application/",
                "│   ├── services/",
                "│   ├── dtos/",
                "│   └── mappers/",
                "└── api/",
                "    ├── controllers/",
                "    ├── middleware/",
                "    └── validators/"
            };

            return structure;
        }

        private IDictionary<string, string> GetFileNameMapping()
        {
            var mapping = new Dictionary<string, string>();
            foreach (var entity in Entities)
            {
                var baseName = entity.ClassName;
                mapping[$"{baseName}.cs"] = $"domain/entities/{baseName}.cs";
                mapping[$"I{baseName}.cs"] = $"domain/interfaces/I{baseName}.cs";
                mapping[$"{baseName}Controller.cs"] = $"api/controllers/{baseName}Controller.cs";
                mapping[$"{baseName}Service.cs"] = $"application/services/{baseName}Service.cs";
                mapping[$"{baseName}Dto.cs"] = $"application/dtos/{baseName}Dto.cs";
                mapping[$"{baseName}Profile.cs"] = $"application/mappers/{baseName}Profile.cs";
            }
            return mapping;
        }
    }
}
