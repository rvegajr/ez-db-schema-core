using System.IO;
using Microsoft.Extensions.Configuration;
namespace EzDbSchema.Internal
{
	public class AppSettings 
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
        public static AppSettings Instance
        {
            get
            {

                if (instance == null)
                {
					var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
					IConfigurationRoot configuration = builder.Build();
					instance = new AppSettings();
					foreach (var item in configuration.GetChildren())
					{
						var p= instance.GetType().GetProperty(item.Key);
						if (p != null) p.SetValue(instance, item.Value);
					}
				}
                return instance;
            }
        }
    }
}
