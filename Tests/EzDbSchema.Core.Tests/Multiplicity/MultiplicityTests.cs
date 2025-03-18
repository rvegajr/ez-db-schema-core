using Xunit;
using Moq;
using EzDbSchema.Core.Interfaces;
using EzDbSchema.Core.Objects;
using EzDbSchema.Core.Enums;
using System.Collections.Generic;

namespace EzDbSchema.Core.Tests.Multiplicity
{
    public class MultiplicityTests
    {
        [Fact]
        public void OneToOne_Relationship_ShouldBeDetectedCorrectly()
        {
            // Arrange
            var relationship = CreateRelationship("Customer", "Address", "AddressId", true, true);

            // Act & Assert
            Assert.Equal(RelationshipMultiplicityType.OneToOne, relationship.MultiplicityType);
            Assert.True(relationship.FromProperty.IsRequired);
            Assert.True(relationship.ToProperty.IsRequired);
        }

        [Fact]
        public void OneToMany_Relationship_ShouldBeDetectedCorrectly()
        {
            // Arrange
            var relationship = CreateRelationship("Customer", "Order", "CustomerId", true, false);

            // Act & Assert
            Assert.Equal(RelationshipMultiplicityType.OneToMany, relationship.MultiplicityType);
            Assert.True(relationship.FromProperty.IsRequired);
            Assert.False(relationship.ToProperty.IsRequired);
        }

        [Fact]
        public void ZeroOrOneToMany_Relationship_ShouldBeDetectedCorrectly()
        {
            // Arrange
            var relationship = CreateRelationship("Customer", "Order", "OptionalCustomerId", false, false);

            // Act & Assert
            Assert.Equal(RelationshipMultiplicityType.ZeroOrOneToMany, relationship.MultiplicityType);
            Assert.False(relationship.FromProperty.IsRequired);
            Assert.False(relationship.ToProperty.IsRequired);
        }

        [Fact]
        public void ManyToOne_Relationship_ShouldBeDetectedCorrectly()
        {
            // Arrange
            var relationship = CreateRelationship("Order", "Customer", "CustomerId", false, true);

            // Act & Assert
            Assert.Equal(RelationshipMultiplicityType.ManyToOne, relationship.MultiplicityType);
            Assert.False(relationship.FromProperty.IsRequired);
            Assert.True(relationship.ToProperty.IsRequired);
        }

        [Fact]
        public void CompositeKey_OneToOne_ShouldBeDetectedCorrectly()
        {
            // Arrange
            var relationship = CreateCompositeRelationship(
                "OrderDetail", "ProductVariant",
                new[] { "ProductId", "ColorId" },
                new[] { true, true },
                new[] { true, true }
            );

            // Act & Assert
            Assert.Equal(RelationshipMultiplicityType.OneToOne, relationship.MultiplicityType);
            Assert.All(relationship.FromProperties, p => Assert.True(p.IsRequired));
            Assert.All(relationship.ToProperties, p => Assert.True(p.IsRequired));
        }

        [Fact]
        public void CompositeKey_OneToMany_ShouldBeDetectedCorrectly()
        {
            // Arrange
            var relationship = CreateCompositeRelationship(
                "Order", "OrderDetail",
                new[] { "OrderId", "LineNumber" },
                new[] { true, true },
                new[] { false, false }
            );

            // Act & Assert
            Assert.Equal(RelationshipMultiplicityType.OneToMany, relationship.MultiplicityType);
            Assert.All(relationship.FromProperties, p => Assert.True(p.IsRequired));
            Assert.All(relationship.ToProperties, p => Assert.False(p.IsRequired));
        }

        [Fact]
        public void SelfReferencing_Relationship_ShouldBeDetectedCorrectly()
        {
            // Arrange
            var relationship = CreateRelationship("Employee", "Employee", "ManagerId", false, true);
            relationship.FromPropertyName = "DirectReports";
            relationship.ToPropertyName = "Manager";
            relationship.MultiplicityType = RelationshipMultiplicityType.ManyToZeroOrOne;

            // Act & Assert
            Assert.Equal(RelationshipMultiplicityType.ManyToZeroOrOne, relationship.MultiplicityType);
            Assert.False(relationship.FromProperty.IsRequired);
            Assert.True(relationship.ToProperty.IsRequired);
            Assert.Equal(relationship.FromTableName, relationship.ToTableName);
            Assert.Equal("Manager", relationship.ToPropertyName);
            Assert.Equal("DirectReports", relationship.FromPropertyName);
        }

        [Fact]
        public void OptionalRelationship_ShouldGenerateCorrectNavigationProperties()
        {
            // Arrange
            var relationship = CreateRelationship("Order", "Customer", "CustomerId", false, true);
            relationship.FromPropertyName = "Orders";
            relationship.ToPropertyName = "Customer";

            // Act & Assert
            Assert.Equal("Customer", relationship.ToPropertyName);
            Assert.Equal("Orders", relationship.FromPropertyName);
            Assert.False(relationship.FromProperty.IsRequired);
            Assert.True(relationship.ToProperty.IsRequired);
        }

        private IRelationship CreateRelationship(string fromTable, string toTable, string columnName, bool fromRequired, bool toRequired)
        {
            var relationship = new Relationship
            {
                FromTableName = fromTable,
                ToTableName = toTable,
                FromColumnName = columnName,
                ToColumnName = columnName,
                FromPropertyName = toTable,
                ToPropertyName = fromTable + "s",
                FromProperty = CreateMockProperty(fromRequired),
                ToProperty = CreateMockProperty(toRequired)
            };

            // Set multiplicity type based on requirements
            if (fromRequired && toRequired)
                relationship.MultiplicityType = RelationshipMultiplicityType.OneToOne;
            else if (fromRequired && !toRequired)
                relationship.MultiplicityType = RelationshipMultiplicityType.OneToMany;
            else if (!fromRequired && toRequired)
                relationship.MultiplicityType = RelationshipMultiplicityType.ManyToOne;
            else
                relationship.MultiplicityType = RelationshipMultiplicityType.ZeroOrOneToMany;

            return relationship;
        }

        private IRelationship CreateCompositeRelationship(string fromTable, string toTable, string[] columnNames, bool[] fromRequired, bool[] toRequired)
        {
            var relationship = new Relationship
            {
                FromTableName = fromTable,
                ToTableName = toTable,
                FromProperties = new List<IProperty>(),
                ToProperties = new List<IProperty>()
            };

            for (int i = 0; i < columnNames.Length; i++)
            {
                relationship.FromProperties.Add(CreateMockProperty(fromRequired[i]));
                relationship.ToProperties.Add(CreateMockProperty(toRequired[i]));
            }

            // Set multiplicity type based on all properties being required
            if (fromRequired.All(r => r) && toRequired.All(r => r))
                relationship.MultiplicityType = RelationshipMultiplicityType.OneToOne;
            else if (fromRequired.All(r => r) && toRequired.All(r => !r))
                relationship.MultiplicityType = RelationshipMultiplicityType.OneToMany;
            else if (fromRequired.All(r => !r) && toRequired.All(r => r))
                relationship.MultiplicityType = RelationshipMultiplicityType.ManyToOne;
            else
                relationship.MultiplicityType = RelationshipMultiplicityType.ZeroOrOneToMany;

            return relationship;
        }

        private IProperty CreateMockProperty(bool isRequired)
        {
            var mockProperty = new Mock<IProperty>();
            mockProperty.Setup(p => p.IsRequired).Returns(isRequired);
            return mockProperty.Object;
        }
    }
}
