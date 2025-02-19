using System;
using System.Linq;
using EzDbSchema.Core.CodeGen;
using EzDbSchema.Core.Interfaces;
using Moq;
using Xunit;

namespace EzDbSchema.Core.Tests.CodeGen
{
    public class RelationshipInfoTests
    {
        private readonly Mock<IRelationship> _mockRelationship;
        private readonly RelationshipInfo _sut;

        public RelationshipInfoTests()
        {
            _mockRelationship = new Mock<IRelationship>();
            _mockRelationship.Setup(r => r.ConstraintName).Returns("FK_Test");
            _mockRelationship.Setup(r => r.FromTableName).Returns("Order");
            _mockRelationship.Setup(r => r.FromPropertyName).Returns("CustomerId");
            _mockRelationship.Setup(r => r.ToTableName).Returns("Customer");
            _mockRelationship.Setup(r => r.ToPropertyName).Returns("Id");

            _sut = new RelationshipInfo(_mockRelationship.Object, RelationType.ManyToOne);
        }

        [Fact]
        public void BasicProperties_ShouldReturnCorrectValues()
        {
            // Assert
            Assert.Equal("FK_Test", _sut.ConstraintName);
            Assert.Equal(RelationType.ManyToOne, _sut.Type);
            Assert.Equal("Order", _sut.FromTableName);
            Assert.Equal("CustomerId", _sut.FromPropertyName);
            Assert.Equal("Customer", _sut.ToTableName);
            Assert.Equal("Id", _sut.ToPropertyName);
        }

        [Fact]
        public void FormattedNames_ShouldBeCorrectlyFormatted()
        {
            // Assert
            Assert.Equal("Order", _sut.FromClassName);
            Assert.Equal("CustomerId", _sut.FromPropertyClassName);
            Assert.Equal("Customer", _sut.ToClassName);
            Assert.Equal("Id", _sut.ToPropertyClassName);
        }

        [Fact]
        public void NavigationProperties_ShouldReflectRelationType()
        {
            // Test ManyToOne
            var manyToOne = new RelationshipInfo(_mockRelationship.Object, RelationType.ManyToOne);
            Assert.Equal("Customer", manyToOne.NavigationPropertyName);
            Assert.Equal("Orders", manyToOne.InverseNavigationPropertyName);

            // Test OneToMany
            var oneToMany = new RelationshipInfo(_mockRelationship.Object, RelationType.OneToMany);
            Assert.Equal("Customers", oneToMany.NavigationPropertyName);
            Assert.Equal("Order", oneToMany.InverseNavigationPropertyName);
        }

        [Fact]
        public void RelationshipFeatures_ShouldReflectConfiguration()
        {
            // Arrange
            _mockRelationship.Setup(r => r.IsOptional).Returns(true);
            _mockRelationship.Setup(r => r.CascadeDelete).Returns(true);
            _mockRelationship.Setup(r => r.CascadeAction).Returns("CASCADE");

            // Assert
            Assert.True(_sut.IsOptional);
            Assert.True(_sut.CascadeDelete);
            Assert.Equal("CASCADE", _sut.CascadeAction);
        }

        [Fact]
        public void PerformanceFeatures_ShouldIdentifyLoadingStrategies()
        {
            // Arrange
            _mockRelationship.Setup(r => r.RequiresLazyLoading).Returns(true);
            _mockRelationship.Setup(r => r.RequiresEagerLoading).Returns(false);
            _mockRelationship.Setup(r => r.RequiresIndexing).Returns(true);

            // Assert
            Assert.True(_sut.RequiresLazyLoading);
            Assert.False(_sut.RequiresEagerLoading);
            Assert.True(_sut.RequiresIndexing);
        }

        [Fact]
        public void ValidationFeatures_ShouldIdentifyConstraints()
        {
            // Arrange
            _mockRelationship.Setup(r => r.HasConstraints).Returns(true);
            _mockRelationship.Setup(r => r.Constraints).Returns(new[] { "NOT NULL", "UNIQUE" });
            _mockRelationship.Setup(r => r.RequiresReferentialIntegrity).Returns(true);

            // Assert
            Assert.True(_sut.HasConstraints);
            Assert.Equal(2, _sut.Constraints.Count());
            Assert.True(_sut.RequiresReferentialIntegrity);
        }

        [Fact]
        public void BusinessLogicFeatures_ShouldIdentifyPatterns()
        {
            // Arrange
            _mockRelationship.Setup(r => r.IsOwnership).Returns(true);
            _mockRelationship.Setup(r => r.IsAggregation).Returns(true);
            _mockRelationship.Setup(r => r.IsComposition).Returns(false);
            _mockRelationship.Setup(r => r.BusinessRole).Returns("Ownership");

            // Assert
            Assert.True(_sut.IsOwnership);
            Assert.True(_sut.IsAggregation);
            Assert.False(_sut.IsComposition);
            Assert.Equal("Ownership", _sut.BusinessRole);
        }

        [Fact]
        public void ApiFeatures_ShouldIdentifyIntegrationRequirements()
        {
            // Arrange
            _mockRelationship.Setup(r => r.IncludeInDefaultFetch).Returns(true);
            _mockRelationship.Setup(r => r.RequiresAuthorization).Returns(true);
            _mockRelationship.Setup(r => r.GeneratesEvents).Returns(true);

            // Assert
            Assert.True(_sut.IncludeInDefaultFetch);
            Assert.True(_sut.RequiresAuthorization);
            Assert.True(_sut.GeneratesEvents);
        }

        [Fact]
        public void DocumentationFeatures_ShouldProvideMetadata()
        {
            // Arrange
            _mockRelationship.Setup(r => r.Description).Returns("Customer Orders");
            _mockRelationship.Setup(r => r.Notes).Returns("Important relationship");
            _mockRelationship.Setup(r => r.Version).Returns("1.0");

            // Assert
            Assert.Equal("Customer Orders", _sut.Description);
            Assert.Equal("Important relationship", _sut.Notes);
            Assert.Equal("1.0", _sut.Version);
        }

        [Fact]
        public void ChangeTracking_ShouldIdentifyAuditRequirements()
        {
            // Arrange
            _mockRelationship.Setup(r => r.TrackChanges).Returns(true);
            _mockRelationship.Setup(r => r.AuditChanges).Returns(true);
            _mockRelationship.Setup(r => r.ChangeValidation).Returns("RequireReason");

            // Assert
            Assert.True(_sut.TrackChanges);
            Assert.True(_sut.AuditChanges);
            Assert.Equal("RequireReason", _sut.ChangeValidation);
        }
    }
}
