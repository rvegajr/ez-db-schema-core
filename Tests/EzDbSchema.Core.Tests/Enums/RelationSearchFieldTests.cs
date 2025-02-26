using EzDbSchema.Core.Enums;
using Xunit;

namespace EzDbSchema.Core.Tests.Enums
{
    public class RelationSearchFieldTests
    {
        [Fact]
        public void RelationSearchField_HasExpectedValues()
        {
            // Verify all expected enum values exist
            Assert.Equal(0, (int)RelationSearchField.ToTableName);
            Assert.Equal(1, (int)RelationSearchField.ToColumnName);
            Assert.Equal(2, (int)RelationSearchField.ToFieldName);
            Assert.Equal(3, (int)RelationSearchField.FromTableName);
            Assert.Equal(4, (int)RelationSearchField.FromFieldName);
            Assert.Equal(5, (int)RelationSearchField.FromColumnName);
            
            // Verify we can cast between int and enum
            RelationSearchField field = (RelationSearchField)3;
            Assert.Equal(RelationSearchField.FromTableName, field);
            
            // Verify enum can be used in a switch statement
            string result = "";
            var enumValue = RelationSearchField.ToTableName;
            switch (enumValue)
            {
                case RelationSearchField.FromTableName:
                    result = "FromTableName";
                    break;
                case RelationSearchField.ToTableName:
                    result = "ToTableName";
                    break;
                default:
                    result = "Other";
                    break;
            }
            
            Assert.Equal("ToTableName", result);
        }
    }
}
