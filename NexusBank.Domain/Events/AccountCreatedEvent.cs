namespace NexusBank.Domain.Events;

public record AccountCreatedEvent(Guid AccountId, string OwnerName) : IDomainEvent;
