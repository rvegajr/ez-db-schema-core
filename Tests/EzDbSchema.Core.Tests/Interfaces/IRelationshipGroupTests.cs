using EzDbSchema.Core.Interfaces;
using Moq;
using Xunit;

namespace EzDbSchema.Core.Tests.Interfaces
{
    public class IRelationshipGroupTests
    {
        [Fact]
        public void IRelationshipGroup_ImplementsDictionary()
        {
            // Verify that IRelationshipGroup extends IDictionary<string, IRelationshipList>
            Assert.True(typeof(IDictionary<string, IRelationshipList>).IsAssignableFrom(typeof(IRelationshipGroup)));
        }
        
        [Fact]
        public void IRelationshipGroup_HasDatabaseProperty()
        {
            // Verify that IRelationshipGroup has a Database property of type IDatabase
            var databaseProperty = typeof(IRelationshipGroup).GetProperty("Database");
            Assert.NotNull(databaseProperty);
            Assert.Equal(typeof(IDatabase), databaseProperty.PropertyType);
            Assert.True(databaseProperty.CanRead);
            Assert.True(databaseProperty.CanWrite);
        }
    }
}
