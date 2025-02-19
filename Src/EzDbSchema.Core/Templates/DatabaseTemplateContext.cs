using System;
using System.Collections.Generic;
using System.Linq;
using EzDbSchema.Core.Interfaces;
using Newtonsoft.Json;

namespace EzDbSchema.Core.Templates
{
    public class DatabaseTemplateContext
    {
        private readonly IDatabase _database;

        public DatabaseTemplateContext(IDatabase database)
        {
            _database = database;
        }

        [JsonProperty("entities")]
        public IDictionary<string, EntityTemplateView> Entities =>
            _database.ToDictionary(
                kvp => kvp.Key,
                kvp => new EntityTemplateView(kvp.Value)
            );

        [JsonProperty("tables")]
        public IEnumerable<EntityTemplateView> Tables =>
            Entities.Values.Where(e => e.IsTable);

        [JsonProperty("views")]
        public IEnumerable<EntityTemplateView> Views =>
            Entities.Values.Where(e => e.IsView);

        [JsonProperty("hasEntities")]
        public bool HasEntities => _database.Values.Any();

        [JsonProperty("hasTables")]
        public bool HasTables => Tables.Any();

        [JsonProperty("hasViews")]
        public bool HasViews => Views.Any();

        [JsonProperty("entityCount")]
        public int EntityCount => _database.Values.Count();

        [JsonProperty("tableCount")]
        public int TableCount => Tables.Count();

        [JsonProperty("viewCount")]
        public int ViewCount => Views.Count();
    }
}
