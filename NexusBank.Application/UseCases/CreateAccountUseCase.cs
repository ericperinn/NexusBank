using NexusBank.Domain.Entities;
using NexusBank.Domain.Repositories;

namespace NexusBank.Application.UseCases;

public class CreateAccountUseCase
{
    private readonly IAccountRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAccountUseCase(IAccountRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> ExecuteAsync(string ownerName)
    {
        var account = new Account(ownerName);

        await _repository.AddAsync(account);
        await _unitOfWork.CommitAsync();

        return account.Id;
    }
}
