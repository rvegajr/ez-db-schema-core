using System;
using System.Collections.Generic;
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
                ComparisonResult result = compareLogic.Compare(dbschema, dbschema2);

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