using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SoftwareEngineerSkills.Infrastructure.Configuration;
using System.ComponentModel.DataAnnotations;

namespace SoftwareEngineerSkills.Infrastructure.UnitTests.Configuration;

public class SettingsExtensionsTests
{
    [Fact]
    public void AddSettings_BasicValidation_ShouldRegisterOptionsCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "TestSettings:StringProperty", "Test Value" },
                { "TestSettings:IntProperty", "42" }
            })
            .Build();
        
        // Act
        var optionsBuilder = services.AddSettings<TestSettings>(configuration, "TestSettings");
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<TestSettings>>();
        
        // Assert
        optionsBuilder.Should().NotBeNull();
        options.Should().NotBeNull();
        options!.Value.Should().NotBeNull();
        options.Value.StringProperty.Should().Be("Test Value");
        options.Value.IntProperty.Should().Be(42);
    }
    
    [Fact]
    public void AddSettings_WithDataAnnotationsValidation_ShouldThrowWhenInvalid()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                // Missing required StringProperty
                { "TestSettings:IntProperty", "42" }
            })
            .Build();
        
        // Act
        services.AddSettings<TestSettings>(configuration, "TestSettings");
        var serviceProvider = services.BuildServiceProvider();
        
        // Assert
        // Validation happens when accessing the options, so we expect an exception
        Action action = () => { var _ = serviceProvider.GetService<IOptionsMonitor<TestSettings>>()!.CurrentValue; };
        
        action.Should().Throw<OptionsValidationException>()
            .Which.Message.Should().Contain("DataAnnotation validation failed");
    }
    
    [Fact]
    public void AddSettings_WithCustomValidation_ShouldValidateCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "TestSettings:StringProperty", "Valid" },
                { "TestSettings:IntProperty", "42" }
            })
            .Build();
        
        // Act
        services.AddSettings<TestSettings>(
            configuration, 
            "TestSettings", 
            settings => settings.IntProperty > 0,  // Custom validation
            "IntProperty must be positive");
            
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetService<IOptions<TestSettings>>();
        
        // Assert
        options.Should().NotBeNull();
        options!.Value.IntProperty.Should().Be(42);
    }
    
    [Fact]
    public void AddSettings_WithCustomValidationFailure_ShouldThrow()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "TestSettings:StringProperty", "Valid" },
                { "TestSettings:IntProperty", "-1" } // Negative value will fail custom validation
            })
            .Build();
        
        // Act
        services.AddSettings<TestSettings>(
            configuration, 
            "TestSettings", 
            settings => settings.IntProperty > 0,  // Custom validation
            "IntProperty must be positive");
            
        var serviceProvider = services.BuildServiceProvider();
        
        // Assert
        Action action = () => { var _ = serviceProvider.GetService<IOptionsMonitor<TestSettings>>()!.CurrentValue; };
        action.Should().Throw<OptionsValidationException>()
            .Which.Message.Should().Contain("IntProperty must be positive");
    }

    // Test class for validation
    public class TestSettings
    {
        [Required]
        public string StringProperty { get; set; } = string.Empty;
        
        public int IntProperty { get; set; }
    }
}
