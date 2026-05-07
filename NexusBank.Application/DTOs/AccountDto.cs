namespace NexusBank.Application.DTOs;

public record AccountDto(Guid Id, string OwnerName, decimal Balance);
