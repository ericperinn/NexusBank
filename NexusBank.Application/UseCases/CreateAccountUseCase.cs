using NexusBank.Domain.Entities;
using NexusBank.Domain.Repositories;

namespace NexusBank.Application.UseCases;

public class CreateAccountUseCase
{
    private readonly IAccountRepository _repository;

    // Injeção de Dependência: A aplicação recebe o banco de dados pronto, 
    // mas ela só conhece a "Interface", não sabe qual é o banco real.
    public CreateAccountUseCase(IAccountRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> ExecuteAsync(string ownerName)
    {
        // 1. Cria a Entidade (Aqui o Domain valida se o nome é nulo/vazio)
        var account = new Account(ownerName);

        // 2. Salva a conta
        await _repository.AddAsync(account);

        // 3. Retorna o ID gerado para o usuário
        return account.Id;
    }
}