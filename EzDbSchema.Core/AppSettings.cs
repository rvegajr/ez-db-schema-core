using System;
using System.IO;
using EzDbSchema.Core.Extentions;
using NetJSON;
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
        }
        public static AppSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    var configFileName = "{ASSEMBLY_PATH}appsettings.json".ResolvePathVars();
                    try
                    {
                        //Complete ghetto way to deal with working around a Newtonsoft JSON bug 
                        var appsettingsText = File.ReadAllText(configFileName);
                        instance = NetJSON.NetJSON.Deserialize<AppSettings>(appsettingsText);
                    }
                    catch (System.Exception ex)
                    {
                        throw new Exception(string.Format("Error while parsing {0}. {1}", configFileName, ex.Message), ex);
                    }
				}
                return instance;
            }
        }
    }
}
