using System;
using System.Linq;
using EzDbSchema.Core.CodeGen;
using EzDbSchema.Core.Interfaces;
using Moq;
using Xunit;

namespace EzDbSchema.Core.Tests.CodeGen
{
    public class PropertyCodeGenInfoTests
    {
        private readonly Mock<IProperty> _mockProperty;
        private readonly PropertyCodeGenInfo _sut;

        public PropertyCodeGenInfoTests()
        {
            _mockProperty = new Mock<IProperty>();
            _mockProperty.Setup(p => p.ColumnName).Returns("TestColumn");
            _mockProperty.Setup(p => p.DataType).Returns("varchar");
            _sut = new PropertyCodeGenInfo(_mockProperty.Object);
        }

        [Fact]
        public void BasicProperties_ShouldReturnCorrectValues()
        {
            // Assert
            Assert.Equal("TestColumn", _sut.ColumnName);
            Assert.Equal("varchar", _sut.DataType);
            Assert.Equal("TestColumn", _sut.PropertyName);
            Assert.Equal("testColumn", _sut.VariableName);
            Assert.Equal("TEST_COLUMN", _sut.ConstantName);
            Assert.Equal("testColumn", _sut.ParameterName);
        }

        [Fact]
        public void DatabaseFeatures_ShouldReflectPropertyConfiguration()
        {
            // Arrange
            _mockProperty.Setup(p => p.IsComputed).Returns(true);
            _mockProperty.Setup(p => p.ComputedExpression).Returns("LEN(Name)");
            _mockProperty.Setup(p => p.IsUnique).Returns(true);
            _mockProperty.Setup(p => p.HasDefaultValue).Returns(true);
            _mockProperty.Setup(p => p.DefaultValue).Returns("GETDATE()");
            _mockProperty.Setup(p => p.IsConcurrencyToken).Returns(true);
            _mockProperty.Setup(p => p.IndexOrder).Returns(1);

            // Assert
            Assert.True(_sut.IsComputed);
            Assert.Equal("LEN(Name)", _sut.ComputedExpression);
            Assert.True(_sut.IsUnique);
            Assert.True(_sut.HasDefaultValue);
            Assert.Equal("GETDATE()", _sut.DefaultValue);
            Assert.True(_sut.IsConcurrencyToken);
            Assert.Equal(1, _sut.IndexOrder);
        }

        [Fact]
        public void ValidationFeatures_ShouldIdentifyValidationRules()
        {
            // Arrange
            _mockProperty.Setup(p => p.HasValidationRules).Returns(true);
            _mockProperty.Setup(p => p.ValidationRules).Returns("Required,MaxLength(50)");
            _mockProperty.Setup(p => p.RegexPattern).Returns(@"^\d{3}-\d{2}-\d{4}$");
            _mockProperty.Setup(p => p.MinValue).Returns("0");
            _mockProperty.Setup(p => p.MaxValue).Returns("100");

            // Assert
            Assert.True(_sut.HasValidationRules);
            Assert.Equal(2, _sut.ValidationRules.Count());
            Assert.Equal(@"^\d{3}-\d{2}-\d{4}$", _sut.RegexPattern);
            Assert.Equal("0", _sut.MinValue);
            Assert.Equal("100", _sut.MaxValue);
        }

        [Fact]
        public void SecurityFeatures_ShouldReflectSecurityConfiguration()
        {
            // Arrange
            _mockProperty.Setup(p => p.IsEncrypted).Returns(true);
            _mockProperty.Setup(p => p.EncryptionType).Returns("AES");
            _mockProperty.Setup(p => p.IsSensitive).Returns(true);
            _mockProperty.Setup(p => p.RequiresMasking).Returns(true);

            // Assert
            Assert.True(_sut.IsEncrypted);
            Assert.Equal("AES", _sut.EncryptionType);
            Assert.True(_sut.IsSensitive);
            Assert.True(_sut.RequiresMasking);
        }

        [Fact]
        public void SearchFeatures_ShouldIdentifySearchCapabilities()
        {
            // Arrange
            _mockProperty.Setup(p => p.RequiresIndex).Returns(true);
            _mockProperty.Setup(p => p.SupportsFullText).Returns(true);
            _mockProperty.Setup(p => p.SearchAnalyzer).Returns("standard");

            // Assert
            Assert.True(_sut.RequiresIndex);
            Assert.True(_sut.SupportsFullText);
            Assert.Equal("standard", _sut.SearchAnalyzer);
        }

        [Fact]
        public void UiFeatures_ShouldProvideUiHints()
        {
            // Arrange
            _mockProperty.Setup(p => p.DisplayFormat).Returns("dd/MM/yyyy");
            _mockProperty.Setup(p => p.InputMask).Returns("999-99-9999");
            _mockProperty.Setup(p => p.Placeholder).Returns("Enter SSN");
            _mockProperty.Setup(p => p.IsReadOnly).Returns(true);
            _mockProperty.Setup(p => p.IsHidden).Returns(true);

            // Assert
            Assert.Equal("dd/MM/yyyy", _sut.DisplayFormat);
            Assert.Equal("999-99-9999", _sut.InputMask);
            Assert.Equal("Enter SSN", _sut.Placeholder);
            Assert.True(_sut.IsReadOnly);
            Assert.True(_sut.IsHidden);
        }

        [Fact]
        public void IntegrationFeatures_ShouldIdentifyIntegrationCapabilities()
        {
            // Arrange
            _mockProperty.Setup(p => p.IsFileReference).Returns(true);
            _mockProperty.Setup(p => p.FileType).Returns("pdf");
            _mockProperty.Setup(p => p.IsExternalReference).Returns(true);
            _mockProperty.Setup(p => p.ExternalSystem).Returns("SAP");

            // Assert
            Assert.True(_sut.IsFileReference);
            Assert.Equal("pdf", _sut.FileType);
            Assert.True(_sut.IsExternalReference);
            Assert.Equal("SAP", _sut.ExternalSystem);
        }

        [Fact]
        public void PerformanceFeatures_ShouldIdentifyOptimizationNeeds()
        {
            // Arrange
            _mockProperty.Setup(p => p.IsFrequentlyAccessed).Returns(true);
            _mockProperty.Setup(p => p.RequiresCaching).Returns(true);
            _mockProperty.Setup(p => p.CacheStrategy).Returns("Sliding15");

            // Assert
            Assert.True(_sut.IsFrequentlyAccessed);
            Assert.True(_sut.RequiresCaching);
            Assert.Equal("Sliding15", _sut.CacheStrategy);
        }
    }
}
