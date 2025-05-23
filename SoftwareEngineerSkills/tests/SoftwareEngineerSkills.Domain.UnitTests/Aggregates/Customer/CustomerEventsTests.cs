using SoftwareEngineerSkills.Domain.Aggregates.Customer;
using SoftwareEngineerSkills.Domain.Common.Events;
using SoftwareEngineerSkills.Domain.ValueObjects;

namespace SoftwareEngineerSkills.Domain.UnitTests.Aggregates.Customer;

/// <summary>
/// Unit tests for Customer domain events
/// </summary>
public class CustomerEventsTests
{
    #region CustomerCreatedEvent Tests

    [Fact]
    public void Given_ValidParameters_When_CustomerCreatedEventCreated_Then_ShouldInitializeCorrectly()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        const string name = "John Doe";
        const string email = "john.doe@example.com";

        // Act
        var domainEvent = new CustomerCreatedEvent(customerId, name, email);

        // Assert
        domainEvent.CustomerId.Should().Be(customerId);
        domainEvent.Name.Should().Be(name);
        domainEvent.Email.Should().Be(email);
        domainEvent.OccurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        domainEvent.EventId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Given_CustomerCreatedEvent_When_TypeChecked_Then_ShouldInheritFromDomainEvent()
    {
        // Arrange
        var domainEvent = new CustomerCreatedEvent(Guid.NewGuid(), "John Doe", "john@example.com");

        // Act & Assert
        domainEvent.Should().BeAssignableTo<DomainEvent>();
        domainEvent.Should().BeAssignableTo<IDomainEvent>();
    }

    [Fact]
    public void Given_TwoCustomerCreatedEventsWithSameData_When_Compared_Then_ShouldNotBeEqual()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        const string name = "John Doe";
        const string email = "john@example.com";

        // Act
        var event1 = new CustomerCreatedEvent(customerId, name, email);
        var event2 = new CustomerCreatedEvent(customerId, name, email);

