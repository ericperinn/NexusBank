using FluentAssertions;
using Moq;
using NexusBank.Application.UseCases;
using NexusBank.Domain.Entities;
using NexusBank.Domain.Exceptions;
using NexusBank.Domain.Repositories;
using NexusBank.Domain.Services;
using System.Reflection;

namespace NexusBank.Application.Tests.UseCases;

public class TransferUseCaseTests
{
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly TransferDomainService _transferDomainService;
    private readonly TransferUseCase _sut;

    public TransferUseCaseTests()
    {
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _transferDomainService = new TransferDomainService(); // We can use the real one since it has no dependencies
        
        _sut = new TransferUseCase(
            _accountRepositoryMock.Object,
            _transferDomainService,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidAccountsAndBalance_ShouldTransferAndCommit()
    {
        // Arrange
        var fromAccountId = Guid.NewGuid();
        var toAccountId = Guid.NewGuid();
        var transferAmount = 50m;

        var fromAccount = new Account("Sender");
        SetId(fromAccount, fromAccountId);
        fromAccount.Deposit(100m);

        var toAccount = new Account("Receiver");
        SetId(toAccount, toAccountId);

        _accountRepositoryMock.Setup(repo => repo.GetByIdAsync(fromAccountId))
            .ReturnsAsync(fromAccount);

        _accountRepositoryMock.Setup(repo => repo.GetByIdAsync(toAccountId))
            .ReturnsAsync(toAccount);

        // Act
        await _sut.ExecuteAsync(fromAccountId, toAccountId, transferAmount);

        // Assert
        fromAccount.Balance.Amount.Should().Be(50m);
        toAccount.Balance.Amount.Should().Be(50m);

        _accountRepositoryMock.Verify(repo => repo.UpdateAsync(fromAccount), Times.Once);
        _accountRepositoryMock.Verify(repo => repo.UpdateAsync(toAccount), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidFromAccount_ShouldThrowAccountNotFoundException()
    {
        // Arrange
        var fromAccountId = Guid.NewGuid();
        var toAccountId = Guid.NewGuid();

        _accountRepositoryMock.Setup(repo => repo.GetByIdAsync(fromAccountId))
            .ReturnsAsync((Account)null!);

        // Act
        Func<Task> act = async () => await _sut.ExecuteAsync(fromAccountId, toAccountId, 50m);

        // Assert
        await act.Should().ThrowAsync<AccountNotFoundException>();
        _accountRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Account>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidToAccount_ShouldThrowAccountNotFoundException()
    {
        // Arrange
        var fromAccountId = Guid.NewGuid();
        var toAccountId = Guid.NewGuid();
        
        var fromAccount = new Account("Sender");
        SetId(fromAccount, fromAccountId);
        
        _accountRepositoryMock.Setup(repo => repo.GetByIdAsync(fromAccountId))
            .ReturnsAsync(fromAccount);

        _accountRepositoryMock.Setup(repo => repo.GetByIdAsync(toAccountId))
            .ReturnsAsync((Account)null!);

        // Act
        Func<Task> act = async () => await _sut.ExecuteAsync(fromAccountId, toAccountId, 50m);

        // Assert
        await act.Should().ThrowAsync<AccountNotFoundException>();
        _accountRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Account>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Never);
    }
    
    // Helper to set private ID for testing if needed
    private void SetId(Account account, Guid id)
    {
        var propertyInfo = typeof(Account).GetProperty(nameof(Account.Id));
        if (propertyInfo != null && propertyInfo.CanWrite)
        {
            propertyInfo.SetValue(account, id);
        }
        else
        {
            var backingField = typeof(Account).GetField($"<{nameof(Account.Id)}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
            if (backingField != null)
            {
                backingField.SetValue(account, id);
            }
        }
    }
}
