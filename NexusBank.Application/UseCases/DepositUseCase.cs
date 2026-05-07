using NexusBank.Domain.Repositories;
using NexusBank.Domain.Exceptions;

namespace NexusBank.Application.UseCases;

public class DepositUseCase
{
    private readonly IAccountRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DepositUseCase(IAccountRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid accountId, decimal amount)
    {
        var account = await _repository.GetByIdAsync(accountId) 
            ?? throw new AccountNotFoundException(accountId);

        account.Deposit(amount);

        await _repository.UpdateAsync(account);
        await _unitOfWork.CommitAsync();
    }
}
