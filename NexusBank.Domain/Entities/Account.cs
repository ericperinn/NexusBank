using NexusBank.Domain.Events;
using NexusBank.Domain.Exceptions;
using NexusBank.Domain.ValueObjects;

namespace NexusBank.Domain.Entities;

public class Account
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public Guid Id { get; private set; }
    public string OwnerName { get; private set; } = string.Empty;
    public AccountNumber AccountNumber { get; private set; } = null!;
    public Money Balance { get; private set; } = Money.Zero;

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private Account() { } // for EF Core

    public Account(string ownerName)
    {
        if (string.IsNullOrWhiteSpace(ownerName))
            throw new DomainException("Owner name is required.");

        Id = Guid.NewGuid();
        OwnerName = ownerName;
        AccountNumber = AccountNumber.Generate();
        Balance = Money.Zero;

        _domainEvents.Add(new AccountCreatedEvent(Id, OwnerName));
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Deposit amount must be greater than zero.");

        Balance = Balance.Add(new Money(amount));
        _domainEvents.Add(new MoneyDepositedEvent(Id, amount));
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Withdrawal amount must be greater than zero.");

        if (amount > Balance.Amount)
            throw new InsufficientFundsException(Balance.Amount, amount);

        Balance = Balance.Subtract(new Money(amount));
    }

    public void ClearDomainEvents() => _domainEvents.Clear();
}
