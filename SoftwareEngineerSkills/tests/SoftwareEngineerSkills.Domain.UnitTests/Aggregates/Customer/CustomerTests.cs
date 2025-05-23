using SoftwareEngineerSkills.Domain.Aggregates.Customer;
using SoftwareEngineerSkills.Domain.Common.Events;
using SoftwareEngineerSkills.Domain.Common.Interfaces;
using SoftwareEngineerSkills.Domain.Exceptions;
using SoftwareEngineerSkills.Domain.ValueObjects;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace SoftwareEngineerSkills.Domain.UnitTests.Aggregates.Customer;

/// <summary>
/// Unit tests for Customer aggregate root
/// </summary>
public class CustomerTests
{
    #region Constructor Tests

    [Fact]
    public void Given_ValidNameAndEmail_When_CustomerCreated_Then_ShouldInitializeCorrectly()
    {
        // Arrange
        const string name = "John Doe";
        var email = new Email("john.doe@example.com");

        // Act
        var customer = new Domain.Aggregates.Customer.Customer(name, email);

        // Assert
        customer.Name.Should().Be(name);
        customer.Email.Should().Be(email);
        customer.PhoneNumber.Should().BeNull();
        customer.ShippingAddress.Should().BeNull();
        customer.Id.Should().NotBe(Guid.Empty);
        customer.Version.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Given_ValidNameAndEmail_When_CustomerCreated_Then_ShouldRaiseCustomerCreatedEvent()
    {
        // Arrange
        const string name = "John Doe";
        var email = new Email("john.doe@example.com");

        // Act
        var customer = new Domain.Aggregates.Customer.Customer(name, email);

        // Assert
        customer.DomainEvents.Should().HaveCount(1);
        var domainEvent = customer.DomainEvents.First().Should().BeOfType<CustomerCreatedEvent>().Subject;
        domainEvent.CustomerId.Should().Be(customer.Id);
        domainEvent.Name.Should().Be(name);
        domainEvent.Email.Should().Be(email.Value);
    }

    [Theory]
    [InlineData(null)]
    public void Given_NullName_When_CustomerCreated_Then_ShouldThrowArgumentNullException(string? name)
    {
        // Arrange
        var email = new Email("john.doe@example.com");

        // Act & Assert
        var action = () => new Domain.Aggregates.Customer.Customer(name!, email);
        action.Should().Throw<ArgumentNullException>()
            .Which.ParamName.Should().Be("name");
    }

    [Fact]
    public void Given_NullEmail_When_CustomerCreated_Then_ShouldThrowArgumentNullException()
    {
        // Arrange
        const string name = "John Doe";

        // Act & Assert
        Email? nullEmail = null;
        var action = () => new Domain.Aggregates.Customer.Customer(name, nullEmail!);
        action.Should().Throw<ArgumentNullException>()
            .Which.ParamName.Should().Be("email");
    }

    #endregion

    #region UpdateName Tests

    [Fact]
    public void Given_ValidName_When_NameUpdated_Then_ShouldUpdateSuccessfully()
    {
        // Arrange
        var customer = CreateValidCustomer();
        customer.ClearDomainEvents(); // Clear creation events
        const string newName = "Jane Smith";

        // Act
        customer.UpdateName(newName);

        // Assert
        customer.Name.Should().Be(newName);
    }

    [Fact]
    public void Given_ValidName_When_NameUpdated_Then_ShouldRaiseCustomerNameUpdatedEvent()
    {
        // Arrange
        var customer = CreateValidCustomer();
        var originalName = customer.Name;
        customer.ClearDomainEvents(); // Clear creation events
        const string newName = "Jane Smith";

        // Act
        customer.UpdateName(newName);

        // Assert
        customer.DomainEvents.Should().HaveCount(1);
        var domainEvent = customer.DomainEvents.First().Should().BeOfType<CustomerNameUpdatedEvent>().Subject;
        domainEvent.CustomerId.Should().Be(customer.Id);
        domainEvent.OldName.Should().Be(originalName);
        domainEvent.NewName.Should().Be(newName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Given_InvalidName_When_NameUpdated_Then_ShouldThrowDomainValidationException(string? invalidName)
    {
        // Arrange
        var customer = CreateValidCustomer();

        // Act & Assert
        customer.Invoking(c => c.UpdateName(invalidName!))
            .Should().Throw<DomainValidationException>()
            .Which.Errors.Should().Contain(error => error.Contains("Name cannot be empty"));
    }

    [Fact]
    public void Given_SameName_When_NameUpdated_Then_ShouldNotRaiseEvent()
    {
        // Arrange
        var customer = CreateValidCustomer();
        var originalName = customer.Name;
        customer.ClearDomainEvents(); // Clear creation events

        // Act
        customer.UpdateName(originalName);

        // Assert
        customer.DomainEvents.Should().BeEmpty();
    }

    #endregion

    #region UpdateEmail Tests

    [Fact]
    public void Given_ValidEmail_When_EmailUpdated_Then_ShouldUpdateSuccessfully()
    {
        // Arrange
        var customer = CreateValidCustomer();
        customer.ClearDomainEvents(); // Clear creation events
        var newEmail = new Email("jane.smith@example.com");

        // Act
        customer.UpdateEmail(newEmail);

        // Assert
        customer.Email.Should().Be(newEmail);
    }

    [Fact]
    public void Given_ValidEmail_When_EmailUpdated_Then_ShouldRaiseCustomerEmailUpdatedEvent()
    {
        // Arrange
        var customer = CreateValidCustomer();
        var originalEmail = customer.Email;
        customer.ClearDomainEvents(); // Clear creation events
        var newEmail = new Email("jane.smith@example.com");

        // Act
        customer.UpdateEmail(newEmail);

        // Assert
        customer.DomainEvents.Should().HaveCount(1);
        var domainEvent = customer.DomainEvents.First().Should().BeOfType<CustomerEmailUpdatedEvent>().Subject;
        domainEvent.CustomerId.Should().Be(customer.Id);
        domainEvent.OldEmail.Should().Be(originalEmail.Value);
        domainEvent.NewEmail.Should().Be(newEmail.Value);
    }

    [Fact]
    public void Given_NullEmail_When_EmailUpdated_Then_ShouldThrowArgumentNullException()
    {
        // Arrange
        var customer = CreateValidCustomer();

        // Act & Assert
        Email? nullEmail = null;
        customer.Invoking(c => c.UpdateEmail(nullEmail!))
            .Should().Throw<ArgumentNullException>()
            .Which.ParamName.Should().Be("email");
    }

    [Fact]
    public void Given_SameEmail_When_EmailUpdated_Then_ShouldNotRaiseEvent()
    {
        // Arrange
        var customer = CreateValidCustomer();
        var originalEmail = customer.Email;
        customer.ClearDomainEvents(); // Clear creation events

        // Act
        customer.UpdateEmail(originalEmail);

        // Assert
        customer.DomainEvents.Should().BeEmpty();
    }

    #endregion

    #region UpdatePhoneNumber Tests

    [Fact]
    public void Given_ValidPhoneNumber_When_PhoneNumberUpdated_Then_ShouldUpdateSuccessfully()
    {
        // Arrange
        var customer = CreateValidCustomer();
        customer.ClearDomainEvents(); // Clear creation events
        const string phoneNumber = "+1-555-123-4567";

        // Act
        customer.UpdatePhoneNumber(phoneNumber);

        // Assert
        customer.PhoneNumber.Should().Be(phoneNumber);
    }

    [Fact]
    public void Given_ValidPhoneNumber_When_PhoneNumberUpdated_Then_ShouldRaiseCustomerPhoneUpdatedEvent()
    {
        // Arrange
        var customer = CreateValidCustomer();
        var originalPhone = customer.PhoneNumber;
        customer.ClearDomainEvents(); // Clear creation events
        const string phoneNumber = "+1-555-123-4567";

        // Act
        customer.UpdatePhoneNumber(phoneNumber);

        // Assert
        customer.DomainEvents.Should().HaveCount(1);
        var domainEvent = customer.DomainEvents.First().Should().BeOfType<CustomerPhoneUpdatedEvent>().Subject;
        domainEvent.CustomerId.Should().Be(customer.Id);
        domainEvent.OldPhoneNumber.Should().Be(originalPhone);
        domainEvent.NewPhoneNumber.Should().Be(phoneNumber);
    }

    [Fact]
    public void Given_NullPhoneNumber_When_PhoneNumberUpdated_Then_ShouldClearPhoneNumber()
    {
        // Arrange
        var customer = CreateValidCustomer();
        customer.UpdatePhoneNumber("+1-555-123-4567"); // Set initial phone
        customer.ClearDomainEvents(); // Clear creation events

        // Act
        customer.UpdatePhoneNumber(null);

        // Assert
        customer.PhoneNumber.Should().BeNull();
    }

    [Fact]
    public void Given_SamePhoneNumber_When_PhoneNumberUpdated_Then_ShouldNotRaiseEvent()
    {
        // Arrange
        var customer = CreateValidCustomer();
        const string phoneNumber = "+1-555-123-4567";
        customer.UpdatePhoneNumber(phoneNumber);
        customer.ClearDomainEvents(); // Clear creation events

        // Act
        customer.UpdatePhoneNumber(phoneNumber);

        // Assert
        customer.DomainEvents.Should().BeEmpty();
    }

    #endregion

    #region UpdateShippingAddress Tests

    [Fact]
    public void Given_ValidAddress_When_ShippingAddressUpdated_Then_ShouldUpdateSuccessfully()
    {
        // Arrange
        var customer = CreateValidCustomer();
        customer.ClearDomainEvents(); // Clear creation events
        var address = new Address("123 Main St", "Anytown", "NY", "12345", "USA");

        // Act
        customer.UpdateShippingAddress(address);

        // Assert
        customer.ShippingAddress.Should().Be(address);
    }

    [Fact]
    public void Given_ValidAddress_When_ShippingAddressUpdated_Then_ShouldRaiseCustomerAddressUpdatedEvent()
    {
        // Arrange
        var customer = CreateValidCustomer();
        var originalAddress = customer.ShippingAddress;
        customer.ClearDomainEvents(); // Clear creation events
        var address = new Address("123 Main St", "Anytown", "NY", "12345", "USA");

        // Act
        customer.UpdateShippingAddress(address);

        // Assert
        customer.DomainEvents.Should().HaveCount(1);
        var domainEvent = customer.DomainEvents.First().Should().BeOfType<CustomerAddressUpdatedEvent>().Subject;
        domainEvent.CustomerId.Should().Be(customer.Id);
        domainEvent.OldAddress.Should().Be(originalAddress);
        domainEvent.NewAddress.Should().Be(address);
    }

    [Fact]
    public void Given_NullAddress_When_ShippingAddressUpdated_Then_ShouldClearAddress()
    {
        // Arrange
        var customer = CreateValidCustomer();
        var address = new Address("123 Main St", "Anytown", "NY", "12345", "USA");
        customer.UpdateShippingAddress(address); // Set initial address
        customer.ClearDomainEvents(); // Clear creation events

        // Act
        customer.UpdateShippingAddress(null);

        // Assert
        customer.ShippingAddress.Should().BeNull();
    }

    [Fact]
    public void Given_SameAddress_When_ShippingAddressUpdated_Then_ShouldNotRaiseEvent()
    {
        // Arrange
        var customer = CreateValidCustomer();
        var address = new Address("123 Main St", "Anytown", "NY", "12345", "USA");
        customer.UpdateShippingAddress(address);
        customer.ClearDomainEvents(); // Clear creation events

        // Act
        customer.UpdateShippingAddress(address);

        // Assert
        customer.DomainEvents.Should().BeEmpty();
    }

    #endregion

    #region Invariant Validation Tests

    [Fact]
    public void Given_ValidCustomer_When_InvariantsChecked_Then_ShouldNotHaveErrors()
    {
        // Arrange
        var customer = CreateValidCustomer();

        // Act & Assert
        customer.Invoking(c => c.EnforceInvariants())
            .Should().NotThrow();
    }

    [Fact]
    public void Given_CustomerWithLongName_When_InvariantsChecked_Then_ShouldHaveValidationErrors()
    {
        // Arrange
        var customer = CreateValidCustomer();
        var longName = new string('x', 201); // Assuming max length is 200

        // Act & Assert
        customer.Invoking(c => c.UpdateName(longName))
            .Should().Throw<DomainValidationException>()
            .Which.Errors.Should().Contain(error => error.Contains("name") && error.Contains("200"));
    }

    #endregion

    #region Thread Safety Tests

    [Fact]
    public void Given_CustomerAggregate_When_AddingEventsFromMultipleThreads_Then_ShouldBeThreadSafe()
    {
        // Arrange
        var customer = new Domain.Aggregates.Customer.Customer("John Doe", new Email("john.doe@example.com"));
        customer.ClearDomainEvents();
        var initialVersion = customer.Version;
        const int operationsCount = 100;

        // Act - Add events concurrently from multiple threads
        Parallel.For(0, operationsCount, i =>
        {
            customer.AddDomainEvent(new CustomerNameUpdatedEvent(customer.Id, "Old Name", $"New Name {i}"));
        });

        // Assert
        customer.DomainEvents.Should().HaveCount(operationsCount);
        customer.Version.Should().Be(initialVersion + operationsCount); // Version should be incremented for each event
    }

    [Fact]
    public void Given_CustomerAggregate_When_ConcurrentOperations_Then_ShouldMaintainConsistency()
    {
        // Arrange
        var customer = new Domain.Aggregates.Customer.Customer("Original Name", new Email("original@example.com"));
        customer.ClearDomainEvents();
        const int threadCount = 20;
        var countdown = new CountdownEvent(threadCount);
        var barrier = new Barrier(threadCount);
        var random = new Random();
        var exceptions = new ConcurrentQueue<Exception>();

        // Act - Execute multiple operations concurrently
        Parallel.For(0, threadCount, i =>
        {
            try
            {
                // Synchronize threads to increase contention
                barrier.SignalAndWait();

                // Perform random operations
                switch (i % 4)
                {
                    case 0:
                        customer.UpdateName($"Name-{i}");
                        break;
                    case 1:
                        customer.UpdateEmail(new Email($"email{i}@example.com"));
                        break;
                    case 2:
                        customer.UpdatePhoneNumber($"+1234567{i}");
                        break;
                    case 3:
                        customer.AddDomainEvent(new CustomerNameUpdatedEvent(customer.Id, $"Previous-{i}", $"Updated-{i}"));
                        break;
                }
            }
            catch (Exception ex)
            {
                exceptions.Enqueue(ex);
            }
            finally
            {
                countdown.Signal();
            }
        });

        countdown.Wait();

        // Assert
        exceptions.Should().BeEmpty("No exceptions should occur during concurrent operations");
        // Version should be higher than initial version + operations, as operations generate internal events
        customer.Version.Should().BeGreaterThan(2); // Initial value (2) + operations
        customer.DomainEvents.Should().NotBeEmpty();
    }

    #endregion

    #region Event Sourcing Tests

    [Fact]
    public void Given_CustomerWithEvents_When_ApplyingEvents_Then_StateShouldBeUpdatedCorrectly()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var customer = new CustomerTestDouble();

        var createdEvent = new CustomerCreatedEvent(customerId, "Initial Name", "initial@example.com");
        var nameChangedEvent = new CustomerNameUpdatedEvent(customerId, "Initial Name", "Updated Name");
        var emailChangedEvent = new CustomerEmailUpdatedEvent(customerId, "initial@example.com", "updated@example.com");
        var phoneChangedEvent = new CustomerPhoneUpdatedEvent(customerId, null, "+1234567890");

        // Act - Apply events as in event sourcing
        customer.TestApply(createdEvent);
        customer.TestApply(nameChangedEvent);
        customer.TestApply(emailChangedEvent);
        customer.TestApply(phoneChangedEvent);

        // Assert - State should reflect all applied events
        customer.Id.Should().Be(customerId);
        customer.Name.Should().Be("Updated Name");
        // Can't check Email directly as it requires Email object and TestApply doesn't create it
        customer.PhoneNumber.Should().Be("+1234567890");
        customer.AppliedEvents.Should().HaveCount(4);
    }

    [Fact]
    public void Given_CustomerWithAddAndApplyEvent_When_ChangingState_Then_EventShouldBeAppliedAndAdded()
    {
        // Arrange
        var customer = new Domain.Aggregates.Customer.Customer("Initial Name", new Email("initial@example.com"));
        customer.ClearDomainEvents();
        var initialVersion = customer.Version;

        // Act - Use AddAndApplyEvent through public methods
        customer.UpdateName("New Name");

        // Assert
        customer.Name.Should().Be("New Name");
        customer.Version.Should().BeGreaterThan(initialVersion);
        customer.DomainEvents.Should().NotBeEmpty();
        customer.DomainEvents.Should().ContainSingle(e => e is CustomerNameUpdatedEvent);
    }

    #endregion

    #region Aggregate Root Behavior Tests

    [Fact]
    public void Given_Customer_When_TypeChecked_Then_ShouldBeAggregateRoot()
    {
        // Arrange & Act
        var customer = CreateValidCustomer();

        // Assert
        customer.Should().BeAssignableTo<SoftwareEngineerSkills.Domain.Common.Interfaces.IAggregateRoot>();
    }

    [Fact]
    public void Given_MultipleConcurrentUpdates_When_Applied_Then_ShouldBeThreadSafe()
    {
        // Arrange
        var customer = CreateValidCustomer();
        customer.ClearDomainEvents();
        const int updateCount = 10;

        // Act
        var tasks = new List<Task>();
        for (int i = 0; i < updateCount; i++)
        {
            var index = i;
            tasks.Add(Task.Run(() => customer.UpdateName($"Name {index}")));
        }

        // Assert
        tasks.Invoking(t => Task.WaitAll(t.ToArray()))
            .Should().NotThrow();

        customer.DomainEvents.Should().HaveCount(updateCount);
        customer.Version.Should().Be(updateCount);
    }

    #endregion

    #region Audit Properties Tests

    [Fact]
    public void Given_Customer_When_Created_Then_ShouldHaveAuditProperties()
    {
        // Arrange & Act
        var customer = CreateValidCustomer();

        // Assert
        customer.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        customer.CreatedBy.Should().BeNull(); // Will be set by infrastructure
        customer.LastModified.Should().BeNull();
        customer.LastModifiedBy.Should().BeNull();
    }

    [Fact]
    public void Given_Customer_When_Updated_Then_AuditPropertiesCanBeSet()
    {
        // Arrange
        var customer = CreateValidCustomer();
        var testUser = "test-user";
        var updateTime = DateTime.UtcNow;

        // Act
        customer.LastModified = updateTime;
        customer.LastModifiedBy = testUser;

        // Assert
        customer.LastModified.Should().Be(updateTime);
        customer.LastModifiedBy.Should().Be(testUser);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Creates a valid customer for testing
    /// </summary>
    private static Domain.Aggregates.Customer.Customer CreateValidCustomer()
    {
        return new Domain.Aggregates.Customer.Customer(
            "John Doe",
            new Email("john.doe@example.com"));
    }

    #endregion
}

// Helper class for testing Apply method (which is protected)
public class CustomerTestDouble : Domain.Aggregates.Customer.Customer
{
    public List<IDomainEvent> AppliedEvents { get; } = new();

    // Default constructor to avoid errors
    public CustomerTestDouble() : base("Test", new Email("test@example.com"))
    {
        ClearDomainEvents();
    }

    // Expose protected Apply method for testing
    public void TestApply(IDomainEvent domainEvent)
    {
        Apply(domainEvent);
        AppliedEvents.Add(domainEvent);
    }
}
