using Microsoft.EntityFrameworkCore;
using DSW_3P_PR02.Data;
using DSW_3P_PR02.Repositories;
using DSW_3P_PR02.Services;

var builder = WebApplication.CreateBuilder(args);

// OpenAPI (documento que consume Swagger UI)
builder.Services.AddOpenApi();

// Agregar controladores
builder.Services.AddControllers();

// Agregar dependencias (Repository + Service)
builder.Services.AddScoped<IPersonajeRepository, PersonajeSqlRepository>(); // antes: PersonajeJsonRepository
builder.Services.AddScoped<IPersonajeService, PersonajeService>();

// Configuracion de SQL Server (la cadena de conexion esta en User Secrets)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlServerConexion")
    )
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // Mapear Swagger
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "CharacterHub API v1");
    });
}

app.UseHttpsRedirection();

// Mapear controllers
app.MapControllers();

app.Run();

// dotnet add package Swashbuckle.AspNetCore.SwaggerUI
// dotnet add package Microsoft.EntityFrameworkCore.SqlServer
// dotnet add package Microsoft.EntityFrameworkCore.Design
// dotnet user-secrets set "ConnectionStrings:SqlServerConexion" "<cadena>"
// dotnet ef migrations add Inicial
// dotnet ef database update
