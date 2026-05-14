namespace NexusBank.Domain.ValueObjects;

public record AccountNumber
{
    public string Value { get; }

    private AccountNumber() { Value = string.Empty; } // for EF Core

    private AccountNumber(string value)
    {
        Value = value;
    }

    public static AccountNumber Generate()
    {
        var number = Random.Shared.Next(100000000, 999999999).ToString();
        return new AccountNumber($"NXB-{number}");
    }

    public static AccountNumber From(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.StartsWith("NXB-"))
            throw new ArgumentException("Invalid account number format.", nameof(value));
        return new AccountNumber(value);
    }

    public override string ToString() => Value;
}
