using Microsoft.EntityFrameworkCore;
using NexusBank.Domain.Entities;

namespace NexusBank.Infrastructure.Data;

public class AppDbContext : DbContext
{
    // Esta linha diz: "Crie uma tabela no banco chamada Accounts baseada na entidade Account"
    public DbSet<Account> Accounts { get; set; }

    // Este construtor permite que a nossa API passe a configuração (qual arquivo do SQLite usar)
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Como os 'setters' da nossa entidade Account são privados, o Entity Framework precisa de uma ajudinha
    // para saber como ler e gravar. Isso é feito aqui:
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(builder =>
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.OwnerName).IsRequired().HasMaxLength(100);
            builder.Property(a => a.Balance).IsRequired();
        });
    }
}