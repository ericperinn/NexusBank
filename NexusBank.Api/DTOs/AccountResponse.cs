namespace NexusBank.Api.DTOs;

public record AccountResponse(Guid Id, string OwnerName, decimal Balance);