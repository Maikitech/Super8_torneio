using MeuProjetoApi.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuração dos Serviços
builder.Services.AddControllers();

// Injeção de Dependência do Serviço de Produtos (Singleton para manter os dados em memória)
builder.Services.AddSingleton<IProdutoService, ProdutoService>();

// Configuração do Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Meu Projeto API - Catálogo de Produtos",
        Version = "v1",
        Description = "API RESTful construída com ASP.NET Core 8 demonstrando boas práticas, Injeção de Dependência e documentação Swagger.",
        Contact = new OpenApiContact
        {
            Name = "Suporte à API"
        }
    });
});

// Configuração de CORS para permitir consumo frontend local
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTudo", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configuração do pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catálogo de Produtos v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseCors("PermitirTudo");

app.UseAuthorization();

app.MapControllers();

// Endpoint raiz de boas-vindas e redirecionamento rápido
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
