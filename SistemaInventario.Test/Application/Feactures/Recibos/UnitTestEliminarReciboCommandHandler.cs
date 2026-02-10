using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SistemaInventario.Application.Feactures.Recibos;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

[TestClass]
public class UnitTestEliminarReciboCommandHandler
{
    private Mock<IReciboRepository> _reciboRepoMock = null!;
    private Mock<IProductoRepository> _productoRepoMock = null!;
    private EliminarReciboCommandHandler _handler = null!;

    [TestInitialize]
    public void Setup()
    {
        _reciboRepoMock = new Mock<IReciboRepository>();
        _productoRepoMock = new Mock<IProductoRepository>();

        _handler = new EliminarReciboCommandHandler(
            _reciboRepoMock.Object,
            _productoRepoMock.Object
        );
    }

    [TestMethod]
    public async Task Handle_ReciboExistente_DevuelveStockYEliminaRecibo()
    {
        // Arrange
        var reciboId = Guid.NewGuid();
        var productoId = Guid.NewGuid();
        var recibo = new Recibo
        {
            Id = reciboId,
            Detalles = new List<DetalleRecibo>
            {
                new DetalleRecibo
                {
                    ProductoId = productoId,
                    Cantidad = 3
                }
            }
        };

        var producto = new Producto
        {
            Id = productoId,
            CantidadStock = 0,
            Activo = false
        };

        _reciboRepoMock.Setup(r => r.ObtenerPorIdAsync(reciboId)).ReturnsAsync(recibo);
        _productoRepoMock.Setup(r => r.ObtenerPorIdsync(productoId)).ReturnsAsync(producto);
        _productoRepoMock.Setup(r => r.ActualizarAsync(It.IsAny<Producto>())).Returns(Task.CompletedTask);
        _reciboRepoMock.Setup(r => r.EliminarAsync(reciboId)).Returns(Task.CompletedTask);

        var command = new EliminarReciboCommand(reciboId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.AreEqual(MediatR.Unit.Value, result);
        Assert.AreEqual(3, producto.CantidadStock);
        Assert.IsTrue(producto.Activo);

        _productoRepoMock.Verify(r => r.ActualizarAsync(It.IsAny<Producto>()), Times.Once);
        _reciboRepoMock.Verify(r => r.EliminarAsync(reciboId), Times.Once);
    }
}
