using MediatR;

namespace NexusBank.Application.Commands.Deposit;

public record DepositCommand(Guid AccountId, decimal Amount) : IRequest;
