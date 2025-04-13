using MediatR;

namespace SoftwareEngineerSkills.Application.Common.Commands;

/// <summary>
/// Marker interface to identify commands that modify the system state.
/// All commands should implement this interface instead of IRequest directly.
/// </summary>
/// <typeparam name="TResponse">The command response type</typeparam>
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}