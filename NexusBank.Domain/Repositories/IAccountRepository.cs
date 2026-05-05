using NexusBank.Domain.Entities;

namespace NexusBank.Domain.Repositories;

public interface IAccountRepository
{
    // O Domínio exige que qualquer banco de dados consiga fazer essas 3 coisas:
    Task AddAsync(Account account);
    Task<Account?> GetByIdAsync(Guid id);
    Task UpdateAsync(Account account);
}