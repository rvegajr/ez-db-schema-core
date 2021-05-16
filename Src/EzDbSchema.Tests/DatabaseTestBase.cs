using LocalDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using Microsoft.Data.SqlClient;

namespace EzDbSchema.Tests
{
    public class DatabaseTestBase
    {
        protected static SqlInstance instance;
        protected static string ConnectionString
        {
            get
            {
                return instance.MasterConnectionString;
            }
        }
        static DatabaseTestBase()
        {
            instance = new(
                name: "EzDbScbemaTests", 
                buildTemplate: TestDbBuilder.RestoreDatabase);
            RestoreBackup().GetAwaiter().GetResult();
        }

        public Task<SqlDatabase> LocalDb(
            [CallerFilePath] string testFile = "",
            string? databaseSuffix = null,
            [CallerMemberName] string memberName = "")
        {
            return instance.Build(testFile, databaseSuffix, memberName);
        }

        public static async Task RestoreBackup()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    var command = connection.CreateCommand();
                    var DBPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                    var TempPath = Path.GetTempPath();
                    var DataBaseName = "Northwind";
                    var DatabaseBackup = DBPath + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + DataBaseName + ".bak";
                    if (!File.Exists(DatabaseBackup))
                    {
                        throw new Exception("File " + DatabaseBackup + " does not exist.");
                    }
                    if (File.Exists(string.Format("{0}{1}.mdf", TempPath, DataBaseName))) File.Delete(string.Format("{0}{1}.mdf", TempPath, DataBaseName));
                    if (File.Exists(string.Format("{0}{1}.ldf", TempPath, DataBaseName))) File.Delete(string.Format("{0}{1}.ldf", TempPath, DataBaseName));
                    command.CommandText = string.Format(@"
USE [master]
IF EXISTS (SELECT name FROM master.sys.databases WHERE name = N'{2}') BEGIN
	ALTER DATABASE [{2}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
	DROP DATABASE [{2}];
END

RESTORE DATABASE [{2}] FROM  DISK = N'{0}' WITH  FILE = 1,  
	MOVE N'{2}' TO N'{1}{2}.mdf',  MOVE N'{2}_log' TO N'{1}{2}.ldf',  NOUNLOAD,  STATS = 5
", DatabaseBackup, TempPath, DataBaseName);
                    await command.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

        }

    }


    public static class TestDbBuilder
    {
        public static async Task RestoreDatabase(DbConnection connection)
        {
            /*
            await using var command = connection.CreateCommand();
            var DBPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            var TempPath = Path.GetTempPath();
            var DataBaseName = "Northwind";
            var DatabaseBackup = DBPath + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + DataBaseName + ".bak";
            if (!File.Exists(DatabaseBackup))
            {
                throw new Exception("File " + DatabaseBackup + " does not exist.");
            }
            if (File.Exists(string.Format("{0}{1}.mdf", TempPath, DataBaseName))) File.Delete(string.Format("{0}{1}.mdf", TempPath, DataBaseName));
            if (File.Exists(string.Format("{0}{1}.ldf", TempPath, DataBaseName))) File.Delete(string.Format("{0}{1}.ldf", TempPath, DataBaseName));
            command.CommandText = string.Format(@"
USE [master]
IF EXISTS (SELECT name FROM master.sys.databases WHERE name = N'{2}') BEGIN
	ALTER DATABASE [{2}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
	DROP DATABASE [{2}];
END

RESTORE DATABASE [{2}] FROM  DISK = N'{0}' WITH  FILE = 1,  
	MOVE N'{2}' TO N'{1}{2}.mdf',  MOVE N'{2}_log' TO N'{1}{2}.ldf',  NOUNLOAD,  STATS = 5
", DatabaseBackup, TempPath, DataBaseName);
            await command.ExecuteNonQueryAsync();
            */
        }

        static int intData = 0;

        public static async Task<int> AddData(DbConnection connection)
        {
            await using var command = connection.CreateCommand();
            var addData = intData;
            intData++;
            command.CommandText = $@"
insert into MyTable (Value)
values ({addData});";
            await command.ExecuteNonQueryAsync();
            return addData;
        }

        public static async Task<List<int>> GetData(DbConnection connection)
        {
            List<int> values = new();
            await using var command = connection.CreateCommand();
            command.CommandText = "select Value from MyTable";
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                values.Add(reader.GetInt32(0));
            }

            return values;
        }
    }
}

