using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusBank.Api.DTOs;
using NexusBank.Application.UseCases;

namespace NexusBank.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly CreateAccountUseCase _createAccountUseCase;
    private readonly DepositUseCase _depositUseCase;
    private readonly TransferUseCase _transferUseCase;
    private readonly GetAccountUseCase _getAccountUseCase;

    // The API receives the use cases already wired via DI.
    public AccountsController(
        CreateAccountUseCase createAccountUseCase,
        DepositUseCase depositUseCase,
        TransferUseCase transferUseCase,
        GetAccountUseCase getAccountUseCase)
    {
        _createAccountUseCase = createAccountUseCase;
        _depositUseCase = depositUseCase;
        _transferUseCase = transferUseCase;
        _getAccountUseCase = getAccountUseCase;
    }

    // Endpoint 1: Create account
    // POST: api/accounts
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] string ownerName)
    {
        try
        {
            var accountId = await _createAccountUseCase.ExecuteAsync(ownerName);
            return Ok(new { Message = "Account created successfully!", AccountId = accountId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    // Endpoint 2: Deposit
    // POST: api/accounts/{id}/deposit
    [Authorize]
    [HttpPost("{id}/deposit")]
    public async Task<IActionResult> Deposit(Guid id, [FromBody] decimal amount)
    {
        try
        {
            await _depositUseCase.ExecuteAsync(id, amount);
            return Ok(new { Message = $"Deposit of {amount} completed successfully!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [Authorize]
    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
    {
        try
        {
            await _transferUseCase.ExecuteAsync(request.FromAccountId, request.ToAccountId, request.Amount);
            return Ok(new { Message = "Transfer completed successfully!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAccount(Guid id)
    {
        try
        {
            var account = await _getAccountUseCase.ExecuteAsync(id);

            // Map to API response DTO
            var response = new AccountResponse(account.Id, account.OwnerName, account.Balance);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return NotFound(new { Error = ex.Message });
        }
    }
}