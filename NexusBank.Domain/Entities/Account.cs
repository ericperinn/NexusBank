namespace NexusBank.Domain.Entities;

public class Account
{
    // 1. Setters PRIVADOS: Ninguém de fora pode mudar essas propriedades diretamente.
    public Guid Id { get; private set; }
    public string OwnerName { get; private set; }
    public decimal Balance { get; private set; }

    // 2. Construtor: A única forma de nascer uma conta válida.
    public Account(string ownerName)
    {
        if (string.IsNullOrWhiteSpace(ownerName))
            throw new ArgumentException("O nome do titular é obrigatório.");

        Id = Guid.NewGuid();
        OwnerName = ownerName;
        Balance = 0m; // Começa zerada
    }

    // 3. Comportamentos (Regras de Negócio Puras)
    public void Deposit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("O valor do depósito deve ser maior que zero.");

        Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("O valor do saque deve ser maior que zero.");

        if (amount > Balance)
            throw new InvalidOperationException("Saldo insuficiente para realizar o saque.");

        Balance -= amount;
    }
}