using System;
using System.Collections.Generic;
using System.Linq;
using EzDbSchema.Core.CodeGen;
using EzDbSchema.Core.Interfaces;
using Moq;
using Xunit;

namespace EzDbSchema.Core.Tests.CodeGen
{
    public class DatabaseCodeGenInfoTests
    {
        private readonly Mock<IDatabase> _mockDatabase;
        private readonly Mock<IEntity> _mockEntity1;
        private readonly Mock<IEntity> _mockEntity2;
        private readonly DatabaseCodeGenInfo _sut;

        public DatabaseCodeGenInfoTests()
        {
            _mockDatabase = new Mock<IDatabase>();
            _mockEntity1 = CreateMockEntity("Customer", "sales");
            _mockEntity2 = CreateMockEntity("Order", "sales");

            var entities = new Dictionary<string, IEntity>
            {
                { "Customer", _mockEntity1.Object },
                { "Order", _mockEntity2.Object }
            };

            _mockDatabase.Setup(d => d.Values).Returns(entities.Values);
            _mockDatabase.Setup(d => d[It.IsAny<string>()]).Returns((string name) => entities.ContainsKey(name) ? entities[name] : null!);
            _sut = new DatabaseCodeGenInfo(_mockDatabase.Object);
        }

        private Mock<IEntity> CreateMockEntity(string tableName, string schema)
        {
            var mockEntity = new Mock<IEntity>();
            var mockProperties = new Mock<IPropertyDictionary>();
            var mockRelationships = new Mock<IRelationshipReferenceList>();

            mockEntity.Setup(e => e.TableName).Returns(tableName);
            mockEntity.Setup(e => e.DatabaseSchema).Returns(schema);
            mockEntity.Setup(e => e.Properties).Returns(mockProperties.Object);
            mockEntity.Setup(e => e.Relationships).Returns(mockRelationships.Object);
            mockEntity.Setup(e => e.EntityType).Returns("TABLE");
            mockEntity.Setup(e => e.ParentDatabase).Returns(_mockDatabase.Object);

            // Setup empty lists as defaults
            mockProperties.Setup(p => p.Values).Returns(new List<IProperty>());
            mockRelationships.Setup(r => r.GetEnumerator()).Returns(new List<IRelationship>().GetEnumerator());

            return mockEntity;
        }

        [Fact]
        public void Entities_ShouldReturnAllEntities()
        {
            // Assert
            Assert.Equal(2, _sut.Entities.Count());
            Assert.Contains(_sut.Entities, e => e.TableName == "Customer");
            Assert.Contains(_sut.Entities, e => e.TableName == "Order");
        }

        [Fact]
        public void Tables_ShouldReturnOnlyTables()
        {
            // Arrange
            _mockEntity1.Setup(e => e.EntityType).Returns("TABLE");
            _mockEntity2.Setup(e => e.EntityType).Returns("VIEW");

            // Assert
            Assert.Single(_sut.Tables);
            Assert.Equal("Customer", _sut.Tables.First().TableName);
        }

        [Fact]
        public void Views_ShouldReturnOnlyViews()
        {
            // Arrange
            _mockEntity1.Setup(e => e.EntityType).Returns("TABLE");
            _mockEntity2.Setup(e => e.EntityType).Returns("VIEW");

            // Assert
            Assert.Single(_sut.Views);
            Assert.Equal("Order", _sut.Views.First().TableName);
        }

        [Fact]
        public void Schemas_ShouldReturnUniqueSchemas()
        {
            // Assert
            Assert.Single(_sut.Schemas);
            Assert.Contains("sales", _sut.Schemas);
        }

        [Fact]
        public void EntitiesBySchema_ShouldGroupCorrectly()
        {
            // Assert
            Assert.True(_sut.EntitiesBySchema.ContainsKey("sales"));
            Assert.Equal(2, _sut.EntitiesBySchema["sales"].Count());
        }

        [Fact]
        public void DependencyGraph_WithNoEntities_ReturnsEmptyDictionary()
        {
            // Arrange
            var mockDatabase = new Mock<IDatabase>();
            mockDatabase.Setup(d => d.Values).Returns(new List<IEntity>());
            var sut = new DatabaseCodeGenInfo(mockDatabase.Object);

            // Act
            var graph = sut.DependencyGraph;

            // Assert
            Assert.Empty(graph);
        }

        [Fact]
        public void DependencyGraph_WithSingleEntityNoRelationships_ReturnsEmptyDictionary()
        {
            // Arrange
            var mockDatabase = new Mock<IDatabase>();
            var mockEntity = new Mock<IEntity>();
            mockEntity.Setup(e => e.TableName).Returns("Customer");
            
            var mockRelationships = new Mock<IRelationshipReferenceList>();
            mockRelationships.Setup(r => r.GetEnumerator()).Returns(new List<IRelationship>().GetEnumerator());
            mockEntity.Setup(e => e.Relationships).Returns(mockRelationships.Object);

            mockDatabase.Setup(d => d.Values).Returns(new[] { mockEntity.Object });
            var sut = new DatabaseCodeGenInfo(mockDatabase.Object);

            // Act
            var graph = sut.DependencyGraph;

            // Assert
            Assert.Empty(graph);
        }

