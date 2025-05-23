using SoftwareEngineerSkills.Domain.Exceptions;
using SoftwareEngineerSkills.Domain.ValueObjects;

namespace SoftwareEngineerSkills.Domain.UnitTests.ValueObjects
{
    public class AddressTests
    {
        [Fact]
        public void Constructor_ValidParameters_ShouldCreateAddress()
        {
            // Arrange
            var street1 = "123 Main St";
            var street2 = "Apt 4B";
            var city = "Springfield";
            var state = "IL";
            var postalCode = "62704";
            var country = "USA";

            // Act
            var address = new Address(street1, city, state, postalCode, country, street2);

            // Assert
            address.Street1.Should().Be(street1);
            address.Street2.Should().Be(street2);
            address.City.Should().Be(city);
            address.State.Should().Be(state);
            address.PostalCode.Should().Be(postalCode);
            address.Country.Should().Be(country);
        }

        [Fact]
        public void Constructor_ValidParametersWithoutStreet2_ShouldCreateAddress()
        {
            // Arrange
            var street1 = "123 Main St";
            var city = "Springfield";
            var state = "IL";
            var postalCode = "62704";
            var country = "USA";

            // Act
            var address = new Address(street1, city, state, postalCode, country);

            // Assert
            address.Street1.Should().Be(street1);
            address.Street2.Should().BeNull();
            address.City.Should().Be(city);
            address.State.Should().Be(state);
            address.PostalCode.Should().Be(postalCode);
            address.Country.Should().Be(country);
        }

        [Theory]
        [InlineData(null, "City", "State", "12345", "Country", "Street cannot be empty")]
        [InlineData("", "City", "State", "12345", "Country", "Street cannot be empty")]
        [InlineData("  ", "City", "State", "12345", "Country", "Street cannot be empty")]
        [InlineData("Street", null, "State", "12345", "Country", "City cannot be empty")]
        [InlineData("Street", "", "State", "12345", "Country", "City cannot be empty")]
        [InlineData("Street", "  ", "State", "12345", "Country", "City cannot be empty")]
        [InlineData("Street", "City", null, "12345", "Country", "State cannot be empty")]
        [InlineData("Street", "City", "", "12345", "Country", "State cannot be empty")]
        [InlineData("Street", "City", "  ", "12345", "Country", "State cannot be empty")]
        [InlineData("Street", "City", "State", null, "Country", "Postal code cannot be empty")]
        [InlineData("Street", "City", "State", "", "Country", "Postal code cannot be empty")]
        [InlineData("Street", "City", "State", "  ", "Country", "Postal code cannot be empty")]
        [InlineData("Street", "City", "State", "12345", null, "Country cannot be empty")]
        [InlineData("Street", "City", "State", "12345", "", "Country cannot be empty")]
        [InlineData("Street", "City", "State", "12345", "  ", "Country cannot be empty")]
        public void Constructor_InvalidParameters_ShouldThrowBusinessRuleException(
            string street1, string city, string state, string postalCode, string country, string expectedMessage)
        {
            // Arrange & Act
            Action act = () => new Address(street1, city, state, postalCode, country);

            // Assert
            act.Should().Throw<BusinessRuleException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public void GetEqualityComponents_SameValues_ShouldBeEqual()
        {
            // Arrange
            var address1 = new Address("123 Main St", "Springfield", "IL", "62704", "USA", "Apt 4B");
            var address2 = new Address("123 Main St", "Springfield", "IL", "62704", "USA", "Apt 4B");

            // Act & Assert
            address1.Should().Be(address2);
            address1.GetHashCode().Should().Be(address2.GetHashCode());
        }

        [Fact]
        public void GetEqualityComponents_DifferentValues_ShouldNotBeEqual()
        {
            // Arrange
            var address1 = new Address("123 Main St", "Springfield", "IL", "62704", "USA");
            var address2 = new Address("456 Oak Ave", "Springfield", "IL", "62704", "USA");

            // Act & Assert
            address1.Should().NotBe(address2);
            address1.GetHashCode().Should().NotBe(address2.GetHashCode());
        }

        [Fact]
        public void GetEqualityComponents_DifferentStreet2_ShouldNotBeEqual()
        {
            // Arrange
            var address1 = new Address("123 Main St", "Springfield", "IL", "62704", "USA", "Apt 4B");
            var address2 = new Address("123 Main St", "Springfield", "IL", "62704", "USA", "Suite 101");

            // Act & Assert
            address1.Should().NotBe(address2);
        }

        [Fact]
        public void GetEqualityComponents_OneWithoutStreet2_ShouldNotBeEqual()
        {
            // Arrange
            var address1 = new Address("123 Main St", "Springfield", "IL", "62704", "USA", "Apt 4B");
            var address2 = new Address("123 Main St", "Springfield", "IL", "62704", "USA");

            // Act & Assert
            address1.Should().NotBe(address2);
        }

        [Fact]
        public void ToString_WithStreet2_ShouldFormatCorrectly()
        {
            // Arrange
            var address = new Address("123 Main St", "Springfield", "IL", "62704", "USA", "Apt 4B");
            var expected = "123 Main St, Apt 4B, Springfield, IL 62704, USA";

            // Act
            var result = address.ToString();

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void ToString_WithoutStreet2_ShouldFormatCorrectly()
        {
            // Arrange
            var address = new Address("123 Main St", "Springfield", "IL", "62704", "USA");
            var expected = "123 Main St, Springfield, IL 62704, USA";

            // Act
            var result = address.ToString();

            // Assert
            result.Should().Be(expected);
        }
    }
}
