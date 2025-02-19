using System;
using System.Linq;
using EzDbSchema.Core.CodeGen;
using EzDbSchema.Core.Interfaces;
using Moq;
using Xunit;

namespace EzDbSchema.Core.Tests.CodeGen
{
    public class EntityCodeGenInfoTests
    {
        private readonly Mock<IEntity> _mockEntity;
        private readonly Mock<IPropertyDictionary> _mockProperties;
        private readonly Mock<IRelationshipReferenceList> _mockRelationships;
        private readonly EntityCodeGenInfo _sut;

        public EntityCodeGenInfoTests()
        {
            _mockEntity = new Mock<IEntity>();
            _mockProperties = new Mock<IPropertyDictionary>();
            _mockRelationships = new Mock<IRelationshipReferenceList>();

            _mockEntity.Setup(e => e.Properties).Returns(_mockProperties.Object);
            _mockEntity.Setup(e => e.Relationships).Returns(_mockRelationships.Object);
            _mockEntity.Setup(e => e.TableName).Returns("TestTable");
            _mockEntity.Setup(e => e.DatabaseSchema).Returns("dbo");

            _sut = new EntityCodeGenInfo(_mockEntity.Object);
        }

        [Fact]
        public void BasicProperties_ShouldReturnCorrectValues()
        {
            // Assert
            Assert.Equal("TestTable", _sut.TableName);
            Assert.Equal("dbo", _sut.SchemaName);
            Assert.Equal("dbo.TestTable", _sut.FullName);
            Assert.Equal("TestTable", _sut.ClassName);
            Assert.Equal("TestTables", _sut.ClassNamePlural);
            Assert.Equal("testTable", _sut.VariableName);
            Assert.Equal("testTables", _sut.VariableNamePlural);
            Assert.Equal("TEST_TABLE", _sut.ConstantName);
            Assert.Equal("test-table", _sut.RouteParameter);
        }

        [Fact]
        public void HasCompositeIndex_WhenPropertyHasIndexOrder_ShouldReturnTrue()
        {
            // Arrange
            var mockProperty = new Mock<IProperty>();
            mockProperty.Setup(p => p.IndexOrder).Returns(1);
            _mockProperties.Setup(p => p.Values).Returns(new[] { mockProperty.Object });

            // Assert
            Assert.True(_sut.HasCompositeIndex);
        }

        [Fact]
        public void HasUniqueConstraints_WhenPropertyIsUnique_ShouldReturnTrue()
        {
            // Arrange
            var mockProperty = new Mock<IProperty>();
            mockProperty.Setup(p => p.IsUnique).Returns(true);
            _mockProperties.Setup(p => p.Values).Returns(new[] { mockProperty.Object });

            // Assert
            Assert.True(_sut.HasUniqueConstraints);
        }

        [Fact]
        public void SecurityFeatures_ShouldReflectEntityConfiguration()
        {
            // Arrange
            _mockEntity.Setup(e => e.RequiresAuthorization).Returns(true);
            _mockEntity.Setup(e => e.HasRowLevelSecurity).Returns(true);

            var mockProperty = new Mock<IProperty>();
            mockProperty.Setup(p => p.IsEncrypted).Returns(true);
            _mockProperties.Setup(p => p.Values).Returns(new[] { mockProperty.Object });

            // Assert
            Assert.True(_sut.RequiresAuthorization);
            Assert.True(_sut.HasRowLevelSecurity);
            Assert.True(_sut.HasDataEncryption);
        }

        [Fact]
        public void RelationshipFeatures_ShouldIdentifyRelationshipPatterns()
        {
            // Arrange
            var mockRelationship = new Mock<IRelationship>();
            mockRelationship.Setup(r => r.FromTableName).Returns("TestTable");
            mockRelationship.Setup(r => r.ToTableName).Returns("TestTable");
            var relationships = new List<IRelationship> { mockRelationship.Object };
            _mockRelationships.Setup(r => r.GetEnumerator()).Returns(relationships.GetEnumerator());

            // Assert
            Assert.True(_sut.HasSelfReferencing);
        }

        [Fact]
        public void ValidationFeatures_ShouldIdentifyValidationRequirements()
        {
            // Arrange
            var mockProperty = new Mock<IProperty>();
            mockProperty.Setup(p => p.HasValidationRules).Returns(true);
            _mockProperties.Setup(p => p.Values).Returns(new[] { mockProperty.Object });

            // Assert
            Assert.True(_sut.RequiresValidation);
        }

        [Fact]
        public void IntegrationFeatures_ShouldIdentifyIntegrationPatterns()
        {
            // Arrange
            _mockEntity.Setup(e => e.GeneratesEvents).Returns(true);
            _mockEntity.Setup(e => e.RequiresNotification).Returns(true);
            _mockEntity.Setup(e => e.HasExternalReferences).Returns(true);
            _mockEntity.Setup(e => e.IsPartOfWorkflow).Returns(true);

            // Assert
            Assert.True(_sut.IsEventSource);
            Assert.True(_sut.RequiresNotification);
            Assert.True(_sut.HasExternalReferences);
            Assert.True(_sut.IsPartOfWorkflow);
        }

        [Fact]
        public void PerformanceFeatures_ShouldIdentifyOptimizationNeeds()
        {
            // Arrange
            _mockEntity.Setup(e => e.IsFrequentlyAccessed).Returns(true);
            _mockEntity.Setup(e => e.IsLargeDataset).Returns(true);

            var mockProperty = new Mock<IProperty>();
            mockProperty.Setup(p => p.RequiresIndex).Returns(true);
            _mockProperties.Setup(p => p.Values).Returns(new[] { mockProperty.Object });

            // Assert
            Assert.True(_sut.ShouldImplementCaching);
            Assert.True(_sut.RequiresIndexing);
            Assert.True(_sut.RequiresPagination);
        }
    }
}
