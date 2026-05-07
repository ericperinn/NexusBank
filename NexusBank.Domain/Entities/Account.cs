using NexusBank.Domain.Exceptions;

namespace NexusBank.Domain.Entities;

public class Account
{
    public Guid Id { get; private set; }
    public string OwnerName { get; private set; } = default!;
    public decimal Balance { get; private set; }

    private Account() { }

    public Account(string ownerName)
    {
        if (string.IsNullOrWhiteSpace(ownerName))
            throw new DomainException("Owner name is required.");

        Id = Guid.NewGuid();
        OwnerName = ownerName;
        Balance = 0m; 
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Deposit amount must be greater than zero.");

        Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Withdrawal amount must be greater than zero.");

        if (amount > Balance)
            throw new InsufficientFundsException(Balance, amount);

        Balance -= amount;
    }
}
