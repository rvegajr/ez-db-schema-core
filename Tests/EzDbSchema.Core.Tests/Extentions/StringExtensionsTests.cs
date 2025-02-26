using EzDbSchema.Core.Extentions;
using System;
using Xunit;

namespace EzDbSchema.Core.Tests.Extentions
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("children", "child")]
        [InlineData("people", "person")]
        [InlineData("oxen", "ox")]
        [InlineData("men", "man")]
        [InlineData("women", "woman")]
        [InlineData("teeth", "tooth")]
        [InlineData("feet", "foot")]
        [InlineData("mice", "mouse")]
        [InlineData("criteria", "criterion")]
        [InlineData("categories", "category")]
        [InlineData("cities", "city")]
        [InlineData("queries", "query")]
        [InlineData("wolves", "wolf")]
        [InlineData("lives", "life")]
        [InlineData("dishes", "dish")]
        [InlineData("churches", "church")]
        [InlineData("tables", "table")]
        [InlineData("columns", "column")]
        [InlineData("cars", "car")]
        [InlineData("car", "car")]  // Already singular
        [InlineData("", "")]  // Empty string
        public void ToSingular_ShouldConvertPluralToSingular(string plural, string expectedSingular)
        {
            // Act
            var result = plural?.ToSingular();

            // Assert
            Assert.Equal(expectedSingular, result);
        }

        [Fact]
        public void ToSingular_WithNull_ShouldReturnNull()
        {
            // Arrange
            string? plural = null;

            // Act
            var result = plural?.ToSingular();

            // Assert
            Assert.Null(result);
        }
    }
}
