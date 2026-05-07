using FluentAssertions;
using NexusBank.Domain.Entities;
using NexusBank.Domain.Exceptions;
using NexusBank.Domain.Services;

namespace NexusBank.Domain.Tests.Services;

public class TransferDomainServiceTests
{
    private readonly TransferDomainService _sut;

    public TransferDomainServiceTests()
    {
        _sut = new TransferDomainService();
    }

    [Fact]
    public void Transfer_WithValidAccountsAndSufficientBalance_ShouldTransferAmount()
    {
        // Arrange
        var fromAccount = new Account("Sender");
        fromAccount.Deposit(100m);
        var toAccount = new Account("Receiver");
        var transferAmount = 50m;

        // Act
        _sut.Transfer(fromAccount, toAccount, transferAmount);

        // Assert
        fromAccount.Balance.Should().Be(50m);
        toAccount.Balance.Should().Be(50m);
    }

    [Fact]
    public void Transfer_WithNullFromAccount_ShouldThrowArgumentNullException()
    {
        // Arrange
        Account fromAccount = null!;
        var toAccount = new Account("Receiver");

        // Act
        Action act = () => _sut.Transfer(fromAccount, toAccount, 50m);

        // Assert
        act.Should().Throw<ArgumentNullException>()
           .WithParameterName("from");
    }

    [Fact]
    public void Transfer_WithNullToAccount_ShouldThrowArgumentNullException()
    {
        // Arrange
        var fromAccount = new Account("Sender");
        fromAccount.Deposit(100m);
        Account toAccount = null!;

        // Act
        Action act = () => _sut.Transfer(fromAccount, toAccount, 50m);

        // Assert
        act.Should().Throw<ArgumentNullException>()
           .WithParameterName("to");
    }

    [Fact]
    public void Transfer_WithInsufficientFunds_ShouldThrowInsufficientFundsException()
    {
        // Arrange
        var fromAccount = new Account("Sender");
        fromAccount.Deposit(50m);
        var toAccount = new Account("Receiver");
        var transferAmount = 100m;

        // Act
        Action act = () => _sut.Transfer(fromAccount, toAccount, transferAmount);

        // Assert
        act.Should().Throw<InsufficientFundsException>();
        fromAccount.Balance.Should().Be(50m); // Balance unchanged
        toAccount.Balance.Should().Be(0m);   // Balance unchanged
    }
}
