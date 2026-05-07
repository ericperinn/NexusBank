namespace NexusBank.Domain.Repositories;

public interface IUnitOfWork
{
    Task<int> CommitAsync();
}
