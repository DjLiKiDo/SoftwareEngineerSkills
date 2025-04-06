using System.ComponentModel.DataAnnotations;

namespace SoftwareEngineerSkills.Domain.Common.Configuration;

/// <summary>
/// Represents dummy settings for demonstration purposes
/// </summary>
public class DummySettings
{
    /// <summary>
    /// A string dummy setting
    /// </summary>
    [Required(ErrorMessage = "DummySetting1 is required")]
    [StringLength(50, ErrorMessage = "DummySetting1 cannot exceed 50 characters")]
    public string DummySetting1 { get; set; } = string.Empty;
    
    /// <summary>
    /// A numeric dummy setting
    /// </summary>
    [Range(1, 100, ErrorMessage = "DummySetting2 must be between 1 and 100")]
    public int DummySetting2 { get; set; }
    
    /// <summary>
    /// A boolean dummy setting
    /// </summary>
    [Required(ErrorMessage = "DummySetting3 is required")]
    public bool DummySetting3 { get; set; }
}
