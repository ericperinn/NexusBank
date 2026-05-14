using MediatR;
using NexusBank.Domain.Exceptions;
using NexusBank.Domain.Repositories;

namespace NexusBank.Application.Commands.Deposit;

public class DepositCommandHandler : IRequestHandler<DepositCommand>
{
    private readonly IAccountRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DepositCommandHandler(IAccountRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DepositCommand request, CancellationToken cancellationToken)
    {
        var account = await _repository.GetByIdAsync(request.AccountId)
            ?? throw new AccountNotFoundException(request.AccountId);

        account.Deposit(request.Amount);
        await _repository.UpdateAsync(account);
        await _unitOfWork.CommitAsync();
    }
}
