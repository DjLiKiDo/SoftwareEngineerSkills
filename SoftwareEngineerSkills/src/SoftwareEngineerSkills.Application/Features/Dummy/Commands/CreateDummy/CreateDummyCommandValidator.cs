using FluentValidation;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Commands.CreateDummy;

/// <summary>
/// Validator for the CreateDummyCommand
/// </summary>
public class CreateDummyCommandValidator : AbstractValidator<CreateDummyCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateDummyCommandValidator"/> class.
    /// </summary>
    public CreateDummyCommandValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
            
        RuleFor(x => x.Priority)
            .InclusiveBetween(0, 5).WithMessage("Priority must be between 0 and 5");
    }
}
