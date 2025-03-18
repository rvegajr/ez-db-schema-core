using EzDbSchema.Core.Extentions.Json;
using EzDbSchema.Core.Extentions.Strings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using JsonPair = System.Collections.Generic.KeyValuePair<string, System.Text.Json.Nodes.JsonNode>;
using JsonPairEnumerable = System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, System.Text.Json.Nodes.JsonNode>>;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("EzDbSchema.MsSql")]
[assembly: InternalsVisibleTo("EzDbSchema.Cli")]
[assembly: InternalsVisibleTo("EzDbSchema.Tests")]

namespace EzDbSchema.Internal
{
	internal class AppSettings 
    {
		/// <summary></summary>
		public string ApplicationName { get; set; } = "";
        /// <summary></summary>
		public string ConnectionString { get; set; } = "";
        /// <summary></summary>
		public string SchemaName { get; set; } = "";
        /// <summary></summary>
		public string Version { get; set; } = "";
		/// <summary></summary>
		public bool VerboseMessages { get; set; } = false;
        private static AppSettings instance;
        
		private AppSettings()
        {
			//_configuration = configuration;
        }
        internal static AppSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    var configFileName = "{ASSEMBLY_PATH}appsettings.json".ResolvePathVars();
                    try
                    {
                        var appsettingsText = File.ReadAllText(configFileName);
                        var jsonObject = JsonNode.Parse(appsettingsText)?.AsObject() 
                            ?? throw new System.Text.Json.JsonException($"Failed to parse {configFileName} as JSON object");
                        
                        instance = new AppSettings();
                        foreach (var property in jsonObject)
                        {
                            var propertyInfo = instance.GetType().GetProperty(property.Key);
                            if (propertyInfo != null) 
                            {
                                var stringValue = property.Value?.GetValue<string>();
                                if (propertyInfo.PropertyType == typeof(bool))
                                {
                                    if (bool.TryParse(stringValue, out bool boolValue))
                                    {
                                        propertyInfo.SetValue(instance, boolValue);
                                    }
                                }
                                else
                                {
                                    propertyInfo.SetValue(instance, stringValue ?? "");
                                }
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        throw new Exception($"Error while parsing {configFileName}. {ex.Message}", ex);
                    }
				}
                return instance;
            }
        }
    }
}