        // Assert
        event1.Should().NotBe(event2); // Each event should have unique EventId
        event1.EventId.Should().NotBe(event2.EventId);
        event1.OccurredOn.Should().BeCloseTo(event2.OccurredOn, TimeSpan.FromMilliseconds(100));
    }

    #endregion

    #region CustomerNameUpdatedEvent Tests

    [Fact]
    public void Given_ValidParameters_When_CustomerNameUpdatedEventCreated_Then_ShouldInitializeCorrectly()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        const string oldName = "John Doe";
        const string newName = "Jane Smith";

        // Act
        var domainEvent = new CustomerNameUpdatedEvent(customerId, oldName, newName);

        // Assert
        domainEvent.CustomerId.Should().Be(customerId);
        domainEvent.OldName.Should().Be(oldName);
        domainEvent.NewName.Should().Be(newName);
        domainEvent.OccurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        domainEvent.EventId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Given_CustomerNameUpdatedEvent_When_TypeChecked_Then_ShouldInheritFromDomainEvent()
    {
        // Arrange
        var domainEvent = new CustomerNameUpdatedEvent(Guid.NewGuid(), "Old", "New");

        // Act & Assert
        domainEvent.Should().BeAssignableTo<DomainEvent>();
        domainEvent.Should().BeAssignableTo<IDomainEvent>();
    }

    #endregion

    #region CustomerEmailUpdatedEvent Tests

    [Fact]
    public void Given_ValidParameters_When_CustomerEmailUpdatedEventCreated_Then_ShouldInitializeCorrectly()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        const string oldEmail = "old@example.com";
        const string newEmail = "new@example.com";

        // Act
        var domainEvent = new CustomerEmailUpdatedEvent(customerId, oldEmail, newEmail);

        // Assert
        domainEvent.CustomerId.Should().Be(customerId);
        domainEvent.OldEmail.Should().Be(oldEmail);
        domainEvent.NewEmail.Should().Be(newEmail);
        domainEvent.OccurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        domainEvent.EventId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Given_CustomerEmailUpdatedEvent_When_TypeChecked_Then_ShouldInheritFromDomainEvent()
    {
        // Arrange
        var domainEvent = new CustomerEmailUpdatedEvent(Guid.NewGuid(), "old@example.com", "new@example.com");

        // Act & Assert
        domainEvent.Should().BeAssignableTo<DomainEvent>();
        domainEvent.Should().BeAssignableTo<IDomainEvent>();
    }

    #endregion

    #region CustomerPhoneUpdatedEvent Tests

    [Fact]
    public void Given_ValidParameters_When_CustomerPhoneUpdatedEventCreated_Then_ShouldInitializeCorrectly()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        const string oldPhone = "+1-555-123-4567";
        const string newPhone = "+1-555-987-6543";

        // Act
        var domainEvent = new CustomerPhoneUpdatedEvent(customerId, oldPhone, newPhone);

        // Assert
        domainEvent.CustomerId.Should().Be(customerId);
        domainEvent.OldPhoneNumber.Should().Be(oldPhone);
        domainEvent.NewPhoneNumber.Should().Be(newPhone);
        domainEvent.OccurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        domainEvent.EventId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Given_NullPhoneNumbers_When_CustomerPhoneUpdatedEventCreated_Then_ShouldHandleNulls()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Act
        var domainEvent = new CustomerPhoneUpdatedEvent(customerId, null, null);

        // Assert
        domainEvent.CustomerId.Should().Be(customerId);
        domainEvent.OldPhoneNumber.Should().BeNull();
        domainEvent.NewPhoneNumber.Should().BeNull();
    }

    [Fact]
    public void Given_CustomerPhoneUpdatedEvent_When_TypeChecked_Then_ShouldInheritFromDomainEvent()
    {
        // Arrange
        var domainEvent = new CustomerPhoneUpdatedEvent(Guid.NewGuid(), "old", "new");

        // Act & Assert
        domainEvent.Should().BeAssignableTo<DomainEvent>();
        domainEvent.Should().BeAssignableTo<IDomainEvent>();
    }

    #endregion

    #region CustomerAddressUpdatedEvent Tests

    [Fact]
    public void Given_ValidParameters_When_CustomerAddressUpdatedEventCreated_Then_ShouldInitializeCorrectly()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var oldAddress = new Address("123 Old St", "Old City", "OS", "12345", "USA");
        var newAddress = new Address("456 New Ave", "New City", "NS", "67890", "USA");

        // Act
        var domainEvent = new CustomerAddressUpdatedEvent(customerId, oldAddress, newAddress);

        // Assert
        domainEvent.CustomerId.Should().Be(customerId);
        domainEvent.OldAddress.Should().Be(oldAddress);
        domainEvent.NewAddress.Should().Be(newAddress);
        domainEvent.OccurredOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        domainEvent.EventId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Given_NullAddresses_When_CustomerAddressUpdatedEventCreated_Then_ShouldHandleNulls()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Act
        var domainEvent = new CustomerAddressUpdatedEvent(customerId, null, null);

        // Assert
        domainEvent.CustomerId.Should().Be(customerId);
        domainEvent.OldAddress.Should().BeNull();
        domainEvent.NewAddress.Should().BeNull();
    }

    [Fact]
    public void Given_CustomerAddressUpdatedEvent_When_TypeChecked_Then_ShouldInheritFromDomainEvent()
    {
        // Arrange
        var domainEvent = new CustomerAddressUpdatedEvent(Guid.NewGuid(), null, null);

        // Act & Assert
        domainEvent.Should().BeAssignableTo<DomainEvent>();
        domainEvent.Should().BeAssignableTo<IDomainEvent>();
    }

    #endregion

    #region Event Timestamp Tests

    [Fact]
    public void Given_MultipleEvents_When_CreatedInSequence_Then_TimestampsShouldBeInOrder()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Act
        var event1 = new CustomerCreatedEvent(customerId, "John", "john@example.com");
        Thread.Sleep(1); // Ensure different timestamps
        var event2 = new CustomerNameUpdatedEvent(customerId, "John", "Jane");
        Thread.Sleep(1);
        var event3 = new CustomerEmailUpdatedEvent(customerId, "john@example.com", "jane@example.com");

        // Assert
        event1.OccurredOn.Should().BeBefore(event2.OccurredOn);
        event2.OccurredOn.Should().BeBefore(event3.OccurredOn);
    }

    #endregion

    #region Event Identity Tests

    [Fact]
    public void Given_CustomerEvents_When_Created_Then_ShouldHaveUniqueEventIds()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Act
        var events = new IDomainEvent[]
        {
            new CustomerCreatedEvent(customerId, "John", "john@example.com"),
            new CustomerNameUpdatedEvent(customerId, "John", "Jane"),
            new CustomerEmailUpdatedEvent(customerId, "john@example.com", "jane@example.com"),
            new CustomerPhoneUpdatedEvent(customerId, null, "+1-555-123-4567"),
            new CustomerAddressUpdatedEvent(customerId, null, new Address("123 Main St", "City", "State", "12345", "Country"))
        };

        // Assert
        var eventIds = events.Select(e => e.EventId).ToList();
        eventIds.Should().OnlyHaveUniqueItems();
        eventIds.Should().AllSatisfy(id => id.Should().NotBe(Guid.Empty));
    }

    #endregion

    #region Integration with Customer Aggregate Tests

    [Fact]
    public void Given_CustomerAggregate_When_ActionsPerformed_Then_ShouldGenerateCorrectEvents()
    {
        // Arrange
        var customer = new Domain.Aggregates.Customer.Customer("John Doe", new Email("john@example.com"));
        customer.ClearDomainEvents(); // Clear creation event

        var newEmail = new Email("jane@example.com");
        const string newName = "Jane Smith";
        const string newPhone = "+1-555-123-4567";
        var newAddress = new Address("123 Main St", "City", "State", "12345", "Country");

        // Act
        customer.UpdateName(newName);
        customer.UpdateEmail(newEmail);
        customer.UpdatePhoneNumber(newPhone);
        customer.UpdateShippingAddress(newAddress);

        // Assert
        customer.DomainEvents.Should().HaveCount(4);
        
        customer.DomainEvents.OfType<CustomerNameUpdatedEvent>().Should().HaveCount(1);
        customer.DomainEvents.OfType<CustomerEmailUpdatedEvent>().Should().HaveCount(1);
        customer.DomainEvents.OfType<CustomerPhoneUpdatedEvent>().Should().HaveCount(1);
        customer.DomainEvents.OfType<CustomerAddressUpdatedEvent>().Should().HaveCount(1);

        // Verify event content
        var nameEvent = customer.DomainEvents.OfType<CustomerNameUpdatedEvent>().Single();
        nameEvent.NewName.Should().Be(newName);

        var emailEvent = customer.DomainEvents.OfType<CustomerEmailUpdatedEvent>().Single();
        emailEvent.NewEmail.Should().Be(newEmail.Value);

        var phoneEvent = customer.DomainEvents.OfType<CustomerPhoneUpdatedEvent>().Single();
        phoneEvent.NewPhoneNumber.Should().Be(newPhone);

        var addressEvent = customer.DomainEvents.OfType<CustomerAddressUpdatedEvent>().Single();
        addressEvent.NewAddress.Should().Be(newAddress);
    }

    #endregion
}
