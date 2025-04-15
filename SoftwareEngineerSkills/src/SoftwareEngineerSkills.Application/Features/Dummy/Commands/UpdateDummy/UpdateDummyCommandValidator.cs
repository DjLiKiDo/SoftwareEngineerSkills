using FluentValidation;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Commands.UpdateDummy;

/// <summary>
/// Validator for the UpdateDummyCommand
/// </summary>
public class UpdateDummyCommandValidator : AbstractValidator<UpdateDummyCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateDummyCommandValidator"/> class
    /// </summary>
    public UpdateDummyCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");

        RuleFor(x => x.Name)
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters");

        RuleFor(x => x.Priority)
            .InclusiveBetween(0, 5)
            .WithMessage("Priority must be between 0 and 5");
    }
}
