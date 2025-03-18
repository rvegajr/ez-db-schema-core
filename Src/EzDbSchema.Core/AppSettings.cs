using EzDbSchema.Core.Extentions.Strings;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("EzDbSchema.MsSql")]
[assembly: InternalsVisibleTo("EzDbSchema.Cli")]
[assembly: InternalsVisibleTo("EzDbSchema.Tests")]

namespace EzDbSchema.Internal
{
    internal class AppSettings 
    {
        private static AppSettings _instance;
        private readonly Dictionary<string, object> _settings;
        private readonly string _settingsFile;

        private AppSettings(string settingsFile)
        {
            _settingsFile = settingsFile;
            _settings = new Dictionary<string, object>();
            LoadSettings();
        }

        internal static AppSettings Instance
        {
            get
            {
                var configFileName = "{ASSEMBLY_PATH}appsettings.json".ResolvePathVars();
                return _instance ??= new AppSettings(configFileName);
            }
        }

        public string ApplicationName { get; set; } = "";
        public string ConnectionString { get; set; } = "";
        public string SchemaName { get; set; } = "";
        public string Version { get; set; } = "";
        public bool VerboseMessages { get; set; } = false;

        public T GetValue<T>(string key, T defaultValue = default)
        {
            if (_settings.TryGetValue(key, out var value))
            {
                if (value is JToken token)
                {
                    return token.ToObject<T>();
                }
                return (T)Convert.ChangeType(value, typeof(T));
            }
            return defaultValue;
        }

        public void SetValue<T>(string key, T value)
        {
            _settings[key] = value;
            SaveSettings();
        }

        private void LoadSettings()
        {
            if (!File.Exists(_settingsFile)) return;

            try
            {
                var json = File.ReadAllText(_settingsFile);
                var settings = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                if (settings != null)
                {
                    foreach (var kvp in settings)
                    {
                        _settings[kvp.Key] = kvp.Value;
                    }
                    ApplicationName = GetValue<string>("ApplicationName");
                    ConnectionString = GetValue<string>("ConnectionString");
                    SchemaName = GetValue<string>("SchemaName");
                    Version = GetValue<string>("Version");
                    VerboseMessages = GetValue<bool>("VerboseMessages");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while parsing {_settingsFile}. {ex.Message}", ex);
            }
        }

        private void SaveSettings()
        {
            try
            {
                SetValue("ApplicationName", ApplicationName);
                SetValue("ConnectionString", ConnectionString);
                SetValue("SchemaName", SchemaName);
                SetValue("Version", Version);
                SetValue("VerboseMessages", VerboseMessages);
                var json = JsonConvert.SerializeObject(_settings, Formatting.Indented);
                File.WriteAllText(_settingsFile, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while saving {_settingsFile}. {ex.Message}", ex);
            }
        }
    }
}
