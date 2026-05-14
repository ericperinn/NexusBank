namespace NexusBank.Domain.Events;

public record MoneyDepositedEvent(Guid AccountId, decimal Amount) : IDomainEvent;
