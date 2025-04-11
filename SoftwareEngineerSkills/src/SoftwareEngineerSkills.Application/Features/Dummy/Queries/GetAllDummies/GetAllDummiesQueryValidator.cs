using FluentValidation;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Queries.GetAllDummies;

/// <summary>
/// Validator for the GetAllDummiesQuery
/// </summary>
public class GetAllDummiesQueryValidator : AbstractValidator<GetAllDummiesQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllDummiesQueryValidator"/> class
    /// </summary>
    public GetAllDummiesQueryValidator()
    {
        // No specific validation rules needed for this query as the includeInactive parameter
        // is a boolean with a default value, which doesn't require validation
    }
}