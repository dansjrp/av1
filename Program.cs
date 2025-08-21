using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using System.ComponentModel.DataAnnotations;
using Tree;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Criar conexão SQLite em memória que persiste durante a vida da aplicação
var connection = new SqliteConnection("Data Source=:memory:");
connection.Open();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connection));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Salary API",
        Version = "v1",
        Description = "API para consultar colaboradores com maior salário por departamento"
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Criar as tabelas no banco
    await context.Database.EnsureCreatedAsync();

    // Popular os dados iniciais
    await SeedData(context);
}

// Garantir que a conexão seja fechada quando a aplicação terminar
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStopping.Register(() => connection.Close());

// Endpoint raiz - redireciona para o Swagger
app.MapGet("/", () => Results.Redirect("/swagger"))
.WithName("Home");

// Endpoint de informações da API
app.MapGet("/info", () => new
{
    Api = "Salary API",
    Version = "1.0",
    Description = "API para consultar colaboradores com maior salário por departamento",
    Endpoints = new[]
    {
        "GET /colaboradores-maior-salario - Consulta SQL com CTE",
        "GET /colaboradores-maior-salario-linq - Consulta usando LINQ",
        "GET /colaboradores - Lista todos os colaboradores"
    }
})
.WithName("GetInfo");

// Endpoint para obter colaboradores com maior salário por departamento
app.MapGet("/colaboradores-maior-salario", async (AppDbContext context) =>
{
    var resultado = await context.Database.SqlQueryRaw<ColaboradorMaiorSalarioDto>(@"
        WITH RankedSalarios AS (
            SELECT 
                p.Id,
                p.Nome as NomePessoa,
                p.Salario,
                p.DeptId,
                d.Nome as NomeDepartamento,
                ROW_NUMBER() OVER (PARTITION BY p.DeptId ORDER BY p.Salario DESC) as rn
            FROM Pessoas p
            INNER JOIN Departamentos d ON p.DeptId = d.Id
        )
        SELECT 
            NomeDepartamento,
            NomePessoa,
            Salario
        FROM RankedSalarios 
        WHERE rn = 1
        ORDER BY NomeDepartamento").ToListAsync();

    return Results.Ok(resultado);
})
.WithName("GetColaboradoresMaiorSalario");

// Endpoint alternativo usando LINQ (sem SQL raw)
app.MapGet("/colaboradores-maior-salario-linq", async (AppDbContext context) =>
{
    var resultado = await context.Pessoas
        .Include(p => p.Departamento)
        .GroupBy(p => p.DeptId)
        .Select(g => new ColaboradorMaiorSalarioDto
        {
            NomeDepartamento = g.First().Departamento.Nome,
            NomePessoa = g.OrderByDescending(p => p.Salario).First().Nome,
            Salario = g.Max(p => p.Salario)
        })
        .OrderBy(r => r.NomeDepartamento)
        .ToListAsync();

    return Results.Ok(resultado);
})
.WithName("GetColaboradoresMaiorSalarioLinq");

// Endpoint para listar todos os colaboradores (para verificação)
app.MapGet("/colaboradores", async (AppDbContext context) =>
{
    var colaboradores = await context.Pessoas
        .Include(p => p.Departamento)
        .Select(p => new
        {
            Id = p.Id,
            Nome = p.Nome,
            Salario = p.Salario,
            Departamento = p.Departamento.Nome
        })
        .ToListAsync();

    return Results.Ok(colaboradores);
})
.WithName("GetColaboradores");

// Endpoint para construir e exibir a árvore especial
app.MapPost("/tree-node", ([FromBody] TreeNodeRequest request) =>
{
    if (request.Array == null || request.Array.Length == 0)
    {
        return Results.BadRequest("Array não pode ser nulo ou vazio.");
    }

    SpecialTreeBuilder builder = new SpecialTreeBuilder();

    TreeNode? tree1 = builder.BuildTree(request.Array);

    var arrayOriginal = "Array original: [" + string.Join(", ", request.Array) + "]";
    var maiorValor = "Maior valor (raiz): " + request.Array.Max();

    Console.WriteLine(arrayOriginal);
    Console.WriteLine(maiorValor);
    Console.WriteLine("\nEstrutura da árvore:");
    tree1?.PrintTree();

    var percursoEmOrdem = "\nPercurso em ordem: [" + string.Join(", ", builder.InOrderTraversal(tree1)) + "]";
    var percursoPreOrdem = "\nPercurso pré-ordem: [" + string.Join(", ", builder.PreOrderTraversal(tree1)) + "]";
    var alturaDaArvore = "\nAltura da árvore: " + builder.GetHeight(tree1);
    var numeroDeNos = "\nNúmero de nós: " + builder.CountNodes(tree1);

    Console.WriteLine(percursoEmOrdem);
    Console.WriteLine(percursoPreOrdem);
    Console.WriteLine(alturaDaArvore);
    Console.WriteLine(numeroDeNos);

    Console.WriteLine("\n" + new string('=', 50));

    return Results.Ok(new
    {
        Message = "Veja o console para a saída detalhada da construção e travessia da árvore.",
        ArrayOriginal = arrayOriginal,
        MaiorValor = maiorValor,
        PercursoEmOrdem = percursoEmOrdem,
        PercursoPreOrdem = percursoPreOrdem,
        AlturaDaArvore = alturaDaArvore,
        NumeroDeNos = numeroDeNos
    });
})
.WithName("TreeNodeExample");

app.Run();

// Método para popular os dados iniciais
static async Task SeedData(AppDbContext context)
{
    if (!await context.Departamentos.AnyAsync())
    {
        context.Departamentos.AddRange(
            new Departamento { Id = 1, Nome = "Ti" },
            new Departamento { Id = 2, Nome = "Vendas" }
        );

        context.Pessoas.AddRange(
            new Pessoa { Id = 1, Nome = "Joe", Salario = 7000, DeptId = 1 },
            new Pessoa { Id = 2, Nome = "Henry", Salario = 8000, DeptId = 2 },
            new Pessoa { Id = 3, Nome = "Sam", Salario = 6000, DeptId = 2 },
            new Pessoa { Id = 4, Nome = "Max", Salario = 9000, DeptId = 1 }
        );

        await context.SaveChangesAsync();
    }
}

// Models
public class Pessoa
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Nome { get; set; } = string.Empty;

    public double Salario { get; set; }

    public int DeptId { get; set; }

    public Departamento Departamento { get; set; } = null!;
}

public class Departamento
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Nome { get; set; } = string.Empty;

    public List<Pessoa> Pessoas { get; set; } = new();
}

// DTO para o resultado
public class ColaboradorMaiorSalarioDto
{
    public string NomeDepartamento { get; set; } = string.Empty;
    public string NomePessoa { get; set; } = string.Empty;
    public double Salario { get; set; }
}

// DbContext
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Pessoa> Pessoas { get; set; }
    public DbSet<Departamento> Departamentos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pessoa>()
            .HasOne(p => p.Departamento)
            .WithMany(d => d.Pessoas)
            .HasForeignKey(p => p.DeptId);

        base.OnModelCreating(modelBuilder);
    }
}