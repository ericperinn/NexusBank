using NexusBank.Domain.Repositories;

namespace NexusBank.Application.UseCases;

public class DepositUseCase
{
    private readonly IAccountRepository _repository;

    public DepositUseCase(IAccountRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(Guid accountId, decimal amount)
    {
        // 1. Busca a conta no banco de dados usando a Interface
        var account = await _repository.GetByIdAsync(accountId);

        if (account == null)
            throw new Exception("Conta não encontrada."); // Em um cenário real, usaríamos uma Exceção Customizada

        // 2. Aqui acontece a Mágica do DDD!
        // A aplicação não faz "account.Balance += amount". 
        // Ela manda a Entidade agir. Se o amount for negativo, a PRÓPRIA entidade joga o erro.
        account.Deposit(amount);

        // 3. Manda o banco de dados atualizar e salvar a alteração
        await _repository.UpdateAsync(account);
    }
}