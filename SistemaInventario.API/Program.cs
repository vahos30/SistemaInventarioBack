using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaInventario.Application.Mapping;
using SistemaInventario.Infrastructure.Persistence;
using AutoMapper;
using SistemaInventario.Domain.Interfaces;
using SistemaInventario.Infrastructure.Repositories;
using MediatR;
using SistemaInventario.Application.Feactures.Clientes;
using SistemaInventario.Application.Feactures.Recibos;
using System.Text.Json.Serialization;
using SistemaInventario.Application.Services;
using SistemaInventario.Domain.Interfaces.SistemaInventario.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SistemaInventario.Domain.Entities;
using Microsoft.OpenApi.Models;
using SistemaInventario.Application.Feactures.Facturas;

var builder = WebApplication.CreateBuilder(args);

// Configurar serializaci�n JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new Converters.DateTimeConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "https://sistema-ventas.netlify.app",
            "http://localhost:3000",
            "http://72.61.70.114:3000",
            "http://72.61.70.114:3001",
            "https://tecnofrio.jvcsoluciones.cloud")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Registrar DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositorios
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IReciboRepository, ReciboRepository>();
builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>(); // <- Proveedor
builder.Services.AddScoped<ICompraRepository, CompraRepository>();       // <- Compra
builder.Services.AddScoped<IProductoRepository>(provider =>
    new ProductoRepositoryProxy(
        new ProductoRepository(provider.GetRequiredService<AppDbContext>()),
        provider.GetRequiredService<ILogger<ProductoRepositoryProxy>>()));
builder.Services.AddScoped<IFacturaRepository, FacturaRepository>();
builder.Services.AddScoped<INotaCreditoRepository, NotaCreditoRepository>(); // <-- AGREGA ESTA L�NEA

// Servicios de aplicaci�n (Opcional, pero recomendado)
builder.Services.AddScoped<ProveedorService>();
builder.Services.AddScoped<CompraService>();

// MediatR
builder.Services.AddMediatR(typeof(CrearClienteCommandHandler).Assembly);
builder.Services.AddMediatR(typeof(CrearReciboCommandHandler).Assembly);
builder.Services.AddMediatR(typeof(CrearFacturaCommandHandler).Assembly);
builder.Services.AddMediatR(typeof(CrearFacturaFactusCommandHandler).Assembly);


// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introduce el token JWT con el prefijo 'Bearer'. Ejemplo: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddIdentity<Usuario, IdentityRole>(options =>
{
    options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultProvider;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddHttpClient<FactusAuthService>();
builder.Services.AddHttpClient<FactusFacturaService>();
builder.Services.AddHttpClient<CiudadService>();

var app = builder.Build();

// --- INICIO DE BLOQUE PROTEGIDO ---
// Agrupamos migraciones y semillas de datos en un solo bloque con manejo de errores
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var dbContext = services.GetRequiredService<AppDbContext>();
        
        if (dbContext.Database.IsRelational())
        {
            logger.LogInformation(">>> Iniciando migraciones de base de datos...");
            await dbContext.Database.MigrateAsync();
            logger.LogInformation(">>> Migraciones completadas con éxito.");
        }

        // Configuración de Identidad (Roles y Admin)
        var userManager = services.GetRequiredService<UserManager<Usuario>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Crear el rol Administrador si no existe
        if (!await roleManager.RoleExistsAsync("Administrador"))
        {
            await roleManager.CreateAsync(new IdentityRole("Administrador"));
        }

        // Crear el usuario administrador si no existe
        var adminUser = await userManager.FindByNameAsync("admin");
        if (adminUser == null)
        {
            adminUser = new Usuario { UserName = "admin", Email = "admin@correo.com" };
            var result = await userManager.CreateAsync(adminUser, "TuContraseñaSegura123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Administrador");
                logger.LogInformation(">>> Usuario administrador creado exitosamente.");
            }
        }
    }
    catch (Exception ex)
    {
        // Si la base de datos no está lista, la API mostrará este error pero NO se cerrará
        logger.LogError(ex, ">>> ERROR CRÍTICO: No se pudo conectar o migrar la base de datos.");
        Console.WriteLine($"⚠️ ADVERTENCIA: La API inició sin conexión a DB: {ex.Message}");
    }
}
// --- FIN DE BLOQUE PROTEGIDO ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();

// Agrega la clase en un namespace con nombre
namespace Converters
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public class DateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString()!);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss"));
        }
    }
}








