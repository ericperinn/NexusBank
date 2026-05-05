using Microsoft.EntityFrameworkCore;
using NexusBank.Application.UseCases;
using NexusBank.Domain.Repositories;
using NexusBank.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar o Banco de Dados (SQLite)
// Aqui definimos que o arquivo do banco vai se chamar "nexusbank.db"
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=nexusbank.db"));

// 2. Injeção de Dependência (A Mágica da Clean Architecture!)
// "Sempre que alguém pedir a Interface, entregue a classe real"
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

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
    db.Database.EnsureCreated(); // Se o nexusbank.db não existir, ele cria a tabela de Contas na hora!
}

// Configurar o Swagger (Nossa interface de testes)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();