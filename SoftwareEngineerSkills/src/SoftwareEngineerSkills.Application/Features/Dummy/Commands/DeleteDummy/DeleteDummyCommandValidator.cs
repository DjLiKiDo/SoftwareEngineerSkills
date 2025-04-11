using FluentValidation;

namespace SoftwareEngineerSkills.Application.Features.Dummy.Commands.DeleteDummy;

/// <summary>
/// Validator for the DeleteDummyCommand
/// </summary>
public class DeleteDummyCommandValidator : AbstractValidator<DeleteDummyCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteDummyCommandValidator"/> class
    /// </summary>
    public DeleteDummyCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");
    }
}