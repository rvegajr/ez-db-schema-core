using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Reflection;
using EzDbSchema.Core.Extentions;

namespace EzDbSchema.Core
{
    public class AppSettings : AppSettingsBase
    {
        /// <summary></summary>
        public string ApplicationName
        {
            get
            {
                return HostValue(MethodBase.GetCurrentMethod().Name.Replace("get_", ""));
            }
            set
            {
                SettingsData[MethodBase.GetCurrentMethod().Name.Replace("set_", "")] = value;
            }
        }

        /// <summary></summary>
        public string ConnectionString
        {
            get
            {
                return HostValue(MethodBase.GetCurrentMethod().Name.Replace("get_", ""));
            }
            set
            {
                SettingsData[MethodBase.GetCurrentMethod().Name.Replace("set_", "")] = value;
            }
        }

        /// <summary></summary>
        public string SchemaName
        {
            get
            {
                return HostValue(MethodBase.GetCurrentMethod().Name.Replace("get_", ""));
            }
            set
            {
                SettingsData[MethodBase.GetCurrentMethod().Name.Replace("set_", "")] = value;
            }
        }

        /// <summary></summary>
        public string Version
        {
            get
            {
                return HostValue(MethodBase.GetCurrentMethod().Name.Replace("get_", ""));
            }
            set
            {
                SettingsData[MethodBase.GetCurrentMethod().Name.Replace("set_", "")] = value;
            }
        }

        /// <summary></summary>
        public bool VerboseMessages
        {
            get
            {
                return HostValueAsBool(MethodBase.GetCurrentMethod().Name.Replace("get_", ""));
            }
            set
            {
                SettingsData[MethodBase.GetCurrentMethod().Name.Replace("set_", "")] = value;
            }
        }

        private static AppSettings instance;

        private AppSettings(string FileName)
        {
            if (File.Exists(FileName))
            {
                SettingsData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(File.ReadAllText(FileName));
            }
        }
        public static AppSettings Instance
        {
            get
            {

                if (instance == null)
                {
					var appSettingsFileName = "{SOLUTION_PATH}EzDbSchema.Cli/appsettings.json".ResolvePathVars();
                    instance = new AppSettings(appSettingsFileName);
                }
                return instance;
            }
        }
      
        /// <summary></summary>
        public string SolutionName
        {
            get
            {
                var SolutionName = HostValue(MethodBase.GetCurrentMethod().Name.Replace("get_", ""));
				if (SolutionName.Length == 0) SolutionName = "ez-db-schema-core";
                return SolutionName;
            }
        }
    }

    public class AppSettingsBase
    {
		protected Dictionary<string, object> SettingsData = new Dictionary<string, object>();// JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

        public string ResolvePathVars(string PathToResolve)
        {
            if ((PathToResolve.Contains("{SOLUTION_PATH}")) || (PathToResolve.Contains("{ASSEMBLY_PATH}")))
            {
                var AssemblyPath = Path.GetDirectoryName(System.IO.Path.GetDirectoryName(
         System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)).Replace("file:\\", "");
                AssemblyPath += (AssemblyPath.EndsWith(Path.DirectorySeparatorChar.ToString()) ? "" : Path.DirectorySeparatorChar.ToString());
                var SolutionPath = AssemblyPath;
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    DirectoryInfo di = new DirectoryInfo(SolutionPath);
                    while (di != null)
                    {
                        if (di.Name == AppSettings.Instance.SolutionName)
                        {
                            SolutionPath = di.FullName + Path.DirectorySeparatorChar.ToString();
                            break;
                        }
                        di = di.Parent;
                    }
                    PathToResolve = PathToResolve.Replace("{SOLUTION_PATH}", SolutionPath).Replace("{ASSEMBLY_PATH}", AssemblyPath);
                }
                else
                    PathToResolve = PathToResolve.Replace("{SOLUTION_PATH}", AssemblyPath).Replace("{ASSEMBLY_PATH}", AssemblyPath);
            }

            return PathToResolve;
        }

        /// <summary></summary>
        protected string Value(string settingName, string defaultValue = "")
        {

            string sValue = null;
            if (Exists(settingName, out sValue))
            {
                return ResolvePathVars(sValue);
            }
            else
            {
                return ResolvePathVars(defaultValue);
            }
        }

        /// <summary></summary>
        protected string HostValue(string settingName, string defaultValue = "")
        {

            string sValue = null;
            string hostSettingName = Environment + "_" + settingName;
            //Lets see if a host_setting paid exists first (this will give us a host specific value
            if (Exists(hostSettingName, out sValue))
            {
                return ResolvePathVars(sValue);
            }
            else
            {
                //it doesn't exist, so lets see if a value without the HOST header exists
                if (Exists(settingName, out sValue))
                {
                    return ResolvePathVars(sValue);
                }
                else
                {
                    return ResolvePathVars(defaultValue);
                }
            }
        }

        /// <summary></summary>
        protected bool HostValueAsBool(string settingName)
        {
            string sHostValue = HostValue(settingName).ToUpper();
            return sHostValue.StartsWith("T") || sHostValue.StartsWith("Y");
        }

        /// <summary></summary>
        protected bool Exists(string settingName, out string value)
        {
            if (SettingsData.ContainsKey(settingName))
            {
                value = SettingsData[settingName].ToString();
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }


        /// <summary></summary>
        public string Environment
        {
            get
            {
                string sHostName = HostName.Split(':')[0];  //grab the first part of a host name in case there is a port number
                string sHostDNSKey = "HOST_" + sHostName;
                string sEnv = "";
                return (Exists(sHostDNSKey, out sEnv) ? sEnv : Value("HOST_DEFAULT"));
            }
        }

        /// <summary></summary>
        public string HostName
        {
            get
            {
                return System.Net.Dns.GetHostName().ToLower();
            }
        }
    }
}
