using NexusBank.Domain.Repositories;
using NexusBank.Domain.Services;
using NexusBank.Domain.Exceptions;

namespace NexusBank.Application.UseCases;

public class TransferUseCase
{
    private readonly IAccountRepository _repository;
    private readonly TransferDomainService _transferService;
    private readonly IUnitOfWork _unitOfWork;

    public TransferUseCase(
        IAccountRepository repository, 
        TransferDomainService transferService,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _transferService = transferService;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid fromAccountId, Guid toAccountId, decimal amount)
    {
        var fromAccount = await _repository.GetByIdAsync(fromAccountId) 
            ?? throw new AccountNotFoundException(fromAccountId);

        var toAccount = await _repository.GetByIdAsync(toAccountId) 
            ?? throw new AccountNotFoundException(toAccountId);

        _transferService.Transfer(fromAccount, toAccount, amount);

        await _repository.UpdateAsync(fromAccount);
        await _repository.UpdateAsync(toAccount);

        await _unitOfWork.CommitAsync();
    }
}
