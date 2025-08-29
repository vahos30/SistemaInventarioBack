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

var builder = WebApplication.CreateBuilder(args);

// Configurar serialización JSON
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
        policy.WithOrigins("https://sistema-ventas.netlify.app",
            "https://sistemainventario-cpg5hbcpdacqaubk.centralus-01.azurewebsites.net")
              .AllowAnyHeader()
              .AllowAnyMethod();
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
builder.Services.AddScoped<INotaCreditoRepository, NotaCreditoRepository>(); // <-- AGREGA ESTA LÍNEA

// Servicios de aplicación (Opcional, pero recomendado)
builder.Services.AddScoped<ProveedorService>();
builder.Services.AddScoped<CompraService>();

// MediatR
builder.Services.AddMediatR(typeof(CrearClienteCommandHandler).Assembly);
builder.Services.AddMediatR(typeof(CrearReciboCommandHandler).Assembly);
builder.Services.AddMediatR(typeof(CrearFacturaCommandHandler).Assembly);

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

builder.Services.AddIdentity<Usuario, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

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

var app = builder.Build();

// migracones a la base de datos
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
    if (dbContext.Database.IsRelational())
    {
        dbContext.Database.Migrate();
    }
}

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Crear el rol Administrador si no existe
    if (!await roleManager.RoleExistsAsync("Administrador"))
        await roleManager.CreateAsync(new IdentityRole("Administrador"));

    // Crear el usuario administrador si no existe
    var adminUser = await userManager.FindByNameAsync("admin");
    if (adminUser == null)
    {
        adminUser = new Usuario { UserName = "admin", Email = "admin@correo.com" };
        await userManager.CreateAsync(adminUser, "TuContraseñaSegura123!");
        await userManager.AddToRoleAsync(adminUser, "Administrador");
    }
}

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







