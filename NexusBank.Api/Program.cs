using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NexusBank.Application.UseCases;
using NexusBank.Domain.Repositories;
using NexusBank.Domain.Services;
using NexusBank.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar o Banco de Dados (SQLite)
// Aqui definimos que o arquivo do banco vai se chamar "nexusbank.db"
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=nexusbank.db"));

// --- INÍCIO DAS CONFIGURAÇÕES DE SEGURANÇA ---

// 1.1 Configurando o Identity (Tabelas de Usuários e Senhas da Microsoft)
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// 1.2 Lendo as chaves do appsettings.json
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.ASCII.GetBytes(jwtSettings["Secret"]!);

// 1.3 Ensinando a API a validar o Token JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true
    };
});

// --- FIM DAS CONFIGURAÇÕES DE SEGURANÇA ---

// 2. Injeção de Dependência (A Mágica da Clean Architecture!)
// "Sempre que alguém pedir a Interface, entregue a classe real"
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

// Domain Services
builder.Services.AddScoped<TransferDomainService>();

// Registrar os Casos de Uso para a API poder chamá-los
builder.Services.AddScoped<CreateAccountUseCase>();
builder.Services.AddScoped<DepositUseCase>();
builder.Services.AddScoped<TransferUseCase>();
builder.Services.AddScoped<GetAccountUseCase>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 3. Truque de Engenharia: Criar o banco de dados automaticamente ao rodar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated(); // Vai recriar o banco, agora com as 7 tabelas do Identity!
}

// Configurar o Swagger (Nossa interface de testes)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 4. Catracas de Segurança na ordem exata!
app.UseAuthentication(); // 1º Lê o Token ("Quem é você?")
app.UseAuthorization();  // 2º Valida as regras ("Você pode entrar?")

app.MapControllers();
app.Run();