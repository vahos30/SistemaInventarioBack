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

var builder = WebApplication.CreateBuilder(args);

// Configurar serialización JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new Converters.DateTimeConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Configurar CORS (Versión GitHub + localhost para desarrollo)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "https://sistema-ventas.netlify.app",
                "https://sistemainventario-cpg5hbcpdacqaubk.centralus-01.azurewebsites.net")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Registrar DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositorios (Todos los tuyos locales)
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IReciboRepository, ReciboRepository>();
builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();
builder.Services.AddScoped<ICompraRepository, CompraRepository>();
builder.Services.AddScoped<IProductoRepository>(provider =>
    new ProductoRepositoryProxy(
        new ProductoRepository(provider.GetRequiredService<AppDbContext>()),
        provider.GetRequiredService<ILogger<ProductoRepositoryProxy>>()));
builder.Services.AddScoped<IFacturaRepository, FacturaRepository>(); 
builder.Services.AddScoped<INotaCreditoRepository, NotaCreditoRepository>(); 

// Servicios de aplicación
builder.Services.AddScoped<ProveedorService>();
builder.Services.AddScoped<CompraService>();

// MediatR (Todos tus handlers locales)
builder.Services.AddMediatR(typeof(CrearClienteCommandHandler).Assembly);
builder.Services.AddMediatR(typeof(CrearReciboCommandHandler).Assembly);
builder.Services.AddMediatR(typeof(CrearFacturaCommandHandler).Assembly);

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Migración de base de datos
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
    if (dbContext.Database.IsRelational())
    {
        dbContext.Database.Migrate();
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

// Converter para DateTime (igual en ambos)
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





