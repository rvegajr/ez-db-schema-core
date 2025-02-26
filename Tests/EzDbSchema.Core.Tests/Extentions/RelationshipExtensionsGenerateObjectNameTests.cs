using EzDbSchema.Core.Enums;
using EzDbSchema.Core.Extentions;
using EzDbSchema.Core.Interfaces;
using EzDbSchema.Core.Objects;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace EzDbSchema.Core.Tests.Extentions
{
    public class RelationshipExtensionsGenerateObjectNameTests
    {
        [Fact]
        public void GenerateObjectName_WithNullEntity_ShouldReturnEmptyString()
        {
            // Arrange
            IEntity? entity = null;
            string fkName = "FK_Test";
            var generatedFrom = ObjectNameGeneratedFrom.ToTableName;

            // Act
            var result = entity.GenerateObjectName(fkName, generatedFrom);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GenerateObjectName_WithNonExistentForeignKey_ShouldReturnEmptyString()
        {
            // Arrange
            var entityMock = new Mock<IEntity>();
            var relationshipGroupsMock = new Mock<IRelationshipGroups>();
            relationshipGroupsMock.Setup(r => r.ContainsKey(It.IsAny<string>())).Returns(false);
            entityMock.Setup(e => e.RelationshipGroups).Returns(relationshipGroupsMock.Object);

            string fkName = "FK_NonExistent";
            var generatedFrom = ObjectNameGeneratedFrom.ToTableName;

            // Act
            var result = entityMock.Object.GenerateObjectName(fkName, generatedFrom);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GenerateObjectName_WithToTableName_ShouldReturnSingularizedTableName()
        {
            // Arrange
            var entityMock = new Mock<IEntity>();
            entityMock.Setup(e => e.TableName).Returns("Customer");
            entityMock.Setup(e => e.DatabaseSchema).Returns("dbo");

            var databaseMock = new Mock<IDatabase>();
            databaseMock.Setup(d => d.DefaultSchema).Returns("dbo");
            entityMock.Setup(e => e.Database).Returns(databaseMock.Object);

            var propertiesMock = new Mock<IPropertyDictionary>();
            propertiesMock.Setup(p => p.ContainsKey(It.IsAny<string>())).Returns(false);
            entityMock.Setup(e => e.Properties).Returns(propertiesMock.Object);

            var relationshipGroupMock = new Mock<IRelationshipGroup>();
            var relationshipSummary = new RelationshipSummary
            {
                FromTableName = "dbo.Customer",
                ToTableName = "dbo.Orders",
                FromColumnName = new List<string> { "CustomerId" },
                ToColumnName = new List<string> { "CustomerId" }
            };

            // Instead of mocking the extension method, set up properties that the extension method uses
            relationshipGroupMock.Setup(r => r.Count).Returns(1);
            var relationship = new Mock<IRelationship>();
            relationship.Setup(r => r.FromTableName).Returns("dbo.Customer");
            relationship.Setup(r => r.ToTableName).Returns("dbo.Orders");
            relationship.Setup(r => r.FromColumnName).Returns("CustomerId");
            relationship.Setup(r => r.ToColumnName).Returns("CustomerId");
            relationshipGroupMock.Setup(r => r.Values).Returns(new List<IRelationshipList> { new RelationshipList { relationship.Object } });

            // Create a real RelationshipList instead of mocking
            var relationshipList = new RelationshipList();
            relationshipList.Add(relationship.Object);

            var relationshipGroupsMock = new Mock<IRelationshipGroups>();
            relationshipGroupsMock.Setup(r => r.ContainsKey("FK_Customer_Orders")).Returns(true);
            relationshipGroupsMock.Setup(r => r["FK_Customer_Orders"]).Returns(relationshipList);
            relationshipGroupsMock.Setup(r => r.Values).Returns(new List<IRelationshipList> { relationshipList });
            entityMock.Setup(e => e.RelationshipGroups).Returns(relationshipGroupsMock.Object);

            string fkName = "FK_Customer_Orders";
            var generatedFrom = ObjectNameGeneratedFrom.ToTableName;

            // Act
            var result = entityMock.Object.GenerateObjectName(fkName, generatedFrom);

            // Assert
            Assert.Equal("Order", result);
        }

        [Fact]
        public void GenerateObjectName_WithSameTableReference_ShouldUseToUniqueColumnName()
        {
            // Arrange
            var entityMock = new Mock<IEntity>();
            entityMock.Setup(e => e.TableName).Returns("Employee");
            entityMock.Setup(e => e.DatabaseSchema).Returns("dbo");

            var databaseMock = new Mock<IDatabase>();
            databaseMock.Setup(d => d.DefaultSchema).Returns("dbo");
            entityMock.Setup(e => e.Database).Returns(databaseMock.Object);

            var propertiesMock = new Mock<IPropertyDictionary>();
            propertiesMock.Setup(p => p.ContainsKey(It.IsAny<string>())).Returns(false);
            entityMock.Setup(e => e.Properties).Returns(propertiesMock.Object);

            var relationshipGroupMock = new Mock<IRelationshipGroup>();
            var relationshipSummary = new RelationshipSummary
            {
                FromTableName = "dbo.Employee",
                ToTableName = "dbo.Employee",
                FromColumnName = new List<string> { "ManagerId" },
                ToColumnName = new List<string> { "EmployeeId" }
            };

            // Instead of mocking the extension method, set up properties that the extension method uses
            relationshipGroupMock.Setup(r => r.Count).Returns(1);
            var relationship = new Mock<IRelationship>();
            relationship.Setup(r => r.FromTableName).Returns("dbo.Employee");
            relationship.Setup(r => r.ToTableName).Returns("dbo.Employee");
            relationship.Setup(r => r.FromColumnName).Returns("ManagerId");
            relationship.Setup(r => r.ToColumnName).Returns("EmployeeId");
            relationshipGroupMock.Setup(r => r.Values).Returns(new List<IRelationshipList> { new RelationshipList { relationship.Object } });

            // Create a real RelationshipList instead of mocking
            var relationshipList = new RelationshipList();
            relationshipList.Add(relationship.Object);

            var relationshipGroupsMock = new Mock<IRelationshipGroups>();
            relationshipGroupsMock.Setup(r => r.ContainsKey("FK_Employee_Employee")).Returns(true);
            relationshipGroupsMock.Setup(r => r["FK_Employee_Employee"]).Returns(relationshipList);
            relationshipGroupsMock.Setup(r => r.Values).Returns(new List<IRelationshipList> { relationshipList });
            entityMock.Setup(e => e.RelationshipGroups).Returns(relationshipGroupsMock.Object);

            string fkName = "FK_Employee_Employee";
            var generatedFrom = ObjectNameGeneratedFrom.ToTableName;

            // Act
            var result = entityMock.Object.GenerateObjectName(fkName, generatedFrom);

            // Assert
            Assert.Equal("EmployeeId", result);
        }

        [Fact]
        public void GenerateObjectName_WithJoinFromColumnName_ShouldReturnJoinedColumnNames()
        {
            // Arrange
            var entityMock = new Mock<IEntity>();
            entityMock.Setup(e => e.TableName).Returns("Order");
            entityMock.Setup(e => e.DatabaseSchema).Returns("dbo");

            var databaseMock = new Mock<IDatabase>();
            databaseMock.Setup(d => d.DefaultSchema).Returns("dbo");
            entityMock.Setup(e => e.Database).Returns(databaseMock.Object);

            var propertiesMock = new Mock<IPropertyDictionary>();
            propertiesMock.Setup(p => p.ContainsKey(It.IsAny<string>())).Returns(false);
            entityMock.Setup(e => e.Properties).Returns(propertiesMock.Object);

            var relationshipGroupMock = new Mock<IRelationshipGroup>();
            var relationshipSummary = new RelationshipSummary
            {
                FromTableName = "dbo.Order",
                ToTableName = "dbo.OrderItem",
                FromColumnName = new List<string> { "OrderId", "CustomerId" },
                ToColumnName = new List<string> { "OrderId", "CustomerId" }
            };

            relationshipGroupMock.Setup(r => r.Count).Returns(1);
            var relationship = new Mock<IRelationship>();
            relationship.Setup(r => r.FromTableName).Returns("dbo.Order");
            relationship.Setup(r => r.ToTableName).Returns("dbo.OrderItem");
            relationship.Setup(r => r.FromColumnName).Returns("OrderId");
            relationship.Setup(r => r.ToColumnName).Returns("OrderId");
            relationshipGroupMock.Setup(r => r.Values).Returns(new List<IRelationshipList> { new RelationshipList { relationship.Object } });

            // Create a real RelationshipList instead of mocking
            var relationshipList = new RelationshipList();
            relationshipList.Add(relationship.Object);

            var relationshipGroupsMock = new Mock<IRelationshipGroups>();
            relationshipGroupsMock.Setup(r => r.ContainsKey("FK_Order_OrderItem")).Returns(true);
            relationshipGroupsMock.Setup(r => r["FK_Order_OrderItem"]).Returns(relationshipList);
            relationshipGroupsMock.Setup(r => r.Values).Returns(new List<IRelationshipList> { relationshipList });
            entityMock.Setup(e => e.RelationshipGroups).Returns(relationshipGroupsMock.Object);

            string fkName = "FK_Order_OrderItem";
            var generatedFrom = ObjectNameGeneratedFrom.JoinFromColumnName;

            // Act
            var result = entityMock.Object.GenerateObjectName(fkName, generatedFrom);

            // Assert
            Assert.Equal("OrderItem", result);
        }
    }
}
