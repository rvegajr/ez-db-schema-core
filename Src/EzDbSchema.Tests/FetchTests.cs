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
	public class EntityMethodTests
	{
		[Fact]
		public void FetchTests()
		{
            EzDbSchema.MsSql.Database dbschema = new EzDbSchema.MsSql.Database();
            //Assert.True(!AppSettings.Instance.ConnectionString.Contains("CHANGE ME"), "Change the Connection String in the App Settings");
            AppSettings.Instance.ConnectionString = "Server=localhost;Database=***REMOVED***;user id=***REMOVED***;password=***REMOVED***";
            dbschema.Render("TestSchemaName", AppSettings.Instance.ConnectionString);
            foreach (var e in dbschema.Entities.Values)
            {
                try
                {
                    var k1 = e.Relationships.Fetch(Core.Enums.RelationshipMultiplicityType.ManyToOne);
                    var k2 = e.Relationships.Fetch(Core.Enums.RelationshipMultiplicityType.ManyToZeroOrOne);
                    var k3 = e.Relationships.Fetch(Core.Enums.RelationshipMultiplicityType.OneToMany);
                    var k4 = e.Relationships.Fetch(Core.Enums.RelationshipMultiplicityType.OneToOne);
                    var k5 = e.Relationships.Fetch(Core.Enums.RelationshipMultiplicityType.ZeroOrOneToMany);
                }
                catch (Exception)
                {
                    Assert.True(false, "Fetch calls failed");
                }

                break;
            }
            Assert.True(dbschema.Entities.Count > 0, "No entites returned");
		}
	}
}