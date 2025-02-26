using EzDbSchema.Core.Enums;
using Xunit;

namespace EzDbSchema.Core.Tests.Enums
{
    public class RelationshipMultiplicityTypeTests
    {
        [Fact]
        public void RelationshipMultiplicityType_HasExpectedValues()
        {
            // Verify all expected enum values exist
            Assert.Equal(0, (int)RelationshipMultiplicityType.Unknown);
            Assert.Equal(1, (int)RelationshipMultiplicityType.OneToOne);
            Assert.Equal(2, (int)RelationshipMultiplicityType.OneToMany);
            Assert.Equal(3, (int)RelationshipMultiplicityType.ZeroOrOneToMany);
            Assert.Equal(4, (int)RelationshipMultiplicityType.ManyToOne);
            Assert.Equal(5, (int)RelationshipMultiplicityType.ManyToZeroOrOne);
            Assert.Equal(6, (int)RelationshipMultiplicityType.ZeroOrOneToOne);
            Assert.Equal(7, (int)RelationshipMultiplicityType.OneToZeroOrOne);
            
            // Verify we can cast between int and enum
            RelationshipMultiplicityType type = (RelationshipMultiplicityType)4;
            Assert.Equal(RelationshipMultiplicityType.ManyToOne, type);
            
            // Verify enum can be used in a switch statement
            string result = "";
            var enumValue = RelationshipMultiplicityType.OneToMany;
            switch (enumValue)
            {
                case RelationshipMultiplicityType.OneToOne:
                    result = "OneToOne";
                    break;
                case RelationshipMultiplicityType.OneToMany:
                    result = "OneToMany";
                    break;
                default:
                    result = "Other";
                    break;
            }
            
            Assert.Equal("OneToMany", result);
        }
    }
}
