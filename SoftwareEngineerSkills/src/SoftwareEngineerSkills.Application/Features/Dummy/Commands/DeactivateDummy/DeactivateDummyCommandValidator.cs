using FluentValidation;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Commands.DeactivateDummy;

/// <summary>
/// Validator for the DeactivateDummyCommand
/// </summary>
public class DeactivateDummyCommandValidator : AbstractValidator<DeactivateDummyCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeactivateDummyCommandValidator"/> class
    /// </summary>
    public DeactivateDummyCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("ID is required to deactivate a dummy entity");
    }
}
