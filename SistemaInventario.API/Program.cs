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
// Agrega los namespaces que necesites para tus servicios de aplicación
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

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.WithOrigins("https://localhost:3000")
          .AllowAnyOrigin()
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

// Servicios de aplicación (Opcional, pero recomendado)
builder.Services.AddScoped<ProveedorService>();
builder.Services.AddScoped<CompraService>();

// MediatR
builder.Services.AddMediatR(typeof(CrearClienteCommandHandler).Assembly);
builder.Services.AddMediatR(typeof(CrearReciboCommandHandler).Assembly);

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowLocalhost");
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


