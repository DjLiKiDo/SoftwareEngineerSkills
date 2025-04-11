using FluentValidation;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Queries.GetDummyById;

/// <summary>
/// Validator for the GetDummyByIdQuery
/// </summary>
public class GetDummyByIdQueryValidator : AbstractValidator<GetDummyByIdQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetDummyByIdQueryValidator"/> class
    /// </summary>
    public GetDummyByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Dummy Id is required");
    }
}