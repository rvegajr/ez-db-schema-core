using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
namespace EzDbSchema.Cli
{
    public static class CommandSample
    {
        public static void Enable(CommandLineApplication app)
        {
            app.Command("schemadump2", (command) =>
            {
                // This is a command that has it's own options.
                command.ExtendedHelpText = "Use the 'schema' command to read the Connection String frorm the App Settings file or use the command line to dump the schema file";
                command.Description = "Perform actions on the schema .";
                command.HelpOption("-?|-h|--help");

                var schemaDumpFileName = command.Option("-so|--schema-output <value>",
                    "The file name to dump the schema to.",
                    CommandOptionType.SingleValue);

                // There are 3 possible option types:
                // NoValue
                // SingleValue
                // MultipleValue

                // MultipleValue options can be supplied as one or multiple arguments
                // e.g. -m valueOne -m valueTwo -m valueThree
                var multipleValueOption = command.Option("-m|--multiple-option <value>",
                    "A multiple-value option that can be specified multiple times",
                    CommandOptionType.MultipleValue);

                // SingleValue: A basic Option with a single value
                // e.g. -s sampleValue
                var singleValueOption = command.Option("-s|--single-option <value>",
                    "A basic single-value option",
                    CommandOptionType.SingleValue);

                // NoValue are basically booleans: true if supplied, false otherwise
                var booleanOption = command.Option("-b|--boolean-option",
                    "A true-false, no value option",
                    CommandOptionType.NoValue);

                command.OnExecute(() =>
                {
                    Console.WriteLine("Writing the  schema...");

                    // Do the command's work here, or via another object/method                    

                    // Grab the values of the various options. when not specified, they will be null.

                    // The NoValue type has no Value property, just the HasValue() method.
                    bool booleanOptionValue = booleanOption.HasValue();

                    // MultipleValue returns a List<string>
                    List<string> multipleOptionValues = multipleValueOption.Values;

                    // SingleValue returns a single string
                    string singleOptionValue = singleValueOption.Value();

                    // Check if the various options have values and display them.
                    // Here we're checking HasValue() to see if there is a value before displaying the output.
                    // Alternatively, you could just handle nulls from the Value properties
                    if (booleanOption.HasValue())
                    {
                        Console.WriteLine("booleanOption option: {0}", booleanOptionValue.ToString());
                    }

                    if (multipleValueOption.HasValue())
                    {
                        Console.WriteLine("multipleValueOption option(s): {0}", string.Join(",", multipleOptionValues));
                    }

                    if (singleValueOption.HasValue())
                    {
                        Console.WriteLine("singleValueOption option: {0}", singleOptionValue ?? "null");
                    }

                    Console.WriteLine("complex-command has finished.");
                    return 0; // return 0 on a successful execution
                });
            });

        }
    }
}