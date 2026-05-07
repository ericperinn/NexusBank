namespace NexusBank.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

public class AccountNotFoundException : DomainException
{
    public AccountNotFoundException(Guid id) : base($"Account with ID {id} was not found.") { }
}

public class InsufficientFundsException : DomainException
{
    public InsufficientFundsException(decimal currentBalance, decimal requestedAmount) 
        : base($"Insufficient funds. Current balance: {currentBalance}, Requested: {requestedAmount}") { }
}
