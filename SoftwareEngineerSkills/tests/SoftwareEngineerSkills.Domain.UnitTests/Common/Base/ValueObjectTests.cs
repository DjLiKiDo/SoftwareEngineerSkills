using SoftwareEngineerSkills.Domain.Common.Base;

namespace SoftwareEngineerSkills.Domain.UnitTests.Common.Base;

public class ValueObjectTests
{
    [Fact]
    public void ValueObjects_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        var vo1 = new TestValueObject("test", 123);
        var vo2 = new TestValueObject("test", 123);

        // Act & Assert
        vo1.Should().Be(vo2);
        vo1.Equals(vo2).Should().BeTrue();
        (vo1 == vo2).Should().BeTrue();
        (vo1 != vo2).Should().BeFalse();
    }

    [Fact]
    public void ValueObjects_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var vo1 = new TestValueObject("test1", 123);
        var vo2 = new TestValueObject("test2", 123);

        // Act & Assert
        vo1.Should().NotBe(vo2);
        vo1.Equals(vo2).Should().BeFalse();
        (vo1 == vo2).Should().BeFalse();
        (vo1 != vo2).Should().BeTrue();
    }

    [Fact]
    public void ValueObjects_WithSameValues_ShouldHaveSameHashCode()
    {
        // Arrange
        var vo1 = new TestValueObject("test", 123);
        var vo2 = new TestValueObject("test", 123);

        // Act
        var hashCode1 = vo1.GetHashCode();
        var hashCode2 = vo2.GetHashCode();

        // Assert
        hashCode1.Should().Be(hashCode2);
    }

    [Fact]
    public void ValueObjects_WithDifferentValues_ShouldHaveDifferentHashCodes()
    {
        // Arrange
        var vo1 = new TestValueObject("test1", 123);
        var vo2 = new TestValueObject("test2", 123);

        // Act
        var hashCode1 = vo1.GetHashCode();
        var hashCode2 = vo2.GetHashCode();

        // Assert
        hashCode1.Should().NotBe(hashCode2);
    }

    [Fact]
    public void ValueObject_ComparedWithNull_ShouldNotBeEqual()
    {
        // Arrange
        var vo = new TestValueObject("test", 123);

        // Act & Assert
        vo.Equals(null).Should().BeFalse();
        (vo == null).Should().BeFalse();
        (null == vo).Should().BeFalse();
        (vo != null).Should().BeTrue();
        (null != vo).Should().BeTrue();
    }

    [Fact]
    public void ValueObject_ComparedWithDifferentType_ShouldNotBeEqual()
    {
        // Arrange
        var vo = new TestValueObject("test", 123);
        var differentObject = new object();

        // Act & Assert
        vo.Equals(differentObject).Should().BeFalse();
    }

    [Fact]
    public void NullValueObjects_ComparedWithEachOther_ShouldBeEqual()
    {
        // Act & Assert
        ValueObject? nullVo1 = null;
        ValueObject? nullVo2 = null;
        
        // Both are null, so they should be equal
        (nullVo1 == nullVo2).Should().BeTrue();
        (nullVo1 != nullVo2).Should().BeFalse();
    }

    #region Helper Class
    private class TestValueObject : ValueObject
    {
        public string StringValue { get; }
        public int IntValue { get; }

        public TestValueObject(string stringValue, int intValue)
        {
            StringValue = stringValue;
            IntValue = intValue;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return StringValue;
            yield return IntValue;
        }
    }
    #endregion
}
