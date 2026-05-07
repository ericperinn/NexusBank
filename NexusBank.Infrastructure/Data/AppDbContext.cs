using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using NexusBank.Domain.Entities;
using NexusBank.Domain.Repositories;

namespace NexusBank.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<IdentityUser>, IUnitOfWork
{
    public DbSet<Account> Accounts { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public async Task<int> CommitAsync()
    {
        return await base.SaveChangesAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Account>(builder =>
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.OwnerName).IsRequired().HasMaxLength(100);
            builder.Property(a => a.Balance).IsRequired();
        });
    }
}
