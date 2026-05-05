using Microsoft.AspNetCore.Mvc;
using NexusBank.Application.UseCases;

namespace NexusBank.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly CreateAccountUseCase _createAccountUseCase;
    private readonly DepositUseCase _depositUseCase;
    private readonly TransferUseCase _transferUseCase;

    // A API recebe os casos de uso prontos aqui!
    public AccountsController(
        CreateAccountUseCase createAccountUseCase,
        DepositUseCase depositUseCase,
        TransferUseCase transferUseCase)
    {
        _createAccountUseCase = createAccountUseCase;
        _depositUseCase = depositUseCase;
        _transferUseCase = transferUseCase;
    }

    // Endpoint 1: Criar Conta
    // POST: api/accounts
    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] string ownerName)
    {
        try
        {
            var accountId = await _createAccountUseCase.ExecuteAsync(ownerName);
            return Ok(new { Message = "Conta criada com sucesso!", AccountId = accountId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message }); // Se o DDD bloquear (ex: nome vazio), o erro cai aqui
        }
    }

    // Endpoint 2: Fazer Depósito
    // POST: api/accounts/{id}/deposit
    [HttpPost("{id}/deposit")]
    public async Task<IActionResult> Deposit(Guid id, [FromBody] decimal amount)
    {
        try
        {
            await _depositUseCase.ExecuteAsync(id, amount);
            return Ok(new { Message = $"Depósito de {amount} realizado com sucesso!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message }); // Se tentar depositar valor negativo, o erro do DDD cai aqui
        }
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
    {
        try
        {
            await _transferUseCase.ExecuteAsync(request.FromAccountId, request.ToAccountId, request.Amount);
            return Ok(new { Message = "Transferência realizada com sucesso!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}
public record TransferRequest(Guid FromAccountId, Guid ToAccountId, decimal Amount);