using EzDbSchema.Core.Enums;
using Xunit;

namespace EzDbSchema.Core.Tests.Enums
{
    public class ObjectNameGeneratedFromTests
    {
        [Fact]
        public void ObjectNameGeneratedFrom_HasExpectedValues()
        {
            // Verify all expected enum values exist
            Assert.Equal(0, (int)ObjectNameGeneratedFrom.JoinFromColumnName);
            Assert.Equal(1, (int)ObjectNameGeneratedFrom.ToUniqueColumnName);
            Assert.Equal(2, (int)ObjectNameGeneratedFrom.ToTableName);
            Assert.Equal(3, (int)ObjectNameGeneratedFrom.FromTableName);
            Assert.Equal(4, (int)ObjectNameGeneratedFrom.JoinToColumnName);
            
            // Verify we can cast between int and enum
            ObjectNameGeneratedFrom source = (ObjectNameGeneratedFrom)1;
            Assert.Equal(ObjectNameGeneratedFrom.ToUniqueColumnName, source);
            
            // Verify enum can be used in a switch statement
            string result = "";
            var enumValue = ObjectNameGeneratedFrom.JoinToColumnName;
            switch (enumValue)
            {
                case ObjectNameGeneratedFrom.JoinFromColumnName:
                    result = "JoinFromColumnName";
                    break;
                case ObjectNameGeneratedFrom.JoinToColumnName:
                    result = "JoinToColumnName";
                    break;
                default:
                    result = "Other";
                    break;
            }
            
            Assert.Equal("JoinToColumnName", result);
        }
    }
}
