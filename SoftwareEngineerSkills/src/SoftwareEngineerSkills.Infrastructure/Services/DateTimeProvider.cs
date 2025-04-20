using SoftwareEngineerSkills.Domain.Abstractions.Services;
using System;

namespace SoftwareEngineerSkills.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    /// <summary>
    /// Gets the current date and time.
    /// </summary>
    public DateTimeOffset Now => DateTime.Now;

    /// <summary>
    /// Gets the current date and time in UTC.
    /// </summary>
    public DateTime UtcNow => DateTime.UtcNow;
}
