using System;
using System.Collections.Generic;
using System.IO;
using EzDbSchema.Core;
using EzDbSchema.Core.Objects;
using EzDbSchema.Internal;
using EzDbSchema.MsSql;
using Json.Comparer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Database = EzDbSchema.Core.Objects.Database;

namespace EzDbSchema.Tests
{
    [Collection("DatabaseCollection")]

    public class SchemaRenderTests
    {
        DatabaseFixture fixture;
        public SchemaRenderTests(DatabaseFixture _fixture)
        {
            this.fixture = _fixture;
        }

        [Fact]
        public void MsSqlSchemaRenderTests()
        {
			try
			{
				EzDbSchema.MsSql.Database dbschema = new EzDbSchema.MsSql.Database();
                dbschema.Render("TestSchemaName", this.fixture.ConnectionString);
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
                dbschema.Render("TestSchemaName", this.fixture.ConnectionString);
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

    }
}