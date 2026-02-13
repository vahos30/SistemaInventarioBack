namespace SistemaInventario.Test.Application.Feactures.Facturas
{
    using Microsoft.EntityFrameworkCore.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using SistemaInventario.Application.Feactures.Facturas;
    using SistemaInventario.Domain.Entities;
    using SistemaInventario.Domain.Interfaces;
    using SistemaInventario.Application.DTOs;
    using AutoMapper;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using SistemaInventario.Infrastructure.Persistence;
    using Microsoft.EntityFrameworkCore.Storage;
    using Microsoft.EntityFrameworkCore.Infrastructure;

    [TestClass]
    public class UnitTestCrearFacturaCommandHandler
    {
        private Mock<IFacturaRepository> _facturaRepoMock = null!;
        private Mock<IProductoRepository> _productoRepoMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private CrearFacturaCommandHandler _handler = null!;

        private static AppDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            return new AppDbContext(options);
        }

        [TestInitialize]
        public void Setup()
        {
            _facturaRepoMock = new Mock<IFacturaRepository>();
            _productoRepoMock = new Mock<IProductoRepository>();
            _mapperMock = new Mock<IMapper>();

            var dbContext = GetDbContext("TestDb");

            _handler = new CrearFacturaCommandHandler(
                _facturaRepoMock.Object,
                _productoRepoMock.Object,
                dbContext,
                _mapperMock.Object
            );
        }

        [TestMethod]
        public async Task Handle_ValidCommand_ShouldCreateFactura()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var productoId = Guid.NewGuid();
            var detallesDto = new List<DetalleFacturaDto>
            {
                new DetalleFacturaDto
                {
                    ProductoId = productoId,
                    Cantidad = 1,
                    PrecioUnitario = 1000
                }
            };

            var command = new CrearFacturaCommand
            {
                ClienteId = clienteId,
                Fecha = DateTime.UtcNow,
                FormaPago = "1",
                Detalles = detallesDto
            };

            var producto = new Producto
            {
                Id = productoId,
                Nombre = "Producto Test",
                CantidadStock = 10
            };

            var detalles = new List<DetalleFactura>
            {
                new DetalleFactura
                {
                    ProductoId = productoId,
                    Cantidad = 1,
                    PrecioUnitario = 1000,
                    Producto = producto
                }
            };

            var factura = new Factura
            {
                Id = Guid.NewGuid(),
                NumeroFactura = "F501",
                ClienteId = clienteId,
                Fecha = DateTime.UtcNow,
                FormaPago = "1",
                Detalles = detalles
            };

            var facturaDto = new FacturaDto
            {
                Id = factura.Id,
                NumeroFactura = factura.NumeroFactura,
                ClienteId = clienteId,
                Fecha = factura.Fecha,
                FormaPago = factura.FormaPago,
                Detalles = new List<DetalleFacturaDto>
                {
                    new DetalleFacturaDto
                    {
                        ProductoId = productoId,
                        Cantidad = 1,
                        PrecioUnitario = 1000
                    }
                }
            };

            _mapperMock.Setup(m => m.Map<List<DetalleFactura>>(detallesDto)).Returns(detalles);
            _facturaRepoMock.Setup(r => r.AgregarAsync(It.IsAny<Factura>())).Returns(Task.CompletedTask);
            _productoRepoMock.Setup(r => r.ObtenerPorIdsync(productoId)).ReturnsAsync(producto);
            _productoRepoMock.Setup(r => r.ActualizarAsync(It.IsAny<Producto>())).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<FacturaDto>(It.IsAny<Factura>())).Returns(facturaDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("F501", result.NumeroFactura);
            Assert.AreEqual(clienteId, result.ClienteId);
            Assert.AreEqual(1, result.Detalles.Count);
            _facturaRepoMock.Verify(r => r.AgregarAsync(It.IsAny<Factura>()), Times.Once);
            _productoRepoMock.Verify(r => r.ActualizarAsync(It.IsAny<Producto>()), Times.Once);
        }
    }
}
