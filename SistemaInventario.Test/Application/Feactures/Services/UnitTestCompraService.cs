using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SistemaInventario.Application.Services;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Domain.Interfaces;
using SistemaInventario.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaInventario.Test.Application.Feactures.Services
{
    [TestClass]
    public class UnitTestCompraService
    {
        private Mock<ICompraRepository> _compraRepoMock = null!;
        private Mock<IProductoRepository> _productoRepoMock = null!;
        private CompraService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _compraRepoMock = new Mock<ICompraRepository>();
            _productoRepoMock = new Mock<IProductoRepository>();
            _service = new CompraService(_compraRepoMock.Object, _productoRepoMock.Object);
        }

        [TestMethod]
        public async Task ObtenerTodasAsync_ReturnsCompras()
        {
            var compras = new List<Compra> { new Compra { Id = Guid.NewGuid() } };
            _compraRepoMock.Setup(r => r.ObtenerTodasAsync()).ReturnsAsync(compras);

            var result = await _service.ObtenerTodasAsync();

            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public async Task ObtenerPorIdAsync_ReturnsCompra()
        {
            var compra = new Compra { Id = Guid.NewGuid() };
            _compraRepoMock.Setup(r => r.ObtenerPorIdAsync(compra.Id)).ReturnsAsync(compra);

            var result = await _service.ObtenerPorIdAsync(compra.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(compra.Id, result!.Id);
        }

        [TestMethod]
        public async Task ActualizarAsync_CompraValida_CallsRepository()
        {
            var compra = new Compra { Id = Guid.NewGuid() };
            _compraRepoMock.Setup(r => r.ActualizarAsync(compra)).Returns(Task.CompletedTask);

            await _service.ActualizarAsync(compra);

            _compraRepoMock.Verify(r => r.ActualizarAsync(compra), Times.Once);
        }

        [TestMethod]
        public async Task EliminarAsync_CompraConDetalles_ActualizaStockYElimina()
        {
            var productoId = Guid.NewGuid();
            var producto = new Producto { Id = productoId, CantidadStock = 10, Activo = true };
            var compra = new Compra
            {
                Id = Guid.NewGuid(),
                Detalles = new List<DetalleCompra>
                {
                    new DetalleCompra { ProductoId = productoId, Cantidad = 2, PrecioUnitario = 50 }
                }
            };

            _compraRepoMock.Setup(r => r.ObtenerPorIdAsync(compra.Id)).ReturnsAsync(compra);
            _productoRepoMock.Setup(r => r.ObtenerPorIdsync(productoId)).ReturnsAsync(producto);
            _productoRepoMock.Setup(r => r.ActualizarAsync(It.IsAny<Producto>())).Returns(Task.CompletedTask);
            _compraRepoMock.Setup(r => r.EliminarAsync(compra.Id)).Returns(Task.CompletedTask);

            await _service.EliminarAsync(compra.Id);

            Assert.AreEqual(8, producto.CantidadStock);
            _productoRepoMock.Verify(r => r.ActualizarAsync(It.IsAny<Producto>()), Times.Once);
            _compraRepoMock.Verify(r => r.EliminarAsync(compra.Id), Times.Once);
        }

        [TestMethod]
        public async Task AnularAsync_CompraValida_CambiaEstado()
        {
            var compra = new Compra { Id = Guid.NewGuid(), Estado = "Activa" };
            _compraRepoMock.Setup(r => r.ObtenerPorIdAsync(compra.Id)).ReturnsAsync(compra);
            _compraRepoMock.Setup(r => r.ActualizarAsync(compra)).Returns(Task.CompletedTask);

            await _service.AnularAsync(compra.Id);

            Assert.AreEqual("Anulada", compra.Estado);
            _compraRepoMock.Verify(r => r.ActualizarAsync(compra), Times.Once);
        }

        [TestMethod]
        public async Task AnularParcialAsync_CompraValida_ActualizaDetalleYEstado()
        {
            var productoId = Guid.NewGuid();
            var producto = new Producto { Id = productoId, CantidadStock = 10, Activo = true };
            var detalleCompra = new DetalleCompra
            {
                ProductoId = productoId,
                Cantidad = 5,
                PrecioUnitario = 20,
                SubTotal = 100
            };
            var compra = new Compra
            {
                Id = Guid.NewGuid(),
                Detalles = new List<DetalleCompra> { detalleCompra },
                Estado = "Activa"
            };
            var dto = new CompraAnulacionParcialDto
            {
                CompraId = compra.Id,
                Detalles = new List<DetalleAnulacionDto>
                {
                    new DetalleAnulacionDto
                    {
                        ProductoId = productoId,
                        CantidadAAnular = 3,
                        MotivoDevolucion = "Defecto"
                    }
                }
            };

            _compraRepoMock.Setup(r => r.ObtenerPorIdAsync(compra.Id)).ReturnsAsync(compra);
            _productoRepoMock.Setup(r => r.ObtenerPorIdsync(productoId)).ReturnsAsync(producto);
            _productoRepoMock.Setup(r => r.ActualizarAsync(It.IsAny<Producto>())).Returns(Task.CompletedTask);
            _compraRepoMock.Setup(r => r.ActualizarAsync(compra)).Returns(Task.CompletedTask);

            await _service.AnularParcialAsync(dto);

            Assert.AreEqual(2, detalleCompra.Cantidad);
            Assert.AreEqual("Defecto", detalleCompra.MotivoDevolucion);
            Assert.AreEqual("Anulada Parcial", compra.Estado);
            _compraRepoMock.Verify(r => r.ActualizarAsync(compra), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task AnularAsync_CompraNoExiste_ThrowsException()
        {
            _compraRepoMock.Setup(r => r.ObtenerPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Compra?)null);
            await _service.AnularAsync(Guid.NewGuid());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task AnularParcialAsync_CompraNoExiste_ThrowsException()
        {
            _compraRepoMock.Setup(r => r.ObtenerPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Compra?)null);
            var dto = new CompraAnulacionParcialDto { CompraId = Guid.NewGuid(), Detalles = new List<DetalleAnulacionDto>() };
            await _service.AnularParcialAsync(dto);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task AnularParcialAsync_DetalleNoExiste_ThrowsException()
        {
            var compra = new Compra
            {
                Id = Guid.NewGuid(),
                Detalles = new List<DetalleCompra>()
            };
            var dto = new CompraAnulacionParcialDto
            {
                CompraId = compra.Id,
                Detalles = new List<DetalleAnulacionDto>
                {
                    new DetalleAnulacionDto { ProductoId = Guid.NewGuid(), CantidadAAnular = 1 }
                }
            };
            _compraRepoMock.Setup(r => r.ObtenerPorIdAsync(compra.Id)).ReturnsAsync(compra);

            await _service.AnularParcialAsync(dto);
        }
    }
}
