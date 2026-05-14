namespace NexusBank.Domain.Events;

public record MoneyTransferredEvent(Guid FromAccountId, Guid ToAccountId, decimal Amount) : IDomainEvent;
