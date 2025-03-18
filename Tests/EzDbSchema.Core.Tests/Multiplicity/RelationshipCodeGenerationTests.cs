using Xunit;
using Moq;
using EzDbSchema.Core.Interfaces;
using EzDbSchema.Core.Objects;
using EzDbSchema.Core.Enums;
using System.Collections.Generic;
using System.Linq;

namespace EzDbSchema.Core.Tests.Multiplicity
{
    public class RelationshipCodeGenerationTests
    {
        [Fact]
        public void OneToOne_ShouldGenerateCorrectNavigationProperties()
        {
            // Arrange
            var relationship = CreateRelationship("Customer", "Address", "AddressId", true);
            relationship.FromPropertyName = "Customer";
            relationship.ToPropertyName = "Address";

            // Act & Assert
            Assert.Equal("Address", relationship.ToPropertyName);
            Assert.Equal("Customer", relationship.FromPropertyName);
        }

        [Fact]
        public void OneToMany_ShouldGenerateCorrectNavigationProperties()
        {
            // Arrange
            var relationship = CreateRelationship("Customer", "Order", "CustomerId", true);
            relationship.FromPropertyName = "Orders";
            relationship.ToPropertyName = "Customer";

            // Act & Assert
            Assert.Equal("Customer", relationship.ToPropertyName);
            Assert.Equal("Orders", relationship.FromPropertyName);
            Assert.False(relationship.ToPropertyName.EndsWith("s", StringComparison.OrdinalIgnoreCase));
            Assert.EndsWith("s", relationship.FromPropertyName);
        }

        [Fact]
        public void ManyToMany_ShouldGenerateCorrectNavigationProperties()
        {
            // Arrange
            var relationship = CreateRelationship("Student", "Course", "CourseId", false);
            relationship.MultiplicityType = RelationshipMultiplicityType.ManyToMany;
            relationship.FromPropertyName = "Courses";
            relationship.ToPropertyName = "Students";

            // Act & Assert
            Assert.Equal("Students", relationship.ToPropertyName);
            Assert.Equal("Courses", relationship.FromPropertyName);
            Assert.EndsWith("s", relationship.ToPropertyName);
            Assert.EndsWith("s", relationship.FromPropertyName);
        }

        [Fact]
        public void SelfReferencing_ShouldGenerateDescriptiveNavigationProperties()
        {
            // Arrange
            var relationship = CreateRelationship("Employee", "Employee", "ManagerId", false);
            relationship.FromPropertyName = "DirectReports";
            relationship.ToPropertyName = "Manager";

            // Act & Assert
            Assert.Equal("Manager", relationship.ToPropertyName);
            Assert.Equal("DirectReports", relationship.FromPropertyName);
            Assert.NotEqual(relationship.ToPropertyName, relationship.FromPropertyName);
        }

        [Fact]
        public void CompositeKey_ShouldGenerateAppropriatePropertyNames()
        {
            // Arrange
            var relationship = CreateCompositeRelationship(
                "OrderDetail", "ProductVariant",
                new[] { "ProductId", "ColorId" }
            );
            relationship.FromPropertyName = "OrderDetails";
            relationship.ToPropertyName = "ProductVariant";

            // Act & Assert
            Assert.Equal("ProductVariant", relationship.ToPropertyName);
            Assert.Equal("OrderDetails", relationship.FromPropertyName);
        }

        [Fact]
        public void OptionalRelationship_ShouldGenerateNullableTypes()
        {
            // Arrange
            var relationship = CreateRelationship("Order", "Customer", "OptionalCustomerId", false);

            // Act & Assert
            Assert.True(relationship.FromProperty.IsNullable);
            Assert.Equal("int?", relationship.FromProperty.DataType);
        }

        [Fact]
        public void RequiredRelationship_ShouldGenerateNonNullableTypes()
        {
            // Arrange
            var relationship = CreateRelationship("Order", "Customer", "CustomerId", true);

            // Act & Assert
            Assert.False(relationship.FromProperty.IsNullable);
            Assert.Equal("int", relationship.FromProperty.DataType);
        }

        [Fact]
        public void NavigationProperty_ShouldHaveCorrectCollectionType()
        {
            // Arrange
            var relationship = CreateRelationship("Customer", "Order", "CustomerId", true);
            relationship.MultiplicityType = RelationshipMultiplicityType.OneToMany;
            relationship.FromPropertyName = "Orders";
            relationship.FromProperty = CreateMockProperty("Orders", "ICollection<Order>", true);

            // Act & Assert
            Assert.EndsWith("s", relationship.FromPropertyName);
            Assert.Contains("ICollection<", relationship.FromProperty.DataType);
        }

        [Fact]
        public void ForeignKey_ShouldFollowNamingConvention()
        {
            // Arrange
            var relationship = CreateRelationship("Order", "Customer", "CustomerId", true);

            // Act & Assert
            Assert.EndsWith("Id", relationship.FromColumnName);
            Assert.Equal(relationship.ToColumnName, relationship.FromColumnName);
        }

        private IRelationship CreateRelationship(string fromTable, string toTable, string columnName, bool isRequired)
        {
            var relationship = new Relationship
            {
                FromTableName = fromTable,
                ToTableName = toTable,
                FromColumnName = columnName,
                ToColumnName = columnName,
                FromPropertyName = toTable,
                ToPropertyName = fromTable + "s",
                FromProperty = CreateMockProperty(columnName, isRequired ? "int" : "int?", isRequired),
                ToProperty = CreateMockProperty("Id", "int", true)
            };

            return relationship;
        }

        private IRelationship CreateCompositeRelationship(string fromTable, string toTable, string[] columnNames)
        {
            var relationship = new Relationship
            {
                FromTableName = fromTable,
                ToTableName = toTable,
                FromProperties = columnNames.Select(name => CreateMockProperty(name, "int", true)).ToList(),
                ToProperties = columnNames.Select(name => CreateMockProperty(name, "int", true)).ToList(),
                FromPropertyName = toTable,
                ToPropertyName = fromTable + "s"
            };

            return relationship;
        }

        private IProperty CreateMockProperty(string name, string dataType, bool isRequired)
        {
            var mockProperty = new Mock<IProperty>();
            mockProperty.Setup(p => p.PropertyName).Returns(name);
            mockProperty.Setup(p => p.DataType).Returns(dataType);
            mockProperty.Setup(p => p.IsRequired).Returns(isRequired);
            mockProperty.Setup(p => p.IsNullable).Returns(!isRequired);
            return mockProperty.Object;
        }
    }
}