        [Fact]
        public void DependencyGraph_WithOneToOneRelationship_ReturnsSingleDependency()
        {
            // Arrange
            var mockDatabase = new Mock<IDatabase>();
            
            // Create Order -> Customer relationship
            var relationship = new Mock<IRelationship>();
            relationship.Setup(r => r.FromTableName).Returns("Order");
            relationship.Setup(r => r.ToTableName).Returns("Customer");

            var relationships = new List<IRelationship> { relationship.Object };
            var mockRelationships = new Mock<IRelationshipReferenceList>();
            mockRelationships.Setup(r => r.GetEnumerator()).Returns(relationships.GetEnumerator());

            var orderEntity = new Mock<IEntity>();
            orderEntity.Setup(e => e.TableName).Returns("Order");
            orderEntity.Setup(e => e.Relationships).Returns(mockRelationships.Object);

            mockDatabase.Setup(d => d.Values).Returns(new[] { orderEntity.Object });
            var sut = new DatabaseCodeGenInfo(mockDatabase.Object);

            // Act
            var graph = sut.DependencyGraph;

            // Assert
            Assert.Single(graph);
            Assert.True(graph.ContainsKey("Order"));
            Assert.Single(graph["Order"]);
            Assert.Contains("Customer", graph["Order"]);
        }

        [Fact]
        public void DependencyGraph_WithMultipleRelationships_ReturnsAllDependencies()
        {
            // Arrange
            var mockDatabase = new Mock<IDatabase>();
            
            // Create Order -> Customer and Order -> Product relationships
            var relationship1 = new Mock<IRelationship>();
            relationship1.Setup(r => r.FromTableName).Returns("Order");
            relationship1.Setup(r => r.ToTableName).Returns("Customer");

            var relationship2 = new Mock<IRelationship>();
            relationship2.Setup(r => r.FromTableName).Returns("Order");
            relationship2.Setup(r => r.ToTableName).Returns("Product");

            var relationships = new List<IRelationship> { relationship1.Object, relationship2.Object };
            var mockRelationships = new Mock<IRelationshipReferenceList>();
            mockRelationships.Setup(r => r.GetEnumerator()).Returns(relationships.GetEnumerator());

            var orderEntity = new Mock<IEntity>();
            orderEntity.Setup(e => e.TableName).Returns("Order");
            orderEntity.Setup(e => e.Relationships).Returns(mockRelationships.Object);

            mockDatabase.Setup(d => d.Values).Returns(new[] { orderEntity.Object });
            var sut = new DatabaseCodeGenInfo(mockDatabase.Object);

            // Act
            var graph = sut.DependencyGraph;

            // Assert
            Assert.Single(graph);
            Assert.True(graph.ContainsKey("Order"));
            Assert.Equal(2, graph["Order"].Count());
            Assert.Contains("Customer", graph["Order"]);
            Assert.Contains("Product", graph["Order"]);
        }

        [Fact]
        public void RequiredFeatures_WithNoEntities_ReturnsEmptyList()
        {
            // Arrange
            var mockDatabase = new Mock<IDatabase>();
            mockDatabase.Setup(d => d.Values).Returns(new List<IEntity>());
            var sut = new DatabaseCodeGenInfo(mockDatabase.Object);

            // Act
            var features = sut.RequiredFrameworkFeatures;

            // Assert
            Assert.Empty(features);
        }

        [Fact]
        public void RequiredFeatures_WithAuditableEntity_ReturnsAuditing()
        {
            // Arrange
            var mockDatabase = new Mock<IDatabase>();
            
            var mockProperty = new Mock<IProperty>();
            mockProperty.Setup(p => p.ColumnName).Returns("CreatedDate");

            var mockProperties = new Mock<IPropertyDictionary>();
            mockProperties.Setup(p => p.Values).Returns(new[] { mockProperty.Object });

            var mockEntity = new Mock<IEntity>();
            mockEntity.Setup(e => e.TableName).Returns("Customer");
            mockEntity.Setup(e => e.Properties).Returns(mockProperties.Object);

            var mockRelationships = new Mock<IRelationshipReferenceList>();
            mockRelationships.Setup(r => r.GetEnumerator()).Returns(new List<IRelationship>().GetEnumerator());
            mockEntity.Setup(e => e.Relationships).Returns(mockRelationships.Object);

            mockDatabase.Setup(d => d.Values).Returns(new[] { mockEntity.Object });
            var sut = new DatabaseCodeGenInfo(mockDatabase.Object);

            // Act
            var features = sut.RequiredFrameworkFeatures;

            // Assert
            Assert.Contains("Auditing", features);
        }

