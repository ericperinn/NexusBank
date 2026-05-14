using MediatR;
using NexusBank.Application.DTOs;
using NexusBank.Domain.Exceptions;
using NexusBank.Domain.Repositories;

namespace NexusBank.Application.Queries.GetAccount;

public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, AccountDto>
{
    private readonly IAccountRepository _repository;

    public GetAccountQueryHandler(IAccountRepository repository)
    {
        _repository = repository;
    }

    public async Task<AccountDto> Handle(GetAccountQuery request, CancellationToken cancellationToken)
    {
        var account = await _repository.GetByIdAsync(request.AccountId)
            ?? throw new AccountNotFoundException(request.AccountId);

        return new AccountDto(account.Id, account.OwnerName, account.Balance);
    }
}
