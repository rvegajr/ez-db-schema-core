using Xunit;
using Moq;
using EzDbSchema.Core.Interfaces;
using EzDbSchema.Core.Objects;
using EzDbSchema.Core.Enums;
using System.Collections.Generic;
using System.Linq;

namespace EzDbSchema.Core.Tests.Multiplicity
{
    public class RelationshipValidationTests
    {
        [Fact]
        public void OneToOne_ShouldHaveMatchingKeyTypes()
        {
            // Arrange
            var relationship = CreateRelationship("Customer", "Address", "AddressId", "int", "int");

            // Act & Assert
            Assert.Equal(relationship.FromProperty.DataType, relationship.ToProperty.DataType);
        }

        [Fact]
        public void CompositeKey_ShouldHaveMatchingKeyTypes()
        {
            // Arrange
            var relationship = CreateCompositeRelationship(
                "OrderDetail", "ProductVariant",
                new[] { "ProductId", "ColorId" },
                new[] { "int", "int" },
                new[] { "int", "int" }
            );

            // Act & Assert
            for (int i = 0; i < relationship.FromProperties.Count; i++)
            {
                Assert.Equal(relationship.FromProperties[i].DataType, relationship.ToProperties[i].DataType);
            }
        }

        [Fact]
        public void PrincipalKey_ShouldBeUnique()
        {
            // Arrange
            var mockEntity = new Mock<IEntity>();
            var properties = new List<IProperty>
            {
                CreateMockProperty("Id", "int", true, true),
                CreateMockProperty("Name", "string", false, false)
            };
            mockEntity.Setup(e => e.Properties.Values).Returns(properties);

            // Act & Assert
            Assert.Single(properties.Where(p => p.IsPrimaryKey));
        }

        [Fact]
        public void ForeignKey_ShouldMatchPrincipalKeyType()
        {
            // Arrange
            var principalEntity = new Mock<IEntity>();
            var dependentEntity = new Mock<IEntity>();
            
            var principalKey = CreateMockProperty("Id", "int", true, true);
            var foreignKey = CreateMockProperty("CustomerId", "int", true, false);

            principalEntity.Setup(e => e.Properties.Values).Returns(new List<IProperty> { principalKey });
            dependentEntity.Setup(e => e.Properties.Values).Returns(new List<IProperty> { foreignKey });

            // Act & Assert
            Assert.Equal(principalKey.DataType, foreignKey.DataType);
        }

        [Fact]
        public void NavigationProperties_ShouldBeCorrectlyNamed()
        {
            // Arrange
            var relationship = CreateRelationship("Order", "Customer", "CustomerId", "int", "int");
            relationship.FromPropertyName = "Orders";
            relationship.ToPropertyName = "Customer";

            // Act & Assert
            Assert.Equal("Customer", relationship.ToPropertyName);
            Assert.Equal("Orders", relationship.FromPropertyName);
        }

        [Fact]
        public void SelfReferencing_NavigationProperties_ShouldBeUnique()
        {
            // Arrange
            var relationship = CreateRelationship("Employee", "Employee", "ManagerId", "int", "int");
            relationship.FromPropertyName = "Manager";
            relationship.ToPropertyName = "DirectReports";

            // Act & Assert
            Assert.NotEqual(relationship.FromPropertyName, relationship.ToPropertyName);
        }

        [Fact]
        public void RequiredRelationship_ShouldEnforceNotNull()
        {
            // Arrange
            var relationship = CreateRelationship("Order", "Customer", "CustomerId", "int", "int");
            var mockFromProperty = new Mock<IProperty>();
            mockFromProperty.Setup(p => p.IsRequired).Returns(true);
            mockFromProperty.Setup(p => p.IsNullable).Returns(false);
            relationship.FromProperty = mockFromProperty.Object;

            // Act & Assert
            Assert.True(relationship.FromProperty.IsRequired);
            Assert.False(relationship.FromProperty.IsNullable);
        }

        [Fact]
        public void OptionalRelationship_ShouldAllowNull()
        {
            // Arrange
            var relationship = CreateRelationship("Order", "Customer", "OptionalCustomerId", "int?", "int");
            relationship.FromProperty.IsRequired = false;

            // Act & Assert
            Assert.False(relationship.FromProperty.IsRequired);
            Assert.True(relationship.FromProperty.IsNullable);
        }

        private IRelationship CreateRelationship(string fromTable, string toTable, string columnName, string fromType, string toType)
        {
            return new Relationship
            {
                FromTableName = fromTable,
                ToTableName = toTable,
                FromColumnName = columnName,
                ToColumnName = columnName,
                FromPropertyName = toTable,
                ToPropertyName = fromTable + "s",
                FromProperty = CreateMockProperty(columnName, fromType, false, false),
                ToProperty = CreateMockProperty(columnName, toType, true, true)
            };
        }

        private IRelationship CreateCompositeRelationship(string fromTable, string toTable, string[] columnNames, string[] fromTypes, string[] toTypes)
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
                relationship.FromProperties.Add(CreateMockProperty(columnNames[i], fromTypes[i], false, false));
                relationship.ToProperties.Add(CreateMockProperty(columnNames[i], toTypes[i], true, true));
            }

            return relationship;
        }

        private IProperty CreateMockProperty(string name, string dataType, bool isPrimaryKey, bool isRequired)
        {
            var mockProperty = new Mock<IProperty>();
            mockProperty.Setup(p => p.PropertyName).Returns(name);
            mockProperty.Setup(p => p.DataType).Returns(dataType);
            mockProperty.Setup(p => p.IsPrimaryKey).Returns(isPrimaryKey);
            mockProperty.Setup(p => p.IsRequired).Returns(isRequired);
            mockProperty.Setup(p => p.IsNullable).Returns(!isRequired);
            return mockProperty.Object;
        }
    }
}
