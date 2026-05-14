using MediatR;
using NexusBank.Domain.Exceptions;
using NexusBank.Domain.Repositories;
using NexusBank.Domain.Services;

namespace NexusBank.Application.Commands.Transfer;

public class TransferCommandHandler : IRequestHandler<TransferCommand>
{
    private readonly IAccountRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TransferDomainService _transferService;

    public TransferCommandHandler(
        IAccountRepository repository,
        IUnitOfWork unitOfWork,
        TransferDomainService transferService)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _transferService = transferService;
    }

    public async Task Handle(TransferCommand request, CancellationToken cancellationToken)
    {
        var from = await _repository.GetByIdAsync(request.FromAccountId)
            ?? throw new AccountNotFoundException(request.FromAccountId);
        var to = await _repository.GetByIdAsync(request.ToAccountId)
            ?? throw new AccountNotFoundException(request.ToAccountId);

        _transferService.Transfer(from, to, request.Amount);

        await _repository.UpdateAsync(from);
        await _repository.UpdateAsync(to);
        await _unitOfWork.CommitAsync();
    }
}
