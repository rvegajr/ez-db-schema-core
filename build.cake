#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
#tool "nuget:?package=vswhere"

var IncrementMinorVersion = true;
var NuGetReleaseNotes = new [] {"Proper Warning supress"};

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

public int MAJOR = 0; public int MINOR = 1; public int REVISION = 2; public int BUILD = 3; //Version Segments
var VersionInfoText = System.IO.File.ReadAllText(thisDir + "Src/VersionInfo.cs");
var AssemblyFileVersionAttribute = Pluck(VersionInfoText, "AssemblyFileVersionAttribute(\"", "\")]");
var CurrentAssemblyVersionAttribute = Pluck(VersionInfoText, "System.Reflection.AssemblyVersionAttribute(\"", "\")]");
var deployPath = thisDir + "artifacts" + System.IO.Path.DirectorySeparatorChar;
var publishDir = deployPath + System.IO.Path.DirectorySeparatorChar + "publish" + System.IO.Path.DirectorySeparatorChar;

var AssemblyVersionAttribute = CurrentAssemblyVersionAttribute;
var CurrentNugetVersion = VersionStringParts(AssemblyVersionAttribute, MAJOR, MINOR, REVISION);
var NugetVersion = CurrentNugetVersion;
if (IncrementMinorVersion) {	
	AssemblyVersionAttribute = VersionStringIncrement(CurrentAssemblyVersionAttribute, REVISION);
	NugetVersion = VersionStringParts(AssemblyVersionAttribute, MAJOR, MINOR, REVISION);
	AssemblyFileVersionAttribute = NugetVersion + ".*";
}

// Define directories.
var buildDir = Directory("./Src/EzDbSchema.Cli/bin") + Directory(configuration);
Console.WriteLine(string.Format("target={0}", target));
Console.WriteLine(string.Format("binDir={0}", binDir));
Console.WriteLine(string.Format("thisDir={0}", thisDir));
Console.WriteLine(string.Format("buildDir={0}", buildDir));
var framework = Argument("framework", "netcoreapp3.1");
var runtime = Argument("runtime", "Portable");
var coreProjectFile = thisDir + "Src/EzDbSchema.Core/EzDbSchema.Core.csproj";
var cliProjectFile = thisDir + "Src/EzDbSchema.Cli/EzDbSchema.Cli.csproj";

DirectoryPath vsLatest  = VSWhereLatest();
FilePath msBuildPathX64 = (vsLatest==null)
                            ? null
                            : vsLatest.CombineWithFilePath("./MSBuild/Current/bin/msbuild.exe");

Information("	  AssemblyVersionAttribute: {0}... Next: {1}", CurrentAssemblyVersionAttribute, AssemblyVersionAttribute);
Information("	       CliVersionAttribute: {0}... Next: {1}", GetVersionInProjectFile(cliProjectFile), AssemblyVersionAttribute);
Information("	      CoreVersionAttribute: {0}... Next: {1}", GetVersionInProjectFile(coreProjectFile), AssemblyVersionAttribute);
Information("        		 Nuget version: {0}... Next: {1}", CurrentNugetVersion, NugetVersion);
Information("AssemblyFileVersionAttribute : {0}", AssemblyFileVersionAttribute);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
});


