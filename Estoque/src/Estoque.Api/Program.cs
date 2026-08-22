using Estoque.Api.Middlewares;
using Estoque.Application.UseCases.BaixarSaldo;
using Estoque.Application.UseCases.CadastrarProduto;
using Estoque.Domain.Interfaces;
using Estoque.Infrastructure.Persistence;
using Estoque.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Estoque.Application.Interfaces;
using Estoque.Infrastructure.ExternalServices;
using Estoque.Application.UseCases.SugerirDescricaoProduto;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<IIaClient, GeminiIaClient>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Banco de dados (PostgreSQL via EF Core)
var connectionString = builder.Configuration.GetConnectionString("EstoqueConnection");
builder.Services.AddDbContext<EstoqueDbContext>(options =>
    options.UseNpgsql(connectionString));

// Repositórios (Dependency Inversion — Domain define o contrato, Infrastructure implementa)
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();

// Casos de uso (Application)
builder.Services.AddScoped<CadastrarProdutoUseCase>();
builder.Services.AddScoped<BaixarSaldoUseCase>();
builder.Services.AddScoped<SugerirDescricaoProdutoUseCase>();

// CORS - necessário para o Angular (rodando em outra porta) acessar essa API
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("PermitirAngular");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();