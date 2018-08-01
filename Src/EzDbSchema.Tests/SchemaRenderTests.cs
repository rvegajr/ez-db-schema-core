using System;
using System.Collections.Generic;
using System.IO;
using EzDbSchema.Core;
using EzDbSchema.Core.Objects;
using EzDbSchema.Internal;
using Json.Comparer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace EzDbSchema.Tests
{
	public class SchemaRenderTests
    {
        [Fact]
        public void MsSqlSchemaRenderTests()
        {
			try
			{
				EzDbSchema.MsSql.Database dbschema = new EzDbSchema.MsSql.Database();
                Assert.True(!AppSettings.Instance.ConnectionString.Contains("CHANGE ME"), "Change the Connection String in the App Settings");
                dbschema.Render("TestSchemaName", AppSettings.Instance.ConnectionString);
				Assert.True(dbschema.Entities.Count > 0, "No entites returned");
			}
			catch (Exception ex)
			{
				Assert.True(false, ex.ToString());
			}
        }

        [Fact]
        public void MsSqlSchemaSerializeDeserialzieTests()
        {
            try
            {
                string path = Path.GetTempPath();

                EzDbSchema.MsSql.Database dbschema = new EzDbSchema.MsSql.Database();
                Assert.True(!AppSettings.Instance.ConnectionString.Contains("CHANGE ME"), "Change the Connection String in the App Settings");
                dbschema.Render("TestSchemaName", AppSettings.Instance.ConnectionString);
                dbschema.ToJsonFile(path + "db1.json");
                EzDbSchema.MsSql.Database dbschema2 = Database.FromJsonFile<MsSql.Database>(path + "db1.json");

                CompareLogic compareLogic = new CompareLogic();
                compareLogic.Config.TypeNameHandling = TypeNameHandling.All;
                compareLogic.Config.PreserveReferencesHandling = PreserveReferencesHandling.All;
                var result = compareLogic.Compare(dbschema, dbschema2);

                dbschema2.ToJsonFile(path + "db2.json");
                
                if (!result.AreEqual)
                    Console.WriteLine(result.DifferencesString);
                Assert.True(result.AreEqual, "Objects Are not equal");
                
            }
            catch (Exception ex)
            {
                Assert.True(false, ex.ToString());
            }
        }


        [Fact]
        public void DeserialzieTests()
        {
            try
            {
                string path = Path.GetTempPath();
                var dbschema2 = Database.FromJsonFile(@"\\vmware-host\Shared Folders\Downloads\Unifier.BP.Schema.json");
            }
            catch (Exception ex)
            {
                Assert.True(false, ex.ToString());
            }
        }
    }
}