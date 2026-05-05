using Microsoft.EntityFrameworkCore;
using NexusBank.Domain.Entities;
using NexusBank.Domain.Repositories;

namespace NexusBank.Infrastructure.Data;

// Note que aqui nós avisamos o C# que esta classe ASSINA o contrato IAccountRepository
public class AccountRepository : IAccountRepository
{
    private readonly AppDbContext _context;

    // Recebemos o banco de dados via injeção de dependência
    public AccountRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Account account)
    {
        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync(); // É aqui que o comando INSERT vai para o SQLite
    }

    public async Task<Account?> GetByIdAsync(Guid id)
    {
        // Busca a conta no banco onde o Id seja igual ao Id passado
        return await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task UpdateAsync(Account account)
    {
        _context.Accounts.Update(account);
        await _context.SaveChangesAsync(); // É aqui que o comando UPDATE vai para o SQLite
    }
}