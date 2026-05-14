using MediatR;
using NexusBank.Application.DTOs;

namespace NexusBank.Application.Queries.GetAccount;

public record GetAccountQuery(Guid AccountId) : IRequest<AccountDto>;
