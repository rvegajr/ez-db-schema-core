
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using Microsoft.Data.SqlClient;
using Xunit;
using MartinCostello.SqlLocalDb;

namespace EzDbSchema.Tests
{
    public class DatabaseFixture : IDisposable
    {

        private SqlLocalDbApi localDB;
        private ISqlLocalDbInstanceInfo instance;
        private ISqlLocalDbInstanceManager manager;
        private static string LOCALDB_NAME = "EzDbSchemaTestDB";
        private static string DATABASE_NAME = "Northwind";
        public DatabaseFixture()
        {
            this.localDB = new SqlLocalDbApi();
            instance = localDB.GetOrCreateInstance(DatabaseFixture.LOCALDB_NAME);
            manager = instance.Manage();
            if (!instance.IsRunning)
                manager.Start();
            RestoreBackup().GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            manager.Stop();
        }

        public string ConnectionString
        {
            get
            {
                return string.Format(@"Server=(localdb)\{0};Integrated Security=true;Database={1};", DatabaseFixture.LOCALDB_NAME, DatabaseFixture.DATABASE_NAME);
            }
        }
        
        public async Task RestoreBackup()
        {
            using (SqlConnection connection = new SqlConnection(instance.GetConnectionString()))
            {
                try
                {
                    await connection.OpenAsync();
                    var command = connection.CreateCommand();
                    var DBPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

                    var TempPath = Path.GetTempPath();
                    var DatabaseBackup = DBPath + Path.DirectorySeparatorChar + "Resources" + Path.DirectorySeparatorChar + DatabaseFixture.DATABASE_NAME + ".bak";
                    if (!File.Exists(DatabaseBackup))
                    {
                        throw new Exception("File " + DatabaseBackup + " does not exist.");
                    }
                    if (File.Exists(string.Format("{0}{1}.mdf", TempPath, DatabaseFixture.DATABASE_NAME))) File.Delete(string.Format("{0}{1}.mdf", TempPath, DatabaseFixture.DATABASE_NAME));
                    if (File.Exists(string.Format("{0}{1}.ldf", TempPath, DatabaseFixture.DATABASE_NAME))) File.Delete(string.Format("{0}{1}.ldf", TempPath, DatabaseFixture.DATABASE_NAME));
                    command.CommandText = string.Format(@"
USE [master]
IF EXISTS (SELECT name FROM master.sys.databases WHERE name = N'{2}') BEGIN
	ALTER DATABASE [{2}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
	DROP DATABASE [{2}];
END

RESTORE DATABASE [{2}] FROM  DISK = N'{0}' WITH  FILE = 1,  
	MOVE N'{2}' TO N'{1}{2}.mdf',  MOVE N'{2}_log' TO N'{1}{2}.ldf',  NOUNLOAD,  STATS = 5
", DatabaseBackup, TempPath, DatabaseFixture.DATABASE_NAME);
                    await command.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

        }
    }

    [CollectionDefinition("DatabaseCollection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
        public static string CLASS_NAME = "Database Collection";
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}

