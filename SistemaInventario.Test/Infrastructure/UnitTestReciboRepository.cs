using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Infrastructure.Persistence;
using SistemaInventario.Infrastructure.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaInventario.Test.Infrastructure
{
    [TestClass]
    public class UnitTestReciboRepository
    {
        private AppDbContext _context;
        private ReciboRepository _repository;
        private DbContextOptions<AppDbContext> _options;

        [TestInitialize]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(_options);
            _repository = new ReciboRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task AgregarAsync_DeberiaGuardarReciboCorrectamente()
        {
            // Arrange
            var recibo = new Recibo { Id = Guid.NewGuid(), Fecha = DateTime.UtcNow };

            // Act
            await _repository.AgregarAsync(recibo);
            var resultado = await _context.Recibos.FindAsync(recibo.Id);

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(recibo.Id, resultado.Id);
        }

        [TestMethod]
        public async Task ObtenerPorIdAsync_DeberiaRetornarReciboConIncludes()
        {
            // Arrange
            var cliente = new Cliente { Id = Guid.NewGuid() };
            var producto = new Producto { Id = Guid.NewGuid() };
            var recibo = new Recibo
            {
                Id = Guid.NewGuid(),
                Cliente = cliente,
                Detalles = new List<DetalleRecibo>
                {
                    new DetalleRecibo { Producto = producto }
                }
            };

            await _context.Recibos.AddAsync(recibo);
            await _context.SaveChangesAsync();

            // Act
            var resultado = await _repository.ObtenerPorIdAsync(recibo.Id);

            // Assert
            Assert.IsNotNull(resultado);
            Assert.IsNotNull(resultado.Cliente);
            Assert.IsNotNull(resultado.Detalles);
            Assert.IsTrue(resultado.Detalles.Any());
            Assert.IsNotNull(resultado.Detalles.First().Producto);
        }

        [TestMethod]
        public async Task ObtenerVentasDiariasAsync_DeberiaFiltrarYCalcularTotalCorrectamente()
        {
            //arrange
            var fechaPrueba = new DateTime(2024, 1, 1);

            // Crear cliente y productos validos con Ids unicos
            var cliente = new Cliente { Id = Guid.NewGuid() };
            var productoA = new Producto { Id = Guid.NewGuid(), Nombre = "Producto A" };
            var productoB = new Producto { Id = Guid.NewGuid(), Nombre = "Producto B" };

            //añadir cliente y productos a la base de datos
            await _context.Clientes.AddAsync(cliente);
            await _context.Productos.AddRangeAsync(productoA, productoB);
            await _context.SaveChangesAsync();

            // Crear recibos con detalles validos
            var Recibo1 = new Recibo
            {
                Id = Guid.NewGuid(),
                ClienteId = cliente.Id,
                Fecha = fechaPrueba,
                Detalles = new List<DetalleRecibo>
                {
                    new DetalleRecibo { ProductoId = productoA.Id, Cantidad = 2, PrecioUnitario = 10 },
                    new DetalleRecibo { ProductoId = productoB.Id, Cantidad = 3, PrecioUnitario = 20 }
                }
            };

            var Recibo2 = new Recibo
            {
                Id = Guid.NewGuid(),
                ClienteId = cliente.Id,
                Fecha = fechaPrueba,
                Detalles = new List<DetalleRecibo>
                {
                    new DetalleRecibo { ProductoId = productoA.Id, Cantidad = 1, PrecioUnitario = 10 },
                    new DetalleRecibo { ProductoId = productoB.Id, Cantidad = 1, PrecioUnitario = 20 }
                }
            };
            
            // guardar recibos en la base de datos
            await _context.Recibos.AddRangeAsync(Recibo1, Recibo2);
            await _context.SaveChangesAsync();

            var (recibosDelDia, total) = await _repository.ObtenerVentasDiariasAsync(fechaPrueba);

            // Assert
            Assert.AreEqual(2, recibosDelDia.Count());
            Assert.AreEqual(110, total);


        }











        [TestMethod]
        public async Task ObtenerVentasPorFechaAsync_DeberiaFiltrarPorRangoFechas()
        {
            // Arrange
            var fechaInicio = new DateTime(2024, 1, 1);
            var fechaFin = new DateTime(2024, 1, 31);

            var recibos = new[]
            {
                new Recibo { Fecha = new DateTime(2024, 1, 15) },
                new Recibo { Fecha = new DateTime(2024, 2, 1) },
                new Recibo { Fecha = new DateTime(2023, 12, 31) }
            };

            await _context.Recibos.AddRangeAsync(recibos);
            await _context.SaveChangesAsync();

            // Act
            var resultados = await _repository.ObtenerVentasPorFechaAsync(fechaInicio, fechaFin);

            // Assert
            Assert.AreEqual(1, resultados.Count());
            Assert.AreEqual(new DateTime(2024, 1, 15), resultados.First().Fecha);
        }

        [TestMethod]
        public async Task ObtenerRecibosPorClientesAsync_DeberiaFiltrarPorClienteId()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var recibos = new[]
            {
                new Recibo { ClienteId = clienteId },
                new Recibo { ClienteId = Guid.NewGuid() }
            };

            await _context.Recibos.AddRangeAsync(recibos);
            await _context.SaveChangesAsync();

            // Act
            var resultados = await _repository.ObtenerRecibosPorClientesAsync(clienteId);

            // Assert
            Assert.AreEqual(1, resultados.Count());
            Assert.AreEqual(clienteId, resultados.First().ClienteId);
        }

        [TestMethod]
        public async Task ObtenerPorIdAsync_DeberiaRetornarNullSiNoExiste()
        {
            // Act
            var resultado = await _repository.ObtenerPorIdAsync(Guid.NewGuid());

            // Assert
            Assert.IsNull(resultado);
        }
    }
}
