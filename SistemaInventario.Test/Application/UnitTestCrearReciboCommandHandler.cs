using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SistemaInventario.Application.DTOs;
using SistemaInventario.Application.Feactures.Recibos;
using SistemaInventario.Application.Mapping;
using SistemaInventario.Domain.Entities;
using SistemaInventario.Domain.Interfaces;

namespace SistemaInventario.Test.Application
{
    [TestClass]
    public class UnitTestCrearReciboCommandHandler
    {
        private Mock<IReciboRepository> _reciboRepositoryMock;
        private IMapper _mapper;
        private CrearReciboCommandHandler _handler;

        [TestInitialize]
        public void Setup()
        {
            //configuramos el mock para IReciboRepository
            _reciboRepositoryMock = new Mock<IReciboRepository>();
            //configuramos AutoMapper usando el MappingProfile de la aplicacion
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            _mapper = config.CreateMapper();
            //creamos la instancia del handler inyectandq el mock y el mapper
            _handler = new CrearReciboCommandHandler(_reciboRepositoryMock.Object, _mapper);
        }

        [TestMethod]
        public async Task Handle_ValidCommand_ShouldCreateRecibo()
        {
            //Arrange
            var command = new CrearReciboCommand
            {
                ClienteId = Guid.NewGuid(),
                Detalles = new List<DetalleReciboDto>
                {
                    new DetalleReciboDto { ProductoId = Guid.NewGuid(),Cantidad = 1,PrecioUnitario = 100 },
                    new DetalleReciboDto { ProductoId = Guid.NewGuid(),Cantidad = 2,PrecioUnitario = 200 }

                }
            };
            //configuramos el mock para que el metodo AgregarAsync se complete sin errores
            _reciboRepositoryMock.
                Setup(repo => repo.AgregarAsync(It.IsAny<Recibo>())).
                Returns(Task.CompletedTask);
            //Act
            var result = await _handler.Handle(command, CancellationToken.None);
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(command.ClienteId, result.ClienteId);
            Assert.AreEqual(command.Detalles.Count, result.Detalles.Count);

            _reciboRepositoryMock.Verify(repo => repo.AgregarAsync(It.IsAny<Recibo>()), Times.Once);
        }
    }
}
