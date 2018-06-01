using System;
using System.Collections.Generic;
using System.IO;
using EzDbSchema.Core;
using EzDbSchema.Internal;
using KellermanSoftware.CompareNetObjects;
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
                EzDbSchema.MsSql.Database dbschema = new EzDbSchema.MsSql.Database();
                Assert.True(!AppSettings.Instance.ConnectionString.Contains("CHANGE ME"), "Change the Connection String in the App Settings");
                dbschema.Render("TestSchemaName", AppSettings.Instance.ConnectionString);
                var s = dbschema.AsXml();
                EzDbSchema.MsSql.Database dbschema2 = new EzDbSchema.MsSql.Database();
                dbschema2.FromXml(s);
                CompareLogic compareLogic = new CompareLogic();
                compareLogic.Config.SkipInvalidIndexers = true;
                compareLogic.Config.MaxDifferences = 9999;
                compareLogic.Config.MembersToIgnore.Add("_id");
                ComparisonResult result = compareLogic.Compare(dbschema, dbschema2);

                string path = Path.GetTempPath();
                File.WriteAllText(path + "db1.xml", s);
                File.WriteAllText(path + "db2.xml", dbschema2.AsXml());

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