using EzDbSchema.Core.Enums;
using EzDbSchema.Core.Interfaces;
using EzDbSchema.Core.Objects;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace EzDbSchema.Core.Tests.Objects
{
    public class RelationshipSummaryTests
    {
        [Fact]
        public void RelationshipSummary_DefaultProperties_InitializedCorrectly()
        {
            // Arrange & Act
            var summary = new RelationshipSummary();
            
            // Assert
            Assert.Null(summary.Entity);
            Assert.Empty(summary.FromPropertyName);
            Assert.Empty(summary.FromColumnName);
            Assert.Empty(summary.ToPropertyName);
            Assert.Empty(summary.ToColumnName);
            Assert.Empty(summary.ToColumnProperties);
            Assert.Empty(summary.ToObjectPropertyName);
            Assert.Empty(summary.Types);
            Assert.Empty(summary.MultiplicityTypes);
            Assert.Equal("", summary.FromTableName);
            Assert.Equal("", summary.ConstraintName);
            Assert.Equal("", summary.ToTableName);
            Assert.Equal("", summary.PrimaryTableName);
            Assert.Empty(summary.FromColumnProperties);
            Assert.Equal(RelationshipMultiplicityType.Unknown, summary.MultiplicityType);
            Assert.Empty(summary.FromObjectPropertyName);
            Assert.Equal("", summary.Type);
            Assert.False(summary.MultiplicityTypeWarning);
        }
        
        [Fact]
        public void RelationshipSummary_Properties_CanBeSet()
        {
            // Arrange
            var summary = new RelationshipSummary();
            var mockEntity = new Mock<IEntity>();
            var mockProperty = new Mock<IProperty>();
            
            // Act
            summary.Entity = mockEntity.Object;
            summary.FromPropertyName.Add("CustomerID");
            summary.FromColumnName.Add("CustomerID");
            summary.ToPropertyName.Add("ID");
            summary.ToColumnName.Add("ID");
            summary.ToColumnProperties.Add(mockProperty.Object);
            summary.ToObjectPropertyName.Add("Customer");
            summary.Types.Add("One to Many");
            summary.MultiplicityTypes.Add(RelationshipMultiplicityType.OneToMany);
            summary.FromTableName = "Order";
            summary.ConstraintName = "FK_Order_Customer";
            summary.ToTableName = "Customer";
            summary.PrimaryTableName = "Customer";
            summary.FromColumnProperties.Add(mockProperty.Object);
            summary.MultiplicityType = RelationshipMultiplicityType.OneToMany;
            summary.FromObjectPropertyName.Add("Orders");
            summary.Type = "One to Many";
            summary.MultiplicityTypeWarning = true;
            
            // Assert
            Assert.Equal(mockEntity.Object, summary.Entity);
            Assert.Single(summary.FromPropertyName);
            Assert.Equal("CustomerID", summary.FromPropertyName[0]);
            Assert.Single(summary.FromColumnName);
            Assert.Equal("CustomerID", summary.FromColumnName[0]);
            Assert.Single(summary.ToPropertyName);
            Assert.Equal("ID", summary.ToPropertyName[0]);
            Assert.Single(summary.ToColumnName);
            Assert.Equal("ID", summary.ToColumnName[0]);
            Assert.Single(summary.ToColumnProperties);
            Assert.Equal(mockProperty.Object, summary.ToColumnProperties[0]);
            Assert.Single(summary.ToObjectPropertyName);
            Assert.Equal("Customer", summary.ToObjectPropertyName[0]);
            Assert.Single(summary.Types);
            Assert.Equal("One to Many", summary.Types[0]);
            Assert.Single(summary.MultiplicityTypes);
            Assert.Equal(RelationshipMultiplicityType.OneToMany, summary.MultiplicityTypes[0]);
            Assert.Equal("Order", summary.FromTableName);
            Assert.Equal("FK_Order_Customer", summary.ConstraintName);
            Assert.Equal("Customer", summary.ToTableName);
            Assert.Equal("Customer", summary.PrimaryTableName);
            Assert.Single(summary.FromColumnProperties);
            Assert.Equal(mockProperty.Object, summary.FromColumnProperties[0]);
            Assert.Equal(RelationshipMultiplicityType.OneToMany, summary.MultiplicityType);
            Assert.Single(summary.FromObjectPropertyName);
            Assert.Equal("Orders", summary.FromObjectPropertyName[0]);
            Assert.Equal("One to Many", summary.Type);
            Assert.True(summary.MultiplicityTypeWarning);
        }
    }
}
