using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusBank.Application.Commands.CreateAccount;
using NexusBank.Application.Commands.Deposit;
using NexusBank.Application.Commands.Transfer;
using NexusBank.Application.Queries.GetAccount;
using NexusBank.Api.DTOs;
using NexusBank.Domain.Exceptions;

namespace NexusBank.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] string ownerName)
    {
        try
        {
            var id = await _mediator.Send(new CreateAccountCommand(ownerName));
            return CreatedAtAction(nameof(GetAccount), new { id }, new { id });
        }
        catch (DomainException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/deposit")]
    public async Task<IActionResult> Deposit(Guid id, [FromBody] decimal amount)
    {
        try
        {
            await _mediator.Send(new DepositCommand(id, amount));
            return NoContent();
        }
        catch (AccountNotFoundException)
        {
            return NotFound();
        }
        catch (DomainException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
    {
        try
        {
            await _mediator.Send(new TransferCommand(request.FromAccountId, request.ToAccountId, request.Amount));
            return NoContent();
        }
        catch (AccountNotFoundException)
        {
            return NotFound();
        }
        catch (DomainException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAccount(Guid id)
    {
        try
        {
            var account = await _mediator.Send(new GetAccountQuery(id));
            return Ok(new AccountResponse(account.Id, account.OwnerName, account.Balance));
        }
        catch (AccountNotFoundException)
        {
            return NotFound();
        }
    }
}
