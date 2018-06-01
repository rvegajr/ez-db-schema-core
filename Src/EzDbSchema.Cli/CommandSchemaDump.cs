using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using EzDbSchema.Core;
using EzDbSchema.Core.Enums;
using EzDbSchema.Core.Interfaces;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace EzDbSchema.Cli
{
    public static class CommandSchemaDump
    {
        public static string CommandName = "Schema Dump";
        public static string CommandOperator = "schemadump";
        /*
         * Example Usage - schemadump -sc "Server=NSWIN10VM.local;Database=WideWorldImportersDW;user id=sa;password=sa" -sf "/Users/rvegajr/Downloads/Schema/WideWorldImportersDW.db.json" -sn "WideWorldImportersDWEntities"
         */
        public static void Enable(CommandLineApplication app)
        {
            app.Command(CommandOperator, (command) =>
            {
                command.ExtendedHelpText = "Use the '" + CommandOperator + "' command to read the Connection String frorm the App Settings file or use the command line to dump the schema file";
                command.Description = "Perform actions on the schema .";
                command.HelpOption("-?|-h|--help");

                var verboseOption = command.Option("-verbose|--verbose",
                    "Will display more detailed message about what is going on with the processing",
                    CommandOptionType.NoValue);

                var configFile = command.Option("-cf|--configfile",
                    "the configuration file this template render will use.  the default will be in the same path as this assembly of this applicaiton.  This parm is optional and will override the value in appsettings.json.   ",
                    CommandOptionType.SingleValue);

                var schemaOutput = command.Option("-sf|--schema-filename <value>",
                    "The file name or path to dump the schema to.  This is required field.",
                    CommandOptionType.SingleValue);

                var entityName = command.Option("-sn|--schema-name <value>",
                "The Name that will be given to the schema object.  This value will override the value in appsettings.json.   This is an optional field.",
                CommandOptionType.SingleValue);

                var connectionString = command.Option("-sc|--connection-string <optionvalue>",
                    "Connection String pass via the commandline.  This value will override the value in appsettings.json.  This is an optional field.",
                    CommandOptionType.SingleValue);

                var databaseType = command.Option("-db|--database-type <optionvalue>",
                    "Optional switch to force the appication to process as a certain database type.  This is an optional field.  Default is auto, but can be set to 'mssql' or 'ora'.",
                    CommandOptionType.SingleValue);

                command.OnExecute(() =>
                {
                    try
                    {
                        if (verboseOption.HasValue()) AppSettings.Instance.VerboseMessages = verboseOption.HasValue();
                        if (configFile.HasValue()) AppSettings.Instance.ConfigurationFileName = configFile.Value();
                        if (entityName.HasValue()) AppSettings.Instance.SchemaName = entityName.Value();
                        if (connectionString.HasValue()) AppSettings.Instance.ConnectionString = connectionString.Value();
                        var dbtype = (databaseType.HasValue() ? databaseType.Value() : "auto");

                        Console.WriteLine("Performing " + CommandName + "....");
                        Console.WriteLine("Connection String: " + AppSettings.Instance.ConnectionString);
                        Console.WriteLine("Schema Name: " + AppSettings.Instance.SchemaName);
                        Console.WriteLine("Output File/Path: " + schemaOutput.Value());
                        Console.WriteLine("Database Type: " + dbtype);
                        IDatabase schemaObject = null;
                        if ((AppSettings.Instance.ConnectionString.ToLower().Contains("database=")) || (dbtype.Equals("mssql")))
                        {
							Console.WriteLine("Processing a mssql database");
                        }
                        else
                        {
                            throw new Exception(string.Format("Cannot figure out how to handle the connection string!  '{0}'", AppSettings.Instance.ConnectionString));
                        }
						schemaObject.ShowWarnings = AppSettings.Instance.VerboseMessages;
						schemaObject = schemaObject.Render(
                            AppSettings.Instance.SchemaName,
                            AppSettings.Instance.ConnectionString);

						var schemaAsJson = JsonConvert.SerializeObject(
                            schemaObject
                            , Formatting.Indented
                            , new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.All });
                        File.WriteAllText(schemaOutput.Value(), schemaAsJson);
                        Console.WriteLine(string.Format("Schema has been written to {0}", schemaOutput.Value()));

                        Console.WriteLine(CommandName + " has completed.");
                        Environment.ExitCode = (int)ReturnCode.Ok;
                        Environment.Exit(Environment.ExitCode);
                        return Environment.ExitCode;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(string.Format("{0} failed. {1}", CommandName, ex.Message));
                        Console.WriteLine("Stack Trace:");
                        Console.WriteLine(ex.StackTrace);
                        Environment.ExitCode = (int)ReturnCode.Error;
                        Environment.Exit(Environment.ExitCode);
                        return Environment.ExitCode;
                    }
                });
            });
        }
    }
}