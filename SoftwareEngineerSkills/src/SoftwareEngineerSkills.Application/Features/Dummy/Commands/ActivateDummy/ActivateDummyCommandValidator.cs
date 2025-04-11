using FluentValidation;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Commands.ActivateDummy;

/// <summary>
/// Validator for the ActivateDummyCommand
/// </summary>
public class ActivateDummyCommandValidator : AbstractValidator<ActivateDummyCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ActivateDummyCommandValidator"/> class
    /// </summary>
    public ActivateDummyCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");
    }
}