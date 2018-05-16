using System;
using System.Reflection;
using EzDbSchema.Core.Enums;
using EzDbSchema.Internal;
using McMaster.Extensions.CommandLineUtils;

namespace EzDbSchema.Cli
{
	class Program
    {
        private static CommandLineApplication App = new CommandLineApplication();
        static int Main(string[] args)
        {
            try
            {
                LoadSettings();
                CommandMain.Enable(App);
                App.Execute(args);
            }
            catch (CommandParsingException ex)
            {
                Console.WriteLine(ex.Message);
                Environment.ExitCode = (int)ReturnCode.Error;
                Environment.Exit(Environment.ExitCode);
                return Environment.ExitCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to execute application: {0}", ex.Message);
                Environment.ExitCode = (int)ReturnCode.Error;
                Environment.Exit(Environment.ExitCode);
                return Environment.ExitCode;
            }
            Environment.ExitCode = (int)ReturnCode.Ok;
            Environment.Exit(Environment.ExitCode);
            return Environment.ExitCode;
        }
        
        static void LoadSettings()
        {
            AppSettings.Instance.Version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        }
    }
}
