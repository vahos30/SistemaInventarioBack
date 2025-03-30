using Microsoft.Extensions.Logging;
using Moq;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Domain.Interfaces;
using SistemaInventario.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SistemaInventario.Test.Infrastructure
{
    [TestClass]
    public class UnitTestProductoRepositoryProxy
    {
        private readonly Mock<IProductoRepository> _mockRepo = new();
        private readonly Mock<ILogger<ProductoRepositoryProxy>> _mockLogger = new();
        private ProductoRepositoryProxy _proxy;

        [TestInitialize]
        public void Setup()
        {
            _proxy = new ProductoRepositoryProxy(_mockRepo.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task AgregarAsync_DeberiaLoggearYDelegar()
        {
            // Arrange
            var producto = new Producto { Nombre = "Teclado" };

            // Act
            await _proxy.AgregarAsync(producto);

            // Assert
            _mockRepo.Verify(r => r.AgregarAsync(producto), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Information, "Agregando producto:", Times.Once());
            _mockLogger.VerifyLog(LogLevel.Information, "Producto agregado correctamente.", Times.Once());
        }

        [TestMethod]
        public async Task EliminarAsync_DeberiaLoggearYDelegar()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            await _proxy.EliminarAsync(id);

            // Assert
            _mockRepo.Verify(r => r.EliminarAsync(id), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Information, "Eliminando producto con ID:", Times.Once());
        }

        [TestMethod]
        public async Task ActualizarAsync_DeberiaLoggearYDelegar()
        {
            // Arrange
            var producto = new Producto { Nombre = "Mouse" };

            // Act
            await _proxy.ActualizarAsync(producto);

            // Assert
            _mockRepo.Verify(r => r.ActualizarAsync(producto), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Information, "Actualizando producto:", Times.Once());
        }

        [TestMethod]
        public async Task ObtenerTodosAsync_DeberiaLoggearYDelegar()
        {
            // Arrange
            var productos = new List<Producto> { new(), new() };
            _mockRepo.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(productos);

            // Act
            var resultado = await _proxy.ObtenerTodosAsync();

            // Assert
            _mockRepo.Verify(r => r.ObtenerTodosAsync(), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Information, "Obteniendo todos los productos.", Times.Once());
            Assert.AreEqual(2, resultado.Count());
        }

        [TestMethod]
        public async Task ObtenerPorIdsync_DeberiaLoggearYDelegar()
        {
            // Arrange
            var id = Guid.NewGuid();
            var producto = new Producto { Id = id };
            _mockRepo.Setup(r => r.ObtenerPorIdsync(id)).ReturnsAsync(producto);

            // Act
            var resultado = await _proxy.ObtenerPorIdsync(id);

            // Assert
            _mockRepo.Verify(r => r.ObtenerPorIdsync(id), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Information, "Obteniendo producto con ID:", Times.Once());
            Assert.AreEqual(id, resultado.Id);
        }

        [TestMethod]
        public async Task ObtenerProductosActivosAsync_DeberiaLoggearCantidad()
        {
            // Arrange
            var productos = new List<Producto> { new(), new() };
            _mockRepo.Setup(r => r.ObtenerProductosActivosAsync()).ReturnsAsync(productos);

            // Act
            await _proxy.ObtenerProductosActivosAsync();

            // Assert
            _mockLogger.VerifyLog(LogLevel.Information, "Se obtuvieron 2 productos activos", Times.Once());
        }

        [TestMethod]
        public async Task ObtenerProductosActivosAsync_SinResultados_DeberiaLoggearCero()
        {
            // Arrange
            _mockRepo.Setup(r => r.ObtenerProductosActivosAsync()).ReturnsAsync(new List<Producto>());

            // Act
            await _proxy.ObtenerProductosActivosAsync();

            // Assert
            _mockLogger.VerifyLog(LogLevel.Information, "Se obtuvieron 0 productos activos", Times.Once());
        }
    }

    public static class LoggerExtensions
    {
        public static void VerifyLog<T>(
            this Mock<ILogger<T>> loggerMock,
            LogLevel level,
            string message,
            Times times
        )
        {
            loggerMock.Verify(
                x => x.Log(
                    level,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(message)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                times
            );
        }
    }
}
