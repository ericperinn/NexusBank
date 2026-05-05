using NexusBank.Domain.Repositories;

namespace NexusBank.Application.UseCases;

public class TransferUseCase
{
    private readonly IAccountRepository _repository;

    public TransferUseCase(IAccountRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(Guid fromAccountId, Guid toAccountId, decimal amount)
    {
        // 1. Fetch: Busca as DUAS contas no banco
        var fromAccount = await _repository.GetByIdAsync(fromAccountId);
        var toAccount = await _repository.GetByIdAsync(toAccountId);

        if (fromAccount == null) throw new Exception("Conta de origem não encontrada.");
        if (toAccount == null) throw new Exception("Conta de destino não encontrada.");

        // 2. Mutate
        fromAccount.Transfer(toAccount, amount);

        // 3. Save: Atualiza as duas contas no banco de dados
        // Nota: Como o Entity Framework é inteligente, ele só vai commitar essa transação 
        // no banco de dados se as duas atualizações derem certo!
        await _repository.UpdateAsync(fromAccount);
        await _repository.UpdateAsync(toAccount);
    }
}