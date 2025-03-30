using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SistemaInventario.Application.DTOs;
using SistemaInventario.Application.Feactures.Recibos;
using SistemaInventario.Application.Mapping;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Infrastructure.Persistence;
using SistemaInventario.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaInventario.Test.Application
{
    [TestClass]
    public class UnitTestCrearReciboCommandHandler
    {
        private AppDbContext _context;
        private CrearReciboCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            // Configurar base de datos en memoria
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDB_Recibos")
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning)) // Suprime advertencia de transacciones
                .Options;

            _context = new AppDbContext(options);

            // Configurar AutoMapper
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            var mapper = config.CreateMapper();

            // Inicializar handler con repositorios reales
            _handler = new CrearReciboCommandHandler(
                new ReciboRepository(_context),
                new ProductoRepository(_context),
                _context,
                mapper
            );
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task Handle_ValidCommand_ShouldUpdateStock()
        {
            // Arrange: Crear y agregar producto de prueba
            var producto = new Producto
            {
                Id = Guid.NewGuid(),
                Nombre = "Laptop Gamer",
                Precio = 1500000,
                CantidadStock = 10,
                Activo = true
            };

            await _context.Productos.AddAsync(producto);
            await _context.SaveChangesAsync();

            var command = new CrearReciboCommand
            {
                ClienteId = Guid.NewGuid(),
                Detalles = new List<DetalleReciboDto>
                {
                    new DetalleReciboDto
                    {
                        ProductoId = producto.Id,
                        Cantidad = 3,
                        PrecioUnitario = producto.Precio
                    }
                }
            };

            // Act: Ejecutar el handler
            await _handler.Handle(command, CancellationToken.None);

            // Assert: Verificar actualización de stock
            var productoActualizado = await _context.Productos.FindAsync(producto.Id);
            Assert.AreEqual(7, productoActualizado.CantidadStock); // 10 - 3 = 7
        }
    }
}