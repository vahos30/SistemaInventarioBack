using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SistemaInventario.Application.Feactures.Facturas;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

[TestClass]
public class UnitTestAnularFacturaCommandHandler
{
    private Mock<IFacturaRepository> _facturaRepoMock = null!;
    private Mock<INotaCreditoRepository> _notaCreditoRepoMock = null!;
    private Mock<IProductoRepository> _productoRepoMock = null!;
    private AnularFacturaCommandHandler _handler = null!;

    [TestInitialize]
    public void Setup()
    {
        _facturaRepoMock = new Mock<IFacturaRepository>();
        _notaCreditoRepoMock = new Mock<INotaCreditoRepository>();
        _productoRepoMock = new Mock<IProductoRepository>();

        _handler = new AnularFacturaCommandHandler(
            _facturaRepoMock.Object,
            _notaCreditoRepoMock.Object,
            _productoRepoMock.Object
        );
    }

    [TestMethod]
    public async Task Handle_FacturaExistenteNoAnulada_AnulaFacturaYGeneraNotaCredito()
    {
        // Arrange
        var facturaId = Guid.NewGuid();
        var productoId = Guid.NewGuid();
        var factura = new Factura
        {
            Id = facturaId,
            NumeroFactura = "F501",
            Anulada = false,
            Total = 1000,
            Detalles = new List<DetalleFactura>
            {
                new DetalleFactura
                {
                    ProductoId = productoId,
                    Cantidad = 2,
                    PrecioUnitario = 500
                }
            }
        };

        var producto = new Producto
        {
            Id = productoId,
            CantidadStock = 0,
            Activo = false
        };

        _facturaRepoMock.Setup(r => r.ObtenerPorIdAsync(facturaId)).ReturnsAsync(factura);
        _productoRepoMock.Setup(r => r.ObtenerPorIdsync(productoId)).ReturnsAsync(producto);
        _productoRepoMock.Setup(r => r.ActualizarAsync(It.IsAny<Producto>())).Returns(Task.CompletedTask);
        _notaCreditoRepoMock.Setup(r => r.AgregarAsync(It.IsAny<NotaCredito>())).Returns(Task.CompletedTask);
        _facturaRepoMock.Setup(r => r.ActualizarAsync(It.IsAny<Factura>())).Returns(Task.CompletedTask);

        var command = new AnularFacturaCommand
        {
            FacturaId = facturaId,
            Motivo = "Error de facturación"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.AreEqual(MediatR.Unit.Value, result);
        Assert.IsTrue(factura.Anulada);
        Assert.AreEqual("Error de facturación", factura.MotivoAnulacion);
        Assert.IsTrue(producto.Activo);
        Assert.AreEqual(2, producto.CantidadStock);

        _productoRepoMock.Verify(r => r.ActualizarAsync(It.IsAny<Producto>()), Times.Once);
        _notaCreditoRepoMock.Verify(r => r.AgregarAsync(It.IsAny<NotaCredito>()), Times.Once);
        _facturaRepoMock.Verify(r => r.ActualizarAsync(It.IsAny<Factura>()), Times.Once);
    }
}
