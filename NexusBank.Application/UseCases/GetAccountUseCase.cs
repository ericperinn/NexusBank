using NexusBank.Domain.Repositories;
using NexusBank.Domain.Exceptions;
using NexusBank.Application.DTOs;

namespace NexusBank.Application.UseCases;

public class GetAccountUseCase
{
    private readonly IAccountRepository _repository;

    public GetAccountUseCase(IAccountRepository repository)
    {
        _repository = repository;
    }

    public async Task<AccountDto> ExecuteAsync(Guid id)
    {
        var account = await _repository.GetByIdAsync(id) 
            ?? throw new AccountNotFoundException(id);

        return new AccountDto(account.Id, account.OwnerName, account.Balance);
    }
}
