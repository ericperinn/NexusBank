using MediatR;

namespace NexusBank.Application.Commands.CreateAccount;

public record CreateAccountCommand(string OwnerName) : IRequest<Guid>;
