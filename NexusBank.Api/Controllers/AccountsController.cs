using Microsoft.AspNetCore.Mvc;
using NexusBank.Application.UseCases;
using NexusBank.Api.DTOs;

namespace NexusBank.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly CreateAccountUseCase _createAccountUseCase;
    private readonly DepositUseCase _depositUseCase;
    private readonly TransferUseCase _transferUseCase;
    private readonly GetAccountUseCase _getAccountUseCase;

    // A API recebe os casos de uso prontos aqui!
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAccount(Guid id)
    {
        try
        {
            var account = await _getAccountUseCase.ExecuteAsync(id);

            // Transformamos a Entidade em um DTO antes de mandar para o cliente
            var response = new AccountResponse(account.Id, account.OwnerName, account.Balance);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return NotFound(new { Error = ex.Message }); // Retorna 404 se não achar a conta
        }
    }
}