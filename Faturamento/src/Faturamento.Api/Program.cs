using Faturamento.Api.Middlewares;
using Faturamento.Api.Resilience;
using Faturamento.Application.Interfaces;
using Faturamento.Application.UseCases.AdicionarItem;
using Faturamento.Application.UseCases.CriarNotaFiscal;
using Faturamento.Application.UseCases.ImprimirNotaFiscal;
using Faturamento.Application.UseCases.RemoverItem;
using Faturamento.Domain.Interfaces;
using Faturamento.Infrastructure.ExternalServices;
using Faturamento.Infrastructure.Persistence;
using Faturamento.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;


var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<RemoverItemUseCase>();

// Banco de dados (PostgreSQL via EF Core)
var connectionString = builder.Configuration.GetConnectionString("FaturamentoConnection");
builder.Services.AddDbContext<FaturamentoDbContext>(options =>
    options.UseNpgsql(connectionString));

// Repositórios (Dependency Inversion — Domain define o contrato, Infrastructure implementa)
builder.Services.AddScoped<INotaFiscalRepository, NotaFiscalRepository>();

// Casos de uso (Application)
builder.Services.AddScoped<CriarNotaFiscalUseCase>();
builder.Services.AddScoped<AdicionarItemUseCase>();
builder.Services.AddScoped<ImprimirNotaFiscalUseCase>();

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

builder.Services.AddHttpClient<IEstoqueApiClient, EstoqueApiClient>(client =>
{
    var estoqueApiBaseUrl = builder.Configuration["EstoqueApi:BaseUrl"];
    client.BaseAddress = new Uri(estoqueApiBaseUrl!);
    client.Timeout = TimeSpan.FromSeconds(10); // timeout total do HttpClient, "rede de segurança"
})
.AddPolicyHandler(PollyPolicies.ObterPoliticaRetry())
.AddPolicyHandler(PollyPolicies.ObterPoliticaCircuitBreaker());

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