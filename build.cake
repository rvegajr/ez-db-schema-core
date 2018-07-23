#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var binDir = Directory("./bin") ;
var thisDir = System.IO.Path.GetFullPath(".") + System.IO.Path.DirectorySeparatorChar;
//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./Src/EzDbSchema.Cli/bin") + Directory(configuration);
Console.WriteLine(string.Format("target={0}", target));
Console.WriteLine(string.Format("binDir={0}", binDir));
Console.WriteLine(string.Format("thisDir={0}", thisDir));
Console.WriteLine(string.Format("buildDir={0}", buildDir));

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{

	var settings = new NuGetRestoreSettings()
	{
		// VSTS has old version of Nuget.exe and Automapper restore fails because of that
		ToolPath = "./nuget/nuget.exe",
		Verbosity = NuGetVerbosity.Detailed,
	};
	NuGetRestore("./Src/ez-db-schema-core.sln", settings);
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    if(IsRunningOnWindows())
    {
      // Use MSBuild
      MSBuild("./Src/ez-db-schema-core.sln", settings =>
        settings.SetConfiguration(configuration));
    }
    else
    {
      // Use XBuild
      XBuild("./Src/ez-db-schema-core.sln", settings =>
        settings.SetConfiguration(configuration));
    }
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    NUnit3("./Src/**/bin/" + configuration + "/*.Tests.dll", new NUnit3Settings {
        NoResults = true
        });
});

Task("NuGet-Pack")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
   var nuGetPackSettings   = new NuGetPackSettings {
		BasePath 				= thisDir,
        Id                      = @"EzDbSchema",
        Version                 = @"1.0.14",
        Title                   = @"EzDbSchema - Easy Database Schema Generator",
        Authors                 = new[] {"Ricardo Vega Jr."},
        Owners                  = new[] {"Ricardo Vega Jr."},
        Description             = @"A class library that allows you to point to a database and obtain a schema dump complete with columns, relationships (including fk names and multiplicity).  Some use cases require a schema of a database without the bulk of Entity power tools or Entity Framework.",
        Summary                 = @"A class library that allows you to point to a database and obtain a schema dump in xml format.",
        ProjectUrl              = new Uri(@"https://github.com/rvegajr/ez-db-schema-core"),
        //IconUrl                 = new Uri(""),
        LicenseUrl              = new Uri(@"https://github.com/rvegajr/ez-db-schema-core/blob/master/LICENSE"),
        Copyright               = @"Noctusoft 2018",
        ReleaseNotes            = new [] {"Properly Set Compatability Level", "Works with old version of MSSQL", "netcoreapp2.1", "Issues fixes", "Typos"},
        Tags                    = new [] {"Database ", "Schema"},
        RequireLicenseAcceptance= false,
        Symbols                 = false,
        NoPackageAnalysis       = false,
        OutputDirectory         = thisDir + "artifacts/",
		Properties = new Dictionary<string, string>
		{
			{ @"Configuration", @"Release" }
		},
		Files = new[] {
			new NuSpecContent { Source = thisDir + @"Src/EzDbSchema.Core/bin/Release/net461/EzDbSchema.Core.dll", Target = "lib/net461" },
			new NuSpecContent { Source = thisDir + @"Src/EzDbSchema.MsSql/bin/Release/net461/EzDbSchema.MsSql.dll", Target = "lib/net461" },
			new NuSpecContent { Source = thisDir + @"Src/EzDbSchema.Core/bin/Release/netcoreapp2.0/EzDbSchema.Core.dll", Target = "lib/netcoreapp2.0" },
			new NuSpecContent { Source = thisDir + @"Src/EzDbSchema.MsSql/bin/Release/netcoreapp2.0/EzDbSchema.MsSql.dll", Target = "lib/netcoreapp2.0" },
			new NuSpecContent { Source = thisDir + @"Src/EzDbSchema.Core/bin/Release/netcoreapp2.1/EzDbSchema.Core.dll", Target = "lib/netcoreapp2.1" },
			new NuSpecContent { Source = thisDir + @"Src/EzDbSchema.MsSql/bin/Release/netcoreapp2.1/EzDbSchema.MsSql.dll", Target = "lib/netcoreapp2.1" },
			new NuSpecContent { Source = thisDir + @"Src/EzDbSchema.Core/bin/Release/netstandard2.0/EzDbSchema.Core.dll", Target = "lib/netstandard2.0" },
			new NuSpecContent { Source = thisDir + @"Src/EzDbSchema.MsSql/bin/Release/netstandard2.0/EzDbSchema.MsSql.dll", Target = "lib/netstandard2.0" },
		},
		ArgumentCustomization = args => args.Append("")		
    };
            	
    NuGetPack(thisDir + "NuGet/EzDbSchema.nuspec", nuGetPackSettings);
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("NuGet-Pack");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
