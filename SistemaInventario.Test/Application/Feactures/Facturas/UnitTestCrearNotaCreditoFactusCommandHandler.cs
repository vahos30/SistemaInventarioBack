using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SistemaInventario.Application.Feactures.Facturas;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Application.Services;
using SistemaInventario.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;

namespace SistemaInventario.Test.Application.Feactures.Facturas
{
    [TestClass]
    public class UnitTestCrearNotaCreditoFactusCommandHandler
    {
        private Mock<IFacturaRepository> _mockFacturaRepo = null!;
        private Mock<FactusFacturaService> _mockFactusService = null!;
        private Mock<INotaCreditoRepository> _mockNotaCreditoRepo = null!;
        private Mock<IProductoRepository> _mockProductoRepo = null!;
        private CrearNotaCreditoFactusCommandHandler _handler = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockFacturaRepo = new Mock<IFacturaRepository>();
            _mockFactusService = new Mock<FactusFacturaService>(null, null, null); // Usa nulls si el constructor requiere dependencias, pero no se usan en el mock
            _mockNotaCreditoRepo = new Mock<INotaCreditoRepository>();
            _mockProductoRepo = new Mock<IProductoRepository>();

            // _handler = new CrearNotaCreditoFactusCommandHandler(
            //     _mockFacturaRepo.Object,
            //     _mockFactusService.Object,
            //     _mockNotaCreditoRepo.Object,
            //     _mockProductoRepo.Object
            // );
            var fakeFactusService = new FakeFactusFacturaService();

            _handler = new CrearNotaCreditoFactusCommandHandler(
                _mockFacturaRepo.Object,
                fakeFactusService,
                _mockNotaCreditoRepo.Object,
                _mockProductoRepo.Object
            );
        }

        [TestMethod]
        public async Task Handle_ValidCommand_ShouldCreateNotaCreditoAndReturnFactusJson()
        {
            // Arrange
            var facturaId = Guid.NewGuid();
            var productoId = Guid.NewGuid();
            var cliente = new Cliente { Id = Guid.NewGuid(), Nombre = "Juan" };
            var producto = new Producto
            {
                Id = productoId,
                Nombre = "Producto Test",
                Referencia = "REF-001",
                Descripcion = "Descripción test"
            };

            var factura = new Factura
            {
                Id = facturaId,
                FactusBillId = 123,
                Cliente = cliente,
                Detalles = new List<DetalleFactura>
                {
                    new DetalleFactura
                    {
                        ProductoId = productoId,
                        Cantidad = 2,
                        PrecioUnitario = 100,
                        Producto = producto // <--- Asigna el producto aquí
                    }
                }
            };

            _mockFacturaRepo.Setup(r => r.ObtenerPorIdAsync(facturaId)).ReturnsAsync(factura);


            _mockProductoRepo.Setup(r => r.ObtenerPorIdsync(productoId)).ReturnsAsync(producto);

            var command = new CrearNotaCreditoFactusCommand
            {
                FacturaId = facturaId,
                CorrectionConceptCode = 01,
                CustomizationId = 1,
                PaymentMethodCode = "10",
                Observation = "Motivo"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.Contains("\"number\":\"NC001\""));
            _mockNotaCreditoRepo.Verify(r => r.AgregarAsync(It.IsAny<NotaCredito>()), Times.Once);
            _mockFacturaRepo.Verify(r => r.ActualizarAsync(It.IsAny<Factura>()), Times.Once);
            _mockProductoRepo.Verify(r => r.ActualizarAsync(It.IsAny<Producto>()), Times.Once);
        }
    }

    // Clase fake para pruebas
    public class FakeFactusFacturaService : FactusFacturaService
    {
        public FakeFactusFacturaService() : base(null!, null!, null!) { }

        public override Task<string> CrearNotaCreditoAsync(object request)
        {
            // Usa CultureInfo.InvariantCulture para AM/PM en inglés
            var factusResponse = new FactusNotaCreditoResponse
            {
                data = new FactusNotaCreditoData
                {
                    credit_note = new FactusCreditNote
                    {
                        number = "NC001",
                        validated = DateTime.UtcNow.ToString("dd-MM-yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture),
                        observation = "Motivo",
                        total = "200"
                    }
                }
            };
            var factusJson = JsonSerializer.Serialize(factusResponse);
            return Task.FromResult(factusJson);
        }
    }
}