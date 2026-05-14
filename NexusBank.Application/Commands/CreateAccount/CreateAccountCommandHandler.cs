using MediatR;
using NexusBank.Domain.Entities;
using NexusBank.Domain.Repositories;

namespace NexusBank.Application.Commands.CreateAccount;

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Guid>
{
    private readonly IAccountRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAccountCommandHandler(IAccountRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = new Account(request.OwnerName);
        await _repository.AddAsync(account);
        await _unitOfWork.CommitAsync();
        return account.Id;
    }
}
