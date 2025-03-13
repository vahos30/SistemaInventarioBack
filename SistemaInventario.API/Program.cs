using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaInventario.Application.Mapping;
using SistemaInventario.Infrastructure.Persistence;
using AutoMapper;
using SistemaInventario.Domain.Interfaces;
using SistemaInventario.Infrastructure.Repositories;
using MediatR;
using SistemaInventario.Application.Feactures.Clientes;


var builder = WebApplication.CreateBuilder(args);

//  Registrar DbContext con la cadena de conexión
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//  Registrar Repositorios
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IReciboRepository, ReciboRepository>();

//  Registrar ProductoRepository con Proxy
builder.Services.AddScoped<IProductoRepository>(provider =>
{
    var repoReal = new ProductoRepository(provider.GetRequiredService<AppDbContext>());
    var logger = provider.GetRequiredService<ILogger<ProductoRepositoryProxy>>();
    return new ProductoRepositoryProxy(repoReal, logger);
});

//  Registrar MediatR
builder.Services.AddMediatR(typeof(CrearClienteCommandHandler).Assembly);

//  Registrar AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

//  Registrar controladores
builder.Services.AddControllers();

//  Configurar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//  Middleware de manejo de excepciones (opcional)
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//  Seguridad (si usas autenticación)
app.UseAuthentication();
app.UseAuthorization();

//  Configurar controladores
app.MapControllers();

app.Run();


