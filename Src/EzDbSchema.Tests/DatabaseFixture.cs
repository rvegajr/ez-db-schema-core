﻿
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
        public static string LOCALDB_NAME = "EzDbSchemaTestDB";
        public static string DATABASE_NAME = "Northwind";
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
                    var DBPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + Path.DirectorySeparatorChar;
                    var UserPath = Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + Path.DirectorySeparatorChar;
                    var DatabaseBackupZip = DBPath + "Resources" + Path.DirectorySeparatorChar + DatabaseFixture.DATABASE_NAME + ".zip";
                    var DatabaseBackup = DBPath + "Resources" + Path.DirectorySeparatorChar + DatabaseFixture.DATABASE_NAME + ".bak";

                    if (File.Exists(DatabaseBackup)) File.Delete(DatabaseBackup);
                    if (!File.Exists(DatabaseBackupZip))
                        throw new Exception("File " + DatabaseBackupZip + " does not exist.");
                    System.IO.Compression.ZipFile.ExtractToDirectory(DatabaseBackupZip, DBPath + "Resources" + Path.DirectorySeparatorChar);
                    if (!File.Exists(DatabaseBackup))
                        throw new Exception("File " + DatabaseBackup + " does not exist.");

                    if (File.Exists(string.Format("{0}{1}.mdf", UserPath, DatabaseFixture.DATABASE_NAME))) File.Delete(string.Format("{0}{1}.mdf", UserPath, DatabaseFixture.DATABASE_NAME));
                    if (File.Exists(string.Format("{0}{1}.ldf", UserPath, DatabaseFixture.DATABASE_NAME))) File.Delete(string.Format("{0}{1}.ldf", UserPath, DatabaseFixture.DATABASE_NAME));
                    command.CommandText = string.Format(@"
USE [master]
DECLARE @NOTE VARCHAR(8000) = '';
SET @NOTE = 'Restoring Local DB'; RAISERROR (@NOTE, 10, 1) WITH NOWAIT; 

SET NOCOUNT ON;
DECLARE @PMSG VARCHAR(8000) = ''; DECLARE @PROC VARCHAR(512) = 'LocalDB Restore: ';
BEGIN TRY
	IF EXISTS (SELECT name FROM sys.databases WHERE name = N'{2}') BEGIN
		ALTER DATABASE [{2}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
		EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'{2}';
		SET @NOTE = 'Deleting old database'; RAISERROR (@NOTE, 10, 1) WITH NOWAIT; 
		DROP DATABASE [{2}]
	END
END TRY
BEGIN CATCH
END CATCH
DECLARE @MDF VARCHAR(512) = CONVERT(NVARCHAR, serverproperty('InstanceDefaultDataPath')) + '{2}.mdf';
DECLARE @LDF VARCHAR(512) = CONVERT(NVARCHAR, serverproperty('InstanceDefaultLogPath')) + '{2}.ldf';
BEGIN TRY
	DROP DATABASE [{2}]
END TRY
BEGIN CATCH
END CATCH

SET @NOTE = 'Restoring....'; RAISERROR (@NOTE, 10, 1) WITH NOWAIT; 
RESTORE DATABASE [{2}]
FROM DISK = '{0}'
WITH REPLACE,RECOVERY,
    MOVE '{2}' TO @MDF,
    MOVE '{2}_log' TO @LDF;
", DatabaseBackup, UserPath, DatabaseFixture.DATABASE_NAME).Replace(@"\", @"\\");
                    command.CommandTimeout = 300;
                    await command.ExecuteNonQueryAsync();
                }
                catch (Exception)
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

