using NexusBank.Domain.Entities;
using NexusBank.Domain.Repositories;

namespace NexusBank.Application.UseCases;

public class GetAccountUseCase
{
    private readonly IAccountRepository _repository;

    public GetAccountUseCase(IAccountRepository repository)
    {
        _repository = repository;
    }

    public async Task<Account> ExecuteAsync(Guid id)
    {
        var account = await _repository.GetByIdAsync(id);

        if (account == null)
            throw new Exception("Conta não encontrada.");

        return account;
    }
}