        [Fact]
        public void RequiredFeatures_WithMultipleAuditableEntities_ReturnsAuditingOnce()
        {
            // Arrange
            var mockDatabase = new Mock<IDatabase>();
            
            var mockProperty1 = new Mock<IProperty>();
            mockProperty1.Setup(p => p.ColumnName).Returns("CreatedDate");
            var mockProperties1 = new Mock<IPropertyDictionary>();
            mockProperties1.Setup(p => p.Values).Returns(new[] { mockProperty1.Object });

            var mockProperty2 = new Mock<IProperty>();
            mockProperty2.Setup(p => p.ColumnName).Returns("CreatedDate");
            var mockProperties2 = new Mock<IPropertyDictionary>();
            mockProperties2.Setup(p => p.Values).Returns(new[] { mockProperty2.Object });

            var mockEntity1 = new Mock<IEntity>();
            mockEntity1.Setup(e => e.TableName).Returns("Customer");
            mockEntity1.Setup(e => e.Properties).Returns(mockProperties1.Object);
            mockEntity1.Setup(e => e.Relationships).Returns(new Mock<IRelationshipReferenceList>().Object);

            var mockEntity2 = new Mock<IEntity>();
            mockEntity2.Setup(e => e.TableName).Returns("Order");
            mockEntity2.Setup(e => e.Properties).Returns(mockProperties2.Object);
            mockEntity2.Setup(e => e.Relationships).Returns(new Mock<IRelationshipReferenceList>().Object);

            mockDatabase.Setup(d => d.Values).Returns(new[] { mockEntity1.Object, mockEntity2.Object });
            var sut = new DatabaseCodeGenInfo(mockDatabase.Object);

            // Act
            var features = sut.RequiredFrameworkFeatures;

            // Assert
            Assert.Single(features);
            Assert.Contains("Auditing", features);
        }

        [Fact]
        public void SuggestedPackages_ShouldRecommendAppropriatePackages()
        {
            // Arrange
            var mockProperties = new Mock<IPropertyDictionary>();
            var properties = new Dictionary<string, IProperty>();
            var mockProperty = new Mock<IProperty>();
            mockProperty.Setup(p => p.ColumnName).Returns("CreatedAt");
            properties.Add("CreatedAt", mockProperty.Object);
            mockProperties.Setup(p => p.Values).Returns(properties.Values);
            _mockEntity1.Setup(e => e.Properties).Returns(mockProperties.Object);

            // Assert
            var packages = _sut.SuggestedNuGetPackages.ToList();
            Assert.Contains("Microsoft.EntityFrameworkCore", packages);
            Assert.Contains("Microsoft.EntityFrameworkCore.Auditing", packages);
        }

        [Fact]
        public void ProjectStructure_ShouldProvideStandardLayout()
        {
            // Act
            var structure = _sut.SuggestedProjectStructure.ToList();

            // Assert
            Assert.Contains("src/", structure);
            Assert.Contains("├── domain/", structure);
            Assert.Contains("├── infrastructure/", structure);
            Assert.Contains("├── application/", structure);
        }

        [Fact]
        public void FileNameMapping_ShouldProvideConsistentNaming()
        {
            // Act
            var mapping = _sut.FileNameMapping;

            // Assert
            Assert.True(mapping.ContainsKey("Customer.cs"));
            Assert.Equal("domain/entities/Customer.cs", mapping["Customer.cs"]);
            Assert.True(mapping.ContainsKey("ICustomer.cs"));
            Assert.Equal("domain/interfaces/ICustomer.cs", mapping["ICustomer.cs"]);
        }

        [Fact]
        public void FrameworkConfig_ShouldProvideLanguageSpecificSettings()
        {
            // Act
            var config = _sut.FrameworkSpecificConfig;

            // Assert
            Assert.True(config.ContainsKey("TargetFramework"));
            Assert.Equal("net7.0", config["TargetFramework"]);
            Assert.True(config.ContainsKey("Nullable"));
            Assert.True((bool)config["Nullable"]);
        }

        [Fact]
        public void ApiInfo_ShouldProvideEndpointConfiguration()
        {
            // Assert
            Assert.Equal("/api/v1", _sut.BaseApiPath);
            Assert.Contains(_sut.ApiTags, tag => tag == "Customers");
            Assert.Contains(_sut.ApiTags, tag => tag == "Orders");
        }

        [Fact]
        public void PatternDetection_ShouldIdentifyCommonPatterns()
        {
            // Arrange
            var mockProperties1 = new Mock<IPropertyDictionary>();
            var properties1 = new Dictionary<string, IProperty>();
            var mockProperty1 = new Mock<IProperty>();
            mockProperty1.Setup(p => p.ColumnName).Returns("CreatedAt");
            properties1.Add("CreatedAt", mockProperty1.Object);
            mockProperties1.Setup(p => p.Values).Returns(properties1.Values);
            _mockEntity1.Setup(e => e.Properties).Returns(mockProperties1.Object);

            var mockProperties2 = new Mock<IPropertyDictionary>();
            var properties2 = new Dictionary<string, IProperty>();
            var mockProperty2 = new Mock<IProperty>();
            mockProperty2.Setup(p => p.ColumnName).Returns("Version");
            properties2.Add("Version", mockProperty2.Object);
            mockProperties2.Setup(p => p.Values).Returns(properties2.Values);
            _mockEntity2.Setup(e => e.Properties).Returns(mockProperties2.Object);

            // Assert
            Assert.True(_sut.HasAuditableEntities);
            Assert.True(_sut.HasVersionedEntities);
        }
    }
}
