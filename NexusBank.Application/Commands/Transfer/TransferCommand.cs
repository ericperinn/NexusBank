using MediatR;

namespace NexusBank.Application.Commands.Transfer;

public record TransferCommand(Guid FromAccountId, Guid ToAccountId, decimal Amount) : IRequest;
