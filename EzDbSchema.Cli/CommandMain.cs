using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using EzDbSchema.Core;
using EzDbSchema.Core.Enums;
using EzDbSchema.Core.Interfaces;
using EzDbSchema.Core.Extentions;
using McMaster.Extensions.CommandLineUtils;
using EzDbSchema.MsSql;
using Newtonsoft.Json;
using EzDbSchema.Internal;

namespace EzDbSchema.Cli
{
	public static class CommandMain
    {
        /*
         * Example Usage:  -sc "Server=NSWIN10VM.local;Database=WideWorldImportersDW;user id=sa;password=sa" -sf "/Users/rvegajr/Downloads/Schema/WideWorldImportersDW.db.json" -sn "WideWorldImportersDWEntities"
         */
        public static void Enable(CommandLineApplication app)
        {
			app.ExtendedHelpText = "Used to read the Connection String frorm the App Settings file or use the command line to dump the schema file";
			app.Description = "Perform actions on the schema .";
			app.HelpOption("-?|-h|--help");

			var verboseOption = app.Option("-verbose|--verbose",
                "Will display more detailed message about what is going on with the processing",
                CommandOptionType.NoValue);

			var schemaOutput = app.Option("-sf|--schema-filename <value>",
                "The file name or path to dump the schema to.  This is required field.",
                CommandOptionType.SingleValue);

			var entityName = app.Option("-sn|--schema-name <value>",
                "The Name that will be given to the schema object.  This value will override the value in appsettings.json.   This is an optional field.",
                CommandOptionType.SingleValue);

			var connectionString = app.Option("-sc|--connection-string <optionvalue>",
                "Connection String pass via the commandline.  This value will override the value in appsettings.json.  This is an optional field.",
                CommandOptionType.SingleValue);

			var databaseType = app.Option("-db|--database-type <optionvalue>",
                "Optional switch to force the appication to process as a certain database type.  This is an optional field.  Default is auto, but can be set to 'mssql' or 'ora'.",
                CommandOptionType.SingleValue);

			app.OnExecute(() =>
            {
                try
                {
                    if (verboseOption.HasValue()) AppSettings.Instance.VerboseMessages = verboseOption.HasValue();
                    if (entityName.HasValue()) AppSettings.Instance.SchemaName = entityName.Value();
                    if (connectionString.HasValue()) AppSettings.Instance.ConnectionString = connectionString.Value();
                    var dbtype = (databaseType.HasValue() ? databaseType.Value() : "auto");
					var outputPath = (schemaOutput.HasValue() ? schemaOutput.Value() : (@"{ASSEMBLY_PATH}" + AppSettings.Instance.SchemaName + @".db.json").ResolvePathVars());

                    Console.WriteLine("Performing Schema Dump....");
                    Console.WriteLine("Connection String: " + AppSettings.Instance.ConnectionString);
                    Console.WriteLine("Schema Name: " + AppSettings.Instance.SchemaName);
					Console.WriteLine("Output File/Path: " + outputPath);
                    Console.WriteLine("Database Type: " + dbtype);
                    IDatabase schemaObject = null;
                    if ((AppSettings.Instance.ConnectionString.ToLower().Contains("database=")) || (dbtype.Equals("mssql")))
                    {
                        Console.WriteLine("Processing a mssql database");
						schemaObject = new EzDbSchema.MsSql.Database();
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
                    //schemaAsJson = NetJSON.NetJSON.Serialize(schemaObject);
					File.WriteAllText(outputPath, schemaAsJson);
					Console.WriteLine(string.Format("Schema has been written to {0}", outputPath));

					Console.WriteLine("Schema Dump has completed.");
                    Environment.ExitCode = (int)ReturnCode.Ok;
                    Environment.Exit(Environment.ExitCode);
                    return Environment.ExitCode;
                }
                catch (Exception ex)
                {
					Console.WriteLine(string.Format("{0} failed. {1}", "Schema Dump", ex.Message));
                    Console.WriteLine("Stack Trace:");
                    Console.WriteLine(ex.StackTrace);
                    Environment.ExitCode = (int)ReturnCode.Error;
                    Environment.Exit(Environment.ExitCode);
                    return Environment.ExitCode;
                }
            });
        }
    }
}