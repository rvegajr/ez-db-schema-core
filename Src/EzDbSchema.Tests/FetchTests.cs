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

namespace EzDbSchema.Tests
{
    [Collection("DatabaseCollection")]
    public class EntityMethodTests
	{
        DatabaseFixture fixture;
        public EntityMethodTests(DatabaseFixture _fixture)
        {
            this.fixture = _fixture;
        }

        [Fact]
        public void MsSqlConnectionStringParseTest()
        {
            var connparms = new ConnectionParameters() { Database = DatabaseFixture.DATABASE_NAME, Server = $"(localdb)\\{DatabaseFixture.LOCALDB_NAME}", Trusted = true };
            connparms.ConnectionString = this.fixture.ConnectionString;
            connparms.UserName = "TRUSTED";
            Assert.True(connparms.ConnectionString.Equals(@"Data Source=(localdb)\EzDbSchemaTestDB;Initial Catalog=Northwind;Integrated Security=True;"), @"Connection string should be 'Data Source=(localdb)\EzDbSchemaTestDB;Initial Catalog=Northwind;Integrated Security=True;'");
            connparms.UserName = "";
            connparms.Trusted = false;
            connparms.Trusted = true;
            Assert.True(connparms.ConnectionString.Equals(@"Data Source=(localdb)\EzDbSchemaTestDB;Initial Catalog=Northwind;Integrated Security=True;"), @"Connection string should be 'Data Source=(localdb)\EzDbSchemaTestDB;Initial Catalog=Northwind;Integrated Security=True;'");
            Assert.True(connparms.IsValid(), "Connection should be valid");
        }


        [Fact]
        public void MsSqlConnectionParameterTest()
        {
            var connparms = new ConnectionParameters() { Database = DatabaseFixture.DATABASE_NAME, Server = $"(localdb)\\{DatabaseFixture.LOCALDB_NAME}", Trusted = true };
            Assert.True(connparms.IsValid(), "Connection should be valid");
        }


        [Fact]
		public void FetchTests()
		{
            EzDbSchema.MsSql.Database dbschema = new EzDbSchema.MsSql.Database();
            dbschema.Render("TestSchemaName", this.fixture.ConnectionString);
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