using FluentAssertions;
using NexusBank.Domain.Entities;
using NexusBank.Domain.Exceptions;

namespace NexusBank.Domain.Tests.Entities;

public class AccountTests
{
    [Fact]
    public void Constructor_WithValidOwnerName_ShouldCreateAccount()
    {
        // Arrange
        var ownerName = "John Doe";

        // Act
        var account = new Account(ownerName);

        // Assert
        account.Id.Should().NotBeEmpty();
        account.OwnerName.Should().Be(ownerName);
        account.Balance.Amount.Should().Be(0m);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidOwnerName_ShouldThrowDomainException(string invalidName)
    {
        // Act
        Action act = () => new Account(invalidName);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Owner name is required.");
    }

    [Fact]
    public void Deposit_WithValidAmount_ShouldIncreaseBalance()
    {
        // Arrange
        var account = new Account("John Doe");
        var depositAmount = 100m;

        // Act
        account.Deposit(depositAmount);

        // Assert
        account.Balance.Amount.Should().Be(depositAmount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    public void Deposit_WithInvalidAmount_ShouldThrowDomainException(decimal invalidAmount)
    {
        // Arrange
        var account = new Account("John Doe");

        // Act
        Action act = () => account.Deposit(invalidAmount);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Deposit amount must be greater than zero.");
    }

    [Fact]
    public void Withdraw_WithValidAmount_ShouldDecreaseBalance()
    {
        // Arrange
        var account = new Account("John Doe");
        account.Deposit(100m);
        var withdrawAmount = 50m;

        // Act
        account.Withdraw(withdrawAmount);

        // Assert
        account.Balance.Amount.Should().Be(50m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    public void Withdraw_WithInvalidAmount_ShouldThrowDomainException(decimal invalidAmount)
    {
        // Arrange
        var account = new Account("John Doe");

        // Act
        Action act = () => account.Withdraw(invalidAmount);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Withdrawal amount must be greater than zero.");
    }

    [Fact]
    public void Withdraw_WithAmountGreaterThanBalance_ShouldThrowInsufficientFundsException()
    {
        // Arrange
        var account = new Account("John Doe");
        account.Deposit(50m);
        var withdrawAmount = 100m;

        // Act
        Action act = () => account.Withdraw(withdrawAmount);

        // Assert
        act.Should().Throw<InsufficientFundsException>();
    }
}