Task("SetVersion")
.IsDependentOn("Clean")
.Does(() => {
	var VersionData = string.Format(@"using System.Reflection;
[assembly: System.Reflection.AssemblyFileVersionAttribute(""{0}"")]
[assembly: System.Reflection.AssemblyVersionAttribute(""{1}"")]
", AssemblyFileVersionAttribute, AssemblyVersionAttribute);
		System.IO.File.WriteAllText(thisDir + "Src/VersionInfo.cs", VersionData);
		UpdateVersionInProjectFile(cliProjectFile, AssemblyVersionAttribute);
		UpdateVersionInProjectFile(coreProjectFile, AssemblyVersionAttribute);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("SetVersion")
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
		Information("Building using MSBuild at " + msBuildPathX64);
		
		MSBuild(@"./Src/ez-db-schema-core.sln", new MSBuildSettings {
			ToolPath = msBuildPathX64
			, Configuration = configuration
		});
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
        Version                 = NugetVersion,
        Title                   = @"EzDbSchema - Easy Database Schema Generator",
        Authors                 = new[] {"Ricardo Vega Jr."},
        Owners                  = new[] {"Ricardo Vega Jr."},
        Description             = @"A class library that allows you to point to a database and represent the complete schema with columns, relationships (including fk names and multiplicity) in a simple object hierarchy.  Some use cases require a schema of a database without the bulk of Entity power tools or Entity Framework.  This class library will give you the ability to save this as a json file to be restored later.",
        Summary                 = @"A class library allows the developer to represent the schema (tables, columns and relationships) of a database in a simple object hierarchy.",
        ProjectUrl              = new Uri(@"https://github.com/rvegajr/ez-db-schema-core"),
        //IconUrl                 = new Uri(""),
        LicenseUrl              = new Uri(@"https://github.com/rvegajr/ez-db-schema-core/blob/master/LICENSE"),
        Copyright               = @"Noctusoft 2018-2020",
        ReleaseNotes            = new [] {"Added net5.0", "Updated nuget libs"},
        Tags                    = new [] {"Database", "Schema"},
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
			new NuSpecContent { Source = thisDir + @"Src/EzDbSchema.Core/bin/Release/net472/EzDbSchema.Core.dll", Target = "lib/net472" },
			new NuSpecContent { Source = thisDir + @"Src/EzDbSchema.MsSql/bin/Release/net472/EzDbSchema.MsSql.dll", Target = "lib/net472" },
			new NuSpecContent { Source = thisDir + @"Src/EzDbSchema.Core/bin/Release/net48/EzDbSchema.Core.dll", Target = "lib/net48" },
			new NuSpecContent { Source = thisDir + @"Src/EzDbSchema.MsSql/bin/Release/net48/EzDbSchema.MsSql.dll", Target = "lib/net48" },
			new NuSpecContent { Source = thisDir + @"Src/EzDbSchema.Core/bin/Release/netcoreapp2.2/EzDbSchema.Core.dll", Target = "lib/netcoreapp2.2" },
			new NuSpecContent { Source = thisDir + @"Src/EzDbSchema.MsSql/bin/Release/netcoreapp2.2/EzDbSchema.MsSql.dll", Target = "lib/netcoreapp2.2" },
			new NuSpecContent { Source = thisDir + @"Src/EzDbSchema.Core/bin/Release/netcoreapp3.1/EzDbSchema.Core.dll", Target = "lib/netcoreapp3.1" },
			new NuSpecContent { Source = thisDir + @"Src/EzDbSchema.MsSql/bin/Release/netcoreapp3.1/EzDbSchema.MsSql.dll", Target = "lib/netcoreapp3.1" },
			new NuSpecContent { Source = thisDir + @"Src/EzDbSchema.Core/bin/Release/netstandard2.0/EzDbSchema.Core.dll", Target = "lib/netstandard2.0" },
			new NuSpecContent { Source = thisDir + @"Src/EzDbSchema.MsSql/bin/Release/netstandard2.0/EzDbSchema.MsSql.dll", Target = "lib/netstandard2.0" },
			new NuSpecContent { Source = thisDir + @"Src/EzDbSchema.Core/bin/Release/netstandard2.1/EzDbSchema.Core.dll", Target = "lib/netstandard2.1" },
			new NuSpecContent { Source = thisDir + @"Src/EzDbSchema.MsSql/bin/Release/netstandard2.1/EzDbSchema.MsSql.dll", Target = "lib/netstandard2.1" },
			new NuSpecContent { Source = thisDir + @"Src/EzDbSchema.Core/bin/Release/net5.0/EzDbSchema.Core.dll", Target = "lib/net5.0" },
			new NuSpecContent { Source = thisDir + @"Src/EzDbSchema.MsSql/bin/Release/net5.0/EzDbSchema.MsSql.dll", Target = "lib/net5.0" },
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

//versionSegments can equal Major, Minor, Revision, Build in format Major.Minor.Revision.Build
public string VersionStringParts(string versionString, params int[] versionSegments) {
	var vArr = versionString.Split('.');
	string newVersion = "";
	foreach ( var versionSegment in versionSegments ) {
		newVersion += (newVersion.Length>0 ? "." : "") + vArr[versionSegment].ToString();
	}
	return newVersion;
}

//segmentToIncrement can equal Major, Minor, Revision, Build in format Major.Minor.Revision.Build
public string VersionStringIncrement(string versionString, int segmentToIncrement) {
	var vArr = versionString.Split('.');
	var valAsStr = vArr[segmentToIncrement];
	int valAsInt = 0;
    int.TryParse(valAsStr, out valAsInt);	
	vArr[segmentToIncrement] = (valAsInt + 1).ToString();
	return String.Join(".", vArr);
}


public string Pluck(string str, string leftString, string rightString)
{
	try
	{
		var lpos = str.LastIndexOf(leftString);
		var rpos = str.IndexOf(rightString, lpos+1);
		if (rpos > 0)
		{
			lpos = str.LastIndexOf(leftString, rpos);
			if ((lpos > 0) && (rpos > lpos))
			{
 				return str.Substring(lpos + leftString.Length, (rpos - lpos) - leftString.Length);
			}
		} 
	}
	catch (Exception)
	{
		return "";
	}
	return "";
}


public string GetVersionInProjectFile(string projectFileName) {
	var _VersionInfoText = System.IO.File.ReadAllText(projectFileName);
	var _AssemblyFileVersionAttribute = Pluck(_VersionInfoText, "<Version>", "</Version>");
	return _AssemblyFileVersionAttribute;
}

public bool UpdateVersionInProjectFile(string projectFileName, string NewVersion)
{
	var _VersionInfoText = System.IO.File.ReadAllText(projectFileName);
	var _AssemblyFileVersionAttribute = Pluck(_VersionInfoText, "<Version>", "</Version>");
	var VersionPattern = "<Version>{0}</Version>";
	var _AssemblyFileVersionAttributeTextOld = string.Format(VersionPattern, _AssemblyFileVersionAttribute);
	var _AssemblyFileVersionAttributeTextNew = string.Format(VersionPattern, NewVersion);
	var newText = _VersionInfoText.Replace(_AssemblyFileVersionAttributeTextOld, _AssemblyFileVersionAttributeTextNew);

	System.IO.File.WriteAllText(projectFileName, newText);	
	return true;
}
  
private void DoPackage(string project, string framework, string NugetVersion, string runtimeId = null)
{
    var publishedTo = System.IO.Path.Combine(publishDir, project, framework);
    var projectDir = System.IO.Path.Combine("./Src", project);
    var packageId = $"{project}";
    var nugetPackProperties = new Dictionary<string,string>();
    var publishSettings = new DotNetCorePublishSettings
    {
        Configuration = configuration,
        OutputDirectory = publishedTo,
        Framework = framework,
		ArgumentCustomization = args => args.Append($"/p:Version={NugetVersion}").Append($"--verbosity normal")
    };
    if (!string.IsNullOrEmpty(runtimeId))
    {
        publishedTo = System.IO.Path.Combine(publishedTo, runtimeId);
        publishSettings.OutputDirectory = publishedTo;
        // "portable" is not an actual runtime ID. We're using it to represent the portable .NET core build.
        publishSettings.Runtime = (runtimeId != null && runtimeId != "portable") ? runtimeId : null;
        packageId = $"{project}.{runtimeId}";
        nugetPackProperties.Add("runtimeId", runtimeId);
    }
    DotNetCorePublish(projectDir, publishSettings);
}

