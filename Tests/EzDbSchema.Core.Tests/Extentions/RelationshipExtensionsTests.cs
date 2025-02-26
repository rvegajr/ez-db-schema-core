using EzDbSchema.Core.Enums;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Interfaces;
using Moq;
using System;
using Xunit;

namespace EzDbSchema.Core.Tests.Extentions
{
    public class RelationshipExtensionsTests
    {
        [Theory]
        [InlineData(RelationshipMultiplicityType.OneToMany, true)]
        [InlineData(RelationshipMultiplicityType.ZeroOrOneToMany, true)]
        [InlineData(RelationshipMultiplicityType.OneToOne, false)]
        [InlineData(RelationshipMultiplicityType.ManyToOne, false)]
        [InlineData(RelationshipMultiplicityType.Unknown, false)]
        public void EndsAsMany_ShouldReturnCorrectValue(RelationshipMultiplicityType multiplicityType, bool expected)
        {
            // Arrange
            var relationshipMock = new Mock<IRelationship>();
            relationshipMock.Setup(r => r.MultiplicityType).Returns(multiplicityType);

            // Act
            var result = relationshipMock.Object.EndsAsMany();

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(RelationshipMultiplicityType.ManyToOne, true)]
        [InlineData(RelationshipMultiplicityType.ManyToZeroOrOne, true)]
        [InlineData(RelationshipMultiplicityType.OneToOne, false)]
        [InlineData(RelationshipMultiplicityType.OneToMany, false)]
        [InlineData(RelationshipMultiplicityType.Unknown, false)]
        public void BeginsAsMany_ShouldReturnCorrectValue(RelationshipMultiplicityType multiplicityType, bool expected)
        {
            // Arrange
            var relationshipMock = new Mock<IRelationship>();
            relationshipMock.Setup(r => r.MultiplicityType).Returns(multiplicityType);

            // Act
            var result = relationshipMock.Object.BeginsAsMany();

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(RelationshipMultiplicityType.OneToMany, true)]
        [InlineData(RelationshipMultiplicityType.OneToOne, true)]
        [InlineData(RelationshipMultiplicityType.OneToZeroOrOne, true)]
        [InlineData(RelationshipMultiplicityType.ManyToOne, false)]
        [InlineData(RelationshipMultiplicityType.ZeroOrOneToMany, false)]
        [InlineData(RelationshipMultiplicityType.Unknown, false)]
        public void BeginsAsOne_ShouldReturnCorrectValue(RelationshipMultiplicityType multiplicityType, bool expected)
        {
            // Arrange
            var relationshipMock = new Mock<IRelationship>();
            relationshipMock.Setup(r => r.MultiplicityType).Returns(multiplicityType);

            // Act
            var result = relationshipMock.Object.BeginsAsOne();

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(RelationshipMultiplicityType.ZeroOrOneToMany, true)]
        [InlineData(RelationshipMultiplicityType.ZeroOrOneToOne, true)]
        [InlineData(RelationshipMultiplicityType.OneToOne, false)]
        [InlineData(RelationshipMultiplicityType.ManyToOne, false)]
        [InlineData(RelationshipMultiplicityType.Unknown, false)]
        public void BeginsAsZeroOrOne_ShouldReturnCorrectValue(RelationshipMultiplicityType multiplicityType, bool expected)
        {
            // Arrange
            var relationshipMock = new Mock<IRelationship>();
            relationshipMock.Setup(r => r.MultiplicityType).Returns(multiplicityType);

            // Act
            var result = relationshipMock.Object.BeginsAsZeroOrOne();

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(RelationshipMultiplicityType.ManyToOne, true)]
        [InlineData(RelationshipMultiplicityType.OneToOne, true)]
        [InlineData(RelationshipMultiplicityType.ZeroOrOneToOne, true)]
        [InlineData(RelationshipMultiplicityType.OneToMany, false)]
        [InlineData(RelationshipMultiplicityType.ManyToZeroOrOne, false)]
        [InlineData(RelationshipMultiplicityType.Unknown, false)]
        public void EndsAsOne_ShouldReturnCorrectValue(RelationshipMultiplicityType multiplicityType, bool expected)
        {
            // Arrange
            var relationshipMock = new Mock<IRelationship>();
            relationshipMock.Setup(r => r.MultiplicityType).Returns(multiplicityType);

            // Act
            var result = relationshipMock.Object.EndsAsOne();

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(RelationshipMultiplicityType.ManyToZeroOrOne, true)]
        [InlineData(RelationshipMultiplicityType.OneToZeroOrOne, true)]
        [InlineData(RelationshipMultiplicityType.OneToOne, false)]
        [InlineData(RelationshipMultiplicityType.OneToMany, false)]
        [InlineData(RelationshipMultiplicityType.Unknown, false)]
        public void EndsAsZeroOrOne_ShouldReturnCorrectValue(RelationshipMultiplicityType multiplicityType, bool expected)
        {
            // Arrange
            var relationshipMock = new Mock<IRelationship>();
            relationshipMock.Setup(r => r.MultiplicityType).Returns(multiplicityType);

            // Act
            var result = relationshipMock.Object.EndsAsZeroOrOne();

            // Assert
            Assert.Equal(expected, result);
        }

        // Testing GenerateObjectName would require more complex mocking of IEntity, IRelationshipGroup, etc.
        // This would be implemented in a separate test method
    }
}
