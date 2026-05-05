namespace NexusBank.Api.DTOs;

public record TransferRequest(Guid FromAccountId, Guid ToAccountId, decimal Amount);