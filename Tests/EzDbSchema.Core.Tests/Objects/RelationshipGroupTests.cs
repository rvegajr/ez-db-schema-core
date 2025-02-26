using EzDbSchema.Core.Enums;
using EzDbSchema.Core.Interfaces;
using EzDbSchema.Core.Objects;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace EzDbSchema.Core.Tests.Objects
{
    public class RelationshipGroupTests
    {
        [Fact]
        public void RelationshipGroup_ImplementsIRelationshipGroup()
        {
            // Arrange
            var relationshipGroup = new RelationshipGroup();
            
            // Assert
            Assert.IsAssignableFrom<IRelationshipGroup>(relationshipGroup);
            Assert.IsAssignableFrom<IDictionary<string, IRelationshipList>>(relationshipGroup);
        }
        
        [Fact]
        public void RelationshipGroup_DatabaseProperty_WorksCorrectly()
        {
            // Arrange
            var relationshipGroup = new RelationshipGroup();
            var mockDatabase = new Mock<IDatabase>();
            
            // Act
            relationshipGroup.Database = mockDatabase.Object;
            
            // Assert
            Assert.Equal(mockDatabase.Object, relationshipGroup.Database);
        }
        
        [Fact]
        public void CountItems_WithDefaultSearchField_CountsCorrectly()
        {
            // Arrange
            var relationshipGroup = new RelationshipGroup();
            
            // Create mock relationships
            var mockRelationship1 = new Mock<IRelationship>();
            mockRelationship1.Setup(r => r.ToTableName).Returns("Customer");
            
            var mockRelationship2 = new Mock<IRelationship>();
            mockRelationship2.Setup(r => r.ToTableName).Returns("Order");
            
            var mockRelationship3 = new Mock<IRelationship>();
            mockRelationship3.Setup(r => r.ToTableName).Returns("CustomerOrder");
            
            // Create mock relationship lists
            var mockRelationshipList1 = new Mock<IRelationshipList>();
            mockRelationshipList1.Setup(l => l.GetEnumerator()).Returns(
                new List<IRelationship> { mockRelationship1.Object, mockRelationship3.Object }.GetEnumerator());
            
            var mockRelationshipList2 = new Mock<IRelationshipList>();
            mockRelationshipList2.Setup(l => l.GetEnumerator()).Returns(
                new List<IRelationship> { mockRelationship2.Object }.GetEnumerator());
            
            // Add to relationship group
            relationshipGroup.Add("FK1", mockRelationshipList1.Object);
            relationshipGroup.Add("FK2", mockRelationshipList2.Object);
            
            // Act
            int count = relationshipGroup.CountItems("Customer");
            
            // Assert
            Assert.Equal(2, count); // Should find "Customer" and "CustomerOrder"
        }
        
        [Fact]
        public void CountItems_WithSpecificSearchField_CountsCorrectly()
        {
            // Arrange
            var relationshipGroup = new RelationshipGroup();
            
            // Create mock relationships
            var mockRelationship1 = new Mock<IRelationship>();
            mockRelationship1.Setup(r => r.FromColumnName).Returns("CustomerID");
            
            var mockRelationship2 = new Mock<IRelationship>();
            mockRelationship2.Setup(r => r.FromColumnName).Returns("OrderID");
            
            var mockRelationship3 = new Mock<IRelationship>();
            mockRelationship3.Setup(r => r.FromColumnName).Returns("CustomerOrderID");
            
            // Create mock relationship lists
            var mockRelationshipList1 = new Mock<IRelationshipList>();
            mockRelationshipList1.Setup(l => l.GetEnumerator()).Returns(
                new List<IRelationship> { mockRelationship1.Object, mockRelationship3.Object }.GetEnumerator());
            
            var mockRelationshipList2 = new Mock<IRelationshipList>();
            mockRelationshipList2.Setup(l => l.GetEnumerator()).Returns(
                new List<IRelationship> { mockRelationship2.Object }.GetEnumerator());
            
            // Add to relationship group
            relationshipGroup.Add("FK1", mockRelationshipList1.Object);
            relationshipGroup.Add("FK2", mockRelationshipList2.Object);
            
            // Act
            int count = relationshipGroup.CountItems(RelationSearchField.FromColumnName, "Customer");
            
            // Assert
            Assert.Equal(2, count); // Should find "CustomerID" and "CustomerOrderID"
        }
    }
}
