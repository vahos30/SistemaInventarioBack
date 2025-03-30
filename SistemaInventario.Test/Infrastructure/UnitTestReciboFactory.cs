using Microsoft.VisualStudio.TestTools.UnitTesting;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Infrastructure.Repositories;
using System;
using System.Collections.Generic;

namespace SistemaInventario.Test.Infrastructure
{
    [TestClass]
    public class UnitTestReciboFactory
    {
        [TestMethod]
        public void CrearRecibo_WithValidParameters_ShouldCreateReciboWithCorrectProperties()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var detalles = new List<(Guid, int, decimal)>
            {
                (Guid.NewGuid(), 2, 100m),
                (Guid.NewGuid(), 3, 50m)
            };

            // Act
            var recibo = ReciboFactory.CrearRecibo(clienteId, detalles);

            // Assert
            Assert.AreEqual(clienteId, recibo.ClienteId);
            Assert.IsTrue(DateTime.UtcNow.Subtract(recibo.Fecha).TotalSeconds < 1);
            Assert.AreEqual(2, recibo.Detalles.Count);
        }

        [TestMethod]
        public void CrearRecibo_WithEmptyDetalles_ShouldCreateEmptyRecibo()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var detalles = new List<(Guid, int, decimal)>();

            // Act
            var recibo = ReciboFactory.CrearRecibo(clienteId, detalles);

            // Assert
            Assert.AreEqual(0, recibo.Detalles.Count);
            Assert.IsNotNull(recibo.Detalles);
        }

        [TestMethod]
        public void CrearRecibo_WithMultipleDetalles_ShouldCorrectlyMapDetails()
        {
            // Arrange
            var producto1Id = Guid.NewGuid();
            var producto2Id = Guid.NewGuid();
            var detalles = new List<(Guid, int, decimal)>
            {
                (producto1Id, 2, 100m),
                (producto2Id, 3, 50m)
            };

            // Act
            var recibo = ReciboFactory.CrearRecibo(Guid.NewGuid(), detalles);

            // Assert
            Assert.AreEqual(producto1Id, recibo.Detalles[0].ProductoId);
            Assert.AreEqual(2, recibo.Detalles[0].Cantidad);
            Assert.AreEqual(100m, recibo.Detalles[0].PrecioUnitario);

            Assert.AreEqual(producto2Id, recibo.Detalles[1].ProductoId);
            Assert.AreEqual(3, recibo.Detalles[1].Cantidad);
            Assert.AreEqual(50m, recibo.Detalles[1].PrecioUnitario);
        }

        [TestMethod]
        public void CrearRecibo_ShouldSetCurrentUtcDateTime()
        {
            // Arrange
            var preTestTime = DateTime.UtcNow;

            // Act
            var recibo = ReciboFactory.CrearRecibo(Guid.NewGuid(), new List<(Guid, int, decimal)>());

            // Assert
            var postTestTime = DateTime.UtcNow;
            Assert.IsTrue(recibo.Fecha >= preTestTime && recibo.Fecha <= postTestTime);
        }
    }
}
