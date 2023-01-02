# EZDBSchema - Easy Database Schema

A class library that alllows you to point to a database and obtain a schema dump complete with columns, relationships (including fk names and multiplicity).  Some use cases require a schema of a database without the bulk of Entity power tools or Entity Framework.  

Included in the project is a handy command line interface that can be used to dump the schema of the database to a json file.    

The idea is to obtain the database schema informtion as close to the source as possible. All of the schema definitions can be obtain from the database itself,  with a little sluething,  we can derive that information we need quickly and deliver it into a usable object heirarchy.  

One possible use of this is for code generation based on database objects. 

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.  

### Prerequisites
You will need MSSQL with some database installed.  If you need a sample database,  feel free to look for the [World Wide Importers](https://github.com/Microsoft/sql-server-samples/releases/tag/wide-world-importers-v1.0) samples.

### Using this project:

####  From NuGet
1. Install-Package EzDbSchema.Core 
2. Add the following line of code
```cs
var schema = new EzDbSchema.MsSql.Database().Render("MySchema", "Server=???;Database=???;user id=sa;password=sa");
```
3. Run the app

####  As Cli
1. Change EzDbSchema.Cli/appsettings.json ConnectionString to the proper database authentication credentials.  
2. run the application.  A file called MySchemaName.db.json will be written to the EzDbSchema.Cli folder 

## Deployment

This project was design to be hosted and distributed with nuget.com.

## Built With

* [.net core](https://www.microsoft.com/net/learn/get-started) - The framework used

## Contributing

Please read [CONTRIBUTING.md](https://gist.github.com/rvegajr/651875c08acb76009e563db128f33e7e) for details on our code of conduct, and the process for submitting pull requests to us.

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/rvegajr/tags). 

## Authors

* **Ricky Vega** - *Initial work* - [Noctusoft](https://github.com/rvegajr)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

Many thanks to the following projects that have helped in this project
* McMaster.Extensions.CommandLineUtils

## Release Notes

### V 7.0.0 - Nuget package upgrades,  updated to .net 7.0

### V 6.0.1 - Added the ability to tell the generator to not auto create the primary keys if they are missing

### V 6.0.0 - Migration to .net 6.0
