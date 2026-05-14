using NexusBank.Domain.Entities;
using NexusBank.Domain.Exceptions;

namespace NexusBank.Domain.Services;

public class TransferDomainService
{
    public void Transfer(Account from, Account to, decimal amount)
    {
        if (from == null) throw new ArgumentNullException(nameof(from));
        if (to == null) throw new ArgumentNullException(nameof(to));

        if (from.Balance.Amount < amount)
            throw new InsufficientFundsException(from.Balance.Amount, amount);

        from.Withdraw(amount);
        to.Deposit(amount);
    }
}
