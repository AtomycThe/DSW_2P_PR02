using DSW_3P_PR02.Repositories;
using DSW_3P_PR02.Services;

var builder = WebApplication.CreateBuilder(args);

// OpenAPI (documento que consume Swagger UI)
builder.Services.AddOpenApi();

// Agregar controladores
builder.Services.AddControllers();

// Agregar dependencias (Repository + Service)
builder.Services.AddScoped<IPersonajeRepository, PersonajeJsonRepository>();
builder.Services.AddScoped<IPersonajeService, PersonajeService>();

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
