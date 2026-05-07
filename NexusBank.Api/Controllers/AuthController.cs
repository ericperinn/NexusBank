using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NexusBank.Api.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NexusBank.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;

    // Adicionamos o IConfiguration para poder ler o appsettings.json
    public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = new IdentityUser { UserName = request.Email, Email = request.Email };
        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded) return Ok(new { Message = "Usuário criado com sucesso!" });
        return BadRequest(result.Errors);
    }

    // --- NOVO ENDPOINT DE LOGIN ---
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // 1. Procuramos o usuário no banco pelo e-mail
        var user = await _userManager.FindByEmailAsync(request.Email);

        // 2. Se o usuário não existir OU a senha estiver errada, negamos a entrada!
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized(new { Message = "Email ou senha inválidos." });
        }

        // 3. Se chegou aqui, o login tá certo. Vamos fabricar o Token!
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = Encoding.ASCII.GetBytes(jwtSettings["Secret"]!);

        // Aqui nós colocamos "carimbos" (Claims) dentro do Token, tipo o ID e o E-mail dele
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!)
            }),
            Expires = DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpirationInHours"]!)),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // Devolvemos o Token em formato de texto para o cliente
        return Ok(new { Token = tokenHandler.WriteToken(token) });
    }
}