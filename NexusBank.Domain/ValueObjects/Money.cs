namespace NexusBank.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; }

    private Money() { } // for EF Core

    public Money(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Money amount cannot be negative.", nameof(amount));
        Amount = amount;
    }

    public static Money Zero => new(0);

    public Money Add(Money other) => new(Amount + other.Amount);

    public Money Subtract(Money other)
    {
        if (other.Amount > Amount)
            throw new InvalidOperationException("Insufficient funds.");
        return new(Amount - other.Amount);
    }

    public bool IsGreaterThan(Money other) => Amount > other.Amount;

    public static implicit operator decimal(Money money) => money.Amount;
    public static explicit operator Money(decimal amount) => new(amount);

    public override string ToString() => Amount.ToString("C");
}
