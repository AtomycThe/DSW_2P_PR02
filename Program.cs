using Microsoft.EntityFrameworkCore;
using DSW_3P_PR02.Data;
using DSW_3P_PR02.Repositories;
using DSW_3P_PR02.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// OpenAPI (documento que consume Swagger UI)
builder.Services.AddOpenApi(options =>
{
    // Esquema Bearer para poder usar el boton "Authorize" en Swagger UI
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Pega aqui el access_token obtenido en /api/auth/login"
        };
        document.Security ??= new List<OpenApiSecurityRequirement>();
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
        });
        return Task.CompletedTask;
    });
});

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

// Configuracion de JWT (Jwt:Key esta en User Secrets)
var jwt = builder.Configuration.GetSection("Jwt");
var key = Convert.FromBase64String(
    jwt["Key"] ?? throw new InvalidOperationException("Falta configurar Jwt:Key"));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwt["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });
builder.Services.AddAuthorization();

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

app.UseAuthentication();
app.UseAuthorization();

// Mapear controllers
app.MapControllers();

app.Run();

// dotnet add package Swashbuckle.AspNetCore.SwaggerUI
// dotnet add package Microsoft.EntityFrameworkCore.SqlServer
// dotnet add package Microsoft.EntityFrameworkCore.Design
// dotnet user-secrets set "ConnectionStrings:SqlServerConexion" "<cadena>"
// dotnet ef migrations add Inicial
// dotnet ef database update
// dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version "10.0.*"
// dotnet user-secrets set "Jwt:Key" "<clave base64>"
// dotnet user-secrets set "DemoUser:Password" "<password demo>"
