using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SistemaInventario.Application.Feactures.Facturas;
using SistemaInventario.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaInventario.Test.Application.Feactures.Facturas
{
    [TestClass]
    public class UnitTestEliminarFacturaCommandHandler
    {
        private Mock<IFacturaRepository> _mockFacturaRepo = null!;
        private EliminarFacturaCommandHandler _handler = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockFacturaRepo = new Mock<IFacturaRepository>();
            _handler = new EliminarFacturaCommandHandler(_mockFacturaRepo.Object);
        }

        [TestMethod]
        public async Task Handle_ValidCommand_ShouldCallEliminarAsync()
        {
            // Arrange
            var facturaId = Guid.NewGuid();
            var command = new EliminarFacturaCommand(facturaId);

            _mockFacturaRepo
                .Setup(r => r.EliminarAsync(facturaId))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockFacturaRepo.Verify(r => r.EliminarAsync(facturaId), Times.Once);
            Assert.AreEqual(MediatR.Unit.Value, result);
        }
    }
}
