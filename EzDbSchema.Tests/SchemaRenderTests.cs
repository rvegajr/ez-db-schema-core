using System;
using System.Collections.Generic;
using EzDbSchema.Core;
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
    }
